import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type {
  IniciativaResponse,
  StatusIniciativaResponse,
  TemaResponse,
} from '@domain/entities/VcxIniciativas'
import { ListarIniciativasPorPosicaoIdUseCase } from '@domain/usecases/ListarIniciativasPorPosicaoIdUseCase'
import { BuscarIniciativaPorIdUseCase } from '@domain/usecases/BuscarIniciativaPorIdUseCase'
import { CriarIniciativaComTemaUseCase } from '@domain/usecases/CriarIniciativaComTemaUseCase'
import { AtualizarIniciativaComTemaUseCase } from '@domain/usecases/AtualizarIniciativaComTemaUseCase'
import { ExcluirIniciativaUseCase } from '@domain/usecases/ExcluirIniciativaUseCase'
import { ListarStatusIniciativasUseCase } from '@domain/usecases/ListarStatusIniciativasUseCase'
import { ListarTemasUseCase } from '@domain/usecases/ListarTemasUseCase'
import type { IniciativaPayload } from '@domain/entities/VcxIniciativas'

export interface VcxIniciativasState {
  iniciativas: IniciativaResponse[]
  iniciativasPorPosicao: Record<string, IniciativaResponse[]>
  iniciativaSelecionada: IniciativaResponse | null
  statusIniciativas: StatusIniciativaResponse[]
  temas: TemaResponse[]
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  // Estados específicos para cada operação
  statusListar: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusCriar: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusAtualizar: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusExcluir: 'idle' | 'loading' | 'succeeded' | 'failed'
  statusDadosReferencia: 'idle' | 'loading' | 'succeeded' | 'failed'
}

const initialState: VcxIniciativasState = {
  iniciativas: [],
  iniciativasPorPosicao: {},
  iniciativaSelecionada: null,
  statusIniciativas: [],
  temas: [],
  status: 'idle',
  error: null,
  statusListar: 'idle',
  statusCriar: 'idle',
  statusAtualizar: 'idle',
  statusExcluir: 'idle',
  statusDadosReferencia: 'idle',
}

// Async Thunks
export const listarIniciativasPorPosicaoId = createAsyncThunk(
  'vcxIniciativas/listarPorPosicaoId',
  async ({ token, posicaoId }: { token: string; posicaoId: string }) => {
    const useCase = container.resolve(ListarIniciativasPorPosicaoIdUseCase)
    return { posicaoId, iniciativas: await useCase.execute(token, posicaoId) }
  },
)

export const buscarIniciativaPorId = createAsyncThunk(
  'vcxIniciativas/buscarPorId',
  async ({ token, id }: { token: string; id: string }) => {
    const useCase = container.resolve(BuscarIniciativaPorIdUseCase)
    return useCase.execute(token, id)
  },
)

export const criarIniciativa = createAsyncThunk(
  'vcxIniciativas/criar',
  async ({
    token,
    payload,
    temaDescricao,
  }: {
    token: string
    payload: IniciativaPayload
    temaDescricao: string
  }) => {
    const useCase = container.resolve(CriarIniciativaComTemaUseCase)
    return useCase.execute(token, payload, temaDescricao)
  },
)

export const atualizarIniciativa = createAsyncThunk(
  'vcxIniciativas/atualizar',
  async ({
    token,
    payload,
    temaDescricao,
  }: {
    token: string
    payload: IniciativaPayload & { id: string }
    temaDescricao: string
  }) => {
    const useCase = container.resolve(AtualizarIniciativaComTemaUseCase)
    return useCase.execute(token, payload, temaDescricao)
  },
)

export const excluirIniciativa = createAsyncThunk(
  'vcxIniciativas/excluir',
  async ({ token, id }: { token: string; id: string }) => {
    const useCase = container.resolve(ExcluirIniciativaUseCase)
    await useCase.execute(token, id)
    return id
  },
)

export const listarStatusIniciativas = createAsyncThunk(
  'vcxIniciativas/listarStatus',
  async ({ token }: { token: string }) => {
    const useCase = container.resolve(ListarStatusIniciativasUseCase)
    return useCase.execute(token)
  },
)

export const listarTemas = createAsyncThunk(
  'vcxIniciativas/listarTemas',
  async ({ token }: { token: string }) => {
    const useCase = container.resolve(ListarTemasUseCase)
    return useCase.execute(token)
  },
)


const vcxIniciativasSlice = createSlice({
  name: 'vcxIniciativas',
  initialState,
  reducers: {
    setIniciativas: (state, action: PayloadAction<IniciativaResponse[]>) => {
      state.iniciativas = action.payload
    },
    setIniciativaSelecionada: (
      state,
      action: PayloadAction<IniciativaResponse | null>,
    ) => {
      state.iniciativaSelecionada = action.payload
    },
    limparIniciativas: (state) => {
      state.iniciativas = []
      state.iniciativasPorPosicao = {}
      state.iniciativaSelecionada = null
    },
    limparIniciativasPorPosicao: (state, action: PayloadAction<string>) => {
      delete state.iniciativasPorPosicao[action.payload]
    },
    resetVcxIniciativas: () => initialState,
  },
  extraReducers: (builder) => {
    // Listar Iniciativas por Posição
    builder
      .addCase(listarIniciativasPorPosicaoId.pending, (state) => {
        state.statusListar = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(listarIniciativasPorPosicaoId.fulfilled, (state, action) => {
        state.statusListar = 'succeeded'
        state.status = 'succeeded'
        const { posicaoId, iniciativas } = action.payload
        state.iniciativasPorPosicao[posicaoId] = iniciativas
        // Atualizar lista geral também
        state.iniciativas = iniciativas
      })
      .addCase(listarIniciativasPorPosicaoId.rejected, (state, action) => {
        state.statusListar = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao listar iniciativas'
      })

    // Buscar Iniciativa por ID
    builder
      .addCase(buscarIniciativaPorId.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(buscarIniciativaPorId.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.iniciativaSelecionada = action.payload
      })
      .addCase(buscarIniciativaPorId.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao buscar iniciativa'
      })

    // Criar Iniciativa
    builder
      .addCase(criarIniciativa.pending, (state) => {
        state.statusCriar = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(criarIniciativa.fulfilled, (state, action) => {
        state.statusCriar = 'succeeded'
        state.status = 'succeeded'
        const novaIniciativa = action.payload
        // Adicionar à lista geral
        state.iniciativas.push(novaIniciativa)
        // Adicionar ao cache por posição
        const posicaoId = novaIniciativa.organogramaPosicaoId
        if (!state.iniciativasPorPosicao[posicaoId]) {
          state.iniciativasPorPosicao[posicaoId] = []
        }
        state.iniciativasPorPosicao[posicaoId].push(novaIniciativa)
      })
      .addCase(criarIniciativa.rejected, (state, action) => {
        state.statusCriar = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao criar iniciativa'
      })

    // Atualizar Iniciativa
    builder
      .addCase(atualizarIniciativa.pending, (state) => {
        state.statusAtualizar = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(atualizarIniciativa.fulfilled, (state, action) => {
        state.statusAtualizar = 'succeeded'
        state.status = 'succeeded'
        const iniciativaAtualizada = action.payload
        // Atualizar na lista geral
        const index = state.iniciativas.findIndex(
          (i) => i.id === iniciativaAtualizada.id,
        )
        if (index !== -1) {
          state.iniciativas[index] = iniciativaAtualizada
        }
        // Atualizar no cache por posição
        const posicaoId = iniciativaAtualizada.organogramaPosicaoId
        if (state.iniciativasPorPosicao[posicaoId]) {
          const posIndex = state.iniciativasPorPosicao[posicaoId].findIndex(
            (i) => i.id === iniciativaAtualizada.id,
          )
          if (posIndex !== -1) {
            state.iniciativasPorPosicao[posicaoId][posIndex] =
              iniciativaAtualizada
          }
        }
        // Se for a iniciativa selecionada, atualizar também
        if (state.iniciativaSelecionada?.id === iniciativaAtualizada.id) {
          state.iniciativaSelecionada = iniciativaAtualizada
        }
      })
      .addCase(atualizarIniciativa.rejected, (state, action) => {
        state.statusAtualizar = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao atualizar iniciativa'
      })

    // Excluir Iniciativa
    builder
      .addCase(excluirIniciativa.pending, (state) => {
        state.statusExcluir = 'loading'
        state.status = 'loading'
        state.error = null
      })
      .addCase(excluirIniciativa.fulfilled, (state, action) => {
        state.statusExcluir = 'succeeded'
        state.status = 'succeeded'
        const idExcluido = action.payload
        // Remover da lista geral
        state.iniciativas = state.iniciativas.filter((i) => i.id !== idExcluido)
        // Remover do cache por posição
        Object.keys(state.iniciativasPorPosicao).forEach((posicaoId) => {
          state.iniciativasPorPosicao[posicaoId] =
            state.iniciativasPorPosicao[posicaoId].filter(
              (i) => i.id !== idExcluido,
            )
        })
        // Se for a iniciativa selecionada, limpar
        if (state.iniciativaSelecionada?.id === idExcluido) {
          state.iniciativaSelecionada = null
        }
      })
      .addCase(excluirIniciativa.rejected, (state, action) => {
        state.statusExcluir = 'failed'
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao excluir iniciativa'
      })

    // Listar Status
    builder
      .addCase(listarStatusIniciativas.pending, (state) => {
        state.statusDadosReferencia = 'loading'
        state.error = null
      })
      .addCase(listarStatusIniciativas.fulfilled, (state, action) => {
        state.statusDadosReferencia = 'succeeded'
        state.statusIniciativas = Array.isArray(action.payload) ? action.payload : []
      })
      .addCase(listarStatusIniciativas.rejected, (state, action) => {
        state.statusDadosReferencia = 'failed'
        state.error = action.error.message || 'Erro ao listar status'
      })

    // Listar Temas
    builder
      .addCase(listarTemas.pending, (state) => {
        state.statusDadosReferencia = 'loading'
        state.error = null
      })
      .addCase(listarTemas.fulfilled, (state, action) => {
        state.statusDadosReferencia = 'succeeded'
        state.temas = Array.isArray(action.payload) ? action.payload : []
      })
      .addCase(listarTemas.rejected, (state, action) => {
        state.statusDadosReferencia = 'failed'
        state.error = action.error.message || 'Erro ao listar temas'
      })

  },
})

export const {
  setIniciativas,
  setIniciativaSelecionada,
  limparIniciativas,
  limparIniciativasPorPosicao,
  resetVcxIniciativas,
} = vcxIniciativasSlice.actions

export default vcxIniciativasSlice.reducer
