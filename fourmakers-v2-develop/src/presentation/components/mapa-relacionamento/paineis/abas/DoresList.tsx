import { useState, useEffect, useRef } from 'react'
import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Badge } from '@/components/ui/badge'
import { Plus, Edit, Trash2, X, Save } from 'lucide-react'
import { Spinner } from '@/components/ui/spinner'
import { toast } from 'sonner'
import { useVcxDores } from '@presentation/hooks/useVcxDores'
import {
  mapearRespostaParaDor,
  type PainOpportunity,
} from '@shared/utils/vcxMappers'
import { getImpactBadgeColor, getUrgencyBadgeColor, getImpactIcon, getUrgencyIcon } from '@shared/utils/vcxHelpers'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { DorPayload } from '@domain/entities/VcxDores'

interface DoresListProps {
  no: NoMapaRelacionamento
}

export function DoresList({ no }: DoresListProps) {
  const {
    dores,
    impactos,
    urgencias,
    isLoadingListar,
    isLoadingCriar,
    isLoadingAtualizar,
    isLoadingExcluir,
    isLoadingDadosReferencia,
    carregarDores,
    criarNovaDor,
    atualizarDorExistente,
    excluirDorExistente,
  } = useVcxDores()

  const [isAdding, setIsAdding] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [formData, setFormData] = useState<Partial<PainOpportunity>>({
    title: '',
    description: '',
    priority: 'Média',
    impact: 'Médio',
    vcxUrgenciasDescricao: null,
    vcxImpactosDescricao: null,
  })

  const posicaoId = no.posicaoId || no.id
  const loadedPosicaoRef = useRef<string | null>(null)

  // Funções helper para manter compatibilidade com campos priority/impact
  const resolvePriorityFromDescricao = (descricao: string): 'Alta' | 'Média' | 'Baixa' => {
    const normalized = descricao.toLowerCase().trim()
    if (normalized.includes('urgente') || normalized.includes('alta') || normalized.includes('alto')) return 'Alta'
    if (normalized.includes('normal') || normalized.includes('media') || normalized.includes('medio')) return 'Média'
    if (normalized.includes('baixa') || normalized.includes('baixo')) return 'Baixa'
    return 'Média'
  }

  const resolveImpactFromDescricao = (descricao: string): 'Alto' | 'Médio' | 'Baixo' => {
    const normalized = descricao.toLowerCase().trim()
    if (normalized.includes('alto') || normalized.includes('alta')) return 'Alto'
    if (normalized.includes('medio') || normalized.includes('média')) return 'Médio'
    if (normalized.includes('baixo') || normalized.includes('baixa')) return 'Baixo'
    return 'Médio'
  }

  // Carregar dores quando o componente monta ou quando posicaoId muda (evita duplicação em StrictMode)
  useEffect(() => {
    if (posicaoId && loadedPosicaoRef.current !== posicaoId) {
      loadedPosicaoRef.current = posicaoId
      carregarDores(posicaoId)
    }
  }, [posicaoId, carregarDores])

  // Pronto quando o carregamento de referência terminou (evita loading infinito ou campos desabilitados se API retornar []).
  const referenceDataReady = !isLoadingDadosReferencia

  // Converter dores do backend para formato do componente e ordenar por data de criação (decrescente)
  const doresFormatadas: PainOpportunity[] = dores
    .map((dor) => mapearRespostaParaDor(dor))
    .sort((a, b) => {
      // Ordenar por dataCriacao em ordem decrescente (mais recente primeiro)
      const dateA = new Date(a.dataCriacao).getTime()
      const dateB = new Date(b.dataCriacao).getTime()
      return dateB - dateA
    })

  const handleAddClick = () => {
    setIsAdding(true)
    setFormData({
      title: '',
      description: '',
      priority: 'Média',
      impact: 'Médio',
      vcxUrgenciasDescricao: null,
      vcxImpactosDescricao: null,
    })
  }

  const handleEditClick = (dor: PainOpportunity) => {
    setEditingId(dor.id)
    setFormData(dor)
    setIsAdding(false)
  }

  const handleCancel = () => {
    setIsAdding(false)
    setEditingId(null)
    setFormData({
      title: '',
      description: '',
      priority: 'Média',
      impact: 'Médio',
      vcxUrgenciasDescricao: null,
      vcxImpactosDescricao: null,
    })
  }

  const handleSave = async () => {
    if (!formData.title?.trim()) {
      toast.error('O título é obrigatório')
      return
    }

    if (!formData.description?.trim()) {
      toast.error('A descrição é obrigatória')
      return
    }

    if (!posicaoId) {
      return
    }

    // Garantir que impactos e urgências estão carregados
    if (impactos.length === 0 || urgencias.length === 0) {
      toast.error('Aguarde o carregamento dos dados de referência...')
      return
    }

    // Validar que descrições originais foram selecionadas
    if (!formData.vcxUrgenciasDescricao || !formData.vcxImpactosDescricao) {
      toast.error('Selecione uma opção válida para Prioridade e Impacto')
      return
    }

    try {
      // Encontrar IDs baseados nas descrições originais
      const urgenciaSelecionada = urgencias.find(
        (u) => u.descricao === formData.vcxUrgenciasDescricao
      )
      const impactoSelecionado = impactos.find(
        (i) => i.descricao === formData.vcxImpactosDescricao
      )

      if (!urgenciaSelecionada || !impactoSelecionado) {
        toast.error('Erro ao encontrar ID de impacto ou urgência. Tente novamente.')
        return
      }

      const payload: DorPayload = {
        organogramaPosicaoId: posicaoId,
        titulo: formData.title,
        descricao: formData.description || null,
        vcxImpactosId: impactoSelecionado.id,
        vcxUrgenciasId: urgenciaSelecionada.id,
      }

      if (editingId) {
        await atualizarDorExistente({ ...payload, id: editingId })
        setEditingId(null)
      } else {
        await criarNovaDor(payload)
        setIsAdding(false)
      }

      // Recarregar dores após criar/atualizar
      await carregarDores(posicaoId)

      setFormData({
        title: '',
        description: '',
        priority: 'Média',
        impact: 'Médio',
        vcxUrgenciasDescricao: null,
        vcxImpactosDescricao: null,
      })
    } catch (error) {
      // Erro já tratado no hook com toast
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Tem certeza que deseja excluir esta dor?')) {
      return
    }

    try {
      await excluirDorExistente(id)
      // Recarregar dores após excluir
      if (posicaoId) {
        await carregarDores(posicaoId)
      }
    } catch (error) {
      // Erro já tratado no hook com toast
    }
  }


  return (
    <Card className="p-4">
      <div className="flex items-center justify-between mb-4">
        <p className="text-sm font-semibold">Dores e Oportunidades</p>
        {!isAdding && !editingId && (
          <Button
            size="sm"
            variant="outline"
            onClick={handleAddClick}
            disabled={isLoadingCriar}
          >
            <Plus className="w-4 h-4 mr-2" />
            Adicionar
          </Button>
        )}
      </div>

      {/* Indicador de carregamento dos dados de referência */}
      {!referenceDataReady && (
        <div className="mb-4 p-3 border border-secondary/50 rounded-lg bg-secondary/10 text-secondary-foreground">
          <div className="flex items-center gap-2 text-xs">
            <Spinner size={16} className="text-secondary-foreground" />
            Carregando dados de referência (Impactos e Urgências)...
          </div>
        </div>
      )}

      {/* Formulário de Adicionar - apenas quando está adicionando (não editando) */}
      {isAdding && !editingId && (
        <div className="mb-4 p-4 border rounded-lg space-y-3 bg-muted/50">
          <div>
            <label className="text-xs font-medium mb-1 block">Título *</label>
            <Input
              value={formData.title || ''}
              onChange={(e) =>
                setFormData({ ...formData, title: e.target.value })
              }
              placeholder="Digite o título da dor"
              maxLength={255}
            />
          </div>

          <div>
            <label className="text-xs font-medium mb-1 block">Descrição *</label>
            <Textarea
              value={formData.description || ''}
              onChange={(e) =>
                setFormData({ ...formData, description: e.target.value })
              }
              placeholder="Descreva a dor ou oportunidade"
              rows={3}
              required
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="text-xs font-medium mb-1 block">Prioridade *</label>
              <Select
                value={formData.vcxUrgenciasDescricao || ''}
                onValueChange={(value) => {
                  const urgencia = urgencias.find((u) => u.descricao === value)
                  setFormData({
                    ...formData,
                    vcxUrgenciasDescricao: value,
                    priority: urgencia ? resolvePriorityFromDescricao(urgencia.descricao) : 'Média',
                  })
                }}
                disabled={!referenceDataReady}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a urgência" />
                </SelectTrigger>
                <SelectContent>
                  {urgencias.map((urgencia) => (
                    <SelectItem key={urgencia.id} value={urgencia.descricao}>
                      {urgencia.descricao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <label className="text-xs font-medium mb-1 block">Impacto *</label>
              <Select
                value={formData.vcxImpactosDescricao || ''}
                onValueChange={(value) => {
                  const impacto = impactos.find((i) => i.descricao === value)
                  setFormData({
                    ...formData,
                    vcxImpactosDescricao: value,
                    impact: impacto ? resolveImpactFromDescricao(impacto.descricao) : 'Médio',
                  })
                }}
                disabled={!referenceDataReady}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione o impacto" />
                </SelectTrigger>
                <SelectContent>
                  {impactos.map((impacto) => (
                    <SelectItem key={impacto.id} value={impacto.descricao}>
                      {impacto.descricao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="flex gap-2 justify-end">
            <Button size="sm" variant="outline" onClick={handleCancel}>
              <X className="w-4 h-4 mr-2" />
              Cancelar
            </Button>
            <Button
              size="sm"
              onClick={handleSave}
              disabled={
                !formData.title?.trim() ||
                !formData.description?.trim() ||
                !formData.vcxUrgenciasDescricao ||
                !formData.vcxImpactosDescricao ||
                !referenceDataReady ||
                isLoadingCriar
              }
              title={!referenceDataReady ? 'Aguarde o carregamento dos dados...' : ''}
            >
              <Save className="w-4 h-4 mr-2" />
              Criar
            </Button>
          </div>
        </div>
      )}

      {/* Lista de Dores */}
      {isLoadingListar ? (
        <p className="text-xs text-muted-foreground">Carregando dores...</p>
      ) : doresFormatadas.length === 0 ? (
        <p className="text-xs text-muted-foreground">Nenhuma dor cadastrada</p>
      ) : (
        <div className="space-y-2">
          {doresFormatadas.map((dor) => (
            <div
              key={dor.id}
              className="border-b border-dashed border-border py-3 last:border-b-0"
            >
              {editingId === dor.id ? (
                // Formulário inline de edição
                <div className="space-y-3">
                  <Input
                    value={formData.title || ''}
                    onChange={(e) =>
                      setFormData({ ...formData, title: e.target.value })
                    }
                    placeholder="Título"
                    maxLength={255}
                  />
                  <Textarea
                    value={formData.description || ''}
                    onChange={(e) =>
                      setFormData({ ...formData, description: e.target.value })
                    }
                    placeholder="Descrição *"
                    rows={2}
                    required
                  />
                  <div className="grid grid-cols-2 gap-2">
                    <Select
                      value={formData.vcxUrgenciasDescricao || ''}
                      onValueChange={(value) => {
                        const urgencia = urgencias.find((u) => u.descricao === value)
                        setFormData({
                          ...formData,
                          vcxUrgenciasDescricao: value,
                          priority: urgencia ? resolvePriorityFromDescricao(urgencia.descricao) : 'Média',
                        })
                      }}
                      disabled={!referenceDataReady}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione a urgência" />
                      </SelectTrigger>
                      <SelectContent>
                        {urgencias.map((urgencia) => (
                          <SelectItem key={urgencia.id} value={urgencia.descricao}>
                            {urgencia.descricao}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <Select
                      value={formData.vcxImpactosDescricao || ''}
                      onValueChange={(value) => {
                        const impacto = impactos.find((i) => i.descricao === value)
                        setFormData({
                          ...formData,
                          vcxImpactosDescricao: value,
                          impact: impacto ? resolveImpactFromDescricao(impacto.descricao) : 'Médio',
                        })
                      }}
                      disabled={!referenceDataReady}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione o impacto" />
                      </SelectTrigger>
                      <SelectContent>
                        {impactos.map((impacto) => (
                          <SelectItem key={impacto.id} value={impacto.descricao}>
                            {impacto.descricao}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="flex gap-2 justify-end">
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={handleCancel}
                    >
                      Cancelar
                    </Button>
                    <Button
                      size="sm"
                      onClick={handleSave}
                      disabled={
                        !formData.title?.trim() ||
                        !formData.description?.trim() ||
                        !formData.vcxUrgenciasDescricao ||
                        !formData.vcxImpactosDescricao ||
                        !referenceDataReady ||
                        isLoadingAtualizar
                      }
                      title={!referenceDataReady ? 'Aguarde o carregamento dos dados...' : ''}
                    >
                      Salvar
                    </Button>
                  </div>
                </div>
              ) : (
                // Visualização normal
                <div className="flex items-start justify-between gap-2">
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-semibold">{dor.title}</p>
                    {dor.description && (
                      <p className="text-xs text-muted-foreground mt-1">
                        {dor.description}
                      </p>
                    )}
                    <div className="flex gap-2 mt-2 flex-wrap">
                      {(() => {
                        const UrgencyIcon = getUrgencyIcon(dor.vcxUrgenciasDescricao)
                        const badgeClasses = getUrgencyBadgeColor(dor.vcxUrgenciasDescricao)
                        return (
                          <Badge 
                            className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}
                          >
                            {UrgencyIcon && <UrgencyIcon className="h-3 w-3" />}
                            {dor.vcxUrgenciasDescricao || dor.priority}
                          </Badge>
                        )
                      })()}
                      {(() => {
                        const ImpactIcon = getImpactIcon(dor.vcxImpactosDescricao)
                        const badgeClasses = getImpactBadgeColor(dor.vcxImpactosDescricao)
                        return (
                          <Badge 
                            className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}
                          >
                            {ImpactIcon && <ImpactIcon className="h-3 w-3" />}
                            {dor.vcxImpactosDescricao || dor.impact}
                          </Badge>
                        )
                      })()}
                    </div>
                  </div>
                  <div className="flex gap-1 shrink-0">
                    <Button
                      size="sm"
                      variant="ghost"
                      onClick={() => handleEditClick(dor)}
                      disabled={isLoadingAtualizar}
                    >
                      <Edit className="w-4 h-4" />
                    </Button>
                    <Button
                      size="sm"
                      variant="ghost"
                      onClick={() => handleDelete(dor.id)}
                      disabled={isLoadingExcluir}
                    >
                      <Trash2 className="w-4 h-4 text-destructive" />
                    </Button>
                  </div>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </Card>
  )
}
