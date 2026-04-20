export interface FelizometroPayload {
  collaborator_id: string
  collaborator: string
  manager: string
  emotion: string
  message?: string | null
  date: string
  other_emotion?: string | null
  unit: string
}

export interface FelizometroResponse {
  sucesso: boolean
  mensagem?: string
}

