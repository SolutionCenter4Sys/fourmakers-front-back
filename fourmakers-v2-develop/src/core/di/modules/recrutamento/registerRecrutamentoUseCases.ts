/**
 * Use cases da jornada Recrutamento. Executar após registerRecrutamentoModule e demais deps globais do container.
 */
import type { DependencyContainer } from 'tsyringe'

import { AdicionarRecrutadorVagaUseCase } from '@domain/usecases/AdicionarRecrutadorVagaUseCase'
import { AlterarFormularioColaboradorUseCase } from '@domain/usecases/AlterarFormularioColaboradorUseCase'
import { AtualizarCandidaturaUseCase } from '@domain/usecases/AtualizarCandidaturaUseCase'
import { AtualizarParametrizacaoNotificacaoCandidatosUseCase } from '@domain/usecases/AtualizarParametrizacaoNotificacaoCandidatosUseCase'
import { AtualizarPerfilCorporativoUseCase } from '@domain/usecases/AtualizarPerfilCorporativoUseCase'
import { AtualizarRecrutadorResponsavelUseCase } from '@domain/usecases/AtualizarRecrutadorResponsavelUseCase'
import { AtualizarVagaRecrutamentoPorIdUseCase } from '@domain/usecases/AtualizarVagaRecrutamentoPorIdUseCase'
import { BuscarBancoTalentosComPromptMatchUseCase } from '@domain/usecases/BuscarBancoTalentosComPromptMatchUseCase'
import { BuscarBancoTalentosUseCase } from '@domain/usecases/BuscarBancoTalentosUseCase'
import { BuscarCandidaturasColaboradorUseCase } from '@domain/usecases/BuscarCandidaturasColaboradorUseCase'
import { BuscarInformacoesLoteUseCase } from '@domain/usecases/BuscarInformacoesLoteUseCase'
import { BuscarMelhoresCandidatosMatchSemanticoUseCase } from '@domain/usecases/BuscarMelhoresCandidatosMatchSemanticoUseCase'
import { BuscarMeusLotesUseCase } from '@domain/usecases/BuscarMeusLotesUseCase'
import { BuscarPerfilCorporativoPorIdUseCase } from '@domain/usecases/BuscarPerfilCorporativoPorIdUseCase'
import { BuscarPerfisPorOrgUseCase } from '@domain/usecases/BuscarPerfisPorOrgUseCase'
import { BuscarPessoasCadastradasPorColaboradorUseCase } from '@domain/usecases/BuscarPessoasCadastradasPorColaboradorUseCase'
import { BuscarPessoasCadastradasPorOrgUseCase } from '@domain/usecases/BuscarPessoasCadastradasPorOrgUseCase'
import { CandidatarOutraPessoaUseCase } from '@domain/usecases/CandidatarOutraPessoaUseCase'
import { CandidatarSeUseCase } from '@domain/usecases/CandidatarSeUseCase'
import { CriarPerfilCorporativoUseCase } from '@domain/usecases/CriarPerfilCorporativoUseCase'
import { DeclinarCandidatoUseCase } from '@domain/usecases/DeclinarCandidatoUseCase'
import { DeletarParametrizacaoNotificacaoCandidatosUseCase } from '@domain/usecases/DeletarParametrizacaoNotificacaoCandidatosUseCase'
import { DeletarPerfilCorporativoUseCase } from '@domain/usecases/DeletarPerfilCorporativoUseCase'
import { GerarRelatorioProdutividadeUseCase } from '@domain/usecases/GerarRelatorioProdutividadeUseCase'
import { GerarRelatorioVagasCandidaturasUseCase } from '@domain/usecases/GerarRelatorioVagasCandidaturasUseCase'
import { GerarRelatorioVagasUseCase } from '@domain/usecases/GerarRelatorioVagasUseCase'
import { GetCandidateDetailsUseCase } from '@domain/usecases/GetCandidateDetailsUseCase'
import { GetCandidaturaDetailsUseCase } from '@domain/usecases/GetCandidaturaDetailsUseCase'
import { GetVagaDetalhesPublicoUseCase } from '@domain/usecases/GetVagaDetalhesPublicoUseCase'
import { GetVagaDetalhesUseCase } from '@domain/usecases/GetVagaDetalhesUseCase'
import { GravarPerdaVagaUseCase } from '@domain/usecases/GravarPerdaVagaUseCase'
import { ImportarColaboradorLinkedinUseCase } from '@domain/usecases/ImportarColaboradorLinkedinUseCase'
import { ImportarColaboradorLotePlanilhaUseCase } from '@domain/usecases/ImportarColaboradorLotePlanilhaUseCase'
import { ImportarColaboradorLoteUseCase } from '@domain/usecases/ImportarColaboradorLoteUseCase'
import { ImportarColaboradorUseCase } from '@domain/usecases/ImportarColaboradorUseCase'
import { InserirArquivoCandidaturaUseCase } from '@domain/usecases/InserirArquivoCandidaturaUseCase'
import { InserirInformacoesComplementaresVagaUseCase } from '@domain/usecases/InserirInformacoesComplementaresVagaUseCase'
import { ListCandidatosUseCase } from '@domain/usecases/ListCandidatosUseCase'
import { ListVagasUseCase } from '@domain/usecases/ListVagasUseCase'
import { ListarCandidatosAderentesUseCase } from '@domain/usecases/ListarCandidatosAderentesUseCase'
import { ListarCandidatosInscritosUseCase } from '@domain/usecases/ListarCandidatosInscritosUseCase'
import { ListarClientesGestaoAlocadosUseCase } from '@domain/usecases/ListarClientesGestaoAlocadosUseCase'
import { ListarGestoresExternosUseCase } from '@domain/usecases/ListarGestoresExternosUseCase'
import { ListarGestoresUseCase } from '@domain/usecases/ListarGestoresUseCase'
import { ListarLocalidadesUseCase } from '@domain/usecases/ListarLocalidadesUseCase'
import { ListarMeusTalentosUseCase } from '@domain/usecases/ListarMeusTalentosUseCase'
import { ListarModelosTrabalhoUseCase } from '@domain/usecases/ListarModelosTrabalhoUseCase'
import { ListarMotivosDeclinioUseCase } from '@domain/usecases/ListarMotivosDeclinioUseCase'
import { ListarMotivosPerdaVagaUseCase } from '@domain/usecases/ListarMotivosPerdaVagaUseCase'
import { ListarMotivosReprovacaoUseCase } from '@domain/usecases/ListarMotivosReprovacaoUseCase'
import { ListarNiveisExperienciaUseCase } from '@domain/usecases/ListarNiveisExperienciaUseCase'
import { ListarOpcoesContatoUseCase } from '@domain/usecases/ListarOpcoesContatoUseCase'
import { ListarOrigensColaboradorUseCase } from '@domain/usecases/ListarOrigensColaboradorUseCase'
import { ListarPerfisExternosUseCase } from '@domain/usecases/ListarPerfisExternosUseCase'
import { ListarPermanenciasUseCase } from '@domain/usecases/ListarPermanenciasUseCase'
import { ListarRecrutadoresGestaoAlocadosUseCase } from '@domain/usecases/ListarRecrutadoresGestaoAlocadosUseCase'
import { ListarSkillsPerfilGestorExternoPorIdUseCase } from '@domain/usecases/ListarSkillsPerfilGestorExternoPorIdUseCase'
import { ListarStatusCandidaturaRecrutamentoUseCase } from '@domain/usecases/ListarStatusCandidaturaRecrutamentoUseCase'
import { ListarStatusVagaRecrutamentoUseCase } from '@domain/usecases/ListarStatusVagaRecrutamentoUseCase'
import { ListarStatusVagasUseCase } from '@domain/usecases/ListarStatusVagasUseCase'
import { ListarTiposContratacaoVagaUseCase } from '@domain/usecases/ListarTiposContratacaoVagaUseCase'
import { ListarTiposEmpregoUseCase } from '@domain/usecases/ListarTiposEmpregoUseCase'
import { ListarTiposVagaUseCase } from '@domain/usecases/ListarTiposVagaUseCase'
import { ListarUnidadesVagaUseCase } from '@domain/usecases/ListarUnidadesVagaUseCase'
import { ListarVagasRecrutamentoEPerfisUseCase } from '@domain/usecases/ListarVagasRecrutamentoEPerfisUseCase'
import { ListarVagasRecrutamentoPorParentEmAndamentoUseCase } from '@domain/usecases/ListarVagasRecrutamentoPorParentEmAndamentoUseCase'
import { MudarStatusCandidaturaUseCase } from '@domain/usecases/MudarStatusCandidaturaUseCase'
import { MudarStatusVagaUseCase } from '@domain/usecases/MudarStatusVagaUseCase'
import { ObterDashboardMetricasFunilDeVagasUseCase } from '@domain/usecases/ObterDashboardMetricasFunilDeVagasUseCase'
import { ObterDashboardMetricasRecrutamentoUseCase } from '@domain/usecases/ObterDashboardMetricasRecrutamentoUseCase'
import { ObterDashboardMetricasVagasEmFocoUseCase } from '@domain/usecases/ObterDashboardMetricasVagasEmFocoUseCase'
import { ObterDashboardMetricasVagasPerdidasMotivoUseCase } from '@domain/usecases/ObterDashboardMetricasVagasPerdidasMotivoUseCase'
import { ObterDashboardNovosCandidatosPorOrigemUseCase } from '@domain/usecases/ObterDashboardNovosCandidatosPorOrigemUseCase'
import { ObterHistoricoCandidaturaUseCase } from '@domain/usecases/ObterHistoricoCandidaturaUseCase'
import { ObterParametrizacaoNotificacaoCandidatosPorIdUseCase } from '@domain/usecases/ObterParametrizacaoNotificacaoCandidatosPorIdUseCase'
import { ObterParametrizacaoNotificacaoCandidatosUseCase } from '@domain/usecases/ObterParametrizacaoNotificacaoCandidatosUseCase'
import { ObterTemplateContratacaoPorCandidaturaUseCase } from '@domain/usecases/ObterTemplateContratacaoPorCandidaturaUseCase'
import { ObterTotaisInscritosUseCase } from '@domain/usecases/ObterTotaisInscritosUseCase'
import { ObterVagaRecrutamentoPorIdUseCase } from '@domain/usecases/ObterVagaRecrutamentoPorIdUseCase'
import { RegistrarFeedbackMatchSemanticoUseCase } from '@domain/usecases/RegistrarFeedbackMatchSemanticoUseCase'
import { RegistrarLgpdUseCase } from '@domain/usecases/RegistrarLgpdUseCase'
import { ReprovarCandidaturaUseCase } from '@domain/usecases/ReprovarCandidaturaUseCase'
import { SalvarEBaixarTemplateUseCase } from '@domain/usecases/SalvarEBaixarTemplateUseCase'
import { SalvarParametrizacaoNotificacaoCandidatosUseCase } from '@domain/usecases/SalvarParametrizacaoNotificacaoCandidatosUseCase'
import { SimulateRemuneracaoTotalUseCase } from '@domain/usecases/SimulateRemuneracaoTotalUseCase'
import { SincronizarCurriculoColaboradorUseCase } from '@domain/usecases/SincronizarCurriculoColaboradorUseCase'

import { DiTokens } from '../../tokens'

// eslint-disable-next-line @typescript-eslint/no-explicit-any -- alinhado ao padrão container.registerSingleton(Cls, Cls)
function reg(c: DependencyContainer, UseCase: any): void {
  if (!c.isRegistered(UseCase)) {
    c.registerSingleton(UseCase, UseCase)
  }
}

export function registerRecrutamentoUseCases(c: DependencyContainer): void {
  if (!c.isRegistered(DiTokens.listarVagasRecrutamentoEPerfisUseCase)) {
    c.registerSingleton(
      DiTokens.listarVagasRecrutamentoEPerfisUseCase,
      ListarVagasRecrutamentoEPerfisUseCase
    )
  }
  if (!c.isRegistered(DiTokens.listarStatusVagasUseCase)) {
    c.registerSingleton(DiTokens.listarStatusVagasUseCase, ListarStatusVagasUseCase)
  }

  reg(c, ObterDashboardMetricasRecrutamentoUseCase)
  reg(c, ObterDashboardMetricasVagasEmFocoUseCase)
  reg(c, ObterDashboardMetricasFunilDeVagasUseCase)
  reg(c, ObterDashboardMetricasVagasPerdidasMotivoUseCase)
  reg(c, ObterDashboardNovosCandidatosPorOrigemUseCase)
  reg(c, ObterHistoricoCandidaturaUseCase)
  reg(c, ObterTemplateContratacaoPorCandidaturaUseCase)
  reg(c, SalvarEBaixarTemplateUseCase)
  reg(c, ListarClientesGestaoAlocadosUseCase)
  reg(c, ListarRecrutadoresGestaoAlocadosUseCase)
  reg(c, GetCandidateDetailsUseCase)
  reg(c, GetVagaDetalhesUseCase)
  reg(c, GetVagaDetalhesPublicoUseCase)
  reg(c, CandidatarSeUseCase)
  reg(c, AlterarFormularioColaboradorUseCase)
  reg(c, RegistrarLgpdUseCase)
  reg(c, ListVagasUseCase)
  reg(c, ListCandidatosUseCase)
  reg(c, GetCandidaturaDetailsUseCase)
  reg(c, ListarModelosTrabalhoUseCase)
  reg(c, ListarPermanenciasUseCase)
  reg(c, ListarLocalidadesUseCase)
  reg(c, ListarTiposEmpregoUseCase)
  reg(c, ListarNiveisExperienciaUseCase)
  reg(c, BuscarBancoTalentosUseCase)
  reg(c, BuscarBancoTalentosComPromptMatchUseCase)
  reg(c, BuscarMelhoresCandidatosMatchSemanticoUseCase)
  reg(c, RegistrarFeedbackMatchSemanticoUseCase)
  reg(c, ListarMeusTalentosUseCase)
  reg(c, BuscarInformacoesLoteUseCase)
  reg(c, BuscarPessoasCadastradasPorColaboradorUseCase)
  reg(c, BuscarMeusLotesUseCase)
  reg(c, MudarStatusCandidaturaUseCase)
  reg(c, ListarMotivosReprovacaoUseCase)
  reg(c, ReprovarCandidaturaUseCase)
  reg(c, ListarMotivosDeclinioUseCase)
  reg(c, DeclinarCandidatoUseCase)
  reg(c, AtualizarCandidaturaUseCase)
  reg(c, AtualizarRecrutadorResponsavelUseCase)
  reg(c, InserirArquivoCandidaturaUseCase)
  reg(c, ListarStatusCandidaturaRecrutamentoUseCase)
  reg(c, ListarStatusVagaRecrutamentoUseCase)
  reg(c, BuscarPessoasCadastradasPorOrgUseCase)
  reg(c, BuscarCandidaturasColaboradorUseCase)
  reg(c, ObterTotaisInscritosUseCase)
  reg(c, ListarCandidatosInscritosUseCase)
  reg(c, ListarCandidatosAderentesUseCase)
  reg(c, ListarUnidadesVagaUseCase)
  reg(c, ListarTiposVagaUseCase)
  reg(c, ListarTiposContratacaoVagaUseCase)
  reg(c, CandidatarOutraPessoaUseCase)
  reg(c, ListarOrigensColaboradorUseCase)
  reg(c, AdicionarRecrutadorVagaUseCase)
  reg(c, ListarMotivosPerdaVagaUseCase)
  reg(c, GravarPerdaVagaUseCase)
  reg(c, MudarStatusVagaUseCase)
  reg(c, ListarOpcoesContatoUseCase)
  reg(c, ListarVagasRecrutamentoPorParentEmAndamentoUseCase)
  reg(c, InserirInformacoesComplementaresVagaUseCase)
  reg(c, GerarRelatorioProdutividadeUseCase)
  reg(c, GerarRelatorioVagasUseCase)
  reg(c, GerarRelatorioVagasCandidaturasUseCase)
  reg(c, ListarGestoresUseCase)
  reg(c, ObterVagaRecrutamentoPorIdUseCase)
  reg(c, AtualizarVagaRecrutamentoPorIdUseCase)
  reg(c, ImportarColaboradorUseCase)
  reg(c, ImportarColaboradorLinkedinUseCase)
  reg(c, ImportarColaboradorLoteUseCase)
  reg(c, ImportarColaboradorLotePlanilhaUseCase)
  reg(c, SincronizarCurriculoColaboradorUseCase)
  reg(c, SimulateRemuneracaoTotalUseCase)

  reg(c, ObterParametrizacaoNotificacaoCandidatosUseCase)
  reg(c, ObterParametrizacaoNotificacaoCandidatosPorIdUseCase)
  reg(c, SalvarParametrizacaoNotificacaoCandidatosUseCase)
  reg(c, AtualizarParametrizacaoNotificacaoCandidatosUseCase)
  reg(c, DeletarParametrizacaoNotificacaoCandidatosUseCase)

  reg(c, ListarSkillsPerfilGestorExternoPorIdUseCase)
  reg(c, ListarGestoresExternosUseCase)
  reg(c, ListarPerfisExternosUseCase)
  reg(c, CriarPerfilCorporativoUseCase)
  reg(c, BuscarPerfisPorOrgUseCase)
  reg(c, BuscarPerfilCorporativoPorIdUseCase)
  reg(c, AtualizarPerfilCorporativoUseCase)
  reg(c, DeletarPerfilCorporativoUseCase)
}
