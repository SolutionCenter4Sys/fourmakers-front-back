import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradorDisponivel } from '@domain/entities/comunicacao';
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository';

@injectable()
export class ListarColaboradoresDisponiveisUseCase {
  constructor(
    @inject(DiTokens.comunicacaoGrupoRepository)
    private readonly repository: ComunicacaoGrupoRepository,
  ) {}

  async execute(
    token: string,
    params?: { grupoId?: string; filtro?: string },
  ): Promise<ColaboradorDisponivel[]> {
    return this.repository.listarColaboradoresDisponiveis(token, params);
  }
}
