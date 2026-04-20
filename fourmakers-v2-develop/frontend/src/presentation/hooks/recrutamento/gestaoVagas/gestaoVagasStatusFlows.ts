/**
 * Configuração dos fluxos de mudança de status por transição (statusOrigem → statusDestino).
 * Permite escalar para novas transições (ex.: 2→3, 3→4) com modais e validações diferentes.
 */
export type StatusFlowType = 'duplicar_movimentacao' | 'movimentacao_simples';

export interface StatusFlowConfig {
  /** Identificador do fluxo (qual modal/etapas usar). */
  type: StatusFlowType;
  /** Código numérico do status de destino na API (ex.: 2). */
  codigoStatusDestino: number;
  /** Se exige modal de duplicação antes do modal de movimentação. */
  exigeModalDuplicacao?: boolean;
  /** Regras de validação específicas (para uso futuro). */
  validacao?: Record<string, unknown>;
}

const FLOWS: Record<string, StatusFlowConfig> = {
  '1->2': {
    type: 'duplicar_movimentacao',
    codigoStatusDestino: 2,
    exigeModalDuplicacao: true,
  },
  // Exemplo para futuras transições:
  // '2->3': { type: 'movimentacao_simples', codigoStatusDestino: 3, exigeModalDuplicacao: false },
  // '3->4': { type: 'movimentacao_simples', codigoStatusDestino: 4, exigeModalDuplicacao: false },
};

/**
 * Retorna a configuração do fluxo para a transição statusFrom → statusTo, ou undefined se não houver fluxo definido.
 */
export function getStatusFlowConfig(statusFrom: string, statusTo: string): StatusFlowConfig | undefined {
  const key = `${statusFrom}->${statusTo}`;
  return FLOWS[key];
}
