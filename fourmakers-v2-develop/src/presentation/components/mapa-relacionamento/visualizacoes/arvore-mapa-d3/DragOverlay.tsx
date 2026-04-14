import { useState, useEffect, useRef } from 'react'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { RootDropZone } from './RootDropZone'
import { InstructionMessage } from './InstructionMessage'
import { GhostCard } from './GhostCard'

export interface DragOverlayProps {
  mousePositionRef: React.MutableRefObject<{ x: number; y: number } | null>
  draggedNode: NoMapaRelacionamento | null
  isDragging: boolean
  dropTarget: NoMapaRelacionamento | null
  isHoveringRootZone: boolean
}

/**
 * Overlay de drag: GhostCard + RootDropZone + InstructionMessage.
 * Lê a posição do mouse de um ref via rAF para não re-renderizar a árvore a cada movimento.
 */
export function DragOverlay({
  mousePositionRef,
  draggedNode,
  isDragging,
  dropTarget,
  isHoveringRootZone,
}: DragOverlayProps) {
  const [posicaoLocal, setPosicaoLocal] = useState<{ x: number; y: number } | null>(null)
  const rafIdRef = useRef<number | null>(null)

  useEffect(() => {
    if (!isDragging) {
      setPosicaoLocal(null)
      if (rafIdRef.current !== null) {
        cancelAnimationFrame(rafIdRef.current)
        rafIdRef.current = null
      }
      return
    }
    const tick = () => {
      if (mousePositionRef.current) {
        setPosicaoLocal({ ...mousePositionRef.current })
      }
      rafIdRef.current = requestAnimationFrame(tick)
    }
    rafIdRef.current = requestAnimationFrame(tick)
    return () => {
      if (rafIdRef.current !== null) {
        cancelAnimationFrame(rafIdRef.current)
        rafIdRef.current = null
      }
    }
  }, [isDragging, mousePositionRef])

  if (!isDragging) return null

  return (
    <>
      <RootDropZone isDragging={isDragging} isHoveringRootZone={isHoveringRootZone} />
      <InstructionMessage
        isDragging={isDragging}
        isHoveringRootZone={isHoveringRootZone}
        dropTarget={dropTarget}
      />
      {draggedNode && posicaoLocal && (
        <GhostCard draggedNode={draggedNode} mousePosition={posicaoLocal} />
      )}
    </>
  )
}
