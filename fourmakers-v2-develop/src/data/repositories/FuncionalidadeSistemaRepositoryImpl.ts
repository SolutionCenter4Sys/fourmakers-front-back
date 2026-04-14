import { injectable } from 'tsyringe';
import { listarFuncionalidadesSistema } from '@data/api/FuncionalidadeSistemaApi';
import type { FuncionalidadeSistemaRepository } from '@domain/repositories/FuncionalidadeSistemaRepository';

@injectable()
export class FuncionalidadeSistemaRepositoryImpl implements FuncionalidadeSistemaRepository {
  async listar(token: string) {
    return listarFuncionalidadesSistema(token);
  }
}
