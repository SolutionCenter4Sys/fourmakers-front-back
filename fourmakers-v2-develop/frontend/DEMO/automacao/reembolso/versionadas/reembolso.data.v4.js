/**
 * [DataForge] Dados de teste — Módulo de Reembolso (Gestão ADM)
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v4.md)
 *
 * Dados realistas brasileiros: cidades, clientes e valores em reais.
 * Cada constante é nomeada conforme o ID do cenário BDD.
 */

// ═══════════════════════════════════════════════════════
//  GESTÃO ADM — Reembolso.tsx / GestaoAdmTab.tsx
// ═══════════════════════════════════════════════════════

// G-01 · [Positivo] Filtro por período, cliente e status
export const dadosG01 = {
  dataInicio: '01/04/2026',
  dataFim: '23/04/2026',
  codigoCliente: 'CLI-2741',
  codigoProjeto: 'PRJ-ARQ-8832',
  status: 'aprovado',
  descricao: 'Recorte de abril/2026 para cliente Nova Paulista Energia — status Aprovado',
};

// G-02 · [Positivo] Carregamento de projetos por cliente
export const dadosG02 = {
  clienteSelecionado: 'CLI-2741',
  projetosEsperados: ['PRJ-ARQ-8832', 'PRJ-AZUL-4410'],
  projetoLimpoQuandoClienteTodos: true,
  descricao: 'Troca de cliente deve habilitar projetos relacionados e limpar seleção anterior',
};

// G-03 · [Positivo] Busca textual de solicitações
export const dadosG03 = {
  termoBusca: 'Patrícia Leal',
  descricao: 'Filtrar colaboradora pelo nome; paginação deve voltar para a primeira página',
};

// G-04 · [Positivo] Visão de indicadores da gestão
export const dadosG04 = {
  cardsEsperados: ['Solicitações enviadas', 'Aprovadas', 'Pagas'],
  valoresExemplo: ['R$ 18.750,00', 'R$ 9.430,00', 'R$ 6.210,00'],
  descricao: 'Cards exibem totais consolidados com formatação monetária brasileira',
};

// G-05 · [Positivo] Distribuição de status por colaborador
export const dadosG05 = {
  colaborador: 'Patrícia Leal',
  statusEsperados: [
    { status: 'Em aprovação', quantidade: 1 },
    { status: 'Aprovado', quantidade: 2 },
    { status: 'Pago', quantidade: 1 },
  ],
  descricao: 'Badges devem mostrar cada status com contador para a colaboradora selecionada',
};

// G-06 · [Positivo] Consulta detalhada de uma solicitação
export const dadosG06 = {
  colaborador: 'Paulo Mota',
  objetivo: 'Auditoria de despesas do trimestre',
  destino: 'Curitiba - PR',
  periodo: '05/04/2026 a 09/04/2026',
  clienteProjeto: 'Ecovias Sul / PRJ-ECV-1198',
  valorTotal: '12.850,00',
  itensEsperados: [
    { categoria: 'Alimentação', valorSolicitado: '1.320,00', valorAprovado: '1.150,00' },
    { categoria: 'Transporte', valorSolicitado: '780,00', valorAprovado: '780,00' },
  ],
  descricao: 'Modal deve exibir cabeçalho completo e itens com valores aprovados',
};

// G-07 · [Positivo] Visualização de comprovantes anexados
export const dadosG07 = {
  colaborador: 'Paulo Mota',
  documentosEsperados: [
    { id: 9821, tipo: 'Nota Fiscal', nome: 'nf-viagem-curitiba.pdf' },
    { id: 9822, tipo: 'Recibo', nome: 'recibo-transporte-curitiba.jpg' },
  ],
  descricao: 'Abrir modal de Documentos deve listar todos os comprovantes com opção de download',
};

// G-08 · [Positivo] Habilitação do modo Baixa
export const dadosG08 = {
  colaborador: 'Paulo Mota',
  itensAprovadosIds: [7812, 7813],
  modoBaixaHabilitado: true,
  descricao: 'Ao clicar em Baixar, apenas itens com status Aprovado ficam selecionáveis',
};

// G-09 · [Positivo] Seleção em massa de itens aprovados
export const dadosG09 = {
  colaborador: 'Paulo Mota',
  itensAprovadosIds: [7812, 7813],
  selecionarTodos: true,
  quantidadeEsperada: 2,
  descricao: 'Checkbox Selecionar todos marca todos os aprovados e atualiza contador',
};

// G-10 · [Positivo] Confirmação de baixas com sucesso
export const dadosG10 = {
  colaborador: 'Paulo Mota',
  idsParaBaixa: [7812, 7813],
  toastSucesso: 'Baixas confirmadas',
  statusPosBaixa: 'Pago',
  descricao: 'Confirmação deve fechar modais, limpar seleção e recarregar dados',
};

// G-11 · [Positivo] Paginação e tamanho da página
export const dadosG11 = {
  itemsPerPageInicial: 10,
  itemsPerPageNovo: 20,
  paginaDestino: 2,
  descricao: 'Alterar tamanho da página e navegar mantendo consistência da listagem',
};

// G-12 · [Negativo] Busca sem correspondência
export const dadosG12 = {
  termoBusca: 'ZZ-FORA-ESCOPO-404',
  mensagemEsperada: 'Nenhum colaborador encontrado',
  descricao: 'Busca inexistente deve limpar tabela e ocultar paginação',
};

// G-13 · [Negativo] Solicitação sem documentos anexados
export const dadosG13 = {
  colaborador: 'Marina Duarte',
  documentosEsperados: 0,
  mensagemModal: 'Nenhum documento disponível',
  descricao: 'Item sem anexos deve exibir apenas traço na coluna e mensagem no modal',
};

// G-14 · [Negativo] Ausência de itens aprovados para baixa
export const dadosG14 = {
  colaborador: 'Camila Freitas',
  statusItens: ['Em aprovação', 'Revisão financeira'],
  mostrarBotaoBaixar: false,
  descricao: 'Solicitação sem itens aprovados não deve oferecer ação de Baixar',
};

// G-15 · [Regressivo] Limpeza completa dos filtros
export const dadosG15 = {
  dataInicio: '01/04/2026',
  dataFim: '15/04/2026',
  codigoCliente: 'CLI-1980',
  codigoProjeto: 'PRJ-VERT-5520',
  status: 'pago',
  botaoLimpar: 'Limpar',
  descricao: 'Após aplicar filtros, acionar Limpar deve resetar todos os campos',
};

// G-16 · [Regressivo] Reset ao fechar o modal de detalhes
export const dadosG16 = {
  colaborador: 'Paulo Mota',
  limparSelecaoAoFechar: true,
  modoBaixaDesativado: true,
  descricao: 'Fechar modal deve descartar itens selecionados e sair do modo de baixa',
};

// G-17 · [Regressivo] Busca reinicia paginação
export const dadosG17 = {
  termoInicial: 'consultoria',
  termoNovo: 'auditoria energética',
  paginaReiniciada: 1,
  descricao: 'Ao trocar o termo de busca, a paginação volta para a página 1',
};
