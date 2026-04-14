export interface Equipe {
  id: number;
  nome: string;
}

export interface ColaboradorBook {
  id: string;
  nome: string;
  cargo: string;
  modelo: string;
  saldoHoras: number;
  salario: number;
  status: string;
  email: string;
  telefone: string;
  nascimento: string;
  tempoCasa: string;
  codColaborador: string;
}

export interface ColaboradorDetalhes {
  id: string;
  codColaborador: string;
  nome: string;
  cargo: string;
  email: string;
  telefone: string;
  nascimento: string;
  cpf: string;
  rg: string;
  cidadania: string;
  endereco: {
    rua: string;
    numero: string;
    complemento?: string;
    bairro: string;
    cidade: string;
    estado: string;
    cep: string;
  };
  trabalho: {
    modelo: string;
    salario: number;
    saldoHoras: number;
    tempoCasa: string;
    dataAdmissao: string;
    status: string;
  };
  financeiro: {
    banco: string;
    agencia: string;
    conta: string;
    tipoConta: string;
  };
}

export interface FeriasInfo {
  saldo: number;
  periodo: string;
  proximasFerias?: string;
}

