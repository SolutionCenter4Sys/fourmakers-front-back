import { useAppSelector } from '@app/store/hooks'
import { selectKPIData, selectBigNumbers } from '@app/store/slices/skillsDashboardSlice'
import { TrendingUp } from 'lucide-react'

interface KPICardProps {
  title: string
  value: number
  icon: React.ReactNode
  gradientClass: string
}

const KPICard = ({ title, value, icon, gradientClass }: KPICardProps) => (
  <div
    className={`flex h-32 flex-col justify-between rounded-lgToken p-lg text-white shadow-softToken transition-transform hover:scale-105 hover:shadow-cardHoverToken duration-200 ${gradientClass}`}
  >
    <div className="flex items-start justify-between">
      <h3 className="text-sm font-medium uppercase tracking-wide opacity-90">{title}</h3>
      <div className="opacity-80">{icon}</div>
    </div>
    <p className="mt-2 text-4xl font-bold">{value}</p>
  </div>
)

export const BigNumbers = () => {
  const kpi = useAppSelector((state) => selectKPIData(state))
  const bigNumbers = useAppSelector((state) => selectBigNumbers(state))
  const effective = {
    added: bigNumbers?.skillsAdicionadas ?? kpi.added,
    suggested: bigNumbers?.skillsSugeridas ?? kpi.suggested,
    pdi: bigNumbers?.adicionadasPDI ?? kpi.pdi,
    rejected: bigNumbers?.skillsRejeitadas ?? kpi.rejected,
  }

  return (
    <div className="mb-6 grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
      <KPICard
        title="Skills Adicionadas"
        value={effective.added}
        icon={<TrendingUp size={24} />}
        gradientClass="bg-gradient-to-br from-blue-500 to-blue-600"
      />
      <KPICard
        title="Skills Sugeridas"
        value={effective.suggested}
        icon={<TrendingUp size={24} />}
        gradientClass="bg-gradient-to-br from-indigo-500 to-indigo-600"
      />
      <KPICard
        title="Skills no PDI"
        value={effective.pdi}
        icon={<TrendingUp size={24} />}
        gradientClass="bg-gradient-to-br from-emerald-500 to-emerald-600"
      />
      <KPICard
        title="Skills Rejeitadas"
        value={effective.rejected}
        icon={<TrendingUp size={24} />}
        gradientClass="bg-gradient-to-br from-rose-500 to-rose-600"
      />
    </div>
  )
}

