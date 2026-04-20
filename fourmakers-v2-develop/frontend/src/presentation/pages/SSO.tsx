import { useSSO } from '@presentation/hooks/useSSO';
import { SSOLayout } from '@presentation/components/sso/SSOLayout';

/**
 * Componente de login SSO padrão
 * Rota: /sso
 * 
 * Processa o callback do Azure AD e realiza a autenticação
 */
export function SSO() {
  console.log('[SSO Component] Renderizando - Rota: /sso');
  
  const { loginStatus, errorMessage } = useSSO();

  return <SSOLayout loginStatus={loginStatus} errorMessage={errorMessage} />;
}
