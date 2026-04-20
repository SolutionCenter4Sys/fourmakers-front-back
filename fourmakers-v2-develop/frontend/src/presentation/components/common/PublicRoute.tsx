import { Navigate, Outlet } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';

export const PublicRoute = () => {
  const token = useAppSelector((state) => state.auth.token);

  // Se há token, redireciona para a página inicial da área logada
  if (token) {
    return <Navigate to="/dashboard" replace />;
  }

  // Se não há token, renderiza a rota pública
  return <Outlet />;
};
