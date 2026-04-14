export interface EmpresasRelacionadasResponse {
  retorno: string[]
  sucesso: boolean
  mensagem: string | null
  erros: string | null
}

export interface ListarEmpresasRelacionadasParams {
  nomeEmpresa: string
  limite: number
  cursor: number
}
