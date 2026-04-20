import { useSSO } from '@presentation/hooks/useSSO';
import { SSOLayout } from '@presentation/components/sso/SSOLayout';

/**
 * Componente de login SSO específico para a Numen
 * Rota: /login2
 * 
 * Diferença do componente SSO padrão:
 * - Possui lógica customizada para obter o orgId
 * - Pode ter configurações específicas da Numen
 */
export function LoginNumen() {
  console.log('[LoginNumen Component] Renderizando - Rota: /login2');

  const { loginStatus, errorMessage } = useSSO({
    getOrgId: (params) => {
      console.log('[LoginNumen] getOrgId customizado chamado');
      
      // Lógica customizada para Numen
      // Primeiro tenta obter do searchParams 'from'
      const fromPath = params.get('from');
      
      // Aqui você pode adicionar lógica específica da Numen
      // Por exemplo, extrair orgId de um parâmetro específico
      // ou usar um orgId fixo para a Numen
      
      // Por enquanto, mantém compatibilidade com a lógica antiga
      if (fromPath) {
        console.log('[LoginNumen] fromPath encontrado:', fromPath);
        // Se necessário, adicione lógica específica aqui
        // Por exemplo: if (fromPath.includes('numen')) return NUMEN_ORG_ID;
      }
      
      // Fallback para localStorage
      const orgIdStr = localStorage.getItem('ssoOrgId');
      const orgId = orgIdStr ? parseInt(orgIdStr, 10) : null;
      console.log('[LoginNumen] orgId obtido:', orgId);
      return orgId;
    }
  });

  return <SSOLayout loginStatus={loginStatus} errorMessage={errorMessage} />;
}

