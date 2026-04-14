import { httpClient } from './httpClient'
import type {
  BuscarParceirosParams,
  BuscarParceirosResponse,
  InserirParceiroPayload,
  AtualizarParceiroPayload,
  ParceiroResponse,
  DeletarParceiroParams,
  DeletarParceiroResponse,
} from '@domain/entities/Parceiro'
import type {
  InserirContratoPayload,
  AtualizarContratoPayload,
  DeletarContratoParams,
  ContratoResponse,
  DeletarContratoResponse,
} from '@domain/entities/Contrato'
import type {
  InserirArquivoParams,
  InserirArquivoResponse,
} from '@domain/entities/ArquivoParceiro'
import type { RelatorioParceriaResponse } from '@domain/entities/RelatorioParceria'
import type { ListarUnidadesResponse } from '@domain/entities/NotaFiscalGestao'
import { API_BASE_URL } from '@shared/constants'

export class ParceriaApi {
  // GET - Buscar Todos os Parceiros
  async buscarTodosParceiros(
    token: string,
    params: BuscarParceirosParams
  ): Promise<BuscarParceirosResponse | any> {
    const queryParams = new URLSearchParams({
      orgId: params.orgId.toString(),
    })

    if (params.filtro) {
      queryParams.append('filtro', params.filtro)
    }
    if (params.bucket) {
      queryParams.append('bucket', params.bucket)
    }
    if (params.cursor !== null && params.cursor !== undefined) {
      queryParams.append('cursor', params.cursor)
    }
    if (params.limite !== null && params.limite !== undefined) {
      queryParams.append('limite', params.limite.toString())
    }

    // Retornar qualquer formato (será mapeado no repositório)
    return httpClient.get<any>(
      `/api/GestaoDeAlocados/Parceiros/BuscarTodosParceiros?${queryParams.toString()}`,
      { token }
    )
  }

  // POST - Inserir Parceiro
  async inserirParceiro(
    token: string,
    payload: InserirParceiroPayload
  ): Promise<ParceiroResponse> {
    return httpClient.post<ParceiroResponse>(
      '/api/GestaoDeAlocados/Parceiros/InserirParceiro',
      payload,
      { token }
    )
  }

  // PUT - Atualizar Parceiro
  async atualizarParceiro(
    token: string,
    payload: AtualizarParceiroPayload
  ): Promise<ParceiroResponse> {
    const { id, ...restPayload } = payload
    const parceiroIdParam = encodeURIComponent(id)
    return httpClient.put<ParceiroResponse>(
      `/api/GestaoDeAlocados/Parceiros/AtualizarParceiro?parceiroID=${parceiroIdParam}`,
      restPayload,
      { token }
    )
  }

  // DELETE - Deletar Parceiro
  async deletarParceiro(
    token: string,
    params: DeletarParceiroParams
  ): Promise<DeletarParceiroResponse> {
    const parceiroIdParam = encodeURIComponent(params.parceiroId)
    return httpClient.delete<DeletarParceiroResponse>(
      `/api/GestaoDeAlocados/Parceiros/DeletarParceiro?parceiroID=${parceiroIdParam}`,
      { token }
    )
  }

  // POST - Inserir Contrato
  async inserirContrato(
    token: string,
    payload: InserirContratoPayload
  ): Promise<ContratoResponse> {
    return httpClient.post<ContratoResponse>(
      '/api/GestaoDeAlocados/Parceiros/InserirContrato',
      payload,
      { token }
    )
  }

  // PUT - Atualizar Contrato
  async atualizarContrato(
    token: string,
    payload: AtualizarContratoPayload
  ): Promise<ContratoResponse> {
    return httpClient.put<ContratoResponse>(
      '/api/GestaoDeAlocados/Parceiros/AtualizarContrato',
      payload,
      { token }
    )
  }

  // DELETE - Deletar Contrato
  async deletarContrato(
    token: string,
    params: DeletarContratoParams
  ): Promise<DeletarContratoResponse> {
    return httpClient.delete<DeletarContratoResponse>(
      `/api/GestaoDeAlocados/Parceiros/DeletarContrato?id=${params.id}`,
      { token }
    )
  }

  // POST - Upload de Arquivo (multipart/form-data)
  // ⚠️ EXCEÇÃO: Usa fetch diretamente porque FormData requer boundary automático do browser
  async inserirArquivo(
    token: string,
    params: InserirArquivoParams
  ): Promise<InserirArquivoResponse> {
    const formData = new FormData()

    // Metadata JSON
    const metadata = {
      ParceiroID: params.parceiroId,
      ArquivoTipoID: params.arquivoTipoId,
      ArquivoOriginID: params.arquivoOriginId,
      parceiroGestaoContratoId: params.parceiroGestaoContratoId,
    }

    formData.append('parceirosArquivoParam', JSON.stringify(metadata))
    formData.append('files', params.file)

    const url = `${API_BASE_URL}/api/GestaoDeAlocados/Parceiros/InserirArquivo`

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        // NÃO definir Content-Type manualmente - o browser define automaticamente com boundary
      },
      body: formData,
    })

    if (!response.ok) {
      throw new Error(`Falha ao fazer upload do arquivo: ${response.status}`)
    }

    return response.json()
  }

  // GET - Gerar Relatório
  // ⚠️ EXCEÇÃO: Usa fetch diretamente para download de binários com headers especiais
  async gerarRelatorioParceria(
    token: string
  ): Promise<RelatorioParceriaResponse> {
    const url = `${API_BASE_URL}/api/GestaoDeAlocados/Parceiros/RelatorioParceriaAliancas`

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        Accept: '*/*',
        Authorization: `Bearer ${token}`,
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
      },
    })

    if (!response.ok) {
      throw new Error(`Falha ao gerar relatório: ${response.status}`)
    }

    const contentType =
      response.headers.get('Content-Type') || 'application/octet-stream'
    const contentDisposition = response.headers.get('Content-Disposition') || ''
    
    let fileName = 'relatorio_parceria_aliancas.xlsx'

    // Extração do nome do arquivo
    if (contentDisposition) {
      const filenameStarMatch = contentDisposition.match(
        /filename\*=UTF-8''([^;]+?)(?:;|$)/i
      )
      if (filenameStarMatch && filenameStarMatch[1]) {
        try {
          fileName = decodeURIComponent(filenameStarMatch[1].trim())
        } catch {
          fileName = filenameStarMatch[1].trim()
        }
      } else {
        const filenameMatch = contentDisposition.match(
          /filename[^;=\n]*=((['"]).*?\2|[^;\n]+?)(?:;|$)/i
        )
        if (filenameMatch && filenameMatch[1]) {
          fileName = filenameMatch[1].replace(/['"]/g, '').trim()
        }
      }
    }

    const arrayBuffer = await response.arrayBuffer()
    return {
      arrayBuffer,
      contentType,
      fileName,
    }
  }

  // GET - Listar Unidades
  async listarUnidades(token: string): Promise<ListarUnidadesResponse> {
    return httpClient.get<ListarUnidadesResponse>(
      '/api/Vaga/ListarUnidadesSRS',
      { token }
    )
  }
}
