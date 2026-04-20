import { inject, injectable } from 'tsyringe'

import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { MinhaJornadaProfile } from '@domain/entities/MinhaJornadaProfile'
import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'
import type { MinhaJornadaInteresse } from '@domain/entities/MinhaJornadaInteresse'
import type { MinhaJornadaSugestao } from '@domain/entities/MinhaJornadaSugestao'
import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'

import {
  MinhaJornadaApi,
  MinhaJornadaXanoPDIApi,
  type BuscarSkillColaboradorResponse,
  type BuscarSkillColaboradorAlocadoResponse,
  type CalcularAderenciaResponse,
} from '@data/api/MinhaJornadaApi'
import { CompetenciasApi } from '@data/api/CompetenciasApi'
import { MinhaJornadaXanoApi } from '@data/api/MinhaJornadaXanoApi'
import {
  mapPerfil360Skills,
  flattenPerfilAtuacao,
  mapPerfisAtuacao,
  mapAdherence,
} from '@data/mappers/minhaJornadaMappers'
import { calcularGaps } from '@shared/utils/minhaJornadaComparators'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class MinhaJornadaRepositoryImpl
  implements MinhaJornadaRepository
{
  constructor(
    @inject(DiTokens.minhaJornadaApi)
    private readonly minhaJornadaApi: MinhaJornadaApi,
    @inject(DiTokens.competenciasApi)
    private readonly competenciasApi: CompetenciasApi,
    @inject(DiTokens.minhaJornadaXanoApi)
    private readonly xanoApi: MinhaJornadaXanoApi,
    @inject(DiTokens.minhaJornadaXanoPDIApi)
    private readonly pdiApi: MinhaJornadaXanoPDIApi,
  ) {}

  async getPerfil360Skills(
    codColaborador: string,
    token: string,
  ): Promise<MinhaJornadaSkill[]> {
    try {
      const response: BuscarSkillColaboradorResponse =
        await this.minhaJornadaApi.buscarSkillColaborador(
          token,
          codColaborador,
        )

      const mapped = mapPerfil360Skills(response)
      return mapped
    } catch (err) {
      throw err
    }
  }

  async getPerfilAtuacaoFlattened(
    codColaborador: string,
    orgId: number,
    token: string,
  ): Promise<MinhaJornadaProfile> {
    try {
      const response: BuscarSkillColaboradorAlocadoResponse =
        await this.minhaJornadaApi.buscarSkillColaboradorAlocado(
          token,
          codColaborador,
          orgId,
        )

      const profile = flattenPerfilAtuacao(response, orgId)

      if (!profile) {
        throw new Error(
          'Nenhum perfil de atuação encontrado para o colaborador.',
        )
      }

      return profile
    } catch (err) {
      throw err
    }
  }

  async getPerfisAtuacao(
    codColaborador: string,
    orgId: number,
    token: string,
  ): Promise<MinhaJornadaProfile[]> {
    try {
      const response: BuscarSkillColaboradorAlocadoResponse =
        await this.minhaJornadaApi.buscarSkillColaboradorAlocado(
          token,
          codColaborador,
          orgId,
        )

      return mapPerfisAtuacao(response, orgId)
    } catch (err) {
      throw err
    }
  }

  async getAdherence(
    codigoInternoColaborador: string,
    perfilId: string,
    token: string,
  ): Promise<MinhaJornadaAdherence> {
    const response: CalcularAderenciaResponse =
      await this.minhaJornadaApi.calcularAderenciaDoColaboradorAoPerfil(
        token,
        codigoInternoColaborador,
        perfilId,
      )

    const adherence = mapAdherence(response)
    if (!adherence) {
      throw new Error('Não foi possível calcular a aderência.')
    }

    return adherence
  }

  async calcularGapsEntrePerfilEPerfil360(params: {
    codColaborador: string
    orgId: number
    token: string
  }): Promise<{
    perfil: MinhaJornadaProfile
    gaps: MinhaJornadaSkill[]
    perfil360: MinhaJornadaSkill[]
  }> {
    const [perfil360, perfilAtuacao] = await Promise.all([
      this.getPerfil360Skills(params.codColaborador, params.token),
      this.getPerfilAtuacaoFlattened(
        params.codColaborador,
        params.orgId,
        params.token,
      ),
    ])

    const gaps = calcularGaps({
      requisitos: perfilAtuacao.skills,
      possuidas: perfil360,
    })

    return {
      perfil: perfilAtuacao,
      gaps,
      perfil360,
    }
  }

  async registrarInteresseSkill(
    interesses: MinhaJornadaInteresse[],
    token: string,
  ): Promise<void> {
    // Extrair minhaJornada do primeiro interesse (assumindo que todos têm o mesmo valor)
    const minhaJornada = interesses[0]?.minhaJornada ?? false

    await this.competenciasApi.adicionarInteresseColaborador(
      token,
      interesses,
      minhaJornada,
    )
  }

  async atualizarNivelCompetencia(
    payload: {
      id: number
      nivelId: number
      cpf: string
      tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
      gestorExternoPerfil: string
      minhaJornada: boolean
    },
    token: string,
  ): Promise<void> {
    const { tipo, minhaJornada, ...rest } = payload

    switch (tipo) {
      case 'hard':
        await this.competenciasApi.atualizarHardSkill(token, rest, minhaJornada)
        break
      case 'soft':
        await this.competenciasApi.atualizarSoftSkill(token, rest, minhaJornada)
        break
      case 'metodologia':
        await this.competenciasApi.atualizarMetodologia(token, rest, minhaJornada)
        break
      case 'dominio':
        await this.competenciasApi.atualizarDominio(token, rest, minhaJornada)
        break
      case 'idioma':
        await this.competenciasApi.atualizarIdioma(token, rest, minhaJornada)
        break
      default:
        throw new Error(`Tipo de competência não suportado: ${tipo}`)
    }
  }

  async criarPdi(
    payload: CriarMinhaJornadaPdiPayload,
  ): Promise<void> {
    console.debug('DEBUG MinhaJornadaRepositoryImpl.criarPdi called with payload:', payload)
    await this.xanoApi.criarPdi(payload)
  }

  async adicionarSkillPerfil360(
    payload: {
      tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
      items: Array<{
        id: number
        descricao: string
        nivelId: number
        cpf: string
        gestorExternoPerfil: string
        minhaJornada: boolean
      }>
    },
    token: string,
  ): Promise<void> {
    // Extrair minhaJornada do primeiro item (assumindo que todos têm o mesmo valor)
    const minhaJornada = payload.items[0]?.minhaJornada ?? false

    // Remover minhaJornada dos items
    const itemsWithoutMinhaJornada = payload.items.map(({ minhaJornada: _, ...item }) => item)

    switch (payload.tipo) {
      case 'hard':
        // HardSkill: API espera id, descricao, nivelId, gestorExternoPerfil (sem cpf no body)
        await this.competenciasApi.adicionarHardSkillColaborador(
          token,
          itemsWithoutMinhaJornada.map(({ cpf: _c, ...item }) => item),
          minhaJornada,
        )
        break
      case 'soft':
        // SoftSkill: API espera id, descricao, nivelId, cpf, gestorExternoPerfil
        await this.competenciasApi.adicionarSoftSkillColaborador(
          token,
          itemsWithoutMinhaJornada,
          minhaJornada,
        )
        break
      case 'metodologia':
        // Metodologia: API espera id, descricao, nivelId, gestorExternoPerfil (sem cpf no body)
        await this.competenciasApi.adicionarMetodologiaColaborador(
          token,
          itemsWithoutMinhaJornada.map(({ cpf: _c, ...item }) => item),
          minhaJornada,
        )
        break
      case 'dominio':
        // Dominio: API espera id, descricao, nivelId, cpf, gestorExternoPerfil
        await this.competenciasApi.adicionarDominioColaborador(
          token,
          itemsWithoutMinhaJornada,
          minhaJornada,
        )
        break
      case 'idioma': {
        // Idioma: API espera idiomaId (não id), descricao, nivelId, cpf, gestorExternoPerfil
        const itemsWithIdiomaId = itemsWithoutMinhaJornada.map(({ id, ...rest }) => ({
          idiomaId: id,
          ...rest,
        }))
        await this.competenciasApi.adicionarIdiomaColaborador(
          token,
          itemsWithIdiomaId,
          minhaJornada,
        )
        break
      }
      default:
        throw new Error(
          `Tipo de skill para adicionar ao Perfil 360 não suportado: ${payload.tipo}`,
        )
    }
  }

  async removerSkillPerfil360(
    params: {
      tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
      idOuCompetenciaId: number
      cpf: string
    },
    token: string,
  ): Promise<void> {
    const { tipo, idOuCompetenciaId, cpf } = params
    switch (tipo) {
      case 'hard':
        await this.competenciasApi.removerHardSkillColaborador(
          token,
          { competenciaId: idOuCompetenciaId, cpf },
        )
        break
      case 'soft':
        await this.competenciasApi.removerSoftSkillColaborador(
          token,
          { id: idOuCompetenciaId, cpf },
        )
        break
      case 'metodologia':
        await this.competenciasApi.removerMetodologiaColaborador(
          token,
          idOuCompetenciaId,
        )
        break
      case 'dominio':
        await this.competenciasApi.removerDominioColaborador(
          token,
          { id: idOuCompetenciaId, cpf },
        )
        break
      case 'idioma':
        await this.competenciasApi.removerIdiomaColaborador(
          token,
          { id: idOuCompetenciaId },
        )
        break
      default:
        throw new Error(
          `Tipo de skill para remover do Perfil 360 não suportado: ${tipo}`,
        )
    }
  }

  async sugerirNovaSkill(
    payload: MinhaJornadaSugestao,
    token: string,
  ): Promise<void> {
    // Extrair minhaJornada do payload
    const minhaJornada = payload.minhaJornada ?? false

    await this.competenciasApi.inserirSugestaoSkill(
      token,
      {
        codigoInternoColaborador: payload.codigoInternoColaborador,
        codigoGestorAdm: payload.codigoGestorAdm,
        codigoCliente: payload.codigoCliente,
        tipo_id: payload.tipoId,
        perfil_Id: payload.perfilId,
        ativo: payload.ativo,
        skill_Id: payload.skillId,
        senioridade_id: payload.senioridadeId,
        codigoGestorOper: payload.codigoGestorOper,
        gestorExternoPerfil: payload.gestorExternoPerfil,
      },
      minhaJornada,
    )
  }

  async gravarLogPdiScreenMovement(
    params: {
      token: string
      codigoInternoColaborador: string
      gestorExternoPerfil: string
      skills: MinhaJornadaSkill[]
    },
  ): Promise<void> {
    const { token, codigoInternoColaborador, gestorExternoPerfil, skills } = params

    // PDI trigger constant
    const SKILLS_MOVIMENTACAO_ID_PDI = 3

    // Filtrar skills únicas por skillId (usar Map para manter primeira ocorrência)
    const uniqueSkillsMap = new Map<number, MinhaJornadaSkill>()
    skills.forEach((skill) => {
      if (!uniqueSkillsMap.has(skill.skillId)) {
        uniqueSkillsMap.set(skill.skillId, skill)
      }
    })
    const uniqueSkills = Array.from(uniqueSkillsMap.values())

    try {
      const promises = uniqueSkills
        .filter((skill) => skill.senioridadeId != null && skill.senioridadeId !== undefined)
        .map((skill) => {
          const payload = {
            codigoInternoColaborador,
            gestorExternoPerfil,
            codigoInternoColaboradorLogado: codigoInternoColaborador, // Na Minha Jornada é o mesmo
            skillId: skill.skillId,
            itemPerfil: skill.perfilTipoId,
            nivelId: skill.senioridadeId!,
            skillsMovimentacaoId: SKILLS_MOVIMENTACAO_ID_PDI,
          }

        return this.competenciasApi
          .gravarLogsSkillsMinhaJornada(token, payload)
          .catch((error) => {
            console.error(
              `Erro ao gravar log de skill PDI (${skill.skillId}):`,
              error,
            )
          })
      })

      // Executa as chamadas em paralelo sem aguardar o resultado para não bloquear a UI
      void Promise.all(promises)
    } catch (error) {
      console.error('Erro inesperado ao disparar log de skills PDI:', error)
    }
  }

  async buscarPDIColaborador(params: {
    token: string
    uuid_colab: string
  }): Promise<Array<{
    id: number
    skills: Array<{
      id: number
      skill_name: string
      pdi_id: number
    }>
  }>> {
    const response = await this.pdiApi.buscarPDIColaborador(params)
    
    // Mapear resposta para o formato esperado, extraindo apenas os dados necessários
    return response.map((pdi) => ({
      id: pdi.id,
      skills: (pdi.skills || []).map((skill) => ({
        id: skill.id || 0,
        skill_name: skill.skill_name || '',
        pdi_id: skill.pdi_id || pdi.id,
      })),
    }))
  }
}



