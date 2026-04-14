import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import { BuscarAgendaGestorUseCase } from '@domain/usecases/BuscarAgendaGestorUseCase'
import type { AgendaGestorCompleto } from '@domain/entities/AgendaGestor'

export interface AgendaGestorState {
  agenda: AgendaGestorCompleto | null
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: AgendaGestorState = {
  agenda: null,
  status: 'idle',
  error: null,
}

export const buscarAgendaGestor = createAsyncThunk(
  'agendaGestor/buscar',
  async (
    {
      token,
      codInternoColaborador,
      dataInicio,
      dataFim,
    }: {
      token: string
      codInternoColaborador: string
      dataInicio?: string
      dataFim?: string
    },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(BuscarAgendaGestorUseCase)
      return await useCase.execute(token, codInternoColaborador, dataInicio, dataFim)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao buscar agenda do gestor',
      )
    }
  },
)

const agendaGestorSlice = createSlice({
  name: 'agendaGestor',
  initialState,
  reducers: {
    limparAgenda: (state) => {
      state.agenda = null
      state.status = 'idle'
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(buscarAgendaGestor.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(
        buscarAgendaGestor.fulfilled,
        (state, action: PayloadAction<AgendaGestorCompleto>) => {
          state.status = 'succeeded'
          state.agenda = action.payload
          state.error = null
        },
      )
      .addCase(buscarAgendaGestor.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
  },
})

export const { limparAgenda } = agendaGestorSlice.actions
export default agendaGestorSlice.reducer
