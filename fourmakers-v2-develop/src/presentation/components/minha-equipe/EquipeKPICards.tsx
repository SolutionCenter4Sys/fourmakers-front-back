import { StatCard } from '@presentation/components/common/StatCard'
import { Users, AlertCircle, TrendingUp } from '@/components/ui/system-icons'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'

interface EquipeKPICardsProps {
  kpis: MinhaEquipeKPIs
  isLoading?: boolean
}

export const EquipeKPICards = ({ kpis, isLoading }: EquipeKPICardsProps) => {
  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-32 bg-muted animate-pulse rounded-lg" />
        ))}
      </div>
    )
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
      <StatCard
        title="Total de Colaboradores"
        value={kpis.totColaboradores.toString()}
        icon={Users}
        color="text-primary"
        bgColor="bg-primary/10"
      />

      <StatCard
        title="Skills Pendentes"
        value={kpis.totPendentesSkills.toString()}
        icon={AlertCircle}
        color="text-warning"
        bgColor="bg-warning/10"
      />

      <StatCard
        title="Aderência Média"
        value="Em Breve"
        icon={TrendingUp}
        color="text-muted-foreground"
        bgColor="bg-muted/10"
      />
    </div>
  )
}
