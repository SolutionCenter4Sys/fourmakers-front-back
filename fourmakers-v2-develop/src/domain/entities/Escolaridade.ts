export interface EscolaridadeColaborador {
  id: number
  instituicao: string
  formacaoId: number
  formacaoDescricao: string
  dataInicio: string
  dataTermino: string | null
  descricao: string
  ativo: boolean
  colaboradorCpf: string
  dataCriacao: string
  dataAlteracao: string
  tipoDiplomaId: number
  filePathInternal: string | null
  filePath: string | null
}

export interface ListarEscolaridadeColaboradorResponse {
  escolaridade: EscolaridadeColaborador[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ListarEscolaridadeColaboradorParams {
  busca: string
  cursor: number
  limite: number
}
