export interface ModeloContratacao {
  codigoModeloContratacao: string
  descricao: string
  orgId: number
  deveCriarNF: boolean
}

export interface ModelosContratacaoResponse {
  retorno: ModeloContratacao[]
}

