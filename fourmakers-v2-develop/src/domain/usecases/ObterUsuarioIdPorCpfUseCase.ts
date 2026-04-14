import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { PermissionamentoRepository } from '@domain/repositories/PermissionamentoRepository';

@injectable()
export class ObterUsuarioIdPorCpfUseCase {
  constructor(
    @inject(DiTokens.permissionamentoRepository)
    private readonly repository: PermissionamentoRepository,
  ) {}

  async execute(token: string, cpf: string): Promise<number | null> {
    return this.repository.obterUsuarioIdPorCpf(token, cpf);
  }
}
