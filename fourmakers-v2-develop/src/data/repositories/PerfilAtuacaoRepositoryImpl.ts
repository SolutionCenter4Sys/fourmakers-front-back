import { injectable } from 'tsyringe';
import { fetchModelosTrabalho, fetchGestores, fetchPermanencias, fetchLocalidades, fetchTiposEmprego, fetchNiveisExperiencia } from '@data/api/PerfilAtuacaoApi';
import type { GestorItem, PerfilAtuacaoRepository, Permanencia, Localidade, TipoEmprego, NivelExperiencia } from '@domain/repositories/PerfilAtuacaoRepository';

@injectable()
export class PerfilAtuacaoRepositoryImpl implements PerfilAtuacaoRepository {
  async listarModelosTrabalho(token: string) {
    const list = await fetchModelosTrabalho(token);
    return list.map((m) => ({ id: m.id, descricao: m.descricao, codigo: m.codigo }));
  }

  async listarGestores(
    token: string,
    clientCode: string | null,
    busca: string,
    cursor = 0,
    limite = 5000,
  ): Promise<GestorItem[]> {
    const list = await fetchGestores(token, clientCode, busca, cursor, limite);
    return list.map((g) => ({
      id: g.codigoInternoColaborador ?? g.codGestorExterno ?? '',
      descricao: g.nome ?? '',
      email: g.email,
      codGestorExterno: g.codGestorExterno,
    }));
  }

  async listarPermanencias(token: string): Promise<Permanencia[]> {
    const list = await fetchPermanencias(token);
    return list.map((p) => ({ id: p.id, descricao: p.descricao }));
  }

  async listarLocalidades(token: string): Promise<Localidade[]> {
    const list = await fetchLocalidades(token);
    return list.map((l) => ({ id: l.id, descricao: l.descricao }));
  }

  async listarTiposEmprego(token: string): Promise<TipoEmprego[]> {
    const list = await fetchTiposEmprego(token);
    return list.map((t) => ({ id: t.id, descricao: t.descricao }));
  }

  async listarNiveisExperiencia(token: string): Promise<NivelExperiencia[]> {
    const list = await fetchNiveisExperiencia(token);
    return list.map((n) => ({ id: n.id, descricao: n.descricao }));
  }
}
