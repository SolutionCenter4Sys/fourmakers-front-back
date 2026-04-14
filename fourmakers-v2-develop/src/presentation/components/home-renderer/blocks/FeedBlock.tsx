import type { FeedContent } from '@shared/types/homeBuilder'
import { CommunityFeed } from '@presentation/components/comunicacao/CommunityFeed'

interface FeedBlockProps {
  content: FeedContent
}

export function FeedBlock({ content }: FeedBlockProps) {
  return (
    <div className="w-full min-w-0">
      {content.title && (
        <h2 className="text-lg sm:text-xl font-bold text-primaryText mb-3 sm:mb-4 break-words">{content.title}</h2>
      )}
      <CommunityFeed
        persona="user"
        showFilters={content.showFilters}
        compactMode={content.compactMode}
      />
    </div>
  )
}
