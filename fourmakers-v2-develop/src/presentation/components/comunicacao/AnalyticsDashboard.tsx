import { useMemo, useState, useEffect, useCallback } from 'react';
import { container } from 'tsyringe';
import {
  FileText,
  Megaphone,
  Heart,
  CheckCircle2,
  BarChart3,
  LayoutGrid,
  BookOpen,
  BookMarked,
  Target,
  Eye,
} from 'lucide-react';
import { Spinner } from '@/components/ui/spinner';
import { mockGroups, mockPosts } from '@data/mocks/comunicacao/communityData';
import { mockAnnouncements } from '@data/mocks/comunicacao/announcementsData';
import { mockUserGroups } from '@data/mocks/comunicacao/userGroupsData';
import type {
  CommunityPost,
  Announcement,
  AnalyticsResumoRetorno,
  AnalyticsResumoItem,
} from '@domain/entities/comunicacao';
import { ObterAnalyticsResumoUseCase } from '@domain/usecases/ObterAnalyticsResumoUseCase';
import { useAppSelector } from '@app/store/hooks';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Input } from '@/components/ui/input';
import { DataTable, type Column } from '@presentation/components/common';

const publishedPosts = mockPosts.filter((p) => p.status === 'published');
const publishedAnnouncements = mockAnnouncements.filter(
  (a) => a.status === 'published' || a.status === 'archived',
);

type PostsByCommunity = {
  groupId: string;
  groupName: string;
  memberCount: number;
  postCount: number;
  totalLikes: number;
  totalComments: number;
  totalViews: number;
  totalAcknowledged: number;
  engagementRate: number;
};

type UnifiedRow = {
  id: string;
  kind: 'post' | 'announcement';
  typeLabel: string;
  title: string;
  where: string;
  obligatory: boolean;
  publishedAt: string;
  likes: number;
  comments: number;
  acknowledgments: number;
  views: number;
  reach: number;
  engagementRate: number;
};

type AnnouncementRow = Announcement & { engagementRate: number };

const formatDate = (iso: string) => {
  try {
    return new Date(iso).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  } catch {
    return '-';
  }
};

function mapResumoItemToUnifiedRow(item: AnalyticsResumoItem): UnifiedRow {
  const kind = item.tipo.toLowerCase().includes('comunicado')
    ? 'announcement'
    : 'post';
  return {
    id: item.publicacaoId,
    kind,
    typeLabel: item.tipo,
    title: item.titulo,
    where: item.onde,
    obligatory: item.obrigatorioLeitura,
    publishedAt: item.data,
    likes: item.likes,
    comments: item.comentarios,
    acknowledgments: item.aceites,
    views: item.visualizacoes,
    reach: item.alcance,
    engagementRate: item.engajamentoPercentual,
  };
}

// Colunas: Visão unificada
const UNIFIED_COLUMNS: Column[] = [
  { id: 'typeLabel', label: 'Tipo', sortable: true, width: 'w-[140px]' },
  { id: 'title', label: 'Título', sortable: true, width: 'w-[200px]' },
  { id: 'where', label: 'Onde', sortable: true, width: 'w-[180px]' },
  { id: 'obligatory', label: 'Obrig. leitura', sortable: true, align: 'center' },
  { id: 'publishedAt', label: 'Data', sortable: true, width: 'w-[100px]' },
  { id: 'likes', label: 'Likes', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'comments', label: 'Coment.', sortable: true, align: 'right', width: 'w-[80px]' },
  { id: 'acknowledgments', label: 'Aceites', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'views', label: 'Visualiz.', sortable: true, align: 'right', width: 'w-[80px]' },
  { id: 'reach', label: 'Alcance', sortable: true, align: 'right', width: 'w-[80px]' },
  { id: 'engagementRate', label: 'Engaj. %', sortable: true, align: 'right', width: 'w-[90px]' },
];

// Colunas: Posts por comunidade
const POSTS_BY_COMMUNITY_COLUMNS: Column[] = [
  { id: 'groupName', label: 'Comunidade', sortable: true },
  { id: 'memberCount', label: 'Membros', sortable: true, align: 'center', width: 'w-[90px]' },
  { id: 'postCount', label: 'Posts', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'totalLikes', label: 'Likes', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'totalComments', label: 'Comentários', sortable: true, align: 'right', width: 'w-[100px]' },
  { id: 'totalAcknowledged', label: 'Aceites', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'totalViews', label: 'Visualizações', sortable: true, align: 'right', width: 'w-[110px]' },
  { id: 'engagementRate', label: 'Engajamento %', sortable: true, align: 'right', width: 'w-[120px]' },
];

// Colunas: Comunicados
const ANNOUNCEMENTS_COLUMNS: Column[] = [
  { id: 'title', label: 'Título', sortable: true, width: 'w-[220px]' },
  { id: 'announcementType', label: 'Tipo', sortable: true, width: 'w-[120px]' },
  { id: 'requiresAcknowledgment', label: 'Obrig. leitura', sortable: true, align: 'center', width: 'w-[120px]' },
  { id: 'likesCount', label: 'Likes', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'commentsCount', label: 'Comentários', sortable: true, align: 'right', width: 'w-[100px]' },
  { id: 'acknowledgmentCount', label: 'Aceites', sortable: true, align: 'right', width: 'w-[70px]' },
  { id: 'viewsCount', label: 'Visualizações', sortable: true, align: 'right', width: 'w-[110px]' },
  { id: 'engagementRate', label: 'Engaj. %', sortable: true, align: 'right', width: 'w-[90px]' },
];

const RESUMO_PAYLOAD = {
  dataInicio: null as string | null,
  dataFim: null as string | null,
  tipoConteudo: null as string | null,
  texto: null as string | null,
  pagina: 1,
  tamanhoPagina: 20,
};

export function AnalyticsDashboard() {
  const [searchUnified, setSearchUnified] = useState('');
  const token = useAppSelector((state) => state.auth.token);
  const [resumo, setResumo] = useState<AnalyticsResumoRetorno | null>(null);
  const [loadingResumo, setLoadingResumo] = useState(true);

  const obterResumoUseCase = useMemo(
    () => container.resolve(ObterAnalyticsResumoUseCase),
    [],
  );

  const fetchResumo = useCallback(async () => {
    if (!token) {
      setLoadingResumo(false);
      return;
    }
    setLoadingResumo(true);
    try {
      const data = await obterResumoUseCase.execute(token, RESUMO_PAYLOAD);
      setResumo(data ?? null);
    } catch {
      setResumo(null);
    } finally {
      setLoadingResumo(false);
    }
  }, [token, obterResumoUseCase]);

  useEffect(() => {
    fetchResumo();
  }, [fetchResumo]);

  const postsByCommunity = useMemo((): PostsByCommunity[] => {
    const byGroup = new Map<
      string,
      { posts: CommunityPost[]; groupName: string; memberCount: number }
    >();
    mockGroups.forEach((g) => {
      byGroup.set(g.id, { posts: [], groupName: g.name, memberCount: g.memberCount });
    });
    publishedPosts.forEach((p) => {
      const cur = byGroup.get(p.groupId);
      if (cur) {
        cur.posts.push(p);
      } else {
        byGroup.set(p.groupId, {
          posts: [p],
          groupName: p.groupName,
          memberCount: 0,
        });
      }
    });
    return Array.from(byGroup.entries())
      .filter(([, v]) => v.posts.length > 0)
      .map(([groupId, { posts, groupName, memberCount }]) => {
        const totalLikes = posts.reduce((s, p) => s + p.likesCount, 0);
        const totalComments = posts.reduce((s, p) => s + p.commentsCount, 0);
        const totalViews = posts.reduce((s, p) => s + p.viewsCount, 0);
        const totalAcknowledged = posts.reduce((s, p) => s + p.acknowledgmentCount, 0);
        const interactions = totalLikes + totalComments + totalAcknowledged;
        const engagementRate =
          totalViews > 0 ? Math.round((interactions / totalViews) * 100) : 0;
        return {
          groupId,
          groupName,
          memberCount,
          postCount: posts.length,
          totalLikes,
          totalComments,
          totalViews,
          totalAcknowledged,
          engagementRate,
        };
      })
      .sort((a, b) => b.postCount - a.postCount);
  }, []);

  const announcementsStats = useMemo(() => {
    const byType = { informative: 0, document: 0 };
    const byObligatory = { obligatory: 0, optional: 0 };
    let totalAcknowledged = 0;
    let totalLikes = 0;
    let totalComments = 0;
    let totalViews = 0;
    publishedAnnouncements.forEach((a) => {
      byType[a.announcementType]++;
      if (a.requiresAcknowledgment) byObligatory.obligatory++;
      else byObligatory.optional++;
      totalAcknowledged += a.acknowledgmentCount;
      totalLikes += a.likesCount;
      totalComments += a.commentsCount;
      totalViews += a.viewsCount;
    });
    const totalInteractions = totalLikes + totalComments + totalAcknowledged;
    const engagementRate =
      totalViews > 0 ? Math.round((totalInteractions / totalViews) * 100) : 0;
    return {
      total: publishedAnnouncements.length,
      byType,
      byObligatory,
      totalAcknowledged,
      totalLikes,
      totalComments,
      totalViews,
      engagementRate,
    };
  }, []);

  const reachForAnnouncement = (a: Announcement): number => {
    const ids = new Set(a.targetUserGroupIds);
    return mockUserGroups
      .filter((ug) => ids.has(ug.id))
      .reduce((s, ug) => s + ug.members.length, 0);
  };

  const unifiedRowsFromApi = useMemo((): UnifiedRow[] => {
    if (!resumo?.itens?.length) return [];
    return resumo.itens.map(mapResumoItemToUnifiedRow);
  }, [resumo]);

  const unifiedRowsMock = useMemo((): UnifiedRow[] => {
    const rows: UnifiedRow[] = [];
    publishedPosts.forEach((p) => {
      const group = mockGroups.find((g) => g.id === p.groupId);
      const reach = group?.memberCount ?? 0;
      const interactions =
        p.likesCount + p.commentsCount + p.acknowledgmentCount;
      const engagementRate =
        p.viewsCount > 0
          ? Math.round((interactions / p.viewsCount) * 100)
          : 0;
      const typeLabel =
        p.type === 'document'
          ? 'Post (documento)'
          : p.type === 'image'
            ? 'Post (imagem)'
            : p.type === 'video'
              ? 'Post (vídeo)'
              : 'Post (texto)';
      rows.push({
        id: p.id,
        kind: 'post',
        typeLabel,
        title: p.title,
        where: p.groupName,
        obligatory: p.requiresAcknowledgment,
        publishedAt: p.publishedAt ?? p.createdAt ?? '',
        likes: p.likesCount,
        comments: p.commentsCount,
        acknowledgments: p.acknowledgmentCount,
        views: p.viewsCount,
        reach,
        engagementRate,
      });
    });
    publishedAnnouncements.forEach((a) => {
      const reach = reachForAnnouncement(a);
      const interactions =
        a.likesCount + a.commentsCount + a.acknowledgmentCount;
      const engagementRate =
        a.viewsCount > 0
          ? Math.round((interactions / a.viewsCount) * 100)
          : 0;
      const typeLabel =
        a.announcementType === 'document'
          ? 'Comunicado documento'
          : 'Comunicado informativo';
      rows.push({
        id: a.id,
        kind: 'announcement',
        typeLabel,
        title: a.title,
        where: a.targetUserGroupNames.join(', '),
        obligatory: a.requiresAcknowledgment,
        publishedAt: a.publishedAt ?? a.createdAt ?? '',
        likes: a.likesCount,
        comments: a.commentsCount,
        acknowledgments: a.acknowledgmentCount,
        views: a.viewsCount,
        reach,
        engagementRate,
      });
    });
    return rows;
  }, []);

  const unifiedRows = resumo ? unifiedRowsFromApi : unifiedRowsMock;

  const filteredUnifiedRows = useMemo(() => {
    if (!searchUnified.trim()) return unifiedRows;
    const q = searchUnified.toLowerCase();
    return unifiedRows.filter(
      (r) =>
        r.title.toLowerCase().includes(q) ||
        r.where.toLowerCase().includes(q) ||
        r.typeLabel.toLowerCase().includes(q),
    );
  }, [unifiedRows, searchUnified]);

  const announcementRows = useMemo((): AnnouncementRow[] => {
    return publishedAnnouncements.map((a) => {
      const interactions =
        a.likesCount + a.commentsCount + a.acknowledgmentCount;
      const engagementRate =
        a.viewsCount > 0
          ? Math.round((interactions / a.viewsCount) * 100)
          : 0;
      return { ...a, engagementRate };
    });
  }, []);

  const renderUnifiedCell = (row: UnifiedRow, columnId: string) => {
    switch (columnId) {
      case 'typeLabel':
        return (
          <Badge variant={row.kind === 'post' ? 'default' : 'secondary'}>
            {row.typeLabel}
          </Badge>
        );
      case 'title':
        return (
          <span
            className="font-medium max-w-[200px] truncate block text-primaryText"
            title={row.title}
          >
            {row.title}
          </span>
        );
      case 'where':
        return (
          <span
            className="max-w-[180px] truncate block text-secondaryText"
            title={row.where}
          >
            {row.where}
          </span>
        );
      case 'obligatory':
        return row.obligatory ? (
          <Badge variant="outline" className="gap-1">
            <BookOpen className="w-3 h-3" /> Sim
          </Badge>
        ) : (
          <span className="text-secondaryText">Não</span>
        );
      case 'publishedAt':
        return formatDate(row.publishedAt);
      case 'likes':
      case 'comments':
      case 'acknowledgments':
      case 'views':
      case 'reach':
        return row[columnId as keyof UnifiedRow] as number;
      case 'engagementRate':
        return (
          <span
            className={
              row.engagementRate >= 50 ? 'text-success font-medium' : ''
            }
          >
            {row.engagementRate}%
          </span>
        );
      default:
        return null;
    }
  };

  const renderPostsByCommunityCell = (c: PostsByCommunity, columnId: string) => {
    switch (columnId) {
      case 'groupName':
        return <span className="font-medium text-primaryText">{c.groupName}</span>;
      case 'memberCount':
      case 'postCount':
      case 'totalLikes':
      case 'totalComments':
      case 'totalAcknowledged':
      case 'totalViews':
        return c[columnId as keyof PostsByCommunity] as number;
      case 'engagementRate':
        return (
          <span
            className={
              c.engagementRate >= 50 ? 'text-success font-medium' : ''
            }
          >
            {c.engagementRate}%
          </span>
        );
      default:
        return null;
    }
  };

  const renderAnnouncementCell = (a: AnnouncementRow, columnId: string) => {
    switch (columnId) {
      case 'title':
        return (
          <span
            className="font-medium max-w-[220px] truncate block text-primaryText"
            title={a.title}
          >
            {a.title}
          </span>
        );
      case 'announcementType':
        return (
          <Badge
            variant={a.announcementType === 'document' ? 'secondary' : 'default'}
          >
            {a.announcementType === 'document' ? 'Documento' : 'Informativo'}
          </Badge>
        );
      case 'requiresAcknowledgment':
        return a.requiresAcknowledgment ? (
          <Badge variant="outline" className="gap-1">
            <BookOpen className="w-3 h-3" /> Sim
          </Badge>
        ) : (
          <span className="text-secondaryText">Não</span>
        );
      case 'likesCount':
      case 'commentsCount':
      case 'acknowledgmentCount':
      case 'viewsCount':
        return a[columnId as keyof Announcement] as number;
      case 'engagementRate':
        return (
          <span
            className={
              a.engagementRate >= 50 ? 'text-success font-medium' : ''
            }
          >
            {a.engagementRate}%
          </span>
        );
      default:
        return null;
    }
  };

  const bn = resumo?.bigNumbers;
  const totalPosts = bn ? bn.postsComunidades : publishedPosts.length;
  const totalCommunitiesWithPosts = postsByCommunity.length;
  const totalLikesPosts = bn
    ? bn.likesPosts
    : postsByCommunity.reduce((s, c) => s + c.totalLikes, 0);
  const totalCommentsPosts = bn
    ? bn.comentariosPosts
    : postsByCommunity.reduce((s, c) => s + c.totalComments, 0);
  const engajamentoPosts = bn ? bn.engajamentoPosts : totalLikesPosts + totalCommentsPosts;
  const comunicadosPublicados = bn
    ? bn.comunicadosPublicados
    : announcementsStats.total;
  const comunicadosInformativos = bn
    ? bn.comunicadosInformativos
    : announcementsStats.byType.informative;
  const comunicadosDocumentos = bn
    ? bn.comunicadosDocumentos
    : announcementsStats.byType.document;
  const aceitesComunicados = bn
    ? bn.aceitesComunicados
    : announcementsStats.totalAcknowledged;
  const comunicadosObrigatorios = bn
    ? bn.comunicadosObrigatorios
    : announcementsStats.byObligatory.obligatory;
  const comunicadosOpcionais = bn
    ? bn.comunicadosOpcionais
    : announcementsStats.byObligatory.optional;

  return (
    <div className="space-y-6">
      {/* KPIs */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="p-2 rounded-lg bg-primary/10">
                <LayoutGrid className="w-5 h-5 text-primary" />
              </div>
              <span className="text-2xl font-bold text-primaryText tabular-nums">{totalPosts}</span>
            </div>
            <p className="text-sm text-secondaryText mt-2">Posts (comunidades)</p>
            <p className="text-xs text-secondaryText">
              Em {totalCommunitiesWithPosts} comunidade
              {totalCommunitiesWithPosts !== 1 ? 's' : ''}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="p-2 rounded-lg bg-primary/10">
                <Heart className="w-5 h-5 text-primary" />
              </div>
              <span className="text-2xl font-bold text-primaryText tabular-nums">
                {engajamentoPosts}
              </span>
            </div>
            <p className="text-sm text-secondaryText mt-2">Engajamento em posts</p>
            <p className="text-xs text-secondaryText">
              {totalLikesPosts} likes · {totalCommentsPosts} comentários
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="p-2 rounded-lg bg-primary/10">
                <Megaphone className="w-5 h-5 text-primary" />
              </div>
              <span className="text-2xl font-bold text-primaryText tabular-nums">
                {comunicadosPublicados}
              </span>
            </div>
            <p className="text-sm text-secondaryText mt-2">Comunicados publicados</p>
            <p className="text-xs text-secondaryText">
              {comunicadosInformativos} informativos · {comunicadosDocumentos} documentos
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="p-2 rounded-lg bg-success/10">
                <CheckCircle2 className="w-5 h-5 text-success" />
              </div>
              <span className="text-2xl font-bold text-primaryText tabular-nums">
                {aceitesComunicados}
              </span>
            </div>
            <p className="text-sm text-secondaryText mt-2">Aceites (comunicados)</p>
            <p className="text-xs text-secondaryText">
              {comunicadosObrigatorios} obrigatórios · {comunicadosOpcionais} opcionais
            </p>
          </CardContent>
        </Card>
      </div>

      <Tabs defaultValue="unified" className="space-y-4">
        <div className="flex flex-wrap items-center gap-2 sm:gap-0 sm:flex-nowrap min-w-0">
          <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-1 sm:gap-2 w-auto min-w-0 max-w-full overflow-x-auto shadow-sm [&::-webkit-scrollbar]:h-1">
            <TabsTrigger
              value="unified"
              className="flex items-center gap-2 text-sm font-medium py-2.5 px-3 sm:px-4 rounded-pillToken whitespace-nowrap transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md"
            >
              <Target className="w-4 h-4 shrink-0" />
              <span>Visão unificada</span>
            </TabsTrigger>
            <TabsTrigger
              value="posts"
              className="group flex items-center gap-2 text-sm font-medium py-2.5 px-3 sm:px-4 rounded-pillToken whitespace-nowrap transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md"
            >
              <LayoutGrid className="w-4 h-4 shrink-0" />
              <span>Posts por comunidade</span>
              <span className="text-muted-foreground text-xs font-normal group-data-[state=active]:text-primary-foreground/80">
                — Em breve
              </span>
            </TabsTrigger>
            <TabsTrigger
              value="announcements"
              className="group flex items-center gap-2 text-sm font-medium py-2.5 px-3 sm:px-4 rounded-pillToken whitespace-nowrap transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md"
            >
              <Megaphone className="w-4 h-4 shrink-0" />
              <span>Comunicados</span>
              <span className="text-muted-foreground text-xs font-normal group-data-[state=active]:text-primary-foreground/80">
                — Em breve
              </span>
            </TabsTrigger>
          </TabsList>
        </div>

        <TabsContent value="unified" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <BarChart3 className="w-5 h-5" />
                Alcance e engajamento — todo o conteúdo
              </CardTitle>
              <CardDescription>
                Uma visão única de todos os posts e comunicados: alcance
                estimado, interações e taxa de engajamento.
              </CardDescription>
              <div className="pt-2">
                <Input
                  placeholder="Buscar por título, comunidade ou tipo..."
                  value={searchUnified}
                  onChange={(e) => setSearchUnified(e.target.value)}
                  className="max-w-sm h-10 rounded-lg border-borderDefault"
                />
              </div>
            </CardHeader>
            <CardContent>
              {loadingResumo ? (
                <div className="flex items-center justify-center min-h-[320px] gap-2 text-secondaryText">
                  <Spinner className="w-5 h-5" />
                  <span>Carregando resumo...</span>
                </div>
              ) : (
                <>
                  <div className="rounded-lg border border-borderSoft overflow-hidden [&_.standard-table-wrapper]:border-0 [&_.standard-table-wrapper]:min-h-[320px]">
                    <DataTable<UnifiedRow>
                      columns={UNIFIED_COLUMNS}
                      data={filteredUnifiedRows}
                      keyExtractor={(row) => `${row.kind}-${row.id}`}
                      renderCell={renderUnifiedCell}
                      emptyMessage="Nenhum post ou comunicado encontrado."
                      stickyHeader={true}
                    />
                  </div>
                  <p className="mt-3 text-sm text-secondaryText">
                    Total: {filteredUnifiedRows.length} itens (posts + comunicados). Ordenação e reordenação de colunas disponíveis.
                  </p>
                </>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="posts" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <LayoutGrid className="w-5 h-5" />
                Posts por comunidade
              </CardTitle>
              <CardDescription>
                Quantidade de posts e engajamento (likes, comentários, aceites)
                por comunidade.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="rounded-lg border border-borderSoft overflow-hidden [&_.standard-table-wrapper]:border-0 [&_.standard-table-wrapper]:min-h-[200px]">
                <DataTable<PostsByCommunity>
                  columns={POSTS_BY_COMMUNITY_COLUMNS}
                  data={postsByCommunity}
                  keyExtractor={(c) => c.groupId}
                  renderCell={renderPostsByCommunityCell}
                  emptyMessage="Nenhuma comunidade com posts publicados."
                  stickyHeader={true}
                />
              </div>
              <p className="mt-3 text-sm text-secondaryText">
                Comunidades com pelo menos um post publicado.
              </p>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="announcements" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Megaphone className="w-5 h-5" />
                Resumo de comunicados
              </CardTitle>
              <CardDescription>
                Por tipo (informativo / documento), leitura obrigatória ou não,
                aceites e nível de engajamento.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                <div className="rounded-lg border border-borderSoft bg-surfaceElevated p-4 shadow-softToken">
                  <div className="flex items-center gap-2 text-secondaryText">
                    <FileText className="w-4 h-4" />
                    <span className="text-sm">Informativo</span>
                  </div>
                  <p className="text-2xl font-bold text-primaryText mt-1 tabular-nums">
                    {announcementsStats.byType.informative}
                  </p>
                </div>
                <div className="rounded-lg border border-borderSoft bg-surfaceElevated p-4 shadow-softToken">
                  <div className="flex items-center gap-2 text-secondaryText">
                    <BookMarked className="w-4 h-4" />
                    <span className="text-sm">Documento</span>
                  </div>
                  <p className="text-2xl font-bold text-primaryText mt-1 tabular-nums">
                    {announcementsStats.byType.document}
                  </p>
                </div>
                <div className="rounded-lg border border-borderSoft bg-surfaceElevated p-4 shadow-softToken">
                  <div className="flex items-center gap-2 text-secondaryText">
                    <BookOpen className="w-4 h-4" />
                    <span className="text-sm">Obrigatório leitura</span>
                  </div>
                  <p className="text-2xl font-bold text-primaryText mt-1 tabular-nums">
                    {announcementsStats.byObligatory.obligatory}
                  </p>
                </div>
                <div className="rounded-lg border border-borderSoft bg-surfaceElevated p-4 shadow-softToken">
                  <div className="flex items-center gap-2 text-secondaryText">
                    <Megaphone className="w-4 h-4" />
                    <span className="text-sm">Não obrigatório</span>
                  </div>
                  <p className="text-2xl font-bold text-primaryText mt-1 tabular-nums">
                    {announcementsStats.byObligatory.optional}
                  </p>
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <Card>
                  <CardContent className="p-4 flex items-center gap-4">
                    <div className="p-3 rounded-lg bg-success/10">
                      <CheckCircle2 className="w-6 h-6 text-success" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-primaryText tabular-nums">
                        {announcementsStats.totalAcknowledged}
                      </p>
                      <p className="text-sm text-secondaryText">Total de aceites</p>
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4 flex items-center gap-4">
                    <div className="p-3 rounded-lg bg-primary/10">
                      <Eye className="w-6 h-6 text-primary" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-primaryText tabular-nums">
                        {announcementsStats.totalViews}
                      </p>
                      <p className="text-sm text-secondaryText">Visualizações</p>
                    </div>
                  </CardContent>
                </Card>
                <Card>
                  <CardContent className="p-4 flex items-center gap-4">
                    <div className="p-3 rounded-lg bg-primary/10">
                      <BarChart3 className="w-6 h-6 text-primary" />
                    </div>
                    <div>
                      <p className="text-2xl font-bold text-primaryText tabular-nums">
                        {announcementsStats.engagementRate}%
                      </p>
                      <p className="text-sm text-secondaryText">Taxa de engajamento</p>
                    </div>
                  </CardContent>
                </Card>
              </div>
              <div className="rounded-lg border border-borderSoft overflow-hidden [&_.standard-table-wrapper]:border-0 [&_.standard-table-wrapper]:min-h-[200px]">
                <DataTable<AnnouncementRow>
                  columns={ANNOUNCEMENTS_COLUMNS}
                  data={announcementRows}
                  keyExtractor={(a) => a.id}
                  renderCell={renderAnnouncementCell}
                  emptyMessage="Nenhum comunicado publicado ou arquivado."
                  stickyHeader={true}
                />
              </div>
              <p className="mt-3 text-sm text-secondaryText">
                Comunicados publicados e arquivados. Engajamento = (likes + comentários + aceites) / visualizações.
              </p>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
