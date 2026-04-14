import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ReembolsoParametrosApi } from '@data/api/ReembolsoParametrosApi'
import type { LogEntry } from '@shared/types/reembolso'
import type { VerbaData } from '@shared/types/reembolso'

export const useReembolsoLogs = () => {
  const [logs, setLogs] = useState<LogEntry[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ReembolsoParametrosApi)
        const data = await api.getLogs()
        setLogs(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { logs, loading, error }
}

export const useReembolsoVerbasInicial = () => {
  const [verbas, setVerbas] = useState<VerbaData[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(ReembolsoParametrosApi)
        const data = await api.getVerbasInicial()
        setVerbas(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { verbas, loading, error }
}

