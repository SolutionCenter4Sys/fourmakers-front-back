import { AlertCircle, AlertTriangle, Info, PlayCircle, Calendar, PauseCircle, CheckCircle2, PlusCircle, Edit, Trash2 } from 'lucide-react'
import type { StatusIniciativa } from '@domain/entities/Vcx360'
import type { LucideIcon } from 'lucide-react'

/**
 * Retorna classes CSS customizadas para o Badge baseado no status da iniciativa
 * Status usa cores sutis (menos vibrantes) em comparação com Impacto/Urgência
 * IMPORTANTE: Não altera o valor recebido do endpoint, apenas retorna classes CSS
 * @param status - Status da iniciativa
 * @returns Classes CSS para estilização do badge com cores sutis
 */
export function getStatusColor(status: StatusIniciativa): string {
  // Status usa cores sutis (menos vibrantes que Impacto/Urgência)
  // Cada status tem uma cor sutil diferente para diferenciação visual
  switch (status) {
    case 'Ativa':
      // Verde muito suave (sutil)
      return 'bg-success/5 text-success/80 border border-success/20 !border-success/20 dark:bg-success/10 dark:text-success/70 dark:border-success/30 dark:!border-success/30'
    case 'Em Planejamento':
      // Azul muito suave (sutil)
      return 'bg-info/5 text-info/80 border border-info/20 !border-info/20 dark:bg-info/10 dark:text-info/70 dark:border-info/30 dark:!border-info/30'
    case 'Pausada':
      // Amarelo muito suave (sutil)
      return 'bg-warning/5 text-warning/80 border border-warning/20 !border-warning/20 dark:bg-warning/10 dark:text-warning/70 dark:border-warning/30 dark:!border-warning/30'
    case 'Concluída':
      // Verde escuro (dark green) - sutil mas mais vibrante que outros status
      return 'bg-success/10 text-success border border-success/30 !border-success/30 dark:bg-success/20 dark:text-success dark:border-success/40 dark:!border-success/40'
    default:
      // Fallback neutro
      return 'bg-muted/10 text-muted-foreground border border-borderDefault !border-borderDefault dark:bg-muted/20 dark:text-muted-foreground dark:border-borderDefault dark:!border-borderDefault'
  }
}

/**
 * Retorna classes CSS para o Badge de Impacto baseado na descrição real da API
 * Usa tokens do Design System (destructive, warning, info, muted) em vez de cores hardcoded
 * IMPORTANTE: Não altera o valor recebido do endpoint, apenas retorna classes CSS
 * @param descricao - Descrição do impacto (ex: "Impacto Alto", "Impacto Médio", "Impacto Baixo")
 * @returns Classes CSS para estilização do badge com tokens do Design System
 */
export function getImpactBadgeColor(descricao: string | null): string {
  if (!descricao) {
    return 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
  }

  const normalized = descricao.toLowerCase().trim()

  // Impacto Alto = Vermelho (destructive)
  if (normalized.includes('alto') || normalized.includes('alta')) {
    return 'bg-destructive/10 text-destructive border border-destructive/30 !border-destructive/30 dark:bg-destructive/20 dark:text-destructive dark:border-destructive/40 dark:!border-destructive/40'
  }

  // Impacto Médio = Amarelo (warning)
  // Verifica todas as variações: "medio", "média", "médio", "media"
  if (normalized.includes('medio') || normalized.includes('média') || normalized.includes('médio') || normalized.includes('media')) {
    return 'bg-warning/10 text-warning border border-warning/30 !border-warning/30 dark:bg-warning/20 dark:text-warning dark:border-warning/40 dark:!border-warning/40'
  }

  // Impacto Baixo = Azul (info)
  if (normalized.includes('baixo') || normalized.includes('baixa')) {
    return 'bg-info/10 text-info border border-info/30 !border-info/30 dark:bg-info/20 dark:text-info dark:border-info/40 dark:!border-info/40'
  }

  // Fallback
  return 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
}

/**
 * Retorna classes CSS para o Badge de Urgência baseado na descrição real da API
 * Usa tokens do Design System (destructive, warning, info, muted) em vez de cores hardcoded
 * IMPORTANTE: Não altera o valor recebido do endpoint, apenas retorna classes CSS
 * @param descricao - Descrição da urgência (ex: "Urgência Alta", "Urgência Média", "Urgência Baixa")
 * @returns Classes CSS para estilização do badge com tokens do Design System
 */
export function getUrgencyBadgeColor(descricao: string | null): string {
  if (!descricao) {
    return 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
  }

  const normalized = descricao.toLowerCase().trim()

  // Urgência Alta = Vermelho (destructive)
  if (normalized.includes('alta') || normalized.includes('alto') || normalized.includes('urgente')) {
    return 'bg-destructive/10 text-destructive border border-destructive/30 !border-destructive/30 dark:bg-destructive/20 dark:text-destructive dark:border-destructive/40 dark:!border-destructive/40'
  }

  // Urgência Média = Amarelo (warning) - MESMO PADRÃO QUE IMPACTO MÉDIO
  if (normalized.includes('media') || normalized.includes('média') || normalized.includes('médio') || normalized.includes('medio') || normalized.includes('normal')) {
    return 'bg-warning/10 text-warning border border-warning/30 !border-warning/30 dark:bg-warning/20 dark:text-warning dark:border-warning/40 dark:!border-warning/40'
  }

  // Urgência Baixa = Azul (info)
  if (normalized.includes('baixa') || normalized.includes('baixo')) {
    return 'bg-info/10 text-info border border-info/30 !border-info/30 dark:bg-info/20 dark:text-info dark:border-info/40 dark:!border-info/40'
  }

  // Fallback
  return 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
}

/**
 * Retorna classes CSS para o Badge de Ação (Criado, Atualizado, Excluído)
 * Usa tokens do Design System (success, info, destructive, muted) em vez de cores hardcoded
 * IMPORTANTE: Não altera o valor recebido do endpoint, apenas retorna classes CSS
 * @param acao - Ação do log ('INSERT' | 'UPDATE' | 'DELETE')
 * @returns Classes CSS para estilização do badge com tokens do Design System
 */
export function getAcaoBadgeColor(acao: 'INSERT' | 'UPDATE' | 'DELETE'): string {
  switch (acao) {
    case 'INSERT':
      // Criado = Verde (success)
      return 'bg-success/10 text-success border border-success/30 !border-success/30 dark:bg-success/20 dark:text-success dark:border-success/40 dark:!border-success/40'
    case 'UPDATE':
      // Atualizado = Azul (info)
      return 'bg-info/10 text-info border border-info/30 !border-info/30 dark:bg-info/20 dark:text-info dark:border-info/40 dark:!border-info/40'
    case 'DELETE':
      // Excluído = Vermelho (destructive)
      return 'bg-destructive/10 text-destructive border border-destructive/30 !border-destructive/30 dark:bg-destructive/20 dark:text-destructive dark:border-destructive/40 dark:!border-destructive/40'
    default:
      return 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
  }
}

/**
 * Retorna o ícone apropriado para o nível de Impacto
 * @param descricao - Descrição do impacto (ex: "Impacto Alto", "Impacto Médio", "Impacto Baixo")
 * @returns Componente de ícone do Lucide React
 */
export function getImpactIcon(descricao: string | null): LucideIcon | null {
  if (!descricao) return null

  const normalized = descricao.toLowerCase().trim()

  if (normalized.includes('alto') || normalized.includes('alta')) {
    return AlertCircle // Vermelho - crítico
  }

  if (normalized.includes('medio') || normalized.includes('média') || normalized.includes('médio') || normalized.includes('media')) {
    return AlertTriangle // Amarelo - atenção
  }

  if (normalized.includes('baixo') || normalized.includes('baixa')) {
    return Info // Azul - informativo
  }

  return null
}

/**
 * Retorna o ícone apropriado para o nível de Urgência
 * @param descricao - Descrição da urgência (ex: "Urgência Alta", "Urgência Média", "Urgência Baixa")
 * @returns Componente de ícone do Lucide React
 */
export function getUrgencyIcon(descricao: string | null): LucideIcon | null {
  if (!descricao) return null

  const normalized = descricao.toLowerCase().trim()

  if (normalized.includes('alta') || normalized.includes('alto') || normalized.includes('urgente')) {
    return AlertCircle // Vermelho - crítico
  }

  if (normalized.includes('media') || normalized.includes('média') || normalized.includes('normal')) {
    return AlertTriangle // Amarelo - atenção
  }

  if (normalized.includes('baixa') || normalized.includes('baixo')) {
    return Info // Azul - informativo
  }

  return null
}

/**
 * Valida e normaliza uma descrição de status para o tipo StatusIniciativa
 * @param descricao - Descrição do status (pode vir do backend em diferentes formatos)
 * @returns StatusIniciativa válido ou null se não for reconhecido
 */
export function validarStatusIniciativa(descricao: string | null | undefined): StatusIniciativa | null {
  if (!descricao) return null
  
  const normalized = descricao.toLowerCase().trim()
  
  const statusMap: Record<string, StatusIniciativa> = {
    'ativa': 'Ativa',
    'em planejamento': 'Em Planejamento',
    'pausada': 'Pausada',
    'concluída': 'Concluída',
    'concluida': 'Concluída', // Sem acento
  }
  
  return statusMap[normalized] || null
}

/**
 * Retorna o ícone apropriado para o status da iniciativa
 * Mantém padrão consistente com ícones de Impacto/Urgência
 * @param status - Status da iniciativa
 * @returns Componente de ícone do Lucide React
 */
export function getStatusIcon(status: StatusIniciativa): LucideIcon | null {
  switch (status) {
    case 'Ativa':
      return PlayCircle // Verde - em execução/ativa
    case 'Em Planejamento':
      return Calendar // Azul - planejamento
    case 'Pausada':
      return PauseCircle // Amarelo - pausada
    case 'Concluída':
      return CheckCircle2 // Cinza - concluída
    default:
      return null
  }
}

/**
 * Retorna o ícone apropriado para a ação do log
 * Mantém padrão consistente com ícones de Impacto/Urgência/Status
 * @param acao - Ação do log ('INSERT' | 'UPDATE' | 'DELETE')
 * @returns Componente de ícone do Lucide React
 */
export function getAcaoIcon(acao: 'INSERT' | 'UPDATE' | 'DELETE'): LucideIcon | null {
  switch (acao) {
    case 'INSERT':
      return PlusCircle // Verde - criado
    case 'UPDATE':
      return Edit // Azul - atualizado
    case 'DELETE':
      return Trash2 // Vermelho - excluído
    default:
      return null
  }
}
