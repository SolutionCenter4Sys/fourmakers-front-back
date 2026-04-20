import type {
  CommunityGroup,
  ComunicacaoComunidadeDetalheRetorno,
  CriarComunidadeApiResponse,
} from '@domain/entities/comunicacao';

export interface ComunicacaoComunidadeRepository {
  listarComunidades(token: string): Promise<CommunityGroup[]>;
  obterComunidadePorId(
    token: string,
    comunidadeId: string,
  ): Promise<ComunicacaoComunidadeDetalheRetorno>;
  criarComunidade(
    token: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse>;
  atualizarComunidade(
    token: string,
    comunidadeId: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse>;
  participar(token: string, comunidadeId: string): Promise<{ sucesso: boolean; mensagem: string | null }>;
  sair(token: string, comunidadeId: string): Promise<{ sucesso: boolean; mensagem: string | null }>;
}
