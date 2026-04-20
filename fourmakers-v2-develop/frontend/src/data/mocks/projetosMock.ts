export interface ProjetoMock {
  id: number
  projeto: string
  cliente: string
  aprovadores: string[]
  oportunidade: string
  dataInicio: string
  dataFim: string
  status: string
}

export const projetosMock: ProjetoMock[] = [
  {
    id: 1,
    projeto: "afasgasr - 46352",
    cliente: "10 - Batista, Carvalho and Franco",
    aprovadores: ["null", "+1"],
    oportunidade: "-",
    dataInicio: "03/08/2025",
    dataFim: "03/08/2025",
    status: "Proposta",
  },
  {
    id: 2,
    projeto: "kuuiygiuy - 55",
    cliente: "107 - CSU Cardsystem",
    aprovadores: ["null", "+1"],
    oportunidade: "-",
    dataInicio: "11/08/2025",
    dataFim: "11/08/2025",
    status: "Cancelado",
  },
  {
    id: 3,
    projeto: "2493 - a amet",
    cliente: "2493 - Martins, Carvalho and Barros",
    aprovadores: ["Felipe Oli..."],
    oportunidade: "-",
    dataInicio: "08/02/2024",
    dataFim: "24/04/2024",
    status: "Proposta",
  },
  {
    id: 4,
    projeto: "3687 - a aperiam",
    cliente: "3687 - Moraes, Moraes and Reis",
    aprovadores: ["Anthony Reis"],
    oportunidade: "-",
    dataInicio: "18/02/2024",
    dataFim: "18/12/2024",
    status: "Desenvolvimento",
  },
  {
    id: 5,
    projeto: "3337 - a cumque",
    cliente: "3337 - Nogueira, Santos and Franco",
    aprovadores: ["Tertuliano..."],
    oportunidade: "-",
    dataInicio: "27/01/2024",
    dataFim: "28/06/2025",
    status: "Proposta",
  },
]

