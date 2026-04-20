import { useState, useEffect, useMemo, useRef } from 'react'
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
import { useVcxIniciativas } from '@presentation/hooks/useVcxIniciativas'
import { mapearRespostaParaIniciativa, type Initiative } from '@shared/utils/vcxMappers'
import { getStatusColor, getStatusIcon } from '@shared/utils/vcxHelpers'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { StatusIniciativa } from '@domain/entities/Vcx360'

interface IniciativasListProps {
  no: NoMapaRelacionamento
}

export function IniciativasList({ no }: IniciativasListProps) {
  const {
    iniciativas,
    statusIniciativas,
    isLoadingListar,
    isLoadingCriar,
    isLoadingAtualizar,
    isLoadingExcluir,
    isLoadingDadosReferencia,
    carregarIniciativas,
    criarNovaIniciativa,
    atualizarIniciativaExistente,
    excluirIniciativaExistente,
    filtrarTemas,
  } = useVcxIniciativas()

  const [isAdding, setIsAdding] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [formData, setFormData] = useState<Partial<Initiative>>({
    titulo: '',
    objetivoKpi: '',
    tema: '',
    status: 'Em Planejamento',
  })
  const [temaInput, setTemaInput] = useState('')

  const posicaoId = no.posicaoId || no.id
  const loadedPosicaoRef = useRef<string | null>(null)
  // Pronto quando o carregamento de referência terminou (evita loading infinito ou campos desabilitados se API retornar []).
  const referenceDataReady = !isLoadingDadosReferencia

  // Carregar iniciativas quando o componente monta ou quando posicaoId muda (evita duplicação em StrictMode)
  useEffect(() => {
    if (posicaoId && loadedPosicaoRef.current !== posicaoId) {
      loadedPosicaoRef.current = posicaoId
      carregarIniciativas(posicaoId)
    }
  }, [posicaoId, carregarIniciativas])

  const temasFiltrados = useMemo(
    () => filtrarTemas(temaInput),
    [temaInput, filtrarTemas],
  )

  // Converter iniciativas do backend para formato do componente e ordenar por data de criação (decrescente)
  const iniciativasFormatadas: Initiative[] = useMemo(() => {
    const formatadas = iniciativas
      .map((iniciativa) => mapearRespostaParaIniciativa(iniciativa))
      .sort((a, b) => {
        // Ordenar por dataCriacao em ordem decrescente (mais recente primeiro)
        const dateA = new Date(a.dataCriacao).getTime()
        const dateB = new Date(b.dataCriacao).getTime()
        return dateB - dateA
      })
    return formatadas
  }, [iniciativas])

  // Mapear status para formato de select
  const statusOptions = useMemo(
    () =>
      statusIniciativas.map((status) => ({
        value: status.id,
        label: status.descricao,
      })),
    [statusIniciativas],
  )

  const handleAddClick = () => {
    setIsAdding(true)
    setFormData({
      titulo: '',
      objetivoKpi: '',
      tema: '',
      status: 'Em Planejamento',
    })
    setTemaInput('')
  }

  const handleEditClick = (iniciativa: Initiative) => {
    setEditingId(iniciativa.id)
    setFormData(iniciativa)
    setTemaInput(iniciativa.tema)
    setIsAdding(false)
  }

  const handleCancel = () => {
    setIsAdding(false)
    setEditingId(null)
    setFormData({
      titulo: '',
      objetivoKpi: '',
      tema: '',
      status: 'Em Planejamento',
    })
    setTemaInput('')
  }

  const handleSave = async () => {
    if (!formData.titulo?.trim() || !formData.objetivoKpi?.trim() || !temaInput.trim() || !posicaoId) {
      return
    }

    try {
      // Encontrar status ID
      const statusSelecionado = statusIniciativas.find(
        (s) => s.descricao === formData.status || s.id === formData.status,
      )

      const payload = {
        organogramaPosicaoId: posicaoId,
        titulo: formData.titulo,
        descricao: formData.objetivoKpi,
        vcxStatusId: statusSelecionado?.id || null,
        vcxTemasId: null, // Será resolvido pelo Use Case
      }

      if (editingId) {
        await atualizarIniciativaExistente({ ...payload, id: editingId }, temaInput.trim())
        setEditingId(null)
      } else {
        await criarNovaIniciativa(payload, temaInput.trim())
        setIsAdding(false)
      }

      // Recarregar iniciativas após criar/atualizar
      await carregarIniciativas(posicaoId)

      setFormData({
        titulo: '',
        objetivoKpi: '',
        tema: '',
        status: 'Em Planejamento',
      })
      setTemaInput('')
    } catch (error) {
      // Erro já tratado no hook com toast
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Tem certeza que deseja excluir esta iniciativa?')) {
      return
    }

    try {
      await excluirIniciativaExistente(id)
      // Recarregar iniciativas após excluir
      if (posicaoId) {
        await carregarIniciativas(posicaoId)
      }
    } catch (error) {
      // Erro já tratado no hook com toast
    }
  }

  return (
    <Card className="p-4">
      <div className="flex items-center justify-between mb-4">
        <p className="text-sm font-semibold">Iniciativas Estratégicas</p>
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

      {!referenceDataReady && (
        <div className="mb-4 p-3 border border-secondary/50 rounded-lg bg-secondary/10 text-secondary-foreground">
          <div className="flex items-center gap-2 text-xs">
            <Spinner size={16} className="text-secondary-foreground" />
            Carregando status e temas antes de abrir o formulário
          </div>
        </div>
      )}

      {/* Formulário de Adicionar */}
      {isAdding && !editingId && (
        <div className="mb-4 p-4 border rounded-lg space-y-3 bg-muted/50">
          <div>
            <label className="text-xs font-medium mb-1 block">
              Título da Iniciativa *
            </label>
            <Input
              value={formData.titulo || ''}
              onChange={(e) =>
                setFormData({ ...formData, titulo: e.target.value })
              }
              placeholder="Título da Iniciativa..."
              maxLength={255}
            />
          </div>

          <div>
            <label className="text-xs font-medium mb-1 block">
              Conectar a um Tema *
            </label>
            <div className="relative">
              <Input
                list="temas-datalist"
                value={temaInput}
                onChange={(e) => {
                  setTemaInput(e.target.value)
                  setFormData({ ...formData, tema: e.target.value })
                }}
                placeholder="Conectar a um Tema..."
                className="text-[10px]"
                disabled={!referenceDataReady}
              />
              <datalist id="temas-datalist">
                {temasFiltrados.map((tema) => (
                  <option key={tema.id} value={tema.descricao} />
                ))}
              </datalist>
            </div>
            {temaInput &&
              !temasFiltrados.some(
                (t) => t.descricao.toLowerCase() === temaInput.toLowerCase(),
              ) && (
                <p className="text-xs text-muted-foreground mt-1">
                  Criar novo tema: {temaInput}
                </p>
              )}
          </div>

          <div>
            <label className="text-xs font-medium mb-1 block">
              Objetivo Principal / KPI *
            </label>
            <Textarea
              value={formData.objetivoKpi || ''}
              onChange={(e) =>
                setFormData({ ...formData, objetivoKpi: e.target.value })
              }
              placeholder="Objetivo Principal / KPI..."
              rows={3}
              className="h-20"
            />
          </div>

          <div>
            <label className="text-xs font-medium mb-1 block">Status</label>
            <Select
              value={
                statusOptions.find(
                  (s) =>
                    s.label === formData.status ||
                    statusIniciativas.find(
                      (si) => si.descricao === formData.status,
                    )?.id === s.value,
                )?.value || statusOptions[0]?.value
              }
              disabled={!referenceDataReady}
              onValueChange={(value) => {
                const status = statusIniciativas.find((s) => s.id === value)
                setFormData({
                  ...formData,
                  status: (status?.descricao as StatusIniciativa) || 'Em Planejamento',
                })
              }}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {statusOptions.map((status) => (
                  <SelectItem key={status.value} value={status.value}>
                    {status.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
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
                !formData.titulo?.trim() ||
                !formData.objetivoKpi?.trim() ||
                !temaInput.trim() ||
                !referenceDataReady ||
                isLoadingCriar
              }
            >
              <Save className="w-4 h-4 mr-2" />
              Criar
            </Button>
          </div>
        </div>
      )}

      {/* Lista de Iniciativas */}
      {isLoadingListar ? (
        <p className="text-xs text-muted-foreground">
          Carregando iniciativas...
        </p>
      ) : iniciativasFormatadas.length === 0 ? (
        <p className="text-xs text-muted-foreground">
          Nenhuma iniciativa cadastrada
        </p>
      ) : (
        <div className="space-y-2">
          {iniciativasFormatadas.map((iniciativa) => (
            <div
              key={iniciativa.id}
              className="border-b border-dashed border-border py-3 last:border-b-0"
            >
              {editingId === iniciativa.id ? (
                // Formulário inline de edição
                <div className="space-y-3">
                  <Input
                    value={formData.titulo || ''}
                    onChange={(e) =>
                      setFormData({ ...formData, titulo: e.target.value })
                    }
                    placeholder="Título"
                    maxLength={255}
                  />
                  <div className="relative">
                    <Input
                      list="temas-datalist-edit"
                      value={temaInput}
                      onChange={(e) => {
                        setTemaInput(e.target.value)
                        setFormData({ ...formData, tema: e.target.value })
                      }}
                      placeholder="Tema"
                      className="text-[10px]"
                      disabled={!referenceDataReady}
                    />
                    <datalist id="temas-datalist-edit">
                      {temasFiltrados.map((tema) => (
                        <option key={tema.id} value={tema.descricao} />
                      ))}
                    </datalist>
                  </div>
                  <Textarea
                    value={formData.objetivoKpi || ''}
                    onChange={(e) =>
                      setFormData({ ...formData, objetivoKpi: e.target.value })
                    }
                    placeholder="Objetivo / KPI"
                    rows={2}
                  />
                  <Select
                    value={
                      statusIniciativas.find((si) => si.descricao === formData.status)?.id ||
                      statusOptions[0]?.value
                    }
                    disabled={!referenceDataReady}
                    onValueChange={(value) => {
                      const status = statusIniciativas.find((s) => s.id === value)
                      setFormData({
                        ...formData,
                        status: (status?.descricao as StatusIniciativa) || 'Em Planejamento',
                      })
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {statusOptions.map((status) => (
                        <SelectItem key={status.value} value={status.value}>
                          {status.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
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
                        !formData.titulo?.trim() ||
                        !formData.objetivoKpi?.trim() ||
                        !temaInput.trim() ||
                        !referenceDataReady ||
                        isLoadingAtualizar
                      }
                    >
                      Salvar
                    </Button>
                  </div>
                </div>
              ) : (
                // Visualização normal
                <div className="flex items-start justify-between gap-2">
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-semibold">{iniciativa.titulo}</p>
                    {iniciativa.objetivoKpi && (
                      <p className="text-xs text-muted-foreground mt-1">
                        {iniciativa.objetivoKpi}
                      </p>
                    )}
                    <div className="flex gap-2 mt-2">
                      {(() => {
                        const StatusIcon = getStatusIcon(iniciativa.status)
                        const badgeClasses = getStatusColor(iniciativa.status)
                        return (
                          <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                            {StatusIcon && <StatusIcon className="h-3 w-3" />}
                            {iniciativa.status}
                          </Badge>
                        )
                      })()}
                      {iniciativa.tema && (
                        <Badge variant="outline">
                          {iniciativa.tema}
                        </Badge>
                      )}
                    </div>
                  </div>
                  <div className="flex gap-1 shrink-0">
                    <Button
                      size="sm"
                      variant="ghost"
                      onClick={() => handleEditClick(iniciativa)}
                      disabled={isLoadingAtualizar}
                    >
                      <Edit className="w-4 h-4" />
                    </Button>
                    <Button
                      size="sm"
                      variant="ghost"
                      onClick={() => handleDelete(iniciativa.id)}
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
