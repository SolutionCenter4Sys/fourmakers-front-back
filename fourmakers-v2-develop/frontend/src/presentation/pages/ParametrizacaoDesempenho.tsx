import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { 
  ArrowLeft,
  Settings,
  Clock,
  MessageCircle,
  Info
} from '@/components/ui/system-icons';
import { container } from '@core/di/container';
import { InserirParametrizacaoUseCase } from '@domain/usecases/InserirParametrizacaoUseCase';
import { ObterParametrizacaoUseCase } from '@domain/usecases/ObterParametrizacaoUseCase';
import { useAppSelector } from '@app/store/hooks';
import { toast } from 'sonner';

export default function ParametrizacaoDesempenho() {
  const navigate = useNavigate();
  const { token } = useAppSelector((state) => state.auth);
  const [frequencia1a1, setFrequencia1a1] = useState('14');
  const [frequenciaFeedback, setFrequenciaFeedback] = useState('30');
  const [salvando, setSalvando] = useState(false);
  const [carregando, setCarregando] = useState(true);

  // Carregar dados ao montar o componente
  useEffect(() => {
    const carregarParametrizacao = async () => {
      if (!token) {
        setCarregando(false);
        return;
      }

      try {
        const useCase = container.resolve(ObterParametrizacaoUseCase);
        const response = await useCase.execute(token);

        if (response.sucesso && response.retorno) {
          setFrequencia1a1(response.retorno.frequenciaEsperadaOneOnOneDias.toString());
          setFrequenciaFeedback(response.retorno.frequenciaEsperadaFeedbackDias.toString());
        }
      } catch (error: any) {
        console.error('Erro ao carregar parametrização:', error);
        // Em caso de erro, manter valores padrão já definidos no estado inicial
      } finally {
        setCarregando(false);
      }
    };

    carregarParametrizacao();
  }, [token]);

  const handleSalvar = async () => {
    if (!token) {
      toast.error('Token de autenticação não disponível.');
      return;
    }

    const frequencia1a1Num = parseInt(frequencia1a1, 10);
    const frequenciaFeedbackNum = parseInt(frequenciaFeedback, 10);

    if (isNaN(frequencia1a1Num) || frequencia1a1Num < 1) {
      toast.error('A frequência de 1:1 deve ser um número válido maior que 0.');
      return;
    }

    if (isNaN(frequenciaFeedbackNum) || frequenciaFeedbackNum < 1) {
      toast.error('A frequência de feedback deve ser um número válido maior que 0.');
      return;
    }

    setSalvando(true);
    try {
      const useCase = container.resolve(InserirParametrizacaoUseCase);
      const response = await useCase.execute(token, {
        FrequenciaEsperadaOneOnOneDias: frequencia1a1Num,
        FrequenciaEsperadaFeedbackDias: frequenciaFeedbackNum,
      });

      if (response.sucesso) {
        toast.success('Configurações salvas com sucesso!');
        navigate('/gestao-desempenho-rh');
      } else {
        toast.error(response.mensagem || 'Erro ao salvar configurações.');
      }
    } catch (error: any) {
      console.error('Erro ao salvar parametrização:', error);
      toast.error(error.message || 'Erro ao salvar configurações.');
    } finally {
      setSalvando(false);
    }
  };

  const handleCancelar = () => {
    navigate('/gestao-desempenho-rh');
  };

  if (carregando) {
    return (
      <div className="container mx-auto p-4">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-4 space-y-6">
      {/* Header */}
      <div className="flex items-start justify-between">
        <div>
          <Button
            variant="ghost"
            onClick={() => navigate('/gestao-desempenho-rh')}
            className="mb-2"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <div className="flex items-center gap-2 mb-2">
            <Settings className="h-6 w-6 text-green-600" />
            <h1 className="text-2xl font-bold">Parametrização</h1>
          </div>
          <p className="text-muted-foreground">
            Configure as regras de frequência e alertas para 1:1 e Feedback
          </p>
        </div>
        <Button 
          variant="outline"
          onClick={() => navigate('/gestao-desempenho-rh')}
        >
          <Settings className="h-4 w-4 mr-2" />
          Configurações RH
        </Button>
      </div>

      {/* Frequência de 1:1 */}
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center gap-2 mb-2">
            <Clock className="h-5 w-5 text-green-600" />
            <h2 className="text-lg font-semibold">Frequência de 1:1</h2>
          </div>
          <p className="text-sm text-muted-foreground mb-6">
            Defina a frequência esperada de reuniões 1:1 entre gestores e colaboradores
          </p>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <Label htmlFor="frequencia-1a1">Frequência esperada (dias)</Label>
              <Input
                id="frequencia-1a1"
                type="number"
                value={frequencia1a1}
                onChange={(e) => setFrequencia1a1(e.target.value)}
                min="1"
              />
              <p className="text-xs text-muted-foreground">
                1:1 será considerado "em dia" se realizado dentro deste período
              </p>
            </div>
            <Card className="bg-gray-50">
              <CardContent className="p-4">
                <div className="flex items-center gap-2 mb-3">
                  <Info className="h-4 w-4 text-muted-foreground" />
                  <h3 className="text-sm font-semibold">Regras aplicadas</h3>
                </div>
                <div className="space-y-2 text-sm">
                  <p className="text-green-600">
                    Em dia: até {frequencia1a1} dias desde o último 1:1
                  </p>
                  <p className="text-orange-600">
                    Atrasado: mais de {frequencia1a1} dias desde o último 1:1
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>
        </CardContent>
      </Card>

      {/* Frequência de Feedback */}
      <Card>
        <CardContent className="p-6">
          <div className="flex items-center gap-2 mb-2">
            <MessageCircle className="h-5 w-5 text-green-600" />
            <h2 className="text-lg font-semibold">Frequência de Feedback</h2>
          </div>
          <p className="text-sm text-muted-foreground mb-6">
            Defina a frequência esperada de feedbacks registrados para cada colaborador
          </p>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <Label htmlFor="frequencia-feedback">Frequência esperada (dias)</Label>
              <Input
                id="frequencia-feedback"
                type="number"
                value={frequenciaFeedback}
                onChange={(e) => setFrequenciaFeedback(e.target.value)}
                min="1"
              />
              <p className="text-xs text-muted-foreground">
                Feedback será considerado "em dia" se recebido dentro deste período
              </p>
            </div>
            <Card className="bg-gray-50">
              <CardContent className="p-4">
                <div className="flex items-center gap-2 mb-3">
                  <Info className="h-4 w-4 text-muted-foreground" />
                  <h3 className="text-sm font-semibold">Regras aplicadas</h3>
                </div>
                <div className="space-y-2 text-sm">
                  <p className="text-green-600">
                    Em dia: até {frequenciaFeedback} dias desde o último feedback
                  </p>
                  <p className="text-orange-600">
                    Atrasado: mais de {frequenciaFeedback} dias desde o último feedback
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>
        </CardContent>
      </Card>

      {/* Botões de ação */}
      <div className="flex justify-end gap-2">
        <Button variant="outline" onClick={handleCancelar}>
          Cancelar
        </Button>
        <Button onClick={handleSalvar} disabled={salvando}>
          {salvando ? 'Salvando...' : (
            <>
              <Settings className="h-4 w-4 mr-2" />
              Salvar Configurações
            </>
          )}
        </Button>
      </div>
    </div>
  );
}

