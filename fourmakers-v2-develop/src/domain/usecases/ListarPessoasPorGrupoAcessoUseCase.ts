import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ListarPessoasPorGrupoAcessoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class ListarPessoasPorGrupoAcessoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(token: string): Promise<ListarPessoasPorGrupoAcessoResponse> {
    return this.repository.listarPessoasPorGrupoAcesso(token);
  }
}
