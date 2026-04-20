import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ImportarColaboradorLoteResult } from '@domain/entities/CurriculoColaborador';
import type { CurriculoColaboradorRepository } from '@domain/repositories/CurriculoColaboradorRepository';

@injectable()
export class ImportarColaboradorLoteUseCase {
  constructor(
    @inject(DiTokens.curriculoColaboradorRepository)
    private readonly repository: CurriculoColaboradorRepository
  ) {}

  async execute(token: string, file: File): Promise<ImportarColaboradorLoteResult> {
    return this.repository.importarColaboradorLote(token, file);
  }
}
