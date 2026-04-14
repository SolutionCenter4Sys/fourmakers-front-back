export interface RetornoContabilMock {
  id: number
  competencia: string
  empresa: string
  cnpj: string
  arquivo: string
  registros: number
  status: string
}

export interface ColaboradorRetornoMock {
  id: number
  nome: string
  cargo: string
  cpf: string
  matricula: string
  arquivo: string
  erro: string | null
}

export const retornosContabilMock: RetornoContabilMock[] = [
  {
    id: 1,
    competencia: "12/2024",
    empresa: "FourMakers Tecnologia",
    cnpj: "12.345.678/0001-90",
    arquivo: "retorno_12_2024.zip",
    registros: 45,
    status: "Processado",
  },
  {
    id: 2,
    competencia: "11/2024",
    empresa: "FourMakers Consultoria",
    cnpj: "98.765.432/0001-10",
    arquivo: "retorno_11_2024.zip",
    registros: 38,
    status: "Pendente",
  },
  {
    id: 3,
    competencia: "10/2024",
    empresa: "FourMakers Tecnologia",
    cnpj: "12.345.678/0001-90",
    arquivo: "retorno_10_2024.zip",
    registros: 42,
    status: "Erro",
  },
]

export const colaboradoresRetornoMock: ColaboradorRetornoMock[] = [
  {
    id: 1,
    nome: "João Silva",
    cargo: "Desenvolvedor",
    cpf: "123.456.789-00",
    matricula: "MAT001",
    arquivo: "holerite_joao.pdf",
    erro: null,
  },
  {
    id: 2,
    nome: "Maria Santos",
    cargo: "Analista",
    cpf: "987.654.321-00",
    matricula: "MAT002",
    arquivo: "holerite_maria.pdf",
    erro: "Arquivo corrompido",
  },
  {
    id: 3,
    nome: "Pedro Costa",
    cargo: "Designer",
    cpf: "456.789.123-00",
    matricula: "MAT003",
    arquivo: "holerite_pedro.pdf",
    erro: null,
  },
]

