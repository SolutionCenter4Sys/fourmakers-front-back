import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { IntegracaoFolhaPontoApi } from '@data/api/IntegracaoFolhaPontoApi'
import type {
  LoteProcessadoMock,
  ColaboradorDetalheMock,
} from '@data/mocks/integracaoFolhaPontoMock'

export const useIntegracaoFolhaPonto = () => {
  const [lotes, setLotes] = useState<LoteProcessadoMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadLotes = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(IntegracaoFolhaPontoApi)
        const data = await api.getLotes()
        setLotes(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar lotes')
      } finally {
        setLoading(false)
      }
    }

    loadLotes()
  }, [])

  const getColaboradoresPorLote = async (loteId: number): Promise<ColaboradorDetalheMock[]> => {
    try {
      const api = container.resolve(IntegracaoFolhaPontoApi)
      return await api.getColaboradoresPorLote(loteId)
    } catch (err) {
      console.error('Erro ao carregar colaboradores:', err)
      return []
    }
  }

  return { lotes, loading, error, getColaboradoresPorLote }
}

