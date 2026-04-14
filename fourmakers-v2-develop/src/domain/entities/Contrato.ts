// Entidade para Contratos vinculados a Parceiros
export interface Contrato {
  id: string // UUID
  parceiroId: string // UUID do parceiro
  contrato: string // Número/Nome do contrato
  dataInicio: string // ISO Date
  dataFim: string // ISO Date
  clausulaPenalidades: string
  numeroPaginas: number
  referenciaCotacao: string
  valorContrato: number // decimal
  plataformaAssinatura: string
  reajusteAnual: string
  contratoAssinado: boolean
  /** Status do contrato. Null quando a API não informa (não deve acionar badge "Vencido"). */
  status: StatusContrato | null
  renovado: boolean
  emailsNotificacao: string[]
  arquivo?: ArquivoContrato
  urlAnexo?: string
  diasParaVencimento?: number // Calculado no frontend
  alertaVencimento?: boolean // Calculado no frontend (≤ 150 dias)
  dataCriacao?: string
  dataAtualizacao?: string
}

export type StatusContrato = 'Em andamento' | 'Completo' | 'Arquivado'

/** ID do enum de status no backend: Andamento=0, Arquivado=1, Completo=2 */
export type StatusContratoId = 0 | 1 | 2

/** Converte StatusContrato (valor exibido) para ID numérico enviado à API. Null é tratado como Andamento (0). */
export function statusValorParaId(status: StatusContrato | null): StatusContratoId {
  if (status == null) return 0
  switch (status) {
    case 'Em andamento':
      return 0
    case 'Arquivado':
      return 1
    case 'Completo':
      return 2
    default:
      return 0
  }
}

export interface ArquivoContrato {
  id: string
  nomeArquivo: string
  urlDownload: string
  tipo: string
  tamanho: number
  dataUpload: string
}

// Request/Response Types
export interface InserirContratoPayload {
  parceiroId: string // UUID
  contrato: string
  dataInicio: string
  dataFim: string
  clausulaPenalidades: string
  numeroPaginas: number
  referenciaCotacao: string
  valorContrato: number
  plataformaAssinatura: string
  reajusteAnual: string
  contratoAssinado: boolean
  status: StatusContratoId
  renovado: boolean
  emailsNotificacao: string[]
  urlAnexo?: string // URL do arquivo anexado
  codigoInternoColaborador?: string
}

export interface AtualizarContratoPayload extends InserirContratoPayload {
  id: string // UUID do contrato
}

export interface DeletarContratoParams {
  id: string // UUID do contrato
}

export interface ContratoResponse {
  sucesso: boolean
  mensagem?: string
  dados?: Contrato
  erros?: string[]
}

export interface DeletarContratoResponse {
  sucesso: boolean
  mensagem?: string
}
