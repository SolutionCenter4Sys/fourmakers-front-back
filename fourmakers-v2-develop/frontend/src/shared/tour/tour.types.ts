/**
 * Tipos compartilhados para o sistema de Guide Tour.
 *
 * Os steps definidos aqui são consumidos por dois lugares:
 *  1. Componentes React Joyride  → exibem o tour interativo na UI
 *  2. Testes E2E (Playwright)    → reutilizam testId + action para automação
 *
 * Nunca usar texto visível (labels) como alvo; sempre usar data-testid.
 */

/** Ações que o QA (ou o usuário guiado) executa neste step */
export type TourStepAction =
  | { type: 'observe' }
  | { type: 'click' }
  | { type: 'fill'; value: string }
  | { type: 'select'; value: string };

/** Posicionamento do balão do tour */
export type TourPlacement =
  | 'top'
  | 'top-start'
  | 'top-end'
  | 'bottom'
  | 'bottom-start'
  | 'bottom-end'
  | 'left'
  | 'left-start'
  | 'left-end'
  | 'right'
  | 'right-start'
  | 'right-end'
  | 'center'
  | 'auto';

export interface TourStep {
  /**
   * data-testid do elemento alvo (sem os colchetes).
   * Pode conter {id} para steps dinâmicos — nesses casos, no teste E2E
   * usar locator('[data-testid^="..."]').first() para pegar o primeiro da lista.
   */
  testId: string;
  /** Título do balão (tour UI) */
  title?: string;
  /** Conteúdo do balão (tour UI e documentação do cenário de teste) */
  content: string;
  /** Posicionamento preferido do balão */
  placement?: TourPlacement;
  /** Ação que o QA executaria neste step para o teste automatizado */
  action?: TourStepAction;
  /**
   * data-testid de um elemento que deve estar visível APÓS a ação.
   * Usado em testes para garantir que a UI reagiu corretamente.
   */
  waitFor?: string;
  /** Se true, o step é ignorado nos testes E2E automatizados (ex.: steps meramente explicativos) */
  skipInTests?: boolean;
}
