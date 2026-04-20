/**
 * Utilitários centralizados para Skills Dashboard
 * Garante consistência de cores e labels em toda a aplicação
 * 
 * Seguindo a arquitetura Clean Architecture:
 * - Este arquivo está em shared/utils, permitindo uso em todas as camadas
 * - Centraliza lógica de negócio relacionada a eventos de skills
 * - Evita duplicação de código e garante consistência
 */

/**
 * Lista de eventos disponíveis para filtros
 * Valores são os que vêm da API no novo formato
 */
export const EVENTOS_DISPONIVEIS = [
  { value: 'Adicionado Perfil', label: 'Adicionado ao Perfil' },
  { value: 'Adicionado PDI', label: 'Adicionado ao PDI' },
  { value: 'Sugerido', label: 'Sugerido' },
  { value: 'Rejeitado', label: 'Rejeitado' },
] as const

/**
 * Mapeia valores de evento do backend para textos legíveis
 * Novo formato da API: "Adicionado Perfil", "Adicionado PDI", "Sugerido", "Rejeitado", "Atualizado"
 */
export const getEventLabel = (evento: string): string => {
  if (!evento) return ''
  
  // Normalizar o evento removendo espaços e convertendo para uppercase para comparação
  const normalized = evento.trim().toUpperCase()
  
  const eventMap: Record<string, string> = {
    // Novos valores da API (formato exato)
    'Adicionado Perfil': 'Adicionado ao Perfil',
    'Adicionado PDI': 'Adicionado ao PDI',
    'Sugerido': 'Sugerido',
    'Rejeitado': 'Rejeitado',
    'Atualizado': 'Adicionado ao Perfil', // Atualizado se comporta igual a Adicionado Perfil
    // Valores antigos (mantidos para compatibilidade)
    'ADICIONADO_PERFIL': 'Adicionado ao Perfil',
    'ATUALIZADO': 'Adicionado ao Perfil',
    'ADICIONADO_PDI': 'Adicionado ao PDI',
    'SUGERIDO': 'Sugerido',
    'SUGERIDA': 'Sugerido',
    'NAO_INTERESSADO': 'Rejeitado',
    'REJEITADO': 'Rejeitado',
    'Inserir': 'Adicionado ao Perfil',
    'Sugerir': 'Sugerido',
    'Sugerida': 'Sugerido',
    'Adicionar ao PDI': 'Adicionado ao PDI',
    'Rejeitar': 'Rejeitado',
  }
  
  // Tentar primeiro com o valor original (case-sensitive)
  if (eventMap[evento]) {
    return eventMap[evento]
  }
  
  // Tentar com o valor normalizado (uppercase)
  if (eventMap[normalized]) {
    return eventMap[normalized]
  }
  
  // Se não encontrou, retornar o valor original
  return evento
}

/**
 * Retorna classes CSS para estilização do badge de evento
 * Cores alinhadas com BigNumbers:
 * - Adicionado ao Perfil: blue (from-blue-500 to-blue-600)
 * - Sugerido: indigo (from-indigo-500 to-indigo-600)
 * - Adicionado PDI: emerald (from-emerald-500 to-emerald-600)
 * - Rejeitado: rose (from-rose-500 to-rose-600)
 * 
 * Esta função é centralizada para garantir consistência em toda a aplicação
 */
export const getEventBadgeStyle = (evento: string): string => {
  if (!evento) {
    return 'bg-gray-500/10 text-gray-700 border-gray-300 dark:bg-gray-500/20 dark:text-gray-400 dark:border-gray-600'
  }
  
  // Normalizar o evento removendo espaços e convertendo para uppercase para comparação
  const normalizedEvent = evento.trim().toUpperCase()
  
  // Adicionado ao Perfil (inclui "Atualizado" e "Adicionado Perfil")
  if (
    evento === 'Adicionado Perfil' ||
    evento === 'Atualizado' ||
    normalizedEvent === 'ADICIONADO_PERFIL' ||
    normalizedEvent === 'ATUALIZADO' ||
    normalizedEvent.includes('ADICIONADO_PERFIL') ||
    normalizedEvent.includes('ATUALIZADO') ||
    evento === 'Inserir' ||
    evento === 'Inserido'
  ) {
    return 'bg-blue-500/10 text-blue-700 border-blue-300 dark:bg-blue-500/20 dark:text-blue-400 dark:border-blue-600'
  }
  
  // Adicionado PDI
  if (
    evento === 'Adicionado PDI' ||
    normalizedEvent === 'ADICIONADO_PDI' ||
    normalizedEvent.includes('ADICIONADO_PDI') ||
    evento === 'Adicionar ao PDI' ||
    evento === 'Adicionado ao PDI'
  ) {
    return 'bg-emerald-500/10 text-emerald-700 border-emerald-300 dark:bg-emerald-500/20 dark:text-emerald-400 dark:border-emerald-600'
  }
  
  // Sugerido (usa indigo para corresponder ao BigNumbers)
  if (
    evento === 'Sugerido' ||
    normalizedEvent === 'SUGERIDO' ||
    normalizedEvent === 'SUGERIDA' ||
    normalizedEvent.includes('SUGERIDO') ||
    normalizedEvent.includes('SUGERIDA') ||
    evento === 'Sugerir' ||
    evento === 'Sugerida'
  ) {
    return 'bg-indigo-500/10 text-indigo-700 border-indigo-300 dark:bg-indigo-500/20 dark:text-indigo-400 dark:border-indigo-600'
  }
  
  // Rejeitado
  if (
    evento === 'Rejeitado' ||
    normalizedEvent === 'REJEITADO' ||
    normalizedEvent === 'NAO_INTERESSADO' ||
    normalizedEvent.includes('REJEITADO') ||
    normalizedEvent.includes('NAO_INTERESSADO') ||
    evento === 'Rejeitar'
  ) {
    return 'bg-rose-500/10 text-rose-700 border-rose-300 dark:bg-rose-500/20 dark:text-rose-400 dark:border-rose-600'
  }
  
  // Fallback
  return 'bg-gray-500/10 text-gray-700 border-gray-300 dark:bg-gray-500/20 dark:text-gray-400 dark:border-gray-600'
}

/**
 * Mapeia valores de evento do filtro para valores reais do backend
 * Usado no slice para filtrar dados corretamente
 * Novo formato da API: "Adicionado Perfil", "Adicionado PDI", "Sugerido", "Rejeitado", "Atualizado"
 * Atualizado deve ser tratado como Adicionado Perfil
 * 
 * Esta função centraliza a lógica de mapeamento para garantir consistência entre filtros e dados
 */
export const mapFilterEventToBackendEvents = (filterEvent: string): string[] => {
  if (!filterEvent) return []
  
  const normalizedEvent = filterEvent.toUpperCase()
  
  // Adicionado Perfil (inclui "Atualizado" e variações antigas)
  if (
    filterEvent === 'Adicionado Perfil' ||
    normalizedEvent === 'ADICIONADO_PERFIL' ||
    normalizedEvent.includes('ADICIONADO_PERFIL')
  ) {
    return [
      'Adicionado Perfil',
      'Atualizado',
      'Inserir',
      'Inserido',
      'ADICIONADO_PERFIL',
      'ATUALIZADO',
    ]
  }
  
  // Adicionado PDI
  if (
    filterEvent === 'Adicionado PDI' ||
    normalizedEvent === 'ADICIONADO_PDI' ||
    normalizedEvent.includes('ADICIONADO_PDI')
  ) {
    return [
      'Adicionado PDI',
      'Adicionar ao PDI',
      'Adicionado ao PDI',
      'ADICIONADO_PDI',
    ]
  }
  
  // Sugerido
  if (
    filterEvent === 'Sugerido' ||
    normalizedEvent === 'SUGERIDO' ||
    normalizedEvent === 'SUGERIDA' ||
    normalizedEvent.includes('SUGERIDO') ||
    normalizedEvent.includes('SUGERIDA')
  ) {
    return [
      'Sugerido',
      'Sugerir',
      'Sugerida',
      'SUGERIDO',
      'SUGERIDA',
    ]
  }
  
  // Rejeitado
  if (
    filterEvent === 'Rejeitado' ||
    normalizedEvent === 'REJEITADO' ||
    normalizedEvent === 'NAO_INTERESSADO' ||
    normalizedEvent.includes('REJEITADO') ||
    normalizedEvent.includes('NAO_INTERESSADO')
  ) {
    return [
      'Rejeitado',
      'Rejeitar',
      'REJEITADO',
      'NAO_INTERESSADO',
    ]
  }
  
  // Fallback: retornar o valor original
  return [filterEvent]
}
