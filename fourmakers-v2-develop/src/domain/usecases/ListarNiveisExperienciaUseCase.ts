import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { PerfilAtuacaoRepository } from '@domain/repositories/PerfilAtuacaoRepository';

@injectable()
export class ListarNiveisExperienciaUseCase {
  constructor(
    @inject(DiTokens.perfilAtuacaoRepository)
    private readonly repository: PerfilAtuacaoRepository,
  ) {}

  async execute(token: string) {
    return this.repository.listarNiveisExperiencia(token);
  }
}
