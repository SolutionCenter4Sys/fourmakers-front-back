export interface ParametroConfiguracao {
  id: string
  orgId: number
  dataCriacao: string
  dataAlteracao: string
  prioridade: number
  colaboradorOrgCpf: string | null
  grupoAcessoId: string | null
  codigoParametro: string
  valorParametro: string
  parametroNivelId: number
}

export interface ParametroConfiguracaoPayload {
  colaboradorOrgCpf: string
  grupoAcessoId: string | null
  codigoParametro: string
  valorParametro: string
  parametroNivelId: number
}

export interface ParametrosConfiguracaoResponse {
  retorno: ParametroConfiguracao[]
  sucesso: boolean
  mensagem: string | null
  erros: string | null
}

/** Resposta típica de Inserir (quando a API retorna o registro criado) */
export interface ParametroConfiguracaoInsertResponse {
  retorno: ParametroConfiguracao | null
  sucesso: boolean
  mensagem: string | null
  erros: string | null
}

