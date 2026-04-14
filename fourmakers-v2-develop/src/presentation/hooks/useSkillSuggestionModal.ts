import { useState, useEffect, useCallback, useMemo, useRef } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import type {
  SkillOption,
  NivelOption,
} from '@domain/repositories/SkillsRepository'
import type { SugestaoHistoryItem } from '@data/mappers/sugestaoMappers'
import { SearchSkillsUseCase } from '@domain/usecases/SearchSkillsUseCase'
import { GetSkillLevelsUseCase } from '@domain/usecases/GetSkillLevelsUseCase'
import { GetSuggestionHistoryUseCase } from '@domain/usecases/GetSuggestionHistoryUseCase'
import { TIPO_SKILL_MAP, SKILL_TYPE_LABELS } from '@shared/constants/skillTypes'
import { useDebounced } from '@shared/hooks/useDebounced'

// Re-export types and constants for backward compatibility
export type { SkillOption, NivelOption } from '@domain/repositories/SkillsRepository'
export type { SugestaoHistoryItem } from '@data/mappers/sugestaoMappers'
export { TIPO_SKILL_MAP, SKILL_TYPE_LABELS } from '@shared/constants/skillTypes'

interface UseSkillSuggestionModalProps {
  skillType: MinhaJornadaSkillType
  isOpen: boolean
  profileId: string | null
  userData?: {
    codigoInternoColaborador: string
    codigoGestorAdm: string
    codigoCliente?: string
  }
}

interface UseSkillSuggestionModalReturn {
  // State
  searchTerm: string
  skills: SkillOption[]
  niveis: NivelOption[]
  selectedSkill: SkillOption | null
  selectedNivel: NivelOption | null
  isLoadingSkills: boolean
  isLoadingNiveis: boolean
  error: string | null

  // History
  history: SugestaoHistoryItem[]
  isLoadingHistory: boolean

  // Computed
  canConfirm: boolean
  showAutocomplete: boolean
  tipoSkillLabel: string
  tipoSkillId: number

  // Handlers
  setSearchTerm: (value: string) => void
  handleSkillSelect: (skill: SkillOption) => void
  handleNivelSelect: (nivel: NivelOption) => void
  resetState: () => void
  refreshHistory: () => Promise<void>
}

export const useSkillSuggestionModal = ({
  skillType,
  isOpen,
  profileId,
  userData,
}: UseSkillSuggestionModalProps): UseSkillSuggestionModalReturn => {
  const { token } = useAppSelector((state) => state.auth)

  // Use cases - using useRef to maintain stable reference
  const useCasesRef = useRef<{
    searchSkillsUseCase: SearchSkillsUseCase
    getSkillLevelsUseCase: GetSkillLevelsUseCase
    getSuggestionHistoryUseCase: GetSuggestionHistoryUseCase
  } | null>(null)

  if (!useCasesRef.current) {
    useCasesRef.current = {
      searchSkillsUseCase: container.resolve(SearchSkillsUseCase),
      getSkillLevelsUseCase: container.resolve(GetSkillLevelsUseCase),
      getSuggestionHistoryUseCase: container.resolve(GetSuggestionHistoryUseCase),
    }
  }

  const {
    searchSkillsUseCase,
    getSkillLevelsUseCase,
    getSuggestionHistoryUseCase,
  } = useCasesRef.current

  // State
  const [searchTerm, setSearchTerm] = useState('')
  const [skills, setSkills] = useState<SkillOption[]>([])
  const [niveis, setNiveis] = useState<NivelOption[]>([])
  const [selectedSkill, setSelectedSkill] = useState<SkillOption | null>(null)
  const [selectedNivel, setSelectedNivel] = useState<NivelOption | null>(null)
  const [isLoadingSkills, setIsLoadingSkills] = useState(false)
  const [isLoadingNiveis, setIsLoadingNiveis] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // History state
  const [history, setHistory] = useState<SugestaoHistoryItem[]>([])
  const [isLoadingHistory, setIsLoadingHistory] = useState(false)

  // Debounced search term
  const debouncedSearchTerm = useDebounced(searchTerm, 300)

  // Reset state when modal closes or skill type changes
  const resetState = useCallback(() => {
    setSearchTerm('')
    setSkills([])
    setSelectedSkill(null)
    setSelectedNivel(null)
    setError(null)
  }, [])

  // Load suggestion history via use case
  const loadHistory = useCallback(async () => {
    if (!token || !profileId || !userData) return

    setIsLoadingHistory(true)

    try {
      const historyResult = await getSuggestionHistoryUseCase.execute({
        codInternoGestor: userData.codigoGestorAdm,
        perfilId: profileId,
        codInternoColaborador: userData.codigoInternoColaborador,
        skillType,
        token,
      })

      setHistory(historyResult)
    } catch (err) {
      console.error('Erro ao carregar histórico de sugestões:', err)
      setHistory([])
    } finally {
      setIsLoadingHistory(false)
    }
  }, [token, profileId, userData, skillType, getSuggestionHistoryUseCase])

  // Load levels when modal opens via use case
  useEffect(() => {
    if (!isOpen) {
      resetState()
      return
    }

    const loadNiveis = async () => {
      if (!token) return

      setIsLoadingNiveis(true)
      setError(null)

      try {
        const result = await getSkillLevelsUseCase.execute({
          skillType,
          token,
        })

        setNiveis(result)
      } catch (err) {
        console.error('Erro ao carregar níveis:', err)
        setError('Erro ao carregar níveis. Tente novamente.')
      } finally {
        setIsLoadingNiveis(false)
      }
    }

    void loadNiveis()
    void loadHistory()
  }, [isOpen, skillType, token, resetState, loadHistory, getSkillLevelsUseCase])

  // Search skills when debounced search term changes via use case
  useEffect(() => {
    const searchSkills = async () => {
      if (!isOpen || !token) return

      const query = debouncedSearchTerm.trim()
      if (query.length < 3) {
        setSkills([])
        return
      }

      setIsLoadingSkills(true)
      setError(null)

      try {
        const result = await searchSkillsUseCase.execute({
          skillType,
          searchTerm: query,
          token,
        })

        setSkills(result)
      } catch (err) {
        console.error('Erro ao buscar skills:', err)
        setError('Erro ao buscar skills. Tente novamente.')
        setSkills([])
      } finally {
        setIsLoadingSkills(false)
      }
    }

    void searchSkills()
  }, [debouncedSearchTerm, skillType, token, isOpen, searchSkillsUseCase])

  // Handlers
  const handleSearchTermChange = useCallback((value: string) => {
    setSearchTerm(value)
    setSelectedSkill(null) // Clear selection when typing
  }, [])

  const handleSkillSelect = useCallback((skill: SkillOption) => {
    setSelectedSkill(skill)
    setSearchTerm(skill.nome)
    setSkills([]) // Close autocomplete
  }, [])

  const handleNivelSelect = useCallback((nivel: NivelOption) => {
    setSelectedNivel(nivel)
  }, [])

  // Computed values
  const canConfirm = useMemo(
    () => Boolean(selectedSkill && selectedNivel),
    [selectedSkill, selectedNivel]
  )

  const showAutocomplete = useMemo(
    () => searchTerm.length >= 3 && skills.length > 0 && !selectedSkill,
    [searchTerm, skills, selectedSkill]
  )

  const tipoSkillLabel = SKILL_TYPE_LABELS[skillType]
  const tipoSkillId = TIPO_SKILL_MAP[skillType]

  return {
    // State
    searchTerm,
    skills,
    niveis,
    selectedSkill,
    selectedNivel,
    isLoadingSkills,
    isLoadingNiveis,
    error,

    // History
    history,
    isLoadingHistory,

    // Computed
    canConfirm,
    showAutocomplete,
    tipoSkillLabel,
    tipoSkillId,

    // Handlers
    setSearchTerm: handleSearchTermChange,
    handleSkillSelect,
    handleNivelSelect,
    resetState,
    refreshHistory: loadHistory,
  }
}
