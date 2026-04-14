import { inject, injectable } from 'tsyringe'

import type { MenuResource } from '@domain/entities/MenuResource'
import type { MenuRepository } from '@domain/repositories/MenuRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetMenuResourcesUseCase {
  constructor(
    @inject(DiTokens.menuRepository)
    private readonly repository: MenuRepository,
  ) {}

  async execute(token: string): Promise<MenuResource[]> {
    return this.repository.fetchMenuResources(token)
  }
}

