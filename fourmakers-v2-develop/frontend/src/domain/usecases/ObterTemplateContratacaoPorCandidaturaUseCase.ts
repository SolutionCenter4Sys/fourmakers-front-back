import { inject, injectable } from 'tsyringe'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import type { ObterTemplateContratacaoResult } from '@domain/entities/TemplateContratacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ObterTemplateContratacaoPorCandidaturaUseCase {
  constructor(
    @inject(DiTokens.templateContratacaoRepository)
    private readonly repository: TemplateContratacaoRepository
  ) {}

  async execute(
    token: string,
    idCandidatura: string
  ): Promise<ObterTemplateContratacaoResult> {
    return this.repository.obterPorCandidatura(token, idCandidatura)
  }
}
