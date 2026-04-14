import { createContext } from 'react'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

export interface MapaRelacionamentoTreeContextValue {
  allNodes: NoMapaRelacionamento[]
}

export const MapaRelacionamentoTreeContext = createContext<MapaRelacionamentoTreeContextValue>({
  allNodes: [],
})
