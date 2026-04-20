import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type {
  CriarGrupoPayload,
  CriarGrupoResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository';

@injectable()
export class AtualizarComunicacaoGrupoUseCase {
  constructor(
    @inject(DiTokens.comunicacaoGrupoRepository)
    private readonly repository: ComunicacaoGrupoRepository,
  ) {}

  async execute(
    token: string,
    grupoId: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse> {
    return this.repository.atualizarGrupo(token, grupoId, payload);
  }
}
