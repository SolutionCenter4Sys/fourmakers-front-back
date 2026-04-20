import * as React from 'react'
import type { MinhaJornadaSugestao } from '@domain/entities/MinhaJornadaSugestao'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Badge } from '@/components/ui/badge'
import { Clock, CheckCircle2, XCircle, Loader2, X } from 'lucide-react'
import {
  useSkillSuggestionModal,
  TIPO_SKILL_MAP,
  SKILL_TYPE_LABELS,
  type SugestaoHistoryItem,
} from '@presentation/hooks/useSkillSuggestionModal'

interface SkillSuggestionModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  skillType: MinhaJornadaSkillType
  profileId: string | null
  onSubmit: (sugestao: MinhaJornadaSugestao) => Promise<void>
  userData?: {
    codigoInternoColaborador: string
    codigoGestorAdm: string
    codigoCliente?: string
  }
}

const STATUS_CONFIG: Record<SugestaoHistoryItem['status'], { icon: typeof Clock; label: string; color: string }> = {
  pending: { icon: Clock, label: 'Pendente', color: 'bg-yellow-500/20 text-yellow-700 border-yellow-500/30' },
  approved: { icon: CheckCircle2, label: 'Aprovada', color: 'bg-green-500/20 text-green-700 border-green-500/30' },
  rejected: { icon: XCircle, label: 'Rejeitada', color: 'bg-red-500/20 text-red-700 border-red-500/30' },
}

const formatDate = (dateString: string): string => {
  try {
    const date = new Date(dateString)
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    })
  } catch {
    return dateString
  }
}

export const SkillSuggestionModal = ({
  open,
  onOpenChange,
  skillType,
  profileId,
  onSubmit,
  userData,
}: SkillSuggestionModalProps) => {
  const [isSubmitting, setIsSubmitting] = React.useState(false)

  const {
    searchTerm,
    skills,
    niveis,
    selectedSkill,
    selectedNivel,
    isLoadingSkills,
    isLoadingNiveis,
    error,
    canConfirm,
    showAutocomplete,
    setSearchTerm,
    handleSkillSelect,
    handleNivelSelect,
    resetState,
    history,
    isLoadingHistory,
    refreshHistory,
  } = useSkillSuggestionModal({ skillType, isOpen: open, profileId, userData })

  const handleSubmit = async () => {
    if (!selectedSkill || !selectedNivel || !profileId || !userData) {
      return
    }

    try {
      setIsSubmitting(true)

      const sugestao: MinhaJornadaSugestao = {
        codigoInternoColaborador: userData.codigoInternoColaborador,
        codigoGestorAdm: userData.codigoGestorAdm,
        codigoCliente: userData.codigoCliente || '',
        perfilId: profileId,
        skillId: selectedSkill.id,
        tipoId: TIPO_SKILL_MAP[skillType],
        senioridadeId: selectedNivel.id,
        ativo: true,
        gestorExternoPerfil: profileId ?? '',
        minhaJornada: true,
      }

      await onSubmit(sugestao)
      
      // Refresh history after successful submission
      await refreshHistory()
      
      resetState()
    } catch (error) {
      console.error('Erro ao sugerir skill:', error)
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleClearSelection = () => {
    setSearchTerm('')
    resetState()
  }

  const skillTypeLabel = SKILL_TYPE_LABELS[skillType]

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Sugerir Nova Habilidade</DialogTitle>
          <DialogDescription>
            Sugira uma nova habilidade do tipo <strong>{skillTypeLabel}</strong> para ser incluída no seu perfil de atuação.
          </DialogDescription>
        </DialogHeader>

        <Tabs defaultValue="suggest" className="w-full">
          <TabsList className="grid w-full grid-cols-2 bg-gray-700/50 p-1 rounded-lg">
            <TabsTrigger 
              value="history"
              className="data-[state=active]:bg-white data-[state=active]:text-gray-900 data-[state=active]:shadow-sm text-gray-300 data-[state=inactive]:text-gray-300 rounded-md transition-all py-2"
            >
              Histórico
            </TabsTrigger>
            <TabsTrigger 
              value="suggest"
              className="data-[state=active]:bg-white data-[state=active]:text-gray-900 data-[state=active]:shadow-sm text-gray-300 data-[state=inactive]:text-gray-300 rounded-md transition-all py-2"
            >
              Sugerir
            </TabsTrigger>
          </TabsList>

          {/* Histórico Tab */}
          <TabsContent value="history" className="space-y-4">
            <div className="space-y-2">
              <h3 className="font-semibold text-primaryText">
                Histórico de Sugestões
              </h3>
              {isLoadingHistory ? (
                <div className="flex items-center justify-center gap-2 py-8">
                  <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
                  <span className="text-sm text-muted-foreground">Carregando histórico...</span>
                </div>
              ) : history.length === 0 ? (
                <p className="text-sm text-muted-foreground text-center py-8">
                  Nenhuma sugestão enviada ainda
                </p>
              ) : (
                <div className="space-y-2">
                  {history.map((item) => {
                    const config = STATUS_CONFIG[item.status]
                    const Icon = config.icon
                    return (
                      <div
                        key={item.id}
                        className="flex items-center justify-between p-3 border border-borderDefault rounded-lg bg-surfaceSubtle"
                      >
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-2">
                            <Icon className="w-4 h-4 flex-shrink-0 text-muted-foreground" />
                            <span className="text-sm font-medium text-primaryText truncate">
                              {item.skillName}
                            </span>
                          </div>
                          <div className="flex items-center gap-2 mt-1 ml-6">
                            <span className="text-xs text-muted-foreground">
                              {item.skillType}
                            </span>
                            <span className="text-xs text-muted-foreground">•</span>
                            <span className="text-xs text-muted-foreground">
                              {item.senioridade}
                            </span>
                          </div>
                        </div>
                        <div className="flex items-center gap-2 flex-shrink-0 ml-2">
                          <Badge variant="outline" className={config.color}>
                            {config.label}
                          </Badge>
                          <span className="text-xs text-muted-foreground whitespace-nowrap">
                            {formatDate(item.date)}
                          </span>
                        </div>
                      </div>
                    )
                  })}
                </div>
              )}
            </div>
          </TabsContent>

          {/* Sugerir Tab */}
          <TabsContent value="suggest" className="space-y-4">
            <div className="space-y-4">
              {/* Error Alert */}
              {error && (
                <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
                  <p className="text-sm text-red-600">{error}</p>
                </div>
              )}

              {/* Skill Search with Autocomplete */}
              <div className="space-y-2">
                <Label htmlFor="skill-search">Buscar {skillTypeLabel} *</Label>
                <div className="relative">
                  <Input
                    id="skill-search"
                    placeholder="Digite pelo menos 3 caracteres..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="h-10 rounded-lg pr-10"
                    disabled={!!selectedSkill}
                  />
                  {isLoadingSkills && (
                    <Loader2 className="absolute right-3 top-1/2 -translate-y-1/2 h-4 w-4 animate-spin text-muted-foreground" />
                  )}

                  {/* Autocomplete Dropdown */}
                  {showAutocomplete && (
                    <ul className="absolute z-50 w-full mt-1 bg-white border border-borderDefault rounded-lg shadow-lg max-h-48 overflow-auto">
                      {skills.map((skill) => (
                        <li
                          key={skill.id}
                          onClick={() => handleSkillSelect(skill)}
                          className="px-3 py-2 cursor-pointer hover:bg-gray-100 transition-colors text-sm text-gray-900"
                        >
                          {skill.nome}
                        </li>
                      ))}
                    </ul>
                  )}
                </div>

                {/* Hint text */}
                {searchTerm.length > 0 && searchTerm.length < 3 && !selectedSkill && (
                  <p className="text-xs text-muted-foreground">
                    Digite pelo menos 3 caracteres para buscar
                  </p>
                )}

                {/* No results message */}
                {searchTerm.length >= 3 && skills.length === 0 && !isLoadingSkills && !selectedSkill && (
                  <p className="text-xs text-muted-foreground">
                    Nenhum resultado encontrado para "{searchTerm}"
                  </p>
                )}
              </div>

              {/* Selected Skill Badge */}
              {selectedSkill && (
                <div className="flex items-center gap-2 p-3 bg-primarySoft rounded-lg border border-borderSoft">
                  <div className="flex-1">
                    <p className="text-xs text-muted-foreground mb-1">Selecionado:</p>
                    <p className="text-sm font-medium text-primaryText">{selectedSkill.nome}</p>
                  </div>
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    onClick={handleClearSelection}
                    className="h-8 w-8 p-0"
                  >
                    <X className="h-4 w-4" />
                    <span className="sr-only">Limpar seleção</span>
                  </Button>
                </div>
              )}

              {/* Level Selection - Radio Group */}
              <div className="space-y-3">
                <Label>Nível *</Label>
                {isLoadingNiveis ? (
                  <div className="flex items-center gap-2 text-muted-foreground py-2">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    <span className="text-sm">Carregando níveis...</span>
                  </div>
                ) : niveis.length === 0 ? (
                  <p className="text-sm text-muted-foreground">
                    Nenhum nível disponível
                  </p>
                ) : (
                  <div className="space-y-2">
                    {niveis.map((nivel) => (
                      <label
                        key={nivel.id}
                        className={`flex items-center gap-3 p-3 border rounded-lg cursor-pointer transition-colors ${
                          selectedNivel?.id === nivel.id
                            ? 'border-primary bg-primarySoft'
                            : 'border-borderDefault hover:border-primary/50 hover:bg-surfaceSubtle'
                        }`}
                      >
                        <input
                          type="radio"
                          name="nivel"
                          value={nivel.id}
                          checked={selectedNivel?.id === nivel.id}
                          onChange={() => handleNivelSelect(nivel)}
                          className="h-4 w-4 text-primary focus:ring-primary"
                        />
                        <span className="text-sm text-primaryText">{nivel.descricao}</span>
                      </label>
                    ))}
                  </div>
                )}
              </div>

              {/* Info message */}
              <div className="p-3 bg-surfaceSubtle rounded-lg">
                <p className="text-xs text-muted-foreground">
                  Sua sugestão será enviada para análise do gestor. Você será
                  notificado quando houver uma resposta.
                </p>
              </div>
            </div>
          </TabsContent>
        </Tabs>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={isSubmitting || !canConfirm || !profileId || !userData}
            className="rounded-pill"
          >
            {isSubmitting ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin mr-2" />
                Enviando...
              </>
            ) : (
              'Enviar Sugestão'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
