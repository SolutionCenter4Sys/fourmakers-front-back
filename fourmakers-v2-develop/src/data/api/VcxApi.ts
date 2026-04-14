import { injectable } from 'tsyringe'

import { httpClient } from './httpClient'
import type {
  DorPayload,
  DorResponse,
  ImpactoResponse,
  UrgenciaResponse,
  ApiGenericResult,
} from '@domain/entities/VcxDores'
import type {
  IniciativaPayload,
  IniciativaResponse,
  StatusIniciativaResponse,
  TemaResponse,
  TemaPayload,
} from '@domain/entities/VcxIniciativas'
import type { HistoricoDoresIniciativasResponse } from '@domain/entities/VcxHistorico'
import type {
  VcxAgendasPorColaboradorClienteResult,
} from '@domain/entities/VcxAgenda'

@injectable()
export class VcxApi {
  /**
   * Lista todas as dores de uma posição do organograma
   * GET api/MapaDeRelacionamento/VCX/ListarDoresPorPosicaoId?posicaoId={guid}
   */
  async listarDoresPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<DorResponse[]> {
    const queryParams = new URLSearchParams({
      posicaoId,
    })

    const response = await httpClient.get<
      ApiGenericResult<DorResponse[]>
    >(
      `/api/MapaDeRelacionamento/VCX/ListarDoresPorPosicaoId?${queryParams.toString()}`,
      { token },
    )

    // Extrair o array retorno da resposta
    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }

    return response.retorno
  }

  /**
   * Busca uma dor específica por ID
   * GET api/MapaDeRelacionamento/VCX/BuscarDorPorId?id={guid}
   */
  async buscarDorPorId(token: string, id: string): Promise<DorResponse> {
    const queryParams = new URLSearchParams({
      id,
    })

    const response = await httpClient.get<ApiGenericResult<DorResponse>>(
      `/api/MapaDeRelacionamento/VCX/BuscarDorPorId?${queryParams.toString()}`,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(response?.mensagem || 'Erro ao buscar dor')
    }

    return response.retorno
  }

  /**
   * Cria uma nova dor
   * POST api/MapaDeRelacionamento/VCX/CriarDor
   */
  async criarDor(token: string, payload: DorPayload): Promise<DorResponse> {
    const response = await httpClient.post<ApiGenericResult<DorResponse>>(
      '/api/MapaDeRelacionamento/VCX/CriarDor',
      payload,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(
        response?.mensagem || response?.erros?.join(', ') || 'Erro ao criar dor',
      )
    }

    return response.retorno
  }

  /**
   * Atualiza uma dor existente
   * PUT api/MapaDeRelacionamento/VCX/AtualizarDor
   */
  async atualizarDor(
    token: string,
    payload: DorPayload & { id: string },
  ): Promise<DorResponse> {
    const response = await httpClient.put<ApiGenericResult<DorResponse>>(
      '/api/MapaDeRelacionamento/VCX/AtualizarDor',
      payload,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao atualizar dor',
      )
    }

    return response.retorno
  }

  /**
   * Exclui uma dor
   * DELETE api/MapaDeRelacionamento/VCX/ExcluirDor?id={guid}
   */
  async excluirDor(token: string, id: string): Promise<void> {
    const queryParams = new URLSearchParams({
      id,
    })

    const response = await httpClient.delete<ApiGenericResult<boolean>>(
      `/api/MapaDeRelacionamento/VCX/ExcluirDor?${queryParams.toString()}`,
      { token },
    )

    if (!response || !response.sucesso) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao excluir dor',
      )
    }
  }

  /**
   * Lista todas as opções de Impacto disponíveis
   * GET api/MapaDeRelacionamento/VCX/ListarImpactosDores
   */
  async listarImpactosDores(token: string): Promise<ImpactoResponse[]> {
    const response = await httpClient.get<
      ApiGenericResult<ImpactoResponse[]>
    >('/api/MapaDeRelacionamento/VCX/ListarImpactosDores', { token })

    // Extrair o array retorno da resposta
    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }

    return response.retorno
  }

  /**
   * Lista todas as opções de Urgência disponíveis
   * GET api/MapaDeRelacionamento/VCX/ListarUrgenciasDores
   */
  async listarUrgenciasDores(token: string): Promise<UrgenciaResponse[]> {
    const response = await httpClient.get<
      ApiGenericResult<UrgenciaResponse[]>
    >('/api/MapaDeRelacionamento/VCX/ListarUrgenciasDores', { token })

    // Extrair o array retorno da resposta
    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }

    return response.retorno
  }

  // ============================================================================
  // MÉTODOS DE INICIATIVAS
  // ============================================================================

  /**
   * Lista todas as iniciativas de uma posição do organograma
   * GET api/MapaDeRelacionamento/VCX/ListarIniciativasPorPosicaoId?posicaoId={guid}
   */
  async listarIniciativasPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<IniciativaResponse[]> {
    const queryParams = new URLSearchParams({
      posicaoId,
    })

    const response = await httpClient.get<
      ApiGenericResult<IniciativaResponse[]>
    >(
      `/api/MapaDeRelacionamento/VCX/ListarIniciativasPorPosicaoId?${queryParams.toString()}`,
      { token },
    )

    // Extrair o array retorno da resposta
    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }

    return response.retorno
  }

  /**
   * Busca uma iniciativa específica por ID
   * GET api/MapaDeRelacionamento/VCX/BuscarIniciativaPorId?id={guid}
   */
  async buscarIniciativaPorId(
    token: string,
    id: string,
  ): Promise<IniciativaResponse> {
    const queryParams = new URLSearchParams({
      id,
    })

    const response = await httpClient.get<
      ApiGenericResult<IniciativaResponse>
    >(
      `/api/MapaDeRelacionamento/VCX/BuscarIniciativaPorId?${queryParams.toString()}`,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(response?.mensagem || 'Erro ao buscar iniciativa')
    }

    return response.retorno
  }

  /**
   * Cria uma nova iniciativa
   * POST api/MapaDeRelacionamento/VCX/CriarIniciativa
   */
  async criarIniciativa(
    token: string,
    payload: IniciativaPayload,
  ): Promise<IniciativaResponse> {
    const response = await httpClient.post<
      ApiGenericResult<IniciativaResponse>
    >('/api/MapaDeRelacionamento/VCX/CriarIniciativa', payload, { token })

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao criar iniciativa',
      )
    }

    return response.retorno
  }

  /**
   * Atualiza uma iniciativa existente
   * PUT api/MapaDeRelacionamento/VCX/AtualizarIniciativa
   */
  async atualizarIniciativa(
    token: string,
    payload: IniciativaPayload & { id: string },
  ): Promise<IniciativaResponse> {
    const response = await httpClient.put<ApiGenericResult<IniciativaResponse>>(
      '/api/MapaDeRelacionamento/VCX/AtualizarIniciativa',
      payload,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao atualizar iniciativa',
      )
    }

    return response.retorno
  }

  /**
   * Exclui uma iniciativa
   * DELETE api/MapaDeRelacionamento/VCX/ExcluirIniciativa?id={guid}
   */
  async excluirIniciativa(token: string, id: string): Promise<void> {
    const queryParams = new URLSearchParams({
      id,
    })

    const response = await httpClient.delete<ApiGenericResult<boolean>>(
      `/api/MapaDeRelacionamento/VCX/ExcluirIniciativa?${queryParams.toString()}`,
      { token },
    )

    if (!response || !response.sucesso) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao excluir iniciativa',
      )
    }
  }

  /**
   * Lista todas as opções de Status de Iniciativas disponíveis
   * GET api/MapaDeRelacionamento/VCX/ListarStatusIniciativas
   */
  async listarStatusIniciativas(
    token: string,
  ): Promise<StatusIniciativaResponse[]> {
    const response = await httpClient.get<
      ApiGenericResult<StatusIniciativaResponse[]>
    >('/api/MapaDeRelacionamento/VCX/ListarStatusIniciativas', { token })

    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }
    return response.retorno
  }

  /**
   * Lista todos os temas disponíveis (para autocomplete)
   * GET api/MapaDeRelacionamento/VCX/ListarTemas
   */
  async listarTemas(token: string): Promise<TemaResponse[]> {
    const response = await httpClient.get<ApiGenericResult<TemaResponse[]>>(
      '/api/MapaDeRelacionamento/VCX/ListarTemas',
      { token },
    )

    // Extrair o array retorno da resposta
    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }

    return response.retorno
  }

  /**
   * Cria um novo tema
   * POST api/MapaDeRelacionamento/VCX/CriarTema
   */
  async criarTema(token: string, payload: TemaPayload): Promise<TemaResponse> {
    const response = await httpClient.post<ApiGenericResult<TemaResponse>>(
      '/api/MapaDeRelacionamento/VCX/CriarTema',
      payload,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao criar tema',
      )
    }

    return response.retorno
  }

  // ============================================================================
  // MÉTODOS DE HISTÓRICO
  // ============================================================================

  /**
   * Lista histórico de dores e iniciativas por posição
   * GET api/MapaDeRelacionamento/VCX/ListarHistoricoDoresIniciativasPorPosicaoId?posicaoId={guid}
   */
  async listarHistoricoDoresIniciativasPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<HistoricoDoresIniciativasResponse> {
    const queryParams = new URLSearchParams({
      posicaoId,
    })

    const response = await httpClient.get<
      ApiGenericResult<HistoricoDoresIniciativasResponse>
    >(
      `/api/MapaDeRelacionamento/VCX/ListarHistoricoDoresIniciativasPorPosicaoId?${queryParams.toString()}`,
      { token },
    )

    if (!response || !response.sucesso || !response.retorno) {
      throw new Error(
        response?.mensagem ||
          response?.erros?.join(', ') ||
          'Erro ao buscar histórico',
      )
    }

    return response.retorno
  }

  /**
   * Lista agendas por colaborador e cliente (paginação por cursor)
   * GET api/MapaDeRelacionamento/VCX/ListarAgendasPorColaboradorCliente
   */
  async listarAgendasPorColaboradorCliente(
    token: string,
    codigoColaborador: string,
    codigoCliente: string,
    limit = 20,
    cursor = 0,
  ): Promise<VcxAgendasPorColaboradorClienteResult> {
    const queryParams = new URLSearchParams({
      codigoColaborador,
      codigoCliente,
      limit: String(limit),
      cursor: String(cursor),
    })

    try {
      const response = await httpClient.get<
        ApiGenericResult<VcxAgendasPorColaboradorClienteResult>
      >(
        `/api/MapaDeRelacionamento/VCX/ListarAgendasPorColaboradorCliente?${queryParams.toString()}`,
        { token },
      )

      // Normalizar backend que pode retornar error/message em vez de sucesso/mensagem
      const sucesso =
        (response as { sucesso?: boolean; error?: boolean }).sucesso !== false &&
        (response as { error?: boolean }).error !== true
      const retorno = (response as { retorno?: VcxAgendasPorColaboradorClienteResult })
        ?.retorno

      if (!sucesso || !retorno) {
        const mensagem =
          (response as { mensagem?: string }).mensagem ||
          (response as { message?: string }).message ||
          'Erro ao listar agendas'
        if (typeof mensagem === 'string' && mensagem.includes('404')) {
          throw new Error('Endpoint de agenda não disponível (404).')
        }
        throw new Error(mensagem)
      }

      return {
        agendasAntigas: Array.isArray(retorno.agendasAntigas)
          ? retorno.agendasAntigas
          : [],
        agendasNovas: Array.isArray(retorno.agendasNovas)
          ? retorno.agendasNovas
          : [],
      }
    } catch (err) {
      const msg = err instanceof Error ? err.message : String(err)
      if (msg.includes('404')) {
        throw new Error('Endpoint de agenda não disponível (404).')
      }
      throw err
    }
  }
}
