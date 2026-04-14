import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { GestorItem } from '@domain/repositories/PerfilAtuacaoRepository';
import type { PerfilAtuacaoRepository } from '@domain/repositories/PerfilAtuacaoRepository';

@injectable()
export class ListarGestoresUseCase {
  constructor(
    @inject(DiTokens.perfilAtuacaoRepository)
    private readonly repository: PerfilAtuacaoRepository
  ) {}

  async execute(
    token: string,
    clientCode: string | null,
    busca: string,
    cursor?: number,
    limite?: number,
  ): Promise<GestorItem[]> {
    return this.repository.listarGestores(token, clientCode, busca, cursor, limite);
  }
}
