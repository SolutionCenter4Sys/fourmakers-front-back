import { createAsyncThunk, createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import { BuscarAgendaGestorUseCase } from '@domain/usecases/BuscarAgendaGestorUseCase'
import { CarregarAgendaDetalheUseCase } from '@domain/usecases/CarregarAgendaDetalheUseCase'
import { CriarAgendaUseCase } from '@domain/usecases/CriarAgendaUseCase'
import { AtualizarAgendaUseCase } from '@domain/usecases/AtualizarAgendaUseCase'
import { DeletarAgendaUseCase } from '@domain/usecases/DeletarAgendaUseCase'
import { InserirInteracaoIaUseCase } from '@domain/usecases/InserirInteracaoIaUseCase'
import { AtualizarInteracaoIaUseCase } from '@domain/usecases/AtualizarInteracaoIaUseCase'
import { DeletarInteracaoIaUseCase } from '@domain/usecases/DeletarInteracaoIaUseCase'
import { AtualizarStatusAcoesUseCase } from '@domain/usecases/AtualizarStatusAcoesUseCase'
import { InserirComentarioAcaoUseCase } from '@domain/usecases/InserirComentarioAcaoUseCase'
import { AceitarRecusarConviteAgendaUseCase } from '@domain/usecases/AceitarRecusarConviteAgendaUseCase'
import { CriarEncontroAiProximosPassosUseCase } from '@domain/usecases/CriarEncontroAiProximosPassosUseCase'
import { AtualizarEncontroAiProximosPassosUseCase } from '@domain/usecases/AtualizarEncontroAiProximosPassosUseCase'
import { BuscarEncontroAiPorEncontroIdUseCase } from '@domain/usecases/BuscarEncontroAiPorEncontroIdUseCase'
import { BuscarProximosPassosMoxeUseCase } from '@domain/usecases/BuscarProximosPassosMoxeUseCase'
import type {
  AgendaGestorCompleto,
  ItemAgendaGestor,
  CriarAgendaPayload,
  AtualizarAgendaPayload,
  InserirInteracaoIaPayload,
  AtualizarInteracaoIaPayload,
  AtualizarStatusAcoesPayload,
  InserirComentarioAcaoPayload,
  CriarEncontroAiPayload,
  AtualizarEncontroAiPayload,
  BuscarProximosPassosMoxeRequest,
} from '@domain/entities/AgendaGestor'

export interface AgendasComerciaisState {
  historico: AgendaGestorCompleto | null
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  /** Agenda carregada para exibição no modal Ver Mais (carregarAgenda por id). */
  agendaDetalheCarregada: ItemAgendaGestor | null
  statusDetalhe: 'idle' | 'loading' | 'succeeded' | 'failed'
}

const initialState: AgendasComerciaisState = {
  historico: null,
  status: 'idle',
  error: null,
  agendaDetalheCarregada: null,
  statusDetalhe: 'idle',
}

export const buscarAgendasComerciais = createAsyncThunk(
  'agendasComerciais/buscar',
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
        error instanceof Error ? error.message : 'Erro ao buscar agendas comerciais',
      )
    }
  },
)

// CRUD de Agendas
export const criarAgenda = createAsyncThunk(
  'agendasComerciais/criarAgenda',
  async (
    { token, payload }: { token: string; payload: CriarAgendaPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(CriarAgendaUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao criar agenda',
      )
    }
  },
)

export const atualizarAgenda = createAsyncThunk(
  'agendasComerciais/atualizarAgenda',
  async (
    { token, payload }: { token: string; payload: AtualizarAgendaPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(AtualizarAgendaUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao atualizar agenda',
      )
    }
  },
)

export const deletarAgenda = createAsyncThunk(
  'agendasComerciais/deletarAgenda',
  async (
    { token, agendaId }: { token: string; agendaId: number },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(DeletarAgendaUseCase)
      return await useCase.execute(token, agendaId)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao deletar agenda',
      )
    }
  },
)

// CRUD de Interações
export const inserirInteracaoIa = createAsyncThunk(
  'agendasComerciais/inserirInteracaoIa',
  async (
    { token, payload }: { token: string; payload: InserirInteracaoIaPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(InserirInteracaoIaUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao inserir interação',
      )
    }
  },
)

export const atualizarInteracaoIa = createAsyncThunk(
  'agendasComerciais/atualizarInteracaoIa',
  async (
    { token, payload }: { token: string; payload: AtualizarInteracaoIaPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(AtualizarInteracaoIaUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao atualizar interação',
      )
    }
  },
)

export const deletarInteracaoIa = createAsyncThunk(
  'agendasComerciais/deletarInteracaoIa',
  async (
    { token, interacaoId }: { token: string; interacaoId: number },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(DeletarInteracaoIaUseCase)
      return await useCase.execute(token, interacaoId)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao deletar interação',
      )
    }
  },
)

// CRUD de Ações/Passos
export const atualizarStatusAcoes = createAsyncThunk(
  'agendasComerciais/atualizarStatusAcoes',
  async (
    { token, payload }: { token: string; payload: AtualizarStatusAcoesPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(AtualizarStatusAcoesUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao atualizar status da ação',
      )
    }
  },
)

export const inserirComentarioAcao = createAsyncThunk(
  'agendasComerciais/inserirComentarioAcao',
  async (
    { token, payload }: { token: string; payload: InserirComentarioAcaoPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(InserirComentarioAcaoUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao inserir comentário',
      )
    }
  },
)

export const aceitarConviteAgenda = createAsyncThunk(
  'agendasComerciais/aceitarConviteAgenda',
  async (
    {
      token,
      agendaId,
      codigoColaborador,
      decisaoStatus,
    }: {
      token: string
      agendaId: string
      codigoColaborador: string // cpf
      decisaoStatus: number // 1 = aceitar (aprovado), 2 = recusar (reprovado), 0 = pendente
    },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(AceitarRecusarConviteAgendaUseCase)
      return await useCase.execute(token, {
        decisaoStatus,
        agendaId,
        codigoColaborador,
      })
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao processar convite da agenda',
      )
    }
  },
)

export const carregarAgendaDetalhe = createAsyncThunk(
  'agendasComerciais/carregarAgendaDetalhe',
  async (
    { token, agendaId }: { token: string; agendaId: number },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(CarregarAgendaDetalheUseCase)
      return await useCase.execute(token, agendaId)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao carregar detalhes da agenda',
      )
    }
  },
)

// EncontroAi (próximos passos com IA)
export const criarEncontroAiProximosPassos = createAsyncThunk(
  'agendasComerciais/criarEncontroAiProximosPassos',
  async (
    { token, payload }: { token: string; payload: CriarEncontroAiPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(CriarEncontroAiProximosPassosUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao criar próximos passos',
      )
    }
  },
)

export const atualizarEncontroAiProximosPassos = createAsyncThunk(
  'agendasComerciais/atualizarEncontroAiProximosPassos',
  async (
    { token, payload }: { token: string; payload: AtualizarEncontroAiPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(AtualizarEncontroAiProximosPassosUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao atualizar próximos passos',
      )
    }
  },
)

export const buscarEncontroAiPorEncontroId = createAsyncThunk(
  'agendasComerciais/buscarEncontroAiPorEncontroId',
  async (
    { token, encontroId }: { token: string; encontroId: number },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(BuscarEncontroAiPorEncontroIdUseCase)
      return await useCase.execute(token, encontroId)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Erro ao buscar próximos passos',
      )
    }
  },
)

/** Gera resumo e próximos passos via IA (Moxe). Legado: BuscarProximosPassosIntegracaoMoxe. */
export const buscarProximosPassosMoxe = createAsyncThunk(
  'agendasComerciais/buscarProximosPassosMoxe',
  async (
    { token, payload }: { token: string; payload: BuscarProximosPassosMoxeRequest },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(BuscarProximosPassosMoxeUseCase)
      return await useCase.execute(token, payload)
    } catch (error) {
      return rejectWithValue(
        error instanceof Error ? error.message : 'Não foi possível gerar o resumo com IA.',
      )
    }
  },
)

const agendasComerciaisSlice = createSlice({
  name: 'agendasComerciais',
  initialState,
  reducers: {
    limparAgendas: (state) => {
      state.historico = null
      state.status = 'idle'
      state.error = null
    },
    limparAgendaDetalhe: (state) => {
      state.agendaDetalheCarregada = null
      state.statusDetalhe = 'idle'
    },
  },
  extraReducers: (builder) => {
    builder
      // Buscar agendas
      .addCase(buscarAgendasComerciais.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(
        buscarAgendasComerciais.fulfilled,
        (state, action: PayloadAction<AgendaGestorCompleto>) => {
          state.status = 'succeeded'
          state.historico = action.payload
          state.error = null
        },
      )
      .addCase(buscarAgendasComerciais.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
      // CRUD de Agendas
      .addCase(criarAgenda.fulfilled, (state) => {
        // Após criar, pode recarregar a lista
        state.status = 'idle'
      })
      .addCase(atualizarAgenda.fulfilled, (state) => {
        // Após atualizar, pode recarregar a lista
        state.status = 'idle'
      })
      .addCase(deletarAgenda.fulfilled, (state) => {
        // Após deletar, pode recarregar a lista
        state.status = 'idle'
      })
      // CRUD de Interações
      .addCase(inserirInteracaoIa.fulfilled, (state) => {
        state.status = 'idle'
      })
      .addCase(atualizarInteracaoIa.fulfilled, (state) => {
        state.status = 'idle'
      })
      .addCase(deletarInteracaoIa.fulfilled, (state) => {
        state.status = 'idle'
      })
      // EncontroAi (próximos passos)
      .addCase(criarEncontroAiProximosPassos.fulfilled, (state) => {
        state.error = null
      })
      .addCase(atualizarEncontroAiProximosPassos.fulfilled, (state) => {
        state.error = null
      })
      // CRUD de Ações
      .addCase(atualizarStatusAcoes.fulfilled, (state) => {
        state.status = 'idle'
      })
      .addCase(inserirComentarioAcao.fulfilled, (state) => {
        state.status = 'idle'
      })
      // Aceitar Convite
      .addCase(aceitarConviteAgenda.fulfilled, (state) => {
        state.status = 'idle'
      })
      .addCase(aceitarConviteAgenda.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.payload as string
      })
      // Carregar agenda para modal Ver Mais
      .addCase(carregarAgendaDetalhe.pending, (state) => {
        state.statusDetalhe = 'loading'
      })
      .addCase(
        carregarAgendaDetalhe.fulfilled,
        (state, action: PayloadAction<ItemAgendaGestor>) => {
          state.statusDetalhe = 'succeeded'
          state.agendaDetalheCarregada = action.payload
        },
      )
      .addCase(carregarAgendaDetalhe.rejected, (state) => {
        state.statusDetalhe = 'failed'
      })
  },
})

export const { limparAgendas, limparAgendaDetalhe } = agendasComerciaisSlice.actions
export default agendasComerciaisSlice.reducer
