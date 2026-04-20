import { httpClient } from './httpClient';
import type { FuncionalidadeSistemaItem } from './FuncionalidadeSistemaApi';
import type { PessoaComGruposAcessoItem } from '@domain/entities/GrupoAcesso';

export interface GrupoAcessoComFuncionalidadesItem {
  id: number;
  descricao: string;
  ativo: boolean;
  orgId: number;
  dataCriacao: string;
  dataAlteracao: string;
  funcionalidadeSistema: FuncionalidadeSistemaItem[];
  /** Lista de clientes vinculados ao grupo (quando retornada pela listagem). */
  clientes?: { codigoCliente: string }[];
}

export interface ListarGruposAcessoFuncionalidadesSistemaResponse {
  retorno?: GrupoAcessoComFuncionalidadesItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface CriarGrupoAcessoPayload {
  descricao: string;
  clientes: { codigoCliente: string }[];
}

export interface CriarGrupoAcessoResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * Lista grupos de acesso com suas funcionalidades do sistema (somente leitura).
 */
export function listarGruposAcessoFuncionalidadesSistema(
  token: string
): Promise<ListarGruposAcessoFuncionalidadesSistemaResponse> {
  return httpClient.get<ListarGruposAcessoFuncionalidadesSistemaResponse>(
    '/api/Usuario/Permissao/GrupoAcesso/ListarGruposAcessoFuncionalidadesSistema',
    { token }
  );
}

/**
 * Cria um novo grupo de acesso.
 */
export function criarGrupoAcesso(
  token: string,
  payload: CriarGrupoAcessoPayload
): Promise<CriarGrupoAcessoResponse> {
  return httpClient.post<CriarGrupoAcessoResponse>(
    '/api/Usuario/Permissao/GrupoAcesso/CriarGrupoAcesso',
    payload,
    { token }
  );
}

export interface EditarGrupoAcessoPayload {
  id: number;
  descricao: string;
  clientes: { codigoCliente: string }[];
}

export interface EditarGrupoAcessoResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * Edita um grupo de acesso existente.
 */
export function editarGrupoAcesso(
  token: string,
  payload: EditarGrupoAcessoPayload
): Promise<EditarGrupoAcessoResponse> {
  return httpClient.put<EditarGrupoAcessoResponse>(
    '/api/Usuario/Permissao/GrupoAcesso/EditarGrupoAcesso',
    payload,
    { token }
  );
}

export interface AdicionarFuncionalidadeGrupoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * Atribui uma funcionalidade do sistema a um grupo de acesso.
 */
export function adicionarGrupoAcessoFuncionalidadeSistema(
  token: string,
  grupoAcessoId: number,
  funcionalidadeSistemaId: number
): Promise<AdicionarFuncionalidadeGrupoResponse> {
  const url = `/api/Usuario/Permissao/GrupoAcesso/AdicionarGrupoAcessoFuncionalidadeSistema?grupoAcessoId=${grupoAcessoId}&funcionalidadeSistemaId=${funcionalidadeSistemaId}`;
  return httpClient.post<AdicionarFuncionalidadeGrupoResponse>(url, undefined, { token });
}

export interface AdicionarUsuarioGrupoAcessoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * Adiciona um usuário (colaborador) a um grupo de acesso.
 */
export function adicionarUsuarioGrupoAcesso(
  token: string,
  usuarioId: number,
  grupoAcessoId: number
): Promise<AdicionarUsuarioGrupoAcessoResponse> {
  const url = `/api/Usuario/Permissao/UsuarioGrupoAcesso/AdicionarUsuarioGrupoAcesso?usuarioId=${usuarioId}&grupoAcessoId=${grupoAcessoId}`;
  return httpClient.post<AdicionarUsuarioGrupoAcessoResponse>(url, undefined, { token });
}

export interface RemoverGrupoAcessoFuncionalidadeResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

/**
 * Remove uma funcionalidade do sistema do grupo de acesso.
 */
export function removerGrupoAcessoFuncionalidadeSistema(
  token: string,
  grupoAcessoId: number,
  funcionalidadeSistemaId: number
): Promise<RemoverGrupoAcessoFuncionalidadeResponse> {
  const url = `/api/Usuario/Permissao/GrupoAcesso/RemoverGrupoAcessoFuncionalidadeSistema?grupoAcessoId=${grupoAcessoId}&funcionalidadeSistemaId=${funcionalidadeSistemaId}`;
  return httpClient.post<RemoverGrupoAcessoFuncionalidadeResponse>(url, undefined, { token });
}

export interface RemoverUsuarioGrupoAcessoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

/**
 * Remove um usuário do grupo de acesso.
 */
export function removerUsuarioGrupoAcesso(
  token: string,
  usuarioId: number,
  grupoAcessoId: number
): Promise<RemoverUsuarioGrupoAcessoResponse> {
  const url = `/api/Usuario/Permissao/UsuarioGrupoAcesso/RemoverUsuarioGrupoAcesso?usuarioId=${usuarioId}&grupoAcessoId=${grupoAcessoId}`;
  return httpClient.post<RemoverUsuarioGrupoAcessoResponse>(url, undefined, { token });
}

export interface ListarPessoasPorGrupoAcessoResponse {
  retorno?: PessoaComGruposAcessoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/**
 * Lista todas as pessoas (colaboradores) com os grupos de acesso que integram.
 */
export function listarPessoasPorGrupoAcesso(
  token: string
): Promise<ListarPessoasPorGrupoAcessoResponse> {
  return httpClient.get<ListarPessoasPorGrupoAcessoResponse>(
    '/api/Usuario/Permissao/GrupoAcesso/ListarPessoasPorGrupoAcesso',
    { token }
  );
}
