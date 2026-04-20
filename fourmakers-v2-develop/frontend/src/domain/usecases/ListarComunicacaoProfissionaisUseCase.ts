import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { Professional } from '@domain/entities/comunicacao';
import type { ComunicacaoProfissionaisRepository } from '@domain/repositories/ComunicacaoProfissionaisRepository';

@injectable()
export class ListarComunicacaoProfissionaisUseCase {
  constructor(
    @inject(DiTokens.comunicacaoProfissionaisRepository)
    private readonly repository: ComunicacaoProfissionaisRepository,
  ) {}

  async execute(token: string): Promise<Professional[]> {
    return this.repository.listarProfissionais(token);
  }
}
