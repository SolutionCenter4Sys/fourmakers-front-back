import type { VagaListItem } from '@domain/entities/VagaListItem';

export interface VagaListRepository {
  listVagas(token: string): Promise<VagaListItem[]>;
}
