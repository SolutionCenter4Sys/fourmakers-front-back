import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaListItem } from '@domain/entities/VagaListItem';
import type { VagaListRepository } from '@domain/repositories/VagaListRepository';

@injectable()
export class ListVagasUseCase {
  constructor(
    @inject(DiTokens.vagaListRepository)
    private readonly repository: VagaListRepository,
  ) {}

  async execute(token: string): Promise<VagaListItem[]> {
    return this.repository.listVagas(token);
  }
}
