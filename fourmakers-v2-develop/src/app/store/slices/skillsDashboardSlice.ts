import { createAsyncThunk, createSlice, createSelector } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

import { container } from '@core/di/container'
import { GetSkillsDashboardDataUseCase } from '@domain/usecases/GetSkillsDashboardDataUseCase'
import { GetSkillLogsPaginatedUseCase } from '@domain/usecases/GetSkillLogsPaginatedUseCase'
import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'
import type { TopDezData, SkillTopDezEntry } from '@domain/types/SkillsDashboardTypes'
import { mapFilterEventToBackendEvents } from '@shared/utils/skillsDashboardUtils'

export interface SkillsDashboardFilters {
  nome: string
  cliente: string
  tipoSkill: string
  evento: string
}

export interface SkillsDashboardState {
  rawData: SkillLogEntry[]
  totalUsers: number
  bigNumbers?: {
    skillsAdicionadas: number
    skillsSugeridas: number
    adicionadasPDI: number
    skillsRejeitadas: number
  }
  topDez?: TopDezData
  filters: SkillsDashboardFilters
  status: 'idle' | 'loading' | 'succeeded' | 'failed'
  error: string | null
  pagination: {
    currentPage: number
    itemsPerPage: number
    cursor: number
    hasMore: boolean
    isLoading: boolean
  }
}

export interface ChartDataPoint {
  name: string
  value: number
  [key: string]: string | number
}

export interface ChartsData {
  barAdded: ChartDataPoint[]
  barSuggested: ChartDataPoint[]
  barPDI: ChartDataPoint[]
  barRejected: ChartDataPoint[]
  pieUsage: ChartDataPoint[]
}

export interface KPIData {
  added: number
  suggested: number
  pdi: number
  rejected: number
}

export interface DropdownOptions {
  clientes: string[]
  tiposSkill: string[]
  eventos: string[]
}

const initialState: SkillsDashboardState = {
  rawData: [],
  totalUsers: 0,
  bigNumbers: undefined,
  topDez: undefined,
  filters: {
    nome: '',
    cliente: '',
    tipoSkill: '',
    evento: '',
  },
  status: 'idle',
  error: null,
  pagination: {
    currentPage: 1,
    itemsPerPage: 10,
    cursor: 0,
    hasMore: true,
    isLoading: false,
  },
}

export const fetchSkillsDashboardData = createAsyncThunk(
  'skillsDashboard/fetchData',
  async ({ token, orgId }: { token: string; orgId: number }) => {
    const useCase = container.resolve(GetSkillsDashboardDataUseCase)
    return await useCase.execute(token, orgId)
  },
)

export const fetchSkillLogsPaginated = createAsyncThunk(
  'skillsDashboard/fetchLogsPaginated',
  async ({ token, orgId, limit, cursor }: { token: string; orgId: number; limit: number; cursor: number }) => {
    const useCase = container.resolve(GetSkillLogsPaginatedUseCase)
    return await useCase.execute(token, orgId, limit, cursor)
  },
)

const skillsDashboardSlice = createSlice({
  name: 'skillsDashboard',
  initialState,
  reducers: {
    setFilter: (
      state,
      action: PayloadAction<{ name: keyof SkillsDashboardFilters; value: string }>,
    ) => {
      const { name, value } = action.payload
      state.filters[name] = value
    },
    clearFilters: (state) => {
      state.filters = { ...initialState.filters }
    },
    setPage: (state, action: PayloadAction<number>) => {
      state.pagination.currentPage = action.payload
      state.pagination.cursor = (action.payload - 1) * state.pagination.itemsPerPage
    },
    setItemsPerPage: (state, action: PayloadAction<number>) => {
      state.pagination.itemsPerPage = action.payload
      state.pagination.currentPage = 1
      state.pagination.cursor = 0
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchSkillsDashboardData.pending, (state) => {
        state.status = 'loading'
        state.error = null
      })
      .addCase(fetchSkillsDashboardData.fulfilled, (state, action) => {
        state.status = 'succeeded'
        state.rawData = Array.isArray(action.payload.logs) ? action.payload.logs : []
        state.totalUsers = action.payload.totalUsers ?? 0
        state.bigNumbers = action.payload.bigNumbers
        state.topDez = action.payload.topDez
      })
      .addCase(fetchSkillsDashboardData.rejected, (state, action) => {
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro ao carregar dados do dashboard'
      })
      .addCase(fetchSkillLogsPaginated.pending, (state) => {
        // Não alterar status principal para não bloquear a página inteira
        state.pagination.isLoading = true
        state.error = null
      })
      .addCase(fetchSkillLogsPaginated.fulfilled, (state, action) => {
        state.pagination.isLoading = false
        // Manter status como 'succeeded' se já estava, ou não alterar se estava 'idle'
        if (state.status === 'idle') {
          state.status = 'succeeded'
        }
        // Garantir que logs seja sempre um array, mesmo se vier nulo/undefined
        state.rawData = Array.isArray(action.payload.logs) ? action.payload.logs : []
        state.pagination.hasMore = action.payload.hasMore ?? false
      })
      .addCase(fetchSkillLogsPaginated.rejected, (state, action) => {
        state.pagination.isLoading = false
        state.status = 'failed'
        state.error = action.error.message ?? 'Erro ao carregar logs'
        // Manter os dados anteriores em caso de erro (não limpar)
        // Não resetar rawData para evitar perda de dados já carregados
      })
  },
})

export const { setFilter, clearFilters, setPage, setItemsPerPage } = skillsDashboardSlice.actions

// Selectors
export const selectFilteredData = createSelector(
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.rawData,
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.filters,
  (data, filters) => {
    return data.filter((item) => {
      const itemNome = item.nome ?? ''
      const itemEvento = item.evento ?? ''
      const matchName = !filters.nome || itemNome.toLowerCase().includes(filters.nome.toLowerCase())
      const matchClient = !filters.cliente || item.cliente === filters.cliente
      const matchType = !filters.tipoSkill || item.tipoSkill === filters.tipoSkill
      
      // Lógica especial para evento: considerar mapeamento
      let matchEvent = true
      if (filters.evento) {
        const backendEvents = mapFilterEventToBackendEvents(filters.evento)
        // Comparar evento do item com os eventos mapeados
        matchEvent = backendEvents.some((backendEvent) => {
          // Comparação case-insensitive
          return itemEvento.toLowerCase() === backendEvent.toLowerCase()
        })
      }

      return matchName && matchClient && matchType && matchEvent
    })
  },
)

export const selectDropdownOptions = createSelector(
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.rawData,
  (data: SkillLogEntry[]): DropdownOptions => {
    return {
      clientes: Array.from<string>(new Set(data.map((d) => d.cliente))).sort(),
      tiposSkill: Array.from<string>(new Set(data.map((d) => d.tipoSkill))).sort(),
      eventos: Array.from<string>(new Set(data.map((d) => d.evento))).sort(),
    }
  },
)

// New selectors for extended data contracts (BigNumbers and TopDez)
export const selectBigNumbers = createSelector(
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.bigNumbers,
  (bigNumbers) => bigNumbers
)

export const selectTopDez = createSelector(
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.topDez,
  (topDez) => topDez
)

export const selectKPIData = createSelector(selectFilteredData, (data): KPIData => {
  return {
    // Usar novos valores da API: "Adicionado Perfil", "Atualizado", "Sugerido", "Adicionado PDI", "Rejeitado"
    added: data.filter((d) => {
      const evento = String(d.evento || '').trim()
      return evento === 'Adicionado Perfil' || evento === 'Atualizado' || 
             evento === 'Inserir' || evento === 'Inserido' ||
             evento === 'ADICIONADO_PERFIL' || evento === 'ATUALIZADO'
    }).length,
    suggested: data.filter((d) => {
      const evento = String(d.evento || '').trim()
      return evento === 'Sugerido' || evento === 'Sugerir' || evento === 'Sugerida' ||
             evento === 'SUGERIDO' || evento === 'SUGERIDA'
    }).length,
    pdi: data.filter((d) => {
      const evento = String(d.evento || '').trim()
      return evento === 'Adicionado PDI' || evento === 'Adicionar ao PDI' || evento === 'Adicionado ao PDI' ||
             evento === 'ADICIONADO_PDI'
    }).length,
    rejected: data.filter((d) => {
      const evento = String(d.evento || '').trim()
      return evento === 'Rejeitado' || evento === 'Rejeitar' ||
             evento === 'REJEITADO' || evento === 'NAO_INTERESSADO'
    }).length,
  }
})

/**
 * Transforms TopDez API entries to ChartDataPoint format
 */
const transformTopDezToChartData = (entries: SkillTopDezEntry[]): ChartDataPoint[] => {
  return entries
    .map((entry) => ({
      name: entry.nomeSkill,
      value: entry.total,
    }))
    .sort((a, b) => b.value - a.value)
    .slice(0, 10) // Ensure top 10
}

/**
 * Gets Skills Adicionadas chart data
 * Combines: ADICIONADO_PERFIL + ATUALIZADO
 * Aggregates by skillId to ensure correct counting when same skill appears in both categories
 */
const getSkillsAdicionadas = (topDez: TopDezData): ChartDataPoint[] => {
  const added = topDez.topDezPorMovimentacao.ADICIONADO_PERFIL || []
  const updated = topDez.topDezPorMovimentacao.ATUALIZADO || []

  // Combine and aggregate by skillId (in case same skill appears in both)
  // Use Map for O(1) lookup performance
  const skillMap = new Map<number, SkillTopDezEntry>()

  // Process ADICIONADO_PERFIL entries
  added.forEach((entry) => {
    const existing = skillMap.get(entry.skillId)
    if (existing) {
      // Same skillId found: sum the totals
      existing.total += entry.total
    } else {
      // New skillId: add to map
      skillMap.set(entry.skillId, { ...entry })
    }
  })

  // Process ATUALIZADO entries
  updated.forEach((entry) => {
    const existing = skillMap.get(entry.skillId)
    if (existing) {
      // Same skillId found: sum the totals
      existing.total += entry.total
    } else {
      // New skillId: add to map
      skillMap.set(entry.skillId, { ...entry })
    }
  })

  // Convert map values to array and transform to chart data
  const combined = Array.from(skillMap.values())
  return transformTopDezToChartData(combined)
}

/**
 * Gets Skills Sugeridas chart data
 * Uses: SUGERIDA
 */
const getSkillsSugeridas = (topDez: TopDezData): ChartDataPoint[] => {
  const suggested = topDez.topDezPorMovimentacao.SUGERIDA || []
  return transformTopDezToChartData(suggested)
}

/**
 * Gets Skills no PDI chart data
 * Uses: ADICIONADO_PDI
 */
const getSkillsPDI = (topDez: TopDezData): ChartDataPoint[] => {
  const pdi = topDez.topDezPorMovimentacao.ADICIONADO_PDI || []
  return transformTopDezToChartData(pdi)
}

/**
 * Gets Skills Rejeitadas chart data
 * Uses: NAO_INTERESSADO (movimentacao === 2)
 */
const getSkillsRejeitadas = (topDez: TopDezData): ChartDataPoint[] => {
  const naoInteressado = topDez.topDezPorMovimentacao.NAO_INTERESSADO || []
  // NAO_INTERESSADO represents rejected skills (movimentacao === 2)
  return transformTopDezToChartData(naoInteressado)
}

export const selectChartsData = createSelector(
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.topDez,
  selectFilteredData,
  (state: { skillsDashboard: SkillsDashboardState }) => state.skillsDashboard.totalUsers,
  (topDez, data, totalUsers): ChartsData => {
    // Pie chart still uses logs data
    const uniqueUsersInLog = new Set(data.map((d) => d.nome)).size
    const inactiveUsers = Math.max(0, totalUsers - uniqueUsersInLog)
    const activePercent = totalUsers > 0 ? ((uniqueUsersInLog / totalUsers) * 100).toFixed(1) : '0'
    const inactivePercent = totalUsers > 0 ? ((inactiveUsers / totalUsers) * 100).toFixed(1) : '0'

    // Bar charts use TopDez data
    if (topDez?.topDezPorMovimentacao) {
      return {
        barAdded: getSkillsAdicionadas(topDez),
        barSuggested: getSkillsSugeridas(topDez),
        barPDI: getSkillsPDI(topDez),
        barRejected: getSkillsRejeitadas(topDez),
        pieUsage: [
          { name: 'Usaram a Feature', value: parseFloat(activePercent) },
          { name: 'Não usaram a feature', value: parseFloat(inactivePercent) },
        ],
      }
    }

    // Fallback: return empty arrays if TopDez data is not available
    return {
      barAdded: [],
      barSuggested: [],
      barPDI: [],
      barRejected: [],
      pieUsage: [
        { name: 'Usaram a Feature', value: parseFloat(activePercent) },
        { name: 'Não usaram a feature', value: parseFloat(inactivePercent) },
      ],
    }
  },
)

export { skillsDashboardSlice }
export default skillsDashboardSlice.reducer

