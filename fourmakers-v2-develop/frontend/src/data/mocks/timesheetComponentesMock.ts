import type { LucideIcon } from '@/components/ui/system-icons'
import { Users, Square, Clock, CheckCircle2, XCircle, Triangle, Timer } from '@/components/ui/system-icons'

export interface TimesheetStatMock {
  icon: LucideIcon
  label: string
  value: string
  textColor: string
}

export interface ProjetoApprovalMock {
  id: string
  colaborador: string
  cpfColaborador?: string
  mes?: number
  ano?: number
  situacao: string
  dataSituacao: string
  cliente: string
  projeto: string
  fimProjeto: string
  gestorAdm: string
  status: string
  horas: string
  codProjeto?: string
  codStatusMensal?: number
  ativo?: boolean
}

export interface ColaboradorManagementMock {
  id?: string
  nome: string
  cpf?: string
  situacao: string
  dataSituacao: string
  gestorAdm: string
  status: string
  projeto: string
  aprovador: string
  horasTrabalhadas: string
}

export interface WeekDataMock {
  day: number
  weekDay: string
  status: string
  hours: string
  date: string
}

export interface DayDetailMock {
  cliente: string
  projeto: string
  aprovadores: string
  atividade: string
  resumo: string
  status: string
  horas: string
}

export const timesheetStatsMock: TimesheetStatMock[] = [
  {
    icon: Timer,
    label: "Horas lançadas",
    value: "09:00",
    textColor: "text-foreground",
  },
  {
    icon: CheckCircle2,
    label: "Aprovadas",
    value: "00:00",
    textColor: "text-green-600",
  },
  {
    icon: Clock,
    label: "Pendentes",
    value: "09:00",
    textColor: "text-primary",
  },
  {
    icon: XCircle,
    label: "Reprovados",
    value: "00:00",
    textColor: "text-destructive",
  },
]

export const approvalsStatsMock: TimesheetStatMock[] = [
  {
    icon: Users,
    label: "Colaboradores",
    value: "3664",
    textColor: "text-foreground",
  },
  {
    icon: Square,
    label: "Projetos",
    value: "1",
    textColor: "text-foreground",
  },
  {
    icon: Timer,
    label: "Horas lançadas",
    value: "16:00",
    textColor: "text-foreground",
  },
  {
    icon: CheckCircle2,
    label: "Aprovados",
    value: "00:00",
    textColor: "text-green-600",
  },
  {
    icon: Clock,
    label: "Pendentes",
    value: "16:00",
    textColor: "text-primary",
  },
  {
    icon: XCircle,
    label: "Reprovados",
    value: "00:00",
    textColor: "text-destructive",
  },
]

export const projetosApprovalMock: ProjetoApprovalMock[] = [
  {
    id: "1",
    colaborador: "Joao Vitor Batist...",
    situacao: "Ativo",
    dataSituacao: "07/11/2020",
    cliente: "37 - Inter",
    projeto: "00002 - AssureSync",
    fimProjeto: "31/08/2025",
    gestorAdm: "Joao Marcos dos...",
    status: "Pendente",
    horas: "08:00",
  },
  {
    id: "2",
    colaborador: "Luan Kaique Rodri...",
    situacao: "Ativo",
    dataSituacao: "07/10/2024",
    cliente: "37 - Inter",
    projeto: "00002 - AssureSync",
    fimProjeto: "31/08/2025",
    gestorAdm: "Samuel Manoel D...",
    status: "Pendente",
    horas: "08:00",
  },
]

export const managementStatsMock: TimesheetStatMock[] = [
  {
    icon: Users,
    label: "Colaboradores",
    value: "4",
    textColor: "text-foreground",
  },
  {
    icon: Triangle,
    label: "Não apontado",
    value: "0",
    textColor: "text-muted-foreground",
  },
  {
    icon: Timer,
    label: "Horas lançadas",
    value: "25:01",
    textColor: "text-foreground",
  },
  {
    icon: CheckCircle2,
    label: "Aprovados",
    value: "00:00",
    textColor: "text-green-600",
  },
  {
    icon: Clock,
    label: "Pendentes",
    value: "25:01",
    textColor: "text-primary",
  },
  {
    icon: XCircle,
    label: "Reprovados",
    value: "00:00",
    textColor: "text-destructive",
  },
]

export const colaboradoresManagementMock: ColaboradorManagementMock[] = [
  {
    nome: "Joao Vitor Batista da Silva",
    situacao: "Ativo",
    dataSituacao: "07/11/2020",
    gestorAdm: "Joao Marcos dos Santos Silva",
    status: "Pendente",
    projeto: "00002 - AssureSync",
    aprovador: "JOAO VITOR...",
    horasTrabalhadas: "08:00",
  },
  {
    nome: "Luan Kaique Rodrigues Ribeiro",
    situacao: "Ativo",
    dataSituacao: "07/10/2024",
    gestorAdm: "Samuel Manoel Da Silva",
    status: "Pendente",
    projeto: "00002 - AssureSync",
    aprovador: "LUAN KAIQU...",
    horasTrabalhadas: "08:00",
  },
  {
    nome: "Samuel Manoel Da Silva",
    situacao: "Ativo",
    dataSituacao: "25/05/2024",
    gestorAdm: "Isadora Nogueira",
    status: "Pendente",
    projeto: "9624 - Implanta...",
    aprovador: "SAMUEL MAN...",
    horasTrabalhadas: "08:00",
  },
]

export const weekDataMock: WeekDataMock[] = [
  { day: 25, weekDay: "Domingo", status: "not-logged", hours: "---", date: "25/03/2024" },
  { day: 26, weekDay: "Segunda", status: "not-logged", hours: "---", date: "26/03/2024" },
  { day: 27, weekDay: "Terça", status: "not-logged", hours: "---", date: "27/03/2024" },
  { day: 28, weekDay: "Quarta", status: "not-logged", hours: "---", date: "28/03/2024" },
  { day: 29, weekDay: "Quinta", status: "not-logged", hours: "---", date: "29/03/2024" },
  { day: 1, weekDay: "Sexta", status: "pending", hours: "01:00", date: "01/03/2024" },
  { day: 2, weekDay: "Sábado", status: "not-logged", hours: "---", date: "02/03/2024" },
]

export const dayDetailsMock: DayDetailMock[] = [
  {
    cliente: "35 - Sofisa Direto",
    projeto: "8776 - Agora Broker",
    aprovadores: "",
    atividade: "Criação da massa de dados",
    resumo: "testing",
    status: "pending",
    horas: "08:00",
  },
]

