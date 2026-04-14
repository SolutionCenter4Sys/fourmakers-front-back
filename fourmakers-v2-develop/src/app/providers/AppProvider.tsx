import type { ReactNode } from 'react'
import { Provider } from 'react-redux'
import { BrowserRouter } from 'react-router-dom'
import { ThemeProvider } from 'styled-components'

import { store } from '@app/store'
import { AppRoutes } from '@app/routes/AppRoutes'
import { PageTracker } from '@app/components/PageTracker'
import { GlobalStyles } from '@styles/GlobalStyles'
import { theme } from '@styles/theme'
import { TooltipProvider } from '@/components/ui/tooltip'

interface AppProviderProps {
  children?: ReactNode
}

export const AppProvider = ({ children }: AppProviderProps) => {
  console.log('[AppProvider] Renderizando...')
  
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <BrowserRouter>
          <TooltipProvider>
            <GlobalStyles />
            <PageTracker />
            {children ?? <AppRoutes />}
          </TooltipProvider>
        </BrowserRouter>
      </ThemeProvider>
    </Provider>
  )
}

export default AppProvider
