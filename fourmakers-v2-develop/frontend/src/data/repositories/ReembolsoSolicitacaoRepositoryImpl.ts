import { inject, injectable } from 'tsyringe'

import type { ReembolsoSolicitacaoRepository } from '@domain/repositories/ReembolsoSolicitacaoRepository'
import type {
  ProjetoColaborador,
  Verba,
  VerbaEParametros,
  ArquivoAnalise,
  ResultadoAnaliseComprovante,
  PayloadInserirSolicitacao,
} from '@data/api/ReembolsoSolicitacaoApi'

import { ReembolsoSolicitacaoApi } from '@data/api/ReembolsoSolicitacaoApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ReembolsoSolicitacaoRepositoryImpl implements ReembolsoSolicitacaoRepository {
  constructor(
    @inject(DiTokens.reembolsoSolicitacaoApi)
    private readonly api: ReembolsoSolicitacaoApi,
  ) {}

  async listarProjetosColaborador(
    token: string,
    codigoProfissional: string
  ): Promise<{ sucesso: boolean; retorno?: ProjetoColaborador[]; mensagem?: string }> {
    return this.api.listarProjetosColaborador(token, codigoProfissional)
  }

  async listarVerbasComExcecao(
    token: string,
    codigoProjeto: string,
    codigoCliente: string
  ): Promise<{ sucesso: boolean; retorno?: Verba[]; mensagem?: string }> {
    return this.api.listarVerbasComExcecao(token, codigoProjeto, codigoCliente)
  }

  async obterVerbasEParametroValidacao(
    token: string
  ): Promise<{ sucesso: boolean; retorno?: VerbaEParametros; mensagem?: string }> {
    return this.api.obterVerbasEParametroValidacao(token)
  }

  async analisarComprovantesFiscais(
    token: string,
    arquivos: ArquivoAnalise[]
  ): Promise<{ sucesso: boolean; retorno?: ResultadoAnaliseComprovante; mensagem?: string }> {
    return this.api.analisarComprovantesFiscais(token, arquivos)
  }

  async inserirSolicitacoes(
    token: string,
    payload: PayloadInserirSolicitacao
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.api.inserirSolicitacoes(token, payload)
  }

  async inserirSolicitacoesZip(
    token: string,
    arquivoZip: File
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return this.api.inserirSolicitacoesZip(token, arquivoZip)
  }
}

