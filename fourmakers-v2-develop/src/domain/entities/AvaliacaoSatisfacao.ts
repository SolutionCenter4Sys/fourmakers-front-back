export interface InserirAvaliacaoSatisfacaoPayload {
  servicoRate: number
  recomendacaoRate: number
  experienciaDescricao: string
  aspectoDescricao: string
}

export interface InserirAvaliacaoSatisfacaoResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
}

