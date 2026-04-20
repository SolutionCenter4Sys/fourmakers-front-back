import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { container } from 'tsyringe';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible';
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
import { ScrollArea } from '@/components/ui/scroll-area';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { ObterVagaRecrutamentoPorIdUseCase } from '@domain/usecases/ObterVagaRecrutamentoPorIdUseCase';
import { AtualizarVagaRecrutamentoPorIdUseCase } from '@domain/usecases/AtualizarVagaRecrutamentoPorIdUseCase';
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase';
import { ListarUnidadesVagaUseCase } from '@domain/usecases/ListarUnidadesVagaUseCase';
import { ListarTiposVagaUseCase } from '@domain/usecases/ListarTiposVagaUseCase';
import { ListarTiposContratacaoVagaUseCase } from '@domain/usecases/ListarTiposContratacaoVagaUseCase';
import { ListarGestoresUseCase } from '@domain/usecases/ListarGestoresUseCase';
import type { VagaRecrutamentoCompleto } from '@domain/entities/GestaoVagasCandidatos';
import type { ColaboradorCch } from '@domain/entities/ColaboradorCch';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import { ArrowLeft, ChevronDown, Plus } from '@/components/ui/system-icons';
import { Spinner } from '@/components/ui/spinner';

const EMAIL_PADRAO = 'talentacquisition@foursys.com.br';

interface GestorExternoItem {
  codigoInternoColaborador: string;
  nome: string;
  email: string;
}

export default function EditarVaga() {
  const { vagaId } = useParams<{ vagaId: string }>();
  const navigate = useNavigate();
  const token = useAppSelector((s) => s.auth.token);

  const [vaga, setVaga] = useState<VagaRecrutamentoCompleto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [gestoresExternos, setGestoresExternos] = useState<GestorExternoItem[]>([]);
  const [colaboradores, setColaboradores] = useState<ColaboradorCch[]>([]);
  const [unidades, setUnidades] = useState<Array<{ id: string; descricao: string }>>([]);
  const [tiposVaga, setTiposVaga] = useState<Array<{ id: string; descricao: string }>>([]);
  const [tiposContratacao, setTiposContratacao] = useState<Array<{ id: number; descricao: string }>>([]);

  const [gestorCodigo, setGestorCodigo] = useState('');
  const [propostaCrm, setPropostaCrm] = useState('');
  const [tipoVagaId, setTipoVagaId] = useState('');
  const [tipoContratacaoId, setTipoContratacaoId] = useState<number | null>(null);
  const [unidadeId, setUnidadeId] = useState('');
  const [maquinaColaborador, setMaquinaColaborador] = useState('');
  const [numeroDeVagas, setNumeroDeVagas] = useState(1);
  const [recrutadorCodigo, setRecrutadorCodigo] = useState('');
  const [emailsAdicionais, setEmailsAdicionais] = useState<string[]>([]);
  const [emailInput, setEmailInput] = useState('');
  const [observacoesInternas, setObservacoesInternas] = useState('');
  const [gestorPopoverOpen, setGestorPopoverOpen] = useState(false);
  const [recrutadorPopoverOpen, setRecrutadorPopoverOpen] = useState(false);
  const [detalhesOpen, setDetalhesOpen] = useState(false);

  const obterVaga = container.resolve(ObterVagaRecrutamentoPorIdUseCase);
  const atualizarVaga = container.resolve(AtualizarVagaRecrutamentoPorIdUseCase);
  const listarColaboradoresOrg = container.resolve(ListarColaboradoresOrgUseCase);
  const listarUnidades = container.resolve(ListarUnidadesVagaUseCase);
  const listarTiposVaga = container.resolve(ListarTiposVagaUseCase);
  const listarTiposContratacao = container.resolve(ListarTiposContratacaoVagaUseCase);
  const listarGestores = container.resolve(ListarGestoresUseCase);

  const loadVaga = useCallback(async () => {
    if (!vagaId || !token) return;
    setLoading(true);
    try {
      const data = await obterVaga.execute(token, vagaId);
      setVaga(data);
      if (data) {
        setGestorCodigo(data.codigoGestor ?? '');
        setPropostaCrm(data.propostaCrm ?? '');
        setTipoVagaId(data.tipoVagaId ?? '');
        setTipoContratacaoId(data.tipoContratacaoId ?? null);
        setUnidadeId(data.unidadeId ?? '');
        setMaquinaColaborador(data.maquinaColaborador ?? '');
        setNumeroDeVagas(data.numeroDeVagas ?? 1);
        setRecrutadorCodigo(data.recrutadorVaga ?? '');
        const emails = (data.emailsAnaliseGestor ?? []).filter((e) => e && e !== EMAIL_PADRAO);
        setEmailsAdicionais(emails);
        setObservacoesInternas(data.observacoesInternas ?? '');
      }
    } catch {
      toast.error('Não foi possível carregar a vaga.');
      setVaga(null);
    } finally {
      setLoading(false);
    }
  }, [vagaId, token, obterVaga]);

  useEffect(() => {
    loadVaga();
  }, [loadVaga]);

  useEffect(() => {
    if (!token) return;
    const loadLookups = async () => {
      try {
        const [colabRes, unidRes, tiposVRes, tiposCRes] = await Promise.all([
          listarColaboradoresOrg.execute(token, { busca: '', cursor: 0, limite: 20000 }),
          listarUnidades.execute(token),
          listarTiposVaga.execute(token),
          listarTiposContratacao.execute(token),
        ]);
        setColaboradores(colabRes?.ColaboradoresCchResult ?? []);
        setUnidades(unidRes);
        setTiposVaga(tiposVRes);
        setTiposContratacao(tiposCRes);
      } catch {
        setColaboradores([]);
        setUnidades([]);
        setTiposVaga([]);
        setTiposContratacao([]);
      }
    };
    loadLookups();
  }, [token, listarColaboradoresOrg, listarUnidades, listarTiposVaga, listarTiposContratacao]);

  useEffect(() => {
    if (!token || !vaga?.codigoCliente) return;
    listarGestores
      .execute(token, vaga.codigoCliente, '')
      .then((list) =>
        setGestoresExternos(
          list.map((g) => ({
            codigoInternoColaborador: g.id,
            nome: g.descricao,
            email: g.email ?? '',
          }))
        )
      )
      .catch(() => setGestoresExternos([]));
  }, [token, vaga?.codigoCliente, listarGestores]);

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

  const handleSalvar = async () => {
    if (!token || !vaga) return;
    setSaving(true);
    try {
      const payload: VagaRecrutamentoCompleto = {
        ...vaga,
        codigoGestor: gestorCodigo?.trim() || null,
        propostaCrm: propostaCrm.trim() || null,
        tipoVagaId: tipoVagaId || null,
        tipoContratacaoId: tipoContratacaoId ?? null,
        unidadeId: unidadeId || null,
        maquinaColaborador: maquinaColaborador.trim() || null,
        numeroDeVagas,
        recrutadorVaga: recrutadorCodigo || null,
        emailsAnaliseGestor: [EMAIL_PADRAO, ...emailsAdicionais].filter(Boolean),
        observacoesInternas: observacoesInternas.trim() || null,
      };
      const res = await atualizarVaga.execute(token, payload);
      if (res?.sucesso === false) {
        toast.error(res?.mensagem ?? 'Erro ao salvar.');
        return;
      }
      toast.success(res?.mensagem ?? 'Dados salvos com sucesso.');
    } catch {
      toast.error('Erro ao salvar a vaga.');
    } finally {
      setSaving(false);
    }
  };

  const skillsByType = (vaga?.skills ?? []).reduce<Record<string, Array<{ skillDescription: string; skillNivelDescription: string }>>>(
    (acc, s) => {
      const type = (s as { typeSkillsDescription?: string }).typeSkillsDescription ?? 'Outros';
      if (!acc[type]) acc[type] = [];
      acc[type].push({
        skillDescription: (s as { skillDescription?: string }).skillDescription ?? '',
        skillNivelDescription: (s as { skillNivelDescription?: string }).skillNivelDescription ?? '',
      });
      return acc;
    },
    {}
  );

  if (!vagaId) {
    return (
      <div className="container mx-auto max-w-4xl p-4">
        <p className="text-destructive">ID da vaga não informado.</p>
        <Button variant="outline" className="mt-4" onClick={() => navigate('/recrutamento')}>
          Voltar
        </Button>
      </div>
    );
  }

  return (
    <div className="container mx-auto max-w-4xl p-4 space-y-6">
      <PageBreadcrumb
        items={[
          { label: 'Dashboard', href: '/dashboard' },
          { label: 'Recrutamento', href: '/recrutamento' },
          { label: 'Editar vaga' },
        ]}
      />
      <div className="flex items-center gap-2">
        <Button variant="ghost" size="icon" onClick={() => navigate('/recrutamento')} aria-label="Voltar">
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <PageHeader
          title={`Editar vaga - ${vaga?.titulo ?? '...'}`}
          description="Altere os dados adicionais da vaga. Os detalhes do perfil/vaga são somente leitura."
        />
      </div>

      {loading ? (
        <div className="flex items-center justify-center gap-2 py-12 text-muted-foreground">
          <Spinner size={24} className="shrink-0" />
          <span>Carregando vaga...</span>
        </div>
      ) : !vaga ? (
        <Card className="border-borderSoft">
          <CardContent className="py-8 text-center text-muted-foreground">
            Vaga não encontrada.
            <Button variant="outline" className="mt-4" onClick={() => navigate('/recrutamento')}>
              Voltar ao Kanban
            </Button>
          </CardContent>
        </Card>
      ) : (
        <>
          {/* Seção 1: Dados adicionais da vaga (editável) */}
          <Card className="border-borderSoft">
            <CardHeader>
              <CardTitle>Dados adicionais da vaga</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Gestor</Label>
                  <Popover open={gestorPopoverOpen} onOpenChange={setGestorPopoverOpen}>
                    <PopoverTrigger asChild>
                      <Button variant="outline" className="w-full justify-between font-normal" role="combobox">
                        <span className={cn(!selectedGestor && 'text-muted-foreground')}>
                          {selectedGestor ? selectedGestor.nome : 'Pesquise o nome do gestor'}
                        </span>
                        <ChevronDown className="h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="z-[120] w-[var(--radix-popover-trigger-width)] p-0" data-scroll-lock-ignore>
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
                                >
                                  <span>{g.nome}</span>
                                  {g.email ? (
                                    <span className="text-muted-foreground text-sm ml-1">{g.email}</span>
                                  ) : null}
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
                <Label>Envio de Onepage aos Clientes</Label>
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
              <div className="flex justify-end">
                <Button onClick={handleSalvar} disabled={saving}>
                  {saving ? <Spinner size={16} className="mr-2 shrink-0" /> : null}
                  Salvar dados
                </Button>
              </div>
            </CardContent>
          </Card>

          {/* Seção 2: Detalhes da vaga (somente leitura) */}
          <Collapsible open={detalhesOpen} onOpenChange={setDetalhesOpen}>
            <Card className="border-borderSoft">
              <CollapsibleTrigger asChild>
                <CardHeader className="cursor-pointer hover:bg-muted/50 transition-colors flex flex-row items-center justify-between space-y-0">
                  <CardTitle>Detalhes da vaga</CardTitle>
                  <ChevronDown
                    className={cn('h-5 w-5 transition-transform', detalhesOpen && 'rotate-180')}
                  />
                </CardHeader>
              </CollapsibleTrigger>
              <CollapsibleContent>
                <CardContent className="space-y-4 pt-0">
                  <div className="grid gap-4 sm:grid-cols-2 text-sm">
                    <div>
                      <span className="text-muted-foreground">Título</span>
                      <p className="font-medium">{vaga.titulo ?? '—'}</p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Cargo</span>
                      <p className="font-medium">{vaga.cargo ?? '—'}</p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Cliente</span>
                      <p className="font-medium">{vaga.nomeCliente ?? '—'}</p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Modelo de trabalho</span>
                      <p className="font-medium">{vaga.modeloTrabalhoDescricao ?? '—'}</p>
                    </div>
                  </div>
                  {vaga.descricao ? (
                    <div>
                      <span className="text-muted-foreground text-sm">Descrição</span>
                      <p className="text-sm mt-1 whitespace-pre-wrap max-h-48 overflow-y-auto rounded border border-borderSoft p-3 bg-muted/30">
                        {vaga.descricao}
                      </p>
                    </div>
                  ) : null}
                  {Object.keys(skillsByType).length > 0 ? (
                    <div>
                      <span className="text-muted-foreground text-sm">Skills</span>
                      <div className="mt-2 space-y-2">
                        {Object.entries(skillsByType).map(([type, skills]) => (
                          <div key={type}>
                            <p className="text-xs font-medium text-muted-foreground">{type}</p>
                            <div className="flex flex-wrap gap-1 mt-1">
                              {skills.map((s, i) => (
                                <span
                                  key={i}
                                  className="inline-flex rounded-full border border-borderSoft bg-muted/50 px-2 py-0.5 text-xs"
                                >
                                  {s.skillDescription}
                                  {s.skillNivelDescription ? ` - ${s.skillNivelDescription}` : ''}
                                </span>
                              ))}
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  ) : null}
                </CardContent>
              </CollapsibleContent>
            </Card>
          </Collapsible>
        </>
      )}
    </div>
  );
}
