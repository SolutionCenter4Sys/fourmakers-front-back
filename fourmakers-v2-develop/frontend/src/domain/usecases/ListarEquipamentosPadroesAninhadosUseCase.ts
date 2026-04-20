import { inject, injectable } from 'tsyringe'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import type { EquipamentosPadroesAninhadosGrupo } from '@domain/entities/TemplateContratacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarEquipamentosPadroesAninhadosUseCase {
  constructor(
    @inject(DiTokens.templateContratacaoRepository)
    private readonly repository: TemplateContratacaoRepository
  ) {}

  async execute(token: string): Promise<EquipamentosPadroesAninhadosGrupo[]> {
    return this.repository.listarEquipamentosPadroesAninhados(token)
  }
}
