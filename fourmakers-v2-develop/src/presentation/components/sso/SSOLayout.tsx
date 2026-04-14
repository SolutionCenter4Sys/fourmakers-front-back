import { Spinner } from "@/components/ui/spinner";
import logoFourmakers from "@/assets/logo-fourmakers.svg";

interface SSOLayoutProps {
  loginStatus: 'idle' | 'sending' | 'sent' | 'validating' | 'succeeded' | 'failed';
  errorMessage: string | null;
}

export function SSOLayout({ loginStatus, errorMessage }: SSOLayoutProps) {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-primaryBackground p-8">
      <style>
        {`
          @keyframes floatAndPulse {
            0%, 100% {
              transform: translateY(0px) scale(1);
              opacity: 1;
            }
            25% {
              transform: translateY(-10px) scale(1.05);
              opacity: 0.9;
            }
            50% {
              transform: translateY(-15px) scale(1.08);
              opacity: 0.85;
            }
            75% {
              transform: translateY(-10px) scale(1.05);
              opacity: 0.9;
            }
          }

          .logo-animated {
            animation: floatAndPulse 3s ease-in-out infinite;
          }
        `}
      </style>
      <div className="w-full max-w-md flex flex-col items-center">
        <img 
          src={logoFourmakers} 
          alt="Fourmakers" 
          className="h-16 w-auto dark:invert mb-8 logo-animated"
        />
        {loginStatus === 'validating' || loginStatus === 'idle' ? (
          <div className="flex items-center space-x-3 text-lg font-medium text-primary">
            <Spinner className="h-6 w-6" />
            <span>Validando acesso...</span>
          </div>
        ) : (
          <div className="text-center text-lg font-medium text-destructive">
            {errorMessage || "Ocorreu um erro inesperado."} 
            <p className="text-sm text-muted-foreground mt-2">Redirecionando para a tela de login...</p>
          </div>
        )}
      </div>
    </div>
  );
}
