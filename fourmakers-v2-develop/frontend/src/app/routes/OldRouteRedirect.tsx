import { Navigate, useLocation } from 'react-router-dom'
import { orgLoginConfigs } from '@shared/constants/orgConfig'

export const OldRouteRedirect = () => {
  const location = useLocation()
  
  // Remove a barra inicial e obtém o caminho da rota
  const currentPath = location.pathname.startsWith('/') 
    ? location.pathname.slice(1) 
    : location.pathname
  
  // Procura uma configuração que tenha rotaAntiga correspondente ao caminho atual
  const configComRotaAntiga = orgLoginConfigs.find(
    config => config.rotaAntiga && config.rotaAntiga === currentPath
  )
  
  if (configComRotaAntiga) {
    console.log(`[OldRouteRedirect] Redirecionando de rotaAntiga "/${currentPath}" para "/login/${configComRotaAntiga.slugRota}"`)
    return <Navigate to={`/login/${configComRotaAntiga.slugRota}`} replace />
  }
  
  // Fallback: se por algum motivo não encontrou a configuração, redireciona para login padrão
  console.warn(`[OldRouteRedirect] Rota antiga "/${currentPath}" não encontrada nas configurações`)
  return <Navigate to="/login" replace />
}

