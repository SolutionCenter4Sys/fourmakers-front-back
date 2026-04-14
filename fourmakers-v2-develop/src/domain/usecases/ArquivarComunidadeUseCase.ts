import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CriarComunidadeApiResponse } from '@domain/entities/comunicacao';
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository';
import { buildComunidadeFormDataFromDetalhe } from '@shared/utils/comunicacaoComunidadeMapper';

@injectable()
export class ArquivarComunidadeUseCase {
  constructor(
    @inject(DiTokens.comunicacaoComunidadeRepository)
    private readonly repository: ComunicacaoComunidadeRepository,
  ) {}

  async execute(
    token: string,
    comunidadeId: string,
  ): Promise<CriarComunidadeApiResponse> {
    const detalhe = await this.repository.obterComunidadePorId(token, comunidadeId);
    const formData = await buildComunidadeFormDataFromDetalhe(detalhe, false, token);
    return this.repository.atualizarComunidade(token, comunidadeId, formData);
  }
}
