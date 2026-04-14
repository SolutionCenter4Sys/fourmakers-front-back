import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ImportarColaboradorResult } from '@domain/entities/CurriculoColaborador';
import type { CurriculoColaboradorRepository } from '@domain/repositories/CurriculoColaboradorRepository';

@injectable()
export class ImportarColaboradorLinkedinUseCase {
  constructor(
    @inject(DiTokens.curriculoColaboradorRepository)
    private readonly repository: CurriculoColaboradorRepository
  ) {}

  async execute(token: string, perfilId: string): Promise<ImportarColaboradorResult> {
    return this.repository.importarColaboradorLinkedin(token, perfilId);
  }
}
