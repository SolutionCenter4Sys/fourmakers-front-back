import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoIaApi } from '@data/api/ComunicacaoIaApi';
import type {
  AssistenteIaComunicacaoPayload,
  AssistenteIaComunicacaoProcessamentoResult,
  AssistenteIaComunicacaoRetornoBruto,
} from '@domain/entities/comunicacao';
import type { ComunicacaoIaRepository } from '@domain/repositories/ComunicacaoIaRepository';

@injectable()
export class ComunicacaoIaRepositoryImpl implements ComunicacaoIaRepository {
  constructor(
    @inject(DiTokens.comunicacaoIaApi)
    private readonly api: ComunicacaoIaApi,
  ) {}

  async processarConteudo(
    token: string,
    payload: AssistenteIaComunicacaoPayload,
  ): Promise<AssistenteIaComunicacaoProcessamentoResult> {
    const response = await this.api.postAssistente(token, payload);

    if (!response.sucesso) {
      const msg =
        (typeof response.mensagem === 'string' && response.mensagem.trim()) ||
        'Não foi possível processar o conteúdo com a IA.';
      return { sucesso: false, mensagem: msg };
    }

    const texto = this.extrairTextoProcessado(response.retorno);
    if (!texto) {
      return {
        sucesso: false,
        mensagem: 'A API não retornou texto processado.',
      };
    }

    return { sucesso: true, texto };
  }

  private extrairTextoProcessado(retorno: AssistenteIaComunicacaoRetornoBruto): string | null {
    if (retorno == null) {
      return null;
    }
    if (typeof retorno === 'string') {
      const t = retorno.trim();
      return t.length > 0 ? retorno : null;
    }
    if (typeof retorno === 'object') {
      const candidato =
        retorno.texto ?? retorno.textoProcessado ?? retorno.conteudo;
      if (typeof candidato === 'string' && candidato.trim().length > 0) {
        return candidato;
      }
    }
    return null;
  }
}
