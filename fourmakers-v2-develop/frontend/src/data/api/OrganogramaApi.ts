import { httpClient } from './httpClient'

import type {
  DepartamentoPayload,
  DepartamentoResponse,
  PerfilCorporativoPayload,
  PerfilCorporativoResponse,
  PosicaoPayload,
  PosicaoResponse,
  AlocacaoPayload,
  AlocacaoResponse,
  PosicaoCompletaResponse,
} from '@domain/entities/Organograma'
import type { ClientesMapaRelacionamentoResponse } from '@domain/entities/MapaRelacionamento'

export class OrganogramaApi {
  // Listar Clientes por OrgId
  async retornarClientesPorOrgId(
    token: string,
    orgId: number,
    limite: number,
    cursor: number = 0,
    nomeCliente: string = '',
  ): Promise<ClientesMapaRelacionamentoResponse> {
    const queryParams = new URLSearchParams({
      orgId: orgId.toString(),
      limite: limite.toString(),
      cursor: cursor.toString(),
      nomeCliente: nomeCliente || '',
    })

    return httpClient.get<ClientesMapaRelacionamentoResponse>(
      `/api/Organograma/RetornarClientesPorOrgId?${queryParams.toString()}`,
      { token },
    )
  }

  // Listar Organograma Completo
  async listarOrganogramaCompleto(token: string, codigoCliente: string, orgId: number): Promise<PosicaoCompletaResponse> {
    const queryParams = new URLSearchParams({
      codigoCliente,
      orgId: orgId.toString(),
    })

    return httpClient.get<PosicaoCompletaResponse>(
      `/api/Organograma/OrganogramaCompletoPorCliente?${queryParams.toString()}`,
      { token },
    )
  }

  // Departamento
  async criarDepartamento(token: string, payload: DepartamentoPayload): Promise<DepartamentoResponse> {
    return httpClient.post<DepartamentoResponse>(
      '/api/Organograma/DepartamentoInserir',
      payload,
      { token },
    )
  }

  async atualizarDepartamento(
    token: string,
    payload: DepartamentoPayload & { id: string },
  ): Promise<DepartamentoResponse> {
    return httpClient.post<DepartamentoResponse>(
      '/api/Organograma/DepartamentoAtualizar',
      payload,
      { token },
    )
  }

  async deletarDepartamento(token: string, departamentoId: string): Promise<void> {
    const queryParams = new URLSearchParams({
      departamentotoId: departamentoId,
    })

    return httpClient.delete<void>(
      `/api/Organograma/DepartamentoDeletar?${queryParams.toString()}`,
      { token },
    )
  }

  // Perfil Corporativo
  async criarPerfilCorporativo(
    token: string,
    payload: PerfilCorporativoPayload,
  ): Promise<PerfilCorporativoResponse> {
    return httpClient.post<PerfilCorporativoResponse>(
      '/api/Organograma/PerfilCorporativoInserir',
      payload,
      { token },
    )
  }

  async atualizarPerfilCorporativo(
    token: string,
    payload: PerfilCorporativoPayload & { id: string },
  ): Promise<PerfilCorporativoResponse> {
    return httpClient.post<PerfilCorporativoResponse>(
      '/api/Organograma/PerfilCorporativoAtualizar',
      payload,
      { token },
    )
  }

  async deletarPerfilCorporativo(token: string, perfilCorpId: string): Promise<void> {
    const queryParams = new URLSearchParams({
      perfilCorpId: perfilCorpId,
    })

    return httpClient.delete<void>(
      `/api/Organograma/PerfilCorporativoDeletar?${queryParams.toString()}`,
      { token },
    )
  }

  // Posição
  async criarPosicao(token: string, payload: PosicaoPayload): Promise<PosicaoResponse> {
    return httpClient.post<PosicaoResponse>(
      '/api/Organograma/PosicaoInserir',
      payload,
      { token },
    )
  }

  async atualizarPosicao(token: string, payload: PosicaoPayload & { id: string }): Promise<PosicaoResponse> {
    return httpClient.post<PosicaoResponse>(
      '/api/Organograma/PosicaoAtualizar',
      payload,
      { token },
    )
  }

  async deletarPosicao(token: string, posicaoId: string): Promise<void> {
    const queryParams = new URLSearchParams({
      posicaoId: posicaoId,
    })

    return httpClient.delete<void>(
      `/api/Organograma/PosicaoDeletar?${queryParams.toString()}`,
      { token },
    )
  }

  // Alocação
  async criarAlocacao(token: string, payload: AlocacaoPayload): Promise<AlocacaoResponse> {
    return httpClient.post<AlocacaoResponse>(
      '/api/Organograma/AlocacaoInserir',
      payload,
      { token },
    )
  }

  async atualizarAlocacao(
    token: string,
    payload: AlocacaoPayload & { id: string },
  ): Promise<AlocacaoResponse> {
    return httpClient.post<AlocacaoResponse>(
      '/api/Organograma/AlocacaoAtualizar',
      payload,
      { token },
    )
  }

  async deletarAlocacao(token: string, alocacaoId: string): Promise<void> {
    const queryParams = new URLSearchParams({
      alocacaoId: alocacaoId,
    })

    return httpClient.delete<void>(
      `/api/Organograma/AlocacaoDeletar?${queryParams.toString()}`,
      { token },
    )
  }

  // Buscar Perfis Corporativos por OrgId
  async buscarPerfisPorOrg(
    token: string,
    orgId: number,
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[]; retorno: PerfilCorporativoResponse[] }> {
    const queryParams = new URLSearchParams({
      orgId: orgId.toString(),
    })

    return httpClient.get<{ sucesso: boolean; mensagem?: string; erros?: string[]; retorno: PerfilCorporativoResponse[] }>(
      `/api/Organograma/BuscarPerfisPorOrg?${queryParams.toString()}`,
      { token },
    )
  }

  // Buscar Perfil Corporativo por ID
  // O endpoint retorna o perfil completo com todos os campos
  async buscarPerfilCorporativoPorId(
    token: string,
    perfilCorpId: string,
  ): Promise<{
    sucesso: boolean
    mensagem?: string
    erros?: string[]
    retorno: PerfilCorporativoResponse & {
      codigoInternoColaboradorCriacao?: string
      codigoInternoColaboradorAlteracao?: string | null
      dataCriacao?: string
      dataAlteracao?: string
    }
  }> {
    const queryParams = new URLSearchParams({
      perfilCorpId: perfilCorpId,
    })

    return httpClient.get<{
      sucesso: boolean
      mensagem?: string
      erros?: string[]
      retorno: PerfilCorporativoResponse & {
        codigoInternoColaboradorCriacao?: string
        codigoInternoColaboradorAlteracao?: string | null
        dataCriacao?: string
        dataAlteracao?: string
      }
    }>(`/api/Organograma/PerfilCorporativoListaPorId?${queryParams.toString()}`, { token })
  }

  /**
   * Listar Colaboradores Externos por Cliente (paginação e filtro por nome).
   * Contrato: GET /api/Organograma/ListarColaboradoresExternosPorCliente?codigoCliente=&limit=&cursor=&nome=
   */
  async listarColaboradoresExternosPorCliente(
    token: string,
    codigoCliente: string,
    cursor: number,
    limit: number,
    nome?: string,
  ): Promise<Array<{ CodigoInternoColaborador: string; NomeColaborador: string }>> {
    interface ListarColaboradoresExternosPorClienteResponse {
      retorno: Array<{ codigoInternoColaborador: string; nomeColaborador: string }>
      sucesso: boolean
      mensagem: string | null
      erros: string[] | null
    }

    const queryParams = new URLSearchParams({
      codigoCliente,
      limit: limit.toString(),
      cursor: cursor.toString(),
    })
    if (nome?.trim()) {
      queryParams.set('nome', nome.trim())
    }

    const response = await httpClient.get<ListarColaboradoresExternosPorClienteResponse>(
      `/api/Organograma/ListarColaboradoresExternosPorCliente?${queryParams.toString()}`,
      { token },
    )

    if (!response || !response.sucesso || !Array.isArray(response.retorno)) {
      return []
    }

    return response.retorno.map((colab) => ({
      CodigoInternoColaborador: colab.codigoInternoColaborador,
      NomeColaborador: colab.nomeColaborador,
    }))
  }

  // Inserir Gestor Externo
  async inserirGestorExterno(
    token: string,
    payload: {
      codGestorExterno?: string
      nome: string
      email: string
      telefone: string
      codigoCliente?: string
      perfilLinkedin?: string
      areasDeAtuacao?: Array<{
        areaDeAtuacao: { descricao: string }
        permanencia: { id: string }
      }>
      preferenciasPessoais?: string
    },
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return httpClient.post<{ sucesso: boolean; mensagem?: string; erros?: string[] }>(
      '/api/GestaoDeAlocados/GestorExterno/InserirGestorExterno',
      {
        codGestorExterno: payload.codGestorExterno || '',
        nome: payload.nome,
        email: payload.email,
        telefone: payload.telefone,
        codigoCliente: payload.codigoCliente || '',
        perfilLinkedin: payload.perfilLinkedin || '',
        areasDeAtuacao: payload.areasDeAtuacao || [],
        preferenciasPessoais: payload.preferenciasPessoais || '',
      },
      { token },
    )
  }
}
