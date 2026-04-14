import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { GetProjetosUseCase } from '@domain/usecases/GetProjetosUseCase'
import type { ProjetoMock } from '@data/mocks/projetosMock'

export const useProjetos = () => {
  const [projetos, setProjetos] = useState<ProjetoMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadProjetos = async () => {
      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetProjetosUseCase)
        const data = await useCase.execute()
        setProjetos(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar projetos')
      } finally {
        setLoading(false)
      }
    }

    loadProjetos()
  }, [])

  return { projetos, loading, error }
}

