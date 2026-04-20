export interface MinhaJornadaSugestao {
  codigoInternoColaborador: string
  codigoGestorAdm: string
  codigoCliente: string
  perfilId: string
  skillId: number
  tipoId: number
  senioridadeId: number
  ativo: boolean
  codigoGestorOper?: string
  gestorExternoPerfil: string // UUID
  minhaJornada: boolean // true
}


