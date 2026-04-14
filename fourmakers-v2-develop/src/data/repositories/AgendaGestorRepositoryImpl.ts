import { inject, injectable } from 'tsyringe'

import { DiTokens } from '@core/di/tokens'
import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import type {
  AgendaResponse,
  InteracaoResponse,
  KanbanResponse,
  AgendaGestorCompleto,
  ItemAgendaGestor,
  CriarAgendaPayload,
  AtualizarAgendaPayload,
  InserirInteracaoIaPayload,
  AtualizarInteracaoIaPayload,
  AtualizarStatusAcoesPayload,
  InserirComentarioAcaoPayload,
  CriarEncontroAiPayload,
  AtualizarEncontroAiPayload,
  EncontroAiResponse,
  AcaoResponse,
  ArquivoEncontroDto,
  InserirArquivoEncontroParams,
  BuscarProximosPassosMoxeRequest,
  BuscarProximosPassosMoxeResponse,
  InteracaoCategoriaSubResponse,
  CategoriaAssuntoComSubModel,
  InserirInteracaoCategoriaPayload,
  AtualizarInteracaoCategoriaPayload,
} from '@domain/entities/AgendaGestor'
import { obterDatasPadraoAgenda } from '@shared/constants/agendasComerciais'
import { formatarDataLocalParaFiltro } from '@shared/utils/timezoneAgendaUtils'
import type { AgendasApi } from '@data/api/AgendasApi'
import type { InteracoesApi } from '@data/api/InteracoesApi'
import type { AcoesApi } from '@data/api/AcoesApi'

@injectable()
export class AgendaGestorRepositoryImpl implements AgendaGestorRepository {
  constructor(
    @inject(DiTokens.agendasApi) private readonly agendasApi: AgendasApi,
    @inject(DiTokens.interacoesApi) private readonly interacoesApi: InteracoesApi,
    @inject(DiTokens.acoesApi) private readonly acoesApi: AcoesApi,
  ) {}

  async carregarAgendaDetalhe(token: string, agendaId: number): Promise<ItemAgendaGestor> {
    const agenda = await this.agendasApi.carregarAgenda(token, agendaId)
    const rawAgenda = agenda as AgendaResponse & { Encontros?: InteracaoResponse[] }
    const encontrosList = agenda.encontros ?? rawAgenda.Encontros ?? []
    const ids = encontrosList
      .map((e) => e.id ?? (e as { Id?: number }).Id)
      .filter((id): id is number => typeof id === 'number')
    const resultadosAi = await Promise.allSettled(
      ids.map((id) => this.interacoesApi.buscarEncontroAiPorEncontroId(token, id)),
    )
    const encontroAiPorId = new Map<number, EncontroAiResponse | null>()
    resultadosAi.forEach((result, index) => {
      const id = ids[index]
      if (id == null) return
      const encontroAi = result.status === 'fulfilled' ? result.value : null
      encontroAiPorId.set(id, encontroAi ?? null)
    })
    for (const encontro of encontrosList) {
      const id = encontro.id ?? (encontro as { Id?: number }).Id
      if (typeof id !== 'number') continue
      const encontroAi = encontroAiPorId.get(id)
      if (encontroAi) (encontro as { encontroAi?: EncontroAiResponse }).encontroAi = encontroAi
    }
    if (!agenda.encontros && encontrosList.length > 0) {
      (agenda as { encontros: InteracaoResponse[] }).encontros = encontrosList
    }
    return this.mapearAgendaResponseParaItem(agenda)
  }

  private mapearAgendaResponseParaItem(agenda: AgendaResponse): ItemAgendaGestor {
    const mapearTipoInteracao = (tipo: number): string => {
      const tipos: Record<number, string> = {
        1: 'Reunião',
        2: 'Ligação',
        3: 'Chat',
        4: 'Email',
        5: 'Presencial',
      }
      return tipos[tipo] || 'Outro'
    }
    const p = agenda.participacaoUsuarioLogado
    let statusSolicitacao: 0 | 1 | 2 | 3 | null = null
    if (p) {
      const raw = p.statusSolicitacaoParticipante
      if (typeof raw === 'number' && (raw === 0 || raw === 1 || raw === 2 || raw === 3)) {
        statusSolicitacao = raw
      } else if (typeof raw === 'string') {
        const s = raw.toLowerCase()
        if (s === 'pendente') statusSolicitacao = 0
        else if (s === 'aceito') statusSolicitacao = 1
        else if (s === 'recusado') statusSolicitacao = 2
        else if (s === 'talvez') statusSolicitacao = 3
      }
      if (statusSolicitacao === null && (p as { confirmado?: number | boolean }).confirmado !== undefined) {
        const c = (p as { confirmado?: number | boolean }).confirmado
        if (typeof c === 'number' && (c === 0 || c === 1 || c === 2)) statusSolicitacao = c
        else if (typeof c === 'boolean') statusSolicitacao = c ? 1 : 0
      }
    }
    return {
      tipo: 'agenda',
      id: agenda.id.toString(),
      agendaId: agenda.id,
      titulo: agenda.titulo,
      responsavel: agenda.nomeCompletoColaboradorCriador,
      codColaboradorCriador: agenda.codColaboradorCriador,
      data: agenda.dataAgendada,
      status: agenda.status?.trim() || 'Pendente',
      descricao: agenda.descricao,
      participantes: agenda.quantidadeParticipantes,
      tipoInteracao: mapearTipoInteracao(agenda.tipoInteracao),
      localizacao: agenda.localizacao,
      linkReuniao: agenda.linkReuniao,
      graphEventId: agenda.graphEventId,
      dataInicio: agenda.dataInicio,
      dataFim: agenda.dataFim,
      dataCriacao: agenda.dataCriacao,
      dataAtualizacao: agenda.dataAtualizacao,
      cliente: agenda.cliente
        ? { codigoCliente: agenda.cliente.codigoCliente, nomeCliente: agenda.cliente.nomeCliente }
        : undefined,
      colaboradores: agenda.colaboradores,
      gestoresExternos: agenda.gestoresExternos?.map((g) => ({
        codGestorExterno: g.codGestorExterno || '',
        codigoInternoColaborador: g.codigoInternoColaborador || undefined,
        nome: g.nome,
        email: g.email,
      })),
      participantesDetalhes: agenda.participantes
        ?.filter((p): p is NonNullable<typeof p> => p != null)
        ?.map((p) => ({
          codigoGestorExterno: p.codigoGestorExterno || undefined,
          codigoColaborador: p.codigoColaborador || undefined,
          Nome: p.Nome || undefined,
          confirmado: typeof p.confirmado === 'boolean' ? p.confirmado : (p.confirmado === 1 || p.confirmado === true),
          dataConfirmacao: p.dataConfirmacao || undefined,
        })),
      encontros: agenda.encontros?.map((encontro) => ({
        id: encontro.id,
        tituloInteracao: encontro.tituloInteracao,
        descricaoInteracao: encontro.descricaoInteracao || undefined,
        resumoInteracao: encontro.resumoInteracao || undefined,
        dataRequisicao: encontro.dataRequisicao,
        nomeCompletoColaboradorCriador: encontro.nomeCompletoColaboradorCriador,
        encontroAi: encontro.encontroAi
          ? {
              resumo: encontro.encontroAi.resumo || undefined,
              passos: encontro.encontroAi.passos?.map((passo) => ({
                texto: passo.texto,
                nomeColaborador: passo.nomeColaborador,
                dataLimite: passo.dataLimite || undefined,
              })),
            }
          : undefined,
        arquivos: encontro.arquivos,
      })),
      participacaoUsuarioLogado:
        p != null
          ? {
              agendaId: (p as { agendaId?: number }).agendaId ?? agenda.id,
              confirmado:
                typeof p.confirmado === 'boolean'
                  ? (p.confirmado ? 1 : 0)
                  : (p.confirmado ?? undefined),
              dataConfirmacao: p.dataConfirmacao || undefined,
              statusSolicitacaoParticipante: statusSolicitacao,
            }
          : undefined,
      agendaPaiId: agenda.agendaPaiId ?? undefined,
    }
  }

  async buscarAgendasFilhos(token: string, agendaPaiId: number): Promise<ItemAgendaGestor[]> {
    const agendas = await this.agendasApi.buscarAgendasFilhosCompletas(token, agendaPaiId)
    return agendas.map((agenda) => this.mapearAgendaResponseParaItem(agenda))
  }

  async buscarAgendas(
    token: string,
    dataInicio: string,
    dataFim: string,
  ): Promise<AgendaResponse[]> {
    return this.agendasApi.buscarTodasAgendas(token, dataInicio, dataFim)
  }

  async buscarInteracoesPorColaborador(
    token: string,
    codInternoColaborador: string,
  ): Promise<InteracaoResponse[]> {
    return this.interacoesApi.buscarEncontroPorColaboradorId(token, codInternoColaborador)
  }

  async buscarKanbanAcoes(token: string): Promise<KanbanResponse> {
    return this.acoesApi.buscarKanbanEncontrosAcoesComerciais(token)
  }

  async buscarAgendaCompleta(
    token: string,
    codInternoColaborador: string,
    dataInicio?: string,
    dataFim?: string,
  ): Promise<AgendaGestorCompleto> {
    const { dataInicio: padraoInicio, dataFim: padraoFim } = obterDatasPadraoAgenda()
    const inicio = dataInicio ?? formatarDataLocalParaFiltro(padraoInicio)
    const fim = dataFim ?? formatarDataLocalParaFiltro(padraoFim)

    // Buscar dados em paralelo
    const [agendasRaw, interacoesRaw, kanbanRaw] = await Promise.all([
      this.buscarAgendas(token, inicio, fim),
      this.buscarInteracoesPorColaborador(token, codInternoColaborador),
      this.buscarKanbanAcoes(token),
    ])

    // Garantir que agendasRaw seja um array antes de filtrar
    const agendasArray = Array.isArray(agendasRaw) ? agendasRaw : []

    // Filtrar agendas por colaborador
    // O codInternoColaborador pode ser CPF ou UUID, então verificamos ambos
    // Por enquanto, retornar todas as agendas para debug
    // TODO: Ajustar filtro quando soubermos o formato exato do codInternoColaborador
    const agendasFiltradas = agendasArray.filter(() => {
      // Temporariamente retornando todas para debug
      // Quando corrigir, usar:
      // - agenda.colaboradores?.some((colab) => colab.codInternoColaborador === codInternoColaborador)
      // - agenda.codColaboradorCriador === codInternoColaborador
      return true
    })

    // Mapear tipo de interação
    const mapearTipoInteracao = (tipo: number): string => {
      const tipos: Record<number, string> = {
        1: 'Reunião',
        2: 'Ligação',
        3: 'Chat',
        4: 'Email',
        5: 'Presencial',
      }
      return tipos[tipo] || 'Outro'
    }

    // Mapear agendas para ItemAgendaGestor
    const agendas: ItemAgendaGestor[] = agendasFiltradas.map((agenda) => ({
      tipo: 'agenda',
      id: agenda.id.toString(),
      agendaId: agenda.id,
      titulo: agenda.titulo,
      responsavel: agenda.nomeCompletoColaboradorCriador,
      codColaboradorCriador: agenda.codColaboradorCriador,
      data: agenda.dataAgendada,
      status: agenda.status?.trim() || 'Pendente', // Garantir que status nunca seja string vazia
      descricao: agenda.descricao,
      participantes: agenda.quantidadeParticipantes,
      tipoInteracao: mapearTipoInteracao(agenda.tipoInteracao),
      localizacao: agenda.localizacao,
      linkReuniao: agenda.linkReuniao,
      graphEventId: agenda.graphEventId,
      dataInicio: agenda.dataInicio,
      dataFim: agenda.dataFim,
      dataCriacao: agenda.dataCriacao,
      dataAtualizacao: agenda.dataAtualizacao,
      cliente: agenda.cliente
        ? {
            codigoCliente: agenda.cliente.codigoCliente,
            nomeCliente: agenda.cliente.nomeCliente,
          }
        : undefined,
      colaboradores: agenda.colaboradores,
      gestoresExternos: agenda.gestoresExternos?.map((gestor) => ({
        codGestorExterno: gestor.codGestorExterno || '',
        codigoInternoColaborador: gestor.codigoInternoColaborador || undefined,
        nome: gestor.nome,
        email: gestor.email,
      })),
      participantesDetalhes: agenda.participantes
        ?.filter((p): p is NonNullable<typeof p> => p != null)
        ?.map((p) => ({
          codigoGestorExterno: p.codigoGestorExterno || undefined,
          codigoColaborador: p.codigoColaborador || undefined,
          Nome: p.Nome || undefined, // Preservar campo Nome da API
          confirmado: typeof p.confirmado === 'boolean' ? p.confirmado : (p.confirmado === 1 || p.confirmado === true),
          dataConfirmacao: p.dataConfirmacao || undefined,
        })),
      encontros: agenda.encontros?.map((encontro) => ({
        id: encontro.id,
        tituloInteracao: encontro.tituloInteracao,
        descricaoInteracao: encontro.descricaoInteracao || undefined,
        resumoInteracao: encontro.resumoInteracao || undefined,
        dataRequisicao: encontro.dataRequisicao,
        nomeCompletoColaboradorCriador: encontro.nomeCompletoColaboradorCriador,
        encontroAi: encontro.encontroAi
          ? {
              resumo: encontro.encontroAi.resumo || undefined,
              passos: encontro.encontroAi.passos?.map((passo) => ({
                texto: passo.texto,
                nomeColaborador: passo.nomeColaborador,
                dataLimite: passo.dataLimite || undefined,
              })),
            }
          : undefined,
      })),
      // Mapear participação do usuário logado: statusSolicitacaoParticipante como number | null (0=Pendente, 1=Aceito, 2=Recusado, 3=Talvez)
      participacaoUsuarioLogado: (() => {
        const p = agenda.participacaoUsuarioLogado
        if (!p) return undefined
        let status: 0 | 1 | 2 | 3 | null = null
        const raw = p.statusSolicitacaoParticipante
        if (typeof raw === 'number' && (raw === 0 || raw === 1 || raw === 2 || raw === 3)) {
          status = raw
        } else if (typeof raw === 'string') {
          const s = raw.toLowerCase()
          if (s === 'pendente') status = 0
          else if (s === 'aceito') status = 1
          else if (s === 'recusado') status = 2
          else if (s === 'talvez') status = 3
        }
        if (status === null && (p as { confirmado?: number | boolean }).confirmado !== undefined) {
          const c = (p as { confirmado?: number | boolean }).confirmado
          if (typeof c === 'number' && (c === 0 || c === 1 || c === 2)) status = c
          else if (typeof c === 'boolean') status = c ? 1 : 0
        }
        const c = (p as { confirmado?: number | boolean }).confirmado
        const confirmadoNormalizado: number | undefined =
          typeof c === 'boolean' ? (c ? 1 : 0) : (c ?? undefined)
        return {
          agendaId: (p as { agendaId?: number }).agendaId ?? agenda.id,
          confirmado: confirmadoNormalizado,
          dataConfirmacao: p.dataConfirmacao || undefined,
          statusSolicitacaoParticipante: status,
        }
      })(),
    }))

    // Garantir que interacoesRaw seja um array
    const interacoesArray = Array.isArray(interacoesRaw) ? interacoesRaw : []

    // Mapear interações para ItemAgendaGestor (codColaboradorCriador = criador da interação para permissão editar/excluir)
    const interacoes: ItemAgendaGestor[] = interacoesArray.map((interacao) => {
      const rawCodCriador =
        interacao.codigoInternoColaborador ??
        (interacao as { codColaboradorCriador?: string }).codColaboradorCriador
      const codCriador = rawCodCriador != null ? String(rawCodCriador).trim() : ''
      return {
      tipo: 'interacao',
      id: interacao.id.toString(),
      agendaId: interacao.agendaId,
      titulo: interacao.tituloInteracao,
      responsavel: interacao.nomeCompletoColaboradorCriador,
      codColaboradorCriador: codCriador || undefined,
      data: interacao.dataRequisicao,
      status: 'Registrada',
      descricao: interacao.descricaoInteracao,
      resumoInteracao: interacao.resumoInteracao || undefined,
      encontroAi: interacao.encontroAi
        ? {
            resumo: interacao.encontroAi.resumo || undefined,
            passos: interacao.encontroAi.passos?.map((passo) => ({
              texto: passo.texto,
              nomeColaborador: passo.nomeColaborador,
              dataLimite: passo.dataLimite || undefined,
            })),
          }
        : undefined,
    }
    })

    // Extrair e filtrar ações do kanban com validação defensiva
    const todasAcoes = [
      ...(kanbanRaw?.acoesAndamento?.acoes || []),
      ...(kanbanRaw?.acoesPendentes?.acoes || []),
      ...(kanbanRaw?.acoesRealizadas?.acoes || []),
    ].filter((acao): acao is AcaoResponse => acao != null)

    const acoesFiltradas = todasAcoes.filter(
      (acao) => acao.codInternoColaborador && acao.codInternoColaborador === codInternoColaborador,
    )

    // Mapear ações para ItemAgendaGestor
    const acoes: ItemAgendaGestor[] = acoesFiltradas.map((acao) => {
      let status = 'Pendente'
      if (acao.statusAcoes === 1) status = 'Em andamento'
      if (acao.statusAcoes === 3) status = 'Concluída'
      if (acao.atrasado) status = 'Atrasada'

      return {
        tipo: 'acao',
        id: acao.id.toString(),
        agendaId: acao.agendaId,
        titulo: acao.texto,
        responsavel: acao.nomeResponsavel,
        data: acao.dataLimite,
        status,
        descricao: acao.comentarioAcao,
        atrasado: acao.atrasado,
        vencendo: acao.vencendo,
      }
    })

    return {
      codInternoColaborador,
      nomeGestor: '', // Será preenchido pela UI com dados do nó
      agendas,
      interacoes,
      acoes,
      dataUltimaAtualizacao: new Date().toISOString(),
    }
  }

  // CRUD de Agendas
  async criarAgenda(token: string, payload: CriarAgendaPayload): Promise<AgendaResponse> {
    return this.agendasApi.criarAgenda(token, payload)
  }

  async atualizarAgenda(token: string, payload: AtualizarAgendaPayload): Promise<AgendaResponse> {
    return this.agendasApi.atualizarAgenda(token, payload)
  }

  async deletarAgenda(token: string, agendaId: number): Promise<boolean> {
    return this.agendasApi.deletarAgenda(token, agendaId)
  }

  async carregarAgendaPorId(token: string, agendaId: number): Promise<ItemAgendaGestor> {
    const agenda = await this.agendasApi.carregarAgenda(token, agendaId)
    const mapearTipoInteracao = (tipo: number): string => {
      const tipos: Record<number, string> = {
        1: 'Reunião',
        2: 'Ligação',
        3: 'Chat',
        4: 'Email',
        5: 'Presencial',
      }
      return tipos[tipo] || 'Outro'
    }
    const idAgenda = agenda?.id ?? agendaId
    const tipoInteracaoNum = agenda?.tipoInteracao ?? 0
    return {
      tipo: 'agenda',
      id: String(idAgenda),
      agendaId: idAgenda,
      titulo: agenda?.titulo?.trim() ?? '',
      responsavel: agenda?.nomeCompletoColaboradorCriador?.trim() ?? '',
      codColaboradorCriador: agenda?.codColaboradorCriador ?? undefined,
      data: agenda?.dataAgendada ?? '',
      status: (agenda?.status?.trim()) || 'Pendente',
      descricao: agenda?.descricao ?? undefined,
      participantes: agenda?.quantidadeParticipantes ?? undefined,
      tipoInteracao: mapearTipoInteracao(tipoInteracaoNum),
      localizacao: agenda?.localizacao ?? undefined,
      linkReuniao: agenda?.linkReuniao ?? undefined,
      graphEventId: agenda?.graphEventId ?? undefined,
      dataInicio: agenda?.dataInicio ?? undefined,
      dataFim: agenda?.dataFim ?? undefined,
      dataCriacao: agenda?.dataCriacao ?? undefined,
      dataAtualizacao: agenda?.dataAtualizacao ?? undefined,
      cliente: agenda?.cliente
        ? {
            codigoCliente: agenda.cliente.codigoCliente ?? '',
            nomeCliente: agenda.cliente.nomeCliente ?? '',
          }
        : undefined,
      colaboradores: agenda?.colaboradores ?? undefined,
      gestoresExternos: agenda?.gestoresExternos?.map((gestor) => ({
        codGestorExterno: gestor.codGestorExterno || '',
        codigoInternoColaborador: gestor.codigoInternoColaborador || undefined,
        nome: gestor.nome,
        email: gestor.email,
      })),
      participantesDetalhes: agenda.participantes?.map((p) => ({
        codigoGestorExterno: p.codigoGestorExterno || undefined,
        codigoColaborador: p.codigoColaborador || undefined,
        Nome: p.Nome || undefined,
        confirmado: typeof p.confirmado === 'boolean' ? p.confirmado : (p.confirmado === 1 || p.confirmado === true),
        dataConfirmacao: p.dataConfirmacao || undefined,
      })),
      encontros: agenda.encontros?.map((encontro) => ({
        id: encontro.id,
        tituloInteracao: encontro.tituloInteracao,
        descricaoInteracao: encontro.descricaoInteracao || undefined,
        resumoInteracao: encontro.resumoInteracao || undefined,
        dataRequisicao: encontro.dataRequisicao,
        nomeCompletoColaboradorCriador: encontro.nomeCompletoColaboradorCriador,
        encontroAi: encontro.encontroAi
          ? {
              resumo: encontro.encontroAi.resumo || undefined,
              passos: encontro.encontroAi.passos?.map((passo) => ({
                texto: passo.texto,
                nomeColaborador: passo.nomeColaborador,
                dataLimite: passo.dataLimite || undefined,
              })),
            }
          : undefined,
      })),
      participacaoUsuarioLogado: agenda.participacaoUsuarioLogado
        ? {
            statusSolicitacaoParticipante:
              typeof agenda.participacaoUsuarioLogado.statusSolicitacaoParticipante === 'string'
                ? agenda.participacaoUsuarioLogado.statusSolicitacaoParticipante
                : agenda.participacaoUsuarioLogado.statusSolicitacaoParticipante === 0
                  ? 'Pendente'
                  : agenda.participacaoUsuarioLogado.statusSolicitacaoParticipante === 1
                    ? 'Aceito'
                    : agenda.participacaoUsuarioLogado.statusSolicitacaoParticipante === 2
                      ? 'Recusado'
                      : undefined,
            confirmado:
              typeof agenda.participacaoUsuarioLogado.confirmado === 'number'
                ? agenda.participacaoUsuarioLogado.confirmado
                : agenda.participacaoUsuarioLogado.confirmado === true
                  ? 1
                  : agenda.participacaoUsuarioLogado.confirmado === false
                    ? 0
                    : undefined,
            dataConfirmacao: agenda.participacaoUsuarioLogado.dataConfirmacao || undefined,
          }
        : undefined,
    }
  }

  // CRUD de Interações (inserir = chamada CriarEncontro; id do retorno é usado em InserirArquivo)
  async inserirInteracaoIa(token: string, payload: InserirInteracaoIaPayload): Promise<InteracaoResponse> {
    return this.interacoesApi.criarEncontro(token, payload)
  }

  async atualizarInteracaoIa(token: string, payload: AtualizarInteracaoIaPayload): Promise<InteracaoResponse> {
    return this.interacoesApi.atualizarInteracaoIa(token, payload)
  }

  /** Deleta encontro (interação da agenda); usa endpoint GestaoDeAlocados/Encontros/deletarEncontro. */
  async deletarInteracaoIa(token: string, interacaoId: number): Promise<boolean> {
    return this.interacoesApi.deletarEncontro(token, interacaoId)
  }

  async criarEncontroAiProximosPassos(
    token: string,
    payload: CriarEncontroAiPayload,
  ): Promise<EncontroAiResponse> {
    return this.interacoesApi.criarEncontroAiProximosPassos(token, payload)
  }

  async atualizarEncontroAiProximosPassos(
    token: string,
    payload: AtualizarEncontroAiPayload,
  ): Promise<EncontroAiResponse> {
    return this.interacoesApi.atualizarEncontroAiProximosPassos(token, payload)
  }

  async buscarEncontroAiPorEncontroId(
    token: string,
    encontroId: number,
  ): Promise<EncontroAiResponse | null> {
    return this.interacoesApi.buscarEncontroAiPorEncontroId(token, encontroId)
  }

  async buscarProximosPassosIntegracaoMoxe(
    token: string,
    payload: BuscarProximosPassosMoxeRequest,
  ): Promise<BuscarProximosPassosMoxeResponse> {
    return this.interacoesApi.buscarProximosPassosIntegracaoMoxe(token, payload)
  }

  async inserirArquivoEncontro(
    token: string,
    params: InserirArquivoEncontroParams,
  ): Promise<ArquivoEncontroDto> {
    return this.interacoesApi.inserirArquivoEncontro(token, params)
  }

  async deletarArquivoEncontro(token: string, arquivoId: number): Promise<void> {
    return this.interacoesApi.deletarArquivoEncontro(token, arquivoId)
  }

  async listarCategoriasAssuntoComSub(token: string): Promise<CategoriaAssuntoComSubModel[]> {
    return this.interacoesApi.listarCategoriasAssuntoComSub(token)
  }

  async listarCategoriasSubPorInteracaoId(
    token: string,
    interacaoId: number,
  ): Promise<InteracaoCategoriaSubResponse[]> {
    return this.interacoesApi.listarCategoriasSubPorInteracaoId(token, interacaoId)
  }

  async inserirInteracaoCategoria(
    token: string,
    payload: InserirInteracaoCategoriaPayload,
  ): Promise<{ id?: number }> {
    return this.interacoesApi.inserirInteracaoCategoria(token, payload)
  }

  async atualizarInteracaoCategoria(
    token: string,
    payload: AtualizarInteracaoCategoriaPayload,
  ): Promise<void> {
    return this.interacoesApi.atualizarInteracaoCategoria(token, payload)
  }

  // CRUD de Ações/Passos
  async atualizarStatusAcoes(token: string, payload: AtualizarStatusAcoesPayload): Promise<AcaoResponse> {
    return this.acoesApi.atualizarStatusAcoes(token, payload)
  }

  async inserirComentarioAcao(token: string, payload: InserirComentarioAcaoPayload): Promise<AcaoResponse> {
    return this.acoesApi.inserirComentarioAcao(token, payload)
  }

  // Aceitar/Recusar Convite
  async aceitarRecusarConviteAgenda(
    token: string,
    params: {
      decisaoStatus: number
      agendaId: string
      codigoColaborador: string
    },
  ): Promise<boolean> {
    return this.agendasApi.aceitarRecusarConviteAgenda(token, params)
  }

  async buscarParticipacaoUsuario(
    token: string,
    agendaId: number,
  ): Promise<import('@domain/entities/AgendaGestor').ParticipacaoUsuarioLogado | null> {
    return this.agendasApi.buscarParticipacaoUsuario(token, agendaId)
  }
}
