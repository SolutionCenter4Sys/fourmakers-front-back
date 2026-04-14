/**
 * Carregamento do script de analytics (ContentSquare/Hotjar) na página de candidatura externa.
 * Centralizado em shared para: única fonte de verdade da URL, possível uso de env (VITE_CANDIDATURA_ANALYTICS_SCRIPT_URL)
 * e alinhamento com observabilidade (script complementar ao Firebase Analytics).
 */

const SCRIPT_ID = 'candidatura-analytics-contentsquare'

/** URL padrão do script ContentSquare para heatmaps/session replay na jornada de candidatura. */
export const CANDIDATURA_ANALYTICS_SCRIPT_URL =
  (typeof import.meta !== 'undefined' && import.meta.env?.VITE_CANDIDATURA_ANALYTICS_SCRIPT_URL as string | undefined) ||
  'https://t.contentsquare.net/uxa/e93bd8a52019a.js'

/**
 * Injeta o script de analytics na página. Deve ser chamado apenas na rota de candidatura (/public/vaga).
 * Retorna uma função de cleanup para remover o script ao desmontar.
 */
export function loadCandidaturaAnalyticsScript(): (() => void) | void {
  if (typeof document === 'undefined') return
  if (document.getElementById(SCRIPT_ID)) return

  const script = document.createElement('script')
  script.id = SCRIPT_ID
  script.src = CANDIDATURA_ANALYTICS_SCRIPT_URL
  script.async = true
  document.body.appendChild(script)

  return () => {
    const el = document.getElementById(SCRIPT_ID)
    if (el?.parentNode) el.parentNode.removeChild(el)
  }
}
