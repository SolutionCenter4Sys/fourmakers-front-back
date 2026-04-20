import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoPublicacaoApi } from '@data/api/ComunicacaoPublicacaoApi';
import type {
  ArquivarPublicacaoResponse,
  AtualizarComentarioPublicacaoPayload,
  ComentarioPublicacaoResponse,
  ComunicacaoFeedParams,
  ComunicacaoFeedResult,
  ConfirmacaoLeituraItemApi,
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
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository';
import { publicacaoFeedApiToCommunityPost } from '@shared/utils/comunicacaoFeedMapper';
import { processarUrlComToken } from '@shared/utils/urlUtils';

@injectable()
export class ComunicacaoFeedRepositoryImpl implements ComunicacaoFeedRepository {
  constructor(
    @inject(DiTokens.comunicacaoFeedApi)
    private readonly api: ComunicacaoPublicacaoApi,
  ) {}

  async obterPublicacaoPorId(
    token: string,
    publicacaoId: string,
  ): Promise<CommunityPost> {
    const response = await this.api.getPublicacaoById(token, publicacaoId);
    if (!response.sucesso || !response.retorno) {
      throw new Error(response.mensagem ?? 'Falha ao obter publicação.');
    }
    return publicacaoFeedApiToCommunityPost(response.retorno, token);
  }

  async obterConfirmacoesLeitura(
    token: string,
    publicacaoId: string,
  ): Promise<ConfirmacoesLeituraResult> {
    const response = await this.api.getConfirmacoesLeitura(token, publicacaoId);
    const raw = response.retorno;
    let itens: ConfirmacaoLeituraItemApi[] = [];
    let totalDestinatarios = 0;
    if (Array.isArray(raw)) {
      itens = raw;
    } else if (raw && typeof raw === 'object') {
      totalDestinatarios = (raw as { totalDestinatarios?: number }).totalDestinatarios ?? 0;
      itens =
        (raw as { itens?: ConfirmacaoLeituraItemApi[] }).itens ??
        (raw as { lista?: ConfirmacaoLeituraItemApi[] }).lista ??
        [];
    }
    const confirmacoes = itens.map((item) => ({
      id: item.codigoColaboradorInterno ?? '',
      name: item.nomeCompleto ?? item.nome ?? '—',
      email: item.email ?? undefined,
      cargo: item.cargo ?? undefined,
      unidade: item.departamento ?? item.diretoria ?? undefined,
      urlFoto: item.urlFoto ? processarUrlComToken(item.urlFoto, token) ?? undefined : undefined,
      viewedAt: item.dataVisualizacao ?? '',
      acknowledgedAt: item.dataConfirmacaoLeitura ?? undefined,
    }));
    return {
      totalDestinatarios: totalDestinatarios || confirmacoes.length,
      confirmacoes,
    };
  }

  async getFeed(
    token: string,
    params?: ComunicacaoFeedParams,
  ): Promise<ComunicacaoFeedResult> {
    const response = await this.api.getFeed(token, params ?? {});
    if (!response.sucesso || !response.retorno) {
      throw new Error(
        response.mensagem ?? 'Falha ao carregar feed de comunicação.',
      );
    }
    const retorno = response.retorno;
    const rawList = retorno.publicacoes ?? [];
    const publicacoesRaw =
      params?.excluirPendenteAprovacao === true
        ? rawList.filter((p) => {
            if ((p.aprovacaoStatus ?? '').toLowerCase() === 'pendente') return false;
            if ((p.publicacaoStatus ?? '').toLowerCase().replace(/-/g, '_') === 'agendada') return false;
            return true;
          })
        : rawList;
    const publicacoes = publicacoesRaw.map((p) =>
      publicacaoFeedApiToCommunityPost(p, token),
    );
    const primeiraInteracao = retorno.publicacoes?.[0]?.interacao;
    const codigoColaboradorInternoViewer =
      primeiraInteracao?.codigoInterno ?? undefined;
    return {
      quantidadeTotal: retorno.quantidadeTotal,
      quantidadePendentesLeitura: retorno.quantidadePendentesLeitura,
      quantidadeLidosAceitos: retorno.quantidadeLidosAceitos,
      labels: retorno.labels ?? [],
      publicacoes,
      codigoColaboradorInternoViewer,
    };
  }

  async confirmarLeituraObrigatoria(
    token: string,
    payload: ConfirmarLeituraObrigatoriaPayload,
  ): Promise<ConfirmarLeituraObrigatoriaResponse> {
    return this.api.confirmarLeituraObrigatoria(token, payload);
  }

  async arquivar(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.arquivar(token, publicacaoId);
  }

  async ocultarNoFeed(
    token: string,
    payload: OcultarNoFeedPayload,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.ocultarNoFeed(token, payload);
  }

  async excluir(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.excluir(token, publicacaoId);
  }

  async publicarAgora(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.publicarAgora(token, publicacaoId);
  }

  async aprovar(
    token: string,
    publicacaoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.aprovar(token, publicacaoId);
  }

  async rejeitar(
    token: string,
    publicacaoId: string,
    motivoRejeicao: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.rejeitar(token, publicacaoId, motivoRejeicao);
  }

  async criarPublicacao(
    token: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse> {
    return this.api.criarPublicacao(token, formData);
  }

  async atualizarPublicacao(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<CriarPublicacaoResponse> {
    return this.api.atualizarPublicacao(token, publicacaoId, formData);
  }

  async excluirAnexo(
    token: string,
    publicacaoId: string,
    anexoId: string,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.excluirAnexo(token, publicacaoId, anexoId);
  }

  async adicionarAnexos(
    token: string,
    publicacaoId: string,
    formData: FormData,
  ): Promise<ArquivarPublicacaoResponse> {
    return this.api.adicionarAnexos(token, publicacaoId, formData);
  }

  async inserirComentario(
    token: string,
    payload: InserirComentarioPublicacaoPayload,
  ): Promise<InserirComentarioPublicacaoResponse> {
    return this.api.inserirComentario(token, payload);
  }

  async atualizarComentario(
    token: string,
    payload: AtualizarComentarioPublicacaoPayload,
  ): Promise<ComentarioPublicacaoResponse> {
    return this.api.atualizarComentario(token, payload);
  }

  async excluirComentario(
    token: string,
    publicacaoId: string,
    comentarioId: string,
  ): Promise<ComentarioPublicacaoResponse> {
    return this.api.excluirComentario(token, publicacaoId, comentarioId);
  }

  async inserirInteracaoComentario(
    token: string,
    payload: InserirInteracaoComentarioPayload,
  ): Promise<InserirInteracaoComentarioResponse> {
    return this.api.inserirInteracaoComentario(token, payload);
  }

  async excluirInteracaoComentario(
    token: string,
    payload: ExcluirInteracaoComentarioPayload,
  ): Promise<ExcluirInteracaoComentarioResponse> {
    return this.api.excluirInteracaoComentario(token, payload);
  }

  async inserirInteracaoPublicacao(
    token: string,
    payload: InserirInteracaoPublicacaoPayload,
  ): Promise<InteracaoPublicacaoResponse> {
    return this.api.inserirInteracaoPublicacao(token, payload);
  }

  async excluirInteracaoPublicacao(
    token: string,
    payload: { publicacaoId: string },
  ): Promise<InteracaoPublicacaoResponse> {
    return this.api.excluirInteracaoPublicacao(token, payload);
  }
}
