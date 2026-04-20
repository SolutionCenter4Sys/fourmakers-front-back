import type {
  Tbd,
  InserirTbdPayload,
  InserirTbdResponse,
  AtualizarTbdPayload,
  AtualizarTbdResponse
} from '@domain/entities/Tbd';

export interface TbdRepository {
  listarTbd(token: string): Promise<Tbd[]>;

  inserirTbd(
    token: string,
    payload: InserirTbdPayload
  ): Promise<InserirTbdResponse>;

  atualizarTbd(
    token: string,
    payload: AtualizarTbdPayload
  ): Promise<AtualizarTbdResponse>;
}
