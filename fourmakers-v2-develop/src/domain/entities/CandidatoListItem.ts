export interface CandidatoListItem {
  idCandidatura: string;
  nome: string;
  codigo: string;
  emailUsuario?: string | null;
  /** E-mail alternativo; usado na exibição quando emailUsuario não é válido. */
  emailAlternativo?: string | null;
  descricaoStatusCandidatura?: string | null;
}
