import type { MenuResource } from '@domain/entities/MenuResource'

export interface MenuRepository {
  fetchMenuResources(token: string): Promise<MenuResource[]>
}

