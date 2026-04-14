/**
 * Utilitários compartilhados para o Mapa de Relacionamento
 * Centraliza validações e formatações usadas em múltiplos componentes
 */

export const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'

/**
 * Verifica se uma posição está vaga (sem colaborador alocado)
 * 
 * @param employeeId - ID do colaborador na posição
 * @returns true se a posição está vaga, false caso contrário
 */
export function isPosicaoVaga(employeeId: string | null | undefined): boolean {
  if (!employeeId) return true
  if (typeof employeeId !== 'string') return true
  return employeeId === 'vacant' || employeeId.trim() === '' || employeeId === EMPTY_GUID
}

/**
 * Extrai as iniciais de um nome para exibição em avatares
 * 
 * @param name - Nome completo da pessoa
 * @param fallback - Valor padrão caso o nome seja inválido (default: '??')
 * @returns Iniciais em maiúsculas (máximo 2 caracteres)
 * 
 * @example
 * getInitials('João Silva') // 'JS'
 * getInitials('Maria') // 'MA'
 * getInitials('') // '??'
 * getInitials(null, 'N/A') // 'N/A'
 */
export function getInitials(name: string | null | undefined, fallback = '??'): string {
  if (!name || !name.trim()) return fallback
  return name.split(' ').map((n) => n[0]).join('').toUpperCase().slice(0, 2)
}
