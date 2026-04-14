import { Card, CardContent } from '@/components/ui/card'
import foursysHumanImage from '@/assets/home_assets/royal/foursys_human.png'

interface WelcomeCardProps {
  userName: string
}

export const WelcomeCard = ({ userName }: WelcomeCardProps) => {
  return (
    <Card className="bg-gradient-to-r from-[#9A1BFF] via-[#7B1CE5] to-[#4F46E5] border-0 overflow-hidden rounded-lgToken shadow-softToken">
      <CardContent className="p-6 flex items-center justify-between">
        <div className="flex-1 text-white">
          <h3 className="text-2xl font-bold mb-1">
            Olá, {userName}!
          </h3>
          <p className="text-base text-white/90">
            Seja bem-vindo ao Fourmakers
          </p>
        </div>
        <div className="hidden md:block ml-6 flex-shrink-0">
          <img
            src={foursysHumanImage}
            alt="Avatar corporativo"
            className="w-24 h-24 object-contain"
          />
        </div>
      </CardContent>
    </Card>
  )
}

