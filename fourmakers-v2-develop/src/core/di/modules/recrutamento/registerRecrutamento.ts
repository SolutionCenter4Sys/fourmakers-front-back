/**
 * Registros de DI da jornada Recrutamento.
 * Novas APIs/repos desta jornada aqui; **use cases** em `registerRecrutamentoUseCases.ts`.
 */
import type { DependencyContainer } from 'tsyringe'

import type { CandidateRepository } from '@domain/repositories/CandidateRepository'
import type { CandidatoListRepository } from '@domain/repositories/CandidatoListRepository'
import type { CandidaturaRepository } from '@domain/repositories/CandidaturaRepository'
import type { ClientesGestaoAlocadosRepository } from '@domain/repositories/ClientesGestaoAlocadosRepository'
import type { ColaboradorBancoDeTalentosRepository } from '@domain/repositories/ColaboradorBancoDeTalentosRepository'
import type { CurriculoColaboradorRepository } from '@domain/repositories/CurriculoColaboradorRepository'
import type { DashboardMetricasFunilDeVagasRepository } from '@domain/repositories/DashboardMetricasFunilDeVagasRepository'
import type { DashboardMetricasRecrutamentoRepository } from '@domain/repositories/DashboardMetricasRecrutamentoRepository'
import type { DashboardMetricasVagasEmFocoRepository } from '@domain/repositories/DashboardMetricasVagasEmFocoRepository'
import type { DashboardMetricasVagasPerdidasMotivoRepository } from '@domain/repositories/DashboardMetricasVagasPerdidasMotivoRepository'
import type { DashboardNovosCandidatosPorOrigemRepository } from '@domain/repositories/DashboardNovosCandidatosPorOrigemRepository'
import type { GestorExternoPerfilRepository } from '@domain/repositories/GestorExternoPerfilRepository'
import type { HistoricoCandidaturaRepository } from '@domain/repositories/HistoricoCandidaturaRepository'
import type { LgpdRepository } from '@domain/repositories/LgpdRepository'
import type { MatchSemanticoRepository } from '@domain/repositories/MatchSemanticoRepository'
import type { ParametrizacaoNotificacaoCandidatosRepository } from '@domain/repositories/ParametrizacaoNotificacaoCandidatosRepository'
import type { PerfilAtuacaoRepository } from '@domain/repositories/PerfilAtuacaoRepository'
import type { RecruitmentRepository } from '@domain/repositories/RecruitmentRepository'
import type { RecrutadoresGestaoAlocadosRepository } from '@domain/repositories/RecrutadoresGestaoAlocadosRepository'
import type { RemuneracaoCalculationRepository } from '@domain/repositories/RemuneracaoCalculationRepository'
import type { StatusRecrutamentoRepository } from '@domain/repositories/StatusRecrutamentoRepository'
import type { TemplateContratacaoRepository } from '@domain/repositories/TemplateContratacaoRepository'
import type { VagaListRepository } from '@domain/repositories/VagaListRepository'
import type { VagaRepository } from '@domain/repositories/VagaRepository'
import type { VagasRepository } from '@domain/repositories/VagasRepository'
import { CandidateApi } from '@data/api/CandidateApi'
import { CandidateRepositoryImpl } from '@data/repositories/CandidateRepositoryImpl'
import { CandidatoListApi } from '@data/api/CandidatoListApi'
import { CandidatoListRepositoryImpl } from '@data/repositories/CandidatoListRepositoryImpl'
import { CandidaturaApi } from '@data/api/CandidaturaApi'
import { CandidaturaRepositoryImpl } from '@data/repositories/CandidaturaRepositoryImpl'
import { ClientesGestaoAlocadosApi } from '@data/api/ClientesGestaoAlocadosApi'
import { ClientesGestaoAlocadosRepositoryImpl } from '@data/repositories/ClientesGestaoAlocadosRepositoryImpl'
import { ColaboradorBancoDeTalentosApi } from '@data/api/ColaboradorBancoDeTalentosApi'
import { ColaboradorBancoDeTalentosRepositoryImpl } from '@data/repositories/ColaboradorBancoDeTalentosRepositoryImpl'
import { CurriculoColaboradorApi } from '@data/api/CurriculoColaboradorApi'
import { CurriculoColaboradorRepositoryImpl } from '@data/repositories/CurriculoColaboradorRepositoryImpl'
import { DashboardMetricasFunilDeVagasApi } from '@data/api/DashboardMetricasFunilDeVagasApi'
import { DashboardMetricasFunilDeVagasRepositoryImpl } from '@data/repositories/DashboardMetricasFunilDeVagasRepositoryImpl'
import { DashboardMetricasRecrutamentoApi } from '@data/api/DashboardMetricasRecrutamentoApi'
import { DashboardMetricasRecrutamentoRepositoryImpl } from '@data/repositories/DashboardMetricasRecrutamentoRepositoryImpl'
import { DashboardMetricasVagasEmFocoApi } from '@data/api/DashboardMetricasVagasEmFocoApi'
import { DashboardMetricasVagasEmFocoRepositoryImpl } from '@data/repositories/DashboardMetricasVagasEmFocoRepositoryImpl'
import { DashboardMetricasVagasPerdidasMotivoApi } from '@data/api/DashboardMetricasVagasPerdidasMotivoApi'
import { DashboardMetricasVagasPerdidasMotivoRepositoryImpl } from '@data/repositories/DashboardMetricasVagasPerdidasMotivoRepositoryImpl'
import { DashboardNovosCandidatosPorOrigemApi } from '@data/api/DashboardNovosCandidatosPorOrigemApi'
import { DashboardNovosCandidatosPorOrigemRepositoryImpl } from '@data/repositories/DashboardNovosCandidatosPorOrigemRepositoryImpl'
import { GestorExternoPerfilApi } from '@data/api/GestorExternoPerfilApi'
import { GestorExternoPerfilRepositoryImpl } from '@data/repositories/GestorExternoPerfilRepositoryImpl'
import { HistoricoCandidaturaApi } from '@data/api/HistoricoCandidaturaApi'
import { HistoricoCandidaturaRepositoryImpl } from '@data/repositories/HistoricoCandidaturaRepositoryImpl'
import { LgpdApi } from '@data/api/LgpdApi'
import { LgpdRepositoryImpl } from '@data/repositories/LgpdRepositoryImpl'
import { MatchSemanticoApi } from '@data/api/MatchSemanticoApi'
import { MatchSemanticoRepositoryImpl } from '@data/repositories/MatchSemanticoRepositoryImpl'
import { ParametrizacaoNotificacaoCandidatosApi } from '@data/api/ParametrizacaoNotificacaoCandidatosApi'
import { ParametrizacaoNotificacaoCandidatosRepositoryImpl } from '@data/repositories/ParametrizacaoNotificacaoCandidatosRepositoryImpl'
import { PerfilAtuacaoRepositoryImpl } from '@data/repositories/PerfilAtuacaoRepositoryImpl'
import { RecruitmentApi } from '@data/api/RecruitmentApi'
import { RecruitmentRepositoryImpl } from '@data/repositories/RecruitmentRepositoryImpl'
import { RecrutadoresGestaoAlocadosApi } from '@data/api/RecrutadoresGestaoAlocadosApi'
import { RecrutadoresGestaoAlocadosRepositoryImpl } from '@data/repositories/RecrutadoresGestaoAlocadosRepositoryImpl'
import { RemuneracaoCalculationApi } from '@data/api/RemuneracaoCalculationApi'
import { RemuneracaoCalculationRepositoryImpl } from '@data/repositories/RemuneracaoCalculationRepositoryImpl'
import { StatusRecrutamentoRepositoryImpl } from '@data/repositories/StatusRecrutamentoRepositoryImpl'
import { TemplateContratacaoRepositoryImpl } from '@data/repositories/TemplateContratacaoRepositoryImpl'
import { VagaApi } from '@data/api/VagaApi'
import { VagaEdicaoApi } from '@data/api/VagaEdicaoApi'
import { VagaListApi } from '@data/api/VagaListApi'
import { VagaListRepositoryImpl } from '@data/repositories/VagaListRepositoryImpl'
import { VagaRepositoryImpl } from '@data/repositories/VagaRepositoryImpl'
import { VagasApi } from '@data/api/VagasApi'
import { VagasRepositoryImpl } from '@data/repositories/VagasRepositoryImpl'

import { DiTokens } from '../../tokens'

export function registerRecrutamentoModule(c: DependencyContainer): void {
  if (!c.isRegistered(DiTokens.recruitmentApi)) {
    c.registerSingleton(DiTokens.recruitmentApi, RecruitmentApi)
  }
  if (!c.isRegistered(DiTokens.candidateApi)) {
    c.registerSingleton(DiTokens.candidateApi, CandidateApi)
  }
  if (!c.isRegistered(DiTokens.vagaApi)) {
    c.registerSingleton(DiTokens.vagaApi, VagaApi)
  }
  if (!c.isRegistered(DiTokens.vagaEdicaoApi)) {
    c.registerSingleton(DiTokens.vagaEdicaoApi, VagaEdicaoApi)
  }
  if (!c.isRegistered(DiTokens.vagaListApi)) {
    c.registerSingleton(DiTokens.vagaListApi, VagaListApi)
  }
  if (!c.isRegistered(DiTokens.candidatoListApi)) {
    c.registerSingleton(DiTokens.candidatoListApi, CandidatoListApi)
  }
  if (!c.isRegistered(DiTokens.candidaturaApi)) {
    c.registerSingleton(DiTokens.candidaturaApi, CandidaturaApi)
  }
  if (!c.isRegistered(DiTokens.historicoCandidaturaApi)) {
    c.registerSingleton(DiTokens.historicoCandidaturaApi, HistoricoCandidaturaApi)
  }
  if (!c.isRegistered(DiTokens.curriculoColaboradorApi)) {
    c.registerSingleton(DiTokens.curriculoColaboradorApi, CurriculoColaboradorApi)
  }
  if (!c.isRegistered(DiTokens.curriculoColaboradorRepository)) {
    c.registerSingleton<CurriculoColaboradorRepository>(
      DiTokens.curriculoColaboradorRepository,
      CurriculoColaboradorRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.colaboradorBancoDeTalentosApi)) {
    c.registerSingleton(DiTokens.colaboradorBancoDeTalentosApi, ColaboradorBancoDeTalentosApi)
  }
  if (!c.isRegistered(DiTokens.colaboradorBancoDeTalentosRepository)) {
    c.registerSingleton<ColaboradorBancoDeTalentosRepository>(
      DiTokens.colaboradorBancoDeTalentosRepository,
      ColaboradorBancoDeTalentosRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.matchSemanticoApi)) {
    c.registerSingleton(DiTokens.matchSemanticoApi, MatchSemanticoApi)
  }
  if (!c.isRegistered(DiTokens.matchSemanticoRepository)) {
    c.registerSingleton<MatchSemanticoRepository>(
      DiTokens.matchSemanticoRepository,
      MatchSemanticoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.statusRecrutamentoRepository)) {
    c.registerSingleton<StatusRecrutamentoRepository>(
      DiTokens.statusRecrutamentoRepository,
      StatusRecrutamentoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.perfilAtuacaoRepository)) {
    c.registerSingleton<PerfilAtuacaoRepository>(
      DiTokens.perfilAtuacaoRepository,
      PerfilAtuacaoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.remuneracaoCalculationApi)) {
    c.registerSingleton(DiTokens.remuneracaoCalculationApi, RemuneracaoCalculationApi)
  }

  if (!c.isRegistered(DiTokens.parametrizacaoNotificacaoCandidatosApi)) {
    c.registerSingleton(
      DiTokens.parametrizacaoNotificacaoCandidatosApi,
      ParametrizacaoNotificacaoCandidatosApi
    )
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasRecrutamentoApi)) {
    c.registerSingleton(DiTokens.dashboardMetricasRecrutamentoApi, DashboardMetricasRecrutamentoApi)
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasVagasEmFocoApi)) {
    c.registerSingleton(DiTokens.dashboardMetricasVagasEmFocoApi, DashboardMetricasVagasEmFocoApi)
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasFunilDeVagasApi)) {
    c.registerSingleton(DiTokens.dashboardMetricasFunilDeVagasApi, DashboardMetricasFunilDeVagasApi)
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasVagasPerdidasMotivoApi)) {
    c.registerSingleton(
      DiTokens.dashboardMetricasVagasPerdidasMotivoApi,
      DashboardMetricasVagasPerdidasMotivoApi
    )
  }
  if (!c.isRegistered(DiTokens.dashboardNovosCandidatosPorOrigemApi)) {
    c.registerSingleton(
      DiTokens.dashboardNovosCandidatosPorOrigemApi,
      DashboardNovosCandidatosPorOrigemApi
    )
  }
  if (!c.isRegistered(DiTokens.clientesGestaoAlocadosApi)) {
    c.registerSingleton(DiTokens.clientesGestaoAlocadosApi, ClientesGestaoAlocadosApi)
  }
  if (!c.isRegistered(DiTokens.recrutadoresGestaoAlocadosApi)) {
    c.registerSingleton(DiTokens.recrutadoresGestaoAlocadosApi, RecrutadoresGestaoAlocadosApi)
  }
  if (!c.isRegistered(DiTokens.vagasApi)) {
    c.registerSingleton(DiTokens.vagasApi, VagasApi)
  }
  if (!c.isRegistered(DiTokens.lgpdApi)) {
    c.registerSingleton(DiTokens.lgpdApi, LgpdApi)
  }

  if (!c.isRegistered(DiTokens.recruitmentRepository)) {
    c.registerSingleton<RecruitmentRepository>(
      DiTokens.recruitmentRepository,
      RecruitmentRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.candidateRepository)) {
    c.registerSingleton<CandidateRepository>(DiTokens.candidateRepository, CandidateRepositoryImpl)
  }
  if (!c.isRegistered(DiTokens.vagaRepository)) {
    c.registerSingleton<VagaRepository>(DiTokens.vagaRepository, VagaRepositoryImpl)
  }
  if (!c.isRegistered(DiTokens.vagaListRepository)) {
    c.registerSingleton<VagaListRepository>(DiTokens.vagaListRepository, VagaListRepositoryImpl)
  }
  if (!c.isRegistered(DiTokens.candidatoListRepository)) {
    c.registerSingleton<CandidatoListRepository>(
      DiTokens.candidatoListRepository,
      CandidatoListRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.candidaturaRepository)) {
    c.registerSingleton<CandidaturaRepository>(
      DiTokens.candidaturaRepository,
      CandidaturaRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.historicoCandidaturaRepository)) {
    c.registerSingleton<HistoricoCandidaturaRepository>(
      DiTokens.historicoCandidaturaRepository,
      HistoricoCandidaturaRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.templateContratacaoRepository)) {
    c.registerSingleton<TemplateContratacaoRepository>(
      DiTokens.templateContratacaoRepository,
      TemplateContratacaoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.remuneracaoCalculationRepository)) {
    c.registerSingleton<RemuneracaoCalculationRepository>(
      DiTokens.remuneracaoCalculationRepository,
      RemuneracaoCalculationRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.parametrizacaoNotificacaoCandidatosRepository)) {
    c.registerSingleton<ParametrizacaoNotificacaoCandidatosRepository>(
      DiTokens.parametrizacaoNotificacaoCandidatosRepository,
      ParametrizacaoNotificacaoCandidatosRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasRecrutamentoRepository)) {
    c.registerSingleton<DashboardMetricasRecrutamentoRepository>(
      DiTokens.dashboardMetricasRecrutamentoRepository,
      DashboardMetricasRecrutamentoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasVagasEmFocoRepository)) {
    c.registerSingleton<DashboardMetricasVagasEmFocoRepository>(
      DiTokens.dashboardMetricasVagasEmFocoRepository,
      DashboardMetricasVagasEmFocoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasFunilDeVagasRepository)) {
    c.registerSingleton<DashboardMetricasFunilDeVagasRepository>(
      DiTokens.dashboardMetricasFunilDeVagasRepository,
      DashboardMetricasFunilDeVagasRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.dashboardMetricasVagasPerdidasMotivoRepository)) {
    c.registerSingleton<DashboardMetricasVagasPerdidasMotivoRepository>(
      DiTokens.dashboardMetricasVagasPerdidasMotivoRepository,
      DashboardMetricasVagasPerdidasMotivoRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.dashboardNovosCandidatosPorOrigemRepository)) {
    c.registerSingleton<DashboardNovosCandidatosPorOrigemRepository>(
      DiTokens.dashboardNovosCandidatosPorOrigemRepository,
      DashboardNovosCandidatosPorOrigemRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.clientesGestaoAlocadosRepository)) {
    c.registerSingleton<ClientesGestaoAlocadosRepository>(
      DiTokens.clientesGestaoAlocadosRepository,
      ClientesGestaoAlocadosRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.recrutadoresGestaoAlocadosRepository)) {
    c.registerSingleton<RecrutadoresGestaoAlocadosRepository>(
      DiTokens.recrutadoresGestaoAlocadosRepository,
      RecrutadoresGestaoAlocadosRepositoryImpl
    )
  }
  if (!c.isRegistered(DiTokens.vagasRepository)) {
    c.registerSingleton<VagasRepository>(DiTokens.vagasRepository, VagasRepositoryImpl)
  }
  if (!c.isRegistered(DiTokens.lgpdRepository)) {
    c.registerSingleton<LgpdRepository>(DiTokens.lgpdRepository, LgpdRepositoryImpl)
  }

  if (!c.isRegistered(DiTokens.gestorExternoPerfilApi)) {
    c.registerSingleton(DiTokens.gestorExternoPerfilApi, GestorExternoPerfilApi)
  }
  if (!c.isRegistered(DiTokens.gestorExternoPerfilRepository)) {
    c.registerSingleton<GestorExternoPerfilRepository>(
      DiTokens.gestorExternoPerfilRepository,
      GestorExternoPerfilRepositoryImpl
    )
  }
}
