export interface Org {
  id: number
  descricao: string
  subdominio: string | null
  dominio_email: string | null
  prioridade: number | null
}

export interface DepartamentoColaborador {
  cod: string
  departamento: string
}

export interface DiretoriaColaborador {
  cod: string
  diretoria: string
}

export interface Gestor {
  codigoProfissional: string
  nomeGestor: string
}

export interface CargoColaborador {
  cargo: string
  codigoCargo: string
}

export interface Colaborador {
  codColaborador: string
  cpf: string
  /** Id do usuário no sistema (retornado por ListaColaboradoresOrgId); usado em Permissionamento para Adicionar/Remover do grupo. */
  usuarioId?: number
  ativo: boolean
  modeloContratacao: string
  empresaRelacionada: string
  modeloTrabalho: string
  diasPorSemana: number | null
  valorHora: number
  custoHora: number
  baseHoraMes: number
  primeiroAcessoRealizado: boolean
  dataInativacao: string | null
  email: string
  nome: string
  org: Org
  departamentoColaborador: DepartamentoColaborador
  diretoriaColaborador: DiretoriaColaborador
  gestor: Gestor
  imagemPath: string | null
  dataAdmissao: string
  status: string | null
  documentoColaborador: string
  contatoPrincipalDDI: string
  contatoPrincipal: string
  considerarBancoDeTalentos: boolean
  considerarVisualizacaoAderencia: boolean
  cargoColaborador?: CargoColaborador
}

export interface ColaboradoresResponse {
  retorno: Colaborador[]
}

export interface ColaboradoresParams {
  cursor: number
  limite: number
  nomeOuEmail: string
  org: number
  codExterno?: string
  fourtalents?: boolean
  /** Quando informado, filtra por cliente; não enviar quando não houver cliente selecionado. */
  codigoCliente?: string | null
}

