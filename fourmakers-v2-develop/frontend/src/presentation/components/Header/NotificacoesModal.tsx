import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import { ListarNotificacoesUseCase } from '@domain/usecases/ListarNotificacoesUseCase';
import { MarcarNotificacoesComoLidasUseCase } from '@domain/usecases/MarcarNotificacoesComoLidasUseCase';
import type { Notificacao } from '@domain/entities/Notificacao';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { Loader2, Bell, CheckCircle2, Circle } from 'lucide-react';
import { format, parseISO } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { toast } from 'sonner';

interface NotificacoesModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export const NotificacoesModal = ({ open, onOpenChange }: NotificacoesModalProps) => {
  const { token } = useAppSelector((state) => state.auth);
  const navigate = useNavigate();
  const [notificacoes, setNotificacoes] = useState<Notificacao[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<'novas' | 'todas'>('novas');

  const notificacoesNaoLidas = notificacoes.filter(n => !n.lida);

  useEffect(() => {
    if (open && token) {
      carregarNotificacoes();
    }
  }, [open, token]);

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

  const handleNotificacaoClick = async (notificacao: Notificacao) => {
    if (!token) return;

    // Se não estiver lida, marcar como lida
    if (!notificacao.lida) {
      try {
        const useCase = container.resolve(MarcarNotificacoesComoLidasUseCase);
        await useCase.execute(token);
        
        // Atualizar estado local
        setNotificacoes(prev => 
          prev.map(n => ({ ...n, lida: true, dataLeitura: new Date().toISOString() }))
        );
      } catch (error) {
        console.error('Erro ao marcar notificações como lidas:', error);
        toast.error('Erro ao marcar notificações como lidas');
      }
    }

    // Navegar se houver rota
    if (notificacao.rotaCompleta) {
      navigate(notificacao.rotaCompleta);
      onOpenChange(false);
    } else if (notificacao.rota) {
      navigate(notificacao.rota);
      onOpenChange(false);
    }
  };

  /** Interpreta dataEnvio da API como UTC (se sem timezone) e formata no fuso do usuário. */
  const formatarData = (data: string) => {
    try {
      const trimmed = data?.trim() || '';
      if (!trimmed) return data;
      const jaTemFuso = /Z$|[-+]\d{2}:?\d{2}$/i.test(trimmed);
      const isoUtc = jaTemFuso ? trimmed : (trimmed.endsWith('Z') ? trimmed : `${trimmed.replace(/Z$/i, '')}Z`);
      const dataObj = parseISO(isoUtc);
      if (Number.isNaN(dataObj.getTime())) return data;
      return format(dataObj, "dd 'de' MMMM 'às' HH:mm", { locale: ptBR });
    } catch {
      return data;
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-hidden flex flex-col p-0 gap-0">
        <DialogHeader className="p-6 border-b bg-muted/30">
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
              <Bell className="h-5 w-5 text-primary" />
            </div>
            <div className="flex-1">
              <DialogTitle className="text-2xl font-bold">Notificações</DialogTitle>
              <DialogDescription className="mt-1">
                Visualize suas notificações
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="flex-1 overflow-hidden flex flex-col">
          <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as 'novas' | 'todas')} className="flex-1 flex flex-col">
            <div className="px-6 pt-4 border-b">
              <TabsList className="grid w-full grid-cols-2">
                <TabsTrigger value="novas" className="gap-2">
                  Novas
                  {notificacoesNaoLidas.length > 0 && (
                    <Badge variant="destructive" className="ml-1">
                      {notificacoesNaoLidas.length}
                    </Badge>
                  )}
                </TabsTrigger>
                <TabsTrigger value="todas">
                  Todas
                </TabsTrigger>
              </TabsList>
            </div>

            <TabsContent value="novas" className="flex-1 overflow-y-auto p-6 m-0">
              {isLoading ? (
                <div className="flex items-center justify-center py-8">
                  <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
                </div>
              ) : notificacoesNaoLidas.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">
                  <Bell className="h-12 w-12 mx-auto mb-4 opacity-50" />
                  <p>Nenhuma notificação nova</p>
                </div>
              ) : (
                <div className="space-y-3">
                  {notificacoesNaoLidas.map((notificacao, index) => (
                    <Card
                      key={index}
                      className={`cursor-pointer transition-all hover:shadow-md ${
                        !notificacao.lida ? 'border-primary/50 bg-primary/5' : ''
                      }`}
                      onClick={() => handleNotificacaoClick(notificacao)}
                    >
                      <CardContent className="p-4">
                        <div className="flex items-start gap-3">
                          <div className="mt-1">
                            {notificacao.lida ? (
                              <CheckCircle2 className="h-5 w-5 text-muted-foreground" />
                            ) : (
                              <Circle className="h-5 w-5 text-primary fill-primary" />
                            )}
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="flex items-start justify-between gap-2 mb-2">
                              <h4 className="font-semibold text-sm">{notificacao.titulo}</h4>
                              {!notificacao.lida && (
                                <Badge variant="destructive" className="text-xs">Nova</Badge>
                              )}
                            </div>
                            <div
                              className="text-sm text-muted-foreground mb-2"
                              dangerouslySetInnerHTML={{ __html: notificacao.mensagemHtml || notificacao.mensagem }}
                            />
                            <p className="text-xs text-muted-foreground">
                              {formatarData(notificacao.dataEnvio)}
                            </p>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              )}
            </TabsContent>

            <TabsContent value="todas" className="flex-1 overflow-y-auto p-6 m-0">
              {isLoading ? (
                <div className="flex items-center justify-center py-8">
                  <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
                </div>
              ) : notificacoes.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">
                  <Bell className="h-12 w-12 mx-auto mb-4 opacity-50" />
                  <p>Nenhuma notificação</p>
                </div>
              ) : (
                <div className="space-y-3">
                  {notificacoes.map((notificacao, index) => (
                    <Card
                      key={index}
                      className={`cursor-pointer transition-all hover:shadow-md ${
                        !notificacao.lida ? 'border-primary/50 bg-primary/5' : ''
                      }`}
                      onClick={() => handleNotificacaoClick(notificacao)}
                    >
                      <CardContent className="p-4">
                        <div className="flex items-start gap-3">
                          <div className="mt-1">
                            {notificacao.lida ? (
                              <CheckCircle2 className="h-5 w-5 text-muted-foreground" />
                            ) : (
                              <Circle className="h-5 w-5 text-primary fill-primary" />
                            )}
                          </div>
                          <div className="flex-1 min-w-0">
                            <div className="flex items-start justify-between gap-2 mb-2">
                              <h4 className="font-semibold text-sm">{notificacao.titulo}</h4>
                              {!notificacao.lida && (
                                <Badge variant="destructive" className="text-xs">Nova</Badge>
                              )}
                            </div>
                            <div
                              className="text-sm text-muted-foreground mb-2"
                              dangerouslySetInnerHTML={{ __html: notificacao.mensagemHtml || notificacao.mensagem }}
                            />
                            <p className="text-xs text-muted-foreground">
                              {formatarData(notificacao.dataEnvio)}
                            </p>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              )}
            </TabsContent>
          </Tabs>
        </div>
      </DialogContent>
    </Dialog>
  );
};

