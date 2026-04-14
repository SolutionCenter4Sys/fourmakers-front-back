import { useState, useRef, useEffect } from 'react'
import { throttle } from '@shared/utils/throttle'
import { toast } from 'sonner'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { DragOverlay as DragOverlayComponent } from '@presentation/components/mapa-relacionamento/visualizacoes/arvore-mapa-d3/DragOverlay'

interface UseDragAndDropMapaOptions {
  /**
   * Container ref onde os cards estão renderizados
   */
  containerRef: React.RefObject<HTMLDivElement | null> | React.RefObject<HTMLDivElement>
  
  /**
   * Função para encontrar um nó pelo ID
   * Pode ser uma busca recursiva (árvore) ou linear (lista)
   */
  findNodeById: (nodeId: string) => NoMapaRelacionamento | null | undefined
  
  /**
   * Callback chamado quando um nó é movido
   */
  onMoverNo?: (origem: NoMapaRelacionamento, destino: NoMapaRelacionamento | null) => void
  
  /**
   * Função para verificar se um nó é descendente de outro
   */
  isDescendant: (ancestor: NoMapaRelacionamento, candidate: NoMapaRelacionamento) => boolean
  
  /**
   * Seletor CSS para identificar cards (ex: '[data-mapa-card]' ou '[data-lista-card]')
   */
  cardSelector: string
  
  /**
   * Dependências para re-executar o useEffect quando necessário
   */
  dependencies?: unknown[]
}

/**
 * Hook compartilhado para gerenciar drag-and-drop de nós no Mapa de Relacionamento
 * 
 * Funciona tanto para visualização em árvore quanto em lista, através de funções genéricas
 * para encontrar nós e validar descendência.
 * 
 * @returns Estados e funções para controlar o drag-and-drop
 */
export function useDragAndDropMapa({
  containerRef,
  findNodeById,
  onMoverNo,
  isDescendant,
  cardSelector,
  dependencies = [],
}: UseDragAndDropMapaOptions) {
  const [draggedNode, setDraggedNode] = useState<NoMapaRelacionamento | null>(null)
  const [isDragging, setIsDragging] = useState(false)
  const [dropTarget, setDropTarget] = useState<NoMapaRelacionamento | null>(null)
  const [isHoveringRootZone, setIsHoveringRootZone] = useState(false)
  const dragStartPos = useRef<{ x: number; y: number } | null>(null)
  const mousePositionRef = useRef<{ x: number; y: number } | null>(null)

  useEffect(() => {
    if (!containerRef.current || !onMoverNo) return
    
    const container = containerRef.current
    
    // Variáveis locais (let) ao invés de state para evitar re-renders desnecessários
    // Apenas estados visuais (draggedNode, isDragging, dropTarget) são reativos via useState
    // Estados internos (currentDraggedNode, currentIsDragging, currentDropTarget) são mutáveis
    // sem triggerar re-render, melhorando performance durante o drag
    let currentDraggedNode: NoMapaRelacionamento | null = null
    let currentIsDragging = false
    let currentDropTarget: NoMapaRelacionamento | null = null
    
    const handleMouseDown = (e: MouseEvent) => {
      // Check if click is on a grip icon
      const target = e.target as HTMLElement
      const gripIcon = target.closest('[data-drag-handle]')
      
      // If NOT on grip, allow event to bubble for panning (react-d3-tree will handle it)
      if (!gripIcon) return
      
      const card = gripIcon.closest(cardSelector)
      if (!card) return
      
      const nodeId = card.getAttribute('data-node-id')
      if (!nodeId) return
      
      const foundNode = findNodeById(nodeId)
      if (!foundNode || !foundNode.posicaoId) {
        if (!foundNode?.posicaoId) {
          toast.error('Salve o nó antes de movê-lo')
        }
        return
      }
      
      // NOW we stop propagation (only if we're actually starting a drag)
      // This prevents react-d3-tree from starting its own drag/pan
      e.preventDefault()
      e.stopPropagation()
      
      currentDraggedNode = foundNode
      currentIsDragging = false
      dragStartPos.current = { x: e.clientX, y: e.clientY }
      setDraggedNode(foundNode)
      
      // Change cursor immediately
      document.body.style.cursor = 'grabbing'
    }
    
    // Throttled mouse move handler para melhorar performance (~60fps)
    // Apenas throttla a parte visual (setState), não a detecção de início de drag
    const handleMouseMove = (e: MouseEvent) => {
      if (!currentDraggedNode || !dragStartPos.current) return
      
      // Check if moved enough to start drag (5px threshold) - SEM throttle, precisa ser imediato
      const deltaX = Math.abs(e.clientX - dragStartPos.current.x)
      const deltaY = Math.abs(e.clientY - dragStartPos.current.y)
      
      if (!currentIsDragging && (deltaX > 5 || deltaY > 5)) {
        currentIsDragging = true
        setIsDragging(true)
      }
      
      if (currentIsDragging) {
        // Atualiza ref (não state) para não re-renderizar o pai a cada movimento; DragOverlay lê via rAF
        mousePositionRef.current = { x: e.clientX, y: e.clientY }
        
        // Check if hovering over root zone (top 100px of container)
        const containerRect = container.getBoundingClientRect()
        const isInRootZone = e.clientY < containerRect.top + 100
        setIsHoveringRootZone(isInRootZone)
        
        if (isInRootZone) {
          // Clear drop target when in root zone
          if (currentDropTarget) {
            currentDropTarget = null
            setDropTarget(null)
          }
        } else {
          // Find card under mouse
          const elementUnderMouse = document.elementFromPoint(e.clientX, e.clientY)
          const card = elementUnderMouse?.closest(cardSelector)
          
          if (card) {
            const nodeId = card.getAttribute('data-node-id')
            if (nodeId && nodeId !== currentDraggedNode.id) {
              const targetNode = findNodeById(nodeId)
              if (targetNode && targetNode.id !== currentDropTarget?.id) {
                currentDropTarget = targetNode
                setDropTarget(targetNode)
              }
            }
          } else if (currentDropTarget) {
            currentDropTarget = null
            setDropTarget(null)
          }
        }
      }
    }
    
    // Criar versão throttled do handler (16ms = ~60fps)
    const throttledHandleMouseMove = throttle(handleMouseMove, 16)
    
    const handleMouseUp = (e: MouseEvent) => {
      if (!currentDraggedNode) return
      
      // Check if dropped in root zone
      const containerRect = container.getBoundingClientRect()
      const isInRootZone = e.clientY < containerRect.top + 100
      
      if (currentIsDragging) {
        if (isInRootZone) {
          // Move to root (no parent)
          onMoverNo(currentDraggedNode, null)
          toast.success(`${currentDraggedNode.employeeName || currentDraggedNode.profileName} movido para raiz!`)
        } else if (currentDropTarget) {
          // Validate
          if (currentDraggedNode.id === currentDropTarget.id) {
            toast.error('Não é possível mover um nó para si mesmo')
          } else if (isDescendant(currentDraggedNode, currentDropTarget)) {
            toast.error('Não é possível mover para um descendente')
          } else {
            onMoverNo(currentDraggedNode, currentDropTarget)
            toast.success(`${currentDraggedNode.employeeName || currentDraggedNode.profileName} movido!`)
          }
        } else {
          // Drag was cancelled (dropped outside a valid target)
          toast.info('Arraste cancelado. Solte sobre outro card para mover.')
        }
      }
      
      // Reset cursor
      document.body.style.cursor = ''
      
      // Reset state
      currentDraggedNode = null
      currentIsDragging = false
      currentDropTarget = null
      dragStartPos.current = null
      setDraggedNode(null)
      setIsDragging(false)
      setDropTarget(null)
      mousePositionRef.current = null
      setIsHoveringRootZone(false)
    }
    
    // Use CAPTURE phase to intercept grip clicks before react-d3-tree
    // But we only stopPropagation when it's actually a grip click
    // This allows panning to work normally for all other clicks
    container.addEventListener('mousedown', handleMouseDown, true)
    document.addEventListener('mousemove', throttledHandleMouseMove)
    document.addEventListener('mouseup', handleMouseUp)
    
    return () => {
      container.removeEventListener('mousedown', handleMouseDown, true)
      document.removeEventListener('mousemove', throttledHandleMouseMove)
      document.removeEventListener('mouseup', handleMouseUp)
      // Reset cursor on cleanup
      document.body.style.cursor = ''
    }
  }, [containerRef, findNodeById, onMoverNo, isDescendant, cardSelector, ...dependencies])

  return {
    draggedNode,
    isDragging,
    dropTarget,
    isHoveringRootZone,
    mousePositionRef,
    DragOverlay: DragOverlayComponent,
  }
}
