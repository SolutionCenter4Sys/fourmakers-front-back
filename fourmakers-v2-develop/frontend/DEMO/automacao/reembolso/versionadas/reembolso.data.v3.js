/**
 * [DataForge] Dados de teste — Módulo de Reembolso
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v3.md)
 *
 * Dados realistas brasileiros: CPF/CNPJ válidos, cidades reais, valores em reais,
 * datas no formato dd/MM/yyyy. Cada constante é nomeada conforme o ID do cenário BDD.
 */

// ═══════════════════════════════════════════════════════
//  DASHBOARD — Reembolso.tsx
// ═══════════════════════════════════════════════════════

// R-01 · [Positivo] Visão padrão do módulo
export const dadosR01 = {
  descricao: 'Colaborador comum — deve ver apenas aba Meus Reembolsos',
  abaEsperada: 'Meus Reembolsos',
  abasRestritas: ['Gestão ADM', 'Aprovações'],
};

// R-02 · [Positivo] Visão do gestor no dashboard
export const dadosR02 = {
  descricao: 'Gestor — aba Gestão ADM visível ao lado de Meus Reembolsos',
  abaEsperada: 'Gestão ADM',
  urlAba: '/reembolso?tab=gestaoadm',
};

// R-03 · [Positivo] Visão do aprovador no dashboard
export const dadosR03 = {
  descricao: 'Aprovador — aba Aprovações visível ao lado de Meus Reembolsos',
  abaEsperada: 'Aprovações',
  urlAba: '/reembolso?tab=aprovacoes',
};

// R-04 · [Positivo] Indicadores de reembolsos
export const dadosR04 = {
  descricao: 'Big numbers exibidos com formatação de moeda brasileira',
  formatosEsperados: ['R$', ','],
};

// R-05 · [Positivo] Filtragem por período
export const dadosR05 = {
  dataInicio: '01/04/2026',
  dataFim: '23/04/2026',
  descricao: 'Filtrar reembolsos do início de abril/2026 até a data da demo',
};

// R-06 · [Positivo] Busca textual de reembolsos
export const dadosR06 = {
  termoBusca: 'Votorantim',
  descricao: 'Buscar por cliente Votorantim na tabela de reembolsos',
};

// R-07 · [Positivo] Consulta de detalhes de um reembolso
export const dadosR07 = {
  descricao: 'Abrir modal de detalhes da primeira linha da tabela',
  tituloModalEsperado: 'Detalhes da solicitação de reembolso',
};

// R-08 · [Positivo] Download de documentos anexados
export const dadosR08 = {
  descricao: 'Abrir modal de documentos a partir de um item com comprovante',
  tituloModalEsperado: 'Documentos',
};

// R-09 · [Positivo] Início de nova solicitação
export const dadosR09 = {
  urlDestino: '/inserir-reembolso',
  botao: 'Solicitar Reembolso',
};

// R-10 · [Negativo] Acesso indevido à Gestão ADM
export const dadosR10 = {
  urlDireta: '/reembolso?tab=gestaoadm',
  abaEsperada: 'Meus Reembolsos',
  descricao: 'URL forçada sem permissão de gestor deve redirecionar',
};

// R-11 · [Negativo] Acesso indevido às Aprovações
export const dadosR11 = {
  urlDireta: '/reembolso?tab=aprovacoes',
  abaEsperada: 'Meus Reembolsos',
  descricao: 'URL forçada sem permissão de aprovador deve redirecionar',
};

// R-12 · [Negativo] Busca sem resultados
export const dadosR12 = {
  termoBusca: 'ZZXPTO404_INEXISTENTE',
  mensagemEsperada: 'Nenhum reembolso encontrado',
};

// R-13 · [Regressivo] Limpeza dos filtros de data
export const dadosR13 = {
  dataInicio: '01/04/2026',
  dataFim: '23/04/2026',
  descricao: 'Aplicar filtros e acionar Limpar',
  botaoLimpar: 'Limpar',
};

// R-14 · [Regressivo] Persistência da aba na URL
export const dadosR14 = {
  abaAlvo: 'gestao-adm',
  urlEsperada: 'tab=gestaoadm',
  descricao: 'Alternar aba e validar URL',
};

// R-15 · [Regressivo] Paginação da tabela
export const dadosR15 = {
  itemsPerPageInicial: 10,
  paginaDestino: 2,
  descricao: 'Navegar entre páginas da tabela',
};

// ═══════════════════════════════════════════════════════
//  INSERIR REEMBOLSO — InserirReembolso.tsx
// ═══════════════════════════════════════════════════════

// I-01 · [Positivo] Adição de item com dados completos
export const dadosI01 = {
  objetivo: 'Reunião estratégica com diretoria do Santander',
  destino: 'São Paulo - SP',
  dataInicio: '15/04/2026',
  dataFim: '17/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '15/04/2026',
  valor: '9480',
  descricao: 'Jantar de alinhamento com stakeholders do projeto Core Banking',
};

// I-02 · [Positivo] Preenchimento automático por OCR
export const dadosI02 = {
  objetivo: 'Workshop técnico de integração com parceiro',
  destino: 'Campinas - SP',
  dataInicio: '11/04/2026',
  categoria: 'Alimentação',
  nomeComprovante: 'nota-fiscal-restaurante.pdf',
  descricao: 'Análise OCR deve preencher data da despesa e valor automaticamente',
};

// I-03 · [Positivo] Carregamento dinâmico de categorias
export const dadosI03 = {
  objetivo: 'Reunião de kickoff do projeto ERP Financeiro',
  destino: 'Rio de Janeiro - RJ',
  dataInicio: '13/04/2026',
  descricao: 'Selecionar projeto e validar que categorias são carregadas no combo',
};

// I-04 · [Positivo] Envio do carrinho com sucesso
export const dadosI04 = {
  objetivo: 'Visita técnica ao data center do cliente Itaú',
  destino: 'Osasco - SP',
  dataInicio: '08/04/2026',
  dataFim: '10/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '08/04/2026',
  valor: '7150',
  descricao: 'Jantar com equipe de infraestrutura durante janela de manutenção',
  mensagemSucessoEsperada: 'Solicitações enviadas com sucesso',
};

// I-05 · [Positivo] Edição de item no carrinho
export const dadosI05 = {
  objetivo: 'Treinamento técnico de squad em sede do cliente',
  destino: 'Florianópolis - SC',
  dataInicio: '18/04/2026',
  dataFim: '20/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '18/04/2026',
  valor: '5890',
  descricao: 'Item adicionado para em seguida ser editado',
  valorAtualizado: '6720',
  descricaoAtualizada: 'Item atualizado após edição no carrinho',
  rotuloBotaoEdicao: 'Atualizar Item',
};

// I-06 · [Positivo] Remoção de item do carrinho
export const dadosI06 = {
  objetivo: 'Deslocamento para cerimônia de entrega de release',
  destino: 'Belo Horizonte - MG',
  dataInicio: '23/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '23/04/2026',
  valor: '4380',
  descricao: 'Adicionar dois itens e remover um para validar recálculo do total',
};

// I-07 · [Positivo] Cálculo por quantidade × valor
export const dadosI07 = {
  objetivo: 'Deslocamento para treinamento in-company',
  destino: 'Barueri - SP',
  dataInicio: '22/04/2026',
  dataFim: '23/04/2026',
  categoria: 'Quilometragem',
  dataDespesa: '22/04/2026',
  quantidade: '48',
  descricao: 'Percurso ida e volta sede até filial Alphaville',
};

// I-08 · [Negativo] Submissão com campos obrigatórios vazios
export const dadosI08 = {
  objetivo: '',
  destino: '',
  dataInicio: '',
  categoria: '',
  dataDespesa: '',
  valor: '',
  descricao: '',
  camposComErroEsperado: ['objetivo', 'dataInicio', 'categoria', 'data', 'valor', 'descricao'],
  mensagemEsperada: 'Campos obrigatórios',
};

// I-09 · [Negativo] Comprovante com prazo de validade excedido
export const dadosI09 = {
  objetivo: 'Consultoria em migração de dados legados',
  destino: 'Belo Horizonte - MG',
  dataInicio: '11/04/2026',
  dataFim: '11/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '05/01/2026',
  valor: '4560',
  descricao: 'Comprovante com data antiga para validar alerta de prazo excedido',
};

// I-10 · [Negativo] Valor acima do teto da verba
export const dadosI10 = {
  objetivo: 'Implantação de módulo financeiro ERP',
  destino: 'Curitiba - PR',
  dataInicio: '13/04/2026',
  dataFim: '15/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '13/04/2026',
  valor: '9999999',
  descricao: 'Valor acima do teto para validar alerta de limite excedido',
};

// I-11 · [Negativo] Anexo em formato não suportado
export const dadosI11 = {
  objetivo: 'Acompanhamento de go-live SAP',
  destino: 'Porto Alegre - RS',
  dataInicio: '06/04/2026',
  dataFim: '06/04/2026',
  nomeArquivoInvalido: 'relatorio-despesas.exe',
  mensagemErroEsperada: 'Formato inválido',
};

// I-12 · [Negativo] Envio com carrinho vazio
export const dadosI12 = {
  mensagemEsperada: 'Adicione pelo menos uma solicitação ao carrinho',
};

// I-13 · [Regressivo] Limpeza do formulário
export const dadosI13 = {
  objetivo: 'Diagnóstico de performance em ambiente produtivo',
  destino: 'Rio de Janeiro - RJ',
  dataInicio: '19/04/2026',
  dataFim: '21/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '19/04/2026',
  valor: '8120',
  descricao: 'Preencher formulário completo e depois limpar para validar reset',
};

// I-14 · [Regressivo] Atualização do total do carrinho
export const dadosI14 = {
  objetivo: 'Suporte em migração de ambiente',
  destino: 'Salvador - BA',
  dataInicio: '26/04/2026',
  dataFim: '28/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '26/04/2026',
  itens: [
    { valor: '2890', descricao: 'Item 1 — almoço dia 26' },
    { valor: '4560', descricao: 'Item 2 — jantar dia 27' },
    { valor: '3340', descricao: 'Item 3 — almoço dia 28' },
  ],
  totalEsperado: 'R$ 107,90',
};

// I-15 · [Regressivo] Navegação de retorno ao dashboard
export const dadosI15 = {
  urlOrigem: '/inserir-reembolso',
  urlDestino: '/reembolso',
  descricao: 'Acionar botão de voltar e confirmar retorno ao dashboard',
};
