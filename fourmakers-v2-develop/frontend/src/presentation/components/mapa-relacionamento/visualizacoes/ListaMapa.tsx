import React, { useMemo, useState, useRef, useCallback } from 'react'
import { useWindowedList } from '@shared/hooks/useWindowedList'
import { useDragAndDropMapa } from '@presentation/hooks/useDragAndDropMapa'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
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
import { converterParaListaMapa } from '@domain/services/mapaRelacionamentoTreeService'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { Edit, Eye, UserPlus, Trash, GripVertical, Crown } from 'lucide-react'

interface ListaMapaProps {
  estrutura: NoMapaRelacionamento
  onSelecionarNo: (no: NoMapaRelacionamento | null) => void
  onEditarNo: (no: NoMapaRelacionamento) => void
  onAdicionarFilho?: (pai: NoMapaRelacionamento) => void
  onDelete?: (no: NoMapaRelacionamento) => void
  onMoverNo?: (origem: NoMapaRelacionamento, destino: NoMapaRelacionamento | null) => void
}

function ListaMapaInner({
  estrutura,
  onSelecionarNo: _onSelecionarNo,
  onEditarNo,
  onAdicionarFilho,
  onDelete,
  onMoverNo,
}: ListaMapaProps) {
  const flatList = useMemo(() => converterParaListaMapa(estrutura), [estrutura])
  const [showDeleteDialog, setShowDeleteDialog] = useState<NoMapaRelacionamento | null>(null)
  
  const containerRef = useRef<HTMLDivElement>(null)

  // Virtualização por janela de scroll (sem TanStack) — renderiza só itens visíveis
  const { virtualItems, totalSize } = useWindowedList(
    containerRef,
    flatList.length,
    120,
    5
  )

  // Helper to check if a node is a descendant of another node
  const isDescendant = useCallback((ancestor: NoMapaRelacionamento, candidate: NoMapaRelacionamento): boolean => {
    if (ancestor.id === candidate.id) return true
    return ancestor.children.some((child) => isDescendant(child, candidate))
  }, [])

  // Função para encontrar nó por ID na lista (busca linear)
  const findNodeById = useCallback((nodeId: string): NoMapaRelacionamento | null | undefined => {
    return flatList.find((n) => n.id === nodeId)
  }, [flatList])

  // Use shared drag-and-drop hook (DragOverlay lê posição do mouse via ref)
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
    cardSelector: '[data-lista-card]',
    dependencies: [flatList],
  })

  return (
    <>
      <div ref={containerRef} className="flex flex-col p-4 overflow-y-auto h-full bg-primaryBackground relative">
        {/* Virtualização: renderizar apenas itens visíveis */}
        <div style={{ height: `${totalSize}px`, position: 'relative' }}>
          {virtualItems.map((virtualRow) => {
            const no = flatList[virtualRow.index]
            const nivel = no.nivel
            const indentPx = nivel * 48
            const isBeingDragged = draggedNode?.id === no.id && isDragging
            const isDropTargetNode = dropTarget?.id === no.id && isDragging

            return (
              <div
                key={no.id}
                className="flex absolute top-0 left-0 w-full"
                style={{ 
                  paddingLeft: `${indentPx}px`,
                  transform: `translateY(${virtualRow.start}px)`,
                  height: `${virtualRow.size}px`,
                }}
              >
              {/* Card content */}
              <div
                data-lista-card
                data-node-id={no.id}
                className={`flex-1 rounded-lg border bg-card hover:shadow-sm transition-all ${
                  isBeingDragged
                    ? 'opacity-40 border-dashed border-2 border-border' 
                    : isDropTargetNode
                      ? 'ring-4 ring-success ring-offset-2 border-success scale-105' 
                      : 'border-border hover:border-primary'
                }`}
              >
                <div className="p-4">
                  <div className="flex items-start justify-between gap-4">
                    {/* Grip icon for drag */}
                    {no.posicaoId && (
                      <div
                        data-drag-handle
                        className="text-muted-foreground hover:text-primary p-1 rounded-full transition-all shrink-0 cursor-grab active:cursor-grabbing self-center"
                        title="Arrastar para mover"
                      >
                        <GripVertical className="w-5 h-5 pointer-events-none" />
                      </div>
                    )}
                    
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-2">
                        <h3 className="text-sm font-semibold text-foreground truncate">
                          {no.employeeName || 'Perfil Vago'}
                        </h3>
                        {no.isCLevel && (
                          <Badge variant="secondary" className="text-xs font-bold flex items-center gap-1">
                            <Crown className="w-3 h-3 text-warning" />
                            C-Level
                          </Badge>
                        )}
                      </div>
                      <p className="text-xs text-muted-foreground mb-1 truncate" title={no.profileName || ''}>
                        {no.profileName || 'Sem perfil'}
                      </p>
                      {no.departmentName && (
                        <p className="text-xs text-muted-foreground/70 truncate" title={no.departmentName}>
                          {no.departmentName}
                        </p>
                      )}
                    </div>
                    <div className="flex items-center gap-1.5 shrink-0">
                      <Button
                        type="button"
                        size="icon"
                        variant="outline"
                        onClick={(e) => {
                          e.stopPropagation()
                          if (!isDragging) {
                            onEditarNo(no)
                          }
                        }}
                        className="h-8 w-8"
                        title="Editar Perfil de atuação"
                        disabled={isDragging}
                      >
                        <Edit className="w-4 h-4" />
                      </Button>
                      {onAdicionarFilho && (
                        <Button
                          type="button"
                          size="icon"
                          variant="outline"
                          onClick={(e) => {
                            e.stopPropagation()
                            if (!isDragging) {
                              onAdicionarFilho(no)
                            }
                          }}
                          className="h-8 w-8"
                          title="Adicionar Subordinado"
                          disabled={isDragging}
                        >
                          <UserPlus className="w-4 h-4" />
                        </Button>
                      )}
                      <Button
                        type="button"
                        size="icon"
                        variant="outline"
                        onClick={(e) => {
                          e.stopPropagation()
                          if (!isDragging && _onSelecionarNo) {
                            _onSelecionarNo(no)
                          }
                        }}
                        className="h-8 w-8"
                        title="Visualizar VCX 360"
                        disabled={isDragging}
                      >
                        <Eye className="w-4 h-4" />
                      </Button>
                      {onDelete && (
                        <Button
                          type="button"
                          size="icon"
                          variant="outline"
                          onClick={(e) => {
                            e.stopPropagation()
                            if (!isDragging) {
                              setShowDeleteDialog(no)
                            }
                          }}
                          className="h-8 w-8 border-destructive/20 text-destructive hover:bg-destructive hover:text-destructive-foreground disabled:opacity-50 disabled:cursor-not-allowed"
                          title="Remover"
                          disabled={isDragging}
                        >
                          <Trash className="w-4 h-4" />
                        </Button>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
            )
          })}
        </div>
      </div>

      {/* Instruction message when dragging */}
      <DragOverlayMapa
        mousePositionRef={mousePositionRef}
        draggedNode={draggedNode}
        isDragging={isDragging}
        dropTarget={dropTarget}
        isHoveringRootZone={isHoveringRootZone}
      />

      {/* Delete Dialog - rendered once outside the map */}
      <AlertDialog open={!!showDeleteDialog} onOpenChange={(open) => {
        if (!open) {
          setShowDeleteDialog(null)
        }
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Remover Perfil de atuação</AlertDialogTitle>
            <AlertDialogDescription>
              {showDeleteDialog && showDeleteDialog.children && showDeleteDialog.children.length > 0
                ? 'Esta posição possui posições filhas. Ao remover, as posições filhas serão realocadas para o nível superior. Tem certeza que deseja continuar?'
                : 'Tem certeza que deseja remover este perfil de atuação? Esta ação não pode ser desfeita.'}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={(e) => {
                e.stopPropagation()
                if (showDeleteDialog && onDelete) {
                  onDelete(showDeleteDialog)
                  setShowDeleteDialog(null)
                }
              }}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              Remover
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}

export const ListaMapa = React.memo(ListaMapaInner)
ListaMapa.displayName = 'ListaMapa'
