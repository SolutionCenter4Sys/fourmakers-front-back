/**
 * Retorna o valor do header X-Data-Source baseado no ambiente atual
 * @returns 'homolog' para development/homologation, 'live' para production
 */
export const getXanoDataSource = (): string => {
  const mode = import.meta.env.MODE;
  return mode === 'production' ? 'live' : 'homolog';
};

/**
 * Retorna os headers padrão para requisições XANO
 * @param additionalHeaders - Headers adicionais opcionais
 * @returns Objeto com headers configurados
 */
export const getXanoHeaders = (additionalHeaders?: Record<string, string>): HeadersInit => {
  const headers: HeadersInit = {
    'Content-Type': 'application/json; charset=utf-8',
    'X-Data-Source': getXanoDataSource(),
    ...additionalHeaders,
  };
  
  return headers;
};

/**
 * URL base da API XANO
 * Lê da variável de ambiente VITE_XANO_BASE_URL
 */
export const XANO_BASE_URL = import.meta.env.VITE_XANO_BASE_URL || 'https://xare-axod-hky2.b2.xano.io/api:MLkFdmWD';

