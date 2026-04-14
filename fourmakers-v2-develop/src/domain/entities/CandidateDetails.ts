export interface CandidateAddress {
  cep?: string | null;
  endereco?: string | null;
  complemento?: string | null;
  numero?: number | null;
  bairro?: string | null;
  cidade?: string | null;
  estado?: string | null;
}

export interface CandidateDetails {
  cpf: string;
  nomeCompleto: string;
  email?: string | null;
  contatoPrincipal?: string | null;
  rg?: string | null;
  dataNascimento?: string | null;
  cargo?: string | null;
  diretoriaNome?: string | null;
  endereco?: CandidateAddress;
  candidatoId?: number | null;
}
