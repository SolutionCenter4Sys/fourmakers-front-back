import { useEffect, useRef, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@app/store/hooks';
import { getAccessToken, fetchShowmeProfile } from '@app/store/slices/authSlice';
import { orgLoginConfigs, getOrgIdFromLegacySsoRedirectPath, getRedirectPathAposSSO } from '@shared/constants/orgConfig';

interface UseSSOOptions {
  /**
   * Função customizada para obter o orgId
   * Se não fornecida, usa a lógica padrão (localStorage + fromPath)
   */
  getOrgId?: (searchParams: URLSearchParams) => number | null;
}

export function useSSO(options?: UseSSOOptions) {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const [searchParams] = useSearchParams();
  const { loginStatus } = useAppSelector((state) => state.auth);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  
  // Flag para garantir que o processamento aconteça apenas uma vez
  const hasProcessed = useRef(false);
  const isProcessing = useRef(false);

  useEffect(() => {
    // Evita múltiplas execuções (React StrictMode ou re-renders)
    if (hasProcessed.current || isProcessing.current) {
      console.log('[useSSO] Execução bloqueada - já processado ou em processamento', {
        hasProcessed: hasProcessed.current,
        isProcessing: isProcessing.current
      });
      return;
    }

    console.log('[useSSO] Iniciando processamento SSO');

    const code = searchParams.get('code');
    
    // Obter orgId: usar função customizada se fornecida, senão usar lógica padrão
    let orgId: number | null = null;
    
    if (options?.getOrgId) {
      orgId = options.getOrgId(searchParams);
    } else {
      // Lógica padrão
      const orgIdStr = localStorage.getItem('ssoOrgId');
      const fromPath = searchParams.get('from');
      const orgIdFromStorage = orgIdStr ? parseInt(orgIdStr, 10) : null;
      const orgIdFromLegacyPath = fromPath ? getOrgIdFromLegacySsoRedirectPath(fromPath) : null;
      orgId = orgIdFromStorage ?? orgIdFromLegacyPath;
    }

    let fallbackLoginPath = "/login";
    if (orgId !== null) {
      const orgConfig = orgLoginConfigs.find(config => config.orgId === orgId);
      if (orgConfig && orgConfig.slugRota !== "default") {
        fallbackLoginPath = `/login/${orgConfig.slugRota}`;
      }
    }

    // Validações de dados necessários
    if (!code) {
      console.log('[useSSO] Erro - Code não encontrado na URL');
      hasProcessed.current = true;
      setErrorMessage("Code não encontrado na URL.");
      setTimeout(() => navigate(fallbackLoginPath), 3000);
      return;
    }

    if (orgId === null || Number.isNaN(orgId)) {
      console.log('[useSSO] Erro - OrgId inválido', { orgId });
      hasProcessed.current = true;
      setErrorMessage("ID da organização não encontrado. Por favor, tente fazer o login novamente.");
      setTimeout(() => navigate(fallbackLoginPath), 3000);
      return;
    }

    console.log('[useSSO] Dados válidos - processando token', { code: code.substring(0, 10) + '...', orgId });

    // Marca como em processamento para evitar chamadas duplicadas
    isProcessing.current = true;

    const handleGetAccessToken = async () => {
      try {
        console.log('[useSSO] Chamando getAccessToken - INÍCIO');
        const result = await dispatch(getAccessToken({ code, orgId }));
        console.log('[useSSO] Resposta getAccessToken recebida', { 
          fulfilled: getAccessToken.fulfilled.match(result),
          hasUser: getAccessToken.fulfilled.match(result) ? !!(result.payload as any)?.usuario : false
        });

        if (getAccessToken.fulfilled.match(result)) {
          // Se o usuário não foi retornado na resposta, buscar o perfil
          if (!result.payload.usuario) {
            console.log('[useSSO] Usuário não retornado - buscando perfil');
            await dispatch(fetchShowmeProfile());
          }
          
          // Limpa dados temporários e redireciona
          localStorage.removeItem('ssoOrgId');
          hasProcessed.current = true;
          const redirectPath = getRedirectPathAposSSO(orgId);
          console.log('[useSSO] Sucesso - redirecionando para', redirectPath);
          navigate(redirectPath);
        } else {
          // Extrai mensagem de erro do resultado
          const errorMsg = result.error?.message || "Falha ao obter token de acesso.";
          console.log('[useSSO] Erro na requisição', { error: errorMsg });
          hasProcessed.current = true;
          setErrorMessage(errorMsg);
          setTimeout(() => navigate(fallbackLoginPath), 3000);
        }
      } catch (error) {
        // Tratamento de erro inesperado
        console.error('[useSSO] Erro inesperado', error);
        hasProcessed.current = true;
        setErrorMessage("Erro inesperado ao processar autenticação.");
        setTimeout(() => navigate(fallbackLoginPath), 3000);
      } finally {
        isProcessing.current = false;
        console.log('[useSSO] Processamento finalizado');
      }
    };

    handleGetAccessToken();
    
    // Dependências mínimas: apenas o que realmente pode mudar entre montagens
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return {
    loginStatus,
    errorMessage,
  };
}

