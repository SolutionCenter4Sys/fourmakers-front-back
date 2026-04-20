import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { clientesCampanha } from '@shared/data/clientes'
import { escritoriosEnderecos } from '@shared/data/escritorios'
import { frequenciaMap } from '@shared/data/frequencia'

export interface ClienteCampanha {
  nome: string
  endereco: string
}

interface ClienteCampanhaState {
  clienteCampanha: ClienteCampanha[]
  escritoriosEnderecos: Record<string, string>
  frequenciaMap: Record<number, string>
}

const initialState: ClienteCampanhaState = {
  clienteCampanha: clientesCampanha,
  escritoriosEnderecos,
  frequenciaMap,
}

const clienteCampanhaSlice = createSlice({
  name: 'clienteCampanha',
  initialState,
  reducers: {
    setClienteCampanha: (state, action: PayloadAction<ClienteCampanha[]>) => {
      state.clienteCampanha = action.payload
    },
  },
})

export const { setClienteCampanha } = clienteCampanhaSlice.actions

export default clienteCampanhaSlice.reducer

