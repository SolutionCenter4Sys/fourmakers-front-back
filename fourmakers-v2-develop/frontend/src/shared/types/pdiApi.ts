/**
 * DTOs da API Gestão de Pessoa - PDIs (MeusPdis e PdisDoTime).
 * Base: /api/GestaoPessoa/Pdi | Autorização: JWT obrigatório.
 */

/** Valores de status enviados/recebidos pela API (enum do backend) */
export const PdiStatusApi = {
  NOT_STARTED: 'NOT_STARTED',
  IN_ANALYSIS: 'IN_ANALYSIS',
  IN_PROGRESS: 'IN_PROGRESS',
  COMPLETED: 'COMPLETED',
  CANCELLED: 'CANCELLED',
} as const;

/**
 * Enum de status do PDI para exibição (label em português).
 * API envia: NOT_STARTED | IN_ANALYSIS | IN_PROGRESS | COMPLETED | CANCELLED
 */
export const PdiStatusLabel = {
  NAO_INICIADO: 'Não iniciado',
  EM_ANALISE: 'Em análise',
  IN_PROGRESS: 'Em andamento',
  FINALIZADO: 'Finalizado',
  CANCELADO: 'Cancelado',
} as const;

export type PdiStatusKey = keyof typeof PdiStatusLabel;

/** Mapeia o status retornado pela API (NOT_STARTED, IN_ANALYSIS, etc.) para o label em português */
export function mapPdiStatusToLabel(status: string): string {
  const raw = (status || '').trim();
  const s = raw.toLowerCase();
  // API enum (uppercase)
  if (raw === 'NOT_STARTED') return PdiStatusLabel.NAO_INICIADO;
  if (raw === 'IN_ANALYSIS') return PdiStatusLabel.EM_ANALISE;
  if (raw === 'IN_PROGRESS') return PdiStatusLabel.IN_PROGRESS;
  if (raw === 'COMPLETED') return PdiStatusLabel.FINALIZADO;
  if (raw === 'CANCELLED') return PdiStatusLabel.CANCELADO;
  // Valores em minúsculo / snake_case
  if (s === 'in_progress' || s === 'in progress') return PdiStatusLabel.IN_PROGRESS;
  if (s === 'completed' || s === 'finalizado' || s === 'concluido') return PdiStatusLabel.FINALIZADO;
  if (s === 'not_started' || s === 'nao_iniciado' || s === 'no iniciado') return PdiStatusLabel.NAO_INICIADO;
  if (s === 'in_analysis' || s === 'em_analise' || s === 'em análise') return PdiStatusLabel.EM_ANALISE;
  if (s === 'cancelled' || s === 'cancelado' || s === 'canceled') return PdiStatusLabel.CANCELADO;
  // Fallback por texto
  if (s.includes('não') || s.includes('nao') || s.includes('iniciado')) return PdiStatusLabel.NAO_INICIADO;
  if (s.includes('análise') || s.includes('analise')) return PdiStatusLabel.EM_ANALISE;
  if (s.includes('andamento')) return PdiStatusLabel.IN_PROGRESS;
  if (s.includes('finaliz') || s.includes('concluíd')) return PdiStatusLabel.FINALIZADO;
  if (s.includes('cancel')) return PdiStatusLabel.CANCELADO;
  return status || '-';
}

/** Label para exibição: quando andamento é 0%, exibe "Não iniciado" independente do status da API */
export function getPdiStatusDisplayLabel(status: string, progress?: number): string {
  const percent = progress != null ? progress * 100 : 0;
  const label = mapPdiStatusToLabel(status);
  // Ajuste: quando a API marca IN_PROGRESS mas o progresso ainda é 0%, exibir "Não iniciado".
  // Para EM_ANALISE (0%), manter "Em análise" para fluxo de aprovação.
  if (percent === 0 && label === PdiStatusLabel.IN_PROGRESS) return PdiStatusLabel.NAO_INICIADO;
  return label;
}

export interface PdiSkillDTO {
  id: string;
  nomeSkill: string;
  codigoSkill: string;
}

export interface PdiActionPlanDTO {
  id: string;
  description: string;
  deadline: string | null;
  concluidoEm: string | null;
  codigoInternoColaboradorCriacao?: string | null;
  codigoInternoColaboradorAlteracao?: string | null;
}

/** Resumo de evidência (ex.: em PdiCompletoTimeDTO) */
export interface PdiEvidenciaResumoDTO {
  id: string;
  docName: string;
  docPath: string;
  tipo: string;
}

/** Item da listagem de evidências - GET MeusPdis/{pdiId}/evidencias */
export interface PdiEvidenciaDTO {
  id: string;
  pdiId: string;
  docName: string;
  docPath: string;
  docMime: string;
  docSize: number;
  tipo: string;
  createdAt: string;
}

/** Listagem "meus PDIs" e listagem por colaborador do time (GET MeusPdis | GET PdisDoTime/colaborador/{id}). evidencias opcional quando a API retorna na listagem. deadLine previsão de conclusão do PDI (ex.: 2026-03-25T00:00:00). */
export interface PdiResumoDTO {
  id: string;
  colaboradorId: string;
  titulo: string;
  descricao: string;
  status: string;
  dataCriacao: string;
  dataAtualizacao: string | null;
  codigoInternoColaboradorCriacao?: string | null;
  codigoInternoColaboradorAlteracao?: string | null;
  progress: number;
  skills: PdiSkillDTO[];
  actionPlans: PdiActionPlanDTO[];
  /** Quando a API inclui na listagem: indica se o PDI possui evidência/anexo */
  evidencias?: PdiEvidenciaResumoDTO[];
  /** Previsão de conclusão do PDI (ex.: 2026-03-25T00:00:00). Usado na coluna Previsão da tabela. */
  deadLine?: string | null;
}

/** Listagem PDIs do time - GET /api/GestaoPessoa/Pdi/PdisDoTime (item da lista). previsaoConclusao é opcional (preenchido quando a lista vem de getPdisTimeByColaborador com actionPlans). temAnexo preenchido quando a API retorna evidencias na listagem. */
export interface PdiResumoTimeDTO {
  colaboradorId: string;
  nomeColaborador?: string;
  pdiId: string;
  titulo: string;
  status: string;
  progress: number;
  previsaoConclusao?: string | null;
  /** true quando o PDI possui pelo menos uma evidência/anexo (preenchido a partir de evidencias da API quando disponível) */
  temAnexo?: boolean;
  /** Código do colaborador que criou o PDI (para exibir "Criado por: Gestor" vs "Colaborador") */
  codigoInternoColaboradorCriacao?: string | null;
}

/** Resposta paginada GET /api/GestaoPessoa/Pdi/PdisDoTime */
export interface PdiListagemTimeResult {
  items: PdiResumoTimeDTO[];
  totalCount: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
}

/** Detalhe PDI do time - GET /api/GestaoPessoa/Pdi/PdisDoTime/{pdiId} */
export interface PdiCompletoTimeDTO {
  id: string;
  colaboradorId: string;
  titulo: string;
  descricao: string;
  status: string;
  progress: number;
  skills: PdiSkillDTO[];
  actionPlans: PdiActionPlanDTO[];
  evidencias: PdiEvidenciaResumoDTO[];
  /** Código do colaborador que criou o PDI (permite saber se o gestor atual criou) */
  codigoInternoColaboradorCriacao?: string | null;
  /** Previsão de conclusão do PDI (ex.: 2026-03-25T00:00:00). Usado para limite dos prazos dos planos e exibição no card. */
  deadLine?: string | null;
}

/** Input para adicionar plano de ação */
export interface PdiActionPlanInputDTO {
  description: string;
  deadline?: string;
}

/** Resposta concluir action plan */
export interface PdiConcluirActionPlanResponseDTO {
  id: string;
  concluidoEm: string;
  progress: number;
  status: string;
}

/** Resposta upload evidência */
export interface PdiEvidenciaUploadResultDTO {
  id: string;
  pdiId: string;
  docName: string;
  docPath: string;
  docMime: string;
  docSize: number;
  tipo: string;
  createdAt: string;
}

/** Input skill para criar PDI */
export interface PdiSkillInputDTO {
  nomeSkill: string;
  codigoSkill: string;
}

/** Request criar PDI - POST /api/GestaoPessoa/Pdi/MeusPdis. deadLine: data no formato datetime (.NET) com hora/minuto/segundo zerados (ex.: yyyy-MM-ddT00:00:00.000Z). */
export interface PdiCriarRequestDTO {
  titulo: string;
  descricao: string;
  skills?: PdiSkillInputDTO[];
  actionPlans?: PdiActionPlanInputDTO[];
  /** Data de previsão para conclusão do PDI em formato datetime com tempo zerado (ex.: yyyy-MM-ddT00:00:00.000Z) para compatibilidade com .NET. */
  deadLine?: string;
  /** Código do colaborador que está criando (gestor). Enviado quando o gestor cria PDI para outro; backend pode usar para persistir codigoInternoColaboradorCriacao. */
  codigoInternoColaboradorCriacao?: string;
}

/** Response criar PDI */
export interface PdiCriarResponseDTO {
  id: string;
  status: string;
  dataCriacao: string;
}

/** Request atualizar PDI - PUT /api/GestaoPessoa/Pdi/MeusPdis/{id} */
export interface PdiAtualizarRequestDTO {
  titulo: string;
  descricao: string;
  status: string;
}

/** Response atualizar PDI */
export interface PdiAtualizarResponseDTO {
  id: string;
  dataAtualizacao: string | null;
}

/** Response PATCH Gestor: aprovar PDI — id, status ("IN_PROGRESS"), aprovadoEm */
export interface PdiAprovarResponseDTO {
  id: string;
  status: string;
  aprovadoEm: string;
}

export interface ApiGenericResult<T> {
  sucesso: boolean;
  mensagem: string | null;
  erros: string[] | null;
  retorno: T;
}

/** Métricas PDI - GET /api/GestaoPessoa/Pdi/Metricas (e /gestor, /gestor/colaborador/{id}) */
export interface PdiMetricasBigNumbersDTO {
  naoIniciado: number;
  emAnalise: number;
  emAndamento: number;
  finalizados: number;
  cancelados: number;
}

export interface PdiMetricaItemDTO {
  pdiId: string;
  colaboradorId: string;
  nomeColaborador?: string;
  titulo: string;
  status: string;
  progress: number;
  previsao: string | null;
}

export interface PdiMetricasResultDTO {
  bigNumbers: PdiMetricasBigNumbersDTO;
  ativos: PdiMetricaItemDTO[];
  historicos: PdiMetricaItemDTO[];
}
