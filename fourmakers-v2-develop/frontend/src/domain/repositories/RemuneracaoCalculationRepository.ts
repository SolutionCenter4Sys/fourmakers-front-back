import type {
  RemuneracaoCalculationPayload,
  RemuneracaoCalculationResponse,
} from '@domain/entities/RemuneracaoCalculation';

export interface RemuneracaoCalculationRepository {
  simulateTotalRemuneration(
    token: string,
    payload: RemuneracaoCalculationPayload,
  ): Promise<RemuneracaoCalculationResponse>;
}
