import { useState, useEffect } from 'react'
import { useAppSelector } from '@app/store/hooks'
import type { DepartamentoPayload, DepartamentoResponse } from '@domain/entities/Organograma'
import { toast } from 'sonner'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Building } from 'lucide-react'

interface CriarEditarDepartamentoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  departamento?: DepartamentoResponse | null
  codigoCliente: string
  onSave: (payload: DepartamentoPayload & { id?: string }) => void
}

export function CriarEditarDepartamentoModal({
  open,
  onOpenChange,
  departamento,
  codigoCliente,
  onSave,
}: CriarEditarDepartamentoModalProps) {
  const { user } = useAppSelector((state) => state.auth)
  const [nome, setNome] = useState('')
  const [organogramaPosicaoIdLider, setOrganogramaPosicaoIdLider] = useState<string | null>(null)
  const [errors, setErrors] = useState<{ nome?: string }>({})
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    if (open) {
      if (departamento) {
        setNome(departamento.nome)
        setOrganogramaPosicaoIdLider(departamento.organogramaPosicaoIdLider)
      } else {
        setNome('')
        setOrganogramaPosicaoIdLider(null)
      }
      setErrors({})
      setIsSaving(false)
    }
  }, [open, departamento])

  const validate = (): boolean => {
    const newErrors: { nome?: string } = {}
    
    if (!nome.trim()) {
      newErrors.nome = 'O nome do departamento é obrigatório.'
    } else if (nome.trim().length < 2) {
      newErrors.nome = 'O nome do departamento deve ter pelo menos 2 caracteres.'
    } else if (nome.trim().length > 200) {
      newErrors.nome = 'O nome do departamento não pode exceder 200 caracteres.'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) {
      return
    }

    if (!user?.orgId) {
      toast.error('Erro: orgId não encontrado no usuário')
      return
    }

    setIsSaving(true)

    try {
      const payload: DepartamentoPayload = {
        orgId: user.orgId,
        nome: nome.trim(),
        codigoCliente,
        organogramaPosicaoIdLider,
        ativo: true,
      }

      // Se for edição, incluir ID e não incluir dataCriacao
      if (departamento?.id) {
        (payload as DepartamentoPayload & { id: string }).id = departamento.id
      } else {
        payload.dataCriacao = new Date().toISOString()
      }

      await onSave(payload as DepartamentoPayload & { id?: string })

      setNome('')
      setOrganogramaPosicaoIdLider(null)
      setErrors({})
      onOpenChange(false)
    } catch (error) {
      console.error('Erro ao salvar departamento:', error)
      toast.error('Erro ao salvar departamento. Tente novamente.')
    } finally {
      setIsSaving(false)
    }
  }

  const handleClose = (open: boolean) => {
    if (!open) {
      setNome('')
      setOrganogramaPosicaoIdLider(null)
    }
    onOpenChange(open)
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{departamento ? 'Editar Departamento' : 'Criar Departamento'}</DialogTitle>
          <DialogDescription>
            {departamento
              ? 'Edite as informações do departamento.'
              : 'Preencha os dados para criar um novo departamento.'}
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4">
          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Nome do Departamento *
            </Label>
            <div className="relative">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                <Building className="w-4 h-4" />
              </div>
              <Input
                className={`h-10 pl-12 rounded-lg ${errors.nome ? 'border-destructive' : ''}`}
                placeholder="Digite o nome do departamento..."
                value={nome}
                onChange={(e) => {
                  setNome(e.target.value)
                  if (errors.nome) {
                    setErrors({ ...errors, nome: undefined })
                  }
                }}
                autoFocus
              />
            </div>
            {errors.nome && (
              <p className="text-sm text-destructive mt-1">{errors.nome}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Código do Cliente
            </Label>
            <Input
              className="h-10 rounded-lg"
              value={codigoCliente}
              disabled
            />
          </div>

          {/* TODO: Adicionar dropdown de líder (organogramaPosicaoIdLider) quando necessário */}
          {/* Por enquanto, deixando como null pois não temos lista de posições disponível aqui */}
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)} disabled={isSaving}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={isSaving || !nome.trim() || !user?.orgId}>
            {isSaving ? 'Salvando...' : departamento ? 'Salvar Alterações' : 'Criar Departamento'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
