/**
 * Entidades do Módulo de Admissão Digital.
 * Alinhado à documentação técnica: fluxo pós-aceitação da carta-oferta.
 */

/** Estágios do funil de admissão (Kanban). */
export type AdmissionStageId =
  | 'carta-oferta-aceita'
  | 'coleta-dados'
  | 'validacao-rh'
  | 'contrato-assinatura'
  | 'integracao';

export interface AdmissionStage {
  id: AdmissionStageId;
  nome: string;
  ordem: number;
}

/** Status de um item do checklist de documentos. */
export type DocumentItemStatus = 'Pendente' | 'Em revisão' | 'Aprovado' | 'Reprovado';

/** Tipo de item do checklist. */
export type DocumentItemType = 'upload' | 'data' | 'info';

/** Campo de dado estruturado (extraído por OCR ou manual). */
export interface DocumentField {
  id: string;
  label: string;
  type: 'text' | 'select' | 'date';
  value: string | boolean;
  dataSource?: 'ocr' | 'manual';
}

/** Item do checklist de documentos. */
export interface DocumentItem {
  id: string;
  name: string;
  status: DocumentItemStatus;
  type: DocumentItemType;
  fileUrl?: string;
  fields?: DocumentField[];
  rejectionReason?: string;
}

/** Entrada do log de auditoria. */
export interface AuditLogEntry {
  id: string;
  usuario: string;
  acao: string;
  timestamp: string; // ISO
  detalhe?: string;
}

/** Processo de admissão de um candidato. */
export interface Admission {
  id: string;
  candidateName: string;
  candidatePhotoUrl?: string | null;
  vagaTitulo: string;
  clienteNome: string;
  currentStage: AdmissionStageId;
  admissionStartedAt: string | null; // ISO; null se convite ainda não enviado
  /** Progresso ex.: "Etapa 3/8" */
  progressoEtapa: string;
  /** Tempo desde última ação (ex.: "2 dias") para exibição e SLA */
  tempoInatividade?: string | null;
  /** Horas desde última ação (para indicador SLA: verde/amarelo/vermelho) */
  horasInatividade?: number | null;
  documents: DocumentItem[];
  auditLog: AuditLogEntry[];
}

/** Estágios padrão do Kanban de Admissão. */
export const ADMISSION_STAGES: AdmissionStage[] = [
  { id: 'carta-oferta-aceita', nome: 'Carta-Oferta Aceita', ordem: 1 },
  { id: 'coleta-dados', nome: 'Coleta de Dados', ordem: 2 },
  { id: 'validacao-rh', nome: 'Validação RH', ordem: 3 },
  { id: 'contrato-assinatura', nome: 'Contrato & Assinatura', ordem: 4 },
  { id: 'integracao', nome: 'Integração', ordem: 5 },
];
