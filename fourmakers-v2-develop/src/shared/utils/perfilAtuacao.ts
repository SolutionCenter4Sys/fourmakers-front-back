import type { PerfilAtuacaoFormData, PerfilSkill, PerfilSkillType } from '@domain/entities/PerfilAtuacao';

export const initialPerfilAtuacaoState: PerfilAtuacaoFormData = {
  name: '',
  clientCode: '',
  clientName: '',
  managerCode: '',
  managerInternalCode: '',
  managerName: '',
  cost: '',
  ratecard: '',
  workModelId: '',
  workModelCode: 0,
  hybridDays: 3,
  permanenceId: '',
  locationId: '',
  cep: '',
  uf: '',
  city: '',
  employmentTypeId: '',
  experienceLevelId: '',
  responsibilities: '',
  linkedinInfo: '',
  notes: '',
  aiPrompt: '',
  hardSkills: [],
  softSkills: [],
  methodologies: [],
  businessDomains: [],
  languages: [],
};

export const defaultSkillOptions: Record<PerfilSkillType, PerfilSkill[]> = {
  COMPETENCIA: [
    { id: '1', name: 'React', type: 'COMPETENCIA' },
    { id: '2', name: 'TypeScript', type: 'COMPETENCIA' },
    { id: '3', name: 'Node.js', type: 'COMPETENCIA' },
    { id: '4', name: 'Java', type: 'COMPETENCIA' },
    { id: '5', name: 'AWS', type: 'COMPETENCIA' },
  ],
  SOFTSKILL: [
    { id: '11', name: 'Comunicação', type: 'SOFTSKILL' },
    { id: '12', name: 'Liderança', type: 'SOFTSKILL' },
    { id: '13', name: 'Trabalho em equipe', type: 'SOFTSKILL' },
  ],
  METODOLOGIA: [
    { id: '17', name: 'Scrum', type: 'METODOLOGIA' },
    { id: '18', name: 'Kanban', type: 'METODOLOGIA' },
    { id: '19', name: 'DevOps', type: 'METODOLOGIA' },
  ],
  DOMINIONEGOCIO: [
    { id: '21', name: 'Finanças', type: 'DOMINIONEGOCIO' },
    { id: '22', name: 'E-commerce', type: 'DOMINIONEGOCIO' },
    { id: '23', name: 'Saúde', type: 'DOMINIONEGOCIO' },
  ],
  IDIOMA: [
    { id: '25', name: 'Inglês', type: 'IDIOMA' },
    { id: '26', name: 'Espanhol', type: 'IDIOMA' },
  ],
};

export const skillTypeLabels: Record<PerfilSkillType, string> = {
  COMPETENCIA: 'Hard Skills',
  SOFTSKILL: 'Soft Skills',
  METODOLOGIA: 'Metodologias',
  DOMINIONEGOCIO: 'Domínios de Negócio',
  IDIOMA: 'Idiomas',
};

/** Skill com id 0 ou id string começando com "0-" = nova habilidade a ser criada pelo sistema (enviar pendente: true). */
const isNewSkill = (skill: PerfilSkill) => skill.id === '0' || String(skill.id).startsWith('0-');

export const mapSkillsToPayload = (skills: PerfilSkill[]) =>
  skills.map(skill => ({
    itemPerfil: {
      descricao: skill.type,
      id:
        skill.type === 'COMPETENCIA'
          ? 1
          : skill.type === 'METODOLOGIA'
            ? 3
            : skill.type === 'DOMINIONEGOCIO'
              ? 4
              : skill.type === 'SOFTSKILL'
                ? 8
                : 9,
    },
    skill: {
      descricao: skill.name,
      id: isNewSkill(skill) ? 0 : Number(skill.id) || 0,
      ...(isNewSkill(skill) ? { pendente: true } : {}),
    },
    nivel: { descricao: skill.levelName || '', id: skill.levelId || 0 },
    relevante: skill.relevante ?? true,
  }));

export const normalizeCep = (cep: string) => cep.replace(/\D/g, '').slice(0, 8);

export const formatCep = (value: string) => {
  const digits = normalizeCep(value);
  if (digits.length <= 5) return digits;
  return `${digits.slice(0, 5)}-${digits.slice(5)}`;
};

export const requiredPerfilFields: Array<keyof PerfilAtuacaoFormData> = [
  'name',
  'managerCode',
  'managerName',
  'workModelId',
];

export const validatePerfilForm = (form: PerfilAtuacaoFormData) => {
  const missing = requiredPerfilFields.filter(field => {
    const value = form[field];
    if (typeof value === 'string') return !value.trim();
    return !value;
  });
  return {
    isValid: missing.length === 0,
    missing,
  };
};

/** Campos obrigatórios quando o fluxo é "Criar Vaga" (perfil completo para o back criar a vaga). Ratecard é opcional. */
export const requiredPerfilFieldsNovaVaga: Array<keyof PerfilAtuacaoFormData> = [
  'name',
  'clientCode',
  'clientName',
  'managerCode',
  'managerName',
  'cost',
  'workModelId',
  'permanenceId',
  'locationId',
  'cep',
  'uf',
  'city',
  'employmentTypeId',
  'experienceLevelId',
  'responsibilities',
];

export const MIN_HARD_SKILLS_NOVA_VAGA = 2;
export const MIN_SOFT_SKILLS_NOVA_VAGA = 1;

export type ValidatePerfilFormNovaVagaResult = {
  isValid: boolean;
  missing: string[];
  errors: Record<string, boolean>;
  message?: string;
};

export const validatePerfilFormNovaVaga = (
  form: PerfilAtuacaoFormData,
  isRemote: boolean,
  isHybrid: boolean
): ValidatePerfilFormNovaVagaResult => {
  const errors: Record<string, boolean> = {};
  const missing: string[] = [];

  const fieldsToCheck = [...requiredPerfilFieldsNovaVaga];
  if (isHybrid) fieldsToCheck.push('hybridDays' as keyof PerfilAtuacaoFormData);
  if (isRemote) {
    (['cep', 'uf', 'city'] as Array<keyof PerfilAtuacaoFormData>).forEach(field => {
      const idx = fieldsToCheck.indexOf(field);
      if (idx >= 0) fieldsToCheck.splice(idx, 1);
    });
  }

  for (const field of fieldsToCheck) {
    const value = form[field];
    const isEmpty =
      value === undefined ||
      value === null ||
      (typeof value === 'string' && !String(value).trim()) ||
      (field === 'hybridDays' && (typeof value !== 'number' || value < 0));
    if (isEmpty) {
      errors[field] = true;
      missing.push(field);
    }
  }

  if (form.hardSkills.length < MIN_HARD_SKILLS_NOVA_VAGA) {
    errors.hardSkills = true;
    missing.push('hardSkills');
  }
  if (form.softSkills.length < MIN_SOFT_SKILLS_NOVA_VAGA) {
    errors.softSkills = true;
    missing.push('softSkills');
  }

  const messages: string[] = [];
  if (missing.length > 0) messages.push(`Campos/habilidades: ${missing.join(', ')}`);
  const message = messages.length ? messages.join('. ') : undefined;

  return {
    isValid: missing.length === 0,
    missing,
    errors,
    message,
  };
};
