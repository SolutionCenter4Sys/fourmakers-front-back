import type { TextBlockContent, BackgroundConfig } from '@shared/types/homeBuilder'
import { cn } from '@/lib/utils'

const ALIGNMENT_CLASS: Record<string, string> = {
  left: 'text-left',
  center: 'text-center',
  right: 'text-right',
}

const MIN_HEIGHT_MAP: Record<string, string> = {
  sm: 'min-h-[80px]',
  md: 'min-h-[140px]',
  lg: 'min-h-[220px]',
}

interface TextBlockProps {
  content: TextBlockContent
  background?: BackgroundConfig
}

export function TextBlock({ content }: TextBlockProps) {
  const isLight = content.textColor === 'light'

  return (
    <div
      className={cn(
        'w-full min-w-0 flex-1 flex flex-col justify-center',
        ALIGNMENT_CLASS[content.alignment] ?? 'text-left',
        MIN_HEIGHT_MAP[content.minHeight] ?? MIN_HEIGHT_MAP.sm
      )}
    >
      {content.title && (
        <h2
          className={cn(
            'text-lg sm:text-xl font-bold mb-2 sm:mb-3 break-words',
            isLight ? 'text-white' : 'text-primaryText'
          )}
        >
          {content.title}
        </h2>
      )}
      {content.body && (
        <div
          className={cn(
            'prose prose-sm max-w-none break-words',
            isLight ? 'prose-invert text-white/85' : 'text-secondaryText'
          )}
          dangerouslySetInnerHTML={{ __html: content.body }}
        />
      )}
    </div>
  )
}
