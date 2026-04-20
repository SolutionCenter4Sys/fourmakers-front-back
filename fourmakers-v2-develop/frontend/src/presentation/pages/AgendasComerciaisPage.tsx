import { useState, useEffect, useRef, useCallback } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { Plus, Filter } from 'lucide-react'

import type { ItemAgendaGestor, TipoItemAgenda } from '@domain/entities/AgendaGestor'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { carregarAgendaDetalhe } from '@app/store/slices/agendasComerciaisSlice'
import { PageBreadcrumb, PageHeader } from '@presentation/components/common'
import { Button } from '@/components/ui/button'
import { Spinner } from '@/components/ui/spinner'
import { toast } from 'sonner'
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from '@/components/ui/sheet'
import { useAgendasComerciais } from '@presentation/hooks/useAgendasComerciais'
import { FiltrosAgenda } from '@presentation/components/agendas-comerciais/FiltrosAgenda'
import { TimelineAgendas } from '@presentation/components/agendas-comerciais/TimelineAgendas'
import { NovaAgendaModal } from '@presentation/components/agendas-comerciais/NovaAgendaModal'
import { NovaInteracaoModal } from '@presentation/components/agendas-comerciais/NovaInteracaoModal'
import { DetalhesAgendaModal } from '@presentation/components/agendas-comerciais/DetalhesAgendaModal'

interface LocationStateDetalhes {
  agendaId?: number
  /** Fallback quando a agenda não está na listagem (filtros/período ou API diferente); garante que o modal abra com os dados da VCX */
  detalhesAgenda?: ItemAgendaGestor
}

export default function AgendasComerciaisPage() {
  const location = useLocation()
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const token = useAppSelector((state) => state.auth.token)
  const scrollContainerRef = useRef<HTMLElement | null>(null)
  const setScrollContainerRef = useCallback((el: HTMLDivElement | null) => {
    scrollContainerRef.current = el?.parentElement ?? null
  }, [])
  const carregamentoPorIdEmAndamentoRef = useRef<number | null>(null)

  const {
    agendas,
    interacoes,
    acoes,
    loading,
    isRefetching,
    error,
    filtros,
    handleFiltroChange,
    handleNovaAgenda,
    recarregar,
  } = useAgendasComerciais(scrollContainerRef)

  const [modalAgendaAberto, setModalAgendaAberto] = useState(false)
  const [filtrosAbertos, setFiltrosAbertos] = useState(false)
  /** Modal de criar/editar interação no nível da página para não aninhar dentro de outro Dialog (evita modal não aparecer). */
  const [modalInteracao, setModalInteracao] = useState<{
    open: boolean
    agendaId: number
    interacaoParaEditar?: ItemAgendaGestor | null
    onSuccessExtra?: () => void
  }>({ open: false, agendaId: 0 })
  const [modalDetalhesAberto, setModalDetalhesAberto] = useState(false)
  const [itemDetalhesFromState, setItemDetalhesFromState] = useState<ItemAgendaGestor | null>(null)
  const [tipoDetalhesFromState, setTipoDetalhesFromState] = useState<TipoItemAgenda>('agenda')
  const [loadingDetalhesFromState, setLoadingDetalhesFromState] = useState(false)

  /** Recarrega detalhes da agenda (usado quando o modal de detalhes está aberto por state) após criar/editar/deletar interação. */
  const recarregarDetalheAgenda = useCallback(async () => {
    const agendaId = itemDetalhesFromState?.agendaId
    if (token && agendaId) {
      try {
        const resultado = await dispatch(carregarAgendaDetalhe({ token, agendaId })).unwrap()
        setItemDetalhesFromState(resultado)
      } catch {
        // mantém estado anterior
      }
    }
    recarregar()
  }, [token, itemDetalhesFromState?.agendaId, dispatch, recarregar])

  /** Abre modal Nova Interação; se o Ver Mais estiver aberto para a mesma agenda, ao criar chama carregarAgenda (recarrega modal). */
  const handleAbrirNovaInteracao = useCallback(
    (agendaId: number) => {
      const deveRecarregarDetalhe =
        itemDetalhesFromState != null && itemDetalhesFromState.agendaId === agendaId
      setModalInteracao({
        open: true,
        agendaId,
        interacaoParaEditar: null,
        onSuccessExtra: deveRecarregarDetalhe ? recarregarDetalheAgenda : undefined,
      })
    },
    [itemDetalhesFromState, recarregarDetalheAgenda],
  )
  const handleAbrirEditarInteracao = useCallback((interacao: ItemAgendaGestor, onSuccessEdit?: () => void) => {
    setModalInteracao({
      open: true,
      agendaId: interacao.agendaId ?? 0,
      interacaoParaEditar: interacao,
      onSuccessExtra: onSuccessEdit,
    })
  }, [])
  const handleFecharModalInteracao = useCallback(() => {
    setModalInteracao((prev) => ({ ...prev, open: false, onSuccessExtra: undefined }))
  }, [])

  // Abre o modal Ver Mais quando a navegação vem da listagem (via state) ou da aba Agenda do VCX. Usa carregarAgendaDetalhe (develop).
  useEffect(() => {
    const state = location.state as LocationStateDetalhes | null | undefined
    const agendaId = state?.agendaId
    const detalhesFallback = state?.detalhesAgenda
    const temState = agendaId != null || detalhesFallback != null
    if (!temState) return

    if (agendaId != null && token && carregamentoPorIdEmAndamentoRef.current !== agendaId) {
      carregamentoPorIdEmAndamentoRef.current = agendaId
      setLoadingDetalhesFromState(true)
      dispatch(carregarAgendaDetalhe({ token, agendaId }))
        .unwrap()
        .then((item) => {
          setItemDetalhesFromState(item)
          setTipoDetalhesFromState('agenda')
          setModalDetalhesAberto(true)
          navigate(location.pathname, { replace: true, state: {} })
        })
        .catch((mensagemErro: string | undefined) => {
          if (detalhesFallback) {
            setItemDetalhesFromState(detalhesFallback)
            setTipoDetalhesFromState('agenda')
            setModalDetalhesAberto(true)
          }
          navigate(location.pathname, { replace: true, state: {} })
          if (!detalhesFallback && mensagemErro) {
            toast.error('Não foi possível carregar os detalhes da agenda', {
              description: mensagemErro,
            })
          }
        })
        .finally(() => {
          carregamentoPorIdEmAndamentoRef.current = null
          setLoadingDetalhesFromState(false)
        })
      return
    }

    if (agendaId == null && detalhesFallback) {
      setItemDetalhesFromState(detalhesFallback)
      setTipoDetalhesFromState('agenda')
      setModalDetalhesAberto(true)
    }
    navigate(location.pathname, { replace: true, state: {} })
  }, [
    location.state,
    location.pathname,
    navigate,
    dispatch,
    token,
  ])

  const handleOpenModalAgenda = () => {
    setModalAgendaAberto(true)
    handleNovaAgenda()
  }

  return (
    <div ref={setScrollContainerRef} className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb items={[{ label: 'Agendas Comerciais' }]} />

      <PageHeader
        title="Agendas Comerciais"
        description="Gerencie suas agendas, interações e ações comerciais"
        actions={
          <div className="flex gap-2">
            {/* Botão de filtros mobile */}
            <Sheet open={filtrosAbertos} onOpenChange={setFiltrosAbertos}>
              <SheetTrigger asChild>
                <Button variant="outline" size="icon" className="lg:hidden">
                  <Filter className="h-4 w-4" />
                </Button>
              </SheetTrigger>
              <SheetContent side="left" className="w-[300px] sm:w-[400px] overflow-y-auto">
                <SheetHeader>
                  <SheetTitle>Filtros</SheetTitle>
                </SheetHeader>
                <div className="mt-4">
                  <FiltrosAgenda filtros={filtros} onFiltroChange={handleFiltroChange} />
                </div>
              </SheetContent>
            </Sheet>
            
            <Button onClick={handleOpenModalAgenda} className="gap-2">
              <Plus className="h-4 w-4" />
              <span className="hidden sm:inline">Nova Agenda</span>
              <span className="sm:hidden">Nova</span>
            </Button>
          </div>
        }
      />

      {error && (
        <div className="bg-destructive/10 text-destructive px-4 py-3 rounded-lg">
          <p className="text-sm font-medium">Erro ao carregar agendas</p>
          <p className="text-xs mt-1">{error}</p>
          <Button variant="outline" size="sm" className="mt-2" onClick={recarregar}>
            Tentar novamente
          </Button>
        </div>
      )}

      <div className="flex gap-6">
        {/* Sidebar de filtros */}
        <div className="hidden lg:block">
          <FiltrosAgenda filtros={filtros} onFiltroChange={handleFiltroChange} />
        </div>

        {/* Timeline principal: recebe agendas/interacoes/acoes atualizados quando recarregar termina (historico no slice). */}
        <div className="flex-1 min-w-0 relative">
          {isRefetching && (
            <div className="absolute top-0 right-0 z-10 flex items-center gap-1.5 rounded-md bg-muted/90 px-2 py-1 text-xs text-muted-foreground">
              <span className="h-3 w-3 animate-spin rounded-full border-2 border-muted-foreground border-t-transparent" />
              Atualizando...
            </div>
          )}
          <TimelineAgendas
            agendas={agendas}
            interacoes={interacoes}
            acoes={acoes}
            loading={loading}
            onInteracaoEditOrDelete={recarregar}
            onRecarregar={recarregar}
            onAbrirNovaInteracao={handleAbrirNovaInteracao}
            onAbrirEditarInteracao={handleAbrirEditarInteracao}
          />
        </div>
      </div>

      {/* Modal Nova Agenda - recarrega lista apenas quando criar/atualizar agenda (onSuccess) */}
      <NovaAgendaModal
        open={modalAgendaAberto}
        onOpenChange={setModalAgendaAberto}
        onSuccess={recarregar}
      />

      {/* Modal criar/editar interação no nível da página (fora de qualquer outro Dialog) para exibir corretamente. */}
      <NovaInteracaoModal
        open={modalInteracao.open}
        onOpenChange={(open) => !open && handleFecharModalInteracao()}
        agendaId={modalInteracao.agendaId}
        interacaoParaEditar={modalInteracao.interacaoParaEditar}
        onSuccess={async () => {
          // Capturar valores ANTES de fechar o modal (handleFecharModalInteracao limpa onSuccessExtra)
          const extra = modalInteracao.onSuccessExtra
          const agendaIdCriado = modalInteracao.agendaId
          const agendaIdEmExibicao = itemDetalhesFromState?.agendaId
          
          // Recarrega o modal Ver Mais (chama Encontros/carregarAgenda/{id}) quando criado a partir dele ou quando mesma agenda em exibição
          if (extra) {
            await extra()
          } else if (agendaIdEmExibicao === agendaIdCriado) {
            await recarregarDetalheAgenda()
          } else {
            recarregar()
          }
          
          handleFecharModalInteracao()
        }}
      />

      {/* Overlay de loading ao buscar detalhes (navegação da VCX ou por state) */}
      {loadingDetalhesFromState && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-background/80">
          <div className="flex flex-col items-center gap-3">
            <Spinner size={32} className="text-primary" />
            <p className="text-sm text-muted-foreground">Carregando detalhes da agenda...</p>
          </div>
        </div>
      )}

      {/* Modal Ver Mais - aberto ao navegar da listagem (AgendaCard) ou da aba Agenda do VCX; dados populados por carregarAgendaDetalhe */}
      {itemDetalhesFromState && (
        <DetalhesAgendaModal
          open={modalDetalhesAberto}
          onOpenChange={(aberto) => {
            setModalDetalhesAberto(aberto)
            if (!aberto) setItemDetalhesFromState(null)
          }}
          item={itemDetalhesFromState}
          tipo={tipoDetalhesFromState}
          loadingDetalhe={loadingDetalhesFromState}
          onAbrirNovaInteracao={handleAbrirNovaInteracao}
          onInteracaoEditOrDelete={recarregarDetalheAgenda}
          onAbrirEditarInteracao={(interacao, onSuccessEdit) =>
            handleAbrirEditarInteracao(interacao, onSuccessEdit ?? recarregarDetalheAgenda)
          }
          onRecarregarDetalhe={recarregarDetalheAgenda}
        />
      )}
    </div>
  )
}
