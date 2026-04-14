export interface MinhaJornadaGoalMeta {
  meta: string
  prazo: string // ISO date string
  skills: string[]
}

export interface CriarMinhaJornadaPdiPayload {
  metas: MinhaJornadaGoalMeta[]
  colaborador: string
  manager: string
  uuid_colab: string
  id_colab: number
  org_id: number
  criado_por_colab: boolean
  pp?: string
  id_manager?: number
  email_colab?: string
  email_gestor?: string
  business_unit?: string
  uuid_manager?: string
}


