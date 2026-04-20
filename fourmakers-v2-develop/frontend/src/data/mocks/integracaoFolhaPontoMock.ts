export interface LoteProcessadoMock {
  id: number
  competencia: string
  empresa: string
  cnpj: string
  arquivo: string
  qtdPag: number
  status: string
  processadoErro: string
}

export interface ColaboradorDetalheMock {
  id: number
  colaborador: string
  cargo: string
  cpf: string
  matricula: string
  arquivo: string
  erro?: string
}

export const lotesProcessadosMock: LoteProcessadoMock[] = [
  {
    id: 1,
    competencia: "09/2025",
    empresa: "Empresa A",
    cnpj: "12.345.678/0001-90",
    arquivo: "folha_setembro_2025.xlsx",
    qtdPag: 150,
    status: "processado",
    processadoErro: "150/150",
  },
  {
    id: 2,
    competencia: "08/2025",
    empresa: "Empresa B",
    cnpj: "98.765.432/0001-10",
    arquivo: "folha_agosto_2025.xlsx",
    qtdPag: 200,
    status: "erro",
    processadoErro: "180/200",
  },
  {
    id: 3,
    competencia: "07/2025",
    empresa: "Empresa C",
    cnpj: "11.222.333/0001-44",
    arquivo: "folha_julho_2025.xlsx",
    qtdPag: 175,
    status: "processando",
    processadoErro: "120/175",
  },
]

export const colaboradoresDetalheMock: Record<number, ColaboradorDetalheMock[]> = {
  1: [
    {
      id: 1,
      colaborador: "João Silva",
      cargo: "Desenvolvedor",
      cpf: "123.456.789-00",
      matricula: "12345",
      arquivo: "folha_setembro_2025.xlsx",
      erro: "",
    },
    {
      id: 2,
      colaborador: "Maria Santos",
      cargo: "Analista",
      cpf: "987.654.321-00",
      matricula: "12346",
      arquivo: "folha_setembro_2025.xlsx",
      erro: "",
    },
  ],
  2: [
    {
      id: 1,
      colaborador: "Pedro Costa",
      cargo: "Gerente",
      cpf: "111.222.333-44",
      matricula: "54321",
      arquivo: "folha_agosto_2025.xlsx",
      erro: "Dados incompletos",
    },
    {
      id: 2,
      colaborador: "Ana Oliveira",
      cargo: "Coordenador",
      cpf: "555.666.777-88",
      matricula: "54322",
      arquivo: "folha_agosto_2025.xlsx",
      erro: "",
    },
  ],
  3: [
    {
      id: 1,
      colaborador: "Carlos Ferreira",
      cargo: "Técnico",
      cpf: "222.333.444-55",
      matricula: "98765",
      arquivo: "folha_julho_2025.xlsx",
      erro: "",
    },
  ],
}

