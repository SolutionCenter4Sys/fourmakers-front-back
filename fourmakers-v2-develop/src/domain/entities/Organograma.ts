// Entidades para operações CRUD do Organograma

export interface DepartamentoPayload {
  orgId: number
  nome: string
  codigoCliente: string
  organogramaPosicaoIdLider: string | null
  ativo: boolean
  dataCriacao?: string
}

export interface DepartamentoResponse {
  id: string
  orgId: number
  nome: string
  codigoCliente: string
  organogramaPosicaoIdLider: string | null
  ativo: boolean
  dataCriacao: string
}

export interface PerfilCorporativoPayload {
  orgId: number
  descricao: string
  permanenciaId?: string
  modeloTrabalhoId?: string
  profissionalLocalidadeId?: string
  empregoLinkdinId?: string
  experienciaLinkedinId?: string
  atribuicoes?: string
  ativo?: boolean
}

export interface PerfilCorporativoResponse {
  id: string
  orgId: number
  descricao: string
  permanenciaId: string
  modeloTrabalhoId: string
  profissionalLocalidadeId: string
  empregoLinkdinId: string
  experienciaLinkedinId: string
  atribuicoes: string
  ativo: boolean
}

export interface PosicaoPayload {
  orgId: number
  codigoCliente: string
  organogramaDepartamentoId: string | null
  perfilCorporativoId: string | null
  organogramaPosicaoIdSuperior: string | null
  ativo: boolean
  cLevel: boolean
  profissionalExterno: boolean
  dataCriacao?: string
}

export interface PosicaoResponse {
  id: string
  orgId: number
  codigoCliente: string
  organogramaDepartamentoId: string
  perfilCorporativoId: string
  organogramaPosicaoIdSuperior: string | null
  ativo: boolean
  cLevel: boolean
  profissionalExterno: boolean
  dataCriacao: string
}

export interface AlocacaoPayload {
  orgId: number
  codigoCliente: string
  organogramaPosicaoId: string
  codigoInternoColaborador: string
  dataInicio: string
  dataFim: string | null
  ativo: boolean
  dataCriacao?: string
}

export interface AlocacaoResponse {
  id: string
  orgId: number
  codigoCliente: string
  organogramaPosicaoId: string
  codigoInternoColaborador: string
  dataInicio: string
  dataFim: string | null
  ativo: boolean
  dataCriacao: string
}

// Alocação na resposta completa do organograma
export interface AlocacaoCompleta {
  id: string
  codigoInternoColaborador: string
  nomeColaborador: string
  dataInicio: string
  dataFim: string | null
  ativo: boolean
  dataCriacao: string
  dataAlteracao: string
}

// Resposta completa de posição com dados relacionados
export interface PosicaoCompleta {
  // Dados da Posição
  id: string
  orgId: number
  departamentoId: string | null
  departamentoNome: string | null
  codigoCliente: string
  perfilCorporativoId: string | null
  perfilCorporativoNome: string | null
  posicaoIdSuperior: string | null
  ativo: boolean
  dataCriacao: string
  dataAlteracao: string
  cLevel: boolean
  profissionalExterno: boolean

  // Array de alocações (pode estar vazio se posição estiver vaga)
  alocacoes: AlocacaoCompleta[]
}

export interface PosicaoCompletaResponse {
  orgId: number
  codigoCliente: string
  posicoes: PosicaoCompleta[]
}
