import type { VagaDetails } from '@domain/entities/VagaDetails';
import type {
  MudarStatusCandidaturaPayload,
  MudarStatusCandidaturaRetorno,
  StatusCandidatura,
  ObterTotaisInscritosRetorno,
  CandidatoInscritoRaw,
  CandidatoAderenteRaw,
  UnidadeItem,
  TipoVagaItem,
  TipoContratacaoItem,
  CandidatarOutraPessoaPayload,
  MotivoPerdaVagaItem,
  GravarPerdaVagaPayload,
  MudarStatusVagaPayload,
  InserirInformacoesComplementaresPayload,
  OpcaoContatoItem,
  VagaFilhaItem,
  VagaRecrutamentoCompleto,
} from '@domain/entities/GestaoVagasCandidatos';

/** Retorno dos endpoints de relatório de vagas: 200 com blob (xlsx) ou 204 sem dados. API retorna arquivo binário. */
export type RelatorioVagaResult = { status: 200; blob: Blob } | { status: 204 };

/** Parâmetros para listar candidatos inscritos (Kanban). */
export interface ListarCandidatosInscritosParams {
  vagaId: string;
  busca?: string;
  cursor?: number;
  limite?: number;
  dataInicio?: string;
  dataFim?: string;
  qualificados?: boolean;
  diasUltimaAlteracao?: number;
  localizacaoCidade?: string;
  localizacaoEstado?: string;
}

/** Parâmetros para listar candidatos aderentes. */
export interface ListarCandidatosAderentesParams {
  vagaId: string;
  busca?: string;
  cursor?: number;
  limite?: number;
  dataInicio?: string;
  dataFim?: string;
  origens?: string[];
  pesoHardSkills?: number;
  pesoSoftSkills?: number;
  pesoMetodologias?: number;
  pesoDominiosNegocio?: number;
  pesoIdiomas?: number;
  pesoDisponibilidades?: number;
  qualificados?: boolean;
  diasUltimaAlteracao?: number;
  localizacaoCidade?: string;
  localizacaoEstado?: string;
}

export interface VagaRepository {
  getVagaDetails(token: string, vagaId: string): Promise<VagaDetails>;

  /** Detalhes da vaga para portal público (sem token ou token opcional, por código numérico). */
  getVagaDetalhesPublico(codigoVaga: number): Promise<VagaDetails | null>;

  /** Inscrição do usuário logado na vaga (portal público). Requer pretencaoSalarial, modeloTrabalhoId e opcoesContatoIds (lista de IDs string). */
  candidatarSe(
    token: string,
    payload: {
      codigoVaga: number;
      opcoesContatoIds: string[];
      pretencaoSalarial: string;
      modeloTrabalhoId: string;
    }
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }>;

  getVagaRecrutamentoPorId(token: string, vagaId: string): Promise<VagaRecrutamentoCompleto | null>;

  atualizarVagaRecrutamentoPorId(
    token: string,
    body: VagaRecrutamentoCompleto
  ): Promise<{ sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }>;

  mudarStatusCandidatura(
    token: string,
    payload: MudarStatusCandidaturaPayload
  ): Promise<{ retorno?: MudarStatusCandidaturaRetorno; sucesso?: boolean; mensagem?: string | null }>;

  listarCandidatosInscritos(
    token: string,
    params: ListarCandidatosInscritosParams
  ): Promise<{ retorno?: CandidatoInscritoRaw[]; sucesso?: boolean }>;

  listarCandidatosAderentes(
    token: string,
    params: ListarCandidatosAderentesParams
  ): Promise<{ retorno?: CandidatoAderenteRaw[]; sucesso?: boolean }>;

  listarStatusCandidaturaRecrutamento(token: string): Promise<StatusCandidatura[]>;

  obterTotaisInscritos(token: string, vagaId: string): Promise<ObterTotaisInscritosRetorno | null>;

  listarUnidades(token: string): Promise<UnidadeItem[]>;

  listarTiposVaga(token: string): Promise<TipoVagaItem[]>;

  listarTiposContratacao(token: string): Promise<TipoContratacaoItem[]>;

  candidatarOutraPessoa(
    token: string,
    payload: CandidatarOutraPessoaPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string }>;

  adicionarRecrutadorVaga(
    token: string,
    vagaId: string,
    codInternoColaboradorRecrutador: string
  ): Promise<{ retorno?: string; sucesso?: boolean; mensagem?: string | null }>;

  listarMotivosPerdaVaga(token: string): Promise<MotivoPerdaVagaItem[]>;

  gravarPerdaVaga(
    token: string,
    payload: GravarPerdaVagaPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }>;

  mudarStatusVaga(
    token: string,
    payload: MudarStatusVagaPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }>;

  listarOpcoesContato(token: string): Promise<OpcaoContatoItem[]>;

  listarVagasRecrutamentoPorParentEmAndamento(
    token: string,
    vagaIdParent: string
  ): Promise<VagaFilhaItem[]>;

  inserirInformacoesComplementaresVagaRecrutamento(
    token: string,
    payload: InserirInformacoesComplementaresPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }>;

  /** Relatórios xlsx: 200 com blob ou 204 sem dados. API retorna arquivo binário. */
  getRelatorioProdutividade(token: string, dataInicio: string, dataFim: string): Promise<RelatorioVagaResult>;
  getRelatorioVagas(token: string, dataInicio: string, dataFim: string): Promise<RelatorioVagaResult>;
  getRelatorioVagasCandidaturas(token: string, dataInicio: string, dataFim: string): Promise<RelatorioVagaResult>;
}
