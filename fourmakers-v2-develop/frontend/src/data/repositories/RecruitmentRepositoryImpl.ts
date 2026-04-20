import { inject, injectable } from 'tsyringe';
import type { RecruitmentRepository } from '@domain/repositories/RecruitmentRepository';
import type { Manager, UserProfile } from '@domain/entities/SimulatorTypes';
import { RecruitmentApi } from '@data/api/RecruitmentApi';
import { DiTokens } from '@core/di/tokens';

@injectable()
export class RecruitmentRepositoryImpl implements RecruitmentRepository {
  constructor(
    @inject(DiTokens.recruitmentApi)
    private readonly api: RecruitmentApi
  ) {}

  async fetchManagers(token: string, search: string): Promise<Manager[]> {
    return await this.api.getManagers(token, search);
  }

  async fetchUserProfile(token: string): Promise<UserProfile> {
    return await this.api.getUserProfile(token);
  }
}

