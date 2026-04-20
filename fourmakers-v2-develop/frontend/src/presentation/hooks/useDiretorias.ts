import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ListarDiretoriasUseCase } from '@domain/usecases/ListarDiretoriasUseCase'
import type { DiretoriaColaborador } from '@domain/entities/Diretoria'
import { useAppSelector } from '@app/store/hooks'

export const useDiretorias = () => {
  const [diretorias, setDiretorias] = useState<DiretoriaColaborador[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    const loadDiretorias = async () => {
      if (!token) return

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(ListarDiretoriasUseCase)
        const response = await useCase.execute(token)
        setDiretorias(response.diretoriaColaborador || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar diretorias')
        setDiretorias([]) // Garantir array vazio em caso de erro
      } finally {
        setLoading(false)
      }
    }

    loadDiretorias()
  }, [token])

  return { diretorias, loading, error }
}

