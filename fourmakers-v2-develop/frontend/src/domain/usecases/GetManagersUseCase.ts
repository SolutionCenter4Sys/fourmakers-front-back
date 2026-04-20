import { inject, injectable } from 'tsyringe';
import type { RecruitmentRepository } from '@domain/repositories/RecruitmentRepository';
import type { Manager } from '@domain/entities/SimulatorTypes';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class GetManagersUseCase {
  constructor(
    @inject(DiTokens.recruitmentRepository)
    private readonly repository: RecruitmentRepository
  ) {}

  async execute(token: string, search: string): Promise<Manager[]> {
    return await this.repository.fetchManagers(token, search);
  }
}

