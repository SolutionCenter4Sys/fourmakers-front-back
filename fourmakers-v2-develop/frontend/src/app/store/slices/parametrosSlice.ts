import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import { container } from '@core/di/container'
import { ListarParametrosConfiguracaoUseCase } from '@domain/usecases/ListarParametrosConfiguracaoUseCase'
import type { ParametroConfiguracao } from '@domain/entities/ParametroConfiguracao'

interface ParametrosState {
  parametros: ParametroConfiguracao[]
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
}

const initialState: ParametrosState = {
  parametros: [],
  status: 'idle',
  error: null,
}

export const fetchParametrosConfiguracao = createAsyncThunk(
  'parametros/fetchParametrosConfiguracao',
  async (token: string) => {
    const useCase = container.resolve(ListarParametrosConfiguracaoUseCase)
    const response = await useCase.execute(token)
    return response.retorno || []
  },
)

const parametrosSlice = createSlice({
  name: 'parametros',
  initialState,
  reducers: {
    clearParametros: (state) => {
      state.parametros = []
      state.status = 'idle'
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchParametrosConfiguracao.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(fetchParametrosConfiguracao.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.parametros = action.payload
      })
      .addCase(fetchParametrosConfiguracao.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro ao carregar parâmetros de configuração'
      })
  },
})

// Helper function para buscar um parâmetro específico
export const getParametroByCode = (parametros: ParametroConfiguracao[], codigo: string): string | null => {
  const parametro = parametros.find(p => p.codigoParametro === codigo)
  return parametro?.valorParametro || null
}

// Helper function para verificar se um parâmetro booleano é true
export const isParametroEnabled = (parametros: ParametroConfiguracao[], codigo: string): boolean => {
  const valor = getParametroByCode(parametros, codigo)
  return valor === 'true'
}

export const { clearParametros } = parametrosSlice.actions

export default parametrosSlice.reducer

