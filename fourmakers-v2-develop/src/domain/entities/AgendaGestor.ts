export type TipoItemAgenda = 'agenda' | 'interacao' | 'acao'

// Interfaces para substituir uso de 'any'
export interface AreaDeAtuacao {
  id?: number
  nome?: string
  [key: string]: unknown // Permite campos adicionais desconhecidos
}

export interface PreferenciasPessoais {
  [key: string]: unknown // Estrutura dinâmica, usar unknown ao invés de any
}

/**
 * Participação do usuário logado na agenda.
 * Retornado ao listar agendas e ao carregar detalhe da agenda; define se o usuário confirmou presença.
 * confirmado: null = não participante, 0 = pendente, 1 = confirmado, 2 = recusado
 */
export interface ParticipacaoUsuarioLogado {
  agendaId?: number
  confirmado?: number | null
  dataConfirmacao?: string | null
}

export interface ItemAgendaGestor {
  tipo: TipoItemAgenda
  id: string
  agendaId?: number // Para navegação
  titulo: string
  responsavel: string
  codColaboradorCriador?: string // Código do colaborador criador (CPF ou código interno)
  data: string // ISO 8601
  status: string
  descricao?: string
  participantes?: number // Contagem
  tipoInteracao?: string // Reunião, Ligação, Chat, Email, Presencial
  localizacao?: string
  linkReuniao?: string
  /** ID do evento Microsoft Graph (reunião Teams vinculada). */
  graphEventId?: string
  // Campos adicionais da API
  dataInicio?: string
  dataFim?: string
  dataCriacao?: string
  dataAtualizacao?: string
  /** Data de requisição do encontro/interação (normalizada ao timezone na exibição). */
  dataRequisicao?: string
  cliente?: {
    codigoCliente: string
    nomeCliente: string
  }
  colaboradores?: Array<{
    codInternoColaborador: string
    nome: string
    email: string
  }>
  gestoresExternos?: Array<{
    codGestorExterno: string
    codigoInternoColaborador?: string
    nome: string
    email: string
  }>
  participantesDetalhes?: Array<{
    codigoGestorExterno?: string
    codigoColaborador?: string
    Nome?: string // Nome do participante vindo da API
    confirmado: boolean
    dataConfirmacao?: string
  }>
  // Participação do usuário logado (status do convite/presença)
  // confirmado (referência): null=não participante, 0=pendente, 1=confirmado, 2=recusado
  // statusSolicitacaoParticipante: 0=Pendente, 1=Aceito, 2=Recusado, 3=Talvez (number da API ou string exibível)
  participacaoUsuarioLogado?: ParticipacaoUsuarioLogado & {
    statusSolicitacaoParticipante?: number | string | null
  }
  // Para interações
  encontroAi?: {
    resumo?: string
    passos?: Array<{
      texto: string
      nomeColaborador: string
      dataLimite?: string
    }>
  }
  resumoInteracao?: string
  /** Categorias e subcategorias da interação (Jornada Comercial). */
  interacaoCategorias?: Array<{
    interacaoCategoriaId?: number
    categoriaAssuntoId?: number
    subCategoriaAssuntoId?: number
    categoriaDescricao?: string
    subCategoriaDescricao?: string
  }>
  /** Arquivos anexados (quando o item é uma interação com dados de encontro). */
  arquivos?: Array<{
    id: number
    dataArquivo: string
    linkAudioInteracao?: string | null
    linkImagemInteracao?: string | null
    transcricao?: string | null
  }>
  // Para ações
  atrasado?: boolean
  vencendo?: boolean
  // Encontros/Interações associadas à agenda (retorno de detalhes da agenda)
  encontros?: Array<{
    id: number
    tituloInteracao: string
    descricaoInteracao?: string
    resumoInteracao?: string
    dataRequisicao: string
    nomeCompletoColaboradorCriador: string
    encontroAi?: {
      resumo?: string
      passos?: Array<{
        texto: string
        nomeColaborador: string
        dataLimite?: string
      }>
    }
    /** Categorias e subcategorias da interação (Jornada Comercial). */
    interacaoCategorias?: Array<{
      interacaoCategoriaId?: number
      categoriaAssuntoId?: number
      subCategoriaAssuntoId?: number
      categoriaDescricao?: string
      subCategoriaDescricao?: string
    }>
    /** Arquivos anexados ao encontro (exibidos ao abrir/editar interação). */
    arquivos?: Array<{
      id: number
      dataArquivo: string
      linkAudioInteracao?: string | null
      linkImagemInteracao?: string | null
      transcricao?: string | null
    }>
  }>
  /** ID da agenda pai (null = agenda raiz). Hierarquia pai-filho. */
  agendaPaiId?: number | null
}

export interface AgendaGestorCompleto {
  codInternoColaborador: string
  nomeGestor: string
  agendas: ItemAgendaGestor[]
  interacoes: ItemAgendaGestor[]
  acoes: ItemAgendaGestor[]
  dataUltimaAtualizacao: string
}

// Modelos de resposta da API (mapeados para as entidades acima)

export interface AgendaResponse {
  id: number
  titulo: string
  descricao?: string
  tipoInteracao: number // 1=Reunião, 2=Ligação, 3=Chat, 4=Email, 5=Presencial
  dataAgendada: string
  dataInicio?: string
  dataFim?: string
  status: string
  localizacao?: string
  linkReuniao?: string
  /** ID do evento Microsoft Graph (reunião Teams vinculada). */
  graphEventId?: string
  quantidadeParticipantes?: number
  codColaboradorCriador: string
  nomeCompletoColaboradorCriador: string
  cliente?: {
    id: number
    codigoCliente: string
    nomeCliente: string
  }
  colaboradores?: Array<{
    codInternoColaborador: string
    nome: string
    email: string
  }>
  gestoresExternos?: Array<{
    areasDeAtuacao?: AreaDeAtuacao[]
    codigoInternoColaborador?: string | null
    nome: string
    email: string
    dataCriacao?: string
    dataAlteracao?: string
    telefone?: string | null
    codigoCliente?: string | null
    perfilLinkedin?: string | null
    codGestorExterno?: string | null
    preferenciasPessoais?: PreferenciasPessoais
  }>
  participantes?: Array<{
    id: number
    codigoGestorExterno?: string | null
    codigoColaborador?: string | null
    Nome?: string | null
    confirmado: boolean
    dataConfirmacao?: string | null
    interessado?: boolean
    dataInteresse?: string | null
  }>
  encontros?: InteracaoResponse[]
  dataCriacao: string
  dataAtualizacao?: string
  // Participação do usuário logado na agenda (API pode enviar número ou string)
  participacaoUsuarioLogado?: {
    statusSolicitacaoParticipante?: string | number | null // 0=Pendente, 1=Aceito, 2=Recusado, 3=Talvez
    confirmado?: number | boolean
    dataConfirmacao?: string | null
  }
  /** ID da agenda pai (null = agenda raiz). Hierarquia pai-filho. */
  agendaPaiId?: number | null
}

/** DTO de arquivo anexado a um encontro (interação). Alinhado ao campo arquivos de InteracaoResponse. */
export interface ArquivoEncontroDto {
  id: number
  dataArquivo: string
  linkAudioInteracao?: string
  linkImagemInteracao?: string
  transcricao?: string
}

/** Parâmetros para upload de arquivo em encontro (POST InserirArquivo). */
export interface InserirArquivoEncontroParams {
  encontroId: number
  file: File
  nomeArquivo: string
  tipoArquivo: string
  transcricao?: string
}

export interface InteracaoResponse {
  id: number
  agendaId?: number
  tituloInteracao: string
  descricaoInteracao?: string
  codigoInternoColaborador: string
  nomeCompletoColaboradorCriador: string
  dataRequisicao: string
  encontroAi?: {
    id: number
    resumo: string
    dataGerada: string | null
    passos?: Array<{
      id: number
      texto: string
      codigoColaborador: string
      nomeColaborador: string
      dataLimite?: string
      statusAcoesId: number
    }> | null
  } | null
  resumoInteracao?: string | null
  arquivos?: ArquivoEncontroDto[]
}

export interface AcaoResponse {
  id: number
  interacaoAiId?: number
  agendaId?: number
  interacaoId?: number
  texto: string
  codInternoColaborador: string
  nomeResponsavel: string
  dataLimite: string
  statusAcoes: number // 1=Em andamento, 2=Pendente, 3=Concluída
  comentarioAcao?: string
  atrasado?: boolean
  vencendo?: boolean
  interacaoCategoria?: Array<{
    id: number
    nome: string
  }>
}

export interface KanbanResponse {
  reunioesAgendadas?: {
    cabecalho: {
      total: number
      totalExpirando: number
      totalAtrasados: number
    }
    reunioes: AcaoResponse[]
  }
  reunioesRealizadas?: {
    cabecalho: {
      total: number
    }
    reunioes: AcaoResponse[]
  }
  acoesAndamento?: {
    cabecalho: {
      total: number
      totalExpirando: number
      totalAtrasados: number
    }
    acoes: AcaoResponse[]
  }
  acoesPendentes?: {
    cabecalho: {
      total: number
      totalExpirando: number
      totalAtrasados: number
    }
    acoes: AcaoResponse[]
  }
  acoesRealizadas?: {
    cabecalho: {
      total: number
    }
    acoes: AcaoResponse[]
  }
}

// Payloads para CRUD de Agendas
export interface CriarAgendaPayload {
  tipoInteracao?: number
  graphEventId?: string
  dataAgendada?: string
  dataInicio?: string
  dataFim?: string
  status?: string
  localizacao?: string
  linkReuniao?: string
  quantidadeParticipantes?: number
  titulo?: string
  descricao?: string
  codigoCliente?: string
  codigosGestoresExternos?: string[]
  codigosColaboradores?: string[]
  agendaPaiId?: number
}

export interface AtualizarAgendaPayload {
  id: number
  graphEventId?: string
  agendaPaiId?: number
  tipoInteracao?: number
  dataAgendada?: string
  dataInicio?: string
  dataFim?: string
  status?: string
  localizacao?: string
  linkReuniao?: string
  quantidadeParticipantes?: number
  titulo?: string
  descricao?: string
  codigoCliente?: string
  codigosGestoresExternos?: string[]
  codigosColaboradores?: string[]
}

// Payloads para CRUD de Interações
export interface InserirInteracaoIaPayload {
  agendaId?: number
  tituloInteracao?: string
  descricaoInteracao?: string
  resumoInteracao?: string
}

export interface AtualizarInteracaoIaPayload {
  id: number
  agendaId?: number
  tituloInteracao?: string
  descricaoInteracao?: string
  resumoInteracao?: string
}

// Payloads para CRUD de Ações/Passos
export interface AtualizarStatusAcoesPayload {
  id: number
  statusAcoes: number // 1=Em andamento, 2=Pendente, 3=Concluída
}

export interface InserirComentarioAcaoPayload {
  acaoId: number
  comentarioAcao: string
}

// EncontroAi (próximos passos com IA)
export interface PassoEncontroAi {
  id?: number
  encontroAiId?: number
  texto: string
  codigoColaborador?: string
  nomeColaborador?: string
  dataLimite?: string
  statusAcoesId?: number
}

export interface EncontroAiResponse {
  id: number
  encontroId: number
  resumo: string
  dataGerada: string | null
  passos?: PassoEncontroAi[] | null
}

export interface CriarEncontroAiPayload {
  id?: number
  encontroId: number
  resumo: string
  dataGerada?: string
  passos?: Array<{
    texto: string
    codigoColaborador?: string
    nomeColaborador?: string
    dataLimite?: string
    statusAcoesId?: number
  }>
}

export interface AtualizarEncontroAiPayload {
  id: number
  encontroId?: number
  resumo?: string
  dataGerada?: string
  passos?: Array<{
    id?: number
    texto?: string
    codigoColaborador?: string
    nomeColaborador?: string
    dataLimite?: string
    statusAcoesId?: number
  }>
}

/** DTOs do request BuscarProximosPassosIntegracaoMoxe (legado Flutter: IntegracaoMoxeRequest). */

export interface AgendaMoxeDTO {
  titulo?: string
  tipoInteracao?: string
  data?: string
  horario?: string
  local?: string
  descricao?: string
  vagas?: number
}

export interface InteracaoMoxeDTO {
  ata?: string
  principaisPontosAudio?: string
}

export interface GestorMoxeDTO {
  nome: string
  email: string
}

export interface ClienteMoxeDTO {
  empresa?: string
  codigo?: string
  qtdAlocados?: number
  gestores?: GestorMoxeDTO[]
}

/** Request para BuscarProximosPassosIntegracaoMoxe (Jornada Comercial). Legado: IntegracaoMoxeRequest. */
export interface BuscarProximosPassosMoxeRequest {
  /** Ata/descrição da interação (step 4). Enviado em interacao.ata ou como fallback. */
  ata?: string
  /** Transcrições de áudio (máx. 1500 chars). Enviado em interacao.principaisPontosAudio. */
  principaisPontosAudio?: string
  /** ID da interação/encontro. */
  interacaoId?: number
  /** Dados da agenda (título, tipo, data, horário, local, descrição, vagas). */
  agenda?: AgendaMoxeDTO
  /** Ata e principais pontos de áudio (recomendado quando agenda/cliente/equipe estão preenchidos). */
  interacao?: InteracaoMoxeDTO
  /** Dados do cliente (empresa, código, qtd alocados, gestores). */
  cliente?: ClienteMoxeDTO
  /** Nomes dos colaboradores Fourtalents participantes. */
  equipeFourtalentsParticipante?: string[]
}

/** Resposta da API Moxe: resumo + passos (textos). Legado: take(3) passos iniciais. */
export interface BuscarProximosPassosMoxeResponse {
  resumo: string
  passos: string[]
}

/** Payload para InserirInteracaoIA (Jornada Comercial). Legado: interacaoId + resumo. */
export interface InserirInteracaoIAJornadaPayload {
  interacaoId: number
  resumo: string
}

/** Payload para AtualizarInteracaoIA (Jornada Comercial). Legado: id + resumo. */
export interface AtualizarInteracaoIAJornadaPayload {
  id: number
  resumo: string
}

/** Resposta de Inserir/Atualizar InteracaoIA (Jornada Comercial). */
export interface InteracaoIAJornadaResponse {
  id: number
  interacaoId?: number
  resumo?: string
  dataGerada?: string
}

// --- Categoria e Subcategoria de Interação (Jornada Comercial - legado Flutter) ---

/** Subcategoria de assunto (catálogo). GET ListarCategoriasAssuntoComSub. */
export interface SubcategoriaAssuntoModel {
  id?: number
  descricao?: string
  categoriaAssuntoId?: number
}

/** Categoria de assunto com subcategorias aninhadas. GET ListarCategoriasAssuntoComSub. */
export interface CategoriaAssuntoComSubModel {
  id?: number
  descricao?: string
  subcategorias?: SubcategoriaAssuntoModel[]
}

/** Resposta ao listar categorias de uma interação. GET ListarCategoriasSubPorInteracaoId. */
export interface InteracaoCategoriaSubResponse {
  interacaoId?: number
  interacaoCategoriaId?: number
  categoriaAssuntoId?: number
  subCategoriaAssuntoId?: number
  dataCriacao?: string
  ativo?: boolean
  /** Descrição da categoria (enriquecida localmente a partir do catálogo). */
  categoriaDescricao?: string
  /** Descrição da subcategoria (enriquecida localmente a partir do catálogo). */
  subCategoriaDescricao?: string
}

/** Payload para inserir categoria na interação. POST InsercaoInteracaoCategoria. */
export interface InserirInteracaoCategoriaPayload {
  interacaoId: number
  categoriaAssuntoId: number
  subCategoriaAssuntoId?: number | null
}

/** Payload para atualizar categoria da interação. POST AtualizarInteracaoCategoria. */
export interface AtualizarInteracaoCategoriaPayload {
  interacaoCategoriaId: number
  categoriaAssuntoId?: number
  subCategoriaAssuntoId?: number | null
}
