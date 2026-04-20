// ============================================
// COMMUNITY / INTRANET MODULE TYPES (Comunicação)
// ============================================

// Personas do sistema
export type CommunityPersona = 'user' | 'manager' | 'analytics';

// ============================================
// GRUPOS DE USUÁRIOS
// ============================================

export interface UserGroup {
  id: string;
  name: string;
  description: string;
  status: 'active' | 'inactive';
  requiresApproval: boolean;
  members: UserGroupMember[];
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  /** Preenchido ao carregar da API (GET Grupo por id) para modo edição. */
  permiteCriarPublicacaoInformativo?: boolean;
  aprovaPublicacaoInformativo?: boolean;
  /** Se os membros do grupo podem criar comunidades. */
  permiteCriarComunidade?: boolean;
}

export interface UserGroupMember {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  userAvatar?: string;
  addedAt: string;
}

export interface PersonaConfig {
  id: CommunityPersona;
  label: string;
  description: string;
  icon: string;
  permissions: CommunityPermission[];
}

export type CommunityPermission =
  | 'view_groups'
  | 'join_groups'
  | 'create_posts'
  | 'manage_groups'
  | 'manage_members'
  | 'view_metrics'
  | 'view_all_groups'
  | 'view_all_posts'
  | 'view_global_reports';

// ============================================
// COMUNIDADES (amarradas a Grupos de Usuários)
// ============================================

export type GroupType = 'free' | 'private';
export type GroupStatus = 'active' | 'archived';

export interface CommunityGroup {
  id: string;
  name: string;
  description: string;
  coverImage?: string;
  type: GroupType;
  creatorId: string;
  creatorName: string;
  moderators: GroupMember[];
  status: GroupStatus;
  createdAt: string;
  memberCount: number;
  postCount: number;
  settings: GroupSettings;
  linkedUserGroups: string[];
  individualMembers?: string[];
}

export interface GroupSettings {
  allowFreeEntry: boolean;
  allowMemberPosts: boolean;
  allowMemberLeave: boolean;
  requiresApproval: boolean;
  commentsEnabledByDefault: boolean;
  likesEnabledByDefault: boolean;
}

export interface GroupMember {
  id: string;
  userId: string;
  userName: string;
  userAvatar?: string;
  role: 'member' | 'moderator' | 'creator';
  joinedAt: string;
}

// ============================================
// COMUNICADOS (diretos para Grupos de Usuários)
// ============================================

export type AnnouncementType = 'informative' | 'document';

export type AutoriaTipo = 'pessoal' | 'alternativo';

export interface Announcement {
  id: string;
  targetUserGroupIds: string[];
  targetUserGroupNames: string[];
  authorId: string;
  authorName: string;
  authorAvatar?: string;
  authorArea?: string;
  announcementType: AnnouncementType;
  title: string;
  /** Subtítulo do comunicado (opcional). */
  subtitulo?: string;
  content: string;
  /** Autoria: pessoal (publicado por mim) ou alternativo (publicado pela empresa). */
  autoriaTipo?: AutoriaTipo;
  attachments: PostAttachment[];
  folderPath?: string;
  folderPaths?: string[];
  tags?: string[];
  status: 'draft' | 'scheduled' | 'published' | 'pending_approval' | 'archived' | 'deleted';
  scheduledAt?: string;
  publishedAt?: string;
  expiresAt?: string;
  requiresAcknowledgment: boolean;
  allowComments: boolean;
  allowLikes: boolean;
  isPinned: boolean;
  allowDownload?: boolean;
  likesCount: number;
  commentsCount: number;
  viewsCount: number;
  acknowledgmentCount: number;
  createdAt: string;
  updatedAt: string;
  /** Id da comunidade (quando vindo da API de listagem); usado para filtrar na tab Comunicados. */
  comunidadeId?: string;
  /** Id do comunicado quando a publicação é vinculada a um comunicado (não exibir na lista da tab Comunicados). */
  comunicadoId?: string | null;
  /** Indica se o comunicado está ativo (false = arquivado). Preenchido pelo payload da listagem/edição. */
  ativo?: boolean;
  /** Se true, o comunicado não aparece no feed (apenas na lista de comunicados). Default false. */
  ocultarNoFeed?: boolean;
}

// ============================================
// POSTS (COMUNICADOS EM COMUNIDADES)
// ============================================

export type PostType = 'text' | 'image' | 'video' | 'document';
export type PostStatus = 'draft' | 'scheduled' | 'published' | 'pending_approval' | 'archived';

export interface CommunityPost {
  id: string;
  groupId: string;
  groupName: string;
  authorId: string;
  authorName: string;
  authorAvatar?: string;
  authorArea?: string;
  type: PostType;
  title: string;
  /** Subtítulo do comunicado (opcional, ex.: feed). */
  subtitulo?: string | null;
  content: string;
  attachments: PostAttachment[];
  /** Autoria: pessoal ou alternativo (preenchido quando a API envia nomeAutorAlternativo). */
  autoriaTipo?: AutoriaTipo;
  status: PostStatus;
  folderPath?: string;
  scheduledAt?: string;
  publishedAt?: string;
  visibility: PostVisibility;
  requiresAcknowledgment: boolean;
  allowComments: boolean;
  allowLikes: boolean;
  isPinned: boolean;
  /** Se o usuário pode baixar o documento no preview (permiteDownload da API). */
  allowDownload?: boolean;
  likesCount: number;
  commentsCount: number;
  viewsCount: number;
  acknowledgmentCount: number;
  createdAt: string;
  updatedAt: string;
  /** Data de validade da publicação (após esta data fica invisível). */
  expiresAt?: string;
  /** Preenchido quando a API indica que o usuário já confirmou leitura (interacao.confirmouLeitura + dataConfirmouLeitura). */
  acknowledgedAt?: string;
  /** Labels/tags da publicação (ex.: retorno da API Feed). */
  labels?: string[];
  /** Indica se a publicação está ativa (false = arquivada). Preenchido pelo payload da listagem. */
  ativo?: boolean;
  /** Status de aprovação (nao_requer, pendente, aprovado, rejeitado). Quando pendente, o comunicado aparece em Agendados/aprov. */
  approvalStatus?: string;
  /** Id da comunidade quando a publicação pertence a uma comunidade (feed). */
  comunidadeId?: string | null;
  /** Nome da comunidade quando a publicação pertence a uma comunidade (feed). */
  comunidadeNome?: string | null;
  /** Id do comunicado quando a publicação é vinculada a um comunicado (não exibir na lista da tab Comunicados). */
  comunicadoId?: string | null;
  /** Comentários da publicação (preenchido quando a API retorna comentarios). */
  comments?: PostComment[];
  /** Contagem de reações por emoji (ex.: { "❤️": 2, "👏": 1 }). Preenchido pelo retorno da publicação. */
  reactionCounts?: Record<string, number>;
  /** Emoji que o usuário logado reagiu a esta publicação (preenchido pelo retorno da API). */
  meuEmojiReacao?: string;
  /** Se true, o comunicado não aparece no feed (apenas na lista de comunicados). */
  ocultarNoFeed?: boolean;
}

/** Comentário exibido em um post (mapeado da API). parentId preenchido quando for resposta a outro comentário. */
export interface PostComment {
  id: string;
  postId?: string;
  userId?: string;
  userName?: string;
  content: string;
  createdAt: string;
  authorId: string;
  authorName: string;
  authorAvatar?: string;
  /** Id do comentário pai quando for resposta (um nível). */
  parentId?: string;
  /** Total de reações (likes) no comentário. */
  likesCount?: number;
  /** Contagem por emoji (ex.: { "❤️": 2, "👏": 1 }). */
  reactionCounts?: Record<string, number>;
  /** Emoji que o usuário logado reagiu a este comentário. */
  meuEmojiReacao?: string;
  updatedAt?: string;
}

export interface PostAttachment {
  id: string;
  type: 'image' | 'video' | 'document';
  url: string;
  name: string;
  size?: number;
  mimeType?: string;
}

export interface PostVisibility {
  type: 'all' | 'specific_users' | 'exclude_users';
  userIds?: string[];
}

// ============================================
// INTERAÇÕES
// ============================================

export interface PostLike {
  id: string;
  postId: string;
  userId: string;
  userName: string;
  createdAt: string;
}


export interface PostAcknowledgment {
  id: string;
  postId: string;
  userId: string;
  userName: string;
  acknowledgedAt: string;
}

// ============================================
// BIBLIOTECA
// ============================================

export interface LibraryDocument {
  id: string;
  postId: string;
  groupId: string;
  groupName: string;
  name: string;
  authorId: string;
  authorName: string;
  type: string;
  url: string;
  size?: number;
  folderPath?: string;
  requiresAcknowledgment: boolean;
  isAcknowledged: boolean;
  createdAt: string;
  /** Data de validade do documento (após esta data fica invisível). */
  expiresAt?: string;
  /** Se true, o documento está oculto no feed (exibe tag "Oculto no feed" no card). */
  ocultarNoFeed?: boolean;
}

export type LibraryFilter = {
  groupId?: string;
  acknowledgmentStatus?: 'all' | 'required' | 'acknowledged' | 'pending';
  authorId?: string;
  dateFrom?: string;
  dateTo?: string;
};

// ============================================
// NOTIFICAÇÕES
// ============================================

export type CommunityNotificationType =
  | 'new_post'
  | 'scheduled_post_published'
  | 'acknowledgment_required'
  | 'acknowledgment_reminder';

export interface CommunityNotification {
  id: string;
  type: CommunityNotificationType;
  title: string;
  message: string;
  postId?: string;
  groupId?: string;
  isRead: boolean;
  createdAt: string;
}

// ============================================
// MÉTRICAS
// ============================================

export interface GroupMetrics {
  groupId: string;
  totalPosts: number;
  totalMembers: number;
  postsThisMonth: number;
  averageEngagement: number;
  acknowledgmentRate: number;
  topContributors: { userId: string; userName: string; postCount: number }[];
}

export interface PostMetrics {
  postId: string;
  views: number;
  likes: number;
  comments: number;
  shares: number;
  acknowledgedUsers: { userId: string; userName: string; acknowledgedAt: string }[];
  pendingUsers: { userId: string; userName: string }[];
  acknowledgmentRate: number;
}

export interface GlobalMetrics {
  totalGroups: number;
  totalPosts: number;
  totalUsers: number;
  activeUsersThisMonth: number;
  averageEngagementRate: number;
  totalAcknowledgmentsPending: number;
  topGroups: { groupId: string; groupName: string; engagement: number }[];
}

// ============================================
// PROFISSIONAIS
// ============================================

export interface Professional {
  id: string;
  name: string;
  avatar?: string;
  position: string;
  unit: string;
  managerName?: string;
  coverImage?: string;
  about?: string;
  email?: string;
  phone?: string;
  birthDate?: string;
  /** Preenchido pela API GET Profissionais; usado na aba Meus Favoritos. */
  favoritado?: boolean;
}

/** Item retornado por GET api/Marketing/Comunicacao/Profissionais */
export interface ComunicacaoProfissionalApiItem {
  codigoColaboradorInterno: string;
  email: string | null;
  nomeCompleto: string;
  codCargo: string;
  cargo: string;
  codDepartamento: string;
  departamento: string;
  telefone: string | null;
  sobre: string | null;
  dataAniversario: string | null;
  supervisor: string | null;
  favoritado?: boolean;
  /** URL da foto; pode conter $1 para substituição pelo token em base64. */
  urlFoto?: string | null;
}

/** Resposta de GET api/Marketing/Comunicacao/Profissionais */
export interface ComunicacaoProfissionaisApiResponse {
  retorno: ComunicacaoProfissionalApiItem[];
}

// ============================================
// FEED API (Marketing/Comunicacao/Publicacao/ObterListaPublicacaoGeral)
// ============================================

/** Params/payload para POST ObterListaPublicacaoGeral */
export interface ComunicacaoFeedParams {
  somenteLeituraObrigatoria?: boolean;
  tipo?: string;
  labels?: string[];
  comunidadeId?: string;
  /** Página (1-based) para infinite scroll. */
  pagina?: number;
  /** Itens por página. */
  quantidadePorPagina?: number;
  /** Quando true, exclui publicações com aprovacaoStatus "pendente" (só na rota feed; em announcements não filtra). */
  excluirPendenteAprovacao?: boolean;
  /** Quando true, retorna apenas publicações não ocultas no feed (usado somente na tab feed). */
  somenteNaoOcultoNoFeed?: boolean;
}

export interface ComunicacaoFeedLabel {
  id: string;
  nome: string;
  quantidadeDocumentos: number;
}

export interface ComunicacaoFeedAutor {
  codigoColaboradorInterno: string;
  email: string | null;
  nomeCompleto: string;
  codDepartamento: string | null;
  departamento: string | null;
  /** URL da foto do colaborador; pode conter $1 para substituição pelo token em base64. */
  urlFoto?: string;
  /** Quando preenchido, indica autoria alternativa (publicado pela empresa). Nome exibido como autor. */
  nomeAutorAlternativo?: string | null;
  /** URL da foto a exibir quando autoria é alternativa; pode conter $1 para token. */
  urlFotoAlternativa?: string | null;
}

export interface ComunicacaoFeedAnexo {
  anexoId: string;
  nomeArquivo: string;
  urlArquivo: string;
  tamanhoBytes: number;
  tipo: 'documento' | 'imagem' | 'video';
}

/** Comentário de uma publicação (retorno ObterListaPublicacaoGeral). */
export interface ComunicacaoFeedComentarioApi {
  comentarioId: string;
  conteudo: string;
  dataCriacao: string;
  autor: ComunicacaoFeedAutor | null;
  analitico?: unknown;
  /** Respostas diretas (um nível). */
  respostaComentarios?: ComunicacaoFeedComentarioApi[];
  /** Id do comentário pai quando este item for resposta. */
  comentarioPaiId?: string;
  interacao?: {
    curtidas?: number;
    curtidaEmoji?: unknown;
    /** Emoji que o usuário logado reagiu a este comentário (chave API, ex.: "divertido"). */
    emoji?: string;
    /** Emoji que o usuário logado reagiu (alternativa a emoji). */
    emojiReacaoUsuario?: string;
  };
  /** Lista de emojis com contagem (ex.: [{ emoji: "divertido", count: 1 }]). */
  interacoesPorEmoji?: Array<{ emoji: string; count: number }>;
}

/** Analítico da publicação (contagens). */
export interface ComunicacaoFeedAnaliticoApi {
  quantidadeVisualizacao?: number;
  quantidadeCurtida?: number;
  quantidadeComentarios?: number;
  quantidadeConfirmacoesLeitura?: number;
}

export interface ComunicacaoFeedPublicacaoApi {
  publicacaoId: string;
  tipo: string;
  titulo: string;
  /** Subtítulo do comunicado (opcional). */
  subtitulo?: string | null;
  conteudo: string;
  requerConfirmacaoLeitura: boolean;
  permiteComentarios: boolean;
  permiteCurtidas: boolean;
  fixada: boolean;
  permiteDownload?: boolean;
  /** Data/hora de publicação (ex.: "2026-02-23T20:05:07"); usada para tempo relativo no card. */
  dataPublicacao?: string | null;
  dataAgendamentoPublicacao: string | null;
  dataValidadePublicacao: string | null;
  publicacaoStatus: string;
  aprovacaoStatus: string;
  labels: string[];
  grupos: string[];
  /** Id da comunidade quando a publicação pertence a uma comunidade. */
  comunidadeId?: string | null;
  /** Nome da comunidade quando a publicação pertence a uma comunidade. */
  comunidadeNome?: string | null;
  /** Id do comunicado quando a publicação é vinculada a um comunicado (não exibir na lista da tab Comunicados). */
  comunicadoId?: string | null;
  autor: ComunicacaoFeedAutor | null;
  aprovador: unknown;
  anexos: ComunicacaoFeedAnexo[];
  interacao: {
    curtidas?: number;
    comentarios?: number;
    visualizacoes?: number;
    confirmacoesLeitura?: number;
    codigoInterno?: string;
    dataPrimeiraEntrega?: string;
    visualizado?: boolean;
    dataVisualizado?: string;
    confirmouLeitura?: boolean;
    dataConfirmouLeitura?: string;
    curtidaEmoji?: unknown;
    /** Emoji que o usuário logado reagiu (ex.: "❤️"). */
    emojiReacaoUsuario?: string;
  } | null;
  comentarios: ComunicacaoFeedComentarioApi[];
  /** Lista de emojis com contagem (ex.: [{ emoji: "❤️", count: 1 }]). */
  interacoesPorEmoji?: Array<{ emoji: string; count: number }>;
  analitico: ComunicacaoFeedAnaliticoApi | null;
  gerencial: unknown;
  /** Indica se a publicação está ativa (false = arquivada). Retornado no payload da listagem/edição. */
  ativo?: boolean;
  /** Se true, o comunicado não aparece no feed (apenas na lista de comunicados). */
  ocultarNoFeed?: boolean;
}

export interface ComunicacaoFeedRetornoApi {
  quantidadeTotal: number;
  quantidadePendentesLeitura: number;
  quantidadeLidosAceitos: number;
  labels: ComunicacaoFeedLabel[];
  publicacoes: ComunicacaoFeedPublicacaoApi[];
}

export interface ComunicacaoFeedApiResponse {
  retorno: ComunicacaoFeedRetornoApi;
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

export interface ComunicacaoFeedResult {
  quantidadeTotal: number;
  quantidadePendentesLeitura: number;
  quantidadeLidosAceitos: number;
  labels: ComunicacaoFeedLabel[];
  publicacoes: CommunityPost[];
  /** Código do usuário logado (viewer), preenchido a partir de interacao.codigoInterno da API. Usado ex.: para exibir "Arquivar" só nos posts do próprio usuário. */
  codigoColaboradorInternoViewer?: string;
}

/** Payload para confirmar leitura obrigatória */
export interface ConfirmarLeituraObrigatoriaPayload {
  publicacaoId: string;
}

/** Resposta da API de confirmar leitura obrigatória */
export interface ConfirmarLeituraObrigatoriaResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Resposta da API de arquivar publicação */
export interface ArquivarPublicacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Payload para PATCH api/Marketing/Comunicacao/Publicacao/OcultarNoFeed */
export interface OcultarNoFeedPayload {
  publicacaoId: string;
  ocultarNoFeed: boolean;
}

/** Resposta da API de criar publicação (POST api/Marketing/Comunicacao/Publicacao) */
export interface CriarPublicacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
  retorno?: { id?: string };
}

/** Payload para inserir comentário em publicação (POST .../Publicacao/Comentarios). Incluir comentarioPaiId para resposta. */
export interface InserirComentarioPublicacaoPayload {
  publicacaoId: string;
  conteudo: string;
  /** Id do comentário pai quando for resposta (um nível). */
  comentarioPaiId?: string;
}

/** Resposta da API de inserir comentário */
export interface InserirComentarioPublicacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Payload para atualizar comentário (PUT .../Publicacao/Comentarios) */
export interface AtualizarComentarioPublicacaoPayload {
  comentarioId: string;
  conteudo: string;
}

/** Resposta da API de atualizar/excluir comentário */
export interface ComentarioPublicacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Payload para interação (emoji) em comentário/publicação. POST .../PublicacaoComentario/Comentarios/Interacao */
export interface InserirInteracaoComentarioPayload {
  comentarioId: string;
  emoji: string;
}

/** Resposta da API de inserir interação (emoji) */
export interface InserirInteracaoComentarioResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Payload para remover interação (emoji). DELETE .../PublicacaoComentario/Comentarios/Interacao */
export interface ExcluirInteracaoComentarioPayload {
  comentarioId: string;
}

/** Resposta da API de excluir interação (emoji) */
export interface ExcluirInteracaoComentarioResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Payload para interação (emoji) em publicação. POST api/Marketing/Comunicacao/Publicacao/Interacao */
export interface InserirInteracaoPublicacaoPayload {
  publicacaoId: string;
  emoji?: string;
}

/** Resposta da API de inserir/excluir interação em publicação */
export interface InteracaoPublicacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Resposta da API GET api/Marketing/Comunicacao/Publicacao/{id} */
export interface ObterPublicacaoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
  retorno: ComunicacaoFeedPublicacaoApi | null;
}

/** Item retornado por GET .../Publicacao/{id}/ConfirmacoesLeitura */
export interface ConfirmacaoLeituraItemApi {
  codigoColaboradorInterno?: string;
  email?: string;
  nomeCompleto?: string;
  nome?: string;
  codCargo?: string;
  cargo?: string;
  codDepartamento?: string;
  departamento?: string;
  codDiretoria?: string;
  diretoria?: string;
  urlFoto?: string | null;
  dataVisualizacao?: string | null;
  dataConfirmacaoLeitura?: string | null;
  [key: string]: unknown;
}

/** Retorno da API GET .../Publicacao/{publicacaoId}/ConfirmacoesLeitura (array no retorno) */
export type ConfirmacoesLeituraRetornoApi =
  | ConfirmacaoLeituraItemApi[]
  | { totalDestinatarios?: number; itens?: ConfirmacaoLeituraItemApi[]; lista?: ConfirmacaoLeituraItemApi[] };

export interface ObterConfirmacoesLeituraResponse {
  sucesso: boolean;
  mensagem: string | null;
  retorno?: ConfirmacoesLeituraRetornoApi | null;
}

/** Confirmação de leitura (normalizado para uso no app). */
export interface ConfirmacaoLeitura {
  id: string;
  name: string;
  email?: string;
  cargo?: string;
  unidade?: string;
  urlFoto?: string | null;
  viewedAt: string;
  acknowledgedAt?: string;
}

export interface ConfirmacoesLeituraResult {
  totalDestinatarios: number;
  confirmacoes: ConfirmacaoLeitura[];
}

/** Grupo de comunicação (API GET api/Marketing/Comunicacao/Grupo) */
export interface ComunicacaoGrupo {
  id: string;
  nome: string;
  descricao: string;
  permiteCriarPublicacaoInformativo: boolean;
  publicacaoInformativoRequerAprovacao: boolean;
  aprovaPublicacaoInformativo: boolean;
  status: string;
  quantidadeParticipantes: number;
  iniciaisMembros: string[];
}

/** Resposta da API de listagem de grupos */
export interface ComunicacaoGruposApiResponse {
  retorno: ComunicacaoGrupo[];
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Permissões do usuário logado (GET api/Marketing/Comunicacao/Grupo/PermissoesUsuarioLogado) */
export interface PermissoesUsuarioLogado {
  permiteCriarPublicacaoInformativo: boolean;
  publicacaoInformativoRequerAprovacao: boolean;
  aprovaPublicacaoInformativo: boolean;
  permiteCriarComunidade: boolean;
  gestaoComunicados: boolean;
}

export interface PermissoesUsuarioLogadoApiResponse {
  retorno: PermissoesUsuarioLogado;
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Payload para criar grupo (POST api/Marketing/Comunicacao/Grupo) */
export interface CriarGrupoPayload {
  nome: string;
  descricao: string;
  permiteCriarPublicacaoInformativo: boolean;
  publicacaoInformativoRequerAprovacao: boolean;
  aprovaPublicacaoInformativo: boolean;
  /** Se os membros do grupo podem criar comunidades. */
  permiteCriarComunidade?: boolean;
  codigoInternoColaboradoresParticipantes: string[];
}

/** Resposta da API de criar grupo */
export interface CriarGrupoResponse {
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Colaborador participante (retorno GET Grupo por id) */
export interface GrupoColaboradorParticipante {
  codigoColaboradorInterno: string;
  email: string;
  nomeCompleto: string;
  codDepartamento: string;
  departamento: string;
}

/** Retorno da API GET api/Marketing/Comunicacao/Grupo/{id} */
export interface ComunicacaoGrupoDetalhe {
  id: string;
  nome: string;
  descricao: string;
  permiteCriarPublicacaoInformativo: boolean;
  publicacaoInformativoRequerAprovacao: boolean;
  aprovaPublicacaoInformativo: boolean;
  permiteCriarComunidade?: boolean;
  colaboradoresParticipantes: GrupoColaboradorParticipante[];
}

/** Resposta da API de obter grupo por id */
export interface ComunicacaoGrupoDetalheApiResponse {
  retorno: ComunicacaoGrupoDetalhe;
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Colaborador disponível (API GET .../Grupo/ColaboradoresDisponiveis) */
export interface ColaboradorDisponivel {
  codigoColaboradorInterno: string;
  email: string;
  nomeCompleto: string;
  codDepartamento: string;
  departamento: string;
}

/** Resposta da API de colaboradores disponíveis */
export interface ColaboradoresDisponiveisApiResponse {
  retorno: { colaboradores: ColaboradorDisponivel[] };
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Moderador retornado na listagem de comunidades (array moderadores) */
export interface ComunicacaoComunidadeModeradorLista {
  id: string;
  codigoInternoColaborador: string;
  nomeCompleto: string;
  urlFoto?: string;
}

/** Comunidade (API GET api/Marketing/Comunicacao/Comunidade) */
export interface ComunicacaoComunidade {
  id: string;
  nome: string;
  descricao: string;
  capaUrl: string | null;
  tipo: string;
  permitePostagemMembro: boolean;
  permiteSair: boolean;
  publicacaoConfiguracaoPolitica: string;
  publicacaoPermiteComentario: boolean;
  publicacaoPermiteLikeHabilitado: boolean;
  totalPublicacoes: number;
  totalMembros: number;
  /** Lista de moderadores da comunidade (usuário logado deve estar aqui para ver Editar/Arquivar). */
  moderadores?: ComunicacaoComunidadeModeradorLista[];
}

/** Resposta da API de listagem de comunidades */
export interface ComunicacaoComunidadesApiResponse {
  retorno: { comunidades: ComunicacaoComunidade[] };
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Membro retornado no detalhe da comunidade (GET .../Comunidade/{id}) */
export interface ComunicacaoComunidadeMembro {
  id: string;
  codigoInternoColaborador: string;
  nomeCompleto: string;
}

/** Moderador retornado no detalhe da comunidade (GET .../Comunidade/{id}) */
export interface ComunicacaoComunidadeModerador {
  codigoInternoColaborador: string;
  nomeCompleto?: string;
}

/** Retorno do detalhe da comunidade (GET api/Marketing/Comunicacao/Comunidade/{id}) */
export interface ComunicacaoComunidadeDetalheRetorno {
  membros: ComunicacaoComunidadeMembro[];
  /** Lista de moderadores (quem pode postar quando permitePostagemMembro = false). */
  moderadores?: ComunicacaoComunidadeModerador[];
  id: string;
  nome: string;
  descricao: string;
  capaUrl: string | null;
  tipo: string;
  permitePostagemMembro: boolean;
  permiteSair: boolean;
  publicacaoConfiguracaoPolitica: string;
  publicacaoPermiteComentario: boolean;
  publicacaoPermiteLikeHabilitado: boolean;
  totalPublicacoes: number;
  totalMembros: number;
  /** Se o usuário logado está participando da comunidade. */
  participando?: boolean;
}

/** Resposta da API de detalhe da comunidade por id */
export interface ComunicacaoComunidadeDetalheApiResponse {
  retorno: ComunicacaoComunidadeDetalheRetorno;
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Resposta da API POST criar comunidade (api/Marketing/Comunicacao/Comunidade) */
export interface CriarComunidadeApiResponse {
  sucesso: boolean;
  mensagem: string | null;
  retorno?: { id?: string };
  erros: unknown;
}

/** Payload POST api/Marketing/Comunicacao/Analytics/Resumo */
export interface AnalyticsResumoPayload {
  dataInicio: string | null;
  dataFim: string | null;
  tipoConteudo: string | null;
  texto: string | null;
  pagina: number;
  tamanhoPagina: number;
}

/** Big numbers do retorno do Resumo */
export interface AnalyticsResumoBigNumbers {
  postsComunidades: number;
  engajamentoPosts: number;
  likesPosts: number;
  comentariosPosts: number;
  comunicadosPublicados: number;
  comunicadosInformativos: number;
  comunicadosDocumentos: number;
  aceitesComunicados: number;
  comunicadosObrigatorios: number;
  comunicadosOpcionais: number;
}

/** Item da lista do retorno do Resumo */
export interface AnalyticsResumoItem {
  publicacaoId: string;
  tipo: string;
  titulo: string;
  onde: string;
  obrigatorioLeitura: boolean;
  data: string;
  likes: number;
  comentarios: number;
  aceites: number;
  visualizacoes: number;
  alcance: number;
  engajamentoPercentual: number;
}

/** Retorno da API POST api/Marketing/Comunicacao/Analytics/Resumo */
export interface AnalyticsResumoRetorno {
  bigNumbers: AnalyticsResumoBigNumbers;
  itens: AnalyticsResumoItem[];
  totalItens: number;
}

export interface AnalyticsResumoApiResponse {
  retorno: AnalyticsResumoRetorno;
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

/** Modos do POST api/Marketing/Comunicacao/IA/Assistente */
export type AssistenteIaComunicacaoModo =
  | 'melhorar_texto'
  | 'mais_profissional'
  | 'resumo'
  | 'sumario_executivo'
  | 'adicionar_topicos'
  | 'expandir_conteudo';

/** Payload do assistente de IA de comunicação */
export interface AssistenteIaComunicacaoPayload {
  texto: string;
  modo: AssistenteIaComunicacaoModo;
  negrito: boolean;
}

/**
 * Corpo do retorno do assistente (API pode enviar string ou objeto com texto).
 */
export type AssistenteIaComunicacaoRetornoBruto =
  | string
  | {
      texto?: string;
      textoProcessado?: string;
      conteudo?: string;
    }
  | null;

export interface AssistenteIaComunicacaoApiResponse {
  retorno: AssistenteIaComunicacaoRetornoBruto;
  sucesso: boolean;
  mensagem: string | null;
  erros: unknown;
}

export type AssistenteIaComunicacaoProcessamentoResult =
  | { sucesso: true; texto: string }
  | { sucesso: false; mensagem: string };
