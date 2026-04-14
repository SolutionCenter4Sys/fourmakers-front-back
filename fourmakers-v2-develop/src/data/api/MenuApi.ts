import type { MenuResource } from '@domain/entities/MenuResource'

import { httpClient } from './httpClient'

export class MenuApi {
  async getMenuResources(token: string): Promise<MenuResource[]> {
    const data = await httpClient.get<{ sucesso: boolean; retorno?: MenuResource[]; mensagem?: string }>(
      '/api/Usuario/GestaoDeAcesso/Recurso/ListarRecursosVisaoMenu',
      { token }
    )

    if (!data?.sucesso || !Array.isArray(data.retorno)) {
      throw new Error(data?.mensagem || 'Formato de retorno do menu inválido.')
    }

    // Ordenar os itens de menu por 'ordenacao'
    return data.retorno.sort((a: MenuResource, b: MenuResource) => a.ordenacao - b.ordenacao)
  }
}

