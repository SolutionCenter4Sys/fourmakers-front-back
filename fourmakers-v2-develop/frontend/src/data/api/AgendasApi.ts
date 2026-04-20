import { httpClient } from './httpClient'

import type { AgendaResponse, ParticipacaoUsuarioLogado, InteracaoResponse, ArquivoEncontroDto } from '@domain/entities/AgendaGestor'

interface AgendasApiResponse {
  retorno: AgendaResponse[]
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

/**
 * Normaliza objeto de agenda vindo da API (backend pode retornar PascalCase).
 * Garante camelCase para o mapeamento no repositório.
 */
function normalizarAgendaResponse(raw: Record<string, unknown>): AgendaResponse {
  const get = (camel: string, pascal: string): unknown =>
    raw[camel] ?? raw[pascal]
  const num = (c: string, p: string): number =>
    typeof get(c, p) === 'number' ? (get(c, p) as number) : 0
  const str = (c: string, p: string): string =>
    typeof get(c, p) === 'string' ? (get(c, p) as string) : ''
  const optStr = (c: string, p: string): string | undefined => {
    const v = get(c, p)
    return v === null || v === undefined ? undefined : String(v)
  }
  const optNum = (c: string, p: string): number | undefined => {
    const v = get(c, p)
    return typeof v === 'number' ? v : undefined
  }
  /** Normaliza statusSolicitacaoParticipante para number (0|1|2|3). API envia INT; JSON pode vir como number ou string. */
  const normalizarStatusSolicitacao = (raw: unknown): 0 | 1 | 2 | 3 | undefined => {
    if (typeof raw === 'number' && (raw === 0 || raw === 1 || raw === 2 || raw === 3)) return raw
    if (typeof raw === 'string') {
      const n = parseInt(raw, 10)
      if (Number.isInteger(n) && n >= 0 && n <= 3) return n as 0 | 1 | 2 | 3
      const s = raw.toLowerCase()
      if (s === 'pendente') return 0
      if (s === 'aceito') return 1
      if (s === 'recusado') return 2
      if (s === 'talvez') return 3
    }
    return undefined
  }
  /** Normaliza arquivos do encontro (API pode retornar PascalCase: Arquivos, LinkImagemInteracao, etc.). */
  const normalizarArquivos = (rawList: unknown): ArquivoEncontroDto[] => {
    if (!Array.isArray(rawList)) return []
    return rawList
      .filter((item): item is Record<string, unknown> => item != null && typeof item === 'object')
      .map((obj) => {
        const g = (c: string, p: string): unknown => (obj as Record<string, unknown>)[c] ?? (obj as Record<string, unknown>)[p]
        const num = (c: string, p: string): number => (typeof g(c, p) === 'number' ? (g(c, p) as number) : 0)
        const str = (c: string, p: string): string => String(g(c, p) ?? '')
        const optStr = (c: string, p: string): string | undefined => {
          const v = g(c, p)
          if (v === null || v === undefined) return undefined
          return typeof v === 'string' ? v : String(v)
        }
        return {
          id: num('id', 'Id'),
          dataArquivo: str('dataArquivo', 'DataArquivo'),
          linkAudioInteracao: optStr('linkAudioInteracao', 'LinkAudioInteracao') ?? undefined,
          linkImagemInteracao: optStr('linkImagemInteracao', 'LinkImagemInteracao') ?? undefined,
          transcricao: optStr('transcricao', 'Transcricao') ?? undefined,
        }
      })
  }
  /** Normaliza encontros (API pode retornar Encontros/Arquivos em PascalCase). */
  const normalizarEncontros = (rawList: unknown): InteracaoResponse[] | undefined => {
    if (!Array.isArray(rawList)) return undefined
    return rawList
      .filter((item): item is Record<string, unknown> => item != null && typeof item === 'object')
      .map((obj) => {
        const g = (c: string, p: string): unknown => (obj as Record<string, unknown>)[c] ?? (obj as Record<string, unknown>)[p]
        const num = (c: string, p: string): number => (typeof g(c, p) === 'number' ? (g(c, p) as number) : 0)
        const str = (c: string, p: string): string => String(g(c, p) ?? '')
        const optStr = (c: string, p: string): string | undefined => {
          const v = g(c, p)
          return v === null || v === undefined ? undefined : String(v)
        }
        const rawArquivos = g('arquivos', 'Arquivos')
        return {
          id: num('id', 'Id'),
          agendaId: typeof g('agendaId', 'AgendaId') === 'number' ? (g('agendaId', 'AgendaId') as number) : undefined,
          tituloInteracao: str('tituloInteracao', 'TituloInteracao'),
          descricaoInteracao: optStr('descricaoInteracao', 'DescricaoInteracao'),
          codigoInternoColaborador: str('codigoInternoColaborador', 'CodigoInternoColaborador'),
          nomeCompletoColaboradorCriador: str('nomeCompletoColaboradorCriador', 'NomeCompletoColaboradorCriador'),
          dataRequisicao: str('dataRequisicao', 'DataRequisicao'),
          resumoInteracao: optStr('resumoInteracao', 'ResumoInteracao') ?? null,
          encontroAi: (g('encontroAi', 'EncontroAi') as InteracaoResponse['encontroAi']) ?? undefined,
          arquivos: normalizarArquivos(rawArquivos),
        }
      })
  }

  const participacaoRaw = get('participacaoUsuarioLogado', 'ParticipacaoUsuarioLogado') as Record<string, unknown> | null | undefined
  const participacaoUsuarioLogado = participacaoRaw && typeof participacaoRaw === 'object'
    ? {
        agendaId: (() => {
          const v = participacaoRaw.agendaId ?? participacaoRaw.AgendaId
          return typeof v === 'number' ? v : undefined
        })(),
        confirmado: (participacaoRaw.confirmado ?? participacaoRaw.Confirmado) as number | boolean | null | undefined,
        dataConfirmacao: (participacaoRaw.dataConfirmacao ?? participacaoRaw.DataConfirmacao) as string | null | undefined,
        statusSolicitacaoParticipante: normalizarStatusSolicitacao(
          participacaoRaw.statusSolicitacaoParticipante ?? participacaoRaw.StatusSolicitacaoParticipante
        ),
      }
    : undefined
  const clienteRaw = get('cliente', 'Cliente') as Record<string, unknown> | undefined
  const cliente = clienteRaw && typeof clienteRaw === 'object'
    ? {
        id: typeof clienteRaw.id === 'number' ? clienteRaw.id : (clienteRaw.Id as number) ?? 0,
        codigoCliente: String(clienteRaw.codigoCliente ?? clienteRaw.CodigoCliente ?? ''),
        nomeCliente: String(clienteRaw.nomeCliente ?? clienteRaw.NomeCliente ?? ''),
      }
    : undefined
  const idAgenda =
    (typeof get('id', 'Id') === 'number' ? (get('id', 'Id') as number) : undefined) ??
    (typeof get('agendaId', 'AgendaId') === 'number' ? (get('agendaId', 'AgendaId') as number) : 0)
  return {
    id: idAgenda,
    titulo: str('titulo', 'Titulo'),
    descricao: optStr('descricao', 'Descricao'),
    tipoInteracao: num('tipoInteracao', 'TipoInteracao'),
    dataAgendada: str('dataAgendada', 'DataAgendada'),
    dataInicio: optStr('dataInicio', 'DataInicio'),
    dataFim: optStr('dataFim', 'DataFim'),
    status: str('status', 'Status'),
    localizacao: optStr('localizacao', 'Localizacao'),
    linkReuniao: optStr('linkReuniao', 'LinkReuniao'),
    graphEventId: optStr('graphEventId', 'GraphEventId'),
    quantidadeParticipantes: optNum('quantidadeParticipantes', 'QuantidadeParticipantes'),
    codColaboradorCriador: str('codColaboradorCriador', 'CodColaboradorCriador'),
    nomeCompletoColaboradorCriador: str('nomeCompletoColaboradorCriador', 'NomeCompletoColaboradorCriador'),
    cliente,
    colaboradores: (get('colaboradores', 'Colaboradores') as AgendaResponse['colaboradores']) ?? undefined,
    gestoresExternos: (get('gestoresExternos', 'GestoresExternos') as AgendaResponse['gestoresExternos']) ?? undefined,
    participantes: (get('participantes', 'Participantes') as AgendaResponse['participantes']) ?? undefined,
    encontros: normalizarEncontros(get('encontros', 'Encontros')),
    dataCriacao: str('dataCriacao', 'DataCriacao'),
    dataAtualizacao: optStr('dataAtualizacao', 'DataAtualizacao'),
    participacaoUsuarioLogado: participacaoUsuarioLogado as AgendaResponse['participacaoUsuarioLogado'],
    agendaPaiId: optNum('agendaPaiId', 'AgendaPaiId') ?? (get('agendaPaiId', 'AgendaPaiId') as number | null) ?? undefined,
  }
}

export class AgendasApi {
  async buscarTodasAgendas(
    token: string,
    dataInicio: string,
    dataFim: string,
  ): Promise<AgendaResponse[]> {
    const queryParams = new URLSearchParams({
      dataInicio,
      dataFim,
    })

    const response = await httpClient.get<AgendasApiResponse>(
      `/api/GestaoDeAlocados/Encontros/buscarTodasAgendas?${queryParams.toString()}`,
      { token },
    )

    // Extrair o array retorno da resposta e normalizar (backend pode retornar PascalCase)
    // Sem normalização, participacaoUsuarioLogado viria como ParticipacaoUsuarioLogado e os botões Aceitar/Recusar não aparecem
    if (!response || !Array.isArray(response.retorno)) {
      return []
    }
    return response.retorno.map((item) =>
      normalizarAgendaResponse((item ?? {}) as unknown as Record<string, unknown>)
    )
  }

  async carregarAgenda(token: string, agendaId: number): Promise<AgendaResponse> {
    const response = await httpClient.get<{ retorno: AgendaResponse; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }>(
      `/api/GestaoDeAlocados/Encontros/carregarAgenda/${agendaId}`,
      { token },
    )
    if (!response?.retorno) {
      throw new Error(response?.mensagem ?? 'Resposta da API sem dados da agenda')
    }
    return response.retorno
  }

  /**
   * Busca agendas filhas de uma agenda pai (hierarquia pai-filho).
   * GET /api/GestaoDeAlocados/Encontros/BuscarAgendasFilhosCompletas?agendaPaiId={id}
   */
  async buscarAgendasFilhosCompletas(
    token: string,
    agendaPaiId: number,
  ): Promise<AgendaResponse[]> {
    const response = await httpClient.get<AgendasApiResponse | AgendaResponse[]>(
      `/api/GestaoDeAlocados/Encontros/BuscarAgendasFilhosCompletas?agendaPaiId=${agendaPaiId}`,
      { token },
    )
    let list: unknown[] = []
    if (response && typeof response === 'object' && 'retorno' in response) {
      const apiResponse = response as AgendasApiResponse
      list = Array.isArray(apiResponse.retorno) ? apiResponse.retorno : []
    } else if (Array.isArray(response)) {
      list = response
    }
    return list
      .filter((item): item is Record<string, unknown> => item != null && typeof item === 'object')
      .map((item) => normalizarAgendaResponse(item))
  }

  /**
   * Cria agenda (ou agenda filha quando payload.agendaPaiId está definido).
   * Payload em camelCase (titulo, dataAgendada, agendaPaiId, etc.).
   * Resposta é normalizada para camelCase (backend pode retornar PascalCase).
   */
  async criarAgenda(
    token: string,
    payload: import('@domain/entities/AgendaGestor').CriarAgendaPayload,
  ): Promise<AgendaResponse> {
    const response = await httpClient.post<{ retorno?: Record<string, unknown>; sucesso?: boolean } | Record<string, unknown>>(
      '/api/GestaoDeAlocados/Encontros/CriarAgenda',
      payload,
      { token },
    )
    
    // A API retorna { retorno: {...}, sucesso: true }
    const raw = (response as { retorno?: Record<string, unknown> })?.retorno || response
    
    if (raw && typeof raw === 'object') {
      return normalizarAgendaResponse(raw as Record<string, unknown>)
    }
    throw new Error('Resposta inválida ao criar agenda')
  }

  async atualizarAgenda(
    token: string,
    payload: import('@domain/entities/AgendaGestor').AtualizarAgendaPayload,
  ): Promise<AgendaResponse> {
    const { id, ...restPayload } = payload
    return httpClient.post<AgendaResponse>(
      `/api/GestaoDeAlocados/Encontros/atualizarAgenda/${id}`,
      restPayload,
      { token },
    )
  }

  async deletarAgenda(token: string, agendaId: number): Promise<boolean> {
    const response = await httpClient.delete<{ sucesso: boolean }>(
      `/api/GestaoDeAlocados/Encontros/deletarAgenda/${agendaId}`,
      { token },
    )
    return response?.sucesso ?? false
  }

  /**
   * Aceitar ou recusar convite da agenda.
   * Contrato: parâmetros na query string; body vazio {}.
   * decisaoStatus: 1 = aceitar, 2 = recusar
   * codigoColaborador: identificador do usuário (CPF ou UUID conforme auth)
   */
  async aceitarRecusarConviteAgenda(
    token: string,
    payload: {
      decisaoStatus: number // 1 = aceitar, 2 = recusar
      agendaId: string
      codigoColaborador: string
    },
  ): Promise<boolean> {
    const queryParams = new URLSearchParams({
      decisaoStatus: payload.decisaoStatus.toString(),
      agendaId: payload.agendaId,
      codigoColaborador: payload.codigoColaborador,
    })

    const response = await httpClient.post<{ sucesso: boolean; retorno?: boolean }>(
      `/api/GestaoDeAlocados/Encontros/AceitarRecusarConviteAgenda?${queryParams.toString()}`,
      {},
      { token },
    )

    return response?.sucesso ?? response?.retorno ?? false
  }

  async buscarParticipacaoUsuario(
    token: string,
    agendaId: number,
  ): Promise<ParticipacaoUsuarioLogado | null> {
    const response = await httpClient.get<ParticipacaoUsuarioLogado | { retorno?: ParticipacaoUsuarioLogado }>(
      `/api/GestaoDeAlocados/Encontros/BuscarParticipacaoUsuario/${agendaId}`,
      { token },
    )
    if (response && typeof response === 'object' && 'retorno' in response) {
      return (response as { retorno?: ParticipacaoUsuarioLogado }).retorno ?? null
    }
    return (response as ParticipacaoUsuarioLogado) ?? null
  }
}
