import type {
  AnalyticsResumoPayload,
  AnalyticsResumoRetorno,
} from '@domain/entities/comunicacao';

export interface ComunicacaoAnalyticsRepository {
  obterResumo(
    token: string,
    payload: AnalyticsResumoPayload,
  ): Promise<AnalyticsResumoRetorno | null>;
}
