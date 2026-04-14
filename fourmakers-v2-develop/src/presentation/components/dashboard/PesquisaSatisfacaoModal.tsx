import { useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { InserirAvaliacaoSatisfacaoUseCase } from '@domain/usecases/InserirAvaliacaoSatisfacaoUseCase'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { useToast } from '@/hooks/use-toast'

// Componente de estrela simples
const StarIcon = ({ filled, className }: { filled: boolean; className?: string }) => (
  <svg
    className={className}
    fill={filled ? 'currentColor' : 'none'}
    stroke="currentColor"
    viewBox="0 0 24 24"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={2}
      d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z"
    />
  </svg>
)

interface PesquisaSatisfacaoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  trigger?: React.ReactNode
}

interface RatingOption {
  value: number
  label: string
}

const servicoRatingOptions: RatingOption[] = [
  { value: 1, label: 'Muito Insatisfeito(a)' },
  { value: 2, label: 'Insatisfeito(a)' },
  { value: 3, label: 'Neutro' },
  { value: 4, label: 'Satisfeito(a)' },
  { value: 5, label: 'Muito Satisfeito(a)' },
]

const recomendacaoRatingOptions: RatingOption[] = [
  { value: 1, label: 'Muito Improvável' },
  { value: 2, label: 'Improvável' },
  { value: 3, label: 'Neutro' },
  { value: 4, label: 'Provável' },
  { value: 5, label: 'Muito Provável' },
]

export const PesquisaSatisfacaoModal = ({ open, onOpenChange }: PesquisaSatisfacaoModalProps) => {
  const { token } = useAppSelector((state) => state.auth)
  const { toast } = useToast()
  const [servicoRate, setServicoRate] = useState<number>(0)
  const [recomendacaoRate, setRecomendacaoRate] = useState<number>(0)
  const [experienciaDescricao, setExperienciaDescricao] = useState<string>('')
  const [aspectoDescricao, setAspectoDescricao] = useState<string>('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async () => {
    if (servicoRate === 0 || recomendacaoRate === 0) {
      toast({
        title: 'Campos obrigatórios',
        description: 'Por favor, responda todas as perguntas obrigatórias.',
        variant: 'destructive',
      })
      return
    }

    if (!token) {
      toast({
        title: 'Erro de autenticação',
        description: 'Por favor, faça login novamente.',
        variant: 'destructive',
      })
      return
    }

    setIsSubmitting(true)
    try {
      const useCase = container.resolve(InserirAvaliacaoSatisfacaoUseCase)
      await useCase.execute(token, {
        servicoRate,
        recomendacaoRate,
        experienciaDescricao: experienciaDescricao || '',
        aspectoDescricao: aspectoDescricao || '',
      })

      toast({
        title: 'Avaliação enviada',
        description: 'Obrigado por sua avaliação! Sua opinião é muito importante para nós.',
        variant: 'success',
      })

      // Reset form
      setServicoRate(0)
      setRecomendacaoRate(0)
      setExperienciaDescricao('')
      setAspectoDescricao('')
      onOpenChange(false)
    } catch (error) {
      toast({
        title: 'Erro ao enviar',
        description: error instanceof Error ? error.message : 'Não foi possível enviar sua avaliação. Tente novamente.',
        variant: 'destructive',
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleCancel = () => {
    setServicoRate(0)
    setRecomendacaoRate(0)
    setExperienciaDescricao('')
    setAspectoDescricao('')
    onOpenChange(false)
  }

  const renderRating = (
    value: number,
    onChange: (value: number) => void,
    options: RatingOption[],
  ) => {
    return (
      <div className="flex items-start justify-between w-full">
        {[1, 2, 3, 4, 5].map((star) => (
          <div key={star} className="flex flex-col items-center gap-1 flex-1">
            <button
              type="button"
              onClick={() => onChange(star)}
              className="focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 rounded-smToken transition-colors p-1"
              aria-label={`Avaliar ${star} estrelas`}
            >
              <StarIcon
                filled={star <= value}
                className={`h-8 w-8 ${
                  star <= value
                    ? 'text-primary'
                    : 'text-borderDefault'
                } transition-colors`}
              />
            </button>
            <span className="text-xs text-secondaryText text-center max-w-[100px] leading-tight">
              {options[star - 1]?.label}
            </span>
          </div>
        ))}
      </div>
    )
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-2 bg-primary rounded-lgToken">
              <StarIcon filled={true} className="h-6 w-6 text-inverseText" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold text-primaryText">
                Avalie
              </DialogTitle>
              <DialogDescription className="mt-1 text-secondaryText">
                Agradecemos por dedicar um momento para compartilhar sua opinião. Sua resposta nos ajudará a melhorar nossos serviços.
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Pergunta 1: Satisfação com o serviço */}
          <div className="space-y-3">
            <Label className="text-base font-semibold text-primaryText">
              1. Quão satisfeito(a) você está com nosso serviço?{' '}
              <span className="text-error">*</span>
            </Label>
            {renderRating(servicoRate, setServicoRate, servicoRatingOptions)}
          </div>

          {/* Pergunta 2: Recomendação */}
          <div className="space-y-3">
            <Label className="text-base font-semibold text-primaryText">
              2. O quão provável é que você recomende nosso serviço para um amigo ou colega?{' '}
              <span className="text-error">*</span>
            </Label>
            {renderRating(recomendacaoRate, setRecomendacaoRate, recomendacaoRatingOptions)}
          </div>

          {/* Pergunta 3: Melhorias */}
          <div className="space-y-2">
            <Label htmlFor="experiencia" className="text-base font-semibold text-primaryText">
              3. O que podemos melhorar para tornar sua experiência melhor?
            </Label>
            <Input
              id="experiencia"
              type="text"
              value={experienciaDescricao}
              onChange={(e) => setExperienciaDescricao(e.target.value)}
              placeholder="(opcional)"
              className="rounded-lgToken"
            />
          </div>

          {/* Pergunta 4: Aspectos excepcionais */}
          <div className="space-y-2">
            <Label htmlFor="aspecto" className="text-base font-semibold text-primaryText">
              4. Houve algum aspecto do nosso serviço que você considerou excepcional?
            </Label>
            <Input
              id="aspecto"
              type="text"
              value={aspectoDescricao}
              onChange={(e) => setAspectoDescricao(e.target.value)}
              placeholder="(opcional)"
              className="rounded-lgToken"
            />
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={handleCancel}
            disabled={isSubmitting}
            className="rounded-pillToken"
          >
            Cancelar
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={isSubmitting || servicoRate === 0 || recomendacaoRate === 0}
            className="rounded-pillToken"
          >
            {isSubmitting ? 'Enviando...' : 'Avaliar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

