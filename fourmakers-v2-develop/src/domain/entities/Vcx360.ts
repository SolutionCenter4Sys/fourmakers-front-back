export type Prioridade = 'Alta' | 'Média' | 'Baixa'
export type Impacto = 'Alto' | 'Médio' | 'Baixo'
export type StatusIniciativa = 'Ativa' | 'Em Planejamento' | 'Pausada' | 'Concluída'
export type TipoContato = 'Teams' | 'Google' | 'Digital' | 'Presencial'
export type StatusAgenda = 'Pendente' | 'Em andamento' | 'Concluído' | 'Cancelado'

export interface DorOportunidade {
  id: string
  titulo: string
  descricao: string
  prioridade: Prioridade
  impacto: Impacto
  dataCriacao: string
}

export interface PropostaValor {
  id: string
  titulo: string
  problemaResolvido: string
  temaConectado: string
  dataCriacao: string
}

export interface Kpi {
  id: string
  nome: string
  valor: number | string
  meta?: number | string
  unidade?: string
}

export interface Iniciativa {
  id: string
  titulo: string
  tema: string
  objetivoKpi: string
  status: StatusIniciativa
  dataCriacao: string
  dataAtualizacao?: string
}

export interface ParticipanteAgenda {
  id: string
  nome: string
  iniciais: string
  avatarUrl?: string
}

export interface ItemAgenda {
  id: string
  titulo: string
  tipo: TipoContato
  data: string
  descricao: string
  status: StatusAgenda
  participantes: ParticipanteAgenda[]
  dataCriacao: string
}

export interface ItemHistorico {
  id: string
  nomeCliente: string
  periodo: string
  cargo: string
  descricaoAtuacao: string
  atual: boolean
}

export interface DadosVcx360 {
  departamentoId: string
  colaboradorId?: string
  contexto: {
    doresOportunidades: DorOportunidade[]
    propostasValor: PropostaValor[]
    kpis: Kpi[]
  }
  iniciativas: Iniciativa[]
  agenda: ItemAgenda[]
  historico: ItemHistorico[]
  dataUltimaAtualizacao: string
}
