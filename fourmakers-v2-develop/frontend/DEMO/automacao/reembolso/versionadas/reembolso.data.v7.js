/**
 * [DataForge] Dados de teste — Módulo de Reembolso (v7)
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v7.md)
 *
 * Dados realistas brasileiros: cidades, clientes e valores em reais.
 * Cada constante é nomeada conforme o ID do cenário BDD.
 */

// ═══════════════════════════════════════════════════════
//  DASHBOARD — /reembolso
// ═══════════════════════════════════════════════════════

// R-01 · [Positivo] Filtrar período de solicitação
export const dadosR01 = {
  dataInicio: '01/04/2026',
  dataFim: '24/04/2026',
  paginaEsperada: 1,
  descricao: 'Intervalo de abril/2026 para validar filtro e paginação resetada',
};

// R-02 · [Positivo] Buscar por objetivo/destino
export const dadosR02 = {
  termoBusca: 'Curitiba',
  colunasEsperadas: ['Curitiba - PR', 'Manutenção da rede regional'],
  descricao: 'Busca textual deve reduzir a tabela a registros que contenham o termo',
};

// R-03 · [Negativo] Busca sem correspondência
export const dadosR03 = {
  termoBusca: 'ZZZ-SEM-RESULTADO-404',
  mensagemEsperada: 'Nenhum reembolso encontrado',
  descricao: 'Termo inexistente deve esvaziar a lista e mostrar a mensagem padrão',
};

// R-04 · [Positivo] Abrir detalhes da solicitação
export const dadosR04 = {
  objetivo: 'Auditoria de despesas do trimestre',
  destino: 'Curitiba - PR',
  periodo: '05/04/2026 a 09/04/2026',
  clienteProjeto: 'Ecovias Sul / PRJ-ECV-1198',
  valorTotal: '12.850,00',
  itensEsperados: [
    { categoria: 'Alimentação', valorSolicitado: '1.320,00', valorAprovado: '1.150,00' },
    { categoria: 'Transporte', valorSolicitado: '780,00', valorAprovado: '780,00' },
  ],
  descricao: 'Modal de detalhes deve exibir cabeçalho completo e itens com status',
};

// R-05 · [Positivo] Baixar comprovantes do item
export const dadosR05 = {
  documentoPrincipal: { id: 9821, tipo: 'Nota Fiscal', nome: 'nf-viagem-curitiba.pdf' },
  documentosExtras: [
    { id: 9822, tipo: 'Recibo', nome: 'recibo-transporte-curitiba.jpg' },
  ],
  descricao: 'Modal de Documentos lista cada comprovante com opção de download',
};

// R-06 · [Positivo] Gerar relatório aguardando pagamento
export const dadosR06 = {
  parametro: 'EXIBIR_BOTAO_GERAR_PAGAMENTOS',
  nomeArquivoEsperado: 'relatorio-reembolsos-aprovados.xlsx',
  descricao: 'Gera arquivo consolidado das solicitações aguardando pagamento',
};

// R-07 · [Regressivo] Bloqueio da aba Gestão ADM sem permissão
export const dadosR07 = {
  tabParam: 'gestaoadm',
  fallbackTab: 'meus-reembolsos',
  descricao: 'Sem perfil gestor, a aba volta para Meus Reembolsos ao ler a URL',
};

// R-08 · [Regressivo] Bloqueio da aba Aprovações sem permissão
export const dadosR08 = {
  tabParam: 'aprovacoes',
  fallbackTab: 'meus-reembolsos',
  descricao: 'Sem perfil aprovador, a aba volta para Meus Reembolsos ao ler a URL',
};

// R-09 · [Regressivo] Sincronizar aba com URL quando permitido
export const dadosR09 = {
  tabParam: 'aprovacoes',
  manterAba: true,
  descricao: 'Com permissão, a aba informada na URL permanece ativa após recarregar',
};

// R-10 · [Positivo] Exibir Remessa CNAB apenas com parâmetro ativo
export const dadosR10 = {
  parametro: 'HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO',
  rotaEsperada: '/reembolso/remessa-cnab',
  descricao: 'Botão Remessa CNAB só aparece quando parâmetro está habilitado',
};

// R-11 · [Positivo] Resetar paginação ao alterar busca
export const dadosR11 = {
  termoInicial: 'viagem',
  termoNovo: 'alimentação',
  paginaEsperada: 1,
  descricao: 'Mudar termo de busca deve voltar paginação para a primeira página',
};

// R-12 · [Positivo] Alternar carregamento entre skeleton e cards
export const dadosR12 = {
  cardsEsperados: ['Solicitações enviadas', 'Aprovadas', 'Pagas'],
  descricao: 'Ao sair do loading, skeletons somem e os StatCards ficam visíveis',
};

// R-13 · [Positivo] Acesso rápido para criar novo reembolso
export const dadosR13 = {
  rotaDestino: '/inserir-reembolso',
  descricao: 'CTA de Solicitar Reembolso abre o formulário mantendo histórico',
};

// R-14 · [Negativo] Impedir data fim futura
export const dadosR14 = {
  dataFutura: '31/12/2027',
  descricao: 'Calendário deve bloquear seleção de datas futuras no filtro',
};

// ═══════════════════════════════════════════════════════
//  FORMULÁRIO — /inserir-reembolso
// ═══════════════════════════════════════════════════════

// I-01 · [Positivo] Adicionar item tipo 1 ao carrinho
export const dadosI01 = {
  objetivo: 'Visita técnica na planta da Vibra Energia',
  destino: 'São Paulo - SP',
  dataInicio: '10/04/2026',
  dataFim: '12/04/2026',
  clienteProjeto: { codigoCliente: 'CLI-2841', codigoProjeto: 'PRJ-VIB-4411', label: 'Vibra Energia / PRJ-VIB-4411' },
  categoria: { nome: 'Alimentação', tipoCodigo: 1, unidade: null, valorTeto: 180 },
  dataDespesa: '11/04/2026',
  valor: '87,50',
  descricao: 'Almoço com equipe de manutenção',
  comprovante: { nome: 'recibo-almoco.pdf', tipo: 'pdf' },
};

// I-02 · [Positivo] Calcular valor total para tipo 2
export const dadosI02 = {
  objetivo: 'Rota de visitas em clientes do interior',
  destino: 'Campinas - SP',
  dataInicio: '15/04/2026',
  dataFim: '16/04/2026',
  categoria: { nome: 'Km rodado', tipoCodigo: 2, unidade: 'km', valorUnitario: '2,40' },
  dataDespesa: '15/04/2026',
  quantidade: '48',
  valorUnitario: '2,40',
  valorTotalEsperado: '115,20',
  descricao: 'Deslocamento entre Campinas e Hortolândia',
};

// I-03 · [Positivo] OCR de comprovante pré-preenche data e valor
export const dadosI03 = {
  objetivo: 'Workshop de inovação no cliente',
  destino: 'São Caetano do Sul - SP',
  dataInicio: '18/04/2026',
  dataFim: '19/04/2026',
  categoria: { nome: 'Refeição', tipoCodigo: 1, valorTeto: 220 },
  comprovante: { nome: 'nota-almoco-workshop.pdf', tipo: 'pdf' },
  descricao: 'Nota fiscal do almoço enviada para OCR',
};

// I-04 · [Positivo] Exigir comprovante quando a verba obriga
export const dadosI04 = {
  objetivo: 'Hotel para pernoite durante manutenção',
  destino: 'Sorocaba - SP',
  dataInicio: '20/04/2026',
  dataFim: '21/04/2026',
  categoria: { nome: 'Hospedagem', tipoCodigo: 1, exigirComprovante: true, valorTeto: 420 },
  dataDespesa: '20/04/2026',
  valor: '318,70',
  descricao: 'Diária única em hotel próximo ao cliente',
  comprovante: { nome: 'nota-hospedagem-sorocaba.pdf', tipo: 'pdf' },
};

// I-05 · [Positivo] Editar item do carrinho
export const dadosI05 = {
  objetivo: 'Revisão de equipamentos',
  destino: 'Curitiba - PR',
  dataInicio: '22/04/2026',
  dataFim: '23/04/2026',
  categoria: { nome: 'Transporte', tipoCodigo: 1, valorTeto: 300 },
  dataDespesa: '22/04/2026',
  valor: '145,00',
  descricao: 'Corrida app do aeroporto até o cliente',
};

// I-06 · [Positivo] Excluir item do carrinho
export const dadosI06 = {
  descricao: 'Item temporário usado para validar exclusão do carrinho',
};

// I-07 · [Positivo] Enviar solicitações consolidadas em ZIP
export const dadosI07 = {
  objetivo: 'Sprint review presencial com cliente estratégico',
  destino: 'Rio de Janeiro - RJ',
  dataInicio: '25/04/2026',
  dataFim: '27/04/2026',
  itens: [
    {
      categoria: { nome: 'Alimentação', tipoCodigo: 1, valorTeto: 200 },
      dataDespesa: '25/04/2026',
      valor: '96,40',
      descricao: 'Refeição em aeroporto Santos Dumont',
    },
    {
      categoria: { nome: 'Transporte', tipoCodigo: 2, unidade: 'km', valorUnitario: '2,90' },
      dataDespesa: '26/04/2026',
      quantidade: '22',
      valorUnitario: '2,90',
      valorTotal: '63,80',
      descricao: 'Deslocamento até o escritório do cliente',
    },
  ],
};

// I-08 · [Positivo] Redirecionar após sucesso
export const dadosI08 = {
  rotaSucesso: '/reembolso',
  mensagem: 'Solicitações enviadas com sucesso',
};

// I-09 · [Regressivo] Sinalizar comprovante vencido
export const dadosI09 = {
  categoria: { nome: 'Reembolso rápido', tipoCodigo: 1, valorTeto: 150 },
  dataDespesa: '10/02/2026',
  validadeDias: 30,
  descricao: 'Comprovante enviado após a janela de validade para alertar usuário',
};

// I-10 · [Regressivo] Alertar teto da verba excedido
export const dadosI10 = {
  categoria: { nome: 'Material de escritório', tipoCodigo: 1, valorTeto: 350 },
  valorDigitado: '480,00',
  descricao: 'Valor acima do teto deve sinalizar borda âmbar e ícone de alerta',
};

// I-11 · [Regressivo] Solicitação sem projeto quando permitido
export const dadosI11 = {
  permitirSemProjeto: true,
  categoria: { nome: 'Outros', tipoCodigo: 1, valorTeto: 180 },
  dataDespesa: '14/04/2026',
  valor: '72,90',
  descricao: 'Fluxo permite salvar item mesmo sem selecionar cliente/projeto',
};

// I-12 · [Negativo] Impedir envio com carrinho vazio
export const dadosI12 = {
  mensagemEsperada: 'Carrinho vazio',
  descricao: 'Ao enviar sem itens, app deve bloquear e exibir toast de carrinho vazio',
};

// I-13 · [Negativo] Bloquear inclusão sem campos obrigatórios
export const dadosI13 = {
  objetivo: '',
  valor: '',
  descricao: 'Validação deve destacar campos obrigatórios ausentes e exibir toast',
};

// I-14 · [Negativo] Recusar datas inválidas na submissão
export const dadosI14 = {
  dataInicioInvalida: '32/13/2026',
  dataFimInvalida: '33/13/2026',
  descricao: 'Datas fora do calendário não devem ser aceitas na submissão',
};
