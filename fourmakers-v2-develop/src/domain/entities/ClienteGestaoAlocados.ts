export interface ClienteGestaoAlocados {
  codigoCliente: string
  nomeCliente: string
}

export interface ClientesGestaoAlocadosResponse {
  retorno?: ClienteGestaoAlocados[] | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}
