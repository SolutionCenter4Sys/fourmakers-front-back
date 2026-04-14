import type { Vaga, PerfilVaga, StatusVaga } from '@domain/entities/Vaga';

export interface ListarVagasRecrutamentoEPerfisParams {
  cursor: number;
  limite: number;
  dataInicio: string;
  dataFim: string;
  busca: string;
  statusList: string[];
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

export interface ListarPerfisVagasParams {
  orgId: number;
  vagaId?: number;
  busca?: string;
  cursor?: number;
  limite?: number;
}

export interface ListarVagasRecrutamentoEPerfisResult {
  vagas: Vaga[];
  perfis: PerfilVaga[];
  totalVagas: number | Record<string, number>;
  totalPerfis: number;
}

export interface ListarVagasRecrutamentoResult {
  vagas: Vaga[];
  totalVagas: number | Record<string, number>;
}

export interface ListarPerfisVagasResult {
  perfis: PerfilVaga[];
  totalPerfis: number;
}

export interface VagasRepository {
  listarVagasRecrutamentoEPerfis(
    token: string,
    params: ListarVagasRecrutamentoEPerfisParams
  ): Promise<ListarVagasRecrutamentoEPerfisResult>;

  listarVagasRecrutamento(
    token: string,
    params: ListarVagasRecrutamentoParams
  ): Promise<ListarVagasRecrutamentoResult>;

  listarPerfisVagas(
    token: string,
    params: ListarPerfisVagasParams
  ): Promise<ListarPerfisVagasResult>;

  listarStatusVagas(token: string): Promise<StatusVaga[]>;
}
