import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CommunityGroup } from '@domain/entities/comunicacao';
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository';

@injectable()
export class ListarComunicacaoComunidadesUseCase {
  constructor(
    @inject(DiTokens.comunicacaoComunidadeRepository)
    private readonly repository: ComunicacaoComunidadeRepository,
  ) {}

  async execute(token: string): Promise<CommunityGroup[]> {
    return this.repository.listarComunidades(token);
  }
}
