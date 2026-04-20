import { useState, useMemo, useRef, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Search, Globe, Lock, Users, FileText, LayoutGrid } from 'lucide-react'
import { useComunicacaoComunidades } from '@/presentation/hooks/useComunicacaoComunidades'
import type { CommunityGroup } from '@domain/entities/comunicacao'
import type { ComunidadesMosaicoContent } from '@shared/types/homeBuilder'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar'
import { Spinner } from '@/components/ui/spinner'

const PAGE_SIZE = 12

const HEIGHT_MAP: Record<string, string> = {
  sm: 'h-[390px]',
  md: 'h-[580px]',
  lg: 'h-[770px]',
}

/** Gradiente padrão único para comunidades sem capa. */
const DEFAULT_COVER_GRADIENT = 'from-slate-500 to-slate-700'

function getInitials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean)
  if (parts.length === 0) return '?'
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase()
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase()
}

function formatCount(n: number): string {
  if (n >= 1000) return `${(n / 1000).toFixed(1)}k`
  return String(n)
}

interface CommunityCardProps {
  community: CommunityGroup
  onClick: () => void
}

function CommunityCard({ community, onClick }: CommunityCardProps) {
  const isPrivate = community.type === 'private'
  const initials = getInitials(community.name)

  return (
    <button
      type="button"
      onClick={onClick}
      className="group relative flex flex-col overflow-hidden rounded-xl border border-border/50 bg-card text-left transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md hover:border-primary/30 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary w-full"
    >
      {/* Capa */}
      <div className="relative h-[72px] w-full overflow-hidden flex-shrink-0">
        {community.coverImage ? (
          <img
            src={community.coverImage}
            alt={community.name}
            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
          />
        ) : (
          /* Placeholder padronizado: gradiente + iniciais centralizadas */
          <div
            className={`h-full w-full bg-gradient-to-br ${DEFAULT_COVER_GRADIENT} flex items-center justify-center transition-transform duration-300 group-hover:scale-105`}
          >
            <span className="text-lg font-bold text-white/80 select-none tracking-wide drop-shadow">
              {initials}
            </span>
          </div>
        )}

        {/* Badge tipo — canto superior direito */}
        <span
          className={`absolute top-1.5 right-1.5 flex items-center gap-0.5 rounded-full px-1.5 py-0.5 text-[9px] font-semibold leading-none shadow-sm ring-1 ring-white/20 ${
            isPrivate
              ? 'bg-slate-900/80 text-slate-100'
              : 'bg-emerald-600/90 text-white'
          }`}
        >
          {isPrivate ? <Lock className="h-2.5 w-2.5" /> : <Globe className="h-2.5 w-2.5" />}
          {isPrivate ? 'Privada' : 'Pública'}
        </span>
      </div>

      {/* Avatar flutuante sobreposto à capa */}
      <div className="absolute left-2.5 top-[50px] h-9 w-9 rounded-full ring-2 ring-background shadow-sm">
        <Avatar className="h-full w-full">
          {community.coverImage && (
            <AvatarImage src={community.coverImage} alt={community.name} className="object-cover" />
          )}
          <AvatarFallback
            className={`bg-gradient-to-br ${DEFAULT_COVER_GRADIENT} text-white text-[10px] font-bold`}
          >
            {initials}
          </AvatarFallback>
        </Avatar>
      </div>

      {/* Corpo */}
      <div className="flex flex-col gap-1 px-2.5 pb-2.5 pt-5 flex-1 min-h-0">
        <p className="text-xs font-semibold text-primaryText leading-tight line-clamp-1 group-hover:text-primary transition-colors">
          {community.name}
        </p>

        {community.description && (
          <p className="text-[10px] text-muted-foreground leading-snug line-clamp-2">
            {community.description}
          </p>
        )}

        {/* Stats */}
        <div className="mt-auto pt-1.5 flex items-center gap-2.5 border-t border-border/40">
          <span className="flex items-center gap-0.5 text-[10px] text-muted-foreground">
            <Users className="h-3 w-3 flex-shrink-0" />
            {formatCount(community.memberCount)}
          </span>
          <span className="flex items-center gap-0.5 text-[10px] text-muted-foreground">
            <FileText className="h-3 w-3 flex-shrink-0" />
            {formatCount(community.postCount)}
          </span>
        </div>
      </div>
    </button>
  )
}

interface EmptyStateProps {
  filtered: boolean
}

function EmptyState({ filtered }: EmptyStateProps) {
  return (
    <div className="flex h-full flex-col items-center justify-center gap-3 text-center px-4">
      <div className="rounded-full bg-muted p-4">
        <LayoutGrid className="h-8 w-8 text-muted-foreground opacity-50" />
      </div>
      <div>
        <p className="text-sm font-semibold text-primaryText">
          {filtered ? 'Nenhuma comunidade encontrada' : 'Sem comunidades disponíveis'}
        </p>
        <p className="mt-0.5 text-xs text-muted-foreground">
          {filtered ? 'Tente outros termos de busca' : 'Novas comunidades aparecerão aqui'}
        </p>
      </div>
    </div>
  )
}

interface ComunidadesMosaicoBlockProps {
  content: ComunidadesMosaicoContent
}

export function ComunidadesMosaicoBlock({ content }: ComunidadesMosaicoBlockProps) {
  const { comunidades, loading } = useComunicacaoComunidades()
  const navigate = useNavigate()

  const [searchTerm, setSearchTerm] = useState('')
  const [visibleCount, setVisibleCount] = useState(PAGE_SIZE)
  const scrollRef = useRef<HTMLDivElement>(null)
  const sentinelRef = useRef<HTMLDivElement>(null)

  const heightClass = HEIGHT_MAP[content.height] ?? HEIGHT_MAP.md
  const gridClass = content.columns === 3 ? 'grid-cols-3' : 'grid-cols-2'

  const filtered = useMemo<CommunityGroup[]>(() => {
    const active = content.onlyMine
      ? comunidades.filter((c) => c.status === 'active')
      : comunidades.filter((c) => c.status === 'active')

    if (!searchTerm.trim()) return active

    const q = searchTerm.toLowerCase()
    return active.filter(
      (c) =>
        c.name.toLowerCase().includes(q) ||
        c.description.toLowerCase().includes(q) ||
        c.creatorName.toLowerCase().includes(q),
    )
  }, [comunidades, searchTerm, content.onlyMine])

  useEffect(() => {
    setVisibleCount(PAGE_SIZE)
  }, [searchTerm])

  const visible = useMemo(() => filtered.slice(0, visibleCount), [filtered, visibleCount])
  const hasMore = visibleCount < filtered.length

  useEffect(() => {
    if (!hasMore || !sentinelRef.current || !scrollRef.current) return
    const root = scrollRef.current
    const el = sentinelRef.current
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0]?.isIntersecting) {
          setVisibleCount((prev) => Math.min(prev + PAGE_SIZE, filtered.length))
        }
      },
      { root, rootMargin: '120px', threshold: 0 },
    )
    observer.observe(el)
    return () => observer.disconnect()
  }, [hasMore, filtered.length])

  return (
    <Card className="shadow-softToken rounded-lgToken w-full min-w-0 flex flex-col overflow-hidden">
      <CardHeader className="pb-3 flex-shrink-0 p-4">
        <div className="flex flex-col gap-2">
          {content.title && (
            <CardTitle className="flex items-center gap-2 text-sm font-semibold text-primaryText">
              <div className="flex h-6 w-6 items-center justify-center rounded-smToken bg-primarySoft flex-shrink-0">
                <LayoutGrid className="h-3.5 w-3.5 text-primary" />
              </div>
              <span className="truncate">{content.title}</span>
            </CardTitle>
          )}

          <div className="relative">
            <Search className="pointer-events-none absolute left-2.5 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              type="search"
              placeholder="Buscar comunidades..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="h-8 rounded-lg border-border/50 bg-muted/20 pl-8 text-xs"
            />
          </div>
        </div>
      </CardHeader>

      <CardContent className="flex-1 min-h-0 p-0">
        <div
          ref={scrollRef}
          className={`${heightClass} overflow-y-auto`}
          style={{ scrollbarWidth: 'thin' }}
        >
          {loading ? (
            <div className="flex h-full items-center justify-center">
              <Spinner className="h-6 w-6 text-muted-foreground" />
            </div>
          ) : filtered.length === 0 ? (
            <EmptyState filtered={searchTerm.trim().length > 0} />
          ) : (
            <div className="p-3">
              <div className={`grid ${gridClass} gap-2.5`}>
                {visible.map((community) => (
                  <CommunityCard
                    key={community.id}
                    community={community}
                    onClick={() => navigate(`/comunicacao/grupo/${community.id}`)}
                  />
                ))}
              </div>

              {hasMore && (
                <div ref={sentinelRef} className="flex justify-center py-3">
                  <Spinner className="h-5 w-5 text-muted-foreground" />
                </div>
              )}

              <p className="mt-2 pb-1 text-center text-[10px] text-muted-foreground">
                {visible.length} de {filtered.length} comunidade{filtered.length !== 1 ? 's' : ''}
              </p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  )
}
