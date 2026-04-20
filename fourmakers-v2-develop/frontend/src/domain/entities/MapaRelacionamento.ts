// Entidades de domínio para Mapa de Relacionamento
// (Anteriormente Organograma)
// Baseado na estrutura do PROTOTIPO (types.ts) e seguindo Clean Architecture

export interface NoMapaRelacionamento {
  id: string
  profileId: string | null // ID do perfil de atuação (gestorExternoPerfilId)
  employeeId: string | null // ID do colaborador (codigoInternoColaborador) ou 'vacant'
  profileName?: string // Nome do perfil de atuação (gestorExternoPerfilNome)
  employeeName?: string // Nome do colaborador
  employeeEmail?: string // Email do colaborador
  employeeAvatarUrl?: string // URL do avatar do colaborador
  managerCode?: string // Código do gestor externo (codGestorExterno)
  departmentId?: string // ID do departamento (cod)
  departmentName?: string // Nome do departamento (departamento)
  isCLevel?: boolean // Indica se é perfil C-Level
  isExternal?: boolean // Indica se é profissional externo
  wasConnected?: boolean // Indica se já foi conectado a um colaborador anteriormente
  // IDs de backend para integração com APIs
  posicaoId?: string // ID da posição no backend (organogramaPosicaoId)
  alocacaoId?: string // ID da alocação no backend (alocacaoId)
  departamentoBackendId?: string // ID do departamento no backend (organogramaDepartamentoId)
  perfilCorporativoBackendId?: string // ID do perfil corporativo no backend (perfilCorporativoId)
  children: NoMapaRelacionamento[]
}

export interface ClienteMapaRelacionamento {
  id: string
  codigoCliente: string
  nomeCliente: string
  qtdAlocados: number
  qtdGestoresSemPerfil: number
  qtdGestores: number
}

export interface GestorExterno {
  codigoInternoColaborador: string
  nome: string
  email: string
  codigoCliente: string
  perfilLinkedin: string
  codGestorExterno: string
}

export interface PerfilExterno {
  nomeGestorExterno: string
  codGestorExterno: string
  gestorExternoPerfilId: string
  gestorExternoPerfilNome: string
  codigoInternoColaborador: string
}

export interface DepartamentoMapaRelacionamento extends Record<string, unknown> {
  cod: string
  departamento: string
}

// API returns array directly, not wrapped in { retorno: [] }
export type ClientesMapaRelacionamentoResponse = ClienteMapaRelacionamento[]

export interface GestoresExternosResponse {
  retorno: GestorExterno[]
}

export interface PerfisExternosResponse {
  retorno: PerfilExterno[]
}

export interface DepartamentosMapaRelacionamentoResponse {
  retorno: DepartamentoMapaRelacionamento[]
}

export interface SalvarMapaRelacionamentoPayload {
  codigoCliente: string
  nomeCliente: string
  estrutura: NoMapaRelacionamento
}

// Estrutura de armazenamento local
// Nota: No PROTOTIPO, a chave de armazenamento é: `fourmakers_mapa_rel_data_${codigoCliente}`
// E o objeto salvo inclui diretamente os campos do nó raiz + clientName
export interface DadosMapaRelacionamentoArmazenados {
  id: string // ID do nó raiz
  codigoCliente?: string // Mantido para compatibilidade
  clientName?: string // Nome do cliente (usado no PROTOTIPO)
  nomeCliente?: string // Mantido para compatibilidade
  estrutura?: NoMapaRelacionamento // Estrutura separada (padrão documentado)
  dataUltimaAtualizacao?: string
  // Campos do nó raiz também podem estar diretamente no objeto (padrão PROTOTIPO)
  profileId?: string | null
  employeeId?: string | null
  profileName?: string
  employeeName?: string
  employeeEmail?: string
  employeeAvatarUrl?: string
  managerCode?: string
  departmentId?: string
  departmentName?: string
  isCLevel?: boolean
  isExternal?: boolean
  wasConnected?: boolean
  children?: NoMapaRelacionamento[]
}
