/**
 * [DataForge] Dados de teste — Módulo de Reembolso
 * Gerado a partir dos cenários BDD (REEMBOLSO-BDD-v2.md)
 * Dados realistas para preenchimento do formulário via Playwright
 */

// R-01 · [Positivo] Acesso ao dashboard de reembolsos
export const dadosR01 = {
  perfil: 'colaborador',
  rotaEsperada: '/reembolso',
  abaEsperada: 'Meus Reembolsos',
  periodoInicio: '01/07/2026',
  periodoFim: '12/08/2026',
};

// R-02 · [Negativo] Aba de gestão restrita sem perfil de gestor
export const dadosR02 = {
  perfil: 'colaborador-sem-gestao',
  abaRestrita: 'Gestao ADM',
  abaFallback: 'Meus Reembolsos',
};

// R-04 · [Positivo] Navegação para nova solicitação
export const dadosR04 = {
  botao: 'Solicitar Reembolso',
  rotaEsperada: '/inserir-reembolso',
  tituloEsperado: 'Inserir Reembolso',
};

// I-08 · [Positivo] Inclusão de item no carrinho com comprovante
export const dadosI08 = {
  objetivo: 'Visita técnica ao cliente Votorantim Cimentos',
  destino: 'Curitiba - PR',
  dataInicio: '04/08/2026',
  dataFim: '06/08/2026',
  categoria: 'Alimentação',
  dataDespesa: '05/08/2026',
  valor: '87,50',
  descricao: 'Almoço durante alinhamento com equipe de engenharia do cliente',
  exigeComprovante: true,
  cnpjEstabelecimento: '45.678.910/0001-24',
  cepEstabelecimento: '80010-030',
  telefoneContato: '(41) 98765-3021',
};

// I-09 · [Negativo] Categoria com comprovante obrigatório sem anexo
export const dadosI09 = {
  objetivo: 'Reunião comercial com a Ambev',
  destino: 'São Paulo - SP',
  dataInicio: '07/08/2026',
  dataFim: '07/08/2026',
  categoria: 'Alimentação',
  dataDespesa: '07/08/2026',
  valor: '64,90',
  descricao: 'Café da manhã antes da reunião comercial',
  exigeComprovante: true,
  anexos: [],
};

// I-13 · [Positivo] Envio da solicitação com itens no carrinho
export const dadosI13 = {
  objetivo: 'Participação na Feira Hospitalar 2026',
  destino: 'São Paulo - SP',
  dataInicio: '10/08/2026',
  dataFim: '12/08/2026',
  categoria: 'Hospedagem',
  dataDespesa: '10/08/2026',
  valor: '612,80',
  descricao: 'Diária de hotel próximo ao centro de convenções Anhembi',
  exigeComprovante: true,
  cpfHospede: '529.982.247-25',
  cepHotel: '02012-010',
  telefoneHotel: '(11) 3842-7619',
};

// I-14 · [Regressivo] Leitura automática de dados do comprovante
export const dadosI14 = {
  objetivo: 'Auditoria de processos na filial Campinas',
  destino: 'Campinas - SP',
  dataInicio: '11/08/2026',
  dataFim: '11/08/2026',
  categoria: 'Transporte',
  dataDespesa: '11/08/2026',
  valorOcrSugerido: '142,30',
  dataOcrSugerida: '11/08/2026',
  empresaComprovante: 'Posto Rede Sul Ltda',
  cnpjEstabelecimento: '73.195.846/0001-07',
};

// AP-01 · [Positivo] Aprovação de solicitações pendentes
export const dadosAP01 = {
  perfil: 'aprovador',
  acao: 'aprovar',
  quantidadeSelecionada: 2,
};

// AP-02 · [Negativo] Reprovação sem justificativa
export const dadosAP02 = {
  perfil: 'aprovador',
  acao: 'reprovar',
  justificativa: '',
};

// RP-01 · [Regressivo] Consulta das regras e verbas parametrizadas
export const dadosRP01 = {
  perfil: 'gestor',
  aba: 'regras',
  expectativa: 'verbas-visiveis',
};
