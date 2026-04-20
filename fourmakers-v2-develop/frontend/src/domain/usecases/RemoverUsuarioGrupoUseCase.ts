import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { RemoverUsuarioGrupoAcessoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class RemoverUsuarioGrupoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(
    token: string,
    usuarioId: number,
    grupoAcessoId: number
  ): Promise<RemoverUsuarioGrupoAcessoResponse> {
    return this.repository.removerUsuarioGrupo(token, usuarioId, grupoAcessoId);
  }
}
