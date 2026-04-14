import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type {
  ClienteMapaRelacionamento,
  DepartamentoMapaRelacionamento,
  GestorExterno,
  NoMapaRelacionamento,
  PerfilExterno,
} from '@domain/entities/MapaRelacionamento'
import { ListarClientesMapaRelacionamentoUseCase } from '@domain/usecases/ListarClientesMapaRelacionamentoUseCase'
import { ListarGestoresExternosUseCase } from '@domain/usecases/ListarGestoresExternosUseCase'
import { ListarPerfisExternosUseCase } from '@domain/usecases/ListarPerfisExternosUseCase'
import { ListarDepartamentosMapaRelacionamentoUseCase } from '@domain/usecases/ListarDepartamentosMapaRelacionamentoUseCase'

export interface MapaRelacionamentoState {
  clientes: ClienteMapaRelacionamento[]
  gestores: GestorExterno[]
  perfis: PerfilExterno[]
  departamentos: DepartamentoMapaRelacionamento[]
  clienteSelecionado: ClienteMapaRelacionamento | null
  codigoClienteSelecionado: string | null
  estruturaMapa: NoMapaRelacionamento | null
  noSelecionado: NoMapaRelacionamento | null
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  modoVisualizacao: 'diagrama' | 'lista' | 'c-levels'
  mostrarApenasCLevels: boolean
}

const initialState: MapaRelacionamentoState = {
  clientes: [],
  gestores: [],
  perfis: [],
  departamentos: [],
  clienteSelecionado: null,
  codigoClienteSelecionado: null,
  estruturaMapa: null,
  noSelecionado: null,
  status: 'idle',
  error: null,
  modoVisualizacao: 'diagrama',
  mostrarApenasCLevels: false,
}

export const listarClientesMapa = createAsyncThunk(
  'mapaRelacionamento/listarClientes',
  async ({ 
    token, 
    orgId, 
    limite = 50000, 
    cursor = 0, 
    nomeCliente = '' 
  }: { 
    token: string
    orgId: number
    limite?: number
    cursor?: number
    nomeCliente?: string
  }) => {
    const useCase = container.resolve(ListarClientesMapaRelacionamentoUseCase)
    const response = await useCase.execute(token, orgId, limite, cursor, nomeCliente)
    // API returns array directly
    return Array.isArray(response) ? response : []
  },
)

export const listarGestoresExternos = createAsyncThunk(
  'mapaRelacionamento/listarGestores',
  async ({ token, codigoCliente, limite = 50000 }: { token: string; codigoCliente: string; limite?: number }) => {
    const useCase = container.resolve(ListarGestoresExternosUseCase)
    const response = await useCase.execute(token, { codigoCliente, limite })
    return response.retorno
  },
)

export const listarPerfisExternos = createAsyncThunk(
  'mapaRelacionamento/listarPerfis',
  async ({ token, codigoCliente, limite = 50000 }: { token: string; codigoCliente: string; limite?: number }) => {
    const useCase = container.resolve(ListarPerfisExternosUseCase)
    const response = await useCase.execute(token, { codigoCliente, limite })
    return response.retorno
  },
)

export const listarDepartamentosMapa = createAsyncThunk(
  'mapaRelacionamento/listarDepartamentos',
  async ({ token, codigoCliente }: { token: string; codigoCliente: string }) => {
    const useCase = container.resolve(ListarDepartamentosMapaRelacionamentoUseCase)
    const response = await useCase.execute(token, codigoCliente)
    // Mapear DepartamentoResponse[] para DepartamentoMapaRelacionamento[]
    return response.map((dept) => ({
      cod: dept.id, // Usar o ID como cod
      departamento: dept.nome,
    }))
  },
)

const mapaRelacionamentoSlice = createSlice({
  name: 'mapaRelacionamento',
  initialState,
  reducers: {
    setClientes: (state, action: PayloadAction<ClienteMapaRelacionamento[]>) => {
      state.clientes = action.payload
    },
    setClienteSelecionado: (state, action: PayloadAction<ClienteMapaRelacionamento | null>) => {
      state.clienteSelecionado = action.payload
      state.codigoClienteSelecionado = action.payload?.codigoCliente || null
    },
    setEstruturaMapa: (state, action: PayloadAction<NoMapaRelacionamento | null>) => {
      state.estruturaMapa = action.payload
    },
    setNoSelecionado: (state, action: PayloadAction<NoMapaRelacionamento | null>) => {
      state.noSelecionado = action.payload
    },
    setModoVisualizacao: (state, action: PayloadAction<'diagrama' | 'lista' | 'c-levels'>) => {
      state.modoVisualizacao = action.payload
    },
    setMostrarApenasCLevels: (state, action: PayloadAction<boolean>) => {
      state.mostrarApenasCLevels = action.payload
    },
    resetMapaRelacionamento: () => initialState,
  },
  extraReducers: (builder) => {
    builder
      .addCase(listarClientesMapa.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(listarClientesMapa.fulfilled, (state, action) => {
        state.status = 'succeeded'
        // Só atualiza a lista global quando for carga inicial (sem filtro por nome).
        // Buscas do autocomplete usam resultado local e não devem sobrescrever state.clientes.
        const nomeCliente = action.meta.arg?.nomeCliente
        if (!nomeCliente || String(nomeCliente).trim() === '') {
          state.clientes = action.payload
        }
      })
      .addCase(listarClientesMapa.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao carregar clientes'
      })
      .addCase(listarGestoresExternos.fulfilled, (state, action) => {
        state.gestores = action.payload
      })
      .addCase(listarPerfisExternos.fulfilled, (state, action) => {
        state.perfis = action.payload
      })
      .addCase(listarDepartamentosMapa.fulfilled, (state, action) => {
        state.departamentos = action.payload
      })
  },
})

export const {
  setClientes,
  setClienteSelecionado,
  setEstruturaMapa,
  setNoSelecionado,
  setModoVisualizacao,
  setMostrarApenasCLevels,
  resetMapaRelacionamento,
} = mapaRelacionamentoSlice.actions

export default mapaRelacionamentoSlice.reducer

// Additional exports with full names for consistency
export const setEstruturaMapaRelacionamento = setEstruturaMapa
export const listarClientesMapaRelacionamento = listarClientesMapa
export const listarDepartamentosMapaRelacionamento = listarDepartamentosMapa
