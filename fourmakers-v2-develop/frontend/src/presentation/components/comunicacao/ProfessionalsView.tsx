import { useState, useMemo, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Search,
  Briefcase,
  MapPin,
  Star,
  Users,
  Cake,
  UserCircle,
  Calendar,
  ChevronDown,
} from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { Professional } from '@domain/entities/comunicacao';
import type { ComunicacaoProfissionaisApi } from '@data/api/ComunicacaoProfissionaisApi';
import { useComunicacaoProfissionais } from '@presentation/hooks/useComunicacaoProfissionais';
import { useCargos } from '@presentation/hooks/useCargos';
import { useDepartamentos } from '@presentation/hooks/useDepartamentos';
import { toast } from 'sonner';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Spinner } from '@/components/ui/spinner';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';

const getMonthDay = (
  birthDate?: string,
): { month: number; day: number } | null => {
  if (!birthDate) return null;
  const parts = birthDate.split('-').map(Number);
  if (parts.length === 2) return { month: parts[0], day: parts[1] };
  if (parts.length === 3) return { month: parts[1], day: parts[2] };
  return null;
};

const isBirthday = (birthDate?: string): boolean => {
  const md = getMonthDay(birthDate);
  if (!md) return false;
  const today = new Date();
  return today.getMonth() + 1 === md.month && today.getDate() === md.day;
};

const formatBirthDate = (birthDate?: string): string | null => {
  if (!birthDate) return null;
  const parts = birthDate.split('-');
  if (parts.length === 2)
    return `${parts[1].padStart(2, '0')}/${parts[0].padStart(2, '0')}`;
  if (parts.length === 3)
    return `${parts[2].padStart(2, '0')}/${parts[1].padStart(2, '0')}/${parts[0]}`;
  return null;
};

const PAGE_SIZE = 24;

export function ProfessionalsView() {
  const token = useAppSelector((state) => state.auth.token);
  const { profissionais, loading, error, loadProfissionais } =
    useComunicacaoProfissionais();
  const { cargos, loading: loadingCargos } = useCargos();
  const { departamentos, loading: loadingDepartamentos } = useDepartamentos();
  const [searchTerm, setSearchTerm] = useState('');
  const [unitFilter, setUnitFilter] = useState<string>('all');
  const [positionFilter, setPositionFilter] = useState<string>('all');
  const [showFavoritesOnly, setShowFavoritesOnly] = useState(false);
  const [togglingFavoriteId, setTogglingFavoriteId] = useState<string | null>(null);
  const [visibleCount, setVisibleCount] = useState(PAGE_SIZE);
  const [cargoOpen, setCargoOpen] = useState(false);
  const [unidadeOpen, setUnidadeOpen] = useState(false);
  const loadMoreRef = useRef<HTMLDivElement>(null);
  const navigate = useNavigate();

  const cargosOptions = useMemo(
    () =>
      cargos
        .map((c) => (c.cargo || c.codigoCargo || '').trim())
        .filter(Boolean)
        .filter((v, i, a) => a.indexOf(v) === i)
        .sort(),
    [cargos],
  );
  const unidadesOptions = useMemo(
    () =>
      departamentos
        .map((d) => (d.departamento || d.cod || '').trim())
        .filter(Boolean)
        .filter((v, i, a) => a.indexOf(v) === i)
        .sort(),
    [departamentos],
  );

  const showBirthdayIcon = (professional: Professional): boolean => {
    if (!professional.birthDate) return false;
    return isBirthday(professional.birthDate);
  };

  const toggleFavorite = async (professional: Professional) => {
    if (!token || togglingFavoriteId) return;
    const api = container.resolve<ComunicacaoProfissionaisApi>(DiTokens.comunicacaoProfissionaisApi);
    setTogglingFavoriteId(professional.id);
    try {
      const isFavoritado = professional.favoritado === true;
      if (isFavoritado) {
        await api.desfavoritar(token, professional.id);
        toast.success('Removido dos favoritos.');
      } else {
        await api.favoritar(token, professional.id);
        toast.success('Adicionado aos favoritos.');
      }
      await loadProfissionais();
    } catch {
      toast.error('Não foi possível atualizar o favorito.');
    } finally {
      setTogglingFavoriteId(null);
    }
  };

  const filteredProfessionals = useMemo(
    () =>
      profissionais.filter((prof) => {
        const matchesSearch =
          prof.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          prof.position.toLowerCase().includes(searchTerm.toLowerCase()) ||
          prof.unit.toLowerCase().includes(searchTerm.toLowerCase());
        const matchesUnit = unitFilter === 'all' || prof.unit === unitFilter;
        const matchesPosition =
          positionFilter === 'all' || prof.position === positionFilter;
        const matchesFavorites =
          !showFavoritesOnly || prof.favoritado === true;
        return matchesSearch && matchesUnit && matchesPosition && matchesFavorites;
      }),
    [
      profissionais,
      searchTerm,
      unitFilter,
      positionFilter,
      showFavoritesOnly,
    ],
  );

  const visibleProfessionals = useMemo(
    () => filteredProfessionals.slice(0, visibleCount),
    [filteredProfessionals, visibleCount],
  );
  const hasMore = visibleCount < filteredProfessionals.length;

  useEffect(() => {
    setVisibleCount(PAGE_SIZE);
  }, [searchTerm, unitFilter, positionFilter, showFavoritesOnly]);

  useEffect(() => {
    if (!hasMore || !loadMoreRef.current) return;
    const el = loadMoreRef.current;
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0]?.isIntersecting) {
          setVisibleCount((prev) =>
            Math.min(prev + PAGE_SIZE, filteredProfessionals.length),
          );
        }
      },
      { rootMargin: '200px', threshold: 0 },
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [hasMore, filteredProfessionals.length]);

  const favoritesCount = profissionais.filter((p) => p.favoritado === true).length;

  if (loading) {
    return (
      <div className="flex justify-center py-12">
        <Spinner className="h-8 w-8 text-muted-foreground" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="rounded-lg border border-destructive/50 bg-destructive/10 px-4 py-3 text-sm text-destructive">
        {error}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Filtros */}
      <div className="flex flex-wrap items-center gap-4">
        <div className="relative flex-1 min-w-[200px] max-w-md">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <Input
            placeholder="Buscar profissionais..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10 rounded-lg"
          />
        </div>

        <Popover open={cargoOpen} onOpenChange={setCargoOpen}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              role="combobox"
              aria-expanded={cargoOpen}
              disabled={loadingCargos}
              className="w-[200px] justify-between rounded-lg font-normal"
            >
              <span className="flex items-center gap-2 truncate">
                <Briefcase className="w-4 h-4 shrink-0" />
                {positionFilter === 'all'
                  ? loadingCargos
                    ? 'Carregando...'
                    : 'Cargo'
                  : positionFilter}
              </span>
              <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-[200px] p-0" align="start">
            <Command>
              <CommandInput placeholder="Buscar cargo..." className="h-9" />
              <CommandList>
                <CommandEmpty>Nenhum cargo encontrado.</CommandEmpty>
                <CommandGroup>
                  <CommandItem
                    value="Todos os cargos"
                    onSelect={() => {
                      setPositionFilter('all');
                      setCargoOpen(false);
                    }}
                  >
                    Todos os cargos
                  </CommandItem>
                  {cargosOptions.map((position) => (
                    <CommandItem
                      key={position}
                      value={position}
                      onSelect={() => {
                        setPositionFilter(position);
                        setCargoOpen(false);
                      }}
                    >
                      {position}
                    </CommandItem>
                  ))}
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>

        <Popover open={unidadeOpen} onOpenChange={setUnidadeOpen}>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              role="combobox"
              aria-expanded={unidadeOpen}
              disabled={loadingDepartamentos}
              className="w-[200px] justify-between rounded-lg font-normal"
            >
              <span className="flex items-center gap-2 truncate">
                <MapPin className="w-4 h-4 shrink-0" />
                {unitFilter === 'all'
                  ? loadingDepartamentos
                    ? 'Carregando...'
                    : 'Unidade'
                  : unitFilter}
              </span>
              <ChevronDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-[200px] p-0" align="start">
            <Command>
              <CommandInput placeholder="Buscar unidade..." className="h-9" />
              <CommandList>
                <CommandEmpty>Nenhuma unidade encontrada.</CommandEmpty>
                <CommandGroup>
                  <CommandItem
                    value="Todas as unidades"
                    onSelect={() => {
                      setUnitFilter('all');
                      setUnidadeOpen(false);
                    }}
                  >
                    Todas as unidades
                  </CommandItem>
                  {unidadesOptions.map((unit) => (
                    <CommandItem
                      key={unit}
                      value={unit}
                      onSelect={() => {
                        setUnitFilter(unit);
                        setUnidadeOpen(false);
                      }}
                    >
                      {unit}
                    </CommandItem>
                  ))}
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>

        <Button
          variant={showFavoritesOnly ? 'primary' : 'outline'}
          size="sm"
          onClick={() => setShowFavoritesOnly(!showFavoritesOnly)}
          className="gap-2 rounded-pillToken py-2.5 px-4 text-sm font-medium transition-all duration-200 data-[state=active]:shadow-md"
        >
          <Star
            className={`w-4 h-4 ${showFavoritesOnly ? 'fill-current' : ''}`}
          />
          Meus Favoritos
          {favoritesCount > 0 && (
            <Badge variant="secondary" className="ml-1 rounded-lg">
              {favoritesCount}
            </Badge>
          )}
        </Button>
      </div>

      {/* Lista de Profissionais (scroll infinito: exibe PAGE_SIZE por vez) */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {visibleProfessionals.map((professional) => {
          const isFavorite = professional.favoritado === true;
          const initials = professional.name
            .split(' ')
            .map((n) => n[0])
            .join('')
            .substring(0, 2)
            .toUpperCase();
          const avatarSrc =
            professional.avatar && token
              ? professional.avatar.replace(/\$1/g, btoa(token))
              : professional.avatar ?? undefined;

          return (
            <Card
              key={professional.id}
              className="hover:shadow-lg transition-all cursor-pointer group rounded-lg"
              onClick={() =>
                navigate(`/comunicacao/profissional/${professional.id}`, {
                  state: { professional },
                })
              }
            >
              <CardContent className="p-4">
                <div className="flex items-start gap-4">
                  <div className="relative flex-shrink-0">
                    <Avatar className="w-16 h-16 ring-2 ring-primary/10">
                      <AvatarImage
                        src={avatarSrc}
                        alt={professional.name}
                      />
                      <AvatarFallback className="bg-gradient-to-br from-primary to-accent text-primary-foreground">
                        {initials}
                      </AvatarFallback>
                    </Avatar>
                    {showBirthdayIcon(professional) && (
                      <span
                        className="absolute -top-1 -right-1 flex h-7 w-7 items-center justify-center rounded-full bg-pink-500 text-white shadow-sm ring-2 ring-background"
                        title="Aniversariante do dia"
                      >
                        <Cake className="h-3.5 w-3.5" />
                      </span>
                    )}
                  </div>

                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-2">
                      <div className="flex items-center gap-2 flex-1 min-w-0">
                        <h3 className="font-semibold truncate group-hover:text-primary transition-colors">
                          {professional.name}
                        </h3>
                        {showBirthdayIcon(professional) && (
                          <Badge
                            variant="outline"
                            className="bg-pink-500/10 text-pink-600 border-pink-500/30 gap-1 flex-shrink-0 rounded-lg"
                          >
                            <Cake className="w-3 h-3" />
                            Aniversário
                          </Badge>
                        )}
                      </div>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-8 w-8 flex-shrink-0 rounded-lg"
                        disabled={togglingFavoriteId === professional.id}
                        onClick={(e) => {
                          e.stopPropagation();
                          toggleFavorite(professional);
                        }}
                      >
                        <Star
                          className={`w-4 h-4 transition-all ${
                            isFavorite
                              ? 'fill-yellow-400 text-yellow-500'
                              : 'text-muted-foreground hover:text-yellow-500'
                          }`}
                        />
                      </Button>
                    </div>

                    <div className="space-y-1 mt-2">
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Briefcase className="w-3 h-3 flex-shrink-0" />
                        <span className="truncate">
                          {professional.position}
                        </span>
                      </div>
                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <MapPin className="w-3 h-3 flex-shrink-0" />
                        <span className="truncate">{professional.unit}</span>
                      </div>
                      {professional.managerName && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <UserCircle className="w-3 h-3 flex-shrink-0" />
                          <span className="truncate">
                            Gestor: {professional.managerName}
                          </span>
                        </div>
                      )}
                      {formatBirthDate(professional.birthDate) && (
                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Calendar className="w-3 h-3 flex-shrink-0" />
                          <span>
                            Nasc.:{' '}
                            {formatBirthDate(professional.birthDate)}
                          </span>
                        </div>
                      )}
                    </div>

                    {professional.about && (
                      <p className="text-xs text-muted-foreground mt-3 line-clamp-2">
                        {professional.about}
                      </p>
                    )}
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {hasMore && <div ref={loadMoreRef} className="h-4 flex justify-center py-4" aria-hidden />}

      {filteredProfessionals.length === 0 && (
        <div className="text-center py-12">
          <Users className="w-12 h-12 mx-auto text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">
            Nenhum profissional encontrado
          </p>
        </div>
      )}
    </div>
  );
}
