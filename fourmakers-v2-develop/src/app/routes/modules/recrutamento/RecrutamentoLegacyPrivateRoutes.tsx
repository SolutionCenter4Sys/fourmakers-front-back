/**
 * Helper para redirect /gestaodevagas/* → /recrutamento/*.
 * As rotas legadas em si precisam ser <Route> diretos em AppRoutes (exigência do React Router).
 */
import { Navigate, useLocation } from 'react-router-dom'

export function RedirectGestaodevagasToRecrutamento() {
  const location = useLocation()
  const to = `/recrutamento${location.pathname.replace(/^\/gestaodevagas/, '')}${location.search}${location.hash}`
  return <Navigate to={to} replace />
}
