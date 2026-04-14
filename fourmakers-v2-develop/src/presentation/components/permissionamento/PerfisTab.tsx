import { useState } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Search, Plus } from '@/components/ui/system-icons';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Edit, Eye, CheckCircle, XCircle } from '@/components/ui/system-icons';
import { usePermissionamento } from '@presentation/hooks/usePermissionamento';
import { CriarPerfilModal } from './CriarPerfilModal';

export function PerfisTab() {
  const {
    perfis,
    todosPerfis,
    search,
    setSearch,
    clients,
    clientSearch,
    setClientSearch,
    clientsLoading,
    setShouldLoadClients,
    getEscopoText,
    getClientesSelecionadosText,
    criarPerfil,
    atualizarPerfil,
    toggleStatusPerfil,
  } = usePermissionamento();

  const [modalOpen, setModalOpen] = useState(false);
  const [perfilEditando, setPerfilEditando] = useState<string | null>(null);

  const handleCriarPerfil = () => {
    setPerfilEditando(null);
    setModalOpen(true);
  };

  const handleEditarPerfil = (id: string) => {
    setPerfilEditando(id);
    setModalOpen(true);
  };

  const handleSave = (perfilData: Parameters<typeof criarPerfil>[0]) => {
    if (perfilEditando) {
      atualizarPerfil(perfilEditando, perfilData);
    } else {
      criarPerfil(perfilData);
    }
    setModalOpen(false);
    setPerfilEditando(null);
  };

  const perfilParaEditar = perfilEditando
    ? todosPerfis.find((p) => p.id === perfilEditando)
    : null;

  return (
    <div className="space-y-4">
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div className="flex flex-col md:flex-row gap-4 items-start md:items-center justify-between">
              <div>
                <h2 className="text-lg font-semibold">Perfis de Permissionamento</h2>
                <p className="text-sm text-muted-foreground mt-1">
                  Defina o que cada perfil pode acessar
                </p>
              </div>
              <div className="flex gap-2 w-full md:w-auto">
                <div className="relative flex-1 md:flex-initial md:w-[300px]">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    placeholder="Buscar perfis..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <Button onClick={handleCriarPerfil}>
                  <Plus className="h-4 w-4 mr-2" />
                  Criar Perfil
                </Button>
              </div>
            </div>

            <div className="border border-borderDefault rounded-lg overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Nome do Perfil</TableHead>
                    <TableHead>Escopo</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead className="text-right">Ações</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {perfis.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={4} className="text-center text-muted-foreground py-8">
                        {search ? 'Nenhum perfil encontrado' : 'Nenhum perfil cadastrado'}
                      </TableCell>
                    </TableRow>
                  ) : (
                    perfis.map((perfil) => {
                      const escopoText = getEscopoText(perfil);
                      const clientesText = getClientesSelecionadosText(perfil);
                      const temClientesEspecificos = perfil.escopo.includes('CLIENTES_ESPECIFICOS') && clientesText;
                      
                      return (
                        <TableRow key={perfil.id}>
                          <TableCell className="font-medium">{perfil.nome}</TableCell>
                          <TableCell>
                            {temClientesEspecificos ? (
                              <TooltipProvider>
                                <Tooltip>
                                  <TooltipTrigger asChild>
                                    <span className="cursor-help underline decoration-dotted">
                                      {escopoText}
                                    </span>
                                  </TooltipTrigger>
                                  <TooltipContent className="max-w-md">
                                    <p className="font-medium mb-1">Clientes selecionados:</p>
                                    <p className="text-sm">{clientesText}</p>
                                  </TooltipContent>
                                </Tooltip>
                              </TooltipProvider>
                            ) : (
                              escopoText
                            )}
                          </TableCell>
                          <TableCell>
                          <Badge
                            variant={perfil.status === 'Ativo' ? 'default' : 'secondary'}
                          >
                            {perfil.status}
                          </Badge>
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex items-center justify-end gap-2">
                            <Button
                              variant="ghost"
                              size="icon"
                              title="Editar"
                              onClick={() => handleEditarPerfil(perfil.id)}
                            >
                              <Edit className="h-4 w-4" />
                            </Button>
                            <Button
                              variant="ghost"
                              size="icon"
                              title={perfil.status === 'Ativo' ? 'Inativar' : 'Ativar'}
                              onClick={() => toggleStatusPerfil(perfil.id)}
                            >
                              {perfil.status === 'Ativo' ? (
                                <XCircle className="h-4 w-4" />
                              ) : (
                                <CheckCircle className="h-4 w-4" />
                              )}
                            </Button>
                            <Button variant="ghost" size="icon" title="Ver impacto">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    );
                    })
                  )}
                </TableBody>
              </Table>
            </div>
          </div>
        </CardContent>
      </Card>

      <CriarPerfilModal
        open={modalOpen}
        onOpenChange={setModalOpen}
        perfil={perfilParaEditar}
        clients={clients}
        clientSearch={clientSearch}
        setClientSearch={setClientSearch}
        clientsLoading={clientsLoading}
        setShouldLoadClients={setShouldLoadClients}
        onSave={handleSave}
      />
    </div>
  );
}
