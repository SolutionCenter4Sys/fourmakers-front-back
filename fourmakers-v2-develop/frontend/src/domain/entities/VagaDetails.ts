export interface VagaSkill {
  id: string;
  skillId: number;
  skillDescription: string;
  skillNivelDescription: string;
  tipoSkillId: number;
  relevante: boolean;
}

export interface VagaDetails {
  id: string;
  codigo?: number | null;
  titulo?: string | null;
  descricao?: string | null;
  cargo?: string | null;
  custoProfissional?: number | null;
  rateCard?: number | null;
  modeloTrabalhoId?: string | null;
  modeloTrabalhoDescricao?: string | null;
  nomeGestor?: string | null;
  nomeCliente?: string | null;
  codigoCliente?: string | null;
  localizacaoId?: string | null;
  cidade?: string | null;
  estado?: string | null;
  pais?: string | null;
  dataCriacao?: string | null;
  dataUltimaAlteracao?: string | null;
  statusVagaCod?: string | null;
  frequencia?: string | null;
  skills?: VagaSkill[];
}
