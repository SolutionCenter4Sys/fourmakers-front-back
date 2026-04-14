import type { AlocadoMock } from '../mocks/mapaAlocacaoMock'
import { alocadosMock } from '../mocks/mapaAlocacaoMock'
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
} from '@domain/entities/MapaAlocacao'
import { httpClient } from './httpClient'

export interface ProjetoColaborador {
  codigoProjeto: string
  codigoCliente: string
  projetos: string
  cliente: string
  label?: string
}

export class MapaAlocacaoApi {
  async getAlocados(): Promise<AlocadoMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...alocadosMock]
  }

  async listarProjetosColaborador(
    token: string,
    params?: ListarProjetosColaboradorMapaParams
  ): Promise<ListarProjetosColaboradorMapaResponse> {
    const payload: ListarProjetosColaboradorMapaParams = {
      codigoProfissional: params?.codigoProfissional || '',
      ehTbd: params?.ehTbd ?? null,
      codigoGerenteProjeto: params?.codigoGerenteProjeto || '',
      listaCodigoCliente: params?.listaCodigoCliente || [],
      status: params?.status,
      prioritarioFiltro: params?.prioritarioFiltro,
    }

    return httpClient.post<ListarProjetosColaboradorMapaResponse>(
      '/api/MapaDeAlocacao/ListarProjetosColaborador',
      payload,
      { token }
    )
  }

  async listarAlocacoesColabETbd(
    token: string,
    payload: ListarAlocacoesPayload
  ): Promise<ListarAlocacoesResponse> {
    return httpClient.post<ListarAlocacoesResponse>(
      '/api/MapaDeAlocacao/ListarAlocacoesColabETbd',
      payload,
      { token }
    )
  }

  async listarColaboradoresETbds(
    token: string,
    params?: ListarColaboradoresETbdsParams
  ): Promise<ListarColaboradoresETbdsResponse> {
    const queryParams = new URLSearchParams();
    
    if (params?.codigoDiretoria !== undefined) {
      queryParams.append('codigoDiretoria', params.codigoDiretoria.toString());
    }
    if (params?.codigoGestor !== undefined) {
      queryParams.append('codigoGestor', params.codigoGestor.toString());
    }
    if (params?.filtroTipoProfissional !== undefined) {
      queryParams.append('filtroTipoProfissional', params.filtroTipoProfissional.toString());
    }
    if (params?.codigoDepartamento) {
      queryParams.append('codigoDepartamento', params.codigoDepartamento);
    }

    const queryString = queryParams.toString();
    const url = `/api/MapaDeAlocacao/ListarColaboradoresETbds${queryString ? `?${queryString}` : ''}`;

    return httpClient.get<ListarColaboradoresETbdsResponse>(url, { token });
  }

  async substituirDadosAlocacoesPorPeriodo(
    token: string,
    payload: SubstituirDadosAlocacoesPorPeriodoPayload
  ): Promise<SubstituirDadosAlocacoesPorPeriodoResponse> {
    return httpClient.post<SubstituirDadosAlocacoesPorPeriodoResponse>(
      '/api/MapaDeAlocacao/SubstituirDadosAlocacaoesPorPeriodo',
      payload,
      { token }
    );
  }

  async editarAlocacao(
    token: string,
    payload: EditarAlocacaoPayload
  ): Promise<EditarAlocacaoResponse> {
    return httpClient.post<EditarAlocacaoResponse>(
      '/api/MapaDeAlocacao/EditarAlocacao',
      payload,
      { token }
    );
  }

  async removerAlocacoesEmLote(
    token: string,
    payload: RemoverAlocacoesEmLotePayload
  ): Promise<RemoverAlocacoesEmLoteResponse> {
    return httpClient.post<RemoverAlocacoesEmLoteResponse>(
      '/api/MapaDeAlocacao/RemoverAlocacoesEmLote',
      payload,
      { token }
    );
  }

  async exportarRelatorioAlocacoes(
    token: string,
    payload: ExportarRelatorioAlocacoesPayload
  ): Promise<Response> {
    return httpClient.postBlob(
      '/api/MapaDeAlocacao/Exportacao/RelatorioAlocacoesPorPesquisa',
      payload,
      { token }
    );
  }

  async listarNomesGestores(
    token: string,
    params?: ListarNomesGestoresParams
  ): Promise<ListarNomesGestoresResponse> {
    const queryParams = new URLSearchParams();
    
    // Priorizar novos nomes, mas manter compatibilidade
    const diretoriaParam = params?.codigoDiretoria ?? params?.codDiretoria;
    if (diretoriaParam !== undefined && diretoriaParam !== null) {
      queryParams.append('codigoDiretoria', diretoriaParam.toString());
    }
    
    const departamentoParam = params?.codigoDepartamento ?? params?.codDepartamento;
    if (departamentoParam !== undefined && departamentoParam !== null) {
      queryParams.append('codigoDepartamento', departamentoParam);
    }

    const queryString = queryParams.toString();
    const url = `/api/MapaDeAlocacao/ListarNomesGestores${queryString ? `?${queryString}` : ''}`;

    return httpClient.get<ListarNomesGestoresResponse>(url, { token });
  }

  async cadastrarMapaAlocacao(
    token: string,
    payload: CadastrarMapaAlocacaoPayload
  ): Promise<CadastrarMapaAlocacaoResponse> {
    return httpClient.post<CadastrarMapaAlocacaoResponse>(
      '/api/MapaDeAlocacao/CadastroMapaAlocacao',
      payload,
      { token }
    )
  }

  async listarPerfilAlocacao(
    token: string,
    params?: ListarPerfilAlocacaoParams
  ): Promise<ListarPerfilAlocacaoResponse> {
    const queryParams = new URLSearchParams();
    
    if (params?.codProjeto) {
      queryParams.append('codProjeto', params.codProjeto);
    } else {
      queryParams.append('codProjeto', '');
    }
    
    if (params?.ocultarSkill !== undefined) {
      queryParams.append('ocultarSkill', params.ocultarSkill.toString());
    } else {
      queryParams.append('ocultarSkill', 'true');
    }

    const queryString = queryParams.toString();
    const url = `/api/MapaDeAlocacao/ListarPerfilAlocacao${queryString ? `?${queryString}` : ''}`;

    return httpClient.get<ListarPerfilAlocacaoResponse>(url, { token });
  }

  async alteraPerfilAlocacao(
    token: string,
    payload: AlteraPerfilAlocacaoPayload
  ): Promise<AlteraPerfilAlocacaoResponse> {
    return httpClient.post<AlteraPerfilAlocacaoResponse>(
      '/api/MapaDeAlocacao/AlteraPerfilAlocacao',
      payload,
      { token }
    )
  }

  async getMapaAlocacaoResumo(
    token: string,
    params: GetMapaAlocacaoResumoParams
  ): Promise<GetMapaAlocacaoResumoResponse> {
    const queryParams = new URLSearchParams();
    queryParams.append('trimestral', params.trimestral.toString());
    queryParams.append('mesInicial', params.mesInicial.toString());
    queryParams.append('anoInicial', params.anoInicial.toString());

    const url = `/api/MapaDeAlocacao/GetMapaAlocacaoResumo?${queryParams.toString()}`;
    return httpClient.get<GetMapaAlocacaoResumoResponse>(url, { token });
  }

  async getMapaAlocacaoRecurso(
    token: string,
    params: GetMapaAlocacaoRecursoParams
  ): Promise<GetMapaAlocacaoRecursoResponse> {
    const queryParams = new URLSearchParams();
    if (params.cursor !== undefined) queryParams.append('cursor', params.cursor.toString());
    if (params.limite !== undefined) queryParams.append('limite', params.limite.toString());
    queryParams.append('trimestral', params.trimestral.toString());
    if (params.hardskill) queryParams.append('hardskill', params.hardskill);
    if (params.idioma) queryParams.append('idioma', params.idioma);
    if (params.statusHorasFiltro !== undefined) queryParams.append('statusHorasFiltro', params.statusHorasFiltro.toString());
    if (params.gestorFiltro) queryParams.append('gestorFiltro', params.gestorFiltro);
    if (params.codigoDiretoria) queryParams.append('codigoDiretoria', params.codigoDiretoria);
    if (params.colaboradorOuTbdFiltro) queryParams.append('colaboradorOuTbdFiltro', params.colaboradorOuTbdFiltro);

    const url = `/api/MapaDeAlocacao/GetMapaAlocacaoRecurso?${queryParams.toString()}`;
    return httpClient.get<GetMapaAlocacaoRecursoResponse>(url, { token });
  }

  async consultaProjetoHoras(
    token: string,
    params: ConsultaProjetoHorasParams
  ): Promise<ConsultaProjetoHorasResponse> {
    const queryParams = new URLSearchParams();
    // Sempre enviar todos os parâmetros, mesmo que vazios
    queryParams.append('cursor', (params.cursor ?? 0).toString());
    queryParams.append('limite', (params.limite ?? 20).toString());
    queryParams.append('cdProjeto', params.cdProjeto ?? '');
    queryParams.append('disponibilidadeHorario', (params.disponibilidadeHorario ?? 0).toString());
    queryParams.append('cdStatusProjeto', (params.cdStatusProjeto ?? 0).toString());
    queryParams.append('nomeProjeto', params.nomeProjeto ?? '');
    queryParams.append('codDiretoria', (params.codDiretoria ?? 0).toString());
    queryParams.append('cliente', params.cliente ?? '');
    queryParams.append('gestorProjeto', params.gestorProjeto ?? '');
    queryParams.append('cdCliente', params.cdCliente ?? '');

    const url = `/api/MapaDeAlocacao/ConsultaProjetoHoras?${queryParams.toString()}`;
    return httpClient.get<ConsultaProjetoHorasResponse>(url, { token });
  }
}

