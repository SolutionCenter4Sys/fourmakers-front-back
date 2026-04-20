import type {
  BancoTalentosItem,
  ColaboradorMatchPromptItem,
  DetalhesLote,
  LoteItemGestao,
  PessoaCadastradaPorColaborador,
  PessoaCadastradaPorOrg,
} from '@domain/entities/GestaoVagasCandidatos';

export interface ColaboradorBancoDeTalentosRepository {
  buscarBancoTalentos(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number }
  ): Promise<BancoTalentosItem[]>;

  buscarPessoasCadastradasPorOrg(
    token: string,
    params: {
      busca?: string;
      cursor?: number;
      limite?: number;
      statusVaga?: string;
      statusCandidatura?: number;
      dataInicio?: string;
      dataFim?: string;
    }
  ): Promise<PessoaCadastradaPorOrg[]>;

  buscarPessoasCadastradasPorColaborador(
    token: string,
    codColaborador: string,
    params: { busca?: string; cursor?: number; limite?: number }
  ): Promise<PessoaCadastradaPorColaborador[]>;

  buscarBancoTalentosComPromptMatch(
    token: string,
    params: { texto_vaga: string; limite?: number }
  ): Promise<{
    colaboradores: ColaboradorMatchPromptItem[];
    idLogRankCandidatesIds?: string | null;
    /** Resposta da API com perfil_extraido, validacao_informacoes (mensagem_usuario), etc. */
    prompt?: unknown;
  }>;

  buscarMeusLotes(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number }
  ): Promise<LoteItemGestao[]>;

  buscarInformacoesLote(
    token: string,
    idLote: string,
    params?: { busca?: string; cursor?: string | null; limite?: string | null }
  ): Promise<DetalhesLote>;
}
