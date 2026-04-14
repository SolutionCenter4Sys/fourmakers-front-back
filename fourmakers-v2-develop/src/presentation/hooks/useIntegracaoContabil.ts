import { useState, useEffect } from 'react'
import { useAppSelector } from '@/app/store/hooks'
import { store } from '@/app/store'
import { container } from '@core/di/container'
import { IntegracaoContabilApi } from '@data/api/IntegracaoContabilApi'
import type {
  RetornoContabilMock,
  ColaboradorRetornoMock,
} from '@data/mocks/integracaoContabilMock'
import type { LoteHoleriteDTO } from '@shared/types/integracaoContabilApi'

export const useIntegracaoContabil = () => {
  const token = useAppSelector((state) => state.auth.token)
  const [retornos, setRetornos] = useState<RetornoContabilMock[]>([])
  const [colaboradores, setColaboradores] = useState<ColaboradorRetornoMock[]>([])
  const [lotes, setLotes] = useState<LoteHoleriteDTO[]>([])
  const [loading, setLoading] = useState(true)
  const [loadingLotes, setLoadingLotes] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [errorLotes, setErrorLotes] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(IntegracaoContabilApi)
        const [retornosData, colaboradoresData] = await Promise.all([
          api.getRetornos(),
          api.getColaboradores(),
        ])
        setRetornos(retornosData)
        setColaboradores(colaboradoresData)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  const refetchLotes = async () => {
    const currentToken = store.getState().auth.token
    if (!currentToken) return
    setLoadingLotes(true)
    setErrorLotes(null)
    try {
      const api = container.resolve(IntegracaoContabilApi)
      const list = await api.listarLotesHolerite(currentToken)
      setLotes(list ?? [])
    } catch (err) {
      setLotes([])
      setErrorLotes(err instanceof Error ? err.message : 'Erro ao carregar lotes')
    } finally {
      setLoadingLotes(false)
    }
  }

  useEffect(() => {
    if (!token) {
      setLotes([])
      setErrorLotes(null)
      return
    }
    let cancelled = false
    const loadLotes = async () => {
      const currentToken = store.getState().auth.token
      if (!currentToken) return
      setLoadingLotes(true)
      setErrorLotes(null)
      try {
        const api = container.resolve(IntegracaoContabilApi)
        const list = await api.listarLotesHolerite(currentToken)
        if (!cancelled) setLotes(list ?? [])
      } catch (err) {
        if (!cancelled) {
          setLotes([])
          setErrorLotes(err instanceof Error ? err.message : 'Erro ao carregar lotes')
        }
      } finally {
        if (!cancelled) setLoadingLotes(false)
      }
    }
    loadLotes()
    return () => {
      cancelled = true
    }
  }, [token])

  return { retornos, colaboradores, lotes, loading, loadingLotes, error, errorLotes, refetchLotes }
}

