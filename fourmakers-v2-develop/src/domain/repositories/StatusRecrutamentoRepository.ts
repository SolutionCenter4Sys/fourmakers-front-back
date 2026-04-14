/** Item de status de candidatura (recrutamento) no domínio. */
export interface StatusCandidaturaRecrutamento {
  id: number;
  descricao: string;
}

/** Item de status de vaga (recrutamento) no domínio. */
export interface StatusVagaRecrutamento {
  codigo: string;
  descricao: string;
}

export interface StatusRecrutamentoRepository {
  listarStatusCandidatura(token: string): Promise<StatusCandidaturaRecrutamento[]>;
  listarStatusVaga(token: string): Promise<StatusVagaRecrutamento[]>;
}
