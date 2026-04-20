import { useState, useEffect, useCallback, Fragment, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Label } from '@/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { PageBreadcrumb } from '@presentation/components/common';
import { Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { ArrowLeft, ChevronRight, ChevronDown, ChevronUp, Search, X, AssignmentInd } from '@/components/ui/system-icons';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import type { PessoaCadastradaPorOrg, CandidaturaColaboradorItem } from '@domain/entities/GestaoVagasCandidatos';
import { BuscarPessoasCadastradasPorOrgUseCase } from '@domain/usecases/BuscarPessoasCadastradasPorOrgUseCase';
import { BuscarCandidaturasColaboradorUseCase } from '@domain/usecases/BuscarCandidaturasColaboradorUseCase';
import { ListarStatusCandidaturaRecrutamentoUseCase } from '@domain/usecases/ListarStatusCandidaturaRecrutamentoUseCase';
import { ListarStatusVagaRecrutamentoUseCase } from '@domain/usecases/ListarStatusVagaRecrutamentoUseCase';
import { ListarModelosTrabalhoUseCase } from '@domain/usecases/ListarModelosTrabalhoUseCase';
import type { StatusCandidaturaRecrutamento, StatusVagaRecrutamento } from '@domain/repositories/StatusRecrutamentoRepository';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'
import { toast } from 'sonner';

const NAO_INFORMADO = 'Não informado';
const LIMITE_OPCOES = [5, 10, 20, 50, 100] as const;

function rowKey(index: number, item: PessoaCadastradaPorOrg): string {
  return `${index}-${item.emailUsuario ?? ''}-${item.nome ?? ''}`;
}

/** Primeiro nível: apenas Nome, Nome cadastrante, Data do cadastro, E-mail (em linha, valores abaixo). */
const ROTULOS_NIVEL1: { key: keyof PessoaCadastradaPorOrg; label: string }[] = [
  { key: 'nome', label: 'Nome' },
  { key: 'nomeCadastrante', label: 'Nome cadastrante' },
  { key: 'dataDoCadastro', label: 'Data do cadastro' },
  { key: 'emailUsuario', label: 'E-mail' },
];

function valorNivel1(item: PessoaCadastradaPorOrg, key: keyof PessoaCadastradaPorOrg): string {
  if (key === 'dataDoCadastro') {
    const v = item.dataDoCadastro;
    return v && v !== '0001-01-01T00:00:00' ? formatDatePtBr(v) : NAO_INFORMADO;
  }
  const v = item[key];
  return v ?? NAO_INFORMADO;
}

/** Segundo nível: colunas na ordem pedida; de-para para status Vaga, Status Candidatura e Modelo trabalho (api GestaoDeAlocados/ListarModelosTrabalho). */
type MapsNivel2 = {
  statusVaga: Map<string, string>;
  statusCandidatura: Map<number, string>;
  modeloTrabalho: Map<string, string>;
};
type ColunaNivel2 = { label: string; getValor: (c: CandidaturaColaboradorItem, maps: MapsNivel2) => string };

function getColunasNivel2(): ColunaNivel2[] {
  return [
    { label: 'Cód. Nome Vaga', getValor: (c, _maps) => (c.codigo != null && c.titulo ? `${c.codigo} ${c.titulo}` : c.titulo ?? NAO_INFORMADO) },
    {
      label: 'Status Vaga',
      getValor: (c, maps) => maps.statusVaga.get(c.statusVaga ?? '') ?? c.statusVaga ?? NAO_INFORMADO,
    },
    { label: 'Data candidatura', getValor: (c, _maps) => (c.dataCandidatura ? formatDatePtBr(c.dataCandidatura) : NAO_INFORMADO) },
    {
      label: 'Status candidatura',
      getValor: (c, maps) => {
        const id = c.statusCandidaturaId != null ? Number(c.statusCandidaturaId) : null;
        if (id != null && maps.statusCandidatura.has(id)) return maps.statusCandidatura.get(id)!;
        return c.statusCandidaturaDescricao ?? NAO_INFORMADO;
      },
    },
    { label: 'Pretensão salarial', getValor: (c, _maps) => c.pretensaoSalarial ?? NAO_INFORMADO },
    {
      label: 'Modelo trabalho',
      getValor: (c, maps) => maps.modeloTrabalho.get(c.modeloTrabalhoId ?? '') ?? NAO_INFORMADO,
    },
    { label: 'Disponibilidade entrevista', getValor: (c, _maps) => c.disponibilidadeEntrevistaId ?? NAO_INFORMADO },
    { label: 'Dias presencial', getValor: (c, _maps) => (c.quantidadeDiasPresencial != null ? String(c.quantidadeDiasPresencial) : NAO_INFORMADO) },
  ];
}

const COLUNAS_NIVEL2 = getColunasNivel2();

export interface FiltrosAplicados {
  busca: string;
}

const FILTROS_VAZIOS: FiltrosAplicados = { busca: '' };

function temFiltroPreenchido(f: FiltrosAplicados): boolean {
  return f.busca.trim() !== '';
}

export default function CandidaturasFMU() {
  const navigate = useNavigate();
  const token = useAppSelector((s) => s.auth.token);

  const [busca, setBusca] = useState('');
  const [filtrosAplicados, setFiltrosAplicados] = useState<FiltrosAplicados>(FILTROS_VAZIOS);

  const [limite, setLimite] = useState(20);
  const [cursor, setCursor] = useState(0);
  const [loading, setLoading] = useState(false);
  const [lista, setLista] = useState<PessoaCadastradaPorOrg[]>([]);
  const [expandedKey, setExpandedKey] = useState<string | null>(null);
  const [candidaturasCache, setCandidaturasCache] = useState<Record<string, { loading: boolean; data: CandidaturaColaboradorItem[] }>>({});

  const [statusCandidaturaList, setStatusCandidaturaList] = useState<StatusCandidaturaRecrutamento[]>([]);
  const [statusVagaList, setStatusVagaList] = useState<StatusVagaRecrutamento[]>([]);
  const [modelosTrabalhoList, setModelosTrabalhoList] = useState<{ id: string; descricao: string }[]>([]);
  const [modalEmBreveOpen, setModalEmBreveOpen] = useState(false);

  const buscarPessoasUseCase = container.resolve(BuscarPessoasCadastradasPorOrgUseCase);
  const buscarCandidaturasUseCase = container.resolve(BuscarCandidaturasColaboradorUseCase);
  const listarStatusCandidaturaUseCase = container.resolve(ListarStatusCandidaturaRecrutamentoUseCase);
  const listarStatusVagaUseCase = container.resolve(ListarStatusVagaRecrutamentoUseCase);
  const listarModelosTrabalhoUseCase = container.resolve(ListarModelosTrabalhoUseCase);

  const statusVagaPorCodigo = useMemo(
    () => new Map(statusVagaList.map((s) => [s.codigo, s.descricao])),
    [statusVagaList]
  );
  const statusCandidaturaPorId = useMemo(
    () => new Map(statusCandidaturaList.map((s) => [s.id, s.descricao])),
    [statusCandidaturaList]
  );
  const modeloTrabalhoPorId = useMemo(
    () => new Map(modelosTrabalhoList.map((m) => [m.id, m.descricao])),
    [modelosTrabalhoList]
  );
  const mapsNivel2 = useMemo(
    () => ({
      statusVaga: statusVagaPorCodigo,
      statusCandidatura: statusCandidaturaPorId,
      modeloTrabalho: modeloTrabalhoPorId,
    }),
    [statusVagaPorCodigo, statusCandidaturaPorId, modeloTrabalhoPorId]
  );

  useEffect(() => {
    if (!token) return;
    Promise.all([
      listarStatusCandidaturaUseCase.execute(token),
      listarStatusVagaUseCase.execute(token),
      listarModelosTrabalhoUseCase.execute(token),
    ])
      .then(([listaCandidatura, listaVaga, listaModelosTrabalho]) => {
        setStatusCandidaturaList(listaCandidatura ?? []);
        setStatusVagaList(listaVaga ?? []);
        setModelosTrabalhoList((listaModelosTrabalho ?? []).map((m) => ({ id: m.id, descricao: m.descricao ?? '' })));
      })
      .catch(() => {
        toast.error('Erro ao carregar listas de apoio (status e modelos de trabalho).');
      });
  }, [token, listarStatusCandidaturaUseCase, listarStatusVagaUseCase, listarModelosTrabalhoUseCase]);

  const buscar = useCallback(
    async (filtros: FiltrosAplicados) => {
      if (!token) {
        toast.error('Sessão inválida. Faça login novamente.');
        return;
      }
      setLoading(true);
      try {
        const dados = await buscarPessoasUseCase.execute(token, {
          busca: filtros.busca.trim(),
          cursor,
          limite,
        });
        setLista(dados);
      } catch (e) {
        toast.error('Erro ao carregar pessoas cadastradas.');
        setLista([]);
      } finally {
        setLoading(false);
      }
    },
    [buscarPessoasUseCase, token, cursor, limite]
  );

  useEffect(() => {
    buscar(filtrosAplicados);
  }, [cursor, limite, filtrosAplicados.busca, buscar]);

  const aplicarFiltros = useCallback(() => {
    setFiltrosAplicados({ busca: busca.trim() });
    setCursor(0);
  }, [busca]);

  const limparFiltros = useCallback(() => {
    setBusca('');
    setFiltrosAplicados(FILTROS_VAZIOS);
    setCursor(0);
  }, []);

  const filtrosPreenchidos = temFiltroPreenchido({ busca });
  const temFiltroAplicado = temFiltroPreenchido(filtrosAplicados);

  const carregarCandidaturas = useCallback(
    async (codColaborador: string) => {
      if (!token || !codColaborador) return;
      const cacheKey = codColaborador;
      setCandidaturasCache((prev) => {
        if (prev[cacheKey]) return prev;
        return { ...prev, [cacheKey]: { loading: true, data: [] } };
      });
      try {
        const data = await buscarCandidaturasUseCase.execute(token, codColaborador);
        setCandidaturasCache((prev) => ({ ...prev, [cacheKey]: { loading: false, data } }));
      } catch {
        toast.error('Erro ao carregar candidaturas do colaborador.');
        setCandidaturasCache((prev) => ({ ...prev, [cacheKey]: { loading: false, data: [] } }));
      }
    },
    [token, buscarCandidaturasUseCase]
  );

  const toggleExpand = useCallback(
    (key: string, item: PessoaCadastradaPorOrg) => {
      setExpandedKey((prev) => (prev === key ? null : key));
      const cod = item.codigoInternoColaborador?.trim();
      if (cod && lista.length > 0) {
        carregarCandidaturas(cod);
      }
    },
    [carregarCandidaturas, lista.length]
  );

  const total = lista.length;
  const inicio = total === 0 ? 0 : cursor + 1;
  const fim = cursor + total;
  const textoPaginacao = total === 0 ? '0 - 0 de 0' : `${inicio} - ${fim} de ${total}`;
  const podeAnterior = cursor > 0;
  const podeProximo = lista.length === limite;

  return (
    <div className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento', href: '/recrutamento/dashboard' },
          { label: 'Candidaturas da FMU' },
        ]}
      />

      <div className="flex items-center gap-3">
        <Button
          variant="ghost"
          size="icon"
          aria-label="Voltar"
          onClick={() => navigate('/recrutamento/dashboard')}
        >
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <h1 className="text-xl font-semibold">Candidaturas da FMU</h1>
      </div>

      <Card>
        <CardContent className="p-6 space-y-4">
          <div>
            <h2 className="text-lg font-medium">Candidaturas da FMU</h2>
            <p className="text-sm text-muted-foreground">
              Lista de pessoas cadastradas na organização e suas candidaturas em vagas.
            </p>
          </div>

          <div className="rounded-lg border border-border bg-muted/20 p-4 space-y-4">
            <p className="text-sm font-medium text-muted-foreground">
              Lista de pessoas cadastradas na organização e suas Candidaturas
            </p>
            <div className="flex flex-wrap items-end gap-3">
              <div className="space-y-1.5">
                <Label htmlFor="candidaturas-fmu-busca" className="text-xs text-muted-foreground">
                  Busca
                </Label>
                <Input
                  id="candidaturas-fmu-busca"
                  placeholder="Busca"
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && filtrosPreenchidos && aplicarFiltros()}
                  className="w-[200px]"
                />
              </div>
              <Button
                variant="secondary"
                size="icon"
                onClick={aplicarFiltros}
                disabled={!filtrosPreenchidos}
                aria-label="Buscar"
                title={filtrosPreenchidos ? 'Aplicar filtros' : 'Preencha o campo busca para buscar'}
              >
                <Search className="h-4 w-4" />
              </Button>
              {temFiltroAplicado && (
                <Button variant="outline" size="sm" onClick={limparFiltros} className="gap-1.5">
                  <X className="h-4 w-4" />
                  Limpar filtro
                </Button>
              )}
            </div>
          </div>

          <div className="space-y-4">
            {loading ? (
              <div className="rounded-lg border border-border py-10 text-center text-sm text-muted-foreground">
                Carregando...
              </div>
            ) : lista.length === 0 ? (
              <div className="rounded-lg border border-border py-10 text-center text-sm text-muted-foreground">
                Nenhum registro encontrado. A busca foi concluída e não há dados para exibir.
              </div>
            ) : (
              lista.map((item, index) => {
                const key = rowKey(index, item);
                const isExpanded = expandedKey === key;
                const codColaborador = item.codigoInternoColaborador?.trim() ?? null;
                const cache = codColaborador ? candidaturasCache[codColaborador] : null;
                const candidaturasParaExibir = cache?.data ?? [];
                const loadingCandidaturas = !!cache?.loading;
                const temCandidaturas = (cache?.data?.length ?? 0) > 0;

                return (
                  <Fragment key={key}>
                    <div
                      role="button"
                      tabIndex={0}
                      onClick={() => toggleExpand(key, item)}
                      onKeyDown={(e) => e.key === 'Enter' && toggleExpand(key, item)}
                      className="rounded-lg border border-border bg-card overflow-hidden cursor-pointer hover:bg-muted/40 transition-colors"
                    >
                      {/* Linha de rótulos */}
                      <div className="grid grid-cols-4 gap-x-6 px-5 py-3 bg-muted/50 border-b border-border text-sm font-medium text-muted-foreground">
                        {ROTULOS_NIVEL1.map(({ key: k, label }) => (
                          <div key={k} className="min-w-0">
                            {label}
                          </div>
                        ))}
                      </div>
                      {/* Linha de valores + chevron */}
                      <div className="flex items-center justify-between gap-4 px-5 py-4">
                        <div className="grid grid-cols-4 gap-x-6 min-w-0 flex-1 text-sm">
                          {ROTULOS_NIVEL1.map(({ key: k }) => (
                            <div key={k} className="min-w-0 break-words flex items-center gap-2">
                              {k === 'nome' ? (
                                <>
                                  <span className="min-w-0">{valorNivel1(item, k)}</span>
                                  <Tooltip>
                                    <TooltipTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="icon"
                                        className="h-8 w-8 shrink-0 rounded-full"
                                        aria-label="Em breve"
                                        onClick={(e) => {
                                          e.stopPropagation();
                                          setModalEmBreveOpen(true);
                                        }}
                                      >
                                        <AssignmentInd className="h-4 w-4" />
                                      </Button>
                                    </TooltipTrigger>
                                    <TooltipContent>Em Breve</TooltipContent>
                                  </Tooltip>
                                </>
                              ) : (
                                valorNivel1(item, k)
                              )}
                            </div>
                          ))}
                        </div>
                        <span className="shrink-0 text-muted-foreground" aria-hidden>
                          {isExpanded ? (
                            <ChevronUp className="h-5 w-5" />
                          ) : (
                            <ChevronDown className="h-5 w-5" />
                          )}
                        </span>
                      </div>
                    </div>

                    {isExpanded && (
                      <div className="rounded-lg border border-border border-l-4 border-l-primary/50 bg-muted/20 overflow-hidden ml-2">
                        <div className="p-4 space-y-4">
                          {!codColaborador ? (
                            <p className="py-2 text-sm text-muted-foreground">
                              Código do colaborador não disponível para consultar candidaturas.
                            </p>
                          ) : loadingCandidaturas ? (
                            <p className="py-2 text-sm text-muted-foreground">Carregando candidaturas...</p>
                          ) : !temCandidaturas ? (
                            <p className="py-2 text-sm text-muted-foreground">Nenhuma candidatura encontrada.</p>
                          ) : (
                            <div className="rounded-lg border border-border overflow-x-auto">
                              {/* Cabeçalho do subnível (rótulos em linha) */}
                              <div className="grid grid-cols-8 gap-x-4 gap-y-0 min-w-[640px] px-4 py-3 bg-muted/60 border-b border-border text-xs font-medium text-muted-foreground">
                                {COLUNAS_NIVEL2.map((col, i) => (
                                  <span key={i} className="min-w-0 truncate">{col.label}</span>
                                ))}
                              </div>
                              {/* Uma linha de valores por candidatura */}
                              {candidaturasParaExibir.map((c) => (
                                <div
                                  key={c.candidaturaId}
                                  className="grid grid-cols-8 gap-x-4 min-w-[640px] px-4 py-3 bg-background border-b border-border last:border-b-0 text-sm"
                                >
                                  {COLUNAS_NIVEL2.map((col, i) => (
                                    <div
                                      key={i}
                                      className={`min-w-0 break-words ${col.label === 'Status candidatura' ? 'text-green-600 dark:text-green-400' : ''}`}
                                    >
                                      {col.getValor(c, mapsNivel2)}
                                    </div>
                                  ))}
                                </div>
                              ))}
                            </div>
                          )}
                        </div>
                      </div>
                    )}
                  </Fragment>
                );
              })
            )}
          </div>

          <div className="flex flex-wrap items-center justify-end gap-4 pt-2">
            <div className="flex items-center gap-2">
              <span className="text-sm text-muted-foreground">Linhas por página:</span>
              <Select
                value={String(limite)}
                onValueChange={(v) => {
                  setLimite(Number(v));
                  setCursor(0);
                }}
              >
                <SelectTrigger className="h-8 w-[70px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {LIMITE_OPCOES.map((n) => (
                    <SelectItem key={n} value={String(n)}>
                      {n}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <span className="text-sm text-muted-foreground">{textoPaginacao}</span>
            <div className="flex gap-1">
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8"
                disabled={!podeAnterior}
                onClick={() => setCursor((c) => Math.max(0, c - limite))}
                aria-label="Página anterior"
              >
                <ArrowLeft className="h-4 w-4" />
              </Button>
              <Button
                variant="outline"
                size="icon"
                className="h-8 w-8"
                disabled={!podeProximo}
                onClick={() => setCursor((c) => c + limite)}
                aria-label="Próxima página"
              >
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      <Dialog open={modalEmBreveOpen} onOpenChange={setModalEmBreveOpen}>
        <DialogContent className="sm:max-w-md rounded-lg">
          <DialogHeader>
            <DialogTitle>Dados do candidato</DialogTitle>
            <DialogDescription>Em Breve</DialogDescription>
          </DialogHeader>
          <p className="text-sm text-muted-foreground">Em Breve</p>
        </DialogContent>
      </Dialog>
    </div>
  );
}
