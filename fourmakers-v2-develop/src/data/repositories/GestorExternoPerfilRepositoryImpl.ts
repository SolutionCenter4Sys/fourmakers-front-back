import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { GestorExternoPerfilRepository } from '@domain/repositories/GestorExternoPerfilRepository';
import type {
  ListarSkillsPerfilGestorExternoPorIdParams,
  ListarSkillsPerfilGestorExternoPorIdResponse
} from '@domain/entities/MapaAlocacao';
import type { GestorExternoPerfilApi } from '@data/api/GestorExternoPerfilApi';

@injectable()
export class GestorExternoPerfilRepositoryImpl implements GestorExternoPerfilRepository {
  constructor(
    @inject(DiTokens.gestorExternoPerfilApi)
    private readonly gestorExternoPerfilApi: GestorExternoPerfilApi
  ) {}

  async listarSkillsPerfilGestorExternoPorId(
    token: string,
    params: ListarSkillsPerfilGestorExternoPorIdParams
  ): Promise<ListarSkillsPerfilGestorExternoPorIdResponse> {
    return this.gestorExternoPerfilApi.listarSkillsPerfilGestorExternoPorId(token, params);
  }
}
