import type { Professional } from '@domain/entities/comunicacao';

export interface ComunicacaoProfissionaisRepository {
  listarProfissionais(token: string): Promise<Professional[]>;
}
