export interface Departamento {
  cod: string
  departamento: string
}

export interface DepartamentosResponse {
  retorno: Departamento[]
}

export interface ListarDepartamentosParams {
  codigoDiretoria?: string
}

