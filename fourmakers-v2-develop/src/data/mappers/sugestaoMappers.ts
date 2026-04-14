import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import { TIPO_SKILL_MAP } from '@shared/constants/skillTypes'

/**
 * Possible status values for a skill suggestion
 */
export type SugestaoStatus = 'pending' | 'approved' | 'rejected'

/**
 * Mapped suggestion history item for UI consumption
 */
export interface SugestaoHistoryItem {
  id: string
  skillName: string
  skillType: string
  senioridade: string
  status: SugestaoStatus
  date: string
  ativo: boolean
}

/**
 * Maps API status ID to UI status string
 * tbStatusSugestaoId: 1 = Aprovado, 2 or null = Pendente, 3 = Rejeitado
 */
export const mapSugestaoStatus = (
  tbStatusSugestaoId: number | null | undefined
): SugestaoStatus => {
  if (tbStatusSugestaoId === 1) return 'approved'
  if (tbStatusSugestaoId === 3) return 'rejected'
  return 'pending'
}

/**
 * Raw API response item for suggestion
 */
interface ApiSugestaoItem {
  id: string
  codigoInternoColaborador: string
  codigoGestorAdm: string
  codigoCliente: string
  tipo_Id: number
  descricaoTipo: string
  skill_Id: number
  descricaoSkill: string
  perfil_Id: string
  senioridade_Id: number
  senioridade: string
  data: string
  ativo: boolean
  historicoSugestao?: Array<{
    id: string
    codigoInternoColaboradorAvaliador: string
    codigoInternoColaborador: string
    aprovado: boolean
    tbStatusSugestaoId: number
    perfil_Id: string
    observacao: string
    data: string
  }>
}

/**
 * Maps API suggestion response to UI format, filtered by skill type
 */
export const mapSugestaoHistoryFromApi = (
  apiItems: ApiSugestaoItem[],
  skillType: MinhaJornadaSkillType
): SugestaoHistoryItem[] => {
  const currentTipoId = TIPO_SKILL_MAP[skillType]

  return apiItems
    .filter((item) => item.tipo_Id === currentTipoId)
    .map((item) => {
      const latestHistorico = item.historicoSugestao?.[0]
      const statusId = latestHistorico?.tbStatusSugestaoId

      return {
        id: item.id,
        skillName: item.descricaoSkill,
        skillType: item.descricaoTipo,
        senioridade: item.senioridade,
        status: mapSugestaoStatus(statusId),
        date: item.data,
        ativo: item.ativo,
      }
    })
}

