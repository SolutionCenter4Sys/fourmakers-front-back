import { useState, useEffect, useRef } from 'react';
import { FileText, Download } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
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

interface DocumentPreviewProps {
  url: string;
  name: string;
  type: string;
  allowDownload?: boolean;
  /** Token para requisição autenticada; evita download automático ao usar blob para preview. */
  accessToken?: string | null;
}

const CONFIDENTIALITY_TEXT = `O conteúdo deste arquivo é de propriedade exclusiva da Foursys e destina-se apenas a uso profissional interno, conforme políticas corporativas e a Lei nº 13.709/2018 (LGPD). É proibido o compartilhamento com terceiros ou utilização para fins alheios às atividades da empresa.
Ao clicar em Aceitar, você declara ciência e concordância com estes termos.`;

export function DocumentPreview({
  url,
  name,
  type,
  allowDownload = false,
  accessToken,
}: DocumentPreviewProps) {
  const [confidentialityOpen, setConfidentialityOpen] = useState(false);
  const [previewLoading, setPreviewLoading] = useState(true);
  const [previewObjectUrl, setPreviewObjectUrl] = useState<string | null>(null);
  const [useDirectUrl, setUseDirectUrl] = useState(false);
  const objectUrlRef = useRef<string | null>(null);
  const isPDF = type === 'application/pdf' || name.endsWith('.pdf');
  const isImage =
    type.startsWith('image/') || /\.(jpg|jpeg|png|gif|webp)$/i.test(name);
  const hasPreview = isPDF || isImage;

  const previewSrc = previewObjectUrl ?? (useDirectUrl ? url : null);

  useEffect(() => {
    setPreviewLoading(true);
    setPreviewObjectUrl(null);
    setUseDirectUrl(false);
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }
    if (!url || !hasPreview) {
      setPreviewLoading(false);
      return;
    }
    const controller = new AbortController();
    const headers: HeadersInit = {};
    if (accessToken) {
      headers['Authorization'] = `Bearer ${accessToken}`;
    }
    fetch(url, { signal: controller.signal, credentials: 'include', headers })
      .then((res) => {
        if (!res.ok) throw new Error('Falha ao carregar preview');
        return res.blob();
      })
      .then((blob) => {
        const objectUrl = URL.createObjectURL(blob);
        objectUrlRef.current = objectUrl;
        setPreviewObjectUrl(objectUrl);
        setPreviewLoading(false);
      })
      .catch(() => {
        setUseDirectUrl(true);
        setPreviewLoading(false);
      });

    return () => {
      controller.abort();
      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
        objectUrlRef.current = null;
      }
    };
  }, [url, hasPreview, accessToken]);

  const handleAcceptDownload = () => {
    const link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.setAttribute('target', '_blank');
    link.setAttribute('rel', 'noopener noreferrer');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    setConfidentialityOpen(false);
  };

  return (
    <Card className="h-full flex flex-col min-h-0">
      <CardContent className="p-3 h-full flex flex-col min-h-0 flex-1">
        <div className="flex items-center justify-between mb-2 flex-shrink-0">
          <h4 className="text-sm font-semibold">Preview do Documento</h4>
          {allowDownload && (
            <>
              <Button
                variant="outline"
                size="sm"
                className="gap-2"
                onClick={() => setConfidentialityOpen(true)}
              >
                <Download className="w-4 h-4" />
                Baixar
              </Button>
              <AlertDialog
                open={confidentialityOpen}
                onOpenChange={setConfidentialityOpen}
              >
                <AlertDialogContent className="max-w-lg">
                  <AlertDialogHeader>
                    <AlertDialogTitle>
                      Aviso de Confidencialidade
                    </AlertDialogTitle>
                    <AlertDialogDescription className="text-sm whitespace-pre-line pt-2">
                      {CONFIDENTIALITY_TEXT}
                    </AlertDialogDescription>
                  </AlertDialogHeader>
                  <AlertDialogFooter>
                    <AlertDialogCancel>Fechar</AlertDialogCancel>
                    <AlertDialogAction onClick={handleAcceptDownload}>
                      Aceitar
                    </AlertDialogAction>
                  </AlertDialogFooter>
                </AlertDialogContent>
              </AlertDialog>
            </>
          )}
        </div>

        <div className="flex-1 rounded-lg border bg-muted/30 overflow-hidden min-h-[400px] relative">
          {hasPreview && previewLoading && (
            <div
              className="absolute inset-0 z-10 flex flex-col gap-3 p-4 bg-muted/30 rounded-lg"
              aria-hidden
            >
              <Skeleton className="h-8 w-3/4 rounded" />
              <Skeleton className="h-4 w-full rounded flex-1 min-h-[120px]" />
              <Skeleton className="h-4 w-full rounded" />
              <Skeleton className="h-4 w-5/6 rounded" />
              <Skeleton className="h-4 w-4/5 rounded" />
              <Skeleton className="h-4 w-full rounded flex-1 min-h-[80px]" />
              <Skeleton className="h-4 w-2/3 rounded" />
            </div>
          )}
          {isPDF ? (
            previewSrc ? (
              <iframe
                src={`${previewSrc}#zoom=100&pagemode=none&navpanes=0${!allowDownload ? '&toolbar=0' : ''}`}
                className="w-full h-full min-h-[400px]"
                title={name}
                onLoad={() => setPreviewLoading(false)}
              />
            ) : (
              <div className="flex items-center justify-center min-h-[400px] text-muted-foreground text-sm">
                Carregando…
              </div>
            )
          ) : isImage ? (
            previewSrc ? (
              <img
                src={previewSrc}
                alt={name}
                className="w-full h-full object-contain min-h-[400px]"
                onLoad={() => setPreviewLoading(false)}
              />
            ) : (
              <div className="flex items-center justify-center min-h-[400px] text-muted-foreground text-sm">
                Carregando…
              </div>
            )
          ) : (
            <div className="flex flex-col items-center justify-center h-full p-8 text-center min-h-[400px]">
              <FileText className="w-16 h-16 text-muted-foreground/50 mb-4" />
              <p className="text-sm font-medium mb-2">Preview não disponível</p>
              <p className="text-xs text-muted-foreground">
                Este tipo de arquivo não pode ser visualizado no navegador
              </p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
