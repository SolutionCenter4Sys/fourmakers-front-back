import { Card, CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Calendar, User, MapPin, Video } from 'lucide-react'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'

import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'

interface ItemAgendaCardProps {
  item: ItemAgendaGestor
}

export function ItemAgendaCard({ item }: ItemAgendaCardProps) {
  const dataFormatada = format(new Date(item.data), 'dd/MM/yyyy HH:mm', { locale: ptBR })

  return (
    <Card className="hover:shadow-sm transition-shadow rounded-lg">
      <CardContent className="p-3">
        <div className="space-y-2">
          <div className="flex items-start justify-between gap-2">
            <h4 className="font-semibold text-sm line-clamp-2 flex-1">{item.titulo}</h4>
          </div>

          {item.tipoInteracao && (
            <Badge variant="secondary" className="text-xs">
              {item.tipoInteracao}
            </Badge>
          )}

          <div className="flex flex-col gap-1.5 text-xs text-muted-foreground">
            <div className="flex items-center gap-1.5">
              <Calendar className="h-3 w-3 shrink-0" />
              <span className="truncate">{dataFormatada}</span>
            </div>
            <div className="flex items-center gap-1.5">
              <User className="h-3 w-3 shrink-0" />
              <span className="truncate">{item.responsavel}</span>
            </div>
            {item.localizacao && (
              <div className="flex items-center gap-1.5">
                <MapPin className="h-3 w-3 shrink-0" />
                <span className="truncate">{item.localizacao}</span>
              </div>
            )}
            {item.linkReuniao && (
              <div className="flex items-center gap-1.5">
                <Video className="h-3 w-3 shrink-0" />
                <a
                  href={item.linkReuniao}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-primary hover:underline truncate"
                >
                  Link da reunião
                </a>
              </div>
            )}
            {item.participantes && item.participantes > 0 && (
              <span className="text-xs">Participantes: {item.participantes}</span>
            )}
          </div>

          {item.descricao && (
            <p className="text-xs text-muted-foreground line-clamp-2 mt-2">{item.descricao}</p>
          )}
        </div>
      </CardContent>
    </Card>
  )
}
