import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

/** Limites padrão para as APIs de Banco de Talentos (evitam números mágicos; a UI pode passar outro valor via params). */
export const LIMITES_BANCO_TALENTOS = {
  /** BuscarBancoTalentosComPromptMatch – quantidade de perfis retornados no match por IA. */
  promptMatch: 15,
  /** BuscarBancoTalentos / BuscarPessoasCadastradasPorColaborador – itens por página. */
  listagem: 1000,
  /** BuscarMeusLotes – itens por página em Minhas Importações. */
  meusLotes: 5000,
} as const;

export interface CandidaturaItem {
  idCandidatura: string;
  tituloVaga: string;
  nomeCliente: string | null;
  codigoCliente: string | null;
  codigoGestor: string | null;
  nomeGestor: string | null;
}

export interface PessoaCadastradaPorColaborador {
  nome: string;
  codigoInternoColaborador: string;
  nomeCadastrante: string;
  dataDoCadastro: string;
  possuiCandidatura: boolean;
  candidaturas: CandidaturaItem[];
  match: number;
}

/** Item retornado por BuscarBancoTalentos (banco de talentos da org, sem filtro por colaborador). */
export interface BancoTalentosItem {
  nome: string;
  codigoInternoColaborador: string;
  nomeCadastrante: string | null;
  dataDoCadastro: string;
  possuiCandidatura: boolean;
  candidaturas: CandidaturaItem[] | null;
  match: number;
}

/** Candidatura no formato da API BuscarBancoTalentosComPromptMatch (snake_case). */
export interface CandidaturaPromptMatchItem {
  id_candidatura?: string;
  titulo_vaga?: string;
  codigo_cliente?: string | null;
  nome_cliente?: string | null;
  codigo_gestor?: string | null;
  nome_gestor?: string | null;
}

/** RetornoMatch no formato da API BuscarBancoTalentosComPromptMatch (snake_case). */
export interface RetornoMatchPromptRaw {
  codigo_interno_colaborador?: string;
  nome?: string;
  orgs?: number[];
  match?: number;
  score_candidato?: number;
  score_vaga?: number;
  detalhamento_calculo?: Record<string, { score_bruto_categoria?: number; score_bruto_obrigatorio?: number; score_bruto_desejavel?: number }>;
  comparativo_por_skill?: Record<string, Array<{
    skill_requisitada?: string;
    nivel_requerido?: string;
    obrigatoriedade?: string;
    skill_do_candidato?: string;
    nivel_do_candidato?: string;
    pontuacao_da_skill?: number;
  }>>;
  [key: string]: unknown;
}

/** Item de colaborador retornado por BuscarBancoTalentosComPromptMatch (snake_case). */
export interface ColaboradorMatchPromptItem {
  origem?: string;
  nome?: string;
  codigo_interno_colaborador?: string;
  possui_candidatura?: boolean;
  candidaturas?: CandidaturaPromptMatchItem[] | null;
  match?: number;
  retornoMatch?: RetornoMatchPromptRaw | null;
  organizacoes?: Array<{ orgId?: number; orgDescricao?: string; ativoNaOrg?: boolean }>;
}

/** Resposta da API BuscarBancoTalentosComPromptMatch. */
export interface BuscarBancoTalentosComPromptMatchResponse {
  colaboradores?: ColaboradorMatchPromptItem[];
  prompt?: unknown;
  /** Id da chamada para auditoria; exibir ao preferir Motor Atual. */
  idLogRankCandidatesIds?: string;
}

/** Item retornado por BuscarPessoasCadastradasPorOrg (Candidaturas FMU). */
export interface PessoaCadastradaPorOrgApi {
  nome: string;
  codigoInternoColaborador: string | null;
  nomeCadastrante: string | null;
  dataDoCadastro: string;
  emailUsuario: string;
}

/** Item retornado por BuscarMeusLotes */
export interface LoteItem {
  id: string;
  dataCriacao?: string;
  dataInicioProcessamento?: string;
  processado?: boolean;
  totalItens?: number;
  quantidadeProcessada?: number;
  quantidadeAProcessar?: number;
  identificadorfila?: string;
}

/** Item de erro retornado em BuscarInformacoesLote (erros é array de objetos) */
export interface ErroLoteItem {
  mensagemErro?: string;
  nomeArquivoCv?: string;
  codigoInternoColaborador?: string | null;
  idLote?: string;
  [key: string]: unknown;
}

/** Pessoa cadastrada no lote (item de pessoasCadastradasNesteLote) */
export interface PessoaCadastradaNoLote {
  nome?: string;
  codigoInternoColaborador?: string;
  nomeCadastrante?: string | null;
  dataDoCadastro?: string;
  possuiCandidatura?: boolean;
  candidaturas?: unknown[] | null;
  match?: number;
}

/** Detalhes do lote (retorno de BuscarInformacoesLote) */
export interface DetalhesLote {
  lote?: LoteItem & Record<string, unknown>;
  pessoasCadastradasNesteLote?: (PessoaCadastradaNoLote | null)[];
  /** API retorna array de objetos com mensagemErro, nomeArquivoCv, etc. */
  erros?: (string | ErroLoteItem)[];
  /** API retorna array de strings (nomes de arquivo) ou array de objetos */
  pdfsAProcessar?: (string | { nomeArquivo?: string; nomeArquivoCv?: string; [key: string]: unknown })[];
}

/** Item retornado por BuscarPessoasCadastradasPorOrg (Candidaturas FMU). */
export interface PessoaCadastradaPorOrgApi {
  nome: string;
  codigoInternoColaborador: string | null;
  nomeCadastrante: string | null;
  dataDoCadastro: string;
  emailUsuario: string;
}

@injectable()
export class ColaboradorBancoDeTalentosApi {
  /**
   * Gera match a partir de prompt (IA). Retorna colaboradores com % de match.
   * POST /api/Colaborador/BancoDeTalentos/BuscarBancoTalentosComPromptMatch?limite=20
   * Body: { texto_vaga: string }
   */
  async buscarBancoTalentosComPromptMatch(
    token: string,
    params: { texto_vaga: string; limite?: number }
  ): Promise<BuscarBancoTalentosComPromptMatchResponse> {
    const { texto_vaga, limite = LIMITES_BANCO_TALENTOS.promptMatch } = params;
    const searchParams = new URLSearchParams({ limite: String(limite) });
    const data = await httpClient.post<BuscarBancoTalentosComPromptMatchResponse>(
      `/api/Colaborador/BancoDeTalentos/BuscarBancoTalentosComPromptMatch?${searchParams.toString()}`,
      { texto_vaga: texto_vaga.trim() },
      { token }
    );
    return data ?? { colaboradores: [] };
  }

  /**
   * Busca talentos do banco de talentos (org) para inscrição em vaga.
   * GET /api/Colaborador/BancoDeTalentos/BuscarBancoTalentos?busca=&cursor=0&limite=1000
   */
  async buscarBancoTalentos(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number } = {}
  ): Promise<BancoTalentosItem[]> {
    const { busca = '', cursor = 0, limite = LIMITES_BANCO_TALENTOS.listagem } = params;
    const searchParams = new URLSearchParams({
      busca: String(busca),
      cursor: String(cursor),
      limite: String(limite),
    });
    const data = await httpClient.get<BancoTalentosItem[]>(
      `/api/Colaborador/BancoDeTalentos/BuscarBancoTalentos?${searchParams.toString()}`,
      { token }
    );
    return Array.isArray(data) ? data : [];
  }

  async buscarPessoasCadastradasPorColaborador(
    token: string,
    codColaborador: string,
    params: { busca?: string; cursor?: number; limite?: number } = {}
  ): Promise<PessoaCadastradaPorColaborador[]> {
    const { busca = '', cursor = 0, limite = LIMITES_BANCO_TALENTOS.listagem } = params;
    const searchParams = new URLSearchParams({
      busca: String(busca),
      cursor: String(cursor),
      limite: String(limite),
      codColaborador,
    });
    const data = await httpClient.get<PessoaCadastradaPorColaborador[]>(
      `/api/Colaborador/BancoDeTalentos/BuscarPessoasCadastradasPorColaborador?${searchParams.toString()}`,
      { token }
    );
    return Array.isArray(data) ? data : [];
  }

  /**
   * Busca pessoas cadastradas por organização (Candidaturas FMU).
   * POST /api/Colaborador/BancoDeTalentos/BuscarPessoasCadastradasPorOrg
   */
  async buscarPessoasCadastradasPorOrg(
    token: string,
    params: {
      busca?: string;
      cursor?: number;
      limite?: number;
      statusVaga?: string;
      statusCandidatura?: number;
      dataInicio?: string;
      dataFim?: string;
    } = {}
  ): Promise<PessoaCadastradaPorOrgApi[]> {
    const {
      busca = '',
      cursor = 0,
      limite = LIMITES_BANCO_TALENTOS.listagem,
      statusVaga,
      statusCandidatura,
      dataInicio,
      dataFim,
    } = params;
    const body = {
      cursor,
      limite,
      busca: String(busca),
      ...(statusVaga != null && statusVaga !== '' && { statusVaga }),
      ...(statusCandidatura != null && { statusCandidatura }),
      ...(dataInicio != null && dataInicio !== '' && { dataInicio }),
      ...(dataFim != null && dataFim !== '' && { dataFim }),
    };
    const data = await httpClient.post<PessoaCadastradaPorOrgApi[] | { retorno?: PessoaCadastradaPorOrgApi[] }>(
      '/api/Colaborador/BancoDeTalentos/BuscarPessoasCadastradasPorOrg',
      body,
      { token }
    );
    if (Array.isArray(data)) return data;
    const ret = (data as { retorno?: PessoaCadastradaPorOrgApi[] })?.retorno;
    return Array.isArray(ret) ? ret : [];
  }

  /**
   * Busca os lotes de importação do colaborador (minhas importações).
   */
  async buscarMeusLotes(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number } = {}
  ): Promise<LoteItem[]> {
    const { busca = '', cursor = 0, limite = LIMITES_BANCO_TALENTOS.meusLotes } = params;
    const searchParams = new URLSearchParams({
      busca: String(busca),
      cursor: String(cursor),
      limite: String(limite),
    });
    const data = await httpClient.get<LoteItem[]>(
      `/api/Colaborador/BancoDeTalentos/BuscarMeusLotes?${searchParams.toString()}`,
      { token }
    );
    return Array.isArray(data) ? data : [];
  }

  /**
   * Busca detalhes de um lote (pessoas cadastradas, erros, PDFs a processar).
   */
  async buscarInformacoesLote(
    token: string,
    idLote: string,
    params: { busca?: string; cursor?: string | null; limite?: string | null } = {}
  ): Promise<DetalhesLote> {
    const { busca = '', cursor = 'null', limite = 'null' } = params;
    const searchParams = new URLSearchParams({
      busca: String(busca),
      cursor: String(cursor),
      limite: String(limite),
      idLote,
    });
    const data = await httpClient.get<{ retorno?: DetalhesLote }>(
      `/api/Colaborador/BancoDeTalentos/BuscarInformacoesLote?${searchParams.toString()}`,
      { token }
    );
    return data?.retorno ?? {};
  }
}
