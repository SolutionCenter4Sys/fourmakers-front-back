import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';
import type {
  RemuneracaoCalculationPayload,
  RemuneracaoCalculationResponse,
} from '@domain/entities/RemuneracaoCalculation';

@injectable()
export class RemuneracaoCalculationApi {
  async simularRemuneracaoTotal(
    token: string,
    payload: RemuneracaoCalculationPayload,
  ): Promise<RemuneracaoCalculationResponse> {
    return httpClient.post<RemuneracaoCalculationResponse>(
      '/api/Vaga/Calculos/SimularRemuneracaoTotal',
      payload,
      { token },
    );
  }
}
