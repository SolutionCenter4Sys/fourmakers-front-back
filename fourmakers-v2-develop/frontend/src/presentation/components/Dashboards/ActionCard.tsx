import { Card, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import bgFfImage from '@/assets/home_assets/royal/bg_ff.png'

interface ActionCardProps {
  title: string
  description?: string
  buttonText: string
  onButtonClick: () => void
}

export const ActionCard = ({ title, description, buttonText, onButtonClick }: ActionCardProps) => {
  return (
    <Card
      className="border-0 overflow-hidden rounded-lgToken shadow-softToken relative"
      style={{
        backgroundImage: `url(${bgFfImage})`,
        backgroundSize: 'cover',
        backgroundPosition: 'center',
      }}
    >
      {/* Overlay com gradiente */}
      
      <CardContent className="p-8 relative z-10 flex flex-col justify-between h-full">
        <h3 className="text-2xl font-bold text-white mb-3">
          {title}
        </h3>
        {description && (
          <p className="text-base text-white/90 mb-4">
            {description}
          </p>
        )}
        <Button
          variant="primary"
          className="rounded-pillToken"
          onClick={onButtonClick}
        >
          {buttonText}
        </Button>
      </CardContent>
    </Card>
  )
}

