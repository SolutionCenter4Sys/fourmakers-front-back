export const theme = {
  colors: {
    background: 'var(--color-primary-background)',
    backgroundHover: 'var(--color-primary-soft)',
    surface: 'var(--color-surface-elevated)',
    border: 'var(--color-border-default)',
    primary: 'var(--color-primary)',
    primarySoft: 'var(--color-primary-soft)',
    accentGradient: 'var(--color-background-brand)',
    textPrimary: 'var(--color-primary-text)',
    textSecondary: 'var(--color-secondary-text)',
    textHighlight: 'var(--color-primary-strong)',
    textMuted: 'var(--color-secondary-text)',
  },
  layout: {
    sidebarWidth: '260px',
    headerHeight: '72px',
  },
  typography: {
    fontFamily: "'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Plus Jakarta Sans', sans-serif",
  },
}

export type AppTheme = typeof theme
