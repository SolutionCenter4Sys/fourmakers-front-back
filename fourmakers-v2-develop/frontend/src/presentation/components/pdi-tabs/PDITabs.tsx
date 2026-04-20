import { useNavigate } from 'react-router-dom'
import { useAppSelector } from '@app/store/hooks'
import { hasAccessToFeature } from '@shared/utils/accessUtils'
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { User, Users, TrendingUp } from '@/components/ui/system-icons'

interface PDITabsProps {
  currentTab: 'minha-jornada' | 'minha-equipe' | 'orquestracao'
}

export const PDITabs = ({ currentTab }: PDITabsProps) => {
  const navigate = useNavigate()
  const { user } = useAppSelector((state) => state.auth)

  // Verificar regras de acesso usando função utilitária
  const temMinhaEquipe = hasAccessToFeature(
    user?.funcionalidadeSistema,
    'MINHA_EQUIPE',
  )

  const temMinhaEquipeOrquestracao = hasAccessToFeature(
    user?.funcionalidadeSistema,
    'MINHA_EQUIPE_ORQUESTRACAO',
  )

  // Só mostrar o componente se tiver pelo menos uma das regras
  const deveMostrarTabs = temMinhaEquipe || temMinhaEquipeOrquestracao

  // Se não tiver acesso, não renderizar nada
  if (!deveMostrarTabs) {
    return null
  }

  const handleTabChange = (value: string) => {
    const routes: Record<string, string> = {
      'minha-jornada': '/pdijornada',
      'minha-equipe': '/pdiequipe',
      'orquestracao': '/pdiorquestracao',
    }

    const route = routes[value]
    if (route) {
      navigate(route)
    }
  }

  return (
    <div className="mb-6">
      <Tabs value={currentTab} onValueChange={handleTabChange}>
        <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
          {/* Minha Jornada - sempre visível (acesso padrão) */}
          <TabsTrigger
            value="minha-jornada"
            className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
          >
            <User className="h-4 w-4" />
            <span>Minha Jornada</span>
          </TabsTrigger>

          {/* Minha Equipe - só mostra se tiver MINHA_EQUIPE */}
          {temMinhaEquipe && (
            <TabsTrigger
              value="minha-equipe"
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Users className="h-4 w-4" />
              <span>Minha Equipe</span>
            </TabsTrigger>
          )}

          {/* Orquestração - só mostra se tiver MINHA_EQUIPE_ORQUESTRACAO */}
          {temMinhaEquipeOrquestracao && (
            <TabsTrigger
              value="orquestracao"
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <TrendingUp className="h-4 w-4" />
              <span>Orquestração</span>
            </TabsTrigger>
          )}
        </TabsList>
      </Tabs>
    </div>
  )
}
