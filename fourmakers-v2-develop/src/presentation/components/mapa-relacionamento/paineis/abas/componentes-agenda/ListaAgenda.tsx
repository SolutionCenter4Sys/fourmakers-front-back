import { ItemAgendaCard } from './ItemAgendaCard'
import { ItemInteracaoCard } from './ItemInteracaoCard'
import { ItemAcaoCard } from './ItemAcaoCard'
import type { ItemAgendaGestor, TipoItemAgenda } from '@domain/entities/AgendaGestor'

interface ListaAgendaProps {
  items: ItemAgendaGestor[]
  tipo: TipoItemAgenda
}

export function ListaAgenda({ items, tipo }: ListaAgendaProps) {
  if (items.length === 0) {
    return (
      <p className="text-sm text-muted-foreground text-center py-6">
        Sem registros recentes
      </p>
    )
  }

  // Ordenar por data descendente
  const itemsOrdenados = [...items].sort(
    (a, b) => new Date(b.data).getTime() - new Date(a.data).getTime(),
  )

  return (
    <div className="space-y-2">
      {itemsOrdenados.map((item) => {
        switch (tipo) {
          case 'agenda':
            return <ItemAgendaCard key={item.id} item={item} />
          case 'interacao':
            return <ItemInteracaoCard key={item.id} item={item} />
          case 'acao':
            return <ItemAcaoCard key={item.id} item={item} />
          default:
            return null
        }
      })}
    </div>
  )
}
