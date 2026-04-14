import { Navigate } from 'react-router-dom'
import { useAppSelector } from '@app/store/hooks'

export const RootRedirect = () => {
  const token = useAppSelector((state) => state.auth.token)

  // Se há token, redireciona para dashboard, senão para login
  if (token) {
    return <Navigate to="/dashboard" replace />
  }
  
  return <Navigate to="/login" replace />
}

