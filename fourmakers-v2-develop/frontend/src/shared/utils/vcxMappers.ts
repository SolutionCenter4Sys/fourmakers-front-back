// Funções utilitárias para mapear dados entre formato do protótipo e backend VCX

import type { DorPayload, DorResponse, ImpactoResponse, UrgenciaResponse } from '@domain/entities/VcxDores'
import type { IniciativaResponse } from '@domain/entities/VcxIniciativas'
import type { StatusIniciativa } from '@domain/entities/Vcx360'

const normalizeDescricao = (value?: string | null): string =>
  (value || '')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .trim()
    .toLowerCase()

const stripPrefix = (label: string): string => {
  return label
    .replace(/^(impacto|urgencia)\s+/i, '')
    .replace(/^(impacto|urgencia)\s*:\s*/i, '')
    .trim()
}

/**
 * Interface do protótipo (formato usado no componente SidebarVCX)
 * Esta interface está definida em visao-360-cliente/types.ts
 */
export interface PainOpportunity {
  id: string
  title: string
  description: string
  priority: 'Alta' | 'Média' | 'Baixa' // Mantido para compatibilidade com formulários
  impact: 'Alto' | 'Médio' | 'Baixo' // Mantido para compatibilidade com formulários
  dataCriacao: string
  // Campos com valores reais da API para exibição nos badges
  vcxUrgenciasDescricao: string | null // Valor real: "Urgência Alta", "Urgência Média", etc.
  vcxImpactosDescricao: string | null // Valor real: "Impacto Alto", "Impacto Médio", etc.
}

/**
 * Mapeia uma Dor do formato do protótipo para o payload do backend
 * @param dor - Dor no formato do protótipo (PainOpportunity)
 * @param posicaoId - ID da posição do organograma (GUID)
 * @returns Payload no formato esperado pelo backend
 */
export function mapearDorParaPayload(
  dor: PainOpportunity,
  posicaoId: string,
): DorPayload {
  return {
    organogramaPosicaoId: posicaoId,
    titulo: dor.title,
    descricao: dor.description || null,
    // Os IDs de impacto e urgência precisam ser obtidos dos dropdowns
    // Por enquanto, deixamos null - serão preenchidos pelo componente
    vcxImpactosId: null,
    vcxUrgenciasId: null,
  }
}

/**
 * Resolve o valor de Impacto (display) a partir da descrição da API.
 * Usa normalizeDescricao + stripPrefix para lidar com prefixos ("Impacto Alto"),
 * acentos ("Médio" vs "Medio") e variações de gênero ("Alta"/"Alto").
 */
function resolveImpact(descricao: string | null): 'Alto' | 'Médio' | 'Baixo' {
  const key = stripPrefix(normalizeDescricao(descricao))
  if (!key) return 'Médio'
  if (key.includes('alto') || key.includes('alta')) return 'Alto'
  if (key.includes('medio') || key.includes('media')) return 'Médio'
  if (key.includes('baixo') || key.includes('baixa')) return 'Baixo'
  return 'Médio'
}

/**
 * Resolve o valor de Prioridade (display) a partir da descrição de Urgência da API.
 * Usa normalizeDescricao + stripPrefix para lidar com prefixos ("Urgência Alta"),
 * acentos e variações de gênero.
 */
function resolvePriority(descricao: string | null): 'Alta' | 'Média' | 'Baixa' {
  const key = stripPrefix(normalizeDescricao(descricao))
  if (!key) return 'Média'
  if (key.includes('urgente') || key.includes('alta') || key.includes('alto')) return 'Alta'
  if (key.includes('normal') || key.includes('media') || key.includes('medio')) return 'Média'
  if (key.includes('baixa') || key.includes('baixo')) return 'Baixa'
  return 'Média'
}

/**
 * Mapeia uma resposta do backend para o formato do protótipo
 * @param response - Resposta da API (DorResponse)
 * @returns Dor no formato usado pelo componente (PainOpportunity)
 */
export function mapearRespostaParaDor(response: DorResponse): PainOpportunity {
  return {
    id: response.id,
    title: response.titulo,
    description: response.descricao || '',
    priority: resolvePriority(response.vcxUrgenciasDescricao), // Mantido para compatibilidade com formulários
    impact: resolveImpact(response.vcxImpactosDescricao), // Mantido para compatibilidade com formulários
    dataCriacao: response.dataCriacao,
    // Preservar valores reais da API para exibição nos badges
    vcxUrgenciasDescricao: response.vcxUrgenciasDescricao,
    vcxImpactosDescricao: response.vcxImpactosDescricao,
  }
}

/**
 * Mapeia um Impacto da API para formato de select/dropdown
 * @param impacto - Resposta da API (ImpactoResponse)
 * @returns Objeto com value e label para uso em selects
 */
export function mapearImpactoParaSelect(impacto: ImpactoResponse): {
  value: string
  label: string
} {
  return {
    value: impacto.id,
    label: impacto.descricao,
  }
}

/**
 * Mapeia uma Urgência da API para formato de select/dropdown
 * @param urgencia - Resposta da API (UrgenciaResponse)
 * @returns Objeto com value e label para uso em selects
 */
export function mapearUrgenciaParaSelect(urgencia: UrgenciaResponse): {
  value: string
  label: string
} {
  return {
    value: urgencia.id,
    label: urgencia.descricao,
  }
}

/**
 * Encontra o ID de Impacto baseado na descrição
 * @param impactos - Lista de impactos disponíveis
 * @param descricao - Descrição do impacto (ex: "Alto", "Médio", "Baixo", "Impacto Alto")
 * @returns ID do impacto ou null se não encontrado
 */
export function encontrarImpactoIdPorDescricao(
  impactos: ImpactoResponse[],
  descricao: string,
): string | null {
  const key = stripPrefix(normalizeDescricao(descricao))
  const impacto = impactos.find(
    (i) => stripPrefix(normalizeDescricao(i.descricao)) === key,
  )
  return impacto?.id || null
}

/**
 * Encontra o ID de Urgência baseado na descrição
 * @param urgencias - Lista de urgências disponíveis
 * @param descricao - Descrição da urgência (ex: "Urgente", "Normal", "Baixa", "Urgência Alta")
 * @returns ID da urgência ou null se não encontrado
 */
export function encontrarUrgenciaIdPorDescricao(
  urgencias: UrgenciaResponse[],
  descricao: string,
): string | null {
  const key = stripPrefix(normalizeDescricao(descricao))
  const urgencia = urgencias.find(
    (u) => stripPrefix(normalizeDescricao(u.descricao)) === key,
  )
  return urgencia?.id || null
}

/**
 * Mapeia uma Dor do protótipo para payload, incluindo IDs de impacto e urgência
 * Versão completa que resolve os IDs baseado nas descrições
 * @param dor - Dor no formato do protótipo
 * @param posicaoId - ID da posição do organograma
 * @param impactos - Lista de impactos disponíveis
 * @param urgencias - Lista de urgências disponíveis
 * @returns Payload completo com IDs resolvidos
 */
export function mapearDorParaPayloadCompleto(
  dor: PainOpportunity,
  posicaoId: string,
  impactos: ImpactoResponse[],
  urgencias: UrgenciaResponse[],
): DorPayload {
  // Construir lookup: chave normalizada (sem prefixo/acento) → ID
  // Ex: "Impacto Alto" → "alto" → id
  // Ex: "Urgência Média" → "media" → id
  const impactLookup = new Map<string, string>()
  impactos.forEach((impacto) => {
    const key = stripPrefix(normalizeDescricao(impacto.descricao))
    if (key) impactLookup.set(key, impacto.id)
  })

  const urgencyLookup = new Map<string, string>()
  urgencias.forEach((urgencia) => {
    const key = stripPrefix(normalizeDescricao(urgencia.descricao))
    if (key) urgencyLookup.set(key, urgencia.id)
  })

  // Buscar ID no lookup usando o valor do formulário
  // Ex: dor.impact = "Médio" → normalizado "medio" → match com chave "medio" → ID
  // Ex: dor.priority = "Média" → normalizado "media" → match com chave "media" → ID
  const findId = (lookup: Map<string, string>, value: string): string | null => {
    const normalized = stripPrefix(normalizeDescricao(value))
    if (!normalized) return null

    // Match exato
    if (lookup.has(normalized)) return lookup.get(normalized)!

    // Match parcial (fallback)
    for (const [key, id] of lookup.entries()) {
      if (key.includes(normalized) || normalized.includes(key)) return id
    }

    return null
  }

  // dor.impact: "Alto" | "Médio" | "Baixo" → normalizado → "alto" | "medio" | "baixo"
  // Chaves do impactLookup (API real): "alto", "baixo", "medio" → match direto ✓
  const vcxImpactosId = findId(impactLookup, dor.impact)

  // dor.priority: "Alta" | "Média" | "Baixa" → normalizado → "alta" | "media" | "baixa"
  // Chaves do urgencyLookup (API real): "alta", "baixa", "media" → match direto ✓
  const vcxUrgenciasId = findId(urgencyLookup, dor.priority)

  return {
    organogramaPosicaoId: posicaoId,
    titulo: dor.title,
    descricao: dor.description || null,
    vcxImpactosId,
    vcxUrgenciasId,
  }
}

// ============================================================================
// MAPEAMENTOS PARA INICIATIVAS
// ============================================================================

/**
 * Interface do protótipo para Iniciativas (formato usado no componente)
 * Baseado na interface Iniciativa de Vcx360.ts
 */
export interface Initiative {
  id: string
  titulo: string
  tema: string
  objetivoKpi: string
  status: StatusIniciativa
  dataCriacao: string
  dataAtualizacao?: string
}


/**
 * Mapeia uma resposta do backend para o formato do protótipo
 * @param response - Resposta da API (IniciativaResponse)
 * @returns Iniciativa no formato usado pelo componente (Initiative)
 */
export function mapearRespostaParaIniciativa(
  response: IniciativaResponse,
): Initiative {
  // Mapear descrição de status do backend para status do protótipo
  // O backend retorna: "Ativa", "Concluída", "Em Planejamento", "Pausada"
  // Mantemos a descrição exata da API para evitar problemas de mapeamento
  console.log('vcxMappers.mapearRespostaParaIniciativa - response.vcxStatusDescricao:', response.vcxStatusDescricao)
  
  // Usar a descrição exata da API, sem mapeamento
  // Isso garante que o valor corresponda exatamente ao que vem do endpoint de status
  const status = (response.vcxStatusDescricao as StatusIniciativa) || 'Em Planejamento'

  const iniciativa = {
    id: response.id,
    titulo: response.titulo,
    tema: response.vcxTemasDescricao || '',
    objetivoKpi: response.descricao || '',
    status,
    dataCriacao: response.dataCriacao,
    dataAtualizacao: response.dataAlteracao,
  }

  console.log('vcxMappers.mapearRespostaParaIniciativa - iniciativa mapeada:', iniciativa)
  return iniciativa
}

