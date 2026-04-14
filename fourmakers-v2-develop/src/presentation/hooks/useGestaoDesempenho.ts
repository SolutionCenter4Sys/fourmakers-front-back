import { useState, useEffect, useMemo, useCallback } from 'react';
import { container } from '@core/di/container';
import { ObterDashboardGestorUseCase } from '@domain/usecases/ObterDashboardGestorUseCase';
import { ObterMeusColaboradoresUseCase } from '@domain/usecases/ObterMeusColaboradoresUseCase';
import { ObterListaColaboradoresRHUseCase } from '@domain/usecases/ObterListaColaboradoresRHUseCase';
import { ObterDashboardColaboradorAvaliadoUseCase } from '@domain/usecases/ObterDashboardColaboradorAvaliadoUseCase';
import { ObterDashboardColaboradorUseCase } from '@domain/usecases/ObterDashboardColaboradorUseCase';
import { ObterDashboardRHUseCase } from '@domain/usecases/ObterDashboardRHUseCase';
import { useAppSelector } from '@app/store/hooks';
import type { EstatisticasDesempenho, EstatisticasRH, EstatisticasPessoais, RegistroCritico, ColaboradorDesempenho, FiltroDesempenho, ColaboradorDesempenhoDetalhes, Feedback, RegistroUmAUm, PDI, PautaSugerida } from '@shared/types/gestaoDesempenho';
import type { PdiResumoTimeDTO, PdiCompletoTimeDTO, PdiMetricasResultDTO } from '@shared/types/pdiApi';
import { getPdiStatusDisplayLabel, mapPdiStatusToLabel, PdiStatusLabel } from '@shared/types/pdiApi';
import { ColaboradorPdiApi } from '@data/api/ColaboradorPdiApi';

export const useEstatisticasDesempenho = () => {
  const [estatisticas, setEstatisticas] = useState<EstatisticasDesempenho | null>(null);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const useCase = container.resolve(ObterDashboardGestorUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          const { painelControle } = response.retorno;
          setEstatisticas({
            totalColaboradores: painelControle.qtdTotalColaboradores,
            umAUmEmDia: painelControle.qtdOneOnOneEmDia,
            umAUmAtrasados: painelControle.qtdOneOnOneAtrasado,
            feedbackEmDia: painelControle.qtdFeedbackEmDia,
            feedbackAtrasados: painelControle.qtdFeedbackAtrasado,
            pdisAtivos: 0, // Não vem da API, manter 0 ou buscar de outra fonte se necessário
          });
        }
      } catch (error) {
        console.error('Erro ao buscar estatísticas:', error);
        // Em caso de erro, manter null para que a UI mostre valores padrão
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token]);

  return { estatisticas, loading };
};

export const useRegistrosCriticos = () => {
  const [registros, setRegistros] = useState<RegistroCritico[]>([]);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const useCase = container.resolve(ObterDashboardGestorUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          const { oneOnOnesComRegistroCritico } = response.retorno;
          // Mapear para o formato esperado pela página
          const registrosMapeados: RegistroCritico[] = oneOnOnesComRegistroCritico.map((item, index) => ({
            id: `registro-${index}`,
            colaboradorNome: item.nomeCompleto,
            colaboradorCod: '', // Se necessário buscar de outra fonte, pode ser adicionado depois
            data: item.dataReuniao,
            descricao: item.descricaoAnotacoes,
          }));
          setRegistros(registrosMapeados);
        }
      } catch (error) {
        console.error('Erro ao buscar registros críticos:', error);
        // Em caso de erro, manter array vazio
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token]);

  return { registros, loading };
};

export const useColaboradoresDesempenho = (filtro: FiltroDesempenho = 'todos', busca: string = '') => {
  const [colaboradores, setColaboradores] = useState<ColaboradorDesempenho[]>([]);
  const [loading, setLoading] = useState(true);
  const { token, user } = useAppSelector((state) => state.auth);
  const orgId = user?.colaboradorOrg?.orgId ?? 0;

  const fetchData = async () => {
    if (!token) {
      setLoading(false);
      return;
    }

    try {
      const useCase = container.resolve(ObterMeusColaboradoresUseCase);
      const response = await useCase.execute(token);

      // Contagem de PDIs ativos por colaboradorId via API Gestão de Pessoa (PdisDoTime)
      let contagemPdiPorUuid: Map<string, number> = new Map();
      try {
        const pdiApi = new ColaboradorPdiApi();
        const listagem = await pdiApi.getPdisTime(token, { pagina: 1, tamanhoPagina: 500 });
        const items = listagem?.items ?? [];
        for (const item of items) {
          const label = getPdiStatusDisplayLabel(item.status, item.progress);
          const ativo = label !== PdiStatusLabel.FINALIZADO && label !== PdiStatusLabel.CANCELADO;
          if (ativo && item.colaboradorId) {
            const id = item.colaboradorId.trim();
            contagemPdiPorUuid.set(id, (contagemPdiPorUuid.get(id) ?? 0) + 1);
          }
        }
      } catch (err) {
        console.warn('Erro ao buscar PDIs do time para contagem:', err);
      }

      if (response.sucesso && response.retorno) {
        const { meusColaboradores } = response.retorno;
        // Mapear para o formato esperado pela página
        const colaboradoresMapeados: ColaboradorDesempenho[] = meusColaboradores.map((item) => {
          // Determinar status baseado na string retornada
          let status: 'ativo' | 'ferias' | 'afastado' = 'ativo';
          if (item.status.toLowerCase().includes('férias') || item.status.toLowerCase().includes('ferias')) {
            status = 'ferias';
          } else if (item.status.toLowerCase().includes('afastado')) {
            status = 'afastado';
          }

          // Verificar se tem feedback e 1:1
          const temFeedback = item.dataUltimoFeedback !== null;
          const temUmAUm = item.dataUltimoOneOnOne !== null;

          // codigoInternoColaborador é o uuid do colaborador; match com uuid_colab do XANO
          const pdisAtivos = contagemPdiPorUuid.get(item.codigoInternoColaborador?.trim() ?? '') ?? 0;

          return {
            id: item.codigoInternoColaborador,
            codColaborador: item.codigoInternoColaborador,
            codigoColaboradorExterno: item.codColaboradorExterno || item.codigoInternoColaborador,
            nome: item.nomeCompleto,
            cargo: item.cargo,
            gestor: '', // Não vem da API, pode ser adicionado depois se necessário
            status,
            ultimoFeedback: item.dataUltimoFeedback || undefined, // Passar ISO string diretamente, formatData vai formatar
            ultimaUmAUm: item.dataUltimoOneOnOne || undefined, // Passar ISO string diretamente, formatData vai formatar
            pdisAtivos,
            temPdi: pdisAtivos > 0,
            temFeedback,
            temUmAUm,
          };
        });
        setColaboradores(colaboradoresMapeados);
      }
    } catch (error) {
      console.error('Erro ao buscar colaboradores:', error);
      // Em caso de erro, manter array vazio
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [token, orgId]);

  const colaboradoresFiltrados = useMemo(() => {
    let filtrados = [...colaboradores];

    // Aplicar filtro
    switch (filtro) {
      case 'sem-pdi':
        filtrados = filtrados.filter((c) => !c.temPdi);
        break;
      case 'sem-feedback':
        filtrados = filtrados.filter((c) => !c.temFeedback);
        break;
      case 'sem-1-1':
        filtrados = filtrados.filter((c) => !c.temUmAUm);
        break;
      case 'todos':
      default:
        break;
    }

    // Aplicar busca
    if (busca) {
      const buscaLower = busca.toLowerCase();
      filtrados = filtrados.filter(
        (c) =>
          c.nome.toLowerCase().includes(buscaLower) ||
          c.cargo.toLowerCase().includes(buscaLower) ||
          c.codColaborador.toLowerCase().includes(buscaLower) ||
          (c.gestor && c.gestor.toLowerCase().includes(buscaLower))
      );
    }

    return filtrados;
  }, [colaboradores, filtro, busca]);

  const contadoresFiltros = useMemo(() => {
    return {
      semPdi: colaboradores.filter((c) => !c.temPdi).length,
      semFeedback: colaboradores.filter((c) => !c.temFeedback).length,
      semUmAUm: colaboradores.filter((c) => !c.temUmAUm).length,
    };
  }, [colaboradores]);

  const totalPdisAtivos = useMemo(
    () => colaboradores.reduce((acc, c) => acc + c.pdisAtivos, 0),
    [colaboradores]
  );

  return {
    colaboradores: colaboradoresFiltrados,
    loading,
    contadoresFiltros,
    totalPdisAtivos,
    refetch: fetchData,
  };
};

export const useColaboradoresRH = (filtro: FiltroDesempenho = 'todos', busca: string = '') => {
  const [colaboradores, setColaboradores] = useState<ColaboradorDesempenho[]>([]);
  const [loading, setLoading] = useState(true);
  const [accessDenied, setAccessDenied] = useState(false);
  const { token, user } = useAppSelector((state) => state.auth);
  const orgId = user?.colaboradorOrg?.orgId ?? 0;

  const fetchData = async () => {
    if (!token) {
      setLoading(false);
      return;
    }

    try {
      // Preparar parâmetros de filtro para a API
      const apiParams: { semFeedback?: boolean; semOneOnOne?: boolean } = {};
      
      if (filtro === 'sem-feedback') {
        apiParams.semFeedback = true;
      } else if (filtro === 'sem-1-1') {
        apiParams.semOneOnOne = true;
      }
      // Para 'sem-pdi' e 'todos', não passamos parâmetros (filtro será feito no frontend se necessário)

      const useCase = container.resolve(ObterListaColaboradoresRHUseCase);
      const response = await useCase.execute(token, Object.keys(apiParams).length > 0 ? apiParams : undefined);

      // Contagem de PDIs ativos por colaborador — nova API GET /api/GestaoPessoa/Pdi/Metricas/gestor (ativos já filtrados)
      let contagemPdiPorUuid: Map<string, number> = new Map();
      try {
        const colaboradorPdiApi = new ColaboradorPdiApi();
        const metricas = await colaboradorPdiApi.getMetricasGestor(token);
        const ativos = metricas?.ativos ?? [];
        for (const item of ativos) {
          const id = item.colaboradorId?.trim();
          if (id) {
            contagemPdiPorUuid.set(id, (contagemPdiPorUuid.get(id) ?? 0) + 1);
          }
        }
      } catch (err) {
        console.warn('[useColaboradoresRH] Erro ao buscar métricas PDI para contagem:', err);
      }
      
      // API pode retornar { retorno: { colaboradores: ColaboradorRH[] } } ou { retorno: ColaboradorRH[] } ou PascalCase (Colaboradores)
      const rawRetorno = response?.retorno as
        | { colaboradores?: unknown[]; Colaboradores?: unknown[] }
        | unknown[]
        | null
        | undefined;
      const lista: unknown[] = Array.isArray(rawRetorno)
        ? rawRetorno
        : (rawRetorno && typeof rawRetorno === 'object' && 'colaboradores' in rawRetorno && Array.isArray((rawRetorno as { colaboradores: unknown[] }).colaboradores))
          ? (rawRetorno as { colaboradores: unknown[] }).colaboradores
          : (rawRetorno && typeof rawRetorno === 'object' && 'Colaboradores' in rawRetorno && Array.isArray((rawRetorno as { Colaboradores: unknown[] }).Colaboradores))
            ? (rawRetorno as { Colaboradores: unknown[] }).Colaboradores
            : [];

      if (response?.sucesso && lista.length >= 0) {
        // Normalizar item: backend pode enviar camelCase, PascalCase ou snake_case; ou objeto aninhado
        const getVal = (item: unknown, ...keys: string[]): unknown => {
          let val: unknown = item;
          for (const k of keys) {
            if (val !== null && val !== undefined && typeof val === 'object' && k in (val as Record<string, unknown>)) {
              val = (val as Record<string, unknown>)[k];
            } else {
              val = null;
              break;
            }
          }
          return val;
        };
        const getStr = (item: unknown, ...keySets: string[][]): string => {
          for (const keys of keySets) {
            const v = keys.length === 1 ? getVal(item, keys[0]) : getVal(item, ...keys);
            if (v !== null && v !== undefined && typeof v === 'string') return v;
            if (typeof v === 'number') return String(v);
          }
          return '';
        };
        const getArr = (item: unknown, ...keySets: string[][]): string[] => {
          for (const keys of keySets) {
            const v = keys.length === 1 ? getVal(item, keys[0]) : getVal(item, ...keys);
            if (Array.isArray(v)) return v.map((x) => (typeof x === 'string' ? x : typeof x === 'object' && x && 'nome' in x ? String((x as { nome: unknown }).nome) : String(x)));
          }
          return [];
        };
        const getDate = (item: unknown, ...keySets: string[][]): string | null => {
          for (const keys of keySets) {
            const v = keys.length === 1 ? getVal(item, keys[0]) : getVal(item, ...keys);
            if (v !== null && v !== undefined) return typeof v === 'string' ? v : typeof v === 'number' ? new Date(v).toISOString().slice(0, 10) : null;
          }
          return null;
        };

        /** Converte texto em CAIXA ALTA para formato legível (primeira letra maiúscula, resto minúsculo; em pt-BR mantém "de", "da", "do", "das", "dos", "e" em minúsculo no meio). */
        const normalizarExibicao = (s: string): string => {
          if (!s || s.length < 2) return s;
          const isMostlyUpper = s.replace(/\s/g, '').length > 0 && (s.match(/[A-ZÀ-Ú]/g)?.length ?? 0) > (s.replace(/\s/g, '').length * 0.7);
          if (!isMostlyUpper) return s;
          const minusculasNoMeio = new Set(['de', 'da', 'do', 'das', 'dos', 'e', 'em', 'no', 'na', 'nos', 'nas']);
          const palavras = s.toLowerCase().split(/\s+/);
          return palavras
            .map((palavra, i) => {
              if (i > 0 && minusculasNoMeio.has(palavra)) return palavra;
              return palavra.charAt(0).toUpperCase() + palavra.slice(1);
            })
            .join(' ');
        };

        const colaboradoresMapeados: ColaboradorDesempenho[] = lista.map((item, index) => {
          const codigo = (
            getStr(item, ['codigoInternoColaborador'], ['CodigoInternoColaborador'], ['codigo_interno_colaborador'], ['id']) ||
            (typeof (item as Record<string, unknown>)?.id === 'string' ? (item as Record<string, unknown>).id as string : '')
          ).trim();
          const nome = getStr(item, ['nomeCompleto'], ['NomeCompleto'], ['nome_completo'], ['nome'], ['name']);
          const cargo = getStr(item, ['cargo'], ['Cargo'], ['funcao'], ['position']);
          const statusStr = getStr(item, ['status'], ['Status']).toLowerCase();
          let status: 'ativo' | 'ferias' | 'afastado' = 'ativo';
          if (statusStr.includes('férias') || statusStr.includes('ferias')) status = 'ferias';
          else if (statusStr.includes('afastado')) status = 'afastado';

          const dataFeedback = getDate(item, ['dataUltimoFeedback'], ['DataUltimoFeedback'], ['data_ultimo_feedback'], ['ultimoFeedback']);
          const dataUmAUm = getDate(item, ['dataUltimoOneOnOne'], ['DataUltimoOneOnOne'], ['data_ultimo_one_on_one'], ['ultimaUmAUm']);
          const temFeedback = dataFeedback !== null && dataFeedback !== undefined && String(dataFeedback).trim() !== '';
          const temUmAUm = dataUmAUm !== null && dataUmAUm !== undefined && String(dataUmAUm).trim() !== '';

          const gestoresArray = getArr(item, ['nomesColaboradoresSuperiores'], ['NomesColaboradoresSuperiores'], ['nomes_colaboradores_superiores'], ['gestor'], ['gestorNome']);
          const gestorStr = getStr(item, ['gestor'], ['Gestor'], ['gestor_nome']);
          const gestorRaw = gestoresArray.length > 0
            ? (gestoresArray.length === 1 ? gestoresArray[0] : gestoresArray.join(', '))
            : gestorStr;

          const pdisAtivos = codigo ? contagemPdiPorUuid.get(codigo) ?? 0 : 0;

          // id único para evitar "duplicate key" no React (codigo pode repetir ou vir vazio)
          const uniqueId = codigo ? `rh-${codigo}-${index}` : `rh-row-${index}`;

          return {
            id: uniqueId,
            codColaborador: codigo,
            nome: normalizarExibicao(nome) || '--',
            cargo: normalizarExibicao(cargo) || '--',
            gestor: normalizarExibicao(gestorRaw) || '--',
            status,
            ultimoFeedback: dataFeedback ?? undefined,
            ultimaUmAUm: dataUmAUm ?? undefined,
            pdisAtivos,
            temPdi: pdisAtivos > 0,
            temFeedback,
            temUmAUm,
          };
        });

        setColaboradores(colaboradoresMapeados);
      } else {
        setColaboradores([]);
      }
    } catch (error) {
      const isAccessDenied = error instanceof Error && error.message === ACCESS_DENIED_RH_MESSAGE;
      setAccessDenied(isAccessDenied);
      if (!isAccessDenied) {
        console.error('[useColaboradoresRH] Erro ao buscar colaboradores RH:', error);
      }
      setColaboradores([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setAccessDenied(false);
    fetchData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [token, filtro, orgId]);

  const colaboradoresFiltrados = useMemo(() => {
    let filtrados = [...colaboradores];

    // Aplicar filtro apenas para 'sem-pdi' (que não tem suporte na API)
    // Os filtros 'sem-feedback' e 'sem-1-1' já são aplicados pela API
    switch (filtro) {
      case 'sem-pdi':
        filtrados = filtrados.filter((c) => !c.temPdi);
        break;
      case 'sem-feedback':
      case 'sem-1-1':
      case 'todos':
      default:
        // Filtros já aplicados pela API ou não necessários
        break;
    }

    // Aplicar busca (sempre no frontend)
    if (busca) {
      const buscaLower = busca.toLowerCase();
      filtrados = filtrados.filter(
        (c) =>
          c.nome.toLowerCase().includes(buscaLower) ||
          c.cargo.toLowerCase().includes(buscaLower) ||
          c.codColaborador.toLowerCase().includes(buscaLower) ||
          (c.gestor && c.gestor.toLowerCase().includes(buscaLower))
      );
    }

    return filtrados;
  }, [colaboradores, filtro, busca]);

  const contadoresFiltros = useMemo(() => {
    return {
      semPdi: colaboradores.filter((c) => !c.temPdi).length,
      semFeedback: colaboradores.filter((c) => !c.temFeedback).length,
      semUmAUm: colaboradores.filter((c) => !c.temUmAUm).length,
    };
  }, [colaboradores]);

  return { colaboradores: colaboradoresFiltrados, loading, contadoresFiltros, refetch: fetchData, accessDenied };
};

export const useColaboradorDesempenhoDetalhes = (codColaborador: string) => {
  const [detalhes, setDetalhes] = useState<ColaboradorDesempenhoDetalhes | null>(null);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token || !codColaborador) {
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const useCase = container.resolve(ObterDashboardColaboradorAvaliadoUseCase);
        const response = await useCase.execute(token, codColaborador);
        
        if (response.sucesso && response.retorno) {
          const { retorno } = response;
          
          // Determinar status
          let status: 'ativo' | 'ferias' | 'afastado' = 'ativo';
          if (retorno.status.toLowerCase().includes('férias') || retorno.status.toLowerCase().includes('ferias')) {
            status = 'ferias';
          } else if (retorno.status.toLowerCase().includes('afastado')) {
            status = 'afastado';
          }

          const retornoAny = retorno as unknown as Record<string, unknown>;
          const codigoExterno = (retornoAny.codigoColaboradorExterno ?? retornoAny.CodigoColaboradorExterno) as string | undefined | null;
          const modalidade = (retornoAny.modalidadeContratacao ?? retornoAny.ModalidadeContratacao) as string | undefined | null;
          const regimeTrabalho = (retornoAny.regimeTrabalho ?? retornoAny.RegimeTrabalho) as string | undefined | null;

          setDetalhes({
            id: retorno.codigoInternoColaboradorAvaliado,
            codColaborador: retorno.codigoInternoColaboradorAvaliado,
            nome: retorno.nomeCompletoColaboradorAvaliado,
            cargo: retorno.cargo,
            email: retorno.email || '',
            telefone: retorno.telefone || '',
            dataNascimento: retorno.dataNascimento,
            dataAdmissao: retorno.dataAdmissao,
            tempoCasa: retorno.tempoCasa,
            salario: 0,
            saldoHoras: 0,
            status,
            modelo: '',
            codigoColaboradorExterno: codigoExterno ?? null,
            modalidadeContratacao: modalidade ?? null,
            regimeTrabalho: regimeTrabalho ?? '',
          });
        }
      } catch (error) {
        console.error('Erro ao buscar detalhes do colaborador:', error);
        // Em caso de erro, manter null
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token, codColaborador]);

  return { detalhes, loading };
};

export const useFeedbacks = (codColaborador: string) => {
  const [feedbacks, setFeedbacks] = useState<Feedback[]>([]);
  const [loading, setLoading] = useState(true);
  const { token, user } = useAppSelector((state) => state.auth);

  const fetchData = async () => {
    if (!token || !codColaborador) {
      setLoading(false);
      return;
    }

    try {
      // Verificar se é o próprio colaborador logado
      const isProprioColaborador = user?.colaboradorOrg?.codColaborador === codColaborador;
      
      let dashboardFeedbacks;
      
      if (isProprioColaborador) {
        // Usar endpoint do colaborador
        const useCase = container.resolve(ObterDashboardColaboradorUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          dashboardFeedbacks = response.retorno.dashboardFeedbacks;
        }
      } else {
        // Usar endpoint do gestor (colaborador avaliado)
        const useCase = container.resolve(ObterDashboardColaboradorAvaliadoUseCase);
        const response = await useCase.execute(token, codColaborador);
        
        if (response.sucesso && response.retorno) {
          dashboardFeedbacks = response.retorno.dashboardFeedbacks;
        }
      }
      
      if (dashboardFeedbacks) {
        // Mapear feedbacks para o formato esperado
        const feedbacksMapeados: Feedback[] = dashboardFeedbacks.feedbacks.map((item) => ({
          id: item.id,
          data: item.dataReuniao,
          resumo: item.descricaoObservacoesGerais || '',
          realizadoPor: item.nomeCompletoColaboradorSuperior,
          visto: item.visualizadoPeloColaborador,
          vistoEm: item.dataVisualizadoColaborador || undefined,
          continuar: item.descricaoContinuar ? [item.descricaoContinuar] : undefined,
          comecar: item.descricaoComecar ? [item.descricaoComecar] : undefined,
          parar: item.descricaoParar ? [item.descricaoParar] : undefined,
          observacoesGerais: item.descricaoObservacoesGerais || undefined,
        }));
        
        setFeedbacks(feedbacksMapeados);
      }
    } catch (error) {
      console.error('Erro ao buscar feedbacks:', error);
      // Em caso de erro, manter array vazio
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [token, codColaborador, user?.colaboradorOrg?.codColaborador]);

  const feedbacksVistos = useMemo(() => feedbacks.filter((f) => f.visto).length, [feedbacks]);
  const feedbacksNaoVistos = useMemo(() => feedbacks.filter((f) => !f.visto).length, [feedbacks]);

  return { feedbacks, loading, feedbacksVistos, feedbacksNaoVistos, refetch: fetchData };
};

export const useRegistrosUmAUm = (codColaborador: string) => {
  const [registros, setRegistros] = useState<RegistroUmAUm[]>([]);
  const [loading, setLoading] = useState(true);
  const { token, user } = useAppSelector((state) => state.auth);

  const fetchData = async () => {
    if (!token || !codColaborador) {
      setLoading(false);
      return;
    }

    try {
      // Verificar se é o próprio colaborador logado
      const isProprioColaborador = user?.colaboradorOrg?.codColaborador === codColaborador;
      
      let dashboardOneOnOne;
      
      if (isProprioColaborador) {
        // Usar endpoint do colaborador
        const useCase = container.resolve(ObterDashboardColaboradorUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          dashboardOneOnOne = response.retorno.dashboardOneOnOne;
        }
      } else {
        // Usar endpoint do gestor (colaborador avaliado)
        const useCase = container.resolve(ObterDashboardColaboradorAvaliadoUseCase);
        const response = await useCase.execute(token, codColaborador);
        
        if (response.sucesso && response.retorno) {
          dashboardOneOnOne = response.retorno.dashboardOneOnOne;
        }
      }
      
      if (dashboardOneOnOne) {
        // Mapear registros 1:1 para o formato esperado
        const registrosMapeados: RegistroUmAUm[] = dashboardOneOnOne.oneOnOnes.map((item) => ({
          id: item.id,
          data: item.dataReuniao,
          resumo: item.descricaoAnotacoes || '',
          realizadoPor: item.nomeCompletoColaboradorSuperior,
          visto: item.visualizadoPeloColaborador,
          vistoEm: item.dataVisualizadoColaborador || undefined,
          critico: item.registroCritico,
          anotacoes: item.descricaoAnotacoes || undefined,
        }));
        
        setRegistros(registrosMapeados);
      }
    } catch (error) {
      console.error('Erro ao buscar registros 1:1:', error);
      // Em caso de erro, manter array vazio
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [token, codColaborador, user?.colaboradorOrg?.codColaborador]);

  const registrosVistos = useMemo(() => registros.filter((r) => r.visto).length, [registros]);
  const registrosNaoVistos = useMemo(() => registros.filter((r) => !r.visto).length, [registros]);

  return { registros, loading, registrosVistos, registrosNaoVistos, refetch: fetchData };
};

export const usePautasSugeridas = (codColaborador: string) => {
  const [pautas, setPautas] = useState<PautaSugerida[]>([]);
  const [loading, setLoading] = useState(true);
  const { token, user } = useAppSelector((state) => state.auth);

  const fetchData = async () => {
    if (!token || !codColaborador) {
      setLoading(false);
      return;
    }

    try {
      // Verificar se é o próprio colaborador logado
      const isProprioColaborador = user?.colaboradorOrg?.codColaborador === codColaborador;
      
      let dashboardOneOnOne;
      
      if (isProprioColaborador) {
        // Usar endpoint do colaborador
        const useCase = container.resolve(ObterDashboardColaboradorUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          dashboardOneOnOne = response.retorno.dashboardOneOnOne;
        }
      } else {
        // Usar endpoint do gestor (colaborador avaliado)
        const useCase = container.resolve(ObterDashboardColaboradorAvaliadoUseCase);
        const response = await useCase.execute(token, codColaborador);
        
        if (response.sucesso && response.retorno) {
          dashboardOneOnOne = response.retorno.dashboardOneOnOne;
        }
      }
      
      if (dashboardOneOnOne) {
        // Mapear pautas sugeridas para objetos com origem
        const pautasMapeadas: PautaSugerida[] = dashboardOneOnOne.pautasSugeridas.map((pauta) => ({
          id: pauta.id,
          descricao: pauta.descricaoPautaSugerida,
          origem: pauta.tipoOrigem,
        }));
        
        setPautas(pautasMapeadas);
      }
    } catch (error) {
      console.error('Erro ao buscar pautas sugeridas:', error);
      // Em caso de erro, manter array vazio
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [token, codColaborador, user?.colaboradorOrg?.codColaborador]);

  return { pautas, loading, refetch: fetchData };
};

/** Alinha status da API Gestão de Pessoa (NOT_STARTED, IN_ANALYSIS, IN_PROGRESS, FINALIZADO, CANCELADO) com os cards Concluídos / Em andamento / Pendentes */
function mapApiStatusToPdiStatus(apiStatus: string): PDI['status'] {
  const s = (apiStatus || '').toLowerCase().trim();
  if (s === 'finalizado' || s === 'completed' || s.includes('conclu') || s.includes('finaliz')) return 'concluido';
  if (s === 'in_progress' || s === 'in progress' || s.includes('andamento') || s === 'em_andamento') return 'em-andamento';
  return 'pendente'; // NOT_STARTED, IN_ANALYSIS, CANCELADO, etc.
}

const colaboradorPdiApi = new ColaboradorPdiApi();

export const usePDIs = (codColaborador: string) => {
  const [pdisResumoTime, setPdisResumoTime] = useState<PdiResumoTimeDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  const fetchData = useCallback(async () => {
    if (!token || !codColaborador) {
      setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const list = await colaboradorPdiApi.getPdisTimeByColaborador(token, codColaborador);
      setPdisResumoTime(
        list.map((p) => {
          let previsaoConclusao: string | null = null;
          const pWithDeadline = p as { deadLine?: string; actionPlans?: { deadline?: string }[] };
          if (pWithDeadline.deadLine?.trim()) {
            const dateOnly = pWithDeadline.deadLine.trim().slice(0, 10);
            const [y, m, d] = dateOnly.split('-').map(Number);
            if (y && m && d) previsaoConclusao = `${String(d).padStart(2, '0')}/${String(m).padStart(2, '0')}/${y}`;
          }
          if (!previsaoConclusao) {
            const prazos = (p.actionPlans ?? []).filter((ap) => ap.deadline).map((ap) => ap.deadline!);
            if (prazos.length > 0) {
              const ultimo = prazos.sort((a, b) => new Date(b).getTime() - new Date(a).getTime())[0];
              const [y, m, d] = String(ultimo).split('T')[0].split('-').map(Number);
              if (y && m && d) previsaoConclusao = `${String(d).padStart(2, '0')}/${String(m).padStart(2, '0')}/${y}`;
            }
          }
          const evidencias = (p as { evidencias?: unknown[] }).evidencias;
          const temAnexo = Array.isArray(evidencias) && evidencias.length > 0;
          return {
            colaboradorId: p.colaboradorId,
            pdiId: p.id,
            titulo: p.titulo,
            status: p.status,
            progress: p.progress,
            previsaoConclusao,
            temAnexo,
            codigoInternoColaboradorCriacao: p.codigoInternoColaboradorCriacao ?? undefined,
          };
        })
      );
    } catch (err) {
      console.error('[usePDIs] Erro ao buscar PDIs do colaborador:', err);
      setPdisResumoTime([]);
    } finally {
      setLoading(false);
    }
  }, [token, codColaborador]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const pdis: PDI[] = useMemo(
    () =>
      pdisResumoTime.map((p) => ({
        id: p.pdiId,
        titulo: p.titulo,
        descricao: '',
        status: mapApiStatusToPdiStatus(p.status),
        dataInicio: '',
        criadoPor: 'Gestor',
      })),
    [pdisResumoTime]
  );

  const pdisConcluidos = useMemo(() => pdis.filter((p) => p.status === 'concluido').length, [pdis]);
  const pdisEmAndamento = useMemo(() => pdis.filter((p) => p.status === 'em-andamento').length, [pdis]);
  const pdisPendentes = useMemo(() => pdis.filter((p) => p.status === 'pendente').length, [pdis]);

  /** Contadores pelos 5 status: alinhado ao badge da tabela (mapPdiStatusToLabel) para big number bater com a listagem. */
  const contadoresPdi = useMemo(
    () => ({
      naoIniciados: pdisResumoTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.NAO_INICIADO).length,
      emAnalise: pdisResumoTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.EM_ANALISE).length,
      emAndamento: pdisResumoTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.IN_PROGRESS).length,
      finalizados: pdisResumoTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.FINALIZADO).length,
      cancelados: pdisResumoTime.filter((p) => mapPdiStatusToLabel(p.status) === PdiStatusLabel.CANCELADO).length,
    }),
    [pdisResumoTime]
  );

  const pdisAtivos = useMemo(
    () =>
      pdisResumoTime.filter((p) => {
        const label = getPdiStatusDisplayLabel(p.status, p.progress);
        return label !== PdiStatusLabel.FINALIZADO && label !== PdiStatusLabel.CANCELADO;
      }),
    [pdisResumoTime]
  );

  const pdisHistorico = useMemo(
    () =>
      pdisResumoTime.filter((p) => {
        const label = getPdiStatusDisplayLabel(p.status, p.progress);
        return label === PdiStatusLabel.FINALIZADO || label === PdiStatusLabel.CANCELADO;
      }),
    [pdisResumoTime]
  );

  return {
    pdis,
    pdisResumoTime,
    loading,
    pdisConcluidos,
    pdisEmAndamento,
    pdisPendentes,
    contadoresPdi,
    pdisAtivos,
    pdisHistorico,
    refetchPdis: fetchData,
  };
};

export const usePdiCompletoTime = (pdiId: string) => {
  const [pdi, setPdi] = useState<PdiCompletoTimeDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  const fetchData = useCallback(async () => {
    if (!token || !pdiId) {
      setLoading(false);
      return;
    }
    setLoading(true);
    try {
      const data = await colaboradorPdiApi.getPdiTimeById(token, pdiId);
      setPdi(data);
    } catch (err) {
      console.error('[usePdiCompletoTime] Erro ao buscar PDI:', err);
      setPdi(null);
    } finally {
      setLoading(false);
    }
  }, [token, pdiId]);

  const updatePdi = useCallback((partial: Partial<PdiCompletoTimeDTO>) => {
    setPdi((prev) => (prev ? { ...prev, ...partial } : null));
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return { pdi, loading, refetch: fetchData, updatePdi };
};

const ACCESS_DENIED_RH_MESSAGE = 'Acesso negado ao recurso de RH.';

export const useEstatisticasRH = () => {
  const [estatisticas, setEstatisticas] = useState<EstatisticasRH | null>(null);
  const [loading, setLoading] = useState(true);
  const [accessDenied, setAccessDenied] = useState(false);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }
    setAccessDenied(false);

    const fetchData = async () => {
      try {
        const useCase = container.resolve(ObterDashboardRHUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          const { retorno } = response;
          
          // Mapear para o formato esperado
          setEstatisticas({
            totalColaboradores: retorno.qtdTotalColaboradores,
            gestoresCom1a1EmDia: Math.round(retorno.porcentagemGestoresOneOnOneEmDia * 100), // Converter para porcentagem
            feedbackEmDia: Math.round(retorno.porcentagemGestoresFeedbackEmDia * 100), // Converter para porcentagem
            sem1a1Mais14d: retorno.qtdSemOneOnOneHaMaisQtdParametroDias,
            semFeedbackMais30d: retorno.qtdSemFeedbackHaMaisQtdParametroDias,
            semPdiAtivo: 0, // Não vem na API
          });
        }
      } catch (error) {
        const isAccessDenied = error instanceof Error && error.message === ACCESS_DENIED_RH_MESSAGE;
        setAccessDenied(isAccessDenied);
        if (!isAccessDenied) {
          console.error('Erro ao buscar estatísticas RH:', error);
        }
        // Em caso de erro, manter valores padrão
        setEstatisticas({
          totalColaboradores: 0,
          gestoresCom1a1EmDia: 0,
          feedbackEmDia: 0,
          sem1a1Mais14d: 0,
          semFeedbackMais30d: 0,
          semPdiAtivo: 0,
        });
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token]);

  return { estatisticas, loading, accessDenied };
};

/** Métricas PDI para tela RH — usa GET /api/GestaoPessoa/Pdi/Metricas/gestor (visão empresa quando usuário é RH). */
export const useMetricasPdiRH = () => {
  const [metricas, setMetricas] = useState<PdiMetricasResultDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const api = new ColaboradorPdiApi();
        const data = await api.getMetricasGestor(token);
        setMetricas(data ?? null);
      } catch {
        setMetricas(null);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token]);

  return { metricas, loading };
};

export const useEstatisticasPessoais = (codColaborador: string) => {
  const [estatisticas, setEstatisticas] = useState<EstatisticasPessoais | null>(null);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const useCase = container.resolve(ObterDashboardColaboradorUseCase);
        const response = await useCase.execute(token);
        
        if (response.sucesso && response.retorno) {
          const { meuPainel } = response.retorno;
          
          setEstatisticas({
            totalFeedbacks: meuPainel.qtdFeedbacks,
            totalRegistros1a1: meuPainel.qtdOneOnOne,
            totalPDIs: 0, // Não vem na API
          });
        }
      } catch (error) {
        console.error('Erro ao buscar estatísticas pessoais:', error);
        // Em caso de erro, manter valores padrão
        setEstatisticas({
          totalFeedbacks: 0,
          totalRegistros1a1: 0,
          totalPDIs: 0,
        });
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token, codColaborador]);

  return { estatisticas, loading };
};

export const useRegistrosCriticosPessoais = (codColaborador: string) => {
  const [registros, setRegistros] = useState<RegistroCritico[]>([]);
  const [loading, setLoading] = useState(true);
  const { token } = useAppSelector((state) => state.auth);

  useEffect(() => {
    if (!token || !codColaborador) {
      setLoading(false);
      return;
    }

    const fetchData = async () => {
      try {
        const useCase = container.resolve(ObterDashboardColaboradorUseCase);
        const response = await useCase.execute(token);

        if (response.sucesso && response.retorno) {
          const { registrosCriticos } = response.retorno;

          const registrosMapeados: RegistroCritico[] = registrosCriticos.map((item, index) => ({
            id: `critico-${index}`, // Gerar ID único
            colaboradorNome: '', // Não disponível neste endpoint para o próprio colaborador
            colaboradorCod: codColaborador,
            data: item.dataReuniao,
            descricao: item.descricaoAnotacoes,
          }));
          setRegistros(registrosMapeados);
        }
      } catch (error) {
        console.error('Erro ao buscar registros críticos pessoais:', error);
        setRegistros([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [token, codColaborador]);

  return { registros, loading };
};

