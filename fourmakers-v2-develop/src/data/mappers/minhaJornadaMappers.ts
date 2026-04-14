import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { MinhaJornadaProfile } from '@domain/entities/MinhaJornadaProfile'
import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'

import type {
  BuscarSkillColaboradorResponse,
  BuscarSkillColaboradorAlocadoResponse,
  CalcularAderenciaResponse,
} from '@data/api/MinhaJornadaApi'

export const mapPerfil360Skills = (
  response: BuscarSkillColaboradorResponse,
): MinhaJornadaSkill[] => {
  const entry = response.retorno?.[0]
  if (!entry) return []

  return entry.colaboradorHabilidades.map((h) => ({
    codigoInternoColaborador: h.codigoInternoColaborador,
    codigoCliente: undefined,
    perfilTipoId: h.perfilTipoId,
    tipoPerfil: h.tipoPerfil,
    skillId: h.skillId,
    habilidade: h.habilidade,
    senioridadeId: h.senioridadeId ?? null,
    senioridade: h.senioridade ?? null,
    interesse: h.interesse,
  }))
}

export const mapPerfisAtuacao = (
  response: BuscarSkillColaboradorAlocadoResponse,
  orgId: number,
): MinhaJornadaProfile[] => {
  if (!response?.retorno) return []

  return response.retorno.flatMap((entry) => {
    if (!entry.clientes) return []

    return entry.clientes.map((cliente) => {
      const rawHabilidades = cliente.habilidades ?? []

      const skills: MinhaJornadaSkill[] = rawHabilidades
        .filter((h) => h && h.habilidade)
        .map((h) => ({
          codigoInternoColaborador: entry.codigoInternoColaborador,
          codigoCliente: cliente.codigoCliente,
          perfilTipoId: h.perfilTipoId,
          tipoPerfil: h.tipoPerfil,
          skillId: h.skillId,
          habilidade: h.habilidade,
          senioridadeId: h.senioridadeId ?? null,
          senioridade: h.senioridade ?? null,
          interesse: h.interesse,
        }))

      return {
        perfilId: cliente.perfilId ?? null,
        perfilNome: cliente.perfil ?? '',
        codigoCliente: cliente.codigoCliente,
        nomeCliente: cliente.nomeCliente,
        idAlocacao: entry.idAlocacao,
        codigoProjeto: entry.codigoProjeto,
        orgId,
        codigoInternoColaborador: entry.codigoInternoColaborador,
        nomeColaborador: entry.nomeCompleto,
        codigoInternoGestorAdm: entry.codigoInternoGestorAdm,
        nomeGestorAdm: entry.nomeGestorAdm,
        codGestorCliente: cliente.codGestorCliente,
        nomeGestorCliente: cliente.nomeGestorCliente,
        skills,
      }
    })
  })
}

export const flattenPerfilAtuacao = (
  response: BuscarSkillColaboradorAlocadoResponse,
  orgId: number,
): MinhaJornadaProfile | null => {
  const profiles = mapPerfisAtuacao(response, orgId)
  return profiles.length > 0 ? profiles[0] : null
}

export const mapAdherence = (
  response: CalcularAderenciaResponse,
): MinhaJornadaAdherence | null => {
  const r = response.retorno
  if (!r) return null

  const detalhamento = r.detalhamento_calculo ?? {}
  const comparativo = r.comparativo_por_skill ?? {}

  return {
    codigoInternoColaborador: r.codigoInternoColaborador,
    nome: r.nome,
    orgs: r.orgs ?? [],
    match: r.match,
    scoreCandidato: r.score_candidato,
    scoreVaga: r.score_vaga,
    detalhamentoCalculo: {
      hardSkills: {
        scoreBrutoCategoria:
          detalhamento.hard_skills?.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio:
          detalhamento.hard_skills?.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel:
          detalhamento.hard_skills?.score_bruto_desejavel ?? 0,
      },
      softSkills: {
        scoreBrutoCategoria:
          detalhamento.soft_skills?.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio:
          detalhamento.soft_skills?.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel:
          detalhamento.soft_skills?.score_bruto_desejavel ?? 0,
      },
      metodologias: {
        scoreBrutoCategoria:
          detalhamento.metodologias?.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio:
          detalhamento.metodologias?.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel:
          detalhamento.metodologias?.score_bruto_desejavel ?? 0,
      },
      dominiosNegocio: {
        scoreBrutoCategoria:
          detalhamento.dominios_negocio?.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio:
          detalhamento.dominios_negocio?.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel:
          detalhamento.dominios_negocio?.score_bruto_desejavel ?? 0,
      },
      idiomas: {
        scoreBrutoCategoria:
          detalhamento.idiomas?.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio:
          detalhamento.idiomas?.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel:
          detalhamento.idiomas?.score_bruto_desejavel ?? 0,
      },
      disponibilidades: {
        scoreBrutoCategoria:
          detalhamento.disponibilidades?.score_bruto_categoria ?? 0,
        scoreBrutoObrigatorio:
          detalhamento.disponibilidades?.score_bruto_obrigatorio ?? 0,
        scoreBrutoDesejavel:
          detalhamento.disponibilidades?.score_bruto_desejavel ?? 0,
      },
    },
    comparativoPorSkill: {
      hardSkills: (comparativo.hard_skills ?? []).map((s: any) => ({
        skillRequisitada: s.skill_requisitada,
        nivelRequerido: s.nivel_requerido,
        obrigatoriedade: s.obrigatoriedade,
        skillDoCandidato: s.skill_do_candidato,
        nivelDoCandidato: s.nivel_do_candidato,
        pontuacaoDaSkill: s.pontuacao_da_skill,
      })),
      softSkills: (comparativo.soft_skills ?? []).map((s: any) => ({
        skillRequisitada: s.skill_requisitada,
        nivelRequerido: s.nivel_requerido,
        obrigatoriedade: s.obrigatoriedade,
        skillDoCandidato: s.skill_do_candidato,
        nivelDoCandidato: s.nivel_do_candidato,
        pontuacaoDaSkill: s.pontuacao_da_skill,
      })),
      metodologias: (comparativo.metodologias ?? []).map((s: any) => ({
        skillRequisitada: s.skill_requisitada,
        nivelRequerido: s.nivel_requerido,
        obrigatoriedade: s.obrigatoriedade,
        skillDoCandidato: s.skill_do_candidato,
        nivelDoCandidato: s.nivel_do_candidato,
        pontuacaoDaSkill: s.pontuacao_da_skill,
      })),
      dominiosNegocio: (comparativo.dominios_negocio ?? []).map(
        (s: any) => ({
          skillRequisitada: s.skill_requisitada,
          nivelRequerido: s.nivel_requerido,
          obrigatoriedade: s.obrigatoriedade,
          skillDoCandidato: s.skill_do_candidato,
          nivelDoCandidato: s.nivel_do_candidato,
          pontuacaoDaSkill: s.pontuacao_da_skill,
        }),
      ),
      idiomas: (comparativo.idiomas ?? []).map((s: any) => ({
        skillRequisitada: s.skill_requisitada,
        nivelRequerido: s.nivel_requerido,
        obrigatoriedade: s.obrigatoriedade,
        skillDoCandidato: s.skill_do_candidato,
        nivelDoCandidato: s.nivel_do_candidato,
        pontuacaoDaSkill: s.pontuacao_da_skill,
      })),
      disponibilidades: (comparativo.disponibilidades ?? []).map(
        (s: any) => ({
          skillRequisitada: s.skill_requisitada,
          nivelRequerido: s.nivel_requerido,
          obrigatoriedade: s.obrigatoriedade,
          skillDoCandidato: s.skill_do_candidato,
          nivelDoCandidato: s.nivel_do_candidato,
          pontuacaoDaSkill: s.pontuacao_da_skill,
        }),
      ),
    },
  }
}


