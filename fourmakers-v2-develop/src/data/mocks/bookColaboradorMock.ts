import type { Equipe, ColaboradorBook, ColaboradorDetalhes, FeriasInfo } from "@shared/types/bookColaborador";

export const mockEquipe: Equipe = {
  id: 1,
  nome: "Equipe de Marketing",
};

export const mockColaboradores: ColaboradorBook[] = [
  {
    id: "1",
    codColaborador: "COL001",
    nome: "João Silva",
    cargo: "Desenvolvedor",
    modelo: "CLT",
    saldoHoras: 8.5,
    salario: 5000,
    status: "Ativo",
    email: "joao.silva@example.com",
    telefone: "11987654321",
    nascimento: "1990-05-15",
    tempoCasa: "2 anos",
  },
  {
    id: "2",
    codColaborador: "COL002",
    nome: "Maria Santos",
    cargo: "Designer",
    modelo: "PJ",
    saldoHoras: -2.0,
    salario: 4500,
    status: "Ativo",
    email: "maria.santos@example.com",
    telefone: "11976543210",
    nascimento: "1992-08-20",
    tempoCasa: "1 ano",
  },
  {
    id: "3",
    codColaborador: "COL003",
    nome: "Pedro Oliveira",
    cargo: "Gerente",
    modelo: "CLT",
    saldoHoras: 12.0,
    salario: 8000,
    status: "Ativo",
    email: "pedro.oliveira@example.com",
    telefone: "11965432109",
    nascimento: "1985-03-10",
    tempoCasa: "5 anos",
  },
];

export const mockColaboradorDetalhes = (_codColaborador: string): ColaboradorDetalhes | null => {
  const colaborador = mockColaboradores.find((c) => c.codColaborador === _codColaborador);
  if (!colaborador) return null;

  return {
    id: colaborador.id,
    codColaborador: colaborador.codColaborador,
    nome: colaborador.nome,
    cargo: colaborador.cargo,
    email: colaborador.email,
    telefone: colaborador.telefone,
    nascimento: colaborador.nascimento,
    cpf: "123.456.789-00",
    rg: "12.345.678-9",
    cidadania: "Brasileira",
    endereco: {
      rua: "Rua das Flores",
      numero: "123",
      complemento: "Apto 45",
      bairro: "Centro",
      cidade: "São Paulo",
      estado: "SP",
      cep: "01234-567",
    },
    trabalho: {
      modelo: colaborador.modelo,
      salario: colaborador.salario,
      saldoHoras: colaborador.saldoHoras,
      tempoCasa: colaborador.tempoCasa,
      dataAdmissao: "2022-01-15",
      status: colaborador.status,
    },
    financeiro: {
      banco: "Banco do Brasil",
      agencia: "1234-5",
      conta: "12345-6",
      tipoConta: "Corrente",
    },
  };
};

export const mockFeriasInfo = (_codColaborador: string): FeriasInfo => {
  return {
    saldo: 20,
    periodo: "2024/2025",
    proximasFerias: "2024-12-15",
  };
};

