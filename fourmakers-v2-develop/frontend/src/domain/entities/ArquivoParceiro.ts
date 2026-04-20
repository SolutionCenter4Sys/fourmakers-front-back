// Entidade para Upload de Arquivos
export interface InserirArquivoParams {
  parceiroId: string // UUID da empresa
  arquivoTipoId: number // 1=PDF, 2=Planilha, 3=Imagem
  arquivoOriginId: number // Origem do arquivo
  parceiroGestaoContratoId: string // UUID do contrato vinculado
  file: File // Arquivo binário
}

export interface InserirArquivoResponse {
  sucesso: boolean
  mensagem?: string
  dados?: {
    id: string
    nomeArquivo: string
    urlDownload: string
  }
  erros?: string[]
}
