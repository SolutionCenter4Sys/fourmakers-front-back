import { useState } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Eye } from '@/components/ui/system-icons';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Label } from '@/components/ui/label';

export function AuditoriaTab() {
  const [org, setOrg] = useState('');
  const [usuario, setUsuario] = useState('');
  const [perfil, setPerfil] = useState('');
  const [cliente, setCliente] = useState('');
  const [periodo, setPeriodo] = useState('');

  // Dados mockados - serão substituídos pela integração com API
  const logs = [
    {
      id: '1',
      data: '27/01 10h',
      admin: 'ER',
      acao: 'Edit',
      entidade: 'Perfil',
    },
    {
      id: '2',
      data: '26/01 18h',
      admin: 'Admin',
      acao: 'Criar',
      entidade: 'Grupo',
    },
  ];

  return (
    <div className="space-y-4">
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div>
              <h2 className="text-lg font-semibold">Auditoria de Acessos</h2>
              <p className="text-sm text-muted-foreground mt-1">
                Todas as alterações são registradas
              </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
              <div className="space-y-2">
                <Label>Org</Label>
                <Select value={org} onValueChange={setOrg}>
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todas</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Usuário</Label>
                <Input
                  placeholder="Buscar usuário..."
                  value={usuario}
                  onChange={(e) => setUsuario(e.target.value)}
                />
              </div>

              <div className="space-y-2">
                <Label>Perfil</Label>
                <Select value={perfil} onValueChange={setPerfil}>
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todos</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Cliente</Label>
                <Select value={cliente} onValueChange={setCliente}>
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">Todos</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label>Período</Label>
                <Input
                  type="date"
                  value={periodo}
                  onChange={(e) => setPeriodo(e.target.value)}
                />
              </div>
            </div>

            <div className="border border-borderDefault rounded-lg overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Data / Hora</TableHead>
                    <TableHead>Administrador</TableHead>
                    <TableHead>Ação</TableHead>
                    <TableHead>Entidade</TableHead>
                    <TableHead className="text-right">Detalhes</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {logs.map((log) => (
                    <TableRow key={log.id}>
                      <TableCell>{log.data}</TableCell>
                      <TableCell>{log.admin}</TableCell>
                      <TableCell>{log.acao}</TableCell>
                      <TableCell>{log.entidade}</TableCell>
                      <TableCell className="text-right">
                        <Button variant="ghost" size="icon">
                          <Eye className="h-4 w-4" />
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
