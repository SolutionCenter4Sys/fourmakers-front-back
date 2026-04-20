import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { GetColaboradoresUseCase } from '@domain/usecases/GetColaboradoresUseCase'
import type { Colaborador } from '@domain/entities/Colaborador'
import type { ColaboradoresParams, ColaboradoresResponse } from '@domain/entities/Colaborador'

export const useColaboradores = (params?: ColaboradoresParams) => {
  const [colaboradores, setColaboradores] = useState<Colaborador[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadColaboradores = async () => {
      try {
        setLoading(true)
        setError(null)
        const token = localStorage.getItem('authToken')
        if (!token) {
          throw new Error('Token de autenticação não encontrado')
        }
        
        const useCase = container.resolve(GetColaboradoresUseCase)
        const defaultParams: ColaboradoresParams = {
          cursor: 0,
          limite: 100,
          nomeOuEmail: '',
          org: 0,
          ...params,
        }
        const data: ColaboradoresResponse = await useCase.execute(token, defaultParams)
        setColaboradores(data.retorno || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar colaboradores')
      } finally {
        setLoading(false)
      }
    }

    loadColaboradores()
  }, [params])

  return { colaboradores, loading, error }
}

