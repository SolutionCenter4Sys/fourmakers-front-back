import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ImportarColaboradorResult } from '@domain/entities/CurriculoColaborador';
import type { CurriculoColaboradorRepository } from '@domain/repositories/CurriculoColaboradorRepository';

@injectable()
export class ImportarColaboradorUseCase {
  constructor(
    @inject(DiTokens.curriculoColaboradorRepository)
    private readonly repository: CurriculoColaboradorRepository
  ) {}

  async execute(token: string, file: File): Promise<ImportarColaboradorResult> {
    return this.repository.importarColaborador(token, file);
  }
}
