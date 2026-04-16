/**
 * [DataForge] Dados de teste — Módulo de Reembolso (BDD v5)
 * Constantes R01–R15: Dashboard (filtros/busca/paginação/aba + campos base do formulário).
 * Constantes I01–I15: Inserir Reembolso (cabeçalho + item conforme CarrinhoItem).
 * Valores monetários em pt-BR; datas em dd/MM/yyyy.
 */

/** ✅ R01 [Positivo] Exibir dashboard — baseline de linha representativa na grade */
export const R01 = {
  objetivo: 'Auditoria de processos — cliente Vértice Soluções Financeiras',
  destino: 'Porto Alegre - RS',
  dataInicio: '08/04/2026',
  dataFim: '11/04/2026',
  categoria: 'Hospedagem',
  dataDespesa: '09/04/2026',
  valor: '892,30',
  quantidade: '1',
  descricao:
    'Hospedagem Hotel Figueiras — NF para CNPJ 28.473.916/0001-52, contato (51) 98217-4403.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'meus-reembolsos',
};

/** ✅ R02 [Positivo] Sincronizar aba com URL — parâmetro tab válido */
export const R02 = {
  objetivo: 'Treinamento Power BI — squad analytics',
  destino: 'Campinas - SP',
  dataInicio: '01/04/2026',
  dataFim: '03/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '02/04/2026',
  valor: '156,40',
  quantidade: '1',
  descricao: 'Coffee break e almoço — participantes CPF 046.321.874-10 e 275.983.618-30.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'aprovacoes',
};

/** ✅ R03 [Positivo] Alternar aba gestor — gestão administrativa */
export const R03 = {
  objetivo: 'Fechamento trimestral — revisão de verbas corporativas',
  destino: 'Brasília - DF',
  dataInicio: '14/03/2026',
  dataFim: '16/03/2026',
  categoria: 'Transporte aéreo',
  dataDespesa: '15/03/2026',
  valor: '1.240,00',
  quantidade: '1',
  descricao: 'Passagem LATAM — trecho BSB/GRU, localizador KLM8FQ, CPF titular 529.982.247-25.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'gestaoadm',
  abaComponente: 'gestao-adm',
};

/** ✅ R04 [Positivo] Alternar aba aprovador */
export const R04 = {
  objetivo: 'Homologação integração ERP — Indústria Metalúrgica Nordeste Ltda.',
  destino: 'Recife - PE',
  dataInicio: '22/03/2026',
  dataFim: '25/03/2026',
  categoria: 'Taxi / aplicativo',
  dataDespesa: '23/03/2026',
  valor: '67,80',
  quantidade: '1',
  descricao: 'Corridas Boa Viagem ↔ Cidade Universitária — motorista via app, CEP embarque 51030-420.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'aprovacoes',
  abaComponente: 'aprovacoes',
};

/** ✅ R05 [Positivo] Filtrar por intervalo de datas */
export const R05 = {
  objetivo: 'Implantação catálogo digital — varejo Sul',
  destino: 'Florianópolis - SC',
  dataInicio: '05/04/2026',
  dataFim: '18/04/2026',
  categoria: 'Combustível',
  dataDespesa: '12/04/2026',
  valor: '320,00',
  quantidade: '1',
  descricao: 'Abastecimento posto BR — veículo frota 48, nota fiscal série B, CNPJ 45.678.910/0001-24.',
  termoBusca: '',
  filtroDataInicio: '01/04/2026',
  filtroDataFim: '30/04/2026',
  abaUrl: 'meus-reembolsos',
};

/** ✅ R06 [Positivo] Busca textual case-insensitive */
export const R06 = {
  objetivo: 'Workshop LGPD — holding educacional Aurora',
  destino: 'Belo Horizonte - MG',
  dataInicio: '20/03/2026',
  dataFim: '21/03/2026',
  categoria: 'Alimentação',
  dataDespesa: '20/03/2026',
  valor: '213,55',
  quantidade: '1',
  descricao: 'Jantar equipe projeto Aurora — restaurante Savassi, NF CNPJ 73.195.846/0001-07.',
  termoBusca: 'Aurora',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'meus-reembolsos',
};

/** ✅ R07 [Positivo] Paginação */
export const R07 = {
  objetivo: 'Suporte go-live faturamento — Nexum Pagamentos',
  destino: 'São Paulo - SP',
  dataInicio: '02/04/2026',
  dataFim: '06/04/2026',
  categoria: 'Estacionamento',
  dataDespesa: '04/04/2026',
  valor: '45,00',
  quantidade: '1',
  descricao: 'Estacionamento garagem Av. Faria Lima — período comercial, recibo digital.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'meus-reembolsos',
  paginaAtual: 2,
  itensPorPagina: 10,
};

/** ✅ R08 [Positivo] Botão relatório Gestão ADM (flag de parâmetro habilitado) */
export const R08 = {
  objetivo: 'Consolidação pagamentos fornecedores — Q1/2026',
  destino: 'Curitiba - PR',
  dataInicio: '10/03/2026',
  dataFim: '31/03/2026',
  categoria: 'Material de escritório',
  dataDespesa: '28/03/2026',
  valor: '198,90',
  quantidade: '1',
  descricao: 'Compra papelaria Centro — CNPJ 12.847.365/0001-09, entrega CEP 80020-310.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'gestaoadm',
  exibirBotaoGerarPagamentos: true,
};

/** ✅ R09 [Positivo] Concluir download do relatório */
export const R09 = {
  objetivo: 'Auditoria interna processos RH',
  destino: 'Salvador - BA',
  dataInicio: '15/03/2026',
  dataFim: '19/03/2026',
  categoria: 'Passagem rodoviária',
  dataDespesa: '17/03/2026',
  valor: '187,00',
  quantidade: '1',
  descricao: 'Ônibus SSA/FEIRA — bilhete digital, documento titular CPF 871.456.039-97.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'gestaoadm',
  tokenRelatorioSimulado: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.simulado-relatorio',
};

/** ❌ R10 [Negativo] Bloquear Gestão ADM sem perfil gestor */
export const R10 = {
  objetivo: '',
  destino: '',
  dataInicio: '',
  dataFim: '',
  categoria: '',
  dataDespesa: '',
  valor: '',
  quantidade: '',
  descricao: '',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'gestaoadm',
  usuarioEhGestor: false,
};

/** ❌ R11 [Negativo] Bloquear Aprovações sem perfil aprovador */
export const R11 = {
  objetivo: 'Visita técnica — manutenção preventiva',
  destino: 'Fortaleza - CE',
  dataInicio: '',
  dataFim: '',
  categoria: '',
  dataDespesa: '',
  valor: '',
  quantidade: '',
  descricao: '',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'aprovacoes',
  usuarioEhAprovador: false,
};

/** ❌ R12 [Negativo] Impedir troca manual para aba restrita */
export const R12 = {
  objetivo: 'Reunião alinhamento escopo — projeto Rio Verde',
  destino: 'Goiânia - GO',
  dataInicio: '07/04/2026',
  dataFim: '07/04/2026',
  categoria: 'Alimentação',
  dataDespesa: '07/04/2026',
  valor: '62,00',
  quantidade: '1',
  descricao: 'Lanche rápido entre reuniões — comprovante CPF 619.023.457-83.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaAtual: 'meus-reembolsos',
  abaSolicitadaSemPermissao: 'gestao-adm',
};

/** ❌ R13 [Negativo] Gerar relatório sem token */
export const R13 = {
  objetivo: 'Conciliação despesas evento — Feira Agro Sul',
  destino: 'Pelotas - RS',
  dataInicio: '25/03/2026',
  dataFim: '27/03/2026',
  categoria: 'Hospedagem',
  dataDespesa: '26/03/2026',
  valor: '445,60',
  quantidade: '1',
  descricao: 'Pousada centro histórico — NF emitida CNPJ 08.392.174/0001-66.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'gestaoadm',
  tokenRelatorioSimulado: null,
};

/** ❌ R14 [Negativo] Erro na API ao gerar relatório */
export const R14 = {
  objetivo: 'Capacitação vendas B2B — trimestre atual',
  destino: 'Ribeirão Preto - SP',
  dataInicio: '12/04/2026',
  dataFim: '13/04/2026',
  categoria: 'Combustível',
  dataDespesa: '12/04/2026',
  valor: '210,45',
  quantidade: '1',
  descricao: 'Deslocamento rodoviário — posto Shell, km registrado planilha interna.',
  termoBusca: '',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'gestaoadm',
  simularFalhaApiRelatorio: true,
};

/** 🔁 R15 [Regressivo] Resetar página ao mudar busca */
export const R15 = {
  objetivo: 'Migração legado — cliente Confiança Seguros',
  destino: 'Vitória - ES',
  dataInicio: '03/04/2026',
  dataFim: '09/04/2026',
  categoria: 'Transporte aéreo',
  dataDespesa: '05/04/2026',
  valor: '980,00',
  quantidade: '1',
  descricao: 'Passagem GOL VIX/GRU — bagagem despachada, CPF 497.138.025-71.',
  termoBuscaInicial: 'Confiança',
  termoBuscaAlterada: 'migração',
  filtroDataInicio: '',
  filtroDataFim: '',
  abaUrl: 'meus-reembolsos',
  paginaAntesDaBusca: 3,
};

/** ✅ I01 [Positivo] Carregar projetos e verbas */
export const I01 = {
  objetivo: 'Kick-off projeto integração fiscal — Grupo Horizonte',
  destino: 'São José dos Campos - SP',
  dataInicio: '16/04/2026',
  dataFim: '18/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0847',
  categoria: 'Alimentação',
  dataDespesa: '17/04/2026',
  valor: '128,70',
  quantidade: '1',
  descricao: 'Almoço com equipe cliente — restaurante urbanova, NF CNPJ 61.284.737/0001-08.',
  tipoCodigo: 1,
};

/** ✅ I02 [Positivo] Preencher cabeçalho obrigatório */
export const I02 = {
  objetivo: 'Due diligence tecnológica — aquisição startup',
  destino: 'São Paulo - SP',
  dataInicio: '20/04/2026',
  dataFim: '24/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0912',
  categoria: 'Taxi / aplicativo',
  dataDespesa: '21/04/2026',
  valor: '94,20',
  quantidade: '1',
  descricao: 'Deslocamentos Paulista ↔ Brooklin — corridas Uber Business.',
  tipoCodigo: 1,
};

/** ✅ I03 [Positivo] Incluir item por valor direto (tipoCodigo !== 2) */
export const I03 = {
  objetivo: 'Suporte pós-implantação — módulo compras',
  destino: 'Joinville - SC',
  dataInicio: '08/04/2026',
  dataFim: '12/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0551',
  categoria: 'Material de escritório',
  dataDespesa: '10/04/2026',
  valor: '347,85',
  quantidade: '1',
  descricao: 'Suprimentos gráficos e cabos — nota CNPJ 19.384.726/0001-44, CEP 89201-250.',
  tipoCodigo: 1,
};

/** ✅ I04 [Positivo] Incluir item por quantidade (tipoCodigo === 2) */
export const I04 = {
  objetivo: 'Obras infraestrutura datacenter — cliente Atlas Cloud',
  destino: 'Manaus - AM',
  dataInicio: '01/04/2026',
  dataFim: '30/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0603',
  categoria: 'Diárias campo (quantidade)',
  dataDespesa: '14/04/2026',
  valor: '1.600,00',
  quantidade: '8',
  descricao: 'Oito diárias técnicas — unidade dia conforme tabela da verba, obra Zona Franca.',
  tipoCodigo: 2,
  unidade: 'dia',
  valorUnitario: '200,00',
};

/** ✅ I05 [Positivo] Analisar comprovantes via OCR */
export const I05 = {
  objetivo: 'Treinamento segurança cibernética — equipe interna',
  destino: 'Brasília - DF',
  dataInicio: '11/04/2026',
  dataFim: '12/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0722',
  categoria: 'Alimentação',
  dataDespesa: '11/04/2026',
  valor: '82,40',
  quantidade: '1',
  descricao: 'Comprovante digital anexo — OCR deve sugerir data e valores; CPF 083.714.562-40.',
  tipoCodigo: 1,
  nomeArquivoComprovante: 'nf_almoco_taguatinga_20260411.pdf',
};

/** ✅ I06 [Positivo] Editar item no carrinho */
export const I06 = {
  objetivo: 'Roadshow investidores — série B',
  destino: 'Rio de Janeiro - RJ',
  dataInicio: '22/04/2026',
  dataFim: '25/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0888',
  categoria: 'Hospedagem',
  dataDespesa: '23/04/2026',
  valor: '1.089,00',
  quantidade: '1',
  descricao: 'Hotel Copacabana — diária revisada após edição no carrinho, CNPJ 33.592.744/0001-08.',
  tipoCodigo: 1,
  itemCarrinhoIdEdicao: 'linha-carrinho-01',
};

/** ✅ I07 [Positivo] Enviar solicitação com ZIP */
export const I07 = {
  objetivo: 'Auditoria contratos fornecedores — Q2/2026',
  destino: 'Curitiba - PR',
  dataInicio: '14/04/2026',
  dataFim: '17/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0404',
  categoria: 'Estacionamento',
  dataDespesa: '15/04/2026',
  valor: '38,50',
  quantidade: '1',
  descricao: 'Estacionamento shopping — recibo com horário entrada/saída.',
  tipoCodigo: 1,
  comprovantesAnexos: ['estacionamento_15042026.pdf', 'mapa_rotas.pdf'],
};

/** ❌ I08 [Negativo] Objetivo obrigatório vazio */
export const I08 = {
  objetivo: '',
  destino: 'Maringá - PR',
  dataInicio: '18/04/2026',
  dataFim: '19/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0330',
  categoria: 'Combustível',
  dataDespesa: '18/04/2026',
  valor: '185,00',
  quantidade: '1',
  descricao: 'Abastecimento viagem — posto BR às margens BR-376.',
  tipoCodigo: 1,
};

/** ❌ I09 [Negativo] Data de início ausente */
export const I09 = {
  objetivo: 'Visita cliente agroindustrial — safra 2026',
  destino: 'Uberlândia - MG',
  dataInicio: '',
  dataFim: '28/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0777',
  categoria: 'Alimentação',
  dataDespesa: '25/04/2026',
  valor: '71,30',
  quantidade: '1',
  descricao: 'Almoço região Araguari — churrascaria familiar.',
  tipoCodigo: 1,
};

/** ❌ I10 [Negativo] Cliente/projeto obrigatório ausente */
export const I10 = {
  objetivo: 'Workshop design thinking — inovação aberta',
  destino: 'Florianópolis - SC',
  dataInicio: '05/05/2026',
  dataFim: '06/05/2026',
  clienteProjetoCodigo: '',
  categoria: 'Material de escritório',
  dataDespesa: '05/05/2026',
  valor: '54,90',
  quantidade: '1',
  descricao: 'Canetas, flip chart e post-its — papelaria Trindade.',
  tipoCodigo: 1,
};

/** ❌ I11 [Negativo] Valor inválido (tipo !== 2, valor <= 0) */
export const I11 = {
  objetivo: 'Suporte incidente P1 — plantão fim de semana',
  destino: 'Belém - PA',
  dataInicio: '12/04/2026',
  dataFim: '13/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0991',
  categoria: 'Taxi / aplicativo',
  dataDespesa: '12/04/2026',
  valor: '0,00',
  quantidade: '1',
  descricao: 'Corridas plantão — valor zerado para forçar validação.',
  tipoCodigo: 1,
};

/** ❌ I12 [Negativo] Comprovante obrigatório ausente */
export const I12 = {
  objetivo: 'Conferência anual parceiros — módulo financeiro',
  destino: 'Salvador - BA',
  dataInicio: '20/04/2026',
  dataFim: '22/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0505',
  categoria: 'Hospedagem',
  dataDespesa: '20/04/2026',
  valor: '620,00',
  quantidade: '1',
  descricao: 'Hospedagem hotel Barra — verba exige comprovante; não anexar arquivo no teste.',
  tipoCodigo: 1,
  exigirComprovante: true,
  comprovantesAnexos: [],
};

/** ❌ I13 [Negativo] Data do comprovante fora da validade (> 30 dias) */
export const I13 = {
  objetivo: 'Reunião contrato SLA — operadora telecom',
  destino: 'Natal - RN',
  dataInicio: '14/04/2026',
  dataFim: '15/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0618',
  categoria: 'Alimentação',
  dataDespesa: '10/01/2026',
  valor: '98,00',
  quantidade: '1',
  descricao: 'Nota antiga fora da janela de validade do comprovante — CNPJ 47.392.618/0001-55.',
  tipoCodigo: 1,
  validadeComprovanteDiasReferencia: 30,
};

/** ❌ I14 [Negativo] Valor acima do teto da verba */
export const I14 = {
  objetivo: 'Deslocamento intermunicipal — projeto Sertão Digital',
  destino: 'Juazeiro do Norte - CE',
  dataInicio: '09/04/2026',
  dataFim: '11/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0284',
  categoria: 'Combustível',
  dataDespesa: '10/04/2026',
  valor: '12.450,00',
  quantidade: '1',
  descricao: 'Valor deliberadamente acima do teto da verba (limite fictício 600,00 no cenário).',
  tipoCodigo: 1,
  tetoVerbaReferencia: 600,
};

/** 🔁 I15 [Regressivo] Limpar formulário e remover item do carrinho */
export const I15 = {
  objetivo: 'Planejamento sprint — squad pagamentos',
  destino: 'Osasco - SP',
  dataInicio: '21/04/2026',
  dataFim: '25/04/2026',
  clienteProjetoCodigo: 'PRJ-2026-0701',
  categoria: 'Alimentação',
  dataDespesa: '22/04/2026',
  valor: '112,60',
  quantidade: '1',
  descricao: 'Refeições coworking — após limpar, carrinho deve zerar totais.',
  tipoCodigo: 1,
  acaoLimparFormulario: true,
  acaoRemoverLinhaCarrinho: true,
};
