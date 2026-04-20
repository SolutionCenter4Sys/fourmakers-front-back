import type {
  ArquivarPublicacaoResponse,
  AtualizarComentarioPublicacaoPayload,
  ComentarioPublicacaoResponse,
  ComunicacaoFeedParams,
  ComunicacaoFeedResult,
  ConfirmacoesLeituraResult,
  ConfirmarLeituraObrigatoriaPayload,
  ConfirmarLeituraObrigatoriaResponse,
  CriarPublicacaoResponse,
  InserirComentarioPublicacaoPayload,
  InserirComentarioPublicacaoResponse,
  InserirInteracaoComentarioPayload,
  InserirInteracaoComentarioResponse,
  ExcluirInteracaoComentarioPayload,
  ExcluirInteracaoComentarioResponse,
  InserirInteracaoPublicacaoPayload,
  InteracaoPublicacaoResponse,
  OcultarNoFeedPayload,
} from '@domain/entities/comunicacao';
import type { CommunityPost } from '@domain/entities/comunicacao';

export interface ComunicacaoFeedRepository {
  obterPublicacaoPorId(
    token: string,
    publicacaoId: string,
  ): Promise<CommunityPost>;
  obterConfirmacoesLeitura(
    token: string,
    publicacaoId: string,
  ): Promise<ConfirmacoesLeituraResult>;
  getFeed(
    token: string,
    params?: ComunicacaoFeedParams,
  ): Promise<ComunicacaoFeedResult>;
  confirmarLeituraObrigatoria(
    token: string,
    payload: ConfirmarLeituraObrigatoriaPayload,
  ): Promise<ConfirmarLeituraObrigatoriaResponse>;
  arquivar(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse>;
  ocultarNoFeed(
    token: string,
    payload: OcultarNoFeedPayload,
  ): Promise<ArquivarPublicacaoResponse>;
  excluir(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse>;
  publicarAgora(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse>;
  aprovar(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse>;
  rejeitar(
    token: string,
    publicacaoId: string,
    motivoRejeicao: string,
  ): Promise<ArquivarPublicacaoResponse>;
  criarPublicacao(
    token: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse>;
  atualizarPublicacao(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse>;
  excluirAnexo(
    token: string,
    publicacaoId: string,
    anexoId: string,
  ): Promise<ArquivarPublicacaoResponse>;
  adicionarAnexos(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<ArquivarPublicacaoResponse>;
  inserirComentario(
    token: string,
    payload: InserirComentarioPublicacaoPayload,
  ): Promise<InserirComentarioPublicacaoResponse>;
  atualizarComentario(
    token: string,
    payload: AtualizarComentarioPublicacaoPayload,
  ): Promise<ComentarioPublicacaoResponse>;
  excluirComentario(
    token: string,
    publicacaoId: string,
    comentarioId: string,
  ): Promise<ComentarioPublicacaoResponse>;
  inserirInteracaoComentario(
    token: string,
    payload: InserirInteracaoComentarioPayload,
  ): Promise<InserirInteracaoComentarioResponse>;
  excluirInteracaoComentario(
    token: string,
    payload: ExcluirInteracaoComentarioPayload,
  ): Promise<ExcluirInteracaoComentarioResponse>;
  inserirInteracaoPublicacao(
    token: string,
    payload: InserirInteracaoPublicacaoPayload,
  ): Promise<InteracaoPublicacaoResponse>;
  excluirInteracaoPublicacao(
    token: string,
    payload: { publicacaoId: string },
  ): Promise<InteracaoPublicacaoResponse>;
}
