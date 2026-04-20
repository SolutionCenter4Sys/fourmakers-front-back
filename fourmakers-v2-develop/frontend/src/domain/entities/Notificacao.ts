export interface Notificacao {
  titulo: string
  mensagem: string
  mensagemHtml: string
  lida: boolean
  dataEnvio: string
  dataLeitura: string | null
  rota: string
  rotaCompleta: string
}

export interface ContarNotificacoesNaoLidasResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
  retorno: number
}

export interface ListarNotificacoesResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
  retorno: Notificacao[]
}

export interface MarcarNotificacoesComoLidasResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
}

