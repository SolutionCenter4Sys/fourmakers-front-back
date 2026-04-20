import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ListarFuncionalidadesSistemaResponse } from '@domain/entities/FuncionalidadeSistema';
import type { FuncionalidadeSistemaRepository } from '@domain/repositories/FuncionalidadeSistemaRepository';

@injectable()
export class ListarFuncionalidadesSistemaUseCase {
  constructor(
    @inject(DiTokens.funcionalidadeSistemaRepository)
    private readonly repository: FuncionalidadeSistemaRepository,
  ) {}

  async execute(token: string): Promise<ListarFuncionalidadesSistemaResponse> {
    return this.repository.listar(token);
  }
}
