import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaApi } from '@data/api/VagaApi';
import type {
  StatusRecrutamentoRepository,
  StatusCandidaturaRecrutamento,
  StatusVagaRecrutamento,
} from '@domain/repositories/StatusRecrutamentoRepository';

@injectable()
export class StatusRecrutamentoRepositoryImpl implements StatusRecrutamentoRepository {
  constructor(
    @inject(DiTokens.vagaApi)
    private readonly api: VagaApi,
  ) {}

  async listarStatusCandidatura(token: string): Promise<StatusCandidaturaRecrutamento[]> {
    const res = await this.api.listarStatusCandidaturaRecrutamento(token);
    const list = res?.retorno ?? [];
    return list.map((item) => ({ id: item.id, descricao: item.descricao ?? '' }));
  }

  async listarStatusVaga(token: string): Promise<StatusVagaRecrutamento[]> {
    const res = await this.api.listarStatusVagaRecrutamento(token);
    const list = res?.retorno ?? [];
    return list.map((item) => ({ codigo: item.codigo, descricao: item.descricao ?? '' }));
  }
}
