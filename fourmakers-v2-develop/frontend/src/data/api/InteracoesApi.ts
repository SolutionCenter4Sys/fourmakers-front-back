import { httpClient } from './httpClient'

import type {
  InteracaoResponse,
  ArquivoEncontroDto,
  InserirArquivoEncontroParams,
  CriarEncontroAiPayload,
  AtualizarEncontroAiPayload,
  EncontroAiResponse,
  BuscarProximosPassosMoxeRequest,
  BuscarProximosPassosMoxeResponse,
  InserirInteracaoIAJornadaPayload,
  AtualizarInteracaoIAJornadaPayload,
  InteracaoIAJornadaResponse,
  CategoriaAssuntoComSubModel,
  InteracaoCategoriaSubResponse,
  InserirInteracaoCategoriaPayload,
  AtualizarInteracaoCategoriaPayload,
} from '@domain/entities/AgendaGestor'

interface InteracoesApiResponse {
  retorno?: InteracaoResponse[]
  sucesso?: boolean
  mensagem?: string | null
  erros?: string[] | null
}

export class InteracoesApi {
  async buscarEncontroPorColaboradorId(
    token: string,
    codInternoColaborador: string,
  ): Promise<InteracaoResponse[]> {
    const response = await httpClient.get<InteracoesApiResponse | InteracaoResponse[]>(
      `/api/GestaoDeAlocados/Encontros/buscarEncontroPorColaboradorId/${codInternoColaborador}`,
      { token },
    )

    // Se a resposta for um objeto com 'retorno', extrair o array
    if (response && typeof response === 'object' && 'retorno' in response) {
      const apiResponse = response as InteracoesApiResponse
      return Array.isArray(apiResponse.retorno) ? apiResponse.retorno : []
    }

    // Se a resposta já for um array, retornar diretamente
    return Array.isArray(response) ? response : []
  }

  async buscarEncontroPorId(token: string, encontroId: number): Promise<InteracaoResponse> {
    return httpClient.get<InteracaoResponse>(
      `/api/GestaoDeAlocados/Encontros/buscarEncontroPorId/${encontroId}`,
      { token },
    )
  }

  async buscarEncontroPorIdComComentarios(
    token: string,
    encontroId: number,
  ): Promise<InteracaoResponse> {
    return httpClient.get<InteracaoResponse>(
      `/api/GestaoDeAlocados/Encontros/BuscarEncontroPorIdComComentarios/${encontroId}`,
      { token },
    )
  }

  /**
   * Chama POST /CriarEncontro. O id retornado é o encontroId obrigatório para InserirArquivo.
   */
  async criarEncontro(
    token: string,
    payload: import('@domain/entities/AgendaGestor').InserirInteracaoIaPayload,
  ): Promise<InteracaoResponse> {
    type EnvelopeCriar = {
      retorno?: InteracaoResponse
      Retorno?: InteracaoResponse
      sucesso?: boolean
      mensagem?: unknown
      erros?: unknown
    }
    const response = await httpClient.post<EnvelopeCriar | InteracaoResponse>(
      '/api/GestaoDeAlocados/Encontros/CriarEncontro',
      payload,
      { token },
    )
    if (response && typeof response === 'object') {
      const envelope = response as EnvelopeCriar
      const inner = envelope.retorno ?? envelope.Retorno
      if (inner && typeof inner === 'object' && typeof (inner as InteracaoResponse).id === 'number') {
        return inner as InteracaoResponse
      }
      if (typeof (response as InteracaoResponse).id === 'number') {
        return response as InteracaoResponse
      }
    }
    return (response as InteracaoResponse) ?? ({} as InteracaoResponse)
  }

  async inserirInteracaoIa(
    token: string,
    payload: import('@domain/entities/AgendaGestor').InserirInteracaoIaPayload,
  ): Promise<InteracaoResponse> {
    return this.criarEncontro(token, payload)
  }

  /**
   * Atualiza um encontro (interação da agenda). Endpoint: GestaoDeAlocados/Encontros/atualizarEncontro.
   */
  async atualizarInteracaoIa(
    token: string,
    payload: import('@domain/entities/AgendaGestor').AtualizarInteracaoIaPayload,
  ): Promise<InteracaoResponse> {
    const { id, ...restPayload } = payload
    const response = await httpClient.post<{ retorno?: InteracaoResponse } | InteracaoResponse>(
      `/api/GestaoDeAlocados/Encontros/atualizarEncontro/${id}`,
      restPayload,
      { token },
    )
    if (response && typeof response === 'object' && 'retorno' in response && response.retorno) {
      return response.retorno
    }
    return response as InteracaoResponse
  }

  /**
   * Deleta um encontro (interação da agenda). Endpoint correto: GestaoDeAlocados/Encontros.
   * Flutter: DELETE .../deletarEncontro/{encontroId}
   */
  async deletarEncontro(token: string, encontroId: number): Promise<boolean> {
    const response = await httpClient.delete<{ sucesso?: boolean; retorno?: unknown }>(
      `/api/GestaoDeAlocados/Encontros/deletarEncontro/${encontroId}`,
      { token },
    )
    return response?.sucesso ?? false
  }

  /** @deprecated Use deletarEncontro para encontros da agenda (GestaoDeAlocados). InteracaoIA é outro contexto (Social/JornadaComercial). */
  async deletarInteracaoIa(token: string, interacaoId: number): Promise<boolean> {
    const response = await httpClient.delete<{ sucesso: boolean }>(
      `/api/GestaoDeAlocados/Encontros/deletarEncontro/${interacaoId}`,
      { token },
    )
    return response?.sucesso ?? false
  }

  /** Texto padrão para campos vazios no request Moxe (legado Flutter). */
  private static readonly MOXE_NAO_FORNECIDO = 'Informação não fornecida'

  /**
   * Gera resumo e próximos passos via IA (Moxe). Legado: BuscarProximosPassosIntegracaoMoxe.
   * POST api/Social/JornadaComercialApp/BuscarProximosPassosIntegracaoMoxe
   * Request: agenda, interacao, cliente, equipeFourtalentsParticipante (IntegracaoMoxeRequest).
   * Resposta: { retorno: { sucesso, mensagem, dados: { resumo, passos } }, sucesso, mensagem, erros }
   */
  async buscarProximosPassosIntegracaoMoxe(
    token: string,
    payload: BuscarProximosPassosMoxeRequest,
  ): Promise<BuscarProximosPassosMoxeResponse> {
    type RetornoComDados = { sucesso?: boolean; mensagem?: string; dados?: { resumo?: string; passos?: string[] } }
    type Envelope = { retorno?: RetornoComDados; Retorno?: RetornoComDados; sucesso?: boolean; mensagem?: string; erros?: unknown }
    const nao = InteracoesApi.MOXE_NAO_FORNECIDO
    const ata = (payload.interacao?.ata ?? payload.ata ?? '').trim() || nao
    const principaisPontosAudio = (payload.interacao?.principaisPontosAudio ?? payload.principaisPontosAudio ?? '').trim().slice(0, 1500)

    const body: Record<string, unknown> = {
      interacaoId: payload.interacaoId ?? null,
      interacao: {
        ata,
        principaisPontosAudio: principaisPontosAudio || nao,
      },
    }

    if (payload.agenda && Object.keys(payload.agenda).length > 0) {
      const a = payload.agenda
      body.agenda = {
        titulo: (a.titulo ?? '').trim() || nao,
        tipoInteracao: (a.tipoInteracao ?? '').trim() || nao,
        data: (a.data ?? '').trim() || nao,
        horario: (a.horario ?? '').trim() || nao,
        local: (a.local ?? '').trim() || nao,
        descricao: (a.descricao ?? '').trim() || nao,
        vagas: a.vagas ?? 0,
      }
    }
    if (payload.cliente && Object.keys(payload.cliente).length > 0) {
      const c = payload.cliente
      body.cliente = {
        empresa: (c.empresa ?? '').trim() || nao,
        codigo: (c.codigo ?? '').trim() || nao,
        qtdAlocados: c.qtdAlocados ?? 0,
        gestores: Array.isArray(c.gestores)
          ? c.gestores.map((g) => ({ nome: (g.nome ?? '').trim() || nao, email: (g.email ?? '').trim() || nao }))
          : [],
      }
    }
    if (Array.isArray(payload.equipeFourtalentsParticipante) && payload.equipeFourtalentsParticipante.length > 0) {
      body.equipeFourtalentsParticipante = payload.equipeFourtalentsParticipante.map((n) =>
        (String(n ?? '').trim() || nao),
      )
    }

    const response = await httpClient.post<Envelope | BuscarProximosPassosMoxeResponse>(
      '/api/Social/JornadaComercialApp/BuscarProximosPassosIntegracaoMoxe',
      body,
      { token },
    )
    if (response && typeof response === 'object') {
      const envelope = response as Envelope
      const retorno = envelope.retorno ?? envelope.Retorno
      const dados = retorno && typeof retorno === 'object' && 'dados' in retorno ? (retorno as RetornoComDados).dados : null
      if (dados && typeof dados === 'object') {
        const resumoVal = typeof dados.resumo === 'string' ? dados.resumo.trim() : ''
        const passosVal = Array.isArray(dados.passos) ? dados.passos.map((p) => (typeof p === 'string' ? p : String(p ?? ''))) : []
        return { resumo: resumoVal, passos: passosVal }
      }
      const d = (retorno ?? response) as BuscarProximosPassosMoxeResponse
      if (d && typeof d === 'object' && (typeof d.resumo === 'string' || Array.isArray(d.passos))) {
        const resumoVal = typeof d.resumo === 'string' ? d.resumo.trim() : ''
        const passosVal = Array.isArray(d.passos) ? d.passos.map((p) => (typeof p === 'string' ? p : String(p ?? ''))) : []
        return { resumo: resumoVal, passos: passosVal }
      }
    }
    throw new Error((response as Envelope)?.mensagem ?? 'Resposta inválida ao buscar próximos passos (Moxe)')
  }

  /**
   * Cria registro de Interação IA (resumo) na Jornada Comercial. Legado: InserirInteracaoIA.
   * POST api/Social/JornadaComercialApp/InserirInteracaoIA
   */
  async inserirInteracaoIAJornada(
    token: string,
    payload: InserirInteracaoIAJornadaPayload,
  ): Promise<InteracaoIAJornadaResponse> {
    type Envelope = { retorno?: InteracaoIAJornadaResponse; Retorno?: InteracaoIAJornadaResponse; sucesso?: boolean; mensagem?: string }
    const body = { InteracaoId: payload.interacaoId, Resumo: payload.resumo }
    const response = await httpClient.post<Envelope | InteracaoIAJornadaResponse>(
      '/api/Social/JornadaComercialApp/InserirInteracaoIA',
      body,
      { token },
    )
    if (response && typeof response === 'object') {
      const envelope = response as Envelope
      const inner = envelope.retorno ?? envelope.Retorno
      if (inner && typeof inner === 'object' && typeof (inner as InteracaoIAJornadaResponse).id === 'number') {
        return inner as InteracaoIAJornadaResponse
      }
      if (typeof (response as InteracaoIAJornadaResponse).id === 'number') {
        return response as InteracaoIAJornadaResponse
      }
    }
    throw new Error((response as Envelope)?.mensagem ?? 'Resposta inválida ao inserir Interação IA')
  }

  /**
   * Atualiza registro de Interação IA (resumo) na Jornada Comercial. Legado: AtualizarInteracaoIA.
   * PUT api/Social/JornadaComercialApp/AtualizarInteracaoIA/{id}
   */
  async atualizarInteracaoIAJornada(
    token: string,
    payload: AtualizarInteracaoIAJornadaPayload,
  ): Promise<InteracaoIAJornadaResponse> {
    type Envelope = { retorno?: InteracaoIAJornadaResponse; Retorno?: InteracaoIAJornadaResponse; sucesso?: boolean; mensagem?: string }
    const body = { Resumo: payload.resumo }
    const response = await httpClient.put<Envelope | InteracaoIAJornadaResponse>(
      `/api/Social/JornadaComercialApp/AtualizarInteracaoIA/${payload.id}`,
      body,
      { token },
    )
    if (response && typeof response === 'object') {
      const envelope = response as Envelope
      const inner = envelope.retorno ?? envelope.Retorno
      if (inner && typeof inner === 'object' && typeof (inner as InteracaoIAJornadaResponse).id === 'number') {
        return inner as InteracaoIAJornadaResponse
      }
      if (typeof (response as InteracaoIAJornadaResponse).id === 'number') {
        return response as InteracaoIAJornadaResponse
      }
    }
    throw new Error((response as Envelope)?.mensagem ?? 'Resposta inválida ao atualizar Interação IA')
  }

  /** Id de status "Pendente" para passos EncontroAi (backend exige Int32, não aceita null). */
  private static readonly STATUS_ACOES_PENDENTE = 2

  /**
   * Converte payload de EncontroAi para PascalCase (backend .NET pode esperar esse formato).
   * Garante que passos seja sempre um array ao enviar (nunca undefined), para o backend persistir os passos.
   * Backend espera StatusAcoesId como Int32 (não aceita null); default 2 = Pendente.
   */
  private static payloadCriarEncontroAiParaBackend(payload: CriarEncontroAiPayload): Record<string, unknown> {
    const passos = payload.passos ?? []
    return {
      EncontroId: payload.encontroId,
      Resumo: payload.resumo,
      DataGerada: payload.dataGerada ?? null,
      Passos: passos.map((p) => ({
        Texto: p.texto,
        CodigoColaborador: p.codigoColaborador ?? null,
        NomeColaborador: p.nomeColaborador ?? null,
        DataLimite: p.dataLimite ?? null,
        StatusAcoesId: p.statusAcoesId != null ? p.statusAcoesId : InteracoesApi.STATUS_ACOES_PENDENTE,
      })),
    }
  }

  /**
   * Cria EncontroAi (resumo e próximos passos com IA) para um encontro.
   * POST /api/GestaoDeAlocados/Encontros/CriarEncontroAiProximosPassos
   * Envia payload em PascalCase para compatibilidade com o backend.
   */
  async criarEncontroAiProximosPassos(
    token: string,
    payload: CriarEncontroAiPayload,
  ): Promise<EncontroAiResponse> {
    type Envelope = { retorno?: EncontroAiResponse; Retorno?: EncontroAiResponse; sucesso?: boolean; mensagem?: string; erros?: string[] }
    const body = InteracoesApi.payloadCriarEncontroAiParaBackend(payload)
    const response = await httpClient.post<Envelope | EncontroAiResponse>(
      '/api/GestaoDeAlocados/Encontros/CriarEncontroAiProximosPassos',
      body,
      { token },
    )
    if (response && typeof response === 'object') {
      const envelope = response as Envelope
      const inner = envelope.retorno ?? envelope.Retorno
      if (inner && typeof inner === 'object' && typeof (inner as EncontroAiResponse).id === 'number') {
        return inner as EncontroAiResponse
      }
      if (typeof (response as EncontroAiResponse).id === 'number') {
        return response as EncontroAiResponse
      }
    }
    throw new Error((response as Envelope)?.mensagem ?? 'Resposta inválida ao criar EncontroAi')
  }

  /**
   * Converte payload de atualização EncontroAi para PascalCase (backend .NET).
   * Backend espera Id e StatusAcoesId como Int32 (não aceita null). Passo novo usa Id: 0.
   */
  private static payloadAtualizarEncontroAiParaBackend(payload: AtualizarEncontroAiPayload): Record<string, unknown> {
    const passos = payload.passos ?? []
    return {
      Id: payload.id,
      EncontroId: payload.encontroId ?? null,
      Resumo: payload.resumo ?? null,
      DataGerada: payload.dataGerada ?? null,
      Passos: passos.map((p) => ({
        Id: p.id ?? null,
        Texto: p.texto ?? null,
        CodigoColaborador: p.codigoColaborador ?? null,
        NomeColaborador: p.nomeColaborador ?? null,
        DataLimite: p.dataLimite ?? null,
        StatusAcoesId: p.statusAcoesId != null ? p.statusAcoesId : InteracoesApi.STATUS_ACOES_PENDENTE,
      })),
    }
  }

  /**
   * Atualiza EncontroAi (resumo e próximos passos).
   * POST /api/GestaoDeAlocados/Encontros/AtualizarEncontroAiProximosPassos
   * Envia payload em PascalCase para compatibilidade com o backend.
   */
  async atualizarEncontroAiProximosPassos(
    token: string,
    payload: AtualizarEncontroAiPayload,
  ): Promise<EncontroAiResponse> {
    type Envelope = { retorno?: EncontroAiResponse; Retorno?: EncontroAiResponse; sucesso?: boolean; mensagem?: string; erros?: string[] }
    const body = InteracoesApi.payloadAtualizarEncontroAiParaBackend(payload)
    const response = await httpClient.post<Envelope | EncontroAiResponse>(
      '/api/GestaoDeAlocados/Encontros/AtualizarEncontroAiProximosPassos',
      body,
      { token },
    )
    if (response && typeof response === 'object') {
      const envelope = response as Envelope
      const inner = envelope.retorno ?? envelope.Retorno
      if (inner && typeof inner === 'object' && typeof (inner as EncontroAiResponse).id === 'number') {
        return inner as EncontroAiResponse
      }
      if (typeof (response as EncontroAiResponse).id === 'number') {
        return response as EncontroAiResponse
      }
    }
    throw new Error((response as Envelope)?.mensagem ?? 'Resposta inválida ao atualizar EncontroAi')
  }

  /**
   * Normaliza resposta EncontroAi da API (backend pode retornar PascalCase).
   * Garante camelCase para id, encontroId, dataGerada, resumo e passos (id, codigoColaborador, etc.).
   */
  private static normalizarEncontroAiResponse(raw: Record<string, unknown>): EncontroAiResponse {
    const id = (raw.id ?? raw.Id) as number
    const encontroId = (raw.encontroId ?? raw.EncontroId) as number
    const resumo = (raw.resumo ?? raw.Resumo) as string
    const dataGerada = (raw.dataGerada ?? raw.DataGerada) as string | null
    const passosRaw = (raw.passos ?? raw.Passos) as Array<Record<string, unknown>> | null | undefined
    const passos = Array.isArray(passosRaw)
      ? passosRaw.map((p) => ({
          id: (p.id ?? p.Id) as number | undefined,
          encontroAiId: (p.encontroAiId ?? p.EncontroAiId) as number | undefined,
          texto: (p.texto ?? p.Texto) as string,
          codigoColaborador: (p.codigoColaborador ?? p.CodigoColaborador) as string | undefined,
          nomeColaborador: (p.nomeColaborador ?? p.NomeColaborador) as string | undefined,
          dataLimite: (p.dataLimite ?? p.DataLimite) as string | undefined,
          statusAcoesId: (p.statusAcoesId ?? p.StatusAcoesId) as number | undefined,
        }))
      : undefined
    return { id, encontroId, resumo, dataGerada: dataGerada ?? null, passos }
  }

  /**
   * Busca EncontroAi por encontroId.
   * GET /api/GestaoDeAlocados/Encontros/BuscarEncontroAiPorEncontroId/{encontroId}
   * Normaliza resposta para camelCase (backend pode retornar PascalCase).
   */
  async buscarEncontroAiPorEncontroId(
    token: string,
    encontroId: number,
  ): Promise<EncontroAiResponse | null> {
    type Envelope = { retorno?: unknown; Retorno?: unknown; sucesso?: boolean }
    const response = await httpClient.get<Envelope | Record<string, unknown> | null>(
      `/api/GestaoDeAlocados/Encontros/BuscarEncontroAiPorEncontroId/${encontroId}`,
      { token },
    )
    if (response == null) return null
    let inner: Record<string, unknown> | null = null
    if (typeof response === 'object' && ('retorno' in response || 'Retorno' in response)) {
      const envelope = response as Envelope
      const ret = envelope.retorno ?? envelope.Retorno
      if (ret && typeof ret === 'object' && !Array.isArray(ret)) inner = ret as Record<string, unknown>
    } else if (typeof response === 'object' && !Array.isArray(response)) {
      inner = response as Record<string, unknown>
    }
    if (!inner) return null
    const idVal = inner.id ?? inner.Id
    if (idVal == null || typeof idVal !== 'number') return null
    return InteracoesApi.normalizarEncontroAiResponse(inner)
  }

  /**
   * Upload de arquivo para um encontro (interação).
   * POST .../Encontros/InserirArquivo (multipart/form-data).
   * O encontroId enviado aqui deve ser o id retornado na resposta de CriarEncontro.
   */
  async inserirArquivoEncontro(
    token: string,
    params: InserirArquivoEncontroParams,
  ): Promise<ArquivoEncontroDto> {
    const formData = new FormData()
    // encontroId = id do response de CriarEncontro; obrigatório para a API
    const arquivoParam = {
      encontroId: params.encontroId,
      bytes: '',
      nomeArquivo: params.nomeArquivo,
      tipoArquivo: params.tipoArquivo,
      ...(params.transcricao != null && params.transcricao !== ''
        ? { transcricao: params.transcricao }
        : {}),
    }
    formData.append('arquivoParam', JSON.stringify(arquivoParam))
    formData.append('files', params.file, params.nomeArquivo)

    interface Envelope {
      retorno?: ArquivoEncontroDto
      sucesso?: boolean
      mensagem?: string | null
      erros?: string[] | null
    }

    const response = await httpClient.post<Envelope>(
      '/api/GestaoDeAlocados/Encontros/InserirArquivo',
      formData,
      { token },
    )

    if (response?.retorno) {
      return response.retorno
    }
    throw new Error(
      response?.mensagem ?? 'Resposta inválida ao inserir arquivo no encontro.',
    )
  }

  /**
   * Remove um arquivo de um encontro.
   * DELETE /api/GestaoDeAlocados/Encontros/deletarArquivo/{arquivoId}
   */
  async deletarArquivoEncontro(
    token: string,
    arquivoId: number,
  ): Promise<void> {
    await httpClient.delete<{ sucesso?: boolean }>(
      `/api/GestaoDeAlocados/Encontros/deletarArquivo/${arquivoId}`,
      { token },
    )
  }

  /**
   * Lista categorias de assunto com subcategorias (catálogo). Jornada Comercial.
   * GET api/Social/JornadaComercialApp/ListarCategoriasAssuntoComSub
   */
  async listarCategoriasAssuntoComSub(
    token: string,
  ): Promise<CategoriaAssuntoComSubModel[]> {
    type Envelope = { retorno?: CategoriaAssuntoComSubModel[]; Retorno?: CategoriaAssuntoComSubModel[] }
    const response = await httpClient.get<Envelope | CategoriaAssuntoComSubModel[]>(
      '/api/Social/JornadaComercialApp/ListarCategoriasAssuntoComSub',
      { token },
    )
    if (Array.isArray(response)) return response
    const inner = (response as Envelope)?.retorno ?? (response as Envelope)?.Retorno
    return Array.isArray(inner) ? inner : []
  }

  /**
   * Lista categorias/subcategorias associadas a uma interação.
   * GET api/GestaoDeAlocados/Encontros/ListarCategoriasSubPorInteracaoId/{interacaoId}
   * Normaliza PascalCase do backend para camelCase (InteracaoCategoriaId → interacaoCategoriaId).
   */
  async listarCategoriasSubPorInteracaoId(
    token: string,
    interacaoId: number,
  ): Promise<InteracaoCategoriaSubResponse[]> {
    type Raw = Record<string, unknown>
    type Envelope = { retorno?: Raw[]; Retorno?: Raw[] }
    const response = await httpClient.get<Envelope | Raw[]>(
      `/api/GestaoDeAlocados/Encontros/ListarCategoriasSubPorInteracaoId/${interacaoId}`,
      { token },
    )
    const rawList = Array.isArray(response)
      ? response
      : Array.isArray((response as Envelope)?.retorno)
        ? (response as Envelope).retorno!
        : Array.isArray((response as Envelope)?.Retorno)
          ? (response as Envelope).Retorno!
          : []
    return rawList.map((c: Raw): InteracaoCategoriaSubResponse => ({
      interacaoId: (c.interacaoId ?? c.InteracaoId) as number | undefined,
      interacaoCategoriaId: (c.interacaoCategoriaId ?? c.InteracaoCategoriaId) as number | undefined,
      categoriaAssuntoId: (c.categoriaAssuntoId ?? c.CategoriaAssuntoId) as number | undefined,
      subCategoriaAssuntoId: (c.subCategoriaAssuntoId ?? c.SubCategoriaAssuntoId) as number | undefined,
      dataCriacao: (c.dataCriacao ?? c.DataCriacao) as string | undefined,
      ativo: (c.ativo ?? c.Ativo) as boolean | undefined,
    }))
  }

  /**
   * Insere categoria/subcategoria na interação. Jornada Comercial.
   * POST api/Social/JornadaComercialApp/InsercaoInteracaoCategoria
   */
  async inserirInteracaoCategoria(
    token: string,
    payload: InserirInteracaoCategoriaPayload,
  ): Promise<{ id?: number }> {
    type Envelope = { retorno?: { id?: number }; Retorno?: { id?: number }; sucesso?: boolean; mensagem?: string }
    const response = await httpClient.post<Envelope | { id?: number }>(
      '/api/Social/JornadaComercialApp/InsercaoInteracaoCategoria',
      payload,
      { token },
    )
    if (response && typeof response === 'object' && 'id' in response) {
      return response as { id?: number }
    }
    const inner = (response as Envelope)?.retorno ?? (response as Envelope)?.Retorno
    return (inner && typeof inner === 'object') ? inner : {}
  }

  /**
   * Atualiza categoria/subcategoria da interação. Jornada Comercial.
   * POST api/Social/JornadaComercialApp/AtualizarInteracaoCategoria
   */
  async atualizarInteracaoCategoria(
    token: string,
    payload: AtualizarInteracaoCategoriaPayload,
  ): Promise<void> {
    await httpClient.post<{ sucesso?: boolean }>(
      '/api/Social/JornadaComercialApp/AtualizarInteracaoCategoria',
      payload,
      { token },
    )
  }
}
