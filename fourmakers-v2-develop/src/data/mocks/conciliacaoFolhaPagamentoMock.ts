export interface DivergenciaMock {
  id: number
  descricao: string
  valorContabilidade: number
  valorEsperado: number
  mensagem: string
  status: "ok" | "pendente"
  /** Fórmula de cálculo (ex.: "Valor esperado = Salário Base x (220.00 / 220)") */
  formula?: string
  /** Passos do cálculo para exibir no modal de detalhes */
  passos?: string[]
  /** Variáveis utilizadas (ex.: horas_consideradas, salario_base) */
  variaveis?: Record<string, string | number>
}

export interface ColaboradorConciliacaoMock {
  id: number
  nome: string
  cargo: string
  divergencias: DivergenciaMock[]
  /** URL do holerite com placeholder $1 para o token (base64). */
  holeritePath?: string
  /** URL da folha ponto com placeholder $1 para o token (base64). */
  folhaPontoPath?: string
}

export interface LoteConciliacaoMock {
  /** ID numérico (mock) ou UUID string (API BuscarLotes) */
  id: number | string
  competencia: string
  empresa: string
  cnpj: string
  processadoErro: string
  statusLote: string
  statusConciliacao: string
  total: number
}

export const lotesConciliacaoMock: LoteConciliacaoMock[] = [
  {
    id: 1,
    competencia: "12/2024",
    empresa: "FourMakers Tecnologia",
    cnpj: "12.345.678/0001-90",
    processadoErro: "150/150",
    statusLote: "Processado",
    statusConciliacao: "Aprovado",
    total: 26,
  },
  {
    id: 2,
    competencia: "11/2024",
    empresa: "FourMakers Consultoria",
    cnpj: "98.765.432/0001-10",
    processadoErro: "180/200",
    statusLote: "Erro",
    statusConciliacao: "Pendente",
    total: 30,
  },
  {
    id: 3,
    competencia: "10/2024",
    empresa: "FourMakers Serviços",
    cnpj: "11.222.333/0001-44",
    processadoErro: "175/175",
    statusLote: "Processado",
    statusConciliacao: "Pendente",
    total: 25,
  },
]

export const colaboradoresConciliacaoMock: ColaboradorConciliacaoMock[] = [
  {
    id: 1,
    nome: "DANIEL DA LUZ",
    cargo: "Conferente III",
    holeritePath:
      "https://fourmakershub-api.dev.fourmakers.io/api/Archive/$1/holerite/paginas/def0d0bd-97c1-4342-9440-8c6e6009c0b1_31_20250908211511081.pdf",
    folhaPontoPath:
      "https://fourmakershub-api.dev.fourmakers.io/api/Archive/$1/folhaponto/paginas/4ba34264-2ef8-49b4-aec4-3ed6807af811_29_20250908190803653.pdf",
    divergencias: [
      {
        id: 1,
        descricao: "Horas Extras 50% Diurnas",
        valorContabilidade: 393.46,
        valorEsperado: 393.46,
        mensagem: "Valores são idênticos.",
        status: "ok",
        formula: "Valor esperado = Salário Base x (horas_consideradas / horas_mensal_padrao)",
        passos: [
          "Referência do item: 220:00 hs → 220.00 h",
          "Salário base: R$ 3071.25",
          "Carga mensal padrão: 220 h",
          "Valor esperado proporcional: R$ 393.46",
          "Valor no holerite: R$ 393.46",
        ],
        variaveis: {
          horas_consideradas: 220.0,
          horas_mensal_padrao: 220,
          salario_base: "R$ 3071.25",
          valor_esperado: "R$ 393.46",
        },
      },
      {
        id: 2,
        descricao: "Empréstimo Crédito do Trabalhador",
        valorContabilidade: 557.52,
        valorEsperado: 557.52,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
    ],
  },
  {
    id: 2,
    nome: "LUIZ MIGUEL DA PAZ",
    cargo: "Estoquista III",
    divergencias: [
      {
        id: 1,
        descricao: "Mensalidade Plano de Saúde",
        valorContabilidade: 249.31,
        valorEsperado: 249.31,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
    ],
  },
  {
    id: 3,
    nome: "LUIGI PORTO",
    cargo: "Auxiliar de serviços Gerais III",
    divergencias: [],
  },
  {
    id: 4,
    nome: "MARIA VITÓRIA CARVALHO",
    cargo: "Estoquista III",
    divergencias: [],
  },
  {
    id: 5,
    nome: "MARIA ALICE RAMOS",
    cargo: "Assistente de Faturamento III",
    divergencias: [
      {
        id: 1,
        descricao: "INSS",
        valorContabilidade: 254.79,
        valorEsperado: 254.79,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
      {
        id: 2,
        descricao: "Vale Transporte",
        valorContabilidade: 180.00,
        valorEsperado: 180.00,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
    ],
  },
  {
    id: 6,
    nome: "LUCCA MOREIRA",
    cargo: "Estoquista III",
    divergencias: [
      {
        id: 1,
        descricao: "Horas Extras 100%",
        valorContabilidade: 450.00,
        valorEsperado: 450.00,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
      {
        id: 2,
        descricao: "Adicional Noturno",
        valorContabilidade: 320.00,
        valorEsperado: 320.00,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
      {
        id: 3,
        descricao: "Vale Alimentação",
        valorContabilidade: 500.00,
        valorEsperado: 500.00,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
    ],
  },
  {
    id: 7,
    nome: "JUAN RIBEIRO",
    cargo: "Conferente III",
    divergencias: [
      {
        id: 1,
        descricao: "Salário Base",
        valorContabilidade: 2500.00,
        valorEsperado: 2500.00,
        mensagem: "Valores são idênticos.",
        status: "ok",
      },
    ],
  },
]

