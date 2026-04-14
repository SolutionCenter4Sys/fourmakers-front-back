import type { Step } from 'react-joyride';
import type { TourStep } from './tour.types';

/**
 * Converte os TourSteps do projeto para o formato Step do React Joyride.
 * Filtra steps cujo elemento ainda não existe no DOM.
 */
export function toJoyrideSteps(tourSteps: TourStep[]): Step[] {
  return tourSteps.map((step) => ({
    target: `[data-testid="${step.testId}"]`,
    title: step.title,
    content: step.content,
    placement: step.placement ?? 'auto',
    disableBeacon: true,
  }));
}

/**
 * Gera o atributo CSS selector a partir de um testId.
 * Útil para referenciar no Playwright: page.locator(testIdSelector('foo'))
 */
export function testIdSelector(testId: string): string {
  return `[data-testid="${testId}"]`;
}
