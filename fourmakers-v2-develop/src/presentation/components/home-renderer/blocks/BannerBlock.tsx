import { useState, useEffect, useCallback, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import type { BannerContent, BannerSlide, BackgroundConfig } from '@shared/types/homeBuilder'
import { Button } from '@/components/ui/button'
import { Camera, ChevronLeft, ChevronRight } from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'

const HEIGHT_MAP: Record<string, string> = {
  sm: 'h-28 sm:h-36 md:h-44',
  md: 'h-36 sm:h-48 md:h-56 lg:h-64',
  lg: 'h-40 sm:h-52 md:h-64 lg:h-80 xl:h-96',
}

const TEXT_ALIGN_CLASS: Record<string, string> = {
  left: 'items-start text-left',
  center: 'items-center text-center',
  right: 'items-end text-right',
}

const VERTICAL_ALIGN_CLASS: Record<string, string> = {
  top: 'justify-start pt-3 pb-10 sm:pt-6 sm:pb-4 md:pt-10 md:pb-4',
  center: 'justify-center py-4 pb-10 sm:pb-4',
  bottom: 'justify-end pb-10 sm:pb-4 md:pb-6 pt-2',
}

function buildSlideStyle(bg: BackgroundConfig): React.CSSProperties {
  if (bg.type === 'solid' && bg.color) return { backgroundColor: bg.color }
  if (bg.type === 'gradient' && bg.gradient) {
    const dirMap: Record<string, string> = {
      'to-r': 'to right',
      'to-br': 'to bottom right',
      'to-b': 'to bottom',
      'to-bl': 'to bottom left',
      'to-tr': 'to top right',
    }
    return {
      backgroundImage: `linear-gradient(${dirMap[bg.gradient.direction] ?? 'to bottom right'}, ${bg.gradient.from}, ${bg.gradient.to})`,
    }
  }
  if (bg.type === 'image' && bg.image) {
    return {
      backgroundImage: `url(${bg.image})`,
      backgroundSize: 'cover',
      backgroundPosition: 'center',
    }
  }
  return { backgroundColor: '#1E293B' }
}

interface SlideProps {
  slide: BannerSlide
  onNavigate: (slide: BannerSlide) => void
}

function SlideContent({ slide, onNavigate }: SlideProps) {
  const bgStyle = buildSlideStyle(slide.background)
  const textColorClass = slide.textColor === 'light' ? 'text-white' : 'text-gray-900'
  const hasOverlay = slide.background.type === 'image'

  const handleButtonClick = () => onNavigate(slide)

  return (
    <div className="relative w-full h-full flex-shrink-0 overflow-hidden" style={bgStyle}>
      {hasOverlay && (
        <div className="absolute inset-0 bg-black/40" />
      )}

      {/* Text & CTA — pr-12 no mobile para o badge 1/2 não sobrepor */}
      {(slide.title || slide.subtitle || slide.button) && (
        <div
          className={cn(
            'relative z-10 h-full flex flex-col px-4 pr-12 sm:pr-6 sm:px-6 md:px-10 md:pr-10 gap-1 sm:gap-2 md:gap-3 min-w-0 overflow-hidden',
            TEXT_ALIGN_CLASS[slide.textAlign] ?? 'items-start text-left',
            VERTICAL_ALIGN_CLASS[slide.textVerticalAlign] ?? 'justify-center',
            textColorClass
          )}
        >
          {slide.title && (
            <h2 className="text-sm sm:text-lg md:text-xl lg:text-2xl xl:text-3xl font-bold leading-tight line-clamp-2 break-words drop-shadow-sm w-full">
              {slide.title}
            </h2>
          )}
          {slide.subtitle && (
            <p className={cn('text-[11px] sm:text-xs md:text-sm lg:text-base max-w-full sm:max-w-lg leading-snug line-clamp-2 break-words', slide.textColor === 'light' ? 'text-white/90' : 'text-gray-700')}>
              {slide.subtitle}
            </p>
          )}
          {slide.button && slide.button.label && (
            <div className="mt-1 sm:mt-2 flex-shrink-0">
              <Button
                variant={slide.button.variant as 'primary' | 'secondary' | 'ghost'}
                size="sm"
                className="rounded-pillToken shadow-softToken h-7 sm:h-8 md:h-9 px-3 sm:px-4 text-xs sm:text-sm shrink-0"
                onClick={handleButtonClick}
              >
                {slide.button.label}
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  )
}

interface BannerBlockProps {
  content: BannerContent
}

export function BannerBlock({ content }: BannerBlockProps) {
  const navigate = useNavigate()
  const [activeIndex, setActiveIndex] = useState(0)
  const [isTransitioning, setIsTransitioning] = useState(false)
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null)

  const { slides, autoPlay, interval, showDots, showArrows } = content

  const goTo = useCallback(
    (index: number) => {
      if (isTransitioning || index === activeIndex) return
      setIsTransitioning(true)
      setActiveIndex(index)
      setTimeout(() => setIsTransitioning(false), 400)
    },
    [activeIndex, isTransitioning]
  )

  const goNext = useCallback(() => {
    goTo((activeIndex + 1) % slides.length)
  }, [activeIndex, slides.length, goTo])

  const goPrev = useCallback(() => {
    goTo((activeIndex - 1 + slides.length) % slides.length)
  }, [activeIndex, slides.length, goTo])

  useEffect(() => {
    if (!autoPlay || slides.length <= 1) return
    intervalRef.current = setInterval(goNext, interval * 1000)
    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current)
    }
  }, [autoPlay, interval, goNext, slides.length])

  const handleNavigate = (slide: BannerSlide) => {
    if (!slide.button?.link || slide.button.link.type === 'none') return
    if (slide.button.link.type === 'internal') {
      navigate(slide.button.link.href)
    } else {
      window.open(slide.button.link.href, slide.button.link.target)
    }
  }

  const resetInterval = useCallback(() => {
    if (intervalRef.current) clearInterval(intervalRef.current)
    if (autoPlay && slides.length > 1) {
      intervalRef.current = setInterval(goNext, interval * 1000)
    }
  }, [autoPlay, interval, slides.length, goNext])

  if (!slides || slides.length === 0) {
    return (
      <div
        className={cn(
          'w-full rounded-lgToken bg-muted flex items-center justify-center border-2 border-dashed border-borderDefault',
          HEIGHT_MAP[content.height]
        )}
      >
        <div className="flex flex-col items-center gap-2 text-muted-foreground">
          <Camera size={32} />
          <span className="text-sm">Nenhum slide configurado</span>
        </div>
      </div>
    )
  }

  return (
    <div
      className={cn(
        'relative w-full min-w-0 rounded-lgToken overflow-hidden group shadow-softToken',
        HEIGHT_MAP[content.height]
      )}
      onMouseEnter={() => {
        if (intervalRef.current) clearInterval(intervalRef.current)
      }}
      onMouseLeave={resetInterval}
    >
      {/* Slides track */}
      <div
        className="flex h-full transition-transform duration-500 ease-in-out"
        style={{ transform: `translateX(-${activeIndex * 100}%)` }}
      >
        {slides.map((slide) => (
          <div key={slide.id} className="w-full h-full flex-shrink-0">
            <SlideContent slide={slide} onNavigate={handleNavigate} />
          </div>
        ))}
      </div>

      {/* Arrows — sempre visíveis no mobile (sem hover); em desktop aparecem no hover */}
      {showArrows && slides.length > 1 && (
        <>
          <button
            type="button"
            onClick={(e) => { e.stopPropagation(); goPrev(); resetInterval() }}
            className="absolute left-2 sm:left-3 top-1/2 -translate-y-1/2 z-20 p-2 rounded-full bg-black/40 backdrop-blur-sm text-white active:bg-black/60 transition-all opacity-100 sm:opacity-0 sm:group-hover:opacity-100 touch-manipulation min-w-[44px] min-h-[44px] flex items-center justify-center"
            aria-label="Slide anterior"
          >
            <ChevronLeft className="w-5 h-5" />
          </button>
          <button
            type="button"
            onClick={(e) => { e.stopPropagation(); goNext(); resetInterval() }}
            className="absolute right-2 sm:right-3 top-1/2 -translate-y-1/2 z-20 p-2 rounded-full bg-black/40 backdrop-blur-sm text-white active:bg-black/60 transition-all opacity-100 sm:opacity-0 sm:group-hover:opacity-100 touch-manipulation min-w-[44px] min-h-[44px] flex items-center justify-center"
            aria-label="Próximo slide"
          >
            <ChevronRight className="w-5 h-5" />
          </button>
        </>
      )}

      {/* Dots — sempre brancos para contraste no gradiente roxo; espaço para não colar no botão */}
      {showDots && slides.length > 1 && (
        <div className="absolute bottom-3 sm:bottom-4 left-0 right-0 z-20 flex justify-center items-center">
          <div className="flex items-center gap-1.5 sm:gap-2 bg-black/20 backdrop-blur-sm rounded-full px-2 py-1.5">
            {slides.map((_, i) => (
              <button
                type="button"
                key={i}
                onClick={() => { goTo(i); resetInterval() }}
                aria-label={`Ir para slide ${i + 1}`}
                className={cn(
                  'rounded-full transition-all duration-300 touch-manipulation min-w-[24px] min-h-[24px] flex items-center justify-center',
                  i === activeIndex
                    ? 'w-5 h-1.5 sm:w-6 sm:h-2 bg-white'
                    : 'w-1.5 h-1.5 sm:w-2 sm:h-2 bg-white/60 hover:bg-white/80 active:bg-white'
                )}
              />
            ))}
          </div>
        </div>
      )}

      {/* Slide counter — afastado da borda no mobile para não cortar */}
      {slides.length > 1 && (
        <div className="absolute top-3 right-3 z-20 px-2 py-1 rounded-full bg-black/50 backdrop-blur-sm text-white text-[10px] sm:text-xs font-medium tabular-nums shadow-sm">
          {activeIndex + 1}/{slides.length}
        </div>
      )}
    </div>
  )
}
