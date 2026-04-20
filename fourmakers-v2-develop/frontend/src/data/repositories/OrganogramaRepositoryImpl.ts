import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type {
  DepartamentoPayload,
  DepartamentoResponse,
  PerfilCorporativoPayload,
  PerfilCorporativoResponse,
  PosicaoPayload,
  PosicaoResponse,
  AlocacaoPayload,
  AlocacaoResponse,
  PosicaoCompletaResponse,
} from '@domain/entities/Organograma'
import { OrganogramaApi } from '@data/api/OrganogramaApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class OrganogramaRepositoryImpl implements OrganogramaRepository {
  constructor(
    @inject(DiTokens.organogramaApi)
    private readonly organogramaApi: OrganogramaApi,
  ) {}

  // Listar Organograma Completo
  async listarOrganogramaCompleto(token: string, codigoCliente: string, orgId: number): Promise<PosicaoCompletaResponse> {
    return this.organogramaApi.listarOrganogramaCompleto(token, codigoCliente, orgId)
  }

  // Departamento
  async criarDepartamento(token: string, payload: DepartamentoPayload): Promise<DepartamentoResponse> {
    return this.organogramaApi.criarDepartamento(token, payload)
  }

  async atualizarDepartamento(
    token: string,
    payload: DepartamentoPayload & { id: string },
  ): Promise<DepartamentoResponse> {
    return this.organogramaApi.atualizarDepartamento(token, payload)
  }

  async deletarDepartamento(token: string, departamentoId: string): Promise<void> {
    return this.organogramaApi.deletarDepartamento(token, departamentoId)
  }

  // Perfil Corporativo
  async criarPerfilCorporativo(
    token: string,
    payload: PerfilCorporativoPayload,
  ): Promise<PerfilCorporativoResponse> {
    return this.organogramaApi.criarPerfilCorporativo(token, payload)
  }

  async atualizarPerfilCorporativo(
    token: string,
    payload: PerfilCorporativoPayload & { id: string },
  ): Promise<PerfilCorporativoResponse> {
    return this.organogramaApi.atualizarPerfilCorporativo(token, payload)
  }

  async deletarPerfilCorporativo(token: string, perfilCorpId: string): Promise<void> {
    return this.organogramaApi.deletarPerfilCorporativo(token, perfilCorpId)
  }

  // Posição
  async criarPosicao(token: string, payload: PosicaoPayload): Promise<PosicaoResponse> {
    return this.organogramaApi.criarPosicao(token, payload)
  }

  async atualizarPosicao(token: string, payload: PosicaoPayload & { id: string }): Promise<PosicaoResponse> {
    return this.organogramaApi.atualizarPosicao(token, payload)
  }

  async deletarPosicao(token: string, posicaoId: string): Promise<void> {
    return this.organogramaApi.deletarPosicao(token, posicaoId)
  }

  // Alocação
  async criarAlocacao(token: string, payload: AlocacaoPayload): Promise<AlocacaoResponse> {
    return this.organogramaApi.criarAlocacao(token, payload)
  }

  async atualizarAlocacao(
    token: string,
    payload: AlocacaoPayload & { id: string },
  ): Promise<AlocacaoResponse> {
    return this.organogramaApi.atualizarAlocacao(token, payload)
  }

  async deletarAlocacao(token: string, alocacaoId: string): Promise<void> {
    return this.organogramaApi.deletarAlocacao(token, alocacaoId)
  }

  // Buscar Perfis Corporativos por OrgId
  async buscarPerfisPorOrg(
    token: string,
    orgId: number,
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[]; retorno: PerfilCorporativoResponse[] }> {
    return this.organogramaApi.buscarPerfisPorOrg(token, orgId)
  }

  // Buscar Perfil Corporativo por ID
  // O endpoint retorna o perfil completo com todos os campos
  async buscarPerfilCorporativoPorId(
    token: string,
    perfilCorpId: string,
  ): Promise<{
    sucesso: boolean
    mensagem?: string
    erros?: string[]
    retorno: PerfilCorporativoResponse & {
      codigoInternoColaboradorCriacao?: string
      codigoInternoColaboradorAlteracao?: string | null
      dataCriacao?: string
      dataAlteracao?: string
    }
  }> {
    return this.organogramaApi.buscarPerfilCorporativoPorId(token, perfilCorpId)
  }

  // Listar Colaboradores Externos por Cliente (paginação e filtro por nome)
  async listarColaboradoresExternosPorCliente(
    token: string,
    codigoCliente: string,
    cursor: number,
    limit: number,
    nome?: string,
  ): Promise<Array<{ CodigoInternoColaborador: string; NomeColaborador: string }>> {
    return this.organogramaApi.listarColaboradoresExternosPorCliente(
      token,
      codigoCliente,
      cursor,
      limit,
      nome,
    )
  }

  // Inserir Gestor Externo
  async inserirGestorExterno(
    token: string,
    payload: {
      codGestorExterno?: string
      nome: string
      email: string
      telefone: string
      codigoCliente?: string
      perfilLinkedin?: string
      areasDeAtuacao?: Array<{
        areaDeAtuacao: { descricao: string }
        permanencia: { id: string }
      }>
      preferenciasPessoais?: string
    },
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.organogramaApi.inserirGestorExterno(token, payload)
  }
}
