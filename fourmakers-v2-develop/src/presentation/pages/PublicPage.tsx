import { useParams } from 'react-router-dom';

/**
 * Página pública agnóstica: qualquer path após /public/ é repassado ao iframe do FlutterFlow.
 * - /public/vagas → iframe carrega baseUrl/vagas
 * - /public/qualquer/rota → iframe carrega baseUrl/qualquer/rota
 * - /public → iframe carrega baseUrl (raiz do FF)
 * Sem autenticação nem postMessage.
 */
export function PublicPage() {
  const params = useParams();
  const ffPath = params['*']; // tudo após /public/
  const base = (import.meta.env.VITE_FLUTTERFLOW_BASE_URL as string | undefined)?.replace(/\/$/, '') ?? '';
  const fullFlutterflowUrl = base
    ? (ffPath ? `${base}/${ffPath}` : base)
    : '';

  if (!fullFlutterflowUrl) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-muted/30">
        <p className="text-muted-foreground">URL do FlutterFlow não configurada.</p>
      </div>
    );
  }

  return (
    <div
      className="fixed inset-0 w-full h-full"
      style={{ margin: 0, padding: 0 }}
    >
      <iframe
        src={fullFlutterflowUrl}
        title="Página pública"
        allow="clipboard-write; clipboard-read"
        width="100%"
        height="100%"
        style={{ border: 'none', display: 'block' }}
      />
    </div>
  );
}
