import { useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import {
  selectGraficoFocoCategoria,
  selectFiltros,
  selectEvidenciasPorIndicador,
} from '@app/store/slices/dashboardComercialSlice'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { BarChart2 } from '@/components/ui/system-icons'
// Modal de drilldown inativo nesta entrega — funcionalidade ainda está sendo validada.
import { DrilldownSheet } from './DrilldownSheet'
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip as RechartsTooltip,
  ResponsiveContainer,
} from 'recharts'
import type { IdIndicador } from '@shared/types/dashboardComercialTypes'
import type { SerieGrafico } from '@shared/types/dashboardComercialTypes'
import { COMO_CALCULAMOS } from '@shared/utils/dashboardComercialMocks'

/** Cores alinhadas ao Dashboard Jornadas (ChartCarousel) */
const COR_BARRAS = '#3b82f6'
const COR_BORDA_GRID = '#e5e7eb'
const COR_TEXTO_SECUNDARIO = '#6b7280'

interface GraficoBarraProps {
  titulo: string
  dados: SerieGrafico[]
  idIndicador: IdIndicador
  microResumo?: string
  onAbrirDrilldown: (id: IdIndicador, titulo: string) => void
  altura?: number
  emBreve?: boolean
  /** Quando true, card não abre drilldown (entrega em validação). */
  desabilitado?: boolean
}

function GraficoBarra({
  titulo,
  dados,
  idIndicador,
  microResumo,
  onAbrirDrilldown,
  altura = 280,
  emBreve = false,
  desabilitado = false,
}: GraficoBarraProps) {
  const handleClickCard = () => {
    if (desabilitado) return
    onAbrirDrilldown(idIndicador, titulo)
  }

  const textoVazio = emBreve ? 'Em Breve' : 'Sem dados para exibir'
  const exibirGrafico = !emBreve && dados.length > 0
  const isDarkMode =
    typeof document !== 'undefined' && document.documentElement.classList.contains('dark')
  const hoverFillColor = isDarkMode
    ? 'rgba(30, 41, 59, 0.25)'
    : 'rgba(148, 163, 184, 0.15)'

  return (
    <button
      type="button"
      onClick={handleClickCard}
      className={`w-full rounded-lg border bg-card text-left shadow focus:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 ${desabilitado ? 'cursor-not-allowed opacity-90' : 'cursor-pointer transition-shadow hover:shadow-md'}`}
      aria-label={desabilitado ? undefined : `Ver detalhes do indicador: ${titulo}`}
    >
      <Card className="overflow-hidden border-0 shadow-none">
        <CardHeader className="pb-2">
          <div className="flex items-center gap-2">
            <BarChart2 className="text-muted-foreground" size={20} />
            <CardTitle className="text-lg font-bold text-foreground">{titulo}</CardTitle>
          </div>
        </CardHeader>
        <CardContent>
          <div
            className="w-full cursor-pointer"
            style={{ height: altura }}
            onClick={(e) => {
              e.stopPropagation()
              handleClickCard()
            }}
          >
            {!exibirGrafico ? (
              <div className="flex h-full items-center justify-center text-muted-foreground">
                {textoVazio}
              </div>
            ) : (
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  layout="vertical"
                  data={dados}
                  margin={{ top: 10, right: 20, left: 10, bottom: 5 }}
                >
                  <CartesianGrid strokeDasharray="3 3" horizontal={false} vertical stroke={COR_BORDA_GRID} />
                  <XAxis
                    type="number"
                    allowDecimals={false}
                    tick={{ fill: COR_TEXTO_SECUNDARIO, fontSize: 12 }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <YAxis
                    dataKey="name"
                    type="category"
                    width={90}
                    tick={{ fill: COR_TEXTO_SECUNDARIO, fontSize: 12 }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <RechartsTooltip
                    cursor={{ fill: hoverFillColor }}
                    content={({ active, payload, label }) => {
                      if (active && payload?.length) {
                        const value = payload[0]?.value ?? 0
                        return (
                          <div className="rounded-lgToken border-borderSoft bg-surfaceElevated p-3 shadow-softToken">
                            <p className="mb-1 text-sm font-semibold text-primaryText">{label}</p>
                            <p className="text-sm font-medium" style={{ color: COR_BARRAS }}>{value}</p>
                          </div>
                        )
                      }
                      return null
                    }}
                  />
                  <Bar
                    dataKey="value"
                    fill={COR_BARRAS}
                    radius={[0, 4, 4, 0]}
                    barSize={30}
                  />
                </BarChart>
              </ResponsiveContainer>
            )}
          </div>
          {microResumo && (
            <p className="mt-3 text-xs text-muted-foreground">{microResumo}</p>
          )}
        </CardContent>
      </Card>
    </button>
  )
}

export function GraficosRadarComercial() {
  const user = useAppSelector((state) => state.auth.user)
  const graficoFoco = useAppSelector(selectGraficoFocoCategoria)
  const filtros = useAppSelector(selectFiltros)
  const getEvidencias = useAppSelector(selectEvidenciasPorIndicador)

  const [drilldownAberto, setDrilldownAberto] = useState(false)
  const [indicadorAberto, setIndicadorAberto] = useState<{
    id: IdIndicador
    titulo: string
  } | null>(null)

  /** Drilldown inativo nesta entrega — ainda está sendo validado. Reativar quando aprovado. */
  const DRILLDOWN_HABILITADO = false

  const abrirDrilldown = (id: IdIndicador, titulo: string) => {
    if (!DRILLDOWN_HABILITADO) return
    logUserAction('DashboardComercial', 'AbrirDrilldownIndicador', { idIndicador: id, titulo }, user)
    setIndicadorAberto({ id, titulo })
    setDrilldownAberto(true)
  }

  const handleCloseDrilldown = (open: boolean) => {
    setDrilldownAberto(open)
    if (!open) setIndicadorAberto(null)
  }

  return (
    <>
      <div className="mb-6 grid grid-cols-1 gap-6 lg:grid-cols-2">
        <div className="lg:col-span-2">
          <GraficoBarra
            titulo="Foco por Categoria"
            dados={graficoFoco}
            idIndicador="foco-por-categoria"
            onAbrirDrilldown={abrirDrilldown}
            altura={400}
            desabilitado={!DRILLDOWN_HABILITADO}
          />
        </div>
        <GraficoBarra
          titulo="Níveis Estratégicos Acessados"
          dados={[]}
          idIndicador="niveis-estrategicos"
          onAbrirDrilldown={abrirDrilldown}
          emBreve
          desabilitado={!DRILLDOWN_HABILITADO}
        />
        <GraficoBarra
          titulo="Dedicação por Objetivo"
          dados={[]}
          idIndicador="dedicacao-por-objetivo"
          microResumo="Distribuição do tempo e ações por objetivo comercial no período."
          onAbrirDrilldown={abrirDrilldown}
          emBreve
          desabilitado={!DRILLDOWN_HABILITADO}
        />
      </div>

      {/* Drilldown inativo nesta entrega — ainda está sendo validado. */}
      {indicadorAberto && (
        <DrilldownSheet
          open={drilldownAberto}
          onOpenChange={handleCloseDrilldown}
          titulo={indicadorAberto.titulo}
          comoCalculamos={COMO_CALCULAMOS[indicadorAberto.id]}
          dadosEvidencia={getEvidencias(indicadorAberto.id)}
          filtrosAtivos={filtros}
          emBreve
        />
      )}
    </>
  )
}
