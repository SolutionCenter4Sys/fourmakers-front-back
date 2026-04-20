import type { CandidatoListItem } from '@domain/entities/CandidatoListItem';

export interface CandidatoListRepository {
  listCandidatos(token: string, vagaId: string): Promise<CandidatoListItem[]>;
}
