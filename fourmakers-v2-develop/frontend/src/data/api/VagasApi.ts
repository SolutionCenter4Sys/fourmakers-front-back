import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

export interface ListarVagasRecrutamentoEPerfisParams {
  cursor: number;
  limite: number;
  dataInicio: string;
  dataFim: string;
  busca: string;
  statusList: string[];
}

export interface VagaRecrutamento {
  id: number;
  titulo: string;
  descricao?: string;
  dataCriacao: string;
  dataEnd?: string;
  status: string;
  nomeUsuarioCriador?: string;
  recrutadorVaga?: {
    id: number;
    nome: string;
  };
  totalPerfis?: number;
  totalAderentes?: number;
  [key: string]: unknown;
}

export interface PerfilVaga {
  id: number;
  nome: string;
  email?: string;
  telefone?: string;
  status?: string;
  vagaId?: number;
  [key: string]: unknown;
}

export interface StatusVaga {
  id: string;
  nome: string;
  ativo: boolean;
  ordem?: number;
}

export interface ListarVagasRecrutamentoEPerfisResponse {
  retorno: {
    vagasRecrutamento: VagaRecrutamento[];
    perfis: PerfilVaga[];
    totalVagas: number;
    totalPerfis: number;
  };
}

export interface ListarVagasRecrutamentoParams {
  cursor: number;
  limite: number;
  dataInicio?: string;
  dataFim?: string;
  busca?: string;
  statusList?: string[];
  orgId?: number;
}

export interface ListarVagasRecrutamentoResponse {
  retorno: {
    vagasRecrutamento: VagaRecrutamento[];
    totalVagas: number;
  };
}

export interface ListarPerfisVagasParams {
  orgId: number;
  vagaId?: number;
  busca?: string;
  cursor?: number;
  limite?: number;
}

export interface ListarPerfisVagasResponse {
  retorno: {
    perfis: PerfilVaga[];
    totalPerfis: number;
  };
}

export interface ListarStatusVagasResponse {
  retorno: StatusVaga[];
}

@injectable()
export class VagasApi {
  /**
   * Lista vagas de recrutamento e perfis associados
   */
  async listarVagasRecrutamentoEPerfis(
    token: string,
    params: ListarVagasRecrutamentoEPerfisParams
  ): Promise<ListarVagasRecrutamentoEPerfisResponse> {
    // Em hml/produção este endpoint é POST no controller Vaga
    return httpClient.post<ListarVagasRecrutamentoEPerfisResponse>(
      '/api/Vaga/ListarVagasRecrutamentoEPerfis',
      {
        cursor: params.cursor,
        limite: params.limite,
        dataInicio: params.dataInicio,
        dataFim: params.dataFim,
        busca: params.busca || '',
        statusList: params.statusList,
      },
      { token }
    )
  }

  /**
   * Lista apenas vagas de recrutamento
   */
  async listarVagasRecrutamento(
    token: string,
    params: ListarVagasRecrutamentoParams
  ): Promise<ListarVagasRecrutamentoResponse> {
    // Em hml/produção este endpoint é POST no controller Vaga
    return httpClient.post<ListarVagasRecrutamentoResponse>(
      '/api/Vaga/ListarVagasRecrutamento',
      {
        cursor: params.cursor,
        limite: params.limite,
        dataInicio: params.dataInicio,
        dataFim: params.dataFim,
        busca: params.busca,
        statusList: params.statusList,
        orgId: params.orgId,
      },
      { token }
    )
  }

  /**
   * Lista perfis de vagas
   */
  async listarPerfisVagas(
    token: string,
    params: ListarPerfisVagasParams
  ): Promise<ListarPerfisVagasResponse> {
    const queryParams = new URLSearchParams({
      orgId: params.orgId.toString(),
    });

    if (params.vagaId) {
      queryParams.append('vagaId', params.vagaId.toString());
    }
    if (params.busca) {
      queryParams.append('busca', params.busca);
    }
    if (params.cursor !== undefined) {
      queryParams.append('cursor', params.cursor.toString());
    }
    if (params.limite) {
      queryParams.append('limite', params.limite.toString());
    }

    return httpClient.get<ListarPerfisVagasResponse>(
      `/api/Recrutamento/ListarPerfisVagas?${queryParams.toString()}`,
      { token }
    );
  }

  /**
   * Lista status de vagas disponíveis
   */
  async listarStatusVagas(token: string): Promise<ListarStatusVagasResponse> {
    return httpClient.get<ListarStatusVagasResponse>(
      // Endpoint válido em hml/produção
      '/api/Vaga/ListarStatusVagaRecrutamento',
      { token }
    );
  }
}
