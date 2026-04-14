export interface Aniversariante {
  codigoColaborador: string
  nome: string
  dataNascimento: string
  email: string | null
  imagemId: string | null
  diaDaSemana: string
  urlFoto: string | null
  urlFotoThumb: string | null
  urlFotoThumbMini: string | null
  urlFotoThumbVeryMini: string | null
  ehAniversarianteHoje: boolean
}

export interface AniversariantesSemanaResponse {
  retorno: Aniversariante[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface AniversariantesSemanaParams {
  codDiretoria?: string
}

