export type SectionType =
  | 'banner'
  | 'feed'
  | 'aniversariantes'
  | 'shortcuts'
  | 'action-card'
  | 'welcome'
  | 'canal-denuncias'
  | 'video'
  | 'text-block'
  | 'profissionais-mosaico'
  | 'beneficios'
  | 'comunidades-mosaico'

export interface BackgroundConfig {
  type: 'none' | 'solid' | 'gradient' | 'image'
  color?: string
  gradient?: {
    from: string
    to: string
    direction: 'to-r' | 'to-br' | 'to-b' | 'to-bl' | 'to-tr'
  }
  image?: string
}

export interface LinkConfig {
  type: 'internal' | 'external' | 'none'
  href: string
  target: '_self' | '_blank'
}

export interface ButtonConfig {
  label: string
  link: LinkConfig
  variant: 'primary' | 'secondary' | 'ghost'
}

export interface ShortcutItem {
  id: string
  iconName: string
  label: string
  link: LinkConfig
}

export interface BannerSlide {
  id: string
  background: BackgroundConfig
  title?: string
  subtitle?: string
  textColor: 'light' | 'dark'
  textAlign: 'left' | 'center' | 'right'
  textVerticalAlign: 'top' | 'center' | 'bottom'
  button?: ButtonConfig
}

export interface BannerContent {
  type: 'banner'
  slides: BannerSlide[]
  height: 'sm' | 'md' | 'lg'
  autoPlay: boolean
  interval: number
  showDots: boolean
  showArrows: boolean
}

export interface FeedContent {
  type: 'feed'
  title?: string
  showFilters: boolean
  compactMode: boolean
}

export interface AniversariantesContent {
  type: 'aniversariantes'
  title: string
  height: 'sm' | 'md' | 'lg'
}

export interface ShortcutsContent {
  type: 'shortcuts'
  title?: string
  items: ShortcutItem[]
  columns: 2 | 3 | 4
}

export interface ActionCardContent {
  type: 'action-card'
  title: string
  description?: string
  button: ButtonConfig
  textColor: 'light' | 'dark'
  minHeight: 'sm' | 'md' | 'lg'
}

export interface WelcomeContent {
  type: 'welcome'
  greeting: string
  subtitle: string
  showUserName: boolean
}

export interface CanalDenunciasContent {
  type: 'canal-denuncias'
}

export interface VideoContent {
  type: 'video'
  title?: string
  url: string
  aspectRatio: '16/9' | '4/3'
  autoplay: boolean
  controls: boolean
}

export interface TextBlockContent {
  type: 'text-block'
  title?: string
  body: string
  alignment: 'left' | 'center' | 'right'
  textColor: 'light' | 'dark'
  minHeight: 'sm' | 'md' | 'lg'
}

export interface ProfissionaisMosaicoContent {
  type: 'profissionais-mosaico'
  title?: string
  /** Altura fixa do container com scroll interno. */
  height: 'sm' | 'md' | 'lg'
}

export interface BeneficiosContent {
  type: 'beneficios'
  title?: string
  /** Altura fixa do container com scroll interno. */
  height: 'sm' | 'md' | 'lg'
}

export interface ComunidadesMosaicoContent {
  type: 'comunidades-mosaico'
  title?: string
  /** Altura fixa do container com scroll interno. */
  height: 'sm' | 'md' | 'lg'
  /** Número de colunas no grid de cards. */
  columns: 2 | 3
  /** Exibe apenas comunidades das quais o usuário é membro. */
  onlyMine: boolean
}

export type SectionContent =
  | BannerContent
  | FeedContent
  | AniversariantesContent
  | ShortcutsContent
  | ActionCardContent
  | WelcomeContent
  | CanalDenunciasContent
  | VideoContent
  | TextBlockContent
  | ProfissionaisMosaicoContent
  | BeneficiosContent
  | ComunidadesMosaicoContent

export interface HomeSection {
  id: string
  visible: boolean
  /**
   * Largura da seção no layout de colunas:
   * - full   → 100% (sem colunas)
   * - wide   → ~70% (ocupa a coluna principal; os blocos "narrow/half" do mesmo grupo ficam nos ~30% restantes)
   * - half   → 50% (distribuição balanceada entre as duas colunas)
   * - narrow → ~30% (coluna lateral; complementa um bloco "wide" de 70%)
   */
  columnSpan: 'full' | 'wide' | 'half' | 'narrow'
  background: BackgroundConfig
  content: SectionContent
}

export interface HomeConfig {
  version: '1.0'
  sections: HomeSection[]
}

export const SECTION_TYPE_LABELS: Record<SectionType, string> = {
  banner: 'Banner',
  feed: 'Feed',
  aniversariantes: 'Aniversariantes',
  shortcuts: 'Atalhos Rápidos',
  'action-card': 'Card de Ação',
  welcome: 'Boas-vindas',
  'canal-denuncias': 'Canal de Denúncias',
  video: 'Vídeo',
  'text-block': 'Bloco de Texto',
  'profissionais-mosaico': 'Mosaico de Profissionais',
  beneficios: 'Benefícios',
  'comunidades-mosaico': 'Comunidades',
}

export const SECTION_TYPE_DESCRIPTIONS: Record<SectionType, string> = {
  banner: 'Imagem de destaque em largura total',
  feed: 'Feed de posts e comunicados',
  aniversariantes: 'Lista de aniversariantes da semana',
  shortcuts: 'Grid de atalhos de navegação rápida',
  'action-card': 'Card com título, texto e botão de ação',
  welcome: 'Mensagem de boas-vindas personalizada',
  'canal-denuncias': 'Formulário anônimo de sugestões e denúncias',
  video: 'Vídeo incorporado (YouTube, Vimeo)',
  'text-block': 'Bloco de texto livre com título',
  'profissionais-mosaico': 'Mosaico de profissionais com favoritos e busca',
  beneficios: 'Lista de benefícios e parcerias em mosaico',
  'comunidades-mosaico': 'Mosaico de comunidades com busca e destaque',
}
