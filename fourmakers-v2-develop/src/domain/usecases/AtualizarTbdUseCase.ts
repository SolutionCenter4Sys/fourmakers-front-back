import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { TbdRepository } from '@domain/repositories/TbdRepository';
import type {
  AtualizarTbdPayload,
  AtualizarTbdResponse
} from '@domain/entities/Tbd';

@injectable()
export class AtualizarTbdUseCase {
  constructor(
    @inject(DiTokens.tbdRepository)
    private readonly repository: TbdRepository
  ) {}

  async execute(
    token: string,
    payload: AtualizarTbdPayload
  ): Promise<AtualizarTbdResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!payload.codTbdAlocado) {
      throw new Error('codTbdAlocado é obrigatório');
    }

    if (!payload.descricao || !payload.descricao.trim()) {
      throw new Error('Descrição é obrigatória');
    }

    return this.repository.atualizarTbd(token, payload);
  }
}
