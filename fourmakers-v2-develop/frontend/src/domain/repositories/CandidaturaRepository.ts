import type { CandidaturaDetails, CandidaturaEditPayload } from '@domain/entities/CandidaturaDetails';
import type {
  AtualizarCandidaturaPayload,
  CandidaturaColaboradorItem,
  DeclinarCandidaturaPayload,
  InserirArquivoCandidaturaParams,
  InserirComentarioCandidaturaPayload,
  MeuTalentoItem,
  MotivoDeclinio,
  MotivoReprovacao,
  ReprovarCandidaturaPayload,
} from '@domain/entities/GestaoVagasCandidatos';

/** Resposta ao salvar candidatura (mensagem da API). */
export interface SaveCandidaturaResult {
  mensagem?: string;
  sucesso?: boolean;
}

export interface CandidaturaRepository {
  getCandidaturaDetails(token: string, candidateId: string, candidaturaId?: string): Promise<CandidaturaDetails>;
  saveCandidaturaDetails(token: string, payload: CandidaturaEditPayload): Promise<SaveCandidaturaResult>;

  atualizarCandidatura(token: string, payload: AtualizarCandidaturaPayload): Promise<SaveCandidaturaResult>;
  listarMotivosReprovacao(token: string): Promise<MotivoReprovacao[]>;
  reprovarCandidatura(token: string, payload: ReprovarCandidaturaPayload): Promise<SaveCandidaturaResult>;
  listarMotivosDeclinio(token: string): Promise<MotivoDeclinio[]>;
  declinarCandidato(token: string, payload: DeclinarCandidaturaPayload): Promise<SaveCandidaturaResult>;
  atualizarRecrutadorResponsavel(
    token: string,
    idCandidatura: string,
    codigoRecrutadorResponsavel: string
  ): Promise<SaveCandidaturaResult>;
  inserirArquivo(token: string, params: InserirArquivoCandidaturaParams): Promise<SaveCandidaturaResult>;
  inserirComentarioCandidatura(token: string, payload: InserirComentarioCandidaturaPayload): Promise<SaveCandidaturaResult>;
  listarMeusTalentos(
    token: string,
    params?: { busca?: string; cursor?: number; limite?: number }
  ): Promise<MeuTalentoItem[]>;

  buscarCandidaturasColaborador(token: string, codColaborador: string): Promise<CandidaturaColaboradorItem[]>;
}
