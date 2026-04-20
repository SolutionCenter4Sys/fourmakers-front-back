import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

export interface ListarCandidatosRequest {
  busca: string;
  cursor: number;
  limite: number;
  dataInicio: string;
  dataFim: string;
  vagaId: string;
  qualificados: boolean;
  diasUltimaAlteracao: number;
  localizacaoCidade: string;
  localizacaoEstado: string;
}

export interface ListarCandidatosResponse {
  retorno?: Array<{
    idCandidatura: string;
    nome: string;
    codigo: string;
    emailUsuario?: string | null;
    emailAlternativo?: string | null;
    descricaoStatusCandidatura?: string | null;
  }>;
}

@injectable()
export class CandidatoListApi {
  async listarCandidatos(token: string, payload: ListarCandidatosRequest): Promise<ListarCandidatosResponse> {
    const params = new URLSearchParams({
      busca: payload.busca,
      cursor: String(payload.cursor),
      limite: String(payload.limite),
      dataInicio: payload.dataInicio,
      dataFim: payload.dataFim,
      vagaId: payload.vagaId,
      qualificados: String(payload.qualificados),
      diasUltimaAlteracao: String(payload.diasUltimaAlteracao),
      localizacaoCidade: payload.localizacaoCidade,
      localizacaoEstado: payload.localizacaoEstado,
    }).toString();

    return httpClient.get<ListarCandidatosResponse>(
      `/api/Vaga/ListarCandidatosInscritos?${params}`,
      { token },
    );
  }
}
