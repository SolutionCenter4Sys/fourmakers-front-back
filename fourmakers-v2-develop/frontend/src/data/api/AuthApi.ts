import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'

import { API_BASE_URL } from '@shared/constants'
import { createHttpClient, httpClient } from './httpClient'

export interface SendTokenEmailResponse {
  tipoAcesso: number
  sucesso: boolean
  mensagem?: string
  orgId?: number
}

export interface ValidateTokenEmailResponse {
  token?: string
  usuario?: ShowmeUserProfile
  sucesso: boolean
  mensagem?: string
  orgId?: number
  tipoAcesso?: number
  /** Token Microsoft Graph (opcional - se backend retornar) */
  microsoftAccessToken?: string
}

export class AuthApi {
  async getShowmeProfile(token: string): Promise<ShowmeUserProfile> {
    const data = await httpClient.get<{ sucesso: boolean; usuario?: ShowmeUserProfile }>(
      '/api/Usuario/Showme',
      { token }
    )

    if (!data?.sucesso || !data.usuario) {
      throw new Error('Resposta do Showme não contém dados de usuário válidos.')
    }

    return data.usuario
  }

  async sendTokenEmail(email: string, orgId: number | null): Promise<SendTokenEmailResponse> {
    const apiUrl = import.meta.env.VITE_API_FOURMAKERS_URL || API_BASE_URL
    let endpoint = '/api/Acesso/EnviaTokenAcessoEmail'
    const body: Record<string, string | number> = { email }

    if (orgId === 0 || orgId === null) {
      endpoint = '/api/Acesso/EnviaTokenAcessoEmailSemOrg'
    } else {
      body.orgId = orgId
    }

    const client = createHttpClient({ baseURL: apiUrl })
    const data = await client.post<SendTokenEmailResponse>(endpoint, body)
    return data
  }

  async validateTokenEmail(
    email: string,
    token: string,
    orgId: number | null,
  ): Promise<ValidateTokenEmailResponse> {
    const apiUrl = import.meta.env.VITE_API_FOURMAKERS_URL || API_BASE_URL
    const body: Record<string, string | number> = { email, token }

    // Sempre usar ValidaTokenAcessoEmail, incluindo orgId se disponível
    if (orgId !== null && orgId !== 0) {
      body.orgId = orgId
    }

    const client = createHttpClient({ baseURL: apiUrl })
    const data = await client.post<ValidateTokenEmailResponse>('/api/Acesso/ValidaTokenAcessoEmail', body)

    // Exigir token e usuario em todas as rotas
    if (!data.sucesso || !data.token || !data.usuario) {
      throw new Error(data.mensagem || 'Resposta inválida da API')
    }

    return data
  }

  async getAccessToken(code: string, orgId: number): Promise<ValidateTokenEmailResponse> {
    const apiUrl = import.meta.env.VITE_API_FOURMAKERS_URL || API_BASE_URL
    const client = createHttpClient({ baseURL: apiUrl })
    
    const data = await client.post<ValidateTokenEmailResponse>('/api/Usuario/GetAccessToken', {
      AccessCode: code,
      orgId: orgId,
    })

    if (!data.sucesso || !data.token || !data.usuario) {
      throw new Error(data.mensagem || 'Resposta inválida da API')
    }

    return {
      token: data.token,
      usuario: data.usuario,
      sucesso: data.sucesso,
      mensagem: data.mensagem,
      microsoftAccessToken: data.microsoftAccessToken,
    }
  }

  async logout(token: string, cpf: string): Promise<void> {
    try {
      await httpClient.post('/api/Usuario/Logout', { cpfemail: cpf }, { token })
    } catch (error) {
      console.error('Erro ao chamar API de logout:', error)
      // Continuar com o logout local mesmo que a API falhe
    }
  }
}

