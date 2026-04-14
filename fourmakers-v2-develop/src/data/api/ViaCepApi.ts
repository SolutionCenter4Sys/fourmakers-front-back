import { createHttpClient } from './httpClient';

const viaCepClient = createHttpClient({ baseURL: 'https://viacep.com.br' });

export interface ViaCepResponse {
  cep?: string;
  logradouro?: string;
  complemento?: string;
  bairro?: string;
  localidade?: string;
  uf?: string;
  erro?: boolean;
}

/**
 * Busca endereço por CEP no ViaCEP.
 * Usa httpClient (abstração aprovada) em vez de fetch() direto.
 * @param cep - 8 dígitos (apenas números)
 */
export async function fetchCep(cep: string): Promise<ViaCepResponse> {
  const normalized = cep.replace(/\D/g, '').slice(0, 8);
  if (normalized.length !== 8) {
    throw new Error('CEP deve ter 8 dígitos');
  }
  const data = await viaCepClient.get<ViaCepResponse>(`/ws/${normalized}/json/`);
  return data ?? {};
}

/**
 * Classe usada pelo DI e ViaCepRepositoryImpl.
 * Delega para fetchCep (httpClient) em vez de fetch() direto.
 */
export class ViaCepApi {
  async buscarEnderecoPorCep(cep: string): Promise<ViaCepResponse> {
    return fetchCep(cep);
  }
}
