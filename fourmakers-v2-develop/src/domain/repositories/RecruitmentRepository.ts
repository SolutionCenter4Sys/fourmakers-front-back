
import type { Manager, UserProfile } from '../entities/SimulatorTypes';

export interface RecruitmentRepository {
  fetchManagers(token: string, search: string): Promise<Manager[]>;
  fetchUserProfile(token: string): Promise<UserProfile>;
}
