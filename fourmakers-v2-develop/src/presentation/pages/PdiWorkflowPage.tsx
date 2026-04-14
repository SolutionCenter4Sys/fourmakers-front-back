import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Input } from '@/components/ui/input';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '@/components/ui/dialog';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { cn } from '@/lib/utils';
import { ArrowLeft, Calendar, User, Star, CheckCircle, Edit, Trash2, Upload, FileText } from '@/components/ui/system-icons';
import { usePdiCompletoTime } from '@/presentation/hooks/useGestaoDesempenho';
import { ColaboradorPdiApi } from '@data/api/ColaboradorPdiApi';
import type { PdiActionPlanInputDTO, PdiActionPlanDTO } from '@shared/types/pdiApi';
import { mapPdiStatusToLabel, PdiStatusLabel, PdiStatusApi } from '@shared/types/pdiApi';
import { format, parseISO } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import avatarIan from '@/assets/avatar.png';
import parabensImg from '@/assets/parabens.png';

const WORKFLOW_STEPS = [
  'Criar o PDI',
  'Definição de planos',
  'Aprovação gestor',
  'Planos em andamento',
  'Planos concluídos',
];

const api = new ColaboradorPdiApi();

/** Extrai data civil yyyy-MM-dd de string ISO ou yyyy-MM-dd (evita timezone na exibição). */
function toDateOnly(isoOrYyyyMmDd: string | null | undefined): string {
  if (!isoOrYyyyMmDd?.trim()) return '';
  return isoOrYyyyMmDd.trim().slice(0, 10);
}

/** Backend PUT/DELETE ActionPlans implementados; habilitado para gestor e colaborador. */
const EDIT_DELETE_ACTION_PLAN_ENABLED = true;

export default function PdiWorkflowPage() {
  const { codColaborador, pdiId } = useParams<{ codColaborador?: string; pdiId: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const pathname = location.pathname;
  const isRotaRh = pathname.startsWith('/gestao-desempenho-rh/');
  const { token, user } = useAppSelector((state) => state.auth);
  const state = (location.state || {}) as { colaboradorNome?: string; from?: string; fromCriacao?: boolean; isMeuPdi?: boolean; criadoPorGestor?: boolean; openStep?: number; apenasVisualizar?: boolean; previsaoConclusao?: string };
  // Rota /pdi/:pdiId (sem codColaborador) = "meu PDI"; rota gestao-desempenho-gestor ou gestao-desempenho-rh/colaborador = PDI do time
  const isMeuPdi = state.isMeuPdi ?? !codColaborador;
  const apenasVisualizar = isRotaRh ? true : (state.apenasVisualizar ?? false);
  const isOwnerPdi = !!codColaborador && user?.colaboradorOrg?.codColaborador === codColaborador;
  const criadoPorGestor = state.criadoPorGestor ?? false;
  const openStep = state.openStep; // 2 = definição de planos, 4 = atualizar andamento (concluir)
  const colaboradorNome = isMeuPdi ? 'Você' : (state.colaboradorNome ?? '');
  const nomeBanner = isMeuPdi ? (user?.nomeColaborador?.trim().split(/\s+/)[0] || 'você') : (state.colaboradorNome ?? 'você');
  const rotaOrigem = isRotaRh ? '/gestao-desempenho-rh' : (state.from ?? (isMeuPdi ? '/gestao-desempenho-colaborador' : '/gestao-desempenho-gestor'));
  const fromCriacao = state.fromCriacao ?? false;
  /** Quando gestor edita PDI de outro colaborador, enviar colaboradorId nas chamadas da API (rotas PdisDoTime/colaborador/{id}/pdi/...) */
  const colaboradorIdForApi = (isMeuPdi || isOwnerPdi) ? undefined : (codColaborador ?? undefined);

  const { pdi, loading, refetch, updatePdi } = usePdiCompletoTime(pdiId || '');
  /** Para atualizar planejamento (planos de ação) sempre usamos o endpoint do gestor; este é o dono do PDI. */
  const colaboradorIdParaPlanejamento = codColaborador ?? pdi?.colaboradorId ?? '';
  const [novoPlanoDescricao, setNovoPlanoDescricao] = useState('');
  const [novoPlanoDeadline, setNovoPlanoDeadline] = useState('');
  const [salvandoPlano, setSalvandoPlano] = useState(false);
  const [concluindoPlanId, setConcluindoPlanId] = useState<string | null>(null);
  const [modalConcluirTarefaOpen, setModalConcluirTarefaOpen] = useState(false);
  const [planIdParaConcluir, setPlanIdParaConcluir] = useState<string | null>(null);
  const [modalParabensOpen, setModalParabensOpen] = useState(false);
  const [modalFinalizarPdiOpen, setModalFinalizarPdiOpen] = useState(false);
  const [tipoAnexo, setTipoAnexo] = useState<'EVIDENCIA' | 'CERTIFICADO' | 'OUTRO'>('EVIDENCIA');
  const [arquivoAnexo, setArquivoAnexo] = useState<File | null>(null);
  const [linkAnexo, setLinkAnexo] = useState('');
  const [enviandoAnexo, setEnviandoAnexo] = useState(false);
  const [modalConfirmarAnexoOpen, setModalConfirmarAnexoOpen] = useState(false);
  const [finalizandoSemAnexo, setFinalizandoSemAnexo] = useState(false);
  const [modalCancelarPdiOpen, setModalCancelarPdiOpen] = useState(false);
  const [cancelandoPdi, setCancelandoPdi] = useState(false);
  const [forceStep5, setForceStep5] = useState(false);
  const openedModalStep5Ref = useRef(false);
  const [excluindoPlanId, setExcluindoPlanId] = useState<string | null>(null);
  const [editandoPlanId, setEditandoPlanId] = useState<string | null>(null);
  const [modalEditarPlanoOpen, setModalEditarPlanoOpen] = useState(false);
  const [planEditForm, setPlanEditForm] = useState<{ id: string; description: string; deadline: string }>({ id: '', description: '', deadline: '' });
  const [modalExcluirPlanoOpen, setModalExcluirPlanoOpen] = useState(false);
  const [planIdParaExcluir, setPlanIdParaExcluir] = useState<string | null>(null);

  useEffect(() => {
    if (!pdi?.actionPlans?.length) return;
    const allConcluidos = pdi.actionPlans.every((ap) => !!ap.concluidoEm);
    if (allConcluidos && forceStep5) setForceStep5(false);
  }, [pdi?.actionPlans, forceStep5]);

  useEffect(() => {
    if (!pdi?.actionPlans?.length) return;
    const allDone = pdi.actionPlans!.length > 0 && pdi.actionPlans!.every((ap) => !!ap.concluidoEm);
    const isStep5Reached = forceStep5 || allDone;
    if (isStep5Reached && !apenasVisualizar) {
      if (!openedModalStep5Ref.current) {
        openedModalStep5Ref.current = true;
        setModalParabensOpen(true);
      }
    } else {
      openedModalStep5Ref.current = false;
    }
  }, [pdi?.actionPlans, forceStep5, apenasVisualizar]);

  const handleVoltar = () => {
    if (isMeuPdi) {
      navigate('/gestao-desempenho-colaborador', { state: { openTab: 'pdi' } });
      return;
    }
    if (isRotaRh && codColaborador) {
      navigate(`/gestao-desempenho-rh/colaborador/${codColaborador}`, { state: { from: rotaOrigem, openTab: 'pdi' } });
      return;
    }
    navigate(`/gestao-desempenho-gestor/${codColaborador}`, { state: { from: rotaOrigem, openTab: 'pdi' } });
  };

  const handleCancelarPdi = async () => {
    if (!token || !pdiId || !pdi) return;
    setCancelandoPdi(true);
    try {
      await api.atualizarPdi(
        token,
        pdiId,
        { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.CANCELLED },
        colaboradorIdForApi,
      );
      toast.success('PDI cancelado.');
      setModalCancelarPdiOpen(false);
      navigate(rotaOrigem);
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao cancelar PDI.');
    } finally {
      setCancelandoPdi(false);
    }
  };

  const handleAdicionarPlano = async () => {
    if (!token || !pdiId || !pdi || !novoPlanoDescricao.trim()) {
      toast.error('Preencha a descrição do plano de ação.');
      return;
    }
    if (!novoPlanoDeadline?.trim()) {
      toast.error('Selecione a previsão para concluir esta ação.');
      return;
    }
    const fromPdiDeadLine = toDateOnly(pdi.deadLine);
    const maxPrevisaoStr = fromPdiDeadLine
      ? fromPdiDeadLine
      : pdi.actionPlans?.length && pdi.actionPlans.some((ap) => ap.deadline)
        ? (() => {
            const dates = pdi.actionPlans!
              .filter((ap) => ap.deadline)
              .map((ap) => toDateOnly(ap.deadline!))
              .filter(Boolean);
            if (dates.length) return dates.sort((a, b) => b.localeCompare(a))[0] ?? '';
            return toDateOnly(state.previsaoConclusao) || '';
          })()
        : toDateOnly(state.previsaoConclusao) || '';
    if (novoPlanoDeadline && maxPrevisaoStr && novoPlanoDeadline > maxPrevisaoStr) {
      toast.error(
        `A data de previsão do plano não pode ser superior à previsão de conclusão do PDI (${format(parseISO(maxPrevisaoStr), 'dd/MM/yyyy', { locale: ptBR })}).`
      );
      return;
    }
    setSalvandoPlano(true);
    try {
      const deadlinePayload = novoPlanoDeadline ? `${novoPlanoDeadline}T00:00:00.000Z` : undefined;
      const payload: PdiActionPlanInputDTO = {
        description: novoPlanoDescricao.trim(),
        deadline: deadlinePayload,
      };
      await api.addActionPlan(token, pdiId, payload, colaboradorIdForApi);
      toast.success('Plano de ação adicionado.');
      setNovoPlanoDescricao('');
      setNovoPlanoDeadline('');
      refetch();
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao adicionar plano de ação.');
    } finally {
      setSalvandoPlano(false);
    }
  };

  const openConfirmarConcluirTarefa = (planId: string) => {
    setPlanIdParaConcluir(planId);
    setModalConcluirTarefaOpen(true);
  };

  const handleConfirmarConcluirTarefa = async () => {
    if (!token || !pdiId || !planIdParaConcluir || !pdi) return;
    const plans = pdi.actionPlans ?? [];
    const incompleteCount = plans.filter((ap) => !ap.concluidoEm).length;
    const wasLastPlan = incompleteCount === 1;
    setConcluindoPlanId(planIdParaConcluir);
    try {
      await api.completeActionPlan(token, pdiId, planIdParaConcluir, colaboradorIdForApi);
      toast.success('Tarefa concluída.');
      if (wasLastPlan) {
        setForceStep5(true);
      }
      // Para PDIs criados pelo gestor, o status "Em andamento" só acontece após aprovação do gestor.
      // Para meu PDI (sem aprovação), manter o comportamento de colocar em IN_PROGRESS ao começar a concluir tarefas.
      if (!(criadoPorGestor && isMeuPdi)) {
        try {
          await api.atualizarPdi(
            token,
            pdiId,
            { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.IN_PROGRESS },
            colaboradorIdForApi,
          );
        } catch {
          // Ignora falha do PUT; o refetch pode ainda trazer status atualizado pelo backend
        }
      }
      await refetch();
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao concluir tarefa.');
    } finally {
      setConcluindoPlanId(null);
      setPlanIdParaConcluir(null);
      setModalConcluirTarefaOpen(false);
    }
  };

  const handleEditarPlano = (plan: PdiActionPlanDTO) => {
    setPlanEditForm({
      id: plan.id,
      description: plan.description,
      deadline: plan.deadline ? plan.deadline.slice(0, 10) : '',
    });
    setModalEditarPlanoOpen(true);
  };

  const handleSalvarEdicaoPlano = async () => {
    if (!token || !pdiId || !planEditForm.id || !planEditForm.description.trim()) return;
    setEditandoPlanId(planEditForm.id);
    const deadline = planEditForm.deadline ? `${planEditForm.deadline}T00:00:00.000Z` : undefined;
    try {
      await api.updateActionPlan(
        token,
        pdiId,
        planEditForm.id,
        { description: planEditForm.description.trim(), deadline },
        colaboradorIdParaPlanejamento,
      );
      toast.success('Plano de ação atualizado.');
      setModalEditarPlanoOpen(false);
      setPlanEditForm({ id: '', description: '', deadline: '' });
      await refetch();
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao atualizar plano.');
    } finally {
      setEditandoPlanId(null);
    }
  };

  const handleExcluirPlano = (planId: string) => {
    setPlanIdParaExcluir(planId);
    setModalExcluirPlanoOpen(true);
  };

  const handleConfirmarExcluirPlano = async () => {
    if (!token || !pdiId || !planIdParaExcluir) return;
    setExcluindoPlanId(planIdParaExcluir);
    try {
      await api.deleteActionPlan(token, pdiId, planIdParaExcluir, colaboradorIdParaPlanejamento);
      toast.success('Plano de ação excluído.');
      setModalExcluirPlanoOpen(false);
      setPlanIdParaExcluir(null);
      await refetch();
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao excluir plano.');
    } finally {
      setExcluindoPlanId(null);
    }
  };

  const handleAbrirModalFinalizar = () => {
    setModalFinalizarPdiOpen(true);
    setTipoAnexo('EVIDENCIA');
    setArquivoAnexo(null);
    setLinkAnexo('');
  };

  const handleEntendiParabens = () => {
    setModalParabensOpen(false);
    setModalFinalizarPdiOpen(true);
  };

  const [abrirEvidenciaId, setAbrirEvidenciaId] = useState<string | null>(null);
  const handleVisualizarEvidencia = async (ev: { id: string; docName: string }) => {
    if (!token || !pdiId) return;
    setAbrirEvidenciaId(ev.id);
    try {
      const response = await api.getEvidenciaFile(token, pdiId, ev.id, colaboradorIdForApi);
      const blob = await response.blob();
      const url = URL.createObjectURL(blob);
      window.open(url, '_blank', 'noopener,noreferrer');
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Não foi possível abrir o documento.');
    } finally {
      setAbrirEvidenciaId(null);
    }
  };

  const handleConfirmarEnvioAnexo = () => {
    setModalConfirmarAnexoOpen(true);
  };

  const handleFinalizarSemAnexo = async () => {
    if (!token || !pdiId || !pdi) return;
    setFinalizandoSemAnexo(true);
    try {
      await api.atualizarPdi(
        token,
        pdiId,
        { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.COMPLETED },
        colaboradorIdForApi,
      );
      toast.success('PDI finalizado.');
      setModalFinalizarPdiOpen(false);
      refetch();
      if (isMeuPdi) {
        navigate('/gestao-desempenho-colaborador', { state: { openTab: 'pdi' } });
      } else if (rotaOrigem === '/gestao-desempenho-rh' && codColaborador) {
        navigate(`/gestao-desempenho-rh/colaborador/${codColaborador}`, { state: { from: rotaOrigem, openTab: 'pdi' } });
      } else {
        navigate(`/gestao-desempenho-gestor/${codColaborador}`, { state: { from: rotaOrigem, openTab: 'pdi' } });
      }
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao finalizar PDI.');
    } finally {
      setFinalizandoSemAnexo(false);
    }
  };

  const handleEnviarAnexo = async () => {
    if (!token || !pdiId) return;
    if (!arquivoAnexo) {
      toast.error('Selecione um arquivo para anexar.');
      return;
    }
    setEnviandoAnexo(true);
    try {
      const formData = new FormData();
      formData.append('arquivo', arquivoAnexo);
      formData.append('tipo', tipoAnexo);
      await api.uploadEvidencia(token, pdiId, formData, colaboradorIdForApi);
      toast.success('Documento anexado ao PDI.');
      setModalFinalizarPdiOpen(false);
      setModalConfirmarAnexoOpen(false);
      setArquivoAnexo(null);
      setLinkAnexo('');
      refetch();
      if (isMeuPdi) {
        navigate('/gestao-desempenho-colaborador', { state: { openTab: 'pdi' } });
      } else if (rotaOrigem === '/gestao-desempenho-rh' && codColaborador) {
        navigate(`/gestao-desempenho-rh/colaborador/${codColaborador}`, { state: { from: rotaOrigem, openTab: 'pdi' } });
      } else {
        navigate(`/gestao-desempenho-gestor/${codColaborador}`, { state: { from: rotaOrigem, openTab: 'pdi' } });
      }
    } catch (e: unknown) {
      toast.error(e instanceof Error ? e.message : 'Erro ao enviar anexo.');
    } finally {
      setEnviandoAnexo(false);
    }
  };

  if (loading && !pdi) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-12 text-muted-foreground">Carregando PDI...</div>
      </div>
    );
  }

  if (!pdi) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-12 text-destructive">PDI não encontrado.</div>
        <Button variant="outline" onClick={handleVoltar}>
          <ArrowLeft className="h-4 w-4 mr-2" />
          Voltar
        </Button>
      </div>
    );
  }

  const progressPercent = Math.round((pdi.progress ?? 0) * 100);
  const fromPdiDeadLine = toDateOnly(pdi.deadLine);
  const previsaoConclusao: string | null = fromPdiDeadLine
    ? fromPdiDeadLine
    : pdi.actionPlans?.length
      ? (() => {
          const dates = pdi.actionPlans
            .filter((ap) => ap.deadline)
            .map((ap) => toDateOnly(ap.deadline!))
            .filter(Boolean);
          if (dates.length) return dates.sort((a, b) => b.localeCompare(a))[0] ?? null;
          return toDateOnly(state.previsaoConclusao) || null;
        })()
      : toDateOnly(state.previsaoConclusao) || null;

  const dataMaxPrevisao = previsaoConclusao ?? undefined;

  const actionPlans = pdi.actionPlans ?? [];
  const totalPlans = actionPlans.length;
  const allPlansConcluidos = totalPlans > 0 && actionPlans.every((ap) => !!ap.concluidoEm);
  const statusLabel = mapPdiStatusToLabel(pdi.status);

  /** Gestor que criou este PDI pode editar/excluir os planos de ação (quando API não envia codigoInterno no plano, usa o do PDI). */
  const gestorCriouEstePdi =
    !!codColaborador &&
    !isOwnerPdi &&
    !apenasVisualizar &&
    (fromCriacao || pdi?.codigoInternoColaboradorCriacao === user?.cpf);

  /** Gestor na rota do colaborador pode editar/excluir qualquer plano (backend permite). Colaborador ou dono: só edita planos que criou. Se o gestor criou o PDI, pode editar/excluir todos os planos. */
  const podeEditarEstePlano = (plan: PdiActionPlanDTO): boolean => {
    if (!EDIT_DELETE_ACTION_PLAN_ENABLED) return false;
    if (apenasVisualizar) return false;
    
    const criadoPeloColaborador = plan.codigoInternoColaboradorCriacao === pdi?.colaboradorId;
    const criadoPeloGestorAtual = !!codColaborador && !isOwnerPdi && plan.codigoInternoColaboradorCriacao === user?.cpf;
    return (criadoPeloColaborador && (isMeuPdi || isOwnerPdi)) || criadoPeloGestorAtual || gestorCriouEstePdi;
  };

  // Aprovação do gestor: só na rota do gestor vendo PDI de outro. Em "meu PDI" (/pdi/:id) nunca mostramos etapa 3; em /gestao-desempenho-gestor/:id/pdi/:id só mostramos se não for o próprio gestor.
  const requerAprovacaoGestor = !!codColaborador && !isOwnerPdi;

  // Step único por status + planos: mesmo PDI mostra o mesmo step na visão gestor e colaborador.
  // openStep só é aplicado quando não contradiz o status (ex.: não forçar step 2 se o PDI já está IN_PROGRESS).
  const stepIndex: number = (() => {
    // Só fixar na definição (step 2) quando veio da criação e ainda está não iniciado; após "Concluir definição" (IN_PROGRESS) ir para step 4
    if (fromCriacao && statusLabel === PdiStatusLabel.NAO_INICIADO) return 1;
    if (isMeuPdi) {
      // PDI criado pelo gestor: após "Concluir planejamento" (EM_ANALISE), colaborador vê step 3 (aguardando aprovação)
      if (criadoPorGestor && statusLabel === PdiStatusLabel.EM_ANALISE) return 2;
      if (requerAprovacaoGestor) {
        if (forceStep5 || allPlansConcluidos) return 4;
        // Status tem prioridade sobre openStep: após enviar para aprovação (EM_ANALISE) ou já em andamento (IN_PROGRESS), ir para o step correto
        if (statusLabel === PdiStatusLabel.EM_ANALISE) return 2; // Step 3 - Aprovação do gestor
        if (statusLabel === PdiStatusLabel.IN_PROGRESS) return 3; // Step 4 - Planos em andamento
        if (openStep === 2) return 1;
        if (openStep === 4) return 3;
        if (totalPlans === 0 || statusLabel === PdiStatusLabel.NAO_INICIADO) return 1;
        return 1;
      }
      if (forceStep5 || allPlansConcluidos) return 4; // Step 5 - anexar evidência (prioridade: já concluídos ou conclusão otimista)
      // Status tem prioridade sobre openStep: após "Concluir definição e ir para gestão" (IN_PROGRESS), ir para step 4
      if (statusLabel === PdiStatusLabel.IN_PROGRESS) return 3; // Step 4 - Planos em andamento
      if (openStep === 2) return 1; // Editar: abrir na definição de planos
      if (openStep === 4) return 3; // Atualizar andamento: abrir no step 4 (concluir tarefas)
      if (totalPlans === 0) return 1; // Step 2 - Definição de planos
      return 3; // Step 4 - Planos em andamento (concluir)
    }
    // Gestor: steps dirigidos por status, com step 5 quando todos os planos estão concluídos
    if (forceStep5 || allPlansConcluidos || statusLabel === PdiStatusLabel.FINALIZADO) return 4;
    if (statusLabel === PdiStatusLabel.EM_ANALISE && !isOwnerPdi) return 2;
    if (statusLabel === PdiStatusLabel.EM_ANALISE && isOwnerPdi) return 3;
    if (statusLabel === PdiStatusLabel.IN_PROGRESS) return 3;
    // Não iniciado / ajuste solicitado
    if (statusLabel === PdiStatusLabel.NAO_INICIADO) return 1;
    if (statusLabel === PdiStatusLabel.CANCELADO) return 2;
    // fallback (mantém coerência visual)
    return Math.max(1, progressPercent >= 100 ? 4 : progressPercent >= 75 ? 3 : progressPercent >= 50 ? 2 : 1);
  })();

  const showStep2Content = stepIndex === 1;
  const showStep3Content = (requerAprovacaoGestor || (isMeuPdi && criadoPorGestor)) && stepIndex === 2;
  const showStep4Content = stepIndex === 3;
  const showStep5Content = stepIndex === 4;
  const isStep5 = stepIndex === 4;

  const emStep2OuStep4 = stepIndex === 1 || stepIndex === 3;
  const sempreMostrarEditarExcluir = emStep2OuStep4 && !apenasVisualizar && (isMeuPdi || isOwnerPdi || !!codColaborador);

  /** Usado no stepper para evitar TS2367 (comparison literal 1 e 4): comparação sempre number === number */
  const currentStepIndex: number = stepIndex;

  return (
    <div className="container mx-auto p-4 space-y-6">
      <Button variant="ghost" onClick={handleVoltar} className="mb-2">
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar
      </Button>

      {/* Banner — texto diferente quando criado por gestor vs colaborador; gestor vê mensagem de acompanhamento */}
      <div
        className="relative overflow-hidden rounded-2xl h-[200px] flex items-center gap-4 sm:gap-6 p-0 pr-6 text-white"
        style={{
          backgroundImage: 'linear-gradient(135deg, rgba(15, 118, 110, 0.95) 0%, rgba(88, 28, 135, 0.9) 45%, rgba(49, 46, 129, 0.95) 100%), url(/profile-hero-bg.jpg)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        <div className="self-end">
          <img src={avatarIan} alt="Ian" className="h-32 sm:h-40 w-auto object-contain object-bottom block" />
        </div>
        <div className="flex-1 min-w-0 flex items-center py-4">
          <div className="rounded-xl bg-indigo-900/80 backdrop-blur-sm border border-white/10 shadow-lg px-5 py-4 text-left max-w-xl">
            {!isMeuPdi ? (
              <p className="text-lg sm:text-xl font-medium leading-snug text-white">
                <span className="font-bold text-white bg-white/20 px-1.5 py-0.5 rounded">Acompanhe o PDI</span>
                <span className="text-white/95"> de {nomeBanner}. Meta: {pdi.titulo || '—'}. Conclua as etapas conforme o andamento.</span>
              </p>
            ) : criadoPorGestor ? (
              <p className="text-lg sm:text-xl font-medium leading-snug text-white">
                <span className="font-bold text-white bg-white/20 px-1.5 py-0.5 rounded">Olá, {nomeBanner}!</span>
                <span className="text-white/95"> A sua liderança criou uma meta desafiadora: {pdi.titulo || 'esta meta'}. Agora bora criar os planos de ação para atingir o objetivo.</span>
              </p>
            ) : (
              <p className="text-lg sm:text-xl font-medium leading-snug text-white">
                <span className="font-bold text-white bg-white/20 px-1.5 py-0.5 rounded">Oi, {nomeBanner}</span>
                <span className="text-white/95">. Sou o Ian e vou te acompanhar no seu PDI, vamos juntos?</span>
              </p>
            )}
          </div>
        </div>
      </div>

      {/* Stepper — UI/UX: design system (border-borderSoft, rounded-lg, hierarquia clara, acessibilidade) */}
      <nav
        aria-label="Etapas do PDI"
        className="rounded-lg border border-borderSoft bg-surfaceElevated p-4 shadow-softToken transition-all w-full"
      >
        <ol className="grid grid-cols-5 gap-2 w-full" role="list">
          {WORKFLOW_STEPS.map((label, i) => {
            const isActive = i === currentStepIndex;
            const isSkipped = !requerAprovacaoGestor && i === 2;
            const isPast = !isSkipped && i < currentStepIndex;
            const stepLabel = isSkipped ? 'Aprovação (não se aplica)' : label;
            return (
              <li
                key={label}
                className="min-w-0 flex items-stretch"
                aria-current={isActive ? 'step' : undefined}
              >
                <div
                  className={cn(
                    'flex flex-1 min-w-0 items-center gap-2 rounded-lg border px-2 sm:px-3 py-2 transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2',
                    isActive &&
                      'border-primary bg-primary text-primary-foreground shadow-softToken',
                    isPast &&
                      'border-borderSoft bg-secondaryBackground text-secondaryText',
                    isSkipped &&
                      'border-borderSoft bg-surfaceSubtle text-muted-foreground opacity-60',
                    !isActive && !isPast && !isSkipped &&
                      'border-borderSoft bg-secondaryBackground text-secondaryText',
                  )}
                  title={isSkipped ? 'Não se aplica (meu PDI ou gestor vendo próprio PDI)' : stepLabel}
                >
                  <span
                    className={cn(
                      'flex h-6 w-6 shrink-0 items-center justify-center rounded-full text-[10px] font-bold uppercase',
                      isActive && 'bg-primary-foreground/20 text-primary-foreground',
                      isPast && 'bg-success/20 text-success',
                      isSkipped && 'bg-muted-foreground/20',
                      !isActive && !isPast && !isSkipped && 'bg-borderSoft text-secondaryText',
                    )}
                    aria-hidden
                  >
                    {isPast ? <CheckCircle className="h-3.5 w-3.5" /> : i + 1}
                  </span>
                  <span className="truncate text-xs font-semibold">{stepLabel}</span>
                </div>
              </li>
            );
          })}
        </ol>
      </nav>

      {/* Status Finalizado/Cancelado — exibido ao visualizar PDI do histórico */}
      {(statusLabel === PdiStatusLabel.FINALIZADO || statusLabel === PdiStatusLabel.CANCELADO) && (
        <Card
          className={
            statusLabel === PdiStatusLabel.FINALIZADO
              ? 'border-2 border-green-200 bg-green-50/50 dark:bg-green-950/20 dark:border-green-800'
              : 'border-2 border-slate-200 bg-slate-50/50 dark:bg-slate-900/30 dark:border-slate-700'
          }
        >
          <CardContent className="p-4 flex items-center gap-3">
            <span className="text-sm font-medium text-muted-foreground">Status do PDI:</span>
            <Badge
              variant="secondary"
              className={
                statusLabel === PdiStatusLabel.FINALIZADO
                  ? 'bg-green-100 text-green-800 border-green-300 dark:bg-green-900/50 dark:text-green-300 dark:border-green-700'
                  : 'bg-slate-100 text-slate-700 border-slate-300 dark:bg-slate-800 dark:text-slate-300 dark:border-slate-600'
              }
            >
              {statusLabel}
            </Badge>
          </CardContent>
        </Card>
      )}

      {/* Cards: Colaborador 100% | Meta 25% | Previsão 25% | Aprimoramento 50% (grid 12 cols: 12 | 3 | 3 | 6) */}
      <div className="grid grid-cols-1 md:grid-cols-12 gap-4">
        {!isMeuPdi && (
          <Card className="md:col-span-12 border-2 border-borderSoft bg-surfaceElevated shadow-softToken">
            <CardContent className="p-5 flex items-center gap-4">
              <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-muted ring-2 ring-borderSoft">
                <User className="h-6 w-6 text-muted-foreground" />
              </div>
              <div className="min-w-0 flex-1">
                <p className="text-xs font-bold uppercase tracking-wide text-muted-foreground">Colaborador</p>
                <p className="mt-1 text-lg font-bold text-foreground break-words">{colaboradorNome || '—'}</p>
              </div>
            </CardContent>
          </Card>
        )}
        <Card className="md:col-span-3 border-2 border-primary/40 bg-gradient-to-br from-primary/5 to-transparent shadow-softToken overflow-hidden transition-all hover:shadow-cardHoverToken hover:border-primary/60">
          <CardContent className="p-5 flex items-center gap-4">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/20 ring-2 ring-primary/30">
              <Star className="h-6 w-6 text-primary" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-bold uppercase tracking-wide text-muted-foreground">Meta</p>
              <p className="mt-1 text-lg font-bold text-foreground leading-tight break-words">{pdi.titulo || '—'}</p>
            </div>
          </CardContent>
        </Card>
        <Card className="md:col-span-3 border-2 border-primary/40 bg-gradient-to-br from-primary/5 to-transparent shadow-softToken overflow-hidden transition-all hover:shadow-cardHoverToken hover:border-primary/60">
          <CardContent className="p-5 flex items-center gap-4">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/20 ring-2 ring-primary/30">
              <Calendar className="h-6 w-6 text-primary" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-bold uppercase tracking-wide text-muted-foreground">Previsão conclusão</p>
              <p className="mt-1 text-lg font-bold text-foreground">
                {previsaoConclusao ? format(parseISO(previsaoConclusao), 'dd/MM/yyyy', { locale: ptBR }) : '—'}
              </p>
            </div>
          </CardContent>
        </Card>
        {pdi.skills?.length > 0 && (
          <Card className="md:col-span-6 border-2 border-primary/30 bg-gradient-to-r from-primary/8 via-primary/5 to-transparent shadow-softToken overflow-hidden">
            <CardContent className="p-5 flex items-center gap-4">
              <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/20 ring-2 ring-primary/30">
                <Star className="h-6 w-6 text-primary" />
              </div>
              <div className="min-w-0 flex-1">
                <p className="text-xs font-bold uppercase tracking-wide text-muted-foreground">Aprimoramento de competências</p>
                <div className="mt-1 flex flex-wrap gap-2">
                  {pdi.skills.map((s) => (
                    <Badge
                      key={s.id}
                      className="rounded-lg px-3 py-2 text-sm font-bold bg-primary/20 text-primary border-2 border-primary/50 dark:bg-primary/25 dark:border-primary/50 shadow-sm hover:bg-primary/30 hover:scale-105 transition-all"
                    >
                      {s.nomeSkill}
                    </Badge>
                  ))}
                </div>
              </div>
            </CardContent>
          </Card>
        )}
        <Card className="md:col-span-12 border-2 border-primary/40 bg-gradient-to-br from-primary/5 to-transparent shadow-softToken overflow-hidden transition-all hover:shadow-cardHoverToken hover:border-primary/60">
          <CardContent className="p-5 flex items-start gap-4">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/20 ring-2 ring-primary/30">
              <FileText className="h-6 w-6 text-primary" />
            </div>
            <div className="min-w-0 flex-1">
              <p className="text-xs font-bold uppercase tracking-wide text-muted-foreground">Descrição</p>
              <p className="mt-1 text-sm text-foreground whitespace-pre-wrap break-words">{pdi.descricao || '—'}</p>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Step 2: Definição de planos — colaborador ou gestor podem adicionar planos de ação */}
      {showStep2Content && (
        <>
          {!apenasVisualizar && (isMeuPdi || isOwnerPdi || !!codColaborador) && (
            <Card>
              <CardContent className="p-6 space-y-4">
                <h3 className="font-semibold">{isMeuPdi ? 'Descreva seus planos de ação' : 'Adicionar planos de ação'}</h3>
                <Textarea
                  placeholder="Defina aqui o seu plano de ação..."
                  value={novoPlanoDescricao}
                  onChange={(e) => setNovoPlanoDescricao(e.target.value)}
                  rows={3}
                  className="resize-none"
                />
                <div className="flex flex-wrap items-end gap-4">
                  <div className="space-y-2">
                    <Label>Previsão para concluir esta ação *</Label>
                    <Input
                      type="date"
                      value={novoPlanoDeadline}
                      onChange={(e) => setNovoPlanoDeadline(e.target.value)}
                      max={dataMaxPrevisao}
                      title={dataMaxPrevisao ? `Não pode ser superior a ${format(parseISO(previsaoConclusao!), 'dd/MM/yyyy', { locale: ptBR })}` : undefined}
                    />
                  </div>
                  <Button onClick={handleAdicionarPlano} disabled={!novoPlanoDescricao.trim() || !novoPlanoDeadline.trim() || salvandoPlano}>
                    {salvandoPlano ? 'Adicionando...' : 'Adicionar'}
                  </Button>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Lista de planos: oculta no step 5 para evitar duplicar com o card "Planos concluídos" (gestor) */}
          {!isStep5 && (
            <Card className="border border-borderSoft bg-card shadow-softToken">
              <CardContent className="p-6">
                <h3 className="font-semibold mb-4">Planos de ação</h3>
                <div className="space-y-3">
                  {pdi.actionPlans?.length === 0 ? (
                    <p className="text-muted-foreground py-4">Nenhum plano de ação cadastrado.</p>
                  ) : (
                    pdi.actionPlans?.map((plan) => (
                      <PlanoCard
                        key={plan.id}
                        plan={plan}
                        onConcluir={() => openConfirmarConcluirTarefa(plan.id)}
                        concluindo={concluindoPlanId === plan.id}
                        somenteLeitura={!showStep2Content || apenasVisualizar}
                        editavelQuandoNaoIniciado={podeEditarEstePlano(plan)}
                        sempreMostrarEditarExcluir={sempreMostrarEditarExcluir}
                        podeEditarExcluirEstePlano={podeEditarEstePlano(plan)}
                        onEditar={handleEditarPlano}
                        onExcluir={handleExcluirPlano}
                        excluindo={excluindoPlanId === plan.id}
                        editando={editandoPlanId === plan.id}
                      />
                    ))
                  )}
                </div>
              </CardContent>
            </Card>
          )}
          {/* Colaborador no step 2: Concluir planejamento (criado por gestor) ou Concluir definição (próprio PDI) */}
          {!apenasVisualizar && isMeuPdi && totalPlans > 0 && (
            <div className="flex flex-wrap items-center gap-3">
              {criadoPorGestor ? (
                <>
                  <Button
                    variant="outline"
                    onClick={() => navigate('/gestao-desempenho-colaborador')}
                  >
                    Voltar para lista
                  </Button>
                  <Button
                    onClick={async () => {
                      if (!token || !pdiId) return;
                      try {
                        await api.atualizarPdi(
                          token,
                          pdiId,
                          { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.IN_ANALYSIS },
                          colaboradorIdForApi,
                        );
                        toast.success('Planos enviados para aprovação do gestor.');
                        await refetch();
                      } catch (e: unknown) {
                        toast.error(e instanceof Error ? e.message : 'Erro ao enviar para aprovação.');
                      }
                    }}
                    className="bg-primary text-primary-foreground hover:bg-primary/90"
                  >
                    Concluir planejamento
                  </Button>
                  <p className="text-sm text-muted-foreground w-full">
                    Ao concluir o planejamento, seu gestor poderá aprovar, solicitar ajuste ou cancelar. Enquanto isso, o status ficará em análise.
                  </p>
                </>
              ) : (
                <>
                  <Button
                    variant="outline"
                    onClick={() => navigate('/gestao-desempenho-colaborador')}
                  >
                    Voltar para lista
                  </Button>
                  <Button
                    onClick={async () => {
                      if (!token || !pdiId) return;
                      try {
                        await api.atualizarPdi(
                          token,
                          pdiId,
                          { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.IN_PROGRESS },
                          colaboradorIdForApi,
                        );
                        toast.success('Definição concluída. Siga para Planos em andamento.');
                        await refetch();
                      } catch (e: unknown) {
                        toast.error(e instanceof Error ? e.message : 'Erro ao concluir definição.');
                      }
                    }}
                    className="bg-primary text-primary-foreground hover:bg-primary/90"
                  >
                    Concluir definição e ir para gestão
                  </Button>
                  <p className="text-sm text-muted-foreground w-full">
                    Ao concluir, você seguirá para a etapa Planos em andamento. Não há aprovação do gestor quando você cria seu próprio PDI.
                  </p>
                </>
              )}
            </div>
          )}
          {/* Gestor no step 2 (PDI que ele criou): Concluir planejamento → step 3 (aprovação) */}
          {!apenasVisualizar && !isMeuPdi && !!codColaborador && showStep2Content && totalPlans > 0 && statusLabel === PdiStatusLabel.NAO_INICIADO && (
            <div className="flex flex-wrap items-center gap-3">
              <Button
                variant="outline"
                onClick={handleVoltar}
              >
                Voltar
              </Button>
              <Button
                onClick={async () => {
                  if (!token || !pdiId) return;
                  try {
                    await api.atualizarPdi(
                      token,
                      pdiId,
                      { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.IN_ANALYSIS },
                      colaboradorIdForApi,
                    );
                    toast.success('Planejamento concluído. Agora aprove os planos na etapa 3.');
                    await refetch();
                  } catch (e: unknown) {
                    toast.error(e instanceof Error ? e.message : 'Erro ao concluir planejamento.');
                  }
                }}
                className="bg-primary text-primary-foreground hover:bg-primary/90"
              >
                Concluir planejamento
              </Button>
              <p className="text-sm text-muted-foreground w-full">
                Ao concluir, você seguirá para a etapa 3 (Aprovação do gestor), onde poderá aprovar, solicitar ajuste ou cancelar.
              </p>
            </div>
          )}
        </>
      )}

      {/* Step 3: Aprovação do gestor (aplicável para PDI criado pelo gestor) */}
      {showStep3Content && (
        <>
          <Card className="border border-borderSoft bg-card shadow-softToken">
            <CardContent className="p-6">
              <h3 className="font-semibold mb-4">Planos de ação</h3>
              <p className="text-sm text-muted-foreground mb-4">
                {isMeuPdi
                  ? 'Planos enviados para aprovação do gestor.'
                  : 'Avalie os planos de ação abaixo e escolha uma ação.'}
              </p>
              <div className="space-y-3">
                {pdi.actionPlans?.length === 0 ? (
                  <p className="text-muted-foreground py-4">Nenhum plano de ação cadastrado.</p>
                ) : (
                  pdi.actionPlans?.map((plan) => (
                    <PlanoCard
                      key={plan.id}
                      plan={plan}
                      onConcluir={() => openConfirmarConcluirTarefa(plan.id)}
                      concluindo={concluindoPlanId === plan.id}
                      somenteLeitura
                    />
                  ))
                )}
              </div>
            </CardContent>
          </Card>
          <Card className="border border-borderSoft bg-card shadow-softToken">
            <CardContent className="p-6 space-y-4">
              <h3 className="font-semibold">Aprovação do gestor</h3>
              {isMeuPdi ? (
                <div className="space-y-3">
                  <p className="text-sm text-muted-foreground">
                    Seus planos foram enviados para aprovação. Aguarde a validação do seu gestor para iniciar a execução.
                  </p>
                  <div className="flex flex-wrap gap-2">
                    <Button variant="outline" onClick={() => navigate('/gestao-desempenho-colaborador')}>
                      Voltar para lista
                    </Button>
                  </div>
                </div>
              ) : (
                <p className="text-sm text-muted-foreground">
                  Avalie os planos de ação e escolha uma ação.
                </p>
              )}

              {!isMeuPdi && !apenasVisualizar && (
              <div className="flex flex-wrap gap-3">
                <Button
                  onClick={async () => {
                    if (!token || !pdiId || !codColaborador) return;
                    try {
                      const res = await api.aprovarPdi(token, codColaborador, pdiId);
                      updatePdi({ status: res.status });
                      toast.success('PDI aprovado. Status atualizado para Em andamento.');
                      await new Promise((r) => setTimeout(r, 150));
                      await refetch();
                    } catch (e: unknown) {
                      toast.error(e instanceof Error ? e.message : 'Erro ao aprovar PDI.');
                    }
                  }}
                >
                  Aprovar
                </Button>
                <Button
                  variant="outline"
                  onClick={async () => {
                    if (!token || !pdiId || !pdi) return;
                    try {
                      await api.atualizarPdi(
                        token,
                        pdiId,
                        { titulo: pdi.titulo, descricao: pdi.descricao ?? '', status: PdiStatusApi.NOT_STARTED },
                        colaboradorIdForApi,
                      );
                      toast.success('Ajuste solicitado. Status voltou para Não iniciado.');
                      await refetch();
                    } catch (e: unknown) {
                      toast.error(e instanceof Error ? e.message : 'Erro ao solicitar ajuste.');
                    }
                  }}
                >
                  Solicitar ajuste
                </Button>
                <Button
                  variant="outline"
                  className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                  onClick={() => setModalCancelarPdiOpen(true)}
                >
                  Cancelar PDI
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
        </>
      )}

      {/* Step 4: Planos em andamento — título e planos no mesmo card branco */}
      {showStep4Content && !showStep2Content && !showStep3Content && (
        <div className="space-y-6">
          <Card className="border border-borderSoft bg-card shadow-softToken overflow-hidden">
            <CardContent className="p-6">
              <h3 className="font-semibold text-foreground mb-2 pb-3 border-b border-borderSoft">
                Planos de ação — conclua suas tarefas
              </h3>
              {!apenasVisualizar && isMeuPdi && (
                <p className="text-sm text-muted-foreground mb-4">
                  Depois de concluir todos os planos, você irá para a etapa 5 para anexar a evidência e finalizar o PDI.
                </p>
              )}
              <div className="space-y-3">
                {pdi.actionPlans?.length === 0 ? (
                  <p className="text-sm text-muted-foreground py-4">Nenhum plano de ação.</p>
                ) : (
                  pdi.actionPlans?.map((plan) => (
                    <PlanoCard
                      key={plan.id}
                      plan={plan}
                      onConcluir={() => openConfirmarConcluirTarefa(plan.id)}
                      concluindo={concluindoPlanId === plan.id}
                      somenteLeitura={!isMeuPdi || apenasVisualizar}
                      sempreMostrarEditarExcluir={sempreMostrarEditarExcluir}
                      podeEditarExcluirEstePlano={podeEditarEstePlano(plan)}
                      onEditar={handleEditarPlano}
                      onExcluir={handleExcluirPlano}
                      excluindo={excluindoPlanId === plan.id}
                      editando={editandoPlanId === plan.id}
                    />
                  ))
                )}
              </div>
            </CardContent>
          </Card>
          {!apenasVisualizar && isMeuPdi && (
            <Card className="border border-borderSoft bg-card shadow-softToken">
              <CardContent className="p-6 space-y-4">
                <h4 className="font-semibold text-foreground">Adicionar outro plano de ação</h4>
                <Textarea
                  placeholder="Defina aqui o seu plano de ação..."
                  value={novoPlanoDescricao}
                  onChange={(e) => setNovoPlanoDescricao(e.target.value)}
                  rows={3}
                  className="resize-none border-borderSoft"
                />
                <div className="flex flex-wrap items-end gap-4">
                  <div className="space-y-2">
                    <Label>Previsão para concluir esta ação *</Label>
                    <Input
                      type="date"
                      value={novoPlanoDeadline}
                      onChange={(e) => setNovoPlanoDeadline(e.target.value)}
                      max={dataMaxPrevisao}
                      className="border-borderSoft"
                      title={dataMaxPrevisao ? `Não pode ser superior a ${format(parseISO(previsaoConclusao!), 'dd/MM/yyyy', { locale: ptBR })}` : undefined}
                    />
                  </div>
                  <Button onClick={handleAdicionarPlano} disabled={!novoPlanoDescricao.trim() || !novoPlanoDeadline.trim() || salvandoPlano}>
                    {salvandoPlano ? 'Adicionando...' : 'Adicionar'}
                  </Button>
                </div>
              </CardContent>
            </Card>
          )}
          {!apenasVisualizar && (isMeuPdi || isOwnerPdi) && (
            <div className="flex flex-wrap items-center gap-3 pt-2">
              <Button variant="outline" onClick={handleVoltar}>
                Voltar
              </Button>
              <Button variant="outline" className="text-destructive hover:bg-destructive/10 hover:text-destructive" onClick={() => setModalCancelarPdiOpen(true)}>
                Cancelar PDI
              </Button>
            </div>
          )}
        </div>
      )}

      {/* Step 5: Planos concluídos → anexar evidência e concluir PDI (colaborador, gestor como dono, ou gestor que acabou de concluir todos os planos) */}
      {showStep5Content && (isMeuPdi || isOwnerPdi || stepIndex === 4) && (
        <div className="space-y-6">
          <Card className="border border-borderSoft bg-card shadow-softToken">
            <CardContent className="p-6">
              <h3 className="font-semibold text-foreground mb-2 pb-3 border-b border-borderSoft">Planos concluídos</h3>
              <div className="space-y-3 mt-4">
                {pdi.actionPlans?.map((plan) => (
                  <PlanoCard
                    key={plan.id}
                    plan={plan}
                    onConcluir={() => openConfirmarConcluirTarefa(plan.id)}
                    concluindo={concluindoPlanId === plan.id}
                    somenteLeitura={apenasVisualizar}
                  />
                ))}
              </div>
            </CardContent>
          </Card>
          {!apenasVisualizar && (isMeuPdi || isOwnerPdi) && (
            <Card className="border-2 border-primary/30 bg-primary/5">
              <CardContent className="p-6 space-y-4">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/20">
                    <Upload className="h-6 w-6 text-primary" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-foreground">Etapa 5: Anexar evidência e concluir PDI</h3>
                    <p className="text-sm text-muted-foreground mt-0.5">
                      Todas as tarefas foram concluídas. Você pode anexar um documento (opcional) e concluir o PDI.
                    </p>
                  </div>
                </div>
                <Button onClick={handleAbrirModalFinalizar} className="w-full sm:w-auto">
                  <Upload className="h-4 w-4 mr-2" />
                  Anexar arquivo e Concluir PDI
                </Button>
              </CardContent>
            </Card>
          )}
        </div>
      )}

      {/* Evidências / Anexos — step 5 para colaborador ou sempre para gestor; clicável para visualizar PDF/documento */}
      {(showStep5Content || !isMeuPdi) && pdi.evidencias && pdi.evidencias.length > 0 && (
        <div>
          <h3 className="font-semibold mb-2">Anexos / Evidências</h3>
          <ul className="space-y-1 text-sm text-muted-foreground">
            {pdi.evidencias.map((ev) => (
              <li key={ev.id}>
                <button
                  type="button"
                  onClick={() => handleVisualizarEvidencia(ev)}
                  disabled={abrirEvidenciaId === ev.id}
                  className="flex items-center gap-2 w-full text-left rounded-md px-2 py-1.5 hover:bg-muted/60 transition-colors disabled:opacity-60"
                >
                  <Upload className="h-4 w-4 shrink-0" />
                  <span className="truncate flex-1">{ev.docName}</span>
                  <Badge variant="outline" className="text-xs shrink-0">{ev.tipo}</Badge>
                  {abrirEvidenciaId === ev.id ? (
                    <span className="text-xs shrink-0">Abrindo...</span>
                  ) : null}
                </button>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Botões: Voltar (sempre); Cancelar PDI e Concluir PDI só quando fizer sentido (step 5 = finalizar) */}
      {(showStep5Content || !isMeuPdi) && !apenasVisualizar && (
        <div className="flex flex-wrap gap-4 pt-4 border-t">
          <Button variant="outline" onClick={handleVoltar}>
            Voltar
          </Button>
          {(isMeuPdi || isOwnerPdi) && (
            <Button variant="outline" className="text-destructive hover:bg-destructive/10 hover:text-destructive" onClick={() => setModalCancelarPdiOpen(true)}>
              Cancelar PDI
            </Button>
          )}
          {stepIndex === 4 && (isMeuPdi || isOwnerPdi) && (
            <Button onClick={handleAbrirModalFinalizar}>
              <CheckCircle className="h-4 w-4 mr-2" />
              Concluir PDI
            </Button>
          )}
        </div>
      )}

      {/* Modal Confirmar conclusão da tarefa */}
      <AlertDialog open={modalConcluirTarefaOpen} onOpenChange={setModalConcluirTarefaOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar tarefa</AlertDialogTitle>
            <AlertDialogDescription>
              Deseja confirmar que esta tarefa foi concluída? Atenção! Ao confirmar, esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Fechar</AlertDialogCancel>
            <AlertDialogAction onClick={handleConfirmarConcluirTarefa}>Confirmar</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal Confirmar cancelamento do PDI */}
      <AlertDialog open={modalCancelarPdiOpen} onOpenChange={setModalCancelarPdiOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Cancelar PDI</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja cancelar este PDI? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={cancelandoPdi}>Não</AlertDialogCancel>
            <AlertDialogAction onClick={handleCancelarPdi} disabled={cancelandoPdi} className="bg-destructive text-destructive-foreground hover:bg-destructive/90">
              {cancelandoPdi ? 'Cancelando...' : 'Sim, cancelar PDI'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal Editar plano de ação (quando PDI não iniciado) */}
      <Dialog open={modalEditarPlanoOpen} onOpenChange={setModalEditarPlanoOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Editar plano de ação</DialogTitle>
            <DialogDescription>Altere a descrição e o prazo. Salve para aplicar.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label>Descrição *</Label>
              <Textarea
                value={planEditForm.description}
                onChange={(e) => setPlanEditForm((p) => ({ ...p, description: e.target.value }))}
                rows={3}
                className="resize-none"
                placeholder="Descreva o plano de ação..."
              />
            </div>
            <div className="space-y-2">
              <Label>Previsão para concluir</Label>
              <Input
                type="date"
                value={planEditForm.deadline}
                onChange={(e) => setPlanEditForm((p) => ({ ...p, deadline: e.target.value }))}
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setModalEditarPlanoOpen(false)}>
              Cancelar
            </Button>
            <Button
              onClick={handleSalvarEdicaoPlano}
              disabled={!planEditForm.description.trim() || editandoPlanId === planEditForm.id}
            >
              {editandoPlanId === planEditForm.id ? 'Salvando...' : 'Salvar'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal Confirmar exclusão de plano */}
      <AlertDialog open={modalExcluirPlanoOpen} onOpenChange={setModalExcluirPlanoOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Excluir plano de ação</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja excluir este plano de ação? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={!!excluindoPlanId}>Não</AlertDialogCancel>
            <AlertDialogAction onClick={handleConfirmarExcluirPlano} disabled={!!excluindoPlanId} className="bg-destructive text-destructive-foreground hover:bg-destructive/90">
              {excluindoPlanId ? 'Excluindo...' : 'Sim, excluir'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Modal Parabéns — concluiu os planos de ação; ao clicar Entendi abre o modal de anexo */}
      <Dialog open={modalParabensOpen} onOpenChange={setModalParabensOpen}>
        <DialogContent className="sm:max-w-md p-0 overflow-hidden border-0 gap-0">
          <div className="relative rounded-lg overflow-hidden">
            <img
              src={parabensImg}
              alt="Parabéns"
              className="w-full h-auto object-cover"
            />
            <div className="absolute inset-0 flex flex-col justify-center pl-6 pr-12">
              <p className="text-2xl font-bold text-orange-500 drop-shadow-sm">Parabéns!</p>
              <p className="text-white text-sm font-medium drop-shadow-md mt-0.5">Você concluiu sua meta</p>
            </div>
          </div>
          <p className="px-6 pt-4 pb-2 text-muted-foreground text-center text-sm">
            Vamos para um próximo desafio ?!
          </p>
          <div className="px-6 pb-6">
            <Button
              className="w-full bg-black hover:bg-black/90 text-white"
              onClick={handleEntendiParabens}
            >
              Entendi
            </Button>
          </div>
        </DialogContent>
      </Dialog>

      {/* Modal Anexar documento ao PDI — anexo é opcional; pode concluir com ou sem documento */}
      <Dialog open={modalFinalizarPdiOpen} onOpenChange={setModalFinalizarPdiOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Concluir PDI</DialogTitle>
            <DialogDescription>Você pode anexar um documento (opcional) ou concluir o PDI sem anexo.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label>Tipo do documento</Label>
              <div className="flex gap-4 mt-2">
                {(['EVIDENCIA', 'CERTIFICADO', 'OUTRO'] as const).map((t) => (
                  <label key={t} className="flex items-center gap-2 cursor-pointer">
                    <input
                      type="radio"
                      name="tipoAnexo"
                      checked={tipoAnexo === t}
                      onChange={() => setTipoAnexo(t)}
                    />
                    <span>{t === 'EVIDENCIA' ? 'Evidência' : t === 'CERTIFICADO' ? 'Certificado' : 'Outro'}</span>
                  </label>
                ))}
              </div>
            </div>
            <div>
              <Label>Arquivo (máx. 50MB)</Label>
              <div
                className="mt-2 border-2 border-dashed rounded-lg p-6 text-center cursor-pointer hover:bg-muted/50"
                onClick={() => document.getElementById('pdi-file-input')?.click()}
              >
                <Upload className="h-10 w-10 mx-auto text-muted-foreground mb-2" />
                <p className="text-sm text-muted-foreground">
                  {arquivoAnexo ? arquivoAnexo.name : 'Clique para selecionar o arquivo'}
                </p>
                <input
                  id="pdi-file-input"
                  type="file"
                  className="hidden"
                  accept=".pdf,.doc,.docx,.jpg,.jpeg,.png"
                  onChange={(e) => setArquivoAnexo(e.target.files?.[0] ?? null)}
                />
              </div>
            </div>
            <div>
              <Label>Link (opcional)</Label>
              <Input
                placeholder="Insira um link como evidência"
                value={linkAnexo}
                onChange={(e) => setLinkAnexo(e.target.value)}
                className="mt-2"
              />
            </div>
          </div>
          <DialogFooter className="flex-col sm:flex-row gap-2">
            <Button variant="outline" onClick={() => setModalFinalizarPdiOpen(false)} className="order-2 sm:order-1">
              Voltar
            </Button>
            <div className="flex gap-2 order-1 sm:order-2">
              <Button
                variant="outline"
                onClick={handleFinalizarSemAnexo}
                disabled={finalizandoSemAnexo}
              >
                {finalizandoSemAnexo ? 'Finalizando...' : 'Concluir sem anexo'}
              </Button>
              <Button onClick={handleConfirmarEnvioAnexo} disabled={!arquivoAnexo}>
                Enviar e concluir
              </Button>
            </div>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Modal Confirmar envio de anexo */}
      <AlertDialog open={modalConfirmarAnexoOpen} onOpenChange={setModalConfirmarAnexoOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar envio de anexo</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza? Não pode ser desfeito.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={handleEnviarAnexo} disabled={enviandoAnexo}>
              {enviandoAnexo ? 'Enviando...' : 'Confirmar'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

function PlanoCard({
  plan,
  onConcluir,
  concluindo,
  somenteLeitura,
  editavelQuandoNaoIniciado,
  sempreMostrarEditarExcluir,
  podeEditarExcluirEstePlano,
  onEditar,
  onExcluir,
  excluindo,
  editando,
}: {
  plan: PdiActionPlanDTO;
  onConcluir: () => void;
  concluindo: boolean;
  somenteLeitura?: boolean;
  editavelQuandoNaoIniciado?: boolean;
  /** Step 2 e 4: botões Editar/Excluir sempre visíveis; habilitados conforme podeEditarExcluirEstePlano */
  sempreMostrarEditarExcluir?: boolean;
  podeEditarExcluirEstePlano?: boolean;
  onEditar?: (plan: PdiActionPlanDTO) => void;
  onExcluir?: (planId: string) => void;
  excluindo?: boolean;
  editando?: boolean;
}) {
  const concluido = !!plan.concluidoEm;
  const habilitadoEditarExcluir = (podeEditarExcluirEstePlano ?? editavelQuandoNaoIniciado) && !concluido && onEditar && onExcluir;
  const mostrarBotoesEditarExcluir = sempreMostrarEditarExcluir ? (onEditar && onExcluir) : (editavelQuandoNaoIniciado && !concluido && onEditar && onExcluir);
  const podeConcluir = !somenteLeitura && !concluido && !editavelQuandoNaoIniciado;
  return (
    <Card className={concluido ? 'border-green-200 bg-green-50/30' : ''}>
      <CardContent className="p-4">
        <div className="flex items-start justify-between gap-4">
          <div className="flex-1">
            <p className="text-sm text-muted-foreground">
              Prazo previsto: {plan.deadline ? format(parseISO(plan.deadline.slice(0, 10)), 'dd/MM/yyyy', { locale: ptBR }) : '-'}
            </p>
            {concluido && (
              <p className="text-sm text-green-700 mt-1">
                Concluído em: {format(parseISO(plan.concluidoEm!.slice(0, 10)), 'dd/MM/yyyy', { locale: ptBR })}
              </p>
            )}
            <p className="mt-2">{plan.description}</p>
          </div>
          {mostrarBotoesEditarExcluir && (
            <div className="flex items-center gap-2 shrink-0">
              <Button
                variant="ghost"
                size="icon"
                title="Editar"
                disabled={!habilitadoEditarExcluir || editando}
                onClick={() => onEditar?.(plan)}
              >
                <Edit className="h-4 w-4" />
              </Button>
              <Button
                variant="ghost"
                size="icon"
                title="Excluir"
                disabled={!habilitadoEditarExcluir || excluindo}
                onClick={() => onExcluir?.(plan.id)}
                className="text-destructive hover:text-destructive hover:bg-destructive/10"
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
          )}
          {podeConcluir && (
            <div className="flex items-center gap-2 shrink-0">
              {!sempreMostrarEditarExcluir && (
                <>
                  <Button variant="ghost" size="icon" title="Editar" disabled>
                    <Edit className="h-4 w-4" />
                  </Button>
                  <Button variant="ghost" size="icon" title="Excluir" disabled>
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </>
              )}
              <Button
                variant="primary"
                size="sm"
                className="gap-1 bg-orange-600 hover:bg-orange-700"
                onClick={onConcluir}
                disabled={concluindo}
              >
                <CheckCircle className="h-4 w-4" />
                Concluir
              </Button>
            </div>
          )}
          {concluido && (
            <div className="shrink-0">
              <CheckCircle className="h-6 w-6 text-green-600" />
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
