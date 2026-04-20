import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from '@dnd-kit/core'
import {
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
  arrayMove,
} from '@dnd-kit/sortable'
import { restrictToVerticalAxis, restrictToParentElement } from '@dnd-kit/modifiers'
import type { HomeSection } from '@shared/types/homeBuilder'
import { CanvasBlock } from './canvas/CanvasBlock'
import { Layers } from '@/components/ui/system-icons'

interface BuilderCanvasProps {
  sections: HomeSection[]
  selectedId: string | null
  onSelect: (id: string) => void
  onReorder: (sections: HomeSection[]) => void
  onToggleVisible: (id: string) => void
  onRemove: (id: string) => void
  deviceWidth: 'mobile' | 'tablet' | 'desktop'
}

const DEVICE_MAX_WIDTH: Record<string, string> = {
  mobile: 'max-w-sm',
  tablet: 'max-w-2xl',
  desktop: 'max-w-full',
}

export function BuilderCanvas({
  sections,
  selectedId,
  onSelect,
  onReorder,
  onToggleVisible,
  onRemove,
  deviceWidth,
}: BuilderCanvasProps) {
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 8 },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  )

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (over && active.id !== over.id) {
      const oldIndex = sections.findIndex((s) => s.id === active.id)
      const newIndex = sections.findIndex((s) => s.id === over.id)
      onReorder(arrayMove(sections, oldIndex, newIndex))
    }
  }

  return (
    <main className="flex-1 overflow-y-auto bg-primaryBackground">
      <div className="flex flex-col h-full">
        <div className="flex-1 p-6">
          <div className={`mx-auto ${DEVICE_MAX_WIDTH[deviceWidth]} transition-all duration-300`}>
            {sections.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-24 border-2 border-dashed border-borderDefault rounded-lgToken">
                <div className="p-4 bg-muted rounded-full mb-4">
                  <Layers size={32} className="text-secondaryText" />
                </div>
                <p className="text-primaryText font-semibold mb-1">Nenhum componente adicionado</p>
                <p className="text-sm text-secondaryText text-center max-w-xs">
                  Clique em um componente no painel esquerdo para adicionar à sua home
                </p>
              </div>
            ) : (
              <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragEnd={handleDragEnd}
                modifiers={[restrictToVerticalAxis, restrictToParentElement]}
              >
                <SortableContext
                  items={sections.map((s) => s.id)}
                  strategy={verticalListSortingStrategy}
                >
                  <div className="space-y-2">
                    {sections.map((section) => (
                      <CanvasBlock
                        key={section.id}
                        section={section}
                        isSelected={selectedId === section.id}
                        onSelect={() => onSelect(section.id)}
                        onToggleVisible={() => onToggleVisible(section.id)}
                        onRemove={() => onRemove(section.id)}
                      />
                    ))}
                  </div>
                </SortableContext>
              </DndContext>
            )}
          </div>
        </div>

        {sections.length > 0 && (
          <div className="border-t border-borderDefault px-6 py-3 bg-secondaryBackground">
            <p className="text-xs text-secondaryText text-center">
              {sections.length} componente{sections.length !== 1 ? 's' : ''} •{' '}
              {sections.filter((s) => s.visible).length} visíveis •{' '}
              Arraste para reordenar
            </p>
          </div>
        )}
      </div>
    </main>
  )
}
