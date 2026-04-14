import { inject, injectable } from 'tsyringe'

import type { ViaCepRepository, EnderecoViaCep } from '@domain/repositories/ViaCepRepository'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class GetAddressByCepUseCase {
  constructor(
    @inject(DiTokens.viaCepRepository)
    private readonly repository: ViaCepRepository,
  ) {}

  /**
   * Busca endereço através do CEP
   * @param cep - CEP no formato "00000-000" ou "00000000"
   * @returns Promise com os dados do endereço ou null em caso de erro
   */
  async execute(cep: string): Promise<EnderecoViaCep | null> {
    // Validação básica do CEP
    const cepLimpo = cep.replace(/\D/g, '')
    
    if (cepLimpo.length !== 8) {
      throw new Error('CEP deve conter 8 dígitos')
    }

    return this.repository.buscarEnderecoPorCep(cep)
  }
}

