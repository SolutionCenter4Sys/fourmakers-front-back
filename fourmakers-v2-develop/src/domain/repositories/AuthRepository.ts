import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'

export interface SendTokenEmailResult {
  tipoAcesso: number
  orgId?: number
}

export interface ValidateTokenEmailResult {
  token?: string
  usuario?: ShowmeUserProfile
  orgId?: number
  tipoAcesso?: number
  /** Token Microsoft Graph (opcional - se backend retornar) */
  microsoftAccessToken?: string
}

export interface InsereUsuarioFourmakerPayload {
  cpf: string
  email: string
  nome_completo: string
  senha: string
}

export interface InsereUsuarioFourmakerResponse {
  sucesso: boolean
  mensagem?: string
}

export interface AuthRepository {
  fetchShowmeProfile(token: string): Promise<ShowmeUserProfile>
  sendTokenEmail(email: string, orgId: number | null): Promise<SendTokenEmailResult>
  validateTokenEmail(email: string, token: string, orgId: number | null): Promise<ValidateTokenEmailResult>
  getAccessToken(code: string, orgId: number): Promise<ValidateTokenEmailResult>
  logout(token: string, cpf: string): Promise<void>
  insereUsuarioFourmaker(payload: InsereUsuarioFourmakerPayload): Promise<InsereUsuarioFourmakerResponse>
}

