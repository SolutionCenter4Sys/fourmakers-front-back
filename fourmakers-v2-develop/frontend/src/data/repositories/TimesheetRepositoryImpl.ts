import { inject, injectable } from 'tsyringe'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { TimesheetComponentesApi } from '@data/api/TimesheetComponentesApi'
import { DiTokens } from '@core/di/tokens'

@injectable()
export class TimesheetRepositoryImpl implements TimesheetRepository {
  constructor(
    @inject(DiTokens.timesheetComponentesApi)
    private readonly api: TimesheetComponentesApi,
  ) {}

  async listarVigenciasColaborador(token: string, cpfColaborador: string) {
    return this.api.listarVigenciasColaborador(token, cpfColaborador)
  }

  async listarProjetosComAtividades(token: string, cpfColaborador?: string) {
    return this.api.listarProjetosComAtividades(token, cpfColaborador || '')
  }

  async listarTemplateSemanaVigencia(token: string, mes: number, ano: number) {
    return this.api.listarTemplateSemanaVigencia(token, mes, ano)
  }

  async listarApontamentosPorVigencia(
    token: string,
    mes: number,
    ano: number,
    cpfColaborador: string
  ) {
    return this.api.listarApontamentosPorVigencia(token, mes, ano, cpfColaborador)
  }

  async apontarHorasEmLote(token: string, payload: any) {
    const response = await this.api.apontarHorasEmLote(token, payload)
    return {
      sucesso: response.sucesso,
      mensagem: response.mensagem || undefined,
      erros: response.erros || undefined,
    }
  }

  async editarApontamento(token: string, payload: any) {
    const response = await this.api.editarApontamento(token, payload)
    return {
      sucesso: response.sucesso,
      mensagem: response.mensagem || undefined,
      erros: response.erros || undefined,
    }
  }

  async deletarApontamentoColaborador(token: string, apontamentoId: string, dataColetaDeDados: string) {
    const response = await this.api.deletarApontamentoColaborador(token, apontamentoId, dataColetaDeDados)
    return {
      sucesso: response.sucesso,
      mensagem: response.mensagem || undefined,
      erros: response.erros || undefined,
    }
  }

  async buscarPeriodoFechado(token: string) {
    const response = await this.api.buscarPeriodoFechado(token)
    if (!response) {
      throw new Error('Período fechado não encontrado')
    }
    return response
  }

  async listarGestoresApontamento(token: string) {
    return this.api.listarGestoresApontamento(token)
  }

  async listarProjetosGerenteDeProjeto(token: string) {
    return this.api.listarProjetosGerenteDeProjeto(token)
  }

  async listarGestoresProjeto(token: string, codigoDiretoria?: string, codigoDepartamento?: string, codigoGestorAdm?: string) {
    return this.api.listarGestoresProjeto(token, codigoDiretoria || '', codigoDepartamento || '', codigoGestorAdm || '')
  }

  async listarStatus(token: string) {
    return this.api.listarStatus(token)
  }

  async listarColaboradoresEApontamentosPorGestor(token: string, params: any) {
    return this.api.listarColaboradoresEApontamentosPorGestor(token, params)
  }

  async enviaEmailNotificacaoAprovadoresStatusPendentes(token: string, params: any) {
    return this.api.enviarEmailNotificacaoAprovadoresStatusPendentes(token, params)
  }

  async fecharAlterarPeriodo(token: string, dataFim: string) {
    return this.api.fecharAlterarPeriodo(token, dataFim)
  }

  async listarVigenciasApontamentosGerenteDeProjeto(token: string) {
    return this.api.listarVigenciasApontamentosGerenteDeProjeto(token)
  }

  async listarProjetosVisaoGerenteDeProjeto(token: string, params: any) {
    return this.api.listarProjetosVisaoGerenteDeProjeto(token, params)
  }

  async listarGerenteAdmDosColabsGP(token: string, mes: number, ano: number) {
    return this.api.listarGerenteAdmDosColabsGP(token, mes, ano)
  }

  async listarColaboradoresVinculadosGerenteDeProjeto(
    token: string,
    codProjeto: string,
    mes: number,
    ano: number
  ) {
    return this.api.listarColaboradoresVinculadosGerenteDeProjeto(token, codProjeto, mes, ano)
  }
}

