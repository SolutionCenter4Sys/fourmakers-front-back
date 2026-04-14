import Joyride from 'react-joyride';
import type { CallBackProps, Styles } from 'react-joyride';
import type { TourStep } from './tour.types';
import { toJoyrideSteps } from './tour.utils';

interface GuideTourProps {
  steps: TourStep[];
  run: boolean;
  onCallback: (data: CallBackProps) => void;
}

/**
 * Cores hard-coded pois o Joyride usa inline styles — CSS variables (hsl(var(--x)))
 * não são resolvidas neste contexto e resultam em cor inválida.
 */
const tourStyles: Partial<Styles> = {
  options: {
    primaryColor: '#0F172A',       // slate-900 — fundo do botão Next
    textColor: '#1E293B',          // slate-800 — texto principal
    backgroundColor: '#FFFFFF',    // branco puro para o balão
    arrowColor: '#FFFFFF',
    overlayColor: 'rgba(0, 0, 0, 0.65)',
    zIndex: 9999,
    width: 380,
  },
  tooltip: {
    borderRadius: '12px',
    fontSize: '0.875rem',
    padding: '0',
    boxShadow: '0 20px 60px rgba(15, 23, 42, 0.18), 0 4px 16px rgba(15, 23, 42, 0.1)',
    overflow: 'hidden',
  },
  tooltipContainer: {
    textAlign: 'left',
    padding: '0',
  },
  tooltipTitle: {
    fontSize: '0.9375rem',
    fontWeight: 700,
    color: '#0F172A',
    padding: '1.125rem 1.25rem 0.5rem',
    margin: 0,
    borderBottom: '1px solid #F1F5F9',
  },
  tooltipContent: {
    fontSize: '0.875rem',
    color: '#475569',
    padding: '0.875rem 1.25rem 0.25rem',
    lineHeight: '1.6',
  },
  tooltipFooter: {
    padding: '0.75rem 1.25rem 1rem',
    marginTop: '0',
    borderTop: '1px solid #F1F5F9',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  buttonNext: {
    backgroundColor: '#0F172A',
    color: '#FFFFFF',
    borderRadius: '8px',
    fontSize: '0.8125rem',
    fontWeight: 600,
    padding: '0.5rem 1.125rem',
    border: 'none',
    cursor: 'pointer',
    letterSpacing: '0.01em',
  },
  buttonBack: {
    backgroundColor: 'transparent',
    color: '#64748B',
    borderRadius: '8px',
    fontSize: '0.8125rem',
    fontWeight: 500,
    padding: '0.5rem 0.75rem',
    border: '1px solid #E2E8F0',
    cursor: 'pointer',
    marginRight: '0.5rem',
  },
  buttonSkip: {
    backgroundColor: 'transparent',
    color: '#94A3B8',
    fontSize: '0.75rem',
    fontWeight: 400,
    padding: '0.25rem 0',
    border: 'none',
    cursor: 'pointer',
    textDecoration: 'underline',
    textUnderlineOffset: '2px',
  },
  buttonClose: {
    color: '#94A3B8',
    padding: '0.75rem',
    top: 0,
    right: 0,
  },
};

const locale = {
  back: '← Voltar',
  close: 'Fechar',
  last: 'Concluir tour ✓',
  next: 'Próximo →',
  open: 'Abrir',
  skip: 'Pular tour',
};

export function GuideTour({ steps, run, onCallback }: GuideTourProps) {
  const joyrideSteps = toJoyrideSteps(steps);

  return (
    <Joyride
      steps={joyrideSteps}
      run={run}
      callback={onCallback}
      continuous
      showProgress
      showSkipButton
      scrollToFirstStep
      disableOverlayClose
      spotlightPadding={8}
      styles={tourStyles}
      locale={locale}
    />
  );
}
