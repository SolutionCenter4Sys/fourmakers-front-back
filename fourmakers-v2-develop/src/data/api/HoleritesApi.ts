import { httpClient } from './httpClient';
import type {
  ListarHoleritesColaboradorPorAnoParams,
  ListarHoleritesColaboradorPorAnoResponse,
  AssinarHoleritePorLoteIdParams,
  AssinarHoleritePorLoteIdResponse
} from '@domain/entities/Holerite';

export class HoleritesApi {
  async listarHoleritesColaboradorPorAno(
    token: string,
    params: ListarHoleritesColaboradorPorAnoParams
  ): Promise<ListarHoleritesColaboradorPorAnoResponse> {
    const queryParams = new URLSearchParams();
    queryParams.append('codigoInternoColaborador', params.codigoInternoColaborador);
    queryParams.append('ano', params.ano.toString());

    const queryString = queryParams.toString();
    const url = `/api/Financeiro/Holerite/Colaborador/ListarHoleritesColaboradorPorAno${queryString ? `?${queryString}` : ''}`;

    return httpClient.get<ListarHoleritesColaboradorPorAnoResponse>(url, { token });
  }

  async assinarHoleritePorLoteId(
    token: string,
    params: AssinarHoleritePorLoteIdParams
  ): Promise<AssinarHoleritePorLoteIdResponse> {
    const url = `/api/Financeiro/Holerite/Colaborador/AssinarHoleritePorLoteId?itemLoteId=${params.itemLoteId}`;
    return httpClient.get<AssinarHoleritePorLoteIdResponse>(url, { token });
  }
}

