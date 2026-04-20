import type { LucideIcon } from '@/components/ui/system-icons'
import { Users, DollarSign, CheckCircle, XCircle, Clock } from '@/components/ui/system-icons'

export interface ReembolsoStatMock {
  title: string
  value: string
  icon: LucideIcon
  color: string
  bgColor: string
}

export interface ColaboradorGestaoAdmMock {
  id: number
  nome: string
  qtdDespesa: number
  dataSolicitacao: string
  status: string
  cliente: string
  projeto: string
  aprovador: string
  valoresSolicitados: string
  observacoes: string
}

export interface SolicitacaoAprovacaoMock {
  id: number
  colaborador: string
  cliente: string
  projeto: string
  valorTotal: string
  obs: string
  status: string
}

export const gestaoAdmStatsMock: ReembolsoStatMock[] = [
  {
    title: "Colaboradores",
    value: "45",
    icon: Users,
    color: "text-blue-600 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950/20",
  },
  {
    title: "Valores Lançados",
    value: "R$ 85.430,00",
    icon: DollarSign,
    color: "text-purple-600 dark:text-purple-400",
    bgColor: "bg-purple-50 dark:bg-purple-950/20",
  },
  {
    title: "Total Pago",
    value: "R$ 65.850,00",
    icon: CheckCircle,
    color: "text-green-600 dark:text-green-400",
    bgColor: "bg-green-50 dark:bg-green-950/20",
  },
  {
    title: "Aprovados",
    value: "128",
    icon: CheckCircle,
    color: "text-green-600 dark:text-green-400",
    bgColor: "bg-green-50 dark:bg-green-950/20",
  },
  {
    title: "Reprovados",
    value: "12",
    icon: XCircle,
    color: "text-red-600 dark:text-red-400",
    bgColor: "bg-red-50 dark:bg-red-950/20",
  },
  {
    title: "Pendentes",
    value: "23",
    icon: Clock,
    color: "text-orange-600 dark:text-orange-400",
    bgColor: "bg-orange-50 dark:bg-orange-950/20",
  },
]

export const colaboradoresGestaoAdmMock: ColaboradorGestaoAdmMock[] = [
  {
    id: 1,
    nome: "Tiago Augusto Rocha",
    qtdDespesa: 2,
    dataSolicitacao: "24/10/2025",
    status: "Pendente",
    cliente: "1 - A",
    projeto: "1 - non dignissimos",
    aprovador: "-",
    valoresSolicitados: "R$ 200,00",
    observacoes: "",
  },
  {
    id: 2,
    nome: "Tiago Augusto Rocha",
    qtdDespesa: 2,
    dataSolicitacao: "24/10/2025",
    status: "Pendente",
    cliente: "1 - A",
    projeto: "1 - non dignissimos",
    aprovador: "-",
    valoresSolicitados: "R$ 10,00",
    observacoes: "",
  },
  {
    id: 3,
    nome: "Samuel Manoel Da Silva",
    qtdDespesa: 18,
    dataSolicitacao: "29/07/2025",
    status: "Pago",
    cliente: "37 - Inter",
    projeto: "00002 - AssureSync",
    aprovador: "Samuel Manoel Da Silva",
    valoresSolicitados: "R$ 1.650,00",
    observacoes: "",
  },
]

export const aprovacoesStatsMock: ReembolsoStatMock[] = [
  {
    title: "Colaboradores",
    value: "28",
    icon: Users,
    color: "text-blue-600 dark:text-blue-400",
    bgColor: "bg-blue-50 dark:bg-blue-950/20",
  },
  {
    title: "Valores Lançados",
    value: "R$ 45.230,00",
    icon: DollarSign,
    color: "text-purple-600 dark:text-purple-400",
    bgColor: "bg-purple-50 dark:bg-purple-950/20",
  },
  {
    title: "Aprovados",
    value: "67",
    icon: CheckCircle,
    color: "text-green-600 dark:text-green-400",
    bgColor: "bg-green-50 dark:bg-green-950/20",
  },
  {
    title: "Pendentes",
    value: "15",
    icon: Clock,
    color: "text-orange-600 dark:text-orange-400",
    bgColor: "bg-orange-50 dark:bg-orange-950/20",
  },
  {
    title: "Reprovados",
    value: "8",
    icon: XCircle,
    color: "text-red-600 dark:text-red-400",
    bgColor: "bg-red-50 dark:bg-red-950/20",
  },
]

export const solicitacoesAprovacaoMock: SolicitacaoAprovacaoMock[] = [
  {
    id: 1,
    colaborador: "Samuel Manoel Da Silva",
    cliente: "C6 Bank",
    projeto: "QA alérgico A BUG",
    valorTotal: "R$ 10,50",
    obs: "-",
    status: "Pendente",
  },
  {
    id: 2,
    colaborador: "Samuel Manoel Da Silva",
    cliente: "C6 Bank",
    projeto: "QA alérgico A BUG",
    valorTotal: "R$ 100,00",
    obs: "-",
    status: "Pendente",
  },
  {
    id: 3,
    colaborador: "Samuel Manoel Da Silva",
    cliente: "Inter",
    projeto: "AssureSync",
    valorTotal: "R$ 300,00",
    obs: "-",
    status: "Pago",
  },
  {
    id: 4,
    colaborador: "Samuel Manoel Da Silva",
    cliente: "Inter",
    projeto: "AssureSync",
    valorTotal: "R$ 2.500,00",
    obs: "-",
    status: "Pago",
  },
]

