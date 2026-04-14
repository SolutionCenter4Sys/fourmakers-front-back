import { inject, injectable } from 'tsyringe'
import type { LgpdRepository, LgpdRegistroPayload } from '@domain/repositories/LgpdRepository'
import { LgpdApi } from '@data/api/LgpdApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class LgpdRepositoryImpl implements LgpdRepository {
  constructor(
    @inject(DiTokens.lgpdApi)
    private readonly lgpdApi: LgpdApi,
  ) {}

  async registrar(payload: LgpdRegistroPayload): Promise<void> {
    await this.lgpdApi.registrarLgpd({
      cpf_pessoa: payload.cpfPessoa,
      nome_pessoa: payload.nomePessoa,
      is_colab: payload.isColab ?? false,
      unidade: payload.unidade ?? 'Externo',
      contrato: payload.contrato ?? 'Termos de uso - Fourmakers',
    })
  }
}
