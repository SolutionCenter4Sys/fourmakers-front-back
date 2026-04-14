import { inject, injectable } from 'tsyringe'

import type { ReembolsoSolicitacaoRepository } from '@domain/repositories/ReembolsoSolicitacaoRepository'
import type { Verba, VerbaEParametros } from '@data/api/ReembolsoSolicitacaoApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetVerbasUseCase {
  constructor(
    @inject(DiTokens.reembolsoSolicitacaoRepository)
    private readonly repository: ReembolsoSolicitacaoRepository,
  ) {}

  async listarVerbasComExcecao(
    token: string,
    codigoProjeto: string,
    codigoCliente: string
  ): Promise<{ sucesso: boolean; retorno?: Verba[]; mensagem?: string }> {
    return this.repository.listarVerbasComExcecao(token, codigoProjeto, codigoCliente)
  }

  async obterVerbasEParametroValidacao(
    token: string
  ): Promise<{ sucesso: boolean; retorno?: VerbaEParametros; mensagem?: string }> {
    return this.repository.obterVerbasEParametroValidacao(token)
  }
}

