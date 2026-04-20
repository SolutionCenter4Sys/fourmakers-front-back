import type { ReactNode } from 'react'
import { Calendar, MapPin, MessageSquare, Video } from 'lucide-react'

const CLASSE_ICONE_PADRAO = 'h-4 w-4 shrink-0'

/**
 * Retorna o ícone do tipo de interação para uso em cards e modais de agenda.
 * Reunião/Ligação = Video; Chat/Email = MessageSquare; Presencial = MapPin.
 * @param tipoInteracao Tipo retornado pela API (Reunião, Ligação, Chat, Email, Presencial)
 * @param className Classes CSS do ícone (ex.: 'h-5 w-5' para tamanho maior)
 */
export function getTipoInteracaoIcon(
  tipoInteracao?: string,
  className: string = CLASSE_ICONE_PADRAO
): ReactNode {
  const cn = className || CLASSE_ICONE_PADRAO
  switch (tipoInteracao) {
    case 'Reunião':
    case 'Ligação':
      return <Video className={cn} />
    case 'Chat':
    case 'Email':
      return <MessageSquare className={cn} />
    case 'Presencial':
      return <MapPin className={cn} />
    default:
      return <Calendar className={cn} />
  }
}
