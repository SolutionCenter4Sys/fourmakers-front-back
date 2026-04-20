import { useSSO } from '@presentation/hooks/useSSO';
import { SSOLayout } from '@presentation/components/sso/SSOLayout';
import { getOrgIdFromLegacySsoRedirectPath } from '@shared/constants/orgConfig';

const FMU_ORG_ID = 7;

/**
 * Componente de login SSO específico para a FMU (org 7)
 * Rota: /loginfmu
 *
 * Após o auth SSO da FMU, o redirect_uri aponta para /loginfmu.
 * O orgId 7 é usado para processar o token e redirecionar ao dashboard.
 */
export function LoginFMU() {
  const { loginStatus, errorMessage } = useSSO({
    getOrgId: (params) => {
      const fromPath = params.get('from');
      if (fromPath) {
        const orgIdFromPath = getOrgIdFromLegacySsoRedirectPath(fromPath);
        if (orgIdFromPath != null) return orgIdFromPath;
      }
      const orgIdStr = localStorage.getItem('ssoOrgId');
      const orgId = orgIdStr ? parseInt(orgIdStr, 10) : null;
      return orgId ?? FMU_ORG_ID;
    },
  });

  return <SSOLayout loginStatus={loginStatus} errorMessage={errorMessage} />;
}
