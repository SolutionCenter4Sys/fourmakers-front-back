import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'

import AppProvider from '@app/providers/AppProvider'
import { ErrorBoundary } from '@app/components/ErrorBoundary'
import '@core/config/firebase' // Inicializa o Firebase
import './index.css'

/**
 * SOLUÇÃO GLOBAL: Desabilita Google Translate dinamicamente
 * 
 * Este script garante que o Google Translate seja desabilitado mesmo se
 * for carregado após o carregamento inicial da página. Ele:
 * 
 * 1. Adiciona atributos translate="no" e classe "notranslate" ao HTML e body
 * 2. Remove elementos do Google Translate (iframes, scripts, widgets) se já existirem
 * 3. Monitora mudanças no DOM para remover elementos do Google Translate que possam ser adicionados
 * 
 * Isso previne erros do React como:
 * - NotFoundError: Failed to execute 'insertBefore' on 'Node'
 * - NotFoundError: Failed to execute 'removeChild' on 'Node'
 * 
 * Esses erros ocorrem quando o Google Translate modifica o DOM e o React
 * tenta manipular nós que foram alterados ou removidos.
 */
if (typeof document !== 'undefined') {
  // Adiciona atributos ao HTML e Body para desabilitar tradução
  document.documentElement.setAttribute('translate', 'no')
  document.documentElement.classList.add('notranslate')
  document.body?.setAttribute('translate', 'no')
  document.body?.classList.add('notranslate')
  
  /**
   * Função que remove elementos do Google Translate do DOM
   * Remove iframes, scripts e widgets do Google Translate se já estiverem carregados
   */
  const removeGoogleTranslate = () => {
    // Remove o iframe do Google Translate se existir
    const googleTranslateFrame = document.querySelector('iframe[src*="translate.googleapis.com"]')
    if (googleTranslateFrame) {
      googleTranslateFrame.remove()
    }
    
    // Remove o script do Google Translate se existir
    const googleTranslateScript = document.querySelector('script[src*="translate.googleapis.com"]')
    if (googleTranslateScript) {
      googleTranslateScript.remove()
    }
    
    // Remove o elemento de seleção de idioma do Google Translate
    const googleTranslateWidget = document.querySelector('#google_translate_element')
    if (googleTranslateWidget) {
      googleTranslateWidget.remove()
    }
  }
  
  // Executa imediatamente para remover elementos já existentes
  removeGoogleTranslate()

  /**
   * MutationObserver: monitora o DOM para remover elementos do Google Translate
   * que forem injetados após o carregamento (ex.: usuário ativa o Translate).
   *
   * Alteração (performance): em ambiente de desenvolvimento (DEV) o observer
   * não é criado. Cada re-render do React gera muitas mutações no DOM; o
   * callback seria chamado centenas de vezes e deixava a aplicação lenta.
   * Em produção/homolog há menos mutações, então o observer roda normalmente.
   *
   * Em produção: o callback é agendado com debounce de 500 ms para evitar
   * executar removeGoogleTranslate em toda mutação; o observer é desconectado
   * após 10 segundos para limitar uso de recursos.
   */
  if (!import.meta.env.DEV) {
    let debounceTimer: ReturnType<typeof setTimeout> | null = null
    const scheduleRemove = () => {
      if (debounceTimer) return
      debounceTimer = setTimeout(() => {
        debounceTimer = null
        removeGoogleTranslate()
      }, 500)
    }

    const observer = new MutationObserver(scheduleRemove)
    observer.observe(document.body, {
      childList: true,
      subtree: true,
    })

    // Desconecta o observer após 10 segundos para evitar consumo excessivo de recursos
    setTimeout(() => {
      if (debounceTimer) clearTimeout(debounceTimer)
      observer.disconnect()
    }, 10000)
  }
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ErrorBoundary>
      <AppProvider />
    </ErrorBoundary>
  </StrictMode>,
)
