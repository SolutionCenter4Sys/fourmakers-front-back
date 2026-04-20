import { useCallback, useRef, useEffect } from 'react'

/**
 * Hook para gerenciar interações de mouse no mapa de relacionamento
 * 
 * IMPORTANTE: Este hook gerencia panning no CONTAINER EXTERNO (viewportRef),
 * que é diferente do panning nativo do react-d3-tree (que funciona apenas no SVG interno).
 * 
 * - react-d3-tree (draggable={true}): Panning dentro do SVG da árvore
 * - useMapaInteracoes: Panning/scroll no container scrollável que envolve o SVG
 * 
 * Ambos são necessários:
 * - O panning do SVG permite mover a visualização da árvore dentro do viewport
 * - O panning do container permite scroll quando a árvore é maior que o viewport
 * 
 * Extrai lógica de pan/scroll que estava inline no OrganogramaPage
 */
export function useMapaInteracoes() {
  const viewportRef = useRef<HTMLDivElement>(null)
  const isPanningRef = useRef(false)
  const panStartRef = useRef<{ x: number; y: number } | null>(null)
  const scrollStartRef = useRef<{ x: number; y: number } | null>(null)

  const handleMouseMove = useCallback((e: MouseEvent) => {
    if (!isPanningRef.current || !panStartRef.current || !scrollStartRef.current || !viewportRef.current) {
      return
    }

    const deltaX = e.clientX - panStartRef.current.x
    const deltaY = e.clientY - panStartRef.current.y

    viewportRef.current.scrollLeft = scrollStartRef.current.x - deltaX
    viewportRef.current.scrollTop = scrollStartRef.current.y - deltaY
    
    // Visual feedback
    if (viewportRef.current) {
      viewportRef.current.style.cursor = 'grabbing'
    }
  }, [])

  const handleMouseDown = useCallback((e: MouseEvent) => {
    const target = e.target as HTMLElement
    
    // Don't pan if clicking on a drag handle
    if (target.closest('[data-drag-handle]')) {
      return
    }
    
    // Don't pan if clicking on a card (this covers buttons inside cards too)
    if (target.closest('[data-mapa-card]')) {
      return
    }
    
    // Allow panning everywhere else (including SVG, even if there are buttons for expand/collapse)
    // The cards and drag handles are the only things that should prevent panning
    if (viewportRef.current?.contains(target)) {
      isPanningRef.current = true
      panStartRef.current = { x: e.clientX, y: e.clientY }
      scrollStartRef.current = {
        x: viewportRef.current?.scrollLeft || 0,
        y: viewportRef.current?.scrollTop || 0,
      }
      e.preventDefault()
    }
  }, [])

  const handleMouseUp = useCallback(() => {
    isPanningRef.current = false
    panStartRef.current = null
    scrollStartRef.current = null
    
    // Reset cursor
    if (viewportRef.current) {
      viewportRef.current.style.cursor = 'grab'
    }
  }, [])

  useEffect(() => {
    // Retry with longer intervals to ensure DOM is mounted
    let retryCount = 0
    const maxRetries = 20
    let timeoutId: NodeJS.Timeout
    
    const attachListeners = () => {
      const viewport = viewportRef.current
      
      if (!viewport) {
        retryCount++
        if (retryCount < maxRetries) {
          // Increase interval progressively
          const delay = retryCount < 5 ? 200 : 500
          timeoutId = setTimeout(attachListeners, delay)
        }
        return
      }

      viewport.addEventListener('mousemove', handleMouseMove)
      viewport.addEventListener('mousedown', handleMouseDown)
      viewport.addEventListener('mouseup', handleMouseUp)
      document.addEventListener('mouseup', handleMouseUp)
    }
    
    timeoutId = setTimeout(attachListeners, 200)

    return () => {
      if (timeoutId) clearTimeout(timeoutId)
      const viewport = viewportRef.current
      if (viewport) {
        viewport.removeEventListener('mousemove', handleMouseMove)
        viewport.removeEventListener('mousedown', handleMouseDown)
        viewport.removeEventListener('mouseup', handleMouseUp)
        document.removeEventListener('mouseup', handleMouseUp)
      }
    }
  }, [handleMouseMove, handleMouseDown, handleMouseUp])

  return {
    viewportRef,
    isPanningRef,
  }
}
