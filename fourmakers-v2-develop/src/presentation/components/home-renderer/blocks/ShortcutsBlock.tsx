import { useRef, useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import type { ShortcutsContent } from '@shared/types/homeBuilder'
import { ShortcutCard } from '@presentation/components/Dashboards/ShortcutCard'
import {
  User,
  TrendingUp,
  ArrowUpDown,
  Calendar,
  MessageCircle,
  FileText,
  Settings,
  Globe,
  Zap,
  Star,
  Heart,
  Briefcase,
  GraduationCap,
  DollarSign,
} from '@/components/ui/system-icons'
import type { LucideIcon } from '@/components/ui/system-icons'

const ICON_MAP: Record<string, LucideIcon> = {
  User,
  TrendingUp,
  ArrowUpDown,
  Calendar,
  MessageCircle,
  FileText,
  Settings,
  Globe,
  Zap,
  Star,
  Heart,
  Briefcase,
  GraduationCap,
  DollarSign,
}

// Largura mínima de cada card em pixels
const CARD_MIN_WIDTH = 120

interface ShortcutsBlockProps {
  content: ShortcutsContent
}

export function ShortcutsBlock({ content }: ShortcutsBlockProps) {
  const navigate = useNavigate()
  const scrollRef = useRef<HTMLDivElement>(null)
  const [isOverflowing, setIsOverflowing] = useState(false)
  const [atEnd, setAtEnd] = useState(false)

  useEffect(() => {
    const el = scrollRef.current
    if (!el) return

    const checkOverflow = () => {
      setIsOverflowing(el.scrollWidth > el.clientWidth)
      setAtEnd(el.scrollLeft + el.clientWidth >= el.scrollWidth - 4)
    }

    checkOverflow()
    el.addEventListener('scroll', checkOverflow)
    const ro = new ResizeObserver(checkOverflow)
    ro.observe(el)
    return () => {
      el.removeEventListener('scroll', checkOverflow)
      ro.disconnect()
    }
  }, [content.items])

  const handleShortcutClick = (href: string, type: string, target: string) => {
    if (type === 'internal') {
      navigate(href)
    } else if (type === 'external') {
      window.open(href, target)
    }
  }

  const IconComponent = (iconName: string) => {
    const Icon = ICON_MAP[iconName]
    if (!Icon) return <User className="h-6 w-6 text-primary" />
    return <Icon className="h-6 w-6 text-primary" />
  }

  return (
    <div className="w-full min-w-0">
      {content.title && (
        <h2 className="text-lg sm:text-xl font-bold text-primaryText mb-3 sm:mb-4 break-words">{content.title}</h2>
      )}

      {/* Container relativo para o fade direito */}
      <div className="relative min-w-0">
        {/* Linha de atalhos — scroll horizontal quando overflow */}
        <div
          ref={scrollRef}
          className="flex gap-3 sm:gap-4 overflow-x-auto overflow-y-hidden pb-1 scroll-smooth min-w-0"
          style={{
            scrollbarWidth: 'none',
            msOverflowStyle: 'none',
          }}
        >
          {content.items.map((item) => (
            // flex: 1 0 CARD_MIN_WIDTH → cresce para preencher espaço disponível,
            // mas nunca encolhe abaixo do mínimo (scroll quando overflow)
            // [&>*]:h-full → Card interno preenche a altura do wrapper (uniformidade)
            <div
              key={item.id}
              className="[&>*]:h-full"
              style={{ flex: `1 0 ${CARD_MIN_WIDTH}px` }}
            >
              <ShortcutCard
                icon={IconComponent(item.iconName)}
                label={item.label}
                onClick={() =>
                  handleShortcutClick(item.link.href, item.link.type, item.link.target)
                }
              />
            </div>
          ))}
        </div>

        {/* Fade direito — aparece só quando há overflow e não chegou ao fim */}
        {isOverflowing && !atEnd && (
          <div
            className="pointer-events-none absolute top-0 right-0 h-full w-16"
            style={{
              background: 'linear-gradient(to right, transparent, var(--color-secondary-background, #fff))',
            }}
          />
        )}
      </div>
    </div>
  )
}
