import type { LogEntry } from '@shared/types/reembolso'
import type { VerbaData } from '@shared/types/reembolso'

export const logsMock: LogEntry[] = [
  {
    regra: "Limite de Envio",
    acao: "atualizar",
    de: "0",
    para: "20",
    alteradoEm: "08/05/2025 01:07:44",
    alteradoPor: "Thiago da Cruz",
  },
  {
    regra: "Dia de Pagamento",
    acao: "atualizar",
    de: "0",
    para: "30",
    alteradoEm: "08/05/2025 01:07:44",
    alteradoPor: "Thiago da Cruz",
  },
  {
    regra: "Categoria",
    acao: "atualizar",
    de: "Devolução",
    para: "Alimentação",
    alteradoEm: "07/28/2025 17:54:57",
    alteradoPor: "Samuel Manoel Da Silva",
  },
  {
    regra: "Valor",
    acao: "atualizar",
    de: "500",
    para: "50,00",
    alteradoEm: "07/28/2025 17:54:57",
    alteradoPor: "Samuel Manoel Da Silva",
  },
  {
    regra: "Categoria",
    acao: "atualizar",
    de: "Adiantamento",
    para: "Viagem",
    alteradoEm: "07/28/2025 17:54:57",
    alteradoPor: "Samuel Manoel Da Silva",
  },
]

export const verbasInicialMock: VerbaData[] = [
  {
    id: "1",
    isActive: true,
    tipoCusto: "debito",
    nomeCategoria: "Devolução",
    unidade: "",
    valor: 500.00,
    isCustoCliente: true,
  },
  {
    id: "2",
    isActive: true,
    tipoCusto: "credito",
    nomeCategoria: "Adiantamento",
    unidade: "",
    valor: 1000.00,
    isCustoCliente: true,
  },
  {
    id: "3",
    isActive: true,
    tipoCusto: "fixa",
    nomeCategoria: "KM RODADO",
    unidade: "1",
    valor: 5.00,
    isCustoCliente: true,
  },
  {
    id: "4",
    isActive: true,
    tipoCusto: "variavel",
    nomeCategoria: "Farmácia",
    unidade: "",
    valor: 100.00,
    isCustoCliente: true,
  },
]

