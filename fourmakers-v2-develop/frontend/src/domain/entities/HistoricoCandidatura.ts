/** Item de arquivo em alteração ou comentário */
export interface HistoricoCandidaturaArquivo {
  id?: string
  nome?: string
  url?: string
  [key: string]: unknown
}

/** Alteração (movimentação) de status da candidatura */
export interface HistoricoCandidaturaAlteracao {
  idStatus: number
  statusDescricao: string
  recrutador: string
  dataAlteracao: string
  comentario: string | null
  idComentario: string | null
  motivoReprovacao: string
  motivoDeclinio: string
  recrutadorResponsavel: string
  arquivos: HistoricoCandidaturaArquivo[]
}

/** Comentário da jornada do candidato */
export interface HistoricoCandidaturaComentario {
  id: string
  texto: string
  dataCriacao: string
  dataAlteracao: string | null
  codigoInternoColaborador: string
  codigoInternoColaboradorNome: string
  candidaturaId: string
  arquivos: HistoricoCandidaturaArquivo[]
}

/** Item do retorno: candidatura em uma vaga com alterações e comentários */
export interface HistoricoCandidaturaItem {
  idCandidatura: string
  nomeCandidato: string
  codVaga: number
  nomeVaga: string
  nomeGestor: string
  nomeCliente: string
  pretencaoSalarial: number
  modeloTrabalhoId: string | null
  modeloTrabalhoDescricao: string | null
  disponibilidadeEntrevistaId: string | null
  disponibilidadeEntrevistaDescricao: string | null
  quantidadeDiasPresencial: number | null
  dataUltimaAlteracao: string
  descricaoUltimaAlteracao: string
  qualificado: boolean | null
  statusDaVaga: string
  alteracoes: HistoricoCandidaturaAlteracao[]
  comentarios: HistoricoCandidaturaComentario[]
}

export interface HistoricoCandidaturaResponse {
  retorno?: HistoricoCandidaturaItem[] | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}
