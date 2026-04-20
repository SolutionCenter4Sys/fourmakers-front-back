import { useState, useEffect } from 'react'

/**
 * Hook para debounce de valores
 * Útil para otimizar buscas e evitar chamadas excessivas à API
 * 
 * @param value - Valor a ser debounced
 * @param delay - Delay em milissegundos (default: 300ms)
 * @returns Valor debounced
 * 
 * @example
 * const searchTerm = 'cliente'
 * const debouncedSearch = useDebounced(searchTerm, 500)
 * // debouncedSearch só atualiza 500ms após a última mudança em searchTerm
 */
export function useDebounced<T>(value: T, delay = 300): T {
  const [debounced, setDebounced] = useState(value)

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay)
    return () => clearTimeout(timer)
  }, [value, delay])

  return debounced
}
