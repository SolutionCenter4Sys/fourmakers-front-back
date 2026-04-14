import { httpClient } from './httpClient'
import { XANO_BASE_URL } from '@shared/constants'
import { getXanoHeaders } from '@shared/utils/xanoUtils'
import { createHttpClient } from './httpClient'
import type { XanoPDIResponse } from './MinhaEquipeApi'

export interface BuscarSkillColaboradorResponse {
  retorno: Array<{
    codigoInternoColaborador: string
    nomeCompleto: string
    colaboradorHabilidades: Array<{
      codigoInternoColaborador: string
      perfilTipoId: number
      tipoPerfil: string
      skillId: number
      habilidade: string
      senioridadeId: number
      senioridade: string
      interesse: number
    }>
  }>
}

export interface BuscarSkillColaboradorAlocadoResponse {
  retorno: Array<{
    codigoInternoGestorAdm: string
    nomeGestorAdm: string
    codigoInternoColaborador: string
    nomeCompleto: string
    codigoProjeto: string
    idAlocacao: number
    clientes: Array<{
      codigoCliente: string
      nomeCliente: string
      perfilId: string
      perfil: string
      codGestorCliente: string
      nomeGestorCliente: string
      habilidades: Array<{
        codigoCliente: string
        perfilTipoId: number
        tipoPerfil: string
        skillId: number
        habilidade: string
        senioridadeId: number
        senioridade: string
        interesse: number
      }>
    }>
  }>
}

export interface CalcularAderenciaResponse {
  retorno: any
}

export interface SugestaoHistoricoItem {
  id: string
  codigoInternoColaboradorAvaliador: string
  codigoInternoColaborador: string
  aprovado: boolean
  tbStatusSugestaoId: number
  perfil_Id: string
  observacao: string
  data: string
}

export interface SugestaoItem {
  id: string
  codigoInternoColaborador: string
  codigoGestorAdm: string
  codigoCliente: string
  tipo_Id: number
  descricaoTipo: string
  skill_Id: number
  descricaoSkill: string
  perfil_Id: string
  senioridade_Id: number
  senioridade: string
  data: string
  ativo: boolean
  historicoSugestao: SugestaoHistoricoItem[]
}

export interface BuscarSugestaoResponse {
  retorno: SugestaoItem[]
  sucesso: boolean
  mensagem: string
  erros: string | null
}

export class MinhaJornadaApi {
  async buscarSkillColaborador(
    token: string,
    codColaborador: string,
  ): Promise<BuscarSkillColaboradorResponse> {
    return httpClient.get<BuscarSkillColaboradorResponse>(
      `/api/GestaoDeAlocados/MinhaJornada/BuscarSkillColaborador?codColaborador=${encodeURIComponent(codColaborador)}`,
      { token }
    )
  }

  async buscarSkillColaboradorAlocado(
    token: string,
    codColaborador: string,
    orgId: number,
  ): Promise<BuscarSkillColaboradorAlocadoResponse> {
    return httpClient.get<BuscarSkillColaboradorAlocadoResponse>(
      `/api/GestaoDeAlocados/MinhaJornada/BuscarSkillColaboradorAlocado?codColaborador=${encodeURIComponent(codColaborador)}&orgId=${orgId}`,
      { token }
    )
  }

  async calcularAderenciaDoColaboradorAoPerfil(
    token: string,
    codigoInternoColaborador: string,
    perfilId: string,
  ): Promise<CalcularAderenciaResponse> {
    return httpClient.get<CalcularAderenciaResponse>(
      `/api/GestaoDeAlocados/MinhaJornada/CalcularAderenciaDoColaboradorAoPerfil?codigoInternoColaborador=${encodeURIComponent(codigoInternoColaborador)}&perfilId=${encodeURIComponent(perfilId)}`,
      { token }
    )
  }

  async buscarSugestaoPorCodColaboradorOuAdm(
    token: string,
    codInternoGestor: string,
    perfilId: string,
    codInternoColaborador: string,
  ): Promise<BuscarSugestaoResponse> {
    return httpClient.get<BuscarSugestaoResponse>(
      `/api/GestaoDeAlocados/MinhaJornada/BuscarSugestaoPorCodColaboradorOuAdm?codInternoGestor=${encodeURIComponent(
        codInternoGestor,
      )}&perfilId=${encodeURIComponent(perfilId)}&codInternoColaborador=${encodeURIComponent(
        codInternoColaborador,
      )}`,
      { token }
    )
  }
}

const xanoHttpClient = createHttpClient({ baseURL: XANO_BASE_URL })

export class MinhaJornadaXanoPDIApi {
  async buscarPDIColaborador(params: {
    token: string
    uuid_colab: string
  }): Promise<XanoPDIResponse[]> {
    const queryParams = new URLSearchParams({
      id: '0',
      status: '',
      external: '{}',
      collaborator_id: '0',
      manager_id: '0',
      business_unit: '',
      manager: '',
      people_partner: '',
      vigencia: '',
      status_filter: '',
      uuid_colab: params.uuid_colab,
      uuid_manager: '0',
    })

    const xanoHeaders = getXanoHeaders() as Record<string, string>

    return xanoHttpClient.get<XanoPDIResponse[]>(
      `/pdi?${queryParams.toString()}`,
      {
        token: params.token,
        headers: xanoHeaders,
      }
    )
  }
}
