export interface VagaListItem {
  id: string;
  codigo?: number | null;
  titulo?: string | null;
  modeloTrabalhoDescricao?: string | null;
  nomeGestor?: string | null;
  nomeCliente?: string | null;
  codigoCliente?: string | null;
  /** Preenchido pela API; quando array não vazio, a vaga possui candidatos. */
  quantidadeCandidatosPorEstagio?: Array<{ idStatus?: number; descricaoStatus?: string; quantidade?: number }>;
}
