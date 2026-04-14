import type { ReactNode } from 'react'
import { Bell } from 'lucide-react'

import { Button } from '@/components/ui/button'
import { useReleaseModal } from './ReleaseModalContext'

interface PageHeaderProps {
  /** Texto ou nó React (ex.: título com tooltip no nome). */
  title: ReactNode
  description?: string
  /** Elemento opcional antes do título (ex.: botão Voltar). */
  titlePrefix?: ReactNode
  actions?: ReactNode
}

export const PageHeader = ({ title, description, titlePrefix, actions }: PageHeaderProps) => {
  const { hasReleaseContent, openReleaseModal } = useReleaseModal()

  return (
    <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
      <div>
        <div className="flex items-center gap-2 min-w-0">
          {titlePrefix}
          <h1 className="page-title min-w-0 flex flex-wrap items-baseline gap-x-1 gap-y-0">{title}</h1>
          {hasReleaseContent && (
            <Button
              variant="ghost"
              size="icon"
              onClick={openReleaseModal}
              className="shrink-0 rounded-pillToken h-9 w-9 text-muted-foreground hover:text-foreground"
              aria-label="Ver novidades desta tela"
            >
              <Bell className="h-5 w-5" aria-hidden />
            </Button>
          )}
        </div>
        {description && (
          <p className="text-muted-foreground mt-0.5 text-sm">
            {description}
          </p>
        )}
      </div>
      {actions && (
        <div className="flex flex-col md:flex-row items-stretch md:items-center gap-2 w-full md:w-auto">
          {actions}
        </div>
      )}
    </div>
  )
}

