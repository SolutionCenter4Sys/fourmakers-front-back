import { injectable } from 'tsyringe';
import { obterUsuarioIdPorCpf } from '@data/api/PermissionamentoUsuarioApi';
import { fetchClientes } from '@data/api/PerfilAtuacaoApi';
import type { PermissionamentoRepository } from '@domain/repositories/PermissionamentoRepository';
import type { ClienteGestaoAlocados } from '@domain/entities/ClienteGestaoAlocados';

@injectable()
export class PermissionamentoRepositoryImpl implements PermissionamentoRepository {
  async obterUsuarioIdPorCpf(token: string, cpf: string): Promise<number | null> {
    return obterUsuarioIdPorCpf(token, cpf);
  }

  async listarClientes(token: string, busca: string): Promise<ClienteGestaoAlocados[]> {
    const retorno = await fetchClientes(token, busca);
    return retorno.map((c) => ({
      codigoCliente: c.codigoCliente,
      nomeCliente: c.nomeCliente,
    }));
  }
}
