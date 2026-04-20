import { inject, injectable } from 'tsyringe'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { ExportarRelatorioAlocacoesPayload } from '@domain/entities/MapaAlocacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ExportarRelatorioAlocacoesUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository,
  ) {}

  async execute(
    token: string,
    payload: ExportarRelatorioAlocacoesPayload
  ): Promise<Response> {
    return this.repository.exportarRelatorioAlocacoes(token, payload)
  }
}

