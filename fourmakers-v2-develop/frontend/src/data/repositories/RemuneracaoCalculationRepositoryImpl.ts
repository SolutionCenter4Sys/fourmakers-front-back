import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import { RemuneracaoCalculationApi } from '@data/api/RemuneracaoCalculationApi';
import type {
  RemuneracaoCalculationPayload,
  RemuneracaoCalculationResponse,
} from '@domain/entities/RemuneracaoCalculation';
import type { RemuneracaoCalculationRepository } from '@domain/repositories/RemuneracaoCalculationRepository';

@injectable()
export class RemuneracaoCalculationRepositoryImpl implements RemuneracaoCalculationRepository {
  constructor(
    @inject(DiTokens.remuneracaoCalculationApi)
    private readonly api: RemuneracaoCalculationApi,
  ) {}

  async simulateTotalRemuneration(
    token: string,
    payload: RemuneracaoCalculationPayload,
  ): Promise<RemuneracaoCalculationResponse> {
    return this.api.simularRemuneracaoTotal(token, payload);
  }
}
