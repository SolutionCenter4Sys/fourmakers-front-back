import { useState, useRef, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from '@/components/ui/sheet';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Label } from '@/components/ui/label';
import { Progress } from '@/components/ui/progress';
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
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { TooltipProvider } from '@/components/ui/tooltip';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import {
  ArrowLeft,
  GripVertical,
  Send,
  Clock,
  User,
  FileText,
  CheckSquare,
  Receipt,
  Upload,
  Edit,
  CheckCircle2,
  X,
  AlertTriangle,
} from '@/components/ui/system-icons';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import type { Admission, AdmissionStageId, DocumentItem } from '@domain/entities/AdmissaoDigital';
import { ADMISSION_STAGES } from '@domain/entities/AdmissaoDigital';
import { formatDatePtBr } from '@presentation/hooks/recrutamento'

/** Checklist padrão CLT para simulação do portal do candidato. */
const DEFAULT_CLT_DOCUMENTS: DocumentItem[] = [
  { id: 'doc-oficial', name: 'Documento oficial com foto (RG/CNH)', status: 'Pendente', type: 'upload' },
  { id: 'doc-cpf', name: 'CPF', status: 'Pendente', type: 'data' },
  { id: 'doc-ctps', name: 'CTPS Digital', status: 'Pendente', type: 'data' },
  { id: 'doc-pis', name: 'PIS/NIS', status: 'Pendente', type: 'data' },
  { id: 'doc-residencia', name: 'Comprovante de residência', status: 'Pendente', type: 'upload' },
  { id: 'doc-bancarios', name: 'Dados bancários', status: 'Pendente', type: 'data' },
];

const CANDIDATE_STEPS = 6; // Boas-vindas, Documentos, Revisão, Assinatura, Integração, Conclusão
const DATA_INICIO_PREVISTA = '01/09/2025';
const SALARIO_MOCK = 'R$ 8.500,00';
const TIPO_CONTRATO = 'CLT (padrão)';

/** Mock: lista de admissões para primeira entrega (backend inexistente). */
function useMockAdmissions(): {
  admissions: Admission[];
  setAdmissions: React.Dispatch<React.SetStateAction<Admission[]>>;
} {
  const [admissions, setAdmissions] = useState<Admission[]>(() => [
    {
      id: 'ADM-001',
      candidateName: 'Maria Silva',
      vagaTitulo: 'Desenvolvedor React',
      clienteNome: 'Cliente Alpha',
      currentStage: 'carta-oferta-aceita',
      admissionStartedAt: null,
      progressoEtapa: 'Etapa 0/8',
      tempoInatividade: null,
      horasInatividade: null,
      documents: [],
      auditLog: [],
    },
    {
      id: 'ADM-002',
      candidateName: 'João Santos',
      vagaTitulo: 'Analista de RH',
      clienteNome: 'Cliente Beta',
      currentStage: 'carta-oferta-aceita',
      admissionStartedAt: null,
      progressoEtapa: 'Etapa 0/8',
      tempoInatividade: null,
      horasInatividade: null,
      documents: [],
      auditLog: [],
    },
    {
      id: 'ADM-003',
      candidateName: 'Ana Oliveira',
      vagaTitulo: 'Desenvolvedor React',
      clienteNome: 'Cliente Alpha',
      currentStage: 'coleta-dados',
      admissionStartedAt: '2025-02-20T10:00:00Z',
      progressoEtapa: 'Etapa 3/8',
      tempoInatividade: '1 dia',
      horasInatividade: 24,
      documents: [
        { id: 'doc-1', name: 'Documento oficial com foto', status: 'Aprovado', type: 'upload' },
        { id: 'doc-2', name: 'Comprovante de residência', status: 'Em revisão', type: 'upload' },
      ],
      auditLog: [
        { id: '1', usuario: 'Sistema', acao: 'Convite enviado', timestamp: '2025-02-20T10:00:00Z' },
      ],
    },
    {
      id: 'ADM-004',
      candidateName: 'Pedro Costa',
      vagaTitulo: 'Analista de RH',
      clienteNome: 'Cliente Beta',
      currentStage: 'validacao-rh',
      admissionStartedAt: '2025-02-18T14:00:00Z',
      progressoEtapa: 'Etapa 5/8',
      tempoInatividade: '2 dias',
      horasInatividade: 48,
      documents: [
        { id: 'doc-1', name: 'Documento oficial com foto', status: 'Aprovado', type: 'upload' },
        { id: 'doc-2', name: 'Comprovante de residência', status: 'Aprovado', type: 'upload' },
      ],
      auditLog: [
        { id: '1', usuario: 'Sistema', acao: 'Convite enviado', timestamp: '2025-02-18T14:00:00Z' },
        { id: '2', usuario: 'Carla RH', acao: 'Documento oficial aprovado', timestamp: '2025-02-19T09:00:00Z' },
      ],
    },
    {
      id: 'ADM-005',
      candidateName: 'Carla Mendes',
      vagaTitulo: 'Desenvolvedor React',
      clienteNome: 'Cliente Alpha',
      currentStage: 'contrato-assinatura',
      admissionStartedAt: '2025-02-15T08:00:00Z',
      progressoEtapa: 'Etapa 7/8',
      tempoInatividade: '5 horas',
      horasInatividade: 5,
      documents: [],
      auditLog: [],
    },
  ]);
  return { admissions, setAdmissions };
}

const SLA_VERDE_HORAS = 24;
const SLA_AMARELO_HORAS = 48;

function getSlaVariant(horas: number | null | undefined): 'success' | 'warning' | 'destructive' | 'secondary' {
  if (horas == null) return 'secondary';
  if (horas <= SLA_VERDE_HORAS) return 'success';
  if (horas <= SLA_AMARELO_HORAS) return 'warning';
  return 'destructive';
}

interface AdmissionCardProps {
  admission: Admission;
  stageId: AdmissionStageId;
  onEnviarConvite?: (admission: Admission) => void;
  onOpenDetail?: (admission: Admission) => void;
  onDragStart?: (admissionId: string, stageFrom: AdmissionStageId) => void;
}

function AdmissionCard({ admission, stageId, onEnviarConvite, onOpenDetail, onDragStart }: AdmissionCardProps) {
  const isCartaOfertaAceita = stageId === 'carta-oferta-aceita';
  const conviteNaoEnviado = admission.admissionStartedAt == null;
  const showEnviarConvite = isCartaOfertaAceita && conviteNaoEnviado && onEnviarConvite;

  const slaVariant = getSlaVariant(admission.horasInatividade);
  const slaLabel =
    admission.horasInatividade != null
      ? admission.horasInatividade <= SLA_VERDE_HORAS
        ? 'No prazo'
        : admission.horasInatividade <= SLA_AMARELO_HORAS
          ? 'Atenção'
          : 'Atrasado'
      : null;

  return (
    <Card
      className="shadow-none border-borderSoft cursor-pointer hover:border-primary/40 transition-colors relative"
      data-kanban-card
      onClick={() => onOpenDetail?.(admission)}
    >
      <CardContent className="p-4 space-y-3">
        {onDragStart && (
          <div
            role="button"
            tabIndex={0}
            draggable
            onDragStart={(e) => {
              e.stopPropagation();
              e.dataTransfer.setData('text/plain', 'admission');
              e.dataTransfer.effectAllowed = 'move';
              onDragStart(admission.id, admission.currentStage);
              const cardEl = (e.currentTarget as HTMLElement).closest('[data-kanban-card]') as HTMLElement | null;
              if (cardEl) {
                const rect = cardEl.getBoundingClientRect();
                const clone = cardEl.cloneNode(true) as HTMLElement;
                clone.style.opacity = '0.95';
                clone.style.pointerEvents = 'none';
                clone.style.position = 'absolute';
                clone.style.left = '-9999px';
                clone.style.width = `${rect.width}px`;
                document.body.appendChild(clone);
                e.dataTransfer.setDragImage(clone, rect.width - 24, 16);
                setTimeout(() => clone.remove(), 0);
              }
            }}
            onClick={(e) => e.stopPropagation()}
            className="absolute top-2 right-2 p-1 rounded cursor-grab active:cursor-grabbing text-muted-foreground hover:bg-muted/50 z-10"
            aria-label="Arrastar para alterar coluna"
          >
            <GripVertical className="h-4 w-4" />
          </div>
        )}

        <div className="flex items-start gap-3">
          <Avatar className="h-10 w-10 shrink-0">
            <AvatarImage src={admission.candidatePhotoUrl ?? undefined} alt={admission.candidateName} />
            <AvatarFallback className="bg-primary/10 text-primary text-sm">
              {admission.candidateName.slice(0, 2).toUpperCase()}
            </AvatarFallback>
          </Avatar>
          <div className="min-w-0 flex-1">
            <p className="font-medium text-sm truncate">{admission.candidateName}</p>
            <p className="text-xs text-muted-foreground truncate">{admission.vagaTitulo}</p>
            <p className="text-xs text-muted-foreground truncate">{admission.clienteNome}</p>
          </div>
        </div>

        <div className="flex items-center justify-between text-xs">
          <span className="text-muted-foreground">{admission.progressoEtapa}</span>
          {admission.tempoInatividade && (
            <span className="flex items-center gap-1 text-muted-foreground">
              <Clock className="h-3 w-3" />
              {admission.tempoInatividade}
            </span>
          )}
        </div>

        {slaLabel && (
          <div className="flex items-center gap-1.5">
            <span
              className={cn(
                'h-2 w-2 rounded-full shrink-0',
                slaVariant === 'success' && 'bg-success',
                slaVariant === 'warning' && 'bg-warning',
                slaVariant === 'destructive' && 'bg-destructive',
                slaVariant === 'secondary' && 'bg-muted-foreground'
              )}
            />
            <span className="text-xs text-muted-foreground">{slaLabel}</span>
          </div>
        )}

        {showEnviarConvite && (
          <Button
            size="sm"
            className="w-full mt-1"
            onClick={(e) => {
              e.stopPropagation();
              onEnviarConvite(admission);
            }}
          >
            <Send className="h-4 w-4 mr-1" />
            Enviar Convite
          </Button>
        )}
      </CardContent>
    </Card>
  );
}

type ViewMode = 'recruiter' | 'candidate';

export default function GestaoVagasAdmissao() {
  const { admissions, setAdmissions } = useMockAdmissions();
  const [viewMode, setViewMode] = useState<ViewMode>('recruiter');
  const [candidateStep, setCandidateStep] = useState(0);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedAdmission, setSelectedAdmission] = useState<Admission | null>(null);
  const [enviarConviteModal, setEnviarConviteModal] = useState<Admission | null>(null);
  const [comoEnviarOpen, setComoEnviarOpen] = useState(false);
  const [docEmEnvio, setDocEmEnvio] = useState<string | null>(null);
  const [isKanbanDragging, setIsKanbanDragging] = useState(false);
  const dragPayloadRef = useRef<{ admissionId: string; stageFrom: AdmissionStageId } | null>(null);
  const kanbanScrollRef = useRef<HTMLDivElement>(null);
  const kanbanDragStartX = useRef(0);
  const kanbanDragStartScroll = useRef(0);

  const admissionsPorStage = (stageId: AdmissionStageId) =>
    admissions.filter((a) => a.currentStage === stageId);

  /** Candidato usado na simulação (primeiro da coluna Carta-Oferta Aceita). */
  const candidateAdmission =
    admissions.find((a) => a.currentStage === 'carta-oferta-aceita') ??
    admissions.find((a) => a.currentStage === 'coleta-dados') ??
    admissions[0];

  const updateCandidateAdmission = useCallback(
    (updater: (a: Admission) => Admission) => {
      if (!candidateAdmission) return;
      setAdmissions((prev) =>
        prev.map((a) => (a.id === candidateAdmission.id ? updater(a) : a))
      );
    },
    [candidateAdmission?.id, setAdmissions]
  );

  const handleOpenDetail = useCallback((admission: Admission) => {
    setSelectedAdmission(admission);
    setDrawerOpen(true);
  }, []);

  const handleEnviarConviteClick = useCallback((admission: Admission) => {
    setEnviarConviteModal(admission);
  }, []);

  const handleEnviarConviteConfirm = useCallback(() => {
    const admission = enviarConviteModal;
    if (!admission) return;
    setAdmissions((prev) =>
      prev.map((a) =>
        a.id === admission.id
          ? {
              ...a,
              currentStage: 'coleta-dados' as AdmissionStageId,
              admissionStartedAt: new Date().toISOString(),
              progressoEtapa: 'Etapa 1/8',
              auditLog: [
                ...a.auditLog,
                {
                  id: `log-${Date.now()}`,
                  usuario: 'Usuário atual',
                  acao: 'Convite enviado',
                  timestamp: new Date().toISOString(),
                  detalhe: `Convite enviado por Usuário atual em ${formatDatePtBr(new Date().toISOString())}`,
                },
              ],
            }
          : a
      )
    );
    setEnviarConviteModal(null);
    toast.success('Convite enviado. O candidato foi movido para Coleta de Dados.');
  }, [enviarConviteModal, setAdmissions]);

  const handleDrop = useCallback(
    (stageTo: AdmissionStageId) => {
      const payload = dragPayloadRef.current;
      if (!payload || payload.stageFrom === stageTo) return;
      setAdmissions((prev) =>
        prev.map((a) =>
          a.id === payload.admissionId ? { ...a, currentStage: stageTo } : a
        )
      );
      dragPayloadRef.current = null;
      toast.success('Admissão movida.');
    },
    [setAdmissions]
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
      if (e.button !== 0 || !kanbanScrollRef.current) return;
      if ((e.target as HTMLElement).closest('button')) return;
      e.preventDefault();
      kanbanDragStartX.current = e.clientX;
      kanbanDragStartScroll.current = kanbanScrollRef.current.scrollLeft;
      setIsKanbanDragging(true);
      attachKanbanDragListeners();
    },
    [attachKanbanDragListeners]
  );

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

  return (
    <TooltipProvider>
      <div className="w-full max-w-none mx-auto p-4 space-y-4">
        <PageBreadcrumb
          items={[
            { label: 'Recrutamento', href: '/recrutamento' },
            { label: 'Admissão Digital' },
          ]}
        />
        <PageHeader
          title="Admissão Digital"
          description={
            viewMode === 'recruiter'
              ? 'Painel de controle do fluxo de admissão. Arraste os cards para mover entre estágios.'
              : `Simulando portal do candidato: ${candidateAdmission?.candidateName ?? '—'}`
          }
          actions={
            <div className="flex items-center gap-3">
              <div className="flex items-center gap-2">
                <Label htmlFor="visao-admissao" className="text-sm text-muted-foreground whitespace-nowrap">
                  Visão
                </Label>
                <Select
                  value={viewMode}
                  onValueChange={(v) => {
                    setViewMode(v as ViewMode);
                    if (v === 'recruiter') setCandidateStep(0);
                  }}
                >
                  <SelectTrigger id="visao-admissao" className="w-[180px]">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="recruiter">Recrutador</SelectItem>
                    <SelectItem value="candidate">Candidato</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <Button variant="ghost" size="icon" asChild aria-label="Voltar para Recrutamento">
                <Link to="/recrutamento">
                  <ArrowLeft className="h-5 w-5" />
                </Link>
              </Button>
            </div>
          }
        />

        {viewMode === 'candidate' && candidateAdmission ? (
          <CandidateFlowView
            admission={candidateAdmission}
            step={candidateStep}
            onStepChange={setCandidateStep}
            onUpdate={updateCandidateAdmission}
            onBackToRecruiter={() => {
              setViewMode('recruiter');
              setCandidateStep(0);
            }}
            onEnviarDocClick={(docId) => {
              setDocEmEnvio(docId);
              setComoEnviarOpen(true);
            }}
            onPreencherDoc={(docId) => {
              updateCandidateAdmission((a) => ({
                ...a,
                documents: a.documents.map((d) =>
                  d.id === docId ? { ...d, status: 'Aprovado' as const } : d
                ),
              }));
              toast.success('Dados preenchidos e salvos.');
            }}
          />
        ) : (
        <>
        <Card className="border-borderSoft">
          <CardContent className="p-4">
            <div
              ref={kanbanScrollRef}
              className={cn('flex gap-4 overflow-x-auto pb-2', isKanbanDragging && 'cursor-grabbing')}
              style={{ minHeight: '420px' }}
            >
              {ADMISSION_STAGES.map((stage) => {
                const lista = admissionsPorStage(stage.id);
                return (
                  <div key={stage.id} className="min-w-[300px] w-[300px] flex-shrink-0 flex flex-col">
                    <div
                      role="button"
                      tabIndex={0}
                      className={cn(
                        'rounded-t-md border border-b-0 border-borderSoft bg-primary text-primary-foreground px-4 py-3 flex items-center justify-between select-none',
                        isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'
                      )}
                      onMouseDown={handleKanbanHeaderDragStart}
                    >
                      <span className="font-semibold text-sm">{stage.nome}</span>
                      <span className="text-sm text-primary-foreground/90">{lista.length}</span>
                    </div>
                    <div
                      className={cn(
                        'flex-1 rounded-b-md border border-borderSoft border-t-0 p-3 space-y-3 overflow-y-auto min-h-[320px]',
                        'bg-muted/5',
                        isKanbanDragging ? 'cursor-grabbing' : 'cursor-grab'
                      )}
                      data-stage-id={stage.id}
                      onMouseDown={handleKanbanBodyDragStart}
                      onDragOver={(e) => {
                        e.preventDefault();
                        e.dataTransfer.dropEffect = 'move';
                      }}
                      onDrop={(e) => {
                        e.preventDefault();
                        const stageTo = e.currentTarget.getAttribute('data-stage-id') as AdmissionStageId | null;
                        if (stageTo) handleDrop(stageTo);
                      }}
                    >
                      {lista.length > 0 ? (
                        lista.map((adm) => (
                          <AdmissionCard
                            key={adm.id}
                            admission={adm}
                            stageId={stage.id}
                            onEnviarConvite={handleEnviarConviteClick}
                            onOpenDetail={handleOpenDetail}
                            onDragStart={(id, from) => {
                              dragPayloadRef.current = { admissionId: id, stageFrom: from };
                            }}
                          />
                        ))
                      ) : (
                        <div className="flex flex-col items-center justify-center py-8 text-center text-sm text-muted-foreground">
                          <User className="h-8 w-8 mb-2 opacity-50" />
                          <span>Nenhuma admissão nesta etapa</span>
                        </div>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>

        {/* Modal: Enviar Convite */}
        <AlertDialog open={enviarConviteModal != null} onOpenChange={(open) => !open && setEnviarConviteModal(null)}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Iniciar contratação e enviar convite</AlertDialogTitle>
              <AlertDialogDescription>
                Ao enviar o convite, o candidato terá acesso ao portal para preencher dados e enviar documentos.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancelar</AlertDialogCancel>
              <AlertDialogAction onClick={handleEnviarConviteConfirm} className="bg-primary text-primary-foreground hover:bg-primary/90">
                Enviar convite
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>

        {/* Drawer: Detalhes da Admissão */}
        <Sheet open={drawerOpen} onOpenChange={setDrawerOpen}>
          <SheetContent className="w-full sm:max-w-xl overflow-y-auto">
            <SheetHeader>
              <SheetTitle className="flex items-center gap-2">
                {selectedAdmission && (
                  <>
                    <Avatar className="h-8 w-8">
                      <AvatarImage src={selectedAdmission.candidatePhotoUrl ?? undefined} />
                      <AvatarFallback className="text-xs">
                        {selectedAdmission.candidateName.slice(0, 2).toUpperCase()}
                      </AvatarFallback>
                    </Avatar>
                    {selectedAdmission.candidateName}
                  </>
                )}
              </SheetTitle>
            </SheetHeader>
            {selectedAdmission && (
              <div className="mt-6">
                <Tabs defaultValue="documentos" className="w-full">
                  <TabsList className="grid w-full grid-cols-3">
                    <TabsTrigger value="documentos" className="flex items-center gap-1.5">
                      <FileText className="h-4 w-4" />
                      Documentos
                    </TabsTrigger>
                    <TabsTrigger value="fluxo" className="flex items-center gap-1.5">
                      <CheckSquare className="h-4 w-4" />
                      Fluxo de Etapas
                    </TabsTrigger>
                    <TabsTrigger value="eventos" className="flex items-center gap-1.5">
                      <Receipt className="h-4 w-4" />
                      Eventos & Logs
                    </TabsTrigger>
                  </TabsList>
                  <TabsContent value="documentos" className="mt-4 space-y-3">
                    {selectedAdmission.admissionStartedAt == null ? (
                      <p className="text-sm text-muted-foreground py-4 rounded-lg bg-muted/30 border border-borderSoft px-4">
                        Envie o convite ao candidato para liberar a coleta de documentos. Esta seção ficará disponível após o envio.
                      </p>
                    ) : selectedAdmission.documents.length === 0 ? (
                      <p className="text-sm text-muted-foreground">Nenhum documento no checklist ainda.</p>
                    ) : (
                      <ul className="space-y-2">
                        {selectedAdmission.documents.map((doc) => (
                          <li
                            key={doc.id}
                            className="flex items-center justify-between py-2 px-3 rounded-md border border-borderSoft bg-background"
                          >
                            <span className="text-sm font-medium">{doc.name}</span>
                            <Badge
                              variant={
                                doc.status === 'Aprovado'
                                  ? 'default'
                                  : doc.status === 'Reprovado'
                                    ? 'destructive'
                                    : doc.status === 'Em revisão'
                                      ? 'secondary'
                                      : 'outline'
                              }
                              className={cn(
                                doc.status === 'Aprovado' && 'bg-success/10 text-success hover:bg-success/20'
                              )}
                            >
                              {doc.status}
                            </Badge>
                          </li>
                        ))}
                      </ul>
                    )}
                  </TabsContent>
                  <TabsContent value="fluxo" className="mt-4">
                    <div className="space-y-2">
                      {ADMISSION_STAGES.map((s) => (
                        <div
                          key={s.id}
                          className={cn(
                            'flex items-center gap-3 py-2 px-3 rounded-md border',
                            s.id === selectedAdmission.currentStage
                              ? 'border-primary bg-primary/5'
                              : 'border-borderSoft bg-muted/20'
                          )}
                        >
                          <span className="text-sm font-medium w-8">{s.ordem}.</span>
                          <span className="text-sm">{s.nome}</span>
                          {s.id === selectedAdmission.currentStage && (
                            <Badge variant="secondary" className="ml-auto text-xs">Atual</Badge>
                          )}
                        </div>
                      ))}
                    </div>
                  </TabsContent>
                  <TabsContent value="eventos" className="mt-4 space-y-2">
                    {selectedAdmission.auditLog.length === 0 ? (
                      <p className="text-sm text-muted-foreground">Nenhum evento registrado.</p>
                    ) : (
                      <ul className="space-y-2">
                        {[...selectedAdmission.auditLog].reverse().map((entry) => (
                          <li key={entry.id} className="text-sm py-2 px-3 rounded-md border border-borderSoft bg-muted/10">
                            <span className="font-medium">{entry.acao}</span>
                            <span className="text-muted-foreground"> — {entry.usuario}</span>
                            <span className="text-muted-foreground text-xs block mt-0.5">
                              {formatDatePtBr(entry.timestamp)}
                            </span>
                          </li>
                        ))}
                      </ul>
                    )}
                  </TabsContent>
                </Tabs>
              </div>
            )}
          </SheetContent>
        </Sheet>
        </>
        )}

        {/* Modal: Como você quer enviar? (visão candidato) */}
        <Dialog open={comoEnviarOpen} onOpenChange={setComoEnviarOpen}>
          <DialogContent className="sm:max-w-md">
            <DialogHeader>
              <DialogTitle>Como você quer enviar?</DialogTitle>
              <DialogDescription>
                Você pode tirar uma foto com seu celular ou enviar um arquivo existente.
              </DialogDescription>
            </DialogHeader>
            <div className="grid grid-cols-2 gap-4 py-4">
              <Button
                variant="outline"
                className="h-auto flex flex-col gap-2 py-6"
                onClick={() => {
                  setComoEnviarOpen(false);
                  if (docEmEnvio && candidateAdmission) {
                    updateCandidateAdmission((a) => ({
                      ...a,
                      documents: a.documents.map((d) =>
                        d.id === docEmEnvio ? { ...d, status: 'Em revisão' as const } : d
                      ),
                    }));
                    toast.success('Documento enviado. Em breve o RH validará.');
                  }
                  setDocEmEnvio(null);
                }}
              >
                <span className="text-2xl">📷</span>
                Tirar foto
              </Button>
              <Button
                variant="outline"
                className="h-auto flex flex-col gap-2 py-6"
                onClick={() => {
                  setComoEnviarOpen(false);
                  if (docEmEnvio && candidateAdmission) {
                    updateCandidateAdmission((a) => ({
                      ...a,
                      documents: a.documents.map((d) =>
                        d.id === docEmEnvio ? { ...d, status: 'Aprovado' as const } : d
                      ),
                    }));
                    toast.success('Arquivo enviado e aprovado na simulação.');
                  }
                  setDocEmEnvio(null);
                }}
              >
                <span className="text-2xl">📄</span>
                Enviar arquivo
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </div>
    </TooltipProvider>
  );
}

/** Visão do portal do candidato: fluxo linear por etapas. */
interface CandidateFlowViewProps {
  admission: Admission;
  step: number;
  onStepChange: (step: number) => void;
  onUpdate: (updater: (a: Admission) => Admission) => void;
  onBackToRecruiter: () => void;
  onEnviarDocClick: (docId: string) => void;
  onPreencherDoc: (docId: string) => void;
}

function CandidateFlowView({
  admission,
  step,
  onStepChange,
  onUpdate,
  onBackToRecruiter,
  onEnviarDocClick,
  onPreencherDoc,
}: CandidateFlowViewProps) {
  const progressPercent = (step / (CANDIDATE_STEPS - 1)) * 100;
  const isFirstStep = step === 0;
  const isLastStep = step === CANDIDATE_STEPS - 1;

  const handleComecar = () => {
    onUpdate((a) => ({
      ...a,
      admissionStartedAt: new Date().toISOString(),
      currentStage: 'coleta-dados',
      progressoEtapa: 'Etapa 1/8',
      documents: DEFAULT_CLT_DOCUMENTS.map((d) => ({ ...d })),
      auditLog: [
        ...a.auditLog,
        {
          id: `log-${Date.now()}`,
          usuario: 'Sistema',
          acao: 'Convite enviado',
          timestamp: new Date().toISOString(),
        },
      ],
    }));
    onStepChange(1);
    toast.success('Convite ativado. Preencha seus dados e documentos.');
  };

  const handleConfirmarRevisao = () => onStepChange(3);
  const handleAssinar = () => {
    onUpdate((a) => ({
      ...a,
      currentStage: 'integracao',
      progressoEtapa: 'Etapa 8/8',
      auditLog: [
        ...a.auditLog,
        {
          id: `log-${Date.now()}`,
          usuario: admission.candidateName,
          acao: 'Contrato assinado digitalmente',
          timestamp: new Date().toISOString(),
        },
      ],
    }));
    onStepChange(4);
    toast.success('Contrato assinado com sucesso.');
  };

  const docsAprovados = admission.documents.filter((d) => d.status === 'Aprovado').length;
  const docsTotal = admission.documents.length;
  const podeAvancarDocumentos = docsAprovados >= Math.min(1, docsTotal) || step !== 1;

  return (
    <Card className="border-borderSoft overflow-hidden">
      <div className="bg-muted/30 px-6 py-4 border-b border-borderSoft flex items-center justify-between">
        <div>
          <h2 className="text-lg font-semibold">
            Admissão Digital — {admission.clienteNome}
          </h2>
          <p className="text-sm text-muted-foreground">
            Um processo simples e guiado para sua contratação.
          </p>
        </div>
        <Button variant="ghost" size="icon" onClick={onBackToRecruiter} aria-label="Fechar e voltar ao painel">
          <X className="h-5 w-5" />
        </Button>
      </div>
      <div className="px-6 pt-4">
        <Progress value={progressPercent} className="h-2 mb-6" />
      </div>
      <CardContent className="px-6 pb-8 space-y-6">
        {/* Step 0: Boas-vindas */}
        {step === 0 && (
          <div className="text-center space-y-6 py-4">
            <div className="text-4xl">🎉</div>
            <h3 className="text-xl font-semibold">
              Sua admissão está quase pronta, {admission.candidateName}!
            </h3>
            <p className="text-muted-foreground max-w-md mx-auto">
              Faltam apenas alguns passos para finalizar sua contratação na {admission.clienteNome} para a vaga de {admission.vagaTitulo}. O processo leva apenas alguns minutos.
            </p>
            <p className="text-sm font-medium">Data de início prevista: {DATA_INICIO_PREVISTA}</p>
            <Button size="lg" onClick={handleComecar}>
              Começar →
            </Button>
          </div>
        )}

        {/* Step 1: Seus Dados e Documentos */}
        {step === 1 && (
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">Seus Dados e Documentos</h3>
            <p className="text-sm text-muted-foreground">
              Preencha os campos e envie os documentos solicitados. Itens que precisam de ajuste estarão destacados.
            </p>
            <ul className="space-y-3">
              {admission.documents.map((doc) => (
                <li
                  key={doc.id}
                  className={cn(
                    'flex items-center justify-between gap-4 p-4 rounded-lg border',
                    doc.status === 'Reprovado' ? 'border-warning bg-warning/5' : 'border-borderSoft bg-background'
                  )}
                >
                  <div className="min-w-0 flex-1">
                    <p className="font-medium text-sm">{doc.name}</p>
                    {doc.status === 'Aprovado' && (
                      <p className="text-xs text-success flex items-center gap-1 mt-1">
                        <CheckCircle2 className="h-4 w-4" />
                        Documento aprovado!
                      </p>
                    )}
                    {doc.status === 'Reprovado' && doc.rejectionReason && (
                      <p className="text-xs text-destructive flex items-center gap-1 mt-1">
                        <AlertTriangle className="h-4 w-4" />
                        {doc.rejectionReason}
                      </p>
                    )}
                  </div>
                  <div className="shrink-0">
                    {doc.status === 'Pendente' && (
                      <div className="flex gap-2">
                        {doc.type === 'upload' ? (
                          <Button size="sm" variant="outline" onClick={() => onEnviarDocClick(doc.id)}>
                            <Upload className="h-4 w-4 mr-1" />
                            Enviar
                          </Button>
                        ) : (
                          <Button size="sm" variant="outline" onClick={() => onPreencherDoc(doc.id)}>
                            <Edit className="h-4 w-4 mr-1" />
                            Preencher
                          </Button>
                        )}
                      </div>
                    )}
                    {doc.status === 'Em revisão' && (
                      <Badge variant="secondary">Em revisão</Badge>
                    )}
                    {doc.status === 'Reprovado' && (
                      <Button size="sm" variant="outline" onClick={() => onEnviarDocClick(doc.id)}>
                        Corrigir
                      </Button>
                    )}
                  </div>
                </li>
              ))}
            </ul>
          </div>
        )}

        {/* Step 2: Revisão dos dados */}
        {step === 2 && (
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">Revise os dados</h3>
            <p className="text-sm text-muted-foreground">
              Revise os dados que você preencheu. Se algo estiver incorreto, você pode voltar para corrigir.
            </p>
            <div className="grid gap-4 md:grid-cols-2">
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <h4 className="font-semibold text-sm mb-2">Dados Pessoais</h4>
                  <p className="text-sm text-muted-foreground">Nome: {admission.candidateName}</p>
                  <p className="text-sm text-muted-foreground">CPF: ***.***.***-00</p>
                  <p className="text-sm text-muted-foreground">Documento: Preenchido</p>
                </CardContent>
              </Card>
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <h4 className="font-semibold text-sm mb-2">Dados Bancários</h4>
                  <p className="text-sm text-muted-foreground">Banco: A definir</p>
                  <p className="text-sm text-muted-foreground">Agência / Conta: A definir</p>
                </CardContent>
              </Card>
            </div>
          </div>
        )}

        {/* Step 3: Assinatura */}
        {step === 3 && (
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">Assinatura do contrato</h3>
            <p className="text-sm text-muted-foreground">
              Agora precisamos da sua assinatura para formalizar a contratação. Revise os detalhes abaixo e prossiga para a assinatura digital segura.
            </p>
            <div className="grid grid-cols-2 gap-4">
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Cargo</p>
                  <p className="font-medium">{admission.vagaTitulo}</p>
                </CardContent>
              </Card>
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Tipo de Contrato</p>
                  <p className="font-medium">{TIPO_CONTRATO}</p>
                </CardContent>
              </Card>
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Salário</p>
                  <p className="font-medium">{SALARIO_MOCK}</p>
                </CardContent>
              </Card>
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Data de Início</p>
                  <p className="font-medium">{DATA_INICIO_PREVISTA}</p>
                </CardContent>
              </Card>
            </div>
            <Button size="lg" onClick={handleAssinar}>
              Assinar Contrato Digitalmente
            </Button>
          </div>
        )}

        {/* Step 4: Integração */}
        {step === 4 && (
          <div className="space-y-6 text-center py-4">
            <div className="text-4xl">🚀</div>
            <h3 className="text-xl font-semibold">Sua integração está em andamento!</h3>
            <p className="text-muted-foreground">
              Contrato assinado com sucesso. Agora estamos preparando tudo para sua chegada.
            </p>
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 text-left">
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Acessos</p>
                  <p className="font-medium text-sm">Em preparação</p>
                </CardContent>
              </Card>
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Gestor(a)</p>
                  <p className="font-medium text-sm">RH — {admission.clienteNome}</p>
                </CardContent>
              </Card>
              <Card className="border-borderSoft">
                <CardContent className="p-4">
                  <p className="text-xs text-muted-foreground">Data de Início</p>
                  <p className="font-medium text-sm">{DATA_INICIO_PREVISTA}</p>
                </CardContent>
              </Card>
            </div>
          </div>
        )}

        {/* Step 5: Conclusão */}
        {step === 5 && (
          <div className="space-y-6 text-center py-4">
            <h3 className="text-2xl font-semibold">Tudo certo!</h3>
            <p className="text-muted-foreground">
              Seja bem-vindo(a) à {admission.clienteNome}! Estamos felizes em ter você no time.
            </p>
            <p className="text-sm text-muted-foreground">
              Em breve você receberá um e-mail com seus acessos. Seu gestor ou um colega entrará em contato para alinhar os primeiros dias.
            </p>
            <Button size="lg" onClick={onBackToRecruiter}>
              Fechar Portal
            </Button>
          </div>
        )}

        {/* Navegação */}
        <div className="flex items-center justify-between pt-6 border-t border-borderSoft">
          <Button
            variant="outline"
            disabled={isFirstStep}
            onClick={() => onStepChange(Math.max(0, step - 1))}
          >
            Anterior
          </Button>
          {!isLastStep && (
            <Button
              onClick={() => {
                if (step === 1 && !podeAvancarDocumentos) {
                  toast.info('Complete ao menos alguns documentos para seguir.');
                  return;
                }
                if (step === 2) handleConfirmarRevisao();
                else onStepChange(step + 1);
              }}
            >
            {step === 2 ? 'Confirmar e Continuar' : 'Próximo'}
            </Button>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
