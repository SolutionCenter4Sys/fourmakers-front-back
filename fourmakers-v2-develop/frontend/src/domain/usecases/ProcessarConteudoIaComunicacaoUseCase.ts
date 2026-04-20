import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  AssistenteIaComunicacaoPayload,
  AssistenteIaComunicacaoProcessamentoResult,
} from '@domain/entities/comunicacao';
import type { ComunicacaoIaRepository } from '@domain/repositories/ComunicacaoIaRepository';

@injectable()
export class ProcessarConteudoIaComunicacaoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoIaRepository)
    private readonly repository: ComunicacaoIaRepository,
  ) {}

  async execute(
    token: string,
    payload: AssistenteIaComunicacaoPayload,
  ): Promise<AssistenteIaComunicacaoProcessamentoResult> {
    return this.repository.processarConteudo(token, payload);
  }
}
