import { configureStore, combineReducers } from '@reduxjs/toolkit'

import { setupDependencyInjection } from '@core/di/container'

import authReducer from './slices/authSlice'
import menuReducer from './slices/menuSlice'
import colaboradoresReducer from './slices/colaboradoresSlice'
import parametrosReducer from './slices/parametrosSlice'
import modeloTrabalhoReducer from './slices/modeloTrabalhoSlice'
import simulatorReducer from './slices/simulatorSlice'
import clienteCampanhaReducer from './slices/clienteCampanhaSlice'
import skillsDashboardReducer from './slices/skillsDashboardSlice'
import mapaRelacionamentoReducer from './slices/mapaRelacionamentoSlice'
import vcx360Reducer from './slices/vcx360Slice'
import vcxDoresReducer from './slices/vcxDoresSlice'
import vcxIniciativasReducer from './slices/vcxIniciativasSlice'
import agendaGestorReducer from './slices/agendaGestorSlice'
import agendasComerciaisReducer from './slices/agendasComerciaisSlice'
import dashboardComercialReducer from './slices/dashboardComercialSlice'


setupDependencyInjection()

// Combinar todos os reducers
const appReducer = combineReducers({
  auth: authReducer,
  menu: menuReducer,
  colaboradores: colaboradoresReducer,
  parametros: parametrosReducer,
  modeloTrabalho: modeloTrabalhoReducer,
  simulator: simulatorReducer,
  clienteCampanha: clienteCampanhaReducer,
  skillsDashboard: skillsDashboardReducer,
  mapaRelacionamento: mapaRelacionamentoReducer,
  vcx360: vcx360Reducer,
  vcxDores: vcxDoresReducer,
  vcxIniciativas: vcxIniciativasReducer,
  agendaGestor: agendaGestorReducer,
  agendasComerciais: agendasComerciaisReducer,
  dashboardComercial: dashboardComercialReducer,
})

// Root reducer que reseta todos os states no logout
const rootReducer = (state: ReturnType<typeof appReducer> | undefined, action: any) => {
  // Se for uma action de logout fulfilled ou rejected, reseta todo o state
  if (action.type === 'auth/logout/fulfilled' || action.type === 'auth/logout/rejected') {
    state = undefined
  }
  return appReducer(state, action)
}

export const store = configureStore({
  reducer: rootReducer,
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch

