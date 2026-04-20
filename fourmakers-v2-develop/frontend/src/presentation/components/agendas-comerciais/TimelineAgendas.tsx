import { useMemo } from 'react'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import {
  parseDataAgendaParaExibicao,
  obterDiaCalendarioAgenda,
  obterDataExataAgenda,
} from '@shared/utils/timezoneAgendaUtils'
import { AgendaCard } from './AgendaCard'
import { InteracaoCard } from './InteracaoCard'
import { AcaoCard } from './AcaoCard'
import { TimelineAgendasSkeleton } from './TimelineAgendasSkeleton'

interface TimelineAgendasProps {
  agendas: ItemAgendaGestor[]
  interacoes: ItemAgendaGestor[]
  acoes: ItemAgendaGestor[]
  loading: boolean
  /** Chamado quando uma interação é editada ou deletada; use para recarregar lista. */
  onInteracaoEditOrDelete?: () => void
  /** Recarrega a lista da página (preserva scroll e filtros); passado para AgendaCard ao criar nova interação. */
  onRecarregar?: () => void
  /** Abre o modal de nova interação no nível da página (evita dialog aninhado). */
  onAbrirNovaInteracao?: (agendaId: number) => void
  /** Abre o modal de editar interação no nível da página. Opcional: onSuccessEdit chamado ao salvar (ex.: recarregar detalhe da agenda). */
  onAbrirEditarInteracao?: (interacao: ItemAgendaGestor, onSuccessEdit?: () => void) => void
}

/**
 * Componente de timeline que agrupa agendas, interações e ações por data.
 * Exibe cabeçalho de data formatado e renderiza cards apropriados para cada tipo.
 * Suporta estados de loading (skeleton) e empty (mensagem).
 */
export function TimelineAgendas({
  agendas,
  interacoes,
  acoes,
  loading,
  onInteracaoEditOrDelete,
  onRecarregar,
  onAbrirNovaInteracao,
  onAbrirEditarInteracao,
}: TimelineAgendasProps) {
  // Combinar e ordenar por data (mais recente primeiro).
  // Agenda: data exata = dataAgendada (dia) + dataInicio (hora). Outros itens: usam item.data.
  const todosItens = useMemo(() => {
    const items = [
      ...(Array.isArray(agendas) ? agendas : []),
      ...(Array.isArray(interacoes) ? interacoes : []),
      ...(Array.isArray(acoes) ? acoes : []),
    ]
    return items.sort((a, b) => {
      const getTime = (item: ItemAgendaGestor) => {
        if (item.tipo === 'agenda') {
          return obterDataExataAgenda(item.data, item.dataInicio)?.getTime() ?? 0
        }
        return item.data ? parseDataAgendaParaExibicao(item.data)?.getTime() ?? 0 : 0
      }
      return getTime(b) - getTime(a)
    })
  }, [agendas, interacoes, acoes])

  // Agrupar por dia: agenda usa dataAgendada (dia); interação/ação usam item.data (datetime → dia local).
  const itensPorData = useMemo(() => {
    return todosItens.reduce((acc, item) => {
      try {
        const dataParsed =
          item.tipo === 'agenda'
            ? obterDiaCalendarioAgenda(item.data)
            : item.data
              ? parseDataAgendaParaExibicao(item.data)
              : null
        if (!dataParsed) {
          if (item.data) console.warn('Data inválida para item:', item.id, item.data)
          return acc
        }
        const dataKey = format(dataParsed, 'yyyy-MM-dd')
        if (!acc[dataKey]) {
          acc[dataKey] = []
        }
        acc[dataKey].push(item)
      } catch (error) {
        console.warn('Erro ao processar data para item:', item.id, error)
      }
      return acc
    }, {} as Record<string, ItemAgendaGestor[]>)
  }, [todosItens])

  // Mostrar skeleton durante carregamento
  if (loading) {
    return <TimelineAgendasSkeleton />
  }

  // Mostrar mensagem de lista vazia apenas quando não está carregando
  // e realmente não há itens
  if (todosItens.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-12 text-center">
        <p className="text-lg font-semibold text-foreground mb-2">Nenhuma agenda encontrada</p>
        <p className="text-sm text-muted-foreground">
          Não há agendas, interações ou ações para o período selecionado.
        </p>
      </div>
    )
  }

  return (
    <div className="space-y-8">
      {Object.entries(itensPorData)
        .sort(([dataA], [dataB]) => {
          const parsedA = parseDataAgendaParaExibicao(dataA)?.getTime() ?? 0
          const parsedB = parseDataAgendaParaExibicao(dataB)?.getTime() ?? 0
          return parsedB - parsedA
        })
        .map(([data, itens]) => {
          const dataParsed = parseDataAgendaParaExibicao(data)
          if (!dataParsed) return null
          
          return (
            <div key={data}>
              {/* Cabeçalho da Data */}
              <div className="flex items-center gap-3 mb-4">
                <div className="flex-1 h-px bg-border" />
                <h3 className="text-lg font-semibold text-foreground">
                  {format(dataParsed, "EEEE, d 'de' MMMM", { locale: ptBR })}
                </h3>
                <div className="flex-1 h-px bg-border" />
              </div>

            {/* Cards do Dia - Layout em lista (não wrap) */}
            <div className="space-y-4">
              {itens.map((item) => {
                if (item.tipo === 'agenda') {
                  return (
                    <AgendaCard
                      key={item.id}
                      agenda={item}
                      onRecarregar={onRecarregar}
                      onAbrirNovaInteracao={onAbrirNovaInteracao}
                      onAbrirEditarInteracao={onAbrirEditarInteracao}
                    />
                  )
                }
                if (item.tipo === 'interacao') {
                  return (
                    <InteracaoCard
                      key={item.id}
                      interacao={item}
                      agendas={agendas}
                      onEditSuccess={onInteracaoEditOrDelete}
                      onDeleteSuccess={onInteracaoEditOrDelete}
                      onAbrirEditarInteracao={onAbrirEditarInteracao}
                    />
                  )
                }
                if (item.tipo === 'acao') {
                  return <AcaoCard key={item.id} acao={item} />
                }
                return null
              })}
            </div>
          </div>
          )
        })
        .filter(Boolean)}
    </div>
  )
}
