export interface ColaboradorCch {
  cd_Profissional: string
  nm_Profissional: string
  codigoColaboradorInterno: string
}

export interface ColaboradoresCchResponse {
  ColaboradoresCchResult: ColaboradorCch[]
}

export interface ListarColaboradoresOrgParams {
  busca: string
  cursor: number
  limite: number
}

