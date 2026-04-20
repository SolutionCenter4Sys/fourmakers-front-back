import * as React from 'react'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import { CardContent } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Spinner } from '@/components/ui/spinner'
import { cn } from '@/lib/utils'
import { SenioritySelectionModal } from './SenioritySelectionModal'

interface SkillCardProps {
  skill: MinhaJornadaSkill
  onMarkAlreadyHave: (skill: MinhaJornadaSkill, nivelId: number) => Promise<void>
  onMarkNoInterest: (skill: MinhaJornadaSkill) => Promise<void>
  onRevertInterest?: (skill: MinhaJornadaSkill) => Promise<void>
  onMarkWantToDevelop: (skill: MinhaJornadaSkill) => void
  isPending?: boolean
}

const SENIORITY_LEVELS = [
  { id: 1, label: 'Trainee' },
  { id: 2, label: 'Júnior' },
  { id: 3, label: 'Pleno' },
  { id: 4, label: 'Sênior' },
  { id: 5, label: 'Especialista' },
]

export const SkillCard = ({
  skill,
  onMarkAlreadyHave,
  onMarkNoInterest,
  onRevertInterest,
  onMarkWantToDevelop,
  isPending = false,
}: SkillCardProps) => {
  const [selectedAction, setSelectedAction] = React.useState<
    'have' | 'no-interest' | 'develop' | 'revert' | null
  >(null)
  const [selectedLevel, setSelectedLevel] = React.useState<number>(
    skill.senioridadeId && skill.senioridadeId !== null ? skill.senioridadeId : 3,
  )
  const [isSaving, setIsSaving] = React.useState(false)
  const [isDarkMode, setIsDarkMode] = React.useState(false)
  const [seniorityModalOpen, setSeniorityModalOpen] = React.useState(false)

  React.useEffect(() => {
    const checkDarkMode = () => {
      setIsDarkMode(document.documentElement.classList.contains('dark'))
    }
    checkDarkMode()
    const observer = new MutationObserver(checkDarkMode)
    observer.observe(document.documentElement, {
      attributes: true,
      attributeFilter: ['class'],
    })
    return () => observer.disconnect()
  }, [])

  // Verificar se a skill tem interesse desativado
  // interesse === 0 significa sem interesse (mostrar botão reverter)
  // interesse === 1 ou qualquer outro valor significa com interesse (mostrar botões normais)
  const hasNoInterest = skill.interesse === 0

  // Verificar se a senioridade não foi definida (edge case do endpoint BuscarSkillColaboradorAlocado)
  // Inclui casos: null, undefined, string vazia, ou string contendo "definir"/"definida" (case insensitive)
  const senioridadeNaoDefinida = (() => {
    // Verificar se senioridadeId não está definido
    if (!skill.senioridadeId || skill.senioridadeId === null || skill.senioridadeId === undefined) {
      return true
    }
    
    // Verificar se senioridade não está definido ou está vazio
    if (!skill.senioridade || skill.senioridade === null || skill.senioridade === undefined) {
      return true
    }
    
    // Verificar se é string vazia após trim
    if (typeof skill.senioridade === 'string' && skill.senioridade.trim() === '') {
      return true
    }
    
    // Verificar se contém "definir" ou "definida" (case insensitive)
    if (typeof skill.senioridade === 'string' && /definir|definida/i.test(skill.senioridade.trim())) {
      return true
    }
    
    return false
  })()

  // Verificar se a skill já foi atingida
  // Caso 1: Nivel atual >= nivel exigido (comportamento normal)
  // Caso 2: Se a senioridade do perfil é "a definir" e o usuário tem qualquer nível diferente de "a definir" no perfil 360
  const isCompleted = (() => {
    // Caso normal: tier atual >= tier exigida
    if (
      skill.senioridadeTierAtual !== null && 
      skill.senioridadeTierAtual !== undefined &&
      skill.senioridadeTierExigida !== null &&
      skill.senioridadeTierExigida !== undefined &&
      skill.senioridadeTierAtual >= skill.senioridadeTierExigida
    ) {
      return true
    }

    // Caso especial: se a senioridade do perfil é "a definir" e o usuário tem a skill com nível definido no perfil 360
    if (
      senioridadeNaoDefinida &&
      skill.senioridadeTierAtual !== null &&
      skill.senioridadeTierAtual !== undefined
    ) {
      return true
    }

    return false
  })()

  const handleAction = async (action: 'have' | 'no-interest' | 'develop' | 'revert') => {
    if (isPending || isSaving || isCompleted) return

    try {
      // Se for "have" e a senioridade não estiver definida, abrir modal
      if (action === 'have' && senioridadeNaoDefinida) {
        setSelectedAction('have')
        setSeniorityModalOpen(true)
        return
      }

      setIsSaving(true)
      setSelectedAction(action)

      if (action === 'have') {
        await onMarkAlreadyHave(skill, selectedLevel)
      } else if (action === 'no-interest') {
        await onMarkNoInterest(skill)
      } else if (action === 'develop') {
        onMarkWantToDevelop(skill)
      } else if (action === 'revert' && onRevertInterest) {
        await onRevertInterest(skill)
      }
    } catch (error) {
      console.error('Erro ao processar ação:', error)
    } finally {
      setIsSaving(false)
    }
  }

  const handleSenioritySelect = async (nivelId: number) => {
    try {
      setIsSaving(true)
      setSelectedLevel(nivelId)
      await onMarkAlreadyHave(skill, nivelId)
      setSelectedAction(null)
    } catch (error) {
      console.error('Erro ao processar seleção de senioridade:', error)
    } finally {
      setIsSaving(false)
    }
  }

  const handleRadioChange = async (value: 'have' | 'no-interest' | 'develop') => {
    if (isPending || isSaving || isCompleted) return
    // Não chamar handleAction diretamente aqui, pois ele pode abrir o modal
    // Apenas definir a ação selecionada e deixar handleAction decidir
    setSelectedAction(value)
    await handleAction(value)
  }

  const handleLevelChange = async (level: number) => {
    if (isPending || isSaving || selectedAction !== 'have' || isCompleted) return
    setSelectedLevel(level)
    // Update the action when level is changed
    await handleAction('have')
  }

  // Debug: verificar se isCompleted está true (apenas em desenvolvimento)
  if (import.meta.env.DEV && isCompleted) {
    console.log('[SkillCard] Skill completa detectada:', skill.habilidade, { isCompleted, isDarkMode })
  }

  // Verificar se a skill está em um PDI
  const isInPdi = skill.isInPdi === true

  return (
    <div
      className={cn(
        'rounded-lg transition-all h-full relative border shadow-softToken',
        isCompleted 
          ? 'border-success/30' 
          : isInPdi
          ? 'border-blue-300 dark:border-blue-700'
          : 'border-borderDefault bg-surfaceElevated',
        !isCompleted && !isInPdi && selectedAction === 'have' && 'border-primary border-2',
        !isCompleted && !isInPdi && selectedAction === 'no-interest' && 'bg-muted/50',
        !isCompleted && !isInPdi && hasNoInterest && !selectedAction && 'bg-muted/50',
        isPending && 'opacity-50 pointer-events-none',
        isSaving && 'opacity-90',
      )}
      style={
        isCompleted
          ? {
              backgroundColor: isDarkMode
                ? 'rgba(5, 46, 22, 0.2)' // dark:bg-green-950/20
                : 'rgba(220, 252, 231, 1)', // bg-green-100 sólido para garantir visibilidade
            }
          : isInPdi
          ? {
              backgroundColor: isDarkMode
                ? 'rgba(30, 58, 138, 0.2)' // dark:bg-blue-950/20
                : 'rgba(239, 246, 255, 1)', // bg-blue-50 sólido
            }
          : {}
      }
    >
      {/* Gray overlay when no interest */}
      {(hasNoInterest || selectedAction === 'no-interest') && (
        <div className="absolute inset-0 bg-gray-900/20 dark:bg-gray-100/10 rounded-lg pointer-events-none" />
      )}
      {/* Loading overlay - animação leve */}
      {isSaving && (
        <div className="absolute inset-0 bg-white/50 dark:bg-black/20 rounded-lg flex items-center justify-center z-10">
          <Spinner size={24} className="text-primary" />
        </div>
      )}
      <CardContent 
        className={cn(
          "p-4 h-full flex flex-col relative",
          isCompleted && "!bg-transparent"
        )}
      >
        <div className="space-y-4 flex-1">
          {/* Header: Skill name and badge */}
          <div className="flex items-start justify-between gap-2">
            <h4 className={cn(
              'font-bold uppercase truncate flex-1 min-w-0',
              isCompleted 
                ? 'text-green-700 dark:text-green-400' 
                : isInPdi
                ? 'text-blue-700 dark:text-blue-400'
                : 'text-primaryText'
            )} title={skill.habilidade}>
              {skill.habilidade}
            </h4>
            <Badge 
              variant={isCompleted ? 'default' : 'outline'} 
              className={cn(
                'text-xs flex-shrink-0',
                isCompleted && 'bg-green-100 text-green-800 border-green-300 dark:bg-green-900/30 dark:text-green-300 dark:border-green-700',
                isInPdi && !isCompleted && 'bg-blue-100 text-blue-800 border-blue-300 dark:bg-blue-900/30 dark:text-blue-300 dark:border-blue-700'
              )}
            >
              {skill.senioridade || 'Nível não especificado'}
            </Badge>
          </div>

          {/* Separator line */}
          <div className={cn(
            'border-t',
            isCompleted 
              ? 'border-green-200 dark:border-green-800' 
              : isInPdi
              ? 'border-blue-200 dark:border-blue-800'
              : 'border-borderDefault'
          )} />

          {isCompleted ? (
            /* Completed state - no actions allowed */
            <div className="space-y-3">
              <div className="flex items-center gap-2 text-sm text-green-700 dark:text-green-400">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span className="font-medium">Habilidade já atingida</span>
              </div>
              <p className="text-xs text-green-600 dark:text-green-500">
                Você já possui esta habilidade no nível exigido ou superior.
              </p>
            </div>
          ) : isInPdi ? (
            /* PDI state - only show "Adicionar ao Perfil" option (behaves like "have") */
            <div className="space-y-3">
              <p className="text-sm text-blue-700 dark:text-blue-400 font-medium">
                Esta habilidade está presente em um PDI
              </p>
              <label className="flex items-start gap-2 cursor-pointer group">
                <input
                  type="radio"
                  name={`skill-${skill.skillId}`}
                  checked={selectedAction === 'have'}
                  onChange={() => handleRadioChange('have')}
                  disabled={isPending || isSaving}
                  className="mt-1 w-4 h-4 text-blue-600 border-blue-300 focus:ring-blue-500 focus:ring-2"
                />
                <span className="text-sm text-blue-700 dark:text-blue-400 group-hover:text-blue-800 dark:group-hover:text-blue-300 font-medium">
                  Adicionar ao Perfil
                </span>
              </label>
              {selectedAction === 'have' && !isSaving && (
                <div className="ml-6 space-y-2">
                  <label className="text-xs font-medium text-blue-700 dark:text-blue-400">
                    Seu nível atual:
                  </label>
                  <select
                    value={selectedLevel}
                    onChange={(e) => handleLevelChange(Number(e.target.value))}
                    className="w-full h-9 rounded-lg border border-blue-300 bg-input px-3 py-1 text-sm text-blue-700 dark:text-blue-400"
                    disabled={isSaving}
                  >
                    {SENIORITY_LEVELS.map((level) => (
                      <option key={level.id} value={level.id}>
                        {level.label}
                      </option>
                    ))}
                  </select>
                </div>
              )}
            </div>
          ) : (
            <>
              {/* Question */}
              <p className="text-sm text-primaryText font-medium">
                Você já possui esta habilidade?
              </p>

              {/* Radio button options */}
              <div className="space-y-3">
            {hasNoInterest ? (
              // Se a skill tem interesse desativado, mostrar opção para reverter
              onRevertInterest && (
                <label className="flex items-start gap-2 cursor-pointer group">
                  <input
                    type="radio"
                    name={`skill-${skill.skillId}`}
                    checked={selectedAction === 'revert'}
                    onChange={() => handleAction('revert')}
                    disabled={isPending || isSaving}
                    className="mt-1 w-4 h-4 text-primary border-borderDefault focus:ring-primary focus:ring-2"
                  />
                  <span className="text-sm text-primaryText group-hover:text-primary">
                    Reverter interesse e habilitar novamente
                  </span>
                </label>
              )
            ) : (
              <>
                <label className="flex items-start gap-2 cursor-pointer group">
                  <input
                    type="radio"
                    name={`skill-${skill.skillId}`}
                    checked={selectedAction === 'have'}
                    onChange={() => handleRadioChange('have')}
                    disabled={isPending || isSaving}
                    className="mt-1 w-4 h-4 text-primary border-borderDefault focus:ring-primary focus:ring-2"
                  />
                  <span className="text-sm text-primaryText group-hover:text-primary">
                    Sim, já possuo e quero incluir no meu perfil 360
                  </span>
                </label>

                {selectedAction === 'have' && !isSaving && !senioridadeNaoDefinida && (
                  <div className="ml-6 space-y-2">
                    <label className="text-xs font-medium text-primaryText">
                      Seu nível atual:
                    </label>
                    <select
                      value={selectedLevel}
                      onChange={(e) => handleLevelChange(Number(e.target.value))}
                      className="w-full h-9 rounded-lg border border-input bg-input px-3 py-1 text-sm"
                      disabled={isSaving}
                    >
                      {SENIORITY_LEVELS.map((level) => (
                        <option key={level.id} value={level.id}>
                          {level.label}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                <label className="flex items-start gap-2 cursor-pointer group">
                  <input
                    type="radio"
                    name={`skill-${skill.skillId}`}
                    checked={selectedAction === 'no-interest'}
                    onChange={() => handleRadioChange('no-interest')}
                    disabled={isPending || isSaving}
                    className="mt-1 w-4 h-4 text-primary border-borderDefault focus:ring-primary focus:ring-2"
                  />
                  <span className="text-sm text-primaryText group-hover:text-primary">
                    Não, e não tenho interesse agora.
                  </span>
                </label>

                <label className="flex items-start gap-2 cursor-pointer group">
                  <input
                    type="radio"
                    name={`skill-${skill.skillId}`}
                    checked={selectedAction === 'develop'}
                    onChange={() => handleRadioChange('develop')}
                    disabled={isPending || isSaving}
                    className="mt-1 w-4 h-4 text-primary border-borderDefault focus:ring-primary focus:ring-2"
                  />
                  <span className="text-sm text-primaryText group-hover:text-primary">
                    Quero desenvolver. Adicionar ao PDI.
                  </span>
                </label>
              </>
            )}
              </div>
            </>
          )}

        </div>
      </CardContent>

      {/* Modal de seleção de senioridade quando não definida */}
      <SenioritySelectionModal
        open={seniorityModalOpen}
        onOpenChange={setSeniorityModalOpen}
        perfilTipoId={skill.perfilTipoId}
        onSelect={handleSenioritySelect}
      />
    </div>
  )
}

