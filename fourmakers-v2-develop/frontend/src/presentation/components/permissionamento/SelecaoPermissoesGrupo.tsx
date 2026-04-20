import { useState } from 'react';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { ChevronDown, ChevronRight } from '@/components/ui/system-icons';
import { Button } from '@/components/ui/button';
import type { Modulo, Funcionalidade, Parametro } from '@presentation/hooks/useParametrosFuncionalidades';
import type { FuncionalidadeSelecionada, ParametroSelecionado } from '@presentation/hooks/useGrupos';

interface SelecaoPermissoesGrupoProps {
  modulos: Modulo[];
  funcionalidadesSelecionadas: FuncionalidadeSelecionada[];
  parametrosSelecionados: ParametroSelecionado[];
  isAdmin?: boolean;
  onFuncionalidadesChange: (funcionalidades: FuncionalidadeSelecionada[]) => void;
  onParametrosChange: (parametros: ParametroSelecionado[]) => void;
  getFuncionalidadesByModulo: (moduloId: string) => Funcionalidade[];
  getParametrosByModulo: (moduloId: string) => Parametro[];
  getParametrosByFuncionalidade: (funcionalidadeId: string) => Parametro[];
}

export function SelecaoPermissoesGrupo({
  modulos,
  funcionalidadesSelecionadas,
  parametrosSelecionados,
  isAdmin = false,
  onFuncionalidadesChange,
  onParametrosChange,
  getFuncionalidadesByModulo,
  getParametrosByModulo,
  getParametrosByFuncionalidade,
}: SelecaoPermissoesGrupoProps) {
  const [modulosExpandidos, setModulosExpandidos] = useState<Set<string>>(new Set());
  const [funcionalidadesExpandidas, setFuncionalidadesExpandidas] = useState<Set<string>>(new Set());

  // Verificar se um módulo está totalmente selecionado
  const isModuloCompletamenteSelecionado = (moduloId: string): boolean => {
    if (isAdmin) return true; // Admin tem tudo selecionado
    const funcsDoModulo = getFuncionalidadesByModulo(moduloId);
    if (funcsDoModulo.length === 0) return false;
    return funcsDoModulo.every((func) =>
      funcionalidadesSelecionadas.some((f) => f.funcionalidadeId === func.id)
    );
  };


  // Verificar se uma funcionalidade está selecionada
  const isFuncionalidadeSelecionada = (funcionalidadeId: string): boolean => {
    if (isAdmin) return true; // Admin tem todas selecionadas
    return funcionalidadesSelecionadas.some((f) => f.funcionalidadeId === funcionalidadeId);
  };

  // Verificar se um parâmetro está selecionado
  const isParametroSelecionado = (parametroId: string): boolean => {
    if (isAdmin) return true; // Admin tem todos selecionados
    return parametrosSelecionados.some((p) => p.parametroId === parametroId);
  };

  // Toggle parâmetro
  const toggleParametro = (parametroId: string, moduloId: string, funcionalidadeId?: string) => {
    if (isAdmin) return; // Admin não pode alterar seleções
    if (isParametroSelecionado(parametroId)) {
      // Remover parâmetro
      onParametrosChange(
        parametrosSelecionados.filter((p) => p.parametroId !== parametroId)
      );
    } else {
      // Adicionar parâmetro
      onParametrosChange([
        ...parametrosSelecionados,
        { parametroId, moduloId, funcionalidadeId },
      ]);
    }
  };

  // Toggle módulo (seleciona/deseleciona todas as funcionalidades)
  const toggleModulo = (moduloId: string) => {
    if (isAdmin) return; // Admin não pode alterar seleções
    const funcsDoModulo = getFuncionalidadesByModulo(moduloId);
    const todasSelecionadas = isModuloCompletamenteSelecionado(moduloId);

    if (todasSelecionadas) {
      // Desmarcar todas as funcionalidades do módulo
      const novasFuncionalidades = funcionalidadesSelecionadas.filter(
        (f) => !funcsDoModulo.some((func) => func.id === f.funcionalidadeId)
      );
      onFuncionalidadesChange(novasFuncionalidades);
    } else {
      // Marcar todas as funcionalidades do módulo
      const funcionalidadesParaAdicionar = funcsDoModulo
        .filter((func) => !funcionalidadesSelecionadas.some((f) => f.funcionalidadeId === func.id))
        .map((func) => ({
          funcionalidadeId: func.id,
          moduloId: func.moduloId,
        }));
      onFuncionalidadesChange([...funcionalidadesSelecionadas, ...funcionalidadesParaAdicionar]);
    }
  };

  // Toggle funcionalidade
  const toggleFuncionalidade = (funcionalidadeId: string, moduloId: string) => {
    if (isAdmin) return; // Admin não pode alterar seleções
    if (isFuncionalidadeSelecionada(funcionalidadeId)) {
      // Remover funcionalidade
      onFuncionalidadesChange(
        funcionalidadesSelecionadas.filter((f) => f.funcionalidadeId !== funcionalidadeId)
      );
    } else {
      // Adicionar funcionalidade
      onFuncionalidadesChange([
        ...funcionalidadesSelecionadas,
        { funcionalidadeId, moduloId },
      ]);
    }
  };

  const toggleExpandirModulo = (moduloId: string) => {
    setModulosExpandidos((prev) => {
      const novo = new Set(prev);
      if (novo.has(moduloId)) {
        novo.delete(moduloId);
      } else {
        novo.add(moduloId);
      }
      return novo;
    });
  };

  const toggleExpandirFuncionalidade = (funcionalidadeId: string) => {
    setFuncionalidadesExpandidas((prev) => {
      const novo = new Set(prev);
      if (novo.has(funcionalidadeId)) {
        novo.delete(funcionalidadeId);
      } else {
        novo.add(funcionalidadeId);
      }
      return novo;
    });
  };

  return (
    <div className="space-y-2 max-h-[500px] overflow-y-auto border border-borderDefault rounded-lg p-4">
      {modulos.map((modulo) => {
        const funcionalidadesDoModulo = getFuncionalidadesByModulo(modulo.id);
        const parametrosDoModulo = getParametrosByModulo(modulo.id);
        const isModuloExpanded = modulosExpandidos.has(modulo.id);
        const moduloCompleto = isModuloCompletamenteSelecionado(modulo.id);
        const temConteudo = funcionalidadesDoModulo.length > 0 || parametrosDoModulo.length > 0;

        return (
          <div key={modulo.id} className="space-y-1">
            {/* Módulo */}
            <ModuloCheckbox
              moduloId={modulo.id}
              moduloNome={modulo.nome}
              moduloDescricao={modulo.descricao}
              temConteudo={temConteudo}
              isModuloExpanded={isModuloExpanded}
              moduloCompleto={moduloCompleto}
              isAdmin={isAdmin}
              onToggleExpandir={() => toggleExpandirModulo(modulo.id)}
              onToggleModulo={() => toggleModulo(modulo.id)}
            />

            {/* Funcionalidades e Parâmetros do Módulo */}
            {isModuloExpanded && (
              <div className="pl-7 space-y-1">
                {/* Funcionalidades do Módulo (mostradas primeiro) */}
                {funcionalidadesDoModulo.map((funcionalidade) => {
                  const parametrosDaFuncionalidade = getParametrosByFuncionalidade(funcionalidade.id);
                  const isFuncExpanded = funcionalidadesExpandidas.has(funcionalidade.id);
                  const isFuncSelected = isFuncionalidadeSelecionada(funcionalidade.id);

                  return (
                    <div key={funcionalidade.id} className="space-y-1">
                      {/* Funcionalidade */}
                      <div className="flex items-center space-x-2 py-1">
                        {parametrosDaFuncionalidade.length > 0 && (
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-5 w-5"
                            onClick={() => toggleExpandirFuncionalidade(funcionalidade.id)}
                          >
                            {isFuncExpanded ? (
                              <ChevronDown className="h-4 w-4" />
                            ) : (
                              <ChevronRight className="h-4 w-4" />
                            )}
                          </Button>
                        )}
                        {parametrosDaFuncionalidade.length === 0 && <div className="w-5" />}
                        <Checkbox
                          id={`funcionalidade-${funcionalidade.id}`}
                          checked={isFuncSelected}
                          onCheckedChange={() =>
                            toggleFuncionalidade(funcionalidade.id, funcionalidade.moduloId)
                          }
                          disabled={isAdmin}
                        />
                        <Label
                          htmlFor={`funcionalidade-${funcionalidade.id}`}
                          className="text-sm cursor-pointer flex-1"
                        >
                          {funcionalidade.nome} - {funcionalidade.descricao}
                        </Label>
                      </div>

                      {/* Parâmetros da Funcionalidade (mostrados depois da funcionalidade) */}
                      {isFuncExpanded && parametrosDaFuncionalidade.length > 0 && (
                        <div className="pl-7 space-y-1">
                          {parametrosDaFuncionalidade.map((parametro) => {
                            const isParamSelected = isParametroSelecionado(parametro.id);
                            return (
                              <div key={parametro.id} className="flex items-center space-x-2 py-1">
                                <div className="w-5" />
                                <Checkbox
                                  id={`parametro-func-${parametro.id}`}
                                  checked={isParamSelected}
                                  onCheckedChange={() =>
                                    toggleParametro(parametro.id, parametro.moduloId, funcionalidade.id)
                                  }
                                  disabled={isAdmin}
                                />
                                <Label
                                  htmlFor={`parametro-func-${parametro.id}`}
                                  className="text-sm cursor-pointer flex-1"
                                >
                                  {parametro.codigo} - {parametro.descricao}
                                </Label>
                              </div>
                            );
                          })}
                        </div>
                      )}
                    </div>
                  );
                })}

                {/* Parâmetros do Módulo (mostrados depois das funcionalidades) */}
                {parametrosDoModulo.length > 0 && (
                  <div className="space-y-1 pl-6">
                    {parametrosDoModulo.map((parametro) => {
                      const isParamSelected = isParametroSelecionado(parametro.id);
                      return (
                        <div key={parametro.id} className="flex items-center space-x-2 py-1">
                          <div className="w-5" />
                          <Checkbox
                            id={`parametro-modulo-${parametro.id}`}
                            checked={isParamSelected}
                            onCheckedChange={() =>
                              toggleParametro(parametro.id, parametro.moduloId)
                            }
                            disabled={isAdmin}
                          />
                          <Label
                            htmlFor={`parametro-modulo-${parametro.id}`}
                            className="text-sm cursor-pointer flex-1"
                          >
                            {parametro.codigo} - {parametro.descricao}
                          </Label>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            )}
          </div>
        );
      })}
    </div>
  );
}

// Componente auxiliar para checkbox de módulo
function ModuloCheckbox({
  moduloId,
  moduloNome,
  moduloDescricao,
  temConteudo,
  isModuloExpanded,
  moduloCompleto,
  isAdmin = false,
  onToggleExpandir,
  onToggleModulo,
}: {
  moduloId: string;
  moduloNome: string;
  moduloDescricao: string;
  temConteudo: boolean;
  isModuloExpanded: boolean;
  moduloCompleto: boolean;
  isAdmin?: boolean;
  onToggleExpandir: () => void;
  onToggleModulo: () => void;
}) {
  return (
    <div className="flex items-center space-x-2 py-1">
      {temConteudo && (
        <Button
          variant="ghost"
          size="icon"
          className="h-5 w-5"
          onClick={onToggleExpandir}
        >
          {isModuloExpanded ? (
            <ChevronDown className="h-4 w-4" />
          ) : (
            <ChevronRight className="h-4 w-4" />
          )}
        </Button>
      )}
      {!temConteudo && <div className="w-5" />}
                        <Checkbox
                          id={`modulo-${moduloId}`}
                          checked={moduloCompleto}
                          onCheckedChange={onToggleModulo}
                          disabled={isAdmin}
                        />
      <Label
        htmlFor={`modulo-${moduloId}`}
        className="text-sm font-medium cursor-pointer flex-1"
      >
        {moduloNome} - {moduloDescricao}
      </Label>
    </div>
  );
}
