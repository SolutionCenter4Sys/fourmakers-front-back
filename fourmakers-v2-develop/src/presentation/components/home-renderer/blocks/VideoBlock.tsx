import type { VideoContent } from '@shared/types/homeBuilder'
import { Play } from '@/components/ui/system-icons'

interface VideoBlockProps {
  content: VideoContent
}

function getEmbedUrl(url: string): string {
  if (!url) return ''

  const youtubeMatch = url.match(
    /(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)([a-zA-Z0-9_-]+)/
  )
  if (youtubeMatch) {
    return `https://www.youtube.com/embed/${youtubeMatch[1]}`
  }

  const vimeoMatch = url.match(/vimeo\.com\/(\d+)/)
  if (vimeoMatch) {
    return `https://player.vimeo.com/video/${vimeoMatch[1]}`
  }

  return url
}

export function VideoBlock({ content }: VideoBlockProps) {
  const embedUrl = getEmbedUrl(content.url)
  const paddingTop = content.aspectRatio === '16/9' ? '56.25%' : '75%'

  return (
    <div className="w-full min-w-0">
      {content.title && (
        <h2 className="text-lg sm:text-xl font-bold text-primaryText mb-3 sm:mb-4 break-words">{content.title}</h2>
      )}
      {!embedUrl ? (
        <div className="relative w-full bg-muted rounded-lgToken border-2 border-dashed border-borderDefault flex items-center justify-center" style={{ paddingTop }}>
          <div className="absolute inset-0 flex flex-col items-center justify-center gap-2 text-muted-foreground">
            <Play size={32} />
            <span className="text-sm">Nenhuma URL configurada</span>
          </div>
        </div>
      ) : (
        <div className="relative w-full rounded-lgToken overflow-hidden min-w-0" style={{ paddingTop }}>
          <iframe
            src={`${embedUrl}${content.autoplay ? '?autoplay=1' : ''}${content.controls ? '' : '&controls=0'}`}
            className="absolute inset-0 w-full h-full"
            frameBorder="0"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen
            title={content.title ?? 'Vídeo'}
          />
        </div>
      )}
    </div>
  )
}
