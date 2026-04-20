import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { TbdRepository } from '@domain/repositories/TbdRepository';
import type {
  InserirTbdPayload,
  InserirTbdResponse
} from '@domain/entities/Tbd';

@injectable()
export class InserirTbdUseCase {
  constructor(
    @inject(DiTokens.tbdRepository)
    private readonly repository: TbdRepository
  ) {}

  async execute(
    token: string,
    payload: InserirTbdPayload
  ): Promise<InserirTbdResponse> {
    if (!token) {
      throw new Error('Token de autenticação é obrigatório');
    }

    if (!payload.descricao || !payload.descricao.trim()) {
      throw new Error('Descrição é obrigatória');
    }

    return this.repository.inserirTbd(token, payload);
  }
}
