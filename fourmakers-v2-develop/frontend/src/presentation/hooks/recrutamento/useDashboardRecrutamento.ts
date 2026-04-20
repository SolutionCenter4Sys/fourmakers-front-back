import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import {
  getDefaultFiltrosCandidaturaParams,
  buildFiltrosParams,
  obterIntervaloEDias,
  formatarDataBR,
  vagasEmFocoItemToRow,
  type FiltrosDashboard,
  type VagaEmFocoRow,
} from '@shared/utils/dashboardRecrutamentoUtils'
import type {
  FiltrosCandidaturaParams,
  DashboardMetricasRecrutamentoRetorno,
  DashboardMetricasVagasEmFocoItem,
  DashboardMetricasFunilDeVagasRetorno,
  DashboardMetricasVagasPerdidasMotivoItem,
  DashboardNovosCandidatosPorOrigemItem,
} from '@domain/entities/DashboardMetricasRecrutamento'
import type { ClienteGestaoAlocados } from '@domain/entities/ClienteGestaoAlocados'
import type { RecrutadorGestaoAlocados } from '@domain/entities/RecrutadorGestaoAlocados'
import { ObterDashboardMetricasRecrutamentoUseCase } from '@domain/usecases/ObterDashboardMetricasRecrutamentoUseCase'
import { ObterDashboardMetricasVagasEmFocoUseCase } from '@domain/usecases/ObterDashboardMetricasVagasEmFocoUseCase'
import { ObterDashboardMetricasFunilDeVagasUseCase } from '@domain/usecases/ObterDashboardMetricasFunilDeVagasUseCase'
import { ObterDashboardMetricasVagasPerdidasMotivoUseCase } from '@domain/usecases/ObterDashboardMetricasVagasPerdidasMotivoUseCase'
import { ObterDashboardNovosCandidatosPorOrigemUseCase } from '@domain/usecases/ObterDashboardNovosCandidatosPorOrigemUseCase'
import { ListarClientesGestaoAlocadosUseCase } from '@domain/usecases/ListarClientesGestaoAlocadosUseCase'
import { ListarRecrutadoresGestaoAlocadosUseCase } from '@domain/usecases/ListarRecrutadoresGestaoAlocadosUseCase'
import { toast } from 'sonner'

const FILTROS_INICIAIS: FiltrosDashboard = {
  dataInicio: '',
  dataFim: '',
  clientes: [],
  recrutadores: [],
}

const FUNIL_ETAPAS_KEYS: { key: keyof DashboardMetricasFunilDeVagasRetorno; label: string }[] = [
  { key: 'emFoco', label: 'Em Foco' },
  { key: 'procurandoCandidatos', label: 'Procurando Candidatos' },
  { key: 'entrevistaInicial', label: 'Entrevista Inicial' },
  { key: 'aplicacaoTestes', label: 'Aplicação de Testes' },
  { key: 'entrevistaTecnica', label: 'Entrevista Técnica' },
  { key: 'entrevistaComCliente', label: 'Entrevista com Cliente' },
  { key: 'cartaOferta', label: 'Carta Oferta' },
]

export function useDashboardRecrutamento() {
  const { token } = useAppSelector((state) => state.auth)
  const defaultDates = useMemo(() => getDefaultFiltrosCandidaturaParams(), [])

  const [carregando, setCarregando] = useState(true)
  const [filtros, setFiltros] = useState<FiltrosDashboard>(FILTROS_INICIAIS)
  const [clientesDisponiveis, setClientesDisponiveis] = useState<ClienteGestaoAlocados[]>([])
  const [clientesSelecionados, setClientesSelecionados] = useState<ClienteGestaoAlocados[]>([])
  const [buscaCliente, setBuscaCliente] = useState('')
  const [, setCarregandoClientes] = useState(false)
  const [clientesAtivos, setClientesAtivos] = useState(false)
  const [mostrarListaClientes, setMostrarListaClientes] = useState(false)
  const clienteRef = useRef<HTMLDivElement | null>(null)
  const [recrutadoresDisponiveis, setRecrutadoresDisponiveis] = useState<RecrutadorGestaoAlocados[]>([])
  const [recrutadoresSelecionados, setRecrutadoresSelecionados] = useState<RecrutadorGestaoAlocados[]>([])
  const [buscaRecrutador, setBuscaRecrutador] = useState('')
  const [, setCarregandoRecrutadores] = useState(false)
  const [recrutadoresAtivos, setRecrutadoresAtivos] = useState(false)
  const [mostrarListaRecrutadores, setMostrarListaRecrutadores] = useState(false)
  const recrutadorRef = useRef<HTMLDivElement | null>(null)
  const [modalAberto, setModalAberto] = useState(false)
  const [cardAtivo, setCardAtivo] = useState<string | null>(null)
  const [vagasEmFocoList, setVagasEmFocoList] = useState<DashboardMetricasVagasEmFocoItem[]>([])
  const [carregandoVagasEmFoco, setCarregandoVagasEmFoco] = useState(false)
  const [funilData, setFunilData] = useState<DashboardMetricasFunilDeVagasRetorno | null>(null)
  const [carregandoFunil, setCarregandoFunil] = useState(false)
  const [vagasPerdidasMotivosList, setVagasPerdidasMotivosList] = useState<
    DashboardMetricasVagasPerdidasMotivoItem[]
  >([])
  const [carregandoVagasPerdidas, setCarregandoVagasPerdidas] = useState(false)
  const [fontesCandidatos, setFontesCandidatos] = useState<DashboardNovosCandidatosPorOrigemItem[]>([])
  const [dashboardMetricasRetorno, setDashboardMetricasRetorno] =
    useState<DashboardMetricasRecrutamentoRetorno | null>(null)
  const [sortKey, setSortKey] = useState<keyof VagaEmFocoRow>('ultimaMovimentacaoDate')
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc')
  const [paginaAtual, setPaginaAtual] = useState(1)

  const filtrosAtivos = Boolean(
    filtros.dataInicio ||
      filtros.dataFim ||
      filtros.clientes.length > 0 ||
      filtros.recrutadores.length > 0,
  )

  const carregarClientes = useCallback(
    async (busca: string) => {
      if (!token) return
      setCarregandoClientes(true)
      try {
        const useCase = container.resolve(ListarClientesGestaoAlocadosUseCase)
        const response = await useCase.execute(token, busca)
        const retorno = Array.isArray(response?.retorno) ? response.retorno : []
        setClientesDisponiveis(retorno)
      } catch (error) {
        console.error('Erro ao carregar clientes:', error)
        toast.error('Não foi possível carregar a lista de clientes.')
      } finally {
        setCarregandoClientes(false)
      }
    },
    [token],
  )

  const carregarRecrutadores = useCallback(
    async (busca: string) => {
      if (!token) return
      setCarregandoRecrutadores(true)
      try {
        const useCase = container.resolve(ListarRecrutadoresGestaoAlocadosUseCase)
        const response = await useCase.execute(token, busca)
        const retorno = Array.isArray(response?.retorno) ? response.retorno : []
        setRecrutadoresDisponiveis(retorno)
      } catch (error) {
        console.error('Erro ao carregar recrutadores:', error)
        toast.error('Não foi possível carregar a lista de recrutadores.')
      } finally {
        setCarregandoRecrutadores(false)
      }
    },
    [token],
  )

  const carregarDashboard = useCallback(
    async (params: FiltrosCandidaturaParams) => {
      if (!token) {
        setCarregando(false)
        return
      }
      setCarregando(true)
      try {
        const useCase = container.resolve(ObterDashboardMetricasRecrutamentoUseCase)
        const response = await useCase.execute(token, params)
        setDashboardMetricasRetorno(response?.retorno ?? null)
        const useCaseFontes = container.resolve(ObterDashboardNovosCandidatosPorOrigemUseCase)
        const responseFontes = await useCaseFontes.execute(token, params)
        const retornoFontes = Array.isArray(responseFontes?.retorno) ? responseFontes.retorno : []
        setFontesCandidatos(retornoFontes)
      } catch (error) {
        const mensagem =
          error instanceof Error ? error.message : 'Não foi possível carregar as métricas do dashboard.'
        toast.error(mensagem)
      } finally {
        setCarregando(false)
      }
    },
    [token],
  )

  useEffect(() => {
    if (!token) {
      setCarregando(false)
      return
    }
    const params = buildFiltrosParams(filtros, clientesSelecionados, recrutadoresSelecionados, defaultDates)
    carregarDashboard(params)
  }, [token])

  useEffect(() => {
    if (!token || !clientesAtivos) return
    if (buscaCliente.trim().length < 1) {
      setClientesDisponiveis([])
      return
    }
    carregarClientes(buscaCliente.trim())
  }, [buscaCliente, carregarClientes, clientesAtivos, token])

  useEffect(() => {
    if (!token || !recrutadoresAtivos) return
    if (buscaRecrutador.trim().length < 1) {
      setRecrutadoresDisponiveis([])
      return
    }
    carregarRecrutadores(buscaRecrutador.trim())
  }, [buscaRecrutador, carregarRecrutadores, recrutadoresAtivos, token])

  useEffect(() => {
    setFiltros((prev) => ({
      ...prev,
      clientes: clientesSelecionados.map((c) => c.codigoCliente),
    }))
  }, [clientesSelecionados])

  useEffect(() => {
    setFiltros((prev) => ({
      ...prev,
      recrutadores: recrutadoresSelecionados.map((r) => r.codigoInternoColaborador),
    }))
  }, [recrutadoresSelecionados])

  useEffect(() => {
    if (!token || !modalAberto || !cardAtivo) return
    const params = buildFiltrosParams(filtros, clientesSelecionados, recrutadoresSelecionados, defaultDates)
    if (cardAtivo === 'em-foco') {
      setCarregandoVagasEmFoco(true)
      container
        .resolve(ObterDashboardMetricasVagasEmFocoUseCase)
        .execute(token, params)
        .then((res) => {
          setVagasEmFocoList(Array.isArray(res?.retorno) ? res.retorno : [])
        })
        .catch((err) => {
          toast.error(err instanceof Error ? err.message : 'Não foi possível carregar vagas em foco.')
        })
        .finally(() => setCarregandoVagasEmFoco(false))
    }
    if (cardAtivo === 'em-andamento') {
      setCarregandoFunil(true)
      container
        .resolve(ObterDashboardMetricasFunilDeVagasUseCase)
        .execute(token, params)
        .then((res) => {
          setFunilData(res?.retorno ?? null)
        })
        .catch((err) => {
          toast.error(err instanceof Error ? err.message : 'Não foi possível carregar o funil.')
        })
        .finally(() => setCarregandoFunil(false))
    }
    if (cardAtivo === 'vagas-perdidas') {
      setCarregandoVagasPerdidas(true)
      container
        .resolve(ObterDashboardMetricasVagasPerdidasMotivoUseCase)
        .execute(token, params)
        .then((res) => {
          setVagasPerdidasMotivosList(Array.isArray(res?.retorno) ? res.retorno : [])
        })
        .catch((err) => {
          toast.error(err instanceof Error ? err.message : 'Não foi possível carregar vagas perdidas.')
        })
        .finally(() => setCarregandoVagasPerdidas(false))
    }
  }, [token, modalAberto, cardAtivo, filtros, clientesSelecionados, recrutadoresSelecionados, defaultDates])

  useEffect(() => {
    if (cardAtivo === 'em-foco') {
      setPaginaAtual(1)
      setSortKey('ultimaMovimentacaoDate')
      setSortDirection('desc')
    }
  }, [cardAtivo])

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      const target = event.target as Node
      if (clienteRef.current && !clienteRef.current.contains(target)) setMostrarListaClientes(false)
      if (recrutadorRef.current && !recrutadorRef.current.contains(target)) setMostrarListaRecrutadores(false)
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  const handleFiltrar = useCallback(() => {
    const { dataInicio, dataFim } = filtros
    if (dataFim && !dataInicio) {
      toast.error('É necessária a data inicial para a busca.')
      return
    }
    if (dataInicio && !dataFim) {
      toast.error('É necessária a data final para a busca.')
      return
    }
    if (dataInicio && dataFim) {
      const inicio = new Date(dataInicio + 'T00:00:00')
      const fim = new Date(dataFim + 'T00:00:00')
      if (inicio.getTime() > fim.getTime()) {
        toast.error('As datas não são válidas. A data inicial não pode ser maior que a data final.')
        setFiltros((prev) => ({ ...prev, dataInicio: '', dataFim: '' }))
        return
      }
    }
    const params = buildFiltrosParams(filtros, clientesSelecionados, recrutadoresSelecionados, defaultDates)
    carregarDashboard(params)
  }, [filtros, clientesSelecionados, recrutadoresSelecionados, defaultDates, carregarDashboard])

  const handleLimparFiltros = useCallback(() => {
    setFiltros(FILTROS_INICIAIS)
    setClientesSelecionados([])
    setBuscaCliente('')
    setClientesAtivos(false)
    setRecrutadoresSelecionados([])
    setBuscaRecrutador('')
    setRecrutadoresAtivos(false)
    carregarDashboard(getDefaultFiltrosCandidaturaParams())
  }, [carregarDashboard])

  const handleAdicionarCliente = useCallback((cliente: ClienteGestaoAlocados) => {
    setClientesSelecionados((prev) => {
      if (prev.some((item) => item.codigoCliente === cliente.codigoCliente)) return prev
      return [...prev, cliente]
    })
    setBuscaCliente('')
    setClientesDisponiveis([])
    setMostrarListaClientes(false)
  }, [])

  const handleRemoverCliente = useCallback((codigoCliente: string) => {
    setClientesSelecionados((prev) => prev.filter((item) => item.codigoCliente !== codigoCliente))
    setBuscaCliente('')
    setClientesDisponiveis([])
    setMostrarListaClientes(false)
  }, [])

  const handleAdicionarRecrutador = useCallback((recrutador: RecrutadorGestaoAlocados) => {
    setRecrutadoresSelecionados((prev) => {
      if (prev.some((item) => item.codigoInternoColaborador === recrutador.codigoInternoColaborador)) return prev
      return [...prev, recrutador]
    })
    setBuscaRecrutador('')
    setRecrutadoresDisponiveis([])
    setMostrarListaRecrutadores(false)
  }, [])

  const handleRemoverRecrutador = useCallback((codigoInternoColaborador: string) => {
    setRecrutadoresSelecionados((prev) =>
      prev.filter((item) => item.codigoInternoColaborador !== codigoInternoColaborador),
    )
    setBuscaRecrutador('')
    setRecrutadoresDisponiveis([])
    setMostrarListaRecrutadores(false)
  }, [])

  const handleAbrirModal = useCallback((cardId: string) => {
    setCardAtivo(cardId)
    setModalAberto(true)
  }, [])

  const handleOrdenar = useCallback((key: keyof VagaEmFocoRow) => {
    if (sortKey === key) {
      setSortDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'))
      return
    }
    setSortKey(key)
    setSortDirection('asc')
  }, [sortKey])

  const vagasEmFocoRowsFromApi = useMemo(
    () => vagasEmFocoList.map(vagasEmFocoItemToRow),
    [vagasEmFocoList],
  )

  const vagasEmFocoOrdenadas = useMemo(() => {
    const ordenadas = [...vagasEmFocoRowsFromApi]
    ordenadas.sort((a, b) => {
      const multiplier = sortDirection === 'asc' ? 1 : -1
      if (sortKey === 'ultimaMovimentacaoDate') {
        return (a.ultimaMovimentacaoDate.getTime() - b.ultimaMovimentacaoDate.getTime()) * multiplier
      }
      const valueA = String(a[sortKey]).toLowerCase()
      const valueB = String(b[sortKey]).toLowerCase()
      return valueA.localeCompare(valueB, 'pt-BR') * multiplier
    })
    return ordenadas
  }, [vagasEmFocoRowsFromApi, sortDirection, sortKey])

  const itensPorPagina = 5
  const totalPaginas = Math.max(1, Math.ceil(vagasEmFocoOrdenadas.length / itensPorPagina))
  const paginaSegura = Math.min(paginaAtual, totalPaginas)
  const inicioSlice = (paginaSegura - 1) * itensPorPagina
  const vagasEmFocoPaginadas = vagasEmFocoOrdenadas.slice(inicioSlice, inicioSlice + itensPorPagina)

  const funilEtapasParaModal = useMemo(() => {
    if (!funilData) return []
    return FUNIL_ETAPAS_KEYS.map(({ key, label }) => ({
      etapa: label,
      valor: funilData[key] ?? 0,
    }))
  }, [funilData])

  const totalFunil = funilEtapasParaModal.reduce((acc, item) => acc + item.valor, 0)
  const maxFunil = Math.max(1, ...funilEtapasParaModal.map((item) => item.valor))
  const maxPerdidas = Math.max(1, ...vagasPerdidasMotivosList.map((item) => item.total))

  const totalNovosCandidatosPeriodo = useMemo(
    () => fontesCandidatos.reduce((acc, item) => acc + item.total, 0),
    [fontesCandidatos],
  )

  const totalNovosCandidatosLinkedin = useMemo(() => {
    const item = fontesCandidatos.find((f) => f.origem.toLowerCase().trim() === 'linkedin')
    return item?.total ?? 0
  }, [fontesCandidatos])

  const obterValorCard = useMemo(() => {
    const r = dashboardMetricasRetorno
    if (!r) return () => null
    return (cardId: string): string | number | null => {
      switch (cardId) {
        case 'em-foco':
          return r.emFoco
        case 'em-andamento':
          return r.emAndamento
        case 'entrevista-cliente':
          return r.entrevistaCliente
        case 'tm-inicio':
          return `${r.tempoMedioDiasRecrutamento} dias`
        case 'vagas-perdidas':
          return r.vagasPerdidas
        case 'vagas-canceladas':
          return r.vagasCanceladas
        default:
          return null
      }
    }
  }, [dashboardMetricasRetorno])

  const tituloModal = useMemo(() => {
    switch (cardAtivo) {
      case 'em-foco':
        return 'Vagas em Foco + 24 horas'
      case 'em-andamento':
        return 'Funil de Vagas em Andamento'
      case 'vagas-perdidas':
        return 'Vagas Perdidas por Motivo'
      default:
        return ''
    }
  }, [cardAtivo])

  const descricaoModal = useMemo(() => {
    const { dataInicio, dataFim, dias } = obterIntervaloEDias(
      filtros.dataInicio,
      filtros.dataFim,
      defaultDates.DataInicio ?? '',
      defaultDates.DataFim ?? '',
    )
    switch (cardAtivo) {
      case 'em-foco':
        return 'Lista de vagas filtradas'
      case 'em-andamento':
        return `${totalFunil} vagas no funil`
      case 'vagas-perdidas': {
        const textoIntervalo =
          dataInicio && dataFim
            ? `${formatarDataBR(dataInicio)} a ${formatarDataBR(dataFim)}`
            : ''
        return textoIntervalo
          ? `Vagas perdidas no período de ${textoIntervalo} (${dias} ${dias === 1 ? 'dia' : 'dias'})`
          : `Vagas perdidas no período de ${dias} ${dias === 1 ? 'dia' : 'dias'}`
      }
      default:
        return ''
    }
  }, [cardAtivo, totalFunil, filtros.dataInicio, filtros.dataFim, defaultDates.DataInicio, defaultDates.DataFim])

  return {
    carregando,
    filtros,
    setFiltros,
    clientesDisponiveis,
    clientesSelecionados,
    buscaCliente,
    setBuscaCliente,
    setClientesAtivos,
    clientesAtivos,
    mostrarListaClientes,
    setMostrarListaClientes,
    clienteRef,
    recrutadoresDisponiveis,
    recrutadoresSelecionados,
    buscaRecrutador,
    setBuscaRecrutador,
    setRecrutadoresAtivos,
    recrutadoresAtivos,
    mostrarListaRecrutadores,
    setMostrarListaRecrutadores,
    recrutadorRef,
    modalAberto,
    setModalAberto,
    cardAtivo,
    vagasEmFocoList,
    carregandoVagasEmFoco,
    funilData,
    carregandoFunil,
    vagasPerdidasMotivosList,
    carregandoVagasPerdidas,
    fontesCandidatos,
    sortKey,
    sortDirection,
    paginaAtual,
    setPaginaAtual,
    filtrosAtivos,
    handleFiltrar,
    handleLimparFiltros,
    handleAdicionarCliente,
    handleRemoverCliente,
    handleAdicionarRecrutador,
    handleRemoverRecrutador,
    handleAbrirModal,
    handleOrdenar,
    vagasEmFocoPaginadas,
    funilEtapasParaModal,
    totalFunil,
    maxFunil,
    maxPerdidas,
    totalNovosCandidatosPeriodo,
    totalNovosCandidatosLinkedin,
    obterValorCard,
    tituloModal,
    descricaoModal,
    totalPaginas,
    paginaSegura,
  }
}
