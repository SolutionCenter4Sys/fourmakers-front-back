import { useState, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import type { ColaboradorCch } from '@domain/entities/ColaboradorCch';
import type { InserirInformacoesComplementaresPayload } from '@domain/entities/GestaoVagasCandidatos';
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase';
import { ListarUnidadesVagaUseCase } from '@domain/usecases/ListarUnidadesVagaUseCase';
import { ListarTiposVagaUseCase } from '@domain/usecases/ListarTiposVagaUseCase';
import { ListarTiposContratacaoVagaUseCase } from '@domain/usecases/ListarTiposContratacaoVagaUseCase';
import { ListarGestoresUseCase } from '@domain/usecases/ListarGestoresUseCase';
import { InserirInformacoesComplementaresVagaUseCase } from '@domain/usecases/InserirInformacoesComplementaresVagaUseCase';
import { MudarStatusVagaUseCase } from '@domain/usecases/MudarStatusVagaUseCase';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import { ScrollArea } from '@/components/ui/scroll-area';
import { ChevronDown, Plus } from '@/components/ui/system-icons';

export interface GestorExternoItem {
  codigoInternoColaborador: string;
  nome: string;
  email: string;
  codGestorExterno?: string;
}

interface MovimentacaoVagaModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  vagaId: string;
  codigoVaga: string;
  tituloVaga: string;
  idPerfilGerador?: string;
  /** Código do cliente (ex.: 00012026) para listar gestores externos. */
  codigoCliente?: string;
  /** Código numérico do status de destino (ex.: 2). Permite reutilizar o modal em outros fluxos (2→3, 3→4). */
  codigoStatusDestino?: number;
  token: string | null;
  onSuccess?: () => void;
}

const EMAIL_PADRAO = 'talentacquisition@foursys.com.br';

export function MovimentacaoVagaModal({
  open,
  onOpenChange,
  vagaId,
  codigoVaga,
  tituloVaga,
  idPerfilGerador: _idPerfilGerador,
  codigoCliente,
  codigoStatusDestino = 2,
  token,
  onSuccess,
}: MovimentacaoVagaModalProps) {
  const [saving, setSaving] = useState(false);
  const [gestoresExternos, setGestoresExternos] = useState<GestorExternoItem[]>([]);
  const [colaboradores, setColaboradores] = useState<ColaboradorCch[]>([]);
  const [unidades, setUnidades] = useState<Array<{ id: string; descricao: string }>>([]);
  const [tiposVaga, setTiposVaga] = useState<Array<{ id: string; descricao: string }>>([]);
  const [tiposContratacao, setTiposContratacao] = useState<Array<{ id: number; descricao: string }>>([]);

  const [gestorCodigo, setGestorCodigo] = useState<string>('');
  const [propostaCrm, setPropostaCrm] = useState('');
  const [tipoVagaId, setTipoVagaId] = useState<string>('');
  const [tipoContratacaoId, setTipoContratacaoId] = useState<number | null>(null);
  const [unidadeId, setUnidadeId] = useState<string>('');
  const [maquinaColaborador, setMaquinaColaborador] = useState('');
  const [numeroDeVagas, setNumeroDeVagas] = useState(1);
  const [recrutadorCodigo, setRecrutadorCodigo] = useState<string>('');
  const [emailsAdicionais, setEmailsAdicionais] = useState<string[]>([]);
  const [emailInput, setEmailInput] = useState('');
  const [observacoesInternas, setObservacoesInternas] = useState('');

  const [gestorPopoverOpen, setGestorPopoverOpen] = useState(false);
  const [recrutadorPopoverOpen, setRecrutadorPopoverOpen] = useState(false);

  const listarColaboradoresOrg = container.resolve(ListarColaboradoresOrgUseCase);
  const listarUnidades = container.resolve(ListarUnidadesVagaUseCase);
  const listarTiposVaga = container.resolve(ListarTiposVagaUseCase);
  const listarTiposContratacao = container.resolve(ListarTiposContratacaoVagaUseCase);
  const listarGestores = container.resolve(ListarGestoresUseCase);
  const inserirInformacoes = container.resolve(InserirInformacoesComplementaresVagaUseCase);
  const mudarStatusVaga = container.resolve(MudarStatusVagaUseCase);

  useEffect(() => {
    if (!open || !token) return;
    const load = async () => {
      try {
        const [colabRes, unidRes, tiposVRes, tiposCRes, gestoresRes] = await Promise.all([
          listarColaboradoresOrg.execute(token, { busca: '', cursor: 0, limite: 20000 }),
          listarUnidades.execute(token),
          listarTiposVaga.execute(token),
          listarTiposContratacao.execute(token),
          codigoCliente
            ? listarGestores.execute(token, codigoCliente, '').then((list) =>
                list.map((g) => ({
                  codigoInternoColaborador: g.id,
                  nome: g.descricao,
                  email: g.email ?? '',
                  codGestorExterno: g.codGestorExterno,
                }))
              )
            : Promise.resolve([]),
        ]);
        setColaboradores(colabRes?.ColaboradoresCchResult ?? []);
        setUnidades(unidRes);
        setTiposVaga(tiposVRes);
        setTiposContratacao(tiposCRes);
        setGestoresExternos(gestoresRes);
      } catch {
        setColaboradores([]);
        setUnidades([]);
        setTiposVaga([]);
        setTiposContratacao([]);
        setGestoresExternos([]);
      }
    };
    load();
  }, [open, token, codigoCliente, listarColaboradoresOrg, listarUnidades, listarTiposVaga, listarTiposContratacao, listarGestores]);

  const addEmail = () => {
    const email = emailInput.trim();
    if (email && !emailsAdicionais.includes(email)) {
      setEmailsAdicionais((prev) => [...prev, email]);
      setEmailInput('');
    }
  };

  const removeEmail = (email: string) => {
    setEmailsAdicionais((prev) => prev.filter((e) => e !== email));
  };

  const selectedGestor = gestoresExternos.find((g) => g.codigoInternoColaborador === gestorCodigo);
  const selectedRecrutador = colaboradores.find((c) => c.codigoColaboradorInterno === recrutadorCodigo);

  const handleConfirmar = async () => {
    if (!token) return;
    setSaving(true);
    try {
      const payload: InserirInformacoesComplementaresPayload = {
        idVaga: vagaId,
        colaboradorCodigoInternoColaboradorGestorOrgLogada: gestorCodigo?.trim() || null,
        propostaCrm: propostaCrm.trim() || null,
        tipoVagaId: tipoVagaId || null,
        tipoContratacaoId: tipoContratacaoId ?? null,
        unidadeId: unidadeId || null,
        codColaboradoresEntrevistadores: null,
        numeroDeVagas,
        maquinaColaborador: maquinaColaborador.trim() || null,
        recrutadorVaga: recrutadorCodigo || null,
        emailsAnaliseGestor: [EMAIL_PADRAO, ...emailsAdicionais].filter(Boolean),
        observacoesInternas: observacoesInternas.trim() || null,
      };
      const resPut = await inserirInformacoes.execute(token, payload);
      if (resPut?.sucesso === false) {
        toast.error(resPut?.mensagem ?? 'Erro ao inserir informações.');
        setSaving(false);
        return;
      }
      toast.success(resPut?.mensagem ?? 'Dados complementares salvos.');
      const resPost = await mudarStatusVaga.execute(token, {
        codigoVaga,
        codigoStatus: codigoStatusDestino,
        comentarioVaga: '',
      });
      if (resPost?.sucesso === false) {
        toast.error(resPost?.mensagem ?? 'Erro ao alterar status.');
        setSaving(false);
        return;
      }
      toast.success(resPost?.mensagem ?? 'Status alterado com sucesso.');
      onOpenChange(false);
      onSuccess?.();
    } catch (e) {
      toast.error('Erro ao confirmar movimentação.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange} modal={false}>
      <DialogContent className="sm:max-w-xl max-h-[90vh] overflow-y-auto" overlayClassName="!bg-black/80" forceOverlay aria-labelledby="movimentacao-vaga-title">
        <DialogHeader>
          <DialogTitle id="movimentacao-vaga-title">Movimentação - {tituloVaga}</DialogTitle>
          <DialogDescription>
            Confirme os dados adicionais para realizar a movimentação da vaga.
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-2">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Gestor Interno</Label>
              <Popover open={gestorPopoverOpen} onOpenChange={setGestorPopoverOpen}>
                <PopoverTrigger asChild>
                  <Button variant="outline" className="w-full justify-between font-normal" role="combobox">
                    <span className={cn(!selectedGestor && 'text-muted-foreground')}>
                      {selectedGestor ? selectedGestor.nome : 'Pesquise o nome do gestor'}
                    </span>
                    <ChevronDown className="h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="z-[120] w-[var(--radix-popover-trigger-width)] p-0 [&_[data-selected=true]]:!text-white [&_[data-selected=true]_*]:!text-white" data-scroll-lock-ignore>
                  <Command>
                    <CommandInput placeholder="Buscar gestor..." />
                    <ScrollArea className="h-[300px]" data-scroll-lock-ignore>
                      <CommandList className="max-h-none overflow-visible border-0 p-0">
                        <CommandEmpty>Nenhum gestor encontrado.</CommandEmpty>
                        <CommandGroup>
                          {gestoresExternos.map((g) => (
                            <CommandItem
                              key={g.codigoInternoColaborador}
                              value={`${g.nome} ${g.email ?? ''}`.trim()}
                              onSelect={() => {
                                setGestorCodigo(g.codigoInternoColaborador);
                                setGestorPopoverOpen(false);
                              }}
                              className="group flex flex-col items-start gap-0 py-2 data-[selected=true]:!text-white"
                            >
                              <span className="text-foreground group-data-[selected=true]:!text-white">{g.nome}</span>
                              {g.email ? <span className="text-foreground text-sm group-data-[selected=true]:!text-white">{g.email}</span> : null}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </CommandList>
                    </ScrollArea>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>
            <div className="space-y-2">
              <Label>Proposta / Oportunidade CRM</Label>
              <Input
                placeholder="Digite aqui e pressione enter..."
                value={propostaCrm}
                onChange={(e) => setPropostaCrm(e.target.value)}
              />
            </div>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Tipo de Vaga</Label>
              <Select value={tipoVagaId} onValueChange={setTipoVagaId}>
                <SelectTrigger><SelectValue placeholder="Selecione..." /></SelectTrigger>
                <SelectContent className="z-[120]" data-scroll-lock-ignore>
                  {tiposVaga.map((t) => (
                    <SelectItem key={t.id} value={t.id}>{t.descricao}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Modelo de contratação</Label>
              <Select
                value={tipoContratacaoId != null ? String(tipoContratacaoId) : ''}
                onValueChange={(v) => setTipoContratacaoId(v ? Number(v) : null)}
              >
                <SelectTrigger><SelectValue placeholder="Selecione..." /></SelectTrigger>
                <SelectContent className="z-[120]" data-scroll-lock-ignore>
                  {tiposContratacao.map((t) => (
                    <SelectItem key={t.id} value={String(t.id)}>{t.descricao}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Unidade</Label>
              <Select value={unidadeId} onValueChange={setUnidadeId}>
                <SelectTrigger><SelectValue placeholder="Selecione..." /></SelectTrigger>
                <SelectContent className="z-[120]" data-scroll-lock-ignore>
                  {unidades.map((u) => (
                    <SelectItem key={u.id} value={u.id}>{u.descricao}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Máquina</Label>
              <Select value={maquinaColaborador} onValueChange={setMaquinaColaborador}>
                <SelectTrigger><SelectValue placeholder="Selecione..." /></SelectTrigger>
                <SelectContent className="z-[120]" data-scroll-lock-ignore>
                  <SelectItem value="Foursys">Foursys</SelectItem>
                  <SelectItem value="Cliente">Cliente</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Qtd de Vagas</Label>
              <div className="flex items-center gap-2">
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  className="h-9 w-9"
                  onClick={() => setNumeroDeVagas((n) => Math.max(1, n - 1))}
                >
                  —
                </Button>
                <Input
                  type="number"
                  min={1}
                  value={numeroDeVagas}
                  onChange={(e) => setNumeroDeVagas(Math.max(1, parseInt(e.target.value, 10) || 1))}
                  className="w-20 text-center"
                />
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  className="h-9 w-9"
                  onClick={() => setNumeroDeVagas((n) => n + 1)}
                >
                  +
                </Button>
              </div>
            </div>
            <div className="space-y-2">
              <Label>Recrutador Responsável</Label>
              <Popover open={recrutadorPopoverOpen} onOpenChange={setRecrutadorPopoverOpen}>
                <PopoverTrigger asChild>
                  <Button variant="outline" className="w-full justify-between font-normal" role="combobox">
                    <span className={cn(!selectedRecrutador && 'text-muted-foreground')}>
                      {selectedRecrutador?.nm_Profissional ?? 'Pesquise o nome do responsável'}
                    </span>
                    <ChevronDown className="h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="z-[120] w-[var(--radix-popover-trigger-width)] p-0" data-scroll-lock-ignore>
                  <Command>
                    <CommandInput placeholder="Buscar recrutador..." />
                    <ScrollArea className="h-[300px]" data-scroll-lock-ignore>
                      <CommandList className="max-h-none overflow-visible border-0 p-0">
                        <CommandEmpty>Nenhum colaborador encontrado.</CommandEmpty>
                        <CommandGroup>
                          {colaboradores.map((c) => (
                            <CommandItem
                              key={c.codigoColaboradorInterno}
                              value={c.nm_Profissional ?? ''}
                              onSelect={() => {
                                setRecrutadorCodigo(c.codigoColaboradorInterno);
                                setRecrutadorPopoverOpen(false);
                              }}
                            >
                              {c.nm_Profissional}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </CommandList>
                    </ScrollArea>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>
          </div>
          <div className="space-y-2">
            <Label>Envio de Onepage aos Cliente</Label>
            <p className="text-xs text-muted-foreground">
              Será enviado para {EMAIL_PADRAO}. Informe acima e-mails adicionais.
            </p>
            <div className="flex gap-2">
              <Input
                type="email"
                placeholder="Adicionar e-mail para envio ao cliente"
                value={emailInput}
                onChange={(e) => setEmailInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), addEmail())}
              />
              <Button type="button" variant="outline" size="icon" onClick={addEmail} aria-label="Adicionar e-mail">
                <Plus className="h-4 w-4" />
              </Button>
            </div>
            {emailsAdicionais.length > 0 && (
              <ul className="flex flex-wrap gap-1 mt-1">
                {emailsAdicionais.map((email) => (
                  <li key={email}>
                    <span className="inline-flex items-center rounded-md bg-muted px-2 py-0.5 text-xs">
                      {email}
                      <button
                        type="button"
                        className="ml-1 hover:text-destructive"
                        onClick={() => removeEmail(email)}
                        aria-label={`Remover ${email}`}
                      >
                        ×
                      </button>
                    </span>
                  </li>
                ))}
              </ul>
            )}
          </div>
          <div className="space-y-2">
            <Label>Observações Internas</Label>
            <Textarea
              placeholder="Inserir observações internas"
              value={observacoesInternas}
              onChange={(e) => setObservacoesInternas(e.target.value)}
              rows={3}
            />
          </div>
        </div>
        <DialogFooter>
          <Button onClick={handleConfirmar} disabled={saving}>
            {saving ? 'Salvando…' : 'Confirmar e alterar status'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
