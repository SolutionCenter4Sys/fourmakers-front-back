import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoComunidadeApi } from '@data/api/ComunicacaoComunidadeApi';
import type {
  CommunityGroup,
  ComunicacaoComunidadeDetalheRetorno,
  CriarComunidadeApiResponse,
} from '@domain/entities/comunicacao';
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository';
import { comunidadeApiToCommunityGroup } from '@shared/utils/comunicacaoComunidadeMapper';

@injectable()
export class ComunicacaoComunidadeRepositoryImpl
  implements ComunicacaoComunidadeRepository
{
  constructor(
    @inject(DiTokens.comunicacaoComunidadeApi)
    private readonly api: ComunicacaoComunidadeApi,
  ) {}

  async listarComunidades(token: string): Promise<CommunityGroup[]> {
    const response = await this.api.getComunidades(token);
    if (!response.sucesso || !response.retorno?.comunidades) {
      return [];
    }
    return response.retorno.comunidades.map((c) =>
      comunidadeApiToCommunityGroup(c, token),
    );
  }

  async obterComunidadePorId(
    token: string,
    comunidadeId: string,
  ): Promise<ComunicacaoComunidadeDetalheRetorno> {
    const response = await this.api.getComunidadeById(token, comunidadeId);
    if (!response.sucesso || !response.retorno) {
      throw new Error(response.mensagem ?? 'Falha ao obter comunidade.');
    }
    return response.retorno;
  }

  async criarComunidade(
    token: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse> {
    return this.api.criarComunidade(token, formData);
  }

  async atualizarComunidade(
    token: string,
    comunidadeId: string,
    formData: FormData,
  ): Promise<CriarComunidadeApiResponse> {
    return this.api.atualizarComunidade(token, comunidadeId, formData);
  }

  async participar(
    token: string,
    comunidadeId: string,
  ): Promise<{ sucesso: boolean; mensagem: string | null }> {
    return this.api.participar(token, comunidadeId);
  }

  async sair(
    token: string,
    comunidadeId: string,
  ): Promise<{ sucesso: boolean; mensagem: string | null }> {
    return this.api.sair(token, comunidadeId);
  }
}
