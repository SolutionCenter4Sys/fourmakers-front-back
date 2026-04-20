import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { ListarProjetosComAtividadesResponse } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarProjetosComAtividadesUseCase {
  constructor(
    @inject(DiTokens.timesheetRepository)
    private readonly repository: TimesheetRepository,
  ) {}

  async execute(
    token: string,
    cpfColaborador?: string
  ): Promise<ListarProjetosComAtividadesResponse> {
    return this.repository.listarProjetosComAtividades(token, cpfColaborador)
  }
}

