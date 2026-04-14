import { LogLevel } from '@azure/msal-browser'
import type { Configuration } from '@azure/msal-browser'

/**
 * Configuração do MSAL para autenticação Microsoft.
 * 
 * IMPORTANTE: As variáveis de ambiente devem ser configuradas:
 * - VITE_MICROSOFT_CLIENT_ID: Application (client) ID do Azure AD
 * - VITE_MICROSOFT_TENANT_ID: Tenant ID (ou 'organizations' para multi-tenant)
 * - VITE_MICROSOFT_REDIRECT_URI: URI de redirecionamento (ex: http://localhost:5173/auth/callback)
 */
export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_MICROSOFT_CLIENT_ID || '',
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_MICROSOFT_TENANT_ID || 'organizations'}`,
    redirectUri: import.meta.env.VITE_MICROSOFT_REDIRECT_URI || window.location.origin,
  },
  cache: {
    cacheLocation: 'localStorage',
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return
        switch (level) {
          case LogLevel.Error:
            console.error(message)
            break
          case LogLevel.Warning:
            console.warn(message)
            break
          default:
            break
        }
      },
    },
  },
}

/**
 * Indica se as credenciais Microsoft (Teams/Graph) estão configuradas.
 * Quando false, login MSAL não deve ser tentado (evita erro AADSTS900144 do Azure).
 */
export const isMsalConfigurado =
  typeof import.meta.env.VITE_MICROSOFT_CLIENT_ID === 'string' &&
  import.meta.env.VITE_MICROSOFT_CLIENT_ID.trim() !== ''

/** Mensagem de erro quando MSAL não está configurado (Client ID ausente). Usada pelo MsalService e pela UI. */
export const MSAL_NAO_CONFIGURADO_MENSAGEM =
  'Integração com Microsoft Teams não está configurada. Configure as variáveis de ambiente VITE_MICROSOFT_CLIENT_ID, VITE_MICROSOFT_TENANT_ID e VITE_MICROSOFT_REDIRECT_URI e reconstrua a aplicação.'

/**
 * Escopos necessários para criar reuniões do Teams via Graph API.
 */
export const graphScopes = [
  'User.Read',
  'Calendars.ReadWrite',
  'OnlineMeetings.ReadWrite',
]
