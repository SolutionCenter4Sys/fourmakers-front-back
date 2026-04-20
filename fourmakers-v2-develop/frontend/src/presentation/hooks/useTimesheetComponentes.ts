import { useState, useEffect, useCallback, useRef } from 'react'
import { container } from '@core/di/container'
import { TimesheetComponentesApi } from '@data/api/TimesheetComponentesApi'
import type {
  TimesheetStatMock,
  ProjetoApprovalMock,
  ColaboradorManagementMock,
  WeekDataMock,
  DayDetailMock,
} from '@data/mocks/timesheetComponentesMock'
import type { ColaboradorEApontamento } from '@data/api/TimesheetComponentesApi'
import { Users, Triangle, Timer, CheckCircle2, Clock, XCircle } from '@/components/ui/system-icons'

export const useTimesheetTab = () => {
  const [stats, setStats] = useState<TimesheetStatMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(TimesheetComponentesApi)
        const data = await api.getTimesheetStats()
        setStats(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { stats, loading, error }
}

export interface UseApprovalsTabParams {
  token: string
  cpfColaborador?: string
  codStatusGrupo?: string
  cpfGerenteAdm?: string
  mes: number
  ano: number
  codProjeto?: string
  statusList?: { codStatusGrupo: number; descricao: string }[]
}

// Função auxiliar para mapear dados da API para o formato da tabela de aprovações
const mapProjetoToApprovalTable = (
  projeto: {
    mes: number
    ano: number
    codProjeto: string
    nomeProjeto: string
    dataFimProjeto: string | null
    cpfColaborador: string
    nomeColaborador: string
    ativo: boolean
    dataInativacao: string | null
    dataAdmissao: string
    nomeGestorAdm: string
    orgId: number
    codStatusMensal: number
    descricaoStatusMensal: string
    totalHoras: number
    codCliente: string
    nomeCliente: string
  },
  statusList?: { codStatusGrupo: number; descricao: string }[]
): ProjetoApprovalMock => {
  // Formatar horas (minutos para HH:MM)
  const horasFormatadas = (() => {
    const horas = Math.floor(projeto.totalHoras / 60)
    const mins = projeto.totalHoras % 60
    return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`
  })()

  // Mapear status
  const codigoStatus = projeto.codStatusMensal
  let statusDescricao = projeto.descricaoStatusMensal || 'Sem status'
  
  if (statusList && statusList.length > 0) {
    const statusEncontrado = statusList.find(s => Number(s.codStatusGrupo) === codigoStatus)
    if (statusEncontrado) {
      statusDescricao = statusEncontrado.descricao
    }
  }

  // Criar ID único combinando cpfColaborador, codProjeto, mes e ano para evitar duplicatas
  const uniqueId = `${projeto.cpfColaborador}-${projeto.codProjeto}-${projeto.mes}-${projeto.ano}`;
  
  return {
    id: uniqueId,
    colaborador: projeto.nomeColaborador,
    cpfColaborador: projeto.cpfColaborador, // Adicionar CPF para navegação
    mes: projeto.mes, // Adicionar mês para navegação
    ano: projeto.ano, // Adicionar ano para navegação
    situacao: projeto.ativo ? 'Ativo' : 'Inativo',
    dataSituacao: projeto.dataAdmissao,
    cliente: `${projeto.codCliente} - ${projeto.nomeCliente}`,
    projeto: `${projeto.codProjeto} - ${projeto.nomeProjeto}`,
    fimProjeto: projeto.dataFimProjeto || '---',
    gestorAdm: projeto.nomeGestorAdm,
    status: statusDescricao,
    horas: horasFormatadas,
    codProjeto: projeto.codProjeto,
    codStatusMensal: projeto.codStatusMensal,
    ativo: projeto.ativo,
  }
}

export const useApprovalsTab = (params?: UseApprovalsTabParams) => {
  const [stats, setStats] = useState<TimesheetStatMock[]>([])
  const [projetos, setProjetos] = useState<ProjetoApprovalMock[]>([])
  const [projetosRaw, setProjetosRaw] = useState<Array<{
    mes: number
    ano: number
    codProjeto: string
    nomeProjeto: string
    dataFimProjeto: string | null
    cpfColaborador: string
    nomeColaborador: string
    ativo: boolean
    dataInativacao: string | null
    dataAdmissao: string
    nomeGestorAdm: string
    orgId: number
    codStatusMensal: number
    descricaoStatusMensal: string
    totalHoras: number
    codCliente: string
    nomeCliente: string
  }>>([])
  const [dataColetaDeDados, setDataColetaDeDados] = useState<string>("")
  const [totalizador, setTotalizador] = useState<{
    quantidadeTotalColaboradores: number
    quantidadeTotalProjetos: number
    somaHoras: number
  } | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [totalItems, setTotalItems] = useState(0)
  const [hasMore, setHasMore] = useState(false)
  const paramsRef = useRef<string>('')

  // Criar uma chave serializada dos parâmetros para comparação
  const paramsKey = params 
    ? JSON.stringify({
        token: params.token,
        cpfColaborador: params.cpfColaborador || '',
        codStatusGrupo: params.codStatusGrupo || '',
        cpfGerenteAdm: params.cpfGerenteAdm || '',
        mes: params.mes,
        ano: params.ano,
        codProjeto: params.codProjeto || '',
        statusListLength: params.statusList?.length || 0,
      })
    : ''

  const loadData = useCallback(async () => {
    if (!params?.token) {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(TimesheetComponentesApi)
        const [statsData, projetosData] = await Promise.all([
          api.getApprovalsStats(),
          api.getProjetosApproval(),
        ])
        setStats(statsData)
        setProjetos(projetosData)
        setHasMore(false)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
        setHasMore(false)
      } finally {
        setLoading(false)
      }
      return
    }

    try {
      setLoading(true)
      setError(null)
      const api = container.resolve(TimesheetComponentesApi)
      
      // Usar o novo endpoint de listar projetos visão gerente de projeto
      const response = await api.listarProjetosVisaoGerenteDeProjeto(
        params.token,
        {
          codProjeto: params.codProjeto || '',
          mes: params.mes,
          ano: params.ano,
          cpfColaborador: params.cpfColaborador || '',
          codStatusGrupo: params.codStatusGrupo || '0',
          cpfGerenteAdm: params.cpfGerenteAdm || '',
        }
      )

      // Armazenar dados brutos
      setProjetosRaw(response.retorno.lista)
      
      // Armazenar dataColetaDeDados e totalizador
      setDataColetaDeDados(response.retorno.dataColetaDeDados || "")
      if (response.retorno.totalizador) {
        setTotalizador(response.retorno.totalizador)
      } else {
        setTotalizador(null)
      }

      // Mapear dados para o formato da tabela
      // Adicionar índice para garantir chaves únicas mesmo quando há duplicatas
      let uniqueCounter = 0;
      const projetosMapeados = response.retorno.lista.map((proj, index) => {
        const mapped = mapProjetoToApprovalTable(proj, params.statusList);
        // Adicionar índice e contador para garantir unicidade absoluta
        return {
          ...mapped,
          id: `${mapped.id}-idx${index}-cnt${uniqueCounter++}`
        };
      });
      setProjetos(projetosMapeados)

      // Não há paginação neste endpoint
      setHasMore(false)
      setTotalItems(response.retorno.lista.length)

      // Popular stats - preferir totalizadorBigNumbers/totalizador quando disponíveis
      const formatarHoras = (minutos: number): string => {
        const horas = Math.floor(minutos / 60)
        const mins = minutos % 60
        return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`
      }

      const tot = response.retorno.totalizador
      const bigNumbers = response.retorno.totalizadorBigNumbers
      const colaboradoresUnicos = new Set(response.retorno.lista.map(p => p.cpfColaborador))
      const projetosUnicos = new Set(response.retorno.lista.map(p => p.codProjeto))
      const totalHorasLancadas = response.retorno.lista.reduce((sum, p) => sum + p.totalHoras, 0)
      const totalHorasAprovadas = response.retorno.lista
        .filter(p => p.codStatusMensal === 2)
        .reduce((sum, p) => sum + p.totalHoras, 0)
      const totalHorasPendentes = response.retorno.lista
        .filter(p => p.codStatusMensal === 1)
        .reduce((sum, p) => sum + p.totalHoras, 0)
      const totalHorasReprovadas = response.retorno.lista
        .filter(p => p.codStatusMensal === 3)
        .reduce((sum, p) => sum + p.totalHoras, 0)

      const statsData: TimesheetStatMock[] = [
        {
          icon: Users,
          label: "Colaboradores",
          value: String(bigNumbers?.quantidadeTotalColaboradores ?? tot?.quantidadeTotalColaboradores ?? colaboradoresUnicos.size),
          textColor: "text-foreground",
        },
        {
          icon: Triangle,
          label: "Projetos",
          value: String(tot?.quantidadeTotalProjetos ?? projetosUnicos.size),
          textColor: "text-foreground",
        },
        {
          icon: Timer,
          label: "Horas lançadas",
          value: formatarHoras(totalHorasLancadas),
          textColor: "text-foreground",
        },
        {
          icon: CheckCircle2,
          label: "Aprovados",
          value: formatarHoras(totalHorasAprovadas),
          textColor: "text-green-600",
        },
        {
          icon: Clock,
          label: "Pendentes",
          value: formatarHoras(totalHorasPendentes),
          textColor: "text-blue-600",
        },
        {
          icon: XCircle,
          label: "Reprovados",
          value: formatarHoras(totalHorasReprovadas),
          textColor: "text-destructive",
        },
      ]
      setStats(statsData)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      setProjetos([])
      setTotalizador(null)
      setHasMore(false)
    } finally {
      setLoading(false)
    }
  }, [
    params?.token,
    params?.cpfColaborador,
    params?.codStatusGrupo,
    params?.cpfGerenteAdm,
    params?.mes,
    params?.ano,
    params?.codProjeto,
  ])

  useEffect(() => {
    // Só executar se os parâmetros realmente mudaram
    if (paramsKey !== paramsRef.current) {
      paramsRef.current = paramsKey
      loadData()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paramsKey])

  // Remapear projetos quando statusList mudar
  useEffect(() => {
    if (projetosRaw.length > 0 && params?.statusList && params.statusList.length > 0) {
      // Adicionar índice para garantir chaves únicas mesmo quando há duplicatas
      let uniqueCounter = 0;
      const projetosMapeados = projetosRaw.map((proj, index) => {
        const mapped = mapProjetoToApprovalTable(proj, params.statusList);
        // Adicionar índice e contador para garantir unicidade absoluta
        return {
          ...mapped,
          id: `${mapped.id}-idx${index}-cnt${uniqueCounter++}`
        };
      });
      setProjetos(projetosMapeados)
    }
  }, [projetosRaw, params?.statusList])

  return { stats, projetos, projetosRaw, dataColetaDeDados, totalizador, loading, error, totalItems, hasMore, refetch: loadData }
}

// Função auxiliar para mapear dados da API para o formato da tabela
const mapColaboradorToTable = (colaborador: ColaboradorEApontamento, statusList?: { codStatusGrupo: number; descricao: string }[]): ColaboradorManagementMock & { codigoStatusApontamentoPeriodo: number; aprovadores: { nomeAprovador: string; codigoInternoAprovador: string }[] } => {
  // Formatar horas (somaHoras está em minutos, converter para formato HH:MM)
  const horas = Math.floor(colaborador.somaHoras / 60)
  const minutos = colaborador.somaHoras % 60
  const horasFormatadas = `${String(horas).padStart(2, '0')}:${String(minutos).padStart(2, '0')}`

  // Buscar descrição do status através do codigoStatusApontamentoPeriodo
  // Comparar ambos como números para garantir que a comparação funcione
  const codigoStatus = Number(colaborador.codigoStatusApontamentoPeriodo)
  let statusDescricao = colaborador.descricaoStatusApontamento || 'Sem status'
  
  if (statusList && statusList.length > 0) {
    const statusEncontrado = statusList.find(s => Number(s.codStatusGrupo) === codigoStatus)
    if (statusEncontrado) {
      statusDescricao = statusEncontrado.descricao
    }
  }

  // Criar ID único combinando CPF, projeto e outros campos para evitar duplicatas
  // Se projeto for null/undefined, usar um identificador alternativo
  const projetoId = colaborador.projeto || 'sem-projeto';
  const gestorId = colaborador.codigoGerente || 'sem-gestor';
  // Combinar CPF + projeto + gestor + status para garantir unicidade
  const uniqueId = `${colaborador.colaboradorCPF}-${projetoId}-${gestorId}-${colaborador.codigoStatusApontamentoPeriodo}`;
  
  return {
    id: uniqueId, // Adicionar ID único para usar como chave
    nome: colaborador.colaboradorNome,
    cpf: colaborador.colaboradorCPF, // Adicionar CPF para usar como chave única
    situacao: colaborador.ativo ? 'Ativo' : 'Inativo',
    dataSituacao: colaborador.dataAdmissao,
    gestorAdm: colaborador.nomeCompletoGerente,
    status: statusDescricao,
    projeto: colaborador.projeto,
    aprovador: '', // Será renderizado separadamente
    horasTrabalhadas: horasFormatadas,
    codigoStatusApontamentoPeriodo: colaborador.codigoStatusApontamentoPeriodo,
    aprovadores: colaborador.aprovadores,
  }
}

export interface UseManagementTabParams {
  token: string
  nomeColaborador?: string
  codigoGerente?: string
  codigoStatus?: string
  mesVigencia: number
  anoVigencia: number
  codProjeto?: string
  codColaboradorExternoAprovador?: string
  cursor?: number
  limite?: number
  statusList?: { codStatusGrupo: number; descricao: string }[]
}

export const useManagementTab = (params?: UseManagementTabParams) => {
  const [stats, setStats] = useState<TimesheetStatMock[]>([])
  const [colaboradores, setColaboradores] = useState<(ColaboradorManagementMock & { codigoStatusApontamentoPeriodo?: number; aprovadores?: { nomeAprovador: string; codigoInternoAprovador: string }[] })[]>([])
  const [colaboradoresRaw, setColaboradoresRaw] = useState<ColaboradorEApontamento[]>([]) // Armazenar dados brutos para remapear quando statusList mudar
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [totalItems, setTotalItems] = useState(0)
  const [hasMore, setHasMore] = useState(true)
  const paramsRef = useRef<string>('')

  // Criar uma chave serializada dos parâmetros para comparação
  // Incluir statusList na chave para garantir que quando ele mudar, os colaboradores sejam atualizados
  const paramsKey = params 
    ? JSON.stringify({
        token: params.token,
        nomeColaborador: params.nomeColaborador || '',
        codigoGerente: params.codigoGerente || '0',
        codigoStatus: params.codigoStatus || '0',
        mesVigencia: params.mesVigencia,
        anoVigencia: params.anoVigencia,
        codProjeto: params.codProjeto || '',
        codColaboradorExternoAprovador: params.codColaboradorExternoAprovador || '',
        cursor: params.cursor || 0,
        limite: params.limite || 20,
        statusListLength: params.statusList?.length || 0, // Incluir tamanho do statusList para detectar mudanças
      })
    : ''

  const loadData = useCallback(async () => {
    if (!params?.token) {
      // Se não tiver token ou params, usar mock
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(TimesheetComponentesApi)
        const [statsData, colaboradoresData] = await Promise.all([
          api.getManagementStats(),
          api.getColaboradoresManagement(),
        ])
        setStats(statsData)
        setColaboradores(colaboradoresData)
        setHasMore(false) // Mock não tem paginação
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
        setHasMore(false)
      } finally {
        setLoading(false)
      }
      return
    }

    try {
      setLoading(true)
      setError(null)
      const api = container.resolve(TimesheetComponentesApi)
      
      // Carregar colaboradores da API real
      const response = await api.listarColaboradoresEApontamentosPorGestor(
        params.token,
        {
          nomeColaborador: params.nomeColaborador || '',
          codigoGerente: params.codigoGerente || '0',
          codigoStatus: params.codigoStatus || '0',
          mesVigencia: params.mesVigencia,
          anoVigencia: params.anoVigencia,
          cursor: params.cursor || 0,
          limite: params.limite || 20,
          codProjeto: params.codProjeto || '',
          codColaboradorExternoAprovador: params.codColaboradorExternoAprovador || '',
        }
      )

      // Armazenar dados brutos para poder remapear quando statusList mudar
      setColaboradoresRaw(response.retorno.lista)
      
      // Mapear dados para o formato da tabela
      // Mapear colaboradores e adicionar índice para garantir chaves únicas
      const colaboradoresMapeados = response.retorno.lista.map((colab, index) => {
        const mapped = mapColaboradorToTable(colab, params.statusList);
        // Usar índice diretamente no ID para garantir unicidade absoluta
        // Combinar com timestamp ou índice para evitar qualquer duplicata
        const uniqueId = `${mapped.id ?? `colab-${index}`}-idx${index}-${Date.now()}`;
        return {
          ...mapped,
          id: uniqueId // ID único garantido com timestamp
        };
      });
      setColaboradores(colaboradoresMapeados)
      
      // Verificar se há mais páginas baseado na quantidade retornada
      const itemsReturned = response.retorno.lista.length
      const limite = params.limite || 20
      setHasMore(itemsReturned === limite)

      // Atualizar total de itens
      // Se a API retornar um campo total, usar ele
      // Caso contrário, será calculado no componente baseado em hasMore
      if (response.retorno.total !== undefined) {
        setTotalItems(response.retorno.total)
      }

      // Função para formatar horas (minutos para HH:MM)
      const formatarHoras = (minutos: number): string => {
        const horas = Math.floor(minutos / 60)
        const mins = minutos % 60
        return `${String(horas).padStart(2, '0')}:${String(mins).padStart(2, '0')}`
      }

      // Popular stats com os big numbers da API
      const bigNumbers = response.retorno.totalizadorBigNumbers
      const statsData: TimesheetStatMock[] = [
        {
          icon: Users,
          label: "Colaboradores",
          value: String(bigNumbers?.quantidadeTotalColaboradores || 0),
          textColor: "text-foreground",
        },
        {
          icon: Triangle,
          label: "Não apontado",
          value: bigNumbers?.quantidadeTotalNaoApontado || "0",
          textColor: "text-muted-foreground",
        },
        {
          icon: Timer,
          label: "Horas lançadas",
          value: formatarHoras(bigNumbers?.somaHorasLancadas || 0),
          textColor: "text-foreground",
        },
        {
          icon: CheckCircle2,
          label: "Aprovados",
          value: formatarHoras(bigNumbers?.somaHorasAprovadas || 0),
          textColor: "text-green-600",
        },
        {
          icon: Clock,
          label: "Pendentes",
          value: formatarHoras(bigNumbers?.somaHorasPendentes || 0),
          textColor: "text-blue-600",
        },
        {
          icon: XCircle,
          label: "Reprovados",
          value: formatarHoras(bigNumbers?.somaHorasReprovdas || 0),
          textColor: "text-destructive",
        },
      ]
      setStats(statsData)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      setColaboradores([])
      setHasMore(false)
    } finally {
      setLoading(false)
    }
  }, [
    params?.token,
    params?.nomeColaborador,
    params?.codigoGerente,
    params?.codigoStatus,
    params?.mesVigencia,
    params?.anoVigencia,
    params?.codProjeto,
    params?.codColaboradorExternoAprovador,
    params?.cursor,
    params?.limite,
  ])

  useEffect(() => {
    // Só executar se os parâmetros realmente mudaram
    if (paramsKey !== paramsRef.current) {
      paramsRef.current = paramsKey
      loadData()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paramsKey])

  // Remapear colaboradores quando statusList mudar (sem fazer nova chamada à API)
  useEffect(() => {
    if (colaboradoresRaw.length > 0 && params?.statusList && params.statusList.length > 0) {
      // Mapear colaboradores e adicionar índice para garantir chaves únicas
      // Usar um contador único para cada mapeamento para evitar duplicatas
      let uniqueCounter = 0;
      const colaboradoresMapeados = colaboradoresRaw.map((colab, index) => {
        const mapped = mapColaboradorToTable(colab, params.statusList);
        // Criar ID único usando índice e contador para garantir unicidade absoluta
        const uniqueId = `${mapped.id ?? `colab-${index}`}-idx${index}-cnt${uniqueCounter++}`;
        return {
          ...mapped,
          id: uniqueId // ID único garantido
        };
      });
      setColaboradores(colaboradoresMapeados)
    }
  }, [colaboradoresRaw, params?.statusList])


  return { stats, colaboradores, loading, error, totalItems, hasMore, refetch: loadData }
}

export const useWeeklyView = () => {
  const [weekData, setWeekData] = useState<WeekDataMock[]>([])
  const [dayDetails, setDayDetails] = useState<DayDetailMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        setError(null)
        const api = container.resolve(TimesheetComponentesApi)
        const [weekDataResult, dayDetailsResult] = await Promise.all([
          api.getWeekData(),
          api.getDayDetails(),
        ])
        setWeekData(weekDataResult)
        setDayDetails(dayDetailsResult)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  return { weekData, dayDetails, loading, error }
}

