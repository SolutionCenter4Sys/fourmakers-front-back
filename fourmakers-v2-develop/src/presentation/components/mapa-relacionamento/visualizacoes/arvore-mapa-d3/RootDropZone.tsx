interface RootDropZoneProps {
  isDragging: boolean
  isHoveringRootZone: boolean
}

export function RootDropZone({ isDragging, isHoveringRootZone }: RootDropZoneProps) {
  if (!isDragging) return null

  return (
    <div 
      className={`absolute top-0 left-0 right-0 h-[100px] flex items-center justify-center transition-all duration-200 pointer-events-none z-[200] ${
        isHoveringRootZone 
          ? 'bg-primary/20 border-2 border-primary border-dashed' 
          : 'bg-primary/5 border-2 border-primary/30 border-dashed'
      }`}
    >
      {/* Message is rendered by InstructionMessage to avoid overlap */}
    </div>
  )
}
