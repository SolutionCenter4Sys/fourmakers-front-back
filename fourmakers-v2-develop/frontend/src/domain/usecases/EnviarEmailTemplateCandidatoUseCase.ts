import { inject, injectable } from 'tsyringe'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import { DiTokens } from '@core/di/tokens'

export interface EnviarEmailTemplateCandidatoParams {
  idCandidatura: string
  emailsAdicionais: string[]
  ocultarValores: boolean
}

@injectable()
export class EnviarEmailTemplateCandidatoUseCase {
  constructor(
    @inject(DiTokens.templateContratacaoRepository)
    private readonly repository: TemplateContratacaoRepository
  ) {}

  async execute(
    token: string,
    params: EnviarEmailTemplateCandidatoParams,
    file?: File
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }> {
    return this.repository.enviarEmailTemplateCandidato(token, params, file)
  }
}
