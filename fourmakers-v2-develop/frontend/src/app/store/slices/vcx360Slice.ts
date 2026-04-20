import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import type { DadosVcx360 } from '@domain/entities/Vcx360'
import type { HistoricoDoresIniciativasResponse } from '@domain/entities/VcxHistorico'
import type { VcxAgenda } from '@domain/entities/VcxAgenda'
import { BuscarDadosVcx360UseCase } from '@domain/usecases/BuscarDadosVcx360UseCase'
import { SalvarDadosVcx360UseCase } from '@domain/usecases/SalvarDadosVcx360UseCase'
import { BuscarHistoricoVcxUseCase } from '@domain/usecases/BuscarHistoricoVcxUseCase'
import { ListarAgendasPorColaboradorClienteUseCase } from '@domain/usecases/ListarAgendasPorColaboradorClienteUseCase'

const VCX_AGENDA_PAGE_SIZE = 20

export interface Vcx360State {
  dadosVcx: DadosVcx360 | null
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  abaSelecionada: 'contexto' | 'agenda' | 'historico'
  historico: HistoricoDoresIniciativasResponse | null
  historicoStatus: 'idle' | 'loading' | 'succeeded' | 'failed'
  historicoError: string | null
  // Estado da aba Agenda
  agendasAntigas: VcxAgenda[]
  agendasNovas: VcxAgenda[]
  agendaCursor: number
  agendaHasMore: boolean
  agendaLoadingMore: boolean
  agendaErrorMessage: string | null
}

const initialState: Vcx360State = {
  dadosVcx: null,
  status: 'idle',
  error: null,
  abaSelecionada: 'contexto',
  historico: null,
  historicoStatus: 'idle',
  historicoError: null,
  agendasAntigas: [],
  agendasNovas: [],
  agendaCursor: 0,
  agendaHasMore: false,
  agendaLoadingMore: false,
  agendaErrorMessage: null,
}

export const buscarDadosVcx360 = createAsyncThunk(
  'vcx360/buscarDados',
  async ({ codigoCliente, departamentoId }: { codigoCliente: string; departamentoId: string }) => {
    const useCase = container.resolve(BuscarDadosVcx360UseCase)
    return useCase.execute(codigoCliente, departamentoId)
  },
)

export const salvarDadosVcx360 = createAsyncThunk(
  'vcx360/salvarDados',
  async ({ codigoCliente, dados }: { codigoCliente: string; dados: DadosVcx360 }) => {
    const useCase = container.resolve(SalvarDadosVcx360UseCase)
    await useCase.execute(codigoCliente, dados)
    return dados
  },
)

export const buscarHistoricoVcx = createAsyncThunk(
  'vcx360/buscarHistorico',
  async ({ posicaoId }: { posicaoId: string }, { getState }) => {
    const state = getState() as { auth: { token: string | null } }
    const token = state.auth.token

    if (!token) {
      throw new Error('Token não encontrado')
    }

    const useCase = container.resolve(BuscarHistoricoVcxUseCase)
    return useCase.execute(token, posicaoId)
  },
)

export interface CarregarAgendaParams {
  codigoColaborador: string
  codigoCliente: string
  cursor?: number
}

export const carregarAgenda = createAsyncThunk(
  'vcx360/carregarAgenda',
  async (
    { codigoColaborador, codigoCliente, cursor = 0 }: CarregarAgendaParams,
    { getState },
  ) => {
    const state = getState() as { auth: { token: string | null } }
    const token = state.auth.token

    if (!token) {
      throw new Error('Token não encontrado')
    }

    const useCase = container.resolve(ListarAgendasPorColaboradorClienteUseCase)
    const result = await useCase.execute(token, {
      codigoColaborador,
      codigoCliente,
      limit: VCX_AGENDA_PAGE_SIZE,
      cursor,
    })

    const antigas = result.agendasAntigas ?? []
    const novas = result.agendasNovas ?? []
    const totalRecebido = antigas.length + novas.length
    const hasMore = totalRecebido >= VCX_AGENDA_PAGE_SIZE
    const isFirstPage = cursor === 0

    return {
      agendasAntigas: antigas,
      agendasNovas: novas,
      cursorUsado: cursor,
      totalRecebido,
      hasMore,
      isFirstPage,
    }
  },
)

export const carregarMaisAgenda = createAsyncThunk(
  'vcx360/carregarMaisAgenda',
  async (
    { codigoColaborador, codigoCliente }: CarregarAgendaParams,
    { getState, dispatch },
  ) => {
    const state = getState() as { vcx360: Vcx360State }
    const { agendaLoadingMore, agendaHasMore, agendaCursor } = state.vcx360
    if (agendaLoadingMore || !agendaHasMore) {
      return
    }

    return dispatch(
      carregarAgenda({
        codigoColaborador,
        codigoCliente,
        cursor: agendaCursor,
      }),
    ).unwrap()
  },
)

const vcx360Slice = createSlice({
  name: 'vcx360',
  initialState,
  reducers: {
    setDadosVcx: (state, action: PayloadAction<DadosVcx360 | null>) => {
      state.dadosVcx = action.payload
    },
    setAbaSelecionada: (
      state,
      action: PayloadAction<'contexto' | 'agenda' | 'historico'>,
    ) => {
      state.abaSelecionada = action.payload
    },
    /** Limpa o estado da aba Agenda (ex.: ao trocar de nó no painel) para forçar nova carga. */
    limparAgenda: (state) => {
      state.agendasAntigas = []
      state.agendasNovas = []
      state.agendaCursor = 0
      state.agendaHasMore = false
      state.agendaLoadingMore = false
      state.agendaErrorMessage = null
    },
    resetVcx360: () => initialState,
  },
  extraReducers: (builder) => {
    builder
      .addCase(buscarDadosVcx360.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(buscarDadosVcx360.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.dadosVcx = action.payload
      })
      .addCase(buscarDadosVcx360.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message || 'Erro ao carregar dados VCX 360'
      })
      .addCase(salvarDadosVcx360.fulfilled, (state, action) => {
        state.dadosVcx = action.payload
      })
      .addCase(buscarHistoricoVcx.pending, (state) => {
        state.historicoStatus = 'loading'
        state.historicoError = null
      })
      .addCase(buscarHistoricoVcx.fulfilled, (state, action) => {
        state.historicoStatus = 'succeeded'
        state.historico = action.payload
      })
      .addCase(buscarHistoricoVcx.rejected, (state, action) => {
        state.historicoStatus = 'failed'
        state.historicoError = action.error.message || 'Erro ao carregar histórico'
      })
      .addCase(carregarAgenda.pending, (state) => {
        state.agendaLoadingMore = true
        state.agendaErrorMessage = null
      })
      .addCase(carregarAgenda.fulfilled, (state, action) => {
        state.agendaLoadingMore = false
        const { agendasAntigas, agendasNovas, totalRecebido, hasMore, isFirstPage } =
          action.payload
        if (isFirstPage) {
          state.agendasAntigas = agendasAntigas
          state.agendasNovas = agendasNovas
          state.agendaCursor = totalRecebido
        } else {
          state.agendasAntigas = [...state.agendasAntigas, ...agendasAntigas]
          state.agendasNovas = [...state.agendasNovas, ...agendasNovas]
          state.agendaCursor = state.agendaCursor + totalRecebido
        }
        state.agendaHasMore = hasMore
      })
      .addCase(carregarAgenda.rejected, (state, action) => {
        state.agendaLoadingMore = false
        state.agendaErrorMessage =
          action.error.message || 'Erro ao carregar agenda'
      })
  },
})

export const { setDadosVcx, setAbaSelecionada, limparAgenda, resetVcx360 } =
  vcx360Slice.actions

export default vcx360Slice.reducer
