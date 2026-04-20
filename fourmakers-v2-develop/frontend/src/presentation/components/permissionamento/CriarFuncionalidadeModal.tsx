import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import type { Funcionalidade, Modulo } from '@presentation/hooks/useParametrosFuncionalidades';

interface CriarFuncionalidadeModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  funcionalidade?: Funcionalidade | null;
  moduloId?: string | null;
  modulos: Modulo[];
  onSave: (funcionalidade: Omit<Funcionalidade, 'id' | 'createdAt' | 'updatedAt'>) => void;
}

const tiposFuncionalidade: Array<{ value: Funcionalidade['tipo']; label: string }> = [
  { value: 'visualizacao', label: 'Visualização' },
  { value: 'edicao', label: 'Edição' },
  { value: 'criacao', label: 'Criação' },
  { value: 'exclusao', label: 'Exclusão' },
  { value: 'relatorio', label: 'Relatório' },
  { value: 'configuracao', label: 'Configuração' },
];

export function CriarFuncionalidadeModal({
  open,
  onOpenChange,
  funcionalidade,
  moduloId,
  modulos,
  onSave,
}: CriarFuncionalidadeModalProps) {
  const [codigo, setCodigo] = useState('');
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');
  const [moduloSelecionado, setModuloSelecionado] = useState('');
  const [tipo, setTipo] = useState<Funcionalidade['tipo']>('visualizacao');

  useEffect(() => {
    if (open) {
      if (funcionalidade) {
        setCodigo(funcionalidade.codigo);
        setNome(funcionalidade.nome);
        setDescricao(funcionalidade.descricao);
        setModuloSelecionado(funcionalidade.moduloId);
        setTipo(funcionalidade.tipo);
      } else {
        setCodigo('');
        setNome('');
        setDescricao('');
        setModuloSelecionado(moduloId || '');
        setTipo('visualizacao');
      }
    }
  }, [open, funcionalidade, moduloId]);

  const handleSave = () => {
    if (!codigo.trim() || !nome.trim() || !descricao.trim() || !moduloSelecionado) {
      return;
    }

    onSave({
      codigo: codigo.trim(),
      nome: nome.trim(),
      descricao: descricao.trim(),
      moduloId: moduloSelecionado,
      tipo,
    });

    setCodigo('');
    setNome('');
    setDescricao('');
    setModuloSelecionado('');
    setTipo('visualizacao');
    onOpenChange(false);
  };

  const handleClose = (open: boolean) => {
    if (!open) {
      setCodigo('');
      setNome('');
      setDescricao('');
      setModuloSelecionado('');
      setTipo('visualizacao');
    }
    onOpenChange(open);
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{funcionalidade ? 'Editar Funcionalidade' : 'Criar Funcionalidade'}</DialogTitle>
          <DialogDescription>
            {funcionalidade ? 'Edite as informações da funcionalidade do sistema.' : 'Preencha os dados para criar uma nova funcionalidade do sistema.'}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>
                Código * <span className="text-xs text-muted-foreground">(Ex: VISUALIZAR_APONTAMENTOS)</span>
              </Label>
              <Input
                placeholder="Código da funcionalidade"
                value={codigo}
                onChange={(e) => setCodigo(e.target.value.toUpperCase().replace(/\s/g, '_'))}
              />
            </div>

            <div className="space-y-2">
              <Label>Módulo *</Label>
              <Select value={moduloSelecionado} onValueChange={setModuloSelecionado} disabled={!!moduloId}>
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
          </div>

          <div className="space-y-2">
            <Label>Nome *</Label>
            <Input
              placeholder="Nome da funcionalidade"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label>Tipo *</Label>
            <Select value={tipo} onValueChange={(value) => setTipo(value as Funcionalidade['tipo'])}>
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {tiposFuncionalidade.map((t) => (
                  <SelectItem key={t.value} value={t.value}>
                    {t.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label>Descrição *</Label>
            <textarea
              className="flex min-h-[100px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              placeholder="Descreva a funcionalidade..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={!codigo.trim() || !nome.trim() || !descricao.trim() || !moduloSelecionado}>
            {funcionalidade ? 'Salvar' : 'Criar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
