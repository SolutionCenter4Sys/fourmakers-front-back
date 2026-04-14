import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  CurriculoColaboradorRepository,
  SincronizarCurriculoResult,
} from '@domain/repositories/CurriculoColaboradorRepository';

@injectable()
export class SincronizarCurriculoColaboradorUseCase {
  constructor(
    @inject(DiTokens.curriculoColaboradorRepository)
    private readonly repository: CurriculoColaboradorRepository
  ) {}

  async execute(token: string, file: File): Promise<SincronizarCurriculoResult> {
    return this.repository.sincronizarCurriculoColaborador(token, file);
  }
}
