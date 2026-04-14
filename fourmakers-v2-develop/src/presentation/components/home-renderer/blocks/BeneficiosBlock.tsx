import { useState, useMemo, useRef, useEffect, useCallback } from 'react';
import { container } from 'tsyringe';
import { Search, Star, Gift } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { BeneficioXanoApi } from '@data/api/BeneficioXanoApi';
import type { BeneficioXanoListItem } from '@domain/entities/BeneficioXano';
import type { BeneficiosContent } from '@shared/types/homeBuilder';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Spinner } from '@/components/ui/spinner';

const PAGE_SIZE = 9;
const HEIGHT_MAP: Record<string, string> = {
  sm: 'h-[390px]',
  md: 'h-[580px]',
  lg: 'h-[770px]',
};

/** orgId padrão quando usuário não está logado (endpoint é público). */
const DEFAULT_ORG_ID = 2;

function getDescriptionSnippet(item: BeneficioXanoListItem): string {
  if (item.perguntas?.length) {
    const first = item.perguntas.find((p) => p.pergunta && p.resposta);
    if (first) return first.resposta || '';
  }
  if (item.descricao?.length) {
    const first = item.descricao.find((d) => d.resposta);
    return first?.resposta || '';
  }
  return '';
}

const BADGE_CLASS = 'bg-muted text-muted-foreground border border-border/60';

interface BeneficiosBlockProps {
  content: BeneficiosContent;
}

export function BeneficiosBlock({ content }: BeneficiosBlockProps) {
  const user = useAppSelector((state) => state.auth.user);
  const navigate = useNavigate();
  const orgId = user?.colaboradorOrg?.orgId ?? DEFAULT_ORG_ID;
  const userId = user?.colaborador?.codigoColaboradorInterno ?? '';

  const heightClass = HEIGHT_MAP[content.height] ?? HEIGHT_MAP.md;
  const api = container.resolve<BeneficioXanoApi>(DiTokens.beneficioXanoApi);

  const [items, setItems] = useState<BeneficioXanoListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [visibleCount, setVisibleCount] = useState(PAGE_SIZE);
  const scrollRef = useRef<HTMLDivElement>(null);
  const sentinelRef = useRef<HTMLDivElement>(null);

  const loadBeneficios = useCallback(async () => {
    setLoading(true);
    try {
      const list = await api.listar(orgId, userId);
      setItems(list ?? []);
    } catch {
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, [api, orgId, userId]);

  useEffect(() => {
    loadBeneficios();
  }, [loadBeneficios]);

  const filtered = useMemo(() => {
    if (!searchTerm.trim()) return items;
    const q = searchTerm.toLowerCase();
    return items.filter(
      (b) =>
        b.nome.toLowerCase().includes(q) ||
        (b.segmento && b.segmento.toLowerCase().includes(q)) ||
        (b.tipo && b.tipo.toLowerCase().includes(q)),
    );
  }, [items, searchTerm]);

  useEffect(() => {
    setVisibleCount(PAGE_SIZE);
  }, [searchTerm]);

  const visible = useMemo(() => filtered.slice(0, visibleCount), [filtered, visibleCount]);
  const hasMore = visibleCount < filtered.length;

  useEffect(() => {
    if (!hasMore || !sentinelRef.current || !scrollRef.current) return;
    const root = scrollRef.current;
    const el = sentinelRef.current;
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0]?.isIntersecting) {
          setVisibleCount((prev) => Math.min(prev + PAGE_SIZE, filtered.length));
        }
      },
      { root, rootMargin: '120px', threshold: 0 },
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [hasMore, filtered.length]);

  return (
    <Card className="shadow-softToken rounded-lgToken w-full min-w-0 flex flex-col overflow-hidden">
      <CardHeader className="pb-3 flex-shrink-0 p-4">
        <div className="flex flex-col gap-2">
          {content.title && (
            <CardTitle className="flex items-center gap-2 text-sm font-semibold text-primaryText">
              <div className="w-6 h-6 rounded-smToken flex items-center justify-center flex-shrink-0 bg-primarySoft">
                <Gift className="w-3.5 h-3.5 text-primary" />
              </div>
              <span className="truncate">{content.title}</span>
            </CardTitle>
          )}
          <div className="relative">
            <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
            <Input
              type="search"
              placeholder="Buscar benefícios..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="h-8 pl-8 text-xs rounded-lg border-border/50 bg-muted/20"
            />
          </div>
        </div>
      </CardHeader>

      <CardContent className="p-0 flex-1 min-h-0">
        <div
          ref={scrollRef}
          className={`${heightClass} overflow-y-auto`}
          style={{ scrollbarWidth: 'thin' }}
        >
          {loading ? (
            <div className="flex items-center justify-center h-full">
              <Spinner className="h-6 w-6 text-muted-foreground" />
            </div>
          ) : filtered.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full gap-2 text-muted-foreground">
              <Gift className="w-8 h-8 opacity-40" />
              <p className="text-sm">Nenhum benefício encontrado</p>
            </div>
          ) : (
            <div className="p-3">
              <div className="grid grid-cols-2 lg:grid-cols-3 gap-3">
                {visible.map((item) => {
                  const snippet = getDescriptionSnippet(item);
                  const media = typeof item.media === 'number' ? item.media : 0;

                  return (
                    <div
                      key={item.id}
                      className="relative flex flex-col rounded-lg border border-border/50 bg-background hover:border-primary/40 hover:shadow-softToken transition-all overflow-hidden"
                    >
                      {/* Tag tipo — canto superior direito */}
                      <span
                        className={`absolute top-2.5 right-2.5 z-10 inline-flex items-center text-[10px] font-medium px-2 py-0.5 rounded-full ${BADGE_CLASS}`}
                      >
                        {item.tipo || 'Benefício'}
                      </span>

                      {/* Cabeçalho: logo + nome */}
                      <div className="flex items-center gap-3 p-3 pr-[72px] border-b border-border/40">
                        {/* Logo — sem border-radius */}
                        <div className="w-14 h-14 bg-muted/40 flex items-center justify-center overflow-hidden flex-shrink-0">
                          {item.miniatura?.url ? (
                            <img
                              src={item.miniatura.url}
                              alt={item.nome}
                              className="w-full h-full object-contain p-1.5"
                            />
                          ) : (
                            <Gift className="w-6 h-6 text-muted-foreground/40" />
                          )}
                        </div>
                        <p className="text-sm font-semibold text-primaryText leading-snug line-clamp-3">
                          {item.nome}
                        </p>
                      </div>

                      {/* Corpo */}
                      <div className="p-3 flex flex-col flex-1 gap-2">
                        {/* Descrição snippet — máx 3 linhas */}
                        {snippet && (
                          <p className="text-xs text-muted-foreground line-clamp-3 flex-1">
                            {snippet}
                          </p>
                        )}

                        {/* Avaliação */}
                        {media > 0 && (
                          <div className="flex items-center gap-1 text-xs text-amber-500">
                            <Star className="w-3 h-3 fill-current" />
                            <span className="font-medium">{media.toFixed(1)}</span>
                          </div>
                        )}

                        <Button
                          size="sm"
                          className="w-full mt-auto py-2"
                          onClick={() => navigate(`/beneficio/${item.id}`)}
                        >
                          Saiba mais
                        </Button>
                      </div>
                    </div>
                  );
                })}
              </div>

              {hasMore && (
                <div ref={sentinelRef} className="flex justify-center py-3">
                  <Spinner className="h-5 w-5 text-muted-foreground" />
                </div>
              )}

              <p className="text-center text-[10px] text-muted-foreground mt-2 pb-1">
                {visible.length} de {filtered.length} benefício{filtered.length !== 1 ? 's' : ''}
              </p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
