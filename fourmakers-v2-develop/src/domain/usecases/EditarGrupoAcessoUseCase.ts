import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { EditarGrupoAcessoPayload, EditarGrupoAcessoResponse } from '@domain/entities/GrupoAcesso';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class EditarGrupoAcessoUseCase {
  constructor(
    @inject(DiTokens.grupoAcessoRepository)
    private readonly repository: GrupoAcessoRepository,
  ) {}

  async execute(token: string, payload: EditarGrupoAcessoPayload): Promise<EditarGrupoAcessoResponse> {
    return this.repository.editarGrupo(token, payload);
  }
}
