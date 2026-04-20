import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import { CandidaturaApi } from '@data/api/CandidaturaApi';
import type { MotivoReprovacaoItem, MotivoDeclinioItem } from '@data/api/CandidaturaApi';
import type { CandidaturaDetails, CandidaturaEditPayload } from '@domain/entities/CandidaturaDetails';
import type {
  AtualizarCandidaturaPayload,
  DeclinarCandidaturaPayload,
  InserirArquivoCandidaturaParams,
  InserirComentarioCandidaturaPayload,
  ReprovarCandidaturaPayload,
} from '@domain/entities/GestaoVagasCandidatos';
import type { CandidaturaRepository, SaveCandidaturaResult } from '@domain/repositories/CandidaturaRepository';

@injectable()
export class CandidaturaRepositoryImpl implements CandidaturaRepository {
  constructor(
    @inject(DiTokens.candidaturaApi)
    private readonly api: CandidaturaApi,
  ) {}

  async getCandidaturaDetails(
    token: string,
    candidateId: string,
    candidaturaId?: string,
  ): Promise<CandidaturaDetails> {
    const response = await this.api.getCandidaturaDetails(token, candidateId, candidaturaId);
    const retorno = response?.retorno;

    if (!retorno?.colaborador) {
      throw new Error('Detalhes da candidatura não encontrados.');
    }

    return {
      colaborador: {
        ...retorno.colaborador,
        endereco: retorno.colaborador.endereco ?? null,
      },
      dadosCandidatura: retorno.dadosCandidatura ?? null,
      dadosDemograficos: retorno.dadosDemograficos ?? null,
    };
  }

  async saveCandidaturaDetails(token: string, payload: CandidaturaEditPayload): Promise<SaveCandidaturaResult> {
    const response = await this.api.editCandidaturaDetails(token, payload);
    return {
      mensagem: response?.mensagem,
      sucesso: response?.sucesso,
    };
  }

  async atualizarCandidatura(token: string, payload: AtualizarCandidaturaPayload): Promise<SaveCandidaturaResult> {
    const res = await this.api.atualizarCandidatura(token, payload);
    return { mensagem: res?.mensagem ?? undefined, sucesso: res?.sucesso };
  }

  async listarMotivosReprovacao(token: string) {
    const list = await this.api.listarMotivosReprovacao(token);
    return list.map((m: MotivoReprovacaoItem) => ({ id: m.id, descricao: m.descricao, ativo: m.ativo }));
  }

  async reprovarCandidatura(token: string, payload: ReprovarCandidaturaPayload): Promise<SaveCandidaturaResult> {
    const res = await this.api.reprovarCandidatura(token, payload);
    return { mensagem: res?.mensagem ?? undefined, sucesso: res?.sucesso };
  }

  async listarMotivosDeclinio(token: string) {
    const list = await this.api.listarMotivosDeclinio(token);
    return list.map((m: MotivoDeclinioItem) => ({ id: m.id, descricao: m.descricao, ativo: m.ativo }));
  }

  async declinarCandidato(token: string, payload: DeclinarCandidaturaPayload): Promise<SaveCandidaturaResult> {
    const res = await this.api.declinarCandidato(token, payload);
    return { mensagem: res?.mensagem ?? undefined, sucesso: res?.sucesso };
  }

  async atualizarRecrutadorResponsavel(
    token: string,
    idCandidatura: string,
    codigoRecrutadorResponsavel: string
  ): Promise<SaveCandidaturaResult> {
    const res = await this.api.atualizarRecrutadorResponsavel(token, idCandidatura, codigoRecrutadorResponsavel);
    return { mensagem: res?.mensagem ?? undefined, sucesso: res?.sucesso };
  }

  async inserirArquivo(token: string, params: InserirArquivoCandidaturaParams): Promise<SaveCandidaturaResult> {
    const res = await this.api.inserirArquivo(token, params);
    return { mensagem: res?.mensagem ?? undefined, sucesso: res?.sucesso };
  }

  async inserirComentarioCandidatura(
    token: string,
    payload: InserirComentarioCandidaturaPayload
  ): Promise<SaveCandidaturaResult> {
    const res = await this.api.inserirComentarioCandidatura(token, payload);
    return { mensagem: res?.mensagem ?? undefined, sucesso: res?.sucesso };
  }

  async listarMeusTalentos(
    token: string,
    _params?: { busca?: string; cursor?: number; limite?: number }
  ) {
    const res = await this.api.listarMeusTalentos(token);
    const list = res?.retorno ?? [];
    return list.map((item) => ({
      nomeColaborador: item.nomeColaborador,
      codigoVaga: item.codigoVaga,
      nomeVaga: item.nomeVaga,
      nomeCliente: item.nomeCliente,
      nomeGestor: item.nomeGestor,
      statusVaga: item.statusVaga,
      statusMovimentacao: item.statusMovimentacao,
      ultimaAlteracao: item.ultimaAlteracao,
      codigoColaboradorUltimaMovimentacao: item.codigoColaboradorUltimaMovimentacao ?? null,
      recrutadorUltimaMovimentacao: item.recrutadorUltimaMovimentacao,
      codColaborador: item.codColaborador,
      criadoPor: item.criadoPor,
    }));
  }

  async buscarCandidaturasColaborador(token: string, codColaborador: string) {
    const res = await this.api.buscarCandidaturasColaborador(token, codColaborador);
    const list = res?.retorno ?? [];
    return list.map((item) => ({
      codigo: item.codigo,
      titulo: item.titulo,
      dataCriacao: item.dataCriacao ?? null,
      dataUltimaAlteracao: item.dataUltimaAlteracao ?? null,
      statusCandidaturaId: item.statusCandidaturaId,
      statusCandidaturaDescricao: item.statusCandidaturaDescricao,
      candidaturaId: item.candidaturaId,
      pretensaoSalarial: item.pretensaoSalarial ?? null,
      modeloTrabalhoId: item.modeloTrabalhoId ?? null,
      disponibilidadeEntrevistaId: item.disponibilidadeEntrevistaId ?? null,
      quantidadeDiasPresencial: item.quantidadeDiasPresencial ?? null,
      nomeCliente: item.nomeCliente,
      codigoCliente: item.codigoCliente,
      codigoGestor: item.codigoGestor,
      nomeGestor: item.nomeGestor,
      idVaga: item.idVaga,
      statusVaga: item.statusVaga,
      dataCandidatura: item.dataCandidatura ?? null,
      codigoInternoColaborador: item.codigoInternoColaborador ?? null,
    }));
  }
}
