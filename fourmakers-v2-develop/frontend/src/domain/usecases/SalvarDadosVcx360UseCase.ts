import { inject, injectable } from 'tsyringe'

import type { DadosVcx360 } from '@domain/entities/Vcx360'
import type { Vcx360Repository } from '@domain/repositories/Vcx360Repository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class SalvarDadosVcx360UseCase {
  constructor(
    @inject(DiTokens.vcx360Repository)
    private readonly repository: Vcx360Repository,
  ) {}

  async execute(codigoCliente: string, dados: DadosVcx360): Promise<void> {
    await this.repository.salvarDadosVcx(codigoCliente, dados)
  }
}
