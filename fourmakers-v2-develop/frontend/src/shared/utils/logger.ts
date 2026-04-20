/**
 * Logger utilitário com suporte condicional para desenvolvimento
 * Remove logs de debug em produção, mantendo apenas logs importantes
 */

const isDev = import.meta.env.DEV

export const logger = {
  /**
   * Log apenas em desenvolvimento
   * Use para logs de debug que não devem aparecer em produção
   */
  debug: (...args: unknown[]) => {
    if (isDev) {
      console.log(...args)
    }
  },

  /**
   * Log informativo (sempre aparece)
   * Use para informações importantes que devem aparecer em produção
   */
  info: (...args: unknown[]) => {
    console.info(...args)
  },

  /**
   * Log de aviso (sempre aparece)
   * Use para avisos que devem aparecer em produção
   */
  warn: (...args: unknown[]) => {
    console.warn(...args)
  },

  /**
   * Log de erro (sempre aparece)
   * Use para erros que devem aparecer em produção
   */
  error: (...args: unknown[]) => {
    console.error(...args)
  },
}
