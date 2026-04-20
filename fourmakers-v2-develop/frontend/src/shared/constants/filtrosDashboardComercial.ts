import { subMonths, addMonths, format } from 'date-fns'

/**
 * Constantes dos filtros do Dashboard Comercial.
 * Filtros de período: dataInicio e dataFim (date pickers).
 */

/** Período inicial do dashboard: 1 mês atrás e 1 mês à frente (melhor performance). */
export function getPeriodoInicialDashboardComercial(): { dataInicio: string; dataFim: string } {
  const hoje = new Date()
  return {
    dataInicio: format(subMonths(hoje, 1), 'yyyy-MM-dd'),
    dataFim: format(addMonths(hoje, 1), 'yyyy-MM-dd'),
  }
}
