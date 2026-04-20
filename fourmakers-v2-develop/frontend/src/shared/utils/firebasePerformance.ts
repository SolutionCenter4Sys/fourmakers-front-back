import { 
  trace
} from 'firebase/performance'
import type { 
  PerformanceTrace,
  FirebasePerformance
} from 'firebase/performance'
import { performance } from '@core/config/firebase'

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
 * Obtém a instância do Performance já inicializada
 */
const getPerformanceInstance = (): FirebasePerformance | null => {
  if (!performance) {
    console.warn('[Performance] Performance não foi inicializado.')
    return null
  }
  
  return performance
}

/**
 * Verifica se o Performance está disponível
 */
const isPerformanceAvailable = (): boolean => {
  if (typeof window === 'undefined') {
    return false
  }

  try {
    const perfInstance = getPerformanceInstance()
    return perfInstance !== null
  } catch {
    return false
  }
}

/** Limites do Firebase Performance (evitar invalid attribute value). */
const MAX_TRACE_NAME_LENGTH = 100
const MAX_ATTRIBUTE_KEY_LENGTH = 40
const MAX_ATTRIBUTE_VALUE_LENGTH = 100

/**
 * Sanitiza string para uso em trace name ou atributo (max length, caracteres permitidos).
 */
function sanitizeForFirebase(value: string, maxLength: number): string {
  const sanitized = value.replace(/[^a-zA-Z0-9_\-./]/g, '_').slice(0, maxLength)
  return sanitized || 'unknown'
}

/**
 * Cria e inicia um trace de performance customizado
 * @param traceName Nome do trace
 * @returns Instância do trace ou null se não disponível
 */
export const startTrace = (traceName: string): PerformanceTrace | null => {
  if (!isPerformanceAvailable()) {
    console.warn('[Performance] Performance não disponível, trace não criado')
    return null
  }

  try {
    const perfInstance = getPerformanceInstance()
    if (!perfInstance) return null

    const safeTraceName = sanitizeForFirebase(traceName, MAX_TRACE_NAME_LENGTH)
    const perfTrace = trace(perfInstance, safeTraceName)
    perfTrace.start()

    // Adiciona o trace ID do projeto se disponível (valor sanitizado)
    const traceId = getTraceId()
    if (traceId) {
      perfTrace.putAttribute(
        'trace_id',
        sanitizeForFirebase(traceId, MAX_ATTRIBUTE_VALUE_LENGTH),
      )
    }

    console.log(`[Performance] Trace iniciado: ${safeTraceName}`)
    return perfTrace
  } catch (error) {
    console.error(`[Performance] Erro ao criar trace ${traceName}:`, error)
    return null
  }
}

/**
 * Para um trace de performance
 * @param perfTrace Instância do trace
 */
export const stopTrace = (perfTrace: PerformanceTrace | null): void => {
  if (!perfTrace) {
    return
  }

  try {
    perfTrace.stop()
    console.log('[Performance] Trace finalizado')
  } catch (error) {
    console.error('[Performance] Erro ao finalizar trace:', error)
  }
}

/**
 * Adiciona um atributo customizado a um trace
 * @param perfTrace Instância do trace
 * @param attribute Nome do atributo
 * @param value Valor do atributo
 */
export const putTraceAttribute = (
  perfTrace: PerformanceTrace | null,
  attribute: string,
  value: string,
): void => {
  if (!perfTrace) {
    return
  }

  try {
    const safeKey = sanitizeForFirebase(attribute, MAX_ATTRIBUTE_KEY_LENGTH)
    const safeValue = sanitizeForFirebase(value, MAX_ATTRIBUTE_VALUE_LENGTH)
    perfTrace.putAttribute(safeKey, safeValue)
  } catch (error) {
    console.error(`[Performance] Erro ao adicionar atributo ${attribute}:`, error)
  }
}

/**
 * Adiciona uma métrica customizada a um trace
 * @param perfTrace Instância do trace
 * @param metricName Nome da métrica
 * @param value Valor da métrica (em milissegundos)
 */
export const putTraceMetric = (
  perfTrace: PerformanceTrace | null,
  metricName: string,
  value: number
): void => {
  if (!perfTrace) {
    return
  }

  try {
    perfTrace.putMetric(metricName, value)
  } catch (error) {
    console.error(`[Performance] Erro ao adicionar métrica ${metricName}:`, error)
  }
}

/**
 * Rastreia o tempo de execução de uma função assíncrona
 * @param traceName Nome do trace
 * @param fn Função a ser executada e rastreada
 * @param attributes Atributos customizados para o trace (opcional)
 * @returns Resultado da função
 */
export const traceAsyncFunction = async <T>(
  traceName: string,
  fn: () => Promise<T>,
  attributes?: Record<string, string>
): Promise<T> => {
  const perfTrace = startTrace(traceName)

  // Adiciona o trace ID do projeto se disponível
  const traceId = getTraceId()
  if (traceId && perfTrace) {
    putTraceAttribute(perfTrace, 'trace_id', traceId)
  }

  // Adiciona atributos customizados se fornecidos
  if (attributes && perfTrace) {
    Object.entries(attributes).forEach(([key, value]) => {
      putTraceAttribute(perfTrace, key, value)
    })
  }

  const startTime = typeof window !== 'undefined' ? window.performance.now() : Date.now()

  try {
    const result = await fn()
    const endTime = typeof window !== 'undefined' ? window.performance.now() : Date.now()
    const duration = Math.round(endTime - startTime)
    
    if (perfTrace) {
      putTraceMetric(perfTrace, 'duration', duration)
    }

    stopTrace(perfTrace)
    return result
  } catch (error) {
    const endTime = typeof window !== 'undefined' ? window.performance.now() : Date.now()
    const duration = Math.round(endTime - startTime)
    
    if (perfTrace) {
      putTraceMetric(perfTrace, 'duration', duration)
      putTraceAttribute(perfTrace, 'error', 'true')
      if (error instanceof Error) {
        putTraceAttribute(perfTrace, 'error_message', error.message)
      }
    }

    stopTrace(perfTrace)
    throw error
  }
}

/**
 * Rastreia o tempo de execução de uma função síncrona
 * @param traceName Nome do trace
 * @param fn Função a ser executada e rastreada
 * @param attributes Atributos customizados para o trace (opcional)
 * @returns Resultado da função
 */
export const traceSyncFunction = <T>(
  traceName: string,
  fn: () => T,
  attributes?: Record<string, string>
): T => {
  const perfTrace = startTrace(traceName)

  // Adiciona o trace ID do projeto se disponível
  const traceId = getTraceId()
  if (traceId && perfTrace) {
    putTraceAttribute(perfTrace, 'trace_id', traceId)
  }

  // Adiciona atributos customizados se fornecidos
  if (attributes && perfTrace) {
    Object.entries(attributes).forEach(([key, value]) => {
      putTraceAttribute(perfTrace, key, value)
    })
  }

  const startTime = typeof window !== 'undefined' ? window.performance.now() : Date.now()

  try {
    const result = fn()
    const endTime = typeof window !== 'undefined' ? window.performance.now() : Date.now()
    const duration = Math.round(endTime - startTime)
    
    if (perfTrace) {
      putTraceMetric(perfTrace, 'duration', duration)
    }

    stopTrace(perfTrace)
    return result
  } catch (error) {
    const endTime = typeof window !== 'undefined' ? window.performance.now() : Date.now()
    const duration = Math.round(endTime - startTime)
    
    if (perfTrace) {
      putTraceMetric(perfTrace, 'duration', duration)
      putTraceAttribute(perfTrace, 'error', 'true')
      if (error instanceof Error) {
        putTraceAttribute(perfTrace, 'error_message', error.message)
      }
    }

    stopTrace(perfTrace)
    throw error
  }
}

/**
 * Rastreia uma requisição HTTP
 * @param method Método HTTP (GET, POST, etc.)
 * @param url URL da requisição
 * @param fn Função que executa a requisição
 * @returns Resultado da requisição
 */
export const traceHttpRequest = async <T>(
  method: string,
  url: string,
  fn: () => Promise<T>,
): Promise<T> => {
  // Remove query params e fragmentos da URL para agrupar traces similares
  const cleanUrl = url.split('?')[0].split('#')[0]
  const rawTraceName = `http_${method.toLowerCase()}_${cleanUrl.replace(/[^a-zA-Z0-9]/g, '_')}`
  const traceName = sanitizeForFirebase(rawTraceName, MAX_TRACE_NAME_LENGTH)

  return traceAsyncFunction(traceName, fn, {
    method: method.slice(0, MAX_ATTRIBUTE_VALUE_LENGTH),
    url: sanitizeForFirebase(cleanUrl, MAX_ATTRIBUTE_VALUE_LENGTH),
  })
}

