import { useAppSelector } from '@app/store/hooks';
import type { Colaborador } from '@domain/entities/Colaborador';
import type {
  Feedback360AvaliacaoItem,
  Feedback360CriarPayload,
  Feedback360Item,
  Feedback360RelacionamentoItem,
} from '@domain/entities/Feedback360';
import { AtualizarFeedback360UseCase } from '@domain/usecases/AtualizarFeedback360UseCase';
import { CriarFeedback360UseCase } from '@domain/usecases/CriarFeedback360UseCase';
import { GetColaboradoresUseCase } from '@domain/usecases/GetColaboradoresUseCase';
import { ListarAvaliacoesFeedback360UseCase } from '@domain/usecases/ListarAvaliacoesFeedback360UseCase';
import { ListarEnviadosFeedback360UseCase } from '@domain/usecases/ListarEnviadosFeedback360UseCase';
import { ListarRecebidosFeedback360UseCase } from '@domain/usecases/ListarRecebidosFeedback360UseCase';
import { ListarRelacionamentosFeedback360UseCase } from '@domain/usecases/ListarRelacionamentosFeedback360UseCase';
import { useCallback, useEffect, useRef, useState } from 'react';
import { toast } from 'sonner';
import { container } from 'tsyringe';

const COLAB_PAGE_SIZE = 50;
const COLAB_INITIAL_LIMIT = 100;

export function useFeedback360() {
  const token = useAppSelector((state) => state.auth.token);
  const orgId = useAppSelector((state) => state.auth.user?.colaboradorOrg?.orgId ?? 0);
  const listarAvaliacoesUseCase = container.resolve(ListarAvaliacoesFeedback360UseCase);
  const listarRelacionamentosUseCase = container.resolve(ListarRelacionamentosFeedback360UseCase);
  const listarEnviadosUseCase = container.resolve(ListarEnviadosFeedback360UseCase);
  const listarRecebidosUseCase = container.resolve(ListarRecebidosFeedback360UseCase);
  const criarFeedback360UseCase = container.resolve(CriarFeedback360UseCase);
  const atualizarFeedback360UseCase = container.resolve(AtualizarFeedback360UseCase);
  const getColaboradoresUseCase = container.resolve(GetColaboradoresUseCase);

  const [activeTab, setActiveTab] = useState('inserir');
  const [feedbacksEnviados, setFeedbacksEnviados] = useState<Feedback360Item[]>([]);
  const [feedbacksRecebidos, setFeedbacksRecebidos] = useState<Feedback360Item[]>([]);
  const [loadingList, setLoadingList] = useState(false);
  const [filtroData, setFiltroData] = useState('');
  const [filtroSentimento, setFiltroSentimento] = useState('');
  const [filtroRelacionamento, setFiltroRelacionamento] = useState('');
  const [filtroColaborador, setFiltroColaborador] = useState('');
  const [detalheItem, setDetalheItem] = useState<Feedback360Item | null>(null);
  const [itemEmEdicao, setItemEmEdicao] = useState<Feedback360Item | null>(null);

  const [avaliacoes, setAvaliacoes] = useState<Feedback360AvaliacaoItem[]>([]);
  const [relacionamentos, setRelacionamentos] = useState<Feedback360RelacionamentoItem[]>([]);
  const [loadingOpcoes, setLoadingOpcoes] = useState(false);

  const [colaboradores, setColaboradores] = useState<Colaborador[]>([]);
  const [loadingColab, setLoadingColab] = useState(false);
  const [hasMoreColab, setHasMoreColab] = useState(false);
  const [colabPopoverOpen, setColabPopoverOpen] = useState(false);
  const [buscaColab, setBuscaColab] = useState('');
  const [selectedCodigo, setSelectedCodigo] = useState('');
  const [contexto, setContexto] = useState('');
  const [dataInteracao, setDataInteracao] = useState('');
  const [feedback360AvaliacaoId, setFeedback360AvaliacaoId] = useState<number | null>(null);
  const [feedback360RelacionamentoId, setFeedback360RelacionamentoId] = useState<number | null>(null);
  const [comentario, setComentario] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, boolean>>({});


  const loadOpcoes = useCallback(async () => {
    if (!token || activeTab !== 'inserir') return;
    setLoadingOpcoes(true);
    try {
      const [avaliacoesRes, relacionamentosRes] = await Promise.all([
        listarAvaliacoesUseCase.execute(token),
        listarRelacionamentosUseCase.execute(token),
      ]);
      setAvaliacoes(Array.isArray(avaliacoesRes) ? avaliacoesRes : []);
      setRelacionamentos(Array.isArray(relacionamentosRes) ? relacionamentosRes : []);
    } catch {
      setAvaliacoes([]);
      setRelacionamentos([]);
    } finally {
      setLoadingOpcoes(false);
    }
  }, [token, activeTab, listarAvaliacoesUseCase, listarRelacionamentosUseCase]);

  useEffect(() => {
    loadOpcoes();
  }, [loadOpcoes]);

  const loadEnviados = useCallback(async () => {
    if (!token) return;
    try {
      const lista = await listarEnviadosUseCase.execute(token);
      setFeedbacksEnviados(Array.isArray(lista) ? lista : []);
    } catch {
      setFeedbacksEnviados([]);
    }
  }, [token, listarEnviadosUseCase]);

  const loadRecebidos = useCallback(async () => {
    if (!token) return;
    try {
      const lista = await listarRecebidosUseCase.execute(token);
      setFeedbacksRecebidos(Array.isArray(lista) ? lista : []);
    } catch {
      setFeedbacksRecebidos([]);
    }
  }, [token, listarRecebidosUseCase]);

  const loadListas = useCallback(async () => {
    if (!token) return;
    setLoadingList(true);
    try {
      await Promise.all([loadEnviados(), loadRecebidos()]);
    } finally {
      setLoadingList(false);
    }
  }, [token, loadEnviados, loadRecebidos]);

  useEffect(() => {
    if (activeTab === 'consultar' && token) loadListas();
  }, [activeTab, token, loadListas]);

  const clearColaboradores = useCallback(() => {
    setColaboradores([]);
    setHasMoreColab(false);
  }, []);

  const colabRequestIdRef = useRef(0);

  const UUID_REGEX = /[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i;

  const carregarColaboradores = useCallback(
    async (busca: string, cursor: number) => {
      if (!token || orgId <= 0) return;
      const requestId = ++colabRequestIdRef.current;
      const limit = cursor === 0 ? COLAB_INITIAL_LIMIT : COLAB_PAGE_SIZE;
      const buscaTrim = busca.trim();
      const nomeOuEmail = UUID_REGEX.test(buscaTrim) ? '' : buscaTrim;
      setLoadingColab(true);
      try {
        const res = await getColaboradoresUseCase.execute(token, {
          cursor,
          limite: limit,
          nomeOuEmail,
          org: orgId,
          fourtalents: true,
        });
        if (requestId !== colabRequestIdRef.current) return;
        const lista = (res?.retorno ?? []).filter((c) => c.ativo === true);
        setHasMoreColab(lista.length === limit);
        if (cursor === 0) {
          setColaboradores(lista);
        } else {
          setColaboradores((prev) => [...prev, ...lista]);
        }
      } catch {
        if (requestId !== colabRequestIdRef.current) return;
        if (cursor === 0) setColaboradores([]);
        setHasMoreColab(false);
      } finally {
        if (requestId === colabRequestIdRef.current) setLoadingColab(false);
      }
    },
    [token, orgId, getColaboradoresUseCase],
  );

  const selectedColab = colaboradores.find((c) => c.cpf === selectedCodigo);
  const displayColab =
    itemEmEdicao?.nomeDestinatario ??
    selectedColab?.nome ??
    selectedColab?.cpf ??
    '';

  const iniciarEdicao = useCallback((item: Feedback360Item) => {
    setItemEmEdicao(item);
    setActiveTab('inserir');
    setContexto(item.titulo ?? '');
    setComentario(item.descricao ?? '');
    setDataInteracao(item.dataInteracao?.slice(0, 10) ?? '');
    setFeedback360AvaliacaoId(item.feedback360AvaliacaoId ?? null);
    setFeedback360RelacionamentoId(item.feedback360RelacionamentoId ?? null);
    setSelectedCodigo(item.codigoInternoColaboradorDestinatario ?? '');
    setErrors({});
    setDetalheItem(null);
  }, []);

  const cancelarEdicao = useCallback(() => {
    setItemEmEdicao(null);
    setSelectedCodigo('');
    setBuscaColab('');
    setContexto('');
    setDataInteracao('');
    setFeedback360AvaliacaoId(null);
    setFeedback360RelacionamentoId(null);
    setComentario('');
    setErrors({});
  }, []);

  const handleSalvarAlteracoes = useCallback(async () => {
    if (!itemEmEdicao || !token) return;
    const newErrors: Record<string, boolean> = {};
    if (!contexto.trim()) newErrors.contexto = true;
    if (!dataInteracao) newErrors.data = true;
    if (feedback360AvaliacaoId == null) newErrors.avaliacao = true;
    if (feedback360RelacionamentoId == null) newErrors.relacionamento = true;
    if (!comentario.trim()) newErrors.comentario = true;
    setErrors(newErrors);
    if (Object.keys(newErrors).length > 0) {
      toast.error('Preencha todos os campos obrigatórios.');
      return;
    }
    if (feedback360AvaliacaoId == null || feedback360RelacionamentoId == null) return;
    setSubmitting(true);
    try {
      await atualizarFeedback360UseCase.execute(token, itemEmEdicao.id, {
        titulo: contexto.trim(),
        descricao: comentario.trim(),
        dataInteracao,
        feedback360RelacionamentoId,
        feedback360AvaliacaoId,
      });
      toast.success('Reconhecimento atualizado com sucesso.');
      setItemEmEdicao(null);
      setSelectedCodigo('');
      setBuscaColab('');
      setContexto('');
      setDataInteracao('');
      setFeedback360AvaliacaoId(null);
      setFeedback360RelacionamentoId(null);
      setComentario('');
      setErrors({});
      setActiveTab('consultar');
      loadListas();
    } catch {
      toast.error('Erro ao atualizar Reconhecimento.');
    } finally {
      setSubmitting(false);
    }
  }, [
    itemEmEdicao,
    token,
    contexto,
    dataInteracao,
    feedback360AvaliacaoId,
    feedback360RelacionamentoId,
    comentario,
    atualizarFeedback360UseCase,
    loadListas,
  ]);

  const handleRegistrar = useCallback(async () => {
    const newErrors: Record<string, boolean> = {};
    if (!selectedCodigo) newErrors.colaborador = true;
    if (!contexto.trim()) newErrors.contexto = true;
    if (!dataInteracao) newErrors.data = true;
    if (feedback360AvaliacaoId == null) newErrors.avaliacao = true;
    if (feedback360RelacionamentoId == null) newErrors.relacionamento = true;
    if (!comentario.trim()) newErrors.comentario = true;
    setErrors(newErrors);
    if (Object.keys(newErrors).length > 0) {
      toast.error('Preencha todos os campos obrigatórios.');
      return;
    }
    if (!token || !selectedColab || feedback360AvaliacaoId == null || feedback360RelacionamentoId == null) return;
    setSubmitting(true);
    try {
      const payload: Feedback360CriarPayload = {
        codigoInternoColaboradorDestinatario: selectedCodigo,
        titulo: contexto.trim(),
        descricao: comentario.trim(),
        dataInteracao,
        feedback360RelacionamentoId,
        feedback360AvaliacaoId,
      };
      await criarFeedback360UseCase.execute(token, payload);
      toast.success('Reconhecimento registrado com sucesso.');
      setSelectedCodigo('');
      setBuscaColab('');
      setContexto('');
      setDataInteracao('');
      setFeedback360AvaliacaoId(null);
      setFeedback360RelacionamentoId(null);
      setComentario('');
      setErrors({});
      setActiveTab('consultar');
      loadListas();
    } catch {
      toast.error('Erro ao registrar Reconhecimento.');
    } finally {
      setSubmitting(false);
    }
  }, [
    token,
    selectedCodigo,
    selectedColab,
    contexto,
    dataInteracao,
    feedback360AvaliacaoId,
    feedback360RelacionamentoId,
    comentario,
    criarFeedback360UseCase,
    loadListas,
  ]);

  return {
    activeTab,
    setActiveTab,
    feedbacksEnviados,
    feedbacksRecebidos,
    loadingList,
    loadListas,
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
    clearColaboradores,
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
  };
}
