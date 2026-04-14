import type { EstatisticasDesempenho, EstatisticasRH, RegistroCritico, ColaboradorDesempenho, ColaboradorDesempenhoDetalhes, Feedback, RegistroUmAUm, PDI } from '@shared/types/gestaoDesempenho';

export const mockEstatisticas: EstatisticasDesempenho = {
  totalColaboradores: 6,
  umAUmEmDia: 0,
  umAUmAtrasados: 6,
  feedbackEmDia: 0,
  feedbackAtrasados: 6,
  pdisAtivos: 2,
};

export const mockEstatisticasRH: EstatisticasRH = {
  totalColaboradores: 6,
  gestoresCom1a1EmDia: 0,
  feedbackEmDia: 0,
  sem1a1Mais14d: 6,
  semFeedbackMais30d: 6,
  semPdiAtivo: 5,
};

export const mockRegistrosCriticos: RegistroCritico[] = [
  {
    id: '1',
    colaboradorNome: 'Ana Silva',
    colaboradorCod: 'COL001',
    data: '2024-12-08',
    descricao: 'Discussão sobre metas do próximo trimestre. Ana demonstrou interesse em liderar o projeto de inovação tecnológica.',
  },
  {
    id: '2',
    colaboradorNome: 'Ana Silva',
    colaboradorCod: 'COL001',
    data: '2024-11-10',
    descricao: 'Conversa sobre desenvolvimento de carreira. Ana expressou interesse em trilha de tech lead e desenvolvimento de habilidades de liderança.',
  },
];

export const mockColaboradoresDesempenho: ColaboradorDesempenho[] = [
  {
    id: '1',
    codColaborador: 'COL001',
    nome: 'Ana Silva',
    cargo: 'Desenvolvedora Senior',
    gestor: 'Fernando Almeida',
    status: 'ativo',
    ultimoFeedback: '2024-12-14',
    ultimaUmAUm: undefined,
    pdisAtivos: 2,
    temPdi: true,
    temFeedback: true,
    temUmAUm: false,
    dataAniversario: '14/01', // Aniversário hoje
  },
  {
    id: '2',
    codColaborador: 'COL002',
    nome: 'Carlos Oliveira',
    cargo: 'Analista de Dados',
    gestor: 'Fernando Almeida',
    status: 'ativo',
    ultimoFeedback: '2024-11-19',
    ultimaUmAUm: undefined,
    pdisAtivos: 0,
    temPdi: false,
    temFeedback: true,
    temUmAUm: false,
    dataAniversario: '15/01', // Aniversário amanhã
  },
  {
    id: '3',
    codColaborador: 'COL003',
    nome: 'Juliana Lima',
    cargo: 'Tech Lead',
    gestor: 'Fernando Almeida',
    status: 'ativo',
    ultimoFeedback: undefined,
    ultimaUmAUm: undefined,
    pdisAtivos: 0,
    temPdi: false,
    temFeedback: false,
    temUmAUm: false,
    dataAniversario: '16/01', // Aniversário depois de amanhã
  },
  {
    id: '4',
    codColaborador: 'COL004',
    nome: 'Maria Santos',
    cargo: 'Product Manager',
    gestor: 'Patricia Rocha',
    status: 'ativo',
    ultimoFeedback: undefined,
    ultimaUmAUm: undefined,
    pdisAtivos: 0,
    temPdi: false,
    temFeedback: false,
    temUmAUm: false,
    dataAniversario: '17/01', // Aniversário +3 dias
  },
  {
    id: '5',
    codColaborador: 'COL005',
    nome: 'Pedro Costa',
    cargo: 'Designer UX',
    gestor: 'Patricia Rocha',
    status: 'ferias',
    ultimoFeedback: undefined,
    ultimaUmAUm: undefined,
    pdisAtivos: 0,
    temPdi: false,
    temFeedback: false,
    temUmAUm: false,
    dataAniversario: '18/01', // Aniversário +4 dias
  },
  {
    id: '6',
    codColaborador: 'COL006',
    nome: 'Rafael Mendes',
    cargo: 'DevOps Engineer',
    gestor: 'Patricia Rocha',
    status: 'afastado',
    ultimoFeedback: undefined,
    ultimaUmAUm: undefined,
    pdisAtivos: 0,
    temPdi: false,
    temFeedback: false,
    temUmAUm: false,
    dataAniversario: '19/01', // Aniversário +5 dias
  },
];

export const mockColaboradorDesempenhoDetalhes = (codColaborador: string): ColaboradorDesempenhoDetalhes | null => {
  const colaborador = mockColaboradoresDesempenho.find((c) => c.codColaborador === codColaborador);
  if (!colaborador) return null;

  return {
    id: colaborador.id,
    codColaborador: colaborador.codColaborador,
    nome: colaborador.nome,
    cargo: colaborador.cargo,
    email: 'ana.silva@empresa.com.br',
    telefone: '(11) 99999-1234',
    dataNascimento: '1992-01-07',
    dataAdmissao: '2021-02-28',
    tempoCasa: '4 anos e 10 meses',
    salario: 15000,
    saldoHoras: 12,
    status: colaborador.status,
    modelo: 'CLT',
  };
};

export const mockFeedbacks: Feedback[] = [
  {
    id: '1',
    data: '2024-12-14',
    resumo: 'Excelente comunicação com a equipe e proatividade na resolução de problemas técnicos.',
    realizadoPor: 'Carlos Oliveira',
    visto: true,
    vistoEm: '2024-12-16T14:32:00',
    continuar: [
      'Excelente comunicação com a equipe e proatividade na resolução de problemas técnicos.',
    ],
    comecar: [
      'Participar mais das reuniões de planejamento estratégico do departamento.',
    ],
    parar: [
      'Evitar assumir tarefas que não são da sua responsabilidade sem comunicar previamente.',
    ],
    observacoesGerais: 'Desempenho geral muito positivo no trimestre. Continue desenvolvendo suas habilidades de liderança.',
  },
  {
    id: '2',
    data: '2024-09-09',
    resumo: 'Qualidade técnica impecável nos projetos entregues. Demonstra grande capacidade de aprendizado.',
    realizadoPor: 'Maria Santos',
    visto: true,
    vistoEm: '2024-09-11T09:15:00',
  },
];

export const mockRegistrosUmAUm: RegistroUmAUm[] = [
  {
    id: '1',
    data: '2024-12-09',
    resumo: 'Discussão sobre metas do próximo trimestre. Ana demonstrou interesse em liderar o projeto de inovação tecnológica.',
    realizadoPor: 'Carlos Oliveira',
    visto: true,
    vistoEm: '2024-12-11T10:45:00',
    critico: true,
    pautaSugerida: [
      'Metas Q1 2025',
      'Projeto de modernização',
      'Feedback sobre apresentação',
    ],
    anotacoes: 'Discussão sobre metas do próximo trimestre. Ana demonstrou interesse em liderar o projeto de modernização da plataforma. Alinhamos expectativas e próximos passos.',
  },
  {
    id: '2',
    data: '2024-11-25',
    resumo: 'Revisão do andamento dos projetos atuais. Identificamos pontos de melhoria e próximos passos.',
    realizadoPor: 'Carlos Oliveira',
    visto: true,
    vistoEm: '2024-11-27T08:20:00',
    critico: false,
  },
  {
    id: '3',
    data: '2024-11-11',
    resumo: 'Conversa sobre desenvolvimento de carreira. Ana expressou interesse em trilha de tech lead.',
    realizadoPor: 'Carlos Oliveira',
    visto: false,
    critico: true,
  },
  {
    id: '4',
    data: '2024-10-15',
    resumo: 'Acompanhamento de projetos e feedback sobre desempenho.',
    realizadoPor: 'Carlos Oliveira',
    visto: false,
    critico: false,
  },
];

export const mockPautasSugeridas: string[] = [
  'Revisar metas Q1 2025',
  'Discutir promoção',
  'Feedback sobre apresentação para diretoria',
];

export const mockPDIs: PDI[] = [
  {
    id: '1',
    titulo: 'Certificação AWS Solutions Architect',
    descricao: 'Obter certificação para ampliar conhecimentos em arquitetura cloud.',
    status: 'em-andamento',
    dataInicio: '2024-06-01',
    prazo: '2025-03-29',
    prioridade: 'alta',
    criadoPor: 'Carlos Oliveira',
  },
  {
    id: '2',
    titulo: 'Curso de Liderança Técnica',
    descricao: 'Desenvolver habilidades de gestão de equipes técnicas.',
    status: 'pendente',
    dataInicio: '2025-01-01',
    prazo: '2025-02-14',
    prioridade: 'media',
    criadoPor: 'Carlos Oliveira',
  },
  {
    id: '3',
    titulo: 'Desenvolvimento de Liderança',
    descricao: 'Aprimorar habilidades de liderança técnica e gestão de equipes.',
    status: 'concluido',
    dataInicio: '2024-01-01',
    dataFim: '2024-12-31',
    criadoPor: 'Carlos Oliveira',
  },
];
