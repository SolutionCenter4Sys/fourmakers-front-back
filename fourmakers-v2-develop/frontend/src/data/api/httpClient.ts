import { API_BASE_URL } from '@shared/constants'
import { traceHttpRequest } from '@shared/utils/firebasePerformance'
import { logError } from '@shared/utils/firebaseCrashlytics'
import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'
import { orgLoginConfigs } from '@shared/constants/orgConfig'
import { store } from '@app/store'

/**
 * Remove campos vazios (undefined, null, strings vazias, UUIDs vazias) de um objeto
 */
function removeEmptyFields(obj: unknown): unknown {
  if (obj === null || obj === undefined) {
    return obj
  }
  
  if (Array.isArray(obj)) {
    return obj.map(removeEmptyFields)
  }
  
  if (typeof obj === 'object') {
    const cleaned: Record<string, unknown> = {}
    for (const [key, value] of Object.entries(obj)) {
      // Pular campos undefined
      if (value === undefined) continue
      
      // Pular campos null (se necessário)
      // if (value === null) continue
      
      // Pular strings vazias
      if (typeof value === 'string' && value.trim() === '') continue
      
      // Pular UUIDs vazias
      if (typeof value === 'string' && value === '00000000-0000-0000-0000-000000000000') continue
      
      // Recursivamente limpar objetos aninhados
      cleaned[key] = removeEmptyFields(value)
    }
    return cleaned
  }
  
  return obj
}

/**
 * Gera um novo trace ID único
 */
export function generateTraceId(): string {
  return `${Date.now()}-${Math.random().toString(36).substring(2, 15)}`
}

/**
 * Obtém o trace ID atual do sessionStorage
 */
export function getTraceId(): string | null {
  return sessionStorage.getItem('FRONTEND_TRACE_ID')
}

/**
 * Define o trace ID no sessionStorage
 */
export function setTraceId(traceId: string): void {
  sessionStorage.setItem('FRONTEND_TRACE_ID', traceId)
}

/**
 * Remove o trace ID do sessionStorage
 */
export function clearTraceId(): void {
  sessionStorage.removeItem('FRONTEND_TRACE_ID')
}

/**
 * Obtém os dados do usuário do Redux store.
 */
function getUserFromStore(): ShowmeUserProfile | null {
  try {
    const state = store.getState()
    return state.auth?.user ?? null
  } catch (error) {
    console.warn('[HttpClient] Não foi possível obter dados do usuário:', error)
  }
  return null
}

/** URLs de Gestão Desempenho RH: 401 é tratado como "acesso negado", sem deslogar (igual gestor/colaborador). */
const RH_ENDPOINT_PATH = 'GestaoDesempenho/RH'

/**
 * Função para lidar com erros 401 (Unauthorized)
 * Desloga o usuário e redireciona para a página de login da organização correta
 */
function handleUnauthorizedError() {
  try {
    // Obter lastOrgId do localStorage ANTES de qualquer limpeza
    const lastOrgIdStr = localStorage.getItem('lastOrgId')
    let lastOrgId = lastOrgIdStr ? parseInt(lastOrgIdStr, 10) : null

    console.log('[handleUnauthorizedError] lastOrgId do localStorage:', lastOrgIdStr, 'parsed:', lastOrgId)

    // Se não houver lastOrgId no localStorage, tentar obter do estado do Redux
    if (!lastOrgId || isNaN(lastOrgId)) {
      try {
        const state = store.getState()
        const user = state.auth?.user
        if (user?.colaboradorOrg?.orgId) {
          lastOrgId = user.colaboradorOrg.orgId
          console.log('[handleUnauthorizedError] lastOrgId obtido do Redux:', lastOrgId)
        }
      } catch (error) {
        console.warn('[handleUnauthorizedError] Erro ao obter orgId do Redux:', error)
      }
    }

    // Construir URL de redirecionamento com o slug correto
    let redirectSlug = ''
    if (lastOrgId !== null && !isNaN(lastOrgId)) {
      console.log('[handleUnauthorizedError] Procurando orgConfig para orgId:', lastOrgId, 'tipo:', typeof lastOrgId)
      console.log('[handleUnauthorizedError] orgLoginConfigs disponíveis:', orgLoginConfigs.map(c => ({ orgId: c.orgId, slugRota: c.slugRota })))
      
      const orgConfig = orgLoginConfigs.find((config) => {
        const match = config.orgId === lastOrgId
        console.log('[handleUnauthorizedError] Comparando config.orgId:', config.orgId, 'com lastOrgId:', lastOrgId, 'match:', match)
        return match
      })
      
      console.log('[handleUnauthorizedError] orgConfig encontrado:', orgConfig, 'para orgId:', lastOrgId)
      if (orgConfig && orgConfig.slugRota !== 'default') {
        redirectSlug = orgConfig.slugRota
        console.log('[handleUnauthorizedError] redirectSlug definido:', redirectSlug)
      } else {
        console.log('[handleUnauthorizedError] orgConfig não encontrado ou slugRota é default. orgConfig:', orgConfig)
      }
    } else {
      console.log('[handleUnauthorizedError] lastOrgId é null ou NaN após todas as tentativas')
    }

    // Limpar localStorage (igual ao logout)
    localStorage.removeItem('authToken')
    localStorage.removeItem('fourmakers_api_token')
    localStorage.removeItem('ssoOrgId')
    localStorage.removeItem('lastOrgId')

    // Limpar trace ID
    clearTraceId()

    // Limpar o estado do Redux diretamente (sem chamar logout para evitar redirecionamento duplo)
    store.dispatch({ type: 'auth/logout/fulfilled' })

    // Redirecionar com a URL correta incluindo o slug e o parâmetro unauthorized
    const baseUrl = window.location.origin
    const redirectUrl = `${baseUrl}/login${redirectSlug ? `/${redirectSlug}` : ''}?unauthorized=true`

    console.log('[handleUnauthorizedError] Redirecionando para:', redirectUrl)

    // Redirecionar imediatamente
    window.location.href = redirectUrl
  } catch (error) {
    // Fallback: se algo der errado, tentar redirecionar mesmo assim
    console.error('Erro ao processar logout por 401:', error)
    const lastOrgIdStr = localStorage.getItem('lastOrgId')
    const lastOrgId = lastOrgIdStr ? parseInt(lastOrgIdStr, 10) : null
    localStorage.removeItem('lastOrgId')

    let redirectSlug = ''
    if (lastOrgId !== null && !isNaN(lastOrgId)) {
      const orgConfig = orgLoginConfigs.find((config) => config.orgId === lastOrgId)
      if (orgConfig && orgConfig.slugRota !== 'default') {
        redirectSlug = orgConfig.slugRota
      }
    }

    const baseUrl = window.location.origin
    window.location.href = `${baseUrl}/login${redirectSlug ? `/${redirectSlug}` : ''}?unauthorized=true`
  }
}

export interface HttpClientConfig {
  baseURL?: string
  headers?: Record<string, string>
  token?: string
}

export interface RequestConfig extends RequestInit {
  url: string
  token?: string
  headers?: Record<string, string>
}

/**
 * Factory para criar um client HTTP configurado
 * Inclui automaticamente o header FRONTEND_TRACE_ID quando disponível
 */
export function createHttpClient(config: HttpClientConfig = {}) {
  const baseURL = config.baseURL || API_BASE_URL

  /**
   * Função para fazer requisições HTTP
   */
  async function request<T = unknown>(requestConfig: RequestConfig): Promise<T> {
    const { url, token, headers = {}, ...fetchConfig } = requestConfig
    const method = fetchConfig.method || 'GET'

    // Rastrear performance da requisição HTTP
    return traceHttpRequest(method, url, async () => {
      // Construir URL completa
      // Se a URL já começa com http/https, usar diretamente
      // Caso contrário, concatenar com baseURL
      // IMPORTANTE: Se baseURL estiver definido, sempre usar (mesmo em dev)
      // Se baseURL estiver vazio, usar URL relativa para proxy do Vite
      let fullUrl: string
      if (url.startsWith('http://') || url.startsWith('https://')) {
        fullUrl = url
      } else if (baseURL && baseURL.trim() !== '') {
        // Garantir que baseURL não tenha barra final e url comece com barra
        const cleanBaseURL = baseURL.endsWith('/') ? baseURL.slice(0, -1) : baseURL
        const cleanUrl = url.startsWith('/') ? url : `/${url}`
        fullUrl = `${cleanBaseURL}${cleanUrl}`
      } else {
        // Em dev mode sem baseURL, usar URL relativa (será interceptada pelo proxy do Vite)
        // Mas se o proxy não estiver configurado, isso vai falhar
        fullUrl = url.startsWith('/') ? url : `/${url}`
      }

      // Headers padrão
      const defaultHeaders: Record<string, string> = {
        ...headers,
      }

      // FormData: não definir Content-Type (o fetch define multipart/form-data; boundary=... automaticamente)
      // Se definir manualmente, o boundary fica incorreto e o backend não interpreta o body
      if (fetchConfig.body instanceof FormData) {
        delete defaultHeaders['Content-Type']
      } else {
        defaultHeaders['Content-Type'] = headers['Content-Type'] || 'application/json'
      }

      // Adicionar Authorization se token fornecido
      if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`
      }

      // Adicionar FRONTEND_TRACE_ID apenas para requisições à API interna
      // APIs externas (como ViaCEP) não devem receber esse header para evitar problemas de CORS
      // Verifica se a URL completa não começa com a API_BASE_URL (ou seja, é uma API externa)
      const isExternalApi = baseURL !== API_BASE_URL || (url.startsWith('http') && !fullUrl.startsWith(API_BASE_URL))
      const traceId = getTraceId()
      if (traceId && !isExternalApi) {
        defaultHeaders['FRONTEND_TRACE_ID'] = traceId
      }

      try {
        // Fazer a requisição
        const response = await fetch(fullUrl, {
          ...fetchConfig,
          headers: defaultHeaders,
        })

        // Verificar se a resposta é 401 (Unauthorized) - deslogar automaticamente
        // IMPORTANTE: Verificar 401 ANTES de verificar !response.ok
        // Exceção: endpoints de Gestão Desempenho RH — 401 = acesso negado, não desloga (mantém sessão igual gestor/colaborador)
        if (response.status === 401) {
          const isRhEndpoint = url.includes(RH_ENDPOINT_PATH)
          if (!isRhEndpoint) {
            handleUnauthorizedError()
          }
          throw new Error(
            isRhEndpoint ? 'Acesso negado ao recurso de RH.' : 'Não autorizado. Sessão expirada.'
          )
        }

        // Verificar se a resposta é ok
        if (!response.ok) {
          let errorMessage = `Erro na requisição: ${response.status}`
          let errorData: { mensagem?: string; erros?: string[] } = {}
          try {
            errorData = await response.json().catch(() => ({}))
            if (errorData.mensagem) {
              errorMessage = errorData.mensagem
            }
          } catch {
            // Ignorar erro ao parsear JSON
          }

          // Registrar erro no Firebase com dados do usuário
          const error = new Error(errorMessage) as Error & { erros?: string[] }
          if (errorData.erros && Array.isArray(errorData.erros)) {
            error.erros = errorData.erros
          }
          const user = getUserFromStore()
          logError(error, {
            url: fullUrl,
            method,
            status: response.status,
          }, user)

          throw error
        }

        // Retornar resposta parseada
        // Se a resposta for vazia (204), retornar void
        if (response.status === 204 || response.headers.get('content-length') === '0') {
          return undefined as T
        }

        try {
          return await response.json()
        } catch {
          // Se não conseguir parsear JSON, retornar undefined
          return undefined as T
        }
      } catch (error) {
        // Se já foi tratado como 401, re-lançar o erro sem processar novamente
        if (
          error instanceof Error &&
          (error.message === 'Não autorizado. Sessão expirada.' || error.message === 'Acesso negado ao recurso de RH.')
        ) {
          throw error
        }

        // Registrar erros de rede ou outros erros com dados do usuário
        if (error instanceof Error) {
          const user = getUserFromStore()
          logError(error, {
            url: fullUrl,
            method,
            type: 'network_error',
          }, user)
        }
        throw error
      }
    })
  }

  /**
   * Método GET
   */
  async function get<T = unknown>(url: string, config?: Omit<RequestConfig, 'url' | 'method'>): Promise<T> {
    return request<T>({
      ...config,
      url,
      method: 'GET',
    })
  }

  /**
   * Método POST
   * @param config.keepEmptyStrings - quando true, não remove strings vazias do body (ex.: para APIs que exigem todos os campos no payload)
   */
  async function post<T = unknown>(
    url: string,
    body?: unknown,
    config?: Omit<RequestConfig, 'url' | 'method' | 'body'> & { keepEmptyStrings?: boolean }
  ): Promise<T> {
    const { keepEmptyStrings, ...requestConfig } = config ?? {}
    const isFormData = body instanceof FormData
    const cleanBody = body && !isFormData
      ? (keepEmptyStrings ? body : removeEmptyFields(body))
      : undefined
    const requestBody = isFormData
      ? body
      : (cleanBody ? JSON.stringify(cleanBody) : undefined)
    return request<T>({
      ...requestConfig,
      url,
      method: 'POST',
      body: requestBody,
    })
  }

  /**
   * Método PUT
   * FormData é repassado sem alteração; demais bodies são enviados como JSON.
   */
  async function put<T = unknown>(
    url: string,
    body?: unknown,
    config?: Omit<RequestConfig, 'url' | 'method' | 'body'>
  ): Promise<T> {
    const isFormData = body instanceof FormData
    const requestBody = isFormData
      ? body
      : body
        ? JSON.stringify(removeEmptyFields(body) as object)
        : undefined
    return request<T>({
      ...config,
      url,
      method: 'PUT',
      body: requestBody,
    })
  }

  /**
   * Método PATCH
   * FormData é repassado sem alteração; demais bodies são enviados como JSON.
   */
  async function patch<T = unknown>(
    url: string,
    body?: unknown,
    config?: Omit<RequestConfig, 'url' | 'method' | 'body'>
  ): Promise<T> {
    const isFormData = body instanceof FormData
    const requestBody = isFormData
      ? body
      : body
        ? JSON.stringify(removeEmptyFields(body) as object)
        : undefined
    return request<T>({
      ...config,
      url,
      method: 'PATCH',
      body: requestBody,
    })
  }

  /**
   * Método DELETE
   */
  async function del<T = unknown>(url: string, config?: Omit<RequestConfig, 'url' | 'method'>): Promise<T> {
    return request<T>({
      ...config,
      url,
      method: 'DELETE',
    })
  }

  /**
   * Método GET que retorna Response (para downloads de arquivos)
   */
  async function getBlob(
    url: string,
    config?: Omit<RequestConfig, 'url' | 'method' | 'body'>
  ): Promise<Response> {
    // Rastrear performance da requisição HTTP
    return traceHttpRequest('GET', url, async () => {
      const { token, headers = {}, ...fetchConfig } = config || {}
      let fullUrl: string
      if (url.startsWith('http://') || url.startsWith('https://')) {
        fullUrl = url
      } else if (baseURL) {
        const cleanBaseURL = baseURL.endsWith('/') ? baseURL.slice(0, -1) : baseURL
        const cleanUrl = url.startsWith('/') ? url : `/${url}`
        fullUrl = `${cleanBaseURL}${cleanUrl}`
      } else {
        fullUrl = url.startsWith('/') ? url : `/${url}`
      }

      const defaultHeaders: Record<string, string> = {
        ...headers,
      }

      if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`
      }

      const traceId = getTraceId()
      if (traceId) {
        defaultHeaders['FRONTEND_TRACE_ID'] = traceId
      }

      try {
        const response = await fetch(fullUrl, {
          ...fetchConfig,
          method: 'GET',
          headers: defaultHeaders,
        })

        // Verificar se a resposta é 401 (Unauthorized) - deslogar automaticamente
        // IMPORTANTE: Verificar 401 ANTES de verificar !response.ok
        // Exceção: endpoints Gestão Desempenho RH — 401 = acesso negado, não desloga
        if (response.status === 401) {
          const isRhEndpoint = url.includes(RH_ENDPOINT_PATH)
          if (!isRhEndpoint) {
            handleUnauthorizedError()
          }
          throw new Error(
            isRhEndpoint ? 'Acesso negado ao recurso de RH.' : 'Não autorizado. Sessão expirada.'
          )
        }

        if (!response.ok) {
          let errorMessage = `Erro na requisição: ${response.status}`
          try {
            const errorData = await response.json().catch(() => ({}))
            if (errorData.mensagem) {
              errorMessage = errorData.mensagem
            }
          } catch {
            // Ignorar erro ao parsear JSON
          }
          
          // Registrar erro no Firebase com dados do usuário
          const error = new Error(errorMessage)
          const user = getUserFromStore()
          logError(error, {
            url: fullUrl,
            method: 'GET',
            status: response.status,
            type: 'blob_request',
          }, user)
          
          throw error
        }

        return response
      } catch (error) {
        // Se já foi tratado como 401, re-lançar o erro sem processar novamente
        if (
          error instanceof Error &&
          (error.message === 'Não autorizado. Sessão expirada.' || error.message === 'Acesso negado ao recurso de RH.')
        ) {
          throw error
        }

        // Registrar erros de rede ou outros erros com dados do usuário
        if (error instanceof Error) {
          const user = getUserFromStore()
          logError(error, {
            url: fullUrl,
            method: 'GET',
            type: 'network_error_blob',
          }, user)
        }
        throw error
      }
    })
  }

  /**
   * Método POST que retorna Response (para downloads de arquivos)
   */
  async function postBlob(
    url: string,
    body?: unknown,
    config?: Omit<RequestConfig, 'url' | 'method' | 'body'>
  ): Promise<Response> {
    // Rastrear performance da requisição HTTP
    return traceHttpRequest('POST', url, async () => {
      const { token, headers = {}, ...fetchConfig } = config || {}
      let fullUrl: string
      if (url.startsWith('http://') || url.startsWith('https://')) {
        fullUrl = url
      } else if (baseURL) {
        const cleanBaseURL = baseURL.endsWith('/') ? baseURL.slice(0, -1) : baseURL
        const cleanUrl = url.startsWith('/') ? url : `/${url}`
        fullUrl = `${cleanBaseURL}${cleanUrl}`
      } else {
        fullUrl = url.startsWith('/') ? url : `/${url}`
      }

      const defaultHeaders: Record<string, string> = {
        ...headers,
      }

      if (!(body instanceof FormData)) {
        defaultHeaders['Content-Type'] = headers['Content-Type'] || 'application/json'
      }

      if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`
      }

      const traceId = getTraceId()
      if (traceId) {
        defaultHeaders['FRONTEND_TRACE_ID'] = traceId
      }

      try {
        const response = await fetch(fullUrl, {
          ...fetchConfig,
          method: 'POST',
          headers: defaultHeaders,
          body: body ? JSON.stringify(body) : undefined,
        })

        // Verificar se a resposta é 401 (Unauthorized) - deslogar automaticamente
        // IMPORTANTE: Verificar 401 ANTES de verificar !response.ok
        // Exceção: endpoints Gestão Desempenho RH — 401 = acesso negado, não desloga
        if (response.status === 401) {
          const isRhEndpoint = url.includes(RH_ENDPOINT_PATH)
          if (!isRhEndpoint) {
            handleUnauthorizedError()
          }
          throw new Error(
            isRhEndpoint ? 'Acesso negado ao recurso de RH.' : 'Não autorizado. Sessão expirada.'
          )
        }

        if (!response.ok) {
          let errorMessage = `Erro na requisição: ${response.status}`
          try {
            const errorData = await response.json().catch(() => ({}))
            if (errorData.mensagem) {
              errorMessage = errorData.mensagem
            }
          } catch {
            // Ignorar erro ao parsear JSON
          }
          
          // Registrar erro no Firebase com dados do usuário
          const error = new Error(errorMessage)
          const user = getUserFromStore()
          logError(error, {
            url: fullUrl,
            method: 'POST',
            status: response.status,
            type: 'blob_request',
          }, user)
          
          throw error
        }

        return response
      } catch (error) {
        // Se já foi tratado como 401, re-lançar o erro sem processar novamente
        if (
          error instanceof Error &&
          (error.message === 'Não autorizado. Sessão expirada.' || error.message === 'Acesso negado ao recurso de RH.')
        ) {
          throw error
        }

        // Registrar erros de rede ou outros erros com dados do usuário
        if (error instanceof Error) {
          const user = getUserFromStore()
          logError(error, {
            url: fullUrl,
            method: 'POST',
            type: 'network_error_blob',
          }, user)
        }
        throw error
      }
    })
  }

  return {
    request,
    get,
    post,
    put,
    patch,
    delete: del,
    getBlob,
    postBlob,
  }
}

/**
 * Instância padrão do client HTTP
 */
export const httpClient = createHttpClient()

