import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { RemoverFuncionalidadeGrupoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class RemoverFuncionalidadeGrupoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(
    token: string,
    grupoAcessoId: number,
    funcionalidadeSistemaId: number
  ): Promise<RemoverFuncionalidadeGrupoResponse> {
    return this.repository.removerFuncionalidadeGrupo(token, grupoAcessoId, funcionalidadeSistemaId);
  }
}
