import { useMemo, useRef, useEffect, useState, useCallback, useImperativeHandle, forwardRef, memo, useContext } from 'react'
import { createPortal } from 'react-dom'
import type { HierarchyPointNode } from 'd3-hierarchy'
import Tree from 'react-d3-tree'
import type { CustomNodeElementProps, RawNodeDatum, TreeLinkDatum, TreeNodeDatum } from 'react-d3-tree'
import { useAppSelector } from '@app/store/hooks'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { useDragAndDropMapa } from '@presentation/hooks/useDragAndDropMapa'
import { Badge } from '@/components/ui/badge'
import { Card } from '@/components/ui/card'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { Button } from '@/components/ui/button'
import { Spinner } from '@/components/ui/spinner'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import { Crown, GripVertical, Edit, UserPlus, Eye, Trash, ChevronDown } from 'lucide-react'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { getAllNodes, countAllDescendants, convertToD3Tree, type D3TreeNode } from './arvoreMapaD3Helpers'
import { MapaRelacionamentoTreeContext } from './MapaRelacionamentoTreeContext'
import { isPosicaoVaga, getInitials } from '@shared/utils/mapaRelacionamentoUtils'

interface ArvoreMapaD3Props {
  no: NoMapaRelacionamento
  /** Incrementado após atualização otimista para forçar remount do Tree e exibir dados novos no card */
  treeDataVersion?: number
  onSelecionarNo: (no: NoMapaRelacionamento | null) => void
  onEditarNo: (no: NoMapaRelacionamento) => void
  onAdicionarFilho?: (pai: NoMapaRelacionamento) => void
  onMoverNo: (origem: NoMapaRelacionamento, destino: NoMapaRelacionamento | null) => void
  onDelete?: (no: NoMapaRelacionamento) => void
}

export interface ArvoreMapaD3Ref {
  centralizarArvore: () => void
  zoomIn: () => void
  zoomOut: () => void
}

/** Props do nó customizado: base do react-d3-tree (com onNodeClick nosso) + props do mapa de relacionamento */
interface CustomNodeComponentProps extends Omit<CustomNodeElementProps, 'onNodeClick'> {
  onNodeClick?: (nodeOrHierarchy: HierarchyPointNode<TreeNodeDatum> | TreeNodeDatum) => void
  onEditClick?: (no: NoMapaRelacionamento) => void
  onViewDetails?: (no: NoMapaRelacionamento) => void
  onAddChild?: (no: NoMapaRelacionamento) => void
  onDelete?: (no: NoMapaRelacionamento) => void
  onToggleCollapse?: (nodeId: string) => void
  isCollapsed?: boolean
  draggedNodeId?: string | null
  dropTargetNodeId?: string | null
  isDragging?: boolean
}

// Limites e passo de zoom para evitar zoom "infinito" e comportamento previsível
const ZOOM_MIN = 0.15
const ZOOM_MAX = 2.5
const ZOOM_INCREMENTO = 0.25
const ZOOM_INICIAL = 0.5

// Multi-Departamento Tooltip component that follows mouse pointer
function MultiDepartamentoTooltip({ otherDepts }: { otherDepts: NoMapaRelacionamento[] }) {
  const [isHovered, setIsHovered] = useState(false)
  const [mousePosition, setMousePosition] = useState({ x: 0, y: 0 })
  const [anchorRect, setAnchorRect] = useState<DOMRect | null>(null)
  const [portalEl, setPortalEl] = useState<HTMLElement | null>(null)

  // Create a portal container in the document body to avoid SVG transform offsets
  useEffect(() => {
    const el = document.createElement('div')
    el.setAttribute('data-multi-dept-tooltip', 'true')
    document.body.appendChild(el)
    setPortalEl(el)
    return () => {
      document.body.removeChild(el)
    }
  }, [])

  const handleMouseEnter = (e: React.MouseEvent) => {
    setIsHovered(true)
    setMousePosition({ x: e.clientX, y: e.clientY })
    setAnchorRect((e.currentTarget as HTMLElement).getBoundingClientRect())
  }

  const handleMouseMove = (e: React.MouseEvent) => {
    setMousePosition({ x: e.clientX, y: e.clientY })
  }

  const handleMouseLeave = () => {
    setIsHovered(false)
  }

  // Calculate tooltip position to follow cursor and avoid going off-screen
  const getTooltipStyle = () => {
    const offset = 10 // Small offset from cursor
    const tooltipWidth = 224 // w-56 = 14rem = 224px
    const tooltipHeight = 150 // approximate max height
    
    // Prefer cursor position; fallback to the trigger element's rect
    let left = mousePosition.x
    let top = mousePosition.y
    if ((!left && !top) && anchorRect) {
      left = anchorRect.right
      top = anchorRect.top + anchorRect.height / 2
    }

    // Default: position to the right and slightly below the cursor
    left = left + offset
    top = top + offset

    // Check if tooltip would go off right edge - flip to left side of cursor
    if (left + tooltipWidth > window.innerWidth) {
      left = mousePosition.x - tooltipWidth - offset
    }

    // Check if tooltip would go off bottom edge - show above cursor instead
    if (top + tooltipHeight > window.innerHeight) {
      top = mousePosition.y - tooltipHeight - offset
    }

    // Check if tooltip would go off left edge - constrain to left edge
    if (left < 0) {
      left = offset
    }

    // Check if tooltip would go off top edge - constrain to top edge
    if (top < 0) {
      top = offset
    }

    return {
      position: 'fixed' as const,
      left: `${left}px`,
      top: `${top}px`,
      pointerEvents: 'none' as const,
      transform: 'none', // Ensure no transforms are applied
    }
  }

  return (
    <>
      <div 
        className="absolute -top-1 -right-1 z-20"
        onMouseEnter={handleMouseEnter}
        onMouseMove={handleMouseMove}
        onMouseLeave={handleMouseLeave}
      >
        <div className="w-5 h-5 rounded-full bg-primary text-primary-foreground text-xs font-bold flex items-center justify-center border-2 border-card shadow-sm cursor-help">
          {otherDepts.length}
        </div>
      </div>
      {isHovered && portalEl && createPortal(
        <div
          className="z-[9999] w-56 p-3 text-left bg-popover border border-border rounded-md shadow-md"
          style={getTooltipStyle()}
        >
          <p className="text-xs font-bold uppercase tracking-widest text-primary mb-2 pb-1 border-b border-border">
            Multi-Departamento
          </p>
          <div className="space-y-2">
            {otherDepts.map((d, i) => (
              <div key={i} className="flex flex-col">
                <span className="text-xs font-bold text-foreground">
                  {d.departmentName || 'Institucional'}
                </span>
                <span className="text-xs text-muted-foreground font-medium italic">
                  {d.profileName || 'Sem perfil'}
                </span>
              </div>
            ))}
          </div>
        </div>,
        portalEl,
      )}
    </>
  )
}

// Custom node component — allNodes via Context (evita parse de allNodesData em cada nó)
function CustomNodeComponent({ 
  nodeDatum, 
  onNodeClick: _onNodeClick, 
  onEditClick, 
  onViewDetails: _onViewDetails, 
  onAddChild, 
  onDelete, 
  onToggleCollapse, 
  isCollapsed,
  draggedNodeId,
  dropTargetNodeId,
  isDragging
}: CustomNodeComponentProps) {
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)
  const { allNodes } = useContext(MapaRelacionamentoTreeContext)
  const datum = nodeDatum as unknown as D3TreeNode

  // Reconstruct full node from stored data (nodeData only; allNodes from Context)
  let no: NoMapaRelacionamento
  try {
    if (datum.attributes.nodeData) {
      no = JSON.parse(datum.attributes.nodeData)
    } else {
      // Fallback to attributes if nodeData not available
      no = {
        id: datum.attributes.id,
        profileId: datum.attributes.profileId,
        employeeId: datum.attributes.employeeId,
        profileName: datum.attributes.profileName ?? undefined,
        employeeName: datum.name ?? '',
        employeeEmail: datum.attributes.employeeEmail ?? undefined,
        departmentName: datum.attributes.departmentName ?? undefined,
        isCLevel: datum.attributes.isCLevel,
        isExternal: datum.attributes.isExternal,
        wasConnected: datum.attributes.wasConnected,
        children: [],
      }
    }
  } catch {
    // Fallback if parsing fails
    no = {
      id: datum.attributes.id,
      profileId: datum.attributes.profileId,
      employeeId: datum.attributes.employeeId,
      profileName: datum.attributes.profileName ?? undefined,
      employeeName: datum.attributes.profileName ?? datum.name ?? '',
      employeeEmail: datum.attributes.employeeEmail ?? undefined,
      departmentName: datum.attributes.departmentName ?? undefined,
      isCLevel: datum.attributes.isCLevel,
      isExternal: datum.attributes.isExternal,
      wasConnected: datum.attributes.wasConnected,
      children: [],
    }
  }

  const isVacant = isPosicaoVaga(no.employeeId)
  // hasValidEmployee: true apenas se tem employeeId válido (não é vaga) E tem nome
  const hasValidEmployee = !isVacant && !!no.employeeId && !!no.employeeName?.trim()
  
  // Skip debug log for virtual root node
  if (no.id === 'root-virtual') {
    // Virtual root node doesn't need posicaoId
  }
  
  // Get original children state from attributes (before filtering)
  // This is critical - we must check attributes, not the filtered node's children
  // Check both the boolean flag and fallback to checking original node data
  let hasOriginalChildren = datum.attributes.hasOriginalChildren === true
  if (!hasOriginalChildren && datum.attributes.nodeData) {
    try {
      const originalNode = JSON.parse(datum.attributes.nodeData)
      hasOriginalChildren = originalNode.children && originalNode.children.length > 0
    } catch {
      // Fallback to current node if parsing fails
      hasOriginalChildren = no.children && no.children.length > 0
    }
  }
  // Calculate descendant count from original node data (before filtering)
  const descendantCount = useMemo(() => {
    // Always use the original node data from attributes to get accurate count
    try {
      if (datum.attributes.nodeData) {
        const originalNode = JSON.parse(datum.attributes.nodeData)
        return countAllDescendants(originalNode)
      }
      return countAllDescendants(no)
    } catch {
      return countAllDescendants(no)
    }
  }, [datum.attributes.nodeData, no])
  
  // otherDepts: apenas calcular se NÃO for vaga (não mostrar badge de multi-departamento para vagas)
  const otherDepts = useMemo(() => {
    // IMPORTANTE: Não calcular outros departamentos se for vaga
    if (isVacant || !hasValidEmployee) return []
    // Filtrar apenas nós que têm o mesmo employeeId válido (não vaga)
    return allNodes.filter((n) => {
      if (!n || !n.employeeId || isPosicaoVaga(n.employeeId)) return false
      return n.employeeId === no.employeeId && n.id !== no.id
    })
  }, [no.employeeId, no.id, allNodes, hasValidEmployee, isVacant])

  const profileTitle = no.profileName || 'Perfil Corporativo'
  const departmentLabel = no.departmentName || 'DEPARTAMENTO'
  
  // Only log once per node on first render
  if (no.id !== 'root-virtual') {
    // console.log('Node:', no.id, 'posicaoId:', no.posicaoId, 'handlers:', !!onGripMouseDown)
  }
  
  // Hide virtual root node (created when there are multiple root nodes)
  // Make it invisible and with zero size so it doesn't take up space
  // but still exists in the tree structure for proper layout
  if (no.id === 'root-virtual') {
    return (
      <g style={{ opacity: 0, pointerEvents: 'none' }}>
        <foreignObject x={-170} y={-120} width={340} height={0} overflow="visible">
          <div style={{ width: 0, height: 0 }} />
        </foreignObject>
      </g>
    )
  }

  return (
    <g>
      <foreignObject 
        x={-170} 
        y={-120} 
        width={340} 
        height={240} 
        overflow="visible"
        style={{ pointerEvents: 'all' }}
      >
        <div className="w-full h-full flex items-center justify-center p-2 relative">
          <Card
            data-mapa-card
            data-node-id={no.id}
            className={`w-[340px] max-w-sm bg-card border rounded-lg p-5 shadow-sm transition-all duration-200 relative z-10 text-left cursor-default hover:shadow-md hover:z-[100] ${
              draggedNodeId === no.id && isDragging
                ? 'opacity-40 border-dashed border-2' 
                : dropTargetNodeId === no.id && isDragging
                  ? 'ring-4 ring-success ring-offset-2 border-success scale-105' 
                  : 'border-border hover:border-primary'
            }`}
          >
            {/* Header */}
            <div className="flex justify-between items-start mb-3">
              <div className="flex flex-col gap-0.5 flex-1 min-w-0">
                <span className="text-xs text-primary font-bold uppercase tracking-widest truncate">
                  {departmentLabel}
                </span>
                <span className="text-xs text-muted-foreground font-bold uppercase tracking-tighter opacity-70">
                  PERFIL DE ATUAÇÃO
                </span>
                <div className="flex items-center gap-2 group/title mt-0.5">
                  <div className="text-sm font-bold text-foreground leading-tight truncate pr-2" title={profileTitle}>
                    {profileTitle}
                  </div>
                  {no.isCLevel && (
                    <div className="text-warning shrink-0" title="Perfil C-Level">
                      <Crown className="w-4 h-4" />
                    </div>
                  )}
                </div>
              </div>
              {no.posicaoId ? (
                <div
                  data-drag-handle
                  className="text-muted-foreground hover:text-primary p-1 rounded-full transition-all shrink-0 cursor-grab active:cursor-grabbing"
                  title="Arrastar para mover"
                >
                  <GripVertical className="w-5 h-5 pointer-events-none" />
                </div>
              ) : (
                <div
                  className="text-muted-foreground p-1 rounded-full transition-all shrink-0 cursor-default opacity-50"
                  title="Salve o nó antes de mover"
                >
                  <GripVertical className="w-5 h-5 pointer-events-none" />
                </div>
              )}
            </div>

            {/* Divider */}
            <div className="h-px bg-border w-full mb-4" />

            {/* Avatar and Employee Info */}
            <div className="flex items-center gap-3 mb-5">
              <div className="shrink-0 relative">
                <Avatar className="h-12 w-12">
                  <AvatarFallback className="text-base font-semibold bg-primary/10 text-primary">
                    {getInitials(no.employeeName)}
                  </AvatarFallback>
                </Avatar>
                {hasValidEmployee && otherDepts.length > 0 && (
                  <MultiDepartamentoTooltip otherDepts={otherDepts} />
                )}
              </div>
              <div className="min-w-0 flex-1">
                <div className={`font-bold text-sm truncate ${isVacant ? 'text-muted-foreground italic opacity-60' : 'text-foreground'}`}>
                  {isVacant ? 'Perfil Vago' : no.employeeName}
                </div>
                {!isVacant && no.employeeEmail && (
                  <div className="text-xs text-muted-foreground truncate font-medium" title={no.employeeEmail}>
                    {no.employeeEmail}
                  </div>
                )}
              </div>
            </div>

            {/* Action Buttons */}
            <div className="flex justify-between items-center gap-1.5 pt-2 border-t border-border">
              <div className="flex-1 flex justify-start">
                {descendantCount > 0 && (
                  <Badge variant="secondary" className="text-xs font-black uppercase tracking-widest">
                    GERENCIA {descendantCount} PERFIS
                  </Badge>
                )}
              </div>
              <div className="flex gap-1.5">
                <Button
                  type="button"
                  size="icon"
                  variant="outline"
                  onClick={(e) => {
                    e.stopPropagation()
                    if (!isDragging) {
                      onEditClick?.(no)
                    }
                  }}
                  className="h-8 w-8"
                  title="Editar Perfil de atuação"
                  disabled={isDragging}
                >
                  <Edit className="w-4 h-4" />
                </Button>
                <Button
                  type="button"
                  size="icon"
                  variant="outline"
                  onClick={(e) => {
                    e.stopPropagation()
                    if (!isDragging) {
                      onAddChild?.(no)
                    }
                  }}
                  className="h-8 w-8"
                  title="Adicionar Subordinado"
                  disabled={isDragging}
                >
                  <UserPlus className="w-4 h-4" />
                </Button>
                <Button
                  type="button"
                  size="icon"
                  variant="outline"
                  onClick={(e) => {
                    e.stopPropagation()
                    if (!isDragging && _onViewDetails) {
                      _onViewDetails(no)
                    }
                  }}
                  className="h-8 w-8"
                  title="Visualizar VCX 360"
                  disabled={isDragging}
                >
                  <Eye className="w-4 h-4" />
                </Button>
                {onDelete && (
                  <>
                    <Button
                      type="button"
                      size="icon"
                      variant="outline"
                      onClick={(e) => {
                        e.stopPropagation()
                        if (!isDragging) {
                          setShowDeleteDialog(true)
                        }
                      }}
                      className="h-8 w-8 border-destructive/20 text-destructive hover:bg-destructive hover:text-destructive-foreground disabled:opacity-50 disabled:cursor-not-allowed"
                      title="Remover"
                      disabled={isDragging}
                    >
                      <Trash className="w-4 h-4" />
                    </Button>
                    <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>Remover Perfil de atuação</AlertDialogTitle>
                          <AlertDialogDescription>
                            {no.children && no.children.length > 0
                              ? 'Esta posição possui posições filhas. Ao remover, as posições filhas serão realocadas para o nível superior. Tem certeza que deseja continuar?'
                              : 'Tem certeza que deseja remover este perfil de atuação? Esta ação não pode ser desfeita.'}
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancelar</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={(e) => {
                              e.stopPropagation()
                              onDelete?.(no)
                              setShowDeleteDialog(false)
                            }}
                            className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                          >
                            Remover
                          </AlertDialogAction>
                        </AlertDialogFooter>
                      </AlertDialogContent>
                    </AlertDialog>
                  </>
                )}
              </div>
            </div>
          </Card>
          
          {/* Collapse/Expand Button */}
          {/* Use original children data from attributes, not filtered children */}
          {hasOriginalChildren && (
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={(e) => {
                e.stopPropagation()
                if (!isDragging) {
                  onToggleCollapse?.(no.id)
                }
              }}
              className={`absolute -bottom-3 left-1/2 -translate-x-1/2 h-7 px-3 rounded-full bg-card border shadow-md flex items-center justify-center gap-1.5 transition-all z-20 hover:scale-110 active:scale-95 ${
                isCollapsed ? 'border-primary ring-2 ring-primary/10' : 'border-border'
              }`}
              title={isCollapsed ? 'Expandir' : 'Colapsar'}
              disabled={isDragging}
            >
              <div
                className={`transition-transform duration-300 ${
                  isCollapsed
                    ? 'rotate-180 text-primary'
                    : 'text-muted-foreground'
                }`}
              >
                <ChevronDown className="w-3.5 h-3.5" />
              </div>
              {isCollapsed && (
                <span className="text-xs font-bold text-primary uppercase tracking-tighter">
                  +{descendantCount}
                </span>
              )}
            </Button>
          )}
        </div>
      </foreignObject>
    </g>
  )
}

const CustomNode = memo(
  CustomNodeComponent,
  (prevProps, nextProps) => {
    // Comparação customizada para evitar re-renders desnecessários
    const prevAttrs = (prevProps.nodeDatum as unknown as D3TreeNode).attributes
    const nextAttrs = (nextProps.nodeDatum as unknown as D3TreeNode).attributes
    return (
      prevAttrs?.id === nextAttrs?.id &&
      prevProps.draggedNodeId === nextProps.draggedNodeId &&
      prevProps.dropTargetNodeId === nextProps.dropTargetNodeId &&
      prevProps.isCollapsed === nextProps.isCollapsed &&
      prevProps.isDragging === nextProps.isDragging
    )
  }
)

// Custom path function that always connects from above
// Creates L-shaped paths: down from parent bottom, horizontal, then down to child top
function customPathFromAbove(linkData: TreeLinkDatum) {
  if (!linkData || !linkData.source || !linkData.target) {
    // Return empty path if data is invalid - react-d3-tree will handle it
    return ''
  }
  
  const { source, target } = linkData
  
  // Hide connector lines above root nodes (nodes with no parent or parent is virtual root)
  // Check if source is virtual root
  const sourceId = (source.data?.attributes as Record<string, string> | undefined)?.id ?? (source.data as { id?: string })?.id
  
  // If source is virtual root, hide the line (no connector above root nodes)
  if (sourceId === 'root-virtual') {
    return ''
  }
  
  // Validate coordinates
  if (typeof source.x !== 'number' || typeof source.y !== 'number' ||
      typeof target.x !== 'number' || typeof target.y !== 'number') {
    return ''
  }
  
  // Node size: { x: 350, y: 280 }
  // Card is 340px wide, 240px tall, foreignObject at y={-120}
  // In D3 tree coordinates, node center is at (source.x, source.y)
  // Card spans from y=-120 to y=+120 relative to node center
  // So card top is at source.y - 120, card bottom is at source.y + 120
  
  // Connection points: parent bottom center, child top center
  const cardHalfHeight = 120 // Half of card height (240/2)
  const verticalGap = 10 // Small gap to ensure line is visible beyond card edge
  
  // Start from bottom center of parent node (slightly below card bottom for visibility)
  const x1 = source.x
  const y1 = source.y + cardHalfHeight + verticalGap
  
  // End at top center of child node (slightly above card top for visibility)
  const x2 = target.x
  const y2 = target.y - cardHalfHeight - verticalGap
  
  // Calculate horizontal segment position - place it closer to child for better visibility
  // This creates a clear L-shape with visible vertical segment
  const horizontalY = y1 + (y2 - y1) * 0.6 // Position horizontal segment closer to child
  
  // Create L-shaped path with explicit segments:
  // 1. Move to parent bottom (M)
  // 2. Line down to horizontal level (L)
  // 3. Line horizontal to child x position (L)
  // 4. Line down to child top (L) - this is the vertical line that should be visible
  const path = `M ${x1} ${y1} L ${x1} ${horizontalY} L ${x2} ${horizontalY} L ${x2} ${y2}`
  
  return path
}

const ArvoreMapaD3Component = forwardRef<ArvoreMapaD3Ref, ArvoreMapaD3Props>(
  ({ no, treeDataVersion = 0, onSelecionarNo, onEditarNo, onAdicionarFilho, onMoverNo, onDelete }, ref) => {
    const { user } = useAppSelector((state) => state.auth)
    // Otimização: cache baseado em ID e estrutura de filhos para evitar recálculos desnecessários
    const allNodes = useMemo(() => getAllNodes(no), [no])
    const [collapsedNodes, setCollapsedNodes] = useState<Set<string>>(new Set())
    const containerRef = useRef<HTMLDivElement>(null)
  const [dimensions, setDimensions] = useState({ width: 0, height: 0 })
  const [translate, setTranslate] = useState({ x: 0, y: 0 })
  const [scale, setScale] = useState(1)
  // Evita recentralizar e dar zoom ao atualizar o mapa (mover/editar card); só aplica na primeira carga do root
  const initialViewAppliedForRootIdRef = useRef<string | null>(null)

    // Store original tree structure to preserve children info
    const originalTree = useMemo(() => no, [no])

  const centralizarArvore = useCallback(() => {
    if (containerRef.current) {
      const { width } = containerRef.current.getBoundingClientRect()
      setTranslate({ x: width / 2, y: 50 })
    }
  }, [])

  // Mantém translate e scale controlados para que, ao atualizar data (mover/editar), a biblioteca use estes valores e não resete a vista.
  // Usa functional update com preservação de referência: quando os valores são iguais (caso do onUpdate vindo de componentDidUpdate),
  // retorna o mesmo objeto prev para que o React não enfileire um re-render, quebrando o loop:
  // componentDidUpdate → onUpdate → setTranslate(novoObj) → re-render → componentDidUpdate → …
  const handleTreeUpdate = useCallback((payload: { node?: unknown; translate?: { x: number; y: number }; zoom?: number }) => {
    if (payload.translate != null) {
      setTranslate(prev =>
        prev.x === payload.translate!.x && prev.y === payload.translate!.y ? prev : payload.translate!
      )
    }
    if (payload.zoom != null) {
      const novoZoom = Math.max(ZOOM_MIN, Math.min(ZOOM_MAX, payload.zoom))
      setScale(prev => (prev === novoZoom ? prev : novoZoom))
    }
  }, [])

  // Zoom por incremento direto no state (previsível, um passo por clique, respeitando limites)
  const zoomIn = useCallback(() => {
    setScale(prev => Math.min(ZOOM_MAX, prev + ZOOM_INCREMENTO))
  }, [])

  const zoomOut = useCallback(() => {
    setScale(prev => Math.max(ZOOM_MIN, prev - ZOOM_INCREMENTO))
  }, [])

  // Expose functions via ref
  useImperativeHandle(ref, () => ({
    centralizarArvore,
    zoomIn,
    zoomOut,
  }), [centralizarArvore, zoomIn, zoomOut])

  // Function to filter collapsed nodes from tree
  const filterCollapsedNodes = useCallback((node: NoMapaRelacionamento): NoMapaRelacionamento | null => {
    if (collapsedNodes.has(node.id)) {
      // Return node without children when collapsed
      return { ...node, children: [] }
    }
    // Recursively filter children
    return {
      ...node,
      children: node.children.map(filterCollapsedNodes).filter((n): n is NoMapaRelacionamento => n !== null),
    }
  }, [collapsedNodes])

  const filteredTree = useMemo(() => {
    const filtered = filterCollapsedNodes(no)
    return filtered || no
  }, [no, filterCollapsedNodes])

  // Helper to find original node by ID in the original tree
  const findOriginalNode = useCallback((nodeId: string, tree: NoMapaRelacionamento): NoMapaRelacionamento | null => {
    if (tree.id === nodeId) return tree
    for (const child of tree.children) {
      const found = findOriginalNode(nodeId, child)
      if (found) return found
    }
    return null
  }, [])

  // Convert with original node reference for each node
  // This ensures hasOriginalChildren is always based on the original tree structure
  const treeData = useMemo(() => {
    const convertWithOriginal = (filteredNode: NoMapaRelacionamento, originalNode: NoMapaRelacionamento): D3TreeNode => {
      // Convert this node using the original node's structure
      const result = convertToD3Tree(filteredNode, allNodes, originalNode)
      // Recursively convert children with their original references
      result.children = filteredNode.children.map((filteredChild) => {
        // Find the corresponding original child
        const originalChild = originalNode.children?.find(c => c.id === filteredChild.id)
        if (originalChild) {
          // Recursively convert with original reference
          return convertWithOriginal(filteredChild, originalChild)
        }
        // Fallback if original child not found (shouldn't happen, but safety check)
        return convertToD3Tree(filteredChild, allNodes)
      })
      return result
    }
    
    // If the root is a virtual root (id === 'root-virtual'), keep the structure intact
    // react-d3-tree will automatically render siblings side by side
    // The virtual root will be completely invisible (no parent node visible)
    // and its children (the actual root nodes) will appear side by side at the top
    if (filteredTree && filteredTree.id === 'root-virtual') {
      // Convert the virtual root as-is - its children will be rendered side by side
      // The virtual root itself will be completely invisible in CustomNode component
      return convertWithOriginal(filteredTree, originalTree)
    }
    
    return convertWithOriginal(filteredTree, originalTree)
  }, [filteredTree, originalTree, allNodes])

  // Memoizar objetos de configuração para evitar re-bind do zoom listener em todo render
  const separationConfig = useMemo(() => ({ siblings: 1.5, nonSiblings: 1.3 }), [])
  const nodeSizeConfig = useMemo(() => ({ x: 350, y: 280 }), [])

  // Só define translate na primeira vez que as dimensões são obtidas; resize não deve resetar pan/zoom do usuário
  const initialDimensionsSetRef = useRef(false)

  // Initialize dimensions and center on mount
  useEffect(() => {
    const updateDimensions = () => {
      if (containerRef.current) {
        // Get parent container dimensions if available
        const parent = containerRef.current.parentElement
        const parentRect = parent?.getBoundingClientRect()
        
        // Use parent dimensions or fallback to container itself (no minimum that exceeds viewport to avoid cutoff)
        const width = parentRect?.width || containerRef.current.getBoundingClientRect().width || containerRef.current.clientWidth || window.innerWidth
        const height = parentRect?.height || containerRef.current.getBoundingClientRect().height || containerRef.current.clientHeight || window.innerHeight
        
        // Small minimum only to avoid invalid/zero dimensions on first paint; do not force 800x600 so tree fits viewport
        const finalWidth = Math.max(width, 200)
        const finalHeight = Math.max(height, 200)
        
        if (finalWidth > 0 && finalHeight > 0) {
          setDimensions({ width: finalWidth, height: finalHeight })
          if (!initialDimensionsSetRef.current) {
            initialDimensionsSetRef.current = true
            setTranslate({ x: finalWidth / 2, y: 50 })
          }
        }
      }
    }

    // Tentativa inicial
    updateDimensions()

    // Um único retry (150ms) se container ainda não tiver dimensões; evita múltiplos setState no mount
    const timerRetry = setTimeout(() => {
      if (containerRef.current) {
        const w = containerRef.current.getBoundingClientRect().width || containerRef.current.clientWidth
        if (w === 0) updateDimensions()
      }
    }, 150)
    
    // Use ResizeObserver for better dimension tracking
    let resizeObserver: ResizeObserver | null = null
    if (containerRef.current && typeof ResizeObserver !== 'undefined') {
      resizeObserver = new ResizeObserver(() => {
        updateDimensions()
      })
      resizeObserver.observe(containerRef.current)
      // Also observe parent if available
      if (containerRef.current.parentElement) {
        resizeObserver.observe(containerRef.current.parentElement)
      }
    }
    
    // Also listen for resize events
    window.addEventListener('resize', updateDimensions)
    
    return () => {
      clearTimeout(timerRetry)
      window.removeEventListener('resize', updateDimensions)
      if (resizeObserver) {
        if (containerRef.current) {
          resizeObserver.unobserve(containerRef.current)
        }
        if (containerRef.current?.parentElement) {
          resizeObserver.unobserve(containerRef.current.parentElement)
        }
      }
    }
  }, [])

  // Centraliza e aplica zoom inicial apenas na primeira carga deste root (troca de cliente ou abertura do mapa).
  // Ao mover/editar um card, no.id permanece o mesmo; não recentralizar para manter coordenadas e zoom do usuário.
  useEffect(() => {
    if (dimensions.width <= 0) return
    const alreadyAppliedForThisRoot = initialViewAppliedForRootIdRef.current === no.id
    if (alreadyAppliedForThisRoot) return

    initialViewAppliedForRootIdRef.current = no.id
    let timerZoom: ReturnType<typeof setTimeout> | null = null
    const timer = setTimeout(() => {
      centralizarArvore()
      timerZoom = setTimeout(() => {
        setScale(ZOOM_INICIAL)
      }, 150)
    }, 100)
    return () => {
      clearTimeout(timer)
      if (timerZoom != null) clearTimeout(timerZoom)
    }
  }, [no.id, dimensions.width, centralizarArvore])

  // Recalculate dimensions when structure changes or when container becomes visible
  useEffect(() => {
    if (containerRef.current) {
      const rect = containerRef.current.getBoundingClientRect()
      const width = rect.width || containerRef.current.clientWidth || window.innerWidth
      const height = rect.height || containerRef.current.clientHeight || window.innerHeight
      
      if ((dimensions.width === 0 || dimensions.height === 0) && width > 0 && height > 0) {
        setDimensions({ width, height })
        setTranslate({ x: width / 2, y: 50 })
      }
    }
  }, [no.id, dimensions.width, dimensions.height])

  const handleNodeClick = (nodeOrHierarchy: HierarchyPointNode<TreeNodeDatum> | TreeNodeDatum) => {
    // react-d3-tree pode passar HierarchyPointNode (node) ou o datum; extrair datum para uso uniforme
    const nodeData = nodeOrHierarchy && 'data' in nodeOrHierarchy ? (nodeOrHierarchy as HierarchyPointNode<TreeNodeDatum>).data : (nodeOrHierarchy as TreeNodeDatum)
    let noData: NoMapaRelacionamento
    try {
      if (nodeData?.attributes?.nodeData) {
        noData = JSON.parse(nodeData.attributes.nodeData as string)
      } else if (nodeData?.attributes?.id) {
        // Fallback to attributes
        const attrs = nodeData.attributes as Record<string, string | number | boolean>
        noData = {
          id: String(attrs.id),
          profileId: attrs.profileId != null ? String(attrs.profileId) : null,
          employeeId: attrs.employeeId != null ? String(attrs.employeeId) : null,
          profileName: attrs.profileName != null ? String(attrs.profileName) : undefined,
          employeeName: (attrs.name as string) || nodeData.name || '',
          employeeEmail: attrs.employeeEmail != null ? String(attrs.employeeEmail) : undefined,
          departmentName: attrs.departmentName != null ? String(attrs.departmentName) : undefined,
          isCLevel: Boolean(attrs.isCLevel),
          isExternal: Boolean(attrs.isExternal),
          wasConnected: Boolean(attrs.wasConnected),
          children: [],
        }
      } else {
        // If nodeData is already a NoMapaRelacionamento, use it directly
        if (nodeData && typeof nodeData === 'object' && 'id' in nodeData) {
          noData = nodeData as unknown as NoMapaRelacionamento
        } else {
          return
        }
      }
    } catch (error) {
      console.error('Error parsing node data:', error, nodeData)
      return
    }
    onSelecionarNo(noData)
  }

  const handleEditClick = (noData: NoMapaRelacionamento) => {
    onEditarNo(noData)
  }

  const handleViewDetails = (noData: NoMapaRelacionamento) => {
    onSelecionarNo(noData)
  }

  const handleAddChild = (noData: NoMapaRelacionamento) => {
    // Chamar função específica para adicionar filho se disponível, senão usar editar
    if (onAdicionarFilho) {
      onAdicionarFilho(noData)
    } else {
      onEditarNo(noData)
    }
  }

  const handleDelete = (noData: NoMapaRelacionamento) => {
    // Rastreamento Firebase Analytics
    if (user) {
      logUserAction(
        'MapaRelacionamento',
        'RemoverNo',
        {
          nodeId: noData.id,
          profileId: noData.profileId || null,
          employeeId: noData.employeeId || null,
        },
        user
      )
    }
    
    // Chamar o handler do parent para deletar
    // A validação de filhos será feita no handler do parent
    onDelete?.(noData)
  }

  const handleToggleCollapse = (nodeId: string) => {
    setCollapsedNodes((prev) => {
      const next = new Set(prev)
      if (next.has(nodeId)) {
        next.delete(nodeId)
      } else {
        next.add(nodeId)
      }
      return next
    })
  }

  // Helper to check if a node is a descendant of another node
  const isDescendant = useCallback((ancestor: NoMapaRelacionamento, candidate: NoMapaRelacionamento): boolean => {
    if (ancestor.id === candidate.id) return true
    return ancestor.children.some((child) => isDescendant(child, candidate))
  }, [])
  
  // Map id -> nó para lookup O(1) durante o drag (evita busca recursiva a cada frame)
  const nodesById = useMemo(() => new Map(allNodes.map((n) => [n.id, n])), [allNodes])
  const findNodeById = useCallback((nodeId: string): NoMapaRelacionamento | null => {
    return nodesById.get(nodeId) ?? null
  }, [nodesById])
  
  // Use shared drag-and-drop hook (mousePosition em ref para não re-renderizar árvore a cada movimento)
  const {
    draggedNode,
    isDragging,
    dropTarget,
    isHoveringRootZone,
    mousePositionRef,
    DragOverlay: DragOverlayMapa,
  } = useDragAndDropMapa({
    containerRef,
    findNodeById,
    onMoverNo,
    isDescendant,
    cardSelector: '[data-mapa-card]',
    dependencies: [no],
  })

  // Show loading state while dimensions are being calculated or treeData is not ready
  if (dimensions.width === 0 || dimensions.height === 0 || !treeData) {
    return (
      <div ref={containerRef} className="w-full h-full bg-primaryBackground flex items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <Spinner size={24} className="text-primary" />
          <div className="text-muted-foreground">Carregando diagrama...</div>
        </div>
      </div>
    )
  }

  return (
    <div ref={containerRef} className="w-full h-full bg-primaryBackground" style={{ position: 'relative', width: '100%', height: '100%' }}>
      <style>{`
        /* LINKS - Must be visible and rendered BEFORE nodes */
        .rd3t-link,
        .org-tree-link {
          stroke: var(--color-primary-text) !important;
          stroke-width: 3px !important;
          stroke-opacity: 1 !important;
          fill: none !important;
          stroke-linecap: round !important;
          stroke-linejoin: round !important;
          pointer-events: none !important;
        }
        /* Ensure path elements inside link groups are styled */
        .rd3t-link path,
        .org-tree-link path,
        g.rd3t-link path,
        g.org-tree-link path {
          stroke: var(--color-primary-text) !important;
          stroke-width: 3px !important;
          stroke-opacity: 1 !important;
          fill: none !important;
        }
        /* Nodes */
        .rd3t-node {
          transition: transform 0.3s ease, opacity 0.3s ease;
        }
        .rd3t-node__branch {
          transition: opacity 0.3s ease, transform 0.3s ease;
        }
        .rd3t-leaf-node {
          transition: opacity 0.3s ease, transform 0.3s ease;
        }
        /* SVG must allow overflow for links to be visible */
        svg.rd3t-svg {
          overflow: visible !important;
        }
        /* Ensure links are rendered on top - z-index doesn't work in SVG, but order matters */
        /* Links should be rendered before nodes in SVG order */
      `}</style>
      {treeData && (
        <MapaRelacionamentoTreeContext.Provider value={{ allNodes }}>
          <Tree
            data={treeData as RawNodeDatum}
          orientation="vertical"
          translate={translate}
          zoom={scale}
          onUpdate={handleTreeUpdate}
          pathFunc={customPathFromAbove}
          zoomable={true}
          draggable={true}
          collapsible={false}
          separation={separationConfig}
          nodeSize={nodeSizeConfig}
          dimensions={dimensions}
          key={`${no.id}-${treeDataVersion}-${[...collapsedNodes].sort().join(',')}`}
          transitionDuration={0}
          renderCustomNodeElement={(rd3tProps: CustomNodeElementProps) => {
            // Check if this node is collapsed
            const nodeId = rd3tProps.nodeDatum.attributes?.id as string | undefined
            const isCollapsed = nodeId ? collapsedNodes.has(nodeId) : false
            
            return (
              <CustomNode
                {...rd3tProps}
                onNodeClick={handleNodeClick}
                onEditClick={handleEditClick}
                onViewDetails={handleViewDetails}
                onAddChild={handleAddChild}
                onDelete={handleDelete}
                onToggleCollapse={handleToggleCollapse}
                isCollapsed={isCollapsed}
                draggedNodeId={draggedNode?.id}
                dropTargetNodeId={dropTarget?.id}
                isDragging={isDragging}
              />
            )
          }}
          pathClassFunc={() => 'org-tree-link'}
          />
        </MapaRelacionamentoTreeContext.Provider>
      )}
      
      {/* Overlay de drag: só este componente re-renderiza a cada movimento do mouse */}
      <DragOverlayMapa
        mousePositionRef={mousePositionRef}
        draggedNode={draggedNode}
        isDragging={isDragging}
        dropTarget={dropTarget}
        isHoveringRootZone={isHoveringRootZone}
      />
    </div>
  )
})

ArvoreMapaD3Component.displayName = 'ArvoreMapaD3'

export const ArvoreMapaD3 = memo(
  ArvoreMapaD3Component,
  (prevProps, nextProps) => {
    // Só evita re-render quando a referência da árvore e treeDataVersion são os mesmos.
    // Se o pai passar nova referência (após mover/editar/criar/remover card) ou treeDataVersion mudar (atualização otimista), re-renderiza.
    return prevProps.no === nextProps.no && prevProps.treeDataVersion === nextProps.treeDataVersion
  }
)
