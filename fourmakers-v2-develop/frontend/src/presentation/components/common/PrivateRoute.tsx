import { Navigate, Outlet } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';

export const PrivateRoute = () => {
  const token = useAppSelector((state) => state.auth.token);

  // Se não há token, redireciona para a página de login
  if (!token) {
    return <Navigate to="/login" replace />;
  }

  // Se há token, renderiza a rota filha
  return <Outlet />;
};
