/**
 * Dados mock para Match de Talentos (Beta).
 * Coluna "Motor Atual" usa API; coluna "Motor Beta" usa estes mocks.
 */

import type { MatchCandidate, MatchCardItem, MatchCardSkill } from '@shared/types/matchTalentos';

/** Candidatos fictícios para simular resultados do Motor Beta. */
export const MOCK_CANDIDATES_POOL: (MatchCandidate & { match?: number; skills?: MatchCardSkill[]; origem?: string; candidaturasLabel?: string })[] = [
  { id: 'm1', codigoInternoColaborador: 'C001', nome: 'Ana Silva', resumo: 'Desenvolvedora Full Stack · São Paulo', match: 92, skills: [{ skill: 'React', nivel: 'Senior' }, { skill: 'TypeScript', nivel: 'Pleno' }], origem: 'Linkedin', candidaturasLabel: '2 vagas' },
  { id: 'm2', codigoInternoColaborador: 'C002', nome: 'Bruno Santos', resumo: 'Tech Lead · Remoto', match: 88, skills: [{ skill: 'Java', nivel: 'Senior' }, { skill: 'Spring Boot', nivel: 'Senior' }], origem: 'Colaborador', candidaturasLabel: 'Sem candidaturas' },
  { id: 'm3', codigoInternoColaborador: 'C003', nome: 'Carla Oliveira', resumo: 'UX Designer · Campinas', match: 75, skills: [{ skill: 'Figma', nivel: 'Avançado' }], origem: 'Linkedin', candidaturasLabel: '1 vaga(s)' },
  { id: 'm4', codigoInternoColaborador: 'C004', nome: 'Diego Costa', resumo: 'Backend Developer · São Paulo', match: 85, skills: [{ skill: 'Java', nivel: 'Pleno' }, { skill: 'Kotlin', nivel: 'Pleno' }], origem: 'Colaborador', candidaturasLabel: '3 vagas' },
  { id: 'm5', codigoInternoColaborador: 'C005', nome: 'Elena Ferreira', resumo: 'Product Manager · Híbrido', match: 70, skills: [{ skill: 'Scrum', nivel: 'Avançado' }], origem: 'Linkedin', candidaturasLabel: 'Sem candidaturas' },
  { id: 'm6', codigoInternoColaborador: 'C006', nome: 'Fábio Lima', resumo: 'DevOps Engineer · Remoto', match: 90, skills: [{ skill: 'Kubernetes', nivel: 'Senior' }, { skill: 'AWS', nivel: 'Pleno' }], origem: 'Colaborador', candidaturasLabel: '1 vaga(s)' },
  { id: 'm7', codigoInternoColaborador: 'C007', nome: 'Giovana Martins', resumo: 'Data Analyst · São Paulo', match: 65, skills: [{ skill: 'Python', nivel: 'Pleno' }], origem: 'Linkedin', candidaturasLabel: 'Sem candidaturas' },
  { id: 'm8', codigoInternoColaborador: 'C008', nome: 'Henrique Alves', resumo: 'Frontend React · Campinas', match: 95, skills: [{ skill: 'React', nivel: 'Senior' }, { skill: 'Node.js', nivel: 'Pleno' }], origem: 'Colaborador', candidaturasLabel: '2 vagas' },
  { id: 'm9', codigoInternoColaborador: 'C009', nome: 'Isabela Rocha', resumo: 'QA Engineer · Híbrido', match: 78, skills: [{ skill: 'Cypress', nivel: 'Pleno' }], origem: 'Linkedin', candidaturasLabel: 'Sem candidaturas' },
  { id: 'm10', codigoInternoColaborador: 'C010', nome: 'João Pereira', resumo: 'Scrum Master · Remoto', match: 72, skills: [{ skill: 'Jira', nivel: 'Avançado' }], origem: 'Colaborador', candidaturasLabel: '1 vaga(s)' },
];

/** Índices para simular ordem do Motor Beta (5 primeiros). */
const ORDER_BETA = [2, 4, 0, 3, 1];

export function getMockResultsBeta(): MatchCardItem[] {
  return ORDER_BETA.map((i) => {
    const c = MOCK_CANDIDATES_POOL[i];
    return {
      id: c.id,
      codigoInternoColaborador: c.codigoInternoColaborador,
      nome: c.nome,
      match: c.match ?? 80,
      skills: c.skills ?? [],
      candidaturasLabel: c.candidaturasLabel ?? 'Sem candidaturas',
      origem: c.origem ?? 'Mock',
      resumo: c.resumo,
    };
  });
}

/** Motivos obrigatórios quando o candidato é marcado como "Não Relevante". */
export const MOTIVOS_NAO_RELEVANTE = [
  { value: 'fora_perfil', label: 'Fora do perfil da vaga' },
  { value: 'localizacao', label: 'Localização incompatível' },
  { value: 'experiencia_insuficiente', label: 'Experiência insuficiente' },
  { value: 'experiencia_demais', label: 'Super qualificado para a vaga' },
  { value: 'outro', label: 'Outro motivo' },
] as const;

export const STORAGE_KEY_FEEDBACKS = 'matchTalentos_feedbacks';
