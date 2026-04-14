import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { ListarComunicacaoFeedUseCase } from '@domain/usecases/ListarComunicacaoFeedUseCase';
import { OcultarNoFeedPublicacaoUseCase } from '@domain/usecases/OcultarNoFeedPublicacaoUseCase';
import { communityPostToLibraryDocument } from '@shared/utils/comunicacaoFeedMapper';
import {
  FileText,
  Search,
  Filter,
  Grid,
  List,
  CheckCircle2,
  AlertCircle,
  User,
  FolderOpen,
  Pencil,
  Trash2,
  Settings2,
  ArrowLeft,
  Calendar,
  MoreVertical,
  EyeOff,
  Eye,
} from 'lucide-react';
import type { LibraryDocument, CommunityPersona, CommunityPost } from '@domain/entities/comunicacao';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { RequiredItemDetailModal } from './RequiredItemDetailModal';
import { useComunicacaoBibliotecaLabels } from '@presentation/hooks/useComunicacaoBibliotecaLabels';
import { format, formatDistanceToNow } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { toast } from 'sonner';

interface LibraryViewProps {
  persona: CommunityPersona;
  /** Quando true, o "Ver aceites" no preview do documento fica visível (mesmas regras da aba Comunicados). */
  verAceitesVisivelNoPreview?: boolean;
}

type ViewMode = 'grid' | 'list';
type AcknowledgmentFilter = 'all' | 'required' | 'acknowledged' | 'pending';

/** Montado apenas quando a aba Documentos está ativa. */
export function LibraryView({ persona, verAceitesVisivelNoPreview = false }: LibraryViewProps) {
  const token = useAppSelector((state) => state.auth.token);
  const listarComunicacaoFeedUseCase = container.resolve(ListarComunicacaoFeedUseCase);
  const ocultarNoFeedPublicacaoUseCase = container.resolve(OcultarNoFeedPublicacaoUseCase);
  const {
    labels,
    totals,
    folderStatsFromApi,
    loading: loadingLabels,
    error: labelsError,
    updateLabel,
    deleteLabel,
  } = useComunicacaoBibliotecaLabels();

  const [searchTerm, setSearchTerm] = useState('');
  const [viewMode, setViewMode] = useState<ViewMode>('grid');
  const [acknowledgmentFilter, setAcknowledgmentFilter] =
    useState<AcknowledgmentFilter>('all');
  const [selectedFolderPath, setSelectedFolderPath] = useState<string | null>(null);
  const [selectedDocument, setSelectedDocument] = useState<CommunityPost | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);
  const [foldersOpen, setFoldersOpen] = useState(false);
  const [isRenameFolderOpen, setIsRenameFolderOpen] = useState(false);
  const [folderToRename, setFolderToRename] = useState<string | null>(null);
  const [folderToRenameLabelId, setFolderToRenameLabelId] = useState<string | null>(null);
  const [renameFolderNewPath, setRenameFolderNewPath] = useState('');
  const [isLabelActionLoading, setIsLabelActionLoading] = useState(false);

  /** Documentos da pasta atual quando ela é uma label da API (carregados via ObterListaPublicacaoGeral por label). */
  const [folderDocuments, setFolderDocuments] = useState<LibraryDocument[]>([]);
  const [folderPosts, setFolderPosts] = useState<CommunityPost[]>([]);
  const [folderDocumentsForPath, setFolderDocumentsForPath] = useState<string | null>(null);
  const [folderDocumentsLoading, setFolderDocumentsLoading] = useState(false);
  const [folderDocumentsError, setFolderDocumentsError] = useState<string | null>(null);
  /** Modal de ocultar/mostrar no feed: { doc, ocultarNoFeed } onde ocultarNoFeed é o valor a enviar na API (true = ocultar, false = mostrar). */
  const [ocultarNoFeedModal, setOcultarNoFeedModal] = useState<{
    doc: LibraryDocument;
    ocultarNoFeed: boolean;
  } | null>(null);
  const [ocultarNoFeedLoading, setOcultarNoFeedLoading] = useState(false);

  const canManageFolders = persona === 'manager' || persona === 'analytics';

  const loadFolderDocuments = useCallback(
    async (folderPath: string) => {
      if (!token) return;
      setFolderDocumentsLoading(true);
      setFolderDocumentsError(null);
      setFolderDocumentsForPath(folderPath);
      try {
        const result = await listarComunicacaoFeedUseCase.execute(token, {
          somenteLeituraObrigatoria: false,
          tipo: 'documento',
          labels: [folderPath],
          comunidadeId: '',
        });
        const posts = result.publicacoes ?? [];
        setFolderPosts(posts);
        setFolderDocuments(posts.map((p) => communityPostToLibraryDocument(p, folderPath)));
      } catch (e) {
        setFolderDocumentsError(e instanceof Error ? e.message : 'Falha ao carregar documentos.');
        setFolderDocuments([]);
        setFolderPosts([]);
      } finally {
        setFolderDocumentsLoading(false);
      }
    },
    [token, listarComunicacaoFeedUseCase],
  );

  useEffect(() => {
    if (selectedFolderPath === null) {
      setFolderDocumentsForPath(null);
      setFolderDocuments([]);
      setFolderPosts([]);
      setFolderDocumentsError(null);
      return;
    }
    const isApiLabel = labels.some((l) => l.nome === selectedFolderPath);
    if (isApiLabel) {
      loadFolderDocuments(selectedFolderPath);
    } else {
      setFolderDocumentsForPath(null);
      setFolderDocuments([]);
      setFolderPosts([]);
    }
  }, [selectedFolderPath, labels, loadFolderDocuments]);

  const convertDocumentToPost = (doc: LibraryDocument): CommunityPost => {
    return {
      id: doc.postId,
      groupId: doc.groupId,
      groupName: doc.groupName,
      authorId: doc.authorId,
      authorName: doc.authorName,
      type: 'document',
      title: doc.name,
      content: '<p>Documento disponível para visualização e download.</p>',
      attachments: [
        {
          id: doc.id,
          type: 'document',
          url: doc.url,
          name: doc.name,
          size: doc.size,
          mimeType: doc.type,
        },
      ],
      folderPath: doc.folderPath,
      status: 'published',
      publishedAt: doc.createdAt,
      visibility: { type: 'all' },
      requiresAcknowledgment: doc.requiresAcknowledgment,
      allowComments: true,
      allowLikes: true,
      isPinned: false,
      likesCount: 0,
      commentsCount: 0,
      viewsCount: 0,
      acknowledgmentCount: 0,
      createdAt: doc.createdAt,
      updatedAt: doc.createdAt,
      expiresAt: doc.expiresAt,
    };
  };

  const handleDocumentClick = (doc: LibraryDocument) => {
    const isFromApiFolder = selectedFolderPath === folderDocumentsForPath;
    const postFromApi = isFromApiFolder ? folderPosts.find((p) => p.id === doc.id) : null;
    const post = postFromApi ?? convertDocumentToPost(doc);
    setSelectedDocument(post);
    setIsDetailModalOpen(true);
  };

  /** Documentos a exibir: carregados pela API ao selecionar uma pasta (label). */
  const baseDocumentsForFolder =
    selectedFolderPath === folderDocumentsForPath ? folderDocuments : [];

  const filteredDocuments = baseDocumentsForFolder.filter((doc) => {
    const matchesSearch =
      doc.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      doc.authorName.toLowerCase().includes(searchTerm.toLowerCase());
    let matchesAcknowledgment = true;
    if (acknowledgmentFilter === 'required') matchesAcknowledgment = doc.requiresAcknowledgment;
    if (acknowledgmentFilter === 'acknowledged') matchesAcknowledgment = doc.isAcknowledged;
    if (acknowledgmentFilter === 'pending')
      matchesAcknowledgment = doc.requiresAcknowledgment && !doc.isAcknowledged;
    return matchesSearch && matchesAcknowledgment;
  });

  /** Pastas da biblioteca = labels retornadas pela API (ObterListaPublicacaoGeral). */
  const uniqueFolders =
    labels.length > 0 ? [...labels.map((l) => l.nome)].sort() : [];

  const docCountByFolder = (path: string) => {
    const label = labels.find((l) => l.nome === path);
    return label?.quantidadeDocumentos ?? 0;
  };

  /** Lidos/não lidos por pasta: stats da API (publicações por label). */
  const readUnreadByFolder = (path: string) => {
    const fromApi = folderStatsFromApi[path];
    return fromApi
      ? { read: fromApi.lidos, unread: fromApi.pendentes }
      : { read: 0, unread: 0 };
  };

  const handleRenameFolder = async () => {
    if (!folderToRename || !renameFolderNewPath.trim()) return;
    const newPath = renameFolderNewPath
      .trim()
      .replace(/\/+/g, '/')
      .replace(/^\//, '');
    if (newPath === folderToRename) {
      setIsRenameFolderOpen(false);
      setFolderToRename(null);
      setFolderToRenameLabelId(null);
      return;
    }
    const otherNames =
      folderToRenameLabelId != null
        ? labels.filter((l) => l.id !== folderToRenameLabelId).map((l) => l.nome)
        : uniqueFolders;
    if (otherNames.includes(newPath)) {
      toast.error('Já existe uma pasta com esse nome');
      return;
    }
    if (folderToRenameLabelId != null) {
      setIsLabelActionLoading(true);
      try {
        await updateLabel(folderToRenameLabelId, newPath);
        setFolderToRename(null);
        setFolderToRenameLabelId(null);
        setRenameFolderNewPath('');
        setIsRenameFolderOpen(false);
        toast.success(`Pasta renomeada: "${folderToRename}" → "${newPath}"`);
      } catch (e) {
        toast.error(e instanceof Error ? e.message : 'Falha ao renomear pasta.');
      } finally {
        setIsLabelActionLoading(false);
      }
      return;
    }
    setFolderToRename(null);
    setFolderToRenameLabelId(null);
    setRenameFolderNewPath('');
    setIsRenameFolderOpen(false);
  };

  const handleDeleteLabel = async (label: { id: string; nome: string; quantidadeDocumentos: number }) => {
    if (label.quantidadeDocumentos > 0) {
      toast.error(
        `Não é possível excluir. A pasta tem ${label.quantidadeDocumentos} documento(s). Remova os documentos antes.`,
      );
      return;
    }
    setIsLabelActionLoading(true);
    try {
      await deleteLabel(label.id);
      toast.success(`Pasta "${label.nome}" foi excluída.`);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Falha ao excluir pasta.');
    } finally {
      setIsLabelActionLoading(false);
    }
  };

  const openRenameModalForLabel = (label: { id: string; nome: string }) => {
    setFolderToRenameLabelId(label.id);
    setFolderToRename(label.nome);
    setRenameFolderNewPath(label.nome);
    setIsRenameFolderOpen(true);
  };

  const handleConfirmOcultarNoFeed = async () => {
    if (!token || !ocultarNoFeedModal || !selectedFolderPath) return;
    setOcultarNoFeedLoading(true);
    try {
      const res = await ocultarNoFeedPublicacaoUseCase.execute(token, {
        publicacaoId: ocultarNoFeedModal.doc.id,
        ocultarNoFeed: ocultarNoFeedModal.ocultarNoFeed,
      });
      if (res.sucesso) {
        toast.success(
          ocultarNoFeedModal.ocultarNoFeed
            ? (res.mensagem ?? 'Documento ocultado do feed.')
            : (res.mensagem ?? 'Documento exibido no feed novamente.'),
        );
        setOcultarNoFeedModal(null);
        await loadFolderDocuments(selectedFolderPath);
      } else {
        toast.error(res.mensagem ?? (ocultarNoFeedModal.ocultarNoFeed ? 'Não foi possível ocultar do feed.' : 'Não foi possível exibir no feed.'));
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Erro ao atualizar visibilidade no feed.');
    } finally {
      setOcultarNoFeedLoading(false);
    }
  };

  /** Big numbers: totais da API (ObterListaPublicacaoGeral, tipo documento). */
  const totalCount = totals.quantidadeTotal;
  const totalLeituraObrigatoria =
    totals.quantidadePendentesLeitura + totals.quantidadeLidosAceitos;
  const totalLidoCount = totals.quantidadeLidosAceitos;
  const pendingCount = totals.quantidadePendentesLeitura;

  return (
    <div className="space-y-6">
      {/* Header com Estatísticas — refletem totais da API (tipo documento, com e sem leitura obrigatória) */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4 flex items-center gap-4">
            <div className="p-3 rounded-lg bg-primary/10">
              <FileText className="w-6 h-6 text-primary" />
            </div>
            <div>
              <p className="text-2xl font-bold">{totalCount}</p>
              <p className="text-sm text-muted-foreground">Total de documentos</p>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 flex items-center gap-4">
            <div className="p-3 rounded-lg bg-muted">
              <FileText className="w-6 h-6 text-muted-foreground" />
            </div>
            <div>
              <p className="text-2xl font-bold">{totalLeituraObrigatoria}</p>
              <p className="text-sm text-muted-foreground">
                Leitura obrigatória
              </p>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4 flex items-center gap-4">
            <div className="p-3 rounded-lg bg-success/10">
              <CheckCircle2 className="w-6 h-6 text-success" />
            </div>
            <div>
              <p className="text-2xl font-bold">{totalLidoCount}</p>
              <p className="text-sm text-muted-foreground">Total lido</p>
            </div>
          </CardContent>
        </Card>
        <Card
          className={pendingCount > 0 ? 'border-warning/50' : ''}
        >
          <CardContent className="p-4 flex items-center gap-4">
            <div
              className={`p-3 rounded-lg ${
                pendingCount > 0 ? 'bg-warning/10' : 'bg-muted'
              }`}
            >
              <AlertCircle
                className={`w-6 h-6 ${
                  pendingCount > 0 ? 'text-warning' : 'text-muted-foreground'
                }`}
              />
            </div>
            <div>
              <p className="text-2xl font-bold">{pendingCount}</p>
              <p className="text-sm text-muted-foreground">Pendentes</p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Filtros: só quando está dentro de uma pasta */}
      {selectedFolderPath !== null && (
        <div className="flex flex-wrap items-center gap-4">
          <Button
            variant="ghost"
            className="gap-2 rounded-lg"
            onClick={() => setSelectedFolderPath(null)}
          >
            <ArrowLeft className="w-4 h-4" />
            Voltar para pastas
          </Button>
          <div className="relative flex-1 min-w-[200px] max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Buscar documentos..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 rounded-lg"
            />
          </div>
          <Select
            value={acknowledgmentFilter}
            onValueChange={(v) =>
              setAcknowledgmentFilter(v as AcknowledgmentFilter)
            }
          >
            <SelectTrigger className="w-[180px] rounded-lg">
              <Filter className="w-4 h-4 mr-2" />
              <SelectValue placeholder="Status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Todos</SelectItem>
              <SelectItem value="required">Obrigatórios</SelectItem>
              <SelectItem value="acknowledged">Aceitos</SelectItem>
              <SelectItem value="pending">Pendentes</SelectItem>
            </SelectContent>
          </Select>
          <div className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-pillToken gap-2 shadow-sm ml-auto">
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setViewMode('grid')}
              className={`rounded-pillToken h-8 w-8 transition-all duration-200 ${
                viewMode === 'grid'
                  ? 'bg-primary text-primary-foreground shadow-md scale-[1.02] hover:bg-primary hover:text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground hover:bg-muted/50'
              }`}
            >
              <Grid className="w-4 h-4" />
            </Button>
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setViewMode('list')}
              className={`rounded-pillToken h-8 w-8 transition-all duration-200 ${
                viewMode === 'list'
                  ? 'bg-primary text-primary-foreground shadow-md scale-[1.02] hover:bg-primary hover:text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground hover:bg-muted/50'
              }`}
            >
              <List className="w-4 h-4" />
            </Button>
          </div>
        </div>
      )}

      {/* Gestão de pastas (manager/analytics) */}
      {canManageFolders && (
        <Collapsible open={foldersOpen} onOpenChange={setFoldersOpen}>
          <Card className="rounded-lg">
            <CollapsibleTrigger asChild>
              <CardContent className="p-4 flex items-center justify-between cursor-pointer hover:bg-muted/50 transition-colors">
                <div className="flex items-center gap-3">
                  <div className="p-2 rounded-lg bg-primary/10">
                    <Settings2 className="w-5 h-5 text-primary" />
                  </div>
                  <div>
                    <p className="font-semibold">Gestão de pastas</p>
                    <p className="text-sm text-muted-foreground">
                      Renomear e excluir pastas da biblioteca
                    </p>
                  </div>
                </div>
              </CardContent>
            </CollapsibleTrigger>
            <CollapsibleContent>
              <CardContent className="pt-0 pb-4">
                <div className="flex flex-col gap-2">
                  {labels.length > 0 ? (
                    labels.map((label) => {
                      const isEmpty = label.quantidadeDocumentos === 0;
                      return (
                        <div
                          key={label.id}
                          className="flex items-center justify-between gap-4 py-2 px-3 rounded-lg hover:bg-muted/50"
                        >
                          <div className="flex items-center gap-2 min-w-0">
                            <FolderOpen className="w-4 h-4 text-muted-foreground flex-shrink-0" />
                            <span className="truncate font-medium">{label.nome}</span>
                            <Badge
                              variant="secondary"
                              className="text-xs flex-shrink-0 rounded-lg"
                            >
                              {label.quantidadeDocumentos}{' '}
                              {label.quantidadeDocumentos === 1 ? 'documento' : 'documentos'}
                            </Badge>
                          </div>
                          <DropdownMenu>
                            <DropdownMenuTrigger
                              asChild
                              onClick={(e) => e.stopPropagation()}
                              disabled={isLabelActionLoading}
                            >
                              <Button
                                variant="ghost"
                                size="icon"
                                className="h-8 w-8 rounded-lg"
                              >
                                <Pencil className="w-4 h-4" />
                              </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end">
                              <DropdownMenuItem
                                onClick={() => openRenameModalForLabel(label)}
                                className="gap-2"
                                disabled={isLabelActionLoading}
                              >
                                <Pencil className="w-4 h-4" />
                                Renomear
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={() => handleDeleteLabel(label)}
                                className="gap-2 text-destructive focus:text-destructive"
                                disabled={!isEmpty || isLabelActionLoading}
                              >
                                <Trash2 className="w-4 h-4" />
                                Excluir
                                {!isEmpty && ' (remova os documentos antes)'}
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        </div>
                      );
                    })
                  ) : (
                    <p className="text-sm text-muted-foreground py-4 text-center">
                      Nenhuma pasta ainda. Use pastas ao publicar documentos nas
                      comunidades.
                    </p>
                  )}
                </div>
              </CardContent>
            </CollapsibleContent>
          </Card>
        </Collapsible>
      )}

      {/* Lista de pastas (vista inicial) — populada por ObterListaPublicacaoGeral (labels) */}
      {selectedFolderPath === null && (
        <>
          {loadingLabels ? (
            <Card className="rounded-lg">
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">Carregando pastas...</p>
              </CardContent>
            </Card>
          ) : labelsError ? (
            <Card className="rounded-lg border-destructive/50">
              <CardContent className="py-12 text-center">
                <p className="text-destructive">{labelsError}</p>
              </CardContent>
            </Card>
          ) : uniqueFolders.length === 0 ? (
            <Card className="rounded-lg">
              <CardContent className="py-12 text-center">
                <FolderOpen className="w-12 h-12 mx-auto text-muted-foreground/50 mb-3" />
                <p className="text-muted-foreground">Nenhuma pasta ainda.</p>
                <p className="text-sm text-muted-foreground mt-1">
                  Use pastas ao publicar documentos nas comunidades.
                </p>
              </CardContent>
            </Card>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {uniqueFolders.map((path) => {
                const count = docCountByFolder(path);
                const { read, unread } = readUnreadByFolder(path);
                return (
                  <Card
                    key={path}
                    className="hover:shadow-md transition-all cursor-pointer h-full rounded-lg"
                    onClick={() => setSelectedFolderPath(path)}
                  >
                    <CardContent className="p-5 flex flex-col items-center text-center">
                      <div className="p-4 rounded-xl bg-primary/10 mb-3">
                        <FolderOpen className="w-8 h-8 text-primary" />
                      </div>
                      <p
                        className="font-medium truncate w-full"
                        title={path}
                      >
                        {path}
                      </p>
                      <p className="text-sm text-muted-foreground mt-1">
                        {count}{' '}
                        {count === 1 ? 'documento' : 'documentos'}
                      </p>
                      {count > 0 && (
                        <p className="text-xs text-muted-foreground mt-1.5 flex items-center justify-center gap-1.5 flex-wrap">
                          <span className="text-success">{read} lidos</span>
                          <span className="opacity-50">·</span>
                          <span
                            className={
                              unread > 0
                                ? 'text-warning'
                                : 'text-muted-foreground'
                            }
                          >
                            {unread} não lidos
                          </span>
                        </p>
                      )}
                    </CardContent>
                  </Card>
                );
              })}
            </div>
          )}
        </>
      )}

      {/* Lista de Documentos (grid) — carregados por label ao abrir pasta da API */}
      {selectedFolderPath !== null && viewMode === 'grid' && (
        <>
          {selectedFolderPath === folderDocumentsForPath && folderDocumentsLoading && (
            <Card className="rounded-lg">
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">Carregando documentos da pasta...</p>
              </CardContent>
            </Card>
          )}
          {selectedFolderPath === folderDocumentsForPath && folderDocumentsError && (
            <Card className="rounded-lg border-destructive/50">
              <CardContent className="py-12 text-center">
                <p className="text-destructive">{folderDocumentsError}</p>
              </CardContent>
            </Card>
          )}
          {!(selectedFolderPath === folderDocumentsForPath && (folderDocumentsLoading || folderDocumentsError)) && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filteredDocuments.map((doc) => (
            <Card
              key={doc.id}
              className={`relative hover:shadow-md transition-all cursor-pointer rounded-lg ${
                doc.requiresAcknowledgment && !doc.isAcknowledged
                  ? 'border-warning/50'
                  : ''
              }`}
              onClick={() => handleDocumentClick(doc)}
            >
              <div
                className="absolute top-2 right-2 z-10"
                onClick={(e) => e.stopPropagation()}
              >
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-8 w-8 rounded-full"
                      aria-label="Ações do documento"
                    >
                      <MoreVertical className="w-4 h-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    {doc.ocultarNoFeed ? (
                      <DropdownMenuItem
                        onClick={() => setOcultarNoFeedModal({ doc, ocultarNoFeed: false })}
                      >
                        <Eye className="w-4 h-4 mr-2" />
                        Mostrar no feed
                      </DropdownMenuItem>
                    ) : (
                      <DropdownMenuItem
                        onClick={() => setOcultarNoFeedModal({ doc, ocultarNoFeed: true })}
                      >
                        <EyeOff className="w-4 h-4 mr-2" />
                        Ocultar no feed
                      </DropdownMenuItem>
                    )}
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
              <CardContent className="p-4">
                <div className="flex items-start gap-3">
                  <div
                    className={`p-3 rounded-lg ${
                      doc.requiresAcknowledgment && !doc.isAcknowledged
                        ? 'bg-warning/10'
                        : 'bg-primary/10'
                    }`}
                  >
                    <FileText
                      className={`w-6 h-6 ${
                        doc.requiresAcknowledgment && !doc.isAcknowledged
                          ? 'text-warning'
                          : 'text-primary'
                      }`}
                    />
                  </div>
                  <div className="flex-1 min-w-0 pr-8">
                    <p className="font-medium truncate">{doc.name}</p>
                    <div className="flex items-center gap-2 mt-1 text-xs text-muted-foreground">
                      <User className="w-3 h-3" />
                      <span>{doc.authorName}</span>
                    </div>
                    <div className="flex items-center gap-2 mt-1 text-xs text-muted-foreground">
                      <FolderOpen className="w-3 h-3" />
                      <span>{doc.folderPath || doc.groupName}</span>
                    </div>
                    {doc.expiresAt && (() => {
                      const d = doc.expiresAt ? new Date(doc.expiresAt) : null;
                      return d && !Number.isNaN(d.getTime()) ? (
                        <div className="flex items-center gap-2 mt-1 text-xs text-muted-foreground">
                          <Calendar className="w-3 h-3" />
                          <span>Validade: {format(d, 'dd/MM/yyyy', { locale: ptBR })}</span>
                        </div>
                      ) : null;
                    })()}
                  </div>
                </div>
                <div className="flex items-center mt-4 pt-3 border-t flex-wrap gap-2">
                  {doc.ocultarNoFeed && (
                    <Badge variant="secondary" className="rounded-lg">
                      Oculto no feed
                    </Badge>
                  )}
                  {doc.requiresAcknowledgment && (
                    <Badge
                      className={
                        doc.isAcknowledged
                          ? 'bg-success/10 text-success border-success/30 rounded-lg'
                          : 'bg-warning/10 text-warning border-warning/30 rounded-lg'
                      }
                    >
                      {doc.isAcknowledged ? (
                        <>
                          <CheckCircle2 className="w-3 h-3 mr-1" />
                          Aceito
                        </>
                      ) : (
                        <>
                          <AlertCircle className="w-3 h-3 mr-1" />
                          Pendente
                        </>
                      )}
                    </Badge>
                  )}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
          )}
        </>
      )}

      {/* Lista de Documentos (list) */}
      {selectedFolderPath !== null && viewMode === 'list' && (
        <>
          {selectedFolderPath === folderDocumentsForPath && folderDocumentsLoading && (
            <Card className="rounded-lg">
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">Carregando documentos da pasta...</p>
              </CardContent>
            </Card>
          )}
          {selectedFolderPath === folderDocumentsForPath && folderDocumentsError && (
            <Card className="rounded-lg border-destructive/50">
              <CardContent className="py-12 text-center">
                <p className="text-destructive">{folderDocumentsError}</p>
              </CardContent>
            </Card>
          )}
          {!(selectedFolderPath === folderDocumentsForPath && (folderDocumentsLoading || folderDocumentsError)) && (
        <Card className="rounded-lg">
          <CardContent className="p-0">
            <div className="divide-y">
              {filteredDocuments.map((doc) => (
                <div
                  key={doc.id}
                  className="flex items-center gap-4 p-4 hover:bg-muted/50 transition-colors cursor-pointer"
                  onClick={() => handleDocumentClick(doc)}
                >
                  <div
                    className={`p-2 rounded-lg ${
                      doc.requiresAcknowledgment && !doc.isAcknowledged
                        ? 'bg-warning/10'
                        : 'bg-primary/10'
                    }`}
                  >
                    <FileText
                      className={`w-5 h-5 ${
                        doc.requiresAcknowledgment && !doc.isAcknowledged
                          ? 'text-warning'
                          : 'text-primary'
                      }`}
                    />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-medium truncate">{doc.name}</p>
                    <div className="flex items-center gap-4 text-xs text-muted-foreground mt-1 flex-wrap">
                      <span>{doc.authorName}</span>
                      <span className="flex items-center gap-1">
                        <FolderOpen className="w-3 h-3" />
                        {doc.folderPath || doc.groupName}
                      </span>
                      {doc.expiresAt && (() => {
                        const d = new Date(doc.expiresAt);
                        return !Number.isNaN(d.getTime()) ? (
                          <span className="flex items-center gap-1">
                            <Calendar className="w-3 h-3" />
                            Validade: {format(d, 'dd/MM/yyyy', { locale: ptBR })}
                          </span>
                        ) : null;
                      })()}
                      <span>
                        {formatDistanceToNow(new Date(doc.createdAt), {
                          addSuffix: true,
                          locale: ptBR,
                        })}
                      </span>
                    </div>
                  </div>
                  <div className="flex items-center gap-2 shrink-0" onClick={(e) => e.stopPropagation()}>
                    {doc.ocultarNoFeed && (
                      <Badge variant="secondary" className="rounded-lg">
                        Oculto no feed
                      </Badge>
                    )}
                    {doc.requiresAcknowledgment && (
                      <Badge
                        className={
                          doc.isAcknowledged
                            ? 'bg-success/10 text-success border-success/30 rounded-lg'
                            : 'bg-warning/10 text-warning border-warning/30 rounded-lg'
                        }
                      >
                        {doc.isAcknowledged ? 'Aceito' : 'Pendente'}
                      </Badge>
                    )}
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 rounded-full"
                          aria-label="Ações do documento"
                        >
                          <MoreVertical className="w-4 h-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        {doc.ocultarNoFeed ? (
                          <DropdownMenuItem
                            onClick={() => setOcultarNoFeedModal({ doc, ocultarNoFeed: false })}
                          >
                            <Eye className="w-4 h-4 mr-2" />
                            Mostrar no feed
                          </DropdownMenuItem>
                        ) : (
                          <DropdownMenuItem
                            onClick={() => setOcultarNoFeedModal({ doc, ocultarNoFeed: true })}
                          >
                            <EyeOff className="w-4 h-4 mr-2" />
                            Ocultar no feed
                          </DropdownMenuItem>
                        )}
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
          )}
        </>
      )}

      {selectedFolderPath !== null &&
        !folderDocumentsLoading &&
        !folderDocumentsError &&
        filteredDocuments.length === 0 && (
        <div className="text-center py-12">
          <FileText className="w-12 h-12 mx-auto text-muted-foreground/50 mb-3" />
          <p className="text-muted-foreground">Nenhum documento encontrado</p>
        </div>
      )}

      {/* Modal Renomear Pasta */}
      <Dialog
        open={isRenameFolderOpen}
        onOpenChange={(open) => {
          setIsRenameFolderOpen(open);
          if (!open) setFolderToRenameLabelId(null);
        }}
      >
        <DialogContent className="rounded-lg">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Pencil className="w-5 h-5" />
              Renomear pasta
            </DialogTitle>
            <DialogDescription>
              {folderToRename && (
                <>
                  Alterar &quot;{folderToRename}&quot;. Documentos nesta pasta
                  serão movidos.
                </>
              )}
            </DialogDescription>
          </DialogHeader>
          <Input
            placeholder="Novo nome da pasta"
            value={renameFolderNewPath}
            onChange={(e) => setRenameFolderNewPath(e.target.value)}
            onKeyDown={(e) =>
              e.key === 'Enter' && (e.preventDefault(), handleRenameFolder())
            }
            className="rounded-lg"
          />
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsRenameFolderOpen(false)}
              className="rounded-lg"
              disabled={isLabelActionLoading}
            >
              Cancelar
            </Button>
            <Button
              onClick={() => void handleRenameFolder()}
              className="rounded-lg"
              disabled={!renameFolderNewPath.trim() || isLabelActionLoading}
            >
              Renomear
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal de confirmação Ocultar / Mostrar no feed */}
      <Dialog
        open={!!ocultarNoFeedModal}
        onOpenChange={(open) => {
          if (!open) setOcultarNoFeedModal(null);
        }}
      >
        <DialogContent className="rounded-lg">
          <DialogHeader>
            <DialogTitle>
              {ocultarNoFeedModal?.ocultarNoFeed ? 'Ocultar no feed' : 'Mostrar no feed'}
            </DialogTitle>
            <DialogDescription>
              {ocultarNoFeedModal && (
                <>
                  {ocultarNoFeedModal.ocultarNoFeed ? (
                    <>
                      O documento &quot;{ocultarNoFeedModal.doc.name}&quot; deixará de ser exibido no feed.
                      Ele continuará visível na lista de documentos desta pasta. Deseja continuar?
                    </>
                  ) : (
                    <>
                      O documento &quot;{ocultarNoFeedModal.doc.name}&quot; voltará a ser exibido no feed.
                      Deseja continuar?
                    </>
                  )}
                </>
              )}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setOcultarNoFeedModal(null)}
              className="rounded-lg"
              disabled={ocultarNoFeedLoading}
            >
              Cancelar
            </Button>
            <Button
              onClick={() => void handleConfirmOcultarNoFeed()}
              className="rounded-lg"
              disabled={ocultarNoFeedLoading}
            >
              {ocultarNoFeedLoading ? 'Salvando...' : 'Confirmar'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal de Detalhes do Documento */}
      {selectedDocument && (
        <RequiredItemDetailModal
          open={isDetailModalOpen}
          onOpenChange={setIsDetailModalOpen}
          item={{ ...selectedDocument, itemType: 'post' as const }}
          isAlreadyAcknowledged={
            selectedDocument.acknowledgedAt != null && selectedDocument.acknowledgedAt !== ''
          }
          hideVerAceites={!verAceitesVisivelNoPreview}
        />
      )}
    </div>
  );
}
