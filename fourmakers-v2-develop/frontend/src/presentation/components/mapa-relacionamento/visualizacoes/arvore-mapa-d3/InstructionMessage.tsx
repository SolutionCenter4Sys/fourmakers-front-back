import { GripVertical } from 'lucide-react'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

interface InstructionMessageProps {
  isDragging: boolean
  isHoveringRootZone: boolean
  dropTarget: NoMapaRelacionamento | null
}

export function InstructionMessage({ isDragging, isHoveringRootZone, dropTarget }: InstructionMessageProps) {
  if (!isDragging) return null

  const containerClasses = isHoveringRootZone
    ? 'absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 z-[210] pointer-events-none'
    : 'absolute top-4 left-1/2 -translate-x-1/2 z-[200] pointer-events-none'

  return (
    <div className={containerClasses}>
      <div className="bg-primary text-primary-foreground px-4 py-2 rounded-lg shadow-lg flex items-center gap-2 animate-pulse">
        <GripVertical className="w-4 h-4" />
        <span className="font-medium text-sm">
          {isHoveringRootZone
            ? 'Solte aqui para mover para RAIZ'
            : dropTarget
              ? `Solte sobre "${dropTarget.employeeName || dropTarget.profileName}" para mover`
              : 'Arraste sobre outro card para mover'}
        </span>
      </div>
    </div>
  )
}
