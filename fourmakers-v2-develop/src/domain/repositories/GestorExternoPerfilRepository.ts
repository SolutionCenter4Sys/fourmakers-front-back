import type {
  ListarSkillsPerfilGestorExternoPorIdParams,
  ListarSkillsPerfilGestorExternoPorIdResponse
} from '@domain/entities/MapaAlocacao';

export interface GestorExternoPerfilRepository {
  listarSkillsPerfilGestorExternoPorId(
    token: string,
    params: ListarSkillsPerfilGestorExternoPorIdParams
  ): Promise<ListarSkillsPerfilGestorExternoPorIdResponse>;
}
