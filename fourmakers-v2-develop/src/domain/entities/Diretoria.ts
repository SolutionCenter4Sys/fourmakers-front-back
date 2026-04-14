export interface Diretoria {
  codDiretoria: string
  diretoria: string
  configuracaoParaTodaOrg: boolean
}

export interface ListarDiretoriasDisponiveisResponse {
  retorno: Diretoria[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

// Interface para a resposta da API de Colaboradores
export interface DiretoriaColaborador {
  cod: string
  diretoria: string
}

export interface DiretoriasResponse {
  diretoriaColaborador: DiretoriaColaborador[]
}
