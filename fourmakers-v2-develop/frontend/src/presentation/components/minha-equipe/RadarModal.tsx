import * as React from 'react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { ChevronDown, Check, X, Loader2 } from '@/components/ui/system-icons'
import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeSugestao } from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import type { MinhaEquipeHabilidade } from '@domain/entities/MinhaEquipeHabilidade'
import { TIPO_SKILL_REVERSE_MAP } from '@shared/constants/skillTypes'
import { RadarSkillCard } from './RadarSkillCard'
import { RadarPDICard } from './RadarPDICard'
import { SugestaoAprovacaoModal } from './SugestaoAprovacaoModal'
import { cn } from '@/lib/utils'

interface RadarModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  colaborador: MinhaEquipeColaborador | null
  sugestoes: MinhaEquipeSugestao[]
  pdiMetas: MinhaEquipePDI[]
  loadingRadar: boolean
  onAprovarSugestao: (sugestaoId: string, comentario: string) => void
  onRejeitarSugestao: (sugestaoId: string, comentario: string) => void
}

// Hook para drag-to-scroll - EXATAMENTE igual ao SkillGroupCard que funciona
const useDragToScroll = () => {
  const scrollContainerRef = React.useRef<HTMLDivElement>(null)
  const [isDragging, setIsDragging] = React.useState(false)
  const [startX, setStartX] = React.useState(0)
  const [scrollLeft, setScrollLeft] = React.useState(0)
  // Refs para garantir valores atualizados nos listeners globais
  const startXRef = React.useRef(0)
  const scrollLeftRef = React.useRef(0)

  const handleMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
    // Ignorar apenas se clicar diretamente em botões ou links
    const target = e.target as HTMLElement
    
    // Verificar se clicou em um botão ou link (mas permitir cliques em cards)
    if (target.tagName === 'BUTTON' || target.tagName === 'A') {
      return
    }
    
    // Verificar se está dentro de um botão ou link, mas não dentro de um card
    const button = target.closest('button')
    const link = target.closest('a')
    if (button || link) {
      // Se o botão/link está dentro de um card, permitir o drag
      const card = target.closest('[class*="Card"]')
      if (!card) {
        return
      }
    }

    if (!scrollContainerRef.current) return
    e.preventDefault() // Prevenir seleção de texto
    const startXValue = e.pageX - scrollContainerRef.current.offsetLeft
    const scrollLeftValue = scrollContainerRef.current.scrollLeft
    
    setIsDragging(true)
    setStartX(startXValue)
    setScrollLeft(scrollLeftValue)
    // Atualizar refs também
    startXRef.current = startXValue
    scrollLeftRef.current = scrollLeftValue
  }

  const handleMouseMove = (e: React.MouseEvent<HTMLDivElement>) => {
    if (!isDragging || !scrollContainerRef.current) return
    e.preventDefault()
    const x = e.pageX - scrollContainerRef.current.offsetLeft
    const walk = (x - startX) * 2 // Scroll speed multiplier
    scrollContainerRef.current.scrollLeft = scrollLeft - walk
  }

  const handleMouseUp = () => {
    setIsDragging(false)
  }

  const handleMouseLeave = () => {
    // Não resetar aqui para permitir continuar arrastando quando mouse sai
  }

  // Adicionar listeners globais quando está arrastando
  React.useEffect(() => {
    if (!isDragging) return

    const handleGlobalMouseMove = (e: MouseEvent) => {
      if (!scrollContainerRef.current) return
      e.preventDefault()
      // Usar refs para garantir valores atualizados
      const x = e.pageX - scrollContainerRef.current.offsetLeft
      const walk = (x - startXRef.current) * 2
      scrollContainerRef.current.scrollLeft = scrollLeftRef.current - walk
    }

    const handleGlobalMouseUp = () => {
      setIsDragging(false)
    }

    document.addEventListener('mousemove', handleGlobalMouseMove, { passive: false })
    document.addEventListener('mouseup', handleGlobalMouseUp)

    return () => {
      document.removeEventListener('mousemove', handleGlobalMouseMove)
      document.removeEventListener('mouseup', handleGlobalMouseUp)
    }
  }, [isDragging]) // Removido startX e scrollLeft das dependências, usando refs

  return {
    scrollContainerRef,
    isDragging,
    handleMouseDown,
    handleMouseMove,
    handleMouseUp,
    handleMouseLeave,
  }
}

interface SkillsGroupRowProps {
  tipo: number
  habilidadesGrupo: MinhaEquipeHabilidade[]
}

const SkillsGroupRow = ({ tipo, habilidadesGrupo }: SkillsGroupRowProps) => {
  const scrollHook = useDragToScroll()

  return (
    <div className="space-y-2 min-w-0">
      <h4 className="font-medium text-sm text-muted-foreground">
        {TIPO_SKILL_REVERSE_MAP[tipo] || `Tipo ${tipo}`}
      </h4>
      <div
        ref={scrollHook.scrollContainerRef}
        className={cn(
          'horizontal-scroll-container overflow-x-auto overflow-y-hidden pb-2 -mx-1 px-1 scroll-smooth',
          scrollHook.isDragging
            ? 'cursor-grabbing select-none'
            : 'cursor-grab',
        )}
        style={scrollHook.isDragging ? { userSelect: 'none' } : {}}
        onMouseDown={scrollHook.handleMouseDown}
        onMouseMove={scrollHook.handleMouseMove}
        onMouseUp={scrollHook.handleMouseUp}
        onMouseLeave={scrollHook.handleMouseLeave}
        onWheel={(e) => {
          // Permitir scroll horizontal com mouse wheel (Shift + Wheel ou Wheel horizontal)
          if (e.shiftKey || Math.abs(e.deltaX) > Math.abs(e.deltaY)) {
            e.preventDefault()
            if (scrollHook.scrollContainerRef.current) {
              scrollHook.scrollContainerRef.current.scrollLeft += e.deltaX || e.deltaY
            }
          }
        }}
      >
        <div className="flex gap-4 min-w-max">
          {habilidadesGrupo.map((h) => (
            <div
              key={h.id}
              className="flex-shrink-0 w-80"
              style={{
                pointerEvents: scrollHook.isDragging ? 'none' : 'auto',
                userSelect: 'none',
              }}
            >
              <RadarSkillCard habilidade={h} />
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export const RadarModal = ({
  open,
  onOpenChange,
  colaborador,
  sugestoes,
  pdiMetas,
  loadingRadar,
  onAprovarSugestao,
  onRejeitarSugestao,
}: RadarModalProps) => {
  // Estados para modal de aprovação/rejeição
  const [sugestaoModalOpen, setSugestaoModalOpen] = React.useState(false)
  const [sugestaoModalTipo, setSugestaoModalTipo] = React.useState<
    'aprovar' | 'rejeitar'
  >('aprovar')
  const [sugestaoModalId, setSugestaoModalId] = React.useState<string>('')

  if (!colaborador) return null

  // Separar habilidades por tipo e estado
  const habilidadesAdicionadas =
    colaborador.resultadoHabilidades?.filter((h) => !h.pendencia) || []

  const habilidadesPendentes =
    colaborador.resultadoHabilidades?.filter(
      (h) => h.pendencia && h.interesse === 1,
    ) || []

  const habilidadesRecusadas =
    colaborador.resultadoHabilidades?.filter(
      (h) => !h.pendencia && h.interesse === 0,
    ) || []

  // Agrupar habilidades por tipo (perfilTipoId)
  const agruparPorTipo = (habilidades: typeof habilidadesAdicionadas) => {
    const grupos: Record<number, typeof habilidadesAdicionadas> = {}
    habilidades.forEach((h) => {
      if (!grupos[h.type]) grupos[h.type] = []
      grupos[h.type].push(h)
    })
    return grupos
  }

  const getStatusLabel = (statusId: number): string => {
    if (statusId === 1) return 'Aprovado'
    if (statusId === 2) return 'Pendente'
    if (statusId === 3) return 'Rejeitado'
    return 'Desconhecido'
  }

  const getStatusBadgeVariant = (statusId: number): 'default' | 'destructive' | 'outline' => {
    if (statusId === 1) return 'default' // Verde (sucesso)
    if (statusId === 3) return 'destructive' // Vermelho (rejeitado)
    return 'outline' // Amarelo/laranja (pendente)
  }

  const getStatusBadgeClassName = (statusId: number): string => {
    if (statusId === 1) return 'bg-success/10 text-success border-success/30'
    if (statusId === 3) return 'bg-destructive/10 text-destructive border-destructive/30'
    return 'bg-warning/10 text-warning border-warning/30'
  }

  const handleAbrirModalAprovacao = (
    sugestaoId: string,
    tipo: 'aprovar' | 'rejeitar',
  ) => {
    setSugestaoModalId(sugestaoId)
    setSugestaoModalTipo(tipo)
    setSugestaoModalOpen(true)
  }

  const handleConfirmarAprovacao = (comentario: string) => {
    if (sugestaoModalTipo === 'aprovar') {
      onAprovarSugestao(sugestaoModalId, comentario)
    } else {
      onRejeitarSugestao(sugestaoModalId, comentario)
    }
  }

  // Componente para renderizar seção de skills com scroll horizontal
  const renderSkillsSection = (habilidades: MinhaEquipeHabilidade[]) => {
    const grupos = agruparPorTipo(habilidades)

    if (Object.keys(grupos).length === 0) {
      return (
        <p className="text-sm text-muted-foreground">
          Nenhuma habilidade nesta categoria
        </p>
      )
    }

    return (
      <div className="space-y-4">
        {Object.entries(grupos).map(([tipo, habilidadesGrupo]) => (
          <SkillsGroupRow
            key={tipo}
            tipo={Number(tipo)}
            habilidadesGrupo={habilidadesGrupo}
          />
        ))}
      </div>
    )
  }

  return (
    <>
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto overflow-x-hidden flex flex-col">
          <DialogHeader>
            <DialogTitle className="text-2xl">
              {colaborador.nomeColaborador}
            </DialogTitle>
            <p className="text-sm text-muted-foreground">
              {colaborador.perfil} • {colaborador.nomeCliente}
            </p>
            <div className="text-2xl font-bold mt-2">
              Match: {colaborador.match}%
            </div>
          </DialogHeader>

          {loadingRadar ? (
            <div className="flex items-center justify-center py-20">
              <Loader2 className="w-8 h-8 animate-spin text-primary" />
            </div>
          ) : (
            <div className="space-y-4 py-4 min-w-0 flex-1">
              {/* Habilidades Adicionadas */}
              <Collapsible>
                <CollapsibleTrigger className="flex items-center justify-between w-full p-4 border rounded-lg hover:bg-muted/50 transition-colors">
                  <span className="font-semibold">
                    Habilidades Adicionadas ({habilidadesAdicionadas.length})
                  </span>
                  <ChevronDown className="h-4 w-4" />
                </CollapsibleTrigger>
                <CollapsibleContent className="p-4 min-w-0 overflow-visible">
                  {renderSkillsSection(habilidadesAdicionadas)}
                </CollapsibleContent>
              </Collapsible>

              {/* Habilidades Pendentes */}
              <Collapsible>
                <CollapsibleTrigger className="flex items-center justify-between w-full p-4 border rounded-lg hover:bg-muted/50 transition-colors">
                  <span className="font-semibold">
                    Habilidades Pendentes ({habilidadesPendentes.length})
                  </span>
                  <ChevronDown className="h-4 w-4" />
                </CollapsibleTrigger>
                <CollapsibleContent className="p-4 min-w-0 overflow-visible">
                  {renderSkillsSection(habilidadesPendentes)}
                </CollapsibleContent>
              </Collapsible>

              {/* Sugestões de Habilidades */}
              <Collapsible>
                <CollapsibleTrigger className="flex items-center justify-between w-full p-4 border rounded-lg hover:bg-muted/50 transition-colors">
                  <span className="font-semibold">
                    Sugestões de Habilidades ({sugestoes.length})
                  </span>
                  <ChevronDown className="h-4 w-4" />
                </CollapsibleTrigger>
                <CollapsibleContent className="p-4 space-y-3">
                  {sugestoes.length === 0 ? (
                    <p className="text-sm text-muted-foreground">
                      Nenhuma sugestão pendente
                    </p>
                  ) : (
                    sugestoes.map((sugestao) => (
                      <div
                        key={sugestao.sugestaoId}
                        className="flex items-center justify-between p-3 border rounded-lg"
                      >
                        <div className="flex-1">
                          <div className="flex items-center gap-2 mb-1">
                            <p className="font-medium">
                              {sugestao.nomeHabilidade}</p>
                            <Badge
                              variant={getStatusBadgeVariant(
                                sugestao.tbStatusSugestaoId,
                              )}
                              className={getStatusBadgeClassName(
                                sugestao.tbStatusSugestaoId,
                              )}
                            >
                              {getStatusLabel(sugestao.tbStatusSugestaoId)}
                            </Badge>
                          </div>
                          <p className="text-sm text-muted-foreground">
                            Tipo:{' '}
                            {TIPO_SKILL_REVERSE_MAP[sugestao.perfilTipoId] ||
                              `Tipo ${sugestao.perfilTipoId}`}
                          </p>
                          {sugestao.observacao && (
                            <p className="text-xs text-muted-foreground mt-1">
                              {sugestao.observacao}
                            </p>
                          )}
                        </div>
                        {sugestao.tbStatusSugestaoId === 2 && (
                          <div className="flex gap-2 ml-4">
                            <Button
                              size="sm"
                              variant="primary"
                              onClick={() =>
                                handleAbrirModalAprovacao(
                                  sugestao.sugestaoId,
                                  'aprovar',
                                )
                              }
                              aria-label={`Aprovar sugestão de habilidade ${sugestao.nomeHabilidade}`}
                            >
                              <Check className="h-4 w-4 mr-1" aria-hidden="true" />
                              Aprovar
                            </Button>
                            <Button
                              size="sm"
                              variant="destructive"
                              onClick={() =>
                                handleAbrirModalAprovacao(
                                  sugestao.sugestaoId,
                                  'rejeitar',
                                )
                              }
                              aria-label={`Rejeitar sugestão de habilidade ${sugestao.nomeHabilidade}`}
                            >
                              <X className="h-4 w-4 mr-1" aria-hidden="true" />
                              Rejeitar
                            </Button>
                          </div>
                        )}
                      </div>
                    ))
                  )}
                </CollapsibleContent>
              </Collapsible>

              {/* Habilidades Recusadas */}
              <Collapsible>
                <CollapsibleTrigger className="flex items-center justify-between w-full p-4 border rounded-lg hover:bg-muted/50 transition-colors">
                  <span className="font-semibold">
                    Habilidades Recusadas ({habilidadesRecusadas.length})
                  </span>
                  <ChevronDown className="h-4 w-4" />
                </CollapsibleTrigger>
                <CollapsibleContent className="p-4 min-w-0 overflow-visible">
                  {renderSkillsSection(habilidadesRecusadas)}
                </CollapsibleContent>
              </Collapsible>

              {/* Metas de PDI */}
              <Collapsible>
                <CollapsibleTrigger className="flex items-center justify-between w-full p-4 border rounded-lg hover:bg-muted/50 transition-colors">
                  <span className="font-semibold">
                    Metas de PDI ({pdiMetas.length})
                  </span>
                  <ChevronDown className="h-4 w-4" />
                </CollapsibleTrigger>
                <CollapsibleContent className="p-4 space-y-3">
                  {pdiMetas.length === 0 ? (
                    <p className="text-sm text-muted-foreground">
                      Nenhuma meta de PDI definida
                    </p>
                  ) : (
                    pdiMetas.map((meta) => (
                      <RadarPDICard key={meta.id} meta={meta} />
                    ))
                  )}
                </CollapsibleContent>
              </Collapsible>
            </div>
          )}

          <div className="flex justify-end pt-4 border-t">
            <Button variant="outline" onClick={() => onOpenChange(false)}>
              Fechar
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal de Aprovação/Rejeição */}
      <SugestaoAprovacaoModal
        open={sugestaoModalOpen}
        onOpenChange={setSugestaoModalOpen}
        onConfirm={handleConfirmarAprovacao}
        tipo={sugestaoModalTipo}
      />
    </>
  )
}
