import { useState, useMemo } from 'react';
import { Search, Crown, Shield, User, Calendar } from 'lucide-react';
import type {
  CommunityGroup,
  GroupMember,
  ComunicacaoComunidadeMembro,
} from '@domain/entities/comunicacao';
import { mockMembers } from '@data/mocks/comunicacao/communityData';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

function getInitials(name: string): string {
  return name
    .split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);
}

function getMembersForGroup(group: CommunityGroup): GroupMember[] {
  const creator: GroupMember = {
    id: 'creator',
    userId: group.creatorId,
    userName: group.creatorName,
    role: 'creator',
    joinedAt: group.createdAt,
  };
  const moderatorsOnly = group.moderators.filter((m) => m.userId !== group.creatorId);
  const usedIds = new Set([group.creatorId, ...moderatorsOnly.map((m) => m.userId)]);
  const rest = mockMembers.filter((m) => !usedIds.has(m.userId));
  const members: GroupMember[] = [
    creator,
    ...moderatorsOnly.map((m) => ({ ...m, role: 'moderator' as const })),
    ...rest.slice(0, Math.max(0, group.memberCount - 1 - moderatorsOnly.length)),
  ];
  return members.slice(0, group.memberCount);
}

function apiMembersToGroupMembers(apiMembers: ComunicacaoComunidadeMembro[]): GroupMember[] {
  return apiMembers.map((m) => ({
    id: m.id,
    userId: m.codigoInternoColaborador,
    userName: m.nomeCompleto,
    role: 'member' as const,
    joinedAt: '',
  }));
}

interface CommunityMembersListProps {
  group: CommunityGroup;
  /** Membros vindos da API de detalhe da comunidade (GET .../Comunidade/{id}); quando informado, usa esta lista em vez do mock. */
  apiMembers?: ComunicacaoComunidadeMembro[] | null;
}

export function CommunityMembersList({ group, apiMembers }: CommunityMembersListProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const members = useMemo(() => {
    if (apiMembers != null) {
      return apiMembersToGroupMembers(apiMembers);
    }
    return getMembersForGroup(group);
  }, [group, apiMembers]);

  const filtered = members.filter((m) =>
    m.userName.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const roleConfig = {
    creator: {
      label: 'Criador',
      icon: Crown,
      className: 'bg-warning/15 text-warning border-warning/30',
    },
    moderator: {
      label: 'Moderador',
      icon: Shield,
      className: 'bg-primary/15 text-primary border-primary/30',
    },
    member: {
      label: 'Membro',
      icon: User,
      className: 'bg-muted text-muted-foreground border-border',
    },
  };

  return (
    <div className="space-y-4">
      <div>
        <h2 className="text-xl font-semibold">Membros da Comunidade</h2>
        <p className="text-muted-foreground text-sm mt-1">
          {members.length} membro{members.length !== 1 ? 's' : ''} na comunidade
        </p>
      </div>

      <div className="relative max-w-md">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <Input
          placeholder="Buscar membro..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="pl-10 rounded-lg"
        />
      </div>

      <div className="space-y-0 rounded-xl border border-border overflow-hidden bg-card">
        {filtered.map((member) => {
          const config = roleConfig[member.role];
          const RoleIcon = config.icon;
          const joinedDate = member.joinedAt
            ? format(new Date(member.joinedAt), "d 'de' MMMM 'de' yyyy", { locale: ptBR })
            : '';
          return (
            <div
              key={member.id}
              className="flex items-center gap-4 p-4 border-b border-border last:border-b-0"
            >
              <Avatar className="h-10 w-10">
                <AvatarFallback className="bg-gradient-to-br from-primary to-accent text-primary-foreground">
                  {getInitials(member.userName)}
                </AvatarFallback>
              </Avatar>
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2 flex-wrap">
                  <span className="font-semibold text-foreground">{member.userName}</span>
                  <Badge variant="outline" className={`text-xs gap-1 ${config.className}`}>
                    <RoleIcon className="w-3 h-3" />
                    {config.label}
                  </Badge>
                </div>
                {joinedDate && (
                  <p className="text-sm text-muted-foreground mt-0.5 flex items-center gap-1">
                    <Calendar className="w-3.5 h-3.5 shrink-0" />
                    Entrou em {joinedDate}
                  </p>
                )}
              </div>
              {member.role === 'creator' && (
                <div className="shrink-0 flex items-center gap-2 px-3 py-1.5 rounded-lg border border-border bg-muted/30 text-muted-foreground text-sm">
                  <Crown className="w-4 h-4" />
                  Não pode ser removido
                </div>
              )}
            </div>
          );
        })}
      </div>

      {filtered.length === 0 && (
        <Card>
          <CardContent className="p-8 text-center text-muted-foreground">
            Nenhum membro encontrado.
          </CardContent>
        </Card>
      )}
    </div>
  );
}
