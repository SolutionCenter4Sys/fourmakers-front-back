import { httpClient } from './httpClient'

export interface EnviarDenunciaPayload {
  mensagem: string
  email: string
}

export interface EnviarDenunciaResponse {
  sucesso: boolean
  mensagem?: string
}

export class CanalDenunciaApi {
  /**
   * Envia denúncia via canal de denúncias
   */
  async enviarDenuncia(
    token: string,
    payload: EnviarDenunciaPayload
  ): Promise<EnviarDenunciaResponse> {
    return httpClient.post<EnviarDenunciaResponse>(
      '/api/Usuario/EnviarDenunciaViaCanalDenuncia',
      payload,
      { 
        token,
        headers: {
          accept: '*/*',
        }
      }
    )
  }
}

