import { useState, useEffect, useCallback } from 'react'
import { container } from '@core/di/container'
import { DiTokens } from '@core/di/tokens'
import { ReembolsosApi, type ListarSolicitacoesVisaoAdmParams, type ListarSolicitacoesGerenteProjetoParams, type StatusReembolso } from '@data/api/ReembolsosApi'
import { ProjetosApi, type Cliente, type Projeto } from '@data/api/ProjetosApi'
import { useAppSelector } from '@app/store/hooks'
import type { SolicitacaoVisaoAdm, SolicitacaoGerenteProjeto } from '@domain/entities/SolicitacaoReembolso'
import { formatCurrency } from '@shared/utils/calculations'
import { Users, DollarSign, CheckCircle, XCircle, Clock } from '@/components/ui/system-icons'
import type { LucideIcon } from '@/components/ui/system-icons'

export interface ReembolsoStat {
  title: string
  value: string
  icon: LucideIcon
  color: string
  bgColor: string
}

export interface ColaboradorGestaoAdmRow {
  id: string
  nomeColaborador: string
  saldoAdiantamentos: number
  objetivo: string
  destino: string | null
  periodo: string
  cliente: string
  projeto: string
  somaValores: string
  dataSolicitacao: string
  pagoTotal: string
  objeto: SolicitacaoVisaoAdm['objeto']
  solicitacaoCompleta: SolicitacaoVisaoAdm
}

const formatDate = (dateString: string): string => {
  if (!dateString) return '-'
  try {
    const date = new Date(dateString)
    return date.toLocaleDateString('pt-BR')
  } catch {
    return dateString
  }
}

function formatDateForApi(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

// Função para criar mapeamento dinâmico de status
const createStatusToIdMap = (statusList: StatusReembolso[]): Record<string, number> => {
  const map: Record<string, number> = {}
  statusList.forEach(status => {
    const key = status.descricao.toLowerCase()
    map[key] = status.id
  })
  return map
}

export interface UseGestaoAdmTabParams {
  dataInicio?: Date
  dataFim?: Date
  codigoProjeto?: string
  codigoCliente?: string
  status?: string
  aprovadorId?: string
}

export interface UseGestaoAdmTabReturn {
  stats: ReembolsoStat[]
  colaboradores: ColaboradorGestaoAdmRow[]
  statusList: StatusReembolso[]
  clientesList: Cliente[]
  projetosList: Projeto[]
  loading: boolean
  error: string | null
  loadProjetos: (codigoCliente: string) => Promise<void>
}

export const useGestaoAdmTab = (filters?: UseGestaoAdmTabParams): UseGestaoAdmTabReturn => {
  const { token } = useAppSelector((state) => state.auth)
  const [stats, setStats] = useState<ReembolsoStat[]>([])
  const [colaboradores, setColaboradores] = useState<ColaboradorGestaoAdmRow[]>([])
  const [statusList, setStatusList] = useState<StatusReembolso[]>([])
  const [clientesList, setClientesList] = useState<Cliente[]>([])
  const [projetosList, setProjetosList] = useState<Projeto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Carregar lista de status
  useEffect(() => {
    const loadStatus = async () => {
      if (!token) return

      try {
        const api = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi)
        const response = await api.listarStatus(token)
        
        if (response.sucesso && response.retorno) {
          setStatusList(response.retorno)
        }
      } catch (err) {
        console.error('Erro ao carregar status:', err)
      }
    }

    loadStatus()
  }, [token])

  // Carregar lista de clientes
  useEffect(() => {
    const loadClientes = async () => {
      if (!token) return

      try {
        const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi)
        const response = await projetosApi.listarClientesOrg(token)
        
        // O retorno é sempre um array direto
        const clientes = Array.isArray(response) ? response : []
        setClientesList(clientes)
      } catch (err) {
        console.error('Erro ao carregar clientes:', err)
      }
    }

    loadClientes()
  }, [token])

  // Função para carregar projetos do cliente
  const loadProjetos = useCallback(async (codigoCliente: string) => {
    if (!token) {
      setProjetosList([])
      return
    }
    
    if (!codigoCliente || codigoCliente === "") {
      setProjetosList([])
      return
    }

    try {
      // Criar uma nova instância diretamente para evitar problemas de cache do container
      const projetosApi = new ProjetosApi()
      const response = await projetosApi.listarProjetosDoCliente(token, codigoCliente)
      
      if (response.sucesso && response.retorno) {
        setProjetosList(response.retorno)
      } else {
        setProjetosList([])
      }
    } catch (err) {
      console.error('Erro ao carregar projetos:', err)
      setProjetosList([])
    }
  }, [token])

  useEffect(() => {
    const loadData = async () => {
      if (!token) {
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const api = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi)
        
        // Criar mapeamento dinâmico de status
        const statusToId = createStatusToIdMap(statusList)
        
        const params: ListarSolicitacoesVisaoAdmParams = {
          dataInicio: filters?.dataInicio ? formatDateForApi(filters.dataInicio) : '',
          dataFim: filters?.dataFim ? formatDateForApi(filters.dataFim) : '',
          codigoProjeto: filters?.codigoProjeto || '',
          codigoCliente: filters?.codigoCliente || '',
          statusId: filters?.status ? statusToId[filters.status.toLowerCase()] || 0 : 0,
          aprovadorId: filters?.aprovadorId || '',
        }
        
        const response = await api.listarSolicitacoesVisaoAdm(token, params)
        
        if (response.sucesso && response.retorno) {
          // Criar stats a partir dos dados da API
          setStats([
            {
              title: "Colaboradores",
              value: response.retorno.colaboradores.toString(),
              icon: Users,
              color: "text-blue-600 dark:text-blue-400",
              bgColor: "bg-blue-50 dark:bg-blue-950/20",
            },
            {
              title: "Valores Lançados",
              value: formatCurrency(response.retorno.valorSolicitado),
              icon: DollarSign,
              color: "text-purple-600 dark:text-purple-400",
              bgColor: "bg-purple-50 dark:bg-purple-950/20",
            },
            {
              title: "Valores Aprovados",
              value: formatCurrency(response.retorno.valorAprovado),
              icon: CheckCircle,
              color: "text-green-600 dark:text-green-400",
              bgColor: "bg-green-50 dark:bg-green-950/20",
            },
            {
              title: "Valores Reprovados",
              value: formatCurrency(response.retorno.valorReprovado),
              icon: XCircle,
              color: "text-red-600 dark:text-red-400",
              bgColor: "bg-red-50 dark:bg-red-950/20",
            },
            {
              title: "Valores Pendentes",
              value: formatCurrency(response.retorno.valorPendente),
              icon: Clock,
              color: "text-orange-600 dark:text-orange-400",
              bgColor: "bg-orange-50 dark:bg-orange-950/20",
            },
            {
              title: "Valores Pagos",
              value: formatCurrency(response.retorno.valorPago),
              icon: CheckCircle,
              color: "text-green-600 dark:text-green-400",
              bgColor: "bg-green-50 dark:bg-green-950/20",
            },
          ])

          // Converter para formato de tabela
          const colaboradoresData: ColaboradorGestaoAdmRow[] = response.retorno.solicitacoes.map((solicitacao, index) => {
            // Calcular Pago/Total
            const totalItens = solicitacao.objeto.length
            const itensPagos = solicitacao.objeto.filter(item => item.status === 'Pago' || item.statusId === 4).length
            const pagoTotal = `${itensPagos}/${totalItens}`

            return {
              id: `${solicitacao.dataSolicitacao}-${index}`,
              nomeColaborador: solicitacao.nomeColaborador,
              saldoAdiantamentos: solicitacao.saldoAdiantamentos ?? 0,
              objetivo: solicitacao.objetivo || '-',
              destino: solicitacao.destino,
              periodo: solicitacao.periodo || '-',
              cliente: solicitacao.cliente,
              projeto: solicitacao.projeto,
              somaValores: formatCurrency(solicitacao.somaValores),
              dataSolicitacao: formatDate(solicitacao.dataSolicitacao),
              pagoTotal,
              objeto: solicitacao.objeto,
              solicitacaoCompleta: solicitacao,
            }
          })

          setColaboradores(colaboradoresData)
        } else {
          setError(response.mensagem || 'Erro ao carregar dados')
          setColaboradores([])
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
        setColaboradores([])
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [token, statusList, filters?.dataInicio, filters?.dataFim, filters?.codigoProjeto, filters?.codigoCliente, filters?.status, filters?.aprovadorId])

  return { stats, colaboradores, statusList, clientesList, projetosList, loading, error, loadProjetos }
}

export interface SolicitacaoAprovacaoRow {
  id: string
  nomeColaborador: string
  codigoInternoColaborador: string
  objetivo: string
  destino: string | null
  periodo: string
  cliente: string
  projeto: string
  somaValores: string
  dataSolicitacao: string
  solicitacaoCompleta: SolicitacaoGerenteProjeto
}

export interface UseAprovacoesTabParams {
  dataInicio?: Date
  dataFim?: Date
  codigoCliente?: string
  codigoProjeto?: string
  status?: string
}

export interface UseAprovacoesTabReturn {
  solicitacoes: SolicitacaoAprovacaoRow[]
  stats: ReembolsoStat[]
  statusList: StatusReembolso[]
  clientesList: Cliente[]
  projetosList: Projeto[]
  loading: boolean
  error: string | null
  loadProjetos: (codigoCliente: string) => Promise<void>
}

export const useAprovacoesTab = (filters?: UseAprovacoesTabParams): UseAprovacoesTabReturn => {
  const { token } = useAppSelector((state) => state.auth)
  const [solicitacoes, setSolicitacoes] = useState<SolicitacaoAprovacaoRow[]>([])
  const [stats, setStats] = useState<ReembolsoStat[]>([])
  const [statusList, setStatusList] = useState<StatusReembolso[]>([])
  const [clientesList, setClientesList] = useState<Cliente[]>([])
  const [projetosList, setProjetosList] = useState<Projeto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Carregar status e clientes na montagem do componente
  useEffect(() => {
    const loadInitialData = async () => {
      if (!token) return

      try {
        const reembolsosApi = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi)
        const projetosApi = container.resolve<ProjetosApi>(DiTokens.projetosApi)

        const [statusResponse, clientesResponse] = await Promise.all([
          reembolsosApi.listarStatus(token),
          projetosApi.listarClientesOrg(token),
        ])

        if (statusResponse.sucesso && statusResponse.retorno) {
          setStatusList(statusResponse.retorno)
        }

        // O retorno é sempre um array direto
        const clientes = Array.isArray(clientesResponse) ? clientesResponse : []
        setClientesList(clientes)
      } catch (err) {
        console.error('Erro ao carregar dados iniciais:', err)
      }
    }

    loadInitialData()
  }, [token])

  // Função para carregar projetos do cliente
  const loadProjetos = useCallback(async (codigoCliente: string) => {
    if (!token || !codigoCliente) {
      setProjetosList([])
      return
    }

    try {
      const projetosApi = new ProjetosApi()
      const response = await projetosApi.listarProjetosDoCliente(token, codigoCliente)
      
      if (response.sucesso && response.retorno) {
        setProjetosList(response.retorno)
      } else {
        setProjetosList([])
      }
    } catch (err) {
      console.error('Erro ao carregar projetos:', err)
      setProjetosList([])
    }
  }, [token])

  useEffect(() => {
    const loadData = async () => {
      if (!token) {
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const reembolsosApi = container.resolve<ReembolsosApi>(DiTokens.reembolsosApi)
        
        // Criar mapeamento dinâmico de status
        const statusToId = createStatusToIdMap(statusList)
        
        const params: ListarSolicitacoesGerenteProjetoParams = {
          filtro: '',
          clienteId: filters?.codigoCliente || '',
          projetoId: filters?.codigoProjeto || '',
          dataInicio: filters?.dataInicio ? formatDateForApi(filters.dataInicio) : '',
          dataFim: filters?.dataFim ? formatDateForApi(filters.dataFim) : '',
          statusId: filters?.status ? statusToId[filters.status.toLowerCase()] || 0 : 0,
        }
        
        // Buscar dados em paralelo
        const [solicitacoesResponse, bigNumbersResponse] = await Promise.all([
          reembolsosApi.listarSolicitacoesGerenteProjeto(token, params),
          reembolsosApi.buscarSolicitacaoBigNumbers(token),
        ])

        if (solicitacoesResponse.sucesso && solicitacoesResponse.retorno) {
          // Converter para formato de tabela
          const solicitacoesData: SolicitacaoAprovacaoRow[] = solicitacoesResponse.retorno.map((solicitacao, index) => {
            // Pegar o codigoInternoColaborador do primeiro item do objeto
            const codigoInternoColaborador = solicitacao.objeto && solicitacao.objeto.length > 0 
              ? solicitacao.objeto[0].codigoInternoColaborador 
              : ''
            
            return {
              id: `${solicitacao.dataSolicitacao}-${index}`,
              nomeColaborador: solicitacao.nomeColaborador,
              codigoInternoColaborador,
              objetivo: solicitacao.objetivo || '-',
              destino: solicitacao.destino || '-',
              periodo: solicitacao.periodo || '-',
              cliente: solicitacao.cliente,
              projeto: solicitacao.projeto,
              somaValores: formatCurrency(solicitacao.somaValores),
              dataSolicitacao: formatDate(solicitacao.dataSolicitacao),
              solicitacaoCompleta: solicitacao,
            }
          })

          setSolicitacoes(solicitacoesData)
        }

        if (bigNumbersResponse.sucesso && bigNumbersResponse.retorno) {
          const bigNumbers = bigNumbersResponse.retorno
          setStats([
            {
              title: 'Colaboradores',
              value: bigNumbers.qtdColaborador?.toString() || '0',
              icon: Users,
              color: 'text-gray-700 dark:text-gray-300',
              bgColor: 'bg-gray-100 dark:bg-gray-800',
            },
            {
              title: 'Valores lançados',
              value: formatCurrency(bigNumbers.valoresLancados || 0),
              icon: DollarSign,
              color: 'text-yellow-700 dark:text-yellow-300',
              bgColor: 'bg-yellow-100 dark:bg-yellow-900',
            },
            {
              title: 'Aprovados',
              value: formatCurrency(bigNumbers.valoresAprovados || 0),
              icon: CheckCircle,
              color: 'text-green-700 dark:text-green-300',
              bgColor: 'bg-green-100 dark:bg-green-900',
            },
            {
              title: 'Pendentes',
              value: formatCurrency(bigNumbers.valoresPendentes || 0),
              icon: Clock,
              color: 'text-blue-700 dark:text-blue-300',
              bgColor: 'bg-blue-100 dark:bg-blue-900',
            },
            {
              title: 'Reprovados',
              value: formatCurrency(bigNumbers.valoresReprovados || 0),
              icon: XCircle,
              color: 'text-red-700 dark:text-red-300',
              bgColor: 'bg-red-100 dark:bg-red-900',
            },
          ])
        } else {
          // Se a API não retornou dados, inicializar com valores zerados
          setStats([
            {
              title: 'Colaboradores',
              value: '0',
              icon: Users,
              color: 'text-gray-700 dark:text-gray-300',
              bgColor: 'bg-gray-100 dark:bg-gray-800',
            },
            {
              title: 'Valores lançados',
              value: formatCurrency(0),
              icon: DollarSign,
              color: 'text-yellow-700 dark:text-yellow-300',
              bgColor: 'bg-yellow-100 dark:bg-yellow-900',
            },
            {
              title: 'Aprovados',
              value: formatCurrency(0),
              icon: CheckCircle,
              color: 'text-green-700 dark:text-green-300',
              bgColor: 'bg-green-100 dark:bg-green-900',
            },
            {
              title: 'Pendentes',
              value: formatCurrency(0),
              icon: Clock,
              color: 'text-blue-700 dark:text-blue-300',
              bgColor: 'bg-blue-100 dark:bg-blue-900',
            },
            {
              title: 'Reprovados',
              value: formatCurrency(0),
              icon: XCircle,
              color: 'text-red-700 dark:text-red-300',
              bgColor: 'bg-red-100 dark:bg-red-900',
            },
          ])
        }
      } catch (err) {
        console.error('Erro ao carregar dados de aprovações:', err)
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
        setSolicitacoes([])
        // Inicializar stats com valores zerados mesmo em caso de erro
        setStats([
          {
            title: 'Colaboradores',
            value: '0',
            icon: Users,
            color: 'text-gray-700 dark:text-gray-300',
            bgColor: 'bg-gray-100 dark:bg-gray-800',
          },
          {
            title: 'Valores lançados',
            value: formatCurrency(0),
            icon: DollarSign,
            color: 'text-yellow-700 dark:text-yellow-300',
            bgColor: 'bg-yellow-100 dark:bg-yellow-900',
          },
          {
            title: 'Aprovados',
            value: formatCurrency(0),
            icon: CheckCircle,
            color: 'text-green-700 dark:text-green-300',
            bgColor: 'bg-green-100 dark:bg-green-900',
          },
          {
            title: 'Pendentes',
            value: formatCurrency(0),
            icon: Clock,
            color: 'text-blue-700 dark:text-blue-300',
            bgColor: 'bg-blue-100 dark:bg-blue-900',
          },
          {
            title: 'Reprovados',
            value: formatCurrency(0),
            icon: XCircle,
            color: 'text-red-700 dark:text-red-300',
            bgColor: 'bg-red-100 dark:bg-red-900',
          },
        ])
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [token, statusList, filters?.dataInicio, filters?.dataFim, filters?.codigoProjeto, filters?.codigoCliente, filters?.status])

  return { solicitacoes, stats, statusList, clientesList, projetosList, loading, error, loadProjetos }
}

