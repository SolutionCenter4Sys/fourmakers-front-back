import { inject, injectable } from 'tsyringe'

import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { DepartamentoPayload, PosicaoPayload, AlocacaoPayload } from '@domain/entities/Organograma'
import type { ModalState } from '@shared/types/mapaRelacionamentoTypes'
import { CriarPosicaoUseCase } from './CriarPosicaoUseCase'
import { AtualizarPosicaoUseCase } from './AtualizarPosicaoUseCase'
import { CriarDepartamentoUseCase } from './CriarDepartamentoUseCase'
import { CriarAlocacaoUseCase } from './CriarAlocacaoUseCase'
import { DeletarAlocacaoUseCase } from './DeletarAlocacaoUseCase'
import { logger } from '@shared/utils/logger'

export interface SalvarNoMapaRelacionamentoParams {
  token: string
  payload: NoMapaRelacionamento
  modalState: ModalState | undefined
  orgId: number
  codigoCliente: string
  estruturaMapa: NoMapaRelacionamento | null
  isGestor?: boolean
}

/** Retorno do use case: posicaoId do nó salvo (criado ou editado) para o hook atualizar o card na árvore */
export interface SalvarNoMapaRelacionamentoResult {
  posicaoId: string
}

@injectable()
export class SalvarNoMapaRelacionamentoUseCase {
  private readonly EMPTY_GUID = '00000000-0000-0000-0000-000000000000'

  constructor(
    @inject(CriarPosicaoUseCase) private readonly criarPosicaoUseCase: CriarPosicaoUseCase,
    @inject(AtualizarPosicaoUseCase) private readonly atualizarPosicaoUseCase: AtualizarPosicaoUseCase,
    @inject(CriarDepartamentoUseCase) private readonly criarDepartamentoUseCase: CriarDepartamentoUseCase,
    @inject(CriarAlocacaoUseCase) private readonly criarAlocacaoUseCase: CriarAlocacaoUseCase,
    @inject(DeletarAlocacaoUseCase) private readonly deletarAlocacaoUseCase: DeletarAlocacaoUseCase,
  ) {}

  async execute(params: SalvarNoMapaRelacionamentoParams): Promise<SalvarNoMapaRelacionamentoResult> {
    const { token, payload, modalState, orgId, codigoCliente, estruturaMapa } = params
    // isGestor está disponível mas não é usado atualmente - pode ser usado no futuro para lógica específica

    // ============================================================
    // DEFINIR FLAGS DE CONTROLE
    // ============================================================
    const flags = this.determinarFlagsControle(payload, modalState)
    logger.debug('🔍 FLAGS:', flags)
    logger.debug('🔍 PAYLOAD:', {
      departmentName: payload.departmentName,
      departamentoBackendId: payload.departamentoBackendId,
      departmentId: payload.departmentId,
      colaboradorOriginal: modalState?.alocacao.originalColaboradorId,
      colaboradorNovo: payload.employeeId,
      modalStateDepartamento: modalState?.departamento,
    })

    // ============================================================
    // EXTRAIR IDs DOS DROPDOWNS
    // ============================================================
    const departamentoId = payload.departamentoBackendId || payload.departmentId || null
    const perfilCorporativoId = payload.perfilCorporativoBackendId || payload.profileId || null

    // ============================================================
    // DETERMINAR PAI DA POSIÇÃO (organogramaPosicaoIdSuperior)
    // ============================================================
    const organogramaPosicaoIdSuperior = this.determinarPaiPosicao(payload, estruturaMapa)

    // ============================================================
    // EXECUTAR FLUXO: CRIAR OU EDITAR
    // ============================================================
    if (flags.modalCriacao) {
      const posicaoId = await this.processarCriacaoNo({
        token,
        payload,
        modalState,
        orgId,
        codigoCliente,
        departamentoId,
        perfilCorporativoId,
        organogramaPosicaoIdSuperior,
        flags,
      })
      return { posicaoId }
    }
    await this.processarEdicaoNo({
      token,
      payload,
      modalState,
      orgId,
      codigoCliente,
      departamentoId,
      perfilCorporativoId,
      organogramaPosicaoIdSuperior,
      flags,
    })
    return { posicaoId: payload.posicaoId! }
  }

  private determinarFlagsControle(
    payload: NoMapaRelacionamento,
    modalState: ModalState | undefined,
  ): {
    criouPerfil: boolean
    criouDepartamento: boolean
    alterouColaborador: boolean
    modalCriacao: boolean
  } {
    // criouPerfil: false por enquanto (não implementado)
    const criouPerfil = false

    // criouDepartamento: true se campo "Nome do Novo Departamento" não é nulo
    const criouDepartamentoCondicao1 = !!(payload.departmentName && !payload.departamentoBackendId)
    const criouDepartamentoCondicao2 = !!(modalState?.departamento?.foiCriado)
    const criouDepartamento = criouDepartamentoCondicao1 || criouDepartamentoCondicao2

    // alterouColaborador: true se selecionou colaborador diferente (código diferente e não é vaga)
    const colaboradorOriginal = modalState?.alocacao.originalColaboradorId
    const colaboradorNovo = payload.employeeId
    const alterouColaborador = !!(
      colaboradorNovo &&
      !this.isVaga(colaboradorNovo) &&
      colaboradorNovo !== colaboradorOriginal
    )

    // modalCriacao: true para criação de novos nós, false para edição
    const modalCriacao = !payload.posicaoId

    return {
      criouPerfil,
      criouDepartamento,
      alterouColaborador,
      modalCriacao,
    }
  }

  private isVaga(employeeId: string | null | undefined): boolean {
    if (!employeeId) return true
    if (typeof employeeId !== 'string') return true
    return employeeId === 'vacant' || employeeId.trim() === '' || employeeId === this.EMPTY_GUID
  }

  private determinarPaiPosicao(
    payload: NoMapaRelacionamento,
    estruturaMapa: NoMapaRelacionamento | null,
  ): string | null {
    const nodeWithPai = payload as NoMapaRelacionamento & { _paiPosicaoId?: string; _paiId?: string }

    if (nodeWithPai._paiPosicaoId && nodeWithPai._paiPosicaoId !== 'undefined' && nodeWithPai._paiPosicaoId !== 'null') {
      logger.debug('📌 Usando posicaoIdSuperior do pai:', nodeWithPai._paiPosicaoId)
      return nodeWithPai._paiPosicaoId
    }

    if (estruturaMapa) {
      const encontrarPai = (no: NoMapaRelacionamento, targetId: string): NoMapaRelacionamento | null => {
        for (const child of no.children) {
          if (child.id === targetId) return no
          const found = encontrarPai(child, targetId)
          if (found) return found
        }
        return null
      }
      const pai = encontrarPai(estruturaMapa, payload.id)
      return pai?.posicaoId || null
    }

    return null
  }

  private async processarCriacaoNo(params: {
    token: string
    payload: NoMapaRelacionamento
    modalState: ModalState | undefined
    orgId: number
    codigoCliente: string
    departamentoId: string | null
    perfilCorporativoId: string | null
    organogramaPosicaoIdSuperior: string | null
    flags: { criouDepartamento: boolean; alterouColaborador: boolean; modalCriacao: boolean }
  }): Promise<string> {
    const { token, payload, modalState, orgId, codigoCliente, departamentoId, perfilCorporativoId, organogramaPosicaoIdSuperior, flags } = params

    logger.debug('🆕 FLUXO: Criar Novo Nó')

    // 1. Criar Nova Posição
    const posicaoPayload: PosicaoPayload = {
      orgId,
      codigoCliente,
      organogramaDepartamentoId: flags.criouDepartamento ? null : (departamentoId || null),
      perfilCorporativoId: perfilCorporativoId || null,
      organogramaPosicaoIdSuperior,
      ativo: true,
      cLevel: payload.isCLevel || false,
      profissionalExterno: payload.isExternal || false,
    }

    const posicaoResponse = await this.criarPosicaoUseCase.execute(token, posicaoPayload)
    const posicaoId = posicaoResponse.id

    // 2. Criar Departamento (se criouDepartamento === true)
    if (flags.criouDepartamento) {
      await this.criarDepartamentoEVincularPosicao({
        token,
        payload,
        modalState,
        orgId,
        codigoCliente,
        posicaoId,
        posicaoPayload,
      })
    }

    // 3. Criar Alocação (se colaborador não é "Vago")
    const colaboradorNovo = payload.employeeId
    if (colaboradorNovo && !this.isVaga(colaboradorNovo)) {
      const alocacaoPayload: AlocacaoPayload = {
        orgId,
        codigoCliente,
        organogramaPosicaoId: posicaoId,
        codigoInternoColaborador: colaboradorNovo,
        dataInicio: new Date().toISOString(),
        dataFim: null,
        ativo: true,
      }

      await this.criarAlocacaoUseCase.execute(token, alocacaoPayload)
    }
    return posicaoId
  }

  private async processarEdicaoNo(params: {
    token: string
    payload: NoMapaRelacionamento
    modalState: ModalState | undefined
    orgId: number
    codigoCliente: string
    departamentoId: string | null
    perfilCorporativoId: string | null
    organogramaPosicaoIdSuperior: string | null
    flags: { criouDepartamento: boolean; alterouColaborador: boolean; modalCriacao: boolean }
  }): Promise<void> {
    const { token, payload, modalState, orgId, codigoCliente, departamentoId, perfilCorporativoId, organogramaPosicaoIdSuperior, flags } = params

    logger.debug('✏️ FLUXO: Editar Nó Existente')

    if (!payload.posicaoId) {
      throw new Error('Posição ID é obrigatório para edição')
    }

    const posicaoId = payload.posicaoId

    // Preparar payload base da posição
    const posicaoPayload: PosicaoPayload = {
      orgId,
      codigoCliente,
      organogramaDepartamentoId: payload.departamentoBackendId || null,
      perfilCorporativoId: perfilCorporativoId || null,
      organogramaPosicaoIdSuperior,
      ativo: true,
      cLevel: payload.isCLevel || false,
      profissionalExterno: payload.isExternal || false,
    }

    let novoDepartamentoId = departamentoId

    // 1. Criar Departamento (se criouDepartamento === true)
    if (flags.criouDepartamento) {
      novoDepartamentoId = await this.criarDepartamentoEVincularPosicao({
        token,
        payload,
        modalState,
        orgId,
        codigoCliente,
        posicaoId,
        posicaoPayload,
      })
    }

    // 2. Atualizar Posição
    if (!flags.criouDepartamento) {
      posicaoPayload.organogramaDepartamentoId = payload.departamentoBackendId || null
    } else {
      posicaoPayload.organogramaDepartamentoId = novoDepartamentoId
    }

    logger.debug('📝 [ATUALIZAR POSIÇÃO] Payload:', {
      posicaoId,
      organogramaDepartamentoId: posicaoPayload.organogramaDepartamentoId,
      criouDepartamento: flags.criouDepartamento,
      departamentoId: novoDepartamentoId,
    })

    await this.atualizarPosicaoUseCase.execute(token, {
      ...posicaoPayload,
      id: posicaoId,
    })

    // 3. Criar Nova Alocação (se alterouColaborador === true e não é vaga)
    const colaboradorNovo = payload.employeeId
    if (flags.alterouColaborador && colaboradorNovo && !this.isVaga(colaboradorNovo)) {
      const alocacaoPayload: AlocacaoPayload = {
        orgId,
        codigoCliente,
        organogramaPosicaoId: posicaoId,
        codigoInternoColaborador: colaboradorNovo,
        dataInicio: new Date().toISOString(),
        dataFim: null,
        ativo: true,
      }

      await this.criarAlocacaoUseCase.execute(token, alocacaoPayload)
    }

    // 4. Caso especial: Colaborador mudou para "Vago" - deletar alocação
    if (modalState?.alocacao.originalId && this.isVaga(colaboradorNovo)) {
      await this.deletarAlocacaoUseCase.execute(token, modalState.alocacao.originalId)
    }
  }

  private async criarDepartamentoEVincularPosicao(params: {
    token: string
    payload: NoMapaRelacionamento
    modalState: ModalState | undefined
    orgId: number
    codigoCliente: string
    posicaoId: string
    posicaoPayload: PosicaoPayload
  }): Promise<string> {
    const { token, payload, modalState, orgId, codigoCliente, posicaoId, posicaoPayload } = params

    const nomeDepartamento = modalState?.departamento?.novoNome || payload.departmentName || ''
    logger.debug('🏢 [CRIAR DEPT] Criando departamento:', { nomeDepartamento, posicaoId })

    const departamentoPayload: DepartamentoPayload = {
      orgId,
      nome: nomeDepartamento,
      codigoCliente,
      organogramaPosicaoIdLider: posicaoId,
      ativo: true,
    }

    const deptResponse = await this.criarDepartamentoUseCase.execute(token, departamentoPayload)
    const departamentoId = deptResponse.id
    logger.debug('✅ [CRIAR DEPT] Departamento criado:', { id: departamentoId, nome: nomeDepartamento })

    // Vincular departamento à posição
    logger.debug('📝 [ATUALIZAR POSIÇÃO] Vinculando departamento à posição:', { posicaoId, departamentoId })
    await this.atualizarPosicaoUseCase.execute(token, {
      ...posicaoPayload,
      id: posicaoId,
      organogramaDepartamentoId: departamentoId,
    })
    logger.debug('✅ [ATUALIZAR POSIÇÃO] Posição vinculada ao departamento')

    return departamentoId
  }
}
