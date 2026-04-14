import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import type { Modulo } from '@presentation/hooks/useParametrosFuncionalidades';

interface CriarModuloModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  modulo?: Modulo | null;
  onSave: (modulo: Omit<Modulo, 'id' | 'createdAt' | 'updatedAt'>) => void;
}

export function CriarModuloModal({
  open,
  onOpenChange,
  modulo,
  onSave,
}: CriarModuloModalProps) {
  const [codigo, setCodigo] = useState('');
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');

  useEffect(() => {
    if (open) {
      if (modulo) {
        setCodigo(modulo.codigo);
        setNome(modulo.nome);
        setDescricao(modulo.descricao);
      } else {
        setCodigo('');
        setNome('');
        setDescricao('');
      }
    }
  }, [open, modulo]);

  const handleSave = () => {
    if (!codigo.trim() || !nome.trim() || !descricao.trim()) {
      return;
    }

    onSave({
      codigo: codigo.trim(),
      nome: nome.trim(),
      descricao: descricao.trim(),
    });

    setCodigo('');
    setNome('');
    setDescricao('');
    onOpenChange(false);
  };

  const handleClose = (open: boolean) => {
    if (!open) {
      setCodigo('');
      setNome('');
      setDescricao('');
    }
    onOpenChange(open);
  };

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{modulo ? 'Editar Módulo' : 'Criar Módulo'}</DialogTitle>
          <DialogDescription>
            {modulo ? 'Edite as informações do módulo do sistema.' : 'Preencha os dados para criar um novo módulo do sistema.'}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4">
          <div className="space-y-2">
            <Label>
              Código * <span className="text-xs text-muted-foreground">(Ex: APONTAMENTOS)</span>
            </Label>
            <Input
              placeholder="Código do módulo"
              value={codigo}
              onChange={(e) => setCodigo(e.target.value.toUpperCase().replace(/\s/g, '_'))}
            />
          </div>

          <div className="space-y-2">
            <Label>Nome *</Label>
            <Input
              placeholder="Nome do módulo"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label>Descrição *</Label>
            <textarea
              className="flex min-h-[100px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              placeholder="Descreva o módulo..."
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={!codigo.trim() || !nome.trim() || !descricao.trim()}>
            {modulo ? 'Salvar' : 'Criar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
