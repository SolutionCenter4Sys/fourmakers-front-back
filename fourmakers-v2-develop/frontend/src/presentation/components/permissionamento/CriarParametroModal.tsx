import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import type { Parametro, Modulo, Funcionalidade } from '@presentation/hooks/useParametrosFuncionalidades';

interface CriarParametroModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  parametro?: Parametro | null;
  contexto?: { tipo: 'modulo' | 'funcionalidade'; id: string } | null;
  modulos: Modulo[];
  funcionalidades: Funcionalidade[];
  onSave: (parametro: Omit<Parametro, 'id' | 'createdAt' | 'updatedAt'>) => void;
}

const tiposParametro: Array<{ value: Parametro['tipo']; label: string }> = [
  { value: 'boolean', label: 'Boolean (true/false)' },
  { value: 'string', label: 'String (texto)' },
  { value: 'number', label: 'Number (número)' },
  { value: 'json', label: 'JSON (objeto)' },
];

export function CriarParametroModal({
  open,
  onOpenChange,
  parametro,
  contexto,
  modulos,
  funcionalidades,
  onSave,
}: CriarParametroModalProps) {
  const [codigo, setCodigo] = useState('');
  const [descricao, setDescricao] = useState('');
  const [moduloSelecionado, setModuloSelecionado] = useState('');
  const [funcionalidadeSelecionada, setFuncionalidadeSelecionada] = useState('');
  const [tipo, setTipo] = useState<Parametro['tipo']>('boolean');
  const [valorPadrao, setValorPadrao] = useState('');

  // Filtrar funcionalidades do módulo selecionado
  const funcionalidadesDoModulo = moduloSelecionado
    ? funcionalidades.filter((f) => f.moduloId === moduloSelecionado)
    : [];

  useEffect(() => {
    if (open) {
      if (parametro) {
        setCodigo(parametro.codigo);
        setDescricao(parametro.descricao);
        setModuloSelecionado(parametro.moduloId);
        setFuncionalidadeSelecionada(parametro.funcionalidadeId || '');
        setTipo(parametro.tipo);
        setValorPadrao(parametro.valorPadrao);
      } else if (contexto) {
        if (contexto.tipo === 'modulo') {
          setModuloSelecionado(contexto.id);
          setFuncionalidadeSelecionada('');
        } else {
          const func = funcionalidades.find((f) => f.id === contexto.id);
          setModuloSelecionado(func?.moduloId || '');
          setFuncionalidadeSelecionada(contexto.id);
        }
        setCodigo('');
        setDescricao('');
        setTipo('boolean');
        setValorPadrao('');
      } else {
        setCodigo('');
        setDescricao('');
        setModuloSelecionado('');
        setFuncionalidadeSelecionada('');
        setTipo('boolean');
        setValorPadrao('');
      }
    }
  }, [open, parametro, contexto, funcionalidades]);

  // Quando o módulo mudar, limpar a funcionalidade selecionada se ela não pertencer ao novo módulo
  useEffect(() => {
    if (moduloSelecionado && funcionalidadeSelecionada) {
      const func = funcionalidades.find((f) => f.id === funcionalidadeSelecionada);
      if (func && func.moduloId !== moduloSelecionado) {
        setFuncionalidadeSelecionada('');
      }
    }
  }, [moduloSelecionado, funcionalidadeSelecionada, funcionalidades]);

  const handleSave = () => {
    if (!codigo.trim() || !descricao.trim() || !moduloSelecionado || !valorPadrao.trim()) {
      return;
    }

    onSave({
      codigo: codigo.trim(),
      descricao: descricao.trim(),
      moduloId: moduloSelecionado,
      funcionalidadeId: funcionalidadeSelecionada || undefined, // Opcional: pode ser do módulo ou de uma funcionalidade específica
      tipo,
      valorPadrao: valorPadrao.trim(),
    });

    setCodigo('');
    setDescricao('');
    setModuloSelecionado('');
    setFuncionalidadeSelecionada('');
    setTipo('boolean');
    setValorPadrao('');
    onOpenChange(false);
  };

  const handleClose = (open: boolean) => {
    if (!open) {
      setCodigo('');
      setDescricao('');
      setModuloSelecionado('');
      setFuncionalidadeSelecionada('');
      setTipo('boolean');
      setValorPadrao('');
    }
    onOpenChange(open);
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{parametro ? 'Editar Parâmetro' : 'Criar Parâmetro'}</DialogTitle>
          <DialogDescription>
            {parametro ? 'Edite as informações do parâmetro do sistema.' : 'Preencha os dados para criar um novo parâmetro do sistema.'}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4">
          <div className="space-y-2">
            <Label>
              Código * <span className="text-xs text-muted-foreground">(Ex: MOSTRAR_COLUNA_APROVADORES)</span>
            </Label>
            <Input
              placeholder="Código do parâmetro"
              value={codigo}
              onChange={(e) => setCodigo(e.target.value.toUpperCase().replace(/\s/g, '_'))}
            />
          </div>

          <div className="space-y-2">
            <Label>Descrição *</Label>
            <textarea
              className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              placeholder="Descreva o parâmetro..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label>Módulo *</Label>
            <Select value={moduloSelecionado} onValueChange={setModuloSelecionado} disabled={!!contexto?.tipo && contexto.tipo === 'modulo'}>
              <SelectTrigger>
                <SelectValue placeholder="Selecione o módulo" />
              </SelectTrigger>
              <SelectContent>
                {modulos.map((mod) => (
                  <SelectItem key={mod.id} value={mod.id}>
                    {mod.nome}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          {moduloSelecionado && funcionalidadesDoModulo.length > 0 && (
            <div className="space-y-2">
              <Label>
                Funcionalidade <span className="text-xs text-muted-foreground">(Opcional - deixe em branco para parâmetro do módulo)</span>
              </Label>
              <Select 
                value={funcionalidadeSelecionada || undefined} 
                onValueChange={(value) => setFuncionalidadeSelecionada(value || '')}
                disabled={!!contexto?.tipo && contexto.tipo === 'funcionalidade'}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a funcionalidade (opcional)" />
                </SelectTrigger>
                <SelectContent>
                  {funcionalidadesDoModulo.map((func) => (
                    <SelectItem key={func.id} value={func.id}>
                      {func.nome} ({func.codigo})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {funcionalidadeSelecionada && (
                <div className="flex items-center gap-2">
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    className="h-7 text-xs"
                    onClick={() => setFuncionalidadeSelecionada('')}
                  >
                    Limpar seleção (parâmetro do módulo)
                  </Button>
                </div>
              )}
            </div>
          )}

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Tipo *</Label>
              <Select value={tipo} onValueChange={(value) => setTipo(value as Parametro['tipo'])}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {tiposParametro.map((t) => (
                    <SelectItem key={t.value} value={t.value}>
                      {t.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label>Valor Padrão *</Label>
              <Input
                placeholder="true, false, texto, etc."
                value={valorPadrao}
                onChange={(e) => setValorPadrao(e.target.value)}
              />
            </div>
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={!codigo.trim() || !descricao.trim() || !moduloSelecionado || !valorPadrao.trim()}>
            {parametro ? 'Salvar' : 'Criar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
