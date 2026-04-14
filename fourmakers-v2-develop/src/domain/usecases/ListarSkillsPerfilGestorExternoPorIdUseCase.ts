import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { GestorExternoPerfilRepository } from '@domain/repositories/GestorExternoPerfilRepository';
import type {
  ListarSkillsPerfilGestorExternoPorIdParams,
  ListarSkillsPerfilGestorExternoPorIdResponse
} from '@domain/entities/MapaAlocacao';

@injectable()
export class ListarSkillsPerfilGestorExternoPorIdUseCase {
  constructor(
    @inject(DiTokens.gestorExternoPerfilRepository)
    private readonly repository: GestorExternoPerfilRepository
  ) {}

  async execute(
    token: string,
    params: ListarSkillsPerfilGestorExternoPorIdParams
  ): Promise<ListarSkillsPerfilGestorExternoPorIdResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!params.perfilId) {
      throw new Error('perfilId é obrigatório');
    }

    return this.repository.listarSkillsPerfilGestorExternoPorId(token, params);
  }
}
