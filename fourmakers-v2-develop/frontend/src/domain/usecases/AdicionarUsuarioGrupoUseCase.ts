import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { AdicionarUsuarioGrupoAcessoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class AdicionarUsuarioGrupoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(
    token: string,
    usuarioId: number,
    grupoAcessoId: number
  ): Promise<AdicionarUsuarioGrupoAcessoResponse> {
    return this.repository.adicionarUsuarioGrupo(token, usuarioId, grupoAcessoId);
  }
}
