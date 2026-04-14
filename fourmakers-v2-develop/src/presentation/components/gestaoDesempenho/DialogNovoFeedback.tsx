import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Calendar } from '@/components/ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { CalendarIcon } from '@/components/ui/system-icons';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { cn } from '@/lib/utils';
import { container } from '@core/di/container';
import { InserirFeedbackGestaoDesempenhoUseCase } from '@domain/usecases/InserirFeedbackGestaoDesempenhoUseCase';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';

interface DialogNovoFeedbackProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  colaboradorNome: string;
  colaboradorCargo: string;
  colaboradorCod: string;
  onSuccess?: () => void;
}

export function DialogNovoFeedback({
  open,
  onOpenChange,
  colaboradorNome,
  colaboradorCargo,
  colaboradorCod,
  onSuccess,
}: DialogNovoFeedbackProps) {
  const { token } = useAppSelector((state) => state.auth);
  const [data, setData] = useState<Date>(new Date());
  const [continuar, setContinuar] = useState('');
  const [comecar, setComecar] = useState('');
  const [parar, setParar] = useState('');
  const [observacoesGerais, setObservacoesGerais] = useState('');
  const [salvando, setSalvando] = useState(false);
  const [errors, setErrors] = useState<{
    continuar?: string;
    comecar?: string;
    parar?: string;
    data?: string;
  }>({});

  const validateForm = (): boolean => {
    const newErrors: typeof errors = {};

    if (!data) {
      newErrors.data = 'Data é obrigatória';
    }

    if (!continuar.trim()) {
      newErrors.continuar = 'Campo obrigatório';
    }

    if (!comecar.trim()) {
      newErrors.comecar = 'Campo obrigatório';
    }

    if (!parar.trim()) {
      newErrors.parar = 'Campo obrigatório';
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
      const useCase = container.resolve(InserirFeedbackGestaoDesempenhoUseCase);
      
      // Formatar data para YYYY-MM-DD
      const dataFormatada = format(data, 'yyyy-MM-dd');
      
      const payload = {
        codigoInternoColaboradorAvaliado: colaboradorCod,
        dataReuniao: dataFormatada,
        descricaoContinuar: continuar.trim(),
        descricaoComecar: comecar.trim(),
        descricaoParar: parar.trim(),
        descricaoObservacoesGerais: observacoesGerais.trim() || undefined,
      };

      const response = await useCase.execute(token, payload);

      if (response.sucesso) {
        toast.success('Feedback salvo com sucesso!');
        onOpenChange(false);
        // Resetar campos
        setContinuar('');
        setComecar('');
        setParar('');
        setObservacoesGerais('');
        setData(new Date());
        setErrors({});
        // Chamar callback para refetch
        if (onSuccess) {
          onSuccess();
        }
      } else {
        toast.error(response.mensagem || 'Erro ao salvar feedback');
      }
    } catch (error: any) {
      console.error('Erro ao salvar feedback:', error);
      toast.error(error.message || 'Erro ao salvar feedback. Tente novamente.');
    } finally {
      setSalvando(false);
    }
  };

  const handleCancelar = () => {
    onOpenChange(false);
    setContinuar('');
    setComecar('');
    setParar('');
    setObservacoesGerais('');
    setData(new Date());
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto" data-testid="gestao-desempenho-gestor-dialog-novo-feedback">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold">Novo Feedback</DialogTitle>
        </DialogHeader>

        <div className="space-y-6">
          {/* Colaborador */}
          <div className="p-4 bg-gray-100 rounded-lg">
            <Label className="text-sm font-medium text-muted-foreground block mb-2">Colaborador</Label>
            <div className="text-base font-medium text-foreground">
              {colaboradorNome} - {colaboradorCargo}
            </div>
          </div>

          {/* Data */}
          <div className="space-y-2">
            <Label className="text-sm font-medium text-muted-foreground">Data *</Label>
            <Popover>
              <PopoverTrigger asChild>
                <Button
                  variant="outline"
                  className={cn(
                    'w-full justify-start text-left font-normal',
                    !data && 'text-muted-foreground',
                    errors.data && 'border-destructive'
                  )}
                >
                  <CalendarIcon className="mr-2 h-4 w-4" />
                  {data ? format(data, 'dd/MM/yyyy', { locale: ptBR }) : 'Selecione a data'}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0">
                <Calendar
                  mode="single"
                  selected={data}
                  onSelect={(date) => {
                    if (date) {
                      setData(date);
                      setErrors((prev) => ({ ...prev, data: undefined }));
                    }
                  }}
                  initialFocus
                  locale={ptBR}
                />
              </PopoverContent>
            </Popover>
            {errors.data && (
              <p className="text-sm font-medium text-destructive">{errors.data}</p>
            )}
          </div>

          {/* Continuar */}
          <div className="space-y-2">
            <Label htmlFor="continuar" className="text-sm font-medium text-green-600">
              Continuar *
            </Label>
            <Textarea
              id="continuar"
              placeholder="O que o colaborador deve continuar fazendo..."
              value={continuar}
              onChange={(e) => {
                setContinuar(e.target.value);
                setErrors((prev) => ({ ...prev, continuar: undefined }));
              }}
              rows={3}
              className={cn('resize-none', errors.continuar && 'border-destructive')}
            />
            {errors.continuar && (
              <p className="text-sm font-medium text-destructive">{errors.continuar}</p>
            )}
          </div>

          {/* Começar */}
          <div className="space-y-2">
            <Label htmlFor="comecar" className="text-sm font-medium text-blue-600">
              Começar *
            </Label>
            <Textarea
              id="comecar"
              placeholder="O que o colaborador deve começar a fazer..."
              value={comecar}
              onChange={(e) => {
                setComecar(e.target.value);
                setErrors((prev) => ({ ...prev, comecar: undefined }));
              }}
              rows={3}
              className={cn('resize-none', errors.comecar && 'border-destructive')}
            />
            {errors.comecar && (
              <p className="text-sm font-medium text-destructive">{errors.comecar}</p>
            )}
          </div>

          {/* Cessar */}
          <div className="space-y-2">
            <Label htmlFor="parar" className="text-sm font-medium text-red-600">
              Cessar *
            </Label>
            <Textarea
              id="parar"
              placeholder="O que o colaborador deverá cessar"
              value={parar}
              onChange={(e) => {
                setParar(e.target.value);
                setErrors((prev) => ({ ...prev, parar: undefined }));
              }}
              rows={3}
              className={cn('resize-none', errors.parar && 'border-destructive')}
            />
            {errors.parar && (
              <p className="text-sm font-medium text-destructive">{errors.parar}</p>
            )}
          </div>

          {/* Observações gerais */}
          <div className="space-y-2">
            <Label htmlFor="observacoes" className="text-sm font-medium text-muted-foreground">Observações gerais</Label>
            <Textarea
              id="observacoes"
              placeholder="Comentários adicionais..."
              value={observacoesGerais}
              onChange={(e) => setObservacoesGerais(e.target.value)}
              rows={3}
              className="resize-none"
            />
          </div>

          {/* Botões */}
          <div className="flex justify-end gap-2 pt-4">
            <Button variant="outline" onClick={handleCancelar} data-testid="gestao-desempenho-gestor-dialog-novo-feedback-cancelar-button">
              Cancelar
            </Button>
            <Button onClick={handleSalvar} disabled={salvando || !data || !continuar.trim() || !comecar.trim() || !parar.trim()} data-testid="gestao-desempenho-gestor-dialog-novo-feedback-salvar-button">
              {salvando ? 'Salvando...' : 'Salvar Feedback'}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

