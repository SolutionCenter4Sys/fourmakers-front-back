import { useAppSelector } from '@app/store/hooks'
import { getParametroByCode, isParametroEnabled } from '@app/store/slices/parametrosSlice'

/**
 * Hook para acessar os parâmetros de configuração do usuário logado
 * 
 * Uso:
 * const { getParametro, isEnabled } = useParametros()
 * 
 * // Obter valor de qualquer parâmetro
 * const valor = getParametro('CODIGO_PARAMETRO') // retorna string | null
 * 
 * // Verificar se parâmetro booleano está habilitado
 * if (isEnabled('MOSTRAR_TIMESHEET')) {
 *   // exibir timesheet
 * }
 */
export const useParametros = () => {
  const parametros = useAppSelector((state) => state.parametros.parametros)
  const status = useAppSelector((state) => state.parametros.status)
  const error = useAppSelector((state) => state.parametros.error)

  /**
   * Obtém o valor de um parâmetro pelo código
   * @param codigo - Código do parâmetro (ex: 'MOSTRAR_TIMESHEET')
   * @returns string | null - Valor do parâmetro ou null se não encontrado
   */
  const getParametro = (codigo: string): string | null => {
    return getParametroByCode(parametros, codigo)
  }

  /**
   * Verifica se um parâmetro booleano está habilitado (valor = 'true')
   * @param codigo - Código do parâmetro (ex: 'MOSTRAR_TIMESHEET')
   * @returns boolean - true se o parâmetro existe e seu valor é 'true'
   */
  const isEnabled = (codigo: string): boolean => {
    return isParametroEnabled(parametros, codigo)
  }

  return {
    parametros,      // Array completo de parâmetros
    status,          // Status do carregamento: 'idle' | 'loading' | 'succeeded' | 'failed'
    error,           // Mensagem de erro, se houver
    getParametro,    // Função para obter valor de qualquer parâmetro
    isEnabled,       // Função para verificar se parâmetro booleano está habilitado
  }
}

