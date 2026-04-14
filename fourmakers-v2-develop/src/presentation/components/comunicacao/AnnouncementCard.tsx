import {
  Pin,
  FileText,
  User,
  Eye,
  Heart,
  MessageCircle,
  CheckCircle2,
} from 'lucide-react';
import type { Announcement } from '@domain/entities/comunicacao';
import { Card, CardContent } from '@/components/ui/card';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

function getInitials(name: string): string {
  return name.split(' ').map((n) => n[0]).join('').toUpperCase().slice(0, 2);
}

function stripHtml(html: string): string {
  const tmp = document.createElement('div');
  tmp.innerHTML = html;
  return tmp.textContent || tmp.innerText || '';
}

function formatSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  return `${(bytes / 1024).toFixed(0)} KB`;
}

interface AnnouncementCardProps {
  announcement: Announcement;
}

export function AnnouncementCard({ announcement }: AnnouncementCardProps) {
  const publishedAt = announcement.publishedAt
    ? format(new Date(announcement.publishedAt), "d 'de' MMMM 'às' HH:mm", { locale: ptBR })
    : '';

  return (
    <Card className="overflow-hidden rounded-xl border border-border shadow-sm">
      <CardContent className="p-4 space-y-4">
        <div className="flex items-start gap-3 flex-wrap">
          <Avatar className="h-10 w-10 rounded-full shrink-0 bg-primary/10 text-primary">
            {announcement.authorAvatar ? (
              <AvatarImage src={announcement.authorAvatar} alt={announcement.authorName} />
            ) : null}
            <AvatarFallback className="text-sm bg-primary/10 text-primary">
              {getInitials(announcement.authorName)}
            </AvatarFallback>
          </Avatar>
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2 flex-wrap">
              <span className="font-semibold text-foreground">{announcement.authorName}</span>
              {announcement.isPinned && (
                <Badge variant="outline" className="text-destructive border-destructive/50 bg-destructive/10 gap-1">
                  <Pin className="w-3 h-3" />
                  Fixado
                </Badge>
              )}
              {announcement.status === 'published' && (
                <Badge variant="outline" className="bg-success/10 text-success border-success/30">
                  Publicado
                </Badge>
              )}
            </div>
            {publishedAt && (
              <p className="text-xs text-muted-foreground mt-0.5">Publicado {publishedAt}</p>
            )}
          </div>
        </div>

        <h3 className="font-semibold text-lg text-foreground leading-tight">
          {announcement.title}
        </h3>

        {announcement.targetUserGroupNames.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {announcement.targetUserGroupNames.map((name) => (
              <Badge key={name} variant="secondary" className="gap-1">
                <User className="w-3 h-3" />
                {name}
              </Badge>
            ))}
          </div>
        )}

        <p className="text-sm text-muted-foreground line-clamp-4">
          {stripHtml(announcement.content).slice(0, 400)}
          {stripHtml(announcement.content).length > 400 ? '...' : ''}
        </p>

        {announcement.tags && announcement.tags.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {announcement.tags.map((tag) => (
              <Badge key={tag} variant="outline" className="gap-1 text-muted-foreground">
                <Eye className="w-3 h-3" />
                {tag}
              </Badge>
            ))}
          </div>
        )}

        {announcement.attachments.length > 0 && (
          <div className="space-y-2">
            <p className="text-sm font-medium flex items-center gap-2">
              <FileText className="w-4 h-4" />
              {announcement.attachments.length} Documento{announcement.attachments.length !== 1 ? 's' : ''} Anexo{announcement.attachments.length !== 1 ? 's' : ''}
            </p>
            <ul className="space-y-1">
              {announcement.attachments.map((att) => (
                <li key={att.id} className="flex items-center gap-2 text-sm text-muted-foreground">
                  <FileText className="w-4 h-4 shrink-0" />
                  {att.name}
                  {att.size != null && (
                    <span className="text-xs">({formatSize(att.size)})</span>
                  )}
                </li>
              ))}
            </ul>
          </div>
        )}

        <div className="flex flex-wrap gap-4 pt-2 border-t border-border text-sm text-muted-foreground">
          {announcement.requiresAcknowledgment && (
            <span className="flex items-center gap-1">
              <CheckCircle2 className="w-4 h-4" />
              {announcement.acknowledgmentCount} confirmações
            </span>
          )}
          <span className="flex items-center gap-1">
            <Eye className="w-4 h-4" />
            {announcement.viewsCount}
          </span>
          <span className="flex items-center gap-1">
            <Heart className="w-4 h-4" />
            {announcement.likesCount}
          </span>
          <span className="flex items-center gap-1">
            <MessageCircle className="w-4 h-4" />
            {announcement.commentsCount}
          </span>
        </div>
      </CardContent>
    </Card>
  );
}
