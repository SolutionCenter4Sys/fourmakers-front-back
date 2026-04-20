import { inject, injectable } from 'tsyringe'

import type { ReembolsoSolicitacaoRepository } from '@domain/repositories/ReembolsoSolicitacaoRepository'
import type { ProjetoColaborador } from '@data/api/ReembolsoSolicitacaoApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetProjetosColaboradorUseCase {
  constructor(
    @inject(DiTokens.reembolsoSolicitacaoRepository)
    private readonly repository: ReembolsoSolicitacaoRepository,
  ) {}

  async execute(
    token: string,
    codigoProfissional: string
  ): Promise<{ sucesso: boolean; retorno?: ProjetoColaborador[]; mensagem?: string }> {
    return this.repository.listarProjetosColaborador(token, codigoProfissional)
  }
}

