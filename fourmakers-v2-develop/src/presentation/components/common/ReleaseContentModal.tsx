import { format, parseISO } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { Rocket, Play } from 'lucide-react'

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'

import type { ReleaseContentItem } from '@shared/types/releaseContent'

export interface ReleaseContentModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  /** Título geral do modal: "Novidades fresquinhas - [Nome da tela]" */
  nomeTela: string
  /** Lista de itens de conteúdo (título + data, conteúdo, botão mídia opcional) */
  itens: ReleaseContentItem[]
  /** Ao clicar em Fechar (modal fecha; na próxima visita reabre no load) */
  onFechar?: () => void
  /** Ao clicar em Não ver novamente (marca como visto; só reabre ao clicar no sino) */
  onNaoVerNovamente?: () => void
}

function formatDataInsercao(dataInsercao: string): string {
  try {
    const date = parseISO(dataInsercao)
    if (Number.isNaN(date.getTime())) return dataInsercao
    return format(date, "d 'de' MMMM 'de' yyyy", { locale: ptBR })
  } catch {
    return dataInsercao
  }
}

export function ReleaseContentModal({
  open,
  onOpenChange,
  nomeTela,
  itens,
  onFechar,
  onNaoVerNovamente,
}: ReleaseContentModalProps) {
  const handleOpenChange = (value: boolean) => {
    onOpenChange(value)
    if (!value) onFechar?.()
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-2 bg-primary/10 rounded-lg shrink-0">
              <Rocket className="h-6 w-6 text-primary" aria-hidden />
            </div>
            <div className="min-w-0">
              <DialogTitle className="text-xl font-semibold text-primaryText">
                Novidades fresquinhas – {nomeTela}
              </DialogTitle>
              <DialogDescription className="sr-only">
                Conteúdos de release para esta tela
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-6 py-2">
          {itens.map((item, index) => {
            const hasMidia = Boolean(item.midiaUrl?.trim())
            // key estável: ReleaseContentItem não tem id; dataInsercao + index evita colisão quando há vários itens na mesma data
            return (
              <div
                key={`${item.dataInsercao}-${index}`}
                className="rounded-lg border border-borderSoft bg-surfaceSubtle/50 p-4 space-y-3"
              >
                <div className="flex flex-wrap items-baseline gap-x-2 gap-y-1">
                  <h3 className="text-base font-semibold text-primaryText">
                    {item.titulo}
                  </h3>
                  <span className="text-sm text-secondaryText">
                    {formatDataInsercao(item.dataInsercao)}
                  </span>
                </div>
                <div className="text-sm text-primaryText whitespace-pre-line leading-relaxed">
                  {item.conteudo}
                </div>
                {hasMidia && (
                  <div>
                    <Button variant="secondary" size="sm" asChild>
                      <a
                        href={item.midiaUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="inline-flex items-center gap-2"
                      >
                        <Play className="h-4 w-4 shrink-0" aria-hidden />
                        {item.midiaLabel ?? 'Quero ver!'}
                      </a>
                    </Button>
                  </div>
                )}
              </div>
            )
          })}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={onNaoVerNovamente}>
            Não ver novamente
          </Button>
          <Button onClick={onFechar}>
            Fechar
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
