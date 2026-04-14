import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'

import type { Iniciativa } from '@domain/entities/Vcx360'

interface AbaIniciativasProps {
  iniciativas: Iniciativa[]
}

export function AbaIniciativas({ iniciativas }: AbaIniciativasProps) {
  if (iniciativas.length === 0) {
    return <p className="text-sm text-muted-foreground">Nenhuma iniciativa registrada</p>
  }

  return (
    <div className="space-y-2">
      {iniciativas.map((item) => (
        <Card key={item.id} className="p-4">
          <div className="flex items-center justify-between">
            <p className="text-sm font-semibold">{item.titulo}</p>
            <Badge>{item.status}</Badge>
          </div>
          <p className="text-xs text-muted-foreground">{item.objetivoKpi}</p>
          <p className="text-xs text-muted-foreground">Tema: {item.tema}</p>
        </Card>
      ))}
    </div>
  )
}
