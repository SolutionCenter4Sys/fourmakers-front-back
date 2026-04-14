import type { LogEntry } from '@shared/types/reembolso'
import type { VerbaData } from '@shared/types/reembolso'
import { logsMock, verbasInicialMock } from '../mocks/reembolsoParametrosMock'

export class ReembolsoParametrosApi {
  async getLogs(): Promise<LogEntry[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...logsMock]
  }

  async getVerbasInicial(): Promise<VerbaData[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...verbasInicialMock]
  }
}

