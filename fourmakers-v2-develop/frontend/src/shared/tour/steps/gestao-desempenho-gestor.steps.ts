import type { TourStep } from '../tour.types';

/**
 * Guide tour da rota /gestao-desempenho-gestor
 */
export const gestaoDesempenhoGestorSteps: TourStep[] = [
  {
    testId: 'gestao-desempenho-gestor-card-meus-colaboradores',
    title: 'Meus Colaboradores',
    content:
      'Aqui você visualiza todos os seus colaboradores diretos. Use os filtros e a busca para encontrar rapidamente quem precisa de atenção.',
    placement: 'right',
    action: { type: 'observe' },
  },
  {
    testId: 'card-meus-colaboradores-busca-input',
    title: 'Buscar colaborador',
    content:
      'Digite o nome do colaborador para filtrar a lista. A busca é feita em tempo real.',
    placement: 'bottom',
    action: { type: 'fill', value: 'Colaborador exemplo' },
  },
  {
    testId: 'card-meus-colaboradores-filtro-sem-pdi',
    title: 'Filtro: Sem PDI',
    content:
      'Clique aqui para visualizar apenas os colaboradores que ainda não possuem um PDI ativo.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'card-meus-colaboradores-filtro-sem-feedback',
    title: 'Filtro: Sem Feedback',
    content:
      'Filtra colaboradores que não receberam feedback recentemente. Ajuda a identificar quem priorizar.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'card-meus-colaboradores-filtro-sem-1-1',
    title: 'Filtro: Sem 1:1',
    content:
      'Exibe colaboradores sem reunião 1:1 registrada recentemente.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'card-meus-colaboradores-filtro-todos',
    title: 'Ver todos',
    content: 'Retorna à listagem completa, sem filtros aplicados.',
    placement: 'bottom',
    action: { type: 'click' },
  },
  {
    testId: 'gestao-desempenho-gestor-novo-feedback-button-{codColaborador}',
    title: 'Dar feedback',
    content:
      'Clique no ícone de feedback para registrar um feedback estruturado (continuar, começar, parar) para este colaborador.',
    placement: 'left',
    action: { type: 'click' },
    waitFor: 'gestao-desempenho-gestor-dialog-novo-feedback',
  },
  {
    testId: 'gestao-desempenho-gestor-dialog-novo-feedback',
    title: 'Dialog: Novo Feedback',
    content:
      'Preencha os três campos do feedback: o que o colaborador deve Continuar fazendo, o que deve Começar e o que deve Parar.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'gestao-desempenho-gestor-dialog-novo-feedback-salvar-button',
    title: 'Salvar feedback',
    content:
      'Após preencher os campos, clique em Salvar para registrar o feedback.',
    placement: 'top',
    action: { type: 'click' },
    skipInTests: true,
  },
  {
    testId: 'gestao-desempenho-gestor-dialog-novo-feedback-cancelar-button',
    title: 'Cancelar',
    content: 'Clique em Cancelar para fechar o dialog sem salvar.',
    placement: 'top',
    action: { type: 'click' },
  },
  {
    testId: 'gestao-desempenho-gestor-novo-1-1-button-{codColaborador}',
    title: 'Registrar reunião 1:1',
    content:
      'Clique aqui para registrar as anotações de uma reunião 1:1 realizada com este colaborador.',
    placement: 'left',
    action: { type: 'click' },
    waitFor: 'gestao-desempenho-gestor-dialog-novo-1-1',
  },
  {
    testId: 'gestao-desempenho-gestor-dialog-novo-1-1',
    title: 'Dialog: Novo Registro 1:1',
    content:
      'Informe a data da reunião, preencha as anotações e, se necessário, indique se existe um registro crítico.',
    placement: 'bottom',
    action: { type: 'observe' },
  },
  {
    testId: 'gestao-desempenho-gestor-dialog-novo-1-1-salvar-button',
    title: 'Salvar registro 1:1',
    content: 'Salva as anotações da reunião 1:1.',
    placement: 'top',
    action: { type: 'click' },
    skipInTests: true,
  },
];
