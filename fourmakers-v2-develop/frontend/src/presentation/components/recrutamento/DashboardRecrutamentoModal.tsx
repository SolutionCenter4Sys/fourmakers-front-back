import type { ReactNode } from 'react'

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { cn } from '@/lib/utils'

interface DashboardRecrutamentoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  titulo: string
  descricao?: string
  children?: ReactNode
  contentClassName?: string
}

export const DashboardRecrutamentoModal = ({
  open,
  onOpenChange,
  titulo,
  descricao = 'Em breve',
  children,
  contentClassName,
}: DashboardRecrutamentoModalProps) => (
  <Dialog open={open} onOpenChange={onOpenChange}>
    <DialogContent
      className={cn(
        'overflow-hidden max-h-[90vh] flex flex-col',
        contentClassName || 'max-w-lg',
      )}
    >
      <DialogHeader className="shrink-0">
        <DialogTitle>{titulo}</DialogTitle>
        <DialogDescription>{descricao}</DialogDescription>
      </DialogHeader>
      <div className="overflow-auto min-h-0 flex-1">
        {children}
      </div>
    </DialogContent>
  </Dialog>
)
