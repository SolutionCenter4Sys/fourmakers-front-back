import { inject, injectable } from 'tsyringe'
import { DiTokens } from '@core/di/tokens'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'

export interface CriarGestorExternoPayload {
  codGestorExterno?: string
  nome: string
  email: string
  telefone: string
  codigoCliente?: string
  perfilLinkedin?: string
  areasDeAtuacao?: Array<{
    areaDeAtuacao: { descricao: string }
    permanencia: { id: string }
  }>
  preferenciasPessoais?: string
}

export interface CriarGestorExternoResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
}

@injectable()
export class CriarGestorExternoUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(token: string, payload: CriarGestorExternoPayload): Promise<CriarGestorExternoResponse> {
    return this.repository.inserirGestorExterno(token, payload)
  }
}
