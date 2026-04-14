import type {
  HomeConfig,
  HomeSection,
  BackgroundConfig,
  BannerSlide,
  BannerContent,
} from '@shared/types/homeBuilder'

const DEFAULT_GRADIENT = {
  from: '#9A1BFF',
  to: '#4F46E5',
  direction: 'to-br' as const,
}

function normalizeBackground(bg: BackgroundConfig | undefined): BackgroundConfig {
  if (!bg || typeof bg !== 'object') {
    return { type: 'none' }
  }
  if (bg.type === 'image') {
    const image = (bg as { image?: string }).image
    if (typeof image === 'string' && image.length > 0) {
      return { type: 'image', image }
    }
    return { type: 'none' }
  }
  if (bg.type === 'gradient') {
    const g = (bg as { gradient?: typeof DEFAULT_GRADIENT }).gradient
    return {
      type: 'gradient',
      gradient: g && typeof g === 'object'
        ? {
            from: typeof g.from === 'string' ? g.from : DEFAULT_GRADIENT.from,
            to: typeof g.to === 'string' ? g.to : DEFAULT_GRADIENT.to,
            direction: ['to-r', 'to-br', 'to-b', 'to-bl', 'to-tr'].includes(g.direction) ? g.direction : DEFAULT_GRADIENT.direction,
          }
        : DEFAULT_GRADIENT,
    }
  }
  if (bg.type === 'solid') {
    return {
      type: 'solid',
      color: typeof (bg as { color?: string }).color === 'string' ? (bg as { color: string }).color : '#9A1BFF',
    }
  }
  return { type: 'none' }
}

function normalizeBannerSlide(slide: unknown, index: number): BannerSlide {
  const s = slide && typeof slide === 'object' ? (slide as Record<string, unknown>) : {}
  const id = typeof s.id === 'string' ? s.id : `slide_${Date.now()}_${index}`
  const background = normalizeBackground(s.background as BackgroundConfig | undefined)
  return {
    id,
    background,
    title: typeof s.title === 'string' ? s.title : undefined,
    subtitle: typeof s.subtitle === 'string' ? s.subtitle : undefined,
    textColor: s.textColor === 'dark' ? 'dark' : 'light',
    textAlign: ['left', 'center', 'right'].includes(s.textAlign as string) ? (s.textAlign as 'left' | 'center' | 'right') : 'left',
    textVerticalAlign: ['top', 'center', 'bottom'].includes(s.textVerticalAlign as string) ? (s.textVerticalAlign as 'top' | 'center' | 'bottom') : 'center',
    button:
      s.button && typeof s.button === 'object'
        ? {
            label: typeof (s.button as { label?: string }).label === 'string' ? (s.button as { label: string }).label : '',
            variant: ['primary', 'secondary', 'ghost'].includes((s.button as { variant?: string }).variant as string) ? (s.button as { variant: 'primary' | 'secondary' | 'ghost' }).variant : 'primary',
            link: (s.button as { link?: { type?: string; href?: string; target?: string } }).link && typeof (s.button as { link: object }).link === 'object'
              ? {
                  type: ['internal', 'external', 'none'].includes((s.button as { link: { type?: string } }).link?.type as string) ? (s.button as { link: { type: 'internal' | 'external' | 'none' } }).link.type : 'none',
                  href: typeof (s.button as { link: { href?: string } }).link?.href === 'string' ? (s.button as { link: { href: string } }).link.href : '',
                  target: (s.button as { link: { target?: string } }).link?.target === '_blank' ? '_blank' : '_self',
                }
              : { type: 'none', href: '', target: '_self' },
          }
        : undefined,
  }
}

function normalizeBannerContent(content: Record<string, unknown>): BannerContent {
  const slides = Array.isArray(content.slides)
    ? (content.slides as unknown[]).map((s, i) => normalizeBannerSlide(s, i))
    : [normalizeBannerSlide({}, 0)]
  return {
    type: 'banner',
    slides,
    height: ['sm', 'md', 'lg'].includes(content.height as string) ? (content.height as 'sm' | 'md' | 'lg') : 'lg',
    autoPlay: content.autoPlay !== false,
    interval: typeof content.interval === 'number' ? content.interval : 5,
    showDots: content.showDots !== false,
    showArrows: content.showArrows !== false,
  }
}

/**
 * Converte valorParametro (sempre string na API: criar/editar enviam string, a API devolve string)
 * em HomeConfig. Se a API vier a devolver objeto, também é tratado.
 */
export function parseHomeConfigFromParam(valorParametro: string | unknown): HomeConfig | null {
  try {
    const raw =
      typeof valorParametro === 'string' ? (JSON.parse(valorParametro) as unknown) : valorParametro
    const parsed = raw as Record<string, unknown>
    if (
      parsed &&
      typeof parsed === 'object' &&
      'version' in parsed &&
      Array.isArray(parsed.sections)
    ) {
      return normalizeHomeConfig(parsed as unknown as HomeConfig)
    }
  } catch {
    // ignore
  }
  return null
}

/**
 * Normaliza o config da home após carregar da API, garantindo que todos os
 * atributos existam (ex.: banner slides com background.image quando type é 'image'),
 * para que o preview e a homeNew reflitam os dados salvos mesmo com respostas parciais.
 */
export function normalizeHomeConfig(config: HomeConfig): HomeConfig {
  if (!config || !Array.isArray(config.sections)) {
    return config
  }
  const version = config.version === '1.0' ? '1.0' : '1.0'
  const sections: HomeSection[] = config.sections.map((section) => {
    const s = section as unknown as Record<string, unknown>
    const id = typeof s.id === 'string' ? s.id : `section_${Date.now()}_${Math.random().toString(36).slice(2, 7)}`
    const visible = s.visible !== false
    const columnSpan = (['full', 'wide', 'half', 'narrow'] as const).includes(s.columnSpan as 'full' | 'wide' | 'half' | 'narrow')
      ? (s.columnSpan as 'full' | 'wide' | 'half' | 'narrow')
      : 'half'
    const background = normalizeBackground(s.background as BackgroundConfig | undefined)

    const content = s.content
    if (!content || typeof content !== 'object') {
      return { id, visible, columnSpan, background, content: { type: 'feed', showFilters: true, compactMode: false } } as HomeSection
    }
    const c = content as Record<string, unknown>
    const type = c.type as string

    if (type === 'banner') {
      return {
        id,
        visible,
        columnSpan,
        background,
        content: normalizeBannerContent(c),
      } as HomeSection
    }

    return {
      id,
      visible,
      columnSpan,
      background,
      content: section.content,
    } as HomeSection
  })

  return { version: version as '1.0', sections }
}

export interface HomeConfigValidation {
  valid: boolean
  errors: string[]
}

/**
 * Valida a estrutura do config (seções, slides, atributos, visibilidade, links, imagens).
 * Usar após parse (load) e antes de save para garantir que geramos/gravamos/montamos o JSON correto.
 */
export function validateHomeConfig(config: unknown): HomeConfigValidation {
  const errors: string[] = []
  if (!config || typeof config !== 'object') {
    return { valid: false, errors: ['Config não é um objeto'] }
  }
  const c = config as Record<string, unknown>
  if (c.version !== '1.0') {
    errors.push('version deve ser "1.0"')
  }
  if (!Array.isArray(c.sections)) {
    errors.push('sections deve ser um array')
    return { valid: false, errors }
  }
  const sections = c.sections as Record<string, unknown>[]
  sections.forEach((section, i) => {
    const prefix = `sections[${i}]`
    if (typeof section.id !== 'string') errors.push(`${prefix}: id ausente ou inválido`)
    if (typeof section.visible !== 'boolean') errors.push(`${prefix}: visible deve ser boolean`)
    if (!['full', 'wide', 'half', 'narrow'].includes(section.columnSpan as string)) errors.push(`${prefix}: columnSpan deve ser "full", "wide", "half" ou "narrow"`)
    if (!section.background || typeof section.background !== 'object') errors.push(`${prefix}: background ausente`)
    else {
      const bg = section.background as Record<string, unknown>
      if (bg.type === 'image' && typeof bg.image !== 'string') errors.push(`${prefix}.background: type "image" exige image (string)`)
    }
    if (!section.content || typeof section.content !== 'object') {
      errors.push(`${prefix}: content ausente`)
      return
    }
    const content = section.content as Record<string, unknown>
    if (content.type === 'banner') {
      if (!Array.isArray(content.slides)) errors.push(`${prefix}.content: banner exige slides (array)`)
      else {
        (content.slides as unknown[]).forEach((slide, j) => {
          const s = slide && typeof slide === 'object' ? (slide as Record<string, unknown>) : {}
          const sp = `${prefix}.content.slides[${j}]`
          if (typeof s.id !== 'string') errors.push(`${sp}: id ausente`)
          if (!s.background || typeof s.background !== 'object') errors.push(`${sp}: background ausente`)
          else if ((s.background as Record<string, unknown>).type === 'image' && typeof (s.background as Record<string, unknown>).image !== 'string') errors.push(`${sp}.background: image ausente para type "image"`)
          if (s.button && typeof s.button === 'object') {
            const btn = s.button as Record<string, unknown>
            if (btn.link && typeof btn.link === 'object') {
              const link = btn.link as Record<string, unknown>
              if (link.type !== 'none' && typeof link.href !== 'string') errors.push(`${sp}.button.link: href ausente`)
            }
          }
        })
      }
    }
    if (content.type === 'action-card') {
      if (typeof (content as { title?: unknown }).title !== 'string') errors.push(`${prefix}.content: action-card exige title`)
      if (!content.button || typeof content.button !== 'object') errors.push(`${prefix}.content: action-card exige button`)
      else {
        const btn = content.button as Record<string, unknown>
        if (!btn.link || typeof btn.link !== 'object') errors.push(`${prefix}.content.button: link ausente`)
      }
    }
  })
  return {
    valid: errors.length === 0,
    errors,
  }
}
