// ============================================================
// 🧪 DADOS DE TESTE — Módulo de Reembolso | Fourmakers v2
// Extraídos de: REEMBOLSO-MASSA-v2.sql
// Referência Gherkin: REEMBOLSO-BDD-v2.md
// ============================================================

export const usuarios = {
  colaborador: {
    email: 'renata.santos@foursys.com.br',
    nome: 'Renata Vieira dos Santos',
    cpf: '47832510617',
    banco: 'Nubank',
    agencia: '0001',
    conta: '7834219-0',
    chavePix: '47832510617',
  },
  gestor: {
    email: 'carlos.mendonca@foursys.com.br',
    nome: 'Carlos Eduardo Mendonça',
    cpf: '15870392497',
  },
  aprovador: {
    email: 'juliana.lima@foursys.com.br',
    nome: 'Juliana Ferreira Lima',
    cpf: '28347106517',
  },
  semPermissao: {
    email: 'pedro.almeida@foursys.com.br',
    nome: 'Pedro Henrique Almeida',
    cpf: '36185074290',
  },
};

export const projetos = {
  erpCloud: { nome: 'Sistema ERP Cloud', cliente: 'TechVision' },
  portal: { nome: 'Portal do Colaborador', cliente: 'Nexus' },
};

export const verbas = {
  alimentacao: { categoria: 'Alimentação', teto: 150.0, requerComprovante: true },
  transporte: { categoria: 'Transporte', teto: 80.0, requerComprovante: true },
  hospedagem: { categoria: 'Hospedagem', teto: 500.0, requerComprovante: true },
  quilometragem: { categoria: 'Quilometragem', teto: 1.2, requerComprovante: false },
  material: { categoria: 'Material de Escritório', teto: 200.0, requerComprovante: true },
};

export const solicitacoes = {
  techConf: {
    objetivo: 'Viagem SP - TechConf 2026',
    total: 512.5,
    data: '2026-03-15',
    itens: [
      { categoria: 'Alimentação', valor: 87.5, status: 'Aprovado' },
      { categoria: 'Transporte', valor: 45.0, status: 'Pago' },
      { categoria: 'Hospedagem', valor: 380.0, status: 'Aguardando Pagamento' },
    ],
  },
  nexus: {
    objetivo: 'Reunião cliente Nexus — Campinas',
    total: 158.3,
    data: '2026-02-10',
    itens: [
      { categoria: 'Alimentação', valor: 62.3, status: 'Pago' },
      { categoria: 'Quilometragem', valor: 96.0, status: 'Pago' },
    ],
  },
  workshop: {
    objetivo: 'Workshop Design Thinking — Equipe de Produto',
    total: 243.3,
    data: '2026-04-01',
    itens: [
      { categoria: 'Alimentação', valor: 95.4, status: 'Pendente' },
      { categoria: 'Material de Escritório', valor: 147.9, status: 'Aprovado' },
    ],
  },
  retroativa: {
    objetivo: 'Despesa retroativa — Almoço equipe desenvolvimento',
    total: 134.7,
    data: '2026-04-05',
    dataDespesa: '2026-02-15',
  },
  semProjeto: {
    objetivo: 'Confraternização equipe TI — fim de sprint',
    total: 210.0,
    data: '2026-01-20',
    status: 'Reprovado',
  },
};

export const dadosFormulario = {
  itemValido: {
    objetivo: 'Visita técnica ao cliente',
    destino: 'Campinas - SP',
    descricao: 'Almoço de negócios com equipe do cliente durante visita técnica',
    valor: '85,00',
  },
  valorAcimaTeto: {
    valor: '999,99',
  },
};
