import styled from 'styled-components'

export const LayoutContainer = styled.div<{ $sidebarOffset?: string }>`
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: ${({ theme }) => theme.colors.background};
  --sidebar-offset: ${({ $sidebarOffset }) => $sidebarOffset ?? '260px'};

  @media (max-width: 768px) {
    --sidebar-offset: 0px;
  }
`

export const MainContent = styled.div<{ $sidebarCollapsed?: boolean }>`
  display: flex;
  flex: 1;
  min-height: 0;
  overflow: hidden;
  margin-top: ${({ theme }) => theme.layout.headerHeight};
  margin-left: ${({ $sidebarCollapsed, theme }) => 
    $sidebarCollapsed ? '72px' : theme.layout.sidebarWidth};
  transition: margin-left 0.3s ease;

  @media (max-width: 768px) {
    flex-direction: column;
    margin-left: 0;
  }
`

export const ContentWrapper = styled.div`
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
  background: ${({ theme }) => theme.colors.background};
  overflow: hidden;
`

export const ContentArea = styled.main`
  flex: 1;
  padding: 0.5rem 3rem 2rem;
  background: ${({ theme }) => theme.colors.background};
  overflow-y: auto;
  overflow-x: hidden;
  min-height: 0;

  @media (max-width: 1080px) {
    padding: 0.5rem 1.5rem 3rem;
  }
`
