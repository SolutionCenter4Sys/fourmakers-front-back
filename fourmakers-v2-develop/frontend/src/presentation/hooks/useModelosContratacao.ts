import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ListarModelosContratacaoUseCase } from '@domain/usecases/ListarModelosContratacaoUseCase'
import type { ModeloContratacao } from '@domain/entities/ModeloContratacao'
import { useAppSelector } from '@app/store/hooks'

export const useModelosContratacao = () => {
  const [modelos, setModelos] = useState<ModeloContratacao[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    const loadModelos = async () => {
      if (!token) return

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(ListarModelosContratacaoUseCase)
        const response = await useCase.execute(token)
        setModelos(response.retorno || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar modelos de contratação')
        setModelos([]) // Garantir array vazio em caso de erro
      } finally {
        setLoading(false)
      }
    }

    loadModelos()
  }, [token])

  return { modelos, loading, error }
}

