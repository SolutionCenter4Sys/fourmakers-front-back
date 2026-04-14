import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { useLocation } from 'react-router-dom'

import { useAppSelector } from '@app/store/hooks'

import { releaseContentsByMenuCode } from '@shared/data/releaseContents'
import type { ReleaseContentItem } from '@shared/types/releaseContent'
import { getNomeMenuByMenuCode } from '@shared/utils/releaseContentMenuUtils'
import {
  getMenuCodeByPath,
  markReleaseAsSeen,
  wasReleaseSeen,
} from '@shared/utils/releaseContentUtils'

import { ReleaseContentModal } from './ReleaseContentModal'

interface ReleaseModalContextValue {
  /** Indica se a tela atual tem conteúdo de release (exibe o sino) */
  hasReleaseContent: boolean
  /** Abre o modal de release da tela atual (ao clicar no sino) */
  openReleaseModal: () => void
}

const ReleaseModalContext = createContext<ReleaseModalContextValue | null>(null)

export function useReleaseModal(): ReleaseModalContextValue {
  const ctx = useContext(ReleaseModalContext)
  if (!ctx) {
    return {
      hasReleaseContent: false,
      openReleaseModal: () => {},
    }
  }
  return ctx
}

interface ReleaseModalProviderProps {
  children: React.ReactNode
}

export function ReleaseModalProvider({ children }: ReleaseModalProviderProps) {
  const location = useLocation()
  const menuItemsRaw = useAppSelector((state) => state.menu?.menuItems)
  const menuItems = Array.isArray(menuItemsRaw) ? menuItemsRaw : []

  const [releaseToShow, setReleaseToShow] = useState<{
    menuCode: string
    itens: ReleaseContentItem[]
  } | null>(null)
  const [open, setOpen] = useState(false)

  const menuCode = useMemo(
    () => getMenuCodeByPath(location.pathname),
    [location.pathname]
  )
  const itens = useMemo(
    () => (menuCode ? releaseContentsByMenuCode[menuCode] : undefined),
    [menuCode]
  )
  const hasReleaseContent = Boolean(itens?.length)

  // Atualiza releaseToShow quando muda a rota; auto-abre apenas se não foi "Não ver novamente"
  useEffect(() => {
    if (!menuCode || !itens?.length) {
      setReleaseToShow(null)
      setOpen(false)
      return
    }
    setReleaseToShow({ menuCode, itens })
    if (!wasReleaseSeen(menuCode)) {
      setOpen(true)
    } else {
      setOpen(false)
    }
  }, [menuCode, itens])

  const openReleaseModal = useCallback(() => {
    if (releaseToShow) setOpen(true)
  }, [releaseToShow])

  const handleClose = useCallback(() => {
    setOpen(false)
  }, [])

  const handleNaoVerNovamente = useCallback(() => {
    if (releaseToShow) {
      markReleaseAsSeen(releaseToShow.menuCode)
      setOpen(false)
    }
  }, [releaseToShow])

  const contextValue = useMemo<ReleaseModalContextValue>(
    () => ({
      hasReleaseContent,
      openReleaseModal,
    }),
    [hasReleaseContent, openReleaseModal]
  )

  const nomeTela = releaseToShow
    ? getNomeMenuByMenuCode(menuItems, releaseToShow.menuCode)
    : ''

  return (
    <ReleaseModalContext.Provider value={contextValue}>
      {children}
      {releaseToShow && (
        <ReleaseContentModal
          open={open}
          onOpenChange={(next) => {
            if (!next) setOpen(false)
          }}
          nomeTela={nomeTela}
          itens={releaseToShow.itens}
          onFechar={handleClose}
          onNaoVerNovamente={handleNaoVerNovamente}
        />
      )}
    </ReleaseModalContext.Provider>
  )
}
