import type { SectionType } from '@shared/types/homeBuilder'
import { SECTION_TYPE_LABELS, SECTION_TYPE_DESCRIPTIONS } from '@shared/types/homeBuilder'
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
  Plus,
  Users,
  Award,
  Globe,
} from '@/components/ui/system-icons'
import type { LucideIcon } from '@/components/ui/system-icons'

const PALETTE_GROUPS: { label: string; items: SectionType[] }[] = [
  {
    label: 'Mídia',
    items: ['banner', 'video'],
  },
  {
    label: 'Conteúdo',
    items: ['feed', 'aniversariantes', 'shortcuts', 'text-block', 'profissionais-mosaico', 'beneficios', 'comunidades-mosaico'],
  },
  {
    label: 'Interação',
    items: ['action-card', 'welcome', 'canal-denuncias'],
  },
]

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
  banner: 'bg-blue-100 text-blue-700',
  feed: 'bg-purple-100 text-purple-700',
  aniversariantes: 'bg-pink-100 text-pink-700',
  shortcuts: 'bg-orange-100 text-orange-700',
  'action-card': 'bg-yellow-100 text-yellow-700',
  welcome: 'bg-green-100 text-green-700',
  'canal-denuncias': 'bg-red-100 text-red-700',
  video: 'bg-indigo-100 text-indigo-700',
  'text-block': 'bg-slate-100 text-slate-700',
  'profissionais-mosaico': 'bg-teal-100 text-teal-700',
  beneficios: 'bg-violet-100 text-violet-700',
  'comunidades-mosaico': 'bg-cyan-100 text-cyan-700',
}

interface BuilderPaletteProps {
  onAdd: (type: SectionType) => void
}

export function BuilderPalette({ onAdd }: BuilderPaletteProps) {
  return (
    <aside className="w-64 bg-secondaryBackground border-r border-borderDefault flex flex-col overflow-y-auto">
      <div className="px-4 py-4 border-b border-borderDefault">
        <h2 className="text-sm font-semibold text-primaryText">Componentes</h2>
      </div>

      <div className="flex-1 overflow-y-auto p-3 space-y-5">
        {PALETTE_GROUPS.map((group) => (
          <div key={group.label}>
            <p className="text-xs font-semibold text-secondaryText uppercase tracking-wider mb-2 px-1">
              {group.label}
            </p>
            <div className="space-y-1">
              {group.items.map((type) => {
                const Icon = BLOCK_ICONS[type]
                const colorClass = BLOCK_COLORS[type]
                return (
                  <button
                    key={type}
                    onClick={() => onAdd(type)}
                    className="w-full flex items-center gap-3 p-2.5 rounded-lgToken hover:bg-primarySoft transition-colors duration-150 text-left group"
                  >
                    <div className={`p-2 rounded-mdToken flex-shrink-0 ${colorClass}`}>
                      <Icon size={16} />
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-primaryText truncate">
                        {SECTION_TYPE_LABELS[type]}
                      </p>
                      <p className="text-xs text-secondaryText truncate">
                        {SECTION_TYPE_DESCRIPTIONS[type]}
                      </p>
                    </div>
                    <Plus
                      size={14}
                      className="text-secondaryText opacity-0 group-hover:opacity-100 transition-opacity flex-shrink-0"
                    />
                  </button>
                )
              })}
            </div>
          </div>
        ))}
      </div>
    </aside>
  )
}
