import { useState, useEffect, useCallback } from 'react'
import { container } from '@core/di/container'
import { ConciliacaoFolhaPagamentoApi } from '@data/api/ConciliacaoFolhaPagamentoApi'
import type { StatusVigenciaItemDTO } from '@data/api/ConciliacaoFolhaPagamentoApi'
import type {
  LoteConciliacaoMock,
  ColaboradorConciliacaoMock,
} from '@data/mocks/conciliacaoFolhaPagamentoMock'

export const useConciliacaoFolhaPagamento = (token: string | null | undefined) => {
  const [lotes, setLotes] = useState<LoteConciliacaoMock[]>([])
  const [colaboradores, setColaboradores] = useState<ColaboradorConciliacaoMock[]>([])
  const [statusVigencias, setStatusVigencias] = useState<StatusVigenciaItemDTO[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const loadData = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)
      const api = container.resolve(ConciliacaoFolhaPagamentoApi)
      const lotesPromise = token
        ? api.buscarLotes(token)
        : api.getLotes()
      const statusVigenciasPromise = token
        ? api.statusVigencia(token)
        : Promise.resolve([])
      const [lotesData, colaboradoresData, statusVigenciasData] = await Promise.all([
        lotesPromise,
        api.getColaboradores(),
        statusVigenciasPromise,
      ])
      setLotes(lotesData)
      setColaboradores(colaboradoresData)
      setStatusVigencias(statusVigenciasData)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
    } finally {
      setLoading(false)
    }
  }, [token])

  useEffect(() => {
    loadData()
  }, [loadData])

  return { lotes, colaboradores, statusVigencias, loading, error, recarregarLotes: loadData }
}

