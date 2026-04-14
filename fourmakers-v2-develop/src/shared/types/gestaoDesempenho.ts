export interface EstatisticasDesempenho {
  totalColaboradores: number;
  umAUmEmDia: number;
  umAUmAtrasados: number;
  feedbackEmDia: number;
  feedbackAtrasados: number;
  pdisAtivos: number;
}

export interface EstatisticasRH {
  totalColaboradores: number;
  gestoresCom1a1EmDia: number;
  feedbackEmDia: number;
  sem1a1Mais14d: number;
  semFeedbackMais30d: number;
  semPdiAtivo: number;
}

export interface EstatisticasPessoais {
  totalFeedbacks: number;
  totalRegistros1a1: number;
  totalPDIs: number;
}

export interface RegistroCritico {
  id: string;
  colaboradorNome: string;
  colaboradorCod: string;
  data: string;
  descricao: string;
}

export interface ColaboradorDesempenho {
  id: string;
  codColaborador: string;
  codigoColaboradorExterno?: string;
  nome: string;
  cargo: string;
  gestor: string;
  status: 'ativo' | 'ferias' | 'afastado';
  ultimoFeedback?: string;
  ultimaUmAUm?: string;
  pdisAtivos: number;
  temPdi: boolean;
  temFeedback: boolean;
  temUmAUm: boolean;
  dataAniversario?: string; // Formato: "DD/MM"
}

export type FiltroDesempenho = 'todos' | 'sem-pdi' | 'sem-feedback' | 'sem-1-1';

export interface ColaboradorDesempenhoDetalhes {
  id: string;
  codColaborador: string;
  nome: string;
  cargo: string;
  email: string;
  telefone: string;
  dataNascimento: string;
  dataAdmissao: string;
  tempoCasa: string;
  salario: number;
  saldoHoras: number;
  status: 'ativo' | 'ferias' | 'afastado';
  modelo: string;
  /** Código externo (ex.: matrícula) — exibido no badge quando disponível */
  codigoColaboradorExterno?: string | null;
  /** Modalidade de contratação (CLT, PJ, etc.) */
  modalidadeContratacao?: string | null;
  /** Regime de trabalho */
  regimeTrabalho?: string | null;
}

export interface Feedback {
  id: string;
  data: string;
  resumo: string;
  realizadoPor: string;
  visto: boolean;
  vistoEm?: string;
  continuar?: string[];
  comecar?: string[];
  parar?: string[];
  observacoesGerais?: string;
}

export interface RegistroUmAUm {
  id: string;
  data: string;
  resumo: string;
  realizadoPor: string;
  visto: boolean;
  vistoEm?: string;
  critico?: boolean;
  pautaSugerida?: string[];
  anotacoes?: string;
}

export type PDIStatus = 'concluido' | 'em-andamento' | 'pendente';

export interface PDI {
  id: string;
  titulo: string;
  descricao: string;
  status: PDIStatus;
  dataInicio: string;
  dataFim?: string;
  prazo?: string;
  prioridade?: 'alta' | 'media' | 'baixa';
  criadoPor?: string;
}

export interface PautaSugerida {
  id: string;
  descricao: string;
  origem: string;
}
