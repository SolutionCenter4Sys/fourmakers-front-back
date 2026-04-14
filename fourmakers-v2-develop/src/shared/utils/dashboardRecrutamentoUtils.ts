import type {
  FiltrosCandidaturaParams,
  DashboardMetricasVagasEmFocoItem,
} from '@domain/entities/DashboardMetricasRecrutamento'
import type { ClienteGestaoAlocados } from '@domain/entities/ClienteGestaoAlocados'
import type { RecrutadorGestaoAlocados } from '@domain/entities/RecrutadorGestaoAlocados'

/** Estado local dos filtros na página do Dashboard de Recrutamento */
export type FiltrosDashboard = {
  dataInicio: string
  dataFim: string
  clientes: string[]
  recrutadores: string[]
}

/** Linha exibida na tabela Vagas em Foco no modal */
export type VagaEmFocoRow = {
  cliente: string
  codVaga: string
  vaga: string
  status: string
  responsavel: string
  ultimaMovimentacao: string
  ultimaMovimentacaoDate: Date
  tempoEtapa: string
  sla: string
}

/** Filtros padrão: DataFim = hoje, DataInicio = hoje - 30 dias (YYYY-MM-DD) */
export function getDefaultFiltrosCandidaturaParams(): FiltrosCandidaturaParams {
  const dataFim = new Date()
  const dataInicio = new Date(dataFim)
  dataInicio.setDate(dataInicio.getDate() - 30)
  return {
    DataInicio: dataInicio.toISOString().slice(0, 10),
    DataFim: dataFim.toISOString().slice(0, 10),
  }
}

/** Converte estado dos filtros da página para FiltrosCandidaturaParams; usa datas padrão quando vazias. CodigoRecrutador é alimentado com codigoRecrutador (api/Candidatura/RecrutadorListagem). */
export function buildFiltrosParams(
  filtros: FiltrosDashboard,
  clientesSelecionados: ClienteGestaoAlocados[],
  recrutadoresSelecionados: RecrutadorGestaoAlocados[],
  defaultDates: FiltrosCandidaturaParams,
): FiltrosCandidaturaParams {
  const codigosRecrutador =
    recrutadoresSelecionados.length > 0
      ? recrutadoresSelecionados
          .map((r) => r.codigoRecrutador ?? r.codGestorExterno ?? r.codigoInternoColaborador)
          .filter((c): c is string => Boolean(c?.trim()))
      : undefined
  return {
    DataInicio: filtros.dataInicio || defaultDates.DataInicio,
    DataFim: filtros.dataFim || defaultDates.DataFim,
    CodigoCliente:
      clientesSelecionados.length > 0
        ? clientesSelecionados.map((c) => c.codigoCliente)
        : undefined,
    CodigoRecrutador: codigosRecrutador?.length ? codigosRecrutador : undefined,
  }
}

/** Retorna o intervalo de datas efetivo (filtro ou padrão) e a quantidade de dias entre elas */
export function obterIntervaloEDias(
  dataInicio: string,
  dataFim: string,
  defaultDataInicio: string,
  defaultDataFim: string,
): { dataInicio: string; dataFim: string; dias: number } {
  const inicioStr = dataInicio || defaultDataInicio || ''
  const fimStr = dataFim || defaultDataFim || ''
  const inicio = inicioStr ? new Date(inicioStr + 'T00:00:00') : new Date()
  const fim = fimStr ? new Date(fimStr + 'T23:59:59') : new Date()
  const dias = Math.max(0, Math.ceil((fim.getTime() - inicio.getTime()) / (1000 * 60 * 60 * 24)))
  return { dataInicio: inicioStr, dataFim: fimStr, dias }
}

/** Formata data YYYY-MM-DD para dd/MM/yyyy */
export function formatarDataBR(isoDate: string): string {
  if (!isoDate) return ''
  const d = new Date(isoDate + 'T12:00:00')
  if (Number.isNaN(d.getTime())) return isoDate
  return d.toLocaleDateString('pt-BR')
}

/** Formata ISO para exibição dd/MM/yyyy HH:mm */
export function formatarDataHora(iso: string): string {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return iso
  return d.toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

/** Converte item da API para linha da tabela Vagas em Foco (usa codVaga ou CodVaga do contrato) */
export function vagasEmFocoItemToRow(item: DashboardMetricasVagasEmFocoItem): VagaEmFocoRow {
  const rawCodVaga = item.codVaga ?? item.CodVaga
  const codVaga =
    rawCodVaga != null && String(rawCodVaga).trim() !== ''
      ? String(rawCodVaga).trim()
      : '-'
  return {
    cliente: item.cliente,
    codVaga,
    vaga: item.vaga,
    status: item.status,
    responsavel: item.responsavel ?? '',
    ultimaMovimentacao: formatarDataHora(item.ultimaMovimentacao),
    ultimaMovimentacaoDate: new Date(item.ultimaMovimentacao),
    tempoEtapa: item.tempoNaEtapa === 1 ? '1 dia' : `${item.tempoNaEtapa} dias`,
    sla: item.sla ?? '',
  }
}
