import * as React from 'react'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'
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
import { Textarea } from '@/components/ui/textarea'
import { Input } from '@/components/ui/input'

interface PdiCreationModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  skills: MinhaJornadaSkill[]
  onSubmit: (payload: CriarMinhaJornadaPdiPayload) => Promise<void>
  userData?: {
    nome: string
    cpf: string
    orgId: number
    manager?: string
    emailGestor?: string
  }
  perfilAtuacao?: {
    nomeGestorAdm?: string
    codigoInternoGestorAdm?: string
  } | null
}

// Função para formatar data enquanto digita (dd/mm/yyyy)
const formatarDataInput = (valor: string): string => {
  // Remove tudo que não é número
  const apenasNumeros = valor.replace(/\D/g, '')
  
  // Aplica a máscara
  if (apenasNumeros.length <= 2) {
    return apenasNumeros
  } else if (apenasNumeros.length <= 4) {
    return `${apenasNumeros.slice(0, 2)}/${apenasNumeros.slice(2)}`
  } else {
    return `${apenasNumeros.slice(0, 2)}/${apenasNumeros.slice(2, 4)}/${apenasNumeros.slice(4, 8)}`
  }
}

// Função para converter string dd/mm/yyyy para Date
const stringParaDate = (dataStr: string): Date | null => {
  if (!dataStr || dataStr.length !== 10) return null
  const [dia, mes, ano] = dataStr.split('/').map(Number)
  if (dia && mes && ano) {
    const data = new Date(ano, mes - 1, dia)
    if (data.getDate() === dia && data.getMonth() === mes - 1 && data.getFullYear() === ano) {
      return data
    }
  }
  return null
}

export const PdiCreationModal = ({
  open,
  onOpenChange,
  skills,
  onSubmit,
  userData,
  perfilAtuacao,
}: PdiCreationModalProps) => {
  const [selectedSkills, setSelectedSkills] = React.useState<Set<number>>(
    new Set(skills.map((s) => s.skillId)),
  )
  const [deadline, setDeadline] = React.useState<string>('')
  const [comment, setComment] = React.useState('')
  const [isSubmitting, setIsSubmitting] = React.useState(false)

  // Sincronizar selectedSkills com a lista de skills sempre que a prop mudar
  React.useEffect(() => {
    setSelectedSkills(new Set(skills.map((s) => s.skillId)))
  }, [skills])

  const handleToggleSkill = (skillId: number) => {
    const newSet = new Set(selectedSkills)
    if (newSet.has(skillId)) {
      newSet.delete(skillId)
    } else {
      newSet.add(skillId)
    }
    setSelectedSkills(newSet)
  }

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const valorFormatado = formatarDataInput(e.target.value)
    setDeadline(valorFormatado)
  }

  const handleSubmit = async () => {
    if (selectedSkills.size === 0) {
      alert('Selecione pelo menos uma skill')
      return
    }

    if (!deadline || deadline.length !== 10) {
      alert('Informe uma data de prazo válida (DD/MM/YYYY)')
      return
    }

    const deadlineDate = stringParaDate(deadline)
    if (!deadlineDate) {
      alert('Data de prazo inválida. Use o formato DD/MM/YYYY')
      return
    }

    if (!userData) {
      alert('Dados do usuário não disponíveis')
      return
    }

    try {
      setIsSubmitting(true)

      const selectedSkillsList = skills.filter((s) =>
        selectedSkills.has(s.skillId),
      )

      const payload: CriarMinhaJornadaPdiPayload = {
        metas: [
          {
            meta: comment || 'Desenvolver habilidades selecionadas',
            prazo: deadlineDate.toISOString(),
            skills: selectedSkillsList.map((s) => s.habilidade),
          },
        ],
        colaborador: userData.nome,
        manager: perfilAtuacao?.nomeGestorAdm || userData.manager || '',
        uuid_colab: userData.cpf,
        id_colab: 0,
        org_id: userData.orgId,
        criado_por_colab: true,
        uuid_manager: perfilAtuacao?.codigoInternoGestorAdm || '',
        email_gestor: userData.emailGestor || '',
      }

      // Debug: log payload antes de chamar o endpoint
      console.debug('DEBUG PDI payload prepared for onSubmit:', payload)
      console.debug('DEBUG invoking onSubmit with payload')
      await onSubmit(payload)
      onOpenChange(false)
      
      // Reset form
      setSelectedSkills(new Set(skills.map((s) => s.skillId)))
      setDeadline('')
      setComment('')
    } catch (error) {
      console.error('Erro ao criar PDI:', error)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Criar Plano de Desenvolvimento Individual (PDI)</DialogTitle>
          <DialogDescription>
            Selecione as skills que deseja desenvolver e defina um prazo.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Skills Selection */}
          <div className="space-y-2">
            <Label>Skills Selecionadas ({selectedSkills.size})</Label>
            <div className="max-h-48 overflow-y-auto border border-input rounded-lg p-3 space-y-2">
              {skills.length === 0 ? (
                <p className="text-sm text-muted-foreground text-center py-4">
                  Nenhuma skill disponível
                </p>
              ) : (
                skills.map((skill) => (
                  <label
                    key={skill.skillId}
                    className="flex items-center gap-2 p-2 rounded hover:bg-surfaceSubtle cursor-pointer"
                  >
                    <input
                      type="checkbox"
                      checked={selectedSkills.has(skill.skillId)}
                      onChange={() => handleToggleSkill(skill.skillId)}
                      className="rounded"
                    />
                    <span className="text-sm text-primaryText">
                      {skill.habilidade} ({skill.senioridade || 'Nível não especificado'})
                    </span>
                  </label>
                ))
              )}
            </div>
          </div>

          {/* Deadline */}
          <div className="space-y-2">
            <Label>Prazo *</Label>
            <Input
              type="text"
              value={deadline}
              onChange={handleDateChange}
              placeholder="DD/MM/YYYY"
              maxLength={10}
              className="rounded-lg"
            />
            <p className="text-xs text-muted-foreground">
              Digite a data no formato DD/MM/YYYY
            </p>
          </div>

          {/* Comment */}
          <div className="space-y-2">
            <Label>Observações (opcional)</Label>
            <Textarea
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder="Adicione observações sobre este PDI..."
              className="min-h-[100px] rounded-lg"
              maxLength={500}
            />
            <p className="text-xs text-muted-foreground">
              {comment.length}/500 caracteres
            </p>
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancelar
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={isSubmitting || selectedSkills.size === 0 || !deadline || deadline.length !== 10}
            className="rounded-pill"
          >
            {isSubmitting ? 'Criando...' : 'Criar PDI'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

