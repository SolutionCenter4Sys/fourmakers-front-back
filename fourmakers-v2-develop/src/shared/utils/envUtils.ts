import type { OrgLoginConfig } from '../constants/orgConfig';

/**
 * Retorna a URL de SSO correta com base no ambiente Vite atual.
 * @param orgConfig O objeto de configuração da organização.
 * @returns A URL de SSO para o ambiente atual ou null se não estiver configurada.
 */
export const getSsoUrlForCurrentEnv = (orgConfig: OrgLoginConfig): string | null => {
  const mode = import.meta.env.MODE;

  const byMode =
    mode === 'development'
      ? orgConfig.urlSSoDev
      : mode === 'homologation'
        ? orgConfig.urlSSoHomolog
        : mode === 'production'
          ? orgConfig.urlSSoProd
          : null;

  const url =
    byMode ??
    orgConfig.urlSSoProd ??
    orgConfig.urlSSoHomolog ??
    orgConfig.urlSSoDev ??
    null;

  console.log('[getSsoUrlForCurrentEnv]', {
    mode,
    orgId: orgConfig.orgId,
    orgSlug: orgConfig.slugRota,
    url,
  });

  return url;
};

/**
 * Verifica se o ambiente atual é de desenvolvimento.
 * @returns {boolean} true se o ambiente for 'development', false caso contrário
 */
export const isDevelopment = (): boolean => {
  return import.meta.env.MODE === 'development';
};

/**
 * Verifica se o ambiente atual é de homologação.
 * @returns {boolean} true se o ambiente for 'homologation', false caso contrário
 */
export const isHomologation = (): boolean => {
  return import.meta.env.MODE === 'homologation';
};

/**
 * Verifica se o ambiente atual é de produção.
 * @returns {boolean} true se o ambiente for 'production', false caso contrário
 */
export const isProduction = (): boolean => {
  return import.meta.env.MODE === 'production';
};
