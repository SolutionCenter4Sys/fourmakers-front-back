import { createAsyncThunk, createSlice, createSelector } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { container } from '@core/di/container'
import { ObterEncontrosBigNumbersUseCase } from '@domain/usecases/ObterEncontrosBigNumbersUseCase'
import { ObterBigNumbersCategoriaUseCase } from '@domain/usecases/ObterBigNumbersCategoriaUseCase'
import { ObterAgendaRealizadaDetalheUseCase } from '@domain/usecases/ObterAgendaRealizadaDetalheUseCase'
import { ObterClientesImpactadosDetalheUseCase } from '@domain/usecases/ObterClientesImpactadosDetalheUseCase'
import { ObterAgendaSemInteracaoDetalheUseCase } from '@domain/usecases/ObterAgendaSemInteracaoDetalheUseCase'
import { ObterAcoesEmAtrasoDetalheUseCase } from '@domain/usecases/ObterAcoesEmAtrasoDetalheUseCase'
import { ObterCategoriaComInteracaoDetalheUseCase } from '@domain/usecases/ObterCategoriaComInteracaoDetalheUseCase'
import { ObterGestoresImpactadosDetalheUseCase } from '@domain/usecases/ObterGestoresImpactadosDetalheUseCase'
import type { ObterBigNumbersParams } from '@domain/repositories/EncontrosBigNumbersRepository'
import type {
  FiltrosComercial,
  KpiSaude,
  KpiAlcance,
  SerieGrafico,
  IdIndicador,
  AgendaRealizadaDetalhe,
  ClienteImpactadoDetalhe,
  AgendaSemInteracaoDetalhe,
  AcoesEmAtrasoDetalhe,
  CategoriaComInteracaoDetalhe,
  GestoresImpactadosDetalhe,
} from '@shared/types/dashboardComercialTypes'
import { getPeriodoInicialDashboardComercial } from '@shared/constants/filtrosDashboardComercial'
import {
  MOCK_KPI_SAUDE,
  MOCK_KPI_ALCANCE,
  MOCK_GRAFICO_NIVEIS_ESTRATEGICOS,
  MOCK_GRAFICO_DEDICACAO_OBJETIVO,
  MOCK_GRAFICO_FOCO_CATEGORIA,
  EVIDENCIAS_POR_INDICADOR,
} from '@shared/utils/dashboardComercialMocks'

export interface DashboardComercialState {
  filters: FiltrosComercial
  kpiSaude: KpiSaude
  kpiAlcance: KpiAlcance
  graficoNiveisEstrategicos: typeof MOCK_GRAFICO_NIVEIS_ESTRATEGICOS
  graficoDedicacaoObjetivo: typeof MOCK_GRAFICO_DEDICACAO_OBJETIVO
  graficoFocoCategoria: SerieGrafico[]
  evidenciasPorIndicador: typeof EVIDENCIAS_POR_INDICADOR
  bigNumbersCarregando: boolean
  bigNumbersErro: string | null
  graficoCategoriaCarregando: boolean
  graficoCategoriaErro: string | null
  agendaRealizadaDetalhe: AgendaRealizadaDetalhe[] | null
  agendaRealizadaDetalheCarregando: boolean
  agendaRealizadaDetalheErro: string | null
  clientesImpactadosDetalhe: ClienteImpactadoDetalhe[] | null
  clientesImpactadosDetalheCarregando: boolean
  clientesImpactadosDetalheErro: string | null
  agendaSemInteracaoDetalhe: AgendaSemInteracaoDetalhe[] | null
  agendaSemInteracaoDetalheCarregando: boolean
  agendaSemInteracaoDetalheErro: string | null
  acoesEmAtrasoDetalhe: AcoesEmAtrasoDetalhe[] | null
  acoesEmAtrasoDetalheCarregando: boolean
  acoesEmAtrasoDetalheErro: string | null
  categoriaComInteracaoDetalhe: CategoriaComInteracaoDetalhe[] | null
  categoriaComInteracaoDetalheCarregando: boolean
  categoriaComInteracaoDetalheErro: string | null
  gestoresImpactadosDetalhe: GestoresImpactadosDetalhe[] | null
  gestoresImpactadosDetalheCarregando: boolean
  gestoresImpactadosDetalheErro: string | null
}

const periodoInicial = getPeriodoInicialDashboardComercial()

const initialState: DashboardComercialState = {
  filters: {
    dataInicio: periodoInicial.dataInicio,
    dataFim: periodoInicial.dataFim,
    codigoCliente: null,
    nomeClienteSelecionado: null,
    tipoComercial: 'gestor-externo',
    codigoColaboradorAgendou: null,
    codigoGestorExterno: null,
    nomeComercialSelecionado: null,
  },
  kpiSaude: MOCK_KPI_SAUDE,
  kpiAlcance: MOCK_KPI_ALCANCE,
  graficoNiveisEstrategicos: MOCK_GRAFICO_NIVEIS_ESTRATEGICOS,
  graficoDedicacaoObjetivo: MOCK_GRAFICO_DEDICACAO_OBJETIVO,
  graficoFocoCategoria: MOCK_GRAFICO_FOCO_CATEGORIA,
  evidenciasPorIndicador: EVIDENCIAS_POR_INDICADOR,
  bigNumbersCarregando: false,
  bigNumbersErro: null,
  graficoCategoriaCarregando: false,
  graficoCategoriaErro: null,
  agendaRealizadaDetalhe: null,
  agendaRealizadaDetalheCarregando: false,
  agendaRealizadaDetalheErro: null,
  clientesImpactadosDetalhe: null,
  clientesImpactadosDetalheCarregando: false,
  clientesImpactadosDetalheErro: null,
  agendaSemInteracaoDetalhe: null,
  agendaSemInteracaoDetalheCarregando: false,
  agendaSemInteracaoDetalheErro: null,
  acoesEmAtrasoDetalhe: null,
  acoesEmAtrasoDetalheCarregando: false,
  acoesEmAtrasoDetalheErro: null,
  categoriaComInteracaoDetalhe: null,
  categoriaComInteracaoDetalheCarregando: false,
  categoriaComInteracaoDetalheErro: null,
  gestoresImpactadosDetalhe: null,
  gestoresImpactadosDetalheCarregando: false,
  gestoresImpactadosDetalheErro: null,
}

function dataInicioDataFimParaPayload(filtros: FiltrosComercial): {
  dataInicio: string | null
  dataFim: string | null
} {
  const inicioPreenchido = filtros.dataInicio != null && String(filtros.dataInicio).trim() !== ''
  const fimPreenchido = filtros.dataFim != null && String(filtros.dataFim).trim() !== ''
  if (!inicioPreenchido && !fimPreenchido) {
    const periodo = getPeriodoInicialDashboardComercial()
    return { dataInicio: periodo.dataInicio, dataFim: periodo.dataFim }
  }
  return {
    dataInicio: filtros.dataInicio ?? null,
    dataFim: filtros.dataFim ?? null,
  }
}

export const buscarEncontrosBigNumbers = createAsyncThunk(
  'dashboardComercial/buscarEncontrosBigNumbers',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterEncontrosBigNumbersUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

export const buscarBigNumbersCategoria = createAsyncThunk(
  'dashboardComercial/buscarBigNumbersCategoria',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterBigNumbersCategoriaUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

/** Lista de agendas realizadas: filtros da tela são passados como parâmetros ao endpoint AgendaRealizadaDetalhe. */
export const buscarAgendaRealizadaDetalhe = createAsyncThunk(
  'dashboardComercial/buscarAgendaRealizadaDetalhe',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterAgendaRealizadaDetalheUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

/** Lista de clientes impactados com agendas: filtros da tela são passados ao endpoint ClientesImpactadosDetalhe. */
export const buscarClientesImpactadosDetalhe = createAsyncThunk(
  'dashboardComercial/buscarClientesImpactadosDetalhe',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterClientesImpactadosDetalheUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

/** Lista de agendas sem interação: filtros passados ao endpoint AgendaSemInteracaoDetalhe. */
export const buscarAgendaSemInteracaoDetalhe = createAsyncThunk(
  'dashboardComercial/buscarAgendaSemInteracaoDetalhe',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterAgendaSemInteracaoDetalheUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

/** Lista de ações em atraso (agrupado por status): filtros passados ao endpoint AcoesEmAtrasoDetalhe. */
export const buscarAcoesEmAtrasoDetalhe = createAsyncThunk(
  'dashboardComercial/buscarAcoesEmAtrasoDetalhe',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterAcoesEmAtrasoDetalheUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

/** Lista de categorias com interação (agrupado): filtros passados ao endpoint CategoriaComInteracaoDetalhe. */
export const buscarCategoriaComInteracaoDetalhe = createAsyncThunk(
  'dashboardComercial/buscarCategoriaComInteracaoDetalhe',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterCategoriaComInteracaoDetalheUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

/** Lista de gestores impactados (agrupado): filtros passados ao endpoint GestoresImpactadosDetalhe. */
export const buscarGestoresImpactadosDetalhe = createAsyncThunk(
  'dashboardComercial/buscarGestoresImpactadosDetalhe',
  async ({ token, filtros }: { token: string; filtros: FiltrosComercial }) => {
    const useCase = container.resolve(ObterGestoresImpactadosDetalheUseCase)
    const { dataInicio, dataFim } = dataInicioDataFimParaPayload(filtros)
    const params: ObterBigNumbersParams = {
      dataInicio,
      dataFim,
      codigoCliente: filtros.codigoCliente,
      codigoColaboradorAgendou: filtros.codigoColaboradorAgendou,
      codigoGestorExterno: filtros.codigoGestorExterno,
    }
    return useCase.execute(token, params)
  },
)

const dashboardComercialSlice = createSlice({
  name: 'dashboardComercial',
  initialState,
  reducers: {
    setFilter: (
      state,
      action: PayloadAction<{
        name: keyof FiltrosComercial
        value: number | string | null
      }>,
    ) => {
      const { name, value } = action.payload
      ;(state.filters as Record<string, unknown>)[name] = value
    },
    clearFilters: (state) => {
      state.filters = { ...initialState.filters }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(buscarEncontrosBigNumbers.pending, (state) => {
        state.bigNumbersCarregando = true
        state.bigNumbersErro = null
      })
      .addCase(buscarEncontrosBigNumbers.fulfilled, (state, action) => {
        state.bigNumbersCarregando = false
        state.bigNumbersErro = null
        state.kpiSaude = action.payload.kpiSaude
        state.kpiAlcance = action.payload.kpiAlcance
      })
      .addCase(buscarEncontrosBigNumbers.rejected, (state, action) => {
        state.bigNumbersCarregando = false
        state.bigNumbersErro = action.error.message ?? 'Erro ao carregar Big Numbers'
      })
      .addCase(buscarBigNumbersCategoria.pending, (state) => {
        state.graficoCategoriaCarregando = true
        state.graficoCategoriaErro = null
      })
      .addCase(buscarBigNumbersCategoria.fulfilled, (state, action) => {
        state.graficoCategoriaCarregando = false
        state.graficoCategoriaErro = null
        state.graficoFocoCategoria = action.payload
      })
      .addCase(buscarBigNumbersCategoria.rejected, (state, action) => {
        state.graficoCategoriaCarregando = false
        state.graficoCategoriaErro = action.error.message ?? 'Erro ao carregar gráfico por categoria'
      })
      .addCase(buscarAgendaRealizadaDetalhe.pending, (state) => {
        state.agendaRealizadaDetalheCarregando = true
        state.agendaRealizadaDetalheErro = null
      })
      .addCase(buscarAgendaRealizadaDetalhe.fulfilled, (state, action) => {
        state.agendaRealizadaDetalheCarregando = false
        state.agendaRealizadaDetalheErro = null
        state.agendaRealizadaDetalhe = action.payload
      })
      .addCase(buscarAgendaRealizadaDetalhe.rejected, (state, action) => {
        state.agendaRealizadaDetalheCarregando = false
        state.agendaRealizadaDetalheErro = action.error.message ?? 'Erro ao carregar detalhe de agendas realizadas'
        state.agendaRealizadaDetalhe = null
      })
      .addCase(buscarClientesImpactadosDetalhe.pending, (state) => {
        state.clientesImpactadosDetalheCarregando = true
        state.clientesImpactadosDetalheErro = null
      })
      .addCase(buscarClientesImpactadosDetalhe.fulfilled, (state, action) => {
        state.clientesImpactadosDetalheCarregando = false
        state.clientesImpactadosDetalheErro = null
        state.clientesImpactadosDetalhe = action.payload
      })
      .addCase(buscarClientesImpactadosDetalhe.rejected, (state, action) => {
        state.clientesImpactadosDetalheCarregando = false
        state.clientesImpactadosDetalheErro = action.error.message ?? 'Erro ao carregar detalhe de clientes impactados'
        state.clientesImpactadosDetalhe = null
      })
      .addCase(buscarAgendaSemInteracaoDetalhe.pending, (state) => {
        state.agendaSemInteracaoDetalheCarregando = true
        state.agendaSemInteracaoDetalheErro = null
      })
      .addCase(buscarAgendaSemInteracaoDetalhe.fulfilled, (state, action) => {
        state.agendaSemInteracaoDetalheCarregando = false
        state.agendaSemInteracaoDetalheErro = null
        state.agendaSemInteracaoDetalhe = action.payload
      })
      .addCase(buscarAgendaSemInteracaoDetalhe.rejected, (state, action) => {
        state.agendaSemInteracaoDetalheCarregando = false
        state.agendaSemInteracaoDetalheErro = action.error.message ?? 'Erro ao carregar detalhe de agendas sem interação'
        state.agendaSemInteracaoDetalhe = null
      })
      .addCase(buscarAcoesEmAtrasoDetalhe.pending, (state) => {
        state.acoesEmAtrasoDetalheCarregando = true
        state.acoesEmAtrasoDetalheErro = null
      })
      .addCase(buscarAcoesEmAtrasoDetalhe.fulfilled, (state, action) => {
        state.acoesEmAtrasoDetalheCarregando = false
        state.acoesEmAtrasoDetalheErro = null
        state.acoesEmAtrasoDetalhe = action.payload
      })
      .addCase(buscarAcoesEmAtrasoDetalhe.rejected, (state, action) => {
        state.acoesEmAtrasoDetalheCarregando = false
        state.acoesEmAtrasoDetalheErro = action.error.message ?? 'Erro ao carregar detalhe de ações em atraso'
        state.acoesEmAtrasoDetalhe = null
      })
      .addCase(buscarCategoriaComInteracaoDetalhe.pending, (state) => {
        state.categoriaComInteracaoDetalheCarregando = true
        state.categoriaComInteracaoDetalheErro = null
      })
      .addCase(buscarCategoriaComInteracaoDetalhe.fulfilled, (state, action) => {
        state.categoriaComInteracaoDetalheCarregando = false
        state.categoriaComInteracaoDetalheErro = null
        state.categoriaComInteracaoDetalhe = action.payload
      })
      .addCase(buscarCategoriaComInteracaoDetalhe.rejected, (state, action) => {
        state.categoriaComInteracaoDetalheCarregando = false
        state.categoriaComInteracaoDetalheErro = action.error.message ?? 'Erro ao carregar detalhe de categorias com interação'
        state.categoriaComInteracaoDetalhe = null
      })
      .addCase(buscarGestoresImpactadosDetalhe.pending, (state) => {
        state.gestoresImpactadosDetalheCarregando = true
        state.gestoresImpactadosDetalheErro = null
      })
      .addCase(buscarGestoresImpactadosDetalhe.fulfilled, (state, action) => {
        state.gestoresImpactadosDetalheCarregando = false
        state.gestoresImpactadosDetalheErro = null
        state.gestoresImpactadosDetalhe = action.payload
      })
      .addCase(buscarGestoresImpactadosDetalhe.rejected, (state, action) => {
        state.gestoresImpactadosDetalheCarregando = false
        state.gestoresImpactadosDetalheErro = action.error.message ?? 'Erro ao carregar detalhe de gestores impactados'
        state.gestoresImpactadosDetalhe = null
      })
  },
})

export const { setFilter, clearFilters } = dashboardComercialSlice.actions

export const selectFiltros = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.filters

export const selectKpiSaude = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.kpiSaude

export const selectKpiAlcance = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.kpiAlcance

export const selectGraficoNiveisEstrategicos = (
  state: { dashboardComercial: DashboardComercialState },
) => state.dashboardComercial.graficoNiveisEstrategicos

export const selectGraficoDedicacaoObjetivo = (
  state: { dashboardComercial: DashboardComercialState },
) => state.dashboardComercial.graficoDedicacaoObjetivo

export const selectGraficoFocoCategoria = (
  state: { dashboardComercial: DashboardComercialState },
) => state.dashboardComercial.graficoFocoCategoria

export const selectEvidenciasPorIndicador = createSelector(
  [(state: { dashboardComercial: DashboardComercialState }) => state.dashboardComercial.evidenciasPorIndicador],
  (evidenciasPorIndicador) => (id: IdIndicador) => evidenciasPorIndicador[id] ?? [],
)

export const selectBigNumbersCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.bigNumbersCarregando

export const selectBigNumbersErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.bigNumbersErro

export const selectAgendaRealizadaDetalhe = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.agendaRealizadaDetalhe

export const selectAgendaRealizadaDetalheCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.agendaRealizadaDetalheCarregando

export const selectAgendaRealizadaDetalheErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.agendaRealizadaDetalheErro

export const selectClientesImpactadosDetalhe = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.clientesImpactadosDetalhe

export const selectClientesImpactadosDetalheCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.clientesImpactadosDetalheCarregando

export const selectClientesImpactadosDetalheErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.clientesImpactadosDetalheErro

export const selectAgendaSemInteracaoDetalhe = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.agendaSemInteracaoDetalhe

export const selectAgendaSemInteracaoDetalheCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.agendaSemInteracaoDetalheCarregando

export const selectAgendaSemInteracaoDetalheErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.agendaSemInteracaoDetalheErro

export const selectAcoesEmAtrasoDetalhe = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.acoesEmAtrasoDetalhe

export const selectAcoesEmAtrasoDetalheCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.acoesEmAtrasoDetalheCarregando

export const selectAcoesEmAtrasoDetalheErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.acoesEmAtrasoDetalheErro

export const selectCategoriaComInteracaoDetalhe = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.categoriaComInteracaoDetalhe

export const selectCategoriaComInteracaoDetalheCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.categoriaComInteracaoDetalheCarregando

export const selectCategoriaComInteracaoDetalheErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.categoriaComInteracaoDetalheErro

export const selectGestoresImpactadosDetalhe = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.gestoresImpactadosDetalhe

export const selectGestoresImpactadosDetalheCarregando = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.gestoresImpactadosDetalheCarregando

export const selectGestoresImpactadosDetalheErro = (state: { dashboardComercial: DashboardComercialState }) =>
  state.dashboardComercial.gestoresImpactadosDetalheErro

export { dashboardComercialSlice }
export default dashboardComercialSlice.reducer
