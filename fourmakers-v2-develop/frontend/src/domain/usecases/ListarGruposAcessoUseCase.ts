import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ListarGruposAcessoFuncionalidadesSistemaResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class ListarGruposAcessoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(token: string): Promise<ListarGruposAcessoFuncionalidadesSistemaResponse> {
    return this.repository.listarGruposComFuncionalidades(token);
  }
}
