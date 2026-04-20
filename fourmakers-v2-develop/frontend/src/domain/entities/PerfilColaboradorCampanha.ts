import type { AwsCertificacaoRequest } from './AwsCertificacao'

export interface ArquivoBase64 {
  base64: string
  tipo: number
}

export interface EnderecoRoot {
  codigoPostal: string
  endereco: string
  numero: number
  bairro: string
  cidade: string
  estado: string
  complemento: string
  comprovanteResidenciaPath: string
}

export interface EnderecoRequest {
  arquivoBase64: ArquivoBase64
  root: EnderecoRoot
}

export interface FormaAtuacaoRequest {
  modeloTrabalho: string
  frequencia: string
  frequenciaId: number
  diasSemana: string[]
  localTrabalho: string
  clienteNome: string
  clienteEndereco: string
}

export interface DadosPessoaisRequest {
  linkedin: string | null
  pdc: string
  telefone: string
  ddi: string
}

export interface ColetaPerfilColaboradorCampanhaRequest {
  endereco: EnderecoRequest
  formaAtuacao: FormaAtuacaoRequest
  awsTechnical: AwsCertificacaoRequest
  awsTechnicalFoundational: AwsCertificacaoRequest
  awsTechnicalAccredited: AwsCertificacaoRequest
  dadosPessoais: DadosPessoaisRequest
}

export interface ColetaPerfilColaboradorCampanhaResponse {
  sucesso: boolean
  mensagem: string
  erros: string[]
  retorno: string
}

