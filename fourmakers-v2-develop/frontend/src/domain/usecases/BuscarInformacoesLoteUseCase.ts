import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradorBancoDeTalentosRepository } from '@domain/repositories/ColaboradorBancoDeTalentosRepository';

@injectable()
export class BuscarInformacoesLoteUseCase {
  constructor(
    @inject(DiTokens.colaboradorBancoDeTalentosRepository)
    private readonly repository: ColaboradorBancoDeTalentosRepository
  ) {}

  async execute(token: string, idLote: string, params?: Record<string, string | null | undefined>) {
    return this.repository.buscarInformacoesLote(token, idLote, params);
  }
}
