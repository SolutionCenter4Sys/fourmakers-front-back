import type { MinhaEquipeHabilidade } from '@domain/entities/MinhaEquipeHabilidade'
import { Card, CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { cn } from '@/lib/utils'

interface RadarSkillCardProps {
  habilidade: MinhaEquipeHabilidade
}

const SENIORITY_LEVELS: Record<number, string> = {
  1: 'Trainee',
  2: 'Júnior',
  3: 'Pleno',
  4: 'Sênior',
  5: 'Especialista',
}

const getLevelLabel = (level: string | number): string => {
  if (typeof level === 'number') {
    return SENIORITY_LEVELS[level] || `Nível ${level}`
  }
  // Se for string, tentar converter para número
  const numLevel = parseInt(String(level), 10)
  if (!isNaN(numLevel)) {
    return SENIORITY_LEVELS[numLevel] || `Nível ${numLevel}`
  }
  return String(level)
}

/**
 * Card de visualização de habilidade no Radar de Habilidades.
 * Exibe informações sobre a habilidade, níveis atual e requerido.
 * 
 * @param habilidade - Dados da habilidade a ser exibida
 */
export const RadarSkillCard = ({ habilidade }: RadarSkillCardProps) => {
  // Verificar se a habilidade está completa (nível atual >= nível requerido)
  const isCompleted =
    habilidade.currentLevel !== null &&
    habilidade.currentLevel !== undefined &&
    habilidade.requiredLevel !== null &&
    habilidade.requiredLevel !== undefined &&
    Number(habilidade.currentLevel) >= Number(habilidade.requiredLevel)

  const currentLevelLabel = getLevelLabel(habilidade.currentLevel)
  const requiredLevelLabel = getLevelLabel(habilidade.requiredLevel)

  return (
    <Card
      className={cn(
        'border-borderSoft bg-surfaceElevated shadow-softToken rounded-lg transition-all h-full',
        isCompleted &&
          'border-success/30 bg-green-50/50 dark:bg-green-950/20',
      )}
    >
      <CardContent className="p-4 h-full flex flex-col relative">
        <div className="space-y-4 flex-1">
          {/* Header: Skill name and badge */}
          <div className="flex items-start justify-between gap-2">
            <h4
              className={cn(
                'font-bold uppercase truncate flex-1 min-w-0',
                isCompleted
                  ? 'text-green-700 dark:text-green-400'
                  : 'text-primaryText',
              )}
              title={habilidade.name}
            >
              {habilidade.name}
            </h4>
            <Badge
              variant={isCompleted ? 'default' : 'outline'}
              className={cn(
                'text-xs flex-shrink-0',
                isCompleted &&
                  'bg-green-100 text-green-800 border-green-300 dark:bg-green-900/30 dark:text-green-300 dark:border-green-700',
              )}
            >
              {currentLevelLabel}
            </Badge>
          </div>

          {/* Separator line */}
          <div
            className={cn(
              'border-t',
              isCompleted
                ? 'border-green-200 dark:border-green-800'
                : 'border-borderDefault',
            )}
          />

          {/* Content: Level information */}
          <div className="space-y-2">
            {isCompleted ? (
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-green-700 dark:text-green-400">
                  <svg
                    className="w-5 h-5"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                  <span className="font-medium">Habilidade atingida</span>
                </div>
                <p className="text-xs text-green-600 dark:text-green-500">
                  Nível atual atende ou supera o nível requerido.
                </p>
              </div>
            ) : (
              <div className="space-y-2">
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Nível Atual:</span>
                  <span className="font-medium text-primaryText">
                    {currentLevelLabel}
                  </span>
                </div>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Nível Requerido:</span>
                  <span className="font-medium text-primaryText">
                    {requiredLevelLabel}
                  </span>
                </div>
                {habilidade.pendencia && (
                  <div className="mt-2">
                    <Badge variant="outline" className="text-xs text-warning">
                      Pendente
                    </Badge>
                  </div>
                )}
                {habilidade.interesse === 0 && (
                  <div className="mt-2">
                    <Badge variant="outline" className="text-xs text-muted-foreground">
                      Sem interesse
                    </Badge>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
