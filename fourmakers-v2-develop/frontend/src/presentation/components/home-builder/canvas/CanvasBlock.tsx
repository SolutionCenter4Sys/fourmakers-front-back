import { useSortable } from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import type { HomeSection, SectionType } from '@shared/types/homeBuilder'
import { SECTION_TYPE_LABELS } from '@shared/types/homeBuilder'
import {
  Camera,
  MessageCircle,
  Calendar,
  Layers,
  Zap,
  User,
  AlertCircle,
  Play,
  FileText,
  GripVertical,
  Eye,
  EyeOff,
  Trash2,
  ChevronRight,
  Users,
  Award,
  Globe,
} from '@/components/ui/system-icons'
import type { LucideIcon } from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'

const BLOCK_ICONS: Record<SectionType, LucideIcon> = {
  banner: Camera,
  feed: MessageCircle,
  aniversariantes: Calendar,
  shortcuts: Layers,
  'action-card': Zap,
  welcome: User,
  'canal-denuncias': AlertCircle,
  video: Play,
  'text-block': FileText,
  'profissionais-mosaico': Users,
  beneficios: Award,
  'comunidades-mosaico': Globe,
}

const BLOCK_COLORS: Record<SectionType, string> = {
  banner: 'bg-blue-100 text-blue-700 border-blue-200',
  feed: 'bg-purple-100 text-purple-700 border-purple-200',
  aniversariantes: 'bg-pink-100 text-pink-700 border-pink-200',
  shortcuts: 'bg-orange-100 text-orange-700 border-orange-200',
  'action-card': 'bg-yellow-100 text-yellow-700 border-yellow-200',
  welcome: 'bg-green-100 text-green-700 border-green-200',
  'canal-denuncias': 'bg-red-100 text-red-700 border-red-200',
  video: 'bg-indigo-100 text-indigo-700 border-indigo-200',
  'text-block': 'bg-slate-100 text-slate-700 border-slate-200',
  'profissionais-mosaico': 'bg-teal-100 text-teal-700 border-teal-200',
  beneficios: 'bg-violet-100 text-violet-700 border-violet-200',
  'comunidades-mosaico': 'bg-cyan-100 text-cyan-700 border-cyan-200',
}

function getSectionSummary(section: HomeSection): string {
  const { content } = section
  switch (content.type) {
    case 'banner':
      return `${content.slides.length} slide${content.slides.length !== 1 ? 's' : ''} • ${content.autoPlay ? `Auto ${content.interval}s` : 'Manual'}`
    case 'feed':
      return content.title ?? 'Feed de publicações'
    case 'aniversariantes':
      return content.title
    case 'shortcuts':
      return `${content.items.length} atalho${content.items.length !== 1 ? 's' : ''} • ${content.columns} colunas`
    case 'action-card':
      return content.title
    case 'welcome':
      return content.greeting
    case 'canal-denuncias':
      return 'Formulário anônimo'
    case 'video':
      return content.url ? content.url : 'Sem URL'
    case 'text-block':
      return content.title ?? 'Bloco de texto'
    case 'profissionais-mosaico':
      return content.title ?? 'Mosaico de profissionais'
    case 'beneficios':
      return content.title ?? 'Benefícios'
    default:
      return ''
  }
}

interface CanvasBlockProps {
  section: HomeSection
  isSelected: boolean
  onSelect: () => void
  onToggleVisible: () => void
  onRemove: () => void
}

export function CanvasBlock({
  section,
  isSelected,
  onSelect,
  onToggleVisible,
  onRemove,
}: CanvasBlockProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: section.id,
  })

  const type = section.content.type
  const Icon = BLOCK_ICONS[type]
  const colorClass = BLOCK_COLORS[type]
  const summary = getSectionSummary(section)

  const style: React.CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  }

  return (
    <div
      ref={setNodeRef}
      style={style}
      onClick={onSelect}
      className={cn(
        'group relative flex items-center gap-3 p-3 rounded-lgToken border cursor-pointer transition-all duration-150',
        isSelected
          ? 'border-primary bg-primarySoft ring-2 ring-primary/20 shadow-softToken'
          : 'border-borderDefault bg-secondaryBackground hover:border-primary/40 hover:shadow-softToken',
        !section.visible && 'opacity-50',
        isDragging && 'shadow-cardHoverToken z-50'
      )}
    >
      {/* Drag Handle */}
      <button
        {...attributes}
        {...listeners}
        onClick={(e) => e.stopPropagation()}
        className="flex-shrink-0 text-secondaryText hover:text-primaryText cursor-grab active:cursor-grabbing p-1 rounded"
        aria-label="Arrastar para reordenar"
      >
        <GripVertical size={18} />
      </button>

      {/* Block Type Icon */}
      <div className={`p-2 rounded-mdToken flex-shrink-0 border ${colorClass}`}>
        <Icon size={16} />
      </div>

      {/* Block Info */}
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2">
          <p className="text-sm font-semibold text-primaryText">
            {SECTION_TYPE_LABELS[type]}
          </p>
          {section.columnSpan !== 'full' && (
            <span className="text-xs px-1.5 py-0.5 bg-muted rounded text-secondaryText">
              {section.columnSpan === 'wide'
                ? '70%'
                : section.columnSpan === 'narrow'
                  ? '30%'
                  : '½ largura'}
            </span>
          )}
        </div>
        <p className="text-xs text-secondaryText truncate mt-0.5">{summary}</p>
      </div>

      {/* Controls */}
      <div className="flex items-center gap-1 flex-shrink-0">
        <button
          onClick={(e) => {
            e.stopPropagation()
            onToggleVisible()
          }}
          className="p-1.5 rounded-mdToken text-secondaryText hover:text-primaryText hover:bg-muted transition-colors"
          title={section.visible ? 'Ocultar' : 'Mostrar'}
        >
          {section.visible ? <Eye size={15} /> : <EyeOff size={15} />}
        </button>
        <button
          onClick={(e) => {
            e.stopPropagation()
            onRemove()
          }}
          className="p-1.5 rounded-mdToken text-secondaryText hover:text-error hover:bg-error/10 transition-colors"
          title="Remover"
        >
          <Trash2 size={15} />
        </button>
        <ChevronRight
          size={15}
          className={cn(
            'text-secondaryText transition-colors',
            isSelected && 'text-primary'
          )}
        />
      </div>
    </div>
  )
}
