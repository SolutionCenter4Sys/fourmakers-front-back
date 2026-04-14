import { useState, useEffect, Fragment, useCallback, useRef } from 'react';
import { container } from 'tsyringe';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { ChevronDown, ChevronRight, ContextualTokenAdd, Edit, PersonAdd, PersonRemove, Plus, Trash2 } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import { useAppSelector } from '@app/store/hooks';
import type {
  GrupoAcessoComFuncionalidadesItem,
  PessoaComGruposAcessoItem,
} from '@domain/entities/GrupoAcesso';
import type { FuncionalidadeSistemaItem } from '@domain/entities/FuncionalidadeSistema';
import { ListarGruposAcessoUseCase } from '@domain/usecases/ListarGruposAcessoUseCase';
import { ListarPessoasPorGrupoAcessoUseCase } from '@domain/usecases/ListarPessoasPorGrupoAcessoUseCase';
import { RemoverFuncionalidadeGrupoUseCase } from '@domain/usecases/RemoverFuncionalidadeGrupoUseCase';
import { RemoverUsuarioGrupoUseCase } from '@domain/usecases/RemoverUsuarioGrupoUseCase';
import { formatDatePtBr } from '@presentation/components/permissionamento/permissionamentoUtils';
import { CountBadge } from '@presentation/components/common';
import { CriarGrupoAcessoModal } from './CriarGrupoAcessoModal';
import { AtribuirFuncionalidadeGrupoModal } from './AtribuirFuncionalidadeGrupoModal';
import { AdicionarUsuarioGrupoModal } from './AdicionarUsuarioGrupoModal';
import { RemoverUsuarioGrupoModal } from './RemoverUsuarioGrupoModal';
import { toast } from 'sonner';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';

const NAO_INFORMADO = 'Não informado';

function formatDisplay(value: string | number | null | undefined): string {
  if (value === null || value === undefined || String(value).trim() === '') return NAO_INFORMADO;
  return String(value);
}

interface PessoasNoGrupoSubTableProps {
  grupoAcessoId: number;
  pessoas: PessoaComGruposAcessoItem[];
  loading?: boolean;
  onRemoverPessoa: (grupoAcessoId: number, pessoa: PessoaComGruposAcessoItem) => void;
}

function PessoasNoGrupoSubTable({
  grupoAcessoId,
  pessoas,
  loading,
  onRemoverPessoa,
}: PessoasNoGrupoSubTableProps) {
  if (loading) {
    return (
      <div className="flex items-center gap-2 py-4 text-sm text-muted-foreground">
        <Spinner size={20} />
        <span>Carregando pessoas...</span>
      </div>
    );
  }

  if (pessoas.length === 0) {
    return (
      <p className="text-sm text-muted-foreground py-2">Nenhuma pessoa vinculada ao grupo</p>
    );
  }

  return (
    <div className="border border-borderDefault rounded-lg overflow-hidden mt-2">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="w-[120px]">ID</TableHead>
            <TableHead>Nome</TableHead>
            <TableHead className="w-[200px]">E-mail</TableHead>
            <TableHead className="w-[120px]">Data de inserção</TableHead>
            <TableHead className="w-[100px] text-right">Ações</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {pessoas.map((pessoa) => {
            const temUsuarioId = pessoa.usuarioId != null;
            const idDisplay = temUsuarioId ? String(pessoa.usuarioId) : 'N/A';
            return (
              <TableRow key={pessoa.codigoInternoColaborador}>
                <TableCell className="font-medium text-sm">{idDisplay}</TableCell>
                <TableCell className="font-medium">{formatDisplay(pessoa.nome)}</TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {formatDisplay(pessoa.email)}
                </TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {pessoa.dataInsercao
                    ? formatDatePtBr(pessoa.dataInsercao, NAO_INFORMADO)
                    : NAO_INFORMADO}
                </TableCell>
                <TableCell className="text-right">
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <span>
                        <Button
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 text-muted-foreground hover:text-destructive"
                          aria-label="Remover pessoa do grupo"
                          disabled={!temUsuarioId}
                          onClick={() => temUsuarioId && onRemoverPessoa(grupoAcessoId, pessoa)}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </span>
                    </TooltipTrigger>
                    <TooltipContent side="left">
                      {temUsuarioId
                        ? 'Remover pessoa do grupo'
                        : 'Remoção disponível quando o ID de usuário estiver disponível'}
                    </TooltipContent>
                  </Tooltip>
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </div>
  );
}

interface FuncionalidadesSubTableProps {
  items: FuncionalidadeSistemaItem[];
  grupoAcessoId: number;
  onRemover: (grupoAcessoId: number, funcionalidadeId: number) => Promise<{ sucesso?: boolean; mensagem?: string | null }>;
  onRemoved: () => void;
}

function FuncionalidadesSubTable({ items, grupoAcessoId, onRemover, onRemoved }: FuncionalidadesSubTableProps) {
  const [removendoId, setRemovendoId] = useState<number | null>(null);
  const [confirmarRemover, setConfirmarRemover] = useState<{ funcId: number; descricao: string } | null>(null);

  const handleRemover = (item: FuncionalidadeSistemaItem) => {
    setConfirmarRemover({ funcId: item.id, descricao: item.descricao });
  };

  const confirmarRemocao = async () => {
    if (!confirmarRemover) return;
    const { funcId } = confirmarRemover;
    setRemovendoId(funcId);
    setConfirmarRemover(null);
    try {
      const res = await onRemover(grupoAcessoId, funcId);
      if (res.sucesso) {
        toast.success(res.mensagem ?? 'Funcionalidade removida do grupo.');
        onRemoved();
      } else {
        toast.error(res.mensagem ?? 'Erro ao remover funcionalidade.');
      }
    } catch (err: unknown) {
      const msg =
        err && typeof err === 'object' && err !== null && 'mensagem' in err
          ? String((err as { mensagem?: string }).mensagem)
          : err instanceof Error
            ? err.message
            : 'Erro ao remover funcionalidade.';
      toast.error(msg);
    } finally {
      setRemovendoId(null);
    }
  };

  if (!items.length) {
    return (
      <p className="text-sm text-muted-foreground py-2">Nenhuma funcionalidade vinculada</p>
    );
  }
  return (
    <>
      <div className="border border-borderDefault rounded-lg overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[80px]">ID</TableHead>
              <TableHead>Funcionalidade</TableHead>
              <TableHead className="w-[120px]">Status</TableHead>
              <TableHead className="w-[140px]">Data de Criação</TableHead>
              <TableHead className="w-[140px]">Última Alteração</TableHead>
              <TableHead className="w-[100px] text-right">Ações</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {items.map((item) => (
              <TableRow key={item.id}>
                <TableCell className="font-medium">{item.id}</TableCell>
                <TableCell>{item.descricao}</TableCell>
                <TableCell>
                  <Badge
                    variant={item.ativo ? 'default' : 'secondary'}
                    className={
                      item.ativo
                        ? 'bg-success/10 text-success hover:bg-success/20'
                        : 'bg-muted text-muted-foreground'
                    }
                  >
                    {item.ativo ? 'Ativo' : 'Inativo'}
                  </Badge>
                </TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {formatDatePtBr(item.dataCriacao, '—')}
                </TableCell>
                <TableCell className="text-muted-foreground text-sm">
                  {formatDatePtBr(item.dataAlteracao, '—')}
                </TableCell>
                <TableCell className="text-right">
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-8 w-8 text-muted-foreground hover:text-destructive"
                        aria-label="Excluir funcionalidade do grupo"
                        disabled={removendoId === item.id}
                        onClick={() => handleRemover(item)}
                      >
                        {removendoId === item.id ? (
                          <Spinner size={16} className="text-current" />
                        ) : (
                          <Trash2 className="h-4 w-4" />
                        )}
                      </Button>
                    </TooltipTrigger>
                    <TooltipContent side="left">Excluir funcionalidade do grupo</TooltipContent>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
      <AlertDialog open={!!confirmarRemover} onOpenChange={(open) => !open && setConfirmarRemover(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir funcionalidade do grupo?</AlertDialogTitle>
            <AlertDialogDescription>
              {confirmarRemover
                ? `A funcionalidade "${confirmarRemover.descricao}" será removida do grupo. Esta ação pode ser desfeita posteriormente.`
                : ''}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmarRemocao} className="bg-destructive text-destructive-foreground hover:bg-destructive/90">
              Excluir
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}

export function GruposTab() {
  const token = useAppSelector((state) => state.auth.token);
  const user = useAppSelector((state) => state.auth.user);
  const orgId = user?.colaboradorOrg?.orgId ?? 0;

  const listarGruposAcessoUseCaseRef = useRef(container.resolve(ListarGruposAcessoUseCase));
  const listarPessoasPorGrupoAcessoUseCaseRef = useRef(container.resolve(ListarPessoasPorGrupoAcessoUseCase));
  const removerFuncionalidadeGrupoUseCaseRef = useRef(container.resolve(RemoverFuncionalidadeGrupoUseCase));
  const removerUsuarioGrupoUseCaseRef = useRef(container.resolve(RemoverUsuarioGrupoUseCase));

  const [lista, setLista] = useState<GrupoAcessoComFuncionalidadesItem[]>([]);
  const [pessoasPorGrupoList, setPessoasPorGrupoList] = useState<PessoaComGruposAcessoItem[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadingPessoas, setLoadingPessoas] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [gruposExpandidos, setGruposExpandidos] = useState<Set<number>>(new Set());
  const [criarModalOpen, setCriarModalOpen] = useState(false);
  const [atribuirModalOpen, setAtribuirModalOpen] = useState(false);
  const [atribuirGrupoId, setAtribuirGrupoId] = useState<number | undefined>(undefined);
  const [adicionarUsuarioModalOpen, setAdicionarUsuarioModalOpen] = useState(false);
  const [adicionarUsuarioGrupo, setAdicionarUsuarioGrupo] = useState<{ id: number; descricao: string } | undefined>(undefined);
  const [removerUsuarioModalOpen, setRemoverUsuarioModalOpen] = useState(false);
  const [removerUsuarioGrupo, setRemoverUsuarioGrupo] = useState<{ id: number; descricao: string } | undefined>(undefined);
  const [editarModalOpen, setEditarModalOpen] = useState(false);
  const [editarGrupo, setEditarGrupo] = useState<GrupoAcessoComFuncionalidadesItem | undefined>(undefined);

  const loadLista = useCallback(() => {
    if (!token) return;
    setLoading(true);
    setError(null);
    listarGruposAcessoUseCaseRef.current
      .execute(token)
      .then((res) => {
        setLista(res.retorno ?? []);
        toast.success('A lista de grupos foi atualizada.');
      })
      .catch((err) => {
        setLista([]);
        const msg =
          err && typeof err === 'object' && err !== null && 'mensagem' in err
            ? String((err as { mensagem?: string }).mensagem)
            : err instanceof Error
              ? err.message
              : 'Erro ao carregar grupos de acesso.';
        setError(msg);
        toast.error(msg);
      })
      .finally(() => setLoading(false));
  }, [token]);

  const loadPessoas = useCallback(() => {
    if (!token) return;
    setLoadingPessoas(true);
    listarPessoasPorGrupoAcessoUseCaseRef.current
      .execute(token)
      .then((res) => {
        setPessoasPorGrupoList(res.retorno ?? []);
      })
      .catch(() => setPessoasPorGrupoList([]))
      .finally(() => setLoadingPessoas(false));
  }, [token]);

  const handleRemoverPessoaDoGrupo = useCallback(
    async (grupoAcessoId: number, pessoa: PessoaComGruposAcessoItem) => {
      if (!token || pessoa.usuarioId == null) return;
      try {
        const res = await removerUsuarioGrupoUseCaseRef.current.execute(token, pessoa.usuarioId, grupoAcessoId);
        if (res.sucesso) {
          toast.success(res.mensagem ?? 'Pessoa removida do grupo.');
          loadLista();
          loadPessoas();
        } else {
          toast.error(res.mensagem ?? 'Erro ao remover pessoa do grupo.');
        }
      } catch (err: unknown) {
        const msg =
          err && typeof err === 'object' && err !== null && 'mensagem' in err
            ? String((err as { mensagem?: string }).mensagem)
            : err instanceof Error
              ? err.message
              : 'Erro ao remover pessoa do grupo.';
        toast.error(msg);
      }
    },
    [token, loadLista, loadPessoas]
  );

  const handleRemoverFuncionalidade = useCallback(
    async (grupoAcessoId: number, funcionalidadeId: number) => {
      if (!token) return { sucesso: false };
      return removerFuncionalidadeGrupoUseCaseRef.current.execute(token, grupoAcessoId, funcionalidadeId);
    },
    [token]
  );

  const refreshGruposEPessoas = useCallback(() => {
    loadLista();
    loadPessoas();
  }, [loadLista, loadPessoas]);

  useEffect(() => {
    if (!token) return;
    setLoading(true);
    setError(null);
    listarGruposAcessoUseCaseRef.current
      .execute(token)
      .then((res) => {
        setLista(res.retorno ?? []);
      })
      .catch((err) => {
        setLista([]);
        const msg =
          err && typeof err === 'object' && err !== null && 'mensagem' in err
            ? String((err as { mensagem?: string }).mensagem)
            : err instanceof Error
              ? err.message
              : 'Erro ao carregar grupos de acesso.';
        setError(msg);
        toast.error(msg);
      })
      .finally(() => setLoading(false));
  }, [token]);

  const toggleExpandirGrupo = useCallback((id: number) => {
    const estaExpandido = gruposExpandidos.has(id);
    setGruposExpandidos((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
    if (!estaExpandido && pessoasPorGrupoList === null) {
      loadPessoas();
    }
  }, [gruposExpandidos, pessoasPorGrupoList, loadPessoas]);

  return (
    <div className="space-y-4">
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4">
              <div>
                <h2 className="text-lg font-semibold">Grupos de Acesso</h2>
                <p className="text-sm text-muted-foreground mt-1">
                  Listagem de grupos com funcionalidades vinculadas. Alterações são realizadas no backend.
                </p>
              </div>
              <Button onClick={() => setCriarModalOpen(true)} className="shrink-0">
                <Plus className="h-4 w-4 mr-2" />
                Criar Grupo
              </Button>
            </div>

            {loading ? (
              <div className="flex items-center justify-center py-12 text-muted-foreground">
                <Spinner className="mr-2" size={32} />
                <span>Carregando grupos...</span>
              </div>
            ) : error ? (
              <div className="rounded-lg border border-destructive/50 bg-destructive/10 px-4 py-3 text-sm text-destructive">
                {error}
              </div>
            ) : (
              <div className="border border-borderDefault rounded-lg overflow-hidden">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-[5rem]" />
                      <TableHead className="w-[80px]">ID</TableHead>
                      <TableHead>Nome do Grupo</TableHead>
                      <TableHead className="w-[120px]">Status</TableHead>
                      <TableHead className="w-[140px]">Data de Criação</TableHead>
                      <TableHead className="w-[140px]">Última Alteração</TableHead>
                      <TableHead className="w-[180px] text-right">Ações</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {lista.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={7} className="text-center text-muted-foreground py-8">
                          Nenhum grupo encontrado
                        </TableCell>
                      </TableRow>
                    ) : (
                      lista.map((grupo) => {
                        const isExpanded = gruposExpandidos.has(grupo.id);
                        const funcionalidades = grupo.funcionalidadeSistema ?? [];

                        return (
                          <Fragment key={grupo.id}>
                            <TableRow>
                              <TableCell>
                                <div className="inline-flex items-center gap-1">
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="h-8 w-8 shrink-0"
                                    onClick={() => toggleExpandirGrupo(grupo.id)}
                                    aria-expanded={isExpanded}
                                  >
                                    {isExpanded ? (
                                      <ChevronDown className="h-5 w-5" />
                                    ) : (
                                      <ChevronRight className="h-5 w-5" />
                                    )}
                                  </Button>
                                  <CountBadge
                                    count={funcionalidades.length}
                                    className="!min-w-[1rem] !h-4 !p-0 !text-[10px]"
                                  />
                                </div>
                              </TableCell>
                              <TableCell className="font-medium">{grupo.id}</TableCell>
                              <TableCell>{grupo.descricao}</TableCell>
                              <TableCell>
                                <Badge
                                  variant={grupo.ativo ? 'default' : 'secondary'}
                                  className={
                                    grupo.ativo
                                      ? 'bg-success/10 text-success hover:bg-success/20'
                                      : 'bg-muted text-muted-foreground'
                                  }
                                >
                                  {grupo.ativo ? 'Ativo' : 'Inativo'}
                                </Badge>
                              </TableCell>
                              <TableCell className="text-muted-foreground text-sm">
                                {formatDatePtBr(grupo.dataCriacao, '—')}
                              </TableCell>
                              <TableCell className="text-muted-foreground text-sm">
                                {formatDatePtBr(grupo.dataAlteracao, '—')}
                              </TableCell>
                              <TableCell className="text-right">
                                <div className="flex items-center justify-end gap-1">
                                  <Tooltip>
                                    <TooltipTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="icon"
                                        className="h-8 w-8"
                                        aria-label="Editar grupo de acesso"
                                        onClick={() => {
                                          setEditarGrupo(grupo);
                                          setEditarModalOpen(true);
                                        }}
                                      >
                                        <Edit className="h-4 w-4" />
                                      </Button>
                                    </TooltipTrigger>
                                    <TooltipContent side="left">Editar grupo de acesso</TooltipContent>
                                  </Tooltip>
                                  <Tooltip>
                                    <TooltipTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="icon"
                                        className="h-8 w-8"
                                        aria-label="Incluir funcionalidade de sistema"
                                        onClick={() => {
                                          setAtribuirGrupoId(grupo.id);
                                          setAtribuirModalOpen(true);
                                        }}
                                      >
                                        <ContextualTokenAdd className="h-4 w-4" />
                                      </Button>
                                    </TooltipTrigger>
                                    <TooltipContent side="left">Incluir funcionalidade de sistema</TooltipContent>
                                  </Tooltip>
                                  <Tooltip>
                                    <TooltipTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="icon"
                                        className="h-8 w-8"
                                        aria-label="Adicionar pessoa ao grupo"
                                        onClick={() => {
                                          setAdicionarUsuarioGrupo({ id: grupo.id, descricao: grupo.descricao });
                                          setAdicionarUsuarioModalOpen(true);
                                        }}
                                      >
                                        <PersonAdd className="h-4 w-4" />
                                      </Button>
                                    </TooltipTrigger>
                                    <TooltipContent side="left">Adicionar pessoa ao grupo</TooltipContent>
                                  </Tooltip>
                                  <Tooltip>
                                    <TooltipTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="icon"
                                        className="h-8 w-8"
                                        aria-label="Remover usuário do grupo"
                                        onClick={() => {
                                          setRemoverUsuarioGrupo({ id: grupo.id, descricao: grupo.descricao });
                                          setRemoverUsuarioModalOpen(true);
                                        }}
                                      >
                                        <PersonRemove className="h-4 w-4 text-muted-foreground hover:text-destructive" />
                                      </Button>
                                    </TooltipTrigger>
                                    <TooltipContent side="left">Remover usuário do grupo</TooltipContent>
                                  </Tooltip>
                                </div>
                              </TableCell>
                            </TableRow>
                            {isExpanded && (
                              <TableRow>
                                <TableCell colSpan={7} className="bg-muted/10 p-0">
                                  <div className="p-4 pl-8">
                                    <h4 className="text-sm font-semibold mb-3">
                                      Funcionalidades do grupo
                                    </h4>
                                    <FuncionalidadesSubTable
                                      items={funcionalidades}
                                      grupoAcessoId={grupo.id}
                                      onRemover={handleRemoverFuncionalidade}
                                      onRemoved={loadLista}
                                    />
                                    <h4 className="text-sm font-semibold mb-3 mt-6">
                                      Lista de pessoas neste grupo
                                    </h4>
                                    <PessoasNoGrupoSubTable
                                      grupoAcessoId={grupo.id}
                                      pessoas={
                                        pessoasPorGrupoList?.filter((p) =>
                                          p.gruposAcesso.some((g) => g.id === grupo.id)
                                        ) ?? []
                                      }
                                      loading={loadingPessoas}
                                      onRemoverPessoa={handleRemoverPessoaDoGrupo}
                                    />
                                  </div>
                                </TableCell>
                              </TableRow>
                            )}
                          </Fragment>
                        );
                      })
                    )}
                  </TableBody>
                </Table>
              </div>
            )}
          </div>
        </CardContent>
      </Card>

      {token && (
        <>
          <CriarGrupoAcessoModal
            open={criarModalOpen}
            onOpenChange={setCriarModalOpen}
            token={token}
            onSuccess={loadLista}
          />
          <CriarGrupoAcessoModal
            open={editarModalOpen}
            onOpenChange={(open) => {
              setEditarModalOpen(open);
              if (!open) setEditarGrupo(undefined);
            }}
            token={token}
            onSuccess={loadLista}
            initialGrupo={
              editarGrupo
                ? {
                    id: editarGrupo.id,
                    descricao: editarGrupo.descricao,
                    clientes: editarGrupo.clientes ?? [],
                  }
                : undefined
            }
          />
          <AtribuirFuncionalidadeGrupoModal
            open={atribuirModalOpen}
            onOpenChange={(open) => {
              setAtribuirModalOpen(open);
              if (!open) setAtribuirGrupoId(undefined);
            }}
            token={token}
            initialGrupoId={atribuirGrupoId}
            onSuccess={loadLista}
          />
          {adicionarUsuarioGrupo && (
            <AdicionarUsuarioGrupoModal
              open={adicionarUsuarioModalOpen}
              onOpenChange={(open) => {
                setAdicionarUsuarioModalOpen(open);
                if (!open) setAdicionarUsuarioGrupo(undefined);
              }}
              token={token}
              grupoAcessoId={adicionarUsuarioGrupo.id}
              grupoDescricao={adicionarUsuarioGrupo.descricao}
              orgId={orgId}
              onSuccess={refreshGruposEPessoas}
            />
          )}
          {removerUsuarioGrupo && (
            <RemoverUsuarioGrupoModal
              open={removerUsuarioModalOpen}
              onOpenChange={(open) => {
                setRemoverUsuarioModalOpen(open);
                if (!open) setRemoverUsuarioGrupo(undefined);
              }}
              token={token}
              grupoAcessoId={removerUsuarioGrupo.id}
              grupoDescricao={removerUsuarioGrupo.descricao}
              orgId={orgId}
              onSuccess={refreshGruposEPessoas}
            />
          )}
        </>
      )}
    </div>
  );
}
