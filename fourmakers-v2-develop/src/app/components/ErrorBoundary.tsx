import { Component, type ReactNode } from 'react'
import { logFatalError } from '@shared/utils/firebaseCrashlytics'
import { store } from '@app/store'

interface Props {
  children: ReactNode
}

interface State {
  hasError: boolean
  error?: Error
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props)
    this.state = { hasError: false }
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error }
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error('ErrorBoundary capturou um erro:', error, errorInfo)
    
    // Obter dados do usuário do Redux store
    const state = store.getState()
    const user = state.auth.user
    
    // Registrar erro fatal no Firebase com dados do usuário
    logFatalError(error, {
      componentStack: errorInfo.componentStack,
      errorBoundary: true,
    }, user)
  }

  render() {
    if (this.state.hasError) {
      return (
        <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
          <h1>Algo deu errado</h1>
          <p>{this.state.error?.message}</p>
          <button onClick={() => window.location.reload()}>Recarregar página</button>
        </div>
      )
    }

    return this.props.children
  }
}

