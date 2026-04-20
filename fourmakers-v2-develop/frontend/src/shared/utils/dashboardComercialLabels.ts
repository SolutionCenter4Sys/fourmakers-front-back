import type { FiltrosComercial } from '@shared/types/dashboardComercialTypes'

/**
 * Formata string de data do backend (ISO ou YYYY-MM-DD) para dd/MM/yyyy.
 * REGRA: NUNCA normalizar data com timezone. Tratar sempre como data (dia de calendário), sem criar Date.
 * Usa apenas a parte da data (primeiros 10 caracteres) para evitar qualquer deslocamento por fuso.
 * 
 * @param isoOuData - String de data (YYYY-MM-DD ou ISO com hora)
 * @param valorPadrao - Valor a retornar se a data for inválida (padrão: '—')
 */
function formatarDataSemTimezone(isoOuData: string | null | undefined, valorPadrao = '—'): string {
  if (!isoOuData?.trim()) return valorPadrao
  const s = isoOuData.trim()
  if (s.length >= 10) {
    const y = s.slice(0, 4)
    const m = s.slice(5, 7)
    const d = s.slice(8, 10)
    if (/^\d{4}$/.test(y) && /^\d{2}$/.test(m) && /^\d{2}$/.test(d)) return `${d}/${m}/${y}`
  }
  return valorPadrao === '—' ? s : valorPadrao
}

/**
 * Formata data para exibição em drilldown (tabelas de detalhes).
 * Retorna '—' se a data for inválida ou vazia.
 */
export function formatarApenasDataDrilldown(isoOuData: string | null | undefined): string {
  return formatarDataSemTimezone(isoOuData, '—')
}

export function obterLabelPeriodo(filtros: FiltrosComercial): string {
  const inicio = filtros.dataInicio?.trim()
  const fim = filtros.dataFim?.trim()
  if (!inicio && !fim) return 'Todos'
  const dataInicioStr = inicio ? formatarDataSemTimezone(inicio, '') : ''
  const dataFimStr = fim ? formatarDataSemTimezone(fim, '') : ''
  if (dataInicioStr && dataFimStr) return `${dataInicioStr} - ${dataFimStr}`
  return dataInicioStr || dataFimStr || 'Todos'
}

/** Label de cliente para exibição: usa nome quando disponível, senão texto genérico (sem expor código). */
export function obterLabelCliente(
  codigoCliente: string | null,
  nomeClienteSelecionado?: string | null
): string {
  if (codigoCliente == null || codigoCliente.trim() === '') return 'Todos os clientes'
  if (nomeClienteSelecionado != null && nomeClienteSelecionado.trim() !== '') {
    return nomeClienteSelecionado.trim()
  }
  return 'Cliente selecionado'
}

/** Label comercial para exibição: usa nome do gestor/colaborador quando disponível, senão texto genérico (sem expor código). */
export function obterLabelComercial(filtros: FiltrosComercial): string {
  if (filtros.nomeComercialSelecionado != null && filtros.nomeComercialSelecionado.trim() !== '') {
    return filtros.nomeComercialSelecionado.trim()
  }
  if (filtros.tipoComercial === 'gestor-externo' && filtros.codigoGestorExterno) {
    return 'Por gestor'
  }
  if (filtros.tipoComercial === 'colaborador' && filtros.codigoColaboradorAgendou) {
    return 'Por colaborador'
  }
  return 'Todos'
}
