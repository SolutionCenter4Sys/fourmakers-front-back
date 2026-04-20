// Constantes para o Mapa de Relacionamento (anteriormente Organograma)
// Centraliza strings e configurações mágicas

export const MAPA_RELACIONAMENTO_CONSTANTS = {
  // LocalStorage
  STORAGE_PREFIX: 'fourmakers_mapa_rel_data_',
  STORAGE_PREFIX_OLD: 'fourmakers_org_data_', // Para migração
  
  // Feature Info
  FEATURE_NAME: 'Mapa de Relacionamento',
  FEATURE_NAME_SHORT: 'Mapa',
  
  // Visualização
  DEFAULT_VISUALIZATION: 'diagrama' as const,
  VISUALIZATION_MODES: {
    DIAGRAMA: 'diagrama' as const,
    LISTA: 'lista' as const,
    C_LEVELS: 'c-levels' as const,
  },
  
  // Debounce
  SAVE_DEBOUNCE_MS: 500,
  
  // API Limits
  DEFAULT_API_LIMIT: 50000,
  
  // Node defaults
  DEFAULT_VACANT_ID: 'vacant',
  DEFAULT_VACANT_NAME: 'Vago',
  DEFAULT_PROFILE_NAME: 'CEO / Diretor',
} as const

// Tipos derivados
export type VisualizationMode = typeof MAPA_RELACIONAMENTO_CONSTANTS.VISUALIZATION_MODES[keyof typeof MAPA_RELACIONAMENTO_CONSTANTS.VISUALIZATION_MODES]

// Named exports for convenience
export const MAPA_RELACIONAMENTO_STORAGE_PREFIX = MAPA_RELACIONAMENTO_CONSTANTS.STORAGE_PREFIX
export const MAPA_RELACIONAMENTO_FEATURE_NAME = MAPA_RELACIONAMENTO_CONSTANTS.FEATURE_NAME
export const MAPA_RELACIONAMENTO_DEFAULT_VISUALIZATION_MODE = MAPA_RELACIONAMENTO_CONSTANTS.DEFAULT_VISUALIZATION
