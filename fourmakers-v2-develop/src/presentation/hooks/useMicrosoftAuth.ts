import { useCallback, useEffect, useState } from 'react'
import { MsalService } from '@core/auth/MsalService'

/**
 * Hook para gerenciar autenticação Microsoft via MSAL.
 * 
 * Fornece:
 * - Estado de autenticação
 * - Função de login
 * - Função de logout
 * - Verificação automática de autenticação ao montar
 */
export function useMicrosoftAuth() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const msalService = MsalService.getInstance()

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const authenticated = await msalService.isAuthenticated()
        setIsAuthenticated(authenticated)
      } catch (err) {
        console.error('Erro ao verificar autenticação Microsoft:', err)
        setIsAuthenticated(false)
      } finally {
        setIsLoading(false)
      }
    }

    void checkAuth()
  }, [msalService])

  const signIn = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      const result = await msalService.signIn()
      setIsAuthenticated(true)
      return result
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao fazer login no Microsoft'
      setError(errorMessage)
      setIsAuthenticated(false)
      throw new Error(errorMessage)
    } finally {
      setIsLoading(false)
    }
  }, [msalService])

  const signOut = useCallback(async () => {
    setIsLoading(true)
    setError(null)
    try {
      await msalService.signOut()
      setIsAuthenticated(false)
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao fazer logout do Microsoft'
      setError(errorMessage)
      throw new Error(errorMessage)
    } finally {
      setIsLoading(false)
    }
  }, [msalService])

  return {
    isAuthenticated,
    isLoading,
    error,
    signIn,
    signOut,
  }
}
