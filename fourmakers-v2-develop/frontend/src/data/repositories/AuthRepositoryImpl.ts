import { inject, injectable } from 'tsyringe'

import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'
import type {
  AuthRepository,
  SendTokenEmailResult,
  ValidateTokenEmailResult,
  InsereUsuarioFourmakerPayload,
  InsereUsuarioFourmakerResponse,
} from '@domain/repositories/AuthRepository'

import { AuthApi } from '@data/api/AuthApi'
import { FourmakersApi } from '@data/api/FourmakersApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class AuthRepositoryImpl implements AuthRepository {
  constructor(
    @inject(DiTokens.authApi)
    private readonly api: AuthApi,
    @inject(DiTokens.fourmakersApi)
    private readonly fourmakersApi: FourmakersApi,
  ) {}

  async fetchShowmeProfile(token: string): Promise<ShowmeUserProfile> {
    return this.api.getShowmeProfile(token)
  }

  async sendTokenEmail(email: string, orgId: number | null): Promise<SendTokenEmailResult> {
    const response = await this.api.sendTokenEmail(email, orgId)
    return {
      tipoAcesso: response.tipoAcesso,
      orgId: response.orgId,
    }
  }

  async validateTokenEmail(
    email: string,
    token: string,
    orgId: number | null,
  ): Promise<ValidateTokenEmailResult> {
    const response = await this.api.validateTokenEmail(email, token, orgId)
    return {
      token: response.token,
      usuario: response.usuario,
      orgId: response.orgId,
      tipoAcesso: response.tipoAcesso,
    }
  }

  async getAccessToken(code: string, orgId: number): Promise<ValidateTokenEmailResult> {
    const response = await this.api.getAccessToken(code, orgId)
    return {
      token: response.token,
      usuario: response.usuario,
    }
  }

  async logout(token: string, cpf: string): Promise<void> {
    return this.api.logout(token, cpf)
  }

  async insereUsuarioFourmaker(payload: InsereUsuarioFourmakerPayload): Promise<InsereUsuarioFourmakerResponse> {
    return this.fourmakersApi.insereUsuarioFourmaker(payload)
  }
}

