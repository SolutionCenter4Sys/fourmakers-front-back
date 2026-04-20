import type { ClienteGestaoAlocados } from '@domain/entities/ClienteGestaoAlocados';

export interface PermissionamentoRepository {
  obterUsuarioIdPorCpf(token: string, cpf: string): Promise<number | null>;
  listarClientes(token: string, busca: string): Promise<ClienteGestaoAlocados[]>;
}
