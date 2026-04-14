import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

/**
 * Exporta a estrutura do mapa de relacionamento como arquivo JSON
 */
export function exportarMapaComoJSON(
  estruturaMapa: NoMapaRelacionamento,
  clientName: string,
  codigoCliente: string
): void {
  const dataToExport = {
    ...estruturaMapa,
    clientName: clientName || 'MapaRelacionamento',
    exportedAt: new Date().toISOString(),
  }
  
  const jsonString = JSON.stringify(dataToExport, null, 2)
  const blob = new Blob([jsonString], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  
  const clientSafeName = (clientName || 'mapa-relacionamento')
    .replace(/[^a-z0-9]/gi, '-')
    .toLowerCase()
  a.download = `mapa-relacionamento-${codigoCliente || 'export'}-${clientSafeName}.json`
  
  document.body.appendChild(a)
  a.click()
  URL.revokeObjectURL(url)
  document.body.removeChild(a)
}
