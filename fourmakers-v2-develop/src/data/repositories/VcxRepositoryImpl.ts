import { inject, injectable } from 'tsyringe'

import type {
  DorPayload,
  DorResponse,
  ImpactoResponse,
  UrgenciaResponse,
} from '@domain/entities/VcxDores'
import type {
  IniciativaPayload,
  IniciativaResponse,
  StatusIniciativaResponse,
  TemaResponse,
  TemaPayload,
} from '@domain/entities/VcxIniciativas'
import type { HistoricoDoresIniciativasResponse } from '@domain/entities/VcxHistorico'
import type { VcxAgendasPorColaboradorClienteResult } from '@domain/entities/VcxAgenda'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import { VcxApi } from '@data/api/VcxApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class VcxRepositoryImpl implements VcxRepository {
  constructor(
    @inject(DiTokens.vcxApi)
    private readonly api: VcxApi,
  ) {}

  async listarDoresPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<DorResponse[]> {
    return this.api.listarDoresPorPosicaoId(token, posicaoId)
  }

  async buscarDorPorId(token: string, id: string): Promise<DorResponse> {
    return this.api.buscarDorPorId(token, id)
  }

  async criarDor(token: string, payload: DorPayload): Promise<DorResponse> {
    return this.api.criarDor(token, payload)
  }

  async atualizarDor(
    token: string,
    payload: DorPayload & { id: string },
  ): Promise<DorResponse> {
    return this.api.atualizarDor(token, payload)
  }

  async excluirDor(token: string, id: string): Promise<void> {
    return this.api.excluirDor(token, id)
  }

  async listarImpactosDores(token: string): Promise<ImpactoResponse[]> {
    return this.api.listarImpactosDores(token)
  }

  async listarUrgenciasDores(token: string): Promise<UrgenciaResponse[]> {
    return this.api.listarUrgenciasDores(token)
  }

  // ============================================================================
  // IMPLEMENTAÇÕES DE MÉTODOS DE INICIATIVAS
  // ============================================================================

  async listarIniciativasPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<IniciativaResponse[]> {
    return this.api.listarIniciativasPorPosicaoId(token, posicaoId)
  }

  async buscarIniciativaPorId(
    token: string,
    id: string,
  ): Promise<IniciativaResponse> {
    return this.api.buscarIniciativaPorId(token, id)
  }

  async criarIniciativa(
    token: string,
    payload: IniciativaPayload,
  ): Promise<IniciativaResponse> {
    return this.api.criarIniciativa(token, payload)
  }

  async atualizarIniciativa(
    token: string,
    payload: IniciativaPayload & { id: string },
  ): Promise<IniciativaResponse> {
    return this.api.atualizarIniciativa(token, payload)
  }

  async excluirIniciativa(token: string, id: string): Promise<void> {
    return this.api.excluirIniciativa(token, id)
  }

  async listarStatusIniciativas(
    token: string,
  ): Promise<StatusIniciativaResponse[]> {
    return this.api.listarStatusIniciativas(token)
  }

  async listarTemas(token: string): Promise<TemaResponse[]> {
    return this.api.listarTemas(token)
  }

  async criarTema(token: string, payload: TemaPayload): Promise<TemaResponse> {
    return this.api.criarTema(token, payload)
  }

  // ============================================================================
  // IMPLEMENTAÇÕES DE MÉTODOS DE HISTÓRICO
  // ============================================================================

  async listarHistoricoDoresIniciativasPorPosicaoId(
    token: string,
    posicaoId: string,
  ): Promise<HistoricoDoresIniciativasResponse> {
    return this.api.listarHistoricoDoresIniciativasPorPosicaoId(token, posicaoId)
  }

  // ============================================================================
  // IMPLEMENTAÇÕES DE MÉTODOS DE AGENDA
  // ============================================================================

  async listarAgendasPorColaboradorCliente(
    token: string,
    codigoColaborador: string,
    codigoCliente: string,
    limit?: number,
    cursor?: number,
  ): Promise<VcxAgendasPorColaboradorClienteResult> {
    return this.api.listarAgendasPorColaboradorCliente(
      token,
      codigoColaborador,
      codigoCliente,
      limit,
      cursor,
    )
  }
}
