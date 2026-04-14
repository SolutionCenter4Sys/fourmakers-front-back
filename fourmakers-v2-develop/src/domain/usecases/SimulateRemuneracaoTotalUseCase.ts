import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  RemuneracaoCalculationPayload,
  RemuneracaoCalculationResponse,
} from '@domain/entities/RemuneracaoCalculation';
import type { RemuneracaoCalculationRepository } from '@domain/repositories/RemuneracaoCalculationRepository';

@injectable()
export class SimulateRemuneracaoTotalUseCase {
  constructor(
    @inject(DiTokens.remuneracaoCalculationRepository)
    private readonly repository: RemuneracaoCalculationRepository,
  ) {}

  async execute(
    token: string,
    payload: RemuneracaoCalculationPayload,
  ): Promise<RemuneracaoCalculationResponse> {
    return this.repository.simulateTotalRemuneration(token, payload);
  }
}
