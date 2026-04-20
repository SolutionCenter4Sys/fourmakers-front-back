/**
 * Interface para dados de endereço retornados pelo ViaCEP
 */
export interface EnderecoViaCep {
  endereco: string
  bairro: string
  cidade: string
  estado: string
  complemento?: string
}

/**
 * Repository interface para operações relacionadas ao ViaCEP
 */
export interface ViaCepRepository {
  /**
   * Busca endereço através do CEP
   * @param cep - CEP no formato "00000-000" ou "00000000"
   * @returns Promise com os dados do endereço ou null em caso de erro
   */
  buscarEnderecoPorCep(cep: string): Promise<EnderecoViaCep | null>
}

