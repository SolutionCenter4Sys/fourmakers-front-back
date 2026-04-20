import { useEffect, useRef } from 'react'

export interface OpcoesLimparBuscaPopover {
  /** Atraso em ms antes de limpar (ex.: 100 para garantir após o dropdown fechar). */
  delay?: number
  /** Se true, só executa a limpeza quando valorAtual não estiver vazio. */
  soLimparSePreenchido?: boolean
}

/**
 * Limpa o(s) estado(s) de busca quando o popover é fechado.
 * Elimina duplicação entre FiltrosAgenda e FiltrosComercial.
 * Usa ref para valorAtual quando soLimparSePreenchido é true, para não reexecutar
 * o effect a cada tecla (evita interferir no scroll do dropdown).
 *
 * @param popoverOpen - Estado de abertura do popover
 * @param setSearch - Setter do estado de busca
 * @param setSearchDebounced - Setter opcional do estado de busca com debounce
 * @param options - delay (ms) e/ou soLimparSePreenchido
 * @param valorAtual - Valor atual da busca (obrigatório se soLimparSePreenchido for true)
 */
export function useLimparBuscaAoFecharPopover(
  popoverOpen: boolean,
  setSearch: (value: string) => void,
  setSearchDebounced?: (value: string) => void,
  options?: OpcoesLimparBuscaPopover,
  valorAtual?: string,
): void {
  const { delay, soLimparSePreenchido } = options ?? {}
  const valorAtualRef = useRef(valorAtual ?? '')
  valorAtualRef.current = valorAtual ?? ''

  useEffect(() => {
    if (popoverOpen) return
    if (soLimparSePreenchido && (valorAtualRef.current == null || valorAtualRef.current === '')) return

    const clear = (): void => {
      setSearch('')
      setSearchDebounced?.('')
    }

    if (delay != null && delay > 0) {
      const timeoutId = setTimeout(clear, delay)
      return () => clearTimeout(timeoutId)
    }
    clear()
  }, [popoverOpen, setSearch, setSearchDebounced, delay, soLimparSePreenchido])
}
