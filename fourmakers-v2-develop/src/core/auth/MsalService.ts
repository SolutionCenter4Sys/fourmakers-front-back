import {
  PublicClientApplication,
  type AccountInfo,
  type AuthenticationResult,
} from '@azure/msal-browser'

import { graphScopes, isMsalConfigurado, msalConfig, MSAL_NAO_CONFIGURADO_MENSAGEM } from './msalConfig'

/**
 * Serviço para gerenciar autenticação Microsoft via MSAL.
 * 
 * Responsabilidades:
 * - Inicializar MSAL
 * - Fazer login interativo
 * - Obter token silenciosamente (com renovação automática)
 * - Fazer logout
 */
export class MsalService {
  private static instance: MsalService | null = null
  private msalInstance: PublicClientApplication | null = null
  private initializePromise: Promise<void> | null = null

  private constructor() {}

  static getInstance(): MsalService {
    if (!MsalService.instance) {
      MsalService.instance = new MsalService()
    }
    return MsalService.instance
  }

  /**
   * Inicializa a instância MSAL.
   * Deve ser chamado antes de qualquer operação.
   * @throws Error com MSAL_NAO_CONFIGURADO_MENSAGEM se VITE_MICROSOFT_CLIENT_ID não estiver definido.
   */
  async initialize(): Promise<void> {
    if (!isMsalConfigurado) {
      throw new Error(MSAL_NAO_CONFIGURADO_MENSAGEM)
    }
    if (this.msalInstance) return
    if (this.initializePromise) return this.initializePromise

    this.initializePromise = (async () => {
      this.msalInstance = new PublicClientApplication(msalConfig)
      await this.msalInstance.initialize()
    })()

    return this.initializePromise
  }

  /**
   * Faz login interativo via popup.
   * Solicita os escopos necessários para Graph API.
   */
  async signIn(): Promise<AuthenticationResult> {
    await this.initialize()
    if (!this.msalInstance) {
      throw new Error('MSAL não inicializado')
    }

    return this.msalInstance.loginPopup({
      scopes: graphScopes,
    })
  }

  /**
   * Obtém token de acesso Microsoft de forma silenciosa.
   * Se o token estiver expirado, renova automaticamente.
   * Se não houver conta autenticada, dispara login interativo.
   * 
   * @param fallbackToken Token Microsoft do Redux/localStorage (se disponível)
   */
  async getAccessToken(fallbackToken?: string | null): Promise<string> {
    await this.initialize()
    if (!this.msalInstance) {
      throw new Error('MSAL não inicializado')
    }

    const accounts = this.msalInstance.getAllAccounts()

    if (accounts.length === 0) {
      if (fallbackToken) {
        return fallbackToken
      }
      throw new Error('Usuário não autenticado no Microsoft. Faça login primeiro.')
    }

    const account = accounts[0]

    try {
      const response = await this.msalInstance.acquireTokenSilent({
        scopes: graphScopes,
        account,
      })
      return response.accessToken
    } catch (error) {
      if (fallbackToken) {
        console.warn('Token silencioso falhou, usando fallback do Redux', error)
        return fallbackToken
      }
      console.warn('Token silencioso falhou, tentando popup', error)
      const response = await this.msalInstance.acquireTokenPopup({
        scopes: graphScopes,
        account,
      })
      return response.accessToken
    }
  }

  /**
   * Retorna a conta Microsoft autenticada, se houver.
   */
  async getAccount(): Promise<AccountInfo | null> {
    await this.initialize()
    if (!this.msalInstance) return null

    const accounts = this.msalInstance.getAllAccounts()
    return accounts.length > 0 ? accounts[0] : null
  }

  /**
   * Verifica se há usuário autenticado no Microsoft.
   */
  async isAuthenticated(): Promise<boolean> {
    const account = await this.getAccount()
    return account !== null
  }

  /**
   * Faz logout do Microsoft.
   */
  async signOut(): Promise<void> {
    await this.initialize()
    if (!this.msalInstance) return

    const accounts = this.msalInstance.getAllAccounts()
    if (accounts.length > 0) {
      await this.msalInstance.logoutPopup({
        account: accounts[0],
      })
    }
  }
}
