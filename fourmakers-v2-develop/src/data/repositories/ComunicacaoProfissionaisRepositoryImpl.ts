import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoProfissionaisApi } from '@data/api/ComunicacaoProfissionaisApi';
import type { Professional } from '@domain/entities/comunicacao';
import type { ComunicacaoProfissionaisRepository } from '@domain/repositories/ComunicacaoProfissionaisRepository';
import { profissionalApiToProfessional } from '@shared/utils/comunicacaoProfissionalMapper';

@injectable()
export class ComunicacaoProfissionaisRepositoryImpl
  implements ComunicacaoProfissionaisRepository
{
  constructor(
    @inject(DiTokens.comunicacaoProfissionaisApi)
    private readonly api: ComunicacaoProfissionaisApi,
  ) {}

  async listarProfissionais(token: string): Promise<Professional[]> {
    const response = await this.api.getProfissionais(token);
    const retorno = response?.retorno;
    if (!Array.isArray(retorno)) return [];
    const mapped = retorno.map((item) => profissionalApiToProfessional(item));
    const seen = new Set<string>();
    return mapped.filter((p) => {
      if (seen.has(p.id)) return false;
      seen.add(p.id);
      return true;
    });
  }
}
