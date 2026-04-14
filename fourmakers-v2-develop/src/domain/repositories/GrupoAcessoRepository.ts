import type {
  ListarGruposAcessoFuncionalidadesSistemaResponse,
  ListarPessoasPorGrupoAcessoResponse,
  CriarGrupoAcessoPayload,
  CriarGrupoAcessoResponse,
  EditarGrupoAcessoPayload,
  EditarGrupoAcessoResponse,
  AdicionarFuncionalidadeGrupoResponse,
  RemoverFuncionalidadeGrupoResponse,
  AdicionarUsuarioGrupoAcessoResponse,
  RemoverUsuarioGrupoAcessoResponse,
} from '@domain/entities/GrupoAcesso';

export interface GrupoAcessoRepository {
  listarGruposComFuncionalidades(token: string): Promise<ListarGruposAcessoFuncionalidadesSistemaResponse>;
  listarPessoasPorGrupoAcesso(token: string): Promise<ListarPessoasPorGrupoAcessoResponse>;
  criarGrupo(token: string, payload: CriarGrupoAcessoPayload): Promise<CriarGrupoAcessoResponse>;
  editarGrupo(token: string, payload: EditarGrupoAcessoPayload): Promise<EditarGrupoAcessoResponse>;
  adicionarFuncionalidadeGrupo(
    token: string,
    grupoAcessoId: number,
    funcionalidadeSistemaId: number
  ): Promise<AdicionarFuncionalidadeGrupoResponse>;
  removerFuncionalidadeGrupo(
    token: string,
    grupoAcessoId: number,
    funcionalidadeSistemaId: number
  ): Promise<RemoverFuncionalidadeGrupoResponse>;
  adicionarUsuarioGrupo(token: string, usuarioId: number, grupoAcessoId: number): Promise<AdicionarUsuarioGrupoAcessoResponse>;
  removerUsuarioGrupo(token: string, usuarioId: number, grupoAcessoId: number): Promise<RemoverUsuarioGrupoAcessoResponse>;
}
