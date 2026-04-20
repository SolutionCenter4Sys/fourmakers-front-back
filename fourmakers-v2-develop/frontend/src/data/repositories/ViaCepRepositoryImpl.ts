import { inject, injectable } from 'tsyringe'

import type { ViaCepRepository, EnderecoViaCep } from '@domain/repositories/ViaCepRepository'
import type { ViaCepResponse } from '@data/api/ViaCepApi'

import { ViaCepApi } from '@data/api/ViaCepApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ViaCepRepositoryImpl implements ViaCepRepository {
  constructor(
    @inject(DiTokens.viaCepApi)
    private readonly api: ViaCepApi,
  ) {}

  async buscarEnderecoPorCep(cep: string): Promise<EnderecoViaCep | null> {
    try {
      const data: ViaCepResponse = await this.api.buscarEnderecoPorCep(cep)

      // Mapeia os dados da API para o formato do domínio
      const enderecoData: EnderecoViaCep = {
        endereco: data.logradouro || '',
        bairro: data.bairro || '',
        cidade: data.localidade || '',
        estado: data.uf || '',
        complemento: data.complemento || undefined,
      }

      return enderecoData
    } catch (error) {
      // Retorna null em caso de erro (CEP não encontrado, erro de rede, etc.)
      return null
    }
  }
}

