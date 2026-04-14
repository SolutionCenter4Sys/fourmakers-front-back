import { usePageTracking } from '@shared/hooks/usePageTracking'

/**
 * Componente que rastreia automaticamente visualizações de página
 * Deve ser colocado dentro do BrowserRouter para funcionar
 */
export const PageTracker = () => {
  usePageTracking()
  return null
}
