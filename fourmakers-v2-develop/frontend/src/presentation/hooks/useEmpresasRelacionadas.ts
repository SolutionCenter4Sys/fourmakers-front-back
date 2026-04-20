import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { ListarEmpresasRelacionadasUseCase } from '@domain/usecases/ListarEmpresasRelacionadasUseCase'
import { useAppSelector } from '@app/store/hooks'

export const useEmpresasRelacionadas = (nomeEmpresa: string = '') => {
  const [empresas, setEmpresas] = useState<string[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token } = useAppSelector((state) => state.auth)

  useEffect(() => {
    const loadEmpresas = async () => {
      if (!token) return

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(ListarEmpresasRelacionadasUseCase)
        const response = await useCase.execute(token, {
          nomeEmpresa,
          limite: 1000,
          cursor: 0,
        })
        setEmpresas(response.retorno || [])
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar empresas relacionadas')
        setEmpresas([]) // Garantir array vazio em caso de erro
      } finally {
        setLoading(false)
      }
    }

    // Debounce para evitar muitas requisições durante a digitação
    const timer = setTimeout(() => {
      loadEmpresas()
    }, 300)

    return () => clearTimeout(timer)
  }, [token, nomeEmpresa])

  return { empresas, loading, error }
}

