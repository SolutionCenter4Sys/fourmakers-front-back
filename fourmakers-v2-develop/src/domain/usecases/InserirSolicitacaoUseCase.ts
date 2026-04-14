import { inject, injectable } from 'tsyringe'

import type { ReembolsoSolicitacaoRepository } from '@domain/repositories/ReembolsoSolicitacaoRepository'
import type { PayloadInserirSolicitacao } from '@data/api/ReembolsoSolicitacaoApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class InserirSolicitacaoUseCase {
  constructor(
    @inject(DiTokens.reembolsoSolicitacaoRepository)
    private readonly repository: ReembolsoSolicitacaoRepository,
  ) {}

  async execute(
    token: string,
    payload: PayloadInserirSolicitacao
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.repository.inserirSolicitacoes(token, payload)
  }

  async executeZip(
    token: string,
    arquivoZip: File
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.repository.inserirSolicitacoesZip(token, arquivoZip)
  }
}

