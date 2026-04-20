import { logEvent, setUserId, setUserProperties } from 'firebase/analytics'
import type { Analytics } from 'firebase/analytics'
import { analytics } from '@core/config/firebase'
import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'

/**
 * Obtém o trace ID atual do projeto (se houver)
 */
const getTraceId = (): string | null => {
  if (typeof window === 'undefined' || typeof sessionStorage === 'undefined') {
    return null
  }

  try {
    return sessionStorage.getItem('FRONTEND_TRACE_ID')
  } catch {
    return null
  }
}

/**
 * Obtém a instância do Analytics já inicializada
 */
const getAnalyticsInstance = (): Analytics | null => {
  if (!analytics) {
    console.warn('[Analytics] Analytics não foi inicializado. Verifique se VITE_FIREBASE_MEASUREMENT_ID está configurado.')
    return null
  }
  
  return analytics
}

/**
 * Registra um evento de login no Firebase Analytics
 * @param method Método de login utilizado (ex: 'email', 'sso', 'token')
 * @param user Perfil do usuário (opcional)
 */
export const logLoginEvent = (method: string, user?: ShowmeUserProfile | null): void => {
  const analyticsInstance = getAnalyticsInstance()
  
  if (!analyticsInstance) {
    console.warn('[Analytics] Analytics não disponível, evento de login não registrado')
    return
  }

  try {
    // Define o ID do usuário se disponível
    if (user?.cpf) {
      setUserId(analyticsInstance, user.cpf)
    }

    // Define propriedades do usuário
    if (user) {
      setUserProperties(analyticsInstance, {
        user_email: user.email || undefined,
        user_name: user.nomeColaborador || undefined,
        org_id: user.colaboradorOrg?.orgId?.toString() || undefined,
        org_name: user.colaboradorOrg?.diretoria || undefined,
      })
    }

    // Obtém o trace ID se disponível
    const traceId = getTraceId()

    // Registra o evento de login
    logEvent(analyticsInstance, 'login', {
      method,
      timestamp: new Date().toISOString(),
      ...(traceId && { trace_id: traceId }),
    })

    console.log('[Analytics] Evento de login registrado:', { method, userId: user?.cpf })
  } catch (error) {
    console.error('[Analytics] Erro ao registrar evento de login:', error)
  }
}

/**
 * Registra um evento de logout no Firebase Analytics
 * @param user Perfil do usuário (opcional)
 */
export const logLogoutEvent = (user?: ShowmeUserProfile | null): void => {
  const analyticsInstance = getAnalyticsInstance()
  
  if (!analyticsInstance) {
    console.warn('[Analytics] Analytics não disponível, evento de logout não registrado')
    return
  }

  try {
    // Obtém o trace ID se disponível
    const traceId = getTraceId()

    // Registra o evento de logout
    logEvent(analyticsInstance, 'logout', {
      timestamp: new Date().toISOString(),
      user_id: user?.cpf || undefined,
      ...(traceId && { trace_id: traceId }),
    })

    // Limpa o ID do usuário após logout
    if (user?.cpf) {
      setUserId(analyticsInstance, null)
    }

    console.log('[Analytics] Evento de logout registrado:', { userId: user?.cpf })
  } catch (error) {
    console.error('[Analytics] Erro ao registrar evento de logout:', error)
  }
}

/**
 * Registra uma visualização de página no Firebase Analytics
 * @param pagePath Caminho da página (ex: '/dashboard', '/profile360')
 * @param pageTitle Título da página (opcional)
 * @param user Perfil do usuário (opcional)
 */
export const logPageView = (pagePath: string, pageTitle?: string, user?: ShowmeUserProfile | null): void => {
  const analyticsInstance = getAnalyticsInstance()
  
  if (!analyticsInstance) {
    console.warn('[Analytics] Analytics não disponível, visualização de página não registrada')
    return
  }

  try {
    // Define o ID do usuário se disponível
    if (user?.cpf) {
      setUserId(analyticsInstance, user.cpf)
    }

    // Obtém o trace ID se disponível
    const traceId = getTraceId()

    // Prepara os parâmetros do evento
    const eventParams: Record<string, string> = {
      page_path: pagePath,
      page_location: window.location.href,
      timestamp: new Date().toISOString(),
    }

    // Adiciona título da página se disponível
    if (pageTitle) {
      eventParams.page_title = pageTitle
    }

    // Adiciona trace ID se disponível
    if (traceId) {
      eventParams.trace_id = traceId
    }

    // Adiciona dados do usuário se disponível
    if (user) {
      if (user.cpf) {
        eventParams.user_id = user.cpf
      }
      if (user.email) {
        eventParams.user_email = user.email
      }
      if (user.nomeColaborador) {
        eventParams.user_name = user.nomeColaborador
      }
      if (user.colaboradorOrg?.orgId) {
        eventParams.org_id = user.colaboradorOrg.orgId.toString()
      }
      if (user.colaboradorOrg?.diretoria) {
        eventParams.org_name = user.colaboradorOrg.diretoria
      }
      if (user.colaboradorOrg?.cargo) {
        eventParams.user_role = user.colaboradorOrg.cargo
      }
      if (user.colaboradorOrg?.departamento) {
        eventParams.user_department = user.colaboradorOrg.departamento
      }
    }

    // Registra o evento de visualização de página
    logEvent(analyticsInstance, 'page_view', eventParams)

    console.log('[Analytics] Visualização de página registrada:', { 
      pagePath, 
      pageTitle,
      userId: user?.cpf,
      traceId 
    })
  } catch (error) {
    console.error('[Analytics] Erro ao registrar visualização de página:', error)
  }
}

/**
 * Registra uma ação do usuário na aplicação com contexto completo
 * @param flowIdentifier Identificador do fluxo (ex: 'GestaoColaboradores', 'Reembolso', 'PDI')
 * @param featureName Nome da funcionalidade (ex: 'FormularioNovoColaborador', 'AprovarReembolso')
 * @param additionalData Dados adicionais opcionais (ex: { colaboradorId: '123', acao: 'editar' })
 * @param user Perfil do usuário (opcional - se não fornecido, tentará obter do store)
 * 
 * @example
 * // Exemplo 1: Com usuário do Redux
 * const user = useSelector((state: RootState) => state.auth.user)
 * logUserAction('GestaoColaboradores', 'FormularioNovoColaborador', {}, user)
 * 
 * @example
 * // Exemplo 2: Com dados adicionais
 * logUserAction('Reembolso', 'AprovarReembolso', { 
 *   reembolsoId: '456',
 *   valor: 150.00 
 * }, user)
 * 
 * @example
 * // Exemplo 3: Sem passar usuário (será obtido automaticamente se disponível)
 * logUserAction('PDI', 'VisualizarJornada')
 */
export const logUserAction = (
  flowIdentifier: string,
  featureName: string,
  additionalData?: Record<string, unknown>,
  user?: ShowmeUserProfile | null
): void => {
  const analyticsInstance = getAnalyticsInstance()
  
  if (!analyticsInstance) {
    console.warn('[Analytics] Analytics não disponível, ação do usuário não registrada')
    return
  }

  try {
    // Se o usuário não foi fornecido, tenta obter do store
    let userData = user
    if (!userData) {
      try {
        const storeModule = require('@app/store')
        if (storeModule && storeModule.store) {
          const state = storeModule.store.getState()
          userData = state.auth?.user || null
        }
      } catch (error) {
        console.warn('[Analytics] Não foi possível obter dados do usuário automaticamente')
      }
    }

    // Define o ID do usuário se disponível
    if (userData?.cpf) {
      setUserId(analyticsInstance, userData.cpf)
    }

    // Obtém o trace ID se disponível
    const traceId = getTraceId()

    // Prepara os parâmetros do evento
    const eventParams: Record<string, any> = {
      flow_identifier: flowIdentifier,
      feature_name: featureName,
      page_location: window.location.href,
      timestamp: new Date().toISOString(),
    }

    // Adiciona trace ID se disponível
    if (traceId) {
      eventParams.trace_id = traceId
    }

    // Adiciona dados do usuário se disponível
    if (userData) {
      if (userData.cpf) {
        eventParams.user_id = userData.cpf
      }
      if (userData.email) {
        eventParams.user_email = userData.email
      }
      if (userData.nomeColaborador) {
        eventParams.user_name = userData.nomeColaborador
      }
      if (userData.colaboradorOrg?.orgId) {
        eventParams.org_id = userData.colaboradorOrg.orgId.toString()
      }
      if (userData.colaboradorOrg?.diretoria) {
        eventParams.org_name = userData.colaboradorOrg.diretoria
      }
      if (userData.colaboradorOrg?.cargo) {
        eventParams.user_role = userData.colaboradorOrg.cargo
      }
      if (userData.colaboradorOrg?.departamento) {
        eventParams.user_department = userData.colaboradorOrg.departamento
      }
    }

    // Adiciona dados adicionais se fornecidos
    if (additionalData) {
      Object.entries(additionalData).forEach(([key, value]) => {
        // Converte valores complexos para string
        if (typeof value === 'object' && value !== null) {
          eventParams[key] = JSON.stringify(value)
        } else {
          eventParams[key] = value
        }
      })
    }

    // Registra o evento com nome 'user_action'
    logEvent(analyticsInstance, 'user_action', eventParams)

    console.log('[Analytics] Ação do usuário registrada:', {
      flowIdentifier,
      featureName,
      userId: userData?.cpf,
      traceId,
      additionalData
    })
  } catch (error) {
    console.error('[Analytics] Erro ao registrar ação do usuário:', error)
  }
}

/**
 * Registra um evento customizado no Firebase Analytics
 * @param eventName Nome do evento
 * @param eventParams Parâmetros do evento
 * @deprecated Use logUserAction para ações de usuário com contexto completo
 */
export const logCustomEvent = (eventName: string, eventParams?: Record<string, unknown>): void => {
  const analyticsInstance = getAnalyticsInstance()
  
  if (!analyticsInstance) {
    console.warn('[Analytics] Analytics não disponível, evento customizado não registrado')
    return
  }

  try {
    // Obtém o trace ID se disponível
    const traceId = getTraceId()

    logEvent(analyticsInstance, eventName, {
      ...eventParams,
      timestamp: new Date().toISOString(),
      ...(traceId && { trace_id: traceId }),
    })
    console.log('[Analytics] Evento customizado registrado:', eventName, eventParams)
  } catch (error) {
    console.error('[Analytics] Erro ao registrar evento customizado:', error)
  }
}

