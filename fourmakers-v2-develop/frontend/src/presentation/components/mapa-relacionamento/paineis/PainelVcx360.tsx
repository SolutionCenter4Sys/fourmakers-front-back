import { useEffect, useRef } from 'react'
import { Button } from '@/components/ui/button'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Spinner } from '@/components/ui/spinner'
import { X, Crown, Building2, Briefcase } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  setAbaSelecionada,
  carregarAgenda,
  limparAgenda,
} from '@app/store/slices/vcx360Slice'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { AbaContexto } from './abas/AbaContexto'
import { AbaAgenda } from './abas/AbaAgenda'
import { AbaHistorico } from './abas/AbaHistorico'

interface PainelVcx360Props {
  no: NoMapaRelacionamento
  onFechar: () => void
}

export function PainelVcx360({ no, onFechar }: PainelVcx360Props) {
  const dispatch = useAppDispatch()
  const {
    dadosVcx,
    abaSelecionada,
    status,
    agendasAntigas,
    agendasNovas,
    agendaErrorMessage,
  } = useAppSelector((state) => state.vcx360)
  const { user } = useAppSelector((state) => state.auth)
  const clienteSelecionado = useAppSelector(
    (state) => state.mapaRelacionamento.clienteSelecionado,
  )

  const noIdRef = useRef<string | null>(null)

  // Ao trocar o nó aberto, limpar a lista de agendas e recarregar se a aba Agenda estiver ativa
  useEffect(() => {
    if (noIdRef.current === no.id) return
    noIdRef.current = no.id

    dispatch(limparAgenda())

    const codigoColaborador =
      no.employeeId && no.employeeId !== 'vacant' ? no.employeeId : ''
    const codigoCliente = clienteSelecionado?.codigoCliente ?? ''
    const temCredenciais =
      codigoColaborador.length > 0 && codigoCliente.length > 0

    if (abaSelecionada === 'agenda' && temCredenciais) {
      dispatch(
        carregarAgenda({
          codigoColaborador,
          codigoCliente,
          cursor: 0,
        }),
      )
    }
  }, [
    no.id,
    no.employeeId,
    clienteSelecionado?.codigoCliente,
    abaSelecionada,
    dispatch,
  ])

  const renderConteudo = () => {
    // A aba Agenda não depende de dadosVcx, então pode ser renderizada mesmo sem dadosVcx
    if (abaSelecionada === 'agenda') {
      return <AbaAgenda no={no} />
    }

    // A aba Histórico agora busca dados diretamente da API
    if (abaSelecionada === 'historico') {
      return <AbaHistorico no={no} />
    }

    // A aba Contexto agora usa DoresList que carrega dados diretamente do backend
    // Não precisa mais de dadosVcx para funcionar
    if (abaSelecionada === 'contexto') {
      return (
        <AbaContexto
          contexto={
            dadosVcx?.contexto || {
              doresOportunidades: [],
              propostasValor: [],
              kpis: [],
            }
          }
          no={no}
        />
      )
    }

    // Outras abas ainda dependem de dadosVcx
    if (!dadosVcx) {
      return <p className="text-sm text-muted-foreground">Sem dados disponíveis</p>
    }

    return null
  }

  const isVacant = !no.employeeName || no.employeeId === 'vacant'
  const initials = (no.employeeName || no.profileName || '??')
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2)

  return (
    <div className="h-full w-full bg-card flex flex-col overflow-hidden">
      {/* Header */}
      <div className="flex-shrink-0 border-b border-border">
        {/* Top bar: label + close */}
        <div className="flex items-center justify-between px-4 pt-3 pb-0">
          <span className="text-[10px] font-bold uppercase tracking-widest text-primary">
            Visão 360° Cliente
          </span>
          <Button
            size="icon"
            variant="ghost"
            onClick={onFechar}
            className="shrink-0 h-7 w-7 rounded-full"
            title="Fechar painel"
          >
            <X className="w-4 h-4" />
          </Button>
        </div>

        {/* Profile info */}
        <div className="flex items-center gap-3 px-4 pt-2 pb-3">
          <Avatar className={`h-11 w-11 shrink-0 ${isVacant ? 'opacity-50' : ''}`}>
            <AvatarFallback className="text-sm font-semibold bg-primary/10 text-primary">
              {initials}
            </AvatarFallback>
          </Avatar>

          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2">
              <p
                className={`text-sm font-bold truncate leading-tight ${isVacant ? 'text-muted-foreground italic' : 'text-foreground'}`}
                title={no.employeeName || 'Vago'}
              >
                {isVacant ? 'Perfil Vago' : no.employeeName}
              </p>
              {no.isCLevel && (
                <Badge variant="default" className="text-[10px] px-1.5 py-0 h-4 shrink-0">
                  <Crown className="w-3 h-3 mr-0.5" />
                  C-Level
                </Badge>
              )}
            </div>

            {no.profileName && (
              <div className="flex items-center gap-1.5 mt-0.5">
                <Briefcase className="w-3 h-3 text-muted-foreground shrink-0" />
                <p className="text-xs text-muted-foreground truncate" title={no.profileName}>
                  {no.profileName}
                </p>
              </div>
            )}

            {no.departmentName && (
              <div className="flex items-center gap-1.5 mt-0.5">
                <Building2 className="w-3 h-3 text-muted-foreground/70 shrink-0" />
                <p className="text-xs text-muted-foreground/70 truncate" title={no.departmentName}>
                  {no.departmentName}
                </p>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex-shrink-0 px-4 pt-4">
        <Tabs
          value={abaSelecionada}
          onValueChange={(value) => {
            const aba = value as 'contexto' | 'agenda' | 'historico'
            dispatch(setAbaSelecionada(aba))

            // Lazy load da agenda ao abrir a aba
            if (aba === 'agenda') {
              const codigoColaborador =
                no.employeeId && no.employeeId !== 'vacant' ? no.employeeId : ''
              const codigoCliente = clienteSelecionado?.codigoCliente ?? ''
              const temCredenciais =
                codigoColaborador.length > 0 && codigoCliente.length > 0
              const agendaVazia =
                agendasAntigas.length === 0 && agendasNovas.length === 0
              const semErro = !agendaErrorMessage
              if (temCredenciais && agendaVazia && semErro) {
                dispatch(
                  carregarAgenda({
                    codigoColaborador,
                    codigoCliente,
                    cursor: 0,
                  }),
                )
              }
            }

            // Rastreamento Firebase Analytics
            if (value === 'agenda' && user) {
              logUserAction(
                'MapaRelacionamento',
                'VisualizarAbaAgenda',
                {
                  nodeId: no.id,
                  employeeCode: no.employeeId || null,
                  profileId: no.profileId || null,
                  departmentId: no.departmentId || null,
                },
                user,
              )
            }
          }}
        >
          <TabsList>
            <TabsTrigger value="contexto">Contexto</TabsTrigger>
            <TabsTrigger value="agenda">Agenda</TabsTrigger>
            <TabsTrigger value="historico">Histórico</TabsTrigger>
          </TabsList>
        </Tabs>
      </div>

      {/* Content - Scrollable */}
      <div className="flex-1 overflow-y-auto p-4">
        {status === 'loading' ? (
          <div className="flex items-center gap-2 text-sm text-muted-foreground">
            <Spinner size={16} className="text-primary" />
            <span>Carregando dados...</span>
          </div>
        ) : (
          renderConteudo()
        )}
      </div>
    </div>
  )
}
