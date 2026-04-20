import { Card, CardHeader, CardTitle } from '@/components/ui/card'

interface ShortcutCardProps {
  icon: React.ReactNode
  label: string
  onClick: () => void
}

export const ShortcutCard = ({ icon, label, onClick }: ShortcutCardProps) => {
  return (
    <Card
      className="cursor-pointer hover:shadow-cardHoverToken transition-all duration-200 rounded-lgToken shadow-softToken"
      onClick={onClick}
    >
      <CardHeader className="flex flex-col ">
        <div className="bg-primary/10 rounded-lg">
          {icon}
        </div>
        <CardTitle className="text-sm font-semibold ">
          {label}
        </CardTitle>
      </CardHeader>
    </Card>
  )
}

