import { useState, useEffect, useMemo, useRef } from 'react'

export interface VirtualItem {
  index: number
  start: number
  size: number
  end: number
}

/**
 * Virtualização de lista por janela de scroll (windowing) sem dependências externas.
 * Renderiza apenas os itens visíveis + overscan para scroll suave.
 *
 * @param scrollElementRef - Ref do elemento com overflow/scroll (ex.: div com overflow-y-auto)
 * @param count - Número total de itens na lista
 * @param estimateSize - Altura estimada por item em px (default: 120)
 * @param overscan - Quantos itens extras renderizar acima/abaixo da viewport (default: 5)
 * @returns { virtualItems, totalSize } para posicionar e dimensionar os itens
 */
export function useWindowedList(
  scrollElementRef: React.RefObject<HTMLElement | null>,
  count: number,
  estimateSize = 120,
  overscan = 5
): { virtualItems: VirtualItem[]; totalSize: number } {
  const [scrollTop, setScrollTop] = useState(0)
  const [clientHeight, setClientHeight] = useState(0)
  const rafRef = useRef<number | null>(null)

  useEffect(() => {
    const el = scrollElementRef.current
    if (!el) return

    const readDimensions = () => {
      if (rafRef.current != null) cancelAnimationFrame(rafRef.current)
      rafRef.current = requestAnimationFrame(() => {
        rafRef.current = null
        setScrollTop(el.scrollTop)
        setClientHeight(el.clientHeight)
      })
    }

    readDimensions()
    el.addEventListener('scroll', readDimensions, { passive: true })
    const ro = new ResizeObserver(readDimensions)
    ro.observe(el)

    return () => {
      el.removeEventListener('scroll', readDimensions)
      ro.disconnect()
      if (rafRef.current != null) cancelAnimationFrame(rafRef.current)
    }
  }, [scrollElementRef])

  return useMemo(() => {
    const totalSize = count * estimateSize
    if (count === 0 || clientHeight <= 0) {
      return { virtualItems: [], totalSize }
    }

    const startIndex = Math.max(0, Math.floor(scrollTop / estimateSize) - overscan)
    const endIndex = Math.min(
      count - 1,
      Math.ceil((scrollTop + clientHeight) / estimateSize) + overscan
    )

    const virtualItems: VirtualItem[] = []
    for (let i = startIndex; i <= endIndex; i++) {
      virtualItems.push({
        index: i,
        start: i * estimateSize,
        size: estimateSize,
        end: (i + 1) * estimateSize,
      })
    }

    return { virtualItems, totalSize }
  }, [count, estimateSize, overscan, scrollTop, clientHeight])
}
