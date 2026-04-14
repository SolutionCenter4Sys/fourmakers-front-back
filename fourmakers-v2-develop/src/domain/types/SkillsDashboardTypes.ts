export interface BigNumbersData {
  skillsAdicionadas: number
  skillsSugeridas: number
  adicionadasPDI: number
  skillsRejeitadas: number
}

export interface SkillTopDezEntry {
  skillId: number
  movimentacao: number
  nomeSkill: string
  total: number
}

export interface TopDezByMovimentacao {
  ADICIONADO_PERFIL: SkillTopDezEntry[]
  NAO_INTERESSADO: SkillTopDezEntry[]
  ADICIONADO_PDI: SkillTopDezEntry[]
  SUGERIDA: SkillTopDezEntry[]
  ATUALIZADO: SkillTopDezEntry[]
  INTERESSADO: SkillTopDezEntry[]
}

export interface TopDezData {
  topDezPorMovimentacao: TopDezByMovimentacao
}

