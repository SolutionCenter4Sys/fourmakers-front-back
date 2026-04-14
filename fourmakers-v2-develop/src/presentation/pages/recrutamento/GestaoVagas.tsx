import { useState, useRef, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { MudarStatusVagaUseCase } from '@domain/usecases/MudarStatusVagaUseCase';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { TooltipProvider, Tooltip, TooltipContent, TooltipTrigger } from '@/components/ui/tooltip';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { Spinner } from '@/components/ui/spinner';
import { Edit, Eye, EyeOff, GripVertical, Info, Search, Settings2, ChevronDown, PersonAdd, Share2 } from '@/components/ui/system-icons';
import { cn } from '@/lib/utils';
import { PageBreadcrumb, PageHeader, CountBadge } from '@presentation/components/common';
import {
  MAX_VAGAS_POR_COLUNA_OPTIONS,
  useGestaoVagas,
  type VagaCardModel,
  getStatusFlowConfig,
  parseSlaToHours,
  formatSlaTooltip,
} from '@presentation/hooks/recrutamento'
import { useGrupos } from '@presentation/hooks/useGrupos';
import { toast } from 'sonner';
import {
  VagaInfoModal,
  InscreverCandidatoModal,
  InscricaoPreferenciasModal,
  ImportarTalentosModal,
  MeusCandidatosModal,
  AtribuirRecrutadorModal,
  DuplicarVagaModal,
  MovimentacaoVagaModal,
  PerdaVagaModal,
} from '@presentation/components/gestao-vagas';
import type { VagaFilhaItem } from '@presentation/components/gestao-vagas';

/** Modo admin (visibilidade de perfis/vagas) deve ser atrelado à permissão do usuário quando disponível no backend. */
const showModoAdmin = false;

const SLA_LIMITE_HORAS = 2;

type ClienteGestorCard = {
  codigoCliente?: string;
  nomeCliente?: string;
  codigoGestor?: string;
  nomeGestor?: string;
};

const VagaCard = ({
  model,
  statusOptions,
  onInfo,
  onView,
  onChangeStatus,
  onEditPerfil,
  onEditarVaga,
  onInscrever,
  onAdicionarRecrutador,
  onDragStartCard,
  onCompartilhar,
  modoAdmin,
  visivel,
  onToggleVisibilidade,
  showCandidatosPorEstagio,
}: {
  model: VagaCardModel;
  statusOptions: { id: string; nome: string }[];
  onInfo: (raw: VagaCardModel['raw']) => void;
  onView: (vagaId: VagaCardModel['vagaId'], card?: VagaCardModel) => void;
  onCompartilhar?: (model: VagaCardModel) => void;
  onChangeStatus: (
    vagaId: VagaCardModel['vagaId'],
    statusFrom: string,
    statusTo: string,
    model?: VagaCardModel
  ) => void;
  onEditPerfil?: (idPerfil: string, card?: ClienteGestorCard) => void;
  onEditarVaga?: (vagaId: string | number) => void;
  onInscrever?: (model: VagaCardModel) => void;
  onAdicionarRecrutador?: (model: VagaCardModel) => void;
  onDragStartCard?: (model: VagaCardModel) => void;
  modoAdmin?: boolean;
  visivel?: boolean;
  onToggleVisibilidade?: () => void;
  showCandidatosPorEstagio?: boolean;
}) => {
  const isVisivel = visivel !== false;
  const statusCod = String(model.statusId ?? '');
  const statusNum = Number(statusCod) || 0;
  const editPerfilEnabled = statusNum <= 2;
  const editVagaEnabled = statusNum >= 3;
  const podeEditarPerfil = editPerfilEnabled && !!model.idPerfilGerador;
  const raw = model.raw as Record<string, unknown> | undefined;
  const slaEtapaAtual = raw?.slaDecorridoDaEtapaAtual as string | null | undefined;
  const slaTotal = raw?.slaDecorridoTotal as string | null | undefined;
  const { hours: slaHours, label: slaLabel } = parseSlaToHours(slaEtapaAtual);
  const slaExcedeLimite = slaHours > SLA_LIMITE_HORAS;
  const slaTooltip = formatSlaTooltip(slaEtapaAtual, slaTotal, SLA_LIMITE_HORAS);
  const cardData: ClienteGestorCard | undefined =
    raw && (raw.codigoCliente || raw.nomeCliente || raw.codigoGestorExterno || raw.codGestorExterno || raw.nomeGestor)
      ? {
          codigoCliente: String(raw.codigoCliente ?? raw.codigoClienteOrg ?? ''),
          nomeCliente: String(raw.nomeCliente ?? ''),
          codigoGestor: String(raw.codigoGestorExterno ?? raw.codGestorExterno ?? raw.codigoGestor ?? ''),
          nomeGestor: String(raw.nomeGestorExterno ?? raw.nomeGestor ?? raw.gestorExternoNome ?? ''),
        }
      : undefined;
  return (
    <Card className="shadow-none border-borderSoft cursor-default relative" data-kanban-card>
      <CardContent className="p-4 space-y-3">
        {onDragStartCard && (
          <div
            role="button"
            tabIndex={0}
            draggable
            onDragStart={(e) => {
              e.dataTransfer.setData('text/plain', 'vaga');
              e.dataTransfer.effectAllowed = 'move';
              onDragStartCard(model);
              const cardEl = (e.currentTarget as HTMLElement).closest('[data-kanban-card]') as HTMLElement | null;
              if (cardEl) {
                const rect = cardEl.getBoundingClientRect();
                const clone = cardEl.cloneNode(true) as HTMLElement;
                clone.style.opacity = '0.95';
                clone.style.pointerEvents = 'none';
                clone.style.position = 'absolute';
                clone.style.left = '-9999px';
                clone.style.top = '0';
                clone.style.width = `${rect.width}px`;
                document.body.appendChild(clone);
                const offsetX = rect.width - 36;
                const offsetY = 20;
                e.dataTransfer.setDragImage(clone, offsetX, offsetY);
                setTimeout(() => clone.remove(), 0);
              }
            }}
            className="absolute top-2 right-2 p-1 rounded cursor-grab active:cursor-grabbing text-muted-foreground hover:bg-muted/50 touch-none"
            aria-label="Arrastar para alterar coluna"
          >
            <GripVertical className="h-4 w-4" />
          </div>
        )}
        <div className="flex items-center justify-end gap-2 text-muted-foreground">
          <Tooltip>
            <TooltipTrigger asChild>
              {modoAdmin ? (
                <button
                  type="button"
                  onClick={onToggleVisibilidade}
                  className={cn(
                    'inline-flex h-9 w-9 items-center justify-center rounded-md border-2 transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2',
                    isVisivel
                      ? 'border-success bg-success/15 text-success hover:bg-success/25'
                      : 'border-destructive bg-destructive/15 text-destructive hover:bg-destructive/25'
                  )}
                  aria-label={isVisivel ? 'Visível para grupos — clique para ocultar' : 'Oculto para grupos — clique para exibir'}
                >
                  <Edit className="h-5 w-5" />
                </button>
              ) : editVagaEnabled ? (
                <Button
                  variant="ghost"
                  size="icon"
                  aria-label="Editar Vaga"
                  title="Editar Vaga"
                  onClick={() => onEditarVaga?.(model.vagaId)}
                >
                  <Edit className="h-5 w-5" />
                </Button>
              ) : (
                <Button
                  variant="ghost"
                  size="icon"
                  aria-label="Editar perfil"
                  title={podeEditarPerfil ? 'Editar perfil' : 'Editar perfil (disponível apenas para status iniciais)'}
                  disabled={!podeEditarPerfil}
                  onClick={() => podeEditarPerfil && model.idPerfilGerador && onEditPerfil?.(model.idPerfilGerador, cardData)}
                >
                  <Edit className="h-5 w-5" />
                </Button>
              )}
            </TooltipTrigger>
            <TooltipContent side="bottom">
              {modoAdmin
                ? (isVisivel ? 'Visível para grupos — clique para ocultar' : 'Oculto para grupos — clique para exibir')
                : editVagaEnabled
                  ? 'Editar Vaga'
                  : podeEditarPerfil
                    ? 'Editar perfil'
                    : 'Editar perfil (disponível apenas para status 1 e 2)'}
            </TooltipContent>
          </Tooltip>

          <Tooltip>
            <TooltipTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                aria-label="Inscrever candidato"
                title="Inscrever candidato"
                onClick={() => onInscrever?.(model)}
              >
                <PersonAdd className="h-5 w-5" />
              </Button>
            </TooltipTrigger>
            <TooltipContent side="bottom">Inscrever candidato</TooltipContent>
          </Tooltip>

          <div className="relative inline-flex">
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  aria-label="Ver candidatos"
                  title="Ver candidatos"
                  onClick={() => onView(model.vagaId, model)}
                >
                  <Eye className="h-5 w-5" />
                </Button>
              </TooltipTrigger>
              <TooltipContent side="bottom">Ver candidatos</TooltipContent>
            </Tooltip>
            <span className="pointer-events-none absolute -top-0.5 -right-0.5 z-10">
              <CountBadge count={model.totalCandidatos} className="!min-w-[1rem] !h-4 !p-0 !text-[10px]" />
            </span>
          </div>

          <Tooltip>
            <TooltipTrigger asChild>
              <Button
                variant="ghost"
                size="icon"
                aria-label="Informações"
                title="Informações"
                onClick={() => onInfo(model.raw)}
              >
                <Info className="h-5 w-5" />
              </Button>
            </TooltipTrigger>
            <TooltipContent side="bottom">Informações</TooltipContent>
          </Tooltip>

          {onCompartilhar && (
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  aria-label="Compartilhar vaga (página pública)"
                  title="Compartilhar"
                  onClick={() => onCompartilhar(model)}
                >
                  <Share2 className="h-5 w-5" />
                </Button>
              </TooltipTrigger>
              <TooltipContent side="bottom">Compartilhar (abre página pública)</TooltipContent>
            </Tooltip>
          )}
        </div>

        <div className="text-base font-medium leading-snug">
          {onCompartilhar ? (
            <button
              type="button"
              className="text-left hover:underline focus:outline-none focus:ring-2 focus:ring-primary rounded"
              onClick={() => onCompartilhar(model)}
            >
              {model.titulo}
            </button>
          ) : (
            model.titulo
          )}
        </div>

        <div>
          <span className="inline-flex shrink-0 items-center whitespace-nowrap rounded-md border border-borderSoft bg-surfaceSubtle px-2 py-1 text-[11px] font-medium text-primaryText">
            Posições: {model.numeroDeVagas}
          </span>
        </div>

        <div className="space-y-2 text-sm text-muted-foreground">
          {model.clienteLinha && <div>{model.clienteLinha}</div>}
          {model.gestor && <div>{model.gestor}</div>}
          {model.aberturaEm && <div>{`Abertura em: ${model.aberturaEm}`}</div>}
          {model.criador && <div>{`Criada por: ${model.criador}`}</div>}
        </div>

        <div className="text-sm text-muted-foreground flex items-center justify-between">
          <div className="flex items-center gap-2">
            {model.recrutador != null && String(model.recrutador).trim() !== '' ? (
              <span>Responsável: {model.recrutador}</span>
            ) : (
              <span>Adicione um Recrutador</span>
            )}
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-8 w-8 shrink-0"
                  aria-label={
                    model.recrutador != null && String(model.recrutador).trim() !== ''
                      ? 'Alterar recrutador'
                      : 'Adicionar recrutador'
                  }
                  onClick={() => onAdicionarRecrutador?.(model)}
                >
                  <Edit className="h-4 w-4" />
                </Button>
              </TooltipTrigger>
              <TooltipContent side="left">
                {model.recrutador != null && String(model.recrutador).trim() !== ''
                  ? 'Alterar recrutador'
                  : 'Adicionar recrutador'}
              </TooltipContent>
            </Tooltip>
          </div>
        </div>

        <div className="h-px bg-border" />

        <div className="w-full">
          <Select
            value={model.statusId}
            onValueChange={(value) => {
              if (value === model.statusId) return;
              onChangeStatus(model.vagaId, model.statusId, value, model);
            }}
          >
            <SelectTrigger className="w-full rounded-full border-0 bg-secondary px-4 text-secondary-foreground shadow-none [&>span]:line-clamp-none [&>span]:overflow-visible [&>span]:whitespace-normal">
              <SelectValue placeholder="Status" />
            </SelectTrigger>
            <SelectContent>
              {statusOptions.map((status) => (
                <SelectItem key={status.id} value={status.id}>
                  {status.nome}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="h-px bg-border" />

        {/* Tempo decorrido + tag (SLA etapa atual, total e limite no tooltip) */}
        <div className="flex items-center gap-2 flex-wrap">
          <span className="text-xs text-muted-foreground">Tempo decorrido</span>
          <Tooltip>
            <TooltipTrigger asChild>
              <Badge
                variant={slaExcedeLimite ? 'destructive' : 'default'}
                className={cn(
                  'text-xs',
                  !slaExcedeLimite && 'border-success/50 bg-success/10 text-success'
                )}
              >
                {slaLabel}
              </Badge>
            </TooltipTrigger>
            <TooltipContent className="whitespace-pre-wrap">{slaTooltip}</TooltipContent>
          </Tooltip>
        </div>

        <div className="h-px bg-border" />

        <div className="text-xs text-muted-foreground text-right leading-snug">
          Modificado em {model.modificadoEm || '-'}, por {model.alteradoPor || '-'}.
        </div>

        {showCandidatosPorEstagio && (
          <>
            <div className="h-px bg-border" />
            <div className="space-y-2">
              <span className="text-xs font-medium text-muted-foreground">Candidatos por estágio</span>
              {model.quantidadeCandidatosPorEstagio?.length > 0 ? (
                <div className="flex flex-wrap gap-1.5">
                  {model.quantidadeCandidatosPorEstagio.map((item, idx) => (
                    <span
                      key={item.idStatus ?? idx}
                      className="inline-flex items-center rounded-md border border-borderSoft bg-surfaceSubtle px-2 py-0.5 text-xs text-foreground"
                    >
                      {item.descricaoStatus || 'Estágio'}: {item.quantidade ?? 0}
                    </span>
                  ))}
                </div>
              ) : (
                <p className="text-xs text-muted-foreground">Sem candidatos inscritos</p>
              )}
            </div>
          </>
        )}
      </CardContent>
    </Card>
  );
};

export default function GestaoVagas() {
  const {
    loading,
    busca,
    setBusca,
    dataInicio,
    setDataInicio,
    dataFim,
    setDataFim,
    totalVagasGeral,
    totalVagasPorStatus,
    statusVagas,
    statusColumns,
    vagaCardsByStatus,
    maxVagasPorColuna,
    columnVisibility,
    setColumnVisibility,
    handleNovaVaga,
    handleVisualizarVaga,
    vagaInfoOpen,
    setVagaInfoOpen,
    openVagaInfo,
    vagaInfoModel,
    handleAbrirInformacoesVaga,
    handleTentarAlterarStatus,
    handleAlterarLimiteVagas,
    handleAlterarVisibilidadeColuna,
    limiteVagas,
    setLimiteVagas,
    visualizarCandidatosPorEstagio,
    setVisualizarCandidatosPorEstagio,
    carregarVagas,
  } = useGestaoVagas();

  const { todosGrupos } = useGrupos();
  const navigate = useNavigate();
  const token = useAppSelector((state) => state.auth.token) ?? null;
  const [modoAdmin, setModoAdmin] = useState(false);
  const [inscreverModalOpen, setInscreverModalOpen] = useState(false);
  const [inscreverVaga, setInscreverVaga] = useState<VagaCardModel | null>(null);
  const [codigoColaboradorInscrito, setCodigoColaboradorInscrito] = useState<string | null>(null);
  const [preferenciasModalOpen, setPreferenciasModalOpen] = useState(false);
  const [gruposSelecionadosIds, setGruposSelecionadosIds] = useState<string[]>([]);
  const [visibilidadeVagas, setVisibilidadeVagas] = useState(true);
  const [opcoesTelaOpen, setOpcoesTelaOpen] = useState(false);
  const [importarTalentosModalOpen, setImportarTalentosModalOpen] = useState(false);
  const [meusCandidatosModalOpen, setMeusCandidatosModalOpen] = useState(false);
  const [atribuirRecrutadorVaga, setAtribuirRecrutadorVaga] = useState<VagaCardModel | null>(null);
  const [fluxoStatus12Payload, setFluxoStatus12Payload] = useState<{
    vagaId: string;
    codigoVaga: string;
    titulo: string;
    idPerfilGerador?: string;
    codigoCliente?: string;
  } | null>(null);
  const [showMovimentacaoModal, setShowMovimentacaoModal] = useState(false);
  const [perdaVagaPayload, setPerdaVagaPayload] = useState<{ codigoVaga: string } | null>(null);
  const [isKanbanDragging, setIsKanbanDragging] = useState(false);
  const mudarStatusVagaUseCase = useMemo(() => container.resolve(MudarStatusVagaUseCase), []);
  const kanbanScrollRef = useRef<HTMLDivElement>(null);
  const kanbanDragStartX = useRef(0);
  const kanbanDragStartScroll = useRef(0);
  const dragPayloadRef = useRef<{ vagaId: string; statusFrom: string; model: VagaCardModel } | null>(null);

  const handleKanbanHeaderMouseDown = useCallback(
    (e: React.MouseEvent) => {
      if (e.button !== 0 || !kanbanScrollRef.current) return;
      e.preventDefault();
      kanbanDragStartX.current = e.clientX;
      kanbanDragStartScroll.current = kanbanScrollRef.current.scrollLeft;
      setIsKanbanDragging(true);
    },
    []
  );

  const handleKanbanDragMove = useCallback((e: MouseEvent) => {
    if (!kanbanScrollRef.current) return;
    const dx = kanbanDragStartX.current - e.clientX;
    kanbanScrollRef.current.scrollLeft = kanbanDragStartScroll.current + dx;
  }, []);

  const handleKanbanDragEnd = useCallback(() => {
    setIsKanbanDragging(false);
    document.removeEventListener('mousemove', handleKanbanDragMove);
    document.removeEventListener('mouseup', handleKanbanDragEnd);
    document.body.style.cursor = '';
    document.body.style.userSelect = '';
  }, [handleKanbanDragMove]);

  const attachKanbanDragListeners = useCallback(() => {
    document.addEventListener('mousemove', handleKanbanDragMove);
    document.addEventListener('mouseup', handleKanbanDragEnd);
    document.body.style.cursor = 'grabbing';
    document.body.style.userSelect = 'none';
  }, [handleKanbanDragMove, handleKanbanDragEnd]);

  const handleKanbanHeaderDragStart = useCallback(
    (e: React.MouseEvent) => {
      handleKanbanHeaderMouseDown(e);
      if (e.button === 0) attachKanbanDragListeners();
    },
    [handleKanbanHeaderMouseDown, attachKanbanDragListeners]
  );

  /** Arrastar no body/background das colunas (não inicia se o clique for em um card). */
  const handleKanbanBodyDragStart = useCallback(
    (e: React.MouseEvent) => {
      if (e.button !== 0 || !kanbanScrollRef.current) return;
      if ((e.target as HTMLElement).closest('[data-kanban-card]')) return;
      e.preventDefault();
      kanbanDragStartX.current = e.clientX;
      kanbanDragStartScroll.current = kanbanScrollRef.current.scrollLeft;
      setIsKanbanDragging(true);
      attachKanbanDragListeners();
    },
    [attachKanbanDragListeners]
  );

  const [visibilidadeOpcoesTela, setVisibilidadeOpcoesTela] = useState<Record<string, boolean>>({
    'novo-perfil-atuacao': true,
    'relatorios-fourmakers': true,
    'criar-vaga': true,
  });

  const OPCOES_COM_VISIBILIDADE = ['novo-perfil-atuacao', 'relatorios-fourmakers', 'criar-vaga'];

  const toggleVisibilidadeOpcao = (value: string) => {
    setVisibilidadeOpcoesTela((prev) => ({ ...prev, [value]: !(prev[value] ?? true) }));
  };

  const toggleGrupoSelecionado = (grupoId: string) => {
    setGruposSelecionadosIds((prev) =>
      prev.includes(grupoId) ? prev.filter((id) => id !== grupoId) : [...prev, grupoId]
    );
  };

  const OPCOES_TELA = [
    { value: 'meus-talentos-inscritos', label: 'Meus Talentos Inscritos' },
    { value: 'cadastrar-talentos', label: 'Cadastrar Talentos' },
    { value: 'novo-perfil-atuacao', label: 'Novo Perfil de Atuação' },
    { value: 'relatorios-fourmakers', label: 'Relatórios FourMakers' },
    { value: 'ver-meus-uploads', label: 'Ver meus Uploads' },
    { value: 'meus-candidatos', label: 'Meus Candidatos' },
    { value: 'criar-vaga', label: 'Criar Vaga' },
  ] as const;

  const handleOpcaoTelaChange = (value: string) => {
    switch (value) {
      case 'meus-talentos-inscritos':
        navigate('/recrutamento/talentosInscritos');
        break;
      case 'cadastrar-talentos':
        setImportarTalentosModalOpen(true);
        break;
      case 'criar-vaga':
        handleNovaVaga();
        navigate('/recrutamento/perfil?novaVaga=true');
        break;
      case 'novo-perfil-atuacao':
        navigate('/recrutamento/perfil');
        break;
      case 'relatorios-fourmakers':
        navigate('/recrutamento/relatorios');
        break;
      case 'ver-meus-uploads':
        navigate('/recrutamento/uploads');
        break;
      case 'meus-candidatos':
        setMeusCandidatosModalOpen(true);
        break;
      default:
        toast.info('Opção em breve.');
    }
  };

  const getCodigoVagaFromModel = (model: VagaCardModel) =>
    String(
      model.codigoVaga ??
        (model.raw && typeof model.raw === 'object' && 'codigo' in model.raw
          ? (model.raw as { codigo?: number }).codigo
          : '')
    );

  const handleChangeStatus = (
    vagaId: string | number,
    statusFrom: string,
    statusTo: string,
    model?: VagaCardModel
  ) => {
    handleTentarAlterarStatus(vagaId, statusFrom, statusTo);
    const flow = getStatusFlowConfig(statusFrom, statusTo);

    if (statusTo === '11' && model) {
      const codigoVaga = getCodigoVagaFromModel(model) || String(vagaId);
      setPerdaVagaPayload({ codigoVaga });
      return;
    }

    if (flow?.exigeModalDuplicacao && flow.type === 'duplicar_movimentacao' && model) {
      const codigoVaga = getCodigoVagaFromModel(model) || String(vagaId);
      const raw = model.raw as Record<string, unknown> | undefined;
      const codigoCliente = raw && typeof raw.codigoCliente === 'string'
        ? raw.codigoCliente
        : raw && typeof raw.codigoClienteOrg === 'string'
          ? raw.codigoClienteOrg
          : undefined;
      setFluxoStatus12Payload({
        vagaId: String(vagaId),
        codigoVaga,
        titulo: model.titulo ?? '',
        idPerfilGerador: model.idPerfilGerador,
        codigoCliente: codigoCliente || undefined,
      });
      return;
    }

    if (model && token) {
      const codigoVaga = getCodigoVagaFromModel(model) || String(vagaId);
      mudarStatusVagaUseCase
        .execute(token, {
          codigoVaga,
          codigoStatus: Number(statusTo),
          comentarioVaga: '',
        })
        .then((res) => {
          if (res?.sucesso) {
            toast.success(res?.mensagem ?? 'Status alterado com sucesso.');
            carregarVagas();
          } else {
            toast.error(res?.mensagem ?? res?.erros?.[0] ?? 'Erro ao alterar status.');
          }
        })
        .catch(() => {
          toast.error('Erro ao alterar status da vaga.');
        });
      return;
    }

    toast.info('Ação não implementada: a mudança de status da vaga será implementada em breve.');
  };

  const handleEditarVagaFilha = (item: VagaFilhaItem) => {
    const params = new URLSearchParams();
    params.set('idPerfil', item.idPerfilGerador ?? '');
    if (item.codigoCliente) params.set('codigoCliente', item.codigoCliente);
    if (item.nomeCliente) params.set('nomeCliente', item.nomeCliente);
    if (item.codigoGestor) params.set('codigoGestor', item.codigoGestor);
    if (item.nomeGestor) params.set('nomeGestor', item.nomeGestor);
    navigate(`/recrutamento/perfil?${params.toString()}`);
  };

  const buildCriarPerfilQuery = (idPerfil: string, card?: ClienteGestorCard) => {
    const params = new URLSearchParams();
    params.set('idPerfil', idPerfil);
    if (card?.codigoCliente) params.set('codigoCliente', card.codigoCliente);
    if (card?.nomeCliente) params.set('nomeCliente', card.nomeCliente);
    if (card?.codigoGestor) params.set('codigoGestor', card.codigoGestor);
    if (card?.nomeGestor) params.set('nomeGestor', card.nomeGestor);
    return params.toString();
  };

  const handleEditarVagaParaCriarPerfil = (idPerfilGerador: string, card?: ClienteGestorCard) => {
    if (idPerfilGerador) navigate(`/recrutamento/perfil?${buildCriarPerfilQuery(idPerfilGerador, card)}`);
  };

  const handleEditarVaga = (vagaId: string | number) => {
    navigate(`/recrutamento/editar/${encodeURIComponent(String(vagaId))}`);
  };

  const handleViewCandidatos = (vagaId: string | number, card?: VagaCardModel) => {
    handleVisualizarVaga(vagaId);
    const title = card?.titulo ?? String(vagaId);
    const raw = card?.raw as Record<string, unknown> | undefined;
    navigate('/recrutamento/candidatos', { state: { vagaId: String(vagaId), vagaTitle: title, vagaRaw: raw } });
  };

  const handleAbrirInscreverCandidato = (model: VagaCardModel) => {
    setInscreverVaga(model);
    setInscreverModalOpen(true);
  };

  const handleInscreverSuccess = (codigoColaborador: string) => {
    setInscreverModalOpen(false);
    setCodigoColaboradorInscrito(codigoColaborador);
    setPreferenciasModalOpen(true);
  };

  const handlePreferenciasClose = (open: boolean) => {
    setPreferenciasModalOpen(open);
    if (!open) {
      setInscreverVaga(null);
      setCodigoColaboradorInscrito(null);
    }
  };

  const handleVerCandidaturas = (params: { vagaId: string | number; vagaTitle: string; vagaRaw?: Record<string, unknown> }) => {
    navigate('/recrutamento/candidatos', {
      state: {
        vagaId: String(params.vagaId),
        vagaTitle: params.vagaTitle,
        vagaRaw: params.vagaRaw,
      },
    });
    handlePreferenciasClose(false);
  };

  return (
    <TooltipProvider>
      <div className="w-full max-w-none mx-auto p-4 space-y-4">
        <PageBreadcrumb items={[{ label: 'Recrutamento' }]} />

        <PageHeader
          title="Gestão de Vagas"
          description="Gerencie e visualize todas as vagas de recrutamento"
          actions={
            <div className="flex flex-wrap items-center gap-3">
              {showModoAdmin && (
                <>
                  <div className="flex items-center gap-2">
                    <Label htmlFor="modo-admin" className="text-sm font-medium cursor-pointer">
                      Modo Admin
                    </Label>
                    <Switch
                      id="modo-admin"
                      checked={modoAdmin}
                      onCheckedChange={setModoAdmin}
                      aria-label="Ativar modo admin"
                    />
                  </div>
                  {modoAdmin && (
                <Popover>
                  <PopoverTrigger asChild>
                    <Button variant="outline" className="min-w-[180px] justify-between">
                      <span>
                        {gruposSelecionadosIds.length > 0
                          ? `Grupos (${gruposSelecionadosIds.length})`
                          : 'Selecionar grupos'}
                      </span>
                      <ChevronDown className="h-4 w-4 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent align="end" className="w-[280px] p-0" alignOffset={4}>
                    <div className="p-2 border-b border-border">
                      <p className="text-sm font-medium text-foreground">Grupos de acesso</p>
                      <p className="text-xs text-muted-foreground mt-0.5">
                        Selecione um ou mais grupos
                      </p>
                    </div>
                    <div className="max-h-[280px] overflow-auto p-2">
                      {todosGrupos.length === 0 ? (
                        <p className="text-sm text-muted-foreground py-4 text-center">
                          Nenhum grupo cadastrado. Crie grupos em Permissionamento.
                        </p>
                      ) : (
                        <div className="space-y-1">
                          {todosGrupos.map((grupo) => (
                            <label
                              key={grupo.id}
                              className="flex items-center gap-3 rounded-md px-2 py-2 hover:bg-muted/50 cursor-pointer"
                            >
                              <Checkbox
                                checked={gruposSelecionadosIds.includes(grupo.id)}
                                onCheckedChange={() => toggleGrupoSelecionado(grupo.id)}
                              />
                              <span className="text-sm">{grupo.nome}</span>
                            </label>
                          ))}
                        </div>
                      )}
                    </div>
                  </PopoverContent>
                </Popover>
                  )}
                </>
              )}
            </div>
          }
        />

        <Card>
          <CardContent className="p-6">
            <div className="space-y-4">
              <div className="flex flex-col md:flex-row gap-4">
                <div className="flex-1">
                  <label className="text-sm font-medium mb-2 block">Busca</label>
                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      placeholder="Buscar por título, descrição, recrutador..."
                      value={busca}
                      onChange={(e) => setBusca(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                </div>

                <div className="w-full md:w-48">
                  <label className="text-sm font-medium mb-2 block">Vagas criadas de</label>
                  <Input type="date" value={dataInicio} onChange={(e) => setDataInicio(e.target.value)} />
                </div>

                <div className="w-full md:w-48">
                  <label className="text-sm font-medium mb-2 block">Até</label>
                  <Input type="date" value={dataFim} onChange={(e) => setDataFim(e.target.value)} />
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <VagaInfoModal open={vagaInfoOpen} onOpenChange={setVagaInfoOpen} model={vagaInfoModel} />

        <Card>
          <CardContent className="p-6">
            <div className="flex flex-col md:flex-row md:justify-between md:items-center gap-4 mb-4">
              <h2 className="text-lg font-semibold">
                Painel de vagas
                <span className="ml-2 text-sm text-muted-foreground font-normal">
                  ({totalVagasGeral} vagas)
                </span>
              </h2>
              <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-3">
                <Popover open={opcoesTelaOpen} onOpenChange={setOpcoesTelaOpen}>
                  <PopoverTrigger asChild>
                    <Button variant="outline" className="w-full sm:w-[200px] justify-between">
                      <span>Opções de tela</span>
                      <ChevronDown className="h-4 w-4 opacity-50" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent align="end" className="w-[240px] p-0" alignOffset={4}>
                    <div className="py-1">
                      {OPCOES_TELA.map((op) => {
                        const temVisibilidade = OPCOES_COM_VISIBILIDADE.includes(op.value);
                        const mostrarOlho = temVisibilidade && modoAdmin && gruposSelecionadosIds.length > 0;
                        const isVisivel = visibilidadeOpcoesTela[op.value] !== false;
                        return (
                          <div
                            key={op.value}
                            className="flex items-center gap-2 px-3 py-2 hover:bg-muted/50 cursor-pointer rounded-sm mx-1"
                            onClick={() => {
                              handleOpcaoTelaChange(op.value);
                              setOpcoesTelaOpen(false);
                            }}
                            onKeyDown={(e) => {
                              if (e.key === 'Enter' || e.key === ' ') {
                                e.preventDefault();
                                handleOpcaoTelaChange(op.value);
                                setOpcoesTelaOpen(false);
                              }
                            }}
                            role="menuitem"
                            tabIndex={0}
                          >
                            {mostrarOlho ? (
                              <button
                                type="button"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  toggleVisibilidadeOpcao(op.value);
                                }}
                                className={cn(
                                  'inline-flex h-7 w-7 shrink-0 items-center justify-center rounded border transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2',
                                  isVisivel
                                    ? 'border-success bg-success/15 text-success hover:bg-success/25'
                                    : 'border-destructive bg-destructive/15 text-destructive hover:bg-destructive/25'
                                )}
                                aria-label={isVisivel ? 'Visível para grupos — clique para ocultar' : 'Oculto para grupos — clique para exibir'}
                              >
                                {isVisivel ? <Eye className="h-4 w-4" /> : <EyeOff className="h-4 w-4" />}
                              </button>
                            ) : (
                              <span className="w-7 shrink-0" aria-hidden />
                            )}
                            <span className="text-sm flex-1">{op.label}</span>
                          </div>
                        );
                      })}
                    </div>
                  </PopoverContent>
                </Popover>

                <Popover>
                  <PopoverTrigger asChild>
                    <Button variant="outline" size="icon" aria-label="Filtros e colunas" title="Filtros e colunas">
                      <Settings2 className="h-4 w-4" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent align="end" className="w-[320px] p-3">
                    <div className="flex items-center gap-2 pb-2">
                      <Settings2 className="h-4 w-4" />
                      <div className="font-medium">Filtros e colunas</div>
                    </div>
                    <div className="h-px bg-border my-2" />
                    <div className="space-y-2">
                      <Label className="text-sm text-muted-foreground">Quantidade de amostragem</Label>
                      <Select
                        value={String(limiteVagas)}
                        onValueChange={(v) => {
                          const next = Number(v);
                          setLimiteVagas(next);
                          handleAlterarLimiteVagas(next);
                        }}
                      >
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Mostrar vagas" />
                        </SelectTrigger>
                        <SelectContent>
                          {MAX_VAGAS_POR_COLUNA_OPTIONS.map((value) => (
                            <SelectItem key={value} value={String(value)}>
                              Ver {value} vagas
                            </SelectItem>
                          ))}
                          <SelectItem value="10000">Ver todas</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="h-px bg-border my-2" />
                    <label className="flex items-center gap-3 cursor-pointer py-1">
                      <Checkbox
                        checked={visualizarCandidatosPorEstagio}
                        onCheckedChange={(checked) => setVisualizarCandidatosPorEstagio(Boolean(checked))}
                      />
                      <span className="text-sm">Visualizar candidatos por estágio</span>
                    </label>
                    <div className="h-px bg-border my-2" />
                    <div className="space-y-2">
                      <span className="text-sm font-medium">Mostrar colunas</span>
                      <div className="space-y-2 max-h-[40vh] overflow-auto pr-1 mt-1">
                        {statusVagas.map((status) => (
                          <label key={status.id} className="flex items-center gap-3">
                            <Checkbox
                              checked={columnVisibility[status.id] !== false}
                              onCheckedChange={(checked) =>
                                setColumnVisibility((prev) => {
                                  const next = Boolean(checked);
                                  handleAlterarVisibilidadeColuna(status.id, next);
                                  return { ...prev, [status.id]: next };
                                })
                              }
                            />
                            <span className="text-sm">{status.nome}</span>
                          </label>
                        ))}
                      </div>
                    </div>
                  </PopoverContent>
                </Popover>
              </div>
            </div>

            {loading ? (
              <div className="flex flex-col items-center justify-center gap-2 py-12 text-muted-foreground">
                <Spinner size={24} aria-hidden />
                <span>Carregando vagas...</span>
              </div>
            ) : (
              <div
                ref={kanbanScrollRef}
                className={`flex gap-4 overflow-x-auto pb-2 select-none ${isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'}`}
                onMouseDown={handleKanbanBodyDragStart}
              >
                {statusColumns.map((status) => {
                  const statusId = status.id;
                  const vagasDaColuna = vagaCardsByStatus[statusId] || [];
                  const vagasVisiveis = vagasDaColuna.slice(0, Math.max(1, maxVagasPorColuna));
                  const count = totalVagasPorStatus[statusId] ?? vagasDaColuna.length;

                  return (
                    <div key={statusId} className="min-w-[320px] w-[320px] flex-shrink-0">
                      <div className="rounded-md border border-borderSoft bg-background">
                        <div
                          role="button"
                          tabIndex={0}
                          className={`px-4 py-3 border-b border-primary/30 bg-primary text-primary-foreground flex items-center justify-between rounded-t-md select-none ${isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'}`}
                          onMouseDown={handleKanbanHeaderDragStart}
                        >
                          <div className="font-semibold">{status.nome}</div>
                          <div className="text-sm text-primary-foreground/90">{count}</div>
                        </div>
                        <div
                          className={`p-3 space-y-3 max-h-[70vh] overflow-auto min-h-[120px] ${isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'}`}
                          onMouseDown={handleKanbanBodyDragStart}
                          data-status-id={statusId}
                          onDragOver={(e) => {
                            e.preventDefault();
                            e.dataTransfer.dropEffect = 'move';
                          }}
                          onDrop={(e) => {
                            e.preventDefault();
                            const statusTo = e.currentTarget.getAttribute('data-status-id');
                            const payload = dragPayloadRef.current;
                            if (statusTo && payload && payload.statusFrom !== statusTo) {
                              handleChangeStatus(payload.vagaId, payload.statusFrom, statusTo, payload.model);
                              dragPayloadRef.current = null;
                            }
                          }}
                        >
                          {vagasVisiveis.length > 0 ? (
                            vagasVisiveis.map((model) => (
                              <VagaCard
                                key={model.key}
                                model={model}
                                statusOptions={statusVagas}
                                onInfo={(raw) => {
                                  handleAbrirInformacoesVaga(model.vagaId);
                                  openVagaInfo(raw);
                                }}
                                onView={handleViewCandidatos}
                                onChangeStatus={handleChangeStatus}
                                onEditPerfil={handleEditarVagaParaCriarPerfil}
                                onEditarVaga={handleEditarVaga}
                                onInscrever={handleAbrirInscreverCandidato}
                                onAdicionarRecrutador={(m) => setAtribuirRecrutadorVaga(m)}
                                onCompartilhar={(m) => {
                                  const codigo = m.codigoVaga ?? (m.raw as { codigo?: number })?.codigo;
                                  const segmento = codigo != null ? String(codigo) : String(m.vagaId);
                                  navigate(`/recrutamento/detalhe/${segmento}`, {
                                    state: { vagaId: m.vagaId, codigoVaga: codigo },
                                  });
                                }}
                                onDragStartCard={(m) => {
                                  dragPayloadRef.current = { vagaId: String(m.vagaId), statusFrom: m.statusId, model: m };
                                }}
                                modoAdmin={modoAdmin && gruposSelecionadosIds.length > 0}
                                visivel={visibilidadeVagas}
                                onToggleVisibilidade={() => setVisibilidadeVagas((v) => !v)}
                                showCandidatosPorEstagio={visualizarCandidatosPorEstagio}
                              />
                            ))
                          ) : (
                            <div className="text-sm text-muted-foreground">Nenhuma vaga</div>
                          )}
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
      <InscreverCandidatoModal
        open={inscreverModalOpen}
        onOpenChange={setInscreverModalOpen}
        token={token}
        onSuccess={handleInscreverSuccess}
      />
      {inscreverVaga && codigoColaboradorInscrito && (
        <InscricaoPreferenciasModal
          open={preferenciasModalOpen}
          onOpenChange={handlePreferenciasClose}
          token={token}
          codigoVaga={
            inscreverVaga.codigoVaga ??
            (typeof (inscreverVaga.raw as Record<string, unknown>)?.codigo === 'number'
              ? (inscreverVaga.raw as Record<string, unknown>).codigo as number
              : Number((inscreverVaga.raw as Record<string, unknown>)?.codigo ?? inscreverVaga.vagaId) || 0)
          }
          vagaId={inscreverVaga.vagaId}
          vagaTitle={inscreverVaga.titulo}
          vagaRaw={inscreverVaga.raw as Record<string, unknown>}
          codigoColaborador={codigoColaboradorInscrito}
          onVerCandidaturas={handleVerCandidaturas}
          onInscricaoConcluida={carregarVagas}
        />
      )}
      <ImportarTalentosModal
        open={importarTalentosModalOpen}
        onOpenChange={setImportarTalentosModalOpen}
        token={token}
        onVerMinhasImportacoes={() => navigate('/recrutamento/uploads')}
      />
      <MeusCandidatosModal
        open={meusCandidatosModalOpen}
        onOpenChange={setMeusCandidatosModalOpen}
        token={token}
      />
      <AtribuirRecrutadorModal
        open={!!atribuirRecrutadorVaga}
        onOpenChange={(open) => !open && setAtribuirRecrutadorVaga(null)}
        vagaId={atribuirRecrutadorVaga ? String(atribuirRecrutadorVaga.vagaId) : ''}
        vagaTitulo={atribuirRecrutadorVaga?.titulo}
        currentRecrutadorNome={atribuirRecrutadorVaga?.recrutador}
        token={token}
        onSuccess={() => {
          setAtribuirRecrutadorVaga(null);
          carregarVagas();
        }}
      />
      {fluxoStatus12Payload && !showMovimentacaoModal && (
        <DuplicarVagaModal
          open={true}
          onOpenChange={(open) => !open && setFluxoStatus12Payload(null)}
          vagaIdParent={fluxoStatus12Payload.vagaId}
          tituloPerfil={fluxoStatus12Payload.titulo}
          token={token}
          onProsseguir={() => setShowMovimentacaoModal(true)}
          onEditarVaga={handleEditarVagaFilha}
        />
      )}
      {fluxoStatus12Payload && showMovimentacaoModal && (
        <MovimentacaoVagaModal
          open={true}
          onOpenChange={(open) => {
            if (!open) {
              setShowMovimentacaoModal(false);
              setFluxoStatus12Payload(null);
              carregarVagas(); // atualiza listagem ao fechar (cancelar/overlay)
            }
          }}
          vagaId={fluxoStatus12Payload.vagaId}
          codigoVaga={fluxoStatus12Payload.codigoVaga}
          tituloVaga={fluxoStatus12Payload.titulo}
          idPerfilGerador={fluxoStatus12Payload.idPerfilGerador}
          codigoCliente={fluxoStatus12Payload.codigoCliente}
          token={token}
          onSuccess={() => {
            setShowMovimentacaoModal(false);
            setFluxoStatus12Payload(null);
            carregarVagas(); // atualiza listagem após conclusão da movimentação
          }}
        />
      )}
      {perdaVagaPayload && (
        <PerdaVagaModal
          open={true}
          onOpenChange={(open) => !open && setPerdaVagaPayload(null)}
          codigoVaga={perdaVagaPayload.codigoVaga}
          token={token}
          onSuccess={() => {
            setPerdaVagaPayload(null);
            carregarVagas();
          }}
        />
      )}
    </TooltipProvider>
  );
}
