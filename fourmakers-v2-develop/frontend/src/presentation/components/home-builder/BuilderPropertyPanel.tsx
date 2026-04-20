import { useRef, useState } from 'react'
import type {
  HomeSection,
  BackgroundConfig,
  SectionContent,
  LinkConfig,
  ButtonConfig,
  ShortcutItem,
  BannerSlide,
} from '@shared/types/homeBuilder'
import { SECTION_TYPE_LABELS } from '@shared/types/homeBuilder'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Switch } from '@/components/ui/switch'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Separator } from '@/components/ui/separator'
import { Textarea } from '@/components/ui/textarea'
import { Settings2, Plus, Trash2, Upload, ChevronDown, ChevronUp } from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'

function SectionTitle({ children }: { children: React.ReactNode }) {
  return (
    <p className="text-xs font-semibold text-secondaryText uppercase tracking-wider mb-3">
      {children}
    </p>
  )
}

function Field({
  label,
  children,
  hint,
}: {
  label: string
  children: React.ReactNode
  hint?: string
}) {
  return (
    <div className="space-y-1.5">
      <Label className="text-xs font-medium text-primaryText">{label}</Label>
      {children}
      {hint && <p className="text-xs text-secondaryText">{hint}</p>}
    </div>
  )
}

// ─── Background Editor ──────────────────────────────────────────────────────

function BackgroundEditor({
  value,
  onChange,
}: {
  value: BackgroundConfig
  onChange: (bg: BackgroundConfig) => void
}) {
  const fileRef = useRef<HTMLInputElement>(null)

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    const reader = new FileReader()
    reader.onload = () => {
      onChange({ type: 'image', image: reader.result as string })
    }
    reader.readAsDataURL(file)
  }

  return (
    <div className="space-y-3">
      <Field label="Tipo de fundo">
        <Select
          value={value.type}
          onValueChange={(v) => {
            const newType = v as BackgroundConfig['type']
            if (newType === 'solid') {
              onChange({ type: 'solid', color: value.color ?? '#9A1BFF' })
            } else if (newType === 'gradient') {
              onChange({
                type: 'gradient',
                gradient: value.gradient ?? { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' },
              })
            } else if (newType === 'image') {
              onChange({ type: 'image', image: value.image })
            } else {
              onChange({ type: 'none' })
            }
          }}
        >
          <SelectTrigger className="h-8 text-xs">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="none">Nenhum</SelectItem>
            <SelectItem value="solid">Cor sólida</SelectItem>
            <SelectItem value="gradient">Gradiente</SelectItem>
            <SelectItem value="image">Imagem</SelectItem>
          </SelectContent>
        </Select>
      </Field>

      {value.type === 'solid' && (
        <Field label="Cor">
          <div className="flex items-center gap-2">
            <input
              type="color"
              value={value.color ?? '#000000'}
              onChange={(e) => onChange({ ...value, color: e.target.value })}
              className="h-8 w-10 cursor-pointer rounded border border-borderDefault p-0.5"
            />
            <Input
              className="h-8 text-xs font-mono"
              value={value.color ?? '#000000'}
              onChange={(e) => onChange({ ...value, color: e.target.value })}
              placeholder="#000000"
            />
          </div>
        </Field>
      )}

      {value.type === 'gradient' && (
        <>
          <div className="grid grid-cols-2 gap-2">
            <Field label="Cor inicial">
              <input
                type="color"
                value={value.gradient?.from ?? '#9A1BFF'}
                onChange={(e) =>
                  onChange({
                    ...value,
                    gradient: { ...value.gradient!, from: e.target.value },
                  })
                }
                className="h-8 w-full cursor-pointer rounded border border-borderDefault p-0.5"
              />
            </Field>
            <Field label="Cor final">
              <input
                type="color"
                value={value.gradient?.to ?? '#4F46E5'}
                onChange={(e) =>
                  onChange({
                    ...value,
                    gradient: { ...value.gradient!, to: e.target.value },
                  })
                }
                className="h-8 w-full cursor-pointer rounded border border-borderDefault p-0.5"
              />
            </Field>
          </div>
          <Field label="Direção">
            <Select
              value={value.gradient?.direction ?? 'to-br'}
              onValueChange={(v) =>
                onChange({
                  ...value,
                  gradient: {
                    from: value.gradient?.from ?? '#9A1BFF',
                    to: value.gradient?.to ?? '#4F46E5',
                    direction: v as 'to-r' | 'to-br' | 'to-b' | 'to-bl' | 'to-tr',
                  },
                })
              }
            >
              <SelectTrigger className="h-8 text-xs">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="to-r">Esquerda → Direita</SelectItem>
                <SelectItem value="to-br">Diagonal → ↘</SelectItem>
                <SelectItem value="to-b">Cima → Baixo</SelectItem>
                <SelectItem value="to-bl">Diagonal → ↙</SelectItem>
                <SelectItem value="to-tr">Diagonal → ↗</SelectItem>
              </SelectContent>
            </Select>
          </Field>
          <div
            className="h-10 rounded-lgToken"
            style={{
              background: `linear-gradient(${
                { 'to-r': 'to right', 'to-br': 'to bottom right', 'to-b': 'to bottom', 'to-bl': 'to bottom left', 'to-tr': 'to top right' }[value.gradient?.direction ?? 'to-br']
              }, ${value.gradient?.from ?? '#9A1BFF'}, ${value.gradient?.to ?? '#4F46E5'})`,
            }}
          />
        </>
      )}

      {value.type === 'image' && (
        <div className="space-y-2">
          <input
            ref={fileRef}
            type="file"
            accept="image/*"
            className="hidden"
            onChange={handleImageUpload}
          />
          <Button
            variant="secondary"
            size="sm"
            className="w-full gap-2"
            onClick={() => fileRef.current?.click()}
          >
            <Upload size={14} />
            {value.image ? 'Trocar imagem' : 'Fazer upload'}
          </Button>
          {value.image && (
            <div className="relative">
              <img
                src={value.image}
                alt="Preview do fundo"
                className="w-full h-20 object-cover rounded-lgToken"
              />
              <button
                onClick={() => onChange({ type: 'image' })}
                className="absolute top-1 right-1 p-1 bg-black/60 rounded text-white hover:bg-black/80"
              >
                <Trash2 size={12} />
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  )
}

// ─── Link Editor ─────────────────────────────────────────────────────────────

function LinkEditor({
  value,
  onChange,
  label = 'Link',
}: {
  value: LinkConfig
  onChange: (link: LinkConfig) => void
  label?: string
}) {
  return (
    <div className="space-y-2">
      <Field label={label}>
        <Select
          value={value.type}
          onValueChange={(v) => onChange({ ...value, type: v as LinkConfig['type'] })}
        >
          <SelectTrigger className="h-8 text-xs">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="none">Sem link</SelectItem>
            <SelectItem value="internal">Rota interna</SelectItem>
            <SelectItem value="external">URL externa</SelectItem>
          </SelectContent>
        </Select>
      </Field>
      {value.type !== 'none' && (
        <>
          <Input
            className="h-8 text-xs"
            placeholder={value.type === 'internal' ? '/rota/interna' : 'https://exemplo.com'}
            value={value.href}
            onChange={(e) => onChange({ ...value, href: e.target.value })}
          />
          <div className="flex items-center gap-2">
            <Switch
              id="link-target"
              checked={value.target === '_blank'}
              onCheckedChange={(checked) =>
                onChange({ ...value, target: checked ? '_blank' : '_self' })
              }
            />
            <Label htmlFor="link-target" className="text-xs cursor-pointer">
              Abrir em nova aba
            </Label>
          </div>
        </>
      )}
    </div>
  )
}

// ─── Button Editor ───────────────────────────────────────────────────────────

function ButtonEditor({
  value,
  onChange,
}: {
  value: ButtonConfig
  onChange: (btn: ButtonConfig) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Texto do botão">
        <Input
          className="h-8 text-xs"
          value={value.label}
          onChange={(e) => onChange({ ...value, label: e.target.value })}
          placeholder="Clique aqui"
        />
      </Field>
      <Field label="Estilo">
        <Select
          value={value.variant}
          onValueChange={(v) => onChange({ ...value, variant: v as ButtonConfig['variant'] })}
        >
          <SelectTrigger className="h-8 text-xs">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="primary">Primário</SelectItem>
            <SelectItem value="secondary">Secundário</SelectItem>
            <SelectItem value="ghost">Ghost</SelectItem>
          </SelectContent>
        </Select>
      </Field>
      <LinkEditor
        value={value.link}
        onChange={(link) => onChange({ ...value, link })}
        label="Destino do botão"
      />
    </div>
  )
}

// ─── Content Editors by Type ─────────────────────────────────────────────────

function SlideBackgroundEditor({
  value,
  onChange,
}: {
  value: BackgroundConfig
  onChange: (bg: BackgroundConfig) => void
}) {
  const fileRef = useRef<HTMLInputElement>(null)

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    const reader = new FileReader()
    reader.onload = () => onChange({ type: 'image', image: reader.result as string })
    reader.readAsDataURL(file)
  }

  return (
    <div className="space-y-2">
      <Select
        value={value.type}
        onValueChange={(v) => {
          const newType = v as BackgroundConfig['type']
          if (newType === 'solid') {
            onChange({ type: 'solid', color: value.color ?? '#9A1BFF' })
          } else if (newType === 'gradient') {
            onChange({
              type: 'gradient',
              gradient: value.gradient ?? { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' },
            })
          } else if (newType === 'image') {
            onChange({ type: 'image', image: value.image })
          } else {
            onChange({ type: 'none' })
          }
        }}
      >
        <SelectTrigger className="h-7 text-xs"><SelectValue /></SelectTrigger>
        <SelectContent>
          <SelectItem value="solid">Cor sólida</SelectItem>
          <SelectItem value="gradient">Gradiente</SelectItem>
          <SelectItem value="image">Imagem</SelectItem>
        </SelectContent>
      </Select>

      {value.type === 'solid' && (
        <div className="flex items-center gap-2">
          <input type="color" value={value.color ?? '#000000'} onChange={(e) => onChange({ ...value, color: e.target.value })} className="h-7 w-9 cursor-pointer rounded border border-borderDefault p-0.5" />
          <Input className="h-7 text-xs font-mono flex-1" value={value.color ?? '#000000'} onChange={(e) => onChange({ ...value, color: e.target.value })} />
        </div>
      )}

      {value.type === 'gradient' && (
        <div className="space-y-2">
          <div className="grid grid-cols-2 gap-2">
            <div>
              <p className="text-xs text-secondaryText mb-1">Inicial</p>
              <input type="color" value={value.gradient?.from ?? '#9A1BFF'} onChange={(e) => onChange({ ...value, gradient: { from: e.target.value, to: value.gradient?.to ?? '#4F46E5', direction: value.gradient?.direction ?? 'to-br' } })} className="h-7 w-full cursor-pointer rounded border border-borderDefault p-0.5" />
            </div>
            <div>
              <p className="text-xs text-secondaryText mb-1">Final</p>
              <input type="color" value={value.gradient?.to ?? '#4F46E5'} onChange={(e) => onChange({ ...value, gradient: { from: value.gradient?.from ?? '#9A1BFF', to: e.target.value, direction: value.gradient?.direction ?? 'to-br' } })} className="h-7 w-full cursor-pointer rounded border border-borderDefault p-0.5" />
            </div>
          </div>
          <Select value={value.gradient?.direction ?? 'to-br'} onValueChange={(v) => onChange({ ...value, gradient: { from: value.gradient?.from ?? '#9A1BFF', to: value.gradient?.to ?? '#4F46E5', direction: v as 'to-r' | 'to-br' | 'to-b' | 'to-bl' | 'to-tr' } })}>
            <SelectTrigger className="h-7 text-xs"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem value="to-r">→ Horizontal</SelectItem>
              <SelectItem value="to-br">↘ Diagonal</SelectItem>
              <SelectItem value="to-b">↓ Vertical</SelectItem>
              <SelectItem value="to-bl">↙ Diagonal</SelectItem>
              <SelectItem value="to-tr">↗ Diagonal</SelectItem>
            </SelectContent>
          </Select>
          <div className="h-6 rounded" style={{ backgroundImage: `linear-gradient(${{ 'to-r': 'to right', 'to-br': 'to bottom right', 'to-b': 'to bottom', 'to-bl': 'to bottom left', 'to-tr': 'to top right' }[value.gradient?.direction ?? 'to-br']}, ${value.gradient?.from ?? '#9A1BFF'}, ${value.gradient?.to ?? '#4F46E5'})` }} />
        </div>
      )}

      {value.type === 'image' && (
        <div className="space-y-1.5">
          <input ref={fileRef} type="file" accept="image/*" className="hidden" onChange={handleImageUpload} />
          <Button variant="secondary" size="sm" className="w-full gap-1.5" onClick={() => fileRef.current?.click()}>
            <Upload size={12} />
            {value.image ? 'Trocar imagem' : 'Upload'}
          </Button>
          {value.image && (
            <div className="relative">
              <img src={value.image} alt="" className="w-full h-14 object-cover rounded" />
              <button onClick={() => onChange({ type: 'image' })} className="absolute top-1 right-1 p-0.5 bg-black/60 rounded text-white text-xs"><Trash2 size={11} /></button>
            </div>
          )}
        </div>
      )}
    </div>
  )
}

function SlideEditor({
  slide,
  index,
  onUpdate,
  onRemove,
}: {
  slide: BannerSlide
  index: number
  onUpdate: (id: string, updates: Partial<BannerSlide>) => void
  onRemove: (id: string) => void
}) {
  const [expanded, setExpanded] = useState(index === 0)

  return (
    <div className="border border-borderDefault rounded-lgToken overflow-hidden">
      <button
        onClick={() => setExpanded((p) => !p)}
        className="w-full flex items-center justify-between px-3 py-2.5 bg-muted/40 hover:bg-muted/70 transition-colors"
      >
        <div className="flex items-center gap-2">
          <div
            className="w-4 h-4 rounded flex-shrink-0"
            style={(() => {
              const bg = slide.background
              if (bg.type === 'solid' && bg.color) return { backgroundColor: bg.color }
              if (bg.type === 'gradient' && bg.gradient) return { backgroundImage: `linear-gradient(to right, ${bg.gradient.from}, ${bg.gradient.to})` }
              if (bg.type === 'image') return { backgroundImage: `url(${bg.image})`, backgroundSize: 'cover' }
              return { backgroundColor: '#94a3b8' }
            })()}
          />
          <span className="text-xs font-medium text-primaryText">
            {slide.title ? slide.title.substring(0, 24) + (slide.title.length > 24 ? '...' : '') : `Slide ${index + 1}`}
          </span>
        </div>
        <div className="flex items-center gap-2">
          <button
            onClick={(e) => { e.stopPropagation(); onRemove(slide.id) }}
            className="p-0.5 text-secondaryText hover:text-error transition-colors"
          >
            <Trash2 size={12} />
          </button>
          {expanded ? <ChevronUp size={14} className="text-secondaryText" /> : <ChevronDown size={14} className="text-secondaryText" />}
        </div>
      </button>

      {expanded && (
        <div className="p-3 space-y-3 border-t border-borderDefault">
          {/* Background */}
          <div>
            <p className="text-xs font-medium text-secondaryText mb-1.5">Fundo</p>
            <SlideBackgroundEditor value={slide.background} onChange={(bg) => onUpdate(slide.id, { background: bg })} />
          </div>

          <Separator />

          {/* Título */}
          <div className="space-y-1.5">
            <p className="text-xs font-medium text-primaryText">Título</p>
            <Input className="h-7 text-xs" value={slide.title ?? ''} onChange={(e) => onUpdate(slide.id, { title: e.target.value })} placeholder="Título do slide" />
          </div>

          {/* Subtítulo */}
          <div className="space-y-1.5">
            <p className="text-xs font-medium text-primaryText">Subtítulo</p>
            <Textarea className="text-xs min-h-[50px] resize-none" value={slide.subtitle ?? ''} onChange={(e) => onUpdate(slide.id, { subtitle: e.target.value })} placeholder="Texto de apoio" />
          </div>

          {/* Cor do texto */}
          <div className="grid grid-cols-2 gap-2">
            <div className="space-y-1.5">
              <p className="text-xs font-medium text-primaryText">Cor do texto</p>
              <div className="grid grid-cols-2 gap-1">
                {(['light', 'dark'] as const).map((c) => (
                  <button
                    key={c}
                    onClick={() => onUpdate(slide.id, { textColor: c })}
                    className={cn(
                      'h-7 text-xs rounded border transition-colors',
                      slide.textColor === c ? 'border-primary bg-primarySoft text-primary font-semibold' : 'border-borderDefault text-secondaryText hover:border-primary/40'
                    )}
                  >
                    {c === 'light' ? '☀ Claro' : '◆ Escuro'}
                  </button>
                ))}
              </div>
            </div>
            <div className="space-y-1.5">
              <p className="text-xs font-medium text-primaryText">Alinhamento</p>
              <Select value={slide.textAlign} onValueChange={(v) => onUpdate(slide.id, { textAlign: v as 'left' | 'center' | 'right' })}>
                <SelectTrigger className="h-7 text-xs"><SelectValue /></SelectTrigger>
                <SelectContent>
                  <SelectItem value="left">Esquerda</SelectItem>
                  <SelectItem value="center">Centro</SelectItem>
                  <SelectItem value="right">Direita</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Posição vertical */}
          <div className="space-y-1.5">
            <p className="text-xs font-medium text-primaryText">Posição vertical</p>
            <div className="grid grid-cols-3 gap-1">
              {(['top', 'center', 'bottom'] as const).map((v) => (
                <button
                  key={v}
                  onClick={() => onUpdate(slide.id, { textVerticalAlign: v })}
                  className={cn(
                    'h-7 text-xs rounded border transition-colors',
                    slide.textVerticalAlign === v ? 'border-primary bg-primarySoft text-primary font-semibold' : 'border-borderDefault text-secondaryText hover:border-primary/40'
                  )}
                >
                  {v === 'top' ? '⬆ Topo' : v === 'center' ? '⬛ Meio' : '⬇ Base'}
                </button>
              ))}
            </div>
          </div>

          <Separator />

          {/* Botão CTA */}
          <div className="space-y-2">
            <p className="text-xs font-medium text-primaryText">Botão (opcional)</p>
            <Input
              className="h-7 text-xs"
              value={slide.button?.label ?? ''}
              onChange={(e) => onUpdate(slide.id, {
                button: { ...slide.button ?? { variant: 'primary', link: { type: 'none', href: '', target: '_self' } }, label: e.target.value }
              })}
              placeholder="Texto do botão"
            />
            {slide.button !== undefined && (
              <>
                <Select value={slide.button.variant} onValueChange={(v) => onUpdate(slide.id, { button: { ...slide.button!, variant: v as 'primary' | 'secondary' | 'ghost' } })}>
                  <SelectTrigger className="h-7 text-xs"><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="primary">Primário</SelectItem>
                    <SelectItem value="secondary">Secundário</SelectItem>
                    <SelectItem value="ghost">Ghost</SelectItem>
                  </SelectContent>
                </Select>
                <Select value={slide.button.link.type} onValueChange={(v) => onUpdate(slide.id, { button: { ...slide.button!, link: { ...slide.button!.link, type: v as 'internal' | 'external' | 'none' } } })}>
                  <SelectTrigger className="h-7 text-xs"><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="none">Sem link</SelectItem>
                    <SelectItem value="internal">Rota interna</SelectItem>
                    <SelectItem value="external">URL externa</SelectItem>
                  </SelectContent>
                </Select>
                {slide.button.link.type !== 'none' && (
                  <Input className="h-7 text-xs" value={slide.button.link.href} onChange={(e) => onUpdate(slide.id, { button: { ...slide.button!, link: { ...slide.button!.link, href: e.target.value } } })} placeholder={slide.button.link.type === 'internal' ? '/rota' : 'https://'} />
                )}
              </>
            )}
          </div>
        </div>
      )}
    </div>
  )
}

function BannerEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'banner' }>
  onChange: (c: SectionContent) => void
}) {
  const updateSlide = (id: string, updates: Partial<BannerSlide>) => {
    onChange({ ...content, slides: content.slides.map((s) => (s.id === id ? { ...s, ...updates } : s)) })
  }

  const removeSlide = (id: string) => {
    onChange({ ...content, slides: content.slides.filter((s) => s.id !== id) })
  }

  const addSlide = () => {
    const newSlide: BannerSlide = {
      id: `slide_${Date.now()}`,
      background: { type: 'gradient', gradient: { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' } },
      title: 'Novo slide',
      textColor: 'light',
      textAlign: 'left',
      textVerticalAlign: 'center',
    }
    onChange({ ...content, slides: [...content.slides, newSlide] })
  }

  return (
    <div className="space-y-3">
      {/* Configurações gerais */}
      <Field label="Altura">
        <Select value={content.height} onValueChange={(v) => onChange({ ...content, height: v as 'sm' | 'md' | 'lg' })}>
          <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="sm">Pequeno (160px)</SelectItem>
            <SelectItem value="md">Médio (250px)</SelectItem>
            <SelectItem value="lg">Grande (380px)</SelectItem>
          </SelectContent>
        </Select>
      </Field>

      <div className="grid grid-cols-2 gap-2">
        <div className="flex items-center gap-2">
          <Switch id="banner-autoplay" checked={content.autoPlay} onCheckedChange={(v) => onChange({ ...content, autoPlay: v })} />
          <Label htmlFor="banner-autoplay" className="text-xs cursor-pointer">Auto-play</Label>
        </div>
        <div className="flex items-center gap-2">
          <Switch id="banner-dots" checked={content.showDots} onCheckedChange={(v) => onChange({ ...content, showDots: v })} />
          <Label htmlFor="banner-dots" className="text-xs cursor-pointer">Pontos</Label>
        </div>
        <div className="flex items-center gap-2">
          <Switch id="banner-arrows" checked={content.showArrows} onCheckedChange={(v) => onChange({ ...content, showArrows: v })} />
          <Label htmlFor="banner-arrows" className="text-xs cursor-pointer">Setas</Label>
        </div>
      </div>

      {content.autoPlay && (
        <Field label={`Intervalo: ${content.interval}s`}>
          <input
            type="range"
            min={2}
            max={15}
            value={content.interval}
            onChange={(e) => onChange({ ...content, interval: Number(e.target.value) })}
            className="w-full h-1.5 accent-primary cursor-pointer"
          />
        </Field>
      )}

      <Separator />

      {/* Slides */}
      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <p className="text-xs font-semibold text-primaryText">Slides ({content.slides.length})</p>
        </div>
        {content.slides.map((slide, i) => (
          <SlideEditor key={slide.id} slide={slide} index={i} onUpdate={updateSlide} onRemove={removeSlide} />
        ))}
        <Button variant="secondary" size="sm" className="w-full gap-1.5 mt-1" onClick={addSlide}>
          <Plus size={13} />
          Adicionar slide
        </Button>
      </div>
    </div>
  )
}

function FeedEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'feed' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título (opcional)">
        <Input className="h-8 text-xs" value={content.title ?? ''} onChange={(e) => onChange({ ...content, title: e.target.value })} placeholder="Feed" />
      </Field>
      <div className="flex items-center gap-2">
        <Switch id="feed-filters" checked={content.showFilters} onCheckedChange={(v) => onChange({ ...content, showFilters: v })} />
        <Label htmlFor="feed-filters" className="text-xs cursor-pointer">Mostrar filtros de grupos</Label>
      </div>
      <div className="flex items-center gap-2">
        <Switch id="feed-compact" checked={content.compactMode} onCheckedChange={(v) => onChange({ ...content, compactMode: v })} />
        <Label htmlFor="feed-compact" className="text-xs cursor-pointer">Modo compacto</Label>
      </div>
    </div>
  )
}

function AniversariantesEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'aniversariantes' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título da seção">
        <Input className="h-8 text-xs" value={content.title} onChange={(e) => onChange({ ...content, title: e.target.value })} placeholder="Aniversariantes da semana" />
      </Field>
      <Field label="Altura máxima">
        <Select value={content.height} onValueChange={(v) => onChange({ ...content, height: v as 'sm' | 'md' | 'lg' })}>
          <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="sm">Pequena (~256px)</SelectItem>
            <SelectItem value="md">Média (~384px)</SelectItem>
            <SelectItem value="lg">Grande (~480px)</SelectItem>
          </SelectContent>
        </Select>
      </Field>
    </div>
  )
}

const AVAILABLE_ICONS = [
  'User', 'TrendingUp', 'ArrowUpDown', 'Calendar', 'MessageCircle',
  'FileText', 'Settings', 'Globe', 'Zap', 'Star', 'Heart', 'Briefcase',
  'GraduationCap', 'DollarSign',
]

function ShortcutsEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'shortcuts' }>
  onChange: (c: SectionContent) => void
}) {
  const addItem = () => {
    const newItem: ShortcutItem = {
      id: `item_${Date.now()}`,
      iconName: 'Star',
      label: 'Novo atalho',
      link: { type: 'internal', href: '/', target: '_self' },
    }
    onChange({ ...content, items: [...content.items, newItem] })
  }

  const updateItem = (id: string, updates: Partial<ShortcutItem>) => {
    onChange({
      ...content,
      items: content.items.map((item) => (item.id === id ? { ...item, ...updates } : item)),
    })
  }

  const removeItem = (id: string) => {
    onChange({ ...content, items: content.items.filter((item) => item.id !== id) })
  }

  return (
    <div className="space-y-3">
      <Field label="Título (opcional)">
        <Input className="h-8 text-xs" value={content.title ?? ''} onChange={(e) => onChange({ ...content, title: e.target.value })} placeholder="Acesso rápido" />
      </Field>
      <Field label="Colunas">
        <Select value={String(content.columns)} onValueChange={(v) => onChange({ ...content, columns: Number(v) as 2 | 3 | 4 })}>
          <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="2">2 colunas</SelectItem>
            <SelectItem value="3">3 colunas</SelectItem>
            <SelectItem value="4">4 colunas</SelectItem>
          </SelectContent>
        </Select>
      </Field>
      <div className="space-y-2">
        <Label className="text-xs font-medium text-primaryText">Atalhos</Label>
        {content.items.map((item) => (
          <div key={item.id} className="border border-borderDefault rounded-lgToken p-2.5 space-y-2">
            <div className="flex items-center justify-between">
              <Input
                className="h-7 text-xs flex-1 mr-2"
                value={item.label}
                onChange={(e) => updateItem(item.id, { label: e.target.value })}
                placeholder="Nome do atalho"
              />
              <button
                onClick={() => removeItem(item.id)}
                className="p-1 text-secondaryText hover:text-error transition-colors"
              >
                <Trash2 size={13} />
              </button>
            </div>
            <Select value={item.iconName} onValueChange={(v) => updateItem(item.id, { iconName: v })}>
              <SelectTrigger className="h-7 text-xs"><SelectValue /></SelectTrigger>
              <SelectContent>
                {AVAILABLE_ICONS.map((icon) => (
                  <SelectItem key={icon} value={icon}>{icon}</SelectItem>
                ))}
              </SelectContent>
            </Select>
            <Input
              className="h-7 text-xs"
              value={item.link.href}
              onChange={(e) => updateItem(item.id, { link: { ...item.link, href: e.target.value } })}
              placeholder="/rota"
            />
          </div>
        ))}
        <Button variant="secondary" size="sm" className="w-full gap-1.5" onClick={addItem}>
          <Plus size={14} />
          Adicionar atalho
        </Button>
      </div>
    </div>
  )
}

function ActionCardEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'action-card' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título">
        <Textarea className="text-xs min-h-[70px] resize-none" value={content.title} onChange={(e) => onChange({ ...content, title: e.target.value })} placeholder="Título do card" />
      </Field>
      <Field label="Descrição (opcional)">
        <Textarea className="text-xs min-h-[60px] resize-none" value={content.description ?? ''} onChange={(e) => onChange({ ...content, description: e.target.value })} placeholder="Texto complementar" />
      </Field>

      <div className="grid grid-cols-2 gap-3">
        <Field label="Cor do texto">
          <div className="grid grid-cols-2 gap-1">
            {(['light', 'dark'] as const).map((c) => (
              <button
                key={c}
                onClick={() => onChange({ ...content, textColor: c })}
                className={cn(
                  'h-8 text-xs rounded-lgToken border transition-colors',
                  content.textColor === c
                    ? 'border-primary bg-primarySoft text-primary font-semibold'
                    : 'border-borderDefault text-secondaryText hover:border-primary/40'
                )}
              >
                {c === 'light' ? '☀ Claro' : '◆ Escuro'}
              </button>
            ))}
          </div>
        </Field>
        <Field label="Altura mínima">
          <Select value={content.minHeight} onValueChange={(v) => onChange({ ...content, minHeight: v as 'sm' | 'md' | 'lg' })}>
            <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem value="sm">Pequena</SelectItem>
              <SelectItem value="md">Média</SelectItem>
              <SelectItem value="lg">Grande</SelectItem>
            </SelectContent>
          </Select>
        </Field>
      </div>

      <Separator />
      <p className="text-xs font-medium text-primaryText">Botão</p>
      <ButtonEditor value={content.button} onChange={(btn) => onChange({ ...content, button: btn })} />
    </div>
  )
}

function WelcomeEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'welcome' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Saudação" hint="Use {nomeColaborador} para inserir o nome">
        <Input className="h-8 text-xs" value={content.greeting} onChange={(e) => onChange({ ...content, greeting: e.target.value })} placeholder="Olá, {nomeColaborador}!" />
      </Field>
      <Field label="Subtítulo">
        <Input className="h-8 text-xs" value={content.subtitle} onChange={(e) => onChange({ ...content, subtitle: e.target.value })} placeholder="Seja bem-vindo ao Fourmakers" />
      </Field>
      <div className="flex items-center gap-2">
        <Switch id="welcome-name" checked={content.showUserName} onCheckedChange={(v) => onChange({ ...content, showUserName: v })} />
        <Label htmlFor="welcome-name" className="text-xs cursor-pointer">Exibir nome do usuário</Label>
      </div>
    </div>
  )
}

function VideoEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'video' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título (opcional)">
        <Input className="h-8 text-xs" value={content.title ?? ''} onChange={(e) => onChange({ ...content, title: e.target.value })} placeholder="Título do vídeo" />
      </Field>
      <Field label="URL do vídeo" hint="YouTube ou Vimeo">
        <Input className="h-8 text-xs" value={content.url} onChange={(e) => onChange({ ...content, url: e.target.value })} placeholder="https://youtube.com/watch?v=..." />
      </Field>
      <Field label="Proporção">
        <Select value={content.aspectRatio} onValueChange={(v) => onChange({ ...content, aspectRatio: v as '16/9' | '4/3' })}>
          <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="16/9">16:9 (Widescreen)</SelectItem>
            <SelectItem value="4/3">4:3 (Clássico)</SelectItem>
          </SelectContent>
        </Select>
      </Field>
      <div className="flex items-center gap-2">
        <Switch id="video-autoplay" checked={content.autoplay} onCheckedChange={(v) => onChange({ ...content, autoplay: v })} />
        <Label htmlFor="video-autoplay" className="text-xs cursor-pointer">Reproduzir automaticamente</Label>
      </div>
      <div className="flex items-center gap-2">
        <Switch id="video-controls" checked={content.controls} onCheckedChange={(v) => onChange({ ...content, controls: v })} />
        <Label htmlFor="video-controls" className="text-xs cursor-pointer">Mostrar controles</Label>
      </div>
    </div>
  )
}

function TextBlockEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'text-block' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título (opcional)">
        <Input className="h-8 text-xs" value={content.title ?? ''} onChange={(e) => onChange({ ...content, title: e.target.value })} placeholder="Título da seção" />
      </Field>
      <Field label="Conteúdo">
        <Textarea className="text-xs min-h-[120px] resize-none" value={content.body} onChange={(e) => onChange({ ...content, body: e.target.value })} placeholder="Escreva seu conteúdo aqui..." />
      </Field>
      <div className="grid grid-cols-2 gap-3">
        <Field label="Alinhamento">
          <Select value={content.alignment} onValueChange={(v) => onChange({ ...content, alignment: v as 'left' | 'center' | 'right' })}>
            <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem value="left">Esquerda</SelectItem>
              <SelectItem value="center">Centro</SelectItem>
              <SelectItem value="right">Direita</SelectItem>
            </SelectContent>
          </Select>
        </Field>
        <Field label="Altura mínima">
          <Select value={content.minHeight} onValueChange={(v) => onChange({ ...content, minHeight: v as 'sm' | 'md' | 'lg' })}>
            <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem value="sm">Pequena</SelectItem>
              <SelectItem value="md">Média</SelectItem>
              <SelectItem value="lg">Grande</SelectItem>
            </SelectContent>
          </Select>
        </Field>
      </div>
      <Field label="Cor do texto">
        <div className="grid grid-cols-2 gap-1">
          {(['light', 'dark'] as const).map((c) => (
            <button
              key={c}
              onClick={() => onChange({ ...content, textColor: c })}
              className={cn(
                'h-8 text-xs rounded-lgToken border transition-colors',
                content.textColor === c
                  ? 'border-primary bg-primarySoft text-primary font-semibold'
                  : 'border-borderDefault text-secondaryText hover:border-primary/40'
              )}
            >
              {c === 'light' ? '☀ Claro' : '◆ Escuro'}
            </button>
          ))}
        </div>
      </Field>
    </div>
  )
}

function ProfissionaisMosaicoEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'profissionais-mosaico' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título (opcional)">
        <Input
          className="h-8 text-xs"
          value={content.title ?? ''}
          onChange={(e) => onChange({ ...content, title: e.target.value || undefined })}
          placeholder="Ex.: Nossos profissionais"
        />
      </Field>
      <Field label="Altura do componente">
        <Select
          value={content.height}
          onValueChange={(v) => onChange({ ...content, height: v as 'sm' | 'md' | 'lg' })}
        >
          <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="sm">Pequena (6 cards)</SelectItem>
            <SelectItem value="md">Média (9 cards)</SelectItem>
            <SelectItem value="lg">Grande (12 cards)</SelectItem>
          </SelectContent>
        </Select>
      </Field>
      <div className="rounded-lgToken bg-muted p-3">
        <p className="text-xs text-secondaryText">
          Favoritos aparecem nas primeiras posições. O mosaico exibe 3 profissionais por linha com scroll
          infinito interno.
        </p>
      </div>
    </div>
  )
}

function BeneficiosEditor({
  content,
  onChange,
}: {
  content: Extract<SectionContent, { type: 'beneficios' }>
  onChange: (c: SectionContent) => void
}) {
  return (
    <div className="space-y-3">
      <Field label="Título (opcional)">
        <Input
          className="h-8 text-xs"
          value={content.title ?? ''}
          onChange={(e) => onChange({ ...content, title: e.target.value || undefined })}
          placeholder="Ex.: Benefícios e parcerias"
        />
      </Field>
      <Field label="Altura do componente">
        <Select
          value={content.height}
          onValueChange={(v) => onChange({ ...content, height: v as 'sm' | 'md' | 'lg' })}
        >
          <SelectTrigger className="h-8 text-xs"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="sm">Pequena (6 cards)</SelectItem>
            <SelectItem value="md">Média (9 cards)</SelectItem>
            <SelectItem value="lg">Grande (12 cards)</SelectItem>
          </SelectContent>
        </Select>
      </Field>
      <div className="rounded-lgToken bg-muted p-3">
        <p className="text-xs text-secondaryText">
          Lista de benefícios e parcerias da API XANO em mosaico, com busca e scroll infinito. Ao clicar em &quot;Saiba mais&quot; o usuário é levado à página de detalhes do benefício.
        </p>
      </div>
    </div>
  )
}

// ─── Main Property Panel ──────────────────────────────────────────────────────

interface BuilderPropertyPanelProps {
  section: HomeSection | null
  onUpdate: (id: string, updates: Partial<HomeSection>) => void
}

export function BuilderPropertyPanel({ section, onUpdate }: BuilderPropertyPanelProps) {
  if (!section) {
    return (
      <aside className="w-80 bg-secondaryBackground border-l border-borderDefault flex flex-col items-center justify-center p-6 text-center">
        <div className="p-4 bg-muted rounded-full mb-3">
          <Settings2 size={28} className="text-secondaryText" />
        </div>
        <p className="text-sm font-semibold text-primaryText mb-1">Nenhum componente selecionado</p>
        <p className="text-xs text-secondaryText">
          Clique em um componente no canvas para editar suas propriedades
        </p>
      </aside>
    )
  }

  const type = section.content.type

  return (
    <aside className="w-80 bg-secondaryBackground border-l border-borderDefault flex flex-col overflow-y-auto">
      <div className="px-4 py-4 border-b border-borderDefault">
        <h2 className="text-sm font-semibold text-primaryText">
          Editar: {SECTION_TYPE_LABELS[type]}
        </h2>
      </div>

      <div className="flex-1 overflow-y-auto p-4 space-y-6">
        {/* Layout */}
        <div>
          <SectionTitle>Layout</SectionTitle>
          <div className="space-y-3">
            <Field label="Largura">
              <div className="grid grid-cols-2 gap-2">
                {([
                  { value: 'full',   label: '▬',  sublabel: '100%' },
                  { value: 'wide',   label: '▊',  sublabel: '70%'  },
                  { value: 'half',   label: '▌',  sublabel: '50%'  },
                  { value: 'narrow', label: '▍',  sublabel: '30%'  },
                ] as const).map(({ value, label, sublabel }) => (
                  <button
                    key={value}
                    onClick={() => onUpdate(section.id, { columnSpan: value })}
                    className={`h-8 text-xs rounded-lgToken border transition-colors ${
                      section.columnSpan === value
                        ? 'border-primary bg-primarySoft text-primary font-semibold'
                        : 'border-borderDefault text-secondaryText hover:border-primary/40'
                    }`}
                  >
                    {label} {sublabel}
                  </button>
                ))}
              </div>
            </Field>
            <div className="flex items-center gap-2">
              <Switch
                id="section-visible"
                checked={section.visible}
                onCheckedChange={(v) => onUpdate(section.id, { visible: v })}
              />
              <Label htmlFor="section-visible" className="text-xs cursor-pointer">
                Componente visível
              </Label>
            </div>
          </div>
        </div>

        <Separator />

        {/* Background */}
        <div>
          <SectionTitle>Fundo da seção</SectionTitle>
          <BackgroundEditor
            value={section.background}
            onChange={(bg) => onUpdate(section.id, { background: bg })}
          />
        </div>

        <Separator />

        {/* Content by type */}
        {type !== 'canal-denuncias' && type !== 'profissionais-mosaico' && type !== 'beneficios' && (
          <div>
            <SectionTitle>Conteúdo</SectionTitle>
            {type === 'banner' && (
              <BannerEditor
                content={section.content as Extract<SectionContent, { type: 'banner' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'feed' && (
              <FeedEditor
                content={section.content as Extract<SectionContent, { type: 'feed' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'aniversariantes' && (
              <AniversariantesEditor
                content={section.content as Extract<SectionContent, { type: 'aniversariantes' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'shortcuts' && (
              <ShortcutsEditor
                content={section.content as Extract<SectionContent, { type: 'shortcuts' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'action-card' && (
              <ActionCardEditor
                content={section.content as Extract<SectionContent, { type: 'action-card' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'welcome' && (
              <WelcomeEditor
                content={section.content as Extract<SectionContent, { type: 'welcome' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'video' && (
              <VideoEditor
                content={section.content as Extract<SectionContent, { type: 'video' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
            {type === 'text-block' && (
              <TextBlockEditor
                content={section.content as Extract<SectionContent, { type: 'text-block' }>}
                onChange={(c) => onUpdate(section.id, { content: c })}
              />
            )}
          </div>
        )}

        {type === 'canal-denuncias' && (
          <div className="rounded-lgToken bg-muted p-3">
            <p className="text-xs text-secondaryText">
              O Canal de Denúncias é configurado via parâmetro{' '}
              <code className="bg-muted-foreground/10 px-1 rounded text-primaryText">
                CANAL_DENUNCIA_CONFIGURACOES
              </code>
              . Apenas ative ou desative a exibição acima.
            </p>
          </div>
        )}

        {type === 'profissionais-mosaico' && (
          <div>
            <SectionTitle>Conteúdo</SectionTitle>
            <ProfissionaisMosaicoEditor
              content={section.content as Extract<SectionContent, { type: 'profissionais-mosaico' }>}
              onChange={(c) => onUpdate(section.id, { content: c })}
            />
          </div>
        )}

        {type === 'beneficios' && (
          <div>
            <SectionTitle>Conteúdo</SectionTitle>
            <BeneficiosEditor
              content={section.content as Extract<SectionContent, { type: 'beneficios' }>}
              onChange={(c) => onUpdate(section.id, { content: c })}
            />
          </div>
        )}
      </div>
    </aside>
  )
}
