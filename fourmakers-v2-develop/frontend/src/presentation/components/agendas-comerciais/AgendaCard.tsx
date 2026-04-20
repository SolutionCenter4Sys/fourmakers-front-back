import { useState, useRef, useCallback } from 'react'
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Calendar, User, MapPin, Video, Users, Building2, Clock, Edit, Trash2, MessageSquare, Check, X as XIcon, Timer, PlusCircle } from 'lucide-react'
import { Spinner } from '@/components/ui/spinner'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip'
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
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  deletarAgenda,
  buscarAgendasComerciais,
  aceitarConviteAgenda,
  carregarAgendaDetalhe,
  limparAgendaDetalhe,
} from '@app/store/slices/agendasComerciaisSlice'
import { toast } from 'sonner'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import {
  getCodInternoColaborador,
  calcularDuracaoAgenda,
  LABEL_DATA_NAO_INFORMADA,
} from '@shared/utils/agendaUtils'
import {
  obterDiaCalendarioAgenda,
  parseDataHoraAgendaParaExibicao,
} from '@shared/utils/timezoneAgendaUtils'
import { getTipoInteracaoIcon } from '@shared/utils/agendaTipoInteracaoIcon'
import {
  verificarSeUsuarioECriador,
  verificarSeDeveMostrarBotoesConvite,
  obterBadgeConviteDoUsuario,
  deveMostrarReconsiderar,
  podeCriarInteracao,
} from '@shared/utils/agendaPermissions'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import { DetalhesAgendaModal } from './DetalhesAgendaModal'
import { NovaAgendaModal } from './NovaAgendaModal'
import { NovaInteracaoModal } from './NovaInteracaoModal'

interface AgendaCardProps {
  agenda: ItemAgendaGestor
  /** Recarrega a lista da página (preserva scroll e filtros); quando informado, usado ao criar/editar interação para não perder posição. */
  onRecarregar?: () => void
  /** Abre modal de nova interação no nível da página (evita dialog aninhado). */
  onAbrirNovaInteracao?: (agendaId: number) => void
  /** Abre modal de editar interação no nível da página (para InteracaoCards dentro do detalhe da agenda). */
  onAbrirEditarInteracao?: (interacao: ItemAgendaGestor) => void
}

/**
 * Componente de card para exibir informações de uma agenda comercial.
 * Exibe título, tipo de interação, status, data, responsável, localização,
 * link de reunião, participantes e descrição.
 */

export function AgendaCard({ agenda, onRecarregar, onAbrirNovaInteracao, onAbrirEditarInteracao }: AgendaCardProps) {
  const dispatch = useAppDispatch()
  const { token } = useAppSelector((state) => state.auth)
  const { user } = useAppSelector((state) => state.auth)
  const { agendaDetalheCarregada, statusDetalhe } = useAppSelector((state) => state.agendasComerciais)

  const [modalDetalhesAberto, setModalDetalhesAberto] = useState(false)
  const [modalNovaAgendaComClienteAberto, setModalNovaAgendaComClienteAberto] = useState(false)
  const [modalEditarAberto, setModalEditarAberto] = useState(false)
  const [carregandoDetalheParaEditar, setCarregandoDetalheParaEditar] = useState(false)
  const [modalDeletarAberto, setModalDeletarAberto] = useState(false)
  const [modalNovaInteracaoAberto, setModalNovaInteracaoAberto] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [isRespondingConvite, setIsRespondingConvite] = useState(false)
  const [isRefetching, setIsRefetching] = useState(false)
  const recarregarTimeoutRef = useRef<NodeJS.Timeout | null>(null)
  
  // Data exata da agenda = dataAgendada (dia) + dataInicio/dataFim (hora)
  const diaCalendario = obterDiaCalendarioAgenda(agenda.data)
  const dataCompleta = diaCalendario
    ? format(diaCalendario, 'dd/MM/yyyy', { locale: ptBR })
    : LABEL_DATA_NAO_INFORMADA

  const dataInicioParsed = agenda.dataInicio ? parseDataHoraAgendaParaExibicao(agenda.dataInicio) : null
  const dataFimParsed = agenda.dataFim ? parseDataHoraAgendaParaExibicao(agenda.dataFim) : null
  
  const horaInicio = dataInicioParsed
    ? format(dataInicioParsed, 'HH:mm', { locale: ptBR })
    : null
  const horaFim = dataFimParsed
    ? format(dataFimParsed, 'HH:mm', { locale: ptBR })
    : null
  
  // Calcular duração se tiver dataInicio e dataFim
  const duracao = calcularDuracaoAgenda(dataInicioParsed, dataFimParsed)
  
  // Verificar permissões e badge de convite (statusSolicitacaoParticipante: 0=Pendente, 1=Aceito, 2=Recusado, 3=Talvez, null=sem badge)
  const isCriador = verificarSeUsuarioECriador(agenda, user)
  const podeCriarInteracaoNestaAgenda = podeCriarInteracao(agenda, user)
  const deveMostrarBotoesConvite = verificarSeDeveMostrarBotoesConvite(agenda, user)
  const deveMostrarReconsiderarBtn = deveMostrarReconsiderar(agenda, user)
  const badgeConvite = obterBadgeConviteDoUsuario(agenda, user)

  // Função para recarregar com debounce e deduplicação (usada quando onRecarregar não está disponível)
  const recarregarAgendas = useCallback(async () => {
    if (isRefetching || !token) return

    // Limpar timeout anterior se existir
    if (recarregarTimeoutRef.current) {
      clearTimeout(recarregarTimeoutRef.current)
    }

    // Debounce de 300ms
    recarregarTimeoutRef.current = setTimeout(async () => {
      setIsRefetching(true)
      try {
        const codInternoColaborador = getCodInternoColaborador(user)
        if (codInternoColaborador) {
          await dispatch(
            buscarAgendasComerciais({
              token,
              codInternoColaborador,
            }),
          )
        }
      } finally {
        setIsRefetching(false)
        recarregarTimeoutRef.current = null
      }
    }, 300)
  }, [token, user, dispatch, isRefetching])

  /** Recarrega a lista: usa onRecarregar da página (preserva scroll e filtros) quando disponível; senão recarrega local via recarregarAgendas. */
  const recarregarLista = useCallback(() => {
    if (onRecarregar) {
      onRecarregar()
      return
    }
    void recarregarAgendas()
  }, [onRecarregar, recarregarAgendas])

  const handleDeletar = async () => {
    if (!token || !agenda.agendaId) return

    // Verificar se a agenda tem encontros/interações
    if (agenda.encontros && agenda.encontros.length > 0) {
      toast.error('Não é possível deletar', {
        description: 'Esta agenda possui interações associadas. Remova as interações antes de deletar a agenda.',
      })
      setModalDeletarAberto(false)
      return
    }

    setIsDeleting(true)
    try {
      await dispatch(deletarAgenda({ token, agendaId: agenda.agendaId })).unwrap()
      
      // Rastrear ação do usuário
      if (user) {
        logUserAction(
          'AgendasComerciais',
          'DeletarAgenda',
          {
            agendaId: agenda.agendaId,
            titulo: agenda.titulo,
            tipoInteracao: agenda.tipoInteracao,
          },
          user
        )
      }
      
      toast.success('Agenda deletada', { description: 'A agenda foi deletada com sucesso' })
      
      recarregarLista()
      setModalDeletarAberto(false)
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao deletar agenda',
      })
    } finally {
      setIsDeleting(false)
    }
  }

  const codigoColaborador = getCodInternoColaborador(user)

  /** Confirmar presença (botão "Confirmar Presença") ou reconsiderar (botão "Reconsiderar") — ambos enviam decisaoStatus: 1. */
  const handleConfirmarPresencaOuReconsiderar = async (acao: 'ConfirmarParticipacao' | 'ReconsiderarParticipacao') => {
    if (!token || !agenda.agendaId || !codigoColaborador) return
    setIsRespondingConvite(true)
    try {
      await dispatch(
        aceitarConviteAgenda({
          token,
          agendaId: agenda.agendaId.toString(),
          codigoColaborador,
          decisaoStatus: 1,
        }),
      ).unwrap()
      if (user) {
        logUserAction('AgendasComerciais', acao, { agendaId: agenda.agendaId, titulo: agenda.titulo }, user)
      }
      toast.success('Presença confirmada', {
        description:
          acao === 'ReconsiderarParticipacao'
            ? 'Você reconsiderou e confirmou presença nesta agenda'
            : 'Sua presença foi confirmada nesta agenda',
      })
      void recarregarLista()
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao responder convite',
      })
    } finally {
      setIsRespondingConvite(false)
    }
  }

  const handleResponderConvite = async (aceitar: boolean) => {
    if (!token || !agenda.agendaId || !codigoColaborador) return
    setIsRespondingConvite(true)
    try {
      await dispatch(
        aceitarConviteAgenda({
          token,
          agendaId: agenda.agendaId.toString(),
          codigoColaborador,
          decisaoStatus: aceitar ? 1 : 2,
        }),
      ).unwrap()
      if (user) {
        logUserAction(
          'AgendasComerciais',
          aceitar ? 'AceitarConviteAgenda' : 'RecusarConviteAgenda',
          { agendaId: agenda.agendaId, titulo: agenda.titulo, decisaoStatus: aceitar ? 1 : 2 },
          user
        )
      }
      toast.success(
        aceitar ? 'Convite aceito' : 'Convite recusado',
        { description: aceitar ? 'Você aceitou o convite para esta agenda' : 'Você recusou o convite para esta agenda' }
      )
      void recarregarLista()
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao responder convite',
      })
    } finally {
      setIsRespondingConvite(false)
    }
  }

  return (
    <>
      <Card className={`hover:shadow-lg transition-shadow rounded-lg w-full flex flex-col ${deveMostrarBotoesConvite ? 'border-2 border-warning' : ''}`}>
        <CardHeader className="pb-3">
          <div className="flex items-start justify-between gap-2">
            <div className="flex-1 min-w-0">
              <div className="flex items-center gap-2 mb-1 flex-wrap">
                {getTipoInteracaoIcon(agenda.tipoInteracao, 'h-4 w-4')}
                {agenda.tipoInteracao && (
                  <Badge variant="secondary" className="text-xs">
                    {agenda.tipoInteracao}
                  </Badge>
                )}
                {badgeConvite && (
                  <Badge variant={badgeConvite.variant} className={badgeConvite.className ? `text-xs ${badgeConvite.className}` : 'text-xs'}>
                    {badgeConvite.label}
                  </Badge>
                )}
              </div>
              <CardTitle className="text-base font-semibold line-clamp-2">{agenda.titulo}</CardTitle>
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-2 flex-1">
          <div className="flex items-center gap-2 text-sm text-muted-foreground flex-wrap">
            <span className="flex items-center gap-1.5">
              <Calendar className="h-4 w-4 shrink-0" />
              <span>{dataCompleta}</span>
            </span>
            {horaInicio && horaFim && (
              <>
                <span className="text-muted-foreground/60">•</span>
                <span className="flex items-center gap-1.5">
                  <Clock className="h-4 w-4 shrink-0" />
                  <span>{horaInicio} - {horaFim}</span>
                </span>
              </>
            )}
            {duracao && (
              <>
                <span className="text-muted-foreground/60">•</span>
                <span className="flex items-center gap-1.5">
                  <Timer className="h-4 w-4 shrink-0" />
                  <span>{duracao} min</span>
                </span>
              </>
            )}
          </div>

          {agenda.cliente && (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Building2 className="h-4 w-4 shrink-0" />
              <span className="truncate">{agenda.cliente.nomeCliente}</span>
              {agenda.cliente.codigoCliente &&
                String(agenda.cliente.codigoCliente).trim() !== '' &&
                String(agenda.cliente.codigoCliente) !== '0' && (
                  <Badge variant="outline" className="text-xs">
                    {agenda.cliente.codigoCliente}
                  </Badge>
                )}
            </div>
          )}

          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <User className="h-4 w-4 shrink-0" />
            <span className="truncate">{agenda.responsavel}</span>
          </div>

          {agenda.localizacao && (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <MapPin className="h-4 w-4 shrink-0" />
              <span className="truncate">{agenda.localizacao}</span>
            </div>
          )}

          {agenda.linkReuniao && (
            <div className="flex items-center gap-2 text-sm">
              <Video className="h-4 w-4 shrink-0" />
              <a
                href={agenda.linkReuniao}
                target="_blank"
                rel="noopener noreferrer"
                className="text-primary hover:underline truncate"
              >
                Entrar na reunião
              </a>
            </div>
          )}

          {agenda.participantes && agenda.participantes > 0 && (
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Users className="h-4 w-4 shrink-0" />
              <span>{agenda.participantes} participantes</span>
            </div>
          )}

          {agenda.descricao && (
            <p className="text-sm text-muted-foreground line-clamp-2 mt-2">{agenda.descricao}</p>
          )}
        </CardContent>
        <CardFooter className="pt-3 mt-auto flex flex-col gap-2">
          {/* Botão Confirmar Presença (status pendente) */}
          {deveMostrarBotoesConvite && (
            <div className="flex gap-2 w-full">
              <Button
                size="sm"
                className="flex-1 bg-success hover:bg-success/90 text-white"
                onClick={() => handleConfirmarPresencaOuReconsiderar('ConfirmarParticipacao')}
                disabled={isRespondingConvite}
              >
                {isRespondingConvite ? (
                  <Spinner size={16} className="text-white" />
                ) : (
                  <>
                    <Check className="h-4 w-4 mr-1" />
                    Confirmar Presença
                  </>
                )}
              </Button>
              {/* Recusar Agenda: ocultado conforme solicitação. Código preservado para reativação futura. */}
              {false && (
                <Button
                  size="sm"
                  variant="outline"
                  className="flex-1"
                  onClick={() => handleResponderConvite(false)}
                  disabled={isRespondingConvite}
                >
                  {isRespondingConvite ? (
                    <Spinner size={16} className="text-primary" />
                  ) : (
                    <>
                      <XIcon className="h-4 w-4 mr-1" />
                      Recusar
                    </>
                  )}
                </Button>
              )}
            </div>
          )}
          {/* Botão Reconsiderar (status recusado) */}
          {deveMostrarReconsiderarBtn && (
            <Button
              size="sm"
              className="w-full bg-success hover:bg-success/90 text-white"
              onClick={() => handleConfirmarPresencaOuReconsiderar('ReconsiderarParticipacao')}
              disabled={isRespondingConvite}
            >
              {isRespondingConvite ? (
                <Spinner size={16} className="text-white" />
              ) : (
                <>
                  <Check className="h-4 w-4 mr-1" />
                  Reconsiderar
                </>
              )}
            </Button>
          )}
          
          {/* Botões de Ação Normais */}
          <div className="flex gap-2 w-full">
            <Button
              variant="ghost"
              size="sm"
              className="flex-1"
              onClick={() => {
                // Só dispara carregamento se ainda não temos os detalhes desta agenda (evita chamada duplicada)
                const jaTemDetalhe = agendaDetalheCarregada?.agendaId === agenda.agendaId
                if (agenda.agendaId && token && !jaTemDetalhe) {
                  dispatch(carregarAgendaDetalhe({ token, agendaId: agenda.agendaId }))
                }
                setModalDetalhesAberto(true)
              }}
            >
              {statusDetalhe === 'loading' && agendaDetalheCarregada?.agendaId === agenda.agendaId ? (
                <Spinner size={16} className="text-primary" />
              ) : (
                'Ver Mais'
              )}
            </Button>
            {podeCriarInteracaoNestaAgenda && (
              <Button
                variant="ghost"
                size="sm"
                className="h-9 w-9 p-0"
                title="Nova Interação"
                aria-label="Nova Interação"
                onClick={() => {
                  if (onAbrirNovaInteracao && agenda.agendaId) {
                    onAbrirNovaInteracao(agenda.agendaId)
                  } else {
                    setModalNovaInteracaoAberto(true)
                  }
                }}
              >
                <MessageSquare className="h-4 w-4" />
              </Button>
            )}
            <TooltipProvider>
              <Tooltip>
                <TooltipTrigger asChild>
                  <span className="inline-block">
                    <Button
                      variant="ghost"
                      size="sm"
                      disabled
                      className="h-9 w-9 p-0 opacity-50 cursor-not-allowed"
                      aria-label="Agendas no Cliente: Em Breve"
                    >
                      <PlusCircle className="h-4 w-4" />
                    </Button>
                  </span>
                </TooltipTrigger>
                <TooltipContent>
                  <p>Agendas no Cliente: Em Breve</p>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
            {/* Botões de editar/deletar apenas para o criador */}
            {isCriador && (
              <>
                <Button
                  variant="ghost"
                  size="sm"
                  disabled={carregandoDetalheParaEditar}
                  onClick={async () => {
                    if (!token || !agenda.agendaId) return
                    setCarregandoDetalheParaEditar(true)
                    try {
                      await dispatch(carregarAgendaDetalhe({ token, agendaId: agenda.agendaId })).unwrap()
                      setModalEditarAberto(true)
                    } catch (err) {
                      toast.error('Erro ao carregar detalhes', {
                        description: err instanceof Error ? err.message : 'Não foi possível carregar os dados da agenda.',
                      })
                    } finally {
                      setCarregandoDetalheParaEditar(false)
                    }
                  }}
                  className="h-9 w-9 p-0"
                  title="Editar Agenda"
                >
                  {carregandoDetalheParaEditar ? (
                    <Spinner size={16} className="text-primary" />
                  ) : (
                    <Edit className="h-4 w-4" />
                  )}
                </Button>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => setModalDeletarAberto(true)}
                  disabled={agenda.encontros && agenda.encontros.length > 0}
                  className="h-9 w-9 p-0 text-destructive hover:text-destructive disabled:opacity-50 disabled:cursor-not-allowed"
                  title={
                    agenda.encontros && agenda.encontros.length > 0
                      ? 'Não é possível deletar agenda com interações'
                      : 'Deletar Agenda'
                  }
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </>
            )}
          </div>
        </CardFooter>
      </Card>
      
      <DetalhesAgendaModal
        open={modalDetalhesAberto}
        onOpenChange={(open) => {
          setModalDetalhesAberto(open)
          if (!open) {
            dispatch(limparAgendaDetalhe())
          }
        }}
        item={
          (agendaDetalheCarregada?.agendaId === agenda.agendaId ? agendaDetalheCarregada : null) ?? agenda
        }
        tipo="agenda"
        loadingDetalhe={statusDetalhe === 'loading'}
        onNovaInteracao={() => {
          // Sempre usa o fluxo interno do AgendaCard (não fecha o modal Ver Mais)
          setModalNovaInteracaoAberto(true)
        }}
        onInteracaoEditOrDelete={() => {
          if (token && agenda.agendaId) {
            dispatch(carregarAgendaDetalhe({ token, agendaId: agenda.agendaId }))
          }
          recarregarLista()
        }}
        onAgendaFilhaCriada={() => {
          if (token && agenda.agendaId) {
            dispatch(carregarAgendaDetalhe({ token, agendaId: agenda.agendaId }))
          }
          recarregarLista()
        }}
        onAbrirEditarInteracao={onAbrirEditarInteracao}
      />
      
      <NovaAgendaModal
        open={modalEditarAberto}
        onOpenChange={setModalEditarAberto}
        onSuccess={() => void recarregarLista()}
        agendaParaEditar={
          (agendaDetalheCarregada?.agendaId === agenda.agendaId ? agendaDetalheCarregada : null) ?? agenda
        }
      />

      <NovaAgendaModal
        open={modalNovaAgendaComClienteAberto}
        onOpenChange={setModalNovaAgendaComClienteAberto}
        agendaPai={agenda}
        onSuccess={() => void recarregarLista()}
      />
      
      <NovaInteracaoModal
        open={modalNovaInteracaoAberto}
        onOpenChange={setModalNovaInteracaoAberto}
        agendaId={agenda.agendaId || 0}
        onSuccess={async () => {
          // Se o modal Ver Mais está aberto, recarrega os detalhes da agenda
          if (modalDetalhesAberto && token && agenda.agendaId) {
            await dispatch(carregarAgendaDetalhe({ token, agendaId: agenda.agendaId }))
          }
          recarregarLista()
        }}
      />
      
      <AlertDialog open={modalDeletarAberto} onOpenChange={setModalDeletarAberto}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar exclusão</AlertDialogTitle>
            <AlertDialogDescription>
              {agenda.encontros && agenda.encontros.length > 0 ? (
                <>
                  Esta agenda possui {agenda.encontros.length} interação(ões) associada(s). 
                  Remova as interações antes de deletar a agenda.
                </>
              ) : (
                <>
                  Tem certeza que deseja deletar a agenda &quot;{agenda.titulo}&quot;? Esta ação não pode ser desfeita.
                </>
              )}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isDeleting}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeletar}
              disabled={isDeleting || (agenda.encontros && agenda.encontros.length > 0)}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90 disabled:opacity-50"
            >
              {isDeleting ? 'Deletando...' : 'Deletar'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}
