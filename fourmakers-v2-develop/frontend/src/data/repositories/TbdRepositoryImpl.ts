import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { TbdRepository } from '@domain/repositories/TbdRepository';
import type {
  Tbd,
  InserirTbdPayload,
  InserirTbdResponse,
  AtualizarTbdPayload,
  AtualizarTbdResponse
} from '@domain/entities/Tbd';
import type { TbdApi } from '@data/api/TbdApi';

@injectable()
export class TbdRepositoryImpl implements TbdRepository {
  constructor(
    @inject(DiTokens.tbdApi)
    private readonly tbdApi: TbdApi
  ) {}

  async listarTbd(token: string): Promise<Tbd[]> {
    return this.tbdApi.listarTbd(token);
  }

  async inserirTbd(
    token: string,
    payload: InserirTbdPayload
  ): Promise<InserirTbdResponse> {
    return this.tbdApi.inserirTbd(token, payload);
  }

  async atualizarTbd(
    token: string,
    payload: AtualizarTbdPayload
  ): Promise<AtualizarTbdResponse> {
    return this.tbdApi.atualizarTbd(token, payload);
  }
}
