import { MsalService } from '@core/auth/MsalService'

import type {
  CriarEventoTeamsParams,
  ResultadoEventoTeams,
} from '@domain/repositories/GraphApiRepository'

/** Resposta da Microsoft Graph API ao criar evento. */
interface GraphEventResponse {
  id: string
  onlineMeeting?: {
    joinUrl?: string
  }
}

/**
 * API para criação de reuniões Microsoft Teams via Microsoft Graph API.
 * Endpoint: POST https://graph.microsoft.com/v1.0/me/events
 * 
 * Utiliza MSAL para obter token Microsoft e chamar Graph API diretamente.
 */
export class MicrosoftGraphApi {
  private readonly msalService = MsalService.getInstance()
  private readonly graphBaseUrl = 'https://graph.microsoft.com/v1.0'

  async criarEventoTeams(
    params: CriarEventoTeamsParams,
    fallbackToken?: string | null,
  ): Promise<ResultadoEventoTeams> {
    const token = await this.msalService.getAccessToken(fallbackToken)

    const payload = {
      subject: params.titulo,
      start: {
        dateTime: params.dataInicioIso,
        timeZone: 'UTC',
      },
      end: {
        dateTime: params.dataFimIso,
        timeZone: 'UTC',
      },
      isOnlineMeeting: true,
      onlineMeetingProvider: 'teamsForBusiness',
      attendees: params.participantes.map((p) => ({
        emailAddress: {
          address: p.email,
          name: p.nome,
        },
        type: 'required',
      })),
    }

    const response = await fetch(`${this.graphBaseUrl}/me/events`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(
        `Erro ao criar reunião Teams: ${response.status} - ${errorText}`,
      )
    }

    const data = (await response.json()) as GraphEventResponse

    const id = data?.id
    const joinUrl = data?.onlineMeeting?.joinUrl

    if (!id || !joinUrl) {
      throw new Error(
        'Resposta inválida ao criar reunião Teams: id e joinUrl são obrigatórios',
      )
    }

    return { id, joinUrl }
  }
}

