import { useEffect, useMemo, useRef, useState } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { UnidadeItem, TipoVagaItem, TipoContratacaoItem } from '@domain/entities/GestaoVagasCandidatos';
import type { StatusVaga, Vaga } from '@domain/entities/Vaga';
import { ListarStatusVagasUseCase } from '@domain/usecases/ListarStatusVagasUseCase';
import { ListarVagasRecrutamentoEPerfisUseCase } from '@domain/usecases/ListarVagasRecrutamentoEPerfisUseCase';
import { ListarUnidadesVagaUseCase } from '@domain/usecases/ListarUnidadesVagaUseCase';
import { ListarTiposVagaUseCase } from '@domain/usecases/ListarTiposVagaUseCase';
import { ListarTiposContratacaoVagaUseCase } from '@domain/usecases/ListarTiposContratacaoVagaUseCase';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import { toast } from 'sonner';
import {
  type UnknownRecord,
  formatCurrencyBRL,
  formatDatePtBr,
  firstNonEmpty,
  getStatusIdFromVaga,
  normalizeCountMap,
  safeText,
  toCount,
} from './gestaoVagas/gestaoVagasUtils';

type PerfilKanban = UnknownRecord;

const SEARCH_DEBOUNCE_MS = 500;
const DEFAULT_FETCH_LIMIT = 20;

/** Retorna descricao do item cujo id coincide (string ou number). */
function getDescricaoById<T extends { id: string | number; descricao: string }>(
  list: T[],
  id: string | number | null | undefined
): string {
  if (id == null || !Array.isArray(list)) return '';
  const found = list.find((item) => item.id == id || String(item.id) === String(id));
  return found?.descricao ?? '';
}

/** Data fim = hoje + 1 dia; data início = data fim - 365 dias (em YYYY-MM-DD, data local). */
function getDefaultDates(): { dataInicio: string; dataFim: string } {
  const fim = new Date();
  fim.setDate(fim.getDate() + 1);
  const inicio = new Date(fim);
  inicio.setDate(inicio.getDate() - 365);
  const fmt = (d: Date) => {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  };
  return { dataInicio: fmt(inicio), dataFim: fmt(fim) };
}

export const MAX_VAGAS_POR_COLUNA_OPTIONS = [10, 20, 50, 100] as const;

type ColumnVisibilityKey = 'perfis' | string;

export interface PerfilCardModel {
  key: string;
  perfilId: string;
  titulo: string;
  clienteLinha: string;
  gestor: string;
  aberturaEm: string;
  criadoEm: string;
  criador: string;
  modificadoEm: string;
  alteradoPor: string;
  codigoCliente?: string;
  nomeCliente?: string;
  codigoGestor?: string;
  nomeGestor?: string;
}

export interface VagaCardModel {
  key: string;
  titulo: string;
  clienteLinha: string;
  gestor: string;
  aberturaEm: string;
  criador: string;
  recrutador: string;
  aderentesCount: number;
  numeroDeVagas: number;
  /** Total de candidatos (soma de quantidadeCandidatosPorEstagio[].quantidade). */
  totalCandidatos: number;
  /** Candidatos por estágio/status (do retorno da API). */
  quantidadeCandidatosPorEstagio: Array<{ idStatus?: number; descricaoStatus?: string; quantidade?: number }>;
  statusId: string;
  statusNome: string;
  modificadoEm: string;
  alteradoPor: string;
  raw: UnknownRecord;
  vagaId: string | number;
  /** Código numérico da vaga (retorno da API, ex.: codigo: 466). Usado em CandidatarOutraPessoa. */
  codigoVaga?: number;
  /** ID do perfil gerador (para navegação para criar/editar perfil de atuação). */
  idPerfilGerador?: string;
}

export interface VagaInfoItem {
  label: string;
  value: string;
}

export interface VagaInfoModel {
  left: VagaInfoItem[];
  right: VagaInfoItem[];
  descricao: string;
}

export const useGestaoVagas = () => {
  const { token, user } = useAppSelector((state) => state.auth);

  const [vagas, setVagas] = useState<Vaga[]>([]);
  const [statusVagas, setStatusVagas] = useState<StatusVaga[]>([]);
  const [loading, setLoading] = useState(true);
  const [busca, setBusca] = useState('');
  const [dataInicio, setDataInicio] = useState(() => getDefaultDates().dataInicio);
  const [dataFim, setDataFim] = useState(() => getDefaultDates().dataFim);
  const [totalPerfis, setTotalPerfis] = useState(0);
  const [totalVagasPorStatus, setTotalVagasPorStatus] = useState<Record<string, number>>({});
  const [perfis, setPerfis] = useState<PerfilKanban[]>([]);
  const [maxVagasPorColuna, setMaxVagasPorColuna] = useState<number>(20);
  const [columnVisibility, setColumnVisibility] = useState<Record<string, boolean>>({ perfis: false });
  const [limiteVagas, setLimiteVagas] = useState<number>(DEFAULT_FETCH_LIMIT);
  const [visualizarCandidatosPorEstagio, setVisualizarCandidatosPorEstagio] = useState(false);
  const prefsLoadedRef = useRef(false);

  const [vagaInfoOpen, setVagaInfoOpen] = useState(false);
  const [vagaInfo, setVagaInfo] = useState<UnknownRecord | null>(null);
  const [unidadesList, setUnidadesList] = useState<UnidadeItem[]>([]);
  const [tiposVagaList, setTiposVagaList] = useState<TipoVagaItem[]>([]);
  const [tiposContratacaoList, setTiposContratacaoList] = useState<TipoContratacaoItem[]>([]);
  const hasTrackedInitialLoad = useRef(false);
  const lastSearchTrackKeyRef = useRef<string>('');

  const prefsKey = useMemo(() => {
    const userRecord = user as unknown as UnknownRecord;
    const userId = safeText(userRecord?.cpf ?? userRecord?.email ?? userRecord?.id ?? 'anon');
    const orgId = safeText(userRecord?.orgId ?? userRecord?.OrgId ?? '');
    return `gestaoVagas.kanbanPrefs.v1.${orgId}.${userId}`;
  }, [user]);

  useEffect(() => {
    try {
      const raw = localStorage.getItem(prefsKey);
      if (!raw) {
        prefsLoadedRef.current = true;
        return;
      }

      const parsed = JSON.parse(raw) as UnknownRecord;
      const vis = (parsed.visibility ?? {}) as Record<string, unknown>;
      const limit = toCount(parsed.maxVagasPorColuna);
      const limiteVagasSalvo = toCount(parsed.limiteVagas);
      const visualizarCandidatos = parsed.visualizarCandidatosPorEstagio === true;

      setColumnVisibility((prev) => ({
        ...prev,
        ...Object.fromEntries(Object.entries(vis).map(([k, v]) => [k, Boolean(v)])),
        perfis: false,
      }));
      if (limit > 0) setMaxVagasPorColuna(limit);
      if (limiteVagasSalvo > 0) setLimiteVagas(limiteVagasSalvo);
      setVisualizarCandidatosPorEstagio(visualizarCandidatos);
    } catch {
      // ignore
    } finally {
      prefsLoadedRef.current = true;
    }
  }, [prefsKey]);

  useEffect(() => {
    if (!prefsLoadedRef.current) return;
    try {
      localStorage.setItem(
        prefsKey,
        JSON.stringify({
          visibility: columnVisibility,
          maxVagasPorColuna,
          limiteVagas,
          visualizarCandidatosPorEstagio,
        })
      );
    } catch {
      // ignore
    }
  }, [prefsKey, columnVisibility, maxVagasPorColuna, limiteVagas, visualizarCandidatosPorEstagio]);

  const TOAST_LISTAGEM_ID = 'gestao-vagas-listagem';

  const carregarVagas = async (statusParaFiltro?: StatusVaga[]) => {
    if (!token) return;

    const list = statusParaFiltro ?? statusVagas;
    const statusList = list
      .filter((s) => columnVisibility[s.id] !== false)
      .map((s) => String(s.id));

    toast.info('Atualizando a lista, por favor aguarde...', {
      id: TOAST_LISTAGEM_ID,
      className: '!border-info !bg-card !text-info',
    });

    try {
      const useCase = container.resolve<ListarVagasRecrutamentoEPerfisUseCase>(
        DiTokens.listarVagasRecrutamentoEPerfisUseCase
      );

      const defaults = getDefaultDates();
      const dataInicioFormatada = dataInicio || defaults.dataInicio;
      const dataFimFormatada = dataFim || defaults.dataFim;

      const resultado = await useCase.execute(token, {
        cursor: 0,
        limite: limiteVagas,
        dataInicio: dataInicioFormatada,
        dataFim: dataFimFormatada,
        busca,
        statusList,
      });

      const resultRecord = resultado as unknown as UnknownRecord;
      const vagasResult = (resultRecord.vagas as unknown[]) || [];
      const perfisResult = (resultRecord.perfis as unknown[]) || [];

      setVagas(vagasResult as Vaga[]);
      setPerfis(perfisResult as PerfilKanban[]);
      setTotalVagasPorStatus(normalizeCountMap(resultRecord.totalVagas));
      setTotalPerfis(toCount(resultRecord.totalPerfis));

      toast.success('Lista de Perfis e Vagas atualizadas!', { id: TOAST_LISTAGEM_ID });
    } catch (error) {
      console.error('Erro ao carregar vagas:', error);
      const err = error as { mensagem?: string } | Error | null;
      const mensagem =
        err && typeof err === 'object' && err !== null && 'mensagem' in err
          ? String((err as { mensagem?: string }).mensagem)
          : err instanceof Error
            ? err.message
            : null;
      toast.error(mensagem ?? 'Erro ao atualizar lista.', { id: TOAST_LISTAGEM_ID });
    }
  };

  useEffect(() => {
    if (!token) return;

    const carregarDados = async () => {
      setLoading(true);
      try {
        const listarStatusUseCase = container.resolve<ListarStatusVagasUseCase>(
          DiTokens.listarStatusVagasUseCase
        );
        const status = await listarStatusUseCase.execute(token);
        /** Ordem das colunas segue a ordem retornada pela API (ListarStatusVagaRecrutamento), sem reordenar por código. */
        const statusList = (status || []).filter((s) => s.ativo);
        setStatusVagas(statusList);
        await carregarVagas(statusList);
        if (!hasTrackedInitialLoad.current) {
          logUserAction('GestaoVagas', 'CarregarDados', {}, user);
          hasTrackedInitialLoad.current = true;
        }
      } catch (error) {
        console.error('Erro ao carregar dados:', error);
      } finally {
        setLoading(false);
      }
    };

    carregarDados();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token]);

  useEffect(() => {
    if (!token) return;
    const searchKey = JSON.stringify({
      busca,
      dataInicio,
      dataFim,
    });
    const timeoutId = setTimeout(() => {
      if (hasTrackedInitialLoad.current && lastSearchTrackKeyRef.current !== searchKey) {
        logUserAction(
          'GestaoVagas',
          'BuscarVagas',
          {
            busca,
            dataInicio: dataInicio || getDefaultDates().dataInicio,
            dataFim: dataFim || getDefaultDates().dataFim,
          },
          user
        );
        lastSearchTrackKeyRef.current = searchKey;
      }
      carregarVagas();
    }, SEARCH_DEBOUNCE_MS);
    return () => clearTimeout(timeoutId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [busca, dataInicio, dataFim, token, limiteVagas, columnVisibility]);

  useEffect(() => {
    if (statusVagas.length === 0) return;
    setColumnVisibility((prev) => {
      const next = { ...prev };
      for (const s of statusVagas) {
        if (!(s.id in next)) next[s.id] = true;
      }
      next.perfis = false;
      return next;
    });
  }, [statusVagas]);

  const vagasFiltradas = useMemo(() => {
    return vagas.filter((vaga) => {
      const buscaLower = busca.toLowerCase().trim();
      if (!buscaLower) return true;
      return (
        safeText(vaga.titulo).toLowerCase().includes(buscaLower) ||
        safeText(vaga.descricao).toLowerCase().includes(buscaLower) ||
        safeText(vaga.nomeUsuarioCriador).toLowerCase().includes(buscaLower) ||
        safeText(vaga.recrutadorVaga?.nome).toLowerCase().includes(buscaLower) ||
        (vaga.codigo != null && String(vaga.codigo).toLowerCase().includes(buscaLower))
      );
    });
  }, [vagas, busca]);

  const visibleStatusIds = useMemo(() => {
    const ids = statusVagas.map((s) => s.id).filter((id) => columnVisibility[id] !== false);
    return new Set(ids);
  }, [statusVagas, columnVisibility]);

  const statusColumns = useMemo(() => {
    return statusVagas.filter((s) => visibleStatusIds.has(s.id));
  }, [statusVagas, visibleStatusIds]);

  const statusNameById = useMemo(() => {
    return new Map(statusVagas.map((s) => [s.id, s.nome]));
  }, [statusVagas]);

  const vagasPorStatus = useMemo(() => {
    const map: Record<string, Vaga[]> = {};
    for (const status of statusColumns) {
      map[status.id] = [];
    }
    for (const vaga of vagasFiltradas) {
      const statusId = getStatusIdFromVaga(vaga);
      if (!statusId) continue;
      if (!map[statusId]) map[statusId] = [];
      map[statusId].push(vaga);
    }
    const sortByDataUltimaAlteracao = (a: Vaga, b: Vaga) => {
      const da = a.dataUltimaAlteracao || '';
      const db = b.dataUltimaAlteracao || '';
      return db.localeCompare(da);
    };
    for (const statusId of Object.keys(map)) {
      map[statusId].sort(sortByDataUltimaAlteracao);
    }
    return map;
  }, [vagasFiltradas, statusColumns]);

  const totalVagasGeral = useMemo(() => {
    const sumFromMap = Object.values(totalVagasPorStatus).reduce(
      (sum, n) => sum + (Number.isFinite(n) ? n : 0),
      0
    );
    if (sumFromMap > 0) return sumFromMap;
    return vagasFiltradas.length;
  }, [totalVagasPorStatus, vagasFiltradas.length]);

  const perfisFiltrados = useMemo(() => {
    const buscaLower = busca.toLowerCase();
    if (!buscaLower) return perfis;
    return perfis.filter((p) => {
      const record = p as UnknownRecord;
      const fields = [
        record.gestorExternoPerfilNome,
        record.nomeCliente,
        record.nomeGestorExterno,
        record.usuarioAlterador,
        record.nomeColaboradorCriador,
      ];
      return fields.some((f) => safeText(f).toLowerCase().includes(buscaLower));
    });
  }, [perfis, busca]);

  const perfilCards = useMemo<PerfilCardModel[]>(() => {
    return perfisFiltrados.map((perfil) => {
      const titulo = safeText(perfil.gestorExternoPerfilNome || perfil.nome || perfil.titulo || 'Perfil');
      const nomeCliente = safeText(perfil.nomeCliente);
      const unidadeCliente = safeText(
        perfil.unidadeCliente ||
          perfil.nomeUnidade ||
          perfil.unidade ||
          perfil.nomeUnidadeCliente ||
          perfil.unidadeNome
      );
      const clienteLinha = [nomeCliente, unidadeCliente].filter(Boolean).join(' - ');

      const gestor = safeText(perfil.nomeGestorExterno);
      const aberturaEm = safeText(
        perfil.dataAbertura || perfil.aberturaEm || perfil.dataInicio || perfil.dataAberturaPerfil
      );
      const criadoEmRaw = safeText(perfil.dataCriacao);
      const criadoEm = formatDatePtBr(criadoEmRaw);
      const criador = safeText(perfil.nomeColaboradorCriador);

      const modificadoEmRaw = safeText(perfil.ultimaAlteracao || perfil.dataAlteracao || perfil.dataAtualizacao);
      const modificadoEm = modificadoEmRaw ? formatDatePtBr(modificadoEmRaw) : '';
      const alteradoPor = safeText(perfil.nomeColaboradorAlteracao || perfil.usuarioAlterador);

      const perfilId = safeText(perfil.gestorExternoPerfilId ?? perfil.id ?? '');
      const perfilRecord = perfil as UnknownRecord;
      const codigoCliente = safeText(perfilRecord.codigoCliente ?? perfilRecord.codigoClienteOrg ?? '');
      const codigoGestor = safeText(
        perfilRecord.codigoGestorExterno ?? perfilRecord.codGestorExterno ?? perfilRecord.codigoGestor ?? ''
      );
      const nomeGestor = safeText(perfil.nomeGestorExterno ?? perfilRecord.nomeGestor ?? gestor);
      return {
        key: perfilId || `${titulo}-${criadoEmRaw}`,
        perfilId,
        titulo,
        clienteLinha,
        gestor,
        aberturaEm,
        criadoEm,
        criador,
        modificadoEm,
        alteradoPor,
        codigoCliente: codigoCliente || undefined,
        nomeCliente: nomeCliente || undefined,
        codigoGestor: codigoGestor || undefined,
        nomeGestor: nomeGestor || undefined,
      };
    });
  }, [perfisFiltrados]);

  const vagaCardsByStatus = useMemo<Record<string, VagaCardModel[]>>(() => {
    const out: Record<string, VagaCardModel[]> = {};

    for (const [statusId, vagasDaColuna] of Object.entries(vagasPorStatus)) {
      out[statusId] = vagasDaColuna.map((vaga) => {
        const vagaWithRaw = vaga as unknown as UnknownRecord & { _raw?: UnknownRecord };
        const record = vagaWithRaw._raw ?? (vaga as unknown as UnknownRecord);
        const titulo = safeText(record.titulo ?? record.nome ?? vaga.titulo);
        const nomeCliente = safeText(record.nomeCliente);
        const unidadeCliente = safeText(
          record.unidadeCliente ?? record.nomeUnidade ?? record.unidade ?? record.nomeUnidadeCliente
        );
        const clienteLinha = [nomeCliente, unidadeCliente].filter(Boolean).join(' - ');

        const gestor = safeText(record.nomeGestorExterno ?? record.gestorExternoNome ?? record.gestorNome);
        const aberturaEmRaw = safeText(record.dataAbertura ?? record.aberturaEm ?? record.dataInicio ?? record.dataCriacao);
        const aberturaEm = aberturaEmRaw ? formatDatePtBr(aberturaEmRaw) : '';

        const criador = safeText(record.nomeUsuarioCriador ?? record.nomeColaboradorCriador ?? vaga.nomeUsuarioCriador);
        const recrutador = safeText(
          record.nomeRecrutadorVaga ?? record.nomeRecrutador ?? record.recrutador ?? vaga.recrutadorVaga?.nome
        );

        const aderentesCount = toCount(record.totalAderentes ?? record.aderentes ?? vaga.totalAderentes);
        
        // numeroDeVagas vem diretamente do objeto da vaga normalizado
        const numeroDeVagas = toCount(vaga.numeroDeVagas ?? record.numeroDeVagas ?? 0);

        // Total de candidatos e lista por estágio (do retorno da API)
        const quantidadeCandidatosPorEstagioRaw = record.quantidadeCandidatosPorEstagio as Array<{ idStatus?: number; descricaoStatus?: string; quantidade?: number }> | null | undefined;
        const quantidadeCandidatosPorEstagio = Array.isArray(quantidadeCandidatosPorEstagioRaw)
          ? quantidadeCandidatosPorEstagioRaw.map((item) => ({
              idStatus: item?.idStatus,
              descricaoStatus: safeText(item?.descricaoStatus),
              quantidade: toCount(item?.quantidade),
            }))
          : [];
        const totalCandidatos = quantidadeCandidatosPorEstagio.length > 0
          ? quantidadeCandidatosPorEstagio.reduce((acc, item) => acc + item.quantidade, 0)
          : 0;

        const statusVagaId = getStatusIdFromVaga(vaga);
        const statusNome = safeText(statusNameById.get(statusVagaId) ?? statusVagaId);

        const modificadoEmRaw = safeText(
          record.dataUltimaAlteracao ?? record.ultimaAlteracao ?? record.dataAlteracao ?? record.dataAtualizacao
        );
        const modificadoEm = modificadoEmRaw ? formatDatePtBr(modificadoEmRaw) : '';
        const alteradoPor = safeText(
          record.nomeUsuarioAlterador ?? record.nomeColaboradorAlteracao ?? record.usuarioAlterador
        );

        const codigoVagaNum =
          typeof record.codigo === 'number'
            ? record.codigo
            : typeof record.codigo !== 'undefined' && record.codigo !== null
              ? Number(record.codigo)
              : undefined;
        const codigoVaga = firstNonEmpty(record, ['codigo', 'numeroVagaCliente', 'codigoVaga', 'codigoInterno']);
        const codigoDisplay = codigoVaga || safeText(vaga.id);
        const idPerfilGerador = safeText(record.idPerfilGerador ?? record.perfilGeradorId ?? vaga.idPerfilGerador ?? '');

        return {
          key: String(vaga.id),
          titulo: `${codigoDisplay} - ${titulo}`,
          clienteLinha,
          gestor,
          aberturaEm,
          criador,
          recrutador,
          aderentesCount,
          numeroDeVagas,
          totalCandidatos,
          quantidadeCandidatosPorEstagio,
          statusId: statusVagaId,
          statusNome: statusNome || 'Status',
          modificadoEm,
          alteradoPor,
          raw: record,
          vagaId: vaga.id,
          codigoVaga: codigoVagaNum,
          idPerfilGerador: idPerfilGerador || undefined,
        };
      });
    }

    return out;
  }, [statusNameById, vagasPorStatus]);

  const openVagaInfo = (vagaRaw: UnknownRecord) => {
    setVagaInfo(vagaRaw);
    setVagaInfoOpen(true);
  };

  const listarUnidades = useMemo(() => container.resolve(ListarUnidadesVagaUseCase), []);
  const listarTiposVaga = useMemo(() => container.resolve(ListarTiposVagaUseCase), []);
  const listarTiposContratacao = useMemo(() => container.resolve(ListarTiposContratacaoVagaUseCase), []);

  useEffect(() => {
    if (!vagaInfoOpen || !token) return;
    const needUnidades = unidadesList.length === 0;
    const needTiposVaga = tiposVagaList.length === 0;
    const needTiposContratacao = tiposContratacaoList.length === 0;
    if (!needUnidades && !needTiposVaga && !needTiposContratacao) return;

    const load = async () => {
      try {
        const [unidadesRes, tiposVagaRes, tiposContratacaoRes] = await Promise.all([
          needUnidades ? listarUnidades.execute(token) : Promise.resolve([]),
          needTiposVaga ? listarTiposVaga.execute(token) : Promise.resolve([]),
          needTiposContratacao ? listarTiposContratacao.execute(token) : Promise.resolve([]),
        ]);
        if (unidadesRes.length > 0) setUnidadesList(unidadesRes);
        if (tiposVagaRes.length > 0) setTiposVagaList(tiposVagaRes);
        if (tiposContratacaoRes.length > 0) setTiposContratacaoList(tiposContratacaoRes);
      } catch (e) {
        console.error('Erro ao carregar listas do modal Dados sobre a vaga', e);
        toast.error('Não foi possível carregar algumas informações.');
      }
    };
    load();
  }, [vagaInfoOpen, token, listarUnidades, listarTiposVaga, listarTiposContratacao, unidadesList.length, tiposVagaList.length, tiposContratacaoList.length]);

  const vagaInfoModel = useMemo<VagaInfoModel | null>(() => {
    if (!vagaInfo) return null;

    const tipoVagaDesc = getDescricaoById(tiposVagaList, vagaInfo.tipoVagaId as string | undefined)
      || firstNonEmpty(vagaInfo, ['tipoVagaDescricao', 'tipoVaga']);
    const tipoContratacaoDesc = getDescricaoById(tiposContratacaoList, vagaInfo.tipoContratacaoId as number | string | undefined)
      || firstNonEmpty(vagaInfo, ['tipoContratacaoDescricao', 'tipoContratacao']);
    const unidadeDesc = getDescricaoById(unidadesList, vagaInfo.unidadeId as string | undefined)
      || firstNonEmpty(vagaInfo, ['unidadeDescricao', 'unidade']);

    const left: VagaInfoItem[] = [
      {
        label: 'Código',
        value:
          firstNonEmpty(vagaInfo, ['codigo', 'numeroVagaCliente', 'codigoVaga']) || 'Não informado',
      },
      {
        label: 'Cliente',
        value:
          [
            firstNonEmpty(vagaInfo, ['nomeCliente']),
            firstNonEmpty(vagaInfo, ['unidadeCliente', 'nomeUnidade', 'unidade']),
          ]
            .filter(Boolean)
            .join(' - ') || 'Não informado',
      },
      {
        label: 'Gestor',
        value:
          firstNonEmpty(vagaInfo, ['nomeGestor', 'nomeGestorExterno', 'gestorNome']) || 'Não informado',
      },
      {
        label: 'Abertura em',
        value: formatDatePtBr(firstNonEmpty(vagaInfo, ['dataCriacao', 'dataAbertura']), 'Não informado'),
      },
      {
        label: 'Contratação em',
        value: formatDatePtBr(firstNonEmpty(vagaInfo, ['dataEnd', 'dataContratacao', 'dataUltimaAlteracao']), 'Não informado'),
      },
      {
        label: 'Proposta',
        value: firstNonEmpty(vagaInfo, ['propostaCrm', 'proposta']) || 'Não informado',
      },
      {
        label: 'Tipo de vaga',
        value: tipoVagaDesc || 'Não informado',
      },
      {
        label: 'Observações internas',
        value: firstNonEmpty(vagaInfo, ['observacoesInternas']) || 'Não informado',
      },
    ];

    const right: VagaInfoItem[] = [
      {
        label: 'Tipo de contratação',
        value: tipoContratacaoDesc || 'Não informado',
      },
      {
        label: 'Custo',
        value: formatCurrencyBRL(vagaInfo.custoProfissional ?? firstNonEmpty(vagaInfo, ['custoProfissional'])),
      },
      {
        label: 'Modelo de trabalho',
        value: firstNonEmpty(vagaInfo, ['modeloTrabalhoDescricao', 'modeloTrabalho']) || 'Não informado',
      },
      {
        label: 'Unidade',
        value: unidadeDesc || 'Não informado',
      },
    ];

    const descricaoRaw = safeText(vagaInfo.descricao);
    const descricao =
      !descricaoRaw || descricaoRaw.trim().toLowerCase() === 'null'
        ? 'Sem descrição informada.'
        : descricaoRaw;

    return { left, right, descricao };
  }, [vagaInfo, unidadesList, tiposVagaList, tiposContratacaoList]);

  const handleNovaVaga = () => {
    logUserAction('GestaoVagas', 'CriarNovaVaga', {}, user);
  };

  const handleVisualizarVaga = (vagaId: string | number) => {
    logUserAction('GestaoVagas', 'VisualizarVaga', { vagaId }, user);
  };

  const handleAbrirInformacoesVaga = (vagaId: string | number) => {
    logUserAction('GestaoVagas', 'AbrirInformacoesVaga', { vagaId }, user);
  };

  const handleEditarVaga = (vagaId: string | number) => {
    logUserAction('GestaoVagas', 'EditarVaga', { vagaId }, user);
  };

  const handleEditarAderentes = (vagaId: string | number) => {
    logUserAction('GestaoVagas', 'EditarAderentesVaga', { vagaId }, user);
  };

  const handleAdicionarRecrutador = (vagaId: string | number) => {
    logUserAction('GestaoVagas', 'AdicionarRecrutadorVaga', { vagaId }, user);
  };

  const handleTentarAlterarStatus = (
    vagaId: string | number,
    statusFrom: string,
    statusTo: string
  ) => {
    logUserAction(
      'GestaoVagas',
      'TentarAlterarStatusVaga',
      { vagaId, statusFrom, statusTo },
      user
    );
  };

  const handleAlterarLimiteVagas = (limit: number) => {
    logUserAction('GestaoVagas', 'AlterarLimiteVagasColuna', { limit }, user);
  };

  const handleAlterarVisibilidadeColuna = (key: ColumnVisibilityKey, visible: boolean) => {
    logUserAction('GestaoVagas', 'AlterarVisibilidadeColuna', { key, visible }, user);
  };

  return {
    loading,
    busca,
    setBusca,
    dataInicio,
    setDataInicio,
    dataFim,
    setDataFim,
    totalPerfis,
    totalVagasGeral,
    totalVagasPorStatus,
    statusVagas,
    statusColumns,
    perfisFiltrados,
    perfilCards,
    vagasPorStatus,
    vagaCardsByStatus,
    maxVagasPorColuna,
    setMaxVagasPorColuna,
    columnVisibility,
    setColumnVisibility,
    handleNovaVaga,
    handleVisualizarVaga,
    vagaInfoOpen,
    setVagaInfoOpen,
    openVagaInfo,
    vagaInfoModel,
    handleAbrirInformacoesVaga,
    handleEditarVaga,
    handleEditarAderentes,
    handleAdicionarRecrutador,
    handleTentarAlterarStatus,
    handleAlterarLimiteVagas,
    handleAlterarVisibilidadeColuna,
    limiteVagas,
    setLimiteVagas,
    visualizarCandidatosPorEstagio,
    setVisualizarCandidatosPorEstagio,
    carregarVagas,
  };
};
