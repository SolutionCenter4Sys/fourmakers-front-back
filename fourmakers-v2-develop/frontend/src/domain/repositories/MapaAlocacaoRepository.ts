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

export interface MapaAlocacaoRepository {
  listarAlocacoesColabETbd(
    token: string,
    payload: ListarAlocacoesPayload
  ): Promise<ListarAlocacoesResponse>;
  
  listarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse>;
  
  listarProjetosColaborador(
    token: string,
    params?: ListarProjetosColaboradorMapaParams
  ): Promise<ListarProjetosColaboradorMapaResponse>;
  
  substituirDadosAlocacoesPorPeriodo(
    token: string,
    payload: SubstituirDadosAlocacoesPorPeriodoPayload
  ): Promise<SubstituirDadosAlocacoesPorPeriodoResponse>;
  
  editarAlocacao(
    token: string,
    payload: EditarAlocacaoPayload
  ): Promise<EditarAlocacaoResponse>;
  
  removerAlocacoesEmLote(
    token: string,
    payload: RemoverAlocacoesEmLotePayload
  ): Promise<RemoverAlocacoesEmLoteResponse>;
  
  exportarRelatorioAlocacoes(
    token: string,
    payload: ExportarRelatorioAlocacoesPayload
  ): Promise<Response>;
  
  listarNomesGestores(
    token: string,
    params?: ListarNomesGestoresParams
  ): Promise<ListarNomesGestoresResponse>;
  
  cadastrarMapaAlocacao(
    token: string,
    payload: CadastrarMapaAlocacaoPayload
  ): Promise<CadastrarMapaAlocacaoResponse>;
  
  listarPerfilAlocacao(
    token: string,
    params?: ListarPerfilAlocacaoParams
  ): Promise<ListarPerfilAlocacaoResponse>;
  
  alteraPerfilAlocacao(
    token: string,
    payload: AlteraPerfilAlocacaoPayload
  ): Promise<AlteraPerfilAlocacaoResponse>;

  getMapaAlocacaoResumo(
    token: string,
    params: GetMapaAlocacaoResumoParams
  ): Promise<GetMapaAlocacaoResumoResponse>;

  getMapaAlocacaoRecurso(
    token: string,
    params: GetMapaAlocacaoRecursoParams
  ): Promise<GetMapaAlocacaoRecursoResponse>;

  consultaProjetoHoras(
    token: string,
    params: ConsultaProjetoHorasParams
  ): Promise<ConsultaProjetoHorasResponse>;
}
