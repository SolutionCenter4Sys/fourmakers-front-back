/**
 * [DataForge] Dados de teste — Módulo de Reembolso (v8)
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v8.md)
 *
 * Dados realistas brasileiros: nomes, cidades, valores em reais.
 * Cada constante é nomeada conforme o ID do cenário BDD.
 */

// ═══════════════════════════════════════════════════════
//  DASHBOARD — /reembolso
// ═══════════════════════════════════════════════════════

// R-01 · [Positivo] Visualização padrão do dashboard
export const dadosR01 = {
  abaEsperada: 'meus-reembolsos',
  abasRestritas: ['gestao-adm', 'aprovacoes'],
  cardsEsperados: ['Solicitações enviadas', 'Aprovadas', 'Pagas'],
  descricao: 'Colaborador comum vê apenas Meus Reembolsos e os cards de estatísticas',
};

// R-02 · [Positivo] Filtrar reembolsos por período
export const dadosR02 = {
  dataInicio: '01/05/2026',
  dataFim: '13/05/2026',
  paginaEsperada: 1,
  descricao: 'Intervalo de maio/2026 para validar filtro por período e reset de paginação',
};

// R-03 · [Positivo] Buscar reembolsos por texto livre
export const dadosR03 = {
  termoBusca: 'Porto Alegre',
  colunasEsperadas: ['Porto Alegre - RS', 'Consultoria infraestrutura'],
  descricao: 'Busca textual deve reduzir a tabela a registros que contenham o termo',
};

// R-04 · [Positivo] Abrir modal de detalhes da solicitação
export const dadosR04 = {
  objetivo: 'Implantação de novo módulo no cliente',
  destino: 'Belo Horizonte - MG',
  periodo: '05/05/2026 a 08/05/2026',
  clienteProjeto: 'Cemig Distribuição / PRJ-CMG-3920',
  valorTotal: '6.740,00',
  itensEsperados: [
    { categoria: 'Hospedagem', valorSolicitado: '4.280,00', valorAprovado: '3.800,00' },
    { categoria: 'Alimentação', valorSolicitado: '1.460,00', valorAprovado: '1.460,00' },
    { categoria: 'Transporte', valorSolicitado: '1.000,00', valorAprovado: '980,00' },
  ],
  descricao: 'Modal de detalhes deve exibir cabeçalho completo e itens com status',
};

// R-05 · [Positivo] Baixar comprovante com token autenticado
export const dadosR05 = {
  documentoPrincipal: { id: 14207, tipo: 'Nota Fiscal', nome: 'nf-hospedagem-bh.pdf' },
  documentosExtras: [
    { id: 14208, tipo: 'Recibo', nome: 'recibo-taxi-confins.jpg' },
  ],
  descricao: 'Painel de documentos lista cada comprovante com botão de download',
};

// R-06 · [Positivo] Gerar relatório de solicitações aguardando pagamento
export const dadosR06 = {
  parametro: 'EXIBIR_BOTAO_GERAR_PAGAMENTOS',
  nomeArquivoEsperado: 'relatorio-reembolsos-aprovados.xlsx',
  descricao: 'Gera arquivo consolidado das solicitações com status aguardando pagamento',
};

// R-07 · [Positivo] Solicitar novo reembolso pelo dashboard
export const dadosR07 = {
  rotaDestino: '/inserir-reembolso',
  descricao: 'Botão Solicitar Reembolso navega para o formulário de inserção',
};

// R-08 · [Positivo] Exibir botão Remessa CNAB para usuários autorizados
export const dadosR08 = {
  parametro: 'HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO',
  rotaEsperada: '/reembolso/remessa-cnab',
  descricao: 'Botão Remessa CNAB só aparece na aba Gestão ADM quando parâmetro está ativo',
};

// R-09 · [Negativo] Busca sem resultados exibe mensagem vazia
export const dadosR09 = {
  termoBusca: 'XYZW-SEM-RESULTADO-999',
  mensagemEsperada: 'Nenhum reembolso encontrado',
  descricao: 'Termo inexistente deve esvaziar a lista e mostrar a mensagem padrão',
};

// R-10 · [Negativo] Impedir seleção de data fim futura
export const dadosR10 = {
  dataFutura: '31/12/2027',
  descricao: 'Calendário deve bloquear seleção de datas futuras no filtro Data Fim',
};

// R-11 · [Negativo] Falha ao gerar relatório exibe alerta
export const dadosR11 = {
  mensagemErroEsperada: 'Erro ao gerar relatório. Tente novamente.',
  descricao: 'Quando a API de relatório falha, o alerta é exibido e modal permanece aberto',
};

// R-12 · [Regressivo] Bloquear aba Gestão ADM sem perfil de gestor
export const dadosR12 = {
  tabParam: 'gestaoadm',
  fallbackTab: 'meus-reembolsos',
  descricao: 'Sem perfil gestor, a aba volta para Meus Reembolsos e limpa tab da URL',
};

// R-13 · [Regressivo] Bloquear aba Aprovações sem perfil de aprovador
export const dadosR13 = {
  tabParam: 'aprovacoes',
  fallbackTab: 'meus-reembolsos',
  descricao: 'Sem perfil aprovador, a aba volta para Meus Reembolsos',
};

// R-14 · [Regressivo] Sincronizar aba ativa com parâmetro da URL
export const dadosR14 = {
  tabParam: 'aprovacoes',
  manterAba: true,
  descricao: 'Com permissão, a aba informada na URL permanece ativa após recarregar',
};

// ═══════════════════════════════════════════════════════
//  FORMULÁRIO — /inserir-reembolso
// ═══════════════════════════════════════════════════════

// I-01 · [Positivo] Adicionar item com categoria tipo 1 ao carrinho
export const dadosI01 = {
  objetivo: 'Reunião de alinhamento com equipe de operações',
  destino: 'Florianópolis - SC',
  dataInicio: '05/05/2026',
  dataFim: '07/05/2026',
  clienteProjeto: { codigoCliente: 'CLI-5174', codigoProjeto: 'PRJ-CEL-2839', label: 'Celesc Distribuição / PRJ-CEL-2839' },
  categoria: { nome: 'Alimentação', tipoCodigo: 1, unidade: null, valorTeto: 200 },
  dataDespesa: '06/05/2026',
  valor: '94,30',
  descricao: 'Jantar de trabalho com equipe técnica local',
  comprovante: { nome: 'nf-restaurante-floripa.pdf', tipo: 'pdf' },
};

// I-02 · [Positivo] Calcular valor total para categoria tipo 2
export const dadosI02 = {
  objetivo: 'Visita a fornecedores da região metropolitana',
  destino: 'Guarulhos - SP',
  dataInicio: '08/05/2026',
  dataFim: '08/05/2026',
  categoria: { nome: 'Km rodado', tipoCodigo: 2, unidade: 'km', valorUnitario: '2,60' },
  dataDespesa: '08/05/2026',
  quantidade: '35',
  valorUnitario: '2,60',
  valorTotalEsperado: '91,00',
  descricao: 'Deslocamento entre Guarulhos e Osasco para inspeção',
};

// I-03 · [Positivo] Pré-preenchimento por OCR de comprovante fiscal
export const dadosI03 = {
  objetivo: 'Treinamento presencial na matriz do cliente',
  destino: 'Santo André - SP',
  dataInicio: '11/05/2026',
  dataFim: '12/05/2026',
  categoria: { nome: 'Refeição', tipoCodigo: 1, valorTeto: 250 },
  comprovante: { nome: 'nota-almoco-treinamento.pdf', tipo: 'pdf' },
  descricao: 'Nota fiscal enviada para OCR — data e valor devem ser preenchidos automaticamente',
};

// I-04 · [Positivo] Editar item já adicionado ao carrinho
export const dadosI04 = {
  objetivo: 'Manutenção preventiva em data center',
  destino: 'Curitiba - PR',
  dataInicio: '14/05/2026',
  dataFim: '15/05/2026',
  categoria: { nome: 'Transporte', tipoCodigo: 1, valorTeto: 350 },
  dataDespesa: '14/05/2026',
  valor: '162,00',
  descricao: 'Corrida de app do aeroporto Afonso Pena até o cliente',
};

// I-05 · [Positivo] Excluir item do carrinho
export const dadosI05 = {
  descricao: 'Item temporário incluído para validar exclusão e recálculo do total',
};

// I-06 · [Positivo] Enviar solicitações em lote via ZIP
export const dadosI06 = {
  objetivo: 'Acompanhamento de go-live no cliente',
  destino: 'Recife - PE',
  dataInicio: '18/05/2026',
  dataFim: '21/05/2026',
  itens: [
    {
      categoria: { nome: 'Alimentação', tipoCodigo: 1, valorTeto: 200 },
      dataDespesa: '19/05/2026',
      valor: '78,60',
      descricao: 'Almoço no restaurante próximo ao escritório do cliente',
    },
    {
      categoria: { nome: 'Hospedagem', tipoCodigo: 1, exigirComprovante: true, valorTeto: 450 },
      dataDespesa: '18/05/2026',
      valor: '389,00',
      descricao: 'Diária em hotel no bairro de Boa Viagem',
      comprovante: { nome: 'nf-hotel-recife.pdf', tipo: 'pdf' },
    },
  ],
};

// I-07 · [Positivo] Redirecionar para dashboard após envio com sucesso
export const dadosI07 = {
  rotaSucesso: '/reembolso',
  mensagem: 'Solicitações enviadas com sucesso',
};

// I-08 · [Negativo] Bloquear inclusão sem campos obrigatórios preenchidos
export const dadosI08 = {
  objetivo: '',
  valor: '',
  categoria: null,
  descricao: 'Validação deve destacar campos obrigatórios ausentes e exibir toast',
};

// I-09 · [Negativo] Impedir envio com carrinho vazio
export const dadosI09 = {
  mensagemEsperada: 'Carrinho vazio',
  descricao: 'Ao enviar sem itens, app deve bloquear e exibir toast de carrinho vazio',
};

// I-10 · [Negativo] Bloquear comprovante obrigatório ausente
export const dadosI10 = {
  objetivo: 'Entrega de material no escritório filial',
  destino: 'Salvador - BA',
  dataInicio: '22/05/2026',
  dataFim: '22/05/2026',
  categoria: { nome: 'Hospedagem', tipoCodigo: 1, exigirComprovante: true, valorTeto: 400 },
  dataDespesa: '22/05/2026',
  valor: '275,00',
  descricao: 'Tentativa de salvar item sem anexar comprovante obrigatório',
  comprovante: null,
};

// I-11 · [Negativo] Rejeitar data de despesa inválida no envio
export const dadosI11 = {
  dataInicioInvalida: '32/13/2026',
  dataFimInvalida: '33/13/2026',
  descricao: 'Datas fora do calendário não devem ser aceitas na submissão',
};

// I-12 · [Regressivo] Sinalizar comprovante vencido
export const dadosI12 = {
  categoria: { nome: 'Reembolso rápido', tipoCodigo: 1, valorTeto: 150 },
  dataDespesa: '15/03/2026',
  validadeDias: 30,
  descricao: 'Comprovante com data além da validade ativa alerta visual no formulário',
};

// I-13 · [Regressivo] Alertar valor acima do teto da verba
export const dadosI13 = {
  categoria: { nome: 'Material de escritório', tipoCodigo: 1, valorTeto: 300 },
  valorDigitado: '425,00',
  descricao: 'Valor acima do teto deve sinalizar borda âmbar e ícone de alerta',
};

// I-14 · [Regressivo] Permitir solicitação sem projeto vinculado
export const dadosI14 = {
  permitirSemProjeto: true,
  categoria: { nome: 'Outros', tipoCodigo: 1, valorTeto: 180 },
  dataDespesa: '10/05/2026',
  valor: '58,70',
  descricao: 'Fluxo permite salvar item sem selecionar cliente/projeto',
};
