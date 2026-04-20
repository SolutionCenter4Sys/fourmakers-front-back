import type { RecrutadorGestaoAlocados, RecrutadoresGestaoAlocadosResponse } from '@domain/entities/RecrutadorGestaoAlocados'
import { httpClient } from './httpClient'

/** Resposta da API GET api/Candidatura/RecrutadorListagem */
interface RecrutadorListagemItem {
  codigoRecrutador: string
  nomeRecrutador: string
}

interface RecrutadorListagemResponse {
  retorno?: RecrutadorListagemItem[] | null
  sucesso?: boolean
  mensagem?: string | null
  erros?: unknown | null
}

export class RecrutadoresGestaoAlocadosApi {
  private readonly recrutadoresUrl = '/api/Candidatura/RecrutadorListagem'

  async listarRecrutadores(
    token: string,
    busca: string,
  ): Promise<RecrutadoresGestaoAlocadosResponse> {
    const queryParams = new URLSearchParams()
    queryParams.set('cursor', '0')
    queryParams.set('limite', '100')

    const response = await httpClient.get<RecrutadorListagemResponse>(
      `${this.recrutadoresUrl}?${queryParams.toString()}`,
      { token },
    )

    let lista = response?.retorno ?? []
    if (busca.trim()) {
      const termo = busca.trim().toLowerCase()
      lista = lista.filter(
        (item) =>
          item.nomeRecrutador?.toLowerCase().includes(termo) ||
          item.codigoRecrutador?.toLowerCase().includes(termo),
      )
    }
    const recrutadores: RecrutadorGestaoAlocados[] = lista.map((item) => ({
      codigoRecrutador: item.codigoRecrutador,
      codigoInternoColaborador: item.codigoRecrutador,
      nome: item.nomeRecrutador,
    }))

    return {
      retorno: recrutadores,
      sucesso: response?.sucesso ?? true,
      mensagem: response?.mensagem ?? null,
      erros: response?.erros ?? null,
    }
  }
}
