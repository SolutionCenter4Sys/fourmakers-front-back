import { useState, useEffect } from 'react';
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
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { useAppSelector } from '@app/store/hooks';
import type { FuncionalidadeSistemaItem } from '@domain/entities/FuncionalidadeSistema';
import { ListarFuncionalidadesSistemaUseCase } from '@domain/usecases/ListarFuncionalidadesSistemaUseCase';
import { formatDatePtBr } from '@presentation/components/permissionamento/permissionamentoUtils';
import { ContextualTokenAdd } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';
import { toast } from 'sonner';
import { AtribuirFuncionalidadeGrupoModal } from './AtribuirFuncionalidadeGrupoModal';

export function FuncionalidadesSistemaTab() {
  const token = useAppSelector((state) => state.auth.token);
  const listarFuncionalidadesSistemaUseCase = container.resolve(ListarFuncionalidadesSistemaUseCase);
  const [lista, setLista] = useState<FuncionalidadeSistemaItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [atribuirModalOpen, setAtribuirModalOpen] = useState(false);
  const [atribuirFuncionalidadeId, setAtribuirFuncionalidadeId] = useState<number | undefined>(undefined);

  useEffect(() => {
    if (!token) return;
    setLoading(true);
    setError(null);
    listarFuncionalidadesSistemaUseCase
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
              : 'Erro ao carregar funcionalidades.';
        setError(msg);
        toast.error(msg);
      })
      .finally(() => {
        setLoading(false);
      });
  }, [token, listarFuncionalidadesSistemaUseCase]);

  return (
    <div className="space-y-4">
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div>
              <h2 className="text-lg font-semibold">Funcionalidades do Sistema</h2>
              <p className="text-sm text-muted-foreground mt-1">
                Listagem de funcionalidades cadastradas no sistema. Alterações são realizadas no backend.
              </p>
            </div>

            {loading ? (
              <div className="flex items-center justify-center py-12 text-muted-foreground">
                <Spinner className="mr-2" size={32} />
                <span>Carregando funcionalidades...</span>
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
                      <TableHead className="w-[80px]">ID</TableHead>
                      <TableHead>Funcionalidade</TableHead>
                      <TableHead className="w-[120px]">Status</TableHead>
                      <TableHead className="w-[140px]">Data de Criação</TableHead>
                      <TableHead className="w-[140px]">Última Alteração</TableHead>
                      <TableHead className="w-[100px] text-right">Ações</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {lista.length === 0 ? (
                      <TableRow>
                        <TableCell colSpan={6} className="text-center text-muted-foreground py-8">
                          Nenhuma funcionalidade encontrada
                        </TableCell>
                      </TableRow>
                    ) : (
                      lista.map((item) => (
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
                                  className="h-8 w-8"
                                  aria-label="Atribuir a um grupo de acesso"
                                  onClick={() => {
                                    setAtribuirFuncionalidadeId(item.id);
                                    setAtribuirModalOpen(true);
                                  }}
                                >
                                  <ContextualTokenAdd className="h-4 w-4" />
                                </Button>
                              </TooltipTrigger>
                              <TooltipContent side="left">Atribuir a um grupo de acesso</TooltipContent>
                            </Tooltip>
                          </TableCell>
                        </TableRow>
                      ))
                    )}
                  </TableBody>
                </Table>
              </div>
            )}
          </div>
        </CardContent>
      </Card>

      {token && (
        <AtribuirFuncionalidadeGrupoModal
          open={atribuirModalOpen}
          onOpenChange={(open) => {
            setAtribuirModalOpen(open);
            if (!open) setAtribuirFuncionalidadeId(undefined);
          }}
          token={token}
          initialFuncionalidadeId={atribuirFuncionalidadeId}
          onSuccess={() => {
            listarFuncionalidadesSistemaUseCase.execute(token).then((res) => setLista(res.retorno ?? []));
          }}
        />
      )}
    </div>
  );
}
