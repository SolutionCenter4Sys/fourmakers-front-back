import { useState, useEffect } from 'react'
import { useAppSelector } from '@/app/store/hooks'
import { container } from '@core/di/container'
import { DiTokens } from '@core/di/tokens'
import type { CompetenciaRemessaApi } from '@data/api/CompetenciaRemessaApi'
import type { UnidadeRemessaDTO } from '@shared/types/integracaoContabilApi'

export function useUnidadesRemessa() {
  const token = useAppSelector((state) => state.auth.token)
  const [unidades, setUnidades] = useState<UnidadeRemessaDTO[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!token) {
      setUnidades([])
      setError(null)
      return
    }

    let cancelled = false

    const load = async () => {
      setLoading(true)
      setError(null)
      try {
        const api = container.resolve<CompetenciaRemessaApi>(DiTokens.competenciaRemessaApi)
        const list = await api.listarUnidades(token)
        if (!cancelled) {
          setUnidades(list ?? [])
        }
      } catch (err) {
        if (!cancelled) {
          setUnidades([])
          setError(err instanceof Error ? err.message : 'Erro ao carregar unidades')
        }
      } finally {
        if (!cancelled) {
          setLoading(false)
        }
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [token])

  return { unidades, loading, error }
}
