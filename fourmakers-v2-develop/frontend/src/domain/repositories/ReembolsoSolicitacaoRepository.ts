import type {
  ProjetoColaborador,
  Verba,
  VerbaEParametros,
  ArquivoAnalise,
  ResultadoAnaliseComprovante,
  PayloadInserirSolicitacao,
} from '@data/api/ReembolsoSolicitacaoApi'

export interface ReembolsoSolicitacaoRepository {
  listarProjetosColaborador(
    token: string,
    codigoProfissional: string
  ): Promise<{ sucesso: boolean; retorno?: ProjetoColaborador[]; mensagem?: string }>

  listarVerbasComExcecao(
    token: string,
    codigoProjeto: string,
    codigoCliente: string
  ): Promise<{ sucesso: boolean; retorno?: Verba[]; mensagem?: string }>

  obterVerbasEParametroValidacao(
    token: string
  ): Promise<{ sucesso: boolean; retorno?: VerbaEParametros; mensagem?: string }>

  analisarComprovantesFiscais(
    token: string,
    arquivos: ArquivoAnalise[]
  ): Promise<{ sucesso: boolean; retorno?: ResultadoAnaliseComprovante; mensagem?: string }>

  inserirSolicitacoes(
    token: string,
    payload: PayloadInserirSolicitacao
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }>

  inserirSolicitacoesZip(
    token: string,
    arquivoZip: File
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }>
}

