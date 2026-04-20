import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { PermissoesUsuarioLogado } from '@domain/entities/comunicacao';
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository';

@injectable()
export class ObterPermissoesUsuarioLogadoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoGrupoRepository)
    private readonly repository: ComunicacaoGrupoRepository,
  ) {}

  async execute(token: string): Promise<PermissoesUsuarioLogado | null> {
    return this.repository.obterPermissoesUsuarioLogado(token);
  }
}
