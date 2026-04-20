export interface RecrutadorGestaoAlocados {
  codigoInternoColaborador: string
  nome: string
  /** Código do gestor externo (legado ListarGestoresExterno). */
  codGestorExterno?: string
  /** Código do recrutador retornado por api/Candidatura/RecrutadorListagem; usado nos filtros do dashboard. */
  codigoRecrutador?: string
}

export interface RecrutadoresGestaoAlocadosResponse {
  retorno?: RecrutadorGestaoAlocados[] | null
  sucesso: boolean
  mensagem?: string | null
  erros?: unknown | null
}
