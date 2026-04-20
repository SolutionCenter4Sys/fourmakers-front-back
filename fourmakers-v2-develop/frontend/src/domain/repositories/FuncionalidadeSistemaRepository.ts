import type { ListarFuncionalidadesSistemaResponse } from '@domain/entities/FuncionalidadeSistema';

export interface FuncionalidadeSistemaRepository {
  listar(token: string): Promise<ListarFuncionalidadesSistemaResponse>;
}
