import type { Professional } from '@domain/entities/comunicacao';

// ============================================
// MOCK DATA - PROFISSIONAIS
// ============================================

export const mockProfessionals: Professional[] = [
  {
    id: 'prof1',
    name: 'Ana Silva',
    position: 'Gerente de Projetos',
    unit: 'TI',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ana',
    coverImage: 'https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&h=200&fit=crop',
    about:
      'Profissional com mais de 10 anos de experiência em gestão de projetos de TI. Especialista em metodologias ágeis e transformação digital. Apaixonada por inovação e desenvolvimento de equipes de alta performance.',
    email: 'ana.silva@fourmakers.com',
    phone: '(11) 98765-4321',
    birthDate: '1990-02-04',
  },
  {
    id: 'prof2',
    name: 'Carlos Santos',
    position: 'Analista de RH',
    unit: 'Recursos Humanos',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Carlos',
    coverImage: 'https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=800&h=200&fit=crop',
    about:
      'Especialista em desenvolvimento humano e organizacional. Foco em cultura organizacional, recrutamento e seleção. Certificado em coaching e gestão de talentos.',
    email: 'carlos.santos@fourmakers.com',
    phone: '(11) 98765-4322',
    birthDate: '1985-03-15',
  },
  {
    id: 'prof3',
    name: 'Mariana Costa',
    position: 'Desenvolvedora Senior',
    unit: 'TI',
    managerName: 'Ana Silva',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mariana',
    coverImage: 'https://images.unsplash.com/photo-1497366754035-f200968a6e72?w=800&h=200&fit=crop',
    about:
      'Desenvolvedora full-stack com expertise em React, Node.js e arquitetura de sistemas. Mentora de desenvolvedores júnior e entusiasta de código limpo e boas práticas.',
    email: 'mariana.costa@fourmakers.com',
    phone: '(11) 98765-4323',
  },
  {
    id: 'prof4',
    name: 'João Oliveira',
    position: 'Coordenador Financeiro',
    unit: 'Financeiro',
    managerName: 'Roberto Mendes',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Joao',
    coverImage: 'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?w=800&h=200&fit=crop',
    about:
      'Profissional de finanças com foco em planejamento estratégico e controle orçamentário. MBA em Finanças Corporativas e experiência em gestão de custos.',
    email: 'joao.oliveira@fourmakers.com',
    phone: '(11) 98765-4324',
  },
  {
    id: 'prof5',
    name: 'Beatriz Lima',
    position: 'Designer UX/UI',
    unit: 'Design',
    managerName: 'Luciana Dias',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Beatriz',
    coverImage: 'https://images.unsplash.com/photo-1497366858526-0766cadbe8fa?w=800&h=200&fit=crop',
    about:
      'Designer focada em experiência do usuário e interfaces intuitivas. Especialista em design thinking e prototipagem. Trabalha para criar produtos digitais que as pessoas amam usar.',
    email: 'beatriz.lima@fourmakers.com',
    phone: '(11) 98765-4325',
  },
  {
    id: 'prof6',
    name: 'Rafael Souza',
    position: 'Gerente de Vendas',
    unit: 'Comercial',
    managerName: 'Patricia Gomes',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Rafael',
    coverImage: 'https://images.unsplash.com/photo-1497366412874-3415097a27e7?w=800&h=200&fit=crop',
    about:
      'Profissional de vendas com histórico comprovado de superação de metas. Especialista em vendas consultivas e gestão de relacionamento com clientes corporativos.',
    email: 'rafael.souza@fourmakers.com',
    phone: '(11) 98765-4326',
  },
  {
    id: 'prof7',
    name: 'Juliana Ferreira',
    position: 'Analista de Marketing',
    unit: 'Marketing',
    managerName: 'Patricia Gomes',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Juliana',
    coverImage: 'https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&h=200&fit=crop',
    about:
      'Especialista em marketing digital e gestão de mídias sociais. Experiência em campanhas de performance, SEO e criação de conteúdo estratégico.',
    email: 'juliana.ferreira@fourmakers.com',
    phone: '(11) 98765-4327',
  },
  {
    id: 'prof8',
    name: 'Pedro Almeida',
    position: 'Desenvolvedor Frontend',
    unit: 'TI',
    managerName: 'Ana Silva',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Pedro',
    coverImage: 'https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=800&h=200&fit=crop',
    about:
      'Desenvolvedor especializado em interfaces modernas e responsivas. Expert em React, TypeScript e animações web. Apaixonado por criar experiências visuais incríveis.',
    email: 'pedro.almeida@fourmakers.com',
    phone: '(11) 98765-4328',
  },
  {
    id: 'prof9',
    name: 'Fernanda Rodrigues',
    position: 'Coordenadora de RH',
    unit: 'Recursos Humanos',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fernanda',
    coverImage: 'https://images.unsplash.com/photo-1497366754035-f200968a6e72?w=800&h=200&fit=crop',
    about:
      'Líder de RH focada em desenvolvimento organizacional e bem-estar dos colaboradores. Implementadora de programas de diversidade e inclusão.',
    email: 'fernanda.rodrigues@fourmakers.com',
    phone: '(11) 98765-4329',
  },
  {
    id: 'prof10',
    name: 'Lucas Martins',
    position: 'Analista de Dados',
    unit: 'TI',
    avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Lucas',
    coverImage: 'https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?w=800&h=200&fit=crop',
    about:
      'Cientista de dados com foco em análise preditiva e business intelligence. Especialista em Python, SQL e visualização de dados. Transforma dados em insights acionáveis.',
    email: 'lucas.martins@fourmakers.com',
    phone: '(11) 98765-4330',
  },
];

export const uniqueUnits = Array.from(new Set(mockProfessionals.map((p) => p.unit))).sort();
export const uniquePositions = Array.from(new Set(mockProfessionals.map((p) => p.position))).sort();
