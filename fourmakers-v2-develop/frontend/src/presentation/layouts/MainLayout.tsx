import { useState, useEffect } from 'react'
import { Outlet } from 'react-router-dom'

import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { fetchMenuData } from '@app/store/slices/menuSlice'
import { setToken, fetchShowmeProfile } from '@app/store/slices/authSlice'
import { fetchParametrosConfiguracao } from '@app/store/slices/parametrosSlice'

import { Header } from '@presentation/components/Header'
import { Sidebar } from '@presentation/components/Sidebar'
import { ReleaseModalProvider } from '@presentation/components/common'
import { Toaster } from '@/components/ui/toaster'
import { Toaster as SonnerToaster } from '@/components/ui/sonner'

import { ContentArea, ContentWrapper, LayoutContainer, MainContent } from './MainLayout.styles'

export const MainLayout = () => {
  // Inicia com sidebar fechado em mobile
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(() => {
    return window.innerWidth <= 768
  })
  const dispatch = useAppDispatch()
  const menuStatus = useAppSelector((state) => state.menu.status)
  const authToken = useAppSelector((state) => state.auth.token)
  const user = useAppSelector((state) => state.auth.user)
  const authStatus = useAppSelector((state) => state.auth.status)
  const parametrosStatus = useAppSelector((state) => state.parametros.status)

  // Sincroniza o token do localStorage com o Redux se necessário
  useEffect(() => {
    const tokenFromStorage = localStorage.getItem('authToken')
    if (tokenFromStorage && !authToken) {
      dispatch(setToken(tokenFromStorage))
    }
  }, [dispatch, authToken])

  // Carrega o perfil do usuário se há token mas não há usuário
  useEffect(() => {
    if (authToken && !user && authStatus !== 'loading') {
      void dispatch(fetchShowmeProfile())
    }
  }, [dispatch, authToken, user, authStatus])

  // Carrega os parâmetros de configuração após o login
  useEffect(() => {
    if (authToken && parametrosStatus === 'idle') {
      void dispatch(fetchParametrosConfiguracao(authToken))
    }
  }, [dispatch, authToken, parametrosStatus])

  // Fecha o sidebar quando a tela é redimensionada para mobile
  useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth <= 768) {
        setIsSidebarCollapsed(true)
      }
    }

    window.addEventListener('resize', handleResize)
    return () => window.removeEventListener('resize', handleResize)
  }, [])

  // Carrega o menu quando o layout é montado e há token
  useEffect(() => {
    if (menuStatus === 'idle' && authToken) {
      void dispatch(fetchMenuData())
    }
  }, [dispatch, menuStatus, authToken])

  const toggleSidebar = () => {
    setIsSidebarCollapsed((prev) => !prev)
  }

  const closeSidebar = () => {
    setIsSidebarCollapsed(true)
  }

  const openSidebar = () => {
    setIsSidebarCollapsed(false)
  }

  return (
    <LayoutContainer $sidebarOffset={isSidebarCollapsed ? '72px' : '260px'}>
      <Header onMenuToggle={openSidebar} />
      <MainContent $sidebarCollapsed={isSidebarCollapsed}>
        <Sidebar 
          collapsed={isSidebarCollapsed} 
          onClose={closeSidebar}
          onToggleCollapse={toggleSidebar}
        />
        <ContentWrapper>
          <ContentArea>
            <ReleaseModalProvider>
              <Outlet />
            </ReleaseModalProvider>
          </ContentArea>
        </ContentWrapper>
      </MainContent>
      <Toaster />
      <SonnerToaster />
    </LayoutContainer>
  )
}
