import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';
import type {
  ListarSkillsPerfilGestorExternoPorIdParams,
  ListarSkillsPerfilGestorExternoPorIdResponse
} from '@domain/entities/MapaAlocacao';

@injectable()
export class GestorExternoPerfilApi {
  async listarSkillsPerfilGestorExternoPorId(
    token: string,
    params: ListarSkillsPerfilGestorExternoPorIdParams
  ): Promise<ListarSkillsPerfilGestorExternoPorIdResponse> {
    const url = `/api/GestaoDeAlocados/GestorExternoPerfil/ListarSkillsPerfilGestorExternoPorId?perfilId=${encodeURIComponent(params.perfilId)}`;
    
    return httpClient.get<ListarSkillsPerfilGestorExternoPorIdResponse>(url, { token });
  }
}
