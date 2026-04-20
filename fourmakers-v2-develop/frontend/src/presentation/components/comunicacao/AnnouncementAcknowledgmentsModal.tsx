import { useState } from 'react';
import { X, Users, Search } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { DataTable } from '@presentation/components/common';
import type { ConfirmacaoLeitura } from '@domain/entities/comunicacao';

function getIniciais(name: string): string {
  const parts = String(name).trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
}

interface AnnouncementAcknowledgmentsModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  announcementTitle: string;
  requiresAcknowledgment: boolean;
  totalRecipients: number;
  acknowledgments: ConfirmacaoLeitura[];
}

export function AnnouncementAcknowledgmentsModal({
  open,
  onOpenChange,
  announcementTitle,
  acknowledgments,
}: AnnouncementAcknowledgmentsModalProps) {
  const [searchTerm, setSearchTerm] = useState('');

  const filteredAcknowledgments = acknowledgments.filter((user) =>
    user.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Users className="w-5 h-5" />
            Visualizações e Aceites
          </DialogTitle>
        </DialogHeader>

        <div className="flex-1 overflow-hidden flex flex-col">
          <div className="mb-4">
            <p className="text-sm text-muted-foreground">Comunicado:</p>
            <p className="font-semibold">{announcementTitle}</p>
          </div>

          <div className="flex items-center gap-2 mb-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
              <Input
                placeholder="Buscar por nome..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-9 rounded-lg"
              />
            </div>
          </div>

          <div className="flex-1 min-h-0 overflow-hidden">
            <DataTable<ConfirmacaoLeitura>
              columns={[
                { id: 'foto', label: '', sortable: false, width: 'w-[52px]' },
                { id: 'name', label: 'Nome completo', sortable: true },
                { id: 'email', label: 'E-mail', sortable: true },
                { id: 'cargo', label: 'Cargo', sortable: true },
                { id: 'unidade', label: 'Unidade', sortable: true },
              ]}
              data={filteredAcknowledgments}
              keyExtractor={(u) => u.id}
              renderCell={(row, columnId) => {
                if (columnId === 'foto') {
                  return (
                    <Avatar className="h-9 w-9 shrink-0">
                      {row.urlFoto ? (
                        <AvatarImage src={row.urlFoto} alt={row.name} className="object-cover" />
                      ) : null}
                      <AvatarFallback className="bg-primary/10 text-primary text-sm font-medium">
                        {getIniciais(row.name)}
                      </AvatarFallback>
                    </Avatar>
                  );
                }
                if (columnId === 'name') return <span className="font-medium">{row.name}</span>;
                if (columnId === 'email') return <span className="text-sm text-muted-foreground">{row.email ?? '—'}</span>;
                if (columnId === 'cargo') return <span className="text-sm">{row.cargo ?? '—'}</span>;
                if (columnId === 'unidade') return <span className="text-sm">{row.unidade ?? '—'}</span>;
                return null;
              }}
              emptyMessage="Nenhum usuário encontrado."
              stickyHeader
            />
          </div>
        </div>

        <div className="flex justify-end pt-4 border-t">
          <Button variant="ghost" onClick={() => onOpenChange(false)}>
            <X className="w-4 h-4 mr-2" />
            Fechar
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
