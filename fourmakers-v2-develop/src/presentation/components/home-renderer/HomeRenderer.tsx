import type { HomeConfig, HomeSection, BackgroundConfig } from '@shared/types/homeBuilder'
import { BannerBlock } from './blocks/BannerBlock'
import { FeedBlock } from './blocks/FeedBlock'
import { AniversariantesBlock } from './blocks/AniversariantesBlock'
import { ShortcutsBlock } from './blocks/ShortcutsBlock'
import { ActionCardBlock } from './blocks/ActionCardBlock'
import { WelcomeBlock } from './blocks/WelcomeBlock'
import { VideoBlock } from './blocks/VideoBlock'
import { TextBlock } from './blocks/TextBlock'
import { CanalDenunciasBlock } from './blocks/CanalDenunciasBlock'
import { ProfissionaisMosaicoBlock } from './blocks/ProfissionaisMosaicoBlock'
import { BeneficiosBlock } from './blocks/BeneficiosBlock'
import { ComunidadesMosaicoBlock } from './blocks/ComunidadesMosaicoBlock'

const GRADIENT_DIR_MAP: Record<string, string> = {
  'to-r': 'to right',
  'to-br': 'to bottom right',
  'to-b': 'to bottom',
  'to-bl': 'to bottom left',
  'to-tr': 'to top right',
}

function buildBackgroundStyle(bg: BackgroundConfig): React.CSSProperties {
  if (bg.type === 'solid') {
    return { backgroundColor: bg.color ?? '#000000' }
  }
  if (bg.type === 'gradient') {
    const from = bg.gradient?.from ?? '#9A1BFF'
    const to = bg.gradient?.to ?? '#4F46E5'
    const dir = GRADIENT_DIR_MAP[bg.gradient?.direction ?? 'to-br'] ?? 'to bottom right'
    return { backgroundImage: `linear-gradient(${dir}, ${from}, ${to})` }
  }
  if (bg.type === 'image' && bg.image) {
    return {
      backgroundImage: `url(${bg.image})`,
      backgroundSize: 'cover',
      backgroundPosition: 'center',
    }
  }
  return {}
}

function hasBackground(bg: BackgroundConfig): boolean {
  return bg.type !== 'none'
}

function SectionWrapper({
  section,
  children,
  compactPadding,
}: {
  section: HomeSection
  children: React.ReactNode
  compactPadding?: boolean
}) {
  const bgStyle = buildBackgroundStyle(section.background)
  const withBg = hasBackground(section.background)
  const paddingClass = !withBg ? '' : compactPadding ? 'p-3 sm:p-4' : 'p-4 sm:p-6 lg:p-8'

  return (
    <div
      className={`w-full min-w-0 flex flex-col rounded-lgToken ${paddingClass}`}
      style={bgStyle}
    >
      {children}
    </div>
  )
}

export type PreviewDevice = 'mobile' | 'tablet' | 'desktop'

function renderBlock(section: HomeSection, previewDevice?: PreviewDevice, productionMode?: boolean): React.ReactNode {
  const { content } = section
  switch (content.type) {
    case 'banner':
      return <BannerBlock content={content} />
    case 'feed':
      return <FeedBlock content={content} />
    case 'aniversariantes':
      return <AniversariantesBlock content={content} />
    case 'shortcuts':
      return <ShortcutsBlock content={content} />
    case 'action-card':
      return <ActionCardBlock content={content} background={section.background} />
    case 'welcome':
      return <WelcomeBlock content={content} />
    case 'canal-denuncias':
      return (
        <CanalDenunciasBlock
          previewDevice={previewDevice}
          showParameterHints={previewDevice !== undefined}
          productionMode={productionMode}
        />
      )
    case 'video':
      return <VideoBlock content={content} />
    case 'text-block':
      return <TextBlock content={content} />
    case 'profissionais-mosaico':
      return <ProfissionaisMosaicoBlock content={content} />
    case 'beneficios':
      return <BeneficiosBlock content={content} />
    case 'comunidades-mosaico':
      return <ComunidadesMosaicoBlock content={content} />
    default:
      return null
  }
}

/** Peso estimado de altura por tipo. Feed muito alto para que a outra coluna receba os blocos seguintes (ex.: cards abaixo de aniversariantes). */
function getSectionHeightWeight(section: HomeSection): number {
  switch (section.content.type) {
    case 'feed':
      return 10
    case 'aniversariantes':
      return 2
    case 'video':
    case 'text-block':
      return 2
    case 'canal-denuncias':
      return 3
    case 'profissionais-mosaico':
      return 4
    case 'beneficios':
      return 4
    case 'comunidades-mosaico':
      return 4
    default:
      return 1
  }
}

interface HomeRendererProps {
  config: HomeConfig
  /** Quando definido (preview do builder), força layout 1 coluna em mobile/tablet mesmo com viewport grande */
  previewDevice?: PreviewDevice
  productionMode?: boolean
}

/**
 * Distribui seções "half" em duas colunas pelo peso estimado de altura:
 * a coluna com menor soma de pesos recebe o próximo bloco. Assim o feed
 * (peso alto) fica numa coluna e os próximos blocos (ex.: cards de ação)
 * preenchem a outra coluna abaixo de atalhos/aniversariantes.
 */
export function HomeRenderer({ config, previewDevice, productionMode }: HomeRendererProps) {
  const visibleSections = config.sections.filter((s) => s.visible)
  const nodes: React.ReactNode[] = []
  let col1: HomeSection[] = []
  let col2: HomeSection[] = []
  let weight1 = 0
  let weight2 = 0
  let columnBlockIndex = 0

  const isSingleColumn = previewDevice === 'mobile' || previewDevice === 'tablet'

  const flushColumns = () => {
    if (col1.length === 0 && col2.length === 0) return

    const col1HasWide = col1.some((s) => s.columnSpan === 'wide')
    const col2HasNarrow = col2.some((s) => s.columnSpan === 'narrow')
    const col1HasNarrow = col1.some((s) => s.columnSpan === 'narrow')
    const gridColsClass = isSingleColumn
      ? 'grid-cols-1'
      : col1HasWide || col2HasNarrow
        ? 'grid-cols-1 lg:grid-cols-[7fr_3fr]'
        : col1HasNarrow
          ? 'grid-cols-1 lg:grid-cols-[3fr_7fr]'
          : 'grid-cols-1 lg:grid-cols-2'

    const key = `columns-${columnBlockIndex}-${col1.map((s) => s.id).join('-')}-${col2.map((s) => s.id).join('-')}`
    nodes.push(
      <div key={key} className={`grid ${gridColsClass} gap-4 sm:gap-6 items-start min-w-0`}>
        <div className="flex flex-col gap-4 sm:gap-6 min-w-0">
          {col1.map((section) => (
            <SectionWrapper key={section.id} section={section} compactPadding={isSingleColumn}>
              {renderBlock(section, previewDevice, productionMode)}
            </SectionWrapper>
          ))}
        </div>
        <div className="flex flex-col gap-4 sm:gap-6 min-w-0">
          {col2.map((section) => (
            <SectionWrapper key={section.id} section={section} compactPadding={isSingleColumn}>
              {renderBlock(section, previewDevice, productionMode)}
            </SectionWrapper>
          ))}
        </div>
      </div>,
    )
    columnBlockIndex += 1
    col1 = []
    col2 = []
    weight1 = 0
    weight2 = 0
  }

  for (const section of visibleSections) {
    if (section.columnSpan === 'full') {
      flushColumns()
      nodes.push(
        <SectionWrapper key={section.id} section={section} compactPadding={isSingleColumn}>
          {renderBlock(section, previewDevice, productionMode)}
        </SectionWrapper>,
      )
    } else if (section.columnSpan === 'wide') {
      // Seção wide ancora em col1. Se col1 já tem um wide ou col2 já tem itens, flush primeiro.
      const col1HasWide = col1.some((s) => s.columnSpan === 'wide')
      if (col1HasWide || col2.length > 0) {
        flushColumns()
      }
      col1.push(section)
      weight1 += getSectionHeightWeight(section)
    } else if (section.columnSpan === 'narrow') {
      // Seção narrow (30%) ancora em col2, complementando um wide (70%) em col1.
      // Se col1 não tiver wide ainda, flush antes de criar par narrow+wide.
      const col1HasWide = col1.some((s) => s.columnSpan === 'wide')
      if (!col1HasWide && (col1.length > 0 || col2.length > 0)) {
        flushColumns()
      }
      col2.push(section)
      weight2 += getSectionHeightWeight(section)
    } else {
      const w = getSectionHeightWeight(section)
      const goToCol1 = weight1 < weight2 || (weight1 === weight2 && col1.length <= col2.length)
      if (goToCol1) {
        col1.push(section)
        weight1 += w
      } else {
        col2.push(section)
        weight2 += w
      }
    }
  }
  flushColumns()

  return (
    <div className="flex flex-col gap-4 sm:gap-6 min-w-0 overflow-x-hidden">
      {nodes}
    </div>
  )
}
