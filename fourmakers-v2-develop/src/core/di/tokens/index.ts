import { DiTokensCore } from './core.tokens'
import { DiTokensRecrutamento } from './recrutamento/recrutamento.tokens'

/** Tokens unificados (compatível com import @core/di/tokens). */
export const DiTokens = {
  ...DiTokensCore,
  ...DiTokensRecrutamento,
} as const

export type DiTokenKey = keyof typeof DiTokens
