import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type { MenuResource } from '@domain/entities/MenuResource'
import { GetMenuResourcesUseCase } from '@domain/usecases/GetMenuResourcesUseCase'

export interface MenuState {
  menuItems: MenuResource[]
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: MenuState = {
  menuItems: [],
  status: 'idle',
  error: null,
}

export const fetchMenuData = createAsyncThunk(
  'menu/fetchMenuData',
  async (_, { getState }) => {
    const state = getState() as { auth: { token: string | null } }
    const token = state.auth.token

    if (!token) {
      throw new Error('Token de autenticação não encontrado. Faça login novamente.')
    }

    const useCase = container.resolve(GetMenuResourcesUseCase)
    return useCase.execute(token)
  },
)

const menuSlice = createSlice({
  name: 'menu',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchMenuData.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(fetchMenuData.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.menuItems = action.payload
      })
      .addCase(fetchMenuData.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro desconhecido ao carregar dados do menu.'
      })
  },
})

export default menuSlice.reducer

