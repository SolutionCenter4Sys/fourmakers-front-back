import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import { Card, CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Progress } from '@/components/ui/progress'
import { cn } from '@/lib/utils'
import { logError } from '@shared/utils/firebaseCrashlytics'
import { useAppSelector } from '@app/store/hooks'

interface RadarPDICardProps {
  meta: MinhaEquipePDI
}

/**
 * Helper para converter string ISO para Date válido
 * O mapper SEMPRE converte created_at (number) e deadline (string) para ISO strings
 * Este componente SEMPRE recebe strings, mas a entidade permite Date | string
 */
const toValidDate = (value: Date | string | null | undefined): Date | null => {
  // Verificar valores nulos/undefined
  if (value === null || value === undefined) {
    return null
  }
  
  // Se já é um Date object válido, retornar
  if (value instanceof Date) {
    if (isNaN(value.getTime())) {
      return null
    }
    return value
  }
  
  // Se não é string, retornar null (não esperamos outros tipos)
  if (typeof value !== 'string' || value.trim() === '') {
    return null
  }
  
  // Parsear como ISO string (o mapper sempre retorna ISO)
  const date = new Date(value)
  if (isNaN(date.getTime())) {
    return null
  }
  return date
}

/**
 * Helper para verificar se um valor é um Date object válido e seguro para usar
 */
const isSafeDate = (value: unknown): value is Date => {
  if (!value) return false
  if (!(value instanceof Date)) return false
  if (typeof (value as Date).getTime !== 'function') return false
  try {
    const time = (value as Date).getTime()
    return isFinite(time) && !isNaN(time)
  } catch {
    return false
  }
}

/**
 * Calcula o progresso de uma meta PDI baseado em status e deadline.
 * 
 * @param meta - Meta PDI com informações de status, deadline e data de criação
 * @returns Porcentagem de progresso (0-100)
 */
const calcularProgressoPDI = (meta: MinhaEquipePDI): number => {
  try {
    // Se concluída, progresso é 100%
    if (meta.status === 'concluida') return 100

    // Se atrasada, progresso é 100% (mas será exibido com cor de alerta)
    if (meta.status === 'atrasada') return 100

    // Status em_andamento: calcular baseado em deadline
    const deadline = toValidDate(meta.deadline)
    const criacao = toValidDate(meta.created_at)
    const hoje = new Date()

    // Verificação rigorosa: garantir que são Date objects válidos e seguros
    if (!isSafeDate(deadline) || !isSafeDate(criacao) || !isSafeDate(hoje)) {
      return 0
    }

    // Calcular dias totais e dias decorridos
    // Agora sabemos que getTime() é seguro de chamar
    const deadlineTime = deadline.getTime()
    const criacaoTime = criacao.getTime()
    const hojeTime = hoje.getTime()
    
    // Verificação final de segurança
    if (!isFinite(deadlineTime) || !isFinite(criacaoTime) || !isFinite(hojeTime)) {
      return 0
    }
    
    const diasTotais = Math.ceil((deadlineTime - criacaoTime) / (1000 * 60 * 60 * 24))
    const diasDecorridos = Math.ceil((hojeTime - criacaoTime) / (1000 * 60 * 60 * 24))

    // Casos edge
    if (diasTotais <= 0) return 100
    if (diasDecorridos < 0) return 0

    // Calcular progresso (dias decorridos / dias totais * 100)
    const progresso = Math.min((diasDecorridos / diasTotais) * 100, 100)
    return Math.max(progresso, 0)
  } catch (error) {
    // Erro será logado no componente que chama esta função
    throw error
  }
}

const getStatusLabel = (status: string): string => {
  switch (status) {
    case 'concluida':
      return 'Concluída'
    case 'em_andamento':
      return 'Em andamento'
    case 'atrasada':
      return 'Atrasada'
    default:
      return status
  }
}

const getStatusColor = (status: string): string => {
  switch (status) {
    case 'concluida':
      return 'bg-success text-success-foreground'
    case 'em_andamento':
      return 'bg-warning text-warning-foreground'
    case 'atrasada':
      return 'bg-destructive text-destructive-foreground'
    default:
      return 'bg-muted text-muted-foreground'
  }
}

/**
 * Card de visualização de meta PDI no Radar de Habilidades.
 * Exibe informações sobre a meta, progresso calculado e status.
 * 
 * @param meta - Dados da meta PDI a ser exibida
 */
export const RadarPDICard = ({ meta }: RadarPDICardProps) => {
  const user = useAppSelector((state) => state.auth.user)
  
  // Calcular valores com proteção contra erros
  let progresso = 0
  let deadline: Date | null = null
  let isOverdue = false
  
  try {
    progresso = calcularProgressoPDI(meta)
    deadline = toValidDate(meta.deadline)
    const hoje = new Date()
    
    // Se não temos deadline válido, não considerar atrasado
    // Verificação extra: usar isSafeDate para garantir que deadline é válido
    isOverdue =
      meta.status === 'atrasada' ||
      (isSafeDate(deadline) &&
        isSafeDate(hoje) &&
        deadline < hoje &&
        meta.status !== 'concluida')
  } catch (error) {
    // Em caso de erro, registrar no Firebase Crashlytics e usar valores seguros padrão
    if (error instanceof Error) {
      logError(error, {
        component: 'RadarPDICard',
        action: 'processarMetaPDI',
        metaId: meta.id,
        skillName: meta.skillName,
        status: meta.status,
      }, user)
    }
    progresso = 0
    deadline = null
    isOverdue = false
  }

  return (
    <Card
      className={cn(
        'border-borderSoft bg-surfaceElevated shadow-softToken rounded-lgToken transition-all',
        isOverdue && 'border-destructive/50 bg-destructive/5',
      )}
    >
      <CardContent className="p-4 space-y-4">
        {/* Header: Skill name and status */}
        <div className="flex items-start justify-between gap-2">
          <h4
            className="font-bold uppercase truncate flex-1 min-w-0 text-primaryText"
            title={meta.skillName}
          >
            {meta.skillName}
          </h4>
          <Badge
            variant="outline"
            className={cn('text-xs flex-shrink-0', getStatusColor(meta.status))}
          >
            {getStatusLabel(meta.status)}
          </Badge>
        </div>

        {/* Separator */}
        <div className="border-t border-borderDefault" />

        {/* Progress bar */}
        <div className="space-y-2">
          <div className="flex items-center justify-between text-sm">
            <span className="text-muted-foreground">Progresso</span>
            <span className="font-medium text-primaryText">
              {Math.round(progresso)}%
            </span>
          </div>
          <Progress
            value={progresso}
            className={cn(
              'w-full h-3',
              isOverdue && 'bg-destructive/20',
              meta.status === 'concluida' && 'bg-success/20',
            )}
          />
        </div>

        {/* Deadline */}
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">Prazo:</span>
          <span
            className={cn(
              'font-medium',
              isOverdue
                ? 'text-destructive'
                : meta.status === 'concluida'
                  ? 'text-success'
                  : 'text-primaryText',
            )}
          >
            {isSafeDate(deadline)
              ? deadline.toLocaleDateString('pt-BR')
              : 'Não definido'}
          </span>
        </div>

        {/* Actions */}
        {meta.actions && (
          <div className="space-y-1">
            <span className="text-sm font-medium text-primaryText">Ações:</span>
            <p className="text-sm text-muted-foreground">{meta.actions}</p>
          </div>
        )}
      </CardContent>
    </Card>
  )
}
