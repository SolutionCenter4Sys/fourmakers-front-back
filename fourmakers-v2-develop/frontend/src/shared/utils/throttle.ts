/**
 * Função throttle customizada para limitar execuções de funções
 * Útil para eventos que disparam frequentemente (ex: mousemove, scroll)
 * 
 * @param func - Função a ser throttled
 * @param delay - Delay em milissegundos entre execuções
 * @returns Função throttled
 */
export function throttle<T extends (...args: any[]) => any>(
  func: T,
  delay: number
): (...args: Parameters<T>) => void {
  let lastCall = 0
  let timeoutId: ReturnType<typeof setTimeout> | null = null

  return function (this: any, ...args: Parameters<T>) {
    const now = Date.now()
    const timeSinceLastCall = now - lastCall

    if (timeSinceLastCall >= delay) {
      // Executa imediatamente se passou o delay
      lastCall = now
      func.apply(this, args)
    } else {
      // Agenda execução para quando o delay passar
      if (timeoutId) {
        clearTimeout(timeoutId)
      }
      timeoutId = setTimeout(() => {
        lastCall = Date.now()
        func.apply(this, args)
        timeoutId = null
      }, delay - timeSinceLastCall)
    }
  }
}
