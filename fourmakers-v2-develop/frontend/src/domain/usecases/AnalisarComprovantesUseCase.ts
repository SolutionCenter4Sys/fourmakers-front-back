import { inject, injectable } from 'tsyringe'

import type { ReembolsoSolicitacaoRepository } from '@domain/repositories/ReembolsoSolicitacaoRepository'
import type { ArquivoAnalise, ResultadoAnaliseComprovante } from '@data/api/ReembolsoSolicitacaoApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class AnalisarComprovantesUseCase {
  constructor(
    @inject(DiTokens.reembolsoSolicitacaoRepository)
    private readonly repository: ReembolsoSolicitacaoRepository,
  ) {}

  async execute(
    token: string,
    arquivos: ArquivoAnalise[]
  ): Promise<{ sucesso: boolean; retorno?: ResultadoAnaliseComprovante; mensagem?: string }> {
    return this.repository.analisarComprovantesFiscais(token, arquivos)
  }
}

