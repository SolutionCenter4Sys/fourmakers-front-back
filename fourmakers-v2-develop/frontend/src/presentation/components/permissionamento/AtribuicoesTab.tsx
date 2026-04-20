import { useState, useMemo } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Search, Plus, X } from '@/components/ui/system-icons';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { useGrupos } from '@presentation/hooks/useGrupos';
import { usePermissionamento } from '@presentation/hooks/usePermissionamento';
import { AtribuirPerfilModal } from './AtribuirPerfilModal';
import type { AtribuicaoPerfil } from '@presentation/hooks/useGrupos';

export function AtribuicoesTab() {
  const [search, setSearch] = useState('');
  const [modalOpen, setModalOpen] = useState(false);

  const {
    colaboradores,
    colaboradoresLoading,
    colaboradoresSearch,
    setColaboradoresSearch,
    loadColaboradores,
    atribuicoes,
    adicionarAtribuicao,
    removerAtribuicao,
  } = useGrupos();

  const { todosPerfis, getEscopoText } = usePermissionamento();

  // Filtrar atribuições pela busca
  const atribuicoesFiltradas = useMemo(() => {
    if (!search.trim()) return atribuicoes;
    
    const searchLower = search.toLowerCase();
    return atribuicoes.filter(
      (atrib) =>
        atrib.colaboradorNome.toLowerCase().includes(searchLower) ||
        atrib.colaboradorEmail.toLowerCase().includes(searchLower)
    );
  }, [atribuicoes, search]);

  const handleSave = (colaboradorCpf: string, perfilId: string) => {
    const colaborador = colaboradores.find((c) => c.cpf === colaboradorCpf);
    if (colaborador) {
      adicionarAtribuicao(perfilId, colaborador);
    }
  };

  const handleRemove = (atribuicao: AtribuicaoPerfil) => {
    removerAtribuicao(atribuicao.perfilId, atribuicao.colaboradorCpf);
  };

  return (
    <div className="space-y-4">
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div className="flex flex-col md:flex-row gap-4 items-start md:items-center justify-between">
              <div>
                <h2 className="text-lg font-semibold">Atribuições Diretas</h2>
                <p className="text-sm text-muted-foreground mt-1">
                  Perfis aplicados individualmente
                </p>
              </div>
              <div className="flex gap-2 w-full md:w-auto">
                <div className="relative flex-1 md:flex-initial md:w-[300px]">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    placeholder="Buscar por colaborador..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <Button onClick={() => setModalOpen(true)}>
                  <Plus className="h-4 w-4 mr-2" />
                  Atribuir Perfil
                </Button>
              </div>
            </div>

            <div className="border border-borderDefault rounded-lg overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Colaborador</TableHead>
                    <TableHead>Perfil</TableHead>
                    <TableHead>Escopo</TableHead>
                    <TableHead className="text-right">Ações</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {atribuicoesFiltradas.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={4} className="text-center text-muted-foreground py-8">
                        {search ? 'Nenhuma atribuição encontrada' : 'Nenhuma atribuição cadastrada'}
                      </TableCell>
                    </TableRow>
                  ) : (
                    atribuicoesFiltradas.map((atribuicao) => {
                      const perfil = todosPerfis.find((p) => p.id === atribuicao.perfilId);
                      return (
                        <TableRow key={atribuicao.id}>
                          <TableCell className="font-medium">
                            <div className="flex flex-col">
                              <span>{atribuicao.colaboradorNome}</span>
                              <span className="text-xs text-muted-foreground">
                                {atribuicao.colaboradorEmail}
                              </span>
                            </div>
                          </TableCell>
                          <TableCell>{perfil?.nome || 'Perfil não encontrado'}</TableCell>
                          <TableCell>
                            {perfil ? getEscopoText(perfil) : 'Não definido'}
                          </TableCell>
                          <TableCell className="text-right">
                            <Button 
                              variant="ghost" 
                              size="icon"
                              onClick={() => handleRemove(atribuicao)}
                              title="Remover atribuição"
                            >
                              <X className="h-4 w-4" />
                            </Button>
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

      <AtribuirPerfilModal
        open={modalOpen}
        onOpenChange={setModalOpen}
        colaboradores={colaboradores}
        colaboradoresLoading={colaboradoresLoading}
        colaboradoresSearch={colaboradoresSearch}
        setColaboradoresSearch={setColaboradoresSearch}
        loadColaboradores={loadColaboradores}
        perfis={todosPerfis}
        getEscopoText={getEscopoText}
        onSave={handleSave}
      />
    </div>
  );
}
