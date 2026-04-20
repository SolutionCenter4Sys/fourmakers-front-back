import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoGrupoDetalhe } from '@domain/entities/comunicacao';
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository';

@injectable()
export class ObterComunicacaoGrupoPorIdUseCase {
  constructor(
    @inject(DiTokens.comunicacaoGrupoRepository)
    private readonly repository: ComunicacaoGrupoRepository,
  ) {}

  async execute(
    token: string,
    grupoId: string,
  ): Promise<ComunicacaoGrupoDetalhe | null> {
    return this.repository.obterGrupoPorId(token, grupoId);
  }
}
