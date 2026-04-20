import { inject, injectable } from 'tsyringe';
import type { VagasRepository } from '@domain/repositories/VagasRepository';
import type {
  ListarVagasRecrutamentoEPerfisParams,
  ListarVagasRecrutamentoParams,
  ListarPerfisVagasParams,
  ListarVagasRecrutamentoEPerfisResult,
  ListarVagasRecrutamentoResult,
  ListarPerfisVagasResult,
} from '@domain/repositories/VagasRepository';
import type { Vaga, PerfilVaga, StatusVaga } from '@domain/entities/Vaga';
import { VagasApi } from '@data/api/VagasApi';
import { DiTokens } from '@core/di/tokens';

function toDisplayString(value: unknown): string {
  if (value === null || value === undefined) return '';
  if (typeof value === 'string') return value;
  if (typeof value === 'number' || typeof value === 'boolean') return String(value);

  if (typeof value === 'object') {
    const record = value as Record<string, unknown>;
    const candidateKeys = ['nome', 'descricao', 'label', 'title', 'titulo', 'name', 'text', 'value'];
    for (const key of candidateKeys) {
      const candidate = record[key];
      if (typeof candidate === 'string') return candidate;
      if (typeof candidate === 'number' || typeof candidate === 'boolean') return String(candidate);
    }

    const entries = Object.entries(record);
    if (entries.length > 0 && entries.every(([, v]) => typeof v === 'string' || typeof v === 'number')) {
      return entries
        .sort(([a], [b]) => Number(a) - Number(b))
        .map(([, v]) => String(v))
        .join(', ');
    }

    try {
      return JSON.stringify(value);
    } catch {
      return String(value);
    }
  }

  return String(value);
}

function toCount(value: unknown): number {
  if (typeof value === 'number') return Number.isFinite(value) ? value : 0;
  if (typeof value === 'string') {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }
  if (Array.isArray(value)) return value.length;
  if (value && typeof value === 'object') {
    const values = Object.values(value as Record<string, unknown>);
    const numericValues = values
      .map((v) => (typeof v === 'number' ? v : typeof v === 'string' ? Number(v) : NaN))
      .filter((n) => Number.isFinite(n));
    if (numericValues.length > 0) return numericValues.reduce((sum, n) => sum + n, 0);
  }
  return 0;
}

function normalizeVaga(raw: unknown): Vaga {
  const record = (raw ?? {}) as Record<string, unknown>;

  const recrutadorVagaRaw = record.recrutadorVaga;
  let recrutadorVaga: Vaga['recrutadorVaga'] | undefined;
  if (recrutadorVagaRaw && typeof recrutadorVagaRaw === 'object') {
    const r = recrutadorVagaRaw as Record<string, unknown>;
    const nome = toDisplayString(r.nome ?? r.descricao ?? r.name);
    const idRaw = r.id;
    recrutadorVaga = {
      id: typeof idRaw === 'number' ? idRaw : Number(idRaw),
      nome,
    };
  }

  const statusVagaCod = toDisplayString(record.statusVagaCod ?? record.codigoStatusVaga ?? record.codigoStatus ?? record.status);

  return {
    id: (typeof record.id === 'string' || typeof record.id === 'number') ? record.id : toDisplayString(record.id),
    codigo: typeof record.codigo === 'number' ? record.codigo : record.codigo === undefined ? undefined : Number(record.codigo),
    titulo: toDisplayString(record.titulo),
    descricao: record.descricao === undefined ? undefined : toDisplayString(record.descricao),
    dataCriacao: toDisplayString(record.dataCriacao),
    dataUltimaAlteracao: record.dataUltimaAlteracao === undefined ? undefined : toDisplayString(record.dataUltimaAlteracao),
    dataEnd: record.dataEnd === undefined ? undefined : toDisplayString(record.dataEnd),
    status: statusVagaCod || toDisplayString(record.status),
    statusVagaCod: statusVagaCod || undefined,
    nomeCliente: record.nomeCliente === undefined ? undefined : toDisplayString(record.nomeCliente),
    unidadeCliente: record.unidadeCliente === undefined ? undefined : (record.unidadeCliente === null ? null : toDisplayString(record.unidadeCliente)),
    nomeGestor: record.nomeGestor === undefined ? undefined : toDisplayString(record.nomeGestor),
    nomeUsuarioAlterador: record.nomeUsuarioAlterador === undefined ? undefined : (record.nomeUsuarioAlterador === null ? null : toDisplayString(record.nomeUsuarioAlterador)),
    nomeRecrutadorVaga: record.nomeRecrutadorVaga === undefined ? undefined : (record.nomeRecrutadorVaga === null ? null : toDisplayString(record.nomeRecrutadorVaga)),
    nomeUsuarioCriador:
      record.nomeUsuarioCriador === undefined ? undefined : toDisplayString(record.nomeUsuarioCriador),
    recrutadorVaga,
    totalPerfis: toCount(record.totalPerfis),
    totalAderentes:
      toCount(record.totalAderentes),
    numeroDeVagas: toCount(record.numeroDeVagas),
    quantidadeCandidatosPorEstagio: Array.isArray(record.quantidadeCandidatosPorEstagio)
      ? (record.quantidadeCandidatosPorEstagio as Array<{ idStatus?: number; descricaoStatus?: string; quantidade?: number }>)
      : undefined,
    idPerfilGerador: toDisplayString(record.idPerfilGerador ?? record.perfilGeradorId ?? '') || undefined,
  };
}

function normalizeStatusVaga(raw: unknown): StatusVaga[] {
  const normalizeOne = (item: unknown): StatusVaga | null => {
    if (!item || typeof item !== 'object') return null;
    const record = item as Record<string, unknown>;
    const id = toDisplayString(record.id ?? record.codigo ?? record.value);
    const nome = toDisplayString(record.nome ?? record.descricao ?? record.label ?? record.name);
    const ativoRaw = record.ativo;
    const ativo = typeof ativoRaw === 'boolean' ? ativoRaw : true;
    const ordemRaw = record.ordem;
    const ordem = typeof ordemRaw === 'number' ? ordemRaw : ordemRaw === undefined ? undefined : Number(ordemRaw);
    return { id, nome, ativo, ordem };
  };

  if (Array.isArray(raw)) {
    return raw.map(normalizeOne).filter((v): v is StatusVaga => Boolean(v));
  }

  if (raw && typeof raw === 'object') {
    return Object.entries(raw as Record<string, unknown>).map(([id, nome]) => ({
      id,
      nome: toDisplayString(nome),
      ativo: true,
    }));
  }

  return [];
}

@injectable()
export class VagasRepositoryImpl implements VagasRepository {
  constructor(
    @inject(DiTokens.vagasApi)
    private readonly api: VagasApi
  ) {}

  async listarVagasRecrutamentoEPerfis(
    token: string,
    params: ListarVagasRecrutamentoEPerfisParams
  ): Promise<ListarVagasRecrutamentoEPerfisResult> {
    const response = await this.api.listarVagasRecrutamentoEPerfis(token, params);
    const vagasRecrutamento = response.retorno.vagasRecrutamento || [];

    const vagas = vagasRecrutamento.map((raw) => {
      const vaga = normalizeVaga(raw);
      return Object.assign(vaga, { _raw: raw }) as Vaga & { _raw: typeof raw };
    });

    return {
      vagas,
      perfis: response.retorno.perfis as PerfilVaga[],
      totalVagas: (response.retorno as unknown as Record<string, unknown>).totalVagas as number | Record<string, number>,
      totalPerfis: toCount(response.retorno.totalPerfis),
    };
  }

  async listarVagasRecrutamento(
    token: string,
    params: ListarVagasRecrutamentoParams
  ): Promise<ListarVagasRecrutamentoResult> {
    const response = await this.api.listarVagasRecrutamento(token, params);
    
    return {
      vagas: (response.retorno.vagasRecrutamento || []).map(normalizeVaga),
      totalVagas: (response.retorno as unknown as Record<string, unknown>).totalVagas as number | Record<string, number>,
    };
  }

  async listarPerfisVagas(
    token: string,
    params: ListarPerfisVagasParams
  ): Promise<ListarPerfisVagasResult> {
    const response = await this.api.listarPerfisVagas(token, params);
    
    return {
      perfis: response.retorno.perfis as PerfilVaga[],
      totalPerfis: response.retorno.totalPerfis,
    };
  }

  async listarStatusVagas(token: string): Promise<StatusVaga[]> {
    const response = await this.api.listarStatusVagas(token);
    return normalizeStatusVaga(response.retorno);
  }
}
