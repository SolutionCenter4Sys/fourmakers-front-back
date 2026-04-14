import type {
  AssistenteIaComunicacaoPayload,
  AssistenteIaComunicacaoProcessamentoResult,
} from '@domain/entities/comunicacao';

export interface ComunicacaoIaRepository {
  processarConteudo(
    token: string,
    payload: AssistenteIaComunicacaoPayload,
  ): Promise<AssistenteIaComunicacaoProcessamentoResult>;
}
