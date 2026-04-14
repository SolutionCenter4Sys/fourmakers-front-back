export interface PainelControleGestor {
  qtdTotalColaboradores: number
  qtdOneOnOneEmDia: number
  qtdOneOnOneAtrasado: number
  qtdFeedbackEmDia: number
  qtdFeedbackAtrasado: number
}

export interface OneOnOneRegistroCritico {
  nomeCompleto: string
  dataReuniao: string
  descricaoAnotacoes: string
}

export interface DashboardGestorResponse {
  painelControle: PainelControleGestor
  oneOnOnesComRegistroCritico: OneOnOneRegistroCritico[]
}

export interface ObterDashboardGestorResponse {
  retorno: DashboardGestorResponse
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface ColaboradorGestor {
  /** UUID do colaborador (mesmo valor que uuid_colab no XANO); codColaboradorExterno é o código numérico quando existir */
  codigoInternoColaborador: string
  codColaboradorExterno?: string
  nomeCompleto: string
  cargo: string
  status: string
  dataUltimoFeedback: string | null
  dataUltimoOneOnOne: string | null
}

export interface MeusColaboradoresResponse {
  meusColaboradores: ColaboradorGestor[]
}

export interface ObterMeusColaboradoresResponse {
  retorno: MeusColaboradoresResponse
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface FeedbackDashboard {
  id: string
  codigoInternoColaboradorSuperior: string
  nomeCompletoColaboradorSuperior: string
  dataReuniao: string
  descricaoContinuar: string | null
  descricaoComecar: string | null
  descricaoParar: string | null
  descricaoObservacoesGerais: string | null
  visualizadoPeloColaborador: boolean
  dataVisualizadoColaborador: string | null
}

export interface DashboardFeedbacks {
  qtdVistos: number
  qtdNaoVistos: number
  feedbacks: FeedbackDashboard[]
}

export interface PautaSugerida {
  id: string
  descricaoPautaSugerida: string
  dataCriacao: string
  codigoInternoColaboradorCriacao: string
  nomeCompletoColaboradorCriacao: string
  tipoOrigem: string
}

export interface OneOnOneDashboard {
  id: string
  codigoInternoColaboradorSuperior: string
  nomeCompletoColaboradorSuperior: string
  dataReuniao: string
  descricaoAnotacoes: string | null
  registroCritico: boolean
  visualizadoPeloColaborador: boolean
  dataVisualizadoColaborador: string | null
}

export interface DashboardOneOnOne {
  qtdVistos: number
  qtdNaoVistos: number
  pautasSugeridas: PautaSugerida[]
  oneOnOnes: OneOnOneDashboard[]
}

export interface DashboardColaboradorAvaliadoResponse {
  codigoInternoColaboradorAvaliado: string
  nomeCompletoColaboradorAvaliado: string
  status: string
  regimeTrabalho?: string | null
  cargo: string
  email: string | null
  telefone: string | null
  dataNascimento: string
  dataAdmissao: string
  tempoCasa: string
  dashboardFeedbacks: DashboardFeedbacks
  dashboardOneOnOne: DashboardOneOnOne
  /** Código externo do colaborador (ex.: matrícula) — exibido no badge em vez do UUID quando disponível */
  codigoColaboradorExterno?: string | null
  /** Modalidade de contratação (CLT, PJ, etc.) */
  modalidadeContratacao?: string | null
}

export interface ObterDashboardColaboradorAvaliadoResponse {
  retorno: DashboardColaboradorAvaliadoResponse
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface InserirPautaSugeridaPayload {
  codigoInternoColaboradorAvaliado: string
  descricaoPautaSugerida: string
}

export interface InserirPautaSugeridaColaboradorPayload {
  codigoInternoColaboradorSuperior: string
  descricaoPautaSugerida: string
}

export interface InserirPautaSugeridaResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface InserirFeedbackPayload {
  codigoInternoColaboradorAvaliado: string
  dataReuniao: string
  descricaoContinuar: string
  descricaoComecar: string
  descricaoParar: string
  descricaoObservacoesGerais?: string
}

export interface InserirFeedbackResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface InserirVisualizacaoFeedbackResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface InserirOneOnOnePayload {
  codigoInternoColaboradorAvaliado: string
  dataReuniao: string
  descricaoAnotacoes: string
  registroCritico: boolean
}

export interface InserirOneOnOneResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface MeuPainelColaborador {
  qtdFeedbacks: number
  qtdOneOnOne: number
}

export interface RegistroCriticoColaborador {
  dataReuniao: string
  descricaoAnotacoes: string
}

export interface DashboardColaboradorResponse {
  meuPainel: MeuPainelColaborador
  registrosCriticos: RegistroCriticoColaborador[]
  dashboardFeedbacks: DashboardFeedbacks
  dashboardOneOnOne: DashboardOneOnOne
}

export interface ObterDashboardColaboradorResponse {
  retorno: DashboardColaboradorResponse
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface DashboardRH {
  qtdTotalColaboradores: number
  porcentagemGestoresOneOnOneEmDia: number
  porcentagemGestoresFeedbackEmDia: number
  qtdSemOneOnOneHaMaisQtdParametroDias: number
  qtdSemFeedbackHaMaisQtdParametroDias: number
}

export interface ObterDashboardRHResponse {
  retorno: DashboardRH
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface ParametrizacaoDesempenho {
  frequenciaEsperadaOneOnOneDias: number
  frequenciaEsperadaFeedbackDias: number
  periodoPadraoAnaliseDias: number | null
}

export interface InserirParametrizacaoPayload {
  FrequenciaEsperadaOneOnOneDias: number
  FrequenciaEsperadaFeedbackDias: number
}

export interface InserirParametrizacaoResponse {
  retorno: ParametrizacaoDesempenho
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface ObterParametrizacaoRetorno {
  tbOrgId: number
  frequenciaEsperadaOneOnOneDias: number
  frequenciaEsperadaFeedbackDias: number
}

export interface ObterParametrizacaoResponse {
  retorno: ObterParametrizacaoRetorno
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export interface ColaboradorRH {
  codigoInternoColaborador: string
  nomeCompleto: string
  cargo: string
  nomesColaboradoresSuperiores: string[]
  status: string
  dataUltimoFeedback: string | null
  dataUltimoOneOnOne: string | null
}

export interface ListaColaboradoresRHResponse {
  colaboradores: ColaboradorRH[]
}

export interface ObterListaColaboradoresRHResponse {
  retorno: ListaColaboradoresRHResponse
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}