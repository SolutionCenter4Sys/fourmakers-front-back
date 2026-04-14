import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';
import type {
  Tbd,
  InserirTbdPayload,
  InserirTbdResponse,
  AtualizarTbdPayload,
  AtualizarTbdResponse
} from '@domain/entities/Tbd';

@injectable()
export class TbdApi {
  async listarTbd(token: string): Promise<Tbd[]> {
    return httpClient.get<Tbd[]>('/api/Tbd/ListarTbd', { token });
  }

  async inserirTbd(
    token: string,
    payload: InserirTbdPayload
  ): Promise<InserirTbdResponse> {
    return httpClient.post<InserirTbdResponse>(
      '/api/Tbd/InserirTbd',
      payload,
      { token }
    );
  }

  async atualizarTbd(
    token: string,
    payload: AtualizarTbdPayload
  ): Promise<AtualizarTbdResponse> {
    return httpClient.put<AtualizarTbdResponse>(
      '/api/Tbd/AtualizarTbd',
      payload,
      { token }
    );
  }
}
