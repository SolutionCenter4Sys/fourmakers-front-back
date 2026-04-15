/**
 * [DataForge] Dados de teste — Módulo de Reembolso
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v5.md)
 */

// ═══════════════════════════════════════════════════════
//  DASHBOARD — Reembolso.tsx
// ═══════════════════════════════════════════════════════

// R-01 · [Positivo] Acesso padrão ao módulo
export const dadosR01 = {
  descricao: 'Colaborador comum — deve ver apenas aba Meus Reembolsos',
  abaEsperada: 'Meus Reembolsos',
  abasRestritas: ['Gestão ADM', 'Aprovações'],
};

// R-04 · [Positivo] Exibição dos indicadores numéricos
export const dadosR04 = {
  descricao: 'Verificar big numbers com valores formatados em moeda',
};

// R-05 · [Positivo] Filtro por período de datas
export const dadosR05 = {
  dataInicio: '01/03/2026',
  dataFim: '31/03/2026',
  descricao: 'Filtrar reembolsos de março/2026',
};

// R-06 · [Positivo] Busca textual na tabela
export const dadosR06 = {
  termoBusca: 'Curitiba',
  descricao: 'Buscar por destino Curitiba na tabela',
};

// R-09 · [Positivo] Navegação para nova solicitação
export const dadosR09 = {
  urlDestino: '/inserir-reembolso',
  botao: 'Solicitar Reembolso',
};

// R-10 · [Negativo] Acesso à aba Gestão ADM sem permissão
export const dadosR10 = {
  urlDireta: '/reembolso?tab=gestaoadm',
  abaEsperada: 'Meus Reembolsos',
  descricao: 'URL forçada sem permissão de gestor',
};

// R-12 · [Negativo] Busca sem resultados
export const dadosR12 = {
  termoBusca: 'XYZINEXISTENTE99',
  mensagemEsperada: 'Nenhum reembolso encontrado',
};

// ═══════════════════════════════════════════════════════
//  INSERIR REEMBOLSO — InserirReembolso.tsx
// ═══════════════════════════════════════════════════════

// I-01 · [Positivo] Adição de item ao carrinho com dados completos
export const dadosI01 = {
  objetivo: 'Reunião de alinhamento com equipe de infraestrutura',
  destino: 'Campinas - SP',
  dataInicio: '14/04/2026',
  dataFim: '16/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '14/04/2026',
  valor: '9450',
  descricao: 'Almoço corporativo durante workshop de integração',
};

// I-04 · [Positivo] Envio do carrinho com sucesso
export const dadosI04 = {
  objetivo: 'Visita técnica ao data center do cliente Bradesco',
  destino: 'Osasco - SP',
  dataInicio: '07/04/2026',
  dataFim: '09/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '07/04/2026',
  valor: '6275',
  descricao: 'Jantar com equipe de operações durante manutenção emergencial',
};

// I-07 · [Positivo] Solicitação com categoria baseada em quantidade (tipoCodigo = 2)
export const dadosI07 = {
  objetivo: 'Deslocamento para treinamento in-company',
  destino: 'Barueri - SP',
  dataInicio: '21/04/2026',
  dataFim: '23/04/2026',
  categoria: 'Quilometragem',
  dataDespesa: '21/04/2026',
  quantidade: '45',
  descricao: 'Percurso ida e volta sede até filial do cliente em Alphaville',
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
};

// I-09 · [Negativo] Data do comprovante excede prazo de validade
export const dadosI09 = {
  objetivo: 'Consultoria em migração de dados legados',
  destino: 'Belo Horizonte - MG',
  dataInicio: '10/04/2026',
  dataFim: '10/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '15/01/2026',
  valor: '4580',
  descricao: 'Comprovante com data antiga para validar alerta de prazo excedido',
};

// I-10 · [Negativo] Valor do reembolso excede teto da verba
export const dadosI10 = {
  objetivo: 'Implantação de módulo financeiro ERP',
  destino: 'Curitiba - PR',
  dataInicio: '12/04/2026',
  dataFim: '14/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '12/04/2026',
  valor: '9999999',
  descricao: 'Valor acima do teto para validar alerta de limite excedido',
};

// I-11 · [Negativo] Upload de arquivo em formato não permitido
export const dadosI11 = {
  objetivo: 'Acompanhamento de go-live SAP',
  destino: 'Porto Alegre - RS',
  dataInicio: '05/04/2026',
  dataFim: '05/04/2026',
  nomeArquivoInvalido: 'planilha-gastos.xlsx',
  mensagemErroEsperada: 'Formato inválido',
};

// I-12 · [Negativo] Envio com carrinho vazio
export const dadosI12 = {
  mensagemEsperada: 'Adicione pelo menos uma solicitação ao carrinho',
};

// I-13 · [Regressivo] Limpeza completa do formulário
export const dadosI13 = {
  objetivo: 'Diagnóstico de performance em ambiente produtivo',
  destino: 'Rio de Janeiro - RJ',
  dataInicio: '18/04/2026',
  dataFim: '20/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '18/04/2026',
  valor: '7830',
  descricao: 'Preencher formulário completo e depois limpar para validar reset',
};
