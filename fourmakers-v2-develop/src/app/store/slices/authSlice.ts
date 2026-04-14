import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'
import { GetShowmeProfileUseCase } from '@domain/usecases/GetShowmeProfileUseCase'
import { LogoutUseCase } from '@domain/usecases/LogoutUseCase'
import { SendTokenEmailUseCase } from '@domain/usecases/SendTokenEmailUseCase'
import { ValidateTokenEmailUseCase } from '@domain/usecases/ValidateTokenEmailUseCase'
import { GetAccessTokenUseCase } from '@domain/usecases/GetAccessTokenUseCase'
import { generateTraceId, setTraceId, clearTraceId } from '@data/api/httpClient'
import { logLoginEvent, logLogoutEvent } from '@shared/utils/firebaseAnalytics'

import { orgLoginConfigs } from '@shared/constants/orgConfig'
import { clearFlutterFlowStorageIframe } from '@shared/utils/clearFlutterFlowStorage'

export interface AuthState {
  token: string | null
  user: ShowmeUserProfile | null
  codColaborador: string | null
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  // Estados específicos do login
  loginStatus: 'idle' | 'sending' | 'sent' | 'validating' | 'succeeded' | 'failed'
  loginError: string | null
  tipoAcesso: number | null
  orgIdFromResponse: number | null
  /** Token Microsoft Graph (obtido via SSO ou MSAL) */
  microsoftAccessToken: string | null
}

const getInitialToken = (): string | null => {
  return localStorage.getItem('authToken') || null
}

const getInitialMicrosoftToken = (): string | null => {
  return localStorage.getItem('microsoftAccessToken') || null
}

const initialState: AuthState = {
  token: getInitialToken(),
  user: null,
  codColaborador: null,
  status: 'idle',
  error: null,
  loginStatus: 'idle',
  loginError: null,
  tipoAcesso: null,
  orgIdFromResponse: null,
  microsoftAccessToken: getInitialMicrosoftToken(),
}

export const fetchShowmeProfile = createAsyncThunk(
  'auth/fetchShowmeProfile',
  async (_, { getState }) => {
    const state = getState() as { auth: AuthState }
    let token = state.auth.token

    // Se não houver token no state, buscar do localStorage
    if (!token) {
      token = localStorage.getItem('authToken')
    }

    if (!token) {
      throw new Error('Token não encontrado')
    }

    const useCase = container.resolve(GetShowmeProfileUseCase)
    return useCase.execute(token)
  },
  {
    condition: (_, { getState }) => {
      const state = getState() as { auth: AuthState }
      // Não executar se já há uma requisição pendente
      if (state.auth.status === 'loading') {
        return false
      }
      // Não executar se já há usuário carregado
      if (state.auth.user) {
        return false
      }
      return true
    },
  },
)

export const sendLoginToken = createAsyncThunk(
  'auth/sendLoginToken',
  async ({ email, orgId }: { email: string; orgId: number | null }) => {
    const useCase = container.resolve(SendTokenEmailUseCase)
    return useCase.execute(email, orgId)
  },
)

export const validateLoginToken = createAsyncThunk(
  'auth/validateLoginToken',
  async ({
    email,
    token,
    orgId,
  }: {
    email: string
    token: string
    orgId: number | null
  }) => {
    const useCase = container.resolve(ValidateTokenEmailUseCase)
    return useCase.execute(email, token, orgId)
  },
)

export const getAccessToken = createAsyncThunk(
  'auth/getAccessToken',
  async ({ code, orgId }: { code: string; orgId: number }) => {
    const useCase = container.resolve(GetAccessTokenUseCase)
    return useCase.execute(code, orgId)
  },
)

export const logout = createAsyncThunk(
  'auth/logout',
  async (_, { getState }) => {
    const state = getState() as { auth: AuthState }
    const token = state.auth.token
    const user = state.auth.user

    // Registrar evento de logout no Firebase Analytics antes de limpar os dados
    logLogoutEvent(user || null)

    // Limpar trace ID
    clearTraceId()

    // Obter lastOrgId do localStorage para redirecionar para a org correta
    const lastOrgIdStr = localStorage.getItem('lastOrgId')
    const lastOrgId = lastOrgIdStr ? parseInt(lastOrgIdStr, 10) : null

    // Limpar localStorage imediatamente
    // Limpar TODOS os dados do localStorage relacionados à aplicação
    localStorage.removeItem('authToken')
    localStorage.removeItem('fourmakers_api_token')
    localStorage.removeItem('ssoOrgId')
    localStorage.removeItem('lastOrgId') // Remover lastOrgId após usar
    // Nota: theme (THEME_KEY) não é limpo pois é uma preferência do usuário

    // Executar logout na API em background (sem bloquear o redirecionamento)
    if (token && user?.cpf) {
      const logoutUseCase = container.resolve(LogoutUseCase)
      // Não usar await - executar em background
      logoutUseCase.execute(token, user.cpf).catch((error) => {
        // Silenciosamente ignorar erros da API de logout
        console.error('Erro ao fazer logout na API:', error)
      })
    }

    try {
      clearFlutterFlowStorageIframe()
    } catch {
      // Não bloquear logout se o iframe falhar
    }

    // Redirecionar para a tela de login da org específica (pequeno delay para o iframe iniciar o carregamento)
    let redirectSlug = ''
    const orgIdToUse = lastOrgId ?? user?.colaboradorOrg?.orgId
    if (orgIdToUse) {
      const orgConfig = orgLoginConfigs.find((config) => config.orgId === orgIdToUse)
      if (orgConfig && orgConfig.slugRota !== 'default') {
        redirectSlug = orgConfig.slugRota
      }
    }

    const baseUrl = window.location.origin
    const loginUrl = `${baseUrl}/login${redirectSlug ? `/${redirectSlug}` : ''}`
    setTimeout(() => {
      window.location.href = loginUrl
    }, 150)
  },
)

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setToken: (state, action: PayloadAction<string | null>) => {
      state.token = action.payload
      if (action.payload) {
        localStorage.setItem('authToken', action.payload)
      } else {
        localStorage.removeItem('authToken')
      }
    },
    setUser: (state, action: PayloadAction<ShowmeUserProfile | null>) => {
      state.user = action.payload
    },
    clearAuth: (state) => {
      state.token = null
      state.user = null
      state.codColaborador = null
      state.status = 'idle'
      state.error = null
      state.microsoftAccessToken = null
      localStorage.removeItem('authToken')
      localStorage.removeItem('microsoftAccessToken')
      clearTraceId()
    },
    clearLoginState: (state) => {
      state.loginStatus = 'idle'
      state.loginError = null
      state.tipoAcesso = null
      state.orgIdFromResponse = null
    },
    setMicrosoftAccessToken: (state, action: PayloadAction<string | null>) => {
      state.microsoftAccessToken = action.payload
      if (action.payload) {
        localStorage.setItem('microsoftAccessToken', action.payload)
      } else {
        localStorage.removeItem('microsoftAccessToken')
      }
    },
    updateQuestionariosAtivos: (state, action: PayloadAction<string>) => {
      if (state.user) {
        const questionarioCodigo = action.payload
        // Verificar se o código já existe no array (evitar duplicatas)
        if (!state.user.questionariosAtivos?.includes(questionarioCodigo)) {
          state.user.questionariosAtivos = [
            ...(state.user.questionariosAtivos || []),
            questionarioCodigo,
          ]
        }
      }
    },
    updateQuestionariosPreenchidos: (state, action: PayloadAction<string>) => {
      if (state.user) {
        const questionarioCodigo = action.payload
        // Verificar se o código já existe no array (evitar duplicatas)
        if (!state.user.questionariosPreenchidos?.includes(questionarioCodigo)) {
          state.user.questionariosPreenchidos = [
            ...(state.user.questionariosPreenchidos || []),
            questionarioCodigo,
          ]
        }
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchShowmeProfile.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(fetchShowmeProfile.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.user = action.payload
        state.codColaborador = action.payload?.cpf ?? null
        // Se o token não estava no state, atualizar do localStorage
        if (!state.token) {
          const tokenFromStorage = localStorage.getItem('authToken')
          if (tokenFromStorage) {
            state.token = tokenFromStorage
          }
        }
        // Persistir orgId no localStorage para redirecionamento após logout/401
        if (action.payload?.colaboradorOrg?.orgId) {
          localStorage.setItem('lastOrgId', String(action.payload.colaboradorOrg.orgId))
        }
      })
      .addCase(fetchShowmeProfile.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Não foi possível carregar o perfil do usuário.'
        state.user = null
        state.codColaborador = null
      })
      .addCase(logout.pending, (state) => {
        state.status = 'loading'
      })
      .addCase(logout.fulfilled, (state) => {
        state.token = null
        state.user = null
        state.codColaborador = null
        state.status = 'idle'
        state.error = null
      })
      .addCase(logout.rejected, (state) => {
        state.token = null
        state.user = null
        state.codColaborador = null
        state.status = 'idle'
        state.error = null
      })
      // Login: Enviar token
      .addCase(sendLoginToken.pending, (state) => {
        state.loginStatus = 'sending'
        state.loginError = null
      })
      .addCase(sendLoginToken.fulfilled, (state, action) => {
        state.loginStatus = 'sent' // Alterado de 'succeeded' para 'sent' para não disparar redirecionamento
        state.tipoAcesso = action.payload.tipoAcesso
        state.orgIdFromResponse = action.payload.orgId || null
        state.loginError = null
      })
      .addCase(sendLoginToken.rejected, (state, action) => {
        state.loginStatus = 'failed'
        state.loginError = action.error.message ?? 'Erro ao enviar token de acesso.'
        state.tipoAcesso = null
      })
      // Login: Validar token
      .addCase(validateLoginToken.pending, (state) => {
        state.loginStatus = 'validating'
        state.loginError = null
      })
      .addCase(validateLoginToken.fulfilled, (state, action) => {
        state.loginStatus = 'succeeded'
        state.token = action.payload.token || null
        state.user = action.payload.usuario || null
        state.loginError = null
        if (action.payload.token) {
          localStorage.setItem('authToken', action.payload.token)
          // Gerar e setar trace ID no login
          const traceId = generateTraceId()
          setTraceId(traceId)
          // Registrar evento de login no Firebase Analytics
          logLoginEvent('email_token', action.payload.usuario || null)
        }
        // Persistir orgId no localStorage para redirecionamento após logout/401
        if (action.payload.usuario?.colaboradorOrg?.orgId) {
          localStorage.setItem('lastOrgId', String(action.payload.usuario.colaboradorOrg.orgId))
        }
      })
      .addCase(validateLoginToken.rejected, (state, action) => {
        state.loginStatus = 'failed'
        state.loginError = action.error.message ?? 'Erro ao validar token de acesso.'
      })
      // SSO: Obter token de acesso
      .addCase(getAccessToken.pending, (state) => {
        state.loginStatus = 'validating'
        state.loginError = null
      })
      .addCase(getAccessToken.fulfilled, (state, action) => {
        state.loginStatus = 'succeeded'
        state.token = action.payload.token ?? null
        state.user = action.payload.usuario ?? null
        state.loginError = null
        if (action.payload.token) {
          localStorage.setItem('authToken', action.payload.token)
          // Gerar e setar trace ID no login
          const traceId = generateTraceId()
          setTraceId(traceId)
          // Registrar evento de login no Firebase Analytics
          logLoginEvent('sso', action.payload.usuario ?? null)
        }
        // Persistir token Microsoft se backend retornar
        if (action.payload.microsoftAccessToken) {
          state.microsoftAccessToken = action.payload.microsoftAccessToken
          localStorage.setItem('microsoftAccessToken', action.payload.microsoftAccessToken)
        }
        // Persistir orgId no localStorage para redirecionamento após logout/401
        if (action.payload.usuario?.colaboradorOrg?.orgId) {
          localStorage.setItem('lastOrgId', String(action.payload.usuario.colaboradorOrg.orgId))
        }
      })
      .addCase(getAccessToken.rejected, (state, action) => {
        state.loginStatus = 'failed'
        state.loginError = action.error.message ?? 'Erro ao obter token de acesso.'
      })
  },
})

export const { setToken, setUser, clearAuth, clearLoginState, setMicrosoftAccessToken, updateQuestionariosAtivos, updateQuestionariosPreenchidos } = authSlice.actions
export default authSlice.reducer

