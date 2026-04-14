import type { MinhaJornadaAdherence, MinhaJornadaAdherenceCategory, MinhaJornadaComparativoSkillItem } from '@domain/entities/MinhaJornadaAdherence';
import type { RetornoMatchRaw } from '@domain/entities/GestaoVagasCandidatos';

const CATEGORY_KEYS = ['hard_skills', 'soft_skills', 'metodologias', 'dominios_negocio', 'idiomas', 'disponibilidades'] as const;
const CATEGORY_CAMEL: Record<string, keyof MinhaJornadaAdherence['comparativoPorSkill']> = {
  hard_skills: 'hardSkills',
  soft_skills: 'softSkills',
  metodologias: 'metodologias',
  dominios_negocio: 'dominiosNegocio',
  idiomas: 'idiomas',
  disponibilidades: 'disponibilidades',
};

const defaultCategory: MinhaJornadaAdherenceCategory = {
  scoreBrutoCategoria: 0,
  scoreBrutoObrigatorio: 0,
  scoreBrutoDesejavel: 0,
};

function mapDetalhamento(raw?: RetornoMatchRaw['detalhamento_calculo']): MinhaJornadaAdherence['detalhamentoCalculo'] {
  const out: MinhaJornadaAdherence['detalhamentoCalculo'] = {
    hardSkills: { ...defaultCategory },
    softSkills: { ...defaultCategory },
    metodologias: { ...defaultCategory },
    dominiosNegocio: { ...defaultCategory },
    idiomas: { ...defaultCategory },
    disponibilidades: { ...defaultCategory },
  };
  if (!raw) return out;
  for (const key of CATEGORY_KEYS) {
    const cat = raw[key];
    const camelKey = CATEGORY_CAMEL[key];
    if (cat && camelKey) {
      out[camelKey] = {
        scoreBrutoCategoria: cat.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio: cat.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel: cat.score_bruto_desejavel ?? 0,
      };
    }
  }
  return out;
}

function mapComparativoItem(item: Record<string, unknown>): MinhaJornadaComparativoSkillItem {
  return {
    skillRequisitada: (item.skill_requisitada as string) ?? '',
    nivelRequerido: (item.nivel_requerido as string) ?? '',
    obrigatoriedade: (item.obrigatoriedade as string) ?? '',
    skillDoCandidato: (item.skill_do_candidato as string) ?? '',
    nivelDoCandidato: (item.nivel_do_candidato as string) ?? '',
    pontuacaoDaSkill: typeof item.pontuacao_da_skill === 'number' ? item.pontuacao_da_skill : 0,
  };
}

function mapComparativo(raw?: RetornoMatchRaw['comparativo_por_skill']): MinhaJornadaAdherence['comparativoPorSkill'] {
  const out: MinhaJornadaAdherence['comparativoPorSkill'] = {
    hardSkills: [],
    softSkills: [],
    metodologias: [],
    dominiosNegocio: [],
    idiomas: [],
    disponibilidades: [],
  };
  if (!raw) return out;
  for (const key of CATEGORY_KEYS) {
    const arr = raw[key];
    const camelKey = CATEGORY_CAMEL[key];
    if (Array.isArray(arr) && camelKey) {
      out[camelKey] = arr.map((item) => mapComparativoItem(item as Record<string, unknown>));
    }
  }
  return out;
}

/** Converte retornoMatch da API (ListarCandidatosInscritos) para MinhaJornadaAdherence (usado no AdherenceDetailsModal). */
export function retornoMatchToMinhaJornadaAdherence(
  retornoMatch: RetornoMatchRaw | null | undefined,
  nomeFallback = 'Candidato'
): MinhaJornadaAdherence | null {
  if (!retornoMatch || typeof retornoMatch !== 'object') return null;
  return {
    codigoInternoColaborador: retornoMatch.codigoInternoColaborador ?? '',
    nome: retornoMatch.nome ?? nomeFallback,
    orgs: Array.isArray(retornoMatch.orgs) ? retornoMatch.orgs : [],
    match: typeof retornoMatch.match === 'number' ? retornoMatch.match : 0,
    scoreCandidato: typeof retornoMatch.score_candidato === 'number' ? retornoMatch.score_candidato : 0,
    scoreVaga: typeof retornoMatch.score_vaga === 'number' ? retornoMatch.score_vaga : 0,
    detalhamentoCalculo: mapDetalhamento(retornoMatch.detalhamento_calculo),
    comparativoPorSkill: mapComparativo(retornoMatch.comparativo_por_skill),
  };
}
