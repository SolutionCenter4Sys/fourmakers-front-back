import { useState } from 'react';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import { EnviarSentimentoUseCase } from '@domain/usecases/EnviarSentimentoUseCase';
import type { FelizometroPayload } from '@domain/entities/FelizometroSentimento';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { toast } from 'sonner';
import { Loader2 } from 'lucide-react';

type Sentimento = 
  | 'motivado'
  | 'bem'
  | 'desmotivado'
  | 'preocupado'
  | 'cansado'
  | 'triste'
  | 'doente'
  | 'outro';

interface SentimentoOption {
  id: Sentimento;
  label: string;
  emoji: string;
}

const sentimentos: SentimentoOption[] = [
  { id: 'motivado', label: 'Motivado(a)', emoji: '🚀' },
  { id: 'bem', label: 'Bem', emoji: '😊' },
  { id: 'desmotivado', label: 'Desmotivado(a)', emoji: '😔' },
  { id: 'preocupado', label: 'Preocupado(a)', emoji: '😰' },
  { id: 'cansado', label: 'Cansado(a)', emoji: '😴' },
  { id: 'triste', label: 'Triste', emoji: '😢' },
  { id: 'doente', label: 'Doente', emoji: '🤒' },
  { id: 'outro', label: 'Outro', emoji: '➕' },
];

export default function SentimentoHoje() {
  const user = useAppSelector((state) => state.auth.user);
  const [sentimentoSelecionado, setSentimentoSelecionado] = useState<Sentimento | null>(null);
  const [descricao, setDescricao] = useState('');
  const [tituloOutro, setTituloOutro] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSentimentoClick = (sentimento: Sentimento) => {
    setSentimentoSelecionado(sentimento);
    if (sentimento !== 'outro') {
      setTituloOutro('');
    } else {
      setDescricao('');
    }
  };

  const handleSubmit = async () => {
    if (!user) {
      toast.error('Usuário não encontrado. Por favor, faça login novamente.');
      return;
    }

    setIsSubmitting(true);

    try {
      const useCase = container.resolve(EnviarSentimentoUseCase);
      
      // Determinar o emotion (sentimento)
      const emotion = sentimentoSelecionado === 'outro' 
        ? tituloOutro 
        : sentimentos.find(s => s.id === sentimentoSelecionado)?.label || '';

      // Montar o payload
      const payload: FelizometroPayload = {
        collaborator_id: String(user.usuarioId),
        collaborator: user.nomeColaborador,
        manager: user.orgHierarquia?.nomeProfissionalSuperior || '',
        emotion: emotion,
        message: descricao.trim() || null,
        date: new Date().toISOString(),
        other_emotion: sentimentoSelecionado === 'outro' ? tituloOutro : null,
        unit: user.colaboradorOrg?.diretoria || user.colaborador?.diretoria?.diretoria || '',
      };

      await useCase.execute(payload);

      toast.success('Obrigado!! É sempre bom saber como está se sentindo');
      
      // Limpar formulário
      setSentimentoSelecionado(null);
      setDescricao('');
      setTituloOutro('');
    } catch (error) {
      console.error('Erro ao enviar sentimento:', error);
      toast.error('Houve um erro ao processar a mensagem');
    } finally {
      setIsSubmitting(false);
    }
  };

  const isOutro = sentimentoSelecionado === 'outro';
  const podeEnviar = sentimentoSelecionado && (isOutro ? tituloOutro.trim() : true) && !isSubmitting;
  const sentimentoAtual = sentimentos.find(s => s.id === sentimentoSelecionado);

  if (!user) {
    return (
      <div className="min-h-screen bg-primaryBackground p-4 flex items-center justify-center">
        <p className="text-muted-foreground">Carregando informações do usuário...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-primaryBackground p-4">
      <div className="mx-auto max-w-4xl">
        <Card className="shadow-softToken">
          <CardHeader className="text-center pb-4">
            <CardTitle className="text-2xl md:text-3xl font-bold text-primaryText">
              Como está se sentindo hoje?
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {/* Grid de sentimentos */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
              {sentimentos.map((sentimento) => {
                const isSelected = sentimentoSelecionado === sentimento.id;
                return (
                  <button
                    key={sentimento.id}
                    onClick={() => handleSentimentoClick(sentimento.id)}
                    type="button"
                    disabled={isSubmitting}
                    className={`
                      flex flex-col items-center justify-center gap-2 p-4 rounded-lgToken border-2 transition-all duration-300
                      transform hover:scale-105 active:scale-95
                      ${isSelected 
                        ? 'border-primary bg-primarySoft shadow-softToken scale-105 ring-2 ring-primary ring-offset-2' 
                        : 'border-borderDefault bg-surfaceElevated hover:border-primary/50 hover:bg-primarySoft/30 hover:shadow-softToken'
                      }
                      ${isSubmitting ? 'opacity-50 cursor-not-allowed' : ''}
                    `}
                  >
                    <span className="text-4xl transition-transform duration-300">
                      {sentimento.emoji}
                    </span>
                    <span className={`text-xs font-semibold ${isSelected ? 'text-primary' : 'text-primaryText'}`}>
                      {sentimento.label}
                    </span>
                  </button>
                );
              })}
            </div>

            {/* Exibir emoji do sentimento selecionado */}
            {sentimentoSelecionado && !isOutro && (
              <div className="flex items-center justify-center gap-2 py-2 transition-all duration-300">
                <span className="text-4xl">{sentimentoAtual?.emoji}</span>
                <div className="text-center">
                  <p className="text-base font-semibold text-primaryText">
                    Você selecionou: {sentimentoAtual?.label}
                  </p>
                </div>
              </div>
            )}

            {/* Campo para título quando "Outro" está selecionado */}
            {isOutro && (
              <div className="space-y-2 transition-all duration-300">
                <Label htmlFor="titulo-outro" className="text-primaryText font-semibold text-sm">
                  Informe o título do sentimento
                </Label>
                <Input
                  id="titulo-outro"
                  placeholder="Ex: Ansioso, Feliz, Nervoso, Calmo..."
                  value={tituloOutro}
                  onChange={(e) => setTituloOutro(e.target.value)}
                  className="w-full text-sm"
                  autoFocus
                  disabled={isSubmitting}
                />
              </div>
            )}

            {/* Campo de descrição opcional */}
            {sentimentoSelecionado && (
              <div className="space-y-2 transition-all duration-300">
                <Label htmlFor="descricao" className="text-primaryText font-semibold text-sm">
                  Descreva um pouco do que você está sentindo
                  <span className="text-secondaryText text-xs font-normal ml-2">(opcional)</span>
                </Label>
                <Textarea
                  id="descricao"
                  placeholder="Compartilhe mais sobre como você está se sentindo..."
                  value={descricao}
                  onChange={(e) => setDescricao(e.target.value)}
                  rows={3}
                  className="w-full resize-none text-sm"
                  disabled={isSubmitting}
                />
              </div>
            )}

            {/* Botão de enviar */}
            {podeEnviar && (
              <div className="pt-2 transition-all duration-300">
                <Button
                  onClick={handleSubmit}
                  className="w-full"
                  size="lg"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? (
                    <>
                      <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                      Enviando...
                    </>
                  ) : (
                    'Enviar'
                  )}
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

