import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { AlterarDadosColaboradorParametrosPayload } from '@domain/entities/AlterarDadosColaboradorParametros';
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository';
import type { CurriculoColaboradorRepository } from '@domain/repositories/CurriculoColaboradorRepository';

export interface AtualizarDadosComplementaresCandidaturaParams {
  payload: AlterarDadosColaboradorParametrosPayload;
  fileCurriculo?: File | null;
}

/**
 * Orquestra a atualização de dados complementares do colaborador/candidato (portal público)
 * e, opcionalmente, o upload do currículo.
 */
@injectable()
export class AtualizarDadosComplementaresCandidaturaUseCase {
  constructor(
    @inject(DiTokens.colaboradoresRepository)
    private readonly colaboradoresRepository: ColaboradoresRepository,
    @inject(DiTokens.curriculoColaboradorRepository)
    private readonly curriculoColaboradorRepository: CurriculoColaboradorRepository,
  ) {}

  async execute(
    token: string,
    params: AtualizarDadosComplementaresCandidaturaParams,
  ): Promise<void> {
    await this.colaboradoresRepository.alterarDadosColaboradorParametros(token, params.payload);
    if (params.fileCurriculo) {
      await this.curriculoColaboradorRepository.insereCurriculoColaborador(token, params.fileCurriculo);
    }
  }
}
