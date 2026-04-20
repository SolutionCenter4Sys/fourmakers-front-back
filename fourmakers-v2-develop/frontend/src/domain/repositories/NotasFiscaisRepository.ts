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

export interface NotasFiscaisRepository {
  getNotasFiscais(): Promise<NotaFiscalMock[]>
  getNotasFiscaisGestao(): Promise<NotaFiscalGestaoMock[]>
  getColaboradoresOptions(): Promise<ColaboradorOptionMock[]>
  getUnidadesOptions(): Promise<UnidadeOptionMock[]>
  // Novos métodos para APIs reais
  listarNotaFiscalStatus(token: string): Promise<ListarNotaFiscalStatusResponse>
  listarUnidadesPorOrgId(token: string): Promise<ListarUnidadesResponse>
  listarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse>
  listarNotasFiscaisPorVigenciaVisaoGestor(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse>
  listarNotasFiscaisPorVigencia(
    token: string,
    params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams
  ): Promise<ListarNotasFiscaisPorVigenciaVisaoGestorResponse>
  aprovarNotasFiscais(
    token: string,
    params: AprovarNotasFiscaisParams
  ): Promise<AprovarNotasFiscaisResponse>
  reprovarNotasFiscais(
    token: string,
    params: ReprovarNotasFiscaisParams
  ): Promise<ReprovarNotasFiscaisResponse>
  liberarEmissaoNotasFiscaisPorVigencia(
    token: string,
    params: LiberarEmissaoNotasFiscaisPorVigenciaParams
  ): Promise<LiberarEmissaoNotasFiscaisPorVigenciaResponse>
  listarRubricasColaboradorParaLiberacaoDeNf(
    token: string,
    params: ListarRubricasColaboradorParaLiberacaoDeNfParams
  ): Promise<ListarRubricasColaboradorParaLiberacaoDeNfResponse>
  inserirNotaFiscal(
    token: string,
    params: InserirNotaFiscalParams
  ): Promise<InserirNotaFiscalResponse>
}

