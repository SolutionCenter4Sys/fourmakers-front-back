import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { CurriculoColaboradorApi } from '@data/api/CurriculoColaboradorApi';
import type {
  CurriculoColaboradorRepository,
  SincronizarCurriculoResult,
} from '@domain/repositories/CurriculoColaboradorRepository';
import type {
  ImportarColaboradorResult,
  ImportarColaboradorLoteResult,
  ImportarColaboradorLotePlanilhaResult,
} from '@domain/entities/CurriculoColaborador';
import type { InsereCurriculoColaboradorResult } from '@domain/repositories/CurriculoColaboradorRepository';

@injectable()
export class CurriculoColaboradorRepositoryImpl implements CurriculoColaboradorRepository {
  constructor(
    @inject(DiTokens.curriculoColaboradorApi)
    private readonly api: CurriculoColaboradorApi
  ) {}

  async importarColaborador(token: string, file: File): Promise<ImportarColaboradorResult> {
    const res = await this.api.importarColaborador(token, file);
    const mensagem = res?.mensagem ?? res?.mensagen ?? undefined;
    return {
      sucesso: res?.sucesso,
      mensagem,
      codigoColaborador: res?.codigoColaborador,
    };
  }

  async importarColaboradorLinkedin(token: string, perfilId: string): Promise<ImportarColaboradorResult> {
    const res = await this.api.importarColaboradorLinkedin(token, perfilId);
    const mensagem = res?.mensagem ?? res?.mensagen ?? undefined;
    return {
      sucesso: res?.sucesso,
      mensagem,
      codigoColaborador: res?.codigoColaborador,
    };
  }

  async importarColaboradorLote(token: string, file: File): Promise<ImportarColaboradorLoteResult> {
    return this.api.importarColaboradorLote(token, file);
  }

  async importarColaboradorLotePlanilha(token: string, file: File): Promise<ImportarColaboradorLotePlanilhaResult> {
    return this.api.importarColaboradorLotePlanilha(token, file);
  }

  async sincronizarCurriculoColaborador(token: string, file: File): Promise<SincronizarCurriculoResult> {
    const res = await this.api.sincronizarCurriculoColaborador(token, file);
    return {
      sucesso: res?.sucesso ?? false,
      mensagem: res?.mensagem ?? undefined,
    };
  }

  async insereCurriculoColaborador(token: string, file: File): Promise<InsereCurriculoColaboradorResult> {
    return this.api.insereCurriculoColaborador(token, file);
  }
}
