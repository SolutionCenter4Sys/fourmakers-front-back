import { useRef, useCallback, memo } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { setModoVisualizacao, setClienteSelecionado } from '@app/store/slices/mapaRelacionamentoSlice'
import { ClientSelector, DiagramaMapa, PainelVcx360 } from '@presentation/components/mapa-relacionamento'
import { EditModalMapa } from '@presentation/components/mapa-relacionamento/modais/EditModalMapa'
import { useMapaRelacionamentoPersistencia } from '@presentation/hooks/useMapaRelacionamentoPersistencia'
import { useMapaInteracoes } from '@presentation/hooks/useMapaInteracoes'
import { useMapaRelacionamentoPage } from '@presentation/hooks/useMapaRelacionamentoPage'
import { Building, Plus, ChevronDown, LayoutDashboard, Network, List, Crown, Download, RotateCcw, Maximize2 } from 'lucide-react'
import { MapaRelacionamentoLoadingState } from '@presentation/components/mapa-relacionamento/visualizacoes/MapaRelacionamentoLoadingState'
import { Button } from '@/components/ui/button'
import { Spinner } from '@/components/ui/spinner'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip'
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogAction,
} from '@/components/ui/alert-dialog'
import type { ClienteMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

// Bloco hero isolado e memoizado: evita re-render quando a página atualiza por Redux/hooks (noSelecionado, estruturaMapa, etc.)
const MapaRelacionamentoHero = memo(function MapaRelacionamentoHero({
  clientes,
  onSelecionarCliente,
  onNameChange,
}: {
  clientes: ClienteMapaRelacionamento[]
  onSelecionarCliente: (code: string) => void
  onNameChange: (name: string) => void
}) {
  return (
    <div className="min-h-full flex flex-col items-center justify-center text-center p-4 sm:p-8 py-12 bg-primaryBackground">
      <div className="max-w-2xl w-full p-8 sm:p-12 bg-card rounded-lg shadow-xl border border-border relative overflow-visible">
        <div className="absolute -top-20 -right-20 w-64 h-64 bg-primary/5 rounded-full blur-3xl pointer-events-none" aria-hidden />
        <div className="relative z-10 flex flex-col items-center">
          <div className="w-20 h-20 rounded-lg bg-primary/10 flex items-center justify-center mb-10 text-primary">
            <Building className="h-10 w-10" aria-hidden />
          </div>
          <h3 className="text-3xl sm:text-4xl font-bold text-foreground mb-4 tracking-tight">Comece o Mapeamento</h3>
          <p className="text-muted-foreground max-w-md font-medium leading-relaxed mb-12 text-base sm:text-lg">
            Selecione um <span className="text-primary font-bold">Cliente</span> para visualizar a{' '}
            <span className="text-primary font-bold">Hierarquia</span>.
          </p>
          <div className="w-full space-y-4 relative z-50">
            <ClientSelector
              selectedCode=""
              onSelect={onSelecionarCliente}
              onNameChange={onNameChange}
              variant="hero"
              clientes={clientes}
            />
          </div>
        </div>
      </div>
    </div>
  )
})

export function MapaRelacionamentoPage() {
  const dispatch = useAppDispatch()
  const { user } = useAppSelector((state) => state.auth)
  const { perfis, departamentos, modoVisualizacao: modoAtual } = useAppSelector((state) => state.mapaRelacionamento)
  
  const {
    clientes,
    clienteSelecionado,
    estruturaMapa,
    noSelecionado,
    noEmEdicao,
    selectedClientName,
    modoVisualizacao,
    mostrarApenasCLevels,
    diagramaRef,
    isLoadingEstrutura,
    isSaving,
    handleSelecionarCliente,
    handleSelecionarNo,
    handleEditarNo,
    handleAdicionarFilho,
    handleSalvarNo,
    handleMoverNo,
    handleDeletarNo,
    handleExportJSON,
    handleToggleCLevels,
    handleAdicionarPrimeiraPosicao,
    handleResetToMock,
    handleCentralizarArvore,
    isMockClient,
    treeDataVersion,
    setNoEmEdicao,
    setSelectedClientName,
    showErroVinculoDepartamento,
    setShowErroVinculoDepartamento,
    erroVinculoMensagem,
  } = useMapaRelacionamentoPage()

  const treeContainerRef = useRef<HTMLDivElement>(null)

  const handleEditModalOpenChange = useCallback(
    (open: boolean) => {
      if (!open) setNoEmEdicao(null)
    },
    [setNoEmEdicao],
  )

  const handleFecharPainel = useCallback(() => {
    handleSelecionarNo(null)
  }, [handleSelecionarNo])

  // Use persistence hook for localStorage management
  useMapaRelacionamentoPersistencia()

  // Use interactions hook for mouse panning (provides viewportRef with pan/scroll functionality)
  const { viewportRef, isPanningRef: _isPanningRef } = useMapaInteracoes()

  // Handlers para mudança de modo com rastreamento Firebase Analytics
  const handleModoDiagrama = useCallback(() => {
    if (user && modoAtual !== 'diagrama') {
      logUserAction(
        'MapaRelacionamento',
        'AlterarModoVisualizacao',
        { modo: 'diagrama' },
        user
      )
    }
    dispatch(setModoVisualizacao('diagrama'))
  }, [dispatch, user, modoAtual])

  const handleModoLista = useCallback(() => {
    if (user && modoAtual !== 'lista') {
      logUserAction(
        'MapaRelacionamento',
        'AlterarModoVisualizacao',
        { modo: 'lista' },
        user
      )
    }
    dispatch(setModoVisualizacao('lista'))
  }, [dispatch, user, modoAtual])

  return (
    <div className="w-full h-full bg-primaryBackground text-foreground flex flex-col overflow-hidden relative">
      <div className={`flex-1 flex flex-col min-h-0 overflow-hidden ${clienteSelecionado ? 'pt-4' : ''} transition-all duration-300`}>
        {clienteSelecionado && (
          <header className="mb-0 px-4 py-3 flex-shrink-0 bg-card rounded-2xl border-2 border-border relative mx-4">
            <div className="w-full flex flex-col lg:flex-row gap-3 items-start lg:items-center justify-between">
              <div className="flex items-center gap-3 sm:gap-4 w-full lg:w-auto">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => {
                    dispatch(setClienteSelecionado(null))
                    setSelectedClientName('')
                  }}
                  className="flex-shrink-0"
                  aria-label="Voltar para seleção de cliente"
                >
                  <ChevronDown className="h-4 w-4 rotate-90" />
                </Button>
                <div className="space-y-0.5 min-w-0 flex-1">
                  <h1 className="text-base sm:text-lg md:text-xl font-bold text-foreground tracking-tight uppercase truncate">
                    {selectedClientName || clienteSelecionado.nomeCliente}
                  </h1>
                  <p className="text-muted-foreground text-[10px] font-bold uppercase tracking-widest opacity-60">
                    Visão 360 & Hierarquia
                  </p>
                </div>
              </div>
              <div className="flex flex-col sm:flex-row gap-3 sm:gap-4 items-stretch sm:items-center w-full lg:w-auto">
                <div className="w-full sm:w-64">
                  <ClientSelector
                    selectedCode={clienteSelecionado.codigoCliente}
                    selectedName={selectedClientName || clienteSelecionado.nomeCliente || ''}
                    onSelect={handleSelecionarCliente}
                    onNameChange={setSelectedClientName}
                    clientes={clientes}
                  />
                </div>

                {estruturaMapa && (
                  <TooltipProvider>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Button
                          variant="outline"
                          size="icon"
                          onClick={handleAdicionarPrimeiraPosicao}
                          className="shrink-0 rounded-lg"
                          title="Nova Posição Raiz"
                        >
                          <Plus className="h-4 w-4" />
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p>Nova Posição Raiz</p>
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>
                )}

                <div className="flex bg-muted p-1 rounded-full border border-border w-full sm:w-auto">
                  <TooltipProvider delayDuration={200}>
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Button
                          variant={modoVisualizacao === 'c-levels' ? 'primary' : 'ghost'}
                          size="sm"
                          onClick={(e) => {
                            e.preventDefault()
                            e.stopPropagation()
                          }}
                          disabled
                          className="flex-1 md:flex-none rounded-full font-bold text-[9px] uppercase tracking-widest opacity-50"
                        >
                          <LayoutDashboard className="h-4 w-4" />
                          <span className="hidden sm:inline">Gerencial</span>
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent>
                        <p>Em Breve</p>
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>
                  <Button
                    variant={modoVisualizacao === 'diagrama' ? 'primary' : 'ghost'}
                    size="sm"
                    onClick={handleModoDiagrama}
                    className="flex-1 md:flex-none rounded-full font-bold text-[9px] uppercase tracking-widest"
                  >
                    <Network className="h-4 w-4" />
                    <span className="hidden sm:inline">Diagrama</span>
                  </Button>
                  <Button
                    variant={modoVisualizacao === 'lista' ? 'primary' : 'ghost'}
                    size="sm"
                    onClick={handleModoLista}
                    className="flex-1 md:flex-none rounded-full font-bold text-[9px] uppercase tracking-widest"
                  >
                    <List className="h-4 w-4" />
                    <span className="hidden sm:inline">Lista</span>
                  </Button>
                </div>
              </div>
            </div>
          </header>
        )}

        <main className={`flex-1 overflow-y-auto overflow-x-hidden bg-primaryBackground relative ${clienteSelecionado && modoVisualizacao !== 'diagrama' ? 'pt-4' : ''}`}>
          {!clienteSelecionado ? (
            <MapaRelacionamentoHero
              clientes={clientes}
              onSelecionarCliente={handleSelecionarCliente}
              onNameChange={setSelectedClientName}
            />
          ) : isLoadingEstrutura ? (
            <MapaRelacionamentoLoadingState />
          ) : !estruturaMapa ? (
            <div className="h-full flex flex-col items-center justify-center bg-primaryBackground">
              <div className="p-12 bg-card rounded-lg shadow-xl border border-border flex flex-col items-center max-w-sm text-center">
                <div className="w-16 h-16 rounded-full bg-primary/10 flex items-center justify-center text-primary mb-6">
                  <Plus className="h-8 w-8" />
                </div>
                <h4 className="text-xl font-bold text-foreground mb-2">Nenhuma hierarquia encontrada</h4>
                <p className="text-sm text-muted-foreground mb-8">
                  Este Cliente ainda não possui uma estrutura mapeada. Vamos começar?
                </p>
                <Button onClick={handleAdicionarPrimeiraPosicao} className="w-full h-12 rounded-full shadow-md hover:scale-105 transition-all">
                  Adicionar Primeira Posição
                </Button>
              </div>
            </div>
          ) : modoVisualizacao === 'c-levels' ? (
            <div className="p-6 md:p-10 max-w-[1400px] mx-auto overflow-y-auto h-full bg-primaryBackground">
              <div className="bg-card rounded-2xl border border-border shadow-xl overflow-hidden">
                <div className="p-6 md:p-8 border-b border-border">
                  <h2 className="text-2xl font-bold text-foreground tracking-tight mb-1">Cobertura C-Level</h2>
                  <p className="text-muted-foreground text-xs font-medium">
                    Visão agregada do C-Level, status de conexão e última interação.
                  </p>
                </div>
                <div className="p-6">
                  <p className="text-sm text-muted-foreground">Visualização C-Level em desenvolvimento...</p>
                </div>
              </div>
            </div>
          ) : (
            <div className={`w-full h-full relative flex flex-col bg-primaryBackground transition-all duration-300 isolate ${noSelecionado ? 'mr-96' : ''}`}>
              <div
                ref={viewportRef}
                className="flex-1 overflow-auto bg-primaryBackground"
                style={{ cursor: 'grab', minWidth: 0, minHeight: 0 }}
                onClick={(e) => {
                  const target = e.target as HTMLElement
                  if (target === e.currentTarget) {
                    handleSelecionarNo(null)
                  }
                }}
              >

                <div className="absolute bottom-4 right-4 flex items-center gap-3 z-[100] pointer-events-none">
                  <TooltipProvider delayDuration={200}>
                    {/* Toggle C-Levels Filter */}
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Button
                          size="icon"
                          variant={mostrarApenasCLevels ? 'primary' : 'secondary'}
                          onClick={handleToggleCLevels}
                          className="w-12 h-12 rounded-full shadow-lg pointer-events-auto transition-all hover:shadow-xl"
                        >
                          <Crown className="h-5 w-5" />
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent side="left">
                        <p className="font-semibold">
                          {mostrarApenasCLevels ? 'Mostrar Todos' : 'Filtrar C-Levels'}
                        </p>
                        <p className="text-xs text-muted-foreground">
                          {mostrarApenasCLevels 
                            ? 'Exibir toda a hierarquia' 
                            : 'Mostrar apenas executivos C-Level'}
                        </p>
                      </TooltipContent>
                    </Tooltip>

                    {/* Reset to Mock - Only for dev/mock clients */}
                    {clienteSelecionado && isMockClient(clienteSelecionado.codigoCliente) && (
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <Button
                            size="icon"
                            variant="outline"
                            onClick={handleResetToMock}
                            className="w-12 h-12 rounded-full shadow-lg pointer-events-auto border-2 hover:shadow-xl"
                          >
                            <RotateCcw className="h-5 w-5" />
                          </Button>
                        </TooltipTrigger>
                        <TooltipContent side="left">
                          <p className="font-semibold">Resetar Mock</p>
                          <p className="text-xs text-muted-foreground">
                            Recarregar dados de exemplo
                          </p>
                        </TooltipContent>
                      </Tooltip>
                    )}

                    {/* Center Tree - Only in diagram mode */}
                    {modoVisualizacao === 'diagrama' && (
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <Button
                            size="icon"
                            variant="secondary"
                            onClick={handleCentralizarArvore}
                            className="w-12 h-12 rounded-full shadow-lg pointer-events-auto hover:shadow-xl"
                            disabled={!estruturaMapa}
                          >
                            <Maximize2 className="h-5 w-5" />
                          </Button>
                        </TooltipTrigger>
                        <TooltipContent side="left">
                          <p className="font-semibold">Centralizar</p>
                          <p className="text-xs text-muted-foreground">
                            Voltar ao centro da árvore
                          </p>
                        </TooltipContent>
                      </Tooltip>
                    )}

                    {/* Export JSON */}
                    <Tooltip>
                      <TooltipTrigger asChild>
                        <Button
                          size="icon"
                          variant="secondary"
                          onClick={handleExportJSON}
                          className="w-12 h-12 rounded-full shadow-lg pointer-events-auto hover:shadow-xl"
                        >
                          <Download className="h-5 w-5" />
                        </Button>
                      </TooltipTrigger>
                      <TooltipContent side="left">
                        <p className="font-semibold">Exportar</p>
                        <p className="text-xs text-muted-foreground">
                          Baixar estrutura em JSON
                        </p>
                      </TooltipContent>
                    </Tooltip>
                  </TooltipProvider>
                </div>

                <div ref={treeContainerRef} className="w-full h-full bg-primaryBackground relative" data-mapa-tree>
                  <DiagramaMapa
                    ref={diagramaRef}
                    estrutura={estruturaMapa}
                    treeDataVersion={treeDataVersion}
                    modoVisualizacao={modoVisualizacao}
                    mostrarApenasCLevels={mostrarApenasCLevels}
                    onSelecionarNo={handleSelecionarNo}
                    onEditarNo={handleEditarNo}
                    onAdicionarFilho={handleAdicionarFilho}
                    onMoverNo={handleMoverNo}
                    onDelete={handleDeletarNo}
                  />
                  {isSaving && (
                    <div
                      className="absolute top-3 right-3 z-[90] pointer-events-none flex items-center justify-center rounded-lg bg-background/70 p-2"
                      aria-live="polite"
                      aria-busy="true"
                    >
                      <Spinner size={24} className="text-primary" />
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}
        </main>
      </div>

      {/* Drawer VCX 360 - Posicionado fixo à direita */}
      {noSelecionado && (
        <>
          {/* Overlay para fechar ao clicar fora */}
          <div
            className="fixed inset-0 bg-black/20 z-30"
            onClick={handleFecharPainel}
            aria-hidden="true"
          />
          <div className="fixed top-0 right-0 h-full w-96 z-40 shadow-2xl border-l border-border bg-card">
            <PainelVcx360 no={noSelecionado} onFechar={handleFecharPainel} />
          </div>
        </>
      )}

      {noEmEdicao && clienteSelecionado && (
        <EditModalMapa
          open={!!noEmEdicao}
          onOpenChange={handleEditModalOpenChange}
          node={noEmEdicao}
          clientCode={clienteSelecionado.codigoCliente}
          perfis={perfis}
          departamentos={departamentos}
          onSave={handleSalvarNo}
        />
      )}

      {/* Modal de Erro - Vínculo com Departamento */}
      <AlertDialog open={showErroVinculoDepartamento} onOpenChange={setShowErroVinculoDepartamento}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle className="text-destructive">Não é possível remover a posição</AlertDialogTitle>
            <AlertDialogDescription className="text-base">
              {erroVinculoMensagem || 'Não é possível excluir esta posição, pois existe vínculo com Departamento.'}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogAction onClick={() => setShowErroVinculoDepartamento(false)}>
              Entendi
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

    </div>
  )
}
