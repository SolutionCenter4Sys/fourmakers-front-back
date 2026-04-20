import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ListarCargosUseCase } from '@domain/usecases/ListarCargosUseCase'
import type { Cargo } from '@domain/entities/Cargo'
import { useAppSelector } from '@app/store/hooks'

export const useCargos = () => {
  const [cargos, setCargos] = useState<Cargo[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    const loadCargos = async () => {
      if (!token) return

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(ListarCargosUseCase)
        const response = await useCase.execute(token)
        setCargos(response.retorno || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar cargos')
        setCargos([]) // Garantir array vazio em caso de erro
      } finally {
        setLoading(false)
      }
    }

    loadCargos()
  }, [token])

  return { cargos, loading, error }
}

