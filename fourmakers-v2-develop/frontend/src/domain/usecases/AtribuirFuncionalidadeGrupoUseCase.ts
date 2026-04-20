import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { AdicionarFuncionalidadeGrupoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class AtribuirFuncionalidadeGrupoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(
    token: string,
    grupoAcessoId: number,
    funcionalidadeSistemaId: number
  ): Promise<AdicionarFuncionalidadeGrupoResponse> {
    return this.repository.adicionarFuncionalidadeGrupo(token, grupoAcessoId, funcionalidadeSistemaId);
  }
}
