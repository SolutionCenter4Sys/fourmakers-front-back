import { useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { selectChartsData } from '@app/store/slices/skillsDashboardSlice'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip'
import { ChevronLeft, ChevronRight, BarChart2, PieChart as PieIcon, Filter } from 'lucide-react'
import type { ChartDataPoint } from '@app/store/slices/skillsDashboardSlice'
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip as RechartsTooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts'

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8']
const DEFAULT_CHART_COLOR = '#6b7280'

interface ChartConfig {
  title: string
  data: ChartDataPoint[]
  type: 'bar' | 'pie'
  color?: string
}

export const ChartCarousel = () => {
  const chartsData = useAppSelector((state) => selectChartsData(state))
  const [currentIndex, setCurrentIndex] = useState(0)

  // Cor do hover baseada no background padrão do design system
  // Usa uma cor neutra que funciona bem em ambos os temas (light e dark)
  // Baseada no surfaceSubtle: light mode #F9FAFB, dark mode #020617
  // Nota: Calculado a cada render para reagir a mudanças de tema
  const isDarkMode = typeof document !== 'undefined' && document.documentElement.classList.contains('dark')
  const hoverFillColor = isDarkMode ? 'rgba(30, 41, 59, 0.25)' : 'rgba(148, 163, 184, 0.15)'

  const charts: ChartConfig[] = [
    { title: 'Skills Adicionadas', data: chartsData.barAdded, type: 'bar', color: '#3b82f6' },
    { title: 'Skills Sugeridas', data: chartsData.barSuggested, type: 'bar', color: '#6366f1' },
    { title: 'Skills no PDI', data: chartsData.barPDI, type: 'bar', color: '#10b981' },
    { title: 'Skills Rejeitadas', data: chartsData.barRejected, type: 'bar', color: '#f43f5e' },
    { title: '% Uso da Plataforma', data: chartsData.pieUsage, type: 'pie' },
  ]

  const nextSlide = () => setCurrentIndex((prev) => (prev + 1) % charts.length)
  const prevSlide = () => setCurrentIndex((prev) => (prev - 1 + charts.length) % charts.length)

  const currentChart = charts[currentIndex]

  const getTooltipSuffix = (title: string) => {
    switch (title) {
      case 'Skills Adicionadas':
        return 'skills adicionadas'
      case 'Skills Sugeridas':
        return 'skills sugeridas'
      case 'Skills no PDI':
        return 'skills no PDI'
      case 'Skills Rejeitadas':
        return 'skills rejeitadas'
      default:
        return 'skills'
    }
  }

  const CustomTooltip = (props: any) => {
    const { active, payload, label } = props
    if (active && payload?.length) {
      const value = payload[0]?.value ?? 0
      const chartColor = currentChart.color ?? DEFAULT_CHART_COLOR
      const labelStr = label != null ? String(label) : ''
      return (
        <div className="rounded-lgToken border-borderSoft bg-surfaceElevated p-3 shadow-softToken">
          <p className="mb-1 text-sm font-semibold text-primaryText">{labelStr}</p>
          <p className="text-sm font-medium" style={{ color: chartColor }}>
            {value} {getTooltipSuffix(currentChart.title)}
          </p>
        </div>
      )
    }
    return null
  }

  return (
    <Card className="relative mb-6 overflow-hidden">
      <CardHeader className="mb-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            {currentChart.type === 'bar' ? (
              <BarChart2 className="text-secondaryText" size={20} />
            ) : (
              <PieIcon className="text-secondaryText" size={20} />
            )}
            <CardTitle className="text-xl font-bold text-primaryText">{currentChart.title}</CardTitle>
            {currentChart.type === 'bar' && (
              <span className="ml-2 rounded-pillToken bg-surfaceSubtle px-2 py-0.5 text-xs font-normal text-secondaryText">
                Top 10
              </span>
            )}
          </div>

          <div className="flex items-center gap-4">
            {currentChart.type === 'bar' && (
              <TooltipProvider>
                <Tooltip>
                  <TooltipTrigger asChild>
                    <button
                      disabled
                      className="flex items-center gap-1 rounded-pillToken bg-primarySoft px-3 py-1.5 text-sm font-medium text-primary transition-colors hover:bg-primarySoft/80 disabled:cursor-not-allowed disabled:opacity-60"
                    >
                      <Filter size={14} />
                      Ver Mais
                    </button>
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>Em Breve</p>
                  </TooltipContent>
                </Tooltip>
              </TooltipProvider>
            )}

            <div className="flex gap-2">
              {charts.map((_, idx) => (
                <button
                  key={idx}
                  onClick={() => setCurrentIndex(idx)}
                  className={`h-2 rounded-pillToken transition-all duration-300 ${
                    idx === currentIndex ? 'w-6 bg-primary' : 'w-2 bg-borderDefault'
                  }`}
                />
              ))}
            </div>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        <div className="relative flex h-[500px] w-full items-center justify-center">
          {/* Mensagem "Em Breve" para o último gráfico de pizza */}
          {currentChart.title === '% Uso da Plataforma' ? (
            <div className="absolute inset-0 flex flex-col items-center justify-center bg-surfaceElevated/50 backdrop-blur-sm">
              <p className="text-xl font-semibold text-secondaryText">Em Breve</p>
              <p className="mt-2 text-sm text-secondaryText/80">Este gráfico estará disponível em breve</p>
            </div>
          ) : currentChart.data.length === 0 ? (
            <div className="absolute inset-0 flex flex-col items-center justify-center bg-surfaceElevated/50 backdrop-blur-sm">
              <p className="font-medium text-secondaryText">Sem dados para exibir neste gráfico</p>
              {currentChart.type === 'bar' && (
                <p className="mt-2 text-sm text-secondaryText/80">Tente limpar o filtro de evento</p>
              )}
            </div>
          ) : (
            <ResponsiveContainer width="100%" height="100%">
              {currentChart.type === 'bar' ? (
                <BarChart
                  layout="vertical"
                  data={currentChart.data}
                  margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
                >
                  <CartesianGrid strokeDasharray="3 3" horizontal={false} vertical stroke="#e5e7eb" />
                  <XAxis
                    type="number"
                    allowDecimals={false}
                    tick={{ fill: '#6b7280', fontSize: 12 }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <YAxis
                    dataKey="name"
                    type="category"
                    width={100}
                    tick={{ fill: '#6b7280', fontSize: 12 }}
                    axisLine={false}
                    tickLine={false}
                  />
                  <RechartsTooltip 
                    content={CustomTooltip} 
                    cursor={{ fill: hoverFillColor }} 
                  />
                  <Bar
                    dataKey="value"
                    fill={currentChart.color}
                    radius={[0, 4, 4, 0]}
                    barSize={30}
                  />
                </BarChart>
              ) : (
                <PieChart>
                  <Pie
                    data={currentChart.data}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={100}
                    paddingAngle={5}
                    dataKey="value"
                    label={(props: { name?: string; value?: number }) => {
                      const name = props.name ?? ''
                      const value = props.value ?? 0
                      return `${name}: ${value}%`
                    }}
                  >
                    {currentChart.data.map((_, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <RechartsTooltip
                    formatter={(value: number | undefined, _name?: any, _item?: any, _index?: any, _payload?: any) => {
                      return `${value ?? 0}%`
                    }}
                    contentStyle={{
                      borderRadius: '8px',
                      border: 'none',
                      boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                    }}
                  />
                </PieChart>
              )}
            </ResponsiveContainer>
          )}
        </div>

        {/* Navigation Buttons */}
        <button
          onClick={prevSlide}
          className="absolute left-4 top-1/2 -translate-y-1/2 transform rounded-pillToken border-borderSoft bg-surfaceElevated/80 p-3 text-secondaryText shadow-softToken backdrop-blur-sm transition-all hover:bg-surfaceElevated hover:text-primary"
        >
          <ChevronLeft size={24} />
        </button>
        <button
          onClick={nextSlide}
          className="absolute right-4 top-1/2 -translate-y-1/2 transform rounded-pillToken border-borderSoft bg-surfaceElevated/80 p-3 text-secondaryText shadow-softToken backdrop-blur-sm transition-all hover:bg-surfaceElevated hover:text-primary"
        >
          <ChevronRight size={24} />
        </button>
      </CardContent>
    </Card>
  )
}

