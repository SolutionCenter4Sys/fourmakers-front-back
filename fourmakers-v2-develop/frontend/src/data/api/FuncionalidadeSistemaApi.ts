import { httpClient } from './httpClient';

export interface FuncionalidadeSistemaItem {
  id: number;
  descricao: string;
  ativo: boolean;
  dataCriacao: string;
  dataAlteracao: string;
}

export interface ListarFuncionalidadesSistemaResponse {
  retorno?: FuncionalidadeSistemaItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * Lista funcionalidades do sistema (somente leitura; alterações são feitas no backend/banco).
 */
export function listarFuncionalidadesSistema(
  token: string
): Promise<ListarFuncionalidadesSistemaResponse> {
  return httpClient.get<ListarFuncionalidadesSistemaResponse>(
    '/api/Usuario/Permissao/FuncionalidadeSistema/ListarFuncionalidadesSistema',
    { token }
  );
}
