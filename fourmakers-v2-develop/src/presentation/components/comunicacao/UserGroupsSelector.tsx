import { Users } from 'lucide-react';
import type { ComunicacaoGrupo } from '@domain/entities/comunicacao';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Spinner } from '@/components/ui/spinner';
import { cn } from '@/lib/utils';

export interface UserGroupsSelectorProps {
  grupos: ComunicacaoGrupo[];
  loading: boolean;
  error: string | null;
  selectedIds: string[];
  onToggle: (groupId: string) => void;
  /** Título da seção (ex.: "Grupos de Usuários") */
  title?: string;
  /** Texto de ajuda abaixo do título */
  description?: string;
  /** Se true, exibe badge "X grupo(s) • Y pessoas" no label */
  showTotalMembers?: boolean;
  /** Classe adicional no container */
  className?: string;
  /** Quando true, não permite alterar a seleção (ex.: comunicado oculto no feed) */
  disabled?: boolean;
}

export function UserGroupsSelector({
  grupos,
  loading,
  error,
  selectedIds,
  onToggle,
  title = 'Grupos de Usuários',
  description,
  showTotalMembers = true,
  className,
  disabled = false,
}: UserGroupsSelectorProps) {
  const totalMembers = selectedIds.reduce(
    (sum, id) => sum + (grupos.find((g) => g.id === id)?.quantidadeParticipantes ?? 0),
    0,
  );

  return (
    <div
      className={cn(className, disabled && 'opacity-60')}
      aria-disabled={disabled || undefined}
    >
      <div className="space-y-2">
        <Label className="flex items-center justify-between gap-2">
          <span className="flex items-center gap-2 text-muted-foreground">
            <Users className="w-4 h-4" />
            {title}
          </span>
          {showTotalMembers && (
            <Badge variant="secondary" className="rounded-lg shrink-0">
              {selectedIds.length} grupo(s) • {totalMembers} pessoas
            </Badge>
          )}
        </Label>
        {description && (
          <p className="text-xs text-muted-foreground">{description}</p>
        )}
        <Card className="rounded-lg">
          <CardContent className="p-4 space-y-2">
            {loading ? (
              <div className="flex items-center justify-center gap-2 py-6 text-sm text-muted-foreground">
                <Spinner className="h-4 w-4" />
                Carregando grupos...
              </div>
            ) : error ? (
              <div className="text-center p-4 text-sm text-destructive">
                {error}
              </div>
            ) : grupos.length === 0 ? (
              <div className="text-center p-4 text-sm text-muted-foreground">
                Nenhum grupo de usuários disponível
              </div>
            ) : (
              grupos.map((grupo) => (
                <div
                  key={grupo.id}
                  className={cn(
                    'flex items-center gap-3 p-2 rounded-lg transition-colors',
                    disabled
                      ? 'cursor-not-allowed'
                      : 'cursor-pointer hover:bg-muted/50',
                  )}
                  onClick={() => {
                    if (!disabled) onToggle(grupo.id);
                  }}
                >
                  <Checkbox
                    id={`ug-selector-${grupo.id}`}
                    checked={selectedIds.includes(grupo.id)}
                    disabled={disabled}
                    onCheckedChange={() => {
                      if (!disabled) onToggle(grupo.id);
                    }}
                    onClick={(e) => e.stopPropagation()}
                  />
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium">{grupo.nome}</p>
                    <p className="text-xs text-muted-foreground">
                      {grupo.quantidadeParticipantes ?? 0}{' '}
                      {(grupo.quantidadeParticipantes ?? 0) === 1 ? 'pessoa' : 'pessoas'}
                    </p>
                  </div>
                </div>
              ))
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
