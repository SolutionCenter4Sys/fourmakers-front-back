import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ImportarColaboradorLotePlanilhaResult } from '@domain/entities/CurriculoColaborador';
import type { CurriculoColaboradorRepository } from '@domain/repositories/CurriculoColaboradorRepository';

@injectable()
export class ImportarColaboradorLotePlanilhaUseCase {
  constructor(
    @inject(DiTokens.curriculoColaboradorRepository)
    private readonly repository: CurriculoColaboradorRepository
  ) {}

  async execute(token: string, file: File): Promise<ImportarColaboradorLotePlanilhaResult> {
    return this.repository.importarColaboradorLotePlanilha(token, file);
  }
}
