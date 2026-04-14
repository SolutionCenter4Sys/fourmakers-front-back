import { inject, injectable } from 'tsyringe'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class SalvarEBaixarTemplateUseCase {
  constructor(
    @inject(DiTokens.templateContratacaoRepository)
    private readonly repository: TemplateContratacaoRepository
  ) {}

  async execute(
    token: string,
    idCandidatura: string,
    payload: Record<string, unknown>
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null }> {
    return this.repository.salvarTemplate(token, idCandidatura, payload)
  }
}
