import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository';
import type {
  ListarAlocacoesPayload,
  ListarAlocacoesResponse,
  ListarColaboradoresETbdsParams,
  ListarColaboradoresETbdsResponse,
  ListarProjetosColaboradorMapaParams,
  ListarProjetosColaboradorMapaResponse,
  SubstituirDadosAlocacoesPorPeriodoPayload,
  SubstituirDadosAlocacoesPorPeriodoResponse,
  EditarAlocacaoPayload,
  EditarAlocacaoResponse,
  RemoverAlocacoesEmLotePayload,
  RemoverAlocacoesEmLoteResponse,
  ExportarRelatorioAlocacoesPayload,
  ListarNomesGestoresParams,
  ListarNomesGestoresResponse,
  CadastrarMapaAlocacaoPayload,
  CadastrarMapaAlocacaoResponse,
  ListarPerfilAlocacaoParams,
  ListarPerfilAlocacaoResponse,
  AlteraPerfilAlocacaoPayload,
  AlteraPerfilAlocacaoResponse,
  GetMapaAlocacaoResumoParams,
  GetMapaAlocacaoResumoResponse,
  GetMapaAlocacaoRecursoParams,
  GetMapaAlocacaoRecursoResponse,
  ConsultaProjetoHorasParams,
  ConsultaProjetoHorasResponse
} from '@domain/entities/MapaAlocacao';
import { MapaAlocacaoApi } from '@data/api/MapaAlocacaoApi';

@injectable()
export class MapaAlocacaoRepositoryImpl implements MapaAlocacaoRepository {
  constructor(
    @inject(DiTokens.mapaAlocacaoApi)
    private readonly mapaAlocacaoApi: MapaAlocacaoApi
  ) {}

  async listarAlocacoesColabETbd(
    token: string,
    payload: ListarAlocacoesPayload
  ): Promise<ListarAlocacoesResponse> {
    return this.mapaAlocacaoApi.listarAlocacoesColabETbd(token, payload);
  }

  async listarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse> {
    return this.mapaAlocacaoApi.listarColaboradoresETbds(token, params);
  }

  async listarProjetosColaborador(
    token: string,
    params?: ListarProjetosColaboradorMapaParams
  ): Promise<ListarProjetosColaboradorMapaResponse> {
    return this.mapaAlocacaoApi.listarProjetosColaborador(token, params);
  }

  async substituirDadosAlocacoesPorPeriodo(
    token: string,
    payload: SubstituirDadosAlocacoesPorPeriodoPayload
  ): Promise<SubstituirDadosAlocacoesPorPeriodoResponse> {
    return this.mapaAlocacaoApi.substituirDadosAlocacoesPorPeriodo(token, payload);
  }

  async editarAlocacao(
    token: string,
    payload: EditarAlocacaoPayload
  ): Promise<EditarAlocacaoResponse> {
    return this.mapaAlocacaoApi.editarAlocacao(token, payload);
  }

  async removerAlocacoesEmLote(
    token: string,
    payload: RemoverAlocacoesEmLotePayload
  ): Promise<RemoverAlocacoesEmLoteResponse> {
    return this.mapaAlocacaoApi.removerAlocacoesEmLote(token, payload);
  }

  async exportarRelatorioAlocacoes(
    token: string,
    payload: ExportarRelatorioAlocacoesPayload
  ): Promise<Response> {
    return this.mapaAlocacaoApi.exportarRelatorioAlocacoes(token, payload);
  }

  async listarNomesGestores(
    token: string,
    params?: ListarNomesGestoresParams
  ): Promise<ListarNomesGestoresResponse> {
    return this.mapaAlocacaoApi.listarNomesGestores(token, params);
  }

  async cadastrarMapaAlocacao(
    token: string,
    payload: CadastrarMapaAlocacaoPayload
  ): Promise<CadastrarMapaAlocacaoResponse> {
    return this.mapaAlocacaoApi.cadastrarMapaAlocacao(token, payload);
  }

  async listarPerfilAlocacao(
    token: string,
    params?: ListarPerfilAlocacaoParams
  ): Promise<ListarPerfilAlocacaoResponse> {
    return this.mapaAlocacaoApi.listarPerfilAlocacao(token, params);
  }

  async alteraPerfilAlocacao(
    token: string,
    payload: AlteraPerfilAlocacaoPayload
  ): Promise<AlteraPerfilAlocacaoResponse> {
    return this.mapaAlocacaoApi.alteraPerfilAlocacao(token, payload);
  }

  async getMapaAlocacaoResumo(
    token: string,
    params: GetMapaAlocacaoResumoParams
  ): Promise<GetMapaAlocacaoResumoResponse> {
    return this.mapaAlocacaoApi.getMapaAlocacaoResumo(token, params);
  }

  async getMapaAlocacaoRecurso(
    token: string,
    params: GetMapaAlocacaoRecursoParams
  ): Promise<GetMapaAlocacaoRecursoResponse> {
    return this.mapaAlocacaoApi.getMapaAlocacaoRecurso(token, params);
  }

  async consultaProjetoHoras(
    token: string,
    params: ConsultaProjetoHorasParams
  ): Promise<ConsultaProjetoHorasResponse> {
    return this.mapaAlocacaoApi.consultaProjetoHoras(token, params);
  }
}
