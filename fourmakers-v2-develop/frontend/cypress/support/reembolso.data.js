/**
 * [DataForge] Dados de teste — Módulo de Reembolso
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v3.md)
 * Dados realistas para preenchimento do formulário via Cypress
 */

// R-01 · [Positivo] Aba padrão — colaborador acessa, vê aba Meus Reembolsos
export const dadosR01 = {
  perfilUsuario: 'colaborador',
  abaEsperada: 'meus-reembolsos',
  urlTab: '/reembolso',
  termoBusca: '',
  dataInicioFiltro: '01/03/2026',
  dataFimFiltro: '31/03/2026',
  statCardsVisiveis: true,
  objetivo: 'Auditoria interna de processos — filial Sul',
  destino: 'Curitiba - PR',
  dataInicio: '18/03/2026',
  dataFim: '21/03/2026',
  categoria: 'Hospedagem',
  dataDespesa: '19/03/2026',
  valor: '612,80',
  quantidade: '1',
  descricao: 'Hospedagem próxima ao cliente Metalúrgica Horizonte Ltda.',
};

// R-02 · [Positivo] Aba gestão — gestor muda para Gestão ADM
export const dadosR02 = {
  perfilUsuario: 'gestor',
  abaEsperada: 'gestao-adm',
  urlTab: '/reembolso?tab=gestaoadm',
  termoBusca: '',
  dataInicioFiltro: '01/02/2026',
  dataFimFiltro: '28/02/2026',
  objetivo: 'Fechamento trimestral com diretoria',
  destino: 'São Paulo - SP',
  dataInicio: '05/02/2026',
  dataFim: '07/02/2026',
  categoria: 'Alimentação',
  dataDespesa: '06/02/2026',
  valor: '143,20',
  quantidade: '1',
  descricao: 'Jantar com equipe financeira no Jardins',
};

// R-03 · [Positivo] Aba aprovações — aprovador vê solicitações pendentes
export const dadosR03 = {
  perfilUsuario: 'aprovador',
  abaEsperada: 'aprovacoes',
  urlTab: '/reembolso?tab=aprovacoes',
  termoBusca: 'Fernanda',
  dataInicioFiltro: '10/03/2026',
  dataFimFiltro: '20/03/2026',
  objetivo: 'Treinamento ISO 27001 — equipe segurança',
  destino: 'Belo Horizonte - MG',
  dataInicio: '12/03/2026',
  dataFim: '14/03/2026',
  categoria: 'Transporte',
  dataDespesa: '13/03/2026',
  valor: '67,40',
  quantidade: '1',
  descricao: 'Uber Aeroporto Confins até sede Verdex Tecnologia',
};

// R-04 · [Negativo] Aba sem permissão — colaborador comum tenta gestão
export const dadosR04 = {
  perfilUsuario: 'colaborador',
  abaSolicitadaNaUrl: 'gestaoadm',
  abaEfetivaEsperada: 'meus-reembolsos',
  urlTab: '/reembolso?tab=gestaoadm',
  termoBusca: '',
  dataInicioFiltro: '',
  dataFimFiltro: '',
  objetivo: '',
  destino: '',
  dataInicio: '',
  dataFim: '',
  categoria: 'Material de Escritório',
  dataDespesa: '',
  valor: '',
  quantidade: '',
  descricao: '',
};

// R-05 · [Positivo] Filtro por período — datas início/fim filtram
export const dadosR05 = {
  perfilUsuario: 'colaborador',
  dataInicioFiltro: '01/04/2026',
  dataFimFiltro: '12/04/2026',
  termoBusca: '',
  objetivo: 'Visita comercial AgroPanorama S.A.',
  destino: 'Ribeirão Preto - SP',
  dataInicio: '08/04/2026',
  dataFim: '10/04/2026',
  categoria: 'Combustível',
  dataDespesa: '09/04/2026',
  valor: '218,65',
  quantidade: '1',
  descricao: 'Abastecimento para deslocamento à propriedade piloto',
};

// R-06 · [Positivo] Busca textual — filtra por texto
export const dadosR06 = {
  perfilUsuario: 'colaborador',
  termoBusca: 'workshop UX',
  dataInicioFiltro: '',
  dataFimFiltro: '',
  objetivo: 'Workshop UX Research — cliente Vitta Saúde Digital',
  destino: 'Porto Alegre - RS',
  dataInicio: '22/03/2026',
  dataFim: '24/03/2026',
  categoria: 'Alimentação',
  dataDespesa: '23/03/2026',
  valor: '92,15',
  quantidade: '1',
  descricao: 'Coffee break para participantes do workshop presencial',
};

// R-07 · [Positivo] Detalhes — abre modal de detalhes
export const dadosR07 = {
  perfilUsuario: 'colaborador',
  termoBusca: 'implantação CRM',
  linhaParaDetalhes: 'implantação CRM',
  objetivo: 'Implantação CRM — fase discovery',
  destino: 'Florianópolis - SC',
  dataInicio: '01/04/2026',
  dataFim: '04/04/2026',
  categoria: 'Hospedagem',
  dataDespesa: '02/04/2026',
  valor: '889,00',
  quantidade: '1',
  descricao: 'Duas diárias hotel região Trindade',
};

// R-08 · [Positivo] Documentos — visualiza anexos
export const dadosR08 = {
  perfilUsuario: 'colaborador',
  termoBusca: 'nota fiscal toner',
  objetivo: 'Reposição insumos escritório regional Nordeste',
  destino: 'Recife - PE',
  dataInicio: '25/03/2026',
  dataFim: '26/03/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '25/03/2026',
  valor: '312,45',
  quantidade: '1',
  descricao: 'Toner e papel A4 — Gráfica Pernambucana Ltda.',
};

// R-09 · [Positivo] Nova solicitação — navega para inserir
export const dadosR09 = {
  perfilUsuario: 'colaborador',
  rotaInserir: '/reembolso/inserir',
  objetivo: 'Congresso de Inovação — networking B2B',
  destino: 'Campinas - SP',
  dataInicio: '27/04/2026',
  dataFim: '30/04/2026',
  categoria: 'Transporte',
  dataDespesa: '28/04/2026',
  valor: '54,90',
  quantidade: '1',
  descricao: 'Trem entre Campinas e Barão Geraldo — ida ao evento',
};

// R-10 · [Positivo] Gerar relatório (gestão)
export const dadosR10 = {
  perfilUsuario: 'gestor',
  abaEsperada: 'gestao-adm',
  urlTab: '/reembolso?tab=gestaoadm',
  parametroRelatorio: 'EXIBIR_BOTAO_GERAR_PAGAMENTOS',
  dataInicioFiltro: '01/01/2026',
  dataFimFiltro: '31/03/2026',
  objetivo: 'Consolidação despesas Q1 — projetos integração',
  destino: 'Brasília - DF',
  dataInicio: '10/01/2026',
  dataFim: '12/01/2026',
  categoria: 'Alimentação',
  dataDespesa: '11/01/2026',
  valor: '178,30',
  quantidade: '1',
  descricao: 'Almoço com consultores parceiros no Lago Sul',
};

// R-11 · [Negativo] Relatório indisponível para colaborador
export const dadosR11 = {
  perfilUsuario: 'colaborador',
  botaoRelatorioVisivel: false,
  abaEsperada: 'meus-reembolsos',
  objetivo: 'Suporte onsite cliente Mineração Vale Verde',
  destino: 'Marabá - PA',
  dataInicio: '03/03/2026',
  dataFim: '08/03/2026',
  categoria: 'Combustível',
  dataDespesa: '05/03/2026',
  valor: '426,10',
  quantidade: '1',
  descricao: 'Combustível para trecho entre Serra Pelada e hotel',
};

// R-12 · [Positivo] Remessa CNAB
export const dadosR12 = {
  perfilUsuario: 'gestor',
  parametroCnab: 'HABILITA_BOTAO_REMESSA_CNAB_REEMBOLSO',
  abaEsperada: 'gestao-adm',
  objetivo: 'Pagamento lote reembolsos aprovados — março',
  destino: 'São Paulo - SP',
  dataInicio: '15/03/2026',
  dataFim: '20/03/2026',
  categoria: 'Hospedagem',
  dataDespesa: '17/03/2026',
  valor: '1.240,00',
  quantidade: '1',
  descricao: 'Hospedagem equipe rollout — Hotel Paulista Plaza',
};

// R-13 · [Negativo] Remessa CNAB oculta
export const dadosR13 = {
  perfilUsuario: 'gestor',
  botaoCnabVisivel: false,
  objetivo: 'Reunião alinhamento OKRs trimestrais',
  destino: 'Salvador - BA',
  dataInicio: '06/04/2026',
  dataFim: '07/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '06/04/2026',
  valor: '112,65',
  quantidade: '1',
  descricao: 'Jantar equipe produto no Rio Vermelho',
};

// R-14 · [Regressivo] Sync aba pela URL
export const dadosR14 = {
  perfilUsuario: 'aprovador',
  urlTabInicial: '/reembolso?tab=aprovacoes',
  abaEsperadaAposCarga: 'aprovacoes',
  urlTabAlternativa: '/reembolso?tab=gestaoadm',
  objetivo: 'Homologação portal fornecedores',
  destino: 'Joinville - SC',
  dataInicio: '14/04/2026',
  dataFim: '16/04/2026',
  categoria: 'Transporte',
  dataDespesa: '15/04/2026',
  valor: '38,75',
  quantidade: '1',
  descricao: 'Ônibus executivo Blumenau–Joinville',
};

// R-15 · [Regressivo] StatCards após carga
export const dadosR15 = {
  perfilUsuario: 'colaborador',
  statCardsEsperados: ['total', 'pendentes', 'aprovados', 'pagos'],
  objetivo: 'Capacitação Power BI — time analytics',
  destino: 'Goiânia - GO',
  dataInicio: '02/04/2026',
  dataFim: '04/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '03/04/2026',
  valor: '97,20',
  quantidade: '1',
  descricao: 'Apostila e suprimentos para laboratório de dados',
};

// I-01 · [Positivo] Cabeçalho completo
export const dadosI01 = {
  objetivo: 'Feira Hospitalar 2026 — prospecção equipamentos médicos',
  destino: 'São Paulo - SP',
  dataInicio: '05/05/2026',
  dataFim: '09/05/2026',
  categoria: 'Hospedagem',
  dataDespesa: '06/05/2026',
  valor: '0,00',
  quantidade: '1',
  descricao: 'Reserva hotel próximo ao Expo Center Norte (pré-item)',
};

// I-02 · [Negativo] Objetivo/datas ausentes
export const dadosI02 = {
  objetivo: '',
  destino: 'Fortaleza - CE',
  dataInicio: '',
  dataFim: '',
  categoria: 'Alimentação',
  dataDespesa: '12/04/2026',
  valor: '64,80',
  quantidade: '1',
  descricao: 'Almoço equipe com cliente Nordeste Logística',
};

// I-03 · [Positivo] Item completo no carrinho
export const dadosI03 = {
  objetivo: 'Visita técnica Datacenter Tier III',
  destino: 'Barueri - SP',
  dataInicio: '16/04/2026',
  dataFim: '17/04/2026',
  categoria: 'Transporte',
  dataDespesa: '16/04/2026',
  valor: '41,25',
  quantidade: '1',
  descricao: 'Corridas aplicativo entre Alphaville e sede cliente',
};

// I-04 · [Negativo] Valor acima do teto
export const dadosI04 = {
  objetivo: 'Viagem auditoria ISO — planta industrial',
  destino: 'Limeira - SP',
  dataInicio: '20/04/2026',
  dataFim: '22/04/2026',
  categoria: 'Transporte',
  dataDespesa: '21/04/2026',
  valor: '385,90',
  quantidade: '1',
  descricao: 'Valor intencionalmente acima do teto da verba de transporte',
};

// I-05 · [Negativo] Comprovante fora do prazo
export const dadosI05 = {
  objetivo: 'Retroativo — viagem vendas Sul',
  destino: 'Caxias do Sul - RS',
  dataInicio: '10/04/2026',
  dataFim: '12/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '15/01/2026',
  valor: '58,40',
  quantidade: '1',
  descricao: 'Almoço com representante comercial — data fora da validade',
};

// I-06 · [Positivo] OCR preenche campos
export const dadosI06 = {
  objetivo: 'Despesa com nota fiscal digital — OCR',
  destino: 'Niterói - RJ',
  dataInicio: '11/04/2026',
  dataFim: '11/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '11/04/2026',
  valor: '127,33',
  quantidade: '1',
  descricao: 'Papelaria Centro — NF-e 45218 série 3',
  nomeArquivoComprovanteOcr: 'nota-papelaria-centro-45218.pdf',
};

// I-07 · [Positivo] Formatação monetária
export const dadosI07 = {
  objetivo: 'Teste máscara BRL no campo valor',
  destino: 'Maringá - PR',
  dataInicio: '13/04/2026',
  dataFim: '13/04/2026',
  categoria: 'Combustível',
  dataDespesa: '13/04/2026',
  valorDigitadoBruto: '1.25000',
  valor: '1.250,00',
  quantidade: '1',
  descricao: 'Abastecimento posto Rede Sul — Icaraí',
};

// I-08 · [Positivo] Edição de item
export const dadosI08 = {
  objetivo: 'Kickoff projeto mobile banking',
  destino: 'Vitória - ES',
  dataInicio: '18/04/2026',
  dataFim: '19/04/2026',
  categoria: 'Hospedagem',
  dataDespesa: '18/04/2026',
  valor: '720,00',
  valorEditado: '695,50',
  quantidade: '1',
  descricao: 'Diária hotel Praia do Canto — ajuste após edição',
};

// I-09 · [Positivo] Exclusão de item
export const dadosI09 = {
  objetivo: 'Evento parceiro Microsoft — estande regional',
  destino: 'Curitiba - PR',
  dataInicio: '25/04/2026',
  dataFim: '26/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '25/04/2026',
  valor: '88,90',
  quantidade: '1',
  descricao: 'Item a ser removido do carrinho após validação',
};

// I-10 · [Positivo] Envio completo
export const dadosI10 = {
  objetivo: 'Treinamento certificação AWS — equipe cloud',
  destino: 'São Paulo - SP',
  dataInicio: '28/04/2026',
  dataFim: '02/05/2026',
  categoria: 'Transporte',
  dataDespesa: '29/04/2026',
  valor: '119,00',
  quantidade: '1',
  descricao: 'Taxi Congonhas até Avenida Paulista — curso presencial',
};

// I-11 · [Negativo] Envio com carrinho vazio
export const dadosI11 = {
  objetivo: 'Solicitação sem itens — validação de carrinho',
  destino: 'Osasco - SP',
  dataInicio: '30/04/2026',
  dataFim: '30/04/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '',
  valor: '',
  quantidade: '',
  descricao: '',
  carrinhoDeveEstarVazio: true,
};

// I-12 · [Negativo] Falha no envio (mock)
export const dadosI12 = {
  objetivo: 'Simulação erro 500 no POST de solicitação',
  destino: 'Belém - PA',
  dataInicio: '07/05/2026',
  dataFim: '09/05/2026',
  categoria: 'Hospedagem',
  dataDespesa: '08/05/2026',
  valor: '534,20',
  quantidade: '1',
  descricao: 'Hospedagem próximo ao Hangar — mock falha rede',
  mockInterceptStatus: 500,
  mockUrlParcial: '/reembolso',
};

// I-13 · [Positivo] Limpar formulário
export const dadosI13 = {
  objetivo: 'Rascunho descartado — visita cliente agronegócio',
  destino: 'Uberlândia - MG',
  dataInicio: '04/05/2026',
  dataFim: '06/05/2026',
  categoria: 'Combustível',
  dataDespesa: '05/05/2026',
  valor: '203,45',
  quantidade: '1',
  descricao: 'Combustível para rota Uberlândia–Uberaba',
  aposLimparObjetivoEsperado: '',
};

// I-14 · [Regressivo] Item por quantidade/unidade (tipoCodigo=2)
export const dadosI14 = {
  tipoCodigo: 2,
  objetivo: 'Deslocamento rodoviário — quilometragem contrato',
  destino: 'Sorocaba - SP',
  dataInicio: '08/05/2026',
  dataFim: '08/05/2026',
  categoria: 'Transporte',
  unidade: 'km',
  dataDespesa: '08/05/2026',
  quantidade: '142',
  valorUnitario: '1,85',
  valor: '262,70',
  descricao: 'Reembolso por quilometragem — rota Sorocaba–Itu ida e volta',
};

// I-15 · [Regressivo] Item por valor total (tipoCodigo=1)
export const dadosI15 = {
  tipoCodigo: 1,
  objetivo: 'Compra única — kit ergonomia home office',
  destino: 'São José dos Campos - SP',
  dataInicio: '09/05/2026',
  dataFim: '09/05/2026',
  categoria: 'Material de Escritório',
  dataDespesa: '09/05/2026',
  valor: '449,90',
  quantidade: '1',
  descricao: 'Suporte articulado monitor e apoio de punho — InfoParts Ltda.',
};
