export interface NotaFiscalMock {
  id: number
  vigencia: string
  mes: string
  valorTotal: number
  status: "Aprovada" | "Pendente" | "Emitida" | "Cancelada"
  detalhes: {
    cliente: string
    projeto: string
    dataEmissao?: string
    numeroNF?: string
  }
}

export interface NotaFiscalGestaoMock {
  id: number
  prestador: string
  vigencia: string
  numeroNF: string
  valor: number
  dataEnvio: string
  status: string
  competencia: string
  dataEmissao: string
  lancamentos: Array<{
    rubrica: string
    valor: number
  }>
}

export interface ColaboradorOptionMock {
  id: string
  nome: string
}

export interface UnidadeOptionMock {
  id: string
  nome: string
}

export const notasFiscaisMock: NotaFiscalMock[] = [
  {
    id: 1,
    vigencia: "Outubro de 2025",
    mes: "10/2025",
    valorTotal: 930.00,
    status: "Aprovada",
    detalhes: {
      cliente: "Cliente XYZ Ltda",
      projeto: "Projeto Alpha",
      dataEmissao: "15/10/2025",
      numeroNF: "12345",
    },
  },
  {
    id: 2,
    vigencia: "Setembro de 2025",
    mes: "09/2025",
    valorTotal: 1250.50,
    status: "Emitida",
    detalhes: {
      cliente: "Empresa ABC S.A.",
      projeto: "Projeto Beta",
      dataEmissao: "20/09/2025",
      numeroNF: "12344",
    },
  },
  {
    id: 3,
    vigencia: "Agosto de 2025",
    mes: "08/2025",
    valorTotal: 875.00,
    status: "Pendente",
    detalhes: {
      cliente: "Companhia DEF",
      projeto: "Projeto Gamma",
    },
  },
]

export const notasFiscaisGestaoMock: NotaFiscalGestaoMock[] = [
  {
    id: 1,
    prestador: "Luiz Gustavo Jesus",
    vigencia: "10/2025",
    numeroNF: "1500",
    valor: 930.00,
    dataEnvio: "19/09/2025",
    status: "Aprovada",
    competencia: "10/2025",
    dataEmissao: "19/09/2025",
    lancamentos: [
      { rubrica: "UNIMED", valor: 250.00 },
      { rubrica: "Salario V", valor: 680.00 },
    ],
  },
  {
    id: 2,
    prestador: "Maria Santos",
    vigencia: "10/2025",
    numeroNF: "1501",
    valor: 1250.50,
    dataEnvio: "20/09/2025",
    status: "Pendente",
    competencia: "10/2025",
    dataEmissao: "20/09/2025",
    lancamentos: [
      { rubrica: "Salario Base", valor: 1000.00 },
      { rubrica: "Vale Transporte", valor: 250.50 },
    ],
  },
  {
    id: 3,
    prestador: "Pedro Oliveira",
    vigencia: "09/2025",
    numeroNF: "1499",
    valor: 875.00,
    dataEnvio: "15/09/2025",
    status: "Rejeitada",
    competencia: "09/2025",
    dataEmissao: "15/09/2025",
    lancamentos: [
      { rubrica: "Salario Base", valor: 875.00 },
    ],
  },
]

export const colaboradoresOptionMock: ColaboradorOptionMock[] = [
  { id: "1", nome: "Luiz Gustavo Jesus" },
  { id: "2", nome: "Maria Santos" },
  { id: "3", nome: "Pedro Oliveira" },
]

export const unidadesOptionMock: UnidadeOptionMock[] = [
  { id: "1", nome: "Matriz" },
  { id: "2", nome: "Filial SP" },
  { id: "3", nome: "Filial RJ" },
]

