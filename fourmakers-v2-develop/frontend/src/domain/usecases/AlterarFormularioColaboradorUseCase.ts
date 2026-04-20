import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradoresRepository, AlterarFormularioColaboradorPayload } from '@domain/repositories/ColaboradoresRepository';

@injectable()
export class AlterarFormularioColaboradorUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly repository: ColaboradoresRepository
  ) {}

  async execute(
    token: string,
    payload: AlterarFormularioColaboradorPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return this.repository.alterarFormularioColaborador(token, payload);
  }
}
