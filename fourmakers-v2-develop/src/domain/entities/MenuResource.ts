export interface MenuResource {
  codigoRecurso: string
  codigoRecursoMenu: string
  id: string
  dataCriacao: string
  dataAlteracao: string
  nomeMenu: string
  codigoRecursoMenuPai: string | null
  codigoIcone: string
  tipoMenu: 'item_header' | 'item_profile' | 'group_sidebar' | 'item_sidebar'
  linkExternoNovaPagina: string | null
  ordenacao: number
  emBreve: boolean
  visivel: boolean
  subMenus: MenuResource[]
  rotaReact?: string | null
}

