import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradorBancoDeTalentosRepository } from '@domain/repositories/ColaboradorBancoDeTalentosRepository';

@injectable()
export class BuscarBancoTalentosComPromptMatchUseCase {
  constructor(
    @inject(DiTokens.colaboradorBancoDeTalentosRepository)
    private readonly repository: ColaboradorBancoDeTalentosRepository
  ) {}

  async execute(token: string, params: { texto_vaga: string; limite?: number }) {
    return this.repository.buscarBancoTalentosComPromptMatch(token, params);
  }
}
