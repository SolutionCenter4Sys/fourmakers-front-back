import type { ModeloTrabalho } from '@domain/entities/GestaoVagasCandidatos';

/** Item de gestor (para seleção em modais). */
export interface GestorItem {
  id: string;
  descricao: string;
  email?: string;
  codGestorExterno?: string;
}

export interface Permanencia {
  id: string;
  descricao: string;
}

export interface Localidade {
  id: string;
  descricao: string;
}

export interface TipoEmprego {
  id: string;
  descricao: string;
}

export interface NivelExperiencia {
  id: string;
  descricao: string;
}

export interface PerfilAtuacaoRepository {
  listarModelosTrabalho(token: string): Promise<ModeloTrabalho[]>;
  listarGestores(
    token: string,
    clientCode: string | null,
    busca: string,
    cursor?: number,
    limite?: number,
  ): Promise<GestorItem[]>;
  listarPermanencias(token: string): Promise<Permanencia[]>;
  listarLocalidades(token: string): Promise<Localidade[]>;
  listarTiposEmprego(token: string): Promise<TipoEmprego[]>;
  listarNiveisExperiencia(token: string): Promise<NivelExperiencia[]>;
}
