import React, { useMemo, forwardRef, useRef, useImperativeHandle } from 'react'
import { ZoomIn, ZoomOut } from 'lucide-react'

import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { ArvoreMapaD3, type ArvoreMapaD3Ref } from './arvore-mapa-d3/ArvoreMapaD3'
import { ListaMapa } from './ListaMapa'
import { filtrarCLevelsMapa } from '@domain/services/mapaRelacionamentoTreeService'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

interface DiagramaMapaProps {
  estrutura: NoMapaRelacionamento | null
  /** Versão da árvore; incrementar força remount do Tree para exibir dados atualizados (ex.: após salvar) */
  treeDataVersion?: number
  modoVisualizacao: 'diagrama' | 'lista' | 'c-levels'
  mostrarApenasCLevels: boolean
  onSelecionarNo: (no: NoMapaRelacionamento | null) => void
  onEditarNo: (no: NoMapaRelacionamento) => void
  onAdicionarFilho?: (pai: NoMapaRelacionamento) => void
  onMoverNo: (origem: NoMapaRelacionamento, destino: NoMapaRelacionamento | null) => void
  onDelete?: (no: NoMapaRelacionamento) => void
}

export interface DiagramaMapaRef {
  centralizarArvore: () => void
  zoomIn: () => void
  zoomOut: () => void
}

const DiagramaMapaInner = forwardRef<DiagramaMapaRef, DiagramaMapaProps>(
  ({ estrutura, treeDataVersion = 0, modoVisualizacao, mostrarApenasCLevels, onSelecionarNo, onEditarNo, onAdicionarFilho, onMoverNo, onDelete }, ref) => {
  const arvoreRef = useRef<ArvoreMapaD3Ref>(null)

  // Expose functions via ref
  useImperativeHandle(ref, () => ({
    centralizarArvore: () => {
      arvoreRef.current?.centralizarArvore()
    },
    zoomIn: () => {
      arvoreRef.current?.zoomIn()
    },
    zoomOut: () => {
      arvoreRef.current?.zoomOut()
    },
  }), [])

  const estruturaFiltrada = useMemo(() => {
    if (!estrutura) return null
    // Aplicar filtro APENAS quando mostrarApenasCLevels for explicitamente true
    // Garantir que o filtro não seja aplicado por padrão
    if (mostrarApenasCLevels === true) {
      return filtrarCLevelsMapa(estrutura)
    }
    // Retornar estrutura original quando filtro não está ativo
    return estrutura
  }, [estrutura, mostrarApenasCLevels])

  if (!estruturaFiltrada) {
    return (
      <div className="flex flex-1 items-center justify-center">
        <p className="text-muted-foreground">Estrutura vazia</p>
      </div>
    )
  }

  if (modoVisualizacao === 'lista') {
    return (
      <div className="relative flex-1 w-full h-full overflow-hidden bg-primaryBackground">
        <ListaMapa
          estrutura={estruturaFiltrada}
          onSelecionarNo={onSelecionarNo}
          onEditarNo={onEditarNo}
          onAdicionarFilho={onAdicionarFilho}
          onMoverNo={onMoverNo}
          onDelete={onDelete}
        />
        {mostrarApenasCLevels && (
          <Badge className="absolute right-4 top-4">Visualização C-Level</Badge>
        )}
      </div>
    )
  }

  return (
    <div className="relative w-full h-full bg-primaryBackground" style={{ minWidth: '100%', minHeight: '100%' }}>
      <ArvoreMapaD3
        ref={arvoreRef}
        no={estruturaFiltrada}
        treeDataVersion={treeDataVersion}
        onSelecionarNo={onSelecionarNo}
        onEditarNo={onEditarNo}
        onAdicionarFilho={onAdicionarFilho}
        onMoverNo={onMoverNo}
        onDelete={onDelete}
      />
      {/* Zoom Controls */}
      <div className="absolute top-4 left-4 flex flex-col gap-2 z-50">
        <Button
          size="icon"
          variant="secondary"
          onClick={() => arvoreRef.current?.zoomIn()}
          className="w-10 h-10 rounded-lg shadow-lg hover:shadow-xl"
          aria-label="Aumentar zoom"
          title="Aumentar zoom"
        >
          <ZoomIn className="h-5 w-5" />
        </Button>
        <Button
          size="icon"
          variant="secondary"
          onClick={() => arvoreRef.current?.zoomOut()}
          className="w-10 h-10 rounded-lg shadow-lg hover:shadow-xl"
          aria-label="Diminuir zoom"
          title="Diminuir zoom"
        >
          <ZoomOut className="h-5 w-5" />
        </Button>
      </div>
      {mostrarApenasCLevels && (
        <Badge className="absolute right-4 top-4">Visualização C-Level</Badge>
      )}
    </div>
  )
})

DiagramaMapaInner.displayName = 'DiagramaMapaInner'

export const DiagramaMapa = React.memo(DiagramaMapaInner)
DiagramaMapa.displayName = 'DiagramaMapa'
