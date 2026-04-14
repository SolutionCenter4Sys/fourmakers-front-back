import { inject, injectable } from 'tsyringe'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import type { ListagemAcessosUsuarioResult } from '@domain/entities/TemplateContratacao'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class ListarDiretoriosSistemasGruposUseCase {
  constructor(
    @inject(DiTokens.templateContratacaoRepository)
    private readonly repository: TemplateContratacaoRepository
  ) {}

  async execute(token: string): Promise<ListagemAcessosUsuarioResult> {
    const [diretorios, sistemasLiberados, gruposEmails] = await Promise.all([
      this.repository.listarDiretorios(token),
      this.repository.listarSistemasLiberados(token),
      this.repository.listarGruposEmails(token),
    ])
    return { diretorios, sistemasLiberados, gruposEmails }
  }
}
