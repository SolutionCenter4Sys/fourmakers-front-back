import { useState, useEffect } from 'react'
import { container } from '@core/di/container'
import { GetReembolsosUseCase } from '@domain/usecases/GetReembolsosUseCase'
import type { SolicitacaoReembolso, ListarSolicitacoesResponse } from '@domain/entities/SolicitacaoReembolso'
import type { ListarSolicitacoesParams } from '@data/api/ReembolsosApi'
import { useAppSelector } from '@app/store/hooks'
import { formatCurrency } from '@shared/utils/calculations'
import { TrendingUp, TrendingDown, Wallet } from '@/components/ui/system-icons'
import type { LucideIcon } from '@/components/ui/system-icons'

export interface ReembolsoStat {
  title: string
  value: string
  icon: LucideIcon
  color: string
  bgColor: string
}

export interface ReembolsoTableRow {
  id: string
  objetivo: string
  destino: string
  periodo: string
  clienteProjeto: string
  somaValores: string
  dataSolicitacao: string
  objeto: SolicitacaoReembolso['objeto']
}

export const useReembolsos = (dataInicial?: Date, dataFinal?: Date) => {
  const { token } = useAppSelector((state) => state.auth)
  const [solicitacoes, setSolicitacoes] = useState<SolicitacaoReembolso[]>([])
  const [stats, setStats] = useState<ReembolsoStat[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [responseData, setResponseData] = useState<ListarSolicitacoesResponse | null>(null)

  useEffect(() => {
    const loadSolicitacoes = async () => {
      if (!token) {
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        
        const useCase = container.resolve(GetReembolsosUseCase)
        
        const params: ListarSolicitacoesParams = {}
        
        if (dataInicial) {
          params.dataInicial = formatDateForApi(dataInicial)
        }
        
        if (dataFinal) {
          params.dataFinal = formatDateForApi(dataFinal)
        }

        const response = await useCase.listarSolicitacoesPorColab(token, params)
        
        if (response.sucesso && response.retorno) {
          setSolicitacoes(response.retorno.solicitacoes)
          setResponseData(response.retorno)
          
          // Criar stats a partir dos dados da API
          setStats([
            {
              title: "Total Solicitado",
              value: formatCurrency(response.retorno.totalSolicitado),
              icon: TrendingUp,
              color: "text-blue-600 dark:text-blue-400",
              bgColor: "bg-blue-50 dark:bg-blue-950/20",
            },
            {
              title: "Total Aprovado",
              value: formatCurrency(response.retorno.totalAprovado),
              icon: TrendingDown,
              color: "text-green-600 dark:text-green-400",
              bgColor: "bg-green-50 dark:bg-green-950/20",
            },
            {
              title: "Saldo",
              value: formatCurrency(response.retorno.saldo),
              icon: Wallet,
              color: "text-orange-600 dark:text-orange-400",
              bgColor: "bg-orange-50 dark:bg-orange-950/20",
            },
          ])
        } else {
          setError(response.mensagem || 'Erro ao carregar solicitações')
          setSolicitacoes([])
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar solicitações')
        setSolicitacoes([])
      } finally {
        setLoading(false)
      }
    }

    loadSolicitacoes()
  }, [token, dataInicial, dataFinal])

  // Converter para formato de tabela
  const reembolsos: ReembolsoTableRow[] = solicitacoes.map((solicitacao, index) => ({
    id: `${solicitacao.dataSolicitacao}-${index}`,
    objetivo: solicitacao.objetivo || '',
    destino: solicitacao.destino || '',
    periodo: solicitacao.periodo || '',
    clienteProjeto: `${solicitacao.cliente} / ${solicitacao.projeto}`,
    somaValores: formatCurrency(solicitacao.somaValores),
    dataSolicitacao: formatDate(solicitacao.dataSolicitacao),
    objeto: solicitacao.objeto,
  }))

  return { 
    reembolsos, 
    stats, 
    loading, 
    error,
    solicitacoes,
    souAprovador: responseData?.souAprovador ?? false,
    souGestor: responseData?.souGestor ?? false,
  }
}

function formatDateForApi(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

function formatDate(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleDateString('pt-BR')
}

