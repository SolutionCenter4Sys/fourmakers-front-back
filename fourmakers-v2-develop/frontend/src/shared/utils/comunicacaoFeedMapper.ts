import type {
  CommunityPost,
  LibraryDocument,
  PostAttachment,
  PostComment,
  PostType,
  PostStatus,
  ComunicacaoFeedComentarioApi,
  ComunicacaoFeedPublicacaoApi,
} from '@domain/entities/comunicacao';

/** Chaves string que a API envia/recebe → emoji canônico para exibição. */
const EMOJI_CHAVE_PARA_CHAR: Record<string, string> = {
  coracao: '❤️',
  foguete: '🚀',
  palmas: '👏',
  divertido: '😄',
  foguinho: '🔥',
};

/** Emojis canônicos usados nas reações do feed. */
const EMOJIS_REACOES_CANONICOS = ['❤️', '🚀', '👏', '😄', '🔥'] as const;

/** Variantes que a API pode enviar (ex.: sem variation selector) → emoji canônico. */
const EMOJI_VARIANTES: Record<string, string> = {
  '\u2764': '❤️',
  '\u2764\uFE0F': '❤️',
};

function normalizarEmojiReacao(emoji: string | undefined): string | undefined {
  if (!emoji || typeof emoji !== 'string') return undefined;
  const t = emoji.trim();
  if (!t) return undefined;
  // Prioridade 1: chave da API (ex.: "coracao" → "❤️")
  if (EMOJI_CHAVE_PARA_CHAR[t]) return EMOJI_CHAVE_PARA_CHAR[t];
  // Prioridade 2: variantes de codepoint
  const nfc = t.normalize('NFC');
  const canonical = EMOJI_VARIANTES[nfc] ?? EMOJI_VARIANTES[t];
  if (canonical) return canonical;
  // Prioridade 3: já é um emoji canônico
  if (EMOJIS_REACOES_CANONICOS.includes(nfc as (typeof EMOJIS_REACOES_CANONICOS)[number])) return nfc;
  if (EMOJIS_REACOES_CANONICOS.includes(t as (typeof EMOJIS_REACOES_CANONICOS)[number])) return t;
  return t;
}

function mapTipoToPostType(tipo: string): PostType {
  if (tipo === 'documento') return 'document';
  if (tipo === 'imagem' || tipo === 'image') return 'image';
  if (tipo === 'video') return 'video';
  return 'text';
}

function mapPublicacaoStatusToPostStatus(publicacaoStatus: string): PostStatus {
  const s = (publicacaoStatus ?? '').toLowerCase().replace(/-/g, '_');
  if (s === 'ativa') return 'published';
  if (s === 'agendada') return 'scheduled';
  if (s === 'rascunho') return 'draft';
  if (s === 'arquivada') return 'archived';
  if (s === 'aguardando_aprovacao') return 'pending_approval';
  return 'published';
}

/**
 * Substitui $1 em urlArquivo pelo token do usuário logado em base64 (para o backend autorizar o acesso ao anexo).
 */
function resolveUrlArquivo(urlArquivo: string, token: string | undefined): string {
  if (!token) return urlArquivo;
  const tokenBase64 = btoa(token);
  return urlArquivo.replace(/\$1/g, tokenBase64);
}

function mapAnexoToPostAttachment(
  a: ComunicacaoFeedPublicacaoApi['anexos'][0],
  token: string | undefined,
): PostAttachment {
  const type = a.tipo === 'imagem' ? 'image' : a.tipo === 'video' ? 'video' : 'document';
  return {
    id: a.anexoId,
    type,
    url: resolveUrlArquivo(a.urlArquivo, token),
    name: a.nomeArquivo,
    size: a.tamanhoBytes,
  };
}

function mapComentarioToPostComment(
  c: ComunicacaoFeedComentarioApi,
  token: string | undefined,
  parentId?: string,
): PostComment {
  const authorAvatar =
    c.autor?.urlFoto && c.autor.urlFoto.trim() !== ''
      ? resolveUrlArquivo(c.autor.urlFoto, token)
      : undefined;
  const interacao = c.interacao ?? {};
  const reactionCounts = (() => {
    if (c.interacoesPorEmoji && c.interacoesPorEmoji.length > 0) {
      return c.interacoesPorEmoji.reduce<Record<string, number>>((acc, { emoji, count }) => {
        const key = normalizarEmojiReacao(emoji) ?? emoji;
        acc[key] = (acc[key] ?? 0) + count;
        return acc;
      }, {});
    }
    if (
      typeof interacao.curtidaEmoji === 'object' &&
      interacao.curtidaEmoji !== null &&
      !Array.isArray(interacao.curtidaEmoji)
    ) {
      const raw = interacao.curtidaEmoji as Record<string, number>;
      return Object.entries(raw).reduce<Record<string, number>>((acc, [k, v]) => {
        const key = normalizarEmojiReacao(k) ?? k;
        acc[key] = (acc[key] ?? 0) + v;
        return acc;
      }, {});
    }
    return undefined;
  })();
  const rawMeuEmoji =
    (typeof interacao.emoji === 'string' && interacao.emoji.trim() !== '' ? interacao.emoji.trim() : undefined) ??
    (typeof interacao.emojiReacaoUsuario === 'string' && interacao.emojiReacaoUsuario.trim() !== ''
      ? interacao.emojiReacaoUsuario.trim()
      : undefined);
  const meuEmojiReacao = normalizarEmojiReacao(rawMeuEmoji);
  return {
    id: c.comentarioId,
    content: c.conteudo,
    createdAt: c.dataCriacao,
    authorId: c.autor?.codigoColaboradorInterno ?? '',
    authorName: c.autor?.nomeCompleto ?? '',
    authorAvatar,
    ...(parentId ? { parentId } : {}),
    likesCount: interacao.curtidas ?? (reactionCounts ? Object.values(reactionCounts).reduce((a, b) => a + b, 0) : undefined),
    reactionCounts,
    meuEmojiReacao,
  };
}

/** Data ISO válida para quando a API não retorna data (evita Invalid time value no date-fns). */
const FALLBACK_DATE = new Date().toISOString();

/**
 * Mapeia um item de publicação da API Feed para CommunityPost (domínio).
 * @param token - Token do usuário logado; usado para substituir $1 em urlArquivo dos anexos por token em base64.
 */
export function publicacaoFeedApiToCommunityPost(
  p: ComunicacaoFeedPublicacaoApi,
  token?: string,
): CommunityPost {
  const interacao = p.interacao ?? {};
  /** Prioriza dataPublicacao para exibição (tempo relativo no card); fallback para dataPrimeiraEntrega quando a API não envia data de publicação. */
  const dataPublicacao =
    p.dataPublicacao ??
    p.dataAgendamentoPublicacao ??
    interacao.dataPrimeiraEntrega ??
    FALLBACK_DATE;
  const acknowledgedAt =
    interacao.confirmouLeitura && interacao.dataConfirmouLeitura
      ? interacao.dataConfirmouLeitura
      : undefined;
  const isAutorAlternativo =
    p.autor?.nomeAutorAlternativo != null && String(p.autor.nomeAutorAlternativo).trim() !== '';
  const authorName = isAutorAlternativo
    ? String(p.autor!.nomeAutorAlternativo).trim()
    : (p.autor?.nomeCompleto ?? '');
  const authorAvatar = (() => {
    if (isAutorAlternativo && p.autor?.urlFotoAlternativa && String(p.autor.urlFotoAlternativa).trim() !== '') {
      return resolveUrlArquivo(String(p.autor.urlFotoAlternativa).trim(), token);
    }
    if (p.autor?.urlFoto && p.autor.urlFoto.trim() !== '') {
      return resolveUrlArquivo(p.autor.urlFoto, token);
    }
    return undefined;
  })();

  const comentarios = p.comentarios ?? [];
  const analitico = p.analitico ?? {};
  /** Lista plana: comentários de primeiro nível (com optional parentId da API) + respostas de respostaComentarios, com parentId preenchido. */
  const commentsFlat: PostComment[] = comentarios.flatMap((c) => {
    const parent = mapComentarioToPostComment(c, token, c.comentarioPaiId);
    const replies = (c.respostaComentarios ?? []).map((r) =>
      mapComentarioToPostComment(r, token, c.comentarioId),
    );
    return [parent, ...replies];
  });
  const commentsCount =
    analitico.quantidadeComentarios ??
    commentsFlat.length ??
    interacao.comentarios ??
    0;

  return {
    id: p.publicacaoId,
    groupId: p.grupos?.[0] ?? p.comunidadeId ?? '',
    groupName: p.grupos?.[0] ?? 'Comunicado',
    comunidadeId: p.comunidadeId ?? undefined,
    comunidadeNome: p.comunidadeNome ?? undefined,
    comunicadoId: p.comunicadoId ?? undefined,
    authorId: p.autor?.codigoColaboradorInterno ?? '',
    authorName,
    authorAvatar,
    authorArea: p.autor?.departamento ?? p.autor?.codDepartamento ?? undefined,
    type: mapTipoToPostType(p.tipo),
    title: p.titulo,
    ...(p.subtitulo != null && String(p.subtitulo).trim() !== '' ? { subtitulo: String(p.subtitulo).trim() } : {}),
    content: p.conteudo,
    ...(isAutorAlternativo ? { autoriaTipo: 'alternativo' as const } : { autoriaTipo: 'pessoal' as const }),
    attachments: (p.anexos ?? []).map((a) => mapAnexoToPostAttachment(a, token)),
    status: mapPublicacaoStatusToPostStatus(p.publicacaoStatus),
    scheduledAt: p.dataAgendamentoPublicacao ?? undefined,
    publishedAt: p.dataPublicacao ?? undefined,
    visibility: { type: 'all' },
    requiresAcknowledgment: p.requerConfirmacaoLeitura,
    allowComments: p.permiteComentarios ?? false,
    allowLikes: p.permiteCurtidas ?? false,
    isPinned: p.fixada,
    allowDownload: p.permiteDownload ?? false,
    likesCount: analitico.quantidadeCurtida ?? interacao.curtidas ?? 0,
    commentsCount,
    viewsCount: analitico.quantidadeVisualizacao ?? interacao.visualizacoes ?? 0,
    acknowledgmentCount:
      analitico.quantidadeConfirmacoesLeitura ?? interacao.confirmacoesLeitura ?? 0,
    createdAt: dataPublicacao,
    updatedAt: dataPublicacao,
    expiresAt: p.dataValidadePublicacao ?? undefined,
    acknowledgedAt,
    labels: p.labels ?? [],
    ativo: p.ativo ?? (mapPublicacaoStatusToPostStatus(p.publicacaoStatus) !== 'archived'),
    ...(p.ocultarNoFeed != null ? { ocultarNoFeed: !!p.ocultarNoFeed } : {}),
    approvalStatus: p.aprovacaoStatus,
    comments: commentsFlat,
    reactionCounts: (() => {
      if (p.interacoesPorEmoji && p.interacoesPorEmoji.length > 0) {
        return p.interacoesPorEmoji.reduce<Record<string, number>>((acc, { emoji, count }) => {
          const key = normalizarEmojiReacao(emoji) ?? emoji;
          acc[key] = (acc[key] ?? 0) + count;
          return acc;
        }, {});
      }
      if (
        typeof interacao.curtidaEmoji === 'object' &&
        interacao.curtidaEmoji !== null &&
        !Array.isArray(interacao.curtidaEmoji)
      ) {
        const raw = interacao.curtidaEmoji as Record<string, number>;
        return Object.entries(raw).reduce<Record<string, number>>((acc, [k, v]) => {
          const key = normalizarEmojiReacao(k) ?? k;
          acc[key] = (acc[key] ?? 0) + v;
          return acc;
        }, {});
      }
      return undefined;
    })(),
    meuEmojiReacao: (() => {
      const fromCurtida =
        typeof interacao.curtidaEmoji === 'string' && interacao.curtidaEmoji.trim() !== ''
          ? interacao.curtidaEmoji.trim()
          : undefined;
      const fromUsuario =
        typeof interacao.emojiReacaoUsuario === 'string' && interacao.emojiReacaoUsuario.trim() !== ''
          ? interacao.emojiReacaoUsuario.trim()
          : undefined;
      return normalizarEmojiReacao(fromCurtida ?? fromUsuario) ?? fromCurtida ?? fromUsuario;
    })(),
  };
}

/**
 * Mapeia uma publicação da API Feed para LibraryDocument (lista de documentos da biblioteca por pasta/label).
 * @param p - Item da API ObterListaPublicacaoGeral
 * @param folderPath - Nome da pasta/label que está sendo exibida
 * @param token - Token para resolver $1 em urlArquivo dos anexos
 */
export function publicacaoFeedApiToLibraryDocument(
  p: ComunicacaoFeedPublicacaoApi,
  folderPath: string,
  token?: string,
): LibraryDocument {
  const interacao = p.interacao ?? {};
  const dataPublicacao =
    p.dataPublicacao ?? p.dataAgendamentoPublicacao ?? FALLBACK_DATE;
  const firstAnexo = p.anexos?.[0];
  return {
    id: p.publicacaoId,
    postId: p.publicacaoId,
    groupId: p.grupos?.[0] ?? '',
    groupName: p.grupos?.[0] ?? 'Comunicado',
    name: p.titulo,
    authorId: p.autor?.codigoColaboradorInterno ?? '',
    authorName: p.autor?.nomeCompleto ?? '',
    type: firstAnexo ? (firstAnexo.tipo === 'imagem' ? 'image' : 'document') : 'document',
    url: firstAnexo ? resolveUrlArquivo(firstAnexo.urlArquivo, token) : '',
    size: firstAnexo?.tamanhoBytes,
    folderPath,
    requiresAcknowledgment: p.requerConfirmacaoLeitura,
    isAcknowledged: interacao.confirmouLeitura ?? false,
    createdAt: dataPublicacao,
    expiresAt: p.dataValidadePublicacao ?? undefined,
  };
}

/**
 * Mapeia CommunityPost (já convertido da API) para LibraryDocument para exibição na lista da biblioteca.
 */
export function communityPostToLibraryDocument(
  post: CommunityPost,
  folderPath: string,
): LibraryDocument {
  const firstAttachment = post.attachments?.[0];
  return {
    id: post.id,
    postId: post.id,
    groupId: post.groupId,
    groupName: post.groupName,
    name: post.title,
    authorId: post.authorId,
    authorName: post.authorName,
    type: firstAttachment?.type ?? 'document',
    url: firstAttachment?.url ?? '',
    size: firstAttachment?.size,
    folderPath,
    requiresAcknowledgment: post.requiresAcknowledgment,
    isAcknowledged: !!post.acknowledgedAt,
    createdAt: post.createdAt ?? post.publishedAt ?? FALLBACK_DATE,
    expiresAt: post.expiresAt,
    ...(post.ocultarNoFeed != null ? { ocultarNoFeed: post.ocultarNoFeed } : {}),
  };
}
