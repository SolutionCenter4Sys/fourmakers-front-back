import { inject, injectable } from 'tsyringe'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class BuscarPerfilCorporativoPorIdUseCase {
  constructor(
    @inject(DiTokens.organogramaRepository)
    private readonly repository: OrganogramaRepository,
  ) {}

  async execute(
    token: string,
    perfilCorpId: string,
  ): Promise<{
    sucesso: boolean
    mensagem?: string
    erros?: string[]
    retorno: import('@domain/entities/Organograma').PerfilCorporativoResponse & {
      codigoInternoColaboradorCriacao?: string
      codigoInternoColaboradorAlteracao?: string | null
      dataCriacao?: string
      dataAlteracao?: string
    }
  }> {
    return this.repository.buscarPerfilCorporativoPorId(token, perfilCorpId)
  }
}
