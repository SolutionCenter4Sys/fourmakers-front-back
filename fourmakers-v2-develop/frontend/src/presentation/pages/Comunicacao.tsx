import { useState, useEffect, useMemo } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { container } from 'tsyringe';
import {
  MessageSquare,
  Users,
  FolderOpen,
  Megaphone,
  BookOpen,
  UserCircle,
  BarChart3,
  Bell,
  Settings,
} from 'lucide-react';
import type { Announcement, CommunityPersona, PermissoesUsuarioLogado } from '@domain/entities/comunicacao';
import { ObterPermissoesUsuarioLogadoUseCase } from '@domain/usecases/ObterPermissoesUsuarioLogadoUseCase';
import { useAppSelector } from '@app/store/hooks';
import { mockUserGroups } from '@data/mocks/comunicacao/userGroupsData';
import { useComunicacaoAnnouncements } from '@presentation/hooks/useComunicacaoAnnouncements';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { NotificationSettings } from '@presentation/components/comunicacao/NotificationSettings';
import { CommunityFeed } from '@presentation/components/comunicacao/CommunityFeed';
import { GroupsList } from '@presentation/components/comunicacao/GroupsList';
import { ComunicacaoGruposList } from '@presentation/components/comunicacao/ComunicacaoGruposList';
import { AnnouncementsView } from '@presentation/components/comunicacao/AnnouncementsView';
import { LibraryView } from '@presentation/components/comunicacao/LibraryView';
import { ProfessionalsView } from '@presentation/components/comunicacao/ProfessionalsView';
import { AnalyticsDashboard } from '@presentation/components/comunicacao/AnalyticsDashboard';

const tabConfig: { value: string; label: string; icon: React.ElementType; title: string; description: string }[] = [
  { value: 'feed', label: 'Feed', icon: MessageSquare, title: 'Feed', description: 'Publicações e comunicados em destaque' },
  { value: 'communities', label: 'Comunidades', icon: Users, title: 'Comunidades', description: 'Comunidades e grupos de comunicação' },
  { value: 'groups', label: 'Grupos de Usuários', icon: FolderOpen, title: 'Grupos de Usuários', description: 'Gerencie grupos de usuários vinculados às comunidades' },
  { value: 'announcements', label: 'Comunicados', icon: Megaphone, title: 'Comunicados', description: 'Comunicados oficiais por grupo de usuários' },
  { value: 'library', label: 'Documentos', icon: BookOpen, title: 'Documentos', description: 'Documentos e materiais da comunicação' },
  { value: 'professionals', label: 'Profissionais', icon: UserCircle, title: 'Profissionais', description: 'Diretório de profissionais' },
  { value: 'analytics', label: 'Analytics', icon: BarChart3, title: 'Analytics', description: 'Métricas e relatórios' },
];

export default function Comunicacao() {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = useAppSelector((state) => state.auth.token);
  const obterPermissoesUseCase = container.resolve(ObterPermissoesUsuarioLogadoUseCase);
  const persona: CommunityPersona = 'manager';
  const [userGroups, setUserGroups] = useState(mockUserGroups);
  const [permissoes, setPermissoes] = useState<PermissoesUsuarioLogado | null>(null);

  useEffect(() => {
    if (!token) return;
    obterPermissoesUseCase
      .execute(token)
      .then(setPermissoes)
      .catch(() => setPermissoes(null));
  }, [token, obterPermissoesUseCase]);

  const visibleTabValues = useMemo(() => {
    const base = tabConfig.map((t) => t.value);
    if (!permissoes) return base;
    return base.filter((value) => {
      if (value === 'groups') return permissoes.gestaoComunicados;
      if (value === 'announcements') {
        return (
          permissoes.publicacaoInformativoRequerAprovacao ||
          permissoes.aprovaPublicacaoInformativo ||
          permissoes.permiteCriarPublicacaoInformativo
        );
      }
      return true;
    });
  }, [permissoes]);

  const tabParam = searchParams.get('tab');
  const initialTab =
    tabParam === 'scheduled' ? 'announcements' : (tabConfig.some((t) => t.value === tabParam) ? tabParam! : 'feed');
  const [activeTab, setActiveTab] = useState(initialTab);

  useEffect(() => {
    if (permissoes !== null && !visibleTabValues.includes(activeTab)) {
      const fallback = visibleTabValues[0] ?? 'feed';
      setActiveTab(fallback);
      setSearchParams({ tab: fallback });
    }
  }, [permissoes, visibleTabValues, activeTab, setSearchParams]);

  const { posts: apiPosts, announcements: apiAnnouncements, loading: loadingAnnouncements, loadingMore: loadingMoreAnnouncements, hasMore: hasMoreAnnouncements, error: announcementsError, refetch: refetchAnnouncements, loadMore: loadMoreAnnouncements } = useComunicacaoAnnouncements({
    enabled: activeTab === 'announcements',
  });
  const [announcements, setAnnouncements] = useState<Announcement[]>([]);

  useEffect(() => {
    setAnnouncements(apiAnnouncements);
  }, [apiAnnouncements]);

  const currentConfig = tabConfig.find((t) => t.value === activeTab) ?? tabConfig[0];

  const handleTabChange = (value: string) => {
    setActiveTab(value);
    setSearchParams({ tab: value });
  };

  const handleSelectGroup = (group: { id: string }) => {
    navigate(`/comunicacao/grupo/${group.id}`);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between gap-4">
        <div className="flex flex-col gap-2 min-w-0">
          <h1 className="text-2xl font-semibold tracking-tight">{currentConfig.title}</h1>
          <p className="text-muted-foreground text-sm">{currentConfig.description}</p>
        </div>
        <NotificationSettings
          trigger={
            <button
              type="button"
              aria-label="Configurações de notificações"
              className="inline-flex items-center justify-center gap-2 h-9 px-3 rounded-pillToken border border-border/50 bg-transparent text-foreground hover:bg-muted/30 transition-colors shrink-0"
            >
              <Bell className="h-4 w-4 shrink-0" />
              <Settings className="h-4 w-4 shrink-0" />
            </button>
          }
        />
      </div>

      <Tabs value={activeTab} onValueChange={handleTabChange} className="w-full">
        <div className="flex items-center justify-between gap-4 mb-4">
          <div className="flex-1 min-w-0 overflow-x-auto">
            <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-2 w-auto min-w-max shadow-sm">
              {tabConfig
                .filter((t) => visibleTabValues.includes(t.value))
                .map(({ value, label, icon: Icon }) => (
                  <TabsTrigger
                    key={value}
                    value={value}
                    className="relative flex items-center gap-2 text-sm font-medium py-2.5 px-4 rounded-pillToken transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
                  >
                    <Icon className="h-4 w-4 shrink-0" />
                    <span className="truncate">{label}</span>
                  </TabsTrigger>
                ))}
            </TabsList>
          </div>
        </div>

        <TabsContent value="feed" className="mt-0">
          <CommunityFeed persona={persona} />
        </TabsContent>
        <TabsContent value="communities" className="mt-0">
          <GroupsList persona={persona} onSelectGroup={handleSelectGroup} />
        </TabsContent>
        <TabsContent value="groups" className="mt-0">
          <ComunicacaoGruposList
            userGroups={userGroups}
            setUserGroups={setUserGroups}
          />
        </TabsContent>
        <TabsContent value="announcements" className="mt-0">
          <AnnouncementsView
            persona={persona}
            posts={apiPosts}
            announcements={announcements}
            setAnnouncements={setAnnouncements}
            userGroups={userGroups}
            loading={loadingAnnouncements}
            loadingMore={loadingMoreAnnouncements}
            hasMore={hasMoreAnnouncements}
            onLoadMore={loadMoreAnnouncements}
            error={announcementsError}
            onRefetch={refetchAnnouncements}
            permiteCriarPublicacaoInformativo={permissoes?.permiteCriarPublicacaoInformativo ?? false}
            aprovaPublicacaoInformativo={permissoes?.aprovaPublicacaoInformativo ?? false}
          />
        </TabsContent>
        <TabsContent value="library" className="mt-0">
          {activeTab === 'library' && (
            <LibraryView
              persona={persona}
              verAceitesVisivelNoPreview={
                !!permissoes &&
                (permissoes.publicacaoInformativoRequerAprovacao ||
                  permissoes.aprovaPublicacaoInformativo ||
                  permissoes.permiteCriarPublicacaoInformativo)
              }
            />
          )}
        </TabsContent>
        <TabsContent value="professionals" className="mt-0">
          <ProfessionalsView />
        </TabsContent>
        <TabsContent value="analytics" className="mt-0">
          <AnalyticsDashboard />
        </TabsContent>
      </Tabs>
    </div>
  );
}
