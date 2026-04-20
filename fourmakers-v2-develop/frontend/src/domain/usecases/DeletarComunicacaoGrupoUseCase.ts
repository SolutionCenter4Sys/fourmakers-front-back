import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CriarGrupoResponse } from '@domain/entities/comunicacao';
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository';

@injectable()
export class DeletarComunicacaoGrupoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoGrupoRepository)
    private readonly repository: ComunicacaoGrupoRepository,
  ) {}

  async execute(
    token: string,
    grupoId: string,
  ): Promise<CriarGrupoResponse> {
    return this.repository.deletarGrupo(token, grupoId);
  }
}
