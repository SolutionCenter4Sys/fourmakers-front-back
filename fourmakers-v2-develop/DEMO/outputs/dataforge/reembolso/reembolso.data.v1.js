/**
 * [DataForge] Dados de teste — Módulo de Reembolso
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v1.md)
 * Dados realistas para preenchimento do formulário via Playwright
 */

// R-01 · [Positivo] Acesso à lista de reembolsos
export const dadosR01 = {
  perfil: 'colaborador',
  rota: '/reembolso',
  abaEsperada: 'Meus Reembolsos',
  periodoInicio: '01/07/2026',
  periodoFim: '11/08/2026',
};

// R-02 · [Negativo] Abas restritas ocultas sem permissão
export const dadosR02 = {
  perfil: 'colaborador_sem_gestao',
  rota: '/reembolso',
  abasProibidas: ['Gestão ADM', 'Aprovações'],
  abaVisivel: 'Meus Reembolsos',
};

// R-03 · [Negativo] Lista vazia com mensagem orientativa
export const dadosR03 = {
  perfil: 'colaborador_sem_solicitacoes',
  periodoInicio: '01/01/2020',
  periodoFim: '02/01/2020',
  emptyMessage: 'Nenhum reembolso encontrado',
};

// R-04 · [Positivo] Navegação para nova solicitação
export const dadosR04 = {
  botao: 'Solicitar Reembolso',
  rotaEsperada: '/inserir-reembolso',
  rotaInvalidaDemo: '/inserir-reembolso-invalido',
};

// I-08 · [Positivo] Inclusão de despesa no carrinho
export const dadosI08 = {
  objetivo: 'Workshop de arquitetura com cliente Braskem',
  destino: 'São Paulo - SP',
  dataInicio: '04/08/2026',
  dataFim: '05/08/2026',
  categoria: 'Alimentação',
  dataDespesa: '04/08/2026',
  valor: '87,50',
  descricao: 'Almoço de alinhamento técnico com time de integração SAP',
  exigirComprovante: true,
  comprovanteNome: 'nf-almoco-braskem-sp.pdf',
  cep: '04538-132',
  telefone: '(11) 98473-2651',
};

// I-09 · [Negativo] Bloqueio por campos obrigatórios vazios
export const dadosI09 = {
  objetivo: '',
  destino: '',
  dataInicio: '',
  dataFim: '',
  categoria: '',
  dataDespesa: '',
  valor: '',
  descricao: '',
  toastEsperado: 'Campos obrigatórios',
};

// I-10 · [Negativo] Comprovante obrigatório ausente
export const dadosI10 = {
  objetivo: 'Visita técnica à planta Suzano',
  destino: 'Limeira - SP',
  dataInicio: '06/08/2026',
  dataFim: '06/08/2026',
  categoria: 'Transporte',
  dataDespesa: '06/08/2026',
  valor: '142,90',
  descricao: 'Deslocamento aeroporto–planta industrial sem anexo fiscal',
  exigirComprovante: true,
  anexos: [],
};

// I-13 · [Positivo] Envio da solicitação com itens
export const dadosI13 = {
  objetivo: 'Imersão comercial conta Votorantim Cimentos',
  destino: 'Curitiba - PR',
  dataInicio: '07/08/2026',
  dataFim: '08/08/2026',
  categoria: 'Hospedagem',
  dataDespesa: '07/08/2026',
  valor: '612,80',
  descricao: 'Diária hotel próximo ao centro de operações do cliente',
  exigirComprovante: true,
  comprovanteNome: 'nf-hotel-curitiba.pdf',
  cep: '80010-030',
  telefone: '(41) 98765-3021',
  rotaPosEnvio: '/reembolso',
};

// AP-01 · [Positivo] Aprovação de solicitações selecionadas
export const dadosAP01 = {
  perfil: 'aprovador',
  rota: '/aprovar-reembolso',
  acao: 'aprovar',
  observacao: 'Despesa coerente com política de viagem corporativa',
};

// RP-01 · [Regressivo] Visualização das regras de verba
export const dadosRP01 = {
  perfil: 'gestor',
  rota: '/reembolso-parametros',
  aba: 'Regras',
  verbaExemplo: 'Alimentação',
};
