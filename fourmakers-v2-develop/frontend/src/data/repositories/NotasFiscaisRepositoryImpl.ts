import { inject, injectable } from 'tsyringe'

import type { NotasFiscaisRepository } from '@domain/repositories/NotasFiscaisRepository'
import type {
  NotaFiscalMock,
  NotaFiscalGestaoMock,
  ColaboradorOptionMock,
  UnidadeOptionMock,
} from '@data/mocks/notasFiscaisMock'
import type {
  ListarNotaFiscalStatusResponse,
  ListarUnidadesResponse,
  ListarColaboradoresETbdsResponse,
  ListarColaboradoresETbdsParams,
  ListarNotasFiscaisPorVigenciaVisaoGestorParams,
  ListarNotasFiscaisPorVigenciaVisaoGestorResponse,
  AprovarNotasFiscaisParams,
  AprovarNotasFiscaisResponse,
  ReprovarNotasFiscaisParams,
  ReprovarNotasFiscaisResponse,
  LiberarEmissaoNotasFiscaisPorVigenciaParams,
  LiberarEmissaoNotasFiscaisPorVigenciaResponse,
  ListarRubricasColaboradorParaLiberacaoDeNfParams,
  ListarRubricasColaboradorParaLiberacaoDeNfResponse,
  InserirNotaFiscalParams,
  InserirNotaFiscalResponse,
} from '@domain/entities/NotaFiscalGestao'

import { NotasFiscaisApi } from '@data/api/NotasFiscaisApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class NotasFiscaisRepositoryImpl implements NotasFiscaisRepository {
  constructor(
    @inject(DiTokens.notasFiscaisApi)
    private readonly api: NotasFiscaisApi,
  ) {}

  async getNotasFiscais(): Promise<NotaFiscalMock[]> {
    return this.api.getNotasFiscais()
  }

  async getNotasFiscaisGestao(): Promise<NotaFiscalGestaoMock[]> {
    return this.api.getNotasFiscaisGestao()
  }

  async getColaboradoresOptions(): Promise<ColaboradorOptionMock[]> {
    return this.api.getColaboradoresOptions()
  }

  async getUnidadesOptions(): Promise<UnidadeOptionMock[]> {
    return this.api.getUnidadesOptions()
  }

  // Novos métodos para APIs reais
  async listarNotaFiscalStatus(token: string): Promise<ListarNotaFiscalStatusResponse> {
    return this.api.listarNotaFiscalStatus(token)
  }

  async listarUnidadesPorOrgId(token: string): Promise<ListarUnidadesResponse> {
    return this.api.listarUnidadesPorOrgId(token)
  }

  async listarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse> {
    return this.api.listarColaboradoresETbds(token, params)
  }

  async listarNotasFiscaisPorVigenciaVisaoGestor(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse> {
    return this.api.listarNotasFiscaisPorVigenciaVisaoGestor(token, params)
  }

  async listarNotasFiscaisPorVigencia(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse> {
    return this.api.listarNotasFiscaisPorVigencia(token, params)
  }

  async aprovarNotasFiscais(
    token: string,
    params: AprovarNotasFiscaisParams
  ): Promise<AprovarNotasFiscaisResponse> {
    return this.api.aprovarNotasFiscais(token, params)
  }

  async reprovarNotasFiscais(
    token: string,
    params: ReprovarNotasFiscaisParams
  ): Promise<ReprovarNotasFiscaisResponse> {
    return this.api.reprovarNotasFiscais(token, params)
  }

  async liberarEmissaoNotasFiscaisPorVigencia(
    token: string,
    params: LiberarEmissaoNotasFiscaisPorVigenciaParams
  ): Promise<LiberarEmissaoNotasFiscaisPorVigenciaResponse> {
    return this.api.liberarEmissaoNotasFiscaisPorVigencia(token, params)
  }

  async listarRubricasColaboradorParaLiberacaoDeNf(
    token: string,
    params: ListarRubricasColaboradorParaLiberacaoDeNfParams
  ): Promise<ListarRubricasColaboradorParaLiberacaoDeNfResponse> {
    return this.api.listarRubricasColaboradorParaLiberacaoDeNf(token, params)
  }

  async inserirNotaFiscal(
    token: string,
    params: InserirNotaFiscalParams
  ): Promise<InserirNotaFiscalResponse> {
    return this.api.inserirNotaFiscal(token, params)
  }
}

