import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { Briefcase, FileText, Plus, Trash2, X } from '@/components/ui/system-icons';
import { CheckCircle } from '@/components/ui/system-icons';
import { AlertTriangle } from '@/components/ui/system-icons';
import { cn } from '@/lib/utils';

export type TipoEnvio = 'com_remuneracao' | 'sem_remuneracao';

const TIPO_ENVIO_OPCOES: { value: TipoEnvio; label: string }[] = [
  { value: 'com_remuneracao', label: 'Com remuneração Dep. Pessoal e outros' },
  { value: 'sem_remuneracao', label: 'Sem remuneração, Infra e outros' },
];

export interface EnvioTemplateContratacaoModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  /** Callback ao confirmar envio; pode retornar Promise. O modal só fecha após sucesso (resolve). */
  onEnviar?: (payload: { tipoEnvio: TipoEnvio | null; emails: string[]; arquivo?: File }) => void | Promise<void>;
  /** Quando true (orgId === 9): oculta tipo de envio e upload; apenas lista de e-mails; lista pré-preenchida com emailPessoalInicial. */
  isOrg9?: boolean;
  /** E-mail pessoal do template; quando isOrg9, preenche a lista de e-mails ao abrir o modal. */
  emailPessoalInicial?: string;
}

const MAX_EMAIL_INPUT_LENGTH = 100;

/** Estrutura mínima: algo@algo.com ou algo@algo.com.br */
const EMAIL_ESTRUTURA_MINIMA = /^[^\s@]+@[^\s@]+\.(com\.br|com)$/i;

/** Org 9: aceita qualquer e-mail com @ e domínio (ex.: .edu, .io) para pré-preencher e habilitar envio. */
const EMAIL_ORG9_PREENCHER = /^[^\s@]+@[^\s@]+(\.[^\s@]+)+$/i;

function emailTemEstruturaValida(email: string): boolean {
  return EMAIL_ESTRUTURA_MINIMA.test(email.trim());
}

function emailAceitoParaOrg9(email: string): boolean {
  const t = email.trim();
  return t.length >= 5 && EMAIL_ORG9_PREENCHER.test(t);
}

export function EnvioTemplateContratacaoModal({
  open,
  onOpenChange,
  onEnviar,
  isOrg9 = false,
  emailPessoalInicial = '',
}: EnvioTemplateContratacaoModalProps) {
  const [tipoEnvio, setTipoEnvio] = useState<TipoEnvio | null>(null);
  const [emailInput, setEmailInput] = useState('');
  const [emails, setEmails] = useState<string[]>([]);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [uploadSucesso, setUploadSucesso] = useState(false);
  const [erroEmail, setErroEmail] = useState<string | null>(null);
  const [enviando, setEnviando] = useState(false);

  useEffect(() => {
    if (open && isOrg9 && emailPessoalInicial.trim()) {
      const email = emailPessoalInicial.trim();
      if (emailAceitoParaOrg9(email)) {
        setEmails([email]);
      }
    }
  }, [open, isOrg9, emailPessoalInicial]);

  const handleOpenChange = (newOpen: boolean) => {
    if (!newOpen) {
      setEmails([]);
      setEmailInput('');
      setErroEmail(null);
      setTipoEnvio(null);
      setArquivo(null);
      setUploadSucesso(false);
    }
    onOpenChange(newOpen);
  };

  const handleAdicionarEmail = () => {
    const email = emailInput.trim();
    setErroEmail(null);
    if (!email) return;
    if (!emailTemEstruturaValida(email)) {
      setErroEmail('Informe um e-mail com estrutura válida (ex.: nome@dominio.com ou nome@dominio.com.br).');
      return;
    }
    if (emails.includes(email)) return;
    setEmails((prev) => [...prev, email]);
    setEmailInput('');
  };

  const handleRemoverEmail = (email: string) => {
    setEmails((prev) => prev.filter((e) => e !== email));
  };

  const handleLimparInput = () => setEmailInput('');

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file?.type === 'application/pdf') {
      setArquivo(file);
      setUploadSucesso(true);
    }
    e.target.value = '';
  };

  const handleRemoverArquivo = () => {
    setArquivo(null);
    setUploadSucesso(false);
  };

  const handleEnviar = async () => {
    const emailsFinais =
      isOrg9 && emails.length === 0 && emailAceitoParaOrg9(emailPessoalInicial)
        ? [emailPessoalInicial.trim()]
        : emails;
    const payload = {
      tipoEnvio,
      emails: emailsFinais,
      arquivo: isOrg9 ? undefined : (arquivo ?? undefined),
    };
    const result = onEnviar?.(payload);
    if (result instanceof Promise) {
      setEnviando(true);
      try {
        await result;
        handleOpenChange(false);
      } finally {
        setEnviando(false);
      }
    } else {
      handleOpenChange(false);
    }
  };

  /** Enviar habilitado: org9 = e-mail pessoal na lista ou válido para uso; demais orgs = e-mail ou tipo de envio (+ PDF nas outras). */
  const tipoEnvioSelecionado = tipoEnvio === 'com_remuneracao' || tipoEnvio === 'sem_remuneracao';
  const temPeloMenosUmEmail = emails.length >= 1;
  const org9TemEmailParaEnvio = temPeloMenosUmEmail || emailAceitoParaOrg9(emailPessoalInicial);
  const podeEnviar = isOrg9 ? org9TemEmailParaEnvio : (temPeloMenosUmEmail || tipoEnvioSelecionado);

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent
        className="max-w-[520px] p-0 gap-0 overflow-hidden"
        onPointerDownOutside={(e) => e.preventDefault()}
        aria-labelledby="envio-template-contratacao-title"
        aria-describedby="envio-template-contratacao-desc"
      >
        <DialogTitle id="envio-template-contratacao-title" className="sr-only">
          Envio do Template Contratação
        </DialogTitle>
        <DialogDescription id="envio-template-contratacao-desc" className="sr-only">
          Enviando PDF para Infra e Departamento Pessoal. Preencha os e-mails e opções de envio.
        </DialogDescription>
        {/* Header escuro */}
        <div className="flex items-center gap-3 bg-muted px-6 py-4 pr-14">
          <Briefcase className="h-6 w-6 text-foreground shrink-0" aria-hidden />
          <h2 className="text-lg font-semibold text-foreground">Envio do Template Contratação</h2>
        </div>

        <div className="p-6 space-y-6">
          <p className="text-sm text-muted-foreground">
            Enviando PDF para Infra e Departamento Pessoal
          </p>

          {!isOrg9 && (
          /* Opções de envio: checkboxes independentes com exclusão mútua (no máximo um selecionado) */
          <div className="space-y-3">
            <Label className="text-base font-medium">Tipo de envio</Label>
            <div className="flex flex-col gap-2">
              {TIPO_ENVIO_OPCOES.map((op) => (
                <div key={op.value} className="flex items-center space-x-2">
                  <Checkbox
                    id={`envio-${op.value}`}
                    checked={tipoEnvio === op.value}
                    onCheckedChange={(checked) => {
                      if (checked) {
                        setTipoEnvio(op.value);
                      } else {
                        setTipoEnvio(null);
                      }
                    }}
                  />
                  <Label htmlFor={`envio-${op.value}`} className="font-normal cursor-pointer">
                    {op.label}
                  </Label>
                </div>
              ))}
            </div>
          </div>
          )}

          {/* E-mails */}
          <div className="space-y-2">
            <Label>E-mails para envio</Label>
            <div className="flex gap-2">
              <div className="relative flex-1">
                <Input
                  value={emailInput}
                  onChange={(e) => {
                    setEmailInput(e.target.value.slice(0, MAX_EMAIL_INPUT_LENGTH));
                    if (erroEmail) setErroEmail(null);
                  }}
                  onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), handleAdicionarEmail())}
                  placeholder="Digite mais e-mails para envios"
                  error={!!erroEmail}
                  className="pr-8"
                  maxLength={MAX_EMAIL_INPUT_LENGTH}
                />
                {emailInput.length > 0 && (
                  <button
                    type="button"
                    onClick={handleLimparInput}
                    className="absolute right-2 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                    aria-label="Limpar"
                  >
                    <X className="h-4 w-4" />
                  </button>
                )}
              </div>
              <Button
                type="button"
                variant="primary"
                size="icon"
                onClick={handleAdicionarEmail}
                aria-label="Adicionar e-mail"
              >
                <Plus className="h-4 w-4" />
              </Button>
            </div>
            <div className="flex justify-end">
              <span className="text-xs text-muted-foreground">{emailInput.length}/{MAX_EMAIL_INPUT_LENGTH}</span>
            </div>
            {erroEmail && (
              <p className="text-xs text-destructive">{erroEmail}</p>
            )}
            <p className="text-xs text-muted-foreground">
              Você pode adicionar mais de um e-mail (ex.: nome@dominio.com ou nome@dominio.com.br).
            </p>
            {emails.length > 0 && (
              <div className="flex flex-wrap gap-2">
                {emails.map((email) => (
                  <span
                    key={email}
                    className={cn(
                      'inline-flex items-center gap-1.5 rounded-md border bg-primary/10 text-primary px-2.5 py-1 text-sm'
                    )}
                  >
                    {email}
                    <button
                      type="button"
                      onClick={() => handleRemoverEmail(email)}
                      aria-label={`Remover ${email}`}
                      className="p-0.5 rounded hover:bg-primary/20"
                    >
                      <X className="h-3.5 w-3.5" />
                    </button>
                  </span>
                ))}
              </div>
            )}
          </div>

          {!isOrg9 && (
          /* Upload PDF */
          <div className="space-y-2">
            <Label>Faça upload do Template em PDF</Label>
            <div className="flex items-center gap-2 flex-wrap">
              <label className="cursor-pointer">
                <input
                  type="file"
                  accept=".pdf,application/pdf"
                  onChange={handleFileChange}
                  className="sr-only"
                />
                <Button type="button" variant="outline" size="sm" className="gap-2" asChild>
                  <span>
                    <FileText className="h-4 w-4" />
                    Carregar Template
                  </span>
                </Button>
              </label>
              {uploadSucesso && arquivo && (
                <div className="flex items-center gap-2 rounded-md bg-primary/10 text-primary px-3 py-2 text-sm flex-1 min-w-0">
                  <CheckCircle className="h-4 w-4 shrink-0" />
                  <span className="truncate">Template carregado com sucesso!</span>
                  <button
                    type="button"
                    onClick={handleRemoverArquivo}
                    aria-label="Remover arquivo"
                    className="ml-auto p-1 rounded hover:bg-primary/20 shrink-0"
                  >
                    <Trash2 className="h-4 w-4" />
                  </button>
                </div>
              )}
            </div>
          </div>
          )}
        </div>

        {/* Footer */}
        <div className="px-6 pb-6 flex flex-col gap-4">
          {!isOrg9 && (
          <div className="flex items-start gap-2 rounded-md border border-warning/50 bg-warning/10 px-3 py-2 text-sm text-warning">
            <AlertTriangle className="h-4 w-4 shrink-0 mt-0.5" />
            <span>Faça upload do Template que baixou em PDF.</span>
          </div>
          )}
          <div className="flex justify-end">
            <Button onClick={handleEnviar} className="gap-2" disabled={!podeEnviar || enviando}>
              {enviando ? 'Enviando...' : 'Enviar'}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
