export interface DadosBancarios {
  id?: string
  codigoBancoTed: string
  agenciaTed: string
  agenciaDvTed: string
  contaTed: string
  contaDvTed: string
  chavePix: string
  tipoChavePix: 'C' | 'J' | 'E' | 'T' | 'R'
  formaPagamento: 1 | 2 // 1 = PIX, 2 = TED
}

export interface DadosBancariosResponse {
  retorno: DadosBancarios | null
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface PayloadDadosBancarios {
  codigoBancoTed: string
  agenciaTed: string
  agenciaDvTed: string
  contaTed: string
  contaDvTed: string
  chavePix: string
  tipoChavePix: 'C' | 'J' | 'E' | 'T' | 'R'
  formaPagamento: 1 | 2
}

