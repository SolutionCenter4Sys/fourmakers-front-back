import { useState } from 'react';
import { Megaphone, Plus, Search, Calendar } from 'lucide-react';
import type { Announcement, CommunityGroup } from '@domain/entities/comunicacao';
import { mockAnnouncements } from '@data/mocks/comunicacao/announcementsData';
import { AnnouncementCard } from './AnnouncementCard';
import { CreateAnnouncementModal } from './CreateAnnouncementModal';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

interface CommunityAnnouncementsListProps {
  group: CommunityGroup;
  announcements?: Announcement[];
  setAnnouncements?: React.Dispatch<React.SetStateAction<Announcement[]>>;
}

export function CommunityAnnouncementsList({
  group,
  announcements: controlledAnnouncements,
  setAnnouncements,
}: CommunityAnnouncementsListProps) {
  const [localAnnouncements, setLocalAnnouncements] = useState<Announcement[]>([]);
  const setAnnouncementsOrLocal = setAnnouncements ?? setLocalAnnouncements;
  const effectiveList = setAnnouncements
    ? (controlledAnnouncements ?? [])
    : [...mockAnnouncements, ...localAnnouncements];

  const [searchTerm, setSearchTerm] = useState('');
  const [dateStart, setDateStart] = useState('');
  const [dateEnd, setDateEnd] = useState('');
  const [monthYear, setMonthYear] = useState('');
  const [isCreateOpen, setIsCreateOpen] = useState(false);

  const linkedIds = new Set(group.linkedUserGroups || []);
  const filteredByGroup = effectiveList.filter(
    (a) => a.targetUserGroupIds.some((id) => linkedIds.has(id)),
  );

  let list = filteredByGroup.filter(
    (a) =>
      a.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      a.content.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  if (dateStart) {
    list = list.filter((a) => a.publishedAt && a.publishedAt >= `${dateStart}T00:00:00`);
  }
  if (dateEnd) {
    list = list.filter((a) => a.publishedAt && a.publishedAt <= `${dateEnd}T23:59:59`);
  }
  if (monthYear) {
    const [month, year] = monthYear.split('/').map(Number);
    if (month && year) {
      list = list.filter((a) => {
        if (!a.publishedAt) return false;
        const d = new Date(a.publishedAt);
        return d.getMonth() + 1 === month && d.getFullYear() === year;
      });
    }
  }

  const published = list.filter((a) => a.status === 'published');

  const handleSave = (announcement: Announcement) => {
    setAnnouncementsOrLocal((prev) => [announcement, ...prev]);
    setIsCreateOpen(false);
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-xl font-semibold flex items-center gap-2">
            <Megaphone className="w-5 h-5" />
            Comunicados
          </h2>
          <p className="text-muted-foreground text-sm mt-1">
            Comunicados diretos para grupos de trabalho
          </p>
        </div>
        <Button className="gap-2 shrink-0" onClick={() => setIsCreateOpen(true)}>
          <Plus className="w-4 h-4" />
          Novo comunicado
        </Button>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Buscar comunicado..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10 rounded-lg"
          />
        </div>
        <div className="relative">
          <Calendar className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
          <Input
            placeholder="dd/mm/aaaa"
            value={dateStart}
            onChange={(e) => setDateStart(e.target.value)}
            className="rounded-lg"
          />
        </div>
        <div className="relative">
          <Calendar className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
          <Input
            placeholder="dd/mm/aaaa"
            value={dateEnd}
            onChange={(e) => setDateEnd(e.target.value)}
            className="rounded-lg"
          />
        </div>
        <div className="relative">
          <Calendar className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
          <Input
            placeholder="Mês/Ano"
            value={monthYear}
            onChange={(e) => setMonthYear(e.target.value)}
            className="rounded-lg"
          />
        </div>
      </div>

      <div className="space-y-4">
        {published.length === 0 ? (
          <div className="rounded-xl border border-dashed border-border p-8 text-center text-muted-foreground">
            <Megaphone className="w-12 h-12 mx-auto mb-3 opacity-50" />
            <p>
              {filteredByGroup.length === 0
                ? 'Nenhum comunicado para os grupos desta comunidade.'
                : 'Nenhum comunicado encontrado com os filtros aplicados.'}
            </p>
            <Button
              variant="outline"
              className="mt-4 gap-2"
              onClick={() => setIsCreateOpen(true)}
            >
              <Plus className="w-4 h-4" />
              Novo comunicado
            </Button>
          </div>
        ) : (
          published.map((ann) => (
            <AnnouncementCard key={ann.id} announcement={ann} />
          ))
        )}
      </div>

      <CreateAnnouncementModal
        open={isCreateOpen}
        onOpenChange={setIsCreateOpen}
        onSave={handleSave}
      />
    </div>
  );
}
