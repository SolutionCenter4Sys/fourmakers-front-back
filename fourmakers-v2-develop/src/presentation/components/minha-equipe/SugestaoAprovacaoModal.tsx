import * as React from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'
import { Label } from '@/components/ui/label'
import { cn } from '@/lib/utils'

interface SugestaoAprovacaoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: (comentario: string) => void
  tipo: 'aprovar' | 'rejeitar'
}

const MIN_CARACTERES = 10
const MAX_CARACTERES = 500

/**
 * Modal para capturar comentário obrigatório ao aprovar ou rejeitar uma sugestão de habilidade.
 * Valida o comentário em tempo real (mínimo 10, máximo 500 caracteres).
 * 
 * @param open - Controla se o modal está aberto
 * @param onOpenChange - Callback quando o estado de abertura muda
 * @param onConfirm - Callback quando o comentário é confirmado (recebe o comentário como parâmetro)
 * @param tipo - Tipo de ação: 'aprovar' ou 'rejeitar'
 */
export const SugestaoAprovacaoModal = ({
  open,
  onOpenChange,
  onConfirm,
  tipo,
}: SugestaoAprovacaoModalProps) => {
  const [comentario, setComentario] = React.useState('')
  const [erro, setErro] = React.useState<string | null>(null)

  // Validar comentário em tempo real
  React.useEffect(() => {
    if (comentario.length === 0) {
      setErro(null)
      return
    }

    if (comentario.length < MIN_CARACTERES) {
      setErro(
        `O comentário deve ter no mínimo ${MIN_CARACTERES} caracteres. Faltam ${
          MIN_CARACTERES - comentario.length
        } caracteres.`,
      )
      return
    }

    if (comentario.length > MAX_CARACTERES) {
      setErro(
        `O comentário deve ter no máximo ${MAX_CARACTERES} caracteres. Você digitou ${
          comentario.length - MAX_CARACTERES
        } caracteres a mais.`,
      )
      return
    }

    setErro(null)
  }, [comentario])

  // Resetar estado quando modal fecha
  React.useEffect(() => {
    if (!open) {
      setComentario('')
      setErro(null)
    }
  }, [open])

  const isValid = comentario.length >= MIN_CARACTERES && comentario.length <= MAX_CARACTERES

  const handleConfirm = () => {
    if (isValid) {
      onConfirm(comentario)
      onOpenChange(false)
    }
  }

  const handleCancel = () => {
    onOpenChange(false)
  }

  const caracteresRestantes = MAX_CARACTERES - comentario.length
  const tipoLabel = tipo === 'aprovar' ? 'Aprovar' : 'Rejeitar'
  const tipoDescricao =
    tipo === 'aprovar'
      ? 'Adicione um comentário explicando o motivo da aprovação desta sugestão de habilidade.'
      : 'Adicione um comentário explicando o motivo da rejeição desta sugestão de habilidade.'

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      {/* max-w-2xl usado para acomodar textarea com até 500 caracteres */}
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{tipoLabel} Sugestão de Habilidade</DialogTitle>
          <DialogDescription>{tipoDescricao}</DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="comentario">
              Comentário <span className="text-destructive">*</span>
            </Label>
            <Textarea
              id="comentario"
              placeholder="Digite seu comentário aqui..."
              value={comentario}
              onChange={(e) => setComentario(e.target.value)}
              className={cn(
                'min-h-[120px]',
                erro && 'border-destructive focus-visible:ring-destructive',
              )}
              maxLength={MAX_CARACTERES}
            />
            <div className="flex items-center justify-between text-xs">
              <div className="space-y-1">
                {erro && (
                  <p
                    id="erro-comentario"
                    className="text-destructive"
                    role="alert"
                    aria-live="polite"
                  >
                    {erro}
                  </p>
                )}
                {!erro && comentario.length > 0 && (
                  <p className="text-muted-foreground" aria-live="polite">
                    Comentário válido
                  </p>
                )}
              </div>
              <p
                className={cn(
                  'text-muted-foreground',
                  caracteresRestantes < 50 && 'text-warning',
                  caracteresRestantes < 0 && 'text-destructive',
                )}
              >
                {comentario.length} / {MAX_CARACTERES} caracteres
              </p>
            </div>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={handleCancel}
            type="button"
            aria-label="Cancelar aprovação ou rejeição"
          >
            Cancelar
          </Button>
          <Button
            onClick={handleConfirm}
            disabled={!isValid}
            type="button"
            variant={tipo === 'aprovar' ? 'primary' : 'destructive'}
            aria-label={`${tipoLabel} sugestão de habilidade`}
            aria-describedby={!isValid ? 'erro-comentario' : undefined}
          >
            {tipoLabel}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
