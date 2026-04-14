import type { Announcement, CommunityPost } from '@domain/entities/comunicacao';

/**
 * Mapeia uma publicação do feed (CommunityPost) para Announcement.
 * Usado na tab Comunicados, que consome o mesmo endpoint ObterListaPublicacaoGeral.
 */
export function communityPostToAnnouncement(post: CommunityPost): Announcement {
  const status =
    (post.approvalStatus ?? '').toLowerCase() === 'pendente'
      ? ('pending_approval' as const)
      : mapPostStatusToAnnouncementStatus(post.status);
  const ativo = post.ativo ?? (post.status !== 'archived');
  return {
    id: post.id,
    targetUserGroupIds: post.groupId ? [post.groupId] : [],
    targetUserGroupNames: post.groupName ? [post.groupName] : [],
    authorId: post.authorId,
    authorName: post.authorName,
    authorAvatar: post.authorAvatar,
    authorArea: post.authorArea,
    announcementType: 'informative',
    title: post.title,
    ...(post.subtitulo != null && post.subtitulo.trim() !== '' ? { subtitulo: post.subtitulo.trim() } : {}),
    ...(post.autoriaTipo ? { autoriaTipo: post.autoriaTipo } : {}),
    content: post.content,
    attachments: post.attachments ?? [],
    status,
    scheduledAt: post.scheduledAt,
    publishedAt: post.publishedAt,
    createdAt: post.createdAt,
    updatedAt: post.updatedAt,
    requiresAcknowledgment: post.requiresAcknowledgment,
    allowComments: post.allowComments,
    allowLikes: post.allowLikes,
    isPinned: post.isPinned,
    allowDownload: post.allowDownload,
    likesCount: post.likesCount ?? 0,
    commentsCount: post.commentsCount ?? 0,
    viewsCount: post.viewsCount ?? 0,
    acknowledgmentCount: post.acknowledgmentCount ?? 0,
    ativo,
    ...(post.ocultarNoFeed != null ? { ocultarNoFeed: !!post.ocultarNoFeed } : {}),
    ...(post.groupId ? { comunidadeId: post.groupId } : {}),
    ...(post.comunicadoId != null ? { comunicadoId: post.comunicadoId } : {}),
  };
}

function mapPostStatusToAnnouncementStatus(
  status: CommunityPost['status'],
): Announcement['status'] {
  if (status === 'draft') return 'draft';
  if (status === 'scheduled') return 'scheduled';
  if (status === 'published') return 'published';
  if (status === 'archived') return 'archived';
  return 'published';
}
