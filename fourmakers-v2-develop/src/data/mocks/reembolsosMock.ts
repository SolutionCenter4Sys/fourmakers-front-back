import type { LucideIcon } from '@/components/ui/system-icons'
import { TrendingUp, TrendingDown, Wallet } from '@/components/ui/system-icons'

export interface ReembolsoStatMock {
  title: string
  value: string
  icon: LucideIcon
  color: string
  bgColor: string
}

export interface ReembolsoMock {
  id: number
  clienteProjeto: string
  tipo: string
  valorSolicitado: string
  valorAprovado: string
  dataSolicitacao: string
  dataAprovacao: string
  status: string
  comprovantes: number
  observacoes: string
}

export const reembolsosStatsMock: ReembolsoStatMock[] = [
  {
    title: "Total Solicitado",
    value: "R$ 15.430,00",
    icon: TrendingUp,
    color: "text-blue-600 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950/20",
  },
  {
    title: "Total Aprovado",
    value: "R$ 12.850,00",
    icon: TrendingDown,
    color: "text-green-600 dark:text-green-400",
    bgColor: "bg-green-50 dark:bg-green-950/20",
  },
  {
    title: "Saldo",
    value: "R$ 2.580,00",
    icon: Wallet,
    color: "text-orange-600 dark:text-orange-400",
    bgColor: "bg-orange-50 dark:bg-orange-950/20",
  },
]

export const reembolsosMock: ReembolsoMock[] = [
  {
    id: 1,
    clienteProjeto: "Projeto Alpha - Cliente XYZ",
    tipo: "Alimentação",
    valorSolicitado: "R$ 250,00",
    valorAprovado: "R$ 250,00",
    dataSolicitacao: "15/01/2025",
    dataAprovacao: "16/01/2025",
    status: "Aprovado",
    comprovantes: 2,
    observacoes: "Almoço com cliente",
  },
  {
    id: 2,
    clienteProjeto: "Projeto Beta - Cliente ABC",
    tipo: "Transporte",
    valorSolicitado: "R$ 450,00",
    valorAprovado: "R$ 400,00",
    dataSolicitacao: "18/01/2025",
    dataAprovacao: "19/01/2025",
    status: "Aprovado Parcial",
    comprovantes: 3,
    observacoes: "Uber para reunião",
  },
  {
    id: 3,
    clienteProjeto: "Projeto Gamma - Cliente DEF",
    tipo: "Hospedagem",
    valorSolicitado: "R$ 1.200,00",
    valorAprovado: "-",
    dataSolicitacao: "20/01/2025",
    dataAprovacao: "-",
    status: "Pendente",
    comprovantes: 1,
    observacoes: "Hotel em São Paulo",
  },
]

