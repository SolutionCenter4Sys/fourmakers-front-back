import { useCallback, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  carregarAgenda,
  carregarMaisAgenda,
  type CarregarAgendaParams,
} from '@app/store/slices/vcx360Slice'
import { Card, CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Spinner } from '@/components/ui/spinner'
import {
  User,
  Calendar,
  MapPin,
  Clock,
  Users,
  Plus,
  CalendarCheck,
  CloudOff,
  ExternalLink,
  Video,
} from 'lucide-react'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { VcxAgenda } from '@domain/entities/VcxAgenda'
import {
  mesclarEOrdenarAgendas,
  encontrarAgendaPai,
  contarAgendasFilhas,
  obterLabelTipoInteracao,
  formatarDataCrua,
  formatarIntervaloHorarioCruo,
  vcxAgendaParaItemAgendaGestor,
} from '@shared/utils/vcxAgendaUtils'

interface AbaAgendaProps {
  no: NoMapaRelacionamento
}

interface CardAgendaProps {
  agenda: VcxAgenda
  todas: VcxAgenda[]
  onVerMaisNaAgendasComerciais?: (agenda: VcxAgenda) => void
}

function CardAgenda({ agenda, todas, onVerMaisNaAgendasComerciais }: CardAgendaProps) {
  const dataExibicao = agenda.dataInicio ?? agenda.dataAgendada
  const pai = encontrarAgendaPai(agenda, todas)
  const qtdFilhas = contarAgendasFilhas(agenda.id, todas)

  return (
    <Card className="rounded-lg border border-border bg-card">
      <CardContent className="p-3.5 space-y-2">
        <div className="flex items-center justify-between gap-2">
          <span className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">
            Agenda
          </span>
          <div className="flex items-center gap-1.5">
            <TooltipProvider delayDuration={300}>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 shrink-0 text-muted-foreground hover:text-primary"
                    aria-label="Ver mais na tela Agendas Comerciais"
                    onClick={() => onVerMaisNaAgendasComerciais?.(agenda)}
                  >
                    <ExternalLink className="w-4 h-4" />
                  </Button>
                </TooltipTrigger>
                <TooltipContent side="left" className="max-w-[220px]">
                  <p className="font-medium">Ver mais na tela Agendas Comerciais</p>
                  <p className="text-xs text-muted-foreground mt-0.5">
                    Abre o modal com os detalhes desta agenda
                  </p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          </div>
        </div>

        <p className="text-sm font-semibold text-foreground line-clamp-2">
          {agenda.titulo || 'Sem título'}
        </p>

        {obterLabelTipoInteracao(agenda.tipoInteracao) && (
          <Badge variant="secondary" className="text-xs">
            {obterLabelTipoInteracao(agenda.tipoInteracao)}
          </Badge>
        )}

        <div className="flex flex-col gap-1 text-xs text-muted-foreground">
          <div className="flex items-center gap-1.5">
            <Calendar className="w-3.5 h-3.5 shrink-0" />
            {formatarDataCrua(dataExibicao)}
          </div>
          <div className="flex items-center gap-1.5">
            <Clock className="w-3.5 h-3.5 shrink-0" />
            {formatarIntervaloHorarioCruo(agenda.dataInicio, agenda.dataFim)}
          </div>
          {agenda.linkReuniao?.trim() && (
            <div className="flex items-center gap-1.5">
              <Video className="w-3.5 h-3.5 shrink-0" />
              <a
                href={agenda.linkReuniao.trim()}
                target="_blank"
                rel="noopener noreferrer"
                className="text-primary underline underline-offset-2 hover:text-primary/80 truncate"
              >
                Link da reunião
              </a>
            </div>
          )}
          <div className="flex items-center gap-1.5">
            <MapPin className="w-3.5 h-3.5 shrink-0" />
            {agenda.localizacao?.trim() || '—'}
          </div>
        </div>

        {agenda.quantidadeParticipantes != null && (
          <div className="flex flex-wrap gap-2 pt-1">
            <span className="inline-flex items-center gap-1 rounded-md border border-border bg-muted/30 px-2 py-0.5 text-xs text-muted-foreground">
              <Users className="w-3 h-3" />
              {agenda.quantidadeParticipantes} participante(s)
            </span>
          </div>
        )}

        <div className="space-y-1.5 pt-2 border-t border-border">
          <div className="rounded border border-border bg-muted/20 px-2 py-1.5 text-xs">
            <span className="font-medium text-muted-foreground">Agenda pai: </span>
            {pai ? (pai.titulo || 'Sem título') : 'não possui vínculo'}
          </div>
          <div className="rounded border border-border bg-muted/20 px-2 py-1.5 text-xs">
            <span className="font-medium text-muted-foreground">Agendas filhas: </span>
            {qtdFilhas > 0
              ? `${qtdFilhas} agenda(s) filha(s) vinculada(s)`
              : 'Nenhuma agenda filha'}
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

export function AbaAgenda({ no }: AbaAgendaProps) {
  const dispatch = useAppDispatch()
  const navigate = useNavigate()
  const {
    agendasAntigas,
    agendasNovas,
    agendaHasMore,
    agendaLoadingMore,
    agendaErrorMessage,
  } = useAppSelector((state) => state.vcx360)
  const clienteSelecionado = useAppSelector(
    (state) => state.mapaRelacionamento.clienteSelecionado,
  )

  const handleVerMaisNaAgendasComerciais = useCallback(
    (agenda: VcxAgenda) => {
      const agendaId = agenda.id
      if (agendaId == null) return
      navigate('/agendas-comerciais', {
        state: {
          agendaId,
          detalhesAgenda: vcxAgendaParaItemAgendaGestor(agenda),
        },
      })
    },
    [navigate],
  )

  const codigoColaborador =
    no.employeeId && no.employeeId !== 'vacant' ? no.employeeId : ''
  const codigoCliente = clienteSelecionado?.codigoCliente ?? ''
  const temCredenciais =
    codigoColaborador.length > 0 && codigoCliente.length > 0

  const timeline = useMemo(
    () => mesclarEOrdenarAgendas(agendasAntigas, agendasNovas),
    [agendasAntigas, agendasNovas],
  )

  const params: CarregarAgendaParams = useMemo(
    () => ({
      codigoColaborador,
      codigoCliente,
    }),
    [codigoColaborador, codigoCliente],
  )

  // Sem credenciais: estado vazio
  if (!temCredenciais) {
    return (
      <div className="flex flex-col items-center justify-center py-10 px-4 text-center">
        <div className="rounded-full border border-border p-4 mb-3">
          <User className="w-10 h-10 text-muted-foreground" />
        </div>
        <h3 className="text-sm font-semibold text-foreground mb-1">
          Agenda indisponível
        </h3>
        <p className="text-sm text-muted-foreground leading-relaxed max-w-[260px]">
          Agenda disponível apenas quando o colaborador e o cliente estão definidos
          no perfil.
        </p>
      </div>
    )
  }

  // Erro: mensagem + botão tentar novamente
  if (agendaErrorMessage) {
    return (
      <div className="flex flex-col items-center justify-center py-10 px-4 text-center">
        <CloudOff className="w-12 h-12 text-destructive mb-3" />
        <h3 className="text-sm font-semibold text-foreground mb-1">
          Não foi possível carregar a agenda
        </h3>
        <p className="text-sm text-muted-foreground mb-4 max-w-[280px]">
          {agendaErrorMessage}
        </p>
        <Button
          variant="primary"
          size="sm"
          onClick={() =>
            dispatch(carregarAgenda({ ...params, cursor: 0 }))
          }
          className="rounded-full"
        >
          Tentar novamente
        </Button>
      </div>
    )
  }

  // Loading inicial (lista vazia e loading)
  if (timeline.length === 0 && agendaLoadingMore) {
    return (
      <div className="flex items-center justify-center gap-2 py-10">
        <Spinner size={20} className="text-primary" />
        <span className="text-sm text-muted-foreground">
          Carregando agenda...
        </span>
      </div>
    )
  }

  // Sem dados
  if (timeline.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-10 px-4 text-center">
        <div className="rounded-full border border-border p-4 mb-3">
          <CalendarCheck className="w-10 h-10 text-muted-foreground" />
        </div>
        <h3 className="text-sm font-semibold text-foreground mb-1">
          Nenhuma agenda encontrada
        </h3>
        <p className="text-sm text-muted-foreground leading-relaxed max-w-[260px]">
          Não há agendas vinculadas a este colaborador e cliente no período.
        </p>
      </div>
    )
  }

  // Timeline com cards
  return (
    <div className="space-y-4">
      <h2 className="text-[10px] font-bold uppercase tracking-widest text-muted-foreground">
        Histórico de agendas
      </h2>

      <div className="relative space-y-0">
        {/* Linha vertical da timeline */}
        <div
          className="absolute left-3 top-3 bottom-3 w-0.5 bg-border -translate-x-1/2"
          aria-hidden
        />
        {timeline.map((agenda, index) => (
          <div key={agenda.id ?? index} className="relative flex gap-3 pl-8">
            <div
              className="absolute left-0 top-5 w-3 h-3 rounded-full border-2 border-border bg-card z-[1] -translate-x-1/2"
              aria-hidden
            />
            <div className="flex-1 min-w-0 pb-4">
              <CardAgenda
                agenda={agenda}
                todas={timeline}
                onVerMaisNaAgendasComerciais={handleVerMaisNaAgendasComerciais}
              />
            </div>
          </div>
        ))}
      </div>

      {agendaHasMore && (
        <div className="flex justify-center pt-2">
          {agendaLoadingMore ? (
            <Spinner size={28} className="text-primary" />
          ) : (
            <Button
              variant="ghost"
              size="sm"
              onClick={() => dispatch(carregarMaisAgenda(params))}
              className="rounded-full gap-1.5"
            >
              <Plus className="w-4 h-4" />
              Carregar mais
            </Button>
          )}
        </div>
      )}
    </div>
  )
}
