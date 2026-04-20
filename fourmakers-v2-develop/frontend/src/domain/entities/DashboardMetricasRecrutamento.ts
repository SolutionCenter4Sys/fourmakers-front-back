/** Parâmetros de filtro para os endpoints api/Candidatura/* (DataInicio, DataFim, CodigoCliente, CodigoRecrutador) */
export interface FiltrosCandidaturaParams {
  DataInicio?: string
  DataFim?: string
  CodigoCliente?: string[]
  CodigoRecrutador?: string[]
}

/** Retorno do GET api/Candidatura/dashboardMetricasRecrutamento */
export interface DashboardMetricasRecrutamentoRetorno {
  emFoco: number
  emAndamento: number
  contratacoes: number
  entrevistaCliente: number
  vagasCanceladas: number
  vagasPerdidas: number
  tempoMedioDiasRecrutamento: number
  porcentagemEntrevistaEmAndamento: number
}

export interface DashboardMetricasRecrutamentoResponse {
  retorno?: DashboardMetricasRecrutamentoRetorno | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}

/** Item da lista do GET api/Candidatura/dashboardMetricasVagasEmFoco. Backend pode retornar codVaga (camelCase) ou CodVaga (PascalCase). */
export interface DashboardMetricasVagasEmFocoItem {
  cliente: string
  codVaga?: string | null
  /** Alternativa PascalCase retornada pelo backend; usar codVaga ?? CodVaga para valor normalizado. */
  CodVaga?: string | null
  vaga: string
  status: string
  responsavel: string | null
  ultimaMovimentacao: string
  tempoNaEtapa: number
  sla?: string
}

export interface DashboardMetricasVagasEmFocoResponse {
  retorno?: DashboardMetricasVagasEmFocoItem[] | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}

/** Retorno do GET api/Candidatura/dashboardMetricasFunilDeVagas */
export interface DashboardMetricasFunilDeVagasRetorno {
  emFoco: number
  entrevistaInicial: number
  aplicacaoTestes: number
  entrevistaTecnica: number
  entrevistaComCliente: number
  cartaOferta: number
  procurandoCandidatos: number
}

export interface DashboardMetricasFunilDeVagasResponse {
  retorno?: DashboardMetricasFunilDeVagasRetorno | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}

/** Item do GET api/Candidatura/dashboardMetricasVagasPerdidasMotivo */
export interface DashboardMetricasVagasPerdidasMotivoItem {
  total: number
  id: string | null
  motivo: string
}

export interface DashboardMetricasVagasPerdidasMotivoResponse {
  retorno?: DashboardMetricasVagasPerdidasMotivoItem[] | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}

/** Item do GET api/Candidatura/dashboardNovosCandidatosPorOrigem */
export interface DashboardNovosCandidatosPorOrigemItem {
  total: number
  origem: string
  orgId: number
}

export interface DashboardNovosCandidatosPorOrigemResponse {
  retorno?: DashboardNovosCandidatosPorOrigemItem[] | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}
