import { useEffect } from 'react';
import { useParams, useLocation } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';

export function FlutterFlowContainer() {
  const params = useParams();
  const location = useLocation();
  const ffRoute = params['*']; // Use a chave '*' para acessar a parte da rota que corresponde ao wildcard
  const isLoadingMenu = useAppSelector((state) => state.menu.status === 'loading'); // Para decidir se mostra loading
  const user = useAppSelector((state) => state.auth.user);

  // Obter a URL base do FlutterFlow do ambiente
  const flutterflowBaseUrl = import.meta.env.VITE_FLUTTERFLOW_BASE_URL;

  // Garante que a flutterflowBaseUrl não tenha uma barra no final antes de concatenar
  const cleanFlutterflowBaseUrl = flutterflowBaseUrl?.endsWith('/')
    ? flutterflowBaseUrl.slice(0, -1)
    : flutterflowBaseUrl;
  
  // Repassa os query params da rota React para o iframe (ex.: /page/cvdigital?codigoInternoColaborador=xxx)
  const search = location.search ?? '';
  const fullFlutterflowUrl = cleanFlutterflowBaseUrl && ffRoute 
    ? `${cleanFlutterflowBaseUrl}/${ffRoute}${search}`
    : null;

  useEffect(() => {
    if (!fullFlutterflowUrl) return;

    // Enviar token proativamente quando o iframe carregar
    const sendTokenProactively = () => {
      const token = localStorage.getItem('authToken');
      if (!token) {
        console.warn('Auth token not found in localStorage.');
        return;
      }

      // Obter orgId do usuário
      const orgId = user?.colaboradorOrg?.orgId || user?.orgId || null;

      const iframe = document.getElementById('ff-frame-dynamic') as HTMLIFrameElement | null;
      if (iframe && iframe.contentWindow) {
        // Extrair a origem da URL do FlutterFlow
        try {
          const url = new URL(fullFlutterflowUrl);
          const origin = url.origin;
          
          iframe.contentWindow.postMessage(
            { type: 'AUTH_TOKEN', token, orgId },
            origin
          );
          console.log('Auth token and orgId sent proactively to iframe from FlutterFlowContainer.');
        } catch (error) {
          console.error('Error extracting origin from FlutterFlow URL:', error);
        }
      }
    };

    // Enviar token após um pequeno delay para garantir que o iframe carregou
    const timeoutId = setTimeout(sendTokenProactively, 1000);

    // Lógica para postMessage de autenticação quando solicitado
    function onRequest(e: MessageEvent) {
      const allowedIframeOrigins = new Set([
        'http://app.dev.fourmakers.io',
        'http://app.fourmakers.io',
        'https://fourmakers-2.flutterflow.app',
        'https://d57w8qc3lafbc.cloudfront.net',
        'https://d29p0sxk0z2lsa.cloudfront.net',
        'https://wf.dev.fourmakers.io',
        'https://wf.fourmakers.io',
      ]);

      if (!allowedIframeOrigins.has(e.origin)) {
        console.warn(`Blocked postMessage from untrusted origin: ${e.origin}`);
        return;
      }

      if (e.data?.type === 'REQUEST_AUTH_TOKEN') {
        const token = localStorage.getItem('authToken');
        if (!token) {
          console.warn('Auth token not found in localStorage.');
          return;
        }

        // Obter orgId do usuário
        const orgId = user?.colaboradorOrg?.orgId || user?.orgId || null;

        const iframe = document.getElementById('ff-frame-dynamic') as HTMLIFrameElement | null;
        if (iframe && iframe.contentWindow) {
          iframe.contentWindow.postMessage(
            { type: 'AUTH_TOKEN', token, orgId },
            e.origin
          );
          console.log('Auth token and orgId sent to iframe from FlutterFlowContainer.');
        } else {
          console.error('Dynamic iframe element or contentWindow not found.');
        }
      }

      // Handler para REQUEST_ORG_ID
      if (e.data?.type === 'REQUEST_ORG_ID') {
        // Obter orgId do usuário
        const orgId = user?.colaboradorOrg?.orgId || user?.orgId || null;

        const iframe = document.getElementById('ff-frame-dynamic') as HTMLIFrameElement | null;
        if (iframe && iframe.contentWindow) {
          iframe.contentWindow.postMessage(
            { type: 'ORG_ID', orgId },
            e.origin
          );
          console.log('ORG_ID sent to iframe from FlutterFlowContainer.');
        } else {
          console.error('Dynamic iframe element or contentWindow not found.');
        }
      }
    }

    window.addEventListener('message', onRequest);

    return () => {
      clearTimeout(timeoutId);
      window.removeEventListener('message', onRequest);
    };
  }, [fullFlutterflowUrl, user]);

  // Se o iframe for o único conteúdo, você pode querer um loading inicial
  if (isLoadingMenu || !flutterflowBaseUrl || !ffRoute || !fullFlutterflowUrl) {
    return (
      <div className="flex-1 flex items-center justify-center" style={{ height: 'calc(100vh - 72px)' }}>
        <p className="text-muted-foreground">Carregando conteúdo do FlutterFlow...</p>
      </div>
    );
  }

  return (
    <div 
      className="flex-1" 
      style={{ 
        height: 'calc(100vh - 72px)',
        margin: '0 -3rem -2rem',
        padding: 0
      }}
    >
      <iframe
        id="ff-frame-dynamic"
        src={fullFlutterflowUrl}
        title="FlutterFlow Dynamic Page"
        allow="clipboard-write; clipboard-read"
        width="100%"
        height="100%"
        scrolling="no"
        style={{ border: 'none', display: 'block' }}
      ></iframe>
    </div>
  );
}
