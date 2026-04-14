import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

export interface CandidateApiResponse {
  colaborador?: {
    cpf?: string | null;
    documentoColaborador?: string | null;
    nomeCompleto: string;
    email?: string | null;
    contatoPrincipal?: string | null;
    rg?: string | null;
    dataNascimento?: string | null;
    cargo?: { cargo?: string | null } | null;
    diretoria?: {
      diretoria?: string | null;
    } | null;
    endereco?: {
      cep?: string | null;
      endereco?: string | null;
      complemento?: string | null;
      numero?: number | null;
      bairro?: string | null;
      cidade?: string | null;
      estado?: string | null;
    } | null;
  };
  sucesso?: boolean;
  mensagem?: string | null;
}

@injectable()
export class CandidateApi {
  async buscarDados(token: string, candidateId: string): Promise<CandidateApiResponse> {
    const params = new URLSearchParams({ cpf: candidateId });
    return httpClient.get<CandidateApiResponse>(`/api/Colaborador/BuscarDadosColaborador?${params.toString()}`, {
      token,
    });
  }
}
