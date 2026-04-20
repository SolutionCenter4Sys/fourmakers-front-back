/**
 * Hooks e utilitários da jornada Recrutamento (gestão de vagas, perfil, vaga pública, dashboard).
 * @example import { useGestaoVagas, formatDatePtBr } from '@presentation/hooks/recrutamento'
 */
export { useDashboardRecrutamento } from './useDashboardRecrutamento'
export {
  MAX_VAGAS_POR_COLUNA_OPTIONS,
  useGestaoVagas,
  type PerfilCardModel,
  type VagaCardModel,
  type VagaInfoItem,
  type VagaInfoModel,
} from './useGestaoVagas'
export {
  useGestaoVagasCandidatos,
  DEFAULT_FILTROS_ADERENTES,
  type VagaCandidatosState,
} from './useGestaoVagasCandidatos'
export {
  useTemplateContratacaoCandidato,
  type UseTemplateContratacaoCandidatoResult,
} from './useTemplateContratacaoCandidato'
export { usePerfilAtuacao } from './usePerfilAtuacao'
export { usePerfilAtuacaoPage } from './usePerfilAtuacaoPage'
export * from './usePublicVagaDetalhe'
export * from './gestaoVagas/gestaoVagasUtils'
export * from './gestaoVagas/gestaoVagasStatusFlows'
