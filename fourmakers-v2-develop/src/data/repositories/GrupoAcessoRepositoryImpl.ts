import { injectable } from 'tsyringe';
import {
  listarGruposAcessoFuncionalidadesSistema,
  listarPessoasPorGrupoAcesso,
  criarGrupoAcesso,
  editarGrupoAcesso,
  adicionarGrupoAcessoFuncionalidadeSistema,
  removerGrupoAcessoFuncionalidadeSistema,
  adicionarUsuarioGrupoAcesso,
  removerUsuarioGrupoAcesso,
} from '@data/api/GrupoAcessoApi';
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository';

@injectable()
export class GrupoAcessoRepositoryImpl implements GrupoAcessoRepository {
  async listarGruposComFuncionalidades(token: string) {
    return listarGruposAcessoFuncionalidadesSistema(token);
  }

  async listarPessoasPorGrupoAcesso(token: string) {
    return listarPessoasPorGrupoAcesso(token);
  }

  async criarGrupo(token: string, payload: Parameters<typeof criarGrupoAcesso>[1]) {
    return criarGrupoAcesso(token, payload);
  }

  async editarGrupo(token: string, payload: Parameters<typeof editarGrupoAcesso>[1]) {
    return editarGrupoAcesso(token, payload);
  }

  async adicionarFuncionalidadeGrupo(token: string, grupoAcessoId: number, funcionalidadeSistemaId: number) {
    return adicionarGrupoAcessoFuncionalidadeSistema(token, grupoAcessoId, funcionalidadeSistemaId);
  }

  async removerFuncionalidadeGrupo(token: string, grupoAcessoId: number, funcionalidadeSistemaId: number) {
    return removerGrupoAcessoFuncionalidadeSistema(token, grupoAcessoId, funcionalidadeSistemaId);
  }

  async adicionarUsuarioGrupo(token: string, usuarioId: number, grupoAcessoId: number) {
    return adicionarUsuarioGrupoAcesso(token, usuarioId, grupoAcessoId);
  }

  async removerUsuarioGrupo(token: string, usuarioId: number, grupoAcessoId: number) {
    return removerUsuarioGrupoAcesso(token, usuarioId, grupoAcessoId);
  }
}
