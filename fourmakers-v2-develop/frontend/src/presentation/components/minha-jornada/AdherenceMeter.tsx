import * as React from 'react'
import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'
import { Card, CardContent } from '@/components/ui/card'
import { Progress } from '@/components/ui/progress'
import { Badge } from '@/components/ui/badge'
import { TrendingUp, Target, Trophy, Info } from 'lucide-react'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip'
import { AdherenceDetailsModal } from '@presentation/components/common'

interface AdherenceMeterProps {
  adherence: MinhaJornadaAdherence | null
  isLoading?: boolean
}

export const AdherenceMeter = ({ adherence, isLoading }: AdherenceMeterProps) => {
  const [isDetailsModalOpen, setIsDetailsModalOpen] = React.useState(false)

  if (isLoading) {
    return (
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken rounded-lg">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div className="h-6 bg-muted animate-pulse rounded" />
            <div className="h-4 bg-muted animate-pulse rounded w-3/4" />
          </div>
        </CardContent>
      </Card>
    )
  }

  if (!adherence) {
    return (
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken rounded-lg">
        <CardContent className="p-6">
          <div className="text-sm text-muted-foreground">
            Aderência não disponível
          </div>
        </CardContent>
      </Card>
    )
  }

  const percentage = adherence.match || 0
  let icon = <TrendingUp className="w-5 h-5" />
  let message = 'Em desenvolvimento'
  let motivationalMessage = ''

  if (percentage >= 100) {
    icon = <Trophy className="w-5 h-5" />
    message = 'Perfil completo!'
    motivationalMessage = 'Parabéns! Você atingiu 100% de aderência ao perfil. Continue mantendo esse excelente desempenho!'
  } else if (percentage >= 80) {
    icon = <Target className="w-5 h-5" />
    message = 'Excelente aderência'
    motivationalMessage = 'Excelente trabalho! Você está muito próximo da aderência completa. Continue assim!'
  } else if (percentage >= 50) {
    icon = <TrendingUp className="w-5 h-5" />
    message = 'Boa aderência'
    motivationalMessage = 'Você está no caminho certo! Mantenha o foco no desenvolvimento das habilidades necessárias.'
  } else {
    motivationalMessage = 'Cada passo conta! Continue desenvolvendo suas habilidades para aumentar sua aderência ao perfil.'
  }

  return (
    <>
      <Card className="border-borderSoft bg-surfaceElevated shadow-softToken rounded-lg">
        <CardContent className="p-6">
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <h3 className="text-lg font-semibold text-primaryText">
                  Aderência ao Perfil
                </h3>
                <button
                  onClick={() => setIsDetailsModalOpen(true)}
                  className="cursor-pointer hover:opacity-70 transition-opacity"
                  aria-label="Ver detalhes da aderência"
                >
                  <Info className="w-4 h-4 text-muted-foreground" />
                </button>
                <TooltipProvider>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <span className="sr-only">Informações sobre aderência</span>
                    </TooltipTrigger>
                    <TooltipContent className="max-w-xs">
                      <p className="text-sm">
                        O cálculo oficial de aderência pode demorar alguns minutos para ser atualizado.
                      </p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              </div>
              <Badge variant="outline" className="text-lg font-bold">
                {percentage.toFixed(1)}%
              </Badge>
            </div>

            <div className="space-y-2">
              <div className="flex items-center gap-2">
                {icon}
                <span className="text-sm text-primaryText">{message}</span>
              </div>
              <Progress value={percentage} className="h-3 [&>div]:bg-accent" />
            </div>

            {motivationalMessage && (
              <div className="pt-2 border-t border-borderSoft">
                <p className="text-sm text-primaryText leading-relaxed">
                  {motivationalMessage}
                </p>
              </div>
            )}
          </div>
        </CardContent>
      </Card>

      <AdherenceDetailsModal
        open={isDetailsModalOpen}
        onOpenChange={setIsDetailsModalOpen}
        adherence={adherence}
      />
    </>
  )
}

