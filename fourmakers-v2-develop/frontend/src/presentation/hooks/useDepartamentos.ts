import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ListarDepartamentosUseCase } from '@domain/usecases/ListarDepartamentosUseCase'
import type { Departamento } from '@domain/entities/Departamento'
import { useAppSelector } from '@app/store/hooks'

export const useDepartamentos = (codigoDiretoria?: string) => {
  const [departamentos, setDepartamentos] = useState<Departamento[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    const loadDepartamentos = async () => {
      if (!token) return

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(ListarDepartamentosUseCase)
        const response = await useCase.execute(token, {
          codigoDiretoria,
        })
        setDepartamentos(response.retorno || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar departamentos')
        setDepartamentos([]) // Garantir array vazio em caso de erro
      } finally {
        setLoading(false)
      }
    }

    loadDepartamentos()
  }, [token, codigoDiretoria])

  return { departamentos, loading, error }
}

