import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Calendar } from '@/components/ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { CalendarIcon, AlertTriangle, FileText } from '@/components/ui/system-icons';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { cn } from '@/lib/utils';
import { Checkbox } from '@/components/ui/checkbox';
import { container } from '@core/di/container';
import { InserirOneOnOneUseCase } from '@domain/usecases/InserirOneOnOneUseCase';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';
import { Badge } from '@/components/ui/badge';
import type { PautaSugerida } from '@shared/types/gestaoDesempenho';

interface DialogNovoRegistro1on1Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  colaboradorNome: string;
  colaboradorCargo: string;
  colaboradorCod: string;
  pautas?: PautaSugerida[];
  onSuccess?: () => void;
}

export function DialogNovoRegistro1on1({
  open,
  onOpenChange,
  colaboradorNome,
  colaboradorCargo,
  colaboradorCod,
  pautas = [],
  onSuccess,
}: DialogNovoRegistro1on1Props) {
  const { token } = useAppSelector((state) => state.auth);
  const [dataReuniao, setDataReuniao] = useState<Date>(new Date());
  const [anotacoes, setAnotacoes] = useState('');
  const [registroCritico, setRegistroCritico] = useState(false);
  const [salvando, setSalvando] = useState(false);
  const [errors, setErrors] = useState<{
    dataReuniao?: string;
    anotacoes?: string;
  }>({});

  const validateForm = (): boolean => {
    const newErrors: typeof errors = {};

    if (!dataReuniao) {
      newErrors.dataReuniao = 'Data da reunião é obrigatória';
    }

    if (!anotacoes.trim()) {
      newErrors.anotacoes = 'Campo obrigatório';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSalvar = async () => {
    if (!validateForm()) {
      toast.error('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    if (!token || !colaboradorCod) {
      toast.error('Erro: token ou código do colaborador não encontrado.');
      return;
    }

    setSalvando(true);
    try {
      const useCase = container.resolve(InserirOneOnOneUseCase);
      
      // Formatar data para YYYY-MM-DD
      const dataFormatada = format(dataReuniao, 'yyyy-MM-dd');
      
      const payload = {
        codigoInternoColaboradorAvaliado: colaboradorCod,
        dataReuniao: dataFormatada,
        descricaoAnotacoes: anotacoes.trim(),
        registroCritico: registroCritico,
      };

      const response = await useCase.execute(token, payload);

      if (response.sucesso) {
        toast.success('Registro 1:1 salvo com sucesso!');
        onOpenChange(false);
        // Resetar campos
        setAnotacoes('');
        setRegistroCritico(false);
        setDataReuniao(new Date());
        setErrors({});
        // Chamar callback para refetch
        if (onSuccess) {
          onSuccess();
        }
      } else {
        toast.error(response.mensagem || 'Erro ao salvar registro 1:1');
      }
    } catch (error: any) {
      console.error('Erro ao salvar registro 1:1:', error);
      toast.error(error.message || 'Erro ao salvar registro 1:1. Tente novamente.');
    } finally {
      setSalvando(false);
    }
  };

  const handleCancelar = () => {
    onOpenChange(false);
    setAnotacoes('');
    setRegistroCritico(false);
    setDataReuniao(new Date());
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto" data-testid="gestao-desempenho-gestor-dialog-novo-1-1">
        <DialogHeader>
          <DialogTitle>Novo Registro de 1:1</DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          {/* Colaborador */}
          <div className="p-4 bg-gray-100 rounded-lg">
            <Label className="text-sm font-medium text-muted-foreground block mb-2">Colaborador</Label>
            <div className="text-base font-medium text-foreground">
              {colaboradorNome} - {colaboradorCargo}
            </div>
          </div>

          {/* Data da reunião */}
          <div className="space-y-2">
            <Label>Data da reunião *</Label>
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  className={cn(
                    'w-full justify-start text-left font-normal',
                    !dataReuniao && 'text-muted-foreground',
                    errors.dataReuniao && 'border-destructive'
                  )}
                >
                  <CalendarIcon className="mr-2 h-4 w-4" />
                  {dataReuniao ? format(dataReuniao, 'dd/MM/yyyy', { locale: ptBR }) : 'Selecione a data'}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0">
                <Calendar
                  mode="single"
                  selected={dataReuniao}
                  onSelect={(date) => {
                    if (date) {
                      setDataReuniao(date);
                      setErrors((prev) => ({ ...prev, dataReuniao: undefined }));
                    }
                  }}
                  initialFocus
                  locale={ptBR}
                />
              </PopoverContent>
            </Popover>
            {errors.dataReuniao && (
              <p className="text-sm font-medium text-destructive">{errors.dataReuniao}</p>
            )}
          </div>

          {/* Pautas Sugeridas */}
          {pautas.length > 0 && (
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <FileText className="h-4 w-4 text-muted-foreground" />
                <Label className="text-base font-semibold">Pautas Sugeridas</Label>
              </div>
              <div className="bg-gray-50 rounded-lg p-4 border border-gray-200">
                <ul className="space-y-2">
                  {pautas.map((pauta) => {
                    const mostrarTag = pauta.origem === "COLABORADOR" || pauta.origem === "SUBORDINADO";
                    return (
                      <li key={pauta.id} className="flex items-center gap-2 text-sm text-muted-foreground">
                        <span className="w-2 h-2 bg-green-600 rounded-full flex-shrink-0"></span>
                        <span className="flex-1">{pauta.descricao}</span>
                        {mostrarTag && (
                          <Badge variant="secondary" className="text-xs flex-shrink-0">
                            {pauta.origem}
                          </Badge>
                        )}
                      </li>
                    );
                  })}
                </ul>
              </div>
            </div>
          )}

          {/* Anotações */}
          <div className="space-y-2">
            <Label htmlFor="anotacoes">
              Anotações *
            </Label>
            <Textarea
              id="anotacoes"
              placeholder="Registre os principais pontos discutidos na reunião..."
              value={anotacoes}
              onChange={(e) => {
                setAnotacoes(e.target.value);
                setErrors((prev) => ({ ...prev, anotacoes: undefined }));
              }}
              rows={3}
              className={cn('resize-none', errors.anotacoes && 'border-destructive')}
            />
            {errors.anotacoes && (
              <p className="text-sm font-medium text-destructive">{errors.anotacoes}</p>
            )}
          </div>

          {/* Registro Crítico */}
          <div className="border border-orange-200 bg-orange-50 rounded-lg p-4">
            <div className="flex items-start gap-3">
              <Checkbox
                id="critico"
                checked={registroCritico}
                onCheckedChange={(checked) => setRegistroCritico(checked === true)}
                className="mt-1"
              />
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-2">
                  <AlertTriangle className="h-5 w-5 text-orange-600" />
                  <Label htmlFor="critico" className="font-semibold cursor-pointer">
                    Registro Crítico
                  </Label>
                </div>
                <p className="text-sm text-muted-foreground">
                  Marque se este registro requer atenção especial e acompanhamento prioritário.
                </p>
              </div>
            </div>
          </div>

          {/* Botões */}
          <div className="flex justify-end gap-2 pt-4">
            <Button variant="outline" onClick={handleCancelar} data-testid="gestao-desempenho-gestor-dialog-novo-1-1-cancelar-button">
              Cancelar
            </Button>
            <Button onClick={handleSalvar} disabled={salvando || !dataReuniao || !anotacoes.trim()} data-testid="gestao-desempenho-gestor-dialog-novo-1-1-salvar-button">
              {salvando ? 'Salvando...' : 'Salvar Registro'}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

