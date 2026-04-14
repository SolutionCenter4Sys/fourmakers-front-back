import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import { ListarNotificacoesUseCase } from '@domain/usecases/ListarNotificacoesUseCase';
import { MarcarNotificacoesComoLidasUseCase } from '@domain/usecases/MarcarNotificacoesComoLidasUseCase';
import type { Notificacao } from '@domain/entities/Notificacao';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Loader2, Bell, MoreVertical } from 'lucide-react';
import { format, isToday, isYesterday, differenceInDays, parseISO } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { toast } from 'sonner';
import { cn } from '@/lib/utils';

interface NotificacoesPopoverProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  children: React.ReactNode;
  onMarcarComoLidas?: () => void;
}

export const NotificacoesPopover = ({ open, onOpenChange, children, onMarcarComoLidas }: NotificacoesPopoverProps) => {
  const { token } = useAppSelector((state) => state.auth);
  const navigate = useNavigate();
  const [notificacoes, setNotificacoes] = useState<Notificacao[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<'todas' | 'nao-lidas'>('nao-lidas');
  const prevOpenRef = useRef(open);

  const notificacoesNaoLidas = notificacoes.filter(n => !n.lida);

  useEffect(() => {
    if (open && token) {
      carregarNotificacoes();
    }
  }, [open, token]);

  // Marca como lidas quando o modal é fechado
  useEffect(() => {
    // Detecta quando o modal muda de aberto para fechado
    if (prevOpenRef.current === true && open === false && token) {
      marcarTodasComoLidas();
    }
    prevOpenRef.current = open;
  }, [open, token]);

  const marcarTodasComoLidas = async () => {
    if (!token) return;

    try {
      const useCase = container.resolve(MarcarNotificacoesComoLidasUseCase);
      await useCase.execute(token);
      
      // Atualizar estado local
      setNotificacoes(prev => 
        prev.map(n => ({ ...n, lida: true, dataLeitura: new Date().toISOString() }))
      );
      
      // Notificar o hook para atualizar o contador
      if (onMarcarComoLidas) {
        onMarcarComoLidas();
      }
    } catch (error) {
      console.error('Erro ao marcar notificações como lidas:', error);
      // Não exibir erro para o usuário, apenas logar
    }
  };

  const carregarNotificacoes = async () => {
    if (!token) return;

    setIsLoading(true);
    try {
      const useCase = container.resolve(ListarNotificacoesUseCase);
      const lista = await useCase.execute(token);
      setNotificacoes(lista);
    } catch (error) {
      console.error('Erro ao carregar notificações:', error);
      toast.error('Erro ao carregar notificações');
    } finally {
      setIsLoading(false);
    }
  };

  const handleNotificacaoClick = (notificacao: Notificacao) => {
    // Navegar se houver rota
    if (notificacao.rotaCompleta) {
      navigate(notificacao.rotaCompleta);
      onOpenChange(false);
    } else if (notificacao.rota) {
      navigate(notificacao.rota);
      onOpenChange(false);
    }
  };

  /** Interpreta dataEnvio da API como UTC (se sem timezone) e retorna Date para exibição no fuso do usuário. */
  const parseDataEnvioParaFusoUsuario = (data: string): Date => {
    const trimmed = data?.trim() || '';
    if (!trimmed) return new Date(Number.NaN);
    try {
      const jaTemFuso = /Z$|[-+]\d{2}:?\d{2}$/i.test(trimmed);
      const isoUtc = jaTemFuso ? trimmed : (trimmed.endsWith('Z') ? trimmed : `${trimmed.replace(/Z$/i, '')}Z`);
      return parseISO(isoUtc);
    } catch {
      return new Date(Number.NaN);
    }
  };

  const formatarDataRelativa = (data: string) => {
    try {
      const dataObj = parseDataEnvioParaFusoUsuario(data);
      if (Number.isNaN(dataObj.getTime())) return data;

      if (isToday(dataObj)) {
        const horas = format(dataObj, 'HH:mm');
        return `Hoje às ${horas}`;
      }

      if (isYesterday(dataObj)) {
        const horas = format(dataObj, 'HH:mm');
        return `Ontem às ${horas}`;
      }

      const diasDiff = differenceInDays(new Date(), dataObj);

      if (diasDiff < 7) {
        return format(dataObj, "EEEE 'às' HH:mm", { locale: ptBR });
      }

      if (diasDiff < 30) {
        return format(dataObj, "d 'de' MMMM", { locale: ptBR });
      }

      return format(dataObj, "d 'de' MMMM 'de' yyyy", { locale: ptBR });
    } catch {
      return data;
    }
  };

  const agruparNotificacoes = (lista: Notificacao[]) => {
    const novas = lista.filter(n => !n.lida);

    const hoje = lista.filter(n => {
      try {
        const dataObj = parseDataEnvioParaFusoUsuario(n.dataEnvio);
        return !Number.isNaN(dataObj.getTime()) && isToday(dataObj) && n.lida;
      } catch {
        return false;
      }
    });

    const anteriores = lista.filter(n => {
      try {
        const dataObj = parseDataEnvioParaFusoUsuario(n.dataEnvio);
        return !Number.isNaN(dataObj.getTime()) && !isToday(dataObj) && n.lida;
      } catch {
        return false;
      }
    });

    return { novas, hoje, anteriores };
  };

  const notificacoesExibidas = activeTab === 'nao-lidas' ? notificacoesNaoLidas : notificacoes;
  const { novas, hoje, anteriores } = agruparNotificacoes(notificacoesExibidas);
  const totalExibidas = novas.length + hoje.length + anteriores.length;

  const getNotificationIcon = (_notificacao: Notificacao) => {
    // Você pode personalizar isso baseado no tipo de notificação
    return <Bell className="h-5 w-5 text-primary" />;
  };

  return (
    <Popover open={open} onOpenChange={onOpenChange}>
      <PopoverTrigger asChild>
        {children}
      </PopoverTrigger>
      <PopoverContent
        align="end"
        side="bottom"
        sideOffset={12}
        className={cn(
          "w-[420px] p-0 shadow-2xl border border-border/60",
          "max-h-[600px] overflow-hidden flex flex-col",
          "bg-background backdrop-blur-md",
          "rounded-xl"
        )}
        onOpenAutoFocus={(e) => e.preventDefault()}
      >
        {/* Header */}
        <div className="px-5 py-4 border-b border-border/50">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold text-foreground">Notificações</h3>
            <button
              onClick={() => onOpenChange(false)}
              className="p-1.5 rounded-full hover:bg-muted transition-colors -mr-1"
              aria-label="Fechar"
            >
              <MoreVertical className="h-4 w-4 text-muted-foreground" />
            </button>
          </div>
        </div>

        {/* Tabs */}
        <div className="px-5 pt-3 pb-2 border-b border-border/50">
          <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as 'todas' | 'nao-lidas')}>
            <TabsList className="grid w-full grid-cols-2 h-10 bg-muted/50">
              <TabsTrigger value="nao-lidas" className="text-sm font-semibold data-[state=active]:bg-background">
                Não lidas
                {notificacoesNaoLidas.length > 0 && (
                  <span className="ml-2 px-1.5 py-0.5 text-xs font-bold rounded-full bg-primary text-primary-foreground min-w-[20px] text-center">
                    {notificacoesNaoLidas.length}
                  </span>
                )}
              </TabsTrigger>
              <TabsTrigger value="todas" className="text-sm font-semibold data-[state=active]:bg-background">
                Tudo
              </TabsTrigger>
            </TabsList>
          </Tabs>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto">
          {isLoading ? (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
          ) : totalExibidas === 0 ? (
            <div className="text-center py-12 px-4">
              <div className="w-16 h-16 rounded-full bg-muted flex items-center justify-center mx-auto mb-4">
                <Bell className="h-8 w-8 text-muted-foreground/50" />
              </div>
              <p className="text-sm text-muted-foreground font-medium">
                {activeTab === 'nao-lidas' ? 'Nenhuma notificação nova' : 'Nenhuma notificação'}
              </p>
            </div>
          ) : (
            <div className="py-2">
              {/* Novas */}
              {novas.length > 0 && (
                <div className="px-5 py-3">
                  <h4 className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-3 px-1">
                    Novas
                  </h4>
                  <div className="space-y-0.5">
                    {novas.map((notificacao, index) => (
                      <NotificationItem
                        key={index}
                        notificacao={notificacao}
                        onClick={() => handleNotificacaoClick(notificacao)}
                        formatarData={formatarDataRelativa}
                        getIcon={getNotificationIcon}
                      />
                    ))}
                  </div>
                </div>
              )}

              {/* Hoje */}
              {hoje.length > 0 && (
                <div className={cn("px-5 py-3", novas.length > 0 && "border-t border-border/30")}>
                  <h4 className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-3 px-1">
                    Hoje
                  </h4>
                  <div className="space-y-0.5">
                    {hoje.map((notificacao, index) => (
                      <NotificationItem
                        key={index}
                        notificacao={notificacao}
                        onClick={() => handleNotificacaoClick(notificacao)}
                        formatarData={formatarDataRelativa}
                        getIcon={getNotificationIcon}
                      />
                    ))}
                  </div>
                </div>
              )}

              {/* Anteriores */}
              {anteriores.length > 0 && (
                <div className={cn("px-5 py-3", (novas.length > 0 || hoje.length > 0) && "border-t border-border/30")}>
                  <h4 className="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-3 px-1">
                    Anteriores
                  </h4>
                  <div className="space-y-0.5">
                    {anteriores.map((notificacao, index) => (
                      <NotificationItem
                        key={index}
                        notificacao={notificacao}
                        onClick={() => handleNotificacaoClick(notificacao)}
                        formatarData={formatarDataRelativa}
                        getIcon={getNotificationIcon}
                      />
                    ))}
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </PopoverContent>
    </Popover>
  );
};

interface NotificationItemProps {
  notificacao: Notificacao;
  onClick: () => void;
  formatarData: (data: string) => string;
  getIcon: (notificacao: Notificacao) => React.ReactNode;
}

const NotificationItem = ({ notificacao, onClick, formatarData, getIcon }: NotificationItemProps) => {
  return (
    <div
      onClick={onClick}
      className={cn(
        "flex items-start gap-3 px-3 py-2.5 rounded-lg cursor-pointer transition-all group",
        "hover:bg-muted/60 active:bg-muted/80",
        !notificacao.lida && "bg-primary/5"
      )}
    >
      {/* Avatar/Icon */}
      <div className="relative flex-shrink-0 mt-0.5">
        <div className={cn(
          "w-10 h-10 rounded-full flex items-center justify-center transition-colors",
          !notificacao.lida 
            ? "bg-primary/15" 
            : "bg-muted"
        )}>
          {getIcon(notificacao)}
        </div>
        {!notificacao.lida && (
          <div className="absolute -top-0.5 -right-0.5 w-3 h-3 rounded-full bg-primary border-2 border-background shadow-sm" />
        )}
      </div>

      {/* Content */}
      <div className="flex-1 min-w-0">
        <div className="flex items-start justify-between gap-2 mb-1">
          <h5 className={cn(
            "text-sm leading-tight",
            !notificacao.lida ? "font-semibold text-foreground" : "font-medium text-foreground/90"
          )}>
            {notificacao.titulo}
          </h5>
        </div>
        <div
          className={cn(
            "text-sm leading-relaxed mb-1.5 line-clamp-2",
            !notificacao.lida ? "text-foreground/80" : "text-muted-foreground"
          )}
          dangerouslySetInnerHTML={{ __html: notificacao.mensagemHtml || notificacao.mensagem }}
        />
        <p className="text-xs text-muted-foreground/60 mt-0.5">
          {formatarData(notificacao.dataEnvio)}
        </p>
      </div>
    </div>
  );
};

