import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CriarGrupoAcessoPayload, CriarGrupoAcessoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class CriarGrupoAcessoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(token: string, payload: CriarGrupoAcessoPayload): Promise<CriarGrupoAcessoResponse> {
    return this.repository.criarGrupo(token, payload);
  }
}
