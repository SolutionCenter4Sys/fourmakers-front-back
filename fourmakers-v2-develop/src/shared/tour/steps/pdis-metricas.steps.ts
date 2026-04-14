import type { TourStep } from '../tour.types';

/**
 * Guide tour da rota /pdis-metricas
 */
export const pdisMetricasSteps: TourStep[] = [
  {
    testId: 'pdis-metricas-page-atualizar-button',
    title: 'Atualizar dados',
    content:
      'Clique aqui para recarregar os dados de PDIs com os filtros atualmente selecionados.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtros-card',
    title: 'Painel de Filtros',
    content:
      'Use estes filtros para segmentar os PDIs por período, unidade, colaborador e status.',
    placement: 'right',
    action: { type: 'observe' },
  },
  {
    testId: 'pdis-metricas-page-filtro-data-inicio',
    title: 'Data de início',
    content:
      'Selecione a data inicial do período que deseja consultar.',
    placement: 'bottom',
    action: { type: 'fill', value: '2024-01-01' },
  },
  {
    testId: 'pdis-metricas-page-filtro-data-fim',
    title: 'Data de fim',
    content:
      'Selecione a data final do período de consulta.',
    placement: 'bottom',
    action: { type: 'fill', value: '2024-12-31' },
  },
  {
    testId: 'pdis-metricas-page-filtro-unidade-trigger',
    title: 'Filtro por unidade',
    content:
      'Filtre os PDIs por unidade de negócio. Abra este select para ver as opções disponíveis.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtro-colaborador-trigger',
    title: 'Filtro por colaborador',
    content:
      'Selecione um colaborador específico para ver apenas os PDIs dele.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtro-checkbox-todos',
    title: 'Filtro: Todos os status',
    content:
      'Selecione esta opção para exibir PDIs de qualquer status.',
    placement: 'right',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtro-checkbox-em-andamento',
    title: 'Filtro: Em andamento',
    content:
      'Exibe apenas os PDIs que estão em andamento.',
    placement: 'right',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtro-checkbox-nao-iniciado',
    title: 'Filtro: Não iniciado',
    content:
      'Exibe PDIs criados mas ainda não iniciados.',
    placement: 'right',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtro-checkbox-finalizados',
    title: 'Filtro: Finalizados',
    content:
      'Exibe apenas os PDIs já concluídos.',
    placement: 'right',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-filtros-buscar-button',
    title: 'Aplicar filtros',
    content:
      'Após configurar os filtros, clique aqui para buscar os resultados.',
    placement: 'left',
    action: { type: 'click' },
    waitFor: 'pdis-metricas-page-ativos',
  },
  {
    testId: 'pdis-metricas-page-ativos-header',
    title: 'PDIs Ativos',
    content:
      'Tabela com todos os PDIs ativos. Clique no ícone de detalhe de uma linha para ver as informações completas.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'pdis-metricas-page-ativos-row-detalhe-button-{pdiId}',
    title: 'Ver detalhes do PDI',
    content:
      'Clique neste botão para abrir o modal com os detalhes completos do PDI selecionado.',
    placement: 'left',
    action: { type: 'click' },
    waitFor: 'pdis-metricas-page-detalhe-pdi-modal',
  },
  {
    testId: 'pdis-metricas-page-detalhe-pdi-modal',
    title: 'Detalhe do PDI',
    content:
      'Modal com as informações detalhadas do PDI: descrição, ações, metas e progresso.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'pdis-metricas-page-detalhe-pdi-modal-fechar-button',
    title: 'Fechar modal',
    content: 'Fecha o modal de detalhe e retorna para a listagem.',
    placement: 'top',
    action: { type: 'click' },
  },
  {
    testId: 'pdis-metricas-page-historicos-header',
    title: 'Histórico de PDIs',
    content:
      'Tabela com PDIs finalizados ou arquivados. Útil para analisar o histórico de desenvolvimento da equipe.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'pdis-metricas-page-filtros-limpar-button',
    title: 'Limpar filtros',
    content:
      'Clique para redefinir todos os filtros para seus valores padrão.',
    placement: 'left',
    action: { type: 'click' },
  },
];
