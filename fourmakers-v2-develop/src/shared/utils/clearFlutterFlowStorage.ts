/**
 * Dispara a limpeza do localStorage do domínio FlutterFlow (iframe)
 * criando um iframe invisível que carrega a página /clearstorage.
 * Usado no logout e antes do login (Entrar ou SSO) para zerar dados
 * persistidos no domínio do iframe (ex.: wf.dev.fourmakers.io).
 */
export function clearFlutterFlowStorageIframe(): void {
  const flutterflowBaseUrl = import.meta.env.VITE_FLUTTERFLOW_BASE_URL as string | undefined
  if (!flutterflowBaseUrl || typeof document === 'undefined') return

  const base = flutterflowBaseUrl.replace(/\/$/, '')
  const clearStorageUrl = `${base}/clearstorage`
  const iframe = document.createElement('iframe')
  iframe.setAttribute('src', clearStorageUrl)
  iframe.setAttribute(
    'style',
    'position:absolute;width:0;height:0;border:0;visibility:hidden;pointer-events:none',
  )
  iframe.setAttribute('aria-hidden', 'true')
  document.body?.appendChild(iframe)
}
