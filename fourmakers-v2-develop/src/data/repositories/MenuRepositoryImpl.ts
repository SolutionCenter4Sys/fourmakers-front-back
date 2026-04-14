import { inject, injectable } from 'tsyringe'

import type { MenuResource } from '@domain/entities/MenuResource'
import type { MenuRepository } from '@domain/repositories/MenuRepository'

import { MenuApi } from '@data/api/MenuApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class MenuRepositoryImpl implements MenuRepository {
  constructor(
    @inject(DiTokens.menuApi)
    private readonly api: MenuApi,
  ) {}

  async fetchMenuResources(token: string): Promise<MenuResource[]> {
    return this.api.getMenuResources(token)
  }
}

