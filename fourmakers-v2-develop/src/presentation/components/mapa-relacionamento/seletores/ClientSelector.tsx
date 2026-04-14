import { memo } from 'react'
import type { ClienteMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { AutocompleteClienteMapa } from './AutocompleteClienteMapa'

export interface ClientSelectorProps {
  selectedCode: string
  /** Nome do cliente para exibição (dropdown e tela). Quando informado, evita mostrar código no lugar do nome. */
  selectedName?: string
  /** Recebe código e, quando disponível, o cliente completo (ex.: seleção a partir da busca). */
  onSelect: (code: string, cliente?: ClienteMapaRelacionamento) => void
  onNameChange?: (name: string) => void
  variant?: 'header' | 'hero'
  clientes?: ClienteMapaRelacionamento[]
}

function ClientSelectorInner({
  selectedCode,
  selectedName,
  onSelect,
  onNameChange,
  variant = 'header',
  clientes = [],
}: ClientSelectorProps) {
  return (
    <AutocompleteClienteMapa
      selectedCode={selectedCode}
      selectedName={selectedName}
      onSelect={onSelect}
      onNameChange={onNameChange}
      variant={variant}
      clientes={clientes}
    />
  )
}

export const ClientSelector = memo(ClientSelectorInner)
