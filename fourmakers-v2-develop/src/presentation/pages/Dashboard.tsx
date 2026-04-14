import { useAppSelector } from '@app/store/hooks';
import { useEffect, useState } from 'react';
import { AtualizarPerfilModal } from '@presentation/components/dashboard';
import { DashboardRoyal } from '@presentation/components/Dashboards';

export function Dashboard() {
  const { user } = useAppSelector((state) => state.auth);
  const [isModalQuestionarioOpen, setIsModalQuestionarioOpen] = useState(false);
  const flutterflowBaseUrl = import.meta.env.VITE_FLUTTERFLOW_BASE_URL;
  const dashboardUrl = `${flutterflowBaseUrl}/homedashboard2`;

  // MainLayout já carrega o perfil do usuário, não precisamos fazer aqui

  // Verificar se deve mostrar o modal de atualização de perfil
  useEffect(() => {
    if (user) {
      const QUESTIONARIO_CODIGO = '2025_01_COLETA_PERFIL_COLABORADOR';
      const naoPreenchido = !user.questionariosPreenchidos?.includes(QUESTIONARIO_CODIGO);
      const estaAtivo = user.questionariosAtivos?.includes(QUESTIONARIO_CODIGO);
      
      if (naoPreenchido && estaAtivo) {
        setIsModalQuestionarioOpen(true);
      }
    }
  }, [user]);

  useEffect(() => {
    function onRequest(e: MessageEvent) {
      const allowedIframeOrigins = new Set([
        'https://app.dev.fourmakers.io',
        'https://app.fourmakers.io',
        'https://fourmakers-2.flutterflow.app',
        'https://d57w8qc3lafbc.cloudfront.net',
        'https://d29p0sxk0z2lsa.cloudfront.net',
        'https://wf.dev.fourmakers.io',
        'https://wf.fourmakers.io'

      ]);

      if (!allowedIframeOrigins.has(e.origin)) {
        // Opcional: Logar tentativas de comunicação de origens não permitidas para depuração
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

        const iframe = document.getElementById('ff-frame') as HTMLIFrameElement | null;
        if (iframe && iframe.contentWindow) {
          iframe.contentWindow.postMessage(
            { type: 'AUTH_TOKEN', token, orgId },
            e.origin
          );
          console.log('Auth token and orgId sent to iframe.');
        } else {
          console.error('Iframe element or contentWindow not found.');
        }
      }

      // Handler para REQUEST_ORG_ID
      if (e.data?.type === 'REQUEST_ORG_ID') {
        // Obter orgId do usuário
        const orgId = user?.colaboradorOrg?.orgId || user?.orgId || null;

        const iframe = document.getElementById('ff-frame') as HTMLIFrameElement | null;
        if (iframe && iframe.contentWindow) {
          iframe.contentWindow.postMessage(
            { type: 'ORG_ID', orgId },
            e.origin
          );
          console.log('ORG_ID sent to iframe.');
        } else {
          console.error('Iframe element or contentWindow not found.');
        }
      }
    }

    window.addEventListener('message', onRequest);

    return () => {
      window.removeEventListener('message', onRequest);
    };
  }, [user]); // Adicionar user como dependência para ter acesso ao orgId

  if (!user) {
    return (
      <div className="p-4">
        <h1 className="text-2xl font-bold mb-4">Dashboard</h1>
        <p>Por favor, faça login para acessar a dashboard.</p>
      </div>
    );
  }

  // Verificar se é orgId 9 (Royal) para renderizar dashboard customizado
  const isRoyalOrg = user?.colaboradorOrg?.orgId === 9;

  if (isRoyalOrg) {
    return (
      <>
        <DashboardRoyal />
        {/* Iframe invisível para passar token e orgId para o FlutterFlow */}
        <div style={{ display: 'none' }}>
          <iframe
            id="ff-frame"
            src={dashboardUrl}
            title="FlutterFlow Test Frame"
            allow="clipboard-write; clipboard-read"
            width="100%"
            height="800px"
            style={{ border: 'none' }}
          ></iframe>
        </div>
        <AtualizarPerfilModal
          open={isModalQuestionarioOpen}
          onOpenChange={setIsModalQuestionarioOpen}
        />
      </>
    );
  }

  return (
    <div className="flex-1 space-y-4 p-8 pt-6">
      
      {/* Se o usuário não estiver disponível, você pode mostrar uma mensagem de carregamento ou redirecionar */}
      {!user && (
        <p className="text-muted-foreground">Carregando informações do usuário...</p>
      )}

      {/* Embed da página externa */}
      <div>
        <iframe
          id="ff-frame"
          src={dashboardUrl}
          title="FlutterFlow Test Frame"
          allow="clipboard-write; clipboard-read"
          width="100%"
          height="800px" // Ajuste a altura conforme necessário
          style={{ border: 'none' }}
        ></iframe>
      </div>

      {/* Modal de Atualização de Perfil */}
      <AtualizarPerfilModal
        open={isModalQuestionarioOpen}
        onOpenChange={setIsModalQuestionarioOpen}
      />
    </div>
  );
}
