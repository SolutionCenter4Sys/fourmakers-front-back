/**
 * Utilitário para processamento de URLs
 */

/**
 * Processa URL substituindo placeholder $1 pelo token codificado em Base64
 * Segue o padrão usado em CertificationSection.tsx e RemessaCNAB.tsx
 * 
 * @param url - URL que pode conter placeholder $1
 * @param token - Token de autenticação
 * @returns URL processada ou null se url ou token forem inválidos
 */
export function processarUrlComToken(
  url: string | null | undefined,
  token: string | null | undefined
): string | null {
  if (!url || !token) return null
  
  // Verificar se URL contém placeholder $1
  if (!url.includes('$1')) return url
  
  // Codificar token em Base64
  const tokenEncoded = btoa(token)
  
  // Substituir todas as ocorrências de $1
  return url.replace(/\$1/g, tokenEncoded)
}

/**
 * Baixa um arquivo a partir da URL (fetch + blob + link download).
 * Útil quando a URL contém token ($1) e se quer forçar download em vez de abrir no navegador.
 * @param url - URL do arquivo (já com token processado se necessário)
 * @param nomeArquivo - Nome sugerido para o download; se omitido, extrai do path da URL
 */
export async function baixarArquivoPorUrl(
  url: string,
  nomeArquivo?: string
): Promise<void> {
  const response = await fetch(url)
  if (!response.ok) throw new Error(`Falha ao baixar: ${response.status}`)
  const blob = await response.blob()
  const nome =
    nomeArquivo ||
    decodeURIComponent(url.split('/').pop()?.split('?')[0] || 'arquivo')
  const objectUrl = URL.createObjectURL(blob)
  try {
    const a = document.createElement('a')
    a.href = objectUrl
    a.download = nome
    a.click()
  } finally {
    URL.revokeObjectURL(objectUrl)
  }
}
