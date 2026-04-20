import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradorBancoDeTalentosRepository } from '@domain/repositories/ColaboradorBancoDeTalentosRepository';

@injectable()
export class BuscarPessoasCadastradasPorColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradorBancoDeTalentosRepository)
    private readonly repository: ColaboradorBancoDeTalentosRepository,
  ) {}

  async execute(
    token: string,
    codColaborador: string,
    params: { busca?: string; cursor?: number; limite?: number } = {}
  ) {
    return this.repository.buscarPessoasCadastradasPorColaborador(token, codColaborador, params);
  }
}
