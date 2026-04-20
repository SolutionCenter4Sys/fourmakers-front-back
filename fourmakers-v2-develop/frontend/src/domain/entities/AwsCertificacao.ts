import type { ArquivoBase64 } from './PerfilColaboradorCampanha'

export interface AwsCertificacaoRoot {
  status: string
  filePath: string
  nome: string
  dataEmissao: string
  emissor: string
  cargaHoraria: number
  previsaoConclusao: string
}

export interface AwsCertificacaoRequest {
  arquivoBase64: ArquivoBase64
  root: AwsCertificacaoRoot
}

