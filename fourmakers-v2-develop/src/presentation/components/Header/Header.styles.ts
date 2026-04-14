import styled from 'styled-components'

export const HeaderContainer = styled.header`
  width: 100%;
  height: ${({ theme }) => theme.layout.headerHeight};
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: ${({ theme }) => theme.colors.surface};
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  padding: 0 2rem;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 20;
  flex-shrink: 0;
  box-sizing: border-box;
`

export const BrandArea = styled.div`
  display: flex;
  align-items: center;
  gap: 1rem;
  font-weight: 600;
  font-size: 1.125rem;
  color: ${({ theme }) => theme.colors.textHighlight};

  /* Oculta atalhos no mobile */
  > div:last-child {
    @media (max-width: 768px) {
      display: none !important;
    }
  }
`

export const HamburgerButton = styled.button`
  display: none;
  border: none;
  background: transparent;
  padding: 0.5rem;
  cursor: pointer;
  color: ${({ theme }) => theme.colors.textPrimary};
  transition: background 0.2s ease;
  border-radius: 0.5rem;

  &:hover {
    background: ${({ theme }) => theme.colors.backgroundHover};
  }

  @media (max-width: 768px) {
    display: flex;
    align-items: center;
    justify-content: center;
  }

  svg {
    width: 24px;
    height: 24px;
  }
`

export const MenuButton = styled.button`
  border: none;
  background: transparent;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0.5rem;
  border-radius: 0.75rem;
  color: ${({ theme }) => theme.colors.textPrimary};
  transition: background 0.2s ease;

  &:hover {
    background: ${({ theme }) => theme.colors.backgroundHover};
  }
`

export const ActionsArea = styled.div`
  display: flex;
  align-items: center;
  gap: 1.5rem;
`

export const ActionButton = styled.button.attrs({
  type: 'button',
})`
  border: none;
  background: transparent;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: 1.25rem;
  cursor: pointer;
  position: relative;

  &:hover {
    color: ${({ theme }) => theme.colors.textHighlight};
  }
`

export const ShortcutButton = styled.button`
  border: none;
  background: rgba(154, 27, 255, 0.12);
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  border-radius: 1rem;
  color: #9A1BFF;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s ease;
  font-weight: 500;

  &:hover:not(:disabled) {
    background: #9A1BFF;
    color: #FFFFFF;
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .material-symbols-outlined {
    font-size: 16px;
    line-height: 1;
  }
`

export const UserAvatar = styled.div`
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: ${({ theme }) => theme.colors.accentGradient};
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 600;
  color: #fff;
  cursor: pointer;
  transition: transform 0.2s ease;

  &:hover {
    transform: scale(1.05);
  }
`

export const ProfileDropdown = styled.div`
  position: absolute;
  top: calc(100% + 0.5rem);
  right: 2rem;
  width: 280px;
  background: ${({ theme }) => theme.colors.surface};
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: 0.75rem;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  z-index: 100;
  overflow: hidden;
`

export const ProfileInfo = styled.div`
  padding: 1.5rem;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
`

export const ProfileName = styled.div`
  font-size: 1rem;
  font-weight: 600;
  color: ${({ theme }) => theme.colors.textPrimary};
`

export const ProfileEmail = styled.div`
  font-size: 0.875rem;
  color: ${({ theme }) => theme.colors.textSecondary};
  word-break: break-word;
`

export const ProfileMenu = styled.div`
  padding: 0.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
`

export const ProfileMenuItem = styled.button`
  all: unset;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 0.5rem;
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease;

  &:hover {
    background: ${({ theme }) => theme.colors.backgroundHover};
    color: ${({ theme }) => theme.colors.textHighlight};
  }

  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .material-symbols-outlined {
    font-size: 20px;
    line-height: 1;
  }
`

export const ProfileDropdownOverlay = styled.div`
  position: fixed;
  inset: 0;
  z-index: 99;
`

export const LogoutMenuItem = styled.button`
  all: unset;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 0.5rem;
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease;
  border-top: 1px solid ${({ theme }) => theme.colors.border};
  margin-top: 0.25rem;

  &:hover {
    background: ${({ theme }) => theme.colors.backgroundHover};
    color: ${({ theme }) => theme.colors.textHighlight};
  }

  svg {
    flex-shrink: 0;
    width: 20px;
    height: 20px;
  }
`
