import type { PreviewDevice } from '@presentation/components/home-renderer/HomeRenderer'
import { useParametros } from '@presentation/hooks/useParametros'
import { CanalDenuncias } from '@presentation/components/Dashboards/CanalDenuncias'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { AlertCircle } from '@/components/ui/system-icons'

const gridClassSingle = 'grid grid-cols-1 gap-4 sm:gap-6 min-w-0'
const gridClassResponsive = 'grid grid-cols-1 xl:grid-cols-2 gap-4 sm:gap-6 xl:gap-8 min-w-0'

function CanalDenunciasPlaceholder({
  previewDevice,
  showParameterHints,
}: {
  previewDevice?: PreviewDevice
  showParameterHints?: boolean
}) {
  const singleColumn = previewDevice === 'mobile' || previewDevice === 'tablet'
  const gridClass = singleColumn ? gridClassSingle : gridClassResponsive
  return (
    <Card className="shadow-softToken rounded-lgToken opacity-70 w-full min-w-0 overflow-hidden">
      <CardHeader className="bg-muted rounded-t-lgToken p-4 sm:p-6">
        <CardTitle className="flex flex-col sm:flex-row items-start gap-3 min-w-0">
          <div className="p-2 bg-primary rounded-lgToken flex-shrink-0">
            <AlertCircle className="h-6 w-6 text-inverseText" />
          </div>
          <div className="min-w-0">
            <h2 className="text-lg sm:text-xl font-bold text-primaryText break-words">
              Área de Sugestões, Reclamações e Denúncias (Anônimas)
            </h2>
            <p className="text-sm text-secondaryText mt-1 font-normal break-words">
              Este espaço é destinado para que você registre sugestões, elogios, reclamações ou denúncias de forma totalmente anônima.
            </p>
          </div>
        </CardTitle>
      </CardHeader>
      <CardContent className="p-4 sm:p-6">
        <div className={gridClass}>
          <div className="space-y-3 min-w-0">
            <p className="text-sm text-secondaryText break-words">
              Sua manifestação será encaminhada diretamente ao setor responsável, garantindo:
            </p>
            <ul className="text-sm text-secondaryText space-y-1 list-disc list-inside break-words">
              <li>Anonimato completo</li>
              <li>Sigilo absoluto das informações enviadas</li>
              <li>Tratamento adequado e responsável</li>
              <li>Retorno das ações sempre que possível</li>
            </ul>
          </div>
          <div className="space-y-3 w-full min-w-0">
            <p className="text-sm font-semibold text-primaryText break-words">Descreva abaixo sua manifestação:</p>
            <Textarea
              disabled
              placeholder="Digite sua sugestão, reclamação ou denúncia aqui..."
              className="w-full min-w-0 min-h-[140px] rounded-lgToken resize-none opacity-60"
            />
            <div className="flex flex-wrap justify-between items-center gap-2">
              <p className="text-xs text-secondaryText">0/2000 caracteres</p>
              <Button disabled className="rounded-pillToken opacity-60 shrink-0">Enviar</Button>
            </div>
          </div>
        </div>
        {showParameterHints && (
          <div className="mt-4 flex flex-col sm:flex-row items-start sm:items-center gap-2 rounded-lgToken bg-amber-50 border border-amber-200 px-3 sm:px-4 py-2.5">
            <AlertCircle size={14} className="text-amber-600 flex-shrink-0" />
            <p className="text-xs text-amber-700 break-words min-w-0">
              Preview: configure o parâmetro{' '}
              <code className="font-mono bg-amber-100 px-1 rounded break-all">CANAL_DENUNCIA_CONFIGURACOES</code>{' '}
              para ativar este componente na home real.
            </p>
          </div>
        )}
      </CardContent>
    </Card>
  )
}

export function CanalDenunciasBlock({
  previewDevice,
  showParameterHints = false,
  productionMode = false,
}: {
  previewDevice?: PreviewDevice
  /** Exibir mensagem de configuração de parâmetro (apenas no home-builder). */
  showParameterHints?: boolean
  /**
   * Quando true (HomeNew), o bloco é ocultado completamente se o parâmetro
   * CANAL_DENUNCIA_CONFIGURACOES não estiver configurado para a organização.
   * Quando false (builder/preview), exibe sempre o placeholder para o admin poder gerenciar.
   */
  productionMode?: boolean
}) {
  const { getParametro } = useParametros()
  const hasConfig = getParametro('CANAL_DENUNCIA_CONFIGURACOES') !== null

  if (!hasConfig) {
    if (productionMode) return null
    return (
      <CanalDenunciasPlaceholder
        previewDevice={previewDevice}
        showParameterHints={showParameterHints}
      />
    )
  }

  return <CanalDenuncias previewDevice={previewDevice} />
}
