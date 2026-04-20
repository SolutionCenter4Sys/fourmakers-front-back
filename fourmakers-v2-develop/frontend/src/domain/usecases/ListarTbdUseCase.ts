import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { TbdRepository } from '@domain/repositories/TbdRepository';
import type { Tbd } from '@domain/entities/Tbd';

@injectable()
export class ListarTbdUseCase {
  constructor(
    @inject(DiTokens.tbdRepository)
    private readonly repository: TbdRepository
  ) {}

  async execute(token: string): Promise<Tbd[]> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    return this.repository.listarTbd(token);
  }
}
