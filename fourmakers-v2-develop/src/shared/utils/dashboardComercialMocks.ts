/**
 * Dados mockados do Dashboard Comercial (Maverick Cockpit).
 * KPIs, séries de gráficos e evidências por indicador.
 *
 * @deprecated Este arquivo está em descontinuação. A lógica dos filtros foi movida
 * para @shared/constants/filtrosDashboardComercial. Os demais mocks devem ser
 * gradualmente substituídos por dados reais ou movidos para arquivos específicos.
 */

import type { SerieGrafico, LinhaEvidencia, IdIndicador } from '@shared/types/dashboardComercialTypes'

/** Valores mockados dos KPIs Saúde */
export const MOCK_KPI_SAUDE = {
  encontrosRealizados: 42,
  semInteracao: 5,
  acoesEmAtraso: 3,
} as const

/** Valores mockados dos KPIs Alcance */
export const MOCK_KPI_ALCANCE = {
  clientesImpactados: 18,
  gestoresImpactados: 24,
  categoriasComInteracao: 0,
} as const

/** Dados mockados: Níveis Estratégicos Acessados (gráfico de barras) */
export const MOCK_GRAFICO_NIVEIS_ESTRATEGICOS: SerieGrafico[] = [
  { name: 'C-Level', value: 12 },
  { name: 'Diretoria', value: 28 },
  { name: 'Gerência', value: 45 },
  { name: 'Coordenação', value: 32 },
  { name: 'Operacional', value: 18 },
]

/** Dados mockados: Dedicação por Objetivo (gráfico de barras) */
export const MOCK_GRAFICO_DEDICACAO_OBJETIVO: SerieGrafico[] = [
  { name: 'Expansão', value: 35 },
  { name: 'Retenção', value: 22 },
  { name: 'Onboarding', value: 18 },
  { name: 'Estratégico', value: 15 },
  { name: 'Outros', value: 10 },
]

/** Dados mockados: Foco por Categoria (gráfico de barras) */
export const MOCK_GRAFICO_FOCO_CATEGORIA: SerieGrafico[] = [
  { name: 'Relacionamento', value: 40 },
  { name: 'Produto', value: 25 },
  { name: 'Operacional', value: 20 },
  { name: 'Comercial', value: 15 },
]

/** Linhas de evidência genéricas (ordenadas por data decrescente) para reutilizar nos indicadores */
const linhasEvidenciaBase: LinhaEvidencia[] = [
  { cliente: 'Cliente A', gestor: 'Maria Silva', data: '2025-02-24' },
  { cliente: 'Cliente B', gestor: 'João Santos', data: '2025-02-23' },
  { cliente: 'Cliente A', gestor: 'Ana Costa', data: '2025-02-22' },
  { cliente: 'Cliente C', gestor: 'Pedro Oliveira', data: '2025-02-21' },
  { cliente: 'Cliente B', gestor: 'Maria Silva', data: '2025-02-20' },
  { cliente: 'Cliente A', gestor: 'Carlos Lima', data: '2025-02-19' },
  { cliente: 'Cliente C', gestor: 'João Santos', data: '2025-02-18' },
  { cliente: 'Cliente B', gestor: 'Ana Costa', data: '2025-02-17' },
  { cliente: 'Cliente A', gestor: 'Pedro Oliveira', data: '2025-02-16' },
  { cliente: 'Cliente C', gestor: 'Maria Silva', data: '2025-02-15' },
]

/** Textos "Como calculamos" por indicador */
export const COMO_CALCULAMOS: Record<IdIndicador, string> = {
  'encontros-realizados':
    'Soma de todos os encontros agendados e realizados no período, considerando os filtros de Cliente e Comercial.',
  'sem-interacao':
    'Clientes ou gestores que não tiveram nenhuma interação registrada no período selecionado.',
  'acoes-em-atraso':
    'Ações com data prevista anterior à data atual e ainda não concluídas.',
  'clientes-impactados':
    'Quantidade única de clientes que receberam pelo menos uma interação no período.',
  'gestores-impactados':
    'Quantidade única de gestores que participaram de pelo menos um encontro no período.',
  'categorias-com-interacao':
    'Quantidade de categorias que possuem pelo menos uma interação registrada no período.',
  'niveis-estrategicos':
    'Distribuição de encontros por nível hierárquico acessado (C-Level, Diretoria, Gerência, etc.).',
  'dedicacao-por-objetivo':
    'Distribuição do tempo e ações por objetivo comercial (Expansão, Retenção, Onboarding, etc.).',
  'foco-por-categoria':
    'Distribuição das interações por categoria de foco (Relacionamento, Produto, Operacional, Comercial).',
}

/** Evidências por indicador (ordenadas por data decrescente) */
export const EVIDENCIAS_POR_INDICADOR: Record<IdIndicador, LinhaEvidencia[]> = {
  'encontros-realizados': linhasEvidenciaBase,
  'sem-interacao': linhasEvidenciaBase.slice(0, 6),
  'acoes-em-atraso': linhasEvidenciaBase.slice(0, 4),
  'clientes-impactados': linhasEvidenciaBase,
  'gestores-impactados': linhasEvidenciaBase,
  'categorias-com-interacao': linhasEvidenciaBase.slice(0, 8),
  'niveis-estrategicos': linhasEvidenciaBase,
  'dedicacao-por-objetivo': linhasEvidenciaBase,
  'foco-por-categoria': linhasEvidenciaBase,
}
