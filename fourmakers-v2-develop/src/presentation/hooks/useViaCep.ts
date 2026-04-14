import { useState, useCallback, useRef } from 'react'
import { container } from '@core/di/container'
import { GetAddressByCepUseCase } from '@domain/usecases/GetAddressByCepUseCase'
import type { EnderecoViaCep } from '@domain/repositories/ViaCepRepository'
import { toast } from 'sonner'

/**
 * Hook para buscar endereço através do CEP usando a API ViaCEP
 * Segue a arquitetura Clean Architecture, utilizando Use Case
 * 
 * @example
 * const { buscarCep, loading, error, endereco } = useViaCep();
 * 
 * // Buscar CEP
 * await buscarCep('01310-100');
 * 
 * // Verificar se está carregando
 * if (loading) {
 *   return <div>Buscando...</div>;
 * }
 * 
 * // Usar dados do endereço
 * if (endereco) {
 *   console.log(endereco.endereco, endereco.cidade);
 * }
 */
export const useViaCep = () => {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [endereco, setEndereco] = useState<EnderecoViaCep | null>(null)

  // Use case - usando useRef para manter referência estável
  const useCaseRef = useRef<GetAddressByCepUseCase | null>(null)

  if (!useCaseRef.current) {
    useCaseRef.current = container.resolve(GetAddressByCepUseCase)
  }

  const getAddressByCepUseCase = useCaseRef.current

  /**
   * Busca o endereço através do CEP
   * @param cep - CEP no formato "00000-000" ou "00000000"
   * @returns Promise com os dados do endereço ou null em caso de erro
   */
  const buscarCep = useCallback(async (cep: string): Promise<EnderecoViaCep | null> => {
    setLoading(true)
    setError(null)

    try {
      const enderecoData = await getAddressByCepUseCase.execute(cep)

      if (enderecoData) {
        setEndereco(enderecoData)
        setError(null)
        return enderecoData
      } else {
        setError('CEP não encontrado')
        toast.error('CEP não encontrado')
        setEndereco(null)
        return null
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao buscar CEP. Tente novamente.'
      setError(errorMessage)
      toast.error(errorMessage)
      setEndereco(null)
      return null
    } finally {
      setLoading(false)
    }
  }, [getAddressByCepUseCase])

  /**
   * Limpa os dados do endereço e erros
   */
  const limpar = useCallback(() => {
    setEndereco(null)
    setError(null)
    setLoading(false)
  }, [])

  return {
    buscarCep,
    limpar,
    loading,
    error,
    endereco,
  }
}

// Re-exportar o tipo para compatibilidade
export type { EnderecoViaCep }

