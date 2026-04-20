/**
 * Tipos do Dashboard Comercial (Maverick Cockpit).
 * Filtros, KPIs, séries de gráficos e linhas da tabela de evidência auditável.
 */

/** Filtros do header (Data início/fim, Cliente, Comercial) */
export interface FiltrosComercial {
  /** Data inicial do período (yyyy-MM-dd) ou null */
  dataInicio: string | null
  /** Data final do período (yyyy-MM-dd) ou null */
  dataFim: string | null
  codigoCliente: string | null
  /** Nome do cliente selecionado (para exibição no modal/drilldown). */
  nomeClienteSelecionado?: string | null
  tipoComercial: 'gestor-externo' | 'colaborador'
  codigoColaboradorAgendou: string | null
  codigoGestorExterno: string | null
  /** Nome do comercial selecionado - gestor ou colaborador (para exibição no modal/drilldown). */
  nomeComercialSelecionado?: string | null
}

/** KPIs da coluna Saúde do Radar de Relacionamento */
export interface KpiSaude {
  encontrosRealizados: number
  semInteracao: number
  acoesEmAtraso: number
}

/** KPIs da coluna Alcance do Radar de Relacionamento */
export interface KpiAlcance {
  clientesImpactados: number
  gestoresImpactados: number
  categoriasComInteracao: number
}

/** Ponto de série para gráfico de barras (Recharts: name + value) */
export interface SerieGrafico {
  name: string
  value: number
}

/** Linha da tabela de evidência auditável (Cliente e Gestor sempre em colunas separadas) */
export interface LinhaEvidencia {
  cliente: string
  gestor: string
  data: string
  [key: string]: string | number | undefined
}

/** Identificador do indicador para drilldown (KPI ou gráfico) */
export type IdIndicador =
  | 'encontros-realizados'
  | 'sem-interacao'
  | 'acoes-em-atraso'
  | 'clientes-impactados'
  | 'gestores-impactados'
  | 'categorias-com-interacao'
  | 'niveis-estrategicos'
  | 'dedicacao-por-objetivo'
  | 'foco-por-categoria'

/** Retorno da API GET /api/Social/EncontrosBigNumbers/ObterBigNumbers */
export interface EncontrosBigNumbersRetorno {
  /** Nome alternativo retornado pela API (reuniões realizadas) */
  reunioesRealizados?: number
  encontrosRealizados?: number
  /** Nome alternativo retornado pela API (reuniões sem interação) */
  reunioesSemInteracao?: number
  encontrosSemInteracao?: number
  acoesEmAtraso?: number
  clientesImpactados?: number
  gestoresImpactados?: number
  categoriasComInteracao?: number
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/ObterBigNumbersCategoria */
export interface BigNumbersCategoriaItem {
  categoriaId: number
  categoriaDescricao: string
  totalInteracoes: number
}

/** Gestor no detalhe de agenda realizada (resposta AgendaRealizadaDetalhe) */
export interface GestorClienteDetalhe {
  codigoGestorExterno: string
  nome: string
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/AgendaRealizadaDetalhe */
export interface AgendaRealizadaDetalhe {
  agendaId: number
  organizador: string
  codigoCliente: string
  nomeCliente: string
  titulo: string
  /** Data em que a agenda foi efetivamente agendada (campo retornado pelo endpoint). */
  dataAgendada: string
  gestores: GestorClienteDetalhe[]
}

/** Item do array "agendas" no retorno do endpoint ClientesImpactadosDetalhe */
export interface AgendaDoClienteDetalhe {
  agendaId: number
  titulo: string
  dataAgendada: string
  tipoAgendaId: number | null
  tipoAgendaDescricao: string | null
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/ClientesImpactadosDetalhe (agrupado por cliente) */
export interface ClienteImpactadoDetalhe {
  codigoCliente: string
  nomeCliente: string
  agendas: AgendaDoClienteDetalhe[]
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/AgendaSemInteracaoDetalhe */
export interface AgendaSemInteracaoDetalhe {
  agendaId: number
  titulo: string
  dataAgendada: string
  nomeCliente: string
  tipoAgendaId: number | null
  tipoAgendaDescricao: string | null
}

/** Item do array "agendas" no retorno do endpoint AcoesEmAtrasoDetalhe */
export interface AgendaComAcao {
  agendaId: number
  titulo: string
  dataAgendada: string
  codigoCliente: string
  nomeCliente: string
  tipoAgendaId: number | null
  tipoAgendaDescricao: string | null
  acaoId: number | null
  dataLimiteAcao: string | null
  descricaoAcao: string | null
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/AcoesEmAtrasoDetalhe (agrupado por status) */
export interface AcoesEmAtrasoDetalhe {
  statusAcaoId: number
  statusDescricao: string
  agendas: AgendaComAcao[]
}

/** Item do array "agendas" no retorno do endpoint CategoriaComInteracaoDetalhe */
export interface AgendaDaCategoria {
  agendaId: number
  titulo: string
  dataAgendada: string
  codigoCliente: string
  nomeCliente: string
  tipoAgendaId: number | null
  tipoAgendaDescricao: string | null
  subcategoriaId: number | null
  subcategoriaDescricao: string | null
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/CategoriaComInteracaoDetalhe (agrupado por categoria) */
export interface CategoriaComInteracaoDetalhe {
  categoriaId: number
  categoriaDescricao: string
  agendas: AgendaDaCategoria[]
}

/** Item do array "agendas" no retorno do endpoint GestoresImpactadosDetalhe */
export interface AgendaDoGestor {
  agendaId: number
  titulo: string
  dataAgendada: string
  codigoCliente: string
  nomeCliente: string
  tipoAgendaId: number | null
  tipoAgendaDescricao: string | null
}

/** Item do retorno da API GET /api/Social/EncontrosBigNumbers/GestoresImpactadosDetalhe (agrupado por gestor) */
export interface GestoresImpactadosDetalhe {
  codigoGestorExterno: string
  nomeGestor: string
  agendas: AgendaDoGestor[]
}

/** Dados passados ao DrilldownSheet (indicador, evidências, texto "Como calculamos") */
export interface DadosDrilldown {
  idIndicador: IdIndicador
  titulo: string
  comoCalculamos: string
  dadosEvidencia: LinhaEvidencia[]
}
