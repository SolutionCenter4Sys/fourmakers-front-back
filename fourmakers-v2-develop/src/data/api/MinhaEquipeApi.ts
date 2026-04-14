import { XANO_BASE_URL } from '@shared/constants'
import { getXanoHeaders } from '@shared/utils/xanoUtils'
import { httpClient } from './httpClient'
import { createHttpClient } from './httpClient'
import type { AprovarRejeitarSugestaoPayload } from '@domain/entities/MinhaEquipeSugestao'

export interface ListaIndicadoresLideradosResponse {
  retorno: Array<
    ListaIndicadoresLideradosLegacyItem | ListaIndicadoresLideradosAgrupadoItem
  >
  sucesso: boolean
  mensagem: string
  erros: string[] | null
}

export interface ListaIndicadoresLideradosLegacyItem {
  id: string
  nomeColaborador: string
  nomeCliente: string
  nomeGestorCliente: string
  perfil: string
  perfilId: string
  nomeGestorOperacional: string
  nomeGestorAdm: string
  codigoInternoColaborador: string
  codigoGestorAdm: string
  idAlocacao: number
  match?: number
  retornoMatch?: {
    hardSkills?: number
    softSkills?: number
    methodologies?: number
    domains?: number
    languages?: number
  }
  resultadoHabilidades?: Array<{
    id: string
    name: string
    type: number
    requiredLevel: number
    currentLevel: number
    pendencia: boolean
    interesse: number
  }> | null
}

export interface ListaIndicadoresLideradosAgrupadoItem {
  codigoGestorAdm: string
  nomeGestorAdm: string
  codigoGestorOperacional?: string
  nomeGestorOperacional?: string
  gestoresOperacionais?: ListaIndicadoresGestorOperacional[]
  colaboradores?: ListaIndicadoresAgrupadoColaborador[]
}

export interface ListaIndicadoresGestorOperacional {
  codigoGestorOperacional: string
  nomeGestorOperacional: string
}

export interface ListaIndicadoresAgrupadoColaborador {
  codigoInternoColaborador: string
  nomeColaborador: string
  codigoProjeto?: string
  idAlocacao?: number
  clientes?: ListaIndicadoresCliente[]
}

export interface ListaIndicadoresCliente {
  codigoCliente?: string
  nomeCliente?: string
  perfilId?: string
  perfil?: string
  codGestorCliente?: string
  nomeGestorCliente?: string
  resultadoHabilidades?: ListaIndicadoresRawHabilidade[] | null
  retornoMatch?: ListaIndicadoresRetornoMatch | null
}

export interface ListaIndicadoresRetornoMatch {
  codigoInternoColaborador?: string
  nome?: string
  orgs?: number[]
  match?: number
  score_candidato?: number
  score_vaga?: number
  detalhamento_calculo?: ListaIndicadoresDetalhamentoCalculo
  comparativo_por_skill?: Record<string, unknown>
  origem?: string | null
}

export interface ListaIndicadoresDetalhamentoCalculo {
  hard_skills?: ListaIndicadoresScoreBreakdown
  soft_skills?: ListaIndicadoresScoreBreakdown
  metodologias?: ListaIndicadoresScoreBreakdown
  dominios_negocio?: ListaIndicadoresScoreBreakdown
  idiomas?: ListaIndicadoresScoreBreakdown
  disponibilidades?: ListaIndicadoresScoreBreakdown
}

export interface ListaIndicadoresScoreBreakdown {
  score_bruto_categoria?: number
  score_bruto_obrigatorio?: number
  score_bruto_desejavel?: number
}

export interface ListaIndicadoresRawHabilidade {
  id?: string
  name?: string
  type?: number
  requiredLevel?: number
  currentLevel?: number
  pendencia?: boolean
  interesse?: number
  perfilTipoId?: number
  tipoPerfil?: string
  habilidade?: string
  colaboradorNivel?: string
  vagaNivel?: string
  status?: string
}

export interface TotalizacaoIndicadoresResponse {
  retorno: Array<{
    totColaboradores: number
    totPendentesSkills: number
    mediaMatch: number
  }>
  sucesso: boolean
  mensagem: string
  erros: string[] | null
}

export interface BuscarSugestoesResponse {
  retorno: Array<{
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
    historicoSugestao?: Array<{
      id: string
      codigoInternoColaboradorAvaliador: string
      codigoInternoColaborador: string
      aprovado: boolean
      tbStatusSugestaoId: number
      perfil_Id: string
      observacao: string
      data: string
    }>
  }>
  sucesso: boolean
  mensagem: string
  erros: string[] | null
}

/**
 * Resposta da API Xano para busca de PDIs
 * A API pode retornar campos em diferentes formatos (camelCase ou snake_case)
 */
export interface XanoPDIResponse {
  id: number
  skillName?: string
  skill_name?: string
  deadline?: string | number
  prazo?: string | number
  status?: 'em_andamento' | 'concluida' | 'atrasada' | 'Não iniciado' | 'Em andamento' | string
  actions?: string
  acoes?: string
  created_at?: string | number
  createdAt?: string | number
  updated_at?: string | number
  updatedAt?: string | number
  skills?: Array<{
    id?: number
    skill_name?: string
    pdi_id?: number
  }>
}

/**
 * Item retornado pelo endpoint XANO /pdi/relatorio/
 */
export interface PdiRelatorioItem {
  id: number
  collaborator: string
  status: string
  uuid_colab?: string
  email_colab?: string
  [key: string]: unknown
}

const xanoHttpClient = createHttpClient({ baseURL: XANO_BASE_URL })

export class MinhaEquipeApi {
  async listarIndicadoresLiderados(params: {
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
    cursor: number
    limite: number
  }): Promise<ListaIndicadoresLideradosResponse> {
    const queryParams = new URLSearchParams({
      codGestorAdm: params.codGestorAdm,
      orgId: params.orgId.toString(),
      codGestorOper: params.codGestorOper,
      cursor: params.cursor.toString(),
      limite: params.limite.toString(),
    })

    return httpClient.get<ListaIndicadoresLideradosResponse>(
      `/api/GestaoDeAlocados/MinhaEquipe/ListaIndicadoresDosLideradosAdmOper?${queryParams.toString()}`,
      { token: params.token }
    )
  }

  async buscarTotalizacaoIndicadores(params: {
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
  }): Promise<TotalizacaoIndicadoresResponse> {
    const queryParams = new URLSearchParams({
      codGestorAdm: params.codGestorAdm,
      orgId: params.orgId.toString(),
      codGestorOper: params.codGestorOper,
    })

    return httpClient.get<TotalizacaoIndicadoresResponse>(
      `/api/GestaoDeAlocados/MinhaEquipe/TotalizacaoIndicadoresDosLideradosAdmOper?${queryParams.toString()}`,
      { token: params.token }
    )
  }

  async buscarPDIColaborador(params: {
    token: string
    uuid_colab: string
    uuid_manager: string
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
      uuid_manager: '0', // Sempre '0', não enviar CPF do gestor
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

  /**
   * Lista PDIs do relatório XANO (para contagem por colaborador na gestão de desempenho).
   * Usa todos=true para trazer todos os PDIs da org e o filtro por status ativo é feito no cliente.
   * @param _token - token de autenticação (reservado para uso futuro)
   * @param orgId - orgId do usuário logado
   */
  async getPdiRelatorio(_token: string, orgId: number): Promise<PdiRelatorioItem[]> {
    const queryParams = new URLSearchParams({
      data_inicio: '',
      data_fim: '',
      unidade: '',
      gestor: '',
      people_partner: '',
      todos: 'true',
      em_andamento: 'false',
      nao_iniciado: 'false',
      finalizados: 'false',
      org_id: String(orgId),
    })

    const xanoHeaders = getXanoHeaders() as Record<string, string>

    const result = await xanoHttpClient.get<PdiRelatorioItem[]>(
      `/pdi/relatorio/?${queryParams.toString()}`,
      { headers: xanoHeaders }
    )
    return Array.isArray(result) ? result : []
  }

  async buscarSugestoesColaborador(params: {
    token: string
    codInternoGestor: string
    perfilId: string
    codInternoColaborador: string
  }): Promise<BuscarSugestoesResponse> {
    const queryParams = new URLSearchParams({
      codInternoGestor: params.codInternoGestor,
      perfilId: params.perfilId,
      codInternoColaborador: params.codInternoColaborador,
    })

    return httpClient.get<BuscarSugestoesResponse>(
      `/api/GestaoDeAlocados/MinhaJornada/BuscarSugestaoPorCodColaboradorOuAdm?${queryParams.toString()}`,
      { token: params.token }
    )
  }

  async aprovarRejeitarSugestao(
    token: string,
    payload: AprovarRejeitarSugestaoPayload
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    return httpClient.post<{ sucesso: boolean; mensagem: string; erros: string[] | null }>(
      '/api/GestaoDeAlocados/MinhaJornada/AprovarRejeitarSugestao',
      payload,
      {
        token,
        headers: {
          'Content-Type': 'application/json; charset=utf-8',
        },
      }
    )
  }
}
