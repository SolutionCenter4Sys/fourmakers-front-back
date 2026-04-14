export interface ExperienceMock {
  id: string
  company: string
  position: string
  startDate: string
  endDate: string | null
  current: boolean
  activities: string[]
}

export interface SkillCategoryMock {
  title: string
  icon: any
  skills: string[]
  color: string
}

export interface EducationMock {
  id: string
  degree: string
  institution: string
  level: string
  startDate: string
  endDate: string | null
  current: boolean
  hasAttachment: boolean
}

export interface CertificationMock {
  id: string
  name: string
  issuer: string
  issueDate: string
  workload: string
  hasAttachment: boolean
}

export interface VisaMock {
  id: string
  country: string
  expiryDate: string
}

export interface PassportMock {
  id: string
  nationality: string
  expiryDate: string
}

export const experiencesMock: ExperienceMock[] = [
  {
    id: "1",
    company: "Tech Corp",
    position: "Desenvolvedor Senior",
    startDate: "2020-01",
    endDate: null,
    current: true,
    activities: [
      "Liderança técnica de projetos",
      "Desenvolvimento de arquitetura de sistemas",
      "Mentoria de desenvolvedores júnior",
    ],
  },
  {
    id: "2",
    company: "Innovation Labs",
    position: "Desenvolvedor Pleno",
    startDate: "2017-03",
    endDate: "2019-12",
    current: false,
    activities: [
      "Desenvolvimento full-stack",
      "Implementação de APIs RESTful",
      "Otimização de performance",
    ],
  },
]

export const educationsMock: EducationMock[] = [
  {
    id: "1",
    degree: "Ciência da Computação",
    institution: "Universidade de São Paulo",
    level: "Ensino Superior Completo",
    startDate: "2012-02",
    endDate: "2016-12",
    current: false,
    hasAttachment: true,
  },
  {
    id: "2",
    degree: "MBA em Gestão de Projetos",
    institution: "FGV",
    level: "MBA",
    startDate: "2020-03",
    endDate: null,
    current: true,
    hasAttachment: false,
  },
]

export const certificationsMock: CertificationMock[] = [
  {
    id: "1",
    name: "AWS Solutions Architect",
    issuer: "Amazon Web Services",
    issueDate: "2023-06",
    workload: "40h",
    hasAttachment: true,
  },
  {
    id: "2",
    name: "Scrum Master Certified",
    issuer: "Scrum Alliance",
    issueDate: "2022-11",
    workload: "16h",
    hasAttachment: true,
  },
]

export const visasMock: VisaMock[] = [
  { id: "1", country: "Estados Unidos", expiryDate: "2028-03-15" },
  { id: "2", country: "União Europeia", expiryDate: "2027-11-20" },
]

export const passportsMock: PassportMock[] = [
  { id: "1", nationality: "Brasil", expiryDate: "2030-06-10" },
]

export const skillCategoriesMock: SkillCategoryMock[] = [
  {
    title: "Técnicas",
    icon: "Code",
    skills: ["React", "TypeScript", "Node.js", "Python", "PostgreSQL", "Docker"],
    color: "border-blue-300 bg-blue-50 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:bg-blue-950 dark:text-blue-300",
  },
  {
    title: "Socioemocionais",
    icon: "Heart",
    skills: ["Liderança", "Comunicação", "Trabalho em equipe", "Resolução de problemas"],
    color: "border-blue-300 bg-blue-50 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:bg-blue-950 dark:text-blue-300",
  },
  {
    title: "Idiomas",
    icon: "Globe",
    skills: ["Português (Nativo)", "Inglês (Avançado)", "Espanhol (Intermediário)"],
    color: "border-blue-300 bg-blue-50 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:bg-blue-950 dark:text-blue-300",
  },
  {
    title: "Metodologias",
    icon: "Layers",
    skills: ["Scrum", "Kanban", "Design Thinking", "DevOps"],
    color: "border-blue-300 bg-blue-50 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:bg-blue-950 dark:text-blue-300",
  },
  {
    title: "Domínios",
    icon: "Briefcase",
    skills: ["E-commerce", "Fintech", "HR Tech", "SaaS"],
    color: "border-blue-300 bg-blue-50 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:bg-blue-950 dark:text-blue-300",
  },
]

