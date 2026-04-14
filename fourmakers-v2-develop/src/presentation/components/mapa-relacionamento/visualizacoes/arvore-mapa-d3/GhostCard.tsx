import { Card } from '@/components/ui/card'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { Badge } from '@/components/ui/badge'
import { Crown } from 'lucide-react'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { getInitials } from '@shared/utils/mapaRelacionamentoUtils'

const LARGURA_CARD = 280
const OFFSET_ABAIXO_CURSOR = 12

interface GhostCardProps {
  draggedNode: NoMapaRelacionamento
  mousePosition: { x: number; y: number }
}

export function GhostCard({ draggedNode, mousePosition }: GhostCardProps) {
  // Card centralizado sob o cursor e posicionado logo abaixo (não ao lado)
  const left = mousePosition.x - LARGURA_CARD / 2
  const top = mousePosition.y + OFFSET_ABAIXO_CURSOR

  return (
    <div 
      className="fixed pointer-events-none z-[300]"
      style={{ 
        left,
        top,
        transform: 'rotate(3deg)'
      }}
    >
      <Card className="w-[280px] bg-card border-2 border-primary shadow-2xl opacity-90">
        <div className="p-4 space-y-2">
          <div className="flex items-center gap-3">
            <Avatar className="w-10 h-10 border-2 border-primary/20">
              <AvatarFallback className="bg-primary/10 text-primary font-semibold">
                {getInitials(draggedNode.employeeName || draggedNode.profileName)}
              </AvatarFallback>
            </Avatar>
            <div className="flex-1 min-w-0">
              <h3 className="font-semibold text-sm truncate text-foreground">
                {draggedNode.employeeName || draggedNode.profileName}
              </h3>
              <p className="text-xs text-muted-foreground truncate">
                {draggedNode.departmentName || 'Sem departamento'}
              </p>
            </div>
          </div>
          {draggedNode.isCLevel && (
            <Badge variant="default" className="text-xs">
              <Crown className="w-3 h-3 mr-1" />
              C-Level
            </Badge>
          )}
        </div>
      </Card>
    </div>
  )
}
