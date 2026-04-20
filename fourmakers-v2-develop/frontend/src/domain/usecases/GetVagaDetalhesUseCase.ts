import { inject, injectable } from 'tsyringe';
import { DiTokens } from '@core/di/tokens';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import type { VagaRepository } from '@domain/repositories/VagaRepository';

@injectable()
export class GetVagaDetalhesUseCase {
  constructor(
    @inject(DiTokens.vagaRepository)
    private readonly repository: VagaRepository
  ) {}

  /**
   * Obtém detalhes da vaga por id (guid) ou por código numérico.
   * Se idOrCode for só dígitos, usa endpoint público por código; senão usa ObterVagaRecrutamentoPorId.
   */
  async execute(token: string, idOrCode: string): Promise<VagaDetails> {
    const codigoNumerico = /^\d+$/.test(idOrCode) ? Number(idOrCode) : null;
    if (codigoNumerico != null) {
      const detalhe = await this.repository.getVagaDetalhesPublico(codigoNumerico);
      if (!detalhe) {
        throw new Error('Detalhes da vaga não encontrados.');
      }
      return detalhe;
    }
    return this.repository.getVagaDetails(token, idOrCode);
  }
}
