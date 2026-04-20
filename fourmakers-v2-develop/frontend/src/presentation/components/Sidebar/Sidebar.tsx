import { useState, useMemo, useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'

import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { logout } from '@app/store/slices/authSlice'
import type { MenuResource } from '@domain/entities/MenuResource'

import { flutterflowRouteMap } from '@shared/utils/flutterflowRouteMap'
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip'
import { Icon } from '@/components/ui/icon'

import {
  Nav,
  NavItem,
  Overlay,
  SidebarContainer,
  SubMenuContainer,
  NavItemContent,
  NavItemIcon,
  CollapseButton,
  LogoutArea,
  LogoutButton,
} from './Sidebar.styles'

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

/** Para orgId 9 (Royal), exibe "Gestão Unidades" no lugar de "Gestão Clientes". */
const menuLabelPorOrg = (nomeMenu: string, orgId: number | undefined): string => {
  if (orgId === 9 && nomeMenu === 'Gestão Clientes') return 'Gestão Unidades'
  return nomeMenu
}

interface SidebarProps {
  collapsed: boolean
  onClose: () => void
  onToggleCollapse?: () => void
}

interface MenuItemProps {
  item: MenuResource
  collapsed: boolean
  onNavigate: (item: MenuResource) => void
  isActive: boolean
  level?: number
  checkActive?: (item: MenuResource) => boolean
  onToggleCollapse?: () => void
  orgId?: number
}

const MenuItem = ({ item, collapsed, onNavigate, isActive, level = 0, checkActive, onToggleCollapse, orgId }: MenuItemProps) => {
  const hasSubItems = item.subMenus && item.subMenus.length > 0
  const itemNomeMenu = safeText(item.nomeMenu)
  const itemLabel = menuLabelPorOrg(itemNomeMenu, orgId)
  const itemCodigoIcone = safeString(item.codigoIcone)
  
  // Verifica se algum subitem está ativo para auto-expandir
  const hasActiveChild = hasSubItems && checkActive
    ? item.subMenus.some((subItem) => checkActive(subItem))
    : false
  
  const [isExpanded, setIsExpanded] = useState(hasActiveChild)
  
  // Atualiza expansão quando um filho fica ativo
  useEffect(() => {
    if (hasActiveChild) {
      setIsExpanded(true)
    }
  }, [hasActiveChild])

  // Determina se é um item pai (group_sidebar) ou filho (item_sidebar)
  const isParent = item.tipoMenu === 'group_sidebar'
  const isChild = item.tipoMenu === 'item_sidebar' && item.codigoRecursoMenuPai !== null

  // Se não for visível, não renderiza
  if (!item.visivel) return null

  // Se for um group_sidebar sem subitens, não renderiza (só serve como título)
  if (isParent && !hasSubItems) return null

  const iconVariant = itemCodigoIcone.includes('_outlined') ? 'outlined' : 'default'

  const handleClick = (e: React.MouseEvent) => {
    e.preventDefault()

    if (item.emBreve) {
      return
    }

    // Se está colapsado e tem subitens, expande a sidebar
    if (collapsed && hasSubItems && onToggleCollapse) {
      onToggleCollapse()
      // Depois de expandir, define o item como expandido
      setTimeout(() => setIsExpanded(true), 100)
      return
    }

    // Se está colapsado e não tem subitens, navega
    if (collapsed) {
      if (!hasSubItems && (isChild || !isParent)) {
        if (item.linkExternoNovaPagina) {
          window.open(item.linkExternoNovaPagina, '_blank')
        } else {
          onNavigate(item)
        }
      }
      return
    }

    // Se é um menu pai com subitens, apenas expande/colapsa
    if (isParent && hasSubItems) {
      setIsExpanded((prev) => !prev)
      return
    }

    // Se é um item filho ou item sem subitens, navega
    if (isChild || (!isParent && !hasSubItems)) {
      if (item.linkExternoNovaPagina) {
        window.open(item.linkExternoNovaPagina, '_blank')
      } else {
        onNavigate(item)
      }
    }
  }

  // Renderiza apenas ícone quando colapsado (apenas itens raiz, level 0)
  if (collapsed && level === 0) {
    // Quando colapsado, mostra apenas ícone para todos os itens raiz
    return (
      <Tooltip delayDuration={300}>
        <TooltipTrigger asChild>
          <NavItem
            $active={isActive}
            onClick={handleClick}
            disabled={item.emBreve}
            $collapsed={true}
          >
            <NavItemIcon>
              <Icon codigoIcone={itemCodigoIcone} variant={iconVariant} size={20} />
            </NavItemIcon>
          </NavItem>
        </TooltipTrigger>
        <TooltipContent side="right" className="font-medium">
          {itemLabel}
        </TooltipContent>
      </Tooltip>
    )
  }

  return (
    <>
      <NavItem
        $active={isActive}
        onClick={handleClick}
        disabled={item.emBreve}
        $collapsed={false}
        $level={level}
      >
        <NavItemIcon>
          <Icon codigoIcone={itemCodigoIcone} variant={iconVariant} size={20} />
        </NavItemIcon>
        {!collapsed && (
          <NavItemContent>
            <span>{itemLabel}</span>
            {isParent && hasSubItems && (
              <span style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center' }}>
                {isExpanded ? <Icon name="expand_more" size={16} /> : <Icon name="chevron_right" size={16} />}
              </span>
            )}
            {item.emBreve && (
              <span
                style={{
                  marginLeft: 'auto',
                  fontSize: '0.75rem',
                  padding: '0 0.5rem',
                  color: 'inherit',
                  opacity: 0.7,
                }}
              >
                Em Breve
              </span>
            )}
          </NavItemContent>
        )}
      </NavItem>
      {!collapsed && isParent && hasSubItems && isExpanded && (
        <SubMenuContainer>
          {item.subMenus
            .filter((subItem) => subItem.visivel)
            .sort((a, b) => a.ordenacao - b.ordenacao)
            .map((subItem) => (
              <MenuItem
                key={subItem.id}
                item={subItem}
                collapsed={collapsed}
                onNavigate={onNavigate}
                isActive={checkActive ? checkActive(subItem) : false}
                level={level + 1}
                checkActive={checkActive}
                onToggleCollapse={onToggleCollapse}
                orgId={orgId}
              />
            ))}
        </SubMenuContainer>
      )}
    </>
  )
}


export const Sidebar = ({ collapsed, onClose, onToggleCollapse }: SidebarProps) => {
  const location = useLocation()
  const navigate = useNavigate()
  const dispatch = useAppDispatch()
  const { menuItems } = useAppSelector((state) => state.menu)
  const orgId = useAppSelector((state) => state.auth.user?.colaboradorOrg?.orgId)

  // Filtra apenas itens do sidebar e ordena; preserva a estrutura aninhada (subMenus) retornada pela API
  const sidebarMenuItems = useMemo(() => {
    return menuItems
      .filter((item) => item.tipoMenu === 'group_sidebar' || item.tipoMenu === 'item_sidebar')
      .filter((item) => item.visivel)
      .sort((a, b) => a.ordenacao - b.ordenacao)
  }, [menuItems])

  const handleLogout = () => {
    void dispatch(logout())
  }

  // Função para navegar
  const handleNavigate = (item: MenuResource) => {
    if (item.emBreve) return

    if (item.linkExternoNovaPagina) {
      window.open(item.linkExternoNovaPagina, '_blank')
      return
    }

    if (item.rotaReact) {
      navigate(item.rotaReact)
      onClose()
      return
    }

    // Usa codigoRecurso ou codigoRecursoMenu para buscar no flutterflowRouteMap (tenta exato e em minúsculas)
    const code = item.codigoRecurso || item.codigoRecursoMenu
    const routeInfo = code
      ? (flutterflowRouteMap[code] ?? flutterflowRouteMap[code.toLowerCase()])
      : undefined
    if (routeInfo) {
      if (routeInfo.isFlutterflow) {
        navigate(`/page/${routeInfo.path}`)
      } else {
        // Para rotas internas, garante que tenha barra inicial
        const internalPath = routeInfo.path.startsWith('/') ? routeInfo.path : `/${routeInfo.path}`
        navigate(internalPath)
      }
      onClose()
    } else {
      console.warn(`[React Sidebar] Mapeamento de rota para ${code ?? item.codigoRecurso} não encontrado.`)
    }
  }

  // Função para verificar se item está ativo (com acesso ao location)
  const isItemActiveSelf = (item: MenuResource) => {
    if (item.rotaReact) {
      // Para rotas React, considera ativo se for a rota exata ou começar com ela (rotas filhas)
      return location.pathname === item.rotaReact || location.pathname.startsWith(`${item.rotaReact}/`)
    }

    const code = item.codigoRecurso || item.codigoRecursoMenu
    const routeInfo = code
      ? (flutterflowRouteMap[code] ?? flutterflowRouteMap[code.toLowerCase()])
      : undefined
    if (routeInfo) {
      if (routeInfo.isFlutterflow) {
        return location.pathname.startsWith(`/page/${routeInfo.path}`)
      }
      // Para rotas internas (isFlutterflow = false), adiciona a barra inicial e considera rotas filhas
      const internalPath = routeInfo.path.startsWith('/') ? routeInfo.path : `/${routeInfo.path}`
      return location.pathname === internalPath || location.pathname.startsWith(`${internalPath}/`)
    }

    return false
  }

  const isItemActiveDeep = (item: MenuResource): boolean => {
    if (isItemActiveSelf(item)) return true
    if (!item.subMenus || item.subMenus.length === 0) return false
    return item.subMenus.some((subItem) => isItemActiveDeep(subItem))
  }


  // Filtra apenas os itens raiz (sem pai ou que são group_sidebar)
  const rootItems = useMemo(() => {
    return sidebarMenuItems.filter((item) => {
      // Itens group_sidebar são sempre raiz
      if (item.tipoMenu === 'group_sidebar') return true
      // Itens item_sidebar sem pai também são raiz
      if (item.tipoMenu === 'item_sidebar' && !item.codigoRecursoMenuPai) return true
      return false
    })
  }, [sidebarMenuItems])

  return (
    <TooltipProvider>
      <SidebarContainer $collapsed={collapsed}>
        <Nav $collapsed={collapsed}>
          {rootItems.map((item) => (
            <MenuItem
              key={item.id}
              item={item}
              collapsed={collapsed}
              onNavigate={handleNavigate}
              isActive={collapsed ? isItemActiveDeep(item) : isItemActiveSelf(item)}
              level={0}
              checkActive={isItemActiveSelf}
              onToggleCollapse={onToggleCollapse}
              orgId={orgId}
            />
          ))}
        </Nav>
        <LogoutArea $collapsed={collapsed}>
          {collapsed ? (
            <Tooltip delayDuration={300}>
              <TooltipTrigger asChild>
                <LogoutButton 
                  $collapsed={collapsed} 
                  onClick={handleLogout}
                >
                  <NavItemIcon>
                    <Icon name="logout" size={20} />
                  </NavItemIcon>
                </LogoutButton>
              </TooltipTrigger>
              <TooltipContent side="right" className="font-medium">
                Sair
              </TooltipContent>
            </Tooltip>
          ) : (
            <LogoutButton 
              $collapsed={collapsed} 
              onClick={handleLogout}
            >
              <NavItemIcon>
                <Icon name="logout" size={20} />
              </NavItemIcon>
              <span>Sair</span>
            </LogoutButton>
          )}
        </LogoutArea>
      </SidebarContainer>
      {onToggleCollapse && (
        <CollapseButton
          $collapsed={collapsed}
          onClick={onToggleCollapse}
          aria-label={collapsed ? 'Expandir menu' : 'Colapsar menu'}
        >
          {collapsed ? <Icon name="chevron_right" size={14} /> : <Icon name="chevron_left" size={14} />}
        </CollapseButton>
      )}
      <Overlay $collapsed={collapsed} onClick={onClose} />
    </TooltipProvider>
  )
}
