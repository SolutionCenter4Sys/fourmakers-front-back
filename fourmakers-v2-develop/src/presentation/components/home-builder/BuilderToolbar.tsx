import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import type { HomeConfig } from '@shared/types/homeBuilder'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { useToast } from '@/hooks/use-toast'
import { HomeRenderer } from '../home-renderer/HomeRenderer'
import {
  ArrowLeft,
  Eye,
  Download,
  Upload,
  RefreshCw,
  Save,
} from '@/components/ui/system-icons'
import { Spinner } from '@/components/ui/spinner'

type DeviceMode = 'mobile' | 'tablet' | 'desktop'

const DEVICE_OPTIONS: { value: DeviceMode; label: string; icon: string; width: string }[] = [
  { value: 'mobile', label: 'Mobile', icon: '📱', width: 'max-w-sm' },
  { value: 'tablet', label: 'Tablet', icon: '💻', width: 'max-w-2xl' },
  { value: 'desktop', label: 'Desktop', icon: '🖥️', width: 'max-w-full' },
]

interface BuilderToolbarProps {
  config: HomeConfig
  deviceMode: DeviceMode
  onDeviceChange: (device: DeviceMode) => void
  onReset: () => void
  onSave?: () => void
  saving?: boolean
  loadingConfig?: boolean
}

export function BuilderToolbar({
  config,
  deviceMode,
  onDeviceChange,
  onReset,
  onSave,
  saving = false,
  loadingConfig = false,
}: BuilderToolbarProps) {
  const navigate = useNavigate()
  const { toast } = useToast()
  const [previewOpen, setPreviewOpen] = useState(false)
  const [previewDevice, setPreviewDevice] = useState<DeviceMode>('desktop')

  const handleExport = () => {
    const json = JSON.stringify(config, null, 2)
    navigator.clipboard.writeText(json).then(() => {
      toast({
        title: 'JSON copiado!',
        description: 'O JSON da configuração foi copiado para a área de transferência.',
      })
    })
  }

  const handleImport = () => {
    navigator.clipboard.readText().then((text) => {
      try {
        JSON.parse(text)
        toast({
          title: 'JSON válido',
          description: 'Cole o JSON no campo de importação. (Feature em breve)',
        })
      } catch {
        toast({
          title: 'JSON inválido',
          description: 'O conteúdo copiado não é um JSON válido.',
          variant: 'destructive',
        })
      }
    })
  }

  const previewWidthClass = DEVICE_OPTIONS.find((d) => d.value === previewDevice)?.width ?? 'max-w-full'

  return (
    <>
      <header className="h-14 bg-secondaryBackground border-b border-borderDefault flex items-center px-4 gap-3 flex-shrink-0 z-10">
        {/* Left */}
        <div className="flex items-center gap-2 flex-1">
          <Button
            variant="ghost"
            size="sm"
            className="gap-1.5"
            onClick={() => navigate(-1)}
          >
            <ArrowLeft size={16} />
            Sair
          </Button>
          <div className="h-5 w-px bg-borderDefault" />
          <div className="flex items-center gap-1.5">
            <div className="h-2 w-2 rounded-full bg-accent" />
            <span className="text-sm font-semibold text-primaryText">Home Builder</span>
          </div>
        </div>

        {/* Center — Device Selector */}
        <div className="flex items-center gap-1 bg-muted rounded-lgToken p-1">
          {DEVICE_OPTIONS.map((option) => (
            <button
              key={option.value}
              onClick={() => onDeviceChange(option.value)}
              title={option.label}
              className={`px-3 py-1 rounded-mdToken text-xs font-medium transition-all duration-150 ${
                deviceMode === option.value
                  ? 'bg-secondaryBackground shadow-softToken text-primaryText'
                  : 'text-secondaryText hover:text-primaryText'
              }`}
            >
              {option.icon} {option.label}
            </button>
          ))}
        </div>

        {/* Right */}
        <div className="flex items-center gap-2 flex-1 justify-end">
          {onSave && (
            <Button
              variant="primary"
              size="sm"
              className="gap-1.5"
              onClick={onSave}
              disabled={saving || loadingConfig}
              title="Salvar configuração no servidor"
            >
              {saving ? (
                <Spinner className="h-4 w-4" />
              ) : (
                <Save size={14} />
              )}
              Salvar
            </Button>
          )}
          <Button
            variant="ghost"
            size="sm"
            className="gap-1.5 text-secondaryText"
            onClick={onReset}
            title="Resetar para o padrão"
            disabled={loadingConfig}
          >
            <RefreshCw size={14} />
            Resetar
          </Button>
          <Button
            variant="ghost"
            size="sm"
            className="gap-1.5 text-secondaryText"
            onClick={handleImport}
            disabled={loadingConfig}
          >
            <Upload size={14} />
            Importar
          </Button>
          <Button
            variant="secondary"
            size="sm"
            className="gap-1.5"
            onClick={handleExport}
            disabled={loadingConfig}
          >
            <Download size={14} />
            Exportar JSON
          </Button>
          <Button
            variant="primary"
            size="sm"
            className="gap-1.5"
            onClick={() => setPreviewOpen(true)}
            disabled={loadingConfig}
          >
            <Eye size={14} />
            Preview
          </Button>
        </div>
      </header>

      {/* Preview Dialog */}
      <Dialog open={previewOpen} onOpenChange={setPreviewOpen}>
        <DialogContent className="max-w-[95vw] w-full h-[90vh] flex flex-col p-0 gap-0">
          <DialogHeader className="px-6 py-4 border-b border-borderDefault flex-shrink-0">
            <div className="flex items-center justify-between">
              <DialogTitle className="text-base font-semibold">
                Preview da Home
              </DialogTitle>
              <div className="flex items-center gap-1 bg-muted rounded-lgToken p-1 mr-8">
                {DEVICE_OPTIONS.map((option) => (
                  <button
                    key={option.value}
                    onClick={() => setPreviewDevice(option.value)}
                    title={option.label}
                    className={`px-3 py-1 rounded-mdToken text-xs font-medium transition-all duration-150 ${
                      previewDevice === option.value
                        ? 'bg-secondaryBackground shadow-softToken text-primaryText'
                        : 'text-secondaryText hover:text-primaryText'
                    }`}
                  >
                    {option.icon} {option.label}
                  </button>
                ))}
              </div>
            </div>
          </DialogHeader>

          <div className="flex-1 overflow-x-hidden overflow-y-auto bg-primaryBackground p-4 sm:p-6">
            <div className={`mx-auto w-full min-w-0 ${previewWidthClass} transition-all duration-300`}>
              <HomeRenderer config={config} previewDevice={previewDevice} />
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </>
  )
}
