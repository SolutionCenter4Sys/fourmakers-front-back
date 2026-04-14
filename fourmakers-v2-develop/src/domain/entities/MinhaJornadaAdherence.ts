export interface MinhaJornadaAdherenceCategory {
  scoreBrutoCategoria: number
  scoreBrutoObrigatorio: number
  scoreBrutoDesejavel: number
}

export interface MinhaJornadaAdherenceDetalhamento {
  hardSkills: MinhaJornadaAdherenceCategory
  softSkills: MinhaJornadaAdherenceCategory
  metodologias: MinhaJornadaAdherenceCategory
  dominiosNegocio: MinhaJornadaAdherenceCategory
  idiomas: MinhaJornadaAdherenceCategory
  disponibilidades: MinhaJornadaAdherenceCategory
}

export interface MinhaJornadaComparativoSkillItem {
  skillRequisitada: string
  nivelRequerido: string
  obrigatoriedade: string
  skillDoCandidato: string
  nivelDoCandidato: string
  pontuacaoDaSkill: number
}

export interface MinhaJornadaAdherenceComparativo {
  hardSkills: MinhaJornadaComparativoSkillItem[]
  softSkills: MinhaJornadaComparativoSkillItem[]
  metodologias: MinhaJornadaComparativoSkillItem[]
  dominiosNegocio: MinhaJornadaComparativoSkillItem[]
  idiomas: MinhaJornadaComparativoSkillItem[]
  disponibilidades: MinhaJornadaComparativoSkillItem[]
}

export interface MinhaJornadaAdherence {
  codigoInternoColaborador: string
  nome: string
  orgs: number[]

  match: number
  scoreCandidato: number
  scoreVaga: number

  detalhamentoCalculo: MinhaJornadaAdherenceDetalhamento
  comparativoPorSkill: MinhaJornadaAdherenceComparativo
}


