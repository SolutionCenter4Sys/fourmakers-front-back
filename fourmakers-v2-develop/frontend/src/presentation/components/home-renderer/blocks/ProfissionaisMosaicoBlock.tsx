import { useState, useMemo, useRef, useEffect, useCallback } from 'react';
import { container } from 'tsyringe';
import { Search, Star, Briefcase, MapPin, Cake, Users } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { Professional } from '@domain/entities/comunicacao';
import type { ComunicacaoProfissionaisApi } from '@data/api/ComunicacaoProfissionaisApi';
import type { ProfissionaisMosaicoContent } from '@shared/types/homeBuilder';
import { ListarComunicacaoProfissionaisUseCase } from '@domain/usecases/ListarComunicacaoProfissionaisUseCase';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Spinner } from '@/components/ui/spinner';
import { toast } from 'sonner';

const PAGE_SIZE = 9;

const HEIGHT_MAP: Record<string, string> = {
  sm: 'h-[390px]',
  md: 'h-[580px]',
  lg: 'h-[770px]',
};

function getInitials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return '?';
  if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
}

const isBirthday = (birthDate?: string): boolean => {
  if (!birthDate) return false;
  const parts = birthDate.split('-').map(Number);
  const today = new Date();
  const month = parts.length === 2 ? parts[0] : parts[1];
  const day = parts.length === 2 ? parts[1] : parts[2];
  return today.getMonth() + 1 === month && today.getDate() === day;
};

interface ProfissionaisMosaicoBlockProps {
  content: ProfissionaisMosaicoContent;
}

export function ProfissionaisMosaicoBlock({ content }: ProfissionaisMosaicoBlockProps) {
  const token = useAppSelector((state) => state.auth.token);
  const navigate = useNavigate();
  const heightClass = HEIGHT_MAP[content.height] ?? HEIGHT_MAP.md;

  const listarUseCase = container.resolve(ListarComunicacaoProfissionaisUseCase);

  const [profissionais, setProfissionais] = useState<Professional[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [visibleCount, setVisibleCount] = useState(PAGE_SIZE);
  const [togglingId, setTogglingId] = useState<string | null>(null);

  const scrollRef = useRef<HTMLDivElement>(null);
  const sentinelRef = useRef<HTMLDivElement>(null);

  const loadProfissionais = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    try {
      const result = await listarUseCase.execute(token);
      setProfissionais(result ?? []);
    } catch {
      setProfissionais([]);
    } finally {
      setLoading(false);
    }
  }, [token, listarUseCase]);

  useEffect(() => {
    loadProfissionais();
  }, [loadProfissionais]);

  // Favoritados primeiro, depois por nome
  const sorted = useMemo(() => {
    return [...profissionais].sort((a, b) => {
      if (a.favoritado && !b.favoritado) return -1;
      if (!a.favoritado && b.favoritado) return 1;
      return a.name.localeCompare(b.name, 'pt-BR');
    });
  }, [profissionais]);

  const filtered = useMemo(() => {
    if (!searchTerm.trim()) return sorted;
    const q = searchTerm.toLowerCase();
    return sorted.filter(
      (p) =>
        p.name.toLowerCase().includes(q) ||
        p.position.toLowerCase().includes(q) ||
        p.unit.toLowerCase().includes(q),
    );
  }, [sorted, searchTerm]);

  useEffect(() => {
    setVisibleCount(PAGE_SIZE);
  }, [searchTerm]);

  const visible = useMemo(() => filtered.slice(0, visibleCount), [filtered, visibleCount]);
  const hasMore = visibleCount < filtered.length;

  // Infinite scroll dentro do container
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

  const toggleFavorite = async (e: React.MouseEvent, prof: Professional) => {
    e.stopPropagation();
    e.preventDefault();
    if (!token || togglingId) return;
    const api = container.resolve<ComunicacaoProfissionaisApi>(DiTokens.comunicacaoProfissionaisApi);
    setTogglingId(prof.id);
    try {
      if (prof.favoritado) {
        await api.desfavoritar(token, prof.id);
        toast.success('Removido dos favoritos.');
      } else {
        await api.favoritar(token, prof.id);
        toast.success('Adicionado aos favoritos.');
      }
      await loadProfissionais();
    } catch {
      toast.error('Não foi possível atualizar o favorito.');
    } finally {
      setTogglingId(null);
    }
  };

  return (
    <Card className="shadow-softToken rounded-lgToken w-full min-w-0 flex flex-col overflow-hidden">
      {/* Cabeçalho com título e busca */}
      <CardHeader className="pb-3 flex-shrink-0 p-4">
        <div className="flex flex-col gap-2">
          {content.title && (
            <CardTitle className="flex items-center gap-2 text-sm font-semibold text-primaryText">
              <div className="w-6 h-6 rounded-smToken flex items-center justify-center flex-shrink-0 bg-primarySoft">
                <Users className="w-3.5 h-3.5 text-primary" />
              </div>
              <span className="truncate">{content.title}</span>
            </CardTitle>
          )}
          <div className="relative">
            <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground pointer-events-none" />
            <Input
              type="search"
              placeholder="Buscar profissionais..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="h-8 pl-8 text-xs rounded-lg border-border/50 bg-muted/20"
            />
          </div>
        </div>
      </CardHeader>

      {/* Área scrollável com altura fixa */}
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
              <Users className="w-8 h-8 opacity-40" />
              <p className="text-sm">Nenhum profissional encontrado</p>
            </div>
          ) : (
            <div className="p-3">
              <div className="grid grid-cols-3 gap-2">
                {visible.map((prof) => {
                  const initials = getInitials(prof.name);
                  const avatarSrc =
                    prof.avatar && token
                      ? prof.avatar.replace(/\$1/g, btoa(token))
                      : prof.avatar ?? undefined;
                  const birthday = isBirthday(prof.birthDate);
                  const isFav = prof.favoritado === true;

                  return (
                    <div
                      key={prof.id}
                      className="group relative flex flex-col items-center gap-1.5 p-2.5 pt-6 rounded-lg border border-border/50 bg-muted/20 hover:bg-primarySoft/40 hover:border-primary/30 transition-colors cursor-pointer"
                      onClick={() =>
                        navigate(`/comunicacao/profissional/${prof.id}`, {
                          state: { professional: prof },
                        })
                      }
                    >
                      {/* Botão favorito — canto superior direito */}
                      <button
                        type="button"
                        onClick={(e) => toggleFavorite(e, prof)}
                        disabled={togglingId === prof.id}
                        className="absolute top-1.5 right-1.5 z-10 rounded-full p-0.5 opacity-40 hover:opacity-100 group-hover:opacity-70 hover:!opacity-100 transition-opacity"
                        title={isFav ? 'Remover dos favoritos' : 'Adicionar aos favoritos'}
                      >
                        {togglingId === prof.id ? (
                          <Spinner className="h-3.5 w-3.5" />
                        ) : (
                          <Star
                            className={`h-3.5 w-3.5 transition-all ${
                              isFav
                                ? 'fill-yellow-400 text-yellow-500 opacity-100'
                                : 'text-muted-foreground hover:text-yellow-500'
                            }`}
                          />
                        )}
                      </button>

                      {/* Avatar */}
                      <div className="relative">
                        <Avatar className="w-14 h-14">
                          <AvatarImage src={avatarSrc} alt={prof.name} />
                          {/* mesmo design do AniversariantesCard */}
                          <AvatarFallback className="bg-primarySoft text-xs font-semibold text-primaryText">
                            {initials}
                          </AvatarFallback>
                        </Avatar>
                        {birthday && (
                          <span
                            className="absolute -top-0.5 -right-0.5 flex h-5 w-5 items-center justify-center rounded-full bg-pink-500 text-white shadow-sm ring-2 ring-background"
                            title="Aniversariante do dia"
                          >
                            <Cake className="h-3 w-3" />
                          </span>
                        )}
                      </div>

                      {/* Nome */}
                      <p className="text-xs font-semibold text-primaryText leading-tight line-clamp-2 text-center w-full group-hover:text-primary transition-colors">
                        {prof.name}
                      </p>

                      {/* Cargo */}
                      {prof.position && (
                        <div className="flex items-center gap-1 w-full justify-center">
                          <Briefcase className="w-3 h-3 shrink-0 text-muted-foreground" />
                          <p className="text-[10px] text-muted-foreground truncate">{prof.position}</p>
                        </div>
                      )}

                      {/* Unidade */}
                      {prof.unit && (
                        <div className="flex items-center gap-1 w-full justify-center">
                          <MapPin className="w-3 h-3 shrink-0 text-muted-foreground" />
                          <p className="text-[10px] text-muted-foreground truncate">{prof.unit}</p>
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>

              {/* Sentinel para infinite scroll */}
              {hasMore && (
                <div ref={sentinelRef} className="flex justify-center py-3">
                  <Spinner className="h-5 w-5 text-muted-foreground" />
                </div>
              )}

              <p className="text-center text-[10px] text-muted-foreground mt-2 pb-1">
                {visible.length} de {filtered.length} profissional{filtered.length !== 1 ? 'is' : ''}
              </p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
