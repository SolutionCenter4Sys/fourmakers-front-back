import * as React from 'react'
import { useAppSelector } from '@app/store/hooks'
import { useMinhaJornadaViewModel } from '@presentation/hooks/useMinhaJornadaViewModel'
import { PageHeader, PageBreadcrumb } from '@presentation/components/common'
import { PDITabs } from '@presentation/components/pdi-tabs'
import {
  ProfileSelector,
  AdherenceMeter,
  SkillGroupCard,
  PdiCreationModal,
  SkillSuggestionModal,
} from '@presentation/components/minha-jornada'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'
import type { MinhaJornadaSugestao } from '@domain/entities/MinhaJornadaSugestao'
import { AlertCircle, UserX, FileQuestion } from 'lucide-react'
import { Spinner } from '@/components/ui/spinner'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { toast } from 'sonner'

const MinhaJornadaPage = () => {
  const { user } = useAppSelector((state) => state.auth)

  // MainLayout já carrega o perfil do usuário, não precisamos fazer aqui

  const {
    perfilAtuacao,
    perfisDisponiveis,
    aderencia,
    skillGroups,
    isLoading,
    isSaving,
    error,
    hasNoClientes,
    hasNoHabilidades,
    handleMarkAlreadyHave,
    handleMarkNoInterest,
    handleRevertInterest,
    handleMarkWantToDevelop,
    handleSuggestSkill,
    handleSelectProfile,
    handleGravarLogPdiScreenMovement,
  } = useMinhaJornadaViewModel()

  const [pdiModalOpen, setPdiModalOpen] = React.useState(false)
  const [suggestionModalOpen, setSuggestionModalOpen] = React.useState(false)
  const [selectedSkillForPdi, setSelectedSkillForPdi] = React.useState<MinhaJornadaSkill | null>(null)
  const [selectedSkillType, setSelectedSkillType] = React.useState<'hard' | 'soft' | 'methodology' | 'domain' | 'language'>('hard')

const handleOpenPdiModal = (skill: MinhaJornadaSkill) => {
  // Garantir que o perfil esteja carregado antes de abrir o modal
  if (!perfilAtuacao?.perfilId) {
    toast.info('Carregando perfil de atuação. Tente novamente em instantes.')
    return
  }
    setSelectedSkillForPdi(skill)
    setPdiModalOpen(true)
  }

  const handleOpenSuggestionModal = (type: 'hard' | 'soft' | 'methodology' | 'domain' | 'language') => {
    setSelectedSkillType(type)
    setSuggestionModalOpen(true)
  }

  const handleCreatePdi = async (payload: CriarMinhaJornadaPdiPayload) => {
    console.debug('DEBUG handleCreatePdi called with payload:', payload)
    try {
      const skills = selectedSkillForPdi
        ? [selectedSkillForPdi]
        : skillGroups
            .find((g) => g.type === selectedSkillType)
            ?.skills || []

      await handleMarkWantToDevelop(skills, payload)
      toast.success('PDI criado com sucesso!')
      setPdiModalOpen(false)
      setSelectedSkillForPdi(null)

      // Disparar log de movimentação (assíncrono)
      await handleGravarLogPdiScreenMovement(skills)
    } catch (error) {
      toast.error('Erro ao criar PDI. Tente novamente.')
    }
  }

  const handleSuggestSkillSubmit = async (sugestao: MinhaJornadaSugestao) => {
    try {
      await handleSuggestSkill(sugestao)
      toast.success('Sugestão enviada com sucesso!')
      setSuggestionModalOpen(false)
    } catch (error) {
      toast.error('Erro ao enviar sugestão. Tente novamente.')
    }
  }

  const userData = user
    ? {
        nome: user.nomeColaborador,
        cpf: user.cpf,
        orgId: user.orgId || user.colaboradorOrg?.orgId || 0,
        manager: user.orgHierarquia?.nomeProfissionalSuperior,
        emailGestor: user.orgHierarquia?.emailProfissionalSuperior,
      }
    : undefined

  return (
    <div className="container mx-auto p-4 space-y-6 pb-40">
      <PageBreadcrumb
        items={[
          { label: 'Minha Jornada' }
        ]}
      />

      <PDITabs currentTab="minha-jornada" />

      <PageHeader
        title="PDI - Minha Jornada"
        description="Visualize sua aderência ao perfil de atuação e gerencie seu desenvolvimento profissional."
      />

      {error && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {isLoading ? (
        <div className="flex items-center justify-center py-20">
          <Spinner size={32} className="text-primary" />
        </div>
      ) : hasNoClientes ? (
        /* Empty state: Nenhum cliente alocado */
        <div className="flex flex-col items-center justify-center py-20 px-4">
          <div className="rounded-full bg-yellow-100 dark:bg-yellow-900/30 p-6 mb-6">
            <UserX className="w-12 h-12 text-yellow-600 dark:text-yellow-400" />
          </div>
          <h3 className="text-xl font-semibold text-primaryText mb-2">
            Nenhuma alocação encontrada
          </h3>
          <p className="text-sm text-muted-foreground text-center max-w-md">
            O gestor deve alocar o colaborador antes de você poder visualizar sua jornada de desenvolvimento.
          </p>
        </div>
      ) : hasNoHabilidades ? (
        /* Empty state: Nenhuma habilidade no perfil */
        <div className="flex flex-col items-center justify-center py-20 px-4">
          <div className="rounded-full bg-yellow-100 dark:bg-yellow-900/30 p-6 mb-6">
            <FileQuestion className="w-12 h-12 text-yellow-600 dark:text-yellow-400" />
          </div>
          <h3 className="text-xl font-semibold text-primaryText mb-2">
            Nenhuma habilidade encontrada
          </h3>
          <p className="text-sm text-muted-foreground text-center max-w-md">
            O gestor deve incluir skills neste perfil antes de você poder visualizar sua jornada de desenvolvimento.
          </p>
        </div>
      ) : (
        <>
          {/* Hero Section */}
          <div className="flex flex-col gap-6">
            <ProfileSelector
              currentProfile={perfilAtuacao}
              availableProfiles={perfisDisponiveis}
              onSelectProfile={handleSelectProfile}
              isLoading={isLoading}
            />
            <AdherenceMeter
              adherence={aderencia}
              isLoading={isLoading}
            />
          </div>

          {/* Skill Groups */}
          <div className="space-y-4">
            {skillGroups.map((group) => (
              <SkillGroupCard
                key={group.type}
                group={group}
                profileId={perfilAtuacao?.perfilId || null}
                onMarkAlreadyHave={async (skill, nivelId) => {
                  try {
                    await handleMarkAlreadyHave(skill, nivelId)
                    toast.success('Skill atualizada com sucesso!')
                  } catch (error) {
                    toast.error('Erro ao atualizar skill. Tente novamente.')
                    throw error
                  }
                }}
                onMarkNoInterest={async (skill) => {
                  try {
                    await handleMarkNoInterest(skill)
                    toast.success('Interesse atualizado.')
                  } catch (error) {
                    toast.error('Erro ao atualizar interesse. Tente novamente.')
                    throw error
                  }
                }}
                onRevertInterest={async (skill) => {
                  try {
                    await handleRevertInterest(skill)
                    toast.success('Interesse revertido com sucesso!')
                  } catch (error) {
                    toast.error('Erro ao reverter interesse. Tente novamente.')
                    throw error
                  }
                }}
                onMarkWantToDevelop={handleOpenPdiModal}
                onSuggestSkill={() => {
                  handleOpenSuggestionModal(group.type)
                }}
                isSaving={isSaving}
              />
            ))}
          </div>

          {/* Modals */}
          <PdiCreationModal
            open={pdiModalOpen}
            onOpenChange={setPdiModalOpen}
            skills={
              selectedSkillForPdi
                ? [selectedSkillForPdi]
                : skillGroups.find((g) => g.type === selectedSkillType)?.skills || []
            }
            onSubmit={handleCreatePdi}
            userData={userData}
            perfilAtuacao={perfilAtuacao}
          />

          <SkillSuggestionModal
            open={suggestionModalOpen}
            onOpenChange={setSuggestionModalOpen}
            skillType={selectedSkillType}
            profileId={perfilAtuacao?.perfilId || null}
            onSubmit={handleSuggestSkillSubmit}
            userData={
              user && perfilAtuacao
                ? {
                    codigoInternoColaborador: user.cpf,
                    codigoGestorAdm: perfilAtuacao.codigoInternoGestorAdm,
                    codigoCliente: perfilAtuacao.codigoCliente,
                  }
                : undefined
            }
          />
        </>
      )}
    </div>
  )
}

export default MinhaJornadaPage

