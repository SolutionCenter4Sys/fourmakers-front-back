import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository';
import type { ConsultaProjetoHorasParams, ConsultaProjetoHorasResponse } from '@domain/entities/MapaAlocacao';

@injectable()
export class ConsultaProjetoHorasUseCase {
  constructor(
    @inject(DiTokens.mapaAlocacaoRepository)
    private readonly repository: MapaAlocacaoRepository
  ) {}

  async execute(
    token: string,
    params: ConsultaProjetoHorasParams
  ): Promise<ConsultaProjetoHorasResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    return this.repository.consultaProjetoHoras(token, params);
  }
}
