import type { FuncionalidadeSistemaItem } from '@domain/entities/FuncionalidadeSistema';

export interface GrupoAcessoComFuncionalidadesItem {
  id: number;
  descricao: string;
  ativo: boolean;
  orgId: number;
  dataCriacao: string;
  dataAlteracao: string;
  funcionalidadeSistema: FuncionalidadeSistemaItem[];
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

export interface AdicionarFuncionalidadeGrupoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface RemoverFuncionalidadeGrupoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

export interface AdicionarUsuarioGrupoAcessoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface RemoverUsuarioGrupoAcessoResponse {
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

/** Resumo de grupo de acesso (ex.: dentro de PessoaComGruposAcessoItem). */
export interface GrupoAcessoResumoItem {
  id: number;
  descricao: string;
  ativo: boolean;
  orgId: number;
  dataCriacao: string;
  dataAlteracao: string;
  acessoTodosClientes: boolean;
}

/** Item da listagem de pessoas por grupo de acesso (colaborador + grupos que integra). */
export interface PessoaComGruposAcessoItem {
  codigoInternoColaborador: string;
  nome: string;
  gruposAcesso: GrupoAcessoResumoItem[];
  /** ID do usuário; quando retornado pela API, usado na coluna ID e na remoção do grupo. Ausente = exibe N/A e remoção desabilitada. */
  usuarioId?: number;
  /** E-mail do colaborador; quando retornado pela API, exibido na lista. */
  email?: string | null;
  /** Data de inserção no grupo; quando retornada pela API, exibida na lista. */
  dataInsercao?: string | null;
}

export interface ListarPessoasPorGrupoAcessoResponse {
  retorno?: PessoaComGruposAcessoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}
