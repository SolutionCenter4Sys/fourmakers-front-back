import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

export interface ListarVagasRequest {
  limite: number;
  cursor: number;
  busca: string;
  status: string[];
  dataInicio: string;
  dataFim: string;
}

/** Item de quantidade de candidatos por estágio (resposta da API). */
export interface QuantidadeCandidatosPorEstagioItem {
  idStatus?: number;
  descricaoStatus?: string;
  quantidade?: number;
}

export interface ListarVagasResponse {
  retorno?: Array<{
    id: string;
    codigo?: number;
    titulo?: string;
    modeloTrabalhoDescricao?: string;
    nomeGestor?: string;
    nomeCliente?: string;
    codigoCliente?: string;
    quantidadeCandidatosPorEstagio?: Array<{ idStatus?: number; descricaoStatus?: string; quantidade?: number }>;
  }>;
}

@injectable()
export class VagaListApi {
  async listarVagas(token: string, payload: ListarVagasRequest): Promise<ListarVagasResponse> {
    return httpClient.post<ListarVagasResponse>(
      '/api/Vaga/ListarVagasRecrutamento',
      payload,
      { token },
    );
  }
}
