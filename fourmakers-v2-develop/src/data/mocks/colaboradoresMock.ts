export interface ColaboradorMock {
  id: number
  nome: string
  email: string
  telefone: string
  departamento: string
  cargo: string
  situacao: string
  avatar: string
}

export const colaboradoresMock: ColaboradorMock[] = [
  {
    id: 1,
    nome: "Samuel Manoel Da Silva",
    email: "samuel.silva@empresa.com",
    telefone: "(11) 98765-4321",
    departamento: "Ferramentas",
    cargo: "Desenvolvedor Senior",
    situacao: "Ativo",
    avatar: "/placeholder.svg",
  },
  {
    id: 2,
    nome: "Joao Vitor Batista da Silva",
    email: "joao.batista@empresa.com",
    telefone: "(11) 98765-4322",
    departamento: "Desenvolvimento",
    cargo: "Desenvolvedor Pleno",
    situacao: "Ativo",
    avatar: "/placeholder.svg",
  },
  {
    id: 3,
    nome: "Luan Kaique Rodrigues Ribeiro",
    email: "luan.ribeiro@empresa.com",
    telefone: "(11) 98765-4323",
    departamento: "QA",
    cargo: "Analista de Qualidade",
    situacao: "Ativo",
    avatar: "/placeholder.svg",
  },
  {
    id: 4,
    nome: "Isadora Nogueira",
    email: "isadora.nogueira@empresa.com",
    telefone: "(11) 98765-4324",
    departamento: "Gestão",
    cargo: "Gestora ADM",
    situacao: "Ativo",
    avatar: "/placeholder.svg",
  },
]

