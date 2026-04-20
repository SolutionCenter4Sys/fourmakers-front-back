import { Card, CardContent } from '@/components/ui/card'
import { Calendar, User, CheckSquare } from 'lucide-react'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'

interface ItemAcaoCardProps {
  item: ItemAgendaGestor
}

export function ItemAcaoCard({ item }: ItemAcaoCardProps) {
  const dataFormatada = format(new Date(item.data), 'dd/MM/yyyy HH:mm', { locale: ptBR })

  return (
    <Card className="hover:shadow-sm transition-shadow border-l-4 border-l-warning rounded-lg">
      <CardContent className="p-3">
        <div className="space-y-2">
          <div className="flex items-start justify-between gap-2">
            <div className="flex items-start gap-2 flex-1 min-w-0">
              <CheckSquare className="h-4 w-4 text-warning shrink-0 mt-0.5" />
              <h4 className="font-semibold text-sm line-clamp-2 flex-1">{item.titulo}</h4>
            </div>
          </div>

          <div className="flex flex-col gap-1.5 text-xs text-muted-foreground">
            <div className="flex items-center gap-1.5">
              <Calendar className="h-3 w-3 shrink-0" />
              <span className="truncate">Prazo: {dataFormatada}</span>
            </div>
            <div className="flex items-center gap-1.5">
              <User className="h-3 w-3 shrink-0" />
              <span className="truncate">{item.responsavel}</span>
            </div>
          </div>

          {item.descricao && (
            <p className="text-xs text-muted-foreground line-clamp-2 mt-2">{item.descricao}</p>
          )}
        </div>
      </CardContent>
    </Card>
  )
}
