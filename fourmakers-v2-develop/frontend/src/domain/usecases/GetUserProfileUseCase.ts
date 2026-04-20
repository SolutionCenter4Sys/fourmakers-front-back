import { inject, injectable } from 'tsyringe';
import type { RecruitmentRepository } from '@domain/repositories/RecruitmentRepository';
import type { UserProfile } from '@domain/entities/SimulatorTypes';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class GetUserProfileUseCase {
  constructor(
    @inject(DiTokens.recruitmentRepository)
    private readonly repository: RecruitmentRepository
  ) {}

  async execute(token: string): Promise<UserProfile> {
    return await this.repository.fetchUserProfile(token);
  }
}

