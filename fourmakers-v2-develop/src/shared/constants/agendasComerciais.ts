/** Range padrão de datas para agendas comerciais (início = dias relativos a hoje, fim = dias relativos a hoje). */
export const RANGE_DIAS_AGENDA = { inicio: -7, fim: 15 } as const

const MS_POR_DIA = 24 * 60 * 60 * 1000

/**
 * Retorna as datas padrão para o período de agendas comerciais.
 * Fonte única de verdade para o range -7 dias / +15 dias usado na API e nos filtros.
 */
export function obterDatasPadraoAgenda(): { dataInicio: Date; dataFim: Date } {
  const hoje = new Date()
  return {
    dataInicio: new Date(hoje.getTime() + RANGE_DIAS_AGENDA.inicio * MS_POR_DIA),
    dataFim: new Date(hoje.getTime() + RANGE_DIAS_AGENDA.fim * MS_POR_DIA),
  }
}
