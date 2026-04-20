import * as React from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Loader2 } from 'lucide-react'
import {
  fetchNiveisCompetencia,
  fetchNiveisSoftskill,
  fetchNiveisMetodologia,
  fetchNiveisDominio,
  fetchNiveisIdioma,
} from '@data/api/PerfilAtuacaoApi'
import { useAppSelector } from '@app/store/hooks'

interface SenioritySelectionModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  perfilTipoId: number
  onSelect: (nivelId: number) => void
}

export const SenioritySelectionModal = ({
  open,
  onOpenChange,
  perfilTipoId,
  onSelect,
}: SenioritySelectionModalProps) => {
  const { token } = useAppSelector((state) => state.auth)
  const [niveis, setNiveis] = React.useState<Array<{ id: number; descricao: string }>>([])
  const [isLoading, setIsLoading] = React.useState(false)
  const [selectedNivelId, setSelectedNivelId] = React.useState<number | null>(null)
  const [selectedNivelDescricao, setSelectedNivelDescricao] = React.useState<string | null>(null)
  const [showConfirmation, setShowConfirmation] = React.useState(false)
  const [error, setError] = React.useState<string | null>(null)

  // Carregar níveis baseado no tipo da skill
  React.useEffect(() => {
    if (!open || !token) return

    const loadNiveis = async () => {
      setIsLoading(true)
      setError(null)
      setSelectedNivelId(null)
      setSelectedNivelDescricao(null)
      setShowConfirmation(false)

      try {
        let niveisData: Array<{ id: number; descricao: string }> = []

        // Mapear perfilTipoId para o endpoint correto
        // 1 = COMPETENCIA (hard), 3 = METODOLOGIA, 8 = SOFTSKILL, 9 = IDIOMA, 4 = DOMINIO
        if (perfilTipoId === 1) {
          niveisData = await fetchNiveisCompetencia(token)
        } else if (perfilTipoId === 8) {
          niveisData = await fetchNiveisSoftskill(token)
        } else if (perfilTipoId === 3) {
          niveisData = await fetchNiveisMetodologia(token)
        } else if (perfilTipoId === 4) {
          niveisData = await fetchNiveisDominio(token)
        } else if (perfilTipoId === 9) {
          niveisData = await fetchNiveisIdioma(token)
        } else {
          // Fallback para hard skill
          niveisData = await fetchNiveisCompetencia(token)
        }

        setNiveis(niveisData)
      } catch (err) {
        console.error('Erro ao carregar níveis de senioridade:', err)
        setError('Erro ao carregar níveis de senioridade. Tente novamente.')
      } finally {
        setIsLoading(false)
      }
    }

    loadNiveis()
  }, [open, token, perfilTipoId])

  const handleSelectNivel = (nivelId: number, descricao: string) => {
    setSelectedNivelId(nivelId)
    setSelectedNivelDescricao(descricao)
    setShowConfirmation(true)
  }

  const handleConfirm = () => {
    if (selectedNivelId !== null) {
      onSelect(selectedNivelId)
      onOpenChange(false)
      setSelectedNivelId(null)
      setSelectedNivelDescricao(null)
      setShowConfirmation(false)
    }
  }

  const handleCancelConfirmation = () => {
    setShowConfirmation(false)
  }

  const handleCancel = () => {
    onOpenChange(false)
    setSelectedNivelId(null)
    setSelectedNivelDescricao(null)
    setShowConfirmation(false)
    setError(null)
  }

  // Resetar confirmação quando o modal fechar
  React.useEffect(() => {
    if (!open) {
      setShowConfirmation(false)
      setSelectedNivelId(null)
      setSelectedNivelDescricao(null)
    }
  }, [open])

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Selecione seu nível de senioridade</DialogTitle>
          <DialogDescription>
            Escolha o nível que melhor representa sua experiência com esta habilidade.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          {showConfirmation ? (
            /* Tela de confirmação */
            <div className="space-y-4">
              <div className="p-4 rounded-lg border border-warning/50 bg-warning/10">
                <p className="text-sm font-medium text-warning mb-2">
                  Você tem certeza da definição usada?
                </p>
                <p className="text-sm text-warning/90">
                  Você selecionou: <strong>{selectedNivelDescricao}</strong>
                </p>
              </div>
              <div className="flex justify-end gap-2">
                <button
                  onClick={handleCancelConfirmation}
                  className="px-4 py-2 text-sm font-medium text-primaryText hover:bg-accent rounded-lg transition-colors"
                >
                  Voltar
                </button>
                <button
                  onClick={handleConfirm}
                  className="px-4 py-2 text-sm font-medium text-white bg-primary hover:bg-primary/90 rounded-lg transition-colors"
                >
                  Sim, tenho certeza
                </button>
              </div>
            </div>
          ) : (
            <>
              {isLoading ? (
                <div className="flex items-center justify-center py-8">
                  <Loader2 className="w-6 h-6 animate-spin text-primary" />
                </div>
              ) : error ? (
                <div className="text-sm text-destructive text-center py-4">{error}</div>
              ) : niveis.length === 0 ? (
                <div className="text-sm text-muted-foreground text-center py-4">
                  Nenhum nível disponível
                </div>
              ) : (
                <div className="space-y-2 max-h-64 overflow-y-auto">
                  {niveis.map((nivel) => (
                    <label
                      key={nivel.id}
                      className="flex items-start gap-3 p-3 rounded-lg border border-input hover:bg-accent cursor-pointer transition-colors"
                    >
                      <input
                        type="radio"
                        name="seniority-level"
                        value={nivel.id}
                        checked={selectedNivelId === nivel.id}
                        onChange={() => handleSelectNivel(nivel.id, nivel.descricao)}
                        className="mt-1 w-4 h-4 text-primary border-borderDefault focus:ring-primary focus:ring-2"
                      />
                      <span className="text-sm text-primaryText flex-1">{nivel.descricao}</span>
                    </label>
                  ))}
                </div>
              )}

              <div className="flex justify-end gap-2 pt-4">
                <button
                  onClick={handleCancel}
                  className="px-4 py-2 text-sm font-medium text-primaryText hover:bg-accent rounded-lg transition-colors"
                >
                  Cancelar
                </button>
              </div>
            </>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
