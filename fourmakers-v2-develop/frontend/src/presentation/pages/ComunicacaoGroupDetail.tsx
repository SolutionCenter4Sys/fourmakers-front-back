import { useState, useEffect, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { container } from 'tsyringe';
import {
  ArrowLeft,
  Users,
  MessageSquare,
  FileText,
  Plus,
  Paperclip,
  Download,
  UserPlus,
  LogOut,
} from 'lucide-react';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { CommunityPost, PostAttachment } from '@domain/entities/comunicacao';
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository';
import { CommunityFeed } from '@presentation/components/comunicacao/CommunityFeed';
import { CommunityMembersList } from '@presentation/components/comunicacao/CommunityMembersList';
import { CreatePostModal } from '@presentation/components/comunicacao/CreatePostModal';
import { useComunicacaoComunidadeDetalhe } from '@presentation/hooks/useComunicacaoComunidadeDetalhe';
import { useComunicacaoFeed } from '@presentation/hooks/useComunicacaoFeed';
import { comunidadeDetalheToCommunityGroup } from '@shared/utils/comunicacaoComunidadeMapper';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Spinner } from '@/components/ui/spinner';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { toast } from 'sonner';

export default function ComunicacaoGroupDetail() {
  const { groupId } = useParams<{ groupId: string }>();
  const navigate = useNavigate();
  const token = useAppSelector((state) => state.auth.token);
  const codigoColaboradorInterno = useAppSelector(
    (state) => state.auth.user?.colaborador?.codigoColaboradorInterno ?? '',
  );
  const persona = 'manager' as const;

  const { detalhe, loading: loadingDetalhe, error: errorDetalhe, loadDetalhe } = useComunicacaoComunidadeDetalhe(groupId ?? undefined);
  const {
    communityFeed,
    loadCommunityFeed,
  } = useComunicacaoFeed({ comunidadeId: groupId ?? undefined });

  const group = detalhe ? comunidadeDetalheToCommunityGroup(detalhe, token ?? undefined) : undefined;
  const [participandoLoading, setParticipandoLoading] = useState(false);
  const comunidadeRepository = container.resolve<ComunicacaoComunidadeRepository>(DiTokens.comunicacaoComunidadeRepository);

  /** Botão "Criar post": visível se membros podem postar; caso contrário, só para moderadores. */
  const isModerator =
    !!codigoColaboradorInterno &&
    (group?.moderators?.some((m) => m.userId === codigoColaboradorInterno) ?? false);
  const canCreatePost =
    !!group &&
    (group.settings.allowMemberPosts || isModerator);

  const [posts, setPosts] = useState<CommunityPost[]>([]);
  const [isCreatePostOpen, setIsCreatePostOpen] = useState(false);
  const [editPost, setEditPost] = useState<CommunityPost | null>(null);

  useEffect(() => {
    if (communityFeed?.publicacoes) {
      setPosts(communityFeed.publicacoes);
    }
  }, [communityFeed?.publicacoes]);

  const handleSavePost = async (post?: CommunityPost) => {
    if (post) {
      setPosts((prev) => [post, ...prev]);
    }
    // Sempre refaz o fetch do feed da comunidade após criar/editar para refletir no feed.
    await loadCommunityFeed(group!.id);
    setIsCreatePostOpen(false);
    setEditPost(null);
  };

  const anexosDocumentos = useMemo(() => {
    const list: { attachment: PostAttachment; postTitle?: string; authorName?: string }[] = [];
    posts.forEach((post) => {
      const docs = post.attachments?.filter((a) => a.type === 'document') ?? [];
      docs.forEach((attachment) => {
        list.push({
          attachment,
          postTitle: post.title,
          authorName: post.authorName,
        });
      });
    });
    return list;
  }, [posts]);

  const handleBack = () => {
    navigate('/comunicacao?tab=communities');
  };

  const handleParticipar = async () => {
    if (!token || !groupId) return;
    setParticipandoLoading(true);
    try {
      const res = await comunidadeRepository.participar(token, groupId);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Você entrou na comunidade.');
        await loadDetalhe();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível participar da comunidade.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao participar da comunidade.');
    } finally {
      setParticipandoLoading(false);
    }
  };

  const handleSair = async () => {
    if (!token || !groupId) return;
    setParticipandoLoading(true);
    try {
      const res = await comunidadeRepository.sair(token, groupId);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Você saiu da comunidade.');
        await loadDetalhe();
      } else {
        toast.error(res.mensagem ?? 'Não foi possível sair da comunidade.');
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao sair da comunidade.');
    } finally {
      setParticipandoLoading(false);
    }
  };

  if (!groupId) {
    return (
      <div className="space-y-4">
        <p className="text-muted-foreground">Comunidade não informada.</p>
        <Button variant="outline" onClick={handleBack}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar para comunidades
        </Button>
      </div>
    );
  }

  if (loadingDetalhe) {
    return (
      <div className="flex justify-center py-12">
        <Spinner className="h-8 w-8 text-muted-foreground" />
      </div>
    );
  }

  if (errorDetalhe || !group) {
    return (
      <div className="space-y-4">
        <p className="text-muted-foreground">
          {errorDetalhe ?? 'Comunidade não encontrada.'}
        </p>
        <Button variant="outline" onClick={handleBack}>
          <ArrowLeft className="w-4 h-4 mr-2" />
          Voltar para comunidades
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button
          variant="ghost"
          onClick={handleBack}
          className="gap-2 -ml-2"
        >
          <ArrowLeft className="w-4 h-4" />
          Voltar para comunidades
        </Button>
      </div>

      <Card className="overflow-hidden">
        <div
          className="h-32 bg-muted"
          style={
            group.coverImage
              ? {
                  backgroundImage: `url(${group.coverImage})`,
                  backgroundSize: 'cover',
                  backgroundPosition: 'center',
                }
              : undefined
          }
        />
        <CardContent className="p-4">
          <h1 className="text-xl font-semibold">{group.name}</h1>
          {group.description && (
            <p className="text-muted-foreground text-sm mt-1">
              {group.description}
            </p>
          )}
          <div className="flex flex-wrap items-center justify-between gap-3 mt-3">
            <div className="flex gap-4 text-sm text-muted-foreground">
              <span className="flex items-center gap-1">
                <Users className="w-4 h-4" />
                {group.memberCount} membros
              </span>
              <span className="flex items-center gap-1">
                <MessageSquare className="w-4 h-4" />
                {communityFeed?.quantidadeTotal ?? group.postCount} publicações
              </span>
            </div>
            <div className="flex items-center gap-2">
              {detalhe?.participando === false && (
                <Button
                  onClick={handleParticipar}
                  disabled={participandoLoading}
                  className="gap-2"
                >
                  {participandoLoading ? (
                    <Spinner className="h-4 w-4" />
                  ) : (
                    <UserPlus className="w-4 h-4" />
                  )}
                  Participar
                </Button>
              )}
              {detalhe?.participando === true && detalhe?.permiteSair === true && (
                <Button
                  variant="outline"
                  onClick={handleSair}
                  disabled={participandoLoading}
                  className="gap-2"
                >
                  {participandoLoading ? (
                    <Spinner className="h-4 w-4" />
                  ) : (
                    <LogOut className="w-4 h-4" />
                  )}
                  Sair
                </Button>
              )}
            </div>
          </div>
        </CardContent>
      </Card>

      <Tabs defaultValue="feed" className="w-full">
        <div className="flex items-center justify-between gap-4 flex-wrap">
          <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-2 shadow-sm">
            <TabsTrigger
              value="feed"
              className="flex items-center gap-2 text-sm font-medium py-2.5 px-4 rounded-pillToken transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
            >
              <MessageSquare className="h-4 w-4" />
              Feed
            </TabsTrigger>
            <TabsTrigger
              value="members"
              className="flex items-center gap-2 text-sm font-medium py-2.5 px-4 rounded-pillToken transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
            >
              <Users className="h-4 w-4" />
              Membros
            </TabsTrigger>
            <TabsTrigger
              value="documents"
              className="flex items-center gap-2 text-sm font-medium py-2.5 px-4 rounded-pillToken transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground"
            >
              <FileText className="h-4 w-4" />
              Anexos
            </TabsTrigger>
          </TabsList>
          {canCreatePost && (
            <Button className="gap-2 shrink-0" onClick={() => { setEditPost(null); setIsCreatePostOpen(true); }}>
              <Plus className="w-4 h-4" />
              Criar post
            </Button>
          )}
        </div>

        <TabsContent value="feed" className="mt-4">
          <CommunityFeed
            persona={persona}
            groupId={group.id}
            posts={posts}
            setPosts={setPosts}
            onEmptyStateCreatePost={canCreatePost ? () => { setEditPost(null); setIsCreatePostOpen(true); } : undefined}
            onEditar={(post) => {
              setEditPost(post);
              setIsCreatePostOpen(true);
            }}
          />
        </TabsContent>
        <TabsContent value="members" className="mt-4">
          <CommunityMembersList
            group={group}
            apiMembers={detalhe?.membros}
          />
        </TabsContent>
        <TabsContent value="documents" className="mt-4">
          <div className="space-y-3">
            <h2 className="text-lg font-semibold flex items-center gap-2">
              <Paperclip className="h-5 w-5 text-muted-foreground" />
              Anexos dos posts do feed
            </h2>
            {anexosDocumentos.length === 0 ? (
              <Card className="border-dashed">
                <CardContent className="py-12 text-center">
                  <FileText className="h-12 w-12 mx-auto text-muted-foreground/50 mb-3" />
                  <p className="text-muted-foreground text-sm">
                    Nenhum anexo de documento nos posts desta comunidade.
                  </p>
                </CardContent>
              </Card>
            ) : (
              <div className="space-y-2">
                {anexosDocumentos.map(({ attachment, postTitle, authorName }) => (
                  <Card key={attachment.id} className="overflow-hidden">
                    <CardContent className="p-0">
                      <a
                        href={attachment.url}
                        download={attachment.name}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="flex items-center gap-4 p-4 no-underline text-foreground hover:bg-muted/50 transition-colors"
                      >
                        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary">
                          <FileText className="h-6 w-6" />
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="font-medium truncate">{attachment.name}</p>
                          {(postTitle || authorName) && (
                            <p className="text-xs text-muted-foreground truncate mt-0.5">
                              {[postTitle, authorName].filter(Boolean).join(' · ')}
                            </p>
                          )}
                        </div>
                        {attachment.size != null && (
                          <span className="text-sm text-muted-foreground shrink-0">
                            {(attachment.size / 1024 / 1024).toFixed(2)} MB
                          </span>
                        )}
                        <Download className="h-4 w-4 shrink-0 text-muted-foreground" />
                      </a>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}
          </div>
        </TabsContent>
      </Tabs>

      <CreatePostModal
        open={isCreatePostOpen}
        onOpenChange={(open) => {
          setIsCreatePostOpen(open);
          if (!open) setEditPost(null);
        }}
        group={group}
        editPost={editPost}
        onSave={handleSavePost}
      />
    </div>
  );
}
