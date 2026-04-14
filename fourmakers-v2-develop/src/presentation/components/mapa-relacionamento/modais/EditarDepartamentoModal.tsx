import { useState, useEffect } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { ListarPosicoesUseCase } from '@domain/usecases/ListarPosicoesUseCase'
import { ListarDepartamentosMapaRelacionamentoUseCase } from '@domain/usecases/ListarDepartamentosMapaRelacionamentoUseCase'
import type { DepartamentoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { DepartamentoResponse, PosicaoCompleta } from '@domain/entities/Organograma'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { AutocompleteSimplesMapa } from '@presentation/components/mapa-relacionamento/seletores/AutocompleteSimplesMapa'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Building, User } from 'lucide-react'
import { toast } from 'sonner'
import { logError } from '@shared/utils/firebaseCrashlytics'

interface EditarDepartamentoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  departamentos: DepartamentoMapaRelacionamento[]
  departamentoSelecionado?: DepartamentoMapaRelacionamento | null
  onSave: (payload: {
    departamentoId: string
    nome: string
    organogramaPosicaoIdLider: string | null
  }) => Promise<void>
}

export function EditarDepartamentoModal({
  open,
  onOpenChange,
  departamentos,
  departamentoSelecionado,
  onSave,
}: EditarDepartamentoModalProps) {
  const { token, user } = useAppSelector((state) => state.auth)
  const { clienteSelecionado } = useAppSelector((state) => state.mapaRelacionamento)
  const clientCode = clienteSelecionado?.codigoCliente || ''
  const [departamentoId, setDepartamentoId] = useState<string>('')
  const [nome, setNome] = useState<string>('')
  const [posicaoIdLider, setPosicaoIdLider] = useState<string | null>(null)
  const [posicoes, setPosicoes] = useState<PosicaoCompleta[]>([])
  const [departamentosCompletos, setDepartamentosCompletos] = useState<DepartamentoResponse[]>([])
  const [loadingPosicoes, setLoadingPosicoes] = useState(false)
  const [errors, setErrors] = useState<{ departamentoId?: string; nome?: string; posicaoIdLider?: string }>({})
  const [isSaving, setIsSaving] = useState(false)

  // Load positions and full departamento data
  useEffect(() => {
    if (!open || !token || !user?.colaboradorOrg?.orgId) return

    const loadData = async () => {
      setLoadingPosicoes(true)
      try {
        const orgId = user.colaboradorOrg.orgId
        const listarPosicoesUseCase = container.resolve(ListarPosicoesUseCase)
        const listarDepartamentosUseCase = container.resolve(ListarDepartamentosMapaRelacionamentoUseCase)

        const [posicoesResponse, departamentosResponse] = await Promise.all([
          listarPosicoesUseCase.execute(token, clientCode, orgId),
          listarDepartamentosUseCase.execute(token, clientCode),
        ])
        setPosicoes(posicoesResponse?.posicoes ?? [])
        setDepartamentosCompletos(departamentosResponse ?? [])
      } catch (error) {
        logError(error, { component: 'EditarDepartamentoModal', action: 'loadData' }, user ?? undefined)
        toast.error('Erro ao carregar posições e departamentos')
      } finally {
        setLoadingPosicoes(false)
      }
    }

    loadData()
  }, [open, token, user?.colaboradorOrg?.orgId, clientCode])

  // Initialize with selected departamento if provided
  useEffect(() => {
    if (open) {
      if (departamentoSelecionado) {
        setDepartamentoId(departamentoSelecionado.cod)
        setNome(departamentoSelecionado.departamento)
        
        // Find the full departamento data to get organogramaPosicaoIdLider
        const departamentoCompleto = departamentosCompletos.find(d => d.id === departamentoSelecionado.cod)
        if (departamentoCompleto?.organogramaPosicaoIdLider) {
          setPosicaoIdLider(departamentoCompleto.organogramaPosicaoIdLider)
        } else {
          setPosicaoIdLider(null)
        }
      } else {
        setDepartamentoId('')
        setNome('')
        setPosicaoIdLider(null)
      }
      setErrors({})
      setIsSaving(false)
    }
  }, [open, departamentoSelecionado, departamentosCompletos])

  // Update nome and posicaoIdLider when departamento is selected
  useEffect(() => {
    if (departamentoId && departamentosCompletos.length > 0) {
      const departamento = departamentos.find(d => d.cod === departamentoId)
      const departamentoCompleto = departamentosCompletos.find(d => d.id === departamentoId)
      
      if (departamento) {
        setNome(departamento.departamento)
      }
      
      if (departamentoCompleto?.organogramaPosicaoIdLider) {
        setPosicaoIdLider(departamentoCompleto.organogramaPosicaoIdLider)
      } else {
        setPosicaoIdLider(null)
      }
      
      // Clear error if exists
      if (errors.departamentoId) {
        setErrors({ ...errors, departamentoId: undefined })
      }
    }
  }, [departamentoId, departamentos, departamentosCompletos])

  const validate = (): boolean => {
    const newErrors: { departamentoId?: string; nome?: string; posicaoIdLider?: string } = {}
    
    if (!departamentoId) {
      newErrors.departamentoId = 'O departamento é obrigatório.'
    }
    
    if (!nome.trim()) {
      newErrors.nome = 'O nome do departamento é obrigatório.'
    } else if (nome.trim().length < 2) {
      newErrors.nome = 'O nome do departamento deve ter pelo menos 2 caracteres.'
    } else if (nome.trim().length > 200) {
      newErrors.nome = 'O nome do departamento não pode exceder 200 caracteres.'
    }

    if (!posicaoIdLider) {
      newErrors.posicaoIdLider = 'A posição líder é obrigatória.'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) {
      return
    }

    setIsSaving(true)
    try {
      await onSave({
        departamentoId,
        nome: nome.trim(),
        organogramaPosicaoIdLider: posicaoIdLider,
      })
      handleClose(false)
    } catch (error) {
      console.error('Erro ao salvar departamento:', error)
      // Error is handled by parent component
    } finally {
      setIsSaving(false)
    }
  }

  const handleClose = (open: boolean) => {
    if (!open) {
      setDepartamentoId('')
      setNome('')
      setPosicaoIdLider(null)
      setErrors({})
    }
    onOpenChange(open)
  }

  // Helper function to get position display name
  const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'
  const getPosicaoDisplayName = (posicao: PosicaoCompleta): string => {
    const cLevelLabel = posicao.cLevel ? ' [C-Level]' : ''
    
    // Buscar a primeira alocação ativa (se houver)
    const alocacaoAtiva = posicao.alocacoes?.find((aloc) => aloc.ativo && !aloc.dataFim) || posicao.alocacoes?.[0]
    
    // Verificar se é vaga:
    // 1. Se não há alocacoes ou array está vazio
    // 2. Se alocacaoAtiva existe mas nomeColaborador é null/undefined/vazio
    // 3. Se alocacaoAtiva existe mas codigoInternoColaborador é null/undefined/vazio/GUID vazio
    const hasValidAlocacao = !!(
      alocacaoAtiva && 
      alocacaoAtiva.nomeColaborador && 
      typeof alocacaoAtiva.nomeColaborador === 'string' &&
      alocacaoAtiva.nomeColaborador.trim() !== '' &&
      alocacaoAtiva.codigoInternoColaborador && 
      typeof alocacaoAtiva.codigoInternoColaborador === 'string' &&
      alocacaoAtiva.codigoInternoColaborador.trim() !== '' &&
      alocacaoAtiva.codigoInternoColaborador !== EMPTY_GUID
    )
    
    if (hasValidAlocacao) {
      return `${alocacaoAtiva.nomeColaborador}${cLevelLabel}`
    }
    
    return `Vago${cLevelLabel} - ${posicao.departamentoNome || 'Sem departamento'}`
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl overflow-visible">
        <DialogHeader>
          <DialogTitle>Editar Departamento</DialogTitle>
          <DialogDescription>
            Selecione um departamento, edite seu nome e escolha o líder.
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-6 py-4 max-h-[calc(90vh-180px)] overflow-y-auto">
          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Departamento *
            </Label>
            <AutocompleteSimplesMapa
              value={departamentoId}
              onValueChange={(value) => {
                setDepartamentoId(value)
                if (errors.departamentoId) {
                  setErrors({ ...errors, departamentoId: undefined })
                }
              }}
              options={departamentos.map(d => ({ id: d.cod, descricao: d.departamento }))}
              placeholder="Selecione um departamento..."
              error={!!errors.departamentoId}
            />
            {errors.departamentoId && (
              <p className="text-sm text-destructive mt-1">{errors.departamentoId}</p>
            )}
          </div>

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
                disabled={!departamentoId}
                autoFocus
              />
            </div>
            {errors.nome && (
              <p className="text-sm text-destructive mt-1">{errors.nome}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Posição Líder *
            </Label>
            <div className="relative">
              <AutocompleteSimplesMapa
                value={posicaoIdLider || ''}
                onValueChange={(value) => {
                  setPosicaoIdLider(value)
                  if (errors.posicaoIdLider) {
                    setErrors({ ...errors, posicaoIdLider: undefined })
                  }
                }}
                options={posicoes.map(p => ({ id: p.id, descricao: getPosicaoDisplayName(p) }))}
                disabled={!departamentoId || loadingPosicoes}
                placeholder={loadingPosicoes ? 'Carregando posições...' : 'Selecione uma posição líder'}
                icon={<User className="w-4 h-4" />}
                error={!!errors.posicaoIdLider}
              />
            </div>
            {errors.posicaoIdLider && (
              <p className="text-sm text-destructive mt-1">{errors.posicaoIdLider}</p>
            )}
            <p className="text-xs text-muted-foreground">
              Selecione a posição que será o líder deste departamento
            </p>
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => handleClose(false)} disabled={isSaving}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={isSaving || !departamentoId || !posicaoIdLider}>
            {isSaving ? 'Salvando...' : 'Salvar Alterações'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
