import { injectable, inject } from 'tsyringe'
import type { MinhaEquipeRepository } from '@domain/repositories/MinhaEquipeRepository'
import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'
import type {
  MinhaEquipeSugestao,
  AprovarRejeitarSugestaoPayload,
} from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import { MinhaEquipeApi } from '@data/api/MinhaEquipeApi'
import { DiTokens } from '@core/di/tokens'
import {
  mapColaboradoresFromApi,
  mapKPIsFromApi,
  mapSugestoesFromApi,
  mapPDIsFromApi,
} from '@data/mappers/minhaEquipeMappers'

@injectable()
export class MinhaEquipeRepositoryImpl implements MinhaEquipeRepository {
  constructor(
    @inject(DiTokens.minhaEquipeApi)
    private readonly api: MinhaEquipeApi
  ) {}

  async listarIndicadoresLiderados(params: {
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
    cursor: number
    limite: number
  }): Promise<MinhaEquipeColaborador[]> {
    const response = await this.api.listarIndicadoresLiderados(params)
    return mapColaboradoresFromApi(response)
  }

  async buscarTotalizacaoIndicadores(params: {
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
  }): Promise<MinhaEquipeKPIs> {
    const response = await this.api.buscarTotalizacaoIndicadores(params)
    return mapKPIsFromApi(response)
  }

  async buscarPDIColaborador(params: {
    token: string
    uuid_colab: string
    uuid_manager: string
  }): Promise<MinhaEquipePDI[]> {
    const response = await this.api.buscarPDIColaborador(params)
    return mapPDIsFromApi(response)
  }

  async buscarSugestoesColaborador(params: {
    token: string
    codInternoGestor: string
    perfilId: string
    codInternoColaborador: string
  }): Promise<MinhaEquipeSugestao[]> {
    const response = await this.api.buscarSugestoesColaborador(params)
    return mapSugestoesFromApi(response)
  }

  async aprovarRejeitarSugestao(
    token: string,
    payload: AprovarRejeitarSugestaoPayload
  ): Promise<void> {
    await this.api.aprovarRejeitarSugestao(token, payload)
  }
}
