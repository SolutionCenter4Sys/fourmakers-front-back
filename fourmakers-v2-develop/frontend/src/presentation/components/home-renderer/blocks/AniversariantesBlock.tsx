import type { AniversariantesContent } from '@shared/types/homeBuilder'
import { AniversariantesCard } from '@presentation/components/Dashboards/AniversariantesCard'

const HEIGHT_MAP: Record<string, string> = {
  sm: 'max-h-64',
  md: 'max-h-96',
  lg: 'max-h-[480px]',
}

interface AniversariantesBlockProps {
  content: AniversariantesContent
}

export function AniversariantesBlock({ content }: AniversariantesBlockProps) {
  const heightClass = HEIGHT_MAP[content.height] ?? HEIGHT_MAP.md
  return (
    <div className={`w-full min-w-0 flex flex-col min-h-0 overflow-hidden ${heightClass}`}>
      <AniversariantesCard preview />
    </div>
  )
}
