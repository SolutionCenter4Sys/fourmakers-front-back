import { useCallback, useState } from 'react'

import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

export function useMapaDragDrop() {
  const [noArrastando, setNoArrastando] = useState<NoMapaRelacionamento | null>(null)

  const handleDragStart = useCallback((no: NoMapaRelacionamento) => {
    setNoArrastando(no)
  }, [])

  const handleDragEnd = useCallback(() => {
    setNoArrastando(null)
  }, [])

  const handleDrop = useCallback(
    (noDestino: NoMapaRelacionamento, onMove: (origem: NoMapaRelacionamento, destino: NoMapaRelacionamento) => void) => {
      if (!noArrastando || noArrastando.id === noDestino.id) {
        return
      }

      const ehDescendente = (no: NoMapaRelacionamento, candidato: NoMapaRelacionamento): boolean => {
        if (no.id === candidato.id) return true
        return no.children.some((filho) => ehDescendente(filho, candidato))
      }

      if (ehDescendente(noArrastando, noDestino)) {
        console.warn('Não é possível mover um nó para dentro de si mesmo ou de seus descendentes')
        return
      }

      onMove(noArrastando, noDestino)
      setNoArrastando(null)
    },
    [noArrastando],
  )

  return {
    noArrastando,
    handleDragStart,
    handleDragEnd,
    handleDrop,
  }
}
