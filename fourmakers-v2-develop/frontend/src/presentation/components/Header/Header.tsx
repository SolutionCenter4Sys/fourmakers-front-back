import React, { useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { Icon } from '@/components/ui/icon'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'

import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { logout } from '@app/store/slices/authSlice'
import type { MenuResource } from '@domain/entities/MenuResource'
import { useNotificacoes } from '@presentation/hooks/useNotificacoes'
import { NotificacoesPopover } from './NotificacoesPopover'

import { flutterflowRouteMap } from '@shared/utils/flutterflowRouteMap'
import logoFourmakers from '@assets/logo-fourmakers.svg'

import {
  ActionButton,
  ActionsArea,
  BrandArea,
  HeaderContainer,
  HamburgerButton,
  UserAvatar,
  ProfileDropdown,
  ProfileDropdownOverlay,
  ProfileInfo,
  ProfileName,
  ProfileEmail,
  ProfileMenu,
  ProfileMenuItem,
  LogoutMenuItem,
} from './Header.styles'

interface HeaderProps {
  onMenuToggle?: () => void
}

const safeText = (value: unknown): string => {
  if (value === null || value === undefined) return ''
  if (typeof value === 'string') return value
  if (typeof value === 'number' || typeof value === 'boolean') return String(value)
  try {
    return JSON.stringify(value)
  } catch {
    return String(value)
  }
}

const safeString = (value: unknown): string => (typeof value === 'string' ? value : safeText(value))

export const Header = ({ onMenuToggle }: HeaderProps) => {
  type ThemeMode = 'light' | 'dark'
  const THEME_KEY = 'fourmakers-ui-theme-v2'

  const initialTheme = useMemo<ThemeMode>(() => {
    if (typeof window === 'undefined') return 'light'
    const stored = localStorage.getItem(THEME_KEY) as ThemeMode | null
    return stored === 'light' || stored === 'dark' ? stored : 'light'
  }, [])

  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const { menuItems } = useAppSelector((state) => state.menu)
  const { user } = useAppSelector((state) => state.auth)
  const [isProfileDropdownOpen, setIsProfileDropdownOpen] = useState(false)
  const [isNotificacoesPopoverOpen, setIsNotificacoesPopoverOpen] = useState(false)
  const [theme, setTheme] = useState<ThemeMode>(initialTheme)
  const { contador, atualizarContador } = useNotificacoes()
  const notificacoesButtonRef = React.useRef<HTMLButtonElement>(null)

  // Detecta o ambiente
  const environment = import.meta.env.MODE
  const showEnvironmentBadge = environment === 'development' || environment === 'homologation'
  
  const getEnvironmentConfig = () => {
    if (environment === 'development') {
      return {
        label: 'Desenvolvimento',
        badgeClassName: 'border-error text-error bg-error/10',
      }
    }
    if (environment === 'homologation') {
      return {
        label: 'Homologação',
        badgeClassName: 'border-warning text-warning bg-warning/10',
      }
    }
    return null
  }

  const envConfig = getEnvironmentConfig()

  const handleMenuItemClick = (item: MenuResource) => {
    if (item.emBreve) return

    // Fecha o dropdown se estiver aberto
    setIsProfileDropdownOpen(false)

    if (item.linkExternoNovaPagina) {
      window.open(item.linkExternoNovaPagina, '_blank')
    } else if (item.rotaReact) {
      navigate(item.rotaReact)
    } else {
      const code = item.codigoRecurso || item.codigoRecursoMenu
      const routeInfo = code
        ? (flutterflowRouteMap[code] ?? flutterflowRouteMap[code.toLowerCase()])
        : undefined
      if (routeInfo) {
        if (routeInfo.isFlutterflow) {
          navigate(`/page/${routeInfo.path}`)
        } else {
          navigate(routeInfo.path)
        }
      } else {
        console.warn(`[React Header] Mapeamento de rota para ${code ?? item.codigoRecurso} não encontrado.`)
      }
    }
  }

  const headerMenuItems = menuItems
    .filter((item) => item.tipoMenu === 'item_header' && item.visivel)
    .sort((a, b) => a.ordenacao - b.ordenacao)

  const profileMenuItems = menuItems
    .filter((item) => item.tipoMenu === 'item_profile' && item.visivel)
    .sort((a, b) => a.ordenacao - b.ordenacao)

  const handleLogout = () => {
    void dispatch(logout())
  }

  const applyTheme = (mode: ThemeMode) => {
    if (typeof document === 'undefined') return
    const root = document.documentElement
    root.classList.toggle('dark', mode === 'dark')
  }

  useEffect(() => {
    applyTheme(theme)
    localStorage.setItem(THEME_KEY, theme)
  }, [theme])

  useEffect(() => {
    // Garante light como padrão na primeira montagem
    applyTheme(initialTheme)
  }, [initialTheme])

  const toggleTheme = () => {
    setTheme((prev) => (prev === 'dark' ? 'light' : 'dark'))
  }

  const toggleProfileDropdown = () => {
    setIsProfileDropdownOpen((prev) => !prev)
  }

  const closeProfileDropdown = () => {
    setIsProfileDropdownOpen(false)
  }

  const userInitials = user?.nomeColaborador
    ?.split(' ')
    .map((n) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2) || 'U'

  const handleLogoClick = () => {
    navigate('/dashboard')
  }

  return (
    <HeaderContainer>
      <BrandArea>
	        {onMenuToggle && (
	          <HamburgerButton onClick={onMenuToggle} aria-label="Abrir menu" title="Abrir menu">
	            <Icon name="menu" size={20} />
	          </HamburgerButton>
	        )}
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <img
            src={logoFourmakers}
            alt="Four Makers"
            style={{ height: '40px', cursor: 'pointer' }}
            onClick={handleLogoClick}
            title="Ir para Dashboard"
            className="dark:invert"
          />
          
          {showEnvironmentBadge && envConfig && (
            <Badge
              variant="outline"
              className={`${envConfig.badgeClassName} px-xs py-[2px] text-[10px] uppercase tracking-[0.3px]`}
            >
              {safeText(envConfig.label)}
            </Badge>
          )}
        </div>
        {headerMenuItems.length > 0 && (
          <div style={{ display: 'flex', gap: '1rem', marginLeft: '7rem' }}>
            {headerMenuItems.map((item) => {
              const itemNomeMenu = safeText(item.nomeMenu)
              const itemCodigoIcone = safeString(item.codigoIcone)
              const iconVariant = itemCodigoIcone.includes('_outlined') ? 'outlined' : 'default'

              return (
                <Button
                  key={item.id}
                  onClick={() => handleMenuItemClick(item)}
                  disabled={item.emBreve}
                  title={itemNomeMenu}
                  variant="outline"
                  size="sm"
                  className="gap-2 rounded-pillToken"
                >
                  <Icon codigoIcone={itemCodigoIcone} variant={iconVariant} size={16} />
                  <span>{itemNomeMenu}</span>
                </Button>
              )
            })}
          </div>
        )}
      </BrandArea>

	      <ActionsArea>
	        <div style={{ position: 'relative' }}>
	          <NotificacoesPopover
	            open={isNotificacoesPopoverOpen}
	            onOpenChange={setIsNotificacoesPopoverOpen}
	            onMarcarComoLidas={atualizarContador}
	          >
	            <ActionButton 
	              ref={notificacoesButtonRef}
	              aria-label="Notificações" 
	              title="Notificações"
	              style={{ position: 'relative' }}
	            >
	              <Icon name="notifications" size={20} />
	              {contador > 0 && (
	                <span
	                  style={{
	                    position: 'absolute',
	                    top: '-6px',
	                    right: '-6px',
	                    minWidth: contador > 99 ? '24px' : '18px',
	                    height: '18px',
	                    padding: contador > 99 ? '0 5px' : contador > 9 ? '0 4px' : '0',
	                    borderRadius: '9px',
	                    backgroundColor: '#ef4444',
	                    color: '#ffffff',
	                    fontSize: '10px',
	                    fontWeight: 700,
	                    display: 'flex',
	                    alignItems: 'center',
	                    justifyContent: 'center',
	                    lineHeight: 1,
	                    border: '2px solid hsl(var(--background))',
	                    boxSizing: 'border-box',
	                    fontFamily: 'system-ui, -apple-system, sans-serif',
	                    boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
	                    zIndex: 10,
	                  }}
	                >
	                  {contador > 99 ? '99+' : contador}
	                </span>
	              )}
	            </ActionButton>
	          </NotificacoesPopover>
	        </div>
        <div style={{ position: 'relative' }}>
          <UserAvatar onClick={toggleProfileDropdown} title="Menu do perfil">
            {userInitials}
          </UserAvatar>
          {isProfileDropdownOpen && (
            <>
              <ProfileDropdownOverlay onClick={closeProfileDropdown} />
              <ProfileDropdown>
                <ProfileInfo>
                  <ProfileName>{safeText(user?.nomeColaborador)}</ProfileName>
                  <ProfileEmail>{safeText(user?.email)}</ProfileEmail>
                </ProfileInfo>
                <ProfileMenu>
                  {profileMenuItems.map((item) => {
                    const itemNomeMenu = safeText(item.nomeMenu)
                    const itemCodigoIcone = safeString(item.codigoIcone)
                    const iconVariant = itemCodigoIcone.includes('_outlined') ? 'outlined' : 'default'

                    return (
                      <ProfileMenuItem
                        key={item.id}
                        onClick={() => handleMenuItemClick(item)}
                        disabled={item.emBreve}
                        title={itemNomeMenu}
                      >
                        <Icon codigoIcone={itemCodigoIcone} variant={iconVariant} size={20} />
                        <span>{itemNomeMenu}</span>
                      </ProfileMenuItem>
                    )
                  })}
	                  <ProfileMenuItem onClick={toggleTheme} title="Alternar tema claro/escuro">
	                    {theme === 'dark' ? <Icon name="light_mode" size={20} /> : <Icon name="dark_mode" size={20} />}
	                    <span>{theme === 'dark' ? 'Usar modo claro' : 'Usar modo escuro'}</span>
	                  </ProfileMenuItem>
	                  <LogoutMenuItem onClick={handleLogout} title="Sair">
	                    <Icon name="logout" size={20} />
	                    <span>Sair</span>
	                  </LogoutMenuItem>
                </ProfileMenu>
              </ProfileDropdown>
            </>
          )}
        </div>
      </ActionsArea>
    </HeaderContainer>
  )
}
