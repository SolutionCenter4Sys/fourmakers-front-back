export type PerfilSkillType = 'COMPETENCIA' | 'SOFTSKILL' | 'METODOLOGIA' | 'DOMINIONEGOCIO' | 'IDIOMA';

export interface PerfilSkill {
  id: string;
  name: string;
  type: PerfilSkillType;
  levelId?: number;
  levelName?: string;
  /** true = imprescindível (destaque), false = desejável. Default true. */
  relevante?: boolean;
}

export interface PerfilAtuacaoFormData {
  name: string;
  clientCode: string;
  clientName: string;
  managerCode: string;
  managerInternalCode: string;
  managerName: string;
  cost: string;
  ratecard: string;
  workModelId: string;
  workModelCode: number;
  hybridDays: number;
  permanenceId: string;
  locationId: string;
  cep: string;
  uf: string;
  city: string;
  employmentTypeId: string;
  experienceLevelId: string;
  responsibilities: string;
  linkedinInfo: string;
  notes: string;
  aiPrompt: string;
  hardSkills: PerfilSkill[];
  softSkills: PerfilSkill[];
  methodologies: PerfilSkill[];
  businessDomains: PerfilSkill[];
  languages: PerfilSkill[];
}

export interface PerfilAtuacaoOption {
  id: string;
  name: string;
  code?: number;
  type?: 'presencial' | 'hibrido' | 'remoto';
}
