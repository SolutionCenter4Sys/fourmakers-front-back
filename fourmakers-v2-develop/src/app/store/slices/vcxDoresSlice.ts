import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type {
  DorResponse,
  ImpactoResponse,
  UrgenciaResponse,
} from '@domain/entities/VcxDores'
import { ListarDoresPorPosicaoIdUseCase } from '@domain/usecases/ListarDoresPorPosicaoIdUseCase'
import { BuscarDorPorIdUseCase } from '@domain/usecases/BuscarDorPorIdUseCase'
import { CriarDorUseCase } from '@domain/usecases/CriarDorUseCase'
import { AtualizarDorUseCase } from '@domain/usecases/AtualizarDorUseCase'
import { ExcluirDorUseCase } from '@domain/usecases/ExcluirDorUseCase'
import { ListarImpactosDoresUseCase } from '@domain/usecases/ListarImpactosDoresUseCase'
import { ListarUrgenciasDoresUseCase } from '@domain/usecases/ListarUrgenciasDoresUseCase'
import type { DorPayload } from '@domain/entities/VcxDores'

export interface VcxDoresState {
  dores: DorResponse[]
  doresPorPosicao: Record<string, DorResponse[]>
  dorSelecionada: DorResponse | null
  impactos: ImpactoResponse[]
  urgencias: UrgenciaResponse[]
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  // Estados específicos para cada operação
  statusListar: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusCriar: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusAtualizar: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusExcluir: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusDadosReferencia: 'idle' | 'loading' | 'succeeded' | 'failed'
}

const initialState: VcxDoresState = {
  dores: [],
  doresPorPosicao: {},
  dorSelecionada: null,
  impactos: [],
  urgencias: [],
  status: 'idle',
  error: null,
  statusListar: 'idle',
  statusCriar: 'idle',
  statusAtualizar: 'idle',
  statusExcluir: 'idle',
  statusDadosReferencia: 'idle',
}

// Async Thunks
export const listarDoresPorPosicaoId = createAsyncThunk(
  'vcxDores/listarPorPosicaoId',
  async ({ token, posicaoId }: { token: string; posicaoId: string }) => {
    const useCase = container.resolve(ListarDoresPorPosicaoIdUseCase)
    return { posicaoId, dores: await useCase.execute(token, posicaoId) }
  },
)

export const buscarDorPorId = createAsyncThunk(
  'vcxDores/buscarPorId',
  async ({ token, id }: { token: string; id: string }) => {
    const useCase = container.resolve(BuscarDorPorIdUseCase)
    return useCase.execute(token, id)
  },
)

export const criarDor = createAsyncThunk(
  'vcxDores/criar',
  async ({ token, payload }: { token: string; payload: DorPayload }) => {
    const useCase = container.resolve(CriarDorUseCase)
    return useCase.execute(token, payload)
  },
)

export const atualizarDor = createAsyncThunk(
  'vcxDores/atualizar',
  async ({
    token,
    payload,
  }: {
    token: string
    payload: DorPayload & { id: string }
  }) => {
    const useCase = container.resolve(AtualizarDorUseCase)
    return useCase.execute(token, payload)
  },
)

export const excluirDor = createAsyncThunk(
  'vcxDores/excluir',
  async ({ token, id }: { token: string; id: string }) => {
    const useCase = container.resolve(ExcluirDorUseCase)
    await useCase.execute(token, id)
    return id
  },
)

export const listarImpactosDores = createAsyncThunk(
  'vcxDores/listarImpactos',
  async ({ token }: { token: string }) => {
    const useCase = container.resolve(ListarImpactosDoresUseCase)
    return useCase.execute(token)
  },
)

export const listarUrgenciasDores = createAsyncThunk(
  'vcxDores/listarUrgencias',
  async ({ token }: { token: string }) => {
    const useCase = container.resolve(ListarUrgenciasDoresUseCase)
    return useCase.execute(token)
  },
)

const vcxDoresSlice = createSlice({
  name: 'vcxDores',
  initialState,
  reducers: {
    setDores: (state, action: PayloadAction<DorResponse[]>) => {
      state.dores = action.payload
    },
    setDorSelecionada: (state, action: PayloadAction<DorResponse | null>) => {
      state.dorSelecionada = action.payload
    },
    limparDores: (state) => {
      state.dores = []
      state.doresPorPosicao = {}
      state.dorSelecionada = null
    },
    limparDoresPorPosicao: (state, action: PayloadAction<string>) => {
      delete state.doresPorPosicao[action.payload]
    },
    resetVcxDores: () => initialState,
  },
  extraReducers: (builder) => {
    // Listar Dores por Posição
    builder
      .addCase(listarDoresPorPosicaoId.pending, (state) => {
        state.statusListar = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(listarDoresPorPosicaoId.fulfilled, (state, action) => {
        state.statusListar = 'succeeded'
        state.status = 'succeeded'
        const { posicaoId, dores } = action.payload
        state.doresPorPosicao[posicaoId] = dores
        // Atualizar lista geral também
        state.dores = dores
      })
      .addCase(listarDoresPorPosicaoId.rejected, (state, action) => {
        state.statusListar = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao listar dores'
      })

    // Buscar Dor por ID
    builder
      .addCase(buscarDorPorId.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(buscarDorPorId.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.dorSelecionada = action.payload
      })
      .addCase(buscarDorPorId.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao buscar dor'
      })

    // Criar Dor
    builder
      .addCase(criarDor.pending, (state) => {
        state.statusCriar = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(criarDor.fulfilled, (state, action) => {
        state.statusCriar = 'succeeded'
        state.status = 'succeeded'
        const novaDor = action.payload
        // Adicionar à lista geral
        state.dores.push(novaDor)
        // Adicionar ao cache por posição
        const posicaoId = novaDor.organogramaPosicaoId
        if (!state.doresPorPosicao[posicaoId]) {
          state.doresPorPosicao[posicaoId] = []
        }
        state.doresPorPosicao[posicaoId].push(novaDor)
      })
      .addCase(criarDor.rejected, (state, action) => {
        state.statusCriar = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao criar dor'
      })

    // Atualizar Dor
    builder
      .addCase(atualizarDor.pending, (state) => {
        state.statusAtualizar = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(atualizarDor.fulfilled, (state, action) => {
        state.statusAtualizar = 'succeeded'
        state.status = 'succeeded'
        const dorAtualizada = action.payload
        // Atualizar na lista geral
        const index = state.dores.findIndex((d) => d.id === dorAtualizada.id)
        if (index !== -1) {
          state.dores[index] = dorAtualizada
        }
        // Atualizar no cache por posição
        const posicaoId = dorAtualizada.organogramaPosicaoId
        if (state.doresPorPosicao[posicaoId]) {
          const posIndex = state.doresPorPosicao[posicaoId].findIndex(
            (d) => d.id === dorAtualizada.id,
          )
          if (posIndex !== -1) {
            state.doresPorPosicao[posicaoId][posIndex] = dorAtualizada
          }
        }
        // Se for a dor selecionada, atualizar também
        if (
          state.dorSelecionada?.id === dorAtualizada.id
        ) {
          state.dorSelecionada = dorAtualizada
        }
      })
      .addCase(atualizarDor.rejected, (state, action) => {
        state.statusAtualizar = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao atualizar dor'
      })

    // Excluir Dor
    builder
      .addCase(excluirDor.pending, (state) => {
        state.statusExcluir = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(excluirDor.fulfilled, (state, action) => {
        state.statusExcluir = 'succeeded'
        state.status = 'succeeded'
        const idExcluido = action.payload
        // Remover da lista geral
        state.dores = state.dores.filter((d) => d.id !== idExcluido)
        // Remover do cache por posição
        Object.keys(state.doresPorPosicao).forEach((posicaoId) => {
          state.doresPorPosicao[posicaoId] = state.doresPorPosicao[
            posicaoId
          ].filter((d) => d.id !== idExcluido)
        })
        // Se for a dor selecionada, limpar
        if (state.dorSelecionada?.id === idExcluido) {
          state.dorSelecionada = null
        }
      })
      .addCase(excluirDor.rejected, (state, action) => {
        state.statusExcluir = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao excluir dor'
      })

    // Listar Impactos
    builder
      .addCase(listarImpactosDores.pending, (state) => {
        state.statusDadosReferencia = 'loading'
        state.error = null
      })
      .addCase(listarImpactosDores.fulfilled, (state, action) => {
        state.statusDadosReferencia = 'succeeded'
        state.impactos = Array.isArray(action.payload) ? action.payload : []
      })
      .addCase(listarImpactosDores.rejected, (state, action) => {
        state.statusDadosReferencia = 'failed'
        state.error = action.error.message || 'Erro ao listar impactos'
      })

    // Listar Urgências
    builder
      .addCase(listarUrgenciasDores.pending, (state) => {
        state.statusDadosReferencia = 'loading'
        state.error = null
      })
      .addCase(listarUrgenciasDores.fulfilled, (state, action) => {
        state.statusDadosReferencia = 'succeeded'
        state.urgencias = Array.isArray(action.payload) ? action.payload : []
      })
      .addCase(listarUrgenciasDores.rejected, (state, action) => {
        state.statusDadosReferencia = 'failed'
        state.error = action.error.message || 'Erro ao listar urgências'
      })
  },
})

export const {
  setDores,
  setDorSelecionada,
  limparDores,
  limparDoresPorPosicao,
  resetVcxDores,
} = vcxDoresSlice.actions

export default vcxDoresSlice.reducer
