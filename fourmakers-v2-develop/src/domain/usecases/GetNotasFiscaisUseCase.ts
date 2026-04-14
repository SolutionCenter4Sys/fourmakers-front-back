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

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetNotasFiscaisUseCase {
  constructor(
    @inject(DiTokens.notasFiscaisRepository)
    private readonly repository: NotasFiscaisRepository,
  ) {}

  async execute(): Promise<NotaFiscalMock[]> {
    return this.repository.getNotasFiscais()
  }

  async executeGestao(): Promise<NotaFiscalGestaoMock[]> {
    return this.repository.getNotasFiscaisGestao()
  }

  async executeColaboradoresOptions(): Promise<ColaboradorOptionMock[]> {
    return this.repository.getColaboradoresOptions()
  }

  async executeUnidadesOptions(): Promise<UnidadeOptionMock[]> {
    return this.repository.getUnidadesOptions()
  }

  // Novos métodos para APIs reais
  async executeListarStatus(token: string): Promise<ListarNotaFiscalStatusResponse> {
    return this.repository.listarNotaFiscalStatus(token)
  }

  async executeListarUnidades(token: string): Promise<ListarUnidadesResponse> {
    return this.repository.listarUnidadesPorOrgId(token)
  }

  async executeListarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse> {
    return this.repository.listarColaboradoresETbds(token, params)
  }

  async executeListarNotasFiscaisPorVigenciaVisaoGestor(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse> {
    return this.repository.listarNotasFiscaisPorVigenciaVisaoGestor(token, params)
  }

  async executeListarNotasFiscaisPorVigencia(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse> {
    return this.repository.listarNotasFiscaisPorVigencia(token, params)
  }

  async executeAprovarNotasFiscais(
    token: string,
    params: AprovarNotasFiscaisParams
  ): Promise<AprovarNotasFiscaisResponse> {
    return this.repository.aprovarNotasFiscais(token, params)
  }

  async executeReprovarNotasFiscais(
    token: string,
    params: ReprovarNotasFiscaisParams
  ): Promise<ReprovarNotasFiscaisResponse> {
    return this.repository.reprovarNotasFiscais(token, params)
  }

  async executeLiberarEmissaoNotasFiscaisPorVigencia(
    token: string,
    params: LiberarEmissaoNotasFiscaisPorVigenciaParams
  ): Promise<LiberarEmissaoNotasFiscaisPorVigenciaResponse> {
    return this.repository.liberarEmissaoNotasFiscaisPorVigencia(token, params)
  }

  async executeListarRubricasColaboradorParaLiberacaoDeNf(
    token: string,
    params: ListarRubricasColaboradorParaLiberacaoDeNfParams
  ): Promise<ListarRubricasColaboradorParaLiberacaoDeNfResponse> {
    return this.repository.listarRubricasColaboradorParaLiberacaoDeNf(token, params)
  }

  async executeInserirNotaFiscal(
    token: string,
    params: InserirNotaFiscalParams
  ): Promise<InserirNotaFiscalResponse> {
    return this.repository.inserirNotaFiscal(token, params)
  }
}

