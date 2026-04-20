export interface Cargo {
  cargo: string
  codigoCargo: string
}

export interface CargosResponse {
  retorno: Cargo[]
  sucesso: boolean
  mensagem: string | null
  erros: any | null
}

