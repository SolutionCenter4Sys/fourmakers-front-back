import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import type { ColaboradorSelecionado } from '@presentation/hooks/useGrupos';

interface VerMembrosModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  membros: ColaboradorSelecionado[];
  nomeGrupo: string;
}

export function VerMembrosModal({
  open,
  onOpenChange,
  membros,
  nomeGrupo,
}: VerMembrosModalProps) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Membros do Grupo: {nomeGrupo}</DialogTitle>
          <DialogDescription>
            Lista de colaboradores que fazem parte deste grupo de acesso.
          </DialogDescription>
        </DialogHeader>
        <div className="mt-4">
          {membros.length === 0 ? (
            <p className="text-sm text-muted-foreground text-center py-8">
              Nenhum membro cadastrado neste grupo
            </p>
          ) : (
            <div className="border border-borderDefault rounded-lg overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Nome</TableHead>
                    <TableHead>Email</TableHead>
                    <TableHead>Código</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {membros.map((membro) => (
                    <TableRow key={membro.cpf}>
                      <TableCell className="font-medium">{membro.nome}</TableCell>
                      <TableCell>
                        <span className="text-sm text-muted-foreground">{membro.email}</span>
                      </TableCell>
                      <TableCell>
                        <Badge variant="outline">{membro.codColaborador}</Badge>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
