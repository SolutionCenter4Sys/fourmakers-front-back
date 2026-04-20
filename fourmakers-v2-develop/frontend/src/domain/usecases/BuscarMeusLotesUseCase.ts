import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradorBancoDeTalentosRepository } from '@domain/repositories/ColaboradorBancoDeTalentosRepository';

@injectable()
export class BuscarMeusLotesUseCase {
  constructor(
    @inject(DiTokens.colaboradorBancoDeTalentosRepository)
    private readonly repository: ColaboradorBancoDeTalentosRepository,
  ) {}

  async execute(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number } = {}
  ) {
    return this.repository.buscarMeusLotes(token, params);
  }
}
