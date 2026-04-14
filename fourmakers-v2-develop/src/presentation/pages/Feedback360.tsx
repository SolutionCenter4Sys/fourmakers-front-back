import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Spinner } from '@/components/ui/spinner';
import { Calendar, Check, Clock, Eye, Search, Send } from '@/components/ui/system-icons';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Textarea } from '@/components/ui/textarea';
import { cn } from '@/lib/utils';
import type { Feedback360Item } from '@domain/entities/Feedback360';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { useFeedback360 } from '@presentation/hooks/useFeedback360';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { useCallback, useEffect, useRef, useState } from 'react';

const PRAZO_EDICAO_MS = 60 * 60 * 1000;
/** Exibir no máximo 59 min 59s (nunca 60 min 0s). */
const MAX_EXIBICAO_MS = PRAZO_EDICAO_MS - 1000;
const MIN_CHARS_BUSCA_COLAB = 2;
const DEBOUNCE_BUSCA_COLAB_MS = 400;
const UUID_REGEX = /[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i;
const DEBOUNCE_ABRIR_POPOVER_MS = 100;
const THROTTLE_SCROLL_LOAD_MS = 500;

/** API envia dataCriacao em UTC sem sufixo (ex: "2026-02-27T18:43:21"). Interpreta como UTC para cálculo correto no fuso do usuário. */
function dataCriacaoUtcToDeadlineMs(dataCriacao: string): number {
  const iso = /Z$|[-+]\d{2}:?\d{2}$/.test(dataCriacao)
    ? dataCriacao
    : `${dataCriacao.replace(/Z$/i, '')}Z`;
  return new Date(iso).getTime() + PRAZO_EDICAO_MS;
}

function formatTempoRestante(ms: number): string {
  if (ms <= 0) return 'Prazo expirado';
  const min = Math.floor(ms / 60000);
  const seg = Math.floor((ms % 60000) / 1000);
  return `${min} min ${seg}s restantes`;
}

/** Contador em tempo real: 59 min 59s → 0. Atualiza a cada 1s. dataCriacao em UTC. */
function CountdownTimer({
  dataCriacao,
  onTick,
}: {
  dataCriacao: string;
  onTick?: (ms: number) => void;
}) {
  const deadlineMsRef = useRef(0);
  const [restante, setRestante] = useState(0);

  const onTickRef = useRef(onTick);
  useEffect(() => {
    onTickRef.current = onTick;
  }, [onTick]);

  useEffect(() => {
    const deadline = dataCriacaoUtcToDeadlineMs(dataCriacao);
    deadlineMsRef.current = deadline;
    const valorInicial = Math.min(
      MAX_EXIBICAO_MS,
      Math.max(0, deadline - Date.now()),
    );
    setRestante(valorInicial);
    onTickRef.current?.(valorInicial);

    const id = setInterval(() => {
      const valor = Math.min(
        MAX_EXIBICAO_MS,
        Math.max(0, deadlineMsRef.current - Date.now()),
      );
      setRestante(valor);
      onTickRef.current?.(valor);
    }, 1000);

    const onVisibility = () => {
      if (document.visibilityState === 'visible') {
        const v = Math.min(
          MAX_EXIBICAO_MS,
          Math.max(0, deadlineMsRef.current - Date.now()),
        );
        setRestante(v);
        onTickRef.current?.(v);
      }
    };
    document.addEventListener('visibilitychange', onVisibility);

    return () => {
      clearInterval(id);
      document.removeEventListener('visibilitychange', onVisibility);
    };
  }, [dataCriacao]);

  return <span aria-live="polite">{formatTempoRestante(restante)}</span>;
}

function truncate(str: string, max: number): string {
  if (!str) return '';
  return str.length <= max ? str : `${str.slice(0, max)}...`;
}

function formatDataExibicao(isoDate: string): string {
  try {
    const d = new Date(isoDate);
    if (Number.isNaN(d.getTime())) return isoDate;
    return format(d, "d 'de' MMMM 'de' yyyy", { locale: ptBR });
  } catch {
    return isoDate;
  }
}

function filtrarFeedbacks(
  lista: Feedback360Item[],
  filtroData: string,
  filtroSentimento: string,
  filtroRelacionamento: string,
  filtroColaborador: string,
): Feedback360Item[] {
  const colaboradorLower = filtroColaborador.trim().toLowerCase();
  return lista.filter((fb) => {
    if (filtroData) {
      const fbData = fb.dataInteracao?.slice(0, 10) ?? '';
      if (fbData !== filtroData) return false;
    }
    if (filtroSentimento && fb.avaliacaoDescricao !== filtroSentimento) return false;
    if (filtroRelacionamento && fb.relacionamentoDescricao !== filtroRelacionamento) return false;
    if (colaboradorLower) {
      const nomeOutro = (fb.nomeDestinatario ?? fb.nomeRemetente ?? '').toLowerCase();
      if (!nomeOutro.includes(colaboradorLower)) return false;
    }
    return true;
  });
}

export default function Feedback360() {
  const [tempoRestanteEdicaoMs, setTempoRestanteEdicaoMs] = useState(0);
  const [filtroColabPopoverOpen, setFiltroColabPopoverOpen] = useState(false);

  const {
    activeTab,
    setActiveTab,
    feedbacksEnviados,
    feedbacksRecebidos,
    loadingList,
    filtroData,
    setFiltroData,
    filtroSentimento,
    setFiltroSentimento,
    filtroRelacionamento,
    setFiltroRelacionamento,
    filtroColaborador,
    setFiltroColaborador,
    detalheItem,
    setDetalheItem,
    colaboradores,
    loadingColab,
    hasMoreColab,
    buscaColab,
    setBuscaColab,
    carregarColaboradores,
    colabPopoverOpen,
    setColabPopoverOpen,
    selectedCodigo,
    setSelectedCodigo,
    displayColab,
    contexto,
    setContexto,
    dataInteracao,
    setDataInteracao,
    feedback360AvaliacaoId,
    setFeedback360AvaliacaoId,
    feedback360RelacionamentoId,
    setFeedback360RelacionamentoId,
    comentario,
    setComentario,
    submitting,
    errors,
    setErrors,
    handleRegistrar,
    avaliacoes,
    relacionamentos,
    loadingOpcoes,
    itemEmEdicao,
    iniciarEdicao,
    cancelarEdicao,
    handleSalvarAlteracoes,
  } = useFeedback360();

  const formValidCreate =
    !!selectedCodigo?.trim() &&
    !!contexto?.trim() &&
    !!dataInteracao &&
    feedback360AvaliacaoId != null &&
    feedback360RelacionamentoId != null &&
    !!comentario?.trim();
  const formValidEdit =
    !!contexto?.trim() &&
    !!dataInteracao &&
    feedback360AvaliacaoId != null &&
    feedback360RelacionamentoId != null &&
    !!comentario?.trim() &&
    tempoRestanteEdicaoMs > 0;

  const listFiltradaEnviados = filtrarFeedbacks(
    feedbacksEnviados,
    filtroData,
    filtroSentimento,
    filtroRelacionamento,
    filtroColaborador,
  );
  const listFiltradaRecebidos = filtrarFeedbacks(
    feedbacksRecebidos,
    filtroData,
    filtroSentimento,
    filtroRelacionamento,
    filtroColaborador,
  );

  const sentimentosUnicos = Array.from(
    new Set([
      ...feedbacksEnviados.map((fb) => fb.avaliacaoDescricao).filter(Boolean),
      ...feedbacksRecebidos.map((fb) => fb.avaliacaoDescricao).filter(Boolean),
    ]),
  ).sort();

  const relacionamentosUnicos = Array.from(
    new Set([
      ...feedbacksEnviados.map((fb) => fb.relacionamentoDescricao).filter(Boolean),
      ...feedbacksRecebidos.map((fb) => fb.relacionamentoDescricao).filter(Boolean),
    ]),
  ).sort();

  const debounceBuscaRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const timeoutAbrirFormRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const timeoutAbrirFiltroRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const lastLoadMoreAtRef = useRef(0);

  // Ao abrir o popover: apenas limpar o campo de busca (a carga inicial é feita no efeito abaixo)
  useEffect(() => {
    if (!colabPopoverOpen) {
      if (timeoutAbrirFormRef.current) {
        clearTimeout(timeoutAbrirFormRef.current);
        timeoutAbrirFormRef.current = null;
      }
      return;
    }
    if (timeoutAbrirFormRef.current) clearTimeout(timeoutAbrirFormRef.current);
    timeoutAbrirFormRef.current = setTimeout(() => {
      timeoutAbrirFormRef.current = null;
      setBuscaColab('');
    }, DEBOUNCE_ABRIR_POPOVER_MS);
    return () => {
      if (timeoutAbrirFormRef.current) clearTimeout(timeoutAbrirFormRef.current);
    };
  }, [colabPopoverOpen]);

  useEffect(() => {
    if (!filtroColabPopoverOpen) {
      if (timeoutAbrirFiltroRef.current) {
        clearTimeout(timeoutAbrirFiltroRef.current);
        timeoutAbrirFiltroRef.current = null;
      }
      return;
    }
    if (timeoutAbrirFiltroRef.current) clearTimeout(timeoutAbrirFiltroRef.current);
    timeoutAbrirFiltroRef.current = setTimeout(() => {
      timeoutAbrirFiltroRef.current = null;
      setBuscaColab('');
    }, DEBOUNCE_ABRIR_POPOVER_MS);
    return () => {
      if (timeoutAbrirFiltroRef.current) clearTimeout(timeoutAbrirFiltroRef.current);
    };
  }, [filtroColabPopoverOpen]);

  // Carga inicial (cursor 0, limite 100) ao abrir; ao digitar 2+ chars busca por nomeOuEmail; ao limpar, volta aos 100
  useEffect(() => {
    if (!colabPopoverOpen && !filtroColabPopoverOpen) return;
    if (debounceBuscaRef.current) clearTimeout(debounceBuscaRef.current);
    const termo = buscaColab.length >= MIN_CHARS_BUSCA_COLAB ? buscaColab : '';
    debounceBuscaRef.current = setTimeout(() => {
      carregarColaboradores(termo, 0);
      debounceBuscaRef.current = null;
    }, DEBOUNCE_BUSCA_COLAB_MS);
    return () => {
      if (debounceBuscaRef.current) clearTimeout(debounceBuscaRef.current);
    };
  }, [buscaColab, colabPopoverOpen, filtroColabPopoverOpen, carregarColaboradores]);

  const handleColabListScroll = useCallback(
    (e: React.UIEvent<HTMLDivElement>) => {
      const el = e.currentTarget;
      const nearBottom = el.scrollHeight - el.scrollTop - el.clientHeight < 100;
      if (!nearBottom || !hasMoreColab || loadingColab) return;
      const now = Date.now();
      if (now - lastLoadMoreAtRef.current < THROTTLE_SCROLL_LOAD_MS) return;
      lastLoadMoreAtRef.current = now;
      carregarColaboradores(buscaColab, colaboradores.length);
    },
    [hasMoreColab, loadingColab, buscaColab, colaboradores.length, carregarColaboradores],
  );

  // Evita que o valor do item (nome + email + UUID) seja enviado como busca ao navegar com teclado ou ao focar
  const handleBuscaColabChange = useCallback(
    (value: string) => {
      const v = value.trim();
      const contemUuid = UUID_REGEX.test(v);
      const isValorDeItem =
        contemUuid ||
        colaboradores.some((c) => c.cpf === v) ||
        colaboradores.some(
          (c) =>
            v ===
            `${(c.nome ?? '').trim()} ${(c.email ?? '').trim()} ${(c.cpf ?? '').trim()}`.replace(/\s+/g, ' ').trim(),
        );
      setBuscaColab(isValorDeItem ? '' : value);
    },
    [colaboradores, setBuscaColab],
  );

  return (
    <div className="space-y-6">
      <PageBreadcrumb
        items={[
          { label: 'Início', href: '/dashboard' },
          { label: 'Reconhecimento' },
        ]}
      />
      <PageHeader
        title="Compartilhe seu Reconhecimento"
        description="Inserir e consultar Reconhecimentos."
      />

      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <TabsList variant="primary" className="w-auto">
          <TabsTrigger variant="primary" value="inserir">
            Reconhecer
          </TabsTrigger>
          <TabsTrigger variant="primary" value="consultar">
            Consultar Reconhecimentos
          </TabsTrigger>
        </TabsList>

        <TabsContent value="inserir" className="mt-6">
          {itemEmEdicao && (
            <Card className="rounded-lg border border-borderSoft bg-primary/10 mb-6">
              <CardContent className="p-4 flex flex-wrap items-center justify-between gap-3">
                <div className="flex items-center gap-3">
                  <Clock className="h-5 w-5 text-primary shrink-0" aria-hidden />
                  <div>
                    <p className="font-semibold text-foreground">Editando Reconhecimento</p>
                    <p className="text-sm text-muted-foreground">
                      Você tem 1 hora após o envio para editar este Reconhecimento.
                    </p>
                    <p className="text-sm font-medium text-foreground mt-0.5">
                      <CountdownTimer
                        dataCriacao={itemEmEdicao.dataCriacao}
                        onTick={setTempoRestanteEdicaoMs}
                      />
                    </p>
                  </div>
                </div>
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="rounded-full text-destructive hover:text-destructive hover:bg-destructive/10"
                  onClick={cancelarEdicao}
                  aria-label="Cancelar edição"
                >
                  <span className="mr-1" aria-hidden>×</span>
                  Cancelar
                </Button>
              </CardContent>
            </Card>
          )}
          <Card className="rounded-lg border border-borderSoft bg-surfaceElevated">
            <CardContent className="p-6 space-y-6">
              <p className="text-sm text-muted-foreground">
                {itemEmEdicao
                  ? 'Altere os campos abaixo e toque em "Salvar alterações".'
                  : 'Escolha para quem deseja oferecer Reconhecimento.'}
              </p>
              <div className="space-y-2">
                <Label htmlFor="colab-search">Profissional</Label>
                {itemEmEdicao ? (
                  <Input
                    id="colab-search"
                    readOnly
                    value={displayColab}
                    className="rounded-lg border-border bg-muted/50"
                    aria-readonly="true"
                  />
                ) : (
                  <Popover
                    open={colabPopoverOpen}
                    onOpenChange={(open) => {
                      setColabPopoverOpen(open);
                      if (!open) {
                        setTimeout(() => setBuscaColab(''), 150);
                      }
                    }}
                  >
                    <PopoverTrigger asChild>
                      <Button
                        id="colab-search"
                        variant="outline"
                        role="combobox"
                        aria-expanded={colabPopoverOpen}
                        className="w-full justify-between font-normal h-10 rounded-lg border border-border focus-visible:ring-2 focus-visible:ring-primary"
                      >
                        <span className={cn(!displayColab && 'text-muted-foreground')}>
                          {displayColab || 'Ou digite o nome de um colega'}
                        </span>
                        <Search className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent
                      className="w-[var(--radix-popover-trigger-width)] max-h-[70vh] flex flex-col overflow-hidden p-0 rounded-lg"
                      align="start"
                      side="bottom"
                      sideOffset={4}
                      avoidCollisions={false}
                    >
                      <Command
                        className="flex flex-col overflow-hidden"
                        shouldFilter={false}
                        filter={() => 1}
                      >
                        <CommandInput
                          placeholder="Buscar profissional..."
                          value={buscaColab}
                          onValueChange={handleBuscaColabChange}
                        />
                        <CommandList
                          className="max-h-[min(60vh,320px)] overflow-y-auto overflow-x-hidden"
                          onScroll={handleColabListScroll}
                        >
                          {loadingColab && colaboradores.length === 0 ? (
                            <div className="py-6 text-center text-sm text-muted-foreground">
                              Carregando profissionais…
                            </div>
                          ) : (
                            <>
                              <CommandEmpty>
                                {buscaColab.length < MIN_CHARS_BUSCA_COLAB
                                  ? 'Digite nome ou e-mail para buscar'
                                  : 'Nenhum profissional encontrado.'}
                              </CommandEmpty>
                              <CommandGroup>
                                {colaboradores.map((c) => (
                                  <CommandItem
                                    key={c.cpf}
                                    value={`${c.nome ?? ''} ${c.email ?? ''} ${c.cpf}`}
                                    onSelect={() => {
                                      setBuscaColab('');
                                      setSelectedCodigo(c.cpf);
                                      setColabPopoverOpen(false);
                                      setErrors((e) => ({ ...e, colaborador: false }));
                                    }}
                                  >
                                    <div className="flex flex-col items-start gap-0.5">
                                      <span className="font-medium">{c.nome ?? c.cpf}</span>
                                      {c.email ? (
                                        <span className="text-xs text-muted-foreground">{c.email}</span>
                                      ) : null}
                                    </div>
                                  </CommandItem>
                                ))}
                              </CommandGroup>
                            </>
                          )}
                        </CommandList>
                      </Command>
                    </PopoverContent>
                  </Popover>
                )}
                {!itemEmEdicao && errors.colaborador && (
                  <p className="text-xs text-destructive">Selecione um profissional.</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="contexto">Contexto</Label>
                <Input
                  id="contexto"
                  placeholder="Digite o contexto do Reconhecimento"
                  value={contexto}
                  onChange={(e) => {
                    setContexto(e.target.value);
                    setErrors((e) => ({ ...e, contexto: false }));
                  }}
                  className="rounded-lg border-border focus-visible:ring-2 focus-visible:ring-primary"
                  error={errors.contexto}
                />
                {errors.contexto && (
                  <p className="text-xs text-destructive">Preencha o contexto.</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="data">Data da interação</Label>
                <div className="relative">
                  <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    id="data"
                    type="date"
                    value={dataInteracao}
                    max={format(new Date(), 'yyyy-MM-dd')}
                    onChange={(e) => {
                      setDataInteracao(e.target.value);
                      setErrors((e) => ({ ...e, data: false }));
                    }}
                    className="pl-9 rounded-lg border-border focus-visible:ring-2 focus-visible:ring-primary"
                    error={errors.data}
                  />
                </div>
                {errors.data && (
                  <p className="text-xs text-destructive">Informe a data.</p>
                )}
              </div>

              <div className="space-y-2">
                <Label>Avaliação Geral</Label>
                <p className="text-xs text-muted-foreground">Todos os campos obrigatórios.</p>
                {loadingOpcoes ? (
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Spinner size={16} className="text-primary" />
                    Carregando opções…
                  </div>
                ) : (
                  <div className="flex flex-wrap gap-2">
                    {avaliacoes.map((a) => (
                      <Button
                        key={a.id}
                        type="button"
                        variant={feedback360AvaliacaoId === a.id ? 'primary' : 'outline'}
                        size="sm"
                        className="rounded-full h-10 px-4"
                        onClick={() => {
                          setFeedback360AvaliacaoId(a.id);
                          setErrors((e) => ({ ...e, avaliacao: false }));
                        }}
                        aria-pressed={feedback360AvaliacaoId === a.id}
                        aria-label={a.descricao}
                      >
                        {a.descricao}
                      </Button>
                    ))}
                  </div>
                )}
                {errors.avaliacao && (
                  <p className="text-xs text-destructive">Selecione o nível de avaliação.</p>
                )}
              </div>

              <div className="space-y-2">
                <Label>Relacionamento</Label>
                {loadingOpcoes ? (
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Spinner size={16} className="text-primary" />
                    Carregando opções…
                  </div>
                ) : (
                  <div className="flex flex-wrap gap-2">
                    {relacionamentos.map((r) => (
                      <Button
                        key={r.id}
                        type="button"
                        variant={feedback360RelacionamentoId === r.id ? 'primary' : 'outline'}
                        size="sm"
                        className="rounded-full h-10 px-4"
                        onClick={() => {
                          setFeedback360RelacionamentoId(r.id);
                          setErrors((e) => ({ ...e, relacionamento: false }));
                        }}
                        aria-pressed={feedback360RelacionamentoId === r.id}
                        aria-label={r.descricao}
                      >
                        {r.descricao}
                      </Button>
                    ))}
                  </div>
                )}
                {errors.relacionamento && (
                  <p className="text-xs text-destructive">Selecione o tipo de relacionamento.</p>

                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="comentario">
                  Justificativa
                  {/* <span className="text-destructive ml-0.5">*</span> */}
                </Label>
                <Textarea
                  id="comentario"
                  placeholder="Descreva a justificativa do Reconhecimento"
                  value={comentario}
                  onChange={(e) => {
                    setComentario(e.target.value);
                    setErrors((e) => ({ ...e, comentario: false }));
                  }}
                  className={cn(
                    'min-h-[100px] rounded-lg border-border focus-visible:ring-2 focus-visible:ring-primary',
                    errors.comentario && 'border-destructive focus-visible:ring-destructive'
                  )}
                />
                {errors.comentario && (
                  <p className="text-xs text-destructive">Preencha a justificativa.</p>
                )}
              </div>

              <div className="flex justify-end">
                {itemEmEdicao ? (
                  <Button
                    onClick={handleSalvarAlteracoes}
                    disabled={submitting || !formValidEdit}
                    className="rounded-full bg-primary text-primary-foreground hover:bg-primary/90"
                    aria-label="Salvar alterações do Reconhecimento"
                  >
                    <Check className="mr-2 h-4 w-4" />
                    Salvar alterações
                  </Button>
                ) : (
                  <Button
                    onClick={handleRegistrar}
                    disabled={submitting || !formValidCreate}
                    className="rounded-full bg-primary text-primary-foreground hover:bg-primary/90"
                    aria-label="Enviar Reconhecimento"
                  >
                    <Send className="mr-2 h-4 w-4" />
                    Enviar Reconhecimento
                  </Button>
                )}
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="consultar" className="mt-6">
          <div className="space-y-4">
            <div className="flex flex-wrap gap-x-4 gap-y-2 items-center">
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium text-muted-foreground">Data:</span>
                <Input
                  type="date"
                  value={filtroData}
                  onChange={(e) => setFiltroData(e.target.value)}
                  className="w-auto max-w-[180px] h-9 rounded-lg"
                />
              </div>
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium text-muted-foreground">Sentimento:</span>
                <Select
                  value={filtroSentimento || '__todos__'}
                  onValueChange={(v) => setFiltroSentimento(v === '__todos__' ? '' : v)}
                >
                  <SelectTrigger className="h-9 w-[180px] rounded-lg border border-border bg-background">
                    <SelectValue placeholder="Todos" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__todos__">Todos</SelectItem>
                    {sentimentosUnicos.map((s) => (
                      <SelectItem key={s} value={s}>
                        {s}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium text-muted-foreground">Relacionamento:</span>
                <Select
                  value={filtroRelacionamento || '__todos__'}
                  onValueChange={(v) => setFiltroRelacionamento(v === '__todos__' ? '' : v)}
                >
                  <SelectTrigger className="h-9 w-[180px] rounded-lg border border-border bg-background">
                    <SelectValue placeholder="Todos" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__todos__">Todos</SelectItem>
                    {relacionamentosUnicos.map((r) => (
                      <SelectItem key={r} value={r}>
                        {r}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="flex items-center gap-2">
                <span className="text-sm font-medium text-muted-foreground">Profissional:</span>
                <Popover
                  open={filtroColabPopoverOpen}
                  onOpenChange={(open) => {
                    setFiltroColabPopoverOpen(open);
                    if (!open) {
                      setTimeout(() => setBuscaColab(''), 150);
                    }
                  }}
                >
                  <PopoverTrigger asChild>
                    <Button
                      type="button"
                      variant="outline"
                      className="h-9 min-w-[200px] max-w-[280px] justify-between font-normal rounded-lg border border-border bg-background"
                    >
                      <span className={cn(!filtroColaborador && 'text-muted-foreground truncate')}>
                        {filtroColaborador || 'Todos'}
                      </span>
                      <Search className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent
                    className="w-[var(--radix-popover-trigger-width)] max-h-[70vh] flex flex-col overflow-hidden p-0 rounded-lg"
                    align="start"
                    side="bottom"
                    sideOffset={4}
                    avoidCollisions={false}
                  >
                    <Command
                      className="flex flex-col overflow-hidden"
                      shouldFilter={false}
                      filter={() => 1}
                    >
                      <CommandInput
                        placeholder="Buscar profissional..."
                        value={buscaColab}
                        onValueChange={handleBuscaColabChange}
                      />
                      <CommandList
                        className="max-h-[min(60vh,320px)] overflow-y-auto overflow-x-hidden"
                        onScroll={handleColabListScroll}
                      >
                        {loadingColab ? (
                          <div className="py-6 text-center text-sm text-muted-foreground">
                            Carregando profissionais…
                          </div>
                        ) : (
                          <>
                            <CommandEmpty>
                              {buscaColab.length < MIN_CHARS_BUSCA_COLAB
                                ? 'Digite nome ou e-mail para buscar'
                                : 'Nenhum profissional encontrado.'}
                            </CommandEmpty>
                            <CommandGroup>
                              <CommandItem
                                value="__todos__"
                                onSelect={() => {
                                  setFiltroColaborador('');
                                  setFiltroColabPopoverOpen(false);
                                }}
                              >
                                Todos
                              </CommandItem>
                              {colaboradores.map((c) => (
                                <CommandItem
                                  key={c.cpf}
                                  value={`${c.nome ?? ''} ${c.email ?? ''} ${c.cpf}`}
                                  onSelect={() => {
                                    setBuscaColab('');
                                    setFiltroColaborador(c.nome ?? c.cpf);
                                    setFiltroColabPopoverOpen(false);
                                  }}
                                >
                                  <div className="flex flex-col items-start gap-0.5">
                                    <span className="font-medium">{c.nome ?? c.cpf}</span>
                                    {c.email ? (
                                      <span className="text-xs text-muted-foreground">{c.email}</span>
                                    ) : null}
                                  </div>
                                </CommandItem>
                              ))}
                            </CommandGroup>
                          </>
                        )}
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
              </div>
              {(filtroData || filtroSentimento || filtroRelacionamento || filtroColaborador) ? (
                <Button
                  type="button"
                  variant="destructive"
                  size="sm"
                  className="h-9 rounded-lg"
                  onClick={() => {
                    setFiltroData('');
                    setFiltroSentimento('');
                    setFiltroRelacionamento('');
                    setFiltroColaborador('');
                    setFiltroColabPopoverOpen(false);
                  }}
                >
                  Limpar filtros
                </Button>
              ) : null}
            </div>

            {loadingList ? (
              <div className="flex items-center gap-2 text-muted-foreground py-8">
                <Spinner size={24} className="text-primary" />
                Carregando Reconhecimentos…
              </div>
            ) : (
              <Tabs defaultValue="enviados" className="w-full">
                <TabsList variant="primary" className="w-auto">
                  <TabsTrigger variant="primary" value="enviados">
                    Reconhecimentos Enviados
                  </TabsTrigger>
                  <TabsTrigger variant="primary" value="recebidos">
                    Reconhecimentos Recebidos
                  </TabsTrigger>
                </TabsList>
                <TabsContent value="enviados" className="mt-4">
                  {listFiltradaEnviados.length === 0 ? (
                    <Card className="rounded-lg border border-borderSoft">
                      <CardContent className="p-8 text-center text-muted-foreground">
                        Nenhum Reconhecimento enviado encontrado.
                      </CardContent>
                    </Card>
                  ) : (
                    <ul className="space-y-4">
                      {listFiltradaEnviados.map((item) => (
                        <li key={item.id}>
                          <Card className="rounded-lg border border-borderSoft bg-surfaceElevated overflow-hidden">
                            <CardContent className="p-4">
                              <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
                                <div>
                                  <h3 className="font-semibold text-foreground">
                                    Reconhecimento para {item.nomeDestinatario ?? '—'}
                                  </h3>
                                  <p className="text-sm text-muted-foreground mt-0.5">
                                    {formatDataExibicao(item.dataInteracao)}
                                  </p>
                                </div>
                                <Badge
                                  variant="secondary"
                                  className="rounded-full shrink-0 bg-primary/10 text-primary border-0"
                                >
                                  {item.avaliacaoDescricao}
                                </Badge>
                              </div>
                              <p className="text-sm text-muted-foreground mt-2">
                                {truncate(item.titulo || item.descricao || '', 120)}
                              </p>
                              <div className="flex flex-wrap gap-2 mt-3">
                                <Badge variant="outline" className="rounded-full text-xs">
                                  {item.relacionamentoDescricao}
                                </Badge>
                              </div>
                              <div className="flex justify-end gap-2 mt-3">
                                {item.editavel === true && (
                                  <Button
                                    variant="outline"
                                    size="sm"
                                    className="rounded-full text-primary border-primary/30"
                                    onClick={() => iniciarEdicao(item)}
                                    aria-label="Editar Reconhecimento"
                                  >
                                    Editar
                                  </Button>
                                )}
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  className="text-primary"
                                  onClick={() => setDetalheItem(item)}
                                >
                                  <Eye className="mr-1.5 h-4 w-4" />
                                  Ver Detalhes
                                </Button>
                              </div>
                            </CardContent>
                          </Card>
                        </li>
                      ))}
                    </ul>
                  )}
                </TabsContent>
                <TabsContent value="recebidos" className="mt-4">
                  {listFiltradaRecebidos.length === 0 ? (
                    <Card className="rounded-lg border border-borderSoft">
                      <CardContent className="p-8 text-center text-muted-foreground">
                        Nenhum Reconhecimento recebido encontrado.
                      </CardContent>
                    </Card>
                  ) : (
                    <ul className="space-y-4">
                      {listFiltradaRecebidos.map((item) => (
                        <li key={item.id}>
                          <Card className="rounded-lg border border-borderSoft bg-surfaceElevated overflow-hidden">
                            <CardContent className="p-4">
                              <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2">
                                <div>
                                  <h3 className="font-semibold text-foreground">
                                    Reconhecimento de {item.nomeRemetente ?? '—'}
                                  </h3>
                                  <p className="text-sm text-muted-foreground mt-0.5">
                                    {formatDataExibicao(item.dataInteracao)}
                                  </p>
                                </div>
                                <Badge
                                  variant="secondary"
                                  className="rounded-full shrink-0 bg-primary/10 text-primary border-0"
                                >
                                  {item.avaliacaoDescricao}
                                </Badge>
                              </div>
                              <p className="text-sm text-muted-foreground mt-2">
                                {truncate(item.titulo || item.descricao || '', 120)}
                              </p>
                              <div className="flex flex-wrap gap-2 mt-3">
                                <Badge variant="outline" className="rounded-full text-xs">
                                  {item.relacionamentoDescricao}
                                </Badge>
                              </div>
                              <div className="flex justify-end mt-3">
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  className="text-primary"
                                  onClick={() => setDetalheItem(item)}
                                >
                                  <Eye className="mr-1.5 h-4 w-4" />
                                  Ver Detalhes
                                </Button>
                              </div>
                            </CardContent>
                          </Card>
                        </li>
                      ))}
                    </ul>
                  )}
                </TabsContent>
              </Tabs>
            )}
          </div>
        </TabsContent>
      </Tabs>

      <Dialog open={!!detalheItem} onOpenChange={(open) => !open && setDetalheItem(null)}>
        <DialogContent className="sm:max-w-md rounded-lg">
          <DialogHeader>
            <DialogTitle>Detalhes do Reconhecimento</DialogTitle>
            <DialogDescription>
              Informações completas do Reconhecimento registrado.
            </DialogDescription>
          </DialogHeader>
          {detalheItem && (
            <div className="space-y-3 text-sm">
              <p><strong>Para:</strong> {detalheItem.nomeDestinatario ?? '—'}</p>
              <p><strong>De:</strong> {detalheItem.nomeRemetente ?? '—'}</p>
              <p><strong>Data:</strong> {formatDataExibicao(detalheItem.dataInteracao)}</p>
              <p><strong>Avaliação:</strong> {detalheItem.avaliacaoDescricao}</p>
              <p><strong>Relacionamento:</strong> {detalheItem.relacionamentoDescricao}</p>
              <p><strong>Contexto:</strong> {detalheItem.titulo || '—'}</p>
              <p><strong>Comentário:</strong> {detalheItem.descricao || '—'}</p>
              {detalheItem.editavel === true && (
                <div className="pt-2">
                  <Button
                    variant="outline"
                    size="sm"
                    className="rounded-full"
                    onClick={() => {
                      setDetalheItem(null);
                      iniciarEdicao(detalheItem);
                    }}
                    aria-label="Editar este Reconhecimento"
                  >
                    Editar Reconhecimento
                  </Button>
                </div>
              )}
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
