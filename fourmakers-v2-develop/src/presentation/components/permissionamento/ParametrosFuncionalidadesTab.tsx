import { useState } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Search, Plus, Edit, Trash2, ChevronDown, ChevronRight } from '@/components/ui/system-icons';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { useParametrosFuncionalidades } from '@presentation/hooks/useParametrosFuncionalidades';
import { CriarModuloModal } from './CriarModuloModal';
import { CriarFuncionalidadeModal } from './CriarFuncionalidadeModal';
import { CriarParametroModal } from './CriarParametroModal';
import type { Modulo, Funcionalidade, Parametro } from '@presentation/hooks/useParametrosFuncionalidades';

const tipoLabels: Record<Funcionalidade['tipo'], string> = {
  visualizacao: 'Visualização',
  edicao: 'Edição',
  criacao: 'Criação',
  exclusao: 'Exclusão',
  relatorio: 'Relatório',
  configuracao: 'Configuração',
};

const tipoColors: Record<Funcionalidade['tipo'], 'default' | 'secondary' | 'outline'> = {
  visualizacao: 'default',
  edicao: 'secondary',
  criacao: 'default',
  exclusao: 'outline',
  relatorio: 'secondary',
  configuracao: 'outline',
};

const tipoParametroLabels: Record<'boolean' | 'string' | 'number' | 'json', string> = {
  boolean: 'Booleano',
  string: 'Texto',
  number: 'Número',
  json: 'JSON',
};

export function ParametrosFuncionalidadesTab() {
  const {
    modulos,
    todosModulos,
    funcionalidades,
    parametros,
    search,
    setSearch,
    getFuncionalidadesByModulo,
    getParametrosByModulo,
    criarModulo,
    atualizarModulo,
    removerModulo,
    criarFuncionalidade,
    atualizarFuncionalidade,
    removerFuncionalidade,
    criarParametro,
    atualizarParametro,
    removerParametro,
  } = useParametrosFuncionalidades();

  const [moduloModalOpen, setModuloModalOpen] = useState(false);
  const [funcionalidadeModalOpen, setFuncionalidadeModalOpen] = useState(false);
  const [parametroModalOpen, setParametroModalOpen] = useState(false);
  const [moduloEditando, setModuloEditando] = useState<string | null>(null);
  const [funcionalidadeEditando, setFuncionalidadeEditando] = useState<string | null>(null);
  const [parametroEditando, setParametroEditando] = useState<string | null>(null);
  const [moduloParaFuncionalidade, setModuloParaFuncionalidade] = useState<string | null>(null);
  const [contextoParaParametro, setContextoParaParametro] = useState<{ tipo: 'modulo' | 'funcionalidade'; id: string } | null>(null);
  const [modulosExpandidos, setModulosExpandidos] = useState<Set<string>>(new Set());

  const handleCriarModulo = () => {
    setModuloEditando(null);
    setModuloModalOpen(true);
  };

  const handleEditarModulo = (id: string) => {
    setModuloEditando(id);
    setModuloModalOpen(true);
  };

  const handleSaveModulo = (moduloData: Omit<Modulo, 'id' | 'createdAt' | 'updatedAt'>) => {
    if (moduloEditando) {
      atualizarModulo(moduloEditando, moduloData);
    } else {
      criarModulo(moduloData);
    }
    setModuloModalOpen(false);
    setModuloEditando(null);
  };

  const handleCriarFuncionalidade = (moduloId: string) => {
    setModuloParaFuncionalidade(moduloId);
    setFuncionalidadeEditando(null);
    setFuncionalidadeModalOpen(true);
  };

  const handleEditarFuncionalidade = (id: string) => {
    setFuncionalidadeEditando(id);
    setFuncionalidadeModalOpen(true);
  };

  const handleSaveFuncionalidade = (funcionalidadeData: Omit<Funcionalidade, 'id' | 'createdAt' | 'updatedAt'>) => {
    if (funcionalidadeEditando) {
      atualizarFuncionalidade(funcionalidadeEditando, funcionalidadeData);
    } else if (moduloParaFuncionalidade) {
      criarFuncionalidade({ ...funcionalidadeData, moduloId: moduloParaFuncionalidade });
    }
    setFuncionalidadeModalOpen(false);
    setFuncionalidadeEditando(null);
    setModuloParaFuncionalidade(null);
  };

  const handleCriarParametro = (tipo: 'modulo' | 'funcionalidade', id: string) => {
    setContextoParaParametro({ tipo, id });
    setParametroEditando(null);
    setParametroModalOpen(true);
  };

  const handleEditarParametro = (id: string) => {
    setParametroEditando(id);
    setParametroModalOpen(true);
  };

  const handleSaveParametro = (parametroData: Omit<Parametro, 'id' | 'createdAt' | 'updatedAt'>) => {
    if (parametroEditando) {
      atualizarParametro(parametroEditando, parametroData);
    } else if (contextoParaParametro) {
      criarParametro({
        ...parametroData,
        moduloId: contextoParaParametro.tipo === 'modulo' 
          ? contextoParaParametro.id 
          : funcionalidades.find(f => f.id === contextoParaParametro.id)?.moduloId || '',
        funcionalidadeId: contextoParaParametro.tipo === 'funcionalidade' ? contextoParaParametro.id : undefined,
      });
    }
    setParametroModalOpen(false);
    setParametroEditando(null);
    setContextoParaParametro(null);
  };

  const toggleExpandirModulo = (id: string) => {
    setModulosExpandidos((prev) => {
      const novo = new Set(prev);
      if (novo.has(id)) {
        novo.delete(id);
      } else {
        novo.add(id);
      }
      return novo;
    });
  };


  const moduloParaEditar = moduloEditando
    ? todosModulos.find((m) => m.id === moduloEditando)
    : null;

  const funcionalidadeParaEditar = funcionalidadeEditando
    ? funcionalidades.find((f) => f.id === funcionalidadeEditando)
    : null;

  const parametroParaEditar = parametroEditando
    ? parametros.find((p) => p.id === parametroEditando)
    : null;

  return (
    <div className="space-y-4">
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div className="flex flex-col md:flex-row gap-4 items-start md:items-center justify-between">
              <div>
                <h2 className="text-lg font-semibold">Parâmetros e Funcionalidades</h2>
                <p className="text-sm text-muted-foreground mt-1">
                  Organize módulos, funcionalidades e parâmetros do sistema
                </p>
              </div>
              <div className="flex gap-2 w-full md:w-auto">
                <div className="relative flex-1 md:flex-initial md:w-[300px]">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    placeholder="Buscar módulos..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <Button onClick={handleCriarModulo}>
                  <Plus className="h-4 w-4 mr-2" />
                  Novo Módulo
                </Button>
              </div>
            </div>

            <div className="border border-borderDefault rounded-lg overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-12"></TableHead>
                    <TableHead>Módulo</TableHead>
                    <TableHead>Descrição</TableHead>
                    <TableHead>Funcionalidades</TableHead>
                    <TableHead>Parâmetros</TableHead>
                    <TableHead className="text-right">Ações</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {modulos.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={6} className="text-center text-muted-foreground py-8">
                        {search ? 'Nenhum módulo encontrado' : 'Nenhum módulo cadastrado'}
                      </TableCell>
                    </TableRow>
                  ) : (
                    modulos.map((modulo) => {
                      const isModuloExpanded = modulosExpandidos.has(modulo.id);
                      const funcionalidadesDoModulo = getFuncionalidadesByModulo(modulo.id);
                      const parametrosDoModulo = getParametrosByModulo(modulo.id);
                      
                      return (
                        <>
                          <TableRow key={modulo.id}>
                            <TableCell>
                              {(funcionalidadesDoModulo.length > 0 || parametrosDoModulo.length > 0) && (
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="h-6 w-6"
                                  onClick={() => toggleExpandirModulo(modulo.id)}
                                >
                                  {isModuloExpanded ? (
                                    <ChevronDown className="h-4 w-4" />
                                  ) : (
                                    <ChevronRight className="h-4 w-4" />
                                  )}
                                </Button>
                              )}
                            </TableCell>
                            <TableCell className="font-medium">{modulo.nome}</TableCell>
                            <TableCell>
                              <p className="text-sm text-muted-foreground whitespace-normal">
                                {modulo.descricao}
                              </p>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <Badge variant="secondary" className="whitespace-nowrap">
                                  {funcionalidadesDoModulo.length} funcionalidade{funcionalidadesDoModulo.length !== 1 ? 's' : ''}
                                </Badge>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="h-6 w-6"
                                  title="Adicionar Funcionalidade"
                                  onClick={() => handleCriarFuncionalidade(modulo.id)}
                                >
                                  <Plus className="h-3 w-3" />
                                </Button>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="flex items-center gap-2">
                                <Badge variant="outline" className="whitespace-nowrap">
                                  {parametrosDoModulo.length} parâmetro{parametrosDoModulo.length !== 1 ? 's' : ''}
                                </Badge>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="h-6 w-6"
                                  title="Adicionar Parâmetro"
                                  onClick={() => handleCriarParametro('modulo', modulo.id)}
                                >
                                  <Plus className="h-3 w-3" />
                                </Button>
                              </div>
                            </TableCell>
                            <TableCell className="text-right">
                              <div className="flex items-center justify-end gap-2">
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  title="Editar"
                                  onClick={() => handleEditarModulo(modulo.id)}
                                >
                                  <Edit className="h-4 w-4" />
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  title="Remover"
                                  onClick={() => removerModulo(modulo.id)}
                                >
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
                            </TableCell>
                          </TableRow>
                          
                          {isModuloExpanded && (
                            <>
                              {/* Funcionalidades do Módulo */}
                              {funcionalidadesDoModulo.length > 0 && (
                                <TableRow key={`${modulo.id}-funcionalidades`}>
                                  <TableCell colSpan={6} className="bg-muted/10 p-0">
                                    <div className="p-4 pl-8">
                                      <div className="space-y-2">
                                        <h4 className="text-sm font-semibold mb-3">Funcionalidades do Módulo</h4>
                                        <div className="border border-borderDefault rounded-lg overflow-hidden">
                                          <Table>
                                            <TableHeader>
                                              <TableRow>
                                                <TableHead>Código</TableHead>
                                                <TableHead>Nome</TableHead>
                                                <TableHead>Descrição</TableHead>
                                                <TableHead>Tipo</TableHead>
                                                <TableHead className="text-right">Ações</TableHead>
                                              </TableRow>
                                            </TableHeader>
                                            <TableBody>
                                              {funcionalidadesDoModulo.map((funcionalidade) => (
                                                <TableRow key={funcionalidade.id}>
                                                  <TableCell className="font-medium">{funcionalidade.codigo}</TableCell>
                                                  <TableCell>
                                                    <div className="flex items-center gap-2">
                                                      <span className="font-medium">{funcionalidade.nome}</span>
                                                      {funcionalidade.idNumerico && (
                                                        <Badge variant="secondary" className="text-xs">
                                                          ID: {funcionalidade.idNumerico}
                                                        </Badge>
                                                      )}
                                                    </div>
                                                  </TableCell>
                                                  <TableCell className="max-w-md">
                                                    <p className="text-sm text-muted-foreground whitespace-normal" title={funcionalidade.descricao}>
                                                      {funcionalidade.descricao}
                                                    </p>
                                                  </TableCell>
                                                  <TableCell>
                                                    <Badge variant={tipoColors[funcionalidade.tipo]}>
                                                      {tipoLabels[funcionalidade.tipo]}
                                                    </Badge>
                                                  </TableCell>
                                                  <TableCell className="text-right">
                                                    <div className="flex items-center justify-end gap-2">
                                                      <Button
                                                        variant="ghost"
                                                        size="icon"
                                                        title="Editar"
                                                        onClick={() => handleEditarFuncionalidade(funcionalidade.id)}
                                                      >
                                                        <Edit className="h-4 w-4" />
                                                      </Button>
                                                      <Button
                                                        variant="ghost"
                                                        size="icon"
                                                        title="Remover"
                                                        onClick={() => removerFuncionalidade(funcionalidade.id)}
                                                      >
                                                        <Trash2 className="h-4 w-4" />
                                                      </Button>
                                                    </div>
                                                  </TableCell>
                                                </TableRow>
                                              ))}
                                            </TableBody>
                                          </Table>
                                        </div>
                                      </div>
                                    </div>
                                  </TableCell>
                                </TableRow>
                              )}
                              
                              {/* Parâmetros do Módulo */}
                              {parametrosDoModulo.length > 0 && (
                                <TableRow key={`${modulo.id}-params`}>
                                  <TableCell colSpan={6} className="bg-muted/10 p-0">
                                    <div className="p-4 pl-8">
                                      <div className="space-y-2">
                                        <h4 className="text-sm font-semibold mb-3">Parâmetros do Módulo</h4>
                                        <div className="border border-borderDefault rounded-lg overflow-hidden">
                                          <Table>
                                            <TableHeader>
                                              <TableRow>
                                                <TableHead>Código</TableHead>
                                                <TableHead>Descrição</TableHead>
                                                <TableHead>Funcionalidade</TableHead>
                                                <TableHead>Tipo</TableHead>
                                                <TableHead>Valor Padrão</TableHead>
                                                <TableHead className="text-right">Ações</TableHead>
                                              </TableRow>
                                            </TableHeader>
                                            <TableBody>
                                              {parametrosDoModulo.map((parametro) => {
                                                const funcionalidadeAssociada = parametro.funcionalidadeId
                                                  ? funcionalidades.find((f) => f.id === parametro.funcionalidadeId)
                                                  : null;
                                                return (
                                                  <TableRow key={parametro.id}>
                                                    <TableCell className="font-medium">{parametro.codigo}</TableCell>
                                                    <TableCell className="max-w-md">
                                                      <p className="text-sm text-muted-foreground truncate" title={parametro.descricao}>
                                                        {parametro.descricao}
                                                      </p>
                                                    </TableCell>
                                                    <TableCell>
                                                      {funcionalidadeAssociada ? (
                                                        <Badge variant="secondary" className="text-xs">
                                                          {funcionalidadeAssociada.nome}
                                                        </Badge>
                                                      ) : (
                                                        <span className="text-xs text-muted-foreground">Parâmetro do módulo</span>
                                                      )}
                                                    </TableCell>
                                                    <TableCell>
                                                      <Badge variant="outline">
                                                        {parametro.tipo ? tipoParametroLabels[parametro.tipo] || parametro.tipo : 'Não definido'}
                                                      </Badge>
                                                    </TableCell>
                                                    <TableCell>
                                                      <code className="text-xs bg-muted px-2 py-1 rounded">
                                                        {parametro.valorPadrao}
                                                      </code>
                                                    </TableCell>
                                                    <TableCell className="text-right">
                                                      <div className="flex items-center justify-end gap-2">
                                                        <Button
                                                          variant="ghost"
                                                          size="icon"
                                                          title="Editar"
                                                          onClick={() => handleEditarParametro(parametro.id)}
                                                        >
                                                          <Edit className="h-4 w-4" />
                                                        </Button>
                                                        <Button
                                                          variant="ghost"
                                                          size="icon"
                                                          title="Remover"
                                                          onClick={() => removerParametro(parametro.id)}
                                                        >
                                                          <Trash2 className="h-4 w-4" />
                                                        </Button>
                                                      </div>
                                                    </TableCell>
                                                  </TableRow>
                                                );
                                              })}
                                            </TableBody>
                                          </Table>
                                        </div>
                                      </div>
                                    </div>
                                  </TableCell>
                                </TableRow>
                              )}
                            </>
                          )}
                        </>
                      );
                    })
                  )}
                </TableBody>
              </Table>
            </div>
          </div>
        </CardContent>
      </Card>

      <CriarModuloModal
        open={moduloModalOpen}
        onOpenChange={setModuloModalOpen}
        modulo={moduloParaEditar}
        onSave={handleSaveModulo}
      />

      <CriarFuncionalidadeModal
        open={funcionalidadeModalOpen}
        onOpenChange={setFuncionalidadeModalOpen}
        funcionalidade={funcionalidadeParaEditar}
        moduloId={moduloParaFuncionalidade || funcionalidadeParaEditar?.moduloId}
        modulos={todosModulos}
        onSave={handleSaveFuncionalidade}
      />

      <CriarParametroModal
        open={parametroModalOpen}
        onOpenChange={setParametroModalOpen}
        parametro={parametroParaEditar}
        contexto={contextoParaParametro}
        modulos={todosModulos}
        funcionalidades={funcionalidades}
        onSave={handleSaveParametro}
      />
    </div>
  );
}
