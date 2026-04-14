import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CriarComunidadeApiResponse } from '@domain/entities/comunicacao';
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository';

@injectable()
export class CriarComunicacaoComunidadeUseCase {
  constructor(
    @inject(DiTokens.comunicacaoComunidadeRepository)
    private readonly repository: ComunicacaoComunidadeRepository,
  ) {}

  async execute(
    token: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse> {
    return this.repository.criarComunidade(token, formData);
  }
}
