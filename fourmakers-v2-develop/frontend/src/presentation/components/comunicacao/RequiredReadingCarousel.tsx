import { useState, useCallback } from 'react';
import {
  AlertCircle,
  FileText,
  ChevronRight,
  Megaphone,
  CheckCircle2,
} from 'lucide-react';
import type { CommunityPost, Announcement } from '@domain/entities/comunicacao';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  RequiredItemDetailModal,
  type RequiredItem,
} from './RequiredItemDetailModal';
import { Spinner } from '@/components/ui/spinner';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { parseApiDate } from '@shared/utils/dateUtils';

interface RequiredReadingCarouselProps {
  items: RequiredItem[];
  /** Itens já lidos e aceitos (ex.: retorno da API com interacao.confirmouLeitura=true). */
  acknowledgedItems?: RequiredItem[];
  /** Exibir estado de carregamento (ex.: quando os itens vêm da API com somenteLeituraObrigatoria=true). */
  loading?: boolean;
  /** Chamado após o usuário confirmar leitura (para refetch e atualizar abas Pendentes / Lidos e Aceitos). */
  onConfirmadoLeitura?: () => void;
}

export function RequiredReadingCarousel({
  items,
  acknowledgedItems = [],
  loading = false,
  onConfirmadoLeitura,
}: RequiredReadingCarouselProps) {
  const [selectedItem, setSelectedItem] = useState<RequiredItem | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [viewMode, setViewMode] = useState<'pending' | 'acknowledged'>(
    'pending',
  );

  const handleConfirmadoLeitura = useCallback(async () => {
    await onConfirmadoLeitura?.();
    setViewMode('acknowledged');
  }, [onConfirmadoLeitura]);

  const displayItems = viewMode === 'pending' ? items : acknowledgedItems;

  if (items.length === 0 && acknowledgedItems.length === 0 && !loading) return null;

  const handleOpenDetail = (item: RequiredItem) => {
    setSelectedItem(item);
    setIsDetailModalOpen(true);
  };

  return (
    <div className="mb-6 transition-opacity min-w-0">
      <Card
        className={`rounded-2xl !shadow-none hover:!shadow-none min-w-0 overflow-hidden ${
          viewMode === 'pending'
            ? 'border-warning/50 bg-[#FDF5ED] dark:bg-amber-500/15 dark:border-warning/40'
            : 'border-success/40 bg-[#F0F9F5] dark:bg-emerald-500/15 dark:border-success/40'
        }`}
      >
        <CardContent className="p-3 sm:p-4">
          <div className="flex flex-col gap-3 mb-3 min-w-0">
            <div className="flex items-center gap-2 min-w-0">
              {viewMode === 'pending' ? (
                <AlertCircle className="w-5 h-5 flex-shrink-0 text-warning dark:text-amber-400" />
              ) : (
                <CheckCircle2 className="w-5 h-5 flex-shrink-0 text-success dark:text-emerald-400" />
              )}
              <h3
                className={`font-semibold text-sm sm:text-base break-words ${viewMode === 'pending' ? 'text-warning dark:text-amber-400' : 'text-success dark:text-emerald-400'}`}
              >
                Documentos/Comunicados - Leitura Obrigatória
              </h3>
            </div>

            {/* No mobile: botões em coluna (cada um 100% largura). A partir de sm: linha única com pill */}
            <div className="flex flex-col sm:flex-row gap-2 sm:gap-2 bg-muted/30 backdrop-blur-sm border border-border/50 p-2 sm:p-1.5 rounded-lgToken sm:rounded-pillToken w-full min-w-0">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setViewMode('pending')}
                className={`w-full sm:w-auto justify-center sm:inline-flex gap-1.5 sm:gap-2 rounded-lgToken sm:rounded-pillToken text-xs sm:text-sm font-medium py-2.5 sm:py-2.5 px-4 sm:px-5 transition-all duration-200 flex-shrink-0 ${
                  viewMode === 'pending'
                    ? 'bg-warning hover:bg-warning/90 text-white border-transparent dark:bg-amber-500 dark:hover:bg-amber-600 dark:text-white'
                    : 'text-muted-foreground hover:text-foreground hover:bg-muted/50 dark:text-muted-foreground dark:hover:bg-muted/50'
                }`}
              >
                <AlertCircle className="w-3.5 h-3.5 sm:w-4 sm:h-4 flex-shrink-0" />
                Pendentes
                <Badge variant="secondary" className="ml-0.5 sm:ml-1 text-xs flex-shrink-0">
                  {items.length}
                </Badge>
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setViewMode('acknowledged')}
                className={`w-full sm:w-auto justify-center sm:inline-flex gap-1.5 sm:gap-2 rounded-lgToken sm:rounded-pillToken text-xs sm:text-sm font-medium py-2.5 sm:py-2.5 px-4 sm:px-5 transition-all duration-200 flex-shrink-0 ${
                  viewMode === 'acknowledged'
                    ? 'bg-success hover:bg-success/90 text-white border-transparent dark:bg-emerald-600 dark:hover:bg-emerald-700 dark:text-white'
                    : 'text-muted-foreground hover:text-foreground hover:bg-muted/50 dark:text-muted-foreground dark:hover:bg-muted/50'
                }`}
              >
                <CheckCircle2 className="w-3.5 h-3.5 sm:w-4 sm:h-4 flex-shrink-0" />
                Lidos e Aceitos
                <Badge variant="secondary" className="ml-0.5 sm:ml-1 text-xs flex-shrink-0">
                  {acknowledgedItems.length}
                </Badge>
              </Button>
            </div>
          </div>

          <div className="w-full overflow-x-auto pb-2 pt-1">
            <div className="flex gap-3 min-w-0 py-2">
              {loading && viewMode === 'pending' ? (
                <div className="w-full flex justify-center py-8 min-w-full">
                  <Spinner className="h-8 w-8 text-muted-foreground" />
                </div>
              ) : (viewMode === 'acknowledged' && loading) ? (
                <div className="w-full flex justify-center py-8 min-w-full">
                  <Spinner className="h-8 w-8 text-muted-foreground" />
                </div>
              ) : displayItems.length === 0 ? (
                <div className="w-full text-center py-8 min-w-full">
                  <p className="text-muted-foreground">
                    {viewMode === 'pending'
                      ? 'Nenhuma leitura obrigatória pendente! 🎉'
                      : 'Nenhum item aceito ainda.'}
                  </p>
                </div>
              ) : (
                displayItems.map((item: RequiredItem) => {
                  const isAnnouncement = item.itemType === 'announcement';
                  const isAcknowledged =
                    viewMode === 'acknowledged' && 'acknowledgedAt' in item;
                  const title = item.title;
                  const authorName = item.authorName;
                  const groupName = isAnnouncement
                    ? (item as Announcement).targetUserGroupNames[0] ||
                      'Comunicado'
                    : (item as CommunityPost).groupName;

                  return (
                    <div
                      key={item.id}
                      className="flex-shrink-0 w-[280px] transition-transform hover:scale-[1.02]"
                    >
                      <Card
                        className={`h-full rounded-lg overflow-visible bg-surfaceElevated dark:bg-surfaceElevated transition-all cursor-pointer border-2 !shadow-none hover:!shadow-none ${
                          viewMode === 'pending'
                            ? 'border-warning/30 hover:border-warning dark:border-warning/40 dark:hover:border-warning/60'
                            : 'border-success/30 hover:border-success dark:border-success/40 dark:hover:border-success/60'
                        }`}
                        onClick={() => handleOpenDetail(item as RequiredItem)}
                      >
                        <CardContent className="p-3">
                          <div className="flex items-start gap-3">
                            <div
                              className={`p-2 rounded-lg ${viewMode === 'pending' ? 'bg-warning/10 dark:bg-warning/20' : 'bg-success/10 dark:bg-success/20'}`}
                            >
                              {isAnnouncement ? (
                                (item as Announcement).announcementType ===
                                'document' ? (
                                  <FileText
                                    className={`w-5 h-5 ${viewMode === 'pending' ? 'text-warning dark:text-amber-400' : 'text-success dark:text-emerald-400'}`}
                                  />
                                ) : (
                                  <Megaphone
                                    className={`w-5 h-5 ${viewMode === 'pending' ? 'text-warning dark:text-amber-400' : 'text-success dark:text-emerald-400'}`}
                                  />
                                )
                              ) : (
                                <FileText
                                  className={`w-5 h-5 ${viewMode === 'pending' ? 'text-warning dark:text-amber-400' : 'text-success dark:text-emerald-400'}`}
                                />
                              )}
                            </div>
                            <div className="flex-1 min-w-0">
                              <div className="flex items-start gap-2">
                                <p className="font-medium text-sm truncate flex-1 text-foreground">
                                  {title}
                                </p>
                                {isAnnouncement && (
                                  <Badge
                                    variant="secondary"
                                    className="text-xs flex-shrink-0"
                                  >
                                    {(item as Announcement).announcementType ===
                                    'document'
                                      ? '📄'
                                      : '📰'}
                                  </Badge>
                                )}
                              </div>
                              <p className="text-xs text-muted-foreground mt-0.5">
                                {groupName}
                              </p>
                              <p className="text-xs text-muted-foreground">
                                Por {authorName}
                              </p>
                              {isAcknowledged &&
                                'acknowledgedAt' in item && (
                                  <div className="flex items-center gap-1 mt-1">
                                    <CheckCircle2 className="w-3 h-3 text-success dark:text-emerald-400" />
                                    <p className="text-xs text-success dark:text-emerald-400">
                                      Aceito em{' '}
                                      {(() => {
                                        const at = (item as { acknowledgedAt: string }).acknowledgedAt;
                                        const d = parseApiDate(at);
                                        return d ? format(d, "dd/MM/yyyy 'às' HH:mm", { locale: ptBR }) : '—';
                                      })()}
                                    </p>
                                  </div>
                                )}
                            </div>
                          </div>
                          {viewMode === 'pending' ? (
                            <Button
                              variant="ghost"
                              size="sm"
                              className="w-full mt-3 text-warning hover:text-warning hover:bg-warning/10 dark:text-amber-400 dark:hover:text-amber-300 dark:hover:bg-warning/20"
                            >
                              Ler e aceitar
                              <ChevronRight className="w-4 h-4 ml-1" />
                            </Button>
                          ) : (
                            <Button
                              variant="ghost"
                              size="sm"
                              className="w-full mt-3 text-success hover:text-success hover:bg-success/10 dark:text-emerald-400 dark:hover:text-emerald-300 dark:hover:bg-success/20"
                            >
                              Ver detalhes
                              <ChevronRight className="w-4 h-4 ml-1" />
                            </Button>
                          )}
                        </CardContent>
                      </Card>
                    </div>
                  );
                })
              )}
            </div>
          </div>
        </CardContent>
      </Card>

      <RequiredItemDetailModal
        open={isDetailModalOpen}
        onOpenChange={setIsDetailModalOpen}
        item={selectedItem}
        isAlreadyAcknowledged={viewMode === 'acknowledged'}
        acknowledgedAt={
          viewMode === 'acknowledged' &&
          selectedItem &&
          'acknowledgedAt' in selectedItem
            ? (selectedItem as { acknowledgedAt: string }).acknowledgedAt
            : undefined
        }
        onConfirmadoLeitura={handleConfirmadoLeitura}
        hideVerAceites
      />
    </div>
  );
}
