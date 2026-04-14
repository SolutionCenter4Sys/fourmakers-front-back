import styled, { css } from 'styled-components'

interface SidebarContainerProps {
  $collapsed: boolean
}

export const SidebarContainer = styled.aside<SidebarContainerProps>`
  position: fixed;
  top: 0;
  left: 0;
  width: ${({ $collapsed, theme }) => ($collapsed ? '72px' : theme.layout.sidebarWidth)};
  height: 100vh;
  background: ${({ theme }) => theme.colors.surface};
  border-right: 1px solid ${({ theme }) => theme.colors.border};
  display: flex;
  flex-direction: column;
  padding: ${({ $collapsed }) => ($collapsed ? '0 0.5rem 1rem' : '0 0.75rem 1rem')};
  overflow-y: auto;
  overflow-x: visible;
  transition: width 0.3s ease, padding 0.3s ease, transform 0.3s ease;
  box-sizing: border-box;
  flex-shrink: 0;
  z-index: 15;

  @media (max-width: 768px) {
    z-index: 35;
    width: ${({ theme }) => theme.layout.sidebarWidth};
    transform: ${({ $collapsed }) => ($collapsed ? 'translateX(-100%)' : 'translateX(0)')};
  }
`

export const Overlay = styled.div<SidebarContainerProps>`
  display: none;

  @media (max-width: 768px) {
    display: ${({ $collapsed }) => ($collapsed ? 'none' : 'block')};
    position: fixed;
    inset: 0;
    background: rgba(15, 16, 18, 0.45);
    z-index: 34;
    backdrop-filter: blur(2px);
  }
`

export const Logo = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  padding: 0 0.5rem 1.5rem 0.5rem;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  margin-bottom: 1.5rem;
  align-items: center;
  text-align: center;

  span:first-child {
    font-weight: 700;
    font-size: 1.35rem;
    color: ${({ theme }) => theme.colors.textPrimary};
  }

  span:last-child {
    font-size: 0.75rem;
    text-transform: uppercase;
    letter-spacing: 0.08em;
    color: ${({ theme }) => theme.colors.textSecondary};
  }
`

export const Nav = styled.nav<{ $collapsed?: boolean }>`
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  min-height: 0;
  padding-top: calc(${({ theme }) => theme.layout.headerHeight} + 1rem);
  padding-bottom: 0.5rem;

  /* Esconde a scrollbar mas mantém a funcionalidade */
  scrollbar-width: thin;
  scrollbar-color: ${({ theme, $collapsed }) =>
      $collapsed
        ? `color-mix(in srgb, ${theme.colors.border} 20%, transparent)`
        : `color-mix(in srgb, ${theme.colors.border} 35%, transparent)`} transparent;

  &::-webkit-scrollbar {
    width: 2px;
  }

  &::-webkit-scrollbar-track {
    background: transparent;
  }

  &::-webkit-scrollbar-thumb {
    background: ${({ theme }) => theme.colors.border};
    border-radius: 999px;
    opacity: ${({ $collapsed }) => ($collapsed ? 0.2 : 0.35)};
  }

  &::-webkit-scrollbar-thumb:hover {
    opacity: ${({ $collapsed }) => ($collapsed ? 0.35 : 0.6)};
  }
`

interface NavItemProps {
  $active?: boolean
}

interface NavItemExtendedProps extends NavItemProps {
  $collapsed?: boolean
  $level?: number
}

export const NavItem = styled.button<NavItemExtendedProps>`
  all: unset;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: ${({ $collapsed }) => ($collapsed ? '0.75rem' : '0.75rem 0.5rem')};
  padding-left: ${({ $level = 0, $collapsed }) => {
    if ($collapsed) return '0.75rem'
    if ($level === 0) return '0.75rem'
    return '1rem'
  }};
  border-radius: var(--radius-pill);
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease, box-shadow 0.2s ease;
  width: 100%;
  max-width: 100%;
  box-sizing: border-box;
  justify-content: ${({ $collapsed }) => ($collapsed ? 'center' : 'flex-start')};
  min-width: 0;

  &:hover {
    background: var(--color-btn-ghost-hover);
    color: ${({ theme }) => theme.colors.primary};
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  &:focus-visible {
    box-shadow: 0 0 0 2px var(--color-primary), 0 0 0 4px var(--color-secondary-background);
  }

  ${({ $active }) =>
    $active &&
    css`
      background: var(--color-btn-primary);
      color: var(--color-text-inverse);
      box-shadow: var(--elevation-soft);

      &:hover {
        background: var(--color-btn-primary-hover);
        color: var(--color-text-inverse);
      }
    `}
`

export const NavSectionTitle = styled.p`
  margin: 1rem 1rem 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  color: ${({ theme }) => theme.colors.textMuted};
  letter-spacing: 0.08em;
`

export const CollapseButton = styled.button<{ $collapsed: boolean }>`
  position: fixed !important;
  top: calc(${({ theme }) => theme.layout.headerHeight} - 12px);
  left: ${({ $collapsed, theme }) => 
    $collapsed 
      ? 'calc(72px - 12px)' 
      : `calc(${theme.layout.sidebarWidth} - 12px)`
  };
  display: flex !important;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  min-width: 24px;
  min-height: 24px;
  padding: 0;
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: 50%;
  cursor: pointer;
  background: ${({ theme }) => theme.colors.surface};
  color: ${({ theme }) => theme.colors.textSecondary};
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  z-index: 50 !important;
  pointer-events: auto;

  &:hover {
    background: ${({ theme }) => theme.colors.backgroundHover};
    color: ${({ theme }) => theme.colors.textHighlight};
    transform: scale(1.15);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  }

  &:active {
    transform: scale(0.9);
  }

  svg {
    flex-shrink: 0;
    width: 14px;
    height: 14px;
  }

  /* Quando há modal/dialogo aberto (RemoveScroll adiciona data-scroll-locked) escondemos o botão */
  body[data-scroll-locked] & {
    opacity: 0;
    pointer-events: none;
  }
  /* Esconde se houver qualquer dialog/alertdialog aberto no DOM */
  body:has([role="dialog"][data-state="open"]) &,
  body:has([role="alertdialog"][data-state="open"]) & {
    opacity: 0;
    pointer-events: none;
  }

  @media (max-width: 768px) {
    display: none !important;
  }
`

export const SubMenuContainer = styled.div`
  display: flex;
  flex-direction: column;
  margin-left: 0;
  padding-left: 0.5rem;
  margin-right: 0;
  border-left: 2px solid ${({ theme }) => theme.colors.border};
  gap: 0.25rem;
  margin-top: 0.25rem;
  margin-bottom: 0.5rem;
  width: 100%;
  max-width: 100%;
  box-sizing: border-box;
`

export const LogoutArea = styled.div<{ $collapsed: boolean }>`
  display: flex;
  flex-direction: column;
  padding: ${({ $collapsed }) => ($collapsed ? '0.5rem' : '0.75rem')};
  border-top: 1px solid ${({ theme }) => theme.colors.border};
  background: ${({ theme }) => theme.colors.surface};
  flex-shrink: 0;
`

export const LogoutButton = styled.button<{ $collapsed: boolean }>`
  all: unset;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: ${({ $collapsed }) => ($collapsed ? '0.75rem' : '0.75rem 0.5rem')};
  padding-left: 0.75rem;
  border-radius: var(--radius-pill);
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease, box-shadow 0.2s ease;
  width: 100%;
  max-width: 100%;
  box-sizing: border-box;
  justify-content: ${({ $collapsed }) => ($collapsed ? 'center' : 'flex-start')};
  min-width: 0;

  &:hover {
    background: var(--color-btn-ghost-hover);
    color: ${({ theme }) => theme.colors.primary};
  }

  &:focus-visible {
    box-shadow: 0 0 0 2px var(--color-primary), 0 0 0 4px var(--color-secondary-background);
  }

  svg {
    flex-shrink: 0;
  }
`

export const NavItemContent = styled.div`
  display: flex;
  align-items: center;
  flex: 1;
  gap: 0.5rem;
  min-width: 0;
  overflow: hidden;

  > span:first-child {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    flex: 1;
    min-width: 0;
  }
`

export const NavItemIcon = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  width: 24px;
  height: 24px;

  .material-symbols-outlined {
    font-size: 20px;
    line-height: 1;
  }
`
