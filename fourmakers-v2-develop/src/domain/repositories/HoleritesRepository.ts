import type { 
  ListarHoleritesColaboradorPorAnoParams,
  ListarHoleritesColaboradorPorAnoResponse,
  AssinarHoleritePorLoteIdParams,
  AssinarHoleritePorLoteIdResponse
} from '@domain/entities/Holerite';

export interface HoleritesRepository {
  listarHoleritesColaboradorPorAno(
    token: string,
    params: ListarHoleritesColaboradorPorAnoParams
  ): Promise<ListarHoleritesColaboradorPorAnoResponse>;
  assinarHoleritePorLoteId(
    token: string,
    params: AssinarHoleritePorLoteIdParams
  ): Promise<AssinarHoleritePorLoteIdResponse>;
}

