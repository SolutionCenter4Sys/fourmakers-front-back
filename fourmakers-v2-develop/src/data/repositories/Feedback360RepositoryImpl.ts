import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { Feedback360Api } from '@data/api/Feedback360Api';
import type {
  Feedback360Item,
  Feedback360AvaliacaoItem,
  Feedback360RelacionamentoItem,
  Feedback360CriarPayload,
  Feedback360AtualizarPayload,
  Feedback360CreateResponse,
} from '@domain/entities/Feedback360';
import type { Feedback360Repository } from '@domain/repositories/Feedback360Repository';

@injectable()
export class Feedback360RepositoryImpl implements Feedback360Repository {
  constructor(
    @inject(DiTokens.feedback360Api)
    private readonly api: Feedback360Api,
  ) {}

  async listarAvaliacoes(token: string): Promise<Feedback360AvaliacaoItem[]> {
    return this.api.listarAvaliacoes(token);
  }

  async listarRelacionamentos(token: string): Promise<Feedback360RelacionamentoItem[]> {
    return this.api.listarRelacionamentos(token);
  }

  async listarEnviados(token: string): Promise<Feedback360Item[]> {
    const res = await this.api.listarEnviados(token);
    return res?.retorno ?? [];
  }

  async listarRecebidos(token: string): Promise<Feedback360Item[]> {
    const res = await this.api.listarRecebidos(token);
    return res?.retorno ?? [];
  }

  async criar(token: string, payload: Feedback360CriarPayload): Promise<Feedback360CreateResponse> {
    return this.api.criar(token, payload);
  }

  async atualizar(
    token: string,
    id: string,
    payload: Feedback360AtualizarPayload,
  ): Promise<Feedback360CreateResponse> {
    return this.api.atualizar(token, id, payload);
  }
}
