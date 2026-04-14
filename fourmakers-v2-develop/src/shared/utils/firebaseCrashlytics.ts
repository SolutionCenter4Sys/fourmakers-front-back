import { logEvent, setUserId } from 'firebase/analytics'
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
const getAnalyticsInstance = () => {
  if (!analytics) {
    console.warn('[Crashlytics] Analytics não foi inicializado. Verifique se VITE_FIREBASE_MEASUREMENT_ID está configurado.')
    return null
  }
  
  return analytics
}

/**
 * Registra um erro no Firebase
 * @param error Erro a ser registrado
 * @param context Contexto adicional do erro
 * @param user Perfil do usuário (opcional)
 */
export const logError = (error: Error | unknown, context?: Record<string, unknown>, user?: ShowmeUserProfile | null): void => {
  const analytics = getAnalyticsInstance()
  
  if (!analytics) {
    console.warn('[Crashlytics] Analytics não disponível, erro não registrado')
    console.error('Erro capturado:', error, context)
    return
  }

  try {
    // Define o ID do usuário se disponível
    // Usa try-catch interno para evitar que erros do Analytics quebrem a aplicação
    if (user?.cpf) {
      try {
        setUserId(analytics, user.cpf)
      } catch (setUserIdError) {
        console.warn('[Crashlytics] Erro ao definir userId no Analytics:', setUserIdError)
      }
    }

    const errorMessage = error instanceof Error ? error.message : String(error)
    const errorStack = error instanceof Error ? error.stack : undefined
    const traceId = getTraceId()

    // Prepara os parâmetros do evento
    const eventParams: Record<string, any> = {
      description: errorMessage,
      fatal: false,
      timestamp: new Date().toISOString(),
    }

    // Adiciona trace ID se disponível
    if (traceId) {
      eventParams.trace_id = traceId
    }

    // Adiciona stack trace se disponível
    if (errorStack) {
      eventParams.stack = errorStack
    }

    // Adiciona contexto adicional
    if (context) {
      Object.assign(eventParams, context)
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

    // Registra o erro como evento customizado no Analytics
    // Usa setTimeout para evitar bloqueio da thread principal
    try {
      logEvent(analytics, 'exception', eventParams)
    } catch (analyticsError) {
      // Se houver erro ao registrar no Analytics, apenas loga no console
      // Não deve interromper o fluxo da aplicação
      console.warn('[Crashlytics] Erro ao registrar no Analytics:', analyticsError)
    }

    console.error('[Crashlytics] Erro registrado:', errorMessage, { 
      context,
      userId: user?.cpf,
      traceId 
    })
  } catch (logError) {
    console.error('[Crashlytics] Erro ao registrar erro:', logError)
    console.error('Erro original:', error, context)
  }
}

/**
 * Registra um erro fatal
 * @param error Erro fatal a ser registrado
 * @param context Contexto adicional do erro
 * @param user Perfil do usuário (opcional)
 */
export const logFatalError = (error: Error | unknown, context?: Record<string, unknown>, user?: ShowmeUserProfile | null): void => {
  const analytics = getAnalyticsInstance()
  
  if (!analytics) {
    console.warn('[Crashlytics] Analytics não disponível, erro fatal não registrado')
    console.error('Erro fatal capturado:', error, context)
    return
  }

  try {
    // Define o ID do usuário se disponível
    // Usa try-catch interno para evitar que erros do Analytics quebrem a aplicação
    if (user?.cpf) {
      try {
        setUserId(analytics, user.cpf)
      } catch (setUserIdError) {
        console.warn('[Crashlytics] Erro ao definir userId no Analytics:', setUserIdError)
      }
    }

    const errorMessage = error instanceof Error ? error.message : String(error)
    const errorStack = error instanceof Error ? error.stack : undefined
    const traceId = getTraceId()

    // Prepara os parâmetros do evento
    const eventParams: Record<string, any> = {
      description: errorMessage,
      fatal: true,
      timestamp: new Date().toISOString(),
    }

    // Adiciona trace ID se disponível
    if (traceId) {
      eventParams.trace_id = traceId
    }

    // Adiciona stack trace se disponível
    if (errorStack) {
      eventParams.stack = errorStack
    }

    // Adiciona contexto adicional
    if (context) {
      Object.assign(eventParams, context)
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

    // Registra o erro fatal como evento customizado no Analytics
    // Usa try-catch interno para evitar que erros do Analytics quebrem a aplicação
    try {
      logEvent(analytics, 'exception', eventParams)
    } catch (analyticsError) {
      // Se houver erro ao registrar no Analytics, apenas loga no console
      // Não deve interromper o fluxo da aplicação
      console.warn('[Crashlytics] Erro ao registrar erro fatal no Analytics:', analyticsError)
    }

    console.error('[Crashlytics] Erro fatal registrado:', errorMessage, { 
      context,
      userId: user?.cpf,
      traceId 
    })
  } catch (logError) {
    console.error('[Crashlytics] Erro ao registrar erro fatal:', logError)
    console.error('Erro fatal original:', error, context)
  }
}

