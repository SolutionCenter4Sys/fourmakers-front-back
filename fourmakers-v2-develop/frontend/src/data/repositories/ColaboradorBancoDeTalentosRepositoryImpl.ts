import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ColaboradorBancoDeTalentosApi } from '@data/api/ColaboradorBancoDeTalentosApi';
import type { ColaboradorBancoDeTalentosRepository } from '@domain/repositories/ColaboradorBancoDeTalentosRepository';

@injectable()
export class ColaboradorBancoDeTalentosRepositoryImpl implements ColaboradorBancoDeTalentosRepository {
  constructor(
    @inject(DiTokens.colaboradorBancoDeTalentosApi)
    private readonly api: ColaboradorBancoDeTalentosApi,
  ) {}

  async buscarBancoTalentos(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number }
  ) {
    const list = await this.api.buscarBancoTalentos(token, params);
    return list as import('@domain/entities/GestaoVagasCandidatos').BancoTalentosItem[];
  }

  async buscarPessoasCadastradasPorOrg(
    token: string,
    params: {
      busca?: string;
      cursor?: number;
      limite?: number;
      statusVaga?: string;
      statusCandidatura?: number;
      dataInicio?: string;
      dataFim?: string;
    }
  ) {
    const list = await this.api.buscarPessoasCadastradasPorOrg(token, params);
    return list as import('@domain/entities/GestaoVagasCandidatos').PessoaCadastradaPorOrg[];
  }

  async buscarPessoasCadastradasPorColaborador(
    token: string,
    codColaborador: string,
    params: { busca?: string; cursor?: number; limite?: number }
  ) {
    const list = await this.api.buscarPessoasCadastradasPorColaborador(token, codColaborador, params);
    return list as import('@domain/entities/GestaoVagasCandidatos').PessoaCadastradaPorColaborador[];
  }

  async buscarBancoTalentosComPromptMatch(
    token: string,
    params: { texto_vaga: string; limite?: number }
  ) {
    const res = await this.api.buscarBancoTalentosComPromptMatch(token, params);
    return {
      colaboradores: (res?.colaboradores ?? []) as import('@domain/entities/GestaoVagasCandidatos').ColaboradorMatchPromptItem[],
      idLogRankCandidatesIds: res?.idLogRankCandidatesIds ?? null,
      prompt: res?.prompt,
    };
  }

  async buscarMeusLotes(
    token: string,
    params: { busca?: string; cursor?: number; limite?: number }
  ) {
    const list = await this.api.buscarMeusLotes(token, params);
    return list as import('@domain/entities/GestaoVagasCandidatos').LoteItemGestao[];
  }

  async buscarInformacoesLote(
    token: string,
    idLote: string,
    params?: { busca?: string; cursor?: string | null; limite?: string | null }
  ) {
    return this.api.buscarInformacoesLote(token, idLote, params) as Promise<import('@domain/entities/GestaoVagasCandidatos').DetalhesLote>;
  }
}
