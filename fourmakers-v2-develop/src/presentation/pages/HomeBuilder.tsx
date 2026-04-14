import { useState, useCallback, useEffect, useMemo } from 'react'
import { container } from 'tsyringe'
import { useAppSelector } from '@app/store/hooks'
import type { HomeConfig, HomeSection, SectionType } from '@shared/types/homeBuilder'
import { createDefaultConfig } from '@presentation/components/home-builder/defaultConfig'
import { BuilderToolbar } from '@presentation/components/home-builder/BuilderToolbar'
import { BuilderPalette } from '@presentation/components/home-builder/BuilderPalette'
import { BuilderCanvas } from '@presentation/components/home-builder/BuilderCanvas'
import { BuilderPropertyPanel } from '@presentation/components/home-builder/BuilderPropertyPanel'
import { Spinner } from '@/components/ui/spinner'
import { ListarParametrosConfiguracaoUseCase } from '@domain/usecases/ListarParametrosConfiguracaoUseCase'
import { InserirParametroConfiguracaoUseCase } from '@domain/usecases/InserirParametroConfiguracaoUseCase'
import { AtualizarParametroConfiguracaoUseCase } from '@domain/usecases/AtualizarParametroConfiguracaoUseCase'
import { toast } from 'sonner'

const HOME_PARAMETRIZADA_CODE = 'HOME_PARAMETRIZADA'
const PARAMETRO_NIVEL_ID = 3

function parseHomeConfigFromParam(valorParametro: string): HomeConfig | null {
  try {
    const parsed = JSON.parse(valorParametro) as unknown
    if (parsed && typeof parsed === 'object' && 'version' in parsed && Array.isArray((parsed as HomeConfig).sections)) {
      return parsed as HomeConfig
    }
  } catch {
    // ignore
  }
  return null
}

function createEmptySection(type: SectionType): HomeSection {
  const id = `section_${Date.now()}_${Math.random().toString(36).substring(2, 7)}`

  const contentByType: Record<SectionType, HomeSection['content']> = {
    banner: {
      type: 'banner',
      height: 'lg',
      autoPlay: true,
      interval: 5,
      showDots: true,
      showArrows: true,
      slides: [
        {
          id: `slide_${Date.now()}`,
          background: { type: 'gradient', gradient: { from: '#9A1BFF', to: '#4F46E5', direction: 'to-br' } },
          title: 'Título do banner',
          subtitle: 'Subtítulo do banner',
          textColor: 'light',
          textAlign: 'left',
          textVerticalAlign: 'center',
        },
      ],
    },
    feed: { type: 'feed', showFilters: true, compactMode: false },
    aniversariantes: { type: 'aniversariantes', title: 'Aniversariantes da semana', height: 'md' },
    shortcuts: {
      type: 'shortcuts',
      title: 'Acesso rápido',
      items: [
        {
          id: `item_${Date.now()}`,
          iconName: 'Star',
          label: 'Novo atalho',
          link: { type: 'internal', href: '/', target: '_self' },
        },
      ],
      columns: 3,
    },
    'action-card': {
      type: 'action-card',
      title: 'Título do card',
      textColor: 'light',
      minHeight: 'md',
      button: {
        label: 'Saiba mais',
        variant: 'primary',
        link: { type: 'none', href: '', target: '_self' },
      },
    },
    welcome: {
      type: 'welcome',
      greeting: 'Olá, {nomeColaborador}!',
      subtitle: 'Seja bem-vindo ao Fourmakers',
      showUserName: true,
    },
    'canal-denuncias': { type: 'canal-denuncias' },
    video: {
      type: 'video',
      url: '',
      aspectRatio: '16/9',
      autoplay: false,
      controls: true,
    },
    'text-block': {
      type: 'text-block',
      body: 'Escreva seu conteúdo aqui...',
      alignment: 'left',
      textColor: 'dark',
      minHeight: 'sm',
    },
    'profissionais-mosaico': {
      type: 'profissionais-mosaico',
      title: 'Nossos Profissionais',
      height: 'md',
    },
    beneficios: {
      type: 'beneficios',
      title: 'Benefícios e parcerias',
      height: 'md',
    },
    'comunidades-mosaico': {
      type: 'comunidades-mosaico',
      title: 'Comunidades',
      height: 'md',
      columns: 2,
      onlyMine: false,
    },
  }

  return {
    id,
    visible: true,
    columnSpan: type === 'banner' || type === 'feed' || type === 'canal-denuncias' ? 'full' : 'half',
    background: { type: 'none' },
    content: contentByType[type],
  }
}

type DeviceMode = 'mobile' | 'tablet' | 'desktop'

export function HomeBuilder() {
  const token = useAppSelector((state) => state.auth.token)
  const [config, setConfig] = useState<HomeConfig>(createDefaultConfig)
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [deviceMode, setDeviceMode] = useState<DeviceMode>('desktop')
  const [parametroId, setParametroId] = useState<string | null>(null)
  const [parametroOrgId, setParametroOrgId] = useState<number | null>(null)
  const [loadingConfig, setLoadingConfig] = useState(true)
  const [saving, setSaving] = useState(false)

  const listarUseCase = useMemo(() => container.resolve(ListarParametrosConfiguracaoUseCase), [])
  const inserirUseCase = useMemo(() => container.resolve(InserirParametroConfiguracaoUseCase), [])
  const atualizarUseCase = useMemo(() => container.resolve(AtualizarParametroConfiguracaoUseCase), [])

  useEffect(() => {
    if (!token) {
      setLoadingConfig(false)
      return
    }
    let cancelled = false
    setLoadingConfig(true)
    listarUseCase
      .execute(token)
      .then((res) => {
        if (cancelled || !res?.retorno) return
        const param = res.retorno.find((p) => p.codigoParametro === HOME_PARAMETRIZADA_CODE)
        if (param) {
          const parsed = parseHomeConfigFromParam(param.valorParametro)
          if (parsed) {
            setConfig(parsed)
          }
          setParametroId(param.id)
          setParametroOrgId(param.orgId)
        }
      })
      .catch(() => {
        if (!cancelled) toast.error('Não foi possível carregar a configuração da home.')
      })
      .finally(() => {
        if (!cancelled) setLoadingConfig(false)
      })
    return () => {
      cancelled = true
    }
  }, [token, listarUseCase])

  const selectedSection = config.sections.find((s) => s.id === selectedId) ?? null

  const addSection = useCallback((type: SectionType) => {
    const newSection = createEmptySection(type)
    setConfig((prev) => ({
      ...prev,
      sections: [...prev.sections, newSection],
    }))
    setSelectedId(newSection.id)
  }, [])

  const updateSection = useCallback((id: string, updates: Partial<HomeSection>) => {
    setConfig((prev) => ({
      ...prev,
      sections: prev.sections.map((s) => (s.id === id ? { ...s, ...updates } : s)),
    }))
  }, [])

  const removeSection = useCallback((id: string) => {
    setConfig((prev) => ({
      ...prev,
      sections: prev.sections.filter((s) => s.id !== id),
    }))
    setSelectedId((prev) => (prev === id ? null : prev))
  }, [])

  const toggleVisible = useCallback((id: string) => {
    setConfig((prev) => ({
      ...prev,
      sections: prev.sections.map((s) =>
        s.id === id ? { ...s, visible: !s.visible } : s
      ),
    }))
  }, [])

  const reorderSections = useCallback((sections: HomeSection[]) => {
    setConfig((prev) => ({ ...prev, sections }))
  }, [])

  const handleReset = () => {
    setConfig(createDefaultConfig())
    setSelectedId(null)
  }

  const handleSave = useCallback(async () => {
    if (!token) {
      toast.error('Faça login para salvar.')
      return
    }
    const payload = {
      colaboradorOrgCpf: '',
      grupoAcessoId: null as string | null,
      codigoParametro: HOME_PARAMETRIZADA_CODE,
      valorParametro: JSON.stringify(config),
      parametroNivelId: PARAMETRO_NIVEL_ID,
    }
    setSaving(true)
    try {
      if (parametroId != null && parametroOrgId != null) {
        await atualizarUseCase.execute(token, parametroId, parametroOrgId, payload)
        toast.success('Configuração da home salva.')
      } else {
        const res = await inserirUseCase.execute(token, payload)
        if (res?.sucesso) {
          if (res.retorno?.id != null && res.retorno?.orgId != null) {
            setParametroId(res.retorno.id)
            setParametroOrgId(res.retorno.orgId)
          } else {
            const list = await listarUseCase.execute(token)
            const param = list?.retorno?.find((p) => p.codigoParametro === HOME_PARAMETRIZADA_CODE)
            if (param) {
              setParametroId(param.id)
              setParametroOrgId(param.orgId)
            }
          }
          toast.success('Configuração da home salva.')
        } else {
          toast.error(res?.mensagem ?? 'Erro ao salvar.')
        }
      }
    } catch {
      toast.error('Não foi possível salvar a configuração da home.')
    } finally {
      setSaving(false)
    }
  }, [token, config, parametroId, parametroOrgId, listarUseCase, inserirUseCase, atualizarUseCase])

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-primaryBackground relative">
      {loadingConfig && (
        <div className="absolute inset-0 z-20 flex items-center justify-center bg-primaryBackground/80">
          <Spinner className="h-8 w-8 text-primary" />
        </div>
      )}
      <BuilderToolbar
        config={config}
        deviceMode={deviceMode}
        onDeviceChange={setDeviceMode}
        onReset={handleReset}
        onSave={handleSave}
        saving={saving}
        loadingConfig={loadingConfig}
      />

      <div className="flex flex-1 overflow-hidden">
        <BuilderPalette onAdd={addSection} />

        <BuilderCanvas
          sections={config.sections}
          selectedId={selectedId}
          onSelect={setSelectedId}
          onReorder={reorderSections}
          onToggleVisible={toggleVisible}
          onRemove={removeSection}
          deviceWidth={deviceMode}
        />

        <BuilderPropertyPanel
          section={selectedSection}
          onUpdate={updateSection}
        />
      </div>
    </div>
  )
}
