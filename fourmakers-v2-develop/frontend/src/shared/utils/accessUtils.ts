import type { FuncionalidadeSistema } from '@domain/entities/ShowmeUserProfile';
import { isDevelopment } from './envUtils';

/**
 * Verifica se o usuário tem acesso a uma funcionalidade específica do sistema.
 * 
 * Esta função verifica o acesso baseado na resposta da API ShowMe, especificamente
 * no array `funcionalidadeSistema` retornado no perfil do usuário.
 * 
 * Regras de acesso:
 * - Em ambiente de desenvolvimento: SEMPRE retorna true (permite acesso)
 * - Em outros ambientes: Verifica se a funcionalidade existe no array `funcionalidadeSistema`
 *   e se está ativa (`ativo === true`)
 * 
 * @param funcionalidadeSistema - Array de funcionalidades do sistema retornado pela API ShowMe
 * @param nomeFuncionalidade - Nome/código da funcionalidade a ser verificada (ex: 'MINHA_EQUIPE', 'MINHA_EQUIPE_ORQUESTRACAO')
 * @returns {boolean} true se tiver acesso, false caso contrário
 * 
 * @example
 * ```typescript
 * const { user } = useAppSelector((state) => state.auth)
 * const temAcesso = hasAccessToFeature(user?.funcionalidadeSistema, 'MINHA_EQUIPE')
 * 
 * if (temAcesso) {
 *   // Renderizar componente ou permitir ação
 * }
 * ```
 */
export const hasAccessToFeature = (
  funcionalidadeSistema: FuncionalidadeSistema[] | undefined,
  nomeFuncionalidade: string,
): boolean => {
  // Em desenvolvimento, sempre permitir acesso
  if (isDevelopment()) {
    return true;
  }

  // Se não há array de funcionalidades, não tem acesso
  if (!funcionalidadeSistema || !Array.isArray(funcionalidadeSistema)) {
    return false;
  }

  // Verificar se a funcionalidade existe e está ativa
  return funcionalidadeSistema.some(
    (func) => func.descricao === nomeFuncionalidade && func.ativo === true,
  );
};
