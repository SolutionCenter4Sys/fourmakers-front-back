import { useState, useRef, useEffect } from 'react';
import { container } from 'tsyringe';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { FileText, CheckCircle2, Trash2, AlertCircle } from '@/components/ui/system-icons';
import { ImportarColaboradorLinkedinUseCase } from '@domain/usecases/ImportarColaboradorLinkedinUseCase';
import { ImportarColaboradorUseCase } from '@domain/usecases/ImportarColaboradorUseCase';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';
import { getMensagemAmigavelErro } from '@shared/utils/errorMessageUtils';

const LINKEDIN_PREFIX = 'https://www.linkedin.com/in/';

/** Extrai o id do perfil LinkedIn: de URL completa ou string só com o id. */
function extractLinkedInProfileId(input: string): string {
  const trimmed = input.trim();
  if (!trimmed) return '';
  const match = trimmed.match(/linkedin\.com\/in\/([^/?]+)/i);
  if (match) return match[1].trim();
  return trimmed;
}

interface InscreverCandidatoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  token: string | null;
  /** Chamado ao importar com sucesso, com o codigoColaborador retornado pela API. */
  onSuccess?: (codigoColaborador: string) => void;
}

export function InscreverCandidatoModal({
  open,
  onOpenChange,
  token,
  onSuccess,
}: InscreverCandidatoModalProps) {
  const [pdfFile, setPdfFile] = useState<File | null>(null);
  const [linkedinInput, setLinkedinInput] = useState('');
  const [importing, setImporting] = useState(false);
  const [pendingClose, setPendingClose] = useState(false);
  const [importError, setImportError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const importarLinkedin = container.resolve(ImportarColaboradorLinkedinUseCase);
  const importarColaborador = container.resolve(ImportarColaboradorUseCase);

  const linkedinId = extractLinkedInProfileId(linkedinInput);
  const canImport = (pdfFile != null || linkedinId !== '') && !!token && !importing;

  useEffect(() => {
    if (open) setImportError(null);
  }, [open]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (file.type !== 'application/pdf') {
        toast.error('Selecione um arquivo PDF.');
        return;
      }
      setPdfFile(file);
    }
    e.target.value = '';
  };

  const handleRemoveFile = () => {
    setPdfFile(null);
    if (fileInputRef.current) fileInputRef.current.value = '';
  };

  const handleImport = async () => {
    if (!token || !canImport) return;
    setImportError(null);
    setImporting(true);
    try {
      let codigoColaborador: string | undefined;
      if (linkedinId !== '') {
        const res = await importarLinkedin.execute(token, linkedinId);
        if (res?.sucesso === false) {
          setImportError(
            res.mensagem?.trim() || 'Não foi possível importar pelo perfil LinkedIn. Ajuste e tente novamente.'
          );
          return;
        }
        codigoColaborador = res?.codigoColaborador;
        toast.success('Candidato importado com sucesso pelo perfil LinkedIn!');
      } else if (pdfFile) {
        const res = await importarColaborador.execute(token, pdfFile);
        if (res?.sucesso === false) {
          setImportError(
            res.mensagem?.trim() || 'Não foi possível ler o documento. Ajuste e importe novamente.'
          );
          return;
        }
        codigoColaborador = res?.codigoColaborador;
        toast.success('Candidato importado com sucesso pelo currículo em PDF!');
      }
      if (codigoColaborador) {
        onSuccess?.(codigoColaborador);
      }
      onOpenChange(false);
      setPdfFile(null);
      setLinkedinInput('');
      handleRemoveFile();
    } catch (e) {
      console.error('Erro ao importar candidato', e);
      const message = getMensagemAmigavelErro(
        e,
        'Não foi possível importar o candidato. Tente novamente.'
      );
      setImportError(message);
      toast.error(message);
    } finally {
      setImporting(false);
    }
  };

  const hasData = pdfFile != null || linkedinInput.trim() !== '';

  const handleClose = (value: boolean) => {
    if (!value) {
      if (hasData) {
        setPendingClose(true);
        return;
      }
      setPdfFile(null);
      setLinkedinInput('');
      setImportError(null);
      handleRemoveFile();
      onOpenChange(false);
    } else {
      onOpenChange(value);
    }
  };

  const handleConfirmClose = () => {
    setPendingClose(false);
    setPdfFile(null);
    setLinkedinInput('');
    setImportError(null);
    handleRemoveFile();
    onOpenChange(false);
  };

  const handleCancelClose = () => {
    setPendingClose(false);
  };

  return (
    <>
      <AlertDialog open={pendingClose} onOpenChange={(o) => !o && handleCancelClose()}>
        <AlertDialogContent>
          <AlertDialogTitle>Fechar sem inscrever?</AlertDialogTitle>
          <AlertDialogDescription>
            Deseja fechar sem inscrever o talento na vaga? Os dados preenchidos serão descartados.
          </AlertDialogDescription>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={handleCancelClose}>Não</AlertDialogCancel>
            <AlertDialogAction onClick={handleConfirmClose} className="bg-destructive text-destructive-foreground hover:bg-destructive/90">
              Sim, fechar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
      <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-lg">
        <DialogHeader className="flex-row items-center gap-3 space-y-0 pb-2">
          <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10">
            <FileText className="h-7 w-7 text-primary" />
          </div>
          <div className="flex-1">
            <DialogTitle className="text-xl">Inscrever CV de candidato</DialogTitle>
            <DialogDescription className="mt-1">
              Inscreva o Candidato no Fourmakers em segundos com as informações do CV em PDF!
            </DialogDescription>
          </div>
        </DialogHeader>

        <div className="space-y-4 pt-2">
          {importError && (
            <Alert variant="destructive" role="alert">
              <AlertCircle className="h-4 w-4" />
              <AlertTitle>Erro na importação</AlertTitle>
              <AlertDescription>{importError}</AlertDescription>
            </Alert>
          )}
          <div className="space-y-2">
            <Label>Faça upload do currículo em PDF</Label>
            <input
              ref={fileInputRef}
              type="file"
              accept="application/pdf"
              className="hidden"
              onChange={handleFileChange}
            />
            {pdfFile ? (
              <div className="flex items-center justify-between gap-2 rounded-lg border border-borderSoft bg-surfaceSubtle/50 px-4 py-3">
                <div className="flex items-center gap-2">
                  <CheckCircle2 className="h-5 w-5 shrink-0 text-success" />
                  <span className="text-sm font-medium">CV carregado com sucesso!</span>
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
                variant="outline"
                className="w-full border-dashed py-6"
                onClick={() => fileInputRef.current?.click()}
              >
                Clique para selecionar um PDF
              </Button>
            )}
          </div>

          <div className="relative flex items-center gap-2">
            <div className="flex-1 border-t border-borderSoft" />
            <span className="text-xs text-muted-foreground">ou importe pelo link do perfil</span>
            <div className="flex-1 border-t border-borderSoft" />
          </div>

          <div className="space-y-2">
            <Label>Perfil do LinkedIn</Label>
            <div className="flex rounded-md border border-input bg-background focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2">
              <span className="flex items-center border-r border-input bg-muted/50 px-3 text-sm text-muted-foreground">
                {LINKEDIN_PREFIX}
              </span>
              <Input
                type="text"
                placeholder="Inserir perfil do LinkedIn"
                value={linkedinInput.includes('linkedin') ? extractLinkedInProfileId(linkedinInput) : linkedinInput}
                onChange={(e) => setLinkedinInput(e.target.value)}
                className="border-0 bg-transparent focus-visible:ring-0 focus-visible:ring-offset-0"
              />
            </div>
          </div>

          <div className="flex justify-end pt-2">
            <Button
              onClick={handleImport}
              disabled={!canImport}
              className={cn(importing && 'opacity-70')}
            >
              {importing ? 'Importando...' : 'Importar'}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
    </>
  );
}
