import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'

const SENIORITY_MAP: Record<string, number> = {
  trainee: 1,
  estagiario: 1,
  estagiário: 1,
  junior: 2,
  júnior: 2,
  iniciante: 2,
  basico: 2,
  básico: 2,
  pleno: 3,
  intermediario: 3,
  intermediário: 3,
  avancado: 3,
  avançado: 3,
  senior: 4,
  sênior: 4,
  especialista: 5,
  fluente: 4,
  nativo: 5,
}

export const getSeniorityTier = (label?: string | null): number | null => {
  if (!label) return null
  const normalized = label
    .toLowerCase()
    .trim()
    .replace(/\s+/g, ' ')
  const key = normalized
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')

  return SENIORITY_MAP[key] ?? null
}

const normalizeName = (name?: string) =>
  (name || '').trim().toLowerCase()

export interface SkillGapParams {
  requisitos: MinhaJornadaSkill[]
  possuidas: MinhaJornadaSkill[]
}

/**
 * Compara as listas de requisitos (perfil de atuação) com as skills possuídas
 * (Perfil 360) e retorna apenas os gaps.
 */
export const calcularGaps = ({
  requisitos,
  possuidas,
}: SkillGapParams): MinhaJornadaSkill[] => {
  const possuidasByName = new Map<string, MinhaJornadaSkill>()

  possuidas.forEach((s) => {
    const key = normalizeName(s.habilidade)
    if (!key) return
    const existing = possuidasByName.get(key)
    if (!existing) {
      possuidasByName.set(key, s)
      return
    }

    const tierExisting = getSeniorityTier(existing.senioridade) ?? 0
    const tierNew = getSeniorityTier(s.senioridade) ?? 0
    if (tierNew > tierExisting) {
      possuidasByName.set(key, s)
    }
  })

  const gaps: MinhaJornadaSkill[] = []

  requisitos.forEach((req) => {
    const key = normalizeName(req.habilidade)
    if (!key) return

    const possuida = possuidasByName.get(key)
    if (!possuida) {
      gaps.push(req)
      return
    }

    const tierReq = getSeniorityTier(req.senioridade) ?? 0
    const tierPossuida = getSeniorityTier(possuida.senioridade) ?? 0

    if (tierReq > tierPossuida) {
      gaps.push(req)
    }
  })

  return gaps
}



