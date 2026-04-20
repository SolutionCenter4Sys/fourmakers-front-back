import type { TourStep } from '../tour.types';

/**
 * Guide tour da rota /gestao-desempenho-colaborador
 *
 * Cada step contém:
 * - testId  → data-testid do elemento na página
 * - content → descrição exibida no tour e reutilizável como cenário de teste
 * - action  → ação que o QA executaria neste step (útil para E2E)
 */
export const gestaoDesempenhoColaboradorSteps: TourStep[] = [
  {
    testId: 'gestao-desempenho-colaborador-page-tabs',
    title: 'Navegação por abas',
    content:
      'Esta é a área principal de gestão de desempenho. Use as abas para alternar entre Feedbacks, Registros 1:1 e PDIs.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'gestao-desempenho-colaborador-page-feedbacks-tab',
    title: 'Aba de Feedbacks',
    content:
      'A aba Feedbacks exibe todos os feedbacks recebidos do gestor. Clique aqui para visualizá-los.',
    placement: 'bottom',
    action: { type: 'click' },
    waitFor: 'gestao-desempenho-colaborador-page-tabs',
  },
  {
    testId: 'gestao-desempenho-colaborador-page-registros-1-1-tab',
    title: 'Aba Registros 1:1',
    content:
      'Visualize os registros das reuniões 1:1 com seu gestor. Aqui ficam as anotações e pautas discutidas.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'gestao-desempenho-colaborador-page-adicionar-pauta-button',
    title: 'Sugerir pauta',
    content:
      'Clique aqui para sugerir um tema para a próxima reunião 1:1 com seu gestor.',
    placement: 'bottom',
    action: { type: 'click' },
    waitFor: 'gestao-desempenho-colaborador-page-pauta-modal',
  },
  {
    testId: 'gestao-desempenho-colaborador-page-pauta-modal',
    title: 'Modal de sugestão de pauta',
    content:
      'Digite o tema que deseja discutir na reunião 1:1 e clique em Salvar para registrar.',
    placement: 'bottom',
    action: { type: 'observe' },
    skipInTests: true,
  },
  {
    testId: 'gestao-desempenho-colaborador-page-pauta-textarea',
    title: 'Campo de pauta',
    content: 'Escreva o tema ou assunto que deseja abordar na próxima reunião.',
    placement: 'top',
    action: { type: 'fill', value: 'Tema de exemplo para pauta' },
  },
  {
    testId: 'gestao-desempenho-colaborador-page-pauta-modal-salvar-button',
    title: 'Salvar pauta',
    content: 'Confirme e salve a sugestão de pauta para o gestor visualizar.',
    placement: 'top',
    action: { type: 'click' },
  },
  {
    testId: 'gestao-desempenho-colaborador-page-pdi-tab',
    title: 'Aba PDI',
    content:
      'Visualize seu Plano de Desenvolvimento Individual (PDI). Aqui você acompanha os PDIs ativos e o histórico.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'gestao-desempenho-colaborador-page-pdi-ativos-header',
    title: 'PDIs Ativos',
    content:
      'Esta seção lista os PDIs em andamento. Você pode expandir cada PDI para ver as ações vinculadas.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'gestao-desempenho-colaborador-page-criar-pdi-button',
    title: 'Criar novo PDI',
    content:
      'Clique aqui para iniciar a criação de um novo Plano de Desenvolvimento Individual.',
    placement: 'bottom',
    action: { type: 'click' },
    skipInTests: true,
  },
  {
    testId: 'gestao-desempenho-colaborador-page-pdi-historico-header',
    title: 'Histórico de PDIs',
    content:
      'Consulte os PDIs concluídos ou arquivados. Útil para acompanhar a evolução ao longo do tempo.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
];
