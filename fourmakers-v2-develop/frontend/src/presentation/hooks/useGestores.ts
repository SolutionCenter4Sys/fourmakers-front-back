import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase'
import type { ColaboradorCch } from '@domain/entities/ColaboradorCch'
import { useAppSelector } from '@app/store/hooks'

export const useGestores = (busca: string = '') => {
  const [gestores, setGestores] = useState<ColaboradorCch[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    const loadGestores = async () => {
      if (!token) return

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(ListarColaboradoresOrgUseCase)
        const response = await useCase.execute(token, {
          busca,
          cursor: 0,
          limite: 50000, // Carregar todos os gestores
        })
        setGestores(response.ColaboradoresCchResult || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar gestores')
        setGestores([]) // Garantir array vazio em caso de erro
      } finally {
        setLoading(false)
      }
    }

    // Debounce para evitar muitas requisições durante a digitação
    const timer = setTimeout(() => {
      loadGestores()
    }, 300)

    return () => clearTimeout(timer)
  }, [token, busca])

  return { gestores, loading, error }
}

