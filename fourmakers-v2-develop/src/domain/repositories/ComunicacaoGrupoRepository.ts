import type {
  ComunicacaoGrupo,
  ColaboradorDisponivel,
  ComunicacaoGrupoDetalhe,
  CriarGrupoPayload,
  CriarGrupoResponse,
  PermissoesUsuarioLogado,
} from '@domain/entities/comunicacao';

export interface ComunicacaoGrupoRepository {
  listarGrupos(token: string): Promise<ComunicacaoGrupo[]>;
  obterPermissoesUsuarioLogado(token: string): Promise<PermissoesUsuarioLogado | null>;
  criarGrupo(
    token: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse>;
  obterGrupoPorId(
    token: string,
    grupoId: string,
  ): Promise<ComunicacaoGrupoDetalhe | null>;
  atualizarGrupo(
    token: string,
    grupoId: string,
    payload: CriarGrupoPayload,
  ): Promise<CriarGrupoResponse>;
  deletarGrupo(token: string, grupoId: string): Promise<CriarGrupoResponse>;
  listarColaboradoresDisponiveis(
    token: string,
    params?: { grupoId?: string; filtro?: string },
  ): Promise<ColaboradorDisponivel[]>;
}
