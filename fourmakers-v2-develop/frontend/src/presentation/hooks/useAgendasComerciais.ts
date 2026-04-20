import { useState, useEffect, useMemo, useCallback, useRef } from 'react'
import type { RefObject } from 'react'

import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { buscarAgendasComerciais } from '@app/store/slices/agendasComerciaisSlice'
import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { obterDatasPadraoAgenda } from '@shared/constants/agendasComerciais'
import {
  formatarDataLocalParaFiltro,
  parseDataAgendaParaExibicao,
} from '@shared/utils/timezoneAgendaUtils'
import { obterStatusSolicitacaoParticipanteComFallback } from '@shared/utils/agendaPermissions'

/**
 * Interface para filtros de agendas comerciais
 */
export interface FiltrosAgenda {
  dataInicio: Date | null
  dataFim: Date | null
  colaboradores: string[]
  clientes: string[]
  gestores: string[]
  tipoInteracao: string[]
  minhasAgendas: boolean
  apenasConvitesPendentes: boolean
}

/** Retorna os filtros iniciais com o mesmo range padrão usado na API (RANGE_DIAS_AGENDA). */
export function obterFiltrosIniciais(): FiltrosAgenda {
  const { dataInicio, dataFim } = obterDatasPadraoAgenda()
  return {
    dataInicio,
    dataFim,
    colaboradores: [],
    clientes: [],
    gestores: [],
    tipoInteracao: [],
    minhasAgendas: false,
    apenasConvitesPendentes: false,
  }
}

/**
 * Hook customizado para gerenciar estado e lógica da página de Agendas Comerciais.
 * 
 * @returns {Object} Objeto com dados filtrados, handlers e estados
 * @returns {ItemAgendaGestor[]} agendas - Agendas filtradas
 * @returns {ItemAgendaGestor[]} interacoes - Interações filtradas
 * @returns {ItemAgendaGestor[]} acoes - Ações filtradas
 * @returns {boolean} loading - Estado de carregamento
 * @returns {string | null} error - Mensagem de erro, se houver
 * @returns {FiltrosAgenda} filtros - Filtros atuais
 * @returns {Function} handleFiltroChange - Handler para mudança de filtro
 * @returns {Function} handleNovaAgenda - Handler para criar nova agenda
 * @returns {Function} recarregar - Função para recarregar dados
 * @param scrollContainerRef - Ref opcional do elemento de scroll (ex.: ContentArea). Quando fornecido, a posição de scroll é preservada ao recarregar a lista.
 */
export function useAgendasComerciais(scrollContainerRef?: RefObject<HTMLElement | null> | null) {
  const dispatch = useAppDispatch()
  const { historico, status, error } = useAppSelector((state) => state.agendasComerciais)
  const { user, token } = useAppSelector((state) => state.auth)
  const savedScrollTopRef = useRef<number>(0)

  const [filtros, setFiltros] = useState<FiltrosAgenda>(obterFiltrosIniciais)

  // Memoizar valores do user para estabilizar dependências
  const userCpf = useMemo(() => user?.cpf, [user?.cpf])
  const userColaboradorCpf = useMemo(() => user?.colaborador?.cpf, [user?.colaborador?.cpf])
  const userNomeColaborador = useMemo(() => user?.nomeColaborador, [user?.nomeColaborador])

  // Usar useMemo para estabilizar codInternoColaborador
  const codInternoColaborador = useMemo(
    () => userCpf || userColaboradorCpf || '',
    [userCpf, userColaboradorCpf]
  )

  // Usar useMemo para estabilizar as datas do período (dia de calendário local para filtro)
  const dataInicioFormatada = useMemo(
    () => filtros.dataInicio ? formatarDataLocalParaFiltro(filtros.dataInicio) : undefined,
    [filtros.dataInicio]
  )

  const dataFimFormatada = useMemo(
    () => filtros.dataFim ? formatarDataLocalParaFiltro(filtros.dataFim) : undefined,
    [filtros.dataFim]
  )

  // Buscar dados ao montar ou quando filtros de data mudarem.
  // Dependências estáveis: dataInicioFormatada e dataFimFormatada vêm de useMemo (filtros.dataInicio/dataFim).
  useEffect(() => {
    if (token && codInternoColaborador) {
      dispatch(
        buscarAgendasComerciais({
          token,
          codInternoColaborador,
          dataInicio: dataInicioFormatada,
          dataFim: dataFimFormatada,
        }),
      )
    }

    // Não limpar agendas no cleanup para evitar flicker
    // O estado será atualizado quando a nova requisição completar
    // return () => {
    //   dispatch(limparAgendas())
    // }
  }, [dispatch, token, codInternoColaborador, dataInicioFormatada, dataFimFormatada])

  // Função auxiliar para comparar datas apenas pela parte da data (sem hora/timezone)
  const compararDataApenasData = useCallback((dataItem: string | undefined, dataFiltro: Date | null): number => {
    if (!dataItem || !dataFiltro) return 0

    const dataParsed = parseDataAgendaParaExibicao(dataItem)
    if (!dataParsed) return 0
    
    // Comparar apenas ano, mês e dia (ignorar hora e timezone)
    const anoItem = dataParsed.getFullYear()
    const mesItem = dataParsed.getMonth()
    const diaItem = dataParsed.getDate()
    
    const anoFiltro = dataFiltro.getFullYear()
    const mesFiltro = dataFiltro.getMonth()
    const diaFiltro = dataFiltro.getDate()
    
    if (anoItem !== anoFiltro) return anoItem - anoFiltro
    if (mesItem !== mesFiltro) return mesItem - mesFiltro
    return diaItem - diaFiltro
  }, [])

  // Funções auxiliares de filtro
  const filtrarAgendas = useCallback(
    (agendas: ItemAgendaGestor[], filtrosAtuais: FiltrosAgenda): ItemAgendaGestor[] => {
      return agendas.filter((agenda) => {
        // Filtro por data (comparar apenas a parte da data, sem hora/timezone)
        // Só aplicar filtro se ambas as datas estiverem definidas
        if (filtrosAtuais.dataInicio && filtrosAtuais.dataFim && agenda.data) {
          const comparacaoInicio = compararDataApenasData(agenda.data, filtrosAtuais.dataInicio)
          const comparacaoFim = compararDataApenasData(agenda.data, filtrosAtuais.dataFim)
          
          // Data do item deve estar entre dataInicio e dataFim (inclusive)
          if (comparacaoInicio < 0 || comparacaoFim > 0) {
            return false
          }
        }

        // Filtro por "Minhas Agendas"
        if (filtrosAtuais.minhasAgendas && userCpf) {
          const nomeUsuario = userNomeColaborador?.toLowerCase() || ''
          const souResponsavel = (agenda.responsavel?.toLowerCase() || '').includes(nomeUsuario)
          if (!souResponsavel) return false
        }

        // Filtro por convites pendentes (status === 0, com fallback em participantes)
        if (filtrosAtuais.apenasConvitesPendentes) {
          const status = obterStatusSolicitacaoParticipanteComFallback(agenda, user)
          if (status !== 0) {
            return false
          }
        }

        // Filtro por tipo de interação
        if (
          filtrosAtuais.tipoInteracao.length > 0 &&
          agenda.tipoInteracao &&
          !filtrosAtuais.tipoInteracao.includes(agenda.tipoInteracao)
        ) {
          return false
        }

        // Filtro por colaboradores
        if (filtrosAtuais.colaboradores.length > 0 && agenda.colaboradores) {
          const temColaborador = agenda.colaboradores.some(
            (colab) =>
              colab != null &&
              colab.codInternoColaborador != null &&
              filtrosAtuais.colaboradores.includes(colab.codInternoColaborador),
          )
          if (!temColaborador) return false
        }

        // Filtro por clientes
        if (filtrosAtuais.clientes.length > 0 && agenda.cliente) {
          if (!filtrosAtuais.clientes.includes(agenda.cliente.codigoCliente)) {
            return false
          }
        }

        // Filtro por gestores externos
        if (filtrosAtuais.gestores.length > 0 && agenda.gestoresExternos) {
          const temGestor = agenda.gestoresExternos.some(
            (gestor) =>
              gestor != null &&
              gestor.codGestorExterno != null &&
              filtrosAtuais.gestores.includes(gestor.codGestorExterno),
          )
          if (!temGestor) return false
        }

        return true
      })
    },
    [userCpf, userNomeColaborador, compararDataApenasData],
  )

  const filtrarInteracoes = useCallback(
    (interacoes: ItemAgendaGestor[], filtrosAtuais: FiltrosAgenda): ItemAgendaGestor[] => {
      return interacoes.filter((interacao) => {
        // Filtro por data (comparar apenas a parte da data, sem hora/timezone)
        // Só aplicar filtro se ambas as datas estiverem definidas
        if (filtrosAtuais.dataInicio && filtrosAtuais.dataFim && interacao.data) {
          const comparacaoInicio = compararDataApenasData(interacao.data, filtrosAtuais.dataInicio)
          const comparacaoFim = compararDataApenasData(interacao.data, filtrosAtuais.dataFim)
          
          // Data do item deve estar entre dataInicio e dataFim (inclusive)
          if (comparacaoInicio < 0 || comparacaoFim > 0) {
            return false
          }
        }

        // Filtro por "Minhas Agendas"
        if (filtrosAtuais.minhasAgendas && userCpf) {
          const nomeUsuario = userNomeColaborador?.toLowerCase() || ''
          const souResponsavel = (interacao.responsavel?.toLowerCase() || '').includes(nomeUsuario)
          if (!souResponsavel) return false
        }

        // Filtro por colaboradores
        if (filtrosAtuais.colaboradores.length > 0 && interacao.colaboradores) {
          const temColaborador = interacao.colaboradores.some(
            (colab) =>
              colab != null &&
              colab.codInternoColaborador != null &&
              filtrosAtuais.colaboradores.includes(colab.codInternoColaborador),
          )
          if (!temColaborador) return false
        }

        // Filtro por clientes
        if (filtrosAtuais.clientes.length > 0 && interacao.cliente) {
          if (!filtrosAtuais.clientes.includes(interacao.cliente.codigoCliente)) {
            return false
          }
        }

        // Filtro por gestores externos
        if (filtrosAtuais.gestores.length > 0 && interacao.gestoresExternos) {
          const temGestor = interacao.gestoresExternos.some(
            (gestor) =>
              gestor != null &&
              gestor.codGestorExterno != null &&
              filtrosAtuais.gestores.includes(gestor.codGestorExterno),
          )
          if (!temGestor) return false
        }

        return true
      })
    },
    [user, userCpf, userNomeColaborador, compararDataApenasData],
  )

  const filtrarAcoes = useCallback(
    (acoes: ItemAgendaGestor[], filtrosAtuais: FiltrosAgenda): ItemAgendaGestor[] => {
      return acoes.filter((acao) => {
        // Filtro por data (comparar apenas a parte da data, sem hora/timezone)
        // Só aplicar filtro se ambas as datas estiverem definidas
        if (filtrosAtuais.dataInicio && filtrosAtuais.dataFim && acao.data) {
          const comparacaoInicio = compararDataApenasData(acao.data, filtrosAtuais.dataInicio)
          const comparacaoFim = compararDataApenasData(acao.data, filtrosAtuais.dataFim)
          
          // Data do item deve estar entre dataInicio e dataFim (inclusive)
          if (comparacaoInicio < 0 || comparacaoFim > 0) {
            return false
          }
        }

        // Filtro por "Minhas Agendas"
        if (filtrosAtuais.minhasAgendas && userCpf) {
          const nomeUsuario = userNomeColaborador?.toLowerCase() || ''
          const souResponsavel = (acao.responsavel?.toLowerCase() || '').includes(nomeUsuario)
          if (!souResponsavel) return false
        }

        // Filtro por colaboradores (ações podem estar associadas a colaboradores via agenda)
        if (filtrosAtuais.colaboradores.length > 0 && acao.colaboradores) {
          const temColaborador = acao.colaboradores.some(
            (colab) =>
              colab != null &&
              colab.codInternoColaborador != null &&
              filtrosAtuais.colaboradores.includes(colab.codInternoColaborador),
          )
          if (!temColaborador) return false
        }

        // Filtro por clientes
        if (filtrosAtuais.clientes.length > 0 && acao.cliente) {
          if (!filtrosAtuais.clientes.includes(acao.cliente.codigoCliente)) {
            return false
          }
        }

        // Filtro por gestores externos
        if (filtrosAtuais.gestores.length > 0 && acao.gestoresExternos) {
          const temGestor = acao.gestoresExternos.some(
            (gestor) =>
              gestor != null &&
              gestor.codGestorExterno != null &&
              filtrosAtuais.gestores.includes(gestor.codGestorExterno),
          )
          if (!temGestor) return false
        }

        return true
      })
    },
    [userCpf, userNomeColaborador, compararDataApenasData],
  )

  // Aplicar filtros usando useMemo para performance
  const dadosFiltrados = useMemo(() => {
    if (!historico) {
      return { agendas: [], interacoes: [], acoes: [] }
    }

    // Garantir que sejam arrays antes de filtrar
    const agendasArray = Array.isArray(historico.agendas) ? historico.agendas : []
    const interacoesArray = Array.isArray(historico.interacoes) ? historico.interacoes : []
    const acoesArray = Array.isArray(historico.acoes) ? historico.acoes : []

    return {
      agendas: filtrarAgendas(agendasArray, filtros),
      interacoes: filtrarInteracoes(interacoesArray, filtros),
      acoes: filtrarAcoes(acoesArray, filtros),
    }
  }, [historico, filtros, filtrarAgendas, filtrarInteracoes, filtrarAcoes])

  const handleFiltroChange = useCallback(
    (novosFiltros: Partial<FiltrosAgenda>) => {
      setFiltros((prev) => ({ ...prev, ...novosFiltros }))

      // Analytics
      if (user) {
        logUserAction(
          'AgendasComerciais',
          'AplicarFiltros',
          {
            filtrosAplicados: Object.keys(novosFiltros),
          },
          user,
        )
      }
    },
    [user],
  )

  const handleNovaAgenda = useCallback(() => {
    if (user) {
      logUserAction('AgendasComerciais', 'NovaAgenda', {}, user)
    }
    // O modal é gerenciado pela página
  }, [user])

  const recarregar = useCallback(() => {
    if (token && codInternoColaborador) {
      // Salvar scroll antes do dispatch para não perder referência (ex.: ao criar nova interação no card)
      if (scrollContainerRef?.current) {
        savedScrollTopRef.current = scrollContainerRef.current.scrollTop
      }
      dispatch(
        buscarAgendasComerciais({
          token,
          codInternoColaborador,
          dataInicio: dataInicioFormatada,
          dataFim: dataFimFormatada,
        }),
      )
    }
  }, [token, codInternoColaborador, dataInicioFormatada, dataFimFormatada, dispatch, scrollContainerRef])

  // Determinar se está carregando (skeleton full-page)
  // Só exibe skeleton na primeira carga (historico === null). Ao recarregar (ex.: após deletar/editar interação),
  // mantém a lista visível para não desmontar modais abertos (detalhe da agenda, editar interação).
  const isLoading =
    (status === 'loading' && historico === null) || (status === 'idle' && historico === null)

  /** True durante recarregar (já tem dados); quando vira false, a timeline já recebeu agendas/interacoes/acoes atualizados via historico no slice. */
  const isRefetching = status === 'loading' && historico !== null

  // Restaurar posição de scroll quando a lista terminar de carregar (scroll foi salvo em recarregar antes do dispatch)
  useEffect(() => {
    if ((status === 'succeeded' || status === 'failed') && scrollContainerRef?.current && savedScrollTopRef.current > 0) {
      const scrollTop = savedScrollTopRef.current
      savedScrollTopRef.current = 0
      requestAnimationFrame(() => {
        if (scrollContainerRef?.current) {
          scrollContainerRef.current.scrollTop = scrollTop
        }
      })
    }
  }, [status, scrollContainerRef])

  return {
    /** Dados atualizados quando buscarAgendasComerciais.fulfilled roda (historico é substituído no slice). */
    agendas: dadosFiltrados.agendas,
    interacoes: dadosFiltrados.interacoes,
    acoes: dadosFiltrados.acoes,
    loading: isLoading,
    /** True enquanto recarregar está em andamento; a timeline continua visível e recebe dados novos ao terminar. */
    isRefetching,
    error,
    filtros,
    handleFiltroChange,
    handleNovaAgenda,
    recarregar,
  }
}
