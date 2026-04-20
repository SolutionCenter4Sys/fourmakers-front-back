import { useState, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import {
  FileText,
  CheckCircle2,
  Trash2,
  Upload,
  AlertTriangle,
  AlertCircle,
  Info,
} from '@/components/ui/system-icons';
import type { PdfProcessadoItem } from '@domain/entities/CurriculoColaborador';
import { ImportarColaboradorLoteUseCase } from '@domain/usecases/ImportarColaboradorLoteUseCase';
import { ImportarColaboradorLotePlanilhaUseCase } from '@domain/usecases/ImportarColaboradorLotePlanilhaUseCase';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import { getMensagemAmigavelErro } from '@shared/utils/errorMessageUtils';

type ModoImportacao = 'zip' | 'planilha';
type EtapaModal = 'formulario' | 'sucesso';

/** Sucesso ZIP: lista de PDFs + Fechar + Ver minhas importações */
interface SucessoZIP {
  tipo: 'zip';
  pdfs: PdfProcessadoItem[];
  mensagem: string;
}

/** Sucesso planilha: só mensagem + Fechar + Ver minhas importações */
interface SucessoPlanilha {
  tipo: 'planilha';
  mensagem: string;
}

type SucessoPayload = SucessoZIP | SucessoPlanilha;

interface ImportarTalentosModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  /** Chamado ao clicar em "Ver minhas importações" (tela a ser criada depois). */
  onVerMinhasImportacoes?: () => void;
}

const ACCEPT_ZIP = 'application/zip,.zip';
const ACCEPT_EXCEL = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel,.xlsx,.xls';

export function ImportarTalentosModal({
  open,
  onOpenChange,
  token,
  onVerMinhasImportacoes,
}: ImportarTalentosModalProps) {
  const [modo, setModo] = useState<ModoImportacao>('zip');
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [importing, setImporting] = useState(false);
  const [etapa, setEtapa] = useState<EtapaModal>('formulario');
  const [sucesso, setSucesso] = useState<SucessoPayload | null>(null);
  const [importError, setImportError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (open) setImportError(null);
  }, [open]);

  const importarLote = container.resolve(ImportarColaboradorLoteUseCase);
  const importarLotePlanilha = container.resolve(ImportarColaboradorLotePlanilhaUseCase);

  const canImport = !!token && !!arquivo && !importing;

  const handleModoChange = (value: string) => {
    setModo(value as ModoImportacao);
    setArquivo(null);
    if (fileInputRef.current) fileInputRef.current.value = '';
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (modo === 'zip') {
        const isZip = file.type === 'application/zip' || file.name.toLowerCase().endsWith('.zip');
        if (!isZip) {
          toast.error('Selecione um arquivo ZIP.');
          return;
        }
      } else {
        const isExcel =
          file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' ||
          file.type === 'application/vnd.ms-excel' ||
          file.name.toLowerCase().endsWith('.xlsx') ||
          file.name.toLowerCase().endsWith('.xls');
        if (!isExcel) {
          toast.error('Selecione uma planilha .xls ou .xlsx.');
          return;
        }
      }
      setArquivo(file);
    }
    e.target.value = '';
  };

  const handleRemoveFile = () => {
    setArquivo(null);
    if (fileInputRef.current) fileInputRef.current.value = '';
  };

  const handleImportar = async () => {
    if (!token || !arquivo) return;
    setImportError(null);
    setImporting(true);
    try {
      if (modo === 'zip') {
        const res = await importarLote.execute(token, arquivo);
        if (res.sucesso && res.retorno) {
          setSucesso({
            tipo: 'zip',
            pdfs: res.retorno.pdfsProcessados ?? [],
            mensagem: res.mensagem ?? 'Importação concluída.',
          });
          setEtapa('sucesso');
        } else {
          const msg = res.mensagem?.trim() ?? 'Falha ao importar o lote. Ajuste e tente novamente.';
          setImportError(msg);
          toast.error(msg);
        }
      } else {
        const res = await importarLotePlanilha.execute(token, arquivo);
        if (res.sucesso) {
          setSucesso({
            tipo: 'planilha',
            mensagem: res.mensagem ?? 'Processamento concluído.',
          });
          setEtapa('sucesso');
        } else {
          const msg = res.mensagem?.trim() ?? 'Falha ao importar a planilha. Ajuste e tente novamente.';
          setImportError(msg);
          toast.error(msg);
        }
      }
    } catch (e) {
      console.error('Erro ao importar em lote', e);
      const mensagem = getMensagemAmigavelErro(
        e,
        'Não foi possível importar. Tente novamente.'
      );
      setImportError(mensagem);
      toast.error(mensagem);
    } finally {
      setImporting(false);
    }
  };

  const handleFechar = () => {
    setEtapa('formulario');
    setSucesso(null);
    setImportError(null);
    setArquivo(null);
    handleRemoveFile();
    onOpenChange(false);
  };

  const handleVerMinhasImportacoes = () => {
    if (onVerMinhasImportacoes) {
      onVerMinhasImportacoes();
    }
    handleFechar();
  };

  const handleClose = (value: boolean) => {
    if (!value) handleFechar();
  };

  if (etapa === 'sucesso' && sucesso) {
    return (
      <Dialog open={open} onOpenChange={handleClose}>
        <DialogContent className="max-w-md">
          <DialogHeader className="flex-row items-center gap-3 space-y-0 pb-2">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10">
              <FileText className="h-7 w-7 text-primary" />
            </div>
            <div className="flex-1">
              <DialogTitle className="text-xl">Importação de CVs</DialogTitle>
              <p className="mt-1 text-sm text-muted-foreground">
                Processamento concluído.
              </p>
            </div>
          </DialogHeader>
          <div className="space-y-2 py-2">
            {sucesso.tipo === 'zip' && (
              <>
                <p className="text-sm text-muted-foreground">Os seguintes CVs foram importados:</p>
                {sucesso.pdfs.length > 0 ? (
                  <ul className="space-y-2">
                    {sucesso.pdfs.map((item) => (
                      <li
                        key={item.nomeArquivo}
                        className="flex items-center gap-2 text-sm"
                      >
                        <CheckCircle2 className="h-5 w-5 shrink-0 text-success" />
                        <span>{item.nomeArquivo}</span>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="text-sm text-muted-foreground">{sucesso.mensagem}</p>
                )}
              </>
            )}
            {sucesso.tipo === 'planilha' && (
              <p className="text-sm text-muted-foreground">{sucesso.mensagem}</p>
            )}
          </div>
          <div className="flex justify-end gap-2 pt-4">
            <Button variant="outline" onClick={handleFechar}>
              Fechar
            </Button>
            <Button onClick={handleVerMinhasImportacoes}>
              Ver minhas importações
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    );
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-lg">
        <DialogHeader className="flex-row items-center gap-3 space-y-0 pb-2">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10">
            <FileText className="h-7 w-7 text-primary" />
          </div>
          <div className="flex-1">
            <DialogTitle className="text-xl">Importar talentos</DialogTitle>
            <DialogDescription className="mt-1">
              Importe Candidatos no Fourmakers em segundos com as informações do CV em lote!
            </DialogDescription>
          </div>
        </DialogHeader>

        {importError && (
          <Alert variant="destructive" role="alert">
            <AlertCircle className="h-4 w-4" />
            <AlertTitle>Erro na importação</AlertTitle>
            <AlertDescription>{importError}</AlertDescription>
          </Alert>
        )}

        <RadioGroup
          value={modo}
          onValueChange={handleModoChange}
          className="grid gap-3 py-2"
        >
          <div className="flex items-center gap-2">
            <RadioGroupItem value="zip" id="modo-zip" />
            <Label htmlFor="modo-zip" className="cursor-pointer font-normal">
              Importar lote em ZIP
            </Label>
          </div>
          <div className="flex items-center gap-2">
            <RadioGroupItem value="planilha" id="modo-planilha" />
            <Label htmlFor="modo-planilha" className="cursor-pointer font-normal">
              Importar lote via planilha
            </Label>
          </div>
        </RadioGroup>

        {modo === 'zip' && (
          <div className="space-y-3">
            <Label>Faça upload dos currículos em ZIP</Label>
            <input
              ref={fileInputRef}
              type="file"
              accept={ACCEPT_ZIP}
              className="hidden"
              onChange={handleFileChange}
            />
            {arquivo ? (
              <div className="flex items-center justify-between gap-2 rounded-lg border border-borderSoft bg-surfaceSubtle/50 px-4 py-3">
                <div className="flex items-center gap-2">
                  <CheckCircle2 className="h-5 w-5 shrink-0 text-success" />
                  <span className="text-sm font-medium">Arquivo carregado com sucesso!</span>
                </div>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="h-9 w-9 shrink-0 text-muted-foreground hover:text-destructive"
                  aria-label="Remover arquivo"
                  onClick={handleRemoveFile}
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            ) : (
              <Button
                type="button"
                variant="ghost"
                className="w-full border border-dashed border-border py-6"
                onClick={() => fileInputRef.current?.click()}
              >
                <Upload className="h-5 w-5 shrink-0" />
                <span>Importar ZIP com CVs</span>
              </Button>
            )}
            <div className="flex items-start gap-2 rounded-lg border border-amber-200 bg-amber-50 dark:border-amber-900 dark:bg-amber-950/30 px-3 py-2 text-sm text-amber-800 dark:text-amber-200">
              <AlertTriangle className="h-5 w-5 shrink-0 mt-0.5" />
              <span>Faça upload dos CVs em um arquivo compactado formato ZIP.</span>
            </div>
          </div>
        )}

        {modo === 'planilha' && (
          <div className="space-y-3">
            <p className="text-sm text-muted-foreground">
              Insira uma planilha de acordo com o modelo abaixo, informando cada link ou perfil em uma linha separada.
            </p>
            <div className="rounded border border-border bg-muted/30 p-3 text-xs text-muted-foreground font-mono">
              <div className="grid grid-cols-2 gap-1 w-48">
                <span className="font-semibold">A</span>
                <span className="font-semibold">B</span>
                <span>link-perfil-1</span>
                <span></span>
                <span>link-perfil-2</span>
                <span></span>
                <span>link-perfil-3</span>
                <span></span>
              </div>
            </div>
            <input
              ref={fileInputRef}
              type="file"
              accept={ACCEPT_EXCEL}
              className="hidden"
              onChange={handleFileChange}
            />
            {arquivo ? (
              <div className="flex items-center justify-between gap-2 rounded-lg border border-borderSoft bg-surfaceSubtle/50 px-4 py-3">
                <div className="flex items-center gap-2">
                  <CheckCircle2 className="h-5 w-5 shrink-0 text-success" />
                  <span className="text-sm font-medium">Arquivo carregado com sucesso!</span>
                </div>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="h-9 w-9 shrink-0 text-muted-foreground hover:text-destructive"
                  aria-label="Remover arquivo"
                  onClick={handleRemoveFile}
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            ) : (
              <Button
                type="button"
                variant="ghost"
                className="w-full border border-dashed border-border py-6"
                onClick={() => fileInputRef.current?.click()}
              >
                <Info className="h-5 w-5 shrink-0" />
                <span>Faça upload da planilha com os perfis</span>
              </Button>
            )}
            <div className="flex items-start gap-2 rounded-lg border border-amber-200 bg-amber-50 dark:border-amber-900 dark:bg-amber-950/30 px-3 py-2 text-sm text-amber-800 dark:text-amber-200">
              <AlertTriangle className="h-5 w-5 shrink-0 mt-0.5" />
              <span>Faça upload dos CVs em uma planilha .xls ou .xlsx.</span>
            </div>
          </div>
        )}

        <div className="flex justify-end pt-2">
          <Button
            onClick={handleImportar}
            disabled={!canImport}
            className={cn(importing && 'opacity-70')}
          >
            {importing
              ? 'Importando...'
              : modo === 'zip'
                ? 'Importar em lote'
                : 'Importar planilha'}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
