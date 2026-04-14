import { injectable } from 'tsyringe';
import type { Manager, UserProfile } from '@domain/entities/SimulatorTypes';
import { httpClient } from './httpClient';

@injectable()
export class RecruitmentApi {
  async getManagers(token: string, search: string): Promise<Manager[]> {
    const params = new URLSearchParams({
      busca: search ?? '',
      cursor: '0',
      limite: '50000',
    });

    const data = await httpClient.get<{ ColaboradoresCchResult?: Manager[] }>(
      `/api/MapaDeAlocacao/ListarColaboradoresOrg?${params.toString()}`,
      { token }
    );

    return data.ColaboradoresCchResult || [];
  }

  async getUserProfile(token: string): Promise<UserProfile> {
    return httpClient.get<UserProfile>(
      `/api/Colaborador/BuscarFormularioColaborador?token=${token}`,
      { token }
    );
  }
}

