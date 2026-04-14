import { inject, injectable } from 'tsyringe'

import type { 
  ColaboradoresRepository,
  InserirColaboradorPayload,
  EditarColaboradorPayload,
  AlterarFormularioColaboradorPayload,
  ColaboradorResponse,
  RelatorioColaboradoresResult,
  CertificadoColaboradorResult,
} from '@domain/repositories/ColaboradoresRepository'
import type { ColaboradoresResponse, ColaboradoresParams } from '@domain/entities/Colaborador'
import type { ColaboradoresCchResponse, ListarColaboradoresOrgParams } from '@domain/entities/ColaboradorCch'
import type { DepartamentosResponse, ListarDepartamentosParams } from '@domain/entities/Departamento'
import type { DiretoriasResponse } from '@domain/entities/Diretoria'
import type { EmpresasRelacionadasResponse, ListarEmpresasRelacionadasParams } from '@domain/entities/EmpresaRelacionada'
import type { ModelosContratacaoResponse } from '@domain/entities/ModeloContratacao'
import type { CargosResponse } from '@domain/entities/Cargo'
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360'
import type { ListarEscolaridadeColaboradorResponse, ListarEscolaridadeColaboradorParams } from '@domain/entities/Escolaridade'
import type {
  ObterDadosColaboradorResponse,
  EditarDadosColaboradorPayload,
  EditarDadosColaboradorResponse,
} from '@domain/entities/ColaboradorDadosPessoais'
import type { AlterarDadosColaboradorParametrosPayload } from '@domain/entities/AlterarDadosColaboradorParametros'

import { ColaboradoresApi } from '@data/api/ColaboradoresApi'

import { DiTokens } from '@core/di/tokens'

@injectable()
export class ColaboradoresRepositoryImpl implements ColaboradoresRepository {
  constructor(
    @inject(DiTokens.colaboradoresApi)
    private readonly api: ColaboradoresApi,
  ) {}

  async getColaboradores(token: string, params: ColaboradoresParams): Promise<ColaboradoresResponse> {
    return this.api.getColaboradores(token, params)
  }

  async listarColaboradoresOrg(token: string, params: ListarColaboradoresOrgParams): Promise<ColaboradoresCchResponse> {
    return this.api.listarColaboradoresOrg(token, params)
  }

  async listarDepartamentos(token: string, params: ListarDepartamentosParams): Promise<DepartamentosResponse> {
    return this.api.listarDepartamentos(token, params)
  }

  async listarDiretorias(token: string): Promise<DiretoriasResponse> {
    return this.api.listarDiretorias(token)
  }

  async listarEmpresasRelacionadas(token: string, params: ListarEmpresasRelacionadasParams): Promise<EmpresasRelacionadasResponse> {
    return this.api.listarEmpresasRelacionadas(token, params)
  }

  async listarModelosContratacao(token: string): Promise<ModelosContratacaoResponse> {
    return this.api.listarModelosContratacao(token)
  }

  async listarCargos(token: string): Promise<CargosResponse> {
    return this.api.listarCargos(token)
  }

  async inserirColaborador(token: string, payload: InserirColaboradorPayload): Promise<ColaboradorResponse> {
    return this.api.inserirColaborador(token, payload)
  }

  async editarColaborador(token: string, payload: EditarColaboradorPayload): Promise<ColaboradorResponse> {
    return this.api.editarColaborador(token, payload)
  }

  async buscarDadosColaborador(token: string, cpf: string): Promise<BuscarDadosColaboradorResponse> {
    return this.api.buscarDadosColaborador(token, cpf)
  }

  async listarEscolaridadeColaborador(token: string, params: ListarEscolaridadeColaboradorParams): Promise<ListarEscolaridadeColaboradorResponse> {
    return this.api.listarEscolaridadeColaborador(token, params)
  }
  async gerarRelatorioColaboradores(token: string): Promise<RelatorioColaboradoresResult> {
    return this.api.gerarRelatorioColaboradores(token)
  }

  async baixarCertificadoColaborador(token: string, path: string): Promise<CertificadoColaboradorResult> {
    return this.api.baixarCertificadoColaborador(token, path)
  }

  async listarOrigensColaborador(token: string) {
    const res = await this.api.listarOrigensColaborador(token)
    return (res?.retorno ?? []) as import('@domain/entities/GestaoVagasCandidatos').OrigemColaboradorItem[]
  }

  async obterDadosColaborador(token: string, codigoInternoColaborador: string): Promise<ObterDadosColaboradorResponse> {
    return this.api.obterDadosColaborador(token, codigoInternoColaborador)
  }

  async editarDadosColaborador(
    token: string,
    payload: EditarDadosColaboradorPayload,
  ): Promise<EditarDadosColaboradorResponse> {
    return this.api.editarDadosColaborador(token, payload)
  }

  async alterarFormularioColaborador(
    token: string,
    payload: AlterarFormularioColaboradorPayload,
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return this.api.alterarFormularioColaborador(token, payload)
  }

  async alterarDadosColaboradorParametros(
    token: string,
    payload: AlterarDadosColaboradorParametrosPayload,
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return this.api.alterarDadosColaboradorParametros(token, payload)
  }
}
