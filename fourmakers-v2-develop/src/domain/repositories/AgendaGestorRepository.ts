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
  ParticipacaoUsuarioLogado,
  ArquivoEncontroDto,
  InserirArquivoEncontroParams,
  BuscarProximosPassosMoxeRequest,
  BuscarProximosPassosMoxeResponse,
  CategoriaAssuntoComSubModel,
  InteracaoCategoriaSubResponse,
  InserirInteracaoCategoriaPayload,
  AtualizarInteracaoCategoriaPayload,
} from '@domain/entities/AgendaGestor'

export interface AgendaGestorRepository {
  /**
   * Carrega os dados completos de uma agenda por ID (para exibição no modal Ver Mais).
   */
  carregarAgendaDetalhe(token: string, agendaId: number): Promise<ItemAgendaGestor>

  /**
   * Busca agendas filhas de uma agenda pai (hierarquia pai-filho).
   */
  buscarAgendasFilhos(token: string, agendaPaiId: number): Promise<ItemAgendaGestor[]>

  /**
   * Busca todas as agendas dentro de um range de datas
   */
  buscarAgendas(
    token: string,
    dataInicio: string,
    dataFim: string,
  ): Promise<AgendaResponse[]>

  /**
   * Busca interações de um colaborador específico
   */
  buscarInteracoesPorColaborador(
    token: string,
    codInternoColaborador: string,
  ): Promise<InteracaoResponse[]>

  /**
   * Busca o kanban de ações comerciais
   */
  buscarKanbanAcoes(token: string): Promise<KanbanResponse>

  /**
   * Busca agenda consolidada de um gestor (agendas, interações e ações)
   * (combina os três métodos acima e filtra/mapeia os dados)
   */
  buscarAgendaCompleta(
    token: string,
    codInternoColaborador: string,
    dataInicio?: string,
    dataFim?: string,
  ): Promise<AgendaGestorCompleto>

  // CRUD de Agendas
  /** Carrega uma agenda por ID (detalhes completos para o modal Ver Mais). */
  carregarAgendaPorId(token: string, agendaId: number): Promise<ItemAgendaGestor>
  criarAgenda(token: string, payload: CriarAgendaPayload): Promise<AgendaResponse>
  atualizarAgenda(token: string, payload: AtualizarAgendaPayload): Promise<AgendaResponse>
  deletarAgenda(token: string, agendaId: number): Promise<boolean>

  // CRUD de Interações
  inserirInteracaoIa(token: string, payload: InserirInteracaoIaPayload): Promise<InteracaoResponse>
  atualizarInteracaoIa(token: string, payload: AtualizarInteracaoIaPayload): Promise<InteracaoResponse>
  deletarInteracaoIa(token: string, interacaoId: number): Promise<boolean>

  // EncontroAi (próximos passos com IA)
  criarEncontroAiProximosPassos(token: string, payload: CriarEncontroAiPayload): Promise<EncontroAiResponse>
  atualizarEncontroAiProximosPassos(token: string, payload: AtualizarEncontroAiPayload): Promise<EncontroAiResponse>
  buscarEncontroAiPorEncontroId(token: string, encontroId: number): Promise<EncontroAiResponse | null>

  // Jornada Comercial: IA Moxe (gerar resumo + passos)
  buscarProximosPassosIntegracaoMoxe(token: string, payload: BuscarProximosPassosMoxeRequest): Promise<BuscarProximosPassosMoxeResponse>

  // Arquivos de encontro
  inserirArquivoEncontro(
    token: string,
    params: InserirArquivoEncontroParams,
  ): Promise<ArquivoEncontroDto>
  deletarArquivoEncontro(token: string, arquivoId: number): Promise<void>

  // Categoria e Subcategoria de interação (Jornada Comercial)
  listarCategoriasAssuntoComSub(token: string): Promise<CategoriaAssuntoComSubModel[]>
  listarCategoriasSubPorInteracaoId(token: string, interacaoId: number): Promise<InteracaoCategoriaSubResponse[]>
  inserirInteracaoCategoria(
    token: string,
    payload: InserirInteracaoCategoriaPayload,
  ): Promise<{ id?: number }>
  atualizarInteracaoCategoria(
    token: string,
    payload: AtualizarInteracaoCategoriaPayload,
  ): Promise<void>

  // CRUD de Ações/Passos
  atualizarStatusAcoes(token: string, payload: AtualizarStatusAcoesPayload): Promise<AcaoResponse>
  inserirComentarioAcao(token: string, payload: InserirComentarioAcaoPayload): Promise<AcaoResponse>

  // Aceitar/Recusar Convite
  aceitarRecusarConviteAgenda(
    token: string,
    params: {
      decisaoStatus: number // 1 = aceitar, 0 = recusar
      agendaId: string
      codigoColaborador: string // cpf
    },
  ): Promise<boolean>

  buscarParticipacaoUsuario(token: string, agendaId: number): Promise<ParticipacaoUsuarioLogado | null>
}
