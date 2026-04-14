import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoGrupoApi } from '@data/api/ComunicacaoGrupoApi';
import type {
  ComunicacaoGrupo,
  ColaboradorDisponivel,
  ComunicacaoGrupoDetalhe,
  CriarGrupoPayload,
  CriarGrupoResponse,
  PermissoesUsuarioLogado,
} from '@domain/entities/comunicacao';
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository';

@injectable()
export class ComunicacaoGrupoRepositoryImpl implements ComunicacaoGrupoRepository {
  constructor(
    @inject(DiTokens.comunicacaoGrupoApi)
    private readonly api: ComunicacaoGrupoApi,
  ) {}

  async listarGrupos(token: string): Promise<ComunicacaoGrupo[]> {
    const response = await this.api.getGrupos(token);
    if (!response.sucesso || !response.retorno) {
      return [];
    }
    return response.retorno;
  }

  async obterPermissoesUsuarioLogado(
    token: string,
  ): Promise<PermissoesUsuarioLogado | null> {
    const response = await this.api.getPermissoesUsuarioLogado(token);
    if (!response.sucesso || !response.retorno) {
      return null;
    }
    return response.retorno;
  }

  async criarGrupo(
    token: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse> {
    return this.api.criarGrupo(token, payload);
  }

  async obterGrupoPorId(
    token: string,
    grupoId: string,
  ): Promise<ComunicacaoGrupoDetalhe | null> {
    const response = await this.api.getGrupoById(token, grupoId);
    if (!response.sucesso || !response.retorno) {
      return null;
    }
    return response.retorno;
  }

  async atualizarGrupo(
    token: string,
    grupoId: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse> {
    return this.api.atualizarGrupo(token, grupoId, payload);
  }

  async deletarGrupo(
    token: string,
    grupoId: string,
  ): Promise<CriarGrupoResponse> {
    return this.api.deletarGrupo(token, grupoId);
  }

  async listarColaboradoresDisponiveis(
    token: string,
    params?: { grupoId?: string; filtro?: string },
  ): Promise<ColaboradorDisponivel[]> {
    const response = await this.api.getColaboradoresDisponiveis(
      token,
      params ?? {},
    );
    if (!response.sucesso || !response.retorno?.colaboradores) {
      return [];
    }
    return response.retorno.colaboradores;
  }
}
