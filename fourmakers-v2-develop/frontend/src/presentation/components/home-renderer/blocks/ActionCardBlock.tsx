import { useNavigate } from 'react-router-dom'
import type { ActionCardContent, BackgroundConfig } from '@shared/types/homeBuilder'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

const MIN_HEIGHT_MAP: Record<string, string> = {
  sm: 'min-h-[140px]',
  md: 'min-h-[200px]',
  lg: 'min-h-[280px]',
}

interface ActionCardBlockProps {
  content: ActionCardContent
  background?: BackgroundConfig
}

export function ActionCardBlock({ content, background }: ActionCardBlockProps) {
  const navigate = useNavigate()

  const handleButtonClick = () => {
    const { link } = content.button
    if (link.type === 'internal') {
      navigate(link.href)
    } else if (link.type === 'external') {
      window.open(link.href, link.target)
    }
  }

  const isLight = content.textColor === 'light'
  const hasImageBg = background?.type === 'image'

  return (
    <div
      className={cn(
        'relative w-full min-w-0 flex-1 flex flex-col justify-between gap-3 sm:gap-4',
        MIN_HEIGHT_MAP[content.minHeight] ?? MIN_HEIGHT_MAP.md
      )}
    >
      {/* Overlay escuro para imagens de fundo para garantir legibilidade */}
      {hasImageBg && (
        <div className="absolute inset-0 bg-black/40 rounded-lgToken pointer-events-none" />
      )}

      <div className={cn('relative z-10 flex flex-col gap-3 flex-1 justify-between min-w-0')}>
        <div className="flex flex-col gap-2 sm:gap-3 min-w-0">
          <h3
            className={cn(
              'text-lg sm:text-xl md:text-2xl font-bold leading-tight break-words',
              isLight ? 'text-white' : 'text-primaryText'
            )}
          >
            {content.title}
          </h3>
          {content.description && (
            <p
              className={cn(
                'text-sm sm:text-base leading-relaxed break-words',
                isLight ? 'text-white/85' : 'text-secondaryText'
              )}
            >
              {content.description}
            </p>
          )}
        </div>

        <Button
          variant={content.button.variant as 'primary' | 'secondary' | 'ghost'}
          className="rounded-pillToken self-start mt-2"
          onClick={handleButtonClick}
        >
          {content.button.label}
        </Button>
      </div>
    </div>
  )
}
