/**
 * MOCK DATA FOR ORQUESTRACAO PAGE - DEVELOPMENT ONLY
 * Mock data for OrquestracaoPage (Organizational orchestration view)
 * Only used when environment is development
 */

import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'
import type { MinhaEquipeSugestao } from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'

// Mock KPIs for Orquestração View
export const mockOrquestracaoKPIs: MinhaEquipeKPIs = {
  totColaboradores: 25,
  totPendentesSkills: 15, // Atualizado para refletir o número de skills pendentes no mock
  mediaMatch: 74.2,
}

// Mock Colaboradores for Orquestração (multiple clients and managers)
export const mockOrquestracaoColaboradores: MinhaEquipeColaborador[] = [
  {
    id: 'orch-colab-001',
    nomeColaborador: 'Ana Silva Santos',
    nomeCliente: 'Banco XYZ',
    nomeGestorCliente: 'Roberto Oliveira',
    perfil: 'Desenvolvedor Full Stack Sênior',
    perfilId: 'perfil-001',
    nomeGestorOperacional: 'Carlos Mendes',
    nomeGestorAdm: 'Maria Costa',
    codigoInternoColaborador: '12345678901',
    codigoGestorAdm: '98765432100',
    idAlocacao: 1001,
    match: 85,
    retornoMatch: {
      hardSkills: 90,
      softSkills: 85,
      methodologies: 80,
      domains: 85,
      languages: 88,
    },
    resultadoHabilidades: [
      {
        id: 'skill-001',
        name: 'React',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-002',
        name: 'Node.js',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-003',
        name: 'TypeScript',
        type: 9, // Language
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-004',
        name: 'GraphQL',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-005',
        name: 'Comunicação Eficaz',
        type: 8, // Soft Skill
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-006',
        name: 'Scrum',
        type: 3, // Methodology
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Carlos Mendes', 'João Silva'],
  },
  {
    id: 'orch-colab-002',
    nomeColaborador: 'Bruno Almeida',
    nomeCliente: 'Banco XYZ',
    nomeGestorCliente: 'Roberto Oliveira',
    perfil: 'Desenvolvedor Backend Pleno',
    perfilId: 'perfil-002',
    nomeGestorOperacional: 'Carlos Mendes',
    nomeGestorAdm: 'Maria Costa',
    codigoInternoColaborador: '23456789012',
    codigoGestorAdm: '98765432100',
    idAlocacao: 1002,
    match: 68,
    retornoMatch: {
      hardSkills: 70,
      softSkills: 65,
      methodologies: 68,
      domains: 72,
      languages: 65,
    },
    resultadoHabilidades: [
      {
        id: 'skill-007',
        name: 'Java',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-008',
        name: 'Spring Boot',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-009',
        name: 'Microservices Architecture',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-010',
        name: 'PostgreSQL',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-011',
        name: 'Trabalho em Equipe',
        type: 8, // Soft Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Carlos Mendes'],
  },
  {
    id: 'orch-colab-003',
    nomeColaborador: 'Carla Rodrigues',
    nomeCliente: 'Tech Corp',
    nomeGestorCliente: 'Ana Paula Lima',
    perfil: 'QA Engineer Sênior',
    perfilId: 'perfil-003',
    nomeGestorOperacional: 'João Santos',
    nomeGestorAdm: 'Pedro Alves',
    codigoInternoColaborador: '34567890123',
    codigoGestorAdm: '87654321098',
    idAlocacao: 1003,
    match: 92,
    retornoMatch: {
      hardSkills: 95,
      softSkills: 90,
      methodologies: 92,
      domains: 88,
      languages: 94,
    },
    resultadoHabilidades: [
      {
        id: 'skill-012',
        name: 'Selenium',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-013',
        name: 'Cypress',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-014',
        name: 'Testes Automatizados',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-015',
        name: 'Jest',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-016',
        name: 'Atenção aos Detalhes',
        type: 8, // Soft Skill
        requiredLevel: 5,
        currentLevel: 5,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-017',
        name: 'Quality Assurance',
        type: 4, // Domain
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['João Santos', 'Maria Silva'],
  },
  {
    id: 'orch-colab-004',
    nomeColaborador: 'Daniel Ferreira',
    nomeCliente: 'Banco XYZ',
    nomeGestorCliente: 'Roberto Oliveira',
    perfil: 'DevOps Engineer',
    perfilId: 'perfil-004',
    nomeGestorOperacional: 'Carlos Mendes',
    nomeGestorAdm: 'Maria Costa',
    codigoInternoColaborador: '45678901234',
    codigoGestorAdm: '98765432100',
    idAlocacao: 1004,
    match: 75,
    retornoMatch: {
      hardSkills: 78,
      softSkills: 70,
      methodologies: 75,
      domains: 80,
      languages: 72,
    },
    resultadoHabilidades: [
      {
        id: 'skill-018',
        name: 'Docker',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-019',
        name: 'Kubernetes',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-020',
        name: 'Terraform',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-021',
        name: 'AWS',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-022',
        name: 'GitOps',
        type: 3, // Methodology
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-023',
        name: 'Infraestrutura como Código',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Carlos Mendes'],
  },
  {
    id: 'orch-colab-005',
    nomeColaborador: 'Eduarda Martins',
    nomeCliente: 'Tech Corp',
    nomeGestorCliente: 'Ana Paula Lima',
    perfil: 'UI/UX Designer',
    perfilId: 'perfil-005',
    nomeGestorOperacional: 'João Santos',
    nomeGestorAdm: 'Pedro Alves',
    codigoInternoColaborador: '56789012345',
    codigoGestorAdm: '87654321098',
    idAlocacao: 1005,
    match: 88,
    retornoMatch: {
      hardSkills: 90,
      softSkills: 92,
      methodologies: 85,
      domains: 88,
      languages: 85,
    },
    resultadoHabilidades: [
      {
        id: 'skill-024',
        name: 'Figma',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-025',
        name: 'Adobe XD',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-026',
        name: 'Design Systems',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-027',
        name: 'Prototipagem',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-028',
        name: 'Criatividade',
        type: 8, // Soft Skill
        requiredLevel: 4,
        currentLevel: 5,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-029',
        name: 'User Experience',
        type: 4, // Domain
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['João Santos'],
  },
  {
    id: 'orch-colab-006',
    nomeColaborador: 'Felipe Souza',
    nomeCliente: 'StartupXYZ',
    nomeGestorCliente: 'Marcos Silva',
    perfil: 'Desenvolvedor Mobile',
    perfilId: 'perfil-006',
    nomeGestorOperacional: 'Paula Ribeiro',
    nomeGestorAdm: 'Fernanda Lima',
    codigoInternoColaborador: '67890123456',
    codigoGestorAdm: '76543210987',
    idAlocacao: 1006,
    match: 62,
    retornoMatch: {
      hardSkills: 65,
      softSkills: 60,
      methodologies: 58,
      domains: 68,
      languages: 60,
    },
    resultadoHabilidades: [
      {
        id: 'skill-030',
        name: 'React Native',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-031',
        name: 'Flutter',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 1,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-032',
        name: 'Swift',
        type: 9, // Language
        requiredLevel: 3,
        currentLevel: 1,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-033',
        name: 'Kotlin',
        type: 9, // Language
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-034',
        name: 'Mobile Development',
        type: 4, // Domain
        requiredLevel: 4,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Paula Ribeiro'],
  },
  {
    id: 'orch-colab-007',
    nomeColaborador: 'Gabriela Costa',
    nomeCliente: 'Tech Corp',
    nomeGestorCliente: 'Ana Paula Lima',
    perfil: 'Analista de Dados',
    perfilId: 'perfil-007',
    nomeGestorOperacional: 'João Santos',
    nomeGestorAdm: 'Pedro Alves',
    codigoInternoColaborador: '78901234567',
    codigoGestorAdm: '87654321098',
    idAlocacao: 1007,
    match: 70,
    retornoMatch: {
      hardSkills: 72,
      softSkills: 68,
      methodologies: 70,
      domains: 75,
      languages: 65,
    },
    resultadoHabilidades: [
      {
        id: 'skill-035',
        name: 'Python',
        type: 9, // Language
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-036',
        name: 'Pandas',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-037',
        name: 'Machine Learning',
        type: 4, // Domain
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-038',
        name: 'Data Analytics',
        type: 4, // Domain
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-039',
        name: 'SQL',
        type: 9, // Language
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-040',
        name: 'Análise de Dados',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['João Santos'],
  },
  {
    id: 'orch-colab-008',
    nomeColaborador: 'Henrique Lima',
    nomeCliente: 'StartupXYZ',
    nomeGestorCliente: 'Marcos Silva',
    perfil: 'Desenvolvedor Frontend',
    perfilId: 'perfil-008',
    nomeGestorOperacional: 'Paula Ribeiro',
    nomeGestorAdm: 'Fernanda Lima',
    codigoInternoColaborador: '89012345678',
    codigoGestorAdm: '76543210987',
    idAlocacao: 1008,
    match: 55,
    retornoMatch: {
      hardSkills: 58,
      softSkills: 52,
      methodologies: 55,
      domains: 60,
      languages: 50,
    },
    resultadoHabilidades: [
      {
        id: 'skill-041',
        name: 'Vue.js',
        type: 1, // Hard Skill
        requiredLevel: 4,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-042',
        name: 'JavaScript',
        type: 9, // Language
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-043',
        name: 'CSS',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-044',
        name: 'HTML',
        type: 1, // Hard Skill
        requiredLevel: 3,
        currentLevel: 3,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-045',
        name: 'Responsive Design',
        type: 3, // Methodology
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Paula Ribeiro'],
  },
  {
    id: 'orch-colab-009',
    nomeColaborador: 'Isabela Rocha',
    nomeCliente: 'Fintech ABC',
    nomeGestorCliente: 'Lucas Pereira',
    perfil: 'Product Manager',
    perfilId: 'perfil-009',
    nomeGestorOperacional: 'Rafael Santos',
    nomeGestorAdm: 'Juliana Oliveira',
    codigoInternoColaborador: '90123456789',
    codigoGestorAdm: '65432109876',
    idAlocacao: 1009,
    match: 80,
    retornoMatch: {
      hardSkills: 75,
      softSkills: 88,
      methodologies: 82,
      domains: 78,
      languages: 77,
    },
    resultadoHabilidades: [
      {
        id: 'skill-046',
        name: 'Product Strategy',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-047',
        name: 'Liderança',
        type: 8, // Soft Skill
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-048',
        name: 'Comunicação Eficaz',
        type: 8, // Soft Skill
        requiredLevel: 5,
        currentLevel: 4,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-049',
        name: 'Product Management',
        type: 4, // Domain
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-050',
        name: 'Agile',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-051',
        name: 'Stakeholder Management',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 3,
        pendencia: true,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Rafael Santos'],
  },
  {
    id: 'orch-colab-010',
    nomeColaborador: 'João Pedro Alves',
    nomeCliente: 'Fintech ABC',
    nomeGestorCliente: 'Lucas Pereira',
    perfil: 'Tech Lead',
    perfilId: 'perfil-010',
    nomeGestorOperacional: 'Rafael Santos',
    nomeGestorAdm: 'Juliana Oliveira',
    codigoInternoColaborador: '01234567890',
    codigoGestorAdm: '65432109876',
    idAlocacao: 1010,
    match: 90,
    retornoMatch: {
      hardSkills: 92,
      softSkills: 88,
      methodologies: 90,
      domains: 91,
      languages: 89,
    },
    resultadoHabilidades: [
      {
        id: 'skill-052',
        name: 'System Architecture',
        type: 3, // Methodology
        requiredLevel: 5,
        currentLevel: 4,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-053',
        name: 'Cloud Architecture',
        type: 3, // Methodology
        requiredLevel: 5,
        currentLevel: 4,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-054',
        name: 'Liderança Técnica',
        type: 8, // Soft Skill
        requiredLevel: 5,
        currentLevel: 4,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-055',
        name: 'Mentoria',
        type: 8, // Soft Skill
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-056',
        name: 'Code Review',
        type: 3, // Methodology
        requiredLevel: 4,
        currentLevel: 4,
        pendencia: false,
        interesse: 1,
      },
      {
        id: 'skill-057',
        name: 'Arquitetura de Software',
        type: 4, // Domain
        requiredLevel: 5,
        currentLevel: 4,
        pendencia: true,
        interesse: 1,
      },
      {
        id: 'skill-058',
        name: 'Go',
        type: 9, // Language
        requiredLevel: 3,
        currentLevel: 2,
        pendencia: true,
        interesse: 1,
      },
    ],
    gestoresOperacionais: ['Rafael Santos', 'Carlos Mendes'],
  },
]

// Mock Suggestions for Orquestração Radar
export const mockOrquestracaoSugestoes: MinhaEquipeSugestao[] = [
  // Skills Pendentes (tbStatusSugestaoId: 2)
  {
    sugestaoId: 'orch-sug-001',
    nomeHabilidade: 'GraphQL',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '12345678901',
    perfilId: 'perfil-001',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Necessário para novo projeto do cliente',
    skillId: 1001,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-002',
    nomeHabilidade: 'Microservices Architecture',
    perfilTipoId: 3, // Methodology
    codigoInternoColaborador: '23456789012',
    perfilId: 'perfil-002',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Alinhado com arquitetura do projeto',
    skillId: 1003,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-003',
    nomeHabilidade: 'Kubernetes',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '45678901234',
    perfilId: 'perfil-004',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Requisito para migração de infraestrutura',
    skillId: 1005,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-004',
    nomeHabilidade: 'Swift',
    perfilTipoId: 9, // Language
    codigoInternoColaborador: '67890123456',
    perfilId: 'perfil-006',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Necessário para desenvolvimento iOS nativo',
    skillId: 1006,
    nivelId: 3,
    senioridadeNome: 'Pleno',
  },
  {
    sugestaoId: 'orch-sug-005',
    nomeHabilidade: 'Machine Learning',
    perfilTipoId: 4, // Domain
    codigoInternoColaborador: '78901234567',
    perfilId: 'perfil-007',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Interesse em expandir conhecimentos em IA',
    skillId: 1007,
    nivelId: 3,
    senioridadeNome: 'Pleno',
  },
  {
    sugestaoId: 'orch-sug-006',
    nomeHabilidade: 'Vue.js',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '89012345678',
    perfilId: 'perfil-008',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Requisito para novo projeto frontend',
    skillId: 1008,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-007',
    nomeHabilidade: 'Liderança de Equipes',
    perfilTipoId: 8, // Soft Skill
    codigoInternoColaborador: '90123456789',
    perfilId: 'perfil-009',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 6 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Desenvolvimento de habilidades de gestão',
    skillId: 1009,
    nivelId: 5,
    senioridadeNome: 'Especialista',
  },
  {
    sugestaoId: 'orch-sug-008',
    nomeHabilidade: 'Cloud Architecture',
    perfilTipoId: 3, // Methodology
    codigoInternoColaborador: '01234567890',
    perfilId: 'perfil-010',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 8 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Necessário para liderança técnica em projetos cloud',
    skillId: 1010,
    nivelId: 5,
    senioridadeNome: 'Especialista',
  },
  {
    sugestaoId: 'orch-sug-009',
    nomeHabilidade: 'Redis',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '12345678901',
    perfilId: 'perfil-001',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Para otimização de cache em alta performance',
    skillId: 1011,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-010',
    nomeHabilidade: 'TypeScript Avançado',
    perfilTipoId: 9, // Language
    codigoInternoColaborador: '23456789012',
    perfilId: 'perfil-002',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 9 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Melhorar qualidade de código e type safety',
    skillId: 1012,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-011',
    nomeHabilidade: 'Testes Automatizados',
    perfilTipoId: 3, // Methodology
    codigoInternoColaborador: '34567890123',
    perfilId: 'perfil-003',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 12 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Implementar estratégias de testes end-to-end',
    skillId: 1013,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-012',
    nomeHabilidade: 'Terraform',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '45678901234',
    perfilId: 'perfil-004',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 11 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Infraestrutura como código para automação',
    skillId: 1014,
    nivelId: 3,
    senioridadeNome: 'Pleno',
  },
  {
    sugestaoId: 'orch-sug-013',
    nomeHabilidade: 'Design Systems',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '56789012345',
    perfilId: 'perfil-005',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 13 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Criar sistema de design consistente para produtos',
    skillId: 1015,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-014',
    nomeHabilidade: 'Flutter',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '67890123456',
    perfilId: 'perfil-006',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Desenvolvimento cross-platform mobile',
    skillId: 1016,
    nivelId: 3,
    senioridadeNome: 'Pleno',
  },
  {
    sugestaoId: 'orch-sug-015',
    nomeHabilidade: 'Data Analytics',
    perfilTipoId: 4, // Domain
    codigoInternoColaborador: '78901234567',
    perfilId: 'perfil-007',
    tbStatusSugestaoId: 2, // Pendente
    dataSugestao: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Análise de dados para tomada de decisão',
    skillId: 1017,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  // Skills Aprovadas (tbStatusSugestaoId: 1)
  {
    sugestaoId: 'orch-sug-016',
    nomeHabilidade: 'AWS Solutions Architect',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '12345678901',
    perfilId: 'perfil-001',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 20 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Certificação alinhada com estratégia cloud',
    skillId: 1018,
    nivelId: 5,
    senioridadeNome: 'Especialista',
  },
  {
    sugestaoId: 'orch-sug-017',
    nomeHabilidade: 'Scrum Master',
    perfilTipoId: 3, // Methodology
    codigoInternoColaborador: '23456789012',
    perfilId: 'perfil-002',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 18 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Necessário para liderança de equipes ágeis',
    skillId: 1019,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-018',
    nomeHabilidade: 'Cypress',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '34567890123',
    perfilId: 'perfil-003',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 16 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Ferramenta essencial para testes E2E',
    skillId: 1020,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-019',
    nomeHabilidade: 'GitOps',
    perfilTipoId: 3, // Methodology
    codigoInternoColaborador: '45678901234',
    perfilId: 'perfil-004',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 19 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Prática moderna de DevOps',
    skillId: 1021,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  {
    sugestaoId: 'orch-sug-020',
    nomeHabilidade: 'Comunicação Eficaz',
    perfilTipoId: 8, // Soft Skill
    codigoInternoColaborador: '90123456789',
    perfilId: 'perfil-009',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 17 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Fundamental para Product Management',
    skillId: 1022,
    nivelId: 5,
    senioridadeNome: 'Especialista',
  },
  {
    sugestaoId: 'orch-sug-021',
    nomeHabilidade: 'System Design',
    perfilTipoId: 3, // Methodology
    codigoInternoColaborador: '01234567890',
    perfilId: 'perfil-010',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 21 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Essencial para Tech Lead',
    skillId: 1023,
    nivelId: 5,
    senioridadeNome: 'Especialista',
  },
  {
    sugestaoId: 'orch-sug-022',
    nomeHabilidade: 'MongoDB',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '12345678901',
    perfilId: 'perfil-001',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 22 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Banco NoSQL para novos projetos',
    skillId: 1024,
    nivelId: 3,
    senioridadeNome: 'Pleno',
  },
  {
    sugestaoId: 'orch-sug-023',
    nomeHabilidade: 'Elasticsearch',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '23456789012',
    perfilId: 'perfil-002',
    tbStatusSugestaoId: 1, // Aprovado
    dataSugestao: new Date(Date.now() - 23 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Aprovado - Busca e análise de logs em escala',
    skillId: 1025,
    nivelId: 4,
    senioridadeNome: 'Sênior',
  },
  // Skills Rejeitadas (tbStatusSugestaoId: 3)
  {
    sugestaoId: 'orch-sug-024',
    nomeHabilidade: 'PHP Legacy',
    perfilTipoId: 9, // Language
    codigoInternoColaborador: '89012345678',
    perfilId: 'perfil-008',
    tbStatusSugestaoId: 3, // Rejeitado
    dataSugestao: new Date(Date.now() - 25 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Rejeitado - Tecnologia não alinhada com stack atual',
    skillId: 1026,
    nivelId: 2,
    senioridadeNome: 'Júnior',
  },
  {
    sugestaoId: 'orch-sug-025',
    nomeHabilidade: 'Flash',
    perfilTipoId: 1, // Hard Skill
    codigoInternoColaborador: '56789012345',
    perfilId: 'perfil-005',
    tbStatusSugestaoId: 3, // Rejeitado
    dataSugestao: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
    observacao: 'Rejeitado - Tecnologia obsoleta',
    skillId: 1027,
    nivelId: 1,
    senioridadeNome: 'Iniciante',
  },
]

// Mock PDI Metas for Orquestração
export const mockOrquestracaoPDIMetas: MinhaEquipePDI[] = [
  {
    id: 1,
    skillName: 'TypeScript Avançado',
    deadline: new Date(Date.now() + 90 * 24 * 60 * 60 * 1000).toISOString(),
    status: 'em_andamento',
    actions: 'Curso online na Udemy + projeto prático interno',
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString(),
  },
  {
    id: 2,
    skillName: 'AWS Solutions Architect',
    deadline: new Date(Date.now() + 180 * 24 * 60 * 60 * 1000).toISOString(),
    status: 'em_andamento',
    actions: 'Certificação AWS + hands-on com projetos da empresa',
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString(),
  },
  {
    id: 3,
    skillName: 'Docker & Kubernetes',
    deadline: new Date(Date.now() + 120 * 24 * 60 * 60 * 1000).toISOString(),
    status: 'em_andamento',
    actions: 'Curso Kubernetes for Developers + laboratório prático',
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString(),
  },
]
