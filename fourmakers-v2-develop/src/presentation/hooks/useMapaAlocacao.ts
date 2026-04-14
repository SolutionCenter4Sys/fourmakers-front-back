import { useState, useCallback } from 'react'
import { container } from '@core/di/container'
import { ListarAlocacoesColabETbdUseCase } from '@domain/usecases/ListarAlocacoesColabETbdUseCase'
import type { AlocacaoColaboradorTbd, ListarAlocacoesPayload } from '@domain/entities/MapaAlocacao'

export const useAlocacoesTab = (token: string | null) => {
  const [alocados, setAlocados] = useState<AlocacaoColaboradorTbd[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const buscarAlocacoes = useCallback(async (payload: ListarAlocacoesPayload) => {
    if (!token) {
      setError('Token de autenticação não disponível')
      return
    }

    try {
      setLoading(true)
      setError(null)
      const useCase = container.resolve(ListarAlocacoesColabETbdUseCase)
      const response = await useCase.execute(token, payload)
      
      if (response.sucesso) {
        setAlocados(response.retorno || [])
      } else {
        setError(response.mensagem || 'Erro ao buscar alocações')
        setAlocados([])
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao buscar alocações')
      setAlocados([])
    } finally {
      setLoading(false)
    }
  }, [token])

  return { alocados, loading, error, buscarAlocacoes }
}
