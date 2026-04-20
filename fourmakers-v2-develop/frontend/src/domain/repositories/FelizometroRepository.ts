import type { FelizometroPayload, FelizometroResponse } from '@domain/entities/FelizometroSentimento'

export interface FelizometroRepository {
  enviarSentimento(payload: FelizometroPayload): Promise<FelizometroResponse>
}

