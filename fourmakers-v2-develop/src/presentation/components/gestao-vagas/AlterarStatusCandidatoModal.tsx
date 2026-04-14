import { useState, useEffect, useRef } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { AlertCircle, FileText, PersonRemove, Upload, X } from '@/components/ui/system-icons';
import { CurrencyInput } from '@presentation/components/simulator/ui/SimulatorUI';
import type { MudarStatusCandidaturaRetorno } from '@domain/entities/GestaoVagasCandidatos';
import { ListarModelosTrabalhoUseCase } from '@domain/usecases/ListarModelosTrabalhoUseCase';
import { ListarMotivosReprovacaoUseCase } from '@domain/usecases/ListarMotivosReprovacaoUseCase';
import { ReprovarCandidaturaUseCase } from '@domain/usecases/ReprovarCandidaturaUseCase';
import { ListarMotivosDeclinioUseCase } from '@domain/usecases/ListarMotivosDeclinioUseCase';
import { DeclinarCandidatoUseCase } from '@domain/usecases/DeclinarCandidatoUseCase';
import { AtualizarCandidaturaUseCase } from '@domain/usecases/AtualizarCandidaturaUseCase';
import { MudarStatusCandidaturaUseCase } from '@domain/usecases/MudarStatusCandidaturaUseCase';
import { InserirArquivoCandidaturaUseCase } from '@domain/usecases/InserirArquivoCandidaturaUseCase';
import { toast } from 'sonner';

export interface AlterarStatusCandidatoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  idCandidatura: string;
  codigoStatus: number;
  /** Nome do candidato (opcional, para exibição). */
  nomeCandidato?: string;
  /** Nome do status de destino (opcional). */
  statusNome?: string;
  /** Chamado após sucesso na API; recebe o retorno (candidaturaId, comentarioId) para próximas ações. */
  onSuccess?: (retorno: MudarStatusCandidaturaRetorno) => void;
}

const STATUS_CODIGO_QUALIFICADO = 3;
const STATUS_CODIGO_REPROVADO = 10;
const STATUS_CODIGO_DECLINIO = 12;
const DIAS_PRESENCIAIS_OPCOES = [
  { value: 1, label: '1 dia presencial' },
  { value: 2, label: '2 dias presenciais' },
  { value: 3, label: '3 dias presenciais' },
  { value: 4, label: '4 dias presenciais' },
] as const;

export function AlterarStatusCandidatoModal({
  open,
  onOpenChange,
  token,
  idCandidatura,
  codigoStatus,
  onSuccess,
}: AlterarStatusCandidatoModalProps) {
  const [comentario, setComentario] = useState('');
  const [comentarioError, setComentarioError] = useState(false);
  const [saving, setSaving] = useState(false);
  const [file, setFile] = useState<File | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const isStatus3 = codigoStatus === STATUS_CODIGO_QUALIFICADO;
  const [modeloTrabalhoId, setModeloTrabalhoId] = useState('');
  const [quantidadeDiasPresencial, setQuantidadeDiasPresencial] = useState<number>(1);
  const [pretencaoSalarialNum, setPretencaoSalarialNum] = useState<number>(0);
  const [status3Errors, setStatus3Errors] = useState<{ modelo?: boolean; dias?: boolean; pretencao?: boolean }>({});
  const [modelosTrabalho, setModelosTrabalho] = useState<import('@domain/entities/GestaoVagasCandidatos').ModeloTrabalho[]>([]);
  const [loadingModelos, setLoadingModelos] = useState(false);

  const isStatus10 = codigoStatus === STATUS_CODIGO_REPROVADO;
  const [motivosReprovacao, setMotivosReprovacao] = useState<import('@domain/entities/GestaoVagasCandidatos').MotivoReprovacao[]>([]);
  const [idMotivoReprovacao, setIdMotivoReprovacao] = useState('');
  const [motivoError, setMotivoError] = useState(false);
  const [loadingMotivos, setLoadingMotivos] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [showSuccessModal, setShowSuccessModal] = useState(false);

  const isStatus12 = codigoStatus === STATUS_CODIGO_DECLINIO;
  const [motivosDeclinio, setMotivosDeclinio] = useState<import('@domain/entities/GestaoVagasCandidatos').MotivoDeclinio[]>([]);
  const [idMotivoDeclinio, setIdMotivoDeclinio] = useState('');
  const [motivoDeclinioError, setMotivoDeclinioError] = useState(false);
  const [loadingMotivosDeclinio, setLoadingMotivosDeclinio] = useState(false);

  const listarModelosTrabalhoUseCase = container.resolve(ListarModelosTrabalhoUseCase);
  const listarMotivosReprovacaoUseCase = container.resolve(ListarMotivosReprovacaoUseCase);
  const reprovarCandidaturaUseCase = container.resolve(ReprovarCandidaturaUseCase);
  const listarMotivosDeclinioUseCase = container.resolve(ListarMotivosDeclinioUseCase);
  const declinarCandidatoUseCase = container.resolve(DeclinarCandidatoUseCase);
  const atualizarCandidaturaUseCase = container.resolve(AtualizarCandidaturaUseCase);
  const mudarStatusCandidaturaUseCase = container.resolve(MudarStatusCandidaturaUseCase);
  const inserirArquivoCandidaturaUseCase = container.resolve(InserirArquivoCandidaturaUseCase);

  const selectedModelo = modelosTrabalho.find((m) => m.id === modeloTrabalhoId);
  const isHibrido = selectedModelo?.codigo === 2;

  useEffect(() => {
    if (!open) {
      setComentario('');
      setComentarioError(false);
      setFile(null);
      setModeloTrabalhoId('');
      setQuantidadeDiasPresencial(1);
      setPretencaoSalarialNum(0);
      setStatus3Errors({});
      setModelosTrabalho([]);
      setMotivosReprovacao([]);
      setIdMotivoReprovacao('');
      setMotivoError(false);
      setSuccessMessage('');
      setShowSuccessModal(false);
      setMotivosDeclinio([]);
      setIdMotivoDeclinio('');
      setMotivoDeclinioError(false);
    }
  }, [open]);

  useEffect(() => {
    if (!open || !isStatus3 || !token) return;
    let cancelled = false;
    setLoadingModelos(true);
    listarModelosTrabalhoUseCase
      .execute(token)
      .then((list) => {
        if (!cancelled) setModelosTrabalho(Array.isArray(list) ? list : []);
      })
      .catch((e) => {
        console.error('Erro ao listar modelos de trabalho', e);
        if (!cancelled) {
          setModelosTrabalho([]);
          toast.error('Não foi possível carregar os modelos de trabalho.');
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingModelos(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, isStatus3, token, listarModelosTrabalhoUseCase]);

  useEffect(() => {
    if (!open || !isStatus10 || !token) return;
    let cancelled = false;
    setLoadingMotivos(true);
    listarMotivosReprovacaoUseCase
      .execute(token)
      .then((list) => {
        if (!cancelled) setMotivosReprovacao(Array.isArray(list) ? list : []);
      })
      .catch((e) => {
        console.error('Erro ao listar motivos de reprovação', e);
        if (!cancelled) {
          setMotivosReprovacao([]);
          toast.error('Não foi possível carregar os motivos de reprovação.');
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingMotivos(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, isStatus10, token, listarMotivosReprovacaoUseCase]);

  useEffect(() => {
    if (!open || !isStatus12 || !token) return;
    let cancelled = false;
    setLoadingMotivosDeclinio(true);
    listarMotivosDeclinioUseCase
      .execute(token)
      .then((list) => {
        if (!cancelled) setMotivosDeclinio(Array.isArray(list) ? list : []);
      })
      .catch((e) => {
        console.error('Erro ao listar motivos de declínio', e);
        if (!cancelled) {
          setMotivosDeclinio([]);
          toast.error('Não foi possível carregar os motivos de declínio.');
        }
      })
      .finally(() => {
        if (!cancelled) setLoadingMotivosDeclinio(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, isStatus12, token, listarMotivosDeclinioUseCase]);

  const handleSalvar = async () => {
    const comentarioTrim = comentario.trim();
    const comentarioObrigatorio = !isStatus10 && !isStatus12;
    if (comentarioObrigatorio && !comentarioTrim) {
      setComentarioError(true);
      return;
    }
    setComentarioError(false);
    if (!token) {
      toast.error('Sessão inválida. Faça login novamente.');
      return;
    }

    if (isStatus3) {
      const err: typeof status3Errors = {};
      if (!modeloTrabalhoId?.trim()) err.modelo = true;
      if (pretencaoSalarialNum <= 0) err.pretencao = true;
      if (isHibrido && (quantidadeDiasPresencial < 1 || quantidadeDiasPresencial > 4)) err.dias = true;
      if (Object.keys(err).length > 0) {
        setStatus3Errors(err);
        toast.error('Preencha modelo de trabalho, pretensão salarial e, se híbrido, os dias presenciais.');
        return;
      }
      setStatus3Errors({});
    }
    if (isStatus10) {
      if (!idMotivoReprovacao?.trim()) {
        setMotivoError(true);
        toast.error('Selecione o motivo da reprovação.');
        return;
      }
      setMotivoError(false);
    }
    if (isStatus12) {
      if (!idMotivoDeclinio?.trim()) {
        setMotivoDeclinioError(true);
        toast.error('Selecione o motivo do declínio.');
        return;
      }
      setMotivoDeclinioError(false);
    }

    setSaving(true);
    try {
      if (isStatus10) {
        const res = await reprovarCandidaturaUseCase.execute(token, {
          idCandidatura,
          comentario: comentarioTrim,
          idMotivoReprovacao: idMotivoReprovacao.trim(),
        });
        if (!res.sucesso) {
          toast.error(res.mensagem ?? 'Não foi possível reprovar a candidatura.');
          return;
        }
        setSuccessMessage(res.mensagem ?? 'Feedback registrado com sucesso.');
        setShowSuccessModal(true);
        return;
      }
      if (isStatus12) {
        const res = await declinarCandidatoUseCase.execute(token, {
          idCandidatura,
          idMotivoDeclinio: idMotivoDeclinio.trim(),
          comentario: comentarioTrim,
        });
        if (!res.sucesso) {
          toast.error(res.mensagem ?? 'Não foi possível declinar a candidatura.');
          return;
        }
        setSuccessMessage(res.mensagem ?? 'Candidato declinado.');
        setShowSuccessModal(true);
        return;
      }

      if (isStatus3) {
        const atualizarRes = await atualizarCandidaturaUseCase.execute(token, {
          idCandidatura,
          pretencaoSalarial: pretencaoSalarialNum.toFixed(2),
          modeloTrabalhoId: modeloTrabalhoId.trim(),
          quantidadeDiasPresencial: isHibrido ? quantidadeDiasPresencial : 0,
        });
        if (!atualizarRes.sucesso) {
          toast.error(atualizarRes.mensagem ?? 'Não foi possível atualizar os dados da candidatura.');
          return;
        }
      }

      const res = await mudarStatusCandidaturaUseCase.execute(token, {
        idCandidatura,
        codigoStatus,
        comentario: comentarioTrim,
      });
      if (!res.sucesso || !res.retorno) {
        toast.error(res.mensagem ?? 'Não foi possível alterar o status.');
        return;
      }
      if (file) {
        try {
          await inserirArquivoCandidaturaUseCase.execute(token, {
            idCandidatura: res.retorno?.candidaturaId ?? idCandidatura,
            idComentario: res.retorno?.comentarioId ?? '',
            file,
          });
        } catch (e) {
          console.error('Erro ao anexar arquivo à candidatura', e);
          toast.error(
            e instanceof Error ? e.message : 'Status alterado, mas não foi possível anexar o arquivo.'
          );
        }
      }
      onSuccess?.(res.retorno);
      onOpenChange(false);
      toast.success(res.mensagem ?? 'Status alterado com sucesso.');
    } catch (e) {
      console.error('Erro ao alterar status da candidatura', e);
      toast.error(
        e instanceof Error ? e.message : 'Não foi possível alterar o status. Tente novamente.'
      );
    } finally {
      setSaving(false);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const chosen = e.target.files?.[0];
    setFile(chosen ?? null);
    e.target.value = '';
  };

  return (
    <>
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md p-0 gap-0" aria-labelledby="alterar-status-modal-title">
        <DialogHeader className="flex flex-row items-start gap-4 px-6 py-4 border-b shrink-0">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-lg bg-muted">
            {isStatus10 ? (
              <AlertCircle className="h-7 w-7 text-primary" />
            ) : isStatus12 ? (
              <PersonRemove className="h-7 w-7 text-primary" />
            ) : (
              <FileText className="h-7 w-7 text-primary" />
            )}
          </div>
          <div className="flex-1 min-w-0">
            <DialogTitle id="alterar-status-modal-title" className="text-xl">
              {isStatus10
                ? 'Deixe aqui o FeedBack do candidato.'
                : isStatus12
                  ? 'Declínio do candidato'
                  : 'Antes de terminar insira alguns dados'}
            </DialogTitle>
            <DialogDescription>
              {isStatus10
                ? 'Conta pra gente qual foi o seu feedback.'
                : isStatus12
                  ? 'Conta pra gente qual foi o motivo.'
                  : 'Insira detalhes e informações do candidato abaixo.'}
            </DialogDescription>
          </div>
        </DialogHeader>

        <div className="px-6 py-4 space-y-4">
          {/* Status 12: comentário acima (opcional), motivo abaixo */}
          {isStatus12 && (
            <>
              <div className="space-y-2">
                <Label htmlFor="alterar-status-comentario-declinio">Comentário (opcional)</Label>
                <Textarea
                  id="alterar-status-comentario-declinio"
                  placeholder="Escreva o comentário aqui..."
                  value={comentario}
                  onChange={(e) => setComentario(e.target.value)}
                  rows={4}
                  className="resize-none"
                  disabled={saving}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="alterar-status-motivo-declinio">Motivo</Label>
                <Select
                  value={idMotivoDeclinio}
                  onValueChange={(v) => {
                    setIdMotivoDeclinio(v);
                    if (motivoDeclinioError) setMotivoDeclinioError(false);
                  }}
                  disabled={loadingMotivosDeclinio || saving}
                >
                  <SelectTrigger
                    id="alterar-status-motivo-declinio"
                    className={motivoDeclinioError ? 'border-destructive' : ''}
                  >
                    <SelectValue placeholder={loadingMotivosDeclinio ? 'Carregando...' : 'Selecione o motivo'} />
                  </SelectTrigger>
                  <SelectContent>
                    {motivosDeclinio.map((m) => (
                      <SelectItem key={m.id} value={m.id}>
                        {m.descricao}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {motivoDeclinioError && (
                  <p className="text-sm text-destructive">Selecione o motivo do declínio.</p>
                )}
              </div>
            </>
          )}

          {!isStatus12 && (
            <>
          <div className="space-y-2">
            <Label htmlFor="alterar-status-comentario">Comentário</Label>
            <Textarea
              id="alterar-status-comentario"
              placeholder="Escreva o comentário aqui..."
              value={comentario}
              onChange={(e) => {
                setComentario(e.target.value);
                if (comentarioError) setComentarioError(false);
              }}
              rows={4}
              className="resize-none"
              disabled={saving}
              aria-invalid={comentarioError}
              aria-describedby={comentarioError ? 'alterar-status-comentario-erro' : undefined}
            />
            {comentarioError && (
              <p id="alterar-status-comentario-erro" className="text-sm text-destructive">
                Por favor, justifique a movimentação.
              </p>
            )}
          </div>

          {isStatus10 && (
            <div className="space-y-2">
              <Label htmlFor="alterar-status-motivo">Qual é o motivo da reprovação?</Label>
              <Select
                value={idMotivoReprovacao}
                onValueChange={(v) => {
                  setIdMotivoReprovacao(v);
                  if (motivoError) setMotivoError(false);
                }}
                disabled={loadingMotivos || saving}
              >
                <SelectTrigger
                  id="alterar-status-motivo"
                  className={motivoError ? 'border-destructive' : ''}
                >
                  <SelectValue placeholder={loadingMotivos ? 'Carregando...' : 'Selecione os motivos'} />
                </SelectTrigger>
                <SelectContent>
                  {motivosReprovacao.map((m) => (
                    <SelectItem key={m.id} value={m.id}>
                      {m.descricao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {motivoError && (
                <p className="text-sm text-destructive">Selecione o motivo da reprovação.</p>
              )}
            </div>
          )}
            </>
          )}

          {isStatus3 && (
            <div className="space-y-4 pt-2 border-t">
              <p className="text-sm font-medium text-foreground">Dados adicionais para este status</p>
              <div className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="alterar-status-modelo">Modelo de trabalho</Label>
                  <Select
                    value={modeloTrabalhoId}
                    onValueChange={(v) => {
                      setModeloTrabalhoId(v);
                      if (status3Errors.modelo) setStatus3Errors((e) => ({ ...e, modelo: false }));
                    }}
                    disabled={loadingModelos || saving}
                  >
                    <SelectTrigger
                      id="alterar-status-modelo"
                      className={status3Errors.modelo ? 'border-destructive' : ''}
                    >
                      <SelectValue placeholder={loadingModelos ? 'Carregando...' : 'Selecione'} />
                    </SelectTrigger>
                    <SelectContent>
                      {modelosTrabalho.map((m) => (
                        <SelectItem key={m.id} value={m.id}>
                          {m.descricao}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {status3Errors.modelo && (
                    <p className="text-sm text-destructive">Selecione o modelo de trabalho.</p>
                  )}
                </div>
                <div className="grid gap-4 sm:grid-cols-2 items-end">
                  {isHibrido && (
                    <div className="space-y-2">
                      <Label htmlFor="alterar-status-dias">Dias presenciais</Label>
                      <Select
                        value={String(quantidadeDiasPresencial)}
                        onValueChange={(v) => {
                          setQuantidadeDiasPresencial(Number(v));
                          if (status3Errors.dias) setStatus3Errors((e) => ({ ...e, dias: false }));
                        }}
                        disabled={saving}
                      >
                        <SelectTrigger
                          id="alterar-status-dias"
                          className={status3Errors.dias ? 'border-destructive' : ''}
                        >
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          {DIAS_PRESENCIAIS_OPCOES.map((op) => (
                            <SelectItem key={op.value} value={String(op.value)}>
                              {op.label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      {status3Errors.dias && (
                        <p className="text-sm text-destructive">Selecione os dias presenciais.</p>
                      )}
                    </div>
                  )}
                  <div className="space-y-2">
                    <CurrencyInput
                      label="Qual a pretensão salarial?"
                      value={pretencaoSalarialNum}
                      onChange={(v) => {
                        setPretencaoSalarialNum(v);
                        if (status3Errors.pretencao) setStatus3Errors((e) => ({ ...e, pretencao: false }));
                      }}
                      disabled={saving}
                      error={status3Errors.pretencao}
                    />
                    {status3Errors.pretencao && (
                      <p className="text-sm text-destructive">Informe a pretensão salarial.</p>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}

          {!isStatus10 && !isStatus12 && (
            <div className="space-y-2">
              <Label>Anexar arquivo</Label>
              <input
                ref={fileInputRef}
                type="file"
                accept="*/*"
                className="hidden"
                aria-hidden
                onChange={handleFileChange}
              />
              {file ? (
                <div className="flex items-center gap-2 rounded-md border border-border bg-muted/30 px-3 py-2">
                  <FileText className="h-4 w-4 shrink-0 text-muted-foreground" />
                  <span className="min-w-0 flex-1 truncate text-sm" title={file.name}>
                    {file.name}
                  </span>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="h-8 w-8 shrink-0"
                    aria-label="Remover arquivo"
                    disabled={saving}
                    onClick={() => setFile(null)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              ) : (
                <Button
                  type="button"
                  variant="outline"
                  className="w-full justify-start gap-2"
                  onClick={() => fileInputRef.current?.click()}
                  disabled={saving}
                >
                  <Upload className="h-4 w-4 shrink-0" />
                  Anexar arquivo
                </Button>
              )}
            </div>
          )}
        </div>

        <div className="flex justify-end gap-2 px-6 py-4 border-t bg-muted/30">
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={saving}>
            Cancelar
          </Button>
          <Button onClick={handleSalvar} disabled={saving}>
            {saving ? (isStatus12 ? 'Declinando…' : 'Salvando…') : isStatus12 ? 'Declinar' : 'Salvar'}
          </Button>
        </div>
      </DialogContent>
    </Dialog>

    {/* Modal de retorno após reprovação (status 10) */}
    <Dialog
      open={showSuccessModal}
      onOpenChange={(open) => {
        if (!open) {
          setShowSuccessModal(false);
          onOpenChange(false);
          onSuccess?.({});
        }
      }}
    >
      <DialogContent className="max-w-sm" aria-labelledby="reprovacao-sucesso-title" aria-describedby="reprovacao-sucesso-desc">
        <DialogHeader>
          <DialogTitle id="reprovacao-sucesso-title">Feedback registrado</DialogTitle>
          <DialogDescription id="reprovacao-sucesso-desc">{successMessage}</DialogDescription>
        </DialogHeader>
        <div className="flex justify-end pt-2">
          <Button
            onClick={() => {
              setShowSuccessModal(false);
              onOpenChange(false);
              onSuccess?.({});
            }}
          >
            Ok
          </Button>
        </div>
      </DialogContent>
    </Dialog>
    </>
  );
}
