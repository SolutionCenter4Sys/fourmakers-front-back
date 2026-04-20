import { useState } from 'react';
import { Bell, Mail, Settings } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Switch } from '@/components/ui/switch';
import { Card, CardContent } from '@/components/ui/card';
import { toast } from 'sonner';

interface NotificationSettingsProps {
  trigger?: React.ReactNode;
}

export function NotificationSettings({ trigger }: NotificationSettingsProps) {
  const [open, setOpen] = useState(false);
  const [emailNotifications, setEmailNotifications] = useState(false);

  const handleSave = () => {
    toast.success('Configurações salvas!', {
      description: 'Suas preferências de notificação foram atualizadas.',
    });
    setOpen(false);
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger || (
          <button
            type="button"
            aria-label="Configurações de notificações"
            className="inline-flex items-center justify-center gap-2 h-9 px-3 rounded-pillToken border border-border/50 bg-transparent text-foreground hover:bg-muted/30 transition-colors"
          >
            <Bell className="h-4 w-4 shrink-0" />
            <Settings className="h-4 w-4 shrink-0" />
          </button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-md rounded-lg border-borderSoft">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-foreground">
            <span className="inline-flex items-center gap-2 rounded-pillToken border border-border/50 bg-transparent px-2.5 py-1">
              <Bell className="h-4 w-4 shrink-0" />
              <Settings className="h-4 w-4 shrink-0" />
            </span>
            Configurações de Notificações
          </DialogTitle>
          <DialogDescription>
            Gerencie como você deseja receber notificações da comunidade.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          <Card className="rounded-lg border-borderSoft shadow-none">
            <CardContent className="p-4">
              <div className="flex items-start justify-between gap-4">
                <div className="flex items-start gap-3 flex-1 min-w-0">
                  <div className="p-2 rounded-lg bg-primary/10 shrink-0">
                    <Mail className="w-5 h-5 text-primary" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-medium text-sm text-foreground">E-mail</p>
                    <p className="text-xs text-muted-foreground mt-0.5">
                      Receba notificações por e-mail
                    </p>
                  </div>
                </div>
                <Switch
                  checked={emailNotifications}
                  onCheckedChange={setEmailNotifications}
                />
              </div>
            </CardContent>
          </Card>

          <Card className="rounded-lg border-borderSoft shadow-none bg-muted/20">
            <CardContent className="p-4">
              <div className="flex items-start justify-between gap-4">
                <div className="flex items-start gap-3 flex-1 min-w-0">
                  <div className="p-2 rounded-lg bg-muted shrink-0">
                    <Bell className="w-5 h-5 text-muted-foreground" />
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-medium text-sm text-foreground">Notificações na Plataforma</p>
                    <p className="text-xs text-muted-foreground mt-0.5">
                      Veja notificações dentro de Fourmakers — você também recebe no sininho.
                    </p>
                  </div>
                </div>
                <Switch checked disabled className="data-[state=checked]:opacity-70" />
              </div>
              <p className="text-xs text-muted-foreground mt-2 pl-11">
                Esta opção está sempre ativa.
              </p>
            </CardContent>
          </Card>
        </div>

        <DialogFooter className="flex sm:flex-row sm:justify-end gap-2 pt-2">
          <Button onClick={handleSave} className="rounded-pillToken w-full sm:w-auto">
            Salvar Configurações
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
