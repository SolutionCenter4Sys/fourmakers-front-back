/**
 * Mapeia ColaboradorMatchPromptItem (API) para MatchCardItem (exibição no protótipo Match Talentos).
 */

import type { ColaboradorMatchPromptItem } from '@domain/entities/GestaoVagasCandidatos';
import type { MatchCardItem, MatchCardSkill } from '@shared/types/matchTalentos';

const CATEGORY_KEYS = ['hard_skills', 'soft_skills', 'metodologias', 'dominios_negocio', 'idiomas', 'disponibilidades'] as const;

function extractSkills(item: ColaboradorMatchPromptItem): MatchCardSkill[] {
  const comp = item.retornoMatch?.comparativo_por_skill;
  if (!comp || typeof comp !== 'object') return [];
  const skills: MatchCardSkill[] = [];
  for (const key of CATEGORY_KEYS) {
    const arr = comp[key];
    if (Array.isArray(arr)) {
      for (const s of arr) {
        const skill = (s as { skill_requisitada?: string; skill_do_candidato?: string; nivel_do_candidato?: string }).skill_do_candidato
          ?? (s as { skill_requisitada?: string }).skill_requisitada ?? '';
        const nivel = (s as { nivel_do_candidato?: string }).nivel_do_candidato
          ?? (s as { nivel_requerido?: string }).nivel_requerido ?? '';
        if (skill) skills.push({ skill, nivel });
      }
    }
  }
  return skills;
}

function formatCandidaturasLabel(item: ColaboradorMatchPromptItem): string {
  if (!item.possui_candidatura || !item.candidaturas?.length) return 'Sem candidaturas';
  const n = item.candidaturas.length;
  return n === 1 ? '1 candidatura' : `${n} candidaturas`;
}

export function mapColaboradorMatchToCardItem(item: ColaboradorMatchPromptItem): MatchCardItem {
  const id = item.codigo_interno_colaborador ?? '';
  return {
    id,
    codigoInternoColaborador: id,
    nome: item.nome ?? '—',
    match: typeof item.match === 'number' ? item.match : 0,
    skills: extractSkills(item),
    candidaturasLabel: formatCandidaturasLabel(item),
    origem: item.origem ?? '—',
    retornoMatch: item.retornoMatch ?? undefined,
  };
}
