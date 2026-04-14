import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import { VagaApi } from '@data/api/VagaApi';
import { VagaEdicaoApi } from '@data/api/VagaEdicaoApi';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import type {
  ListarCandidatosAderentesParams,
  ListarCandidatosInscritosParams,
  VagaRepository,
} from '@domain/repositories/VagaRepository';
import type {
  CandidatoInscritoRaw,
  CandidatoAderenteRaw,
  CandidatarOutraPessoaPayload,
  VagaRecrutamentoCompleto,
} from '@domain/entities/GestaoVagasCandidatos';

@injectable()
export class VagaRepositoryImpl implements VagaRepository {
  constructor(
    @inject(DiTokens.vagaApi)
    private readonly api: VagaApi,
    @inject(DiTokens.vagaEdicaoApi)
    private readonly vagaEdicaoApi: VagaEdicaoApi
  ) {}

  async getVagaRecrutamentoPorId(token: string, vagaId: string): Promise<VagaRecrutamentoCompleto | null> {
    const res = await this.vagaEdicaoApi.getVagaRecrutamentoPorId(token, vagaId);
    return res?.retorno ?? null;
  }

  async getVagaDetalhesPublico(codigoVaga: number): Promise<VagaDetails | null> {
    const response = await this.api.getVagaDetalhesPublico(codigoVaga);
    const raw = response as Record<string, unknown> | null;
    // API Srs/DetalharVaga retorna { detalhe: [ {...} ], data: [], sucesso } – payload no array detalhe (data vem vazio)
    let data = (raw?.detalhe ?? raw?.retorno ?? raw?.Retorno ?? raw?.data ?? raw?.Data ?? raw) as Record<string, unknown> | unknown[] | null | undefined;
    if (Array.isArray(data) && data.length > 0) {
      data = data[0] as Record<string, unknown>;
    } else if (Array.isArray(data)) {
      data = null;
    }
    data = data as Record<string, unknown> | null | undefined;
    if (!data || typeof data !== 'object') return null;
    const get = (obj: Record<string, unknown>, ...keys: string[]): unknown => {
      for (const k of keys) {
        const v = obj[k];
        if (v !== undefined) return v;
      }
      return undefined;
    };
    const getStrMulti = (obj: Record<string, unknown>, ...keys: string[]): string | null => {
      const v = get(obj, ...keys);
      if (v != null && String(v).trim() !== '') return String(v).trim();
      return null;
    };
    const getNum = (...keys: string[]): number | null => {
      const v = get(data, ...keys);
      if (typeof v === 'number') return v;
      if (v != null) {
        const n = Number(v);
        return Number.isFinite(n) ? n : null;
      }
      return null;
    };
    const getStr = (...keys: string[]): string | null => {
      const v = get(data, ...keys);
      return v != null ? String(v).trim() || null : null;
    };
    const codigo = getNum('id_vaga', 'codigo', 'Codigo') ?? codigoVaga;
    const titulo = getStrMulti(data, 'titulo', 'Titulo', 'tituloVaga', 'TituloVaga', 'nome', 'Nome');
    const descricao = getStrMulti(data, 'descricao', 'Descricao', 'descricaoVaga', 'DescricaoVaga', 'texto', 'Texto');
    const skillsRaw = (get(data, 'skillsVagasDTO', 'skills', 'Skills', 'habilidades', 'Habilidades') ?? []) as Array<Record<string, unknown>>;
    const skillsList = Array.isArray(skillsRaw) ? skillsRaw : [];
    return {
      id: getStr('id', 'Id', 'id_vaga') ?? String(codigo),
      codigo: codigo ?? null,
      titulo: titulo ?? null,
      descricao: descricao ?? null,
      cargo: getStr('cargo', 'Cargo'),
      custoProfissional: getNum('custoProfissional', 'CustoProfissional'),
      rateCard: getNum('rateCard', 'RateCard'),
      modeloTrabalhoId: getStr('modeloTrabalhoId', 'ModeloTrabalhoId'),
      modeloTrabalhoDescricao: getStr('modalidade', 'modeloTrabalhoDescricao', 'ModeloTrabalhoDescricao'),
      nomeGestor: getStr('nomeGestor', 'NomeGestor'),
      nomeCliente: getStr('nomeCliente', 'NomeCliente'),
      codigoCliente: getStr('codigoCliente', 'CodigoCliente'),
      localizacaoId: getStr('localizacao', 'Localizacao', 'localizacaoId', 'LocalizacaoId'),
      cidade: getStr('cidade', 'Cidade'),
      estado: getStr('estado', 'Estado'),
      pais: getStr('pais', 'Pais'),
      dataCriacao: getStr('dataCriacao', 'DataCriacao', 'data_abertura'),
      dataUltimaAlteracao: getStr('dataUltimaAlteracao', 'DataUltimaAlteracao'),
      statusVagaCod: getStr('statusVagaCod', 'StatusVagaCod'),
      frequencia: getStr('frequencia', 'Frequencia'),
      skills: skillsList.map((s) => ({
        id: String(s.skill_id ?? s.skillId ?? s.id ?? s.Id ?? ''),
        skillId: Number(s.skill_id ?? s.skillId ?? s.SkillId ?? 0),
        skillDescription: String(s.skill_Description ?? s.skillDescription ?? s.SkillDescription ?? ''),
        skillNivelDescription: String(s.skill_nivel_description ?? s.skillNivelDescription ?? s.SkillNivelDescription ?? '').trim(),
        tipoSkillId: Number(s.type_skills_id ?? s.tipoSkillId ?? s.TipoSkillId ?? 0),
        relevante: Boolean(s.relevante ?? s.Relevante ?? true),
      })),
    };
  }

  async candidatarSe(
    token: string,
    payload: {
      codigoVaga: number;
      opcoesContatoIds: string[];
      pretencaoSalarial: string;
      modeloTrabalhoId: string;
    }
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return this.api.candidatarSe(token, payload);
  }

  async atualizarVagaRecrutamentoPorId(
    token: string,
    body: VagaRecrutamentoCompleto
  ): Promise<{ sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    return this.vagaEdicaoApi.atualizarVagaRecrutamentoPorId(token, body as Parameters<VagaEdicaoApi['atualizarVagaRecrutamentoPorId']>[1]);
  }

  async getVagaDetails(token: string, vagaId: string): Promise<VagaDetails> {
    const response = await this.api.getVagaDetalhes(token, vagaId);
    const data = response?.retorno;

    if (!data) {
      throw new Error('Detalhes da vaga não encontrados.');
    }

    return {
      id: data.id,
      codigo: data.codigo ?? null,
      titulo: data.titulo ?? null,
      descricao: data.descricao ?? null,
      cargo: data.cargo ?? null,
      custoProfissional: data.custoProfissional ?? null,
      rateCard: data.rateCard ?? null,
      modeloTrabalhoId: data.modeloTrabalhoId ?? null,
      modeloTrabalhoDescricao: data.modeloTrabalhoDescricao ?? null,
      nomeGestor: data.nomeGestor ?? null,
      nomeCliente: data.nomeCliente ?? null,
      codigoCliente: data.codigoCliente ?? null,
      localizacaoId: data.localizacao ?? null,
      cidade: data.cidade ?? null,
      estado: data.estado ?? null,
      pais: data.pais ?? null,
      dataCriacao: data.dataCriacao ?? null,
      dataUltimaAlteracao: data.dataUltimaAlteracao ?? null,
      statusVagaCod: data.statusVagaCod ?? null,
      frequencia: data.frequencia != null ? String(data.frequencia) : null,
      skills: data.skills?.map((skill) => ({
        id: skill.id,
        skillId: skill.skillId,
        skillDescription: skill.skillDescription,
        skillNivelDescription: skill.skillNivelDescription,
        tipoSkillId: skill.tipoSkillId,
        relevante: skill.relevante ?? false,
      })) ?? [],
    };
  }

  async mudarStatusCandidatura(token: string, payload: import('@domain/entities/GestaoVagasCandidatos').MudarStatusCandidaturaPayload) {
    return this.api.mudarStatusCandidatura(token, payload);
  }

  async listarCandidatosInscritos(token: string, params: ListarCandidatosInscritosParams) {
    const res = await this.api.listarCandidatosInscritos(token, params);
    return { retorno: (res?.retorno ?? []) as CandidatoInscritoRaw[], sucesso: res?.sucesso };
  }

  async listarCandidatosAderentes(token: string, params: ListarCandidatosAderentesParams) {
    const res = await this.api.listarCandidatosAderentes(token, params);
    return { retorno: (res?.retorno ?? []) as CandidatoAderenteRaw[], sucesso: res?.sucesso };
  }

  async listarStatusCandidaturaRecrutamento(token: string) {
    const res = await this.api.listarStatusCandidaturaRecrutamento(token);
    const raw = res?.retorno ?? [];
    return raw.map((item) => ({ id: String(item.id), nome: item.descricao }));
  }

  async obterTotaisInscritos(token: string, vagaId: string) {
    const res = await this.api.obterTotaisInscritos(token, vagaId);
    return res?.retorno ?? null;
  }

  async listarUnidades(token: string) {
    const res = await this.api.listarUnidades(token);
    return (res?.retorno ?? []) as import('@domain/entities/GestaoVagasCandidatos').UnidadeItem[];
  }

  async listarTiposVaga(token: string) {
    const res = await this.api.listarTiposVaga(token);
    return (res?.retorno ?? []) as import('@domain/entities/GestaoVagasCandidatos').TipoVagaItem[];
  }

  async listarTiposContratacao(token: string) {
    const res = await this.api.listarTiposContratacao(token);
    return (res?.retorno ?? []) as import('@domain/entities/GestaoVagasCandidatos').TipoContratacaoItem[];
  }

  async candidatarOutraPessoa(token: string, payload: CandidatarOutraPessoaPayload) {
    return this.api.candidatarOutraPessoa(token, payload);
  }

  async adicionarRecrutadorVaga(
    token: string,
    vagaId: string,
    codInternoColaboradorRecrutador: string
  ) {
    return this.api.adicionarRecrutadorVaga(token, vagaId, codInternoColaboradorRecrutador);
  }

  async listarMotivosPerdaVaga(token: string) {
    const res = await this.api.listarMotivosPerdaVaga(token);
    const list = res?.retorno ?? [];
    return list.sort((a, b) => (a.ordem ?? 0) - (b.ordem ?? 0)) as import('@domain/entities/GestaoVagasCandidatos').MotivoPerdaVagaItem[];
  }

  async gravarPerdaVaga(
    token: string,
    payload: import('@domain/entities/GestaoVagasCandidatos').GravarPerdaVagaPayload
  ) {
    return this.api.gravarPerdaVaga(token, payload);
  }

  async mudarStatusVaga(
    token: string,
    payload: import('@domain/entities/GestaoVagasCandidatos').MudarStatusVagaPayload
  ) {
    return this.api.mudarStatusVaga(token, payload);
  }

  async listarOpcoesContato(token: string) {
    const res = await this.api.listarOpcoesContato(token);
    return (res?.retorno ?? []) as import('@domain/entities/GestaoVagasCandidatos').OpcaoContatoItem[];
  }

  async listarVagasRecrutamentoPorParentEmAndamento(token: string, vagaIdParent: string) {
    const res = await this.api.listarVagasRecrutamentoPorParentEmAndamento(token, vagaIdParent);
    return (Array.isArray(res?.retorno) ? res.retorno : []) as import('@domain/entities/GestaoVagasCandidatos').VagaFilhaItem[];
  }

  async inserirInformacoesComplementaresVagaRecrutamento(
    token: string,
    payload: import('@domain/entities/GestaoVagasCandidatos').InserirInformacoesComplementaresPayload
  ) {
    return this.api.inserirInformacoesComplementaresVagaRecrutamento(token, payload);
  }

  async getRelatorioProdutividade(token: string, dataInicio: string, dataFim: string) {
    return this.api.getRelatorioProdutividade(token, dataInicio, dataFim);
  }

  async getRelatorioVagas(token: string, dataInicio: string, dataFim: string) {
    return this.api.getRelatorioVagas(token, dataInicio, dataFim);
  }

  async getRelatorioVagasCandidaturas(token: string, dataInicio: string, dataFim: string) {
    return this.api.getRelatorioVagasCandidaturas(token, dataInicio, dataFim);
  }
}
