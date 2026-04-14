import type { PayloadAction } from '@reduxjs/toolkit'
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type { Colaborador } from '@domain/entities/Colaborador'
import type { ColaboradorResponse, EditarColaboradorPayload, InserirColaboradorPayload } from '@domain/repositories/ColaboradoresRepository'
import { EditarColaboradorUseCase } from '@domain/usecases/EditarColaboradorUseCase'
import { GetColaboradoresUseCase } from '@domain/usecases/GetColaboradoresUseCase'
import { InserirColaboradorUseCase } from '@domain/usecases/InserirColaboradorUseCase'

export interface ColaboradoresState {
  colaboradores: Colaborador[]
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  cursor: number
  limite: number
  nomeOuEmail: string
  hasMore: boolean
}

const initialState: ColaboradoresState = {
  colaboradores: [],
  status: 'idle',
  error: null,
  cursor: 0,
  limite: 20,
  nomeOuEmail: '',
  hasMore: true,
}

export const fetchColaboradores = createAsyncThunk(
  'colaboradores/fetchColaboradores',
  async ({ token, orgId, cursor, limite, nomeOuEmail, codExterno, fourtalents, append, codigoCliente }: {
    token: string;
    orgId: number;
    cursor: number;
    limite: number;
    nomeOuEmail: string;
    codExterno?: string;
    fourtalents?: boolean;
    append?: boolean;
    /** Enviar apenas quando cliente estiver selecionado no filtro. */
    codigoCliente?: string | null;
  }) => {
    const useCase = container.resolve(GetColaboradoresUseCase)
    const response = await useCase.execute(token, {
      cursor,
      limite,
      nomeOuEmail,
      org: orgId,
      codExterno,
      fourtalents,
      ...(codigoCliente != null && codigoCliente.trim() !== '' ? { codigoCliente } : {}),
    })

    return {
      colaboradores: response.retorno,
      cursor,
      limite,
      append: append === true && cursor > 0,
    }
  },
)

export const inserirColaborador = createAsyncThunk(
  'colaboradores/inserirColaborador',
  async ({ token, payload }: {
    token: string;
    payload: InserirColaboradorPayload;
  }): Promise<ColaboradorResponse> => {
    const useCase = container.resolve(InserirColaboradorUseCase)
    return await useCase.execute(token, payload)
  },
)

export const editarColaborador = createAsyncThunk(
  'colaboradores/editarColaborador',
  async ({ token, payload }: {
    token: string;
    payload: EditarColaboradorPayload;
  }): Promise<ColaboradorResponse> => {
    const useCase = container.resolve(EditarColaboradorUseCase)
    return await useCase.execute(token, payload)
  },
)

const colaboradoresSlice = createSlice({
  name: 'colaboradores',
  initialState,
  reducers: {
    setSearch: (state, action: PayloadAction<string>) => {
      state.nomeOuEmail = action.payload
      state.cursor = 0
    },
    resetColaboradores: (state) => {
      state.colaboradores = []
      state.cursor = 0
      state.hasMore = true
      state.status = 'idle'
      state.error = null
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchColaboradores.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(fetchColaboradores.fulfilled, (state, action) => {
        state.status = 'succeeded'
        if (action.payload.append) {
          state.colaboradores = [...state.colaboradores, ...action.payload.colaboradores]
        } else {
          state.colaboradores = action.payload.colaboradores
        }
        state.cursor = action.payload.cursor + action.payload.colaboradores.length
        state.limite = action.payload.limite
        state.hasMore = action.payload.colaboradores.length === action.payload.limite
      })
      .addCase(fetchColaboradores.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro ao carregar colaboradores.'
      })
      .addCase(inserirColaborador.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(inserirColaborador.fulfilled, (state) => {
        state.status = 'succeeded'
        // Reset para recarregar a lista
        state.cursor = 0
      })
      .addCase(inserirColaborador.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro ao inserir colaborador.'
      })
      .addCase(editarColaborador.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(editarColaborador.fulfilled, (state) => {
        state.status = 'succeeded'
        // Reset para recarregar a lista
        state.cursor = 0
      })
      .addCase(editarColaborador.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro ao editar colaborador.'
      })
  },
})

export const { setSearch, resetColaboradores } = colaboradoresSlice.actions

export default colaboradoresSlice.reducer

