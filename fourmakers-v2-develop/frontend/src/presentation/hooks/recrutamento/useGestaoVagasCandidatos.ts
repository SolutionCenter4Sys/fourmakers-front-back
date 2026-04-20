import { useCallback, useEffect, useMemo, useState } from 'react';
import { useLocation, useSearchParams } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { container } from 'tsyringe';
import type {
  CandidatoInscrito,
  CandidatoAderenteRaw,
  StatusCandidatura,
  TotaisInscritos,
  UnidadeItem,
  TipoVagaItem,
  TipoContratacaoItem,
  OrigemColaboradorItem,
} from '@domain/entities/GestaoVagasCandidatos';
import type { ListarCandidatosAderentesParams } from '@domain/repositories/VagaRepository';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import { GetVagaDetalhesUseCase } from '@domain/usecases/GetVagaDetalhesUseCase';
import { ListarStatusCandidaturaRecrutamentoUseCase } from '@domain/usecases/ListarStatusCandidaturaRecrutamentoUseCase';
import { ObterTotaisInscritosUseCase } from '@domain/usecases/ObterTotaisInscritosUseCase';
import { ListarCandidatosInscritosUseCase } from '@domain/usecases/ListarCandidatosInscritosUseCase';
import { ListarCandidatosAderentesUseCase } from '@domain/usecases/ListarCandidatosAderentesUseCase';
import { ListarUnidadesVagaUseCase } from '@domain/usecases/ListarUnidadesVagaUseCase';
import { ListarTiposVagaUseCase } from '@domain/usecases/ListarTiposVagaUseCase';
import { ListarTiposContratacaoVagaUseCase } from '@domain/usecases/ListarTiposContratacaoVagaUseCase';
import { CandidatarOutraPessoaUseCase } from '@domain/usecases/CandidatarOutraPessoaUseCase';
import { ListarOrigensColaboradorUseCase } from '@domain/usecases/ListarOrigensColaboradorUseCase';
import { normalizeCandidatoInscrito, normalizeTotaisInscritos } from '@shared/utils/gestaoVagasNormalizers';
import { toast } from 'sonner';

export interface VagaCandidatosState {
  vagaId: string;
  vagaTitle: string;
  vagaRaw?: Record<string, unknown>;
}

const DEFAULT_LIMITE = 100;
const DEFAULT_DATA_INICIO = '2000-01-01';
const DEFAULT_DATA_FIM = '2100-01-01';

export const DEFAULT_FILTROS_ADERENTES: Omit<ListarCandidatosAderentesParams, 'vagaId'> = {
  cursor: 0,
  limite: 20,
  busca: '',
  dataInicio: '2000-01-01',
  dataFim: '2100-01-01',
  origens: [],
  pesoHardSkills: 1,
  pesoSoftSkills: 1,
  pesoMetodologias: 1,
  pesoDominiosNegocio: 1,
  pesoIdiomas: 1,
  pesoDisponibilidades: 1,
  qualificados: false,
  diasUltimaAlteracao: 30,
  localizacaoCidade: '',
  localizacaoEstado: '',
};

function getVagaIdFromLocation(
  location: ReturnType<typeof useLocation>,
  searchParams: URLSearchParams
): string | null {
  const state = location.state as VagaCandidatosState | null | undefined;
  if (state?.vagaId) return String(state.vagaId);
  const idFromUrl = searchParams.get('idVaga') ?? searchParams.get('vagaId');
  return idFromUrl ? String(idFromUrl) : null;
}

export function useGestaoVagasCandidatos() {
  const { token } = useAppSelector((state) => state.auth);
  const location = useLocation();
  const [searchParams] = useSearchParams();
  const getVagaDetalhes = container.resolve(GetVagaDetalhesUseCase);
  const listarStatus = container.resolve(ListarStatusCandidaturaRecrutamentoUseCase);
  const obterTotais = container.resolve(ObterTotaisInscritosUseCase);
  const listarCandidatosInscritos = container.resolve(ListarCandidatosInscritosUseCase);
  const listarCandidatosAderentes = container.resolve(ListarCandidatosAderentesUseCase);
  const listarUnidades = container.resolve(ListarUnidadesVagaUseCase);
  const listarTiposVaga = container.resolve(ListarTiposVagaUseCase);
  const listarTiposContratacao = container.resolve(ListarTiposContratacaoVagaUseCase);
  const candidatarOutraPessoa = container.resolve(CandidatarOutraPessoaUseCase);
  const listarOrigensColaborador = container.resolve(ListarOrigensColaboradorUseCase);

  const vagaId = getVagaIdFromLocation(location, searchParams);
  const stateFromNav = location.state as VagaCandidatosState | null | undefined;
  const vagaTitle = stateFromNav?.vagaTitle ?? searchParams.get('nomeVaga') ?? 'Candidatos';
  const vagaRaw = stateFromNav?.vagaRaw;

  const [loading, setLoading] = useState(true);
  const [busca, setBusca] = useState('');
  const [vagaDetalhes, setVagaDetalhes] = useState<VagaDetails | null>(null);
  const [statusCandidatura, setStatusCandidatura] = useState<StatusCandidatura[]>([]);
  const [totais, setTotais] = useState<TotaisInscritos | null>(null);
  const [candidatos, setCandidatos] = useState<CandidatoInscrito[]>([]);
  const [infoVagaExpanded, setInfoVagaExpanded] = useState(false);
  const [limiteInscritos, setLimiteInscritos] = useState(DEFAULT_LIMITE);

  const [aderentes, setAderentes] = useState<CandidatoAderenteRaw[]>([]);
  const [loadingAderentes, setLoadingAderentes] = useState(false);
  const [filtrosAderentes, setFiltrosAderentes] = useState<Omit<ListarCandidatosAderentesParams, 'vagaId'>>(DEFAULT_FILTROS_ADERENTES);
  const [filtrosAderentesOpen, setFiltrosAderentesOpen] = useState(false);
  const [origensColaborador, setOrigensColaborador] = useState<OrigemColaboradorItem[]>([]);
  const [unidadesList, setUnidadesList] = useState<UnidadeItem[]>([]);
  const [tiposVagaList, setTiposVagaList] = useState<TipoVagaItem[]>([]);
  const [tiposContratacaoList, setTiposContratacaoList] = useState<TipoContratacaoItem[]>([]);

  const carregarStatus = useCallback(async () => {
    if (!token) return;
    try {
      const list = await listarStatus.execute(token);
      setStatusCandidatura(
        list.map((s) => ({ id: String(s.id), nome: s.descricao, descricao: s.descricao }))
      );
      return list;
    } catch (e) {
      console.error('Erro ao listar status de candidatura', e);
      toast.error('Não foi possível carregar os status de candidatura.');
      return [];
    }
  }, [token, listarStatus]);

  const carregarTotais = useCallback(async () => {
    if (!token || !vagaId) return;
    try {
      const retorno = await obterTotais.execute(token, vagaId);
      setTotais(normalizeTotaisInscritos(retorno) ?? null);
    } catch (e) {
      console.error('Erro ao obter totais', e);
    }
  }, [token, vagaId, obterTotais]);

  const carregarCandidatos = useCallback(async () => {
    if (!token || !vagaId) return;
    try {
      const res = await listarCandidatosInscritos.execute(token, {
        vagaId,
        busca: busca.trim() || undefined,
        cursor: 0,
        limite: limiteInscritos,
        dataInicio: DEFAULT_DATA_INICIO,
        dataFim: DEFAULT_DATA_FIM,
      });
      setCandidatos(res.retorno.map(normalizeCandidatoInscrito));
    } catch (e) {
      console.error('Erro ao listar candidatos', e);
      toast.error('Não foi possível carregar os candidatos.');
      setCandidatos([]);
    }
  }, [token, vagaId, busca, limiteInscritos, listarCandidatosInscritos]);

  const carregarVagaDetalhes = useCallback(async () => {
    if (!token || !vagaId) return;
    try {
      const details = await getVagaDetalhes.execute(token, vagaId);
      setVagaDetalhes(details);
    } catch (e) {
      console.error('Erro ao carregar detalhes da vaga', e);
    }
  }, [token, vagaId, getVagaDetalhes]);

  const carregarAderentes = useCallback(async () => {
    if (!token || !vagaId) return;
    setLoadingAderentes(true);
    try {
      const res = await listarCandidatosAderentes.execute(token, { ...filtrosAderentes, vagaId });
      setAderentes(res.retorno);
    } catch (e) {
      console.error('Erro ao listar candidatos aderentes', e);
      toast.error('Não foi possível carregar os candidatos aderentes.');
      setAderentes([]);
    } finally {
      setLoadingAderentes(false);
    }
  }, [token, vagaId, filtrosAderentes, listarCandidatosAderentes]);

  const applyFiltrosAderentes = useCallback(
    (partial: Partial<Omit<ListarCandidatosAderentesParams, 'vagaId'>>) => {
      setFiltrosAderentes((prev) => ({ ...prev, ...partial }));
      setFiltrosAderentesOpen(false);
    },
    []
  );

  const limparFiltrosAderentes = useCallback(() => {
    setFiltrosAderentes(DEFAULT_FILTROS_ADERENTES);
    setFiltrosAderentesOpen(false);
  }, []);

  const carregarTudo = useCallback(async () => {
    if (!vagaId || !token) {
      setLoading(false);
      return;
    }
    setLoading(true);
    try {
      await Promise.all([
        carregarVagaDetalhes(),
        carregarStatus(),
        carregarTotais(),
        carregarCandidatos(),
      ]);
    } finally {
      setLoading(false);
    }
  }, [vagaId, token, carregarVagaDetalhes, carregarStatus, carregarTotais, carregarCandidatos]);

  useEffect(() => {
    carregarTudo();
  }, [carregarTudo]);

  /** Carrega listas de tipos/unidades para resolver IDs da vaga (mesma lógica do modal "Dados sobre a vaga" no kanban de vagas). */
  useEffect(() => {
    if (!token || !vagaId) return;
    const load = async () => {
      try {
        const [unidadesListRes, tiposVagaRes, tiposContratacaoRes] = await Promise.all([
          listarUnidades.execute(token),
          listarTiposVaga.execute(token),
          listarTiposContratacao.execute(token),
        ]);
        setUnidadesList(unidadesListRes);
        setTiposVagaList(tiposVagaRes);
        setTiposContratacaoList(tiposContratacaoRes);
      } catch (e) {
        console.error('Erro ao carregar listas (tipos vaga, contratação, unidades)', e);
      }
    };
    load();
  }, [token, vagaId, listarUnidades, listarTiposVaga, listarTiposContratacao]);

  useEffect(() => {
    if (!token) return;
    listarOrigensColaborador
      .execute(token)
      .then(setOrigensColaborador)
      .catch(() => setOrigensColaborador([]));
  }, [token, listarOrigensColaborador]);

  useEffect(() => {
    if (!vagaId || !token) return;
    carregarAderentes();
  }, [vagaId, token, filtrosAderentes, carregarAderentes]);

  useEffect(() => {
    if (!vagaId) return;
    const t = setTimeout(() => {
      carregarCandidatos();
    }, 400);
    return () => clearTimeout(t);
  }, [busca, vagaId, carregarCandidatos]);

  const candidatosPorStatus = useMemo(() => {
    const map: Record<string, CandidatoInscrito[]> = {};
    for (const s of statusCandidatura) {
      map[s.id] = [];
    }
    for (const c of candidatos) {
      const statusId = String(c.statusCandidaturaId ?? c.statusId ?? '');
      if (!map[statusId]) map[statusId] = [];
      map[statusId].push(c);
    }
    const sortByModificadoEm = (a: CandidatoInscrito, b: CandidatoInscrito) => {
      const da = a.modificadoEm || '';
      const db = b.modificadoEm || '';
      return db.localeCompare(da);
    };
    for (const statusId of Object.keys(map)) {
      map[statusId].sort(sortByModificadoEm);
    }
    return map;
  }, [statusCandidatura, candidatos]);

  const refetch = useCallback(() => {
    carregarTotais();
    carregarCandidatos();
  }, [carregarTotais, carregarCandidatos]);

  const inscreverCandidato = useCallback(
    async (codigoColaborador: string) => {
      if (!token || !codigoColaborador) return;
      const codigoVaga = String(vagaDetalhes?.codigo ?? '');
      if (!codigoVaga) {
        toast.error('Código da vaga não disponível.');
        return;
      }
      toast.info('Inscrevendo candidato. Por favor, aguarde...');
      try {
        await candidatarOutraPessoa.execute(token, {
          codigoVaga,
          codigoColaborador,
          opcoesContatoIds: [],
        });
        toast.success('Candidato inscrito com sucesso!');
        refetch();
      } catch (e) {
        console.error('Erro ao inscrever candidato', e);
        toast.error('Não foi possível inscrever o candidato.');
      }
    },
    [token, vagaDetalhes?.codigo, candidatarOutraPessoa, refetch]
  );

  return {
    vagaId,
    vagaTitle,
    vagaRaw,
    vagaDetalhes,
    unidadesList,
    tiposVagaList,
    tiposContratacaoList,
    loading,
    busca,
    setBusca,
    statusCandidatura,
    totais,
    candidatos,
    candidatosPorStatus,
    infoVagaExpanded,
    setInfoVagaExpanded,
    limiteInscritos,
    setLimiteInscritos,
    refetch,
    aderentes,
    loadingAderentes,
    filtrosAderentes,
    setFiltrosAderentes,
    filtrosAderentesOpen,
    setFiltrosAderentesOpen,
    applyFiltrosAderentes,
    limparFiltrosAderentes,
    carregarAderentes,
    origensColaborador,
    inscreverCandidato,
  };
}
