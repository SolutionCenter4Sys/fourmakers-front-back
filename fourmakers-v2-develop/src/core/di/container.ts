import 'reflect-metadata'
import { container } from 'tsyringe'

import type { AgendaGestorRepository } from '@domain/repositories/AgendaGestorRepository'
import type { AniversariantesRepository } from '@domain/repositories/AniversariantesRepository'
import type { AuthRepository } from '@domain/repositories/AuthRepository'
import type { AvaliacaoRepository } from '@domain/repositories/AvaliacaoRepository'
import type { BotFourmakersRepository } from '@domain/repositories/BotFourmakersRepository'
import type { CampanhaRepository } from '@domain/repositories/CampanhaRepository'
import type { CanalDenunciaRepository } from '@domain/repositories/CanalDenunciaRepository'
import type { ClienteRepository } from '@domain/repositories/ClienteRepository'
import type { ColaboradoresRepository } from '@domain/repositories/ColaboradoresRepository'
import type { ComunicacaoAnalyticsRepository } from '@domain/repositories/ComunicacaoAnalyticsRepository'
import type { ComunicacaoComunidadeRepository } from '@domain/repositories/ComunicacaoComunidadeRepository'
import type { ComunicacaoFeedRepository } from '@domain/repositories/ComunicacaoFeedRepository'
import type { ComunicacaoGrupoRepository } from '@domain/repositories/ComunicacaoGrupoRepository'
import type { ComunicacaoIaRepository } from '@domain/repositories/ComunicacaoIaRepository'
import type { ComunicacaoProfissionaisRepository } from '@domain/repositories/ComunicacaoProfissionaisRepository'
import type { DadosBancariosRepository } from '@domain/repositories/DadosBancariosRepository'
import type { EncontrosBigNumbersRepository } from '@domain/repositories/EncontrosBigNumbersRepository'
import type { Feedback360Repository } from '@domain/repositories/Feedback360Repository'
import type { FelizometroRepository } from '@domain/repositories/FelizometroRepository'
import type { FuncionalidadeSistemaRepository } from '@domain/repositories/FuncionalidadeSistemaRepository'
import type { GestaoDesempenhoRepository } from '@domain/repositories/GestaoDesempenhoRepository'
import type { GraphApiRepository } from '@domain/repositories/GraphApiRepository'
import type { GrupoAcessoRepository } from '@domain/repositories/GrupoAcessoRepository'
import type { HoleritesRepository } from '@domain/repositories/HoleritesRepository'
import type { IntegracaoBancariaRepository } from '@domain/repositories/IntegracaoBancariaRepository'
import type { MapaAlocacaoRepository } from '@domain/repositories/MapaAlocacaoRepository'
import type { MapaRelacionamentoRepository } from '@domain/repositories/MapaRelacionamentoRepository'
import type { MenuRepository } from '@domain/repositories/MenuRepository'
import type { MinhaEquipeRepository } from '@domain/repositories/MinhaEquipeRepository'
import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import type { NotasFiscaisRepository } from '@domain/repositories/NotasFiscaisRepository'
import type { NotificacaoRepository } from '@domain/repositories/NotificacaoRepository'
import type { OrganogramaRepository } from '@domain/repositories/OrganogramaRepository'
import type { ParametrosRepository } from '@domain/repositories/ParametrosRepository'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type { PermissionamentoRepository } from '@domain/repositories/PermissionamentoRepository'
import type { ProjetosRepository } from '@domain/repositories/ProjetosRepository'
import type { ReembolsoSolicitacaoRepository } from '@domain/repositories/ReembolsoSolicitacaoRepository'
import type { ReembolsosRepository } from '@domain/repositories/ReembolsosRepository'
import type { RelatorioMinhaJornadaRepository } from '@domain/repositories/RelatorioMinhaJornadaRepository'
import type { SkillsDashboardRepository } from '@domain/repositories/SkillsDashboardRepository'
import type { SkillsRepository } from '@domain/repositories/SkillsRepository'
import type { TbdRepository } from '@domain/repositories/TbdRepository'
import type { TimesheetRepository } from '@domain/repositories/TimesheetRepository'
import type { Vcx360Repository } from '@domain/repositories/Vcx360Repository'
import type { VcxRepository } from '@domain/repositories/VcxRepository'
import type { ViaCepRepository } from '@domain/repositories/ViaCepRepository'
import { AceitarRecusarConviteAgendaUseCase } from '@domain/usecases/AceitarRecusarConviteAgendaUseCase'
import { AcoesApi } from '@data/api/AcoesApi'
import { AddSkillToPerfil360UseCase } from '@domain/usecases/AddSkillToPerfil360UseCase'
import { AdicionarAnexosPublicacaoUseCase } from '@domain/usecases/AdicionarAnexosPublicacaoUseCase'
import { AdicionarUsuarioGrupoUseCase } from '@domain/usecases/AdicionarUsuarioGrupoUseCase'
import { AgendaGestorRepositoryImpl } from '@data/repositories/AgendaGestorRepositoryImpl'
import { AgendasApi } from '@data/api/AgendasApi'
import { AlteraPerfilAlocacaoUseCase } from '@domain/usecases/AlteraPerfilAlocacaoUseCase'
import { AnalisarComprovantesUseCase } from '@domain/usecases/AnalisarComprovantesUseCase'
import { AniversariantesRepositoryImpl } from '@data/repositories/AniversariantesRepositoryImpl'
import { ApontarHorasEmLoteUseCase } from '@domain/usecases/ApontarHorasEmLoteUseCase'
import { AprovarPublicacaoUseCase } from '@domain/usecases/AprovarPublicacaoUseCase'
import { AprovarSugestaoSkillUseCase } from '@domain/usecases/AprovarSugestaoSkillUseCase'
import { ArquivarComunidadeUseCase } from '@domain/usecases/ArquivarComunidadeUseCase'
import { ArquivarPublicacaoUseCase } from '@domain/usecases/ArquivarPublicacaoUseCase'
import { AtribuirFuncionalidadeGrupoUseCase } from '@domain/usecases/AtribuirFuncionalidadeGrupoUseCase'
import { AtualizarAgendaUseCase } from '@domain/usecases/AtualizarAgendaUseCase'
import { AtualizarAlocacaoUseCase } from '@domain/usecases/AtualizarAlocacaoUseCase'
import { AtualizarComentarioPublicacaoUseCase } from '@domain/usecases/AtualizarComentarioPublicacaoUseCase'
import { AtualizarComunicacaoGrupoUseCase } from '@domain/usecases/AtualizarComunicacaoGrupoUseCase'
import { AtualizarComunidadeUseCase } from '@domain/usecases/AtualizarComunidadeUseCase'
import { AtualizarContratoUseCase } from '@domain/usecases/AtualizarContratoUseCase'
import { AtualizarDepartamentoUseCase } from '@domain/usecases/AtualizarDepartamentoUseCase'
import { AtualizarDorUseCase } from '@domain/usecases/AtualizarDorUseCase'
import { AtualizarEncontroAiProximosPassosUseCase } from '@domain/usecases/AtualizarEncontroAiProximosPassosUseCase'
import { AtualizarFeedback360UseCase } from '@domain/usecases/AtualizarFeedback360UseCase'
import { AtualizarInteracaoCategoriaUseCase } from '@domain/usecases/AtualizarInteracaoCategoriaUseCase'
import { AtualizarInteracaoIaUseCase } from '@domain/usecases/AtualizarInteracaoIaUseCase'
import { AtualizarParceiroUseCase } from '@domain/usecases/AtualizarParceiroUseCase'
import { AtualizarPosicaoUseCase } from '@domain/usecases/AtualizarPosicaoUseCase'
import { AtualizarPublicacaoUseCase } from '@domain/usecases/AtualizarPublicacaoUseCase'
import { AtualizarStatusAcoesUseCase } from '@domain/usecases/AtualizarStatusAcoesUseCase'
import { AtualizarTbdUseCase } from '@domain/usecases/AtualizarTbdUseCase'
import { AuthApi } from '@data/api/AuthApi'
import { AuthRepositoryImpl } from '@data/repositories/AuthRepositoryImpl'
import { AvaliacaoRepositoryImpl } from '@data/repositories/AvaliacaoRepositoryImpl'
import { BaixarCertificadoColaboradorUseCase } from '@domain/usecases/BaixarCertificadoColaboradorUseCase'
import { BeneficioXanoApi } from '@data/api/BeneficioXanoApi'
import { BotFourmakersApi } from '@data/api/BotFourmakersApi'
import { BotFourmakersRepositoryImpl } from '@data/repositories/BotFourmakersRepositoryImpl'
import { BuscarAgendaGestorUseCase } from '@domain/usecases/BuscarAgendaGestorUseCase'
import { BuscarAgendasFilhosUseCase } from '@domain/usecases/BuscarAgendasFilhosUseCase'
import { BuscarDadosColaboradorUseCase } from '@domain/usecases/BuscarDadosColaboradorUseCase'
import { BuscarDadosVcx360UseCase } from '@domain/usecases/BuscarDadosVcx360UseCase'
import { BuscarDorPorIdUseCase } from '@domain/usecases/BuscarDorPorIdUseCase'
import { BuscarEncontroAiPorEncontroIdUseCase } from '@domain/usecases/BuscarEncontroAiPorEncontroIdUseCase'
import { BuscarHistoricoVcxUseCase } from '@domain/usecases/BuscarHistoricoVcxUseCase'
import { BuscarPeriodoFechadoUseCase } from '@domain/usecases/BuscarPeriodoFechadoUseCase'
import { BuscarTodosParceirosUseCase } from '@domain/usecases/BuscarTodosParceirosUseCase'
import { CadastrarMapaAlocacaoUseCase } from '@domain/usecases/CadastrarMapaAlocacaoUseCase'
import { CadastrarUsuarioUseCase } from '@domain/usecases/CadastrarUsuarioUseCase'
import { CampanhaApi } from '@data/api/CampanhaApi'
import { CampanhaRepositoryImpl } from '@data/repositories/CampanhaRepositoryImpl'
import { CanalDenunciaApi } from '@data/api/CanalDenunciaApi'
import { CanalDenunciaRepositoryImpl } from '@data/repositories/CanalDenunciaRepositoryImpl'
import { CarregarAgendaDetalheUseCase } from '@domain/usecases/CarregarAgendaDetalheUseCase'
import { CarregarAgendaPorIdUseCase } from '@domain/usecases/CarregarAgendaPorIdUseCase'
import { ClienteRepositoryImpl } from '@data/repositories/ClienteRepositoryImpl'
import { ColaboradoresApi } from '@data/api/ColaboradoresApi'
import { ColaboradoresRepositoryImpl } from '@data/repositories/ColaboradoresRepositoryImpl'
import { ColetaPerfilColaboradorCampanhaUseCase } from '@domain/usecases/ColetaPerfilColaboradorCampanhaUseCase'
import { CompetenciaRemessaApi } from '@data/api/CompetenciaRemessaApi'
import { CompetenciaRepositoryImpl } from '@data/repositories/CompetenciaRepositoryImpl'
import { CompetenciasApi } from '@data/api/CompetenciasApi'
import { ComunicacaoAnalyticsApi } from '@data/api/ComunicacaoAnalyticsApi'
import { ComunicacaoAnalyticsRepositoryImpl } from '@data/repositories/ComunicacaoAnalyticsRepositoryImpl'
import { ComunicacaoComunidadeApi } from '@data/api/ComunicacaoComunidadeApi'
import { ComunicacaoComunidadeRepositoryImpl } from '@data/repositories/ComunicacaoComunidadeRepositoryImpl'
import { ComunicacaoFeedRepositoryImpl } from '@data/repositories/ComunicacaoFeedRepositoryImpl'
import { ComunicacaoGrupoApi } from '@data/api/ComunicacaoGrupoApi'
import { ComunicacaoGrupoRepositoryImpl } from '@data/repositories/ComunicacaoGrupoRepositoryImpl'
import { ComunicacaoIaApi } from '@data/api/ComunicacaoIaApi'
import { ComunicacaoIaRepositoryImpl } from '@data/repositories/ComunicacaoIaRepositoryImpl'
import { ComunicacaoLabelApi } from '@data/api/ComunicacaoLabelApi'
import { ComunicacaoProfissionaisApi } from '@data/api/ComunicacaoProfissionaisApi'
import { ComunicacaoProfissionaisRepositoryImpl } from '@data/repositories/ComunicacaoProfissionaisRepositoryImpl'
import { ComunicacaoPublicacaoApi } from '@data/api/ComunicacaoPublicacaoApi'
import { ConciliacaoFolhaPagamentoApi } from '@data/api/ConciliacaoFolhaPagamentoApi'
import { ConfirmarLeituraObrigatoriaUseCase } from '@domain/usecases/ConfirmarLeituraObrigatoriaUseCase'
import { ConsultaProjetoHorasUseCase } from '@domain/usecases/ConsultaProjetoHorasUseCase'
import { ContarNotificacoesNaoLidasUseCase } from '@domain/usecases/ContarNotificacoesNaoLidasUseCase'
import { CreatePdiUseCase } from '@domain/usecases/CreatePdiUseCase'
import { CriarAgendaUseCase } from '@domain/usecases/CriarAgendaUseCase'
import { CriarAlocacaoUseCase } from '@domain/usecases/CriarAlocacaoUseCase'
import { CriarComunicacaoComunidadeUseCase } from '@domain/usecases/CriarComunicacaoComunidadeUseCase'
import { CriarComunicacaoGrupoUseCase } from '@domain/usecases/CriarComunicacaoGrupoUseCase'
import { CriarDadosBancariosUseCase } from '@domain/usecases/CriarDadosBancariosUseCase'
import { CriarDepartamentoUseCase } from '@domain/usecases/CriarDepartamentoUseCase'
import { CriarDorUseCase } from '@domain/usecases/CriarDorUseCase'
import { CriarEncontroAiProximosPassosUseCase } from '@domain/usecases/CriarEncontroAiProximosPassosUseCase'
import { CriarFeedback360UseCase } from '@domain/usecases/CriarFeedback360UseCase'
import { CriarGestorExternoUseCase } from '@domain/usecases/CriarGestorExternoUseCase'
import { CriarGrupoAcessoUseCase } from '@domain/usecases/CriarGrupoAcessoUseCase'
import { CriarPosicaoUseCase } from '@domain/usecases/CriarPosicaoUseCase'
import { CriarPublicacaoUseCase } from '@domain/usecases/CriarPublicacaoUseCase'
import { CriarReuniaoTeamsUseCase } from '@domain/usecases/CriarReuniaoTeamsUseCase'
import { DadosBancariosApi } from '@data/api/DadosBancariosApi'
import { DadosBancariosRepositoryImpl } from '@data/repositories/DadosBancariosRepositoryImpl'
import { DeletarAgendaUseCase } from '@domain/usecases/DeletarAgendaUseCase'
import { DeletarAlocacaoUseCase } from '@domain/usecases/DeletarAlocacaoUseCase'
import { DeletarApontamentoUseCase } from '@domain/usecases/DeletarApontamentoUseCase'
import { DeletarArquivoEncontroUseCase } from '@domain/usecases/DeletarArquivoEncontroUseCase'
import { DeletarComunicacaoGrupoUseCase } from '@domain/usecases/DeletarComunicacaoGrupoUseCase'
import { DeletarContratoUseCase } from '@domain/usecases/DeletarContratoUseCase'
import { DeletarDepartamentoUseCase } from '@domain/usecases/DeletarDepartamentoUseCase'
import { DeletarInteracaoIaUseCase } from '@domain/usecases/DeletarInteracaoIaUseCase'
import { DeletarPosicaoUseCase } from '@domain/usecases/DeletarPosicaoUseCase'
import { DownloadRelatorioLogDetalhadoUseCase } from '@domain/usecases/DownloadRelatorioLogDetalhadoUseCase'
import { EditarAlocacaoUseCase } from '@domain/usecases/EditarAlocacaoUseCase'
import { EditarApontamentoUseCase } from '@domain/usecases/EditarApontamentoUseCase'
import { EditarColaboradorUseCase } from '@domain/usecases/EditarColaboradorUseCase'
import { EditarDadosBancariosUseCase } from '@domain/usecases/EditarDadosBancariosUseCase'
import { EditarDadosColaboradorUseCase } from '@domain/usecases/EditarDadosColaboradorUseCase'
import { EditarGrupoAcessoUseCase } from '@domain/usecases/EditarGrupoAcessoUseCase'
import { EncontrosBigNumbersApi } from '@data/api/EncontrosBigNumbersApi'
import { EncontrosBigNumbersRepositoryImpl } from '@data/repositories/EncontrosBigNumbersRepositoryImpl'
import { EnviarDenunciaUseCase } from '@domain/usecases/EnviarDenunciaUseCase'
import { EnviarEmailNotificacaoAprovadoresUseCase } from '@domain/usecases/EnviarEmailNotificacaoAprovadoresUseCase'
import { EnviarEmailTemplateCandidatoUseCase } from '@domain/usecases/EnviarEmailTemplateCandidatoUseCase'
import { EnviarSentimentoUseCase } from '@domain/usecases/EnviarSentimentoUseCase'
import { ExcluirAnexoPublicacaoUseCase } from '@domain/usecases/ExcluirAnexoPublicacaoUseCase'
import { ExcluirComentarioPublicacaoUseCase } from '@domain/usecases/ExcluirComentarioPublicacaoUseCase'
import { ExcluirDorUseCase } from '@domain/usecases/ExcluirDorUseCase'
import { ExcluirInteracaoComentarioUseCase } from '@domain/usecases/ExcluirInteracaoComentarioUseCase'
import { ExcluirInteracaoPublicacaoUseCase } from '@domain/usecases/ExcluirInteracaoPublicacaoUseCase'
import { ExcluirPublicacaoUseCase } from '@domain/usecases/ExcluirPublicacaoUseCase'
import { ExportarRelatorioAlocacoesUseCase } from '@domain/usecases/ExportarRelatorioAlocacoesUseCase'
import { FecharAlterarPeriodoUseCase } from '@domain/usecases/FecharAlterarPeriodoUseCase'
import { Feedback360Api } from '@data/api/Feedback360Api'
import { Feedback360RepositoryImpl } from '@data/repositories/Feedback360RepositoryImpl'
import { FelizometroApi } from '@data/api/FelizometroApi'
import { FelizometroRepositoryImpl } from '@data/repositories/FelizometroRepositoryImpl'
import { FourmakersApi } from '@data/api/FourmakersApi'
import { FuncionalidadeSistemaRepositoryImpl } from '@data/repositories/FuncionalidadeSistemaRepositoryImpl'
import { GerarRelatorioColaboradoresUseCase } from '@domain/usecases/GerarRelatorioColaboradoresUseCase'
import { GerarRelatorioParceriaUseCase } from '@domain/usecases/GerarRelatorioParceriaUseCase'
import { GestaoDesempenhoRepositoryImpl } from '@data/repositories/GestaoDesempenhoRepositoryImpl'
import { GetAccessTokenUseCase } from '@domain/usecases/GetAccessTokenUseCase'
import { GetAddressByCepUseCase } from '@domain/usecases/GetAddressByCepUseCase'
import { GetAniversariantesSemanaUseCase } from '@domain/usecases/GetAniversariantesSemanaUseCase'
import { GetColaboradorRadarUseCase } from '@domain/usecases/GetColaboradorRadarUseCase'
import { GetColaboradoresUseCase } from '@domain/usecases/GetColaboradoresUseCase'
import { GetDadosBancariosUseCase } from '@domain/usecases/GetDadosBancariosUseCase'
import { GetHoleritesUseCase } from '@domain/usecases/GetHoleritesUseCase'
import { GetManagersUseCase } from '@domain/usecases/GetManagersUseCase'
import { GetMapaAlocacaoRecursoUseCase } from '@domain/usecases/GetMapaAlocacaoRecursoUseCase'
import { GetMapaAlocacaoResumoUseCase } from '@domain/usecases/GetMapaAlocacaoResumoUseCase'
import { GetMenuResourcesUseCase } from '@domain/usecases/GetMenuResourcesUseCase'
import { GetMinhaEquipeContextUseCase } from '@domain/usecases/GetMinhaEquipeContextUseCase'
import { GetMinhaJornadaContextUseCase } from '@domain/usecases/GetMinhaJornadaContextUseCase'
import { GetNotasFiscaisUseCase } from '@domain/usecases/GetNotasFiscaisUseCase'
import { GetPDIsColaboradorUseCase } from '@domain/usecases/GetPDIsColaboradorUseCase'
import { GetProjetosColaboradorUseCase } from '@domain/usecases/GetProjetosColaboradorUseCase'
import { GetProjetosUseCase } from '@domain/usecases/GetProjetosUseCase'
import { GetReembolsosUseCase } from '@domain/usecases/GetReembolsosUseCase'
import { GetShowmeProfileUseCase } from '@domain/usecases/GetShowmeProfileUseCase'
import { GetSkillLevelsUseCase } from '@domain/usecases/GetSkillLevelsUseCase'
import { GetSkillLogsPaginatedUseCase } from '@domain/usecases/GetSkillLogsPaginatedUseCase'
import { GetSkillsDashboardDataUseCase } from '@domain/usecases/GetSkillsDashboardDataUseCase'
import { GetSuggestionHistoryUseCase } from '@domain/usecases/GetSuggestionHistoryUseCase'
import { GetUserProfileUseCase } from '@domain/usecases/GetUserProfileUseCase'
import { GetVerbasUseCase } from '@domain/usecases/GetVerbasUseCase'
import { GraphApiRepositoryImpl } from '@data/repositories/GraphApiRepositoryImpl'
import { GrupoAcessoRepositoryImpl } from '@data/repositories/GrupoAcessoRepositoryImpl'
import { HoleritesApi } from '@data/api/HoleritesApi'
import { HoleritesRepositoryImpl } from '@data/repositories/HoleritesRepositoryImpl'
import { InserirArquivoEncontroUseCase } from '@domain/usecases/InserirArquivoEncontroUseCase'
import { InserirArquivoParceiroUseCase } from '@domain/usecases/InserirArquivoParceiroUseCase'
import { InserirAvaliacaoSatisfacaoUseCase } from '@domain/usecases/InserirAvaliacaoSatisfacaoUseCase'
import { InserirColaboradorUseCase } from '@domain/usecases/InserirColaboradorUseCase'
import { InserirComentarioAcaoUseCase } from '@domain/usecases/InserirComentarioAcaoUseCase'
import { InserirComentarioCandidaturaUseCase } from '@domain/usecases/InserirComentarioCandidaturaUseCase'
import { InserirComentarioPublicacaoUseCase } from '@domain/usecases/InserirComentarioPublicacaoUseCase'
import { InserirContratoUseCase } from '@domain/usecases/InserirContratoUseCase'
import { InserirFeedbackBotUseCase } from '@domain/usecases/InserirFeedbackBotUseCase'
import { InserirFeedbackGestaoDesempenhoUseCase } from '@domain/usecases/InserirFeedbackGestaoDesempenhoUseCase'
import { InserirInteracaoComentarioUseCase } from '@domain/usecases/InserirInteracaoComentarioUseCase'
import { InserirInteracaoIaUseCase } from '@domain/usecases/InserirInteracaoIaUseCase'
import { InserirInteracaoPublicacaoUseCase } from '@domain/usecases/InserirInteracaoPublicacaoUseCase'
import { InserirOneOnOneUseCase } from '@domain/usecases/InserirOneOnOneUseCase'
import { InserirParametrizacaoUseCase } from '@domain/usecases/InserirParametrizacaoUseCase'
import { InserirParceiroUseCase } from '@domain/usecases/InserirParceiroUseCase'
import { InserirPautaSugeridaColaboradorUseCase } from '@domain/usecases/InserirPautaSugeridaColaboradorUseCase'
import { InserirPautaSugeridaUseCase } from '@domain/usecases/InserirPautaSugeridaUseCase'
import { InserirQuestaoUseCase } from '@domain/usecases/InserirQuestaoUseCase'
import { InserirSolicitacaoUseCase } from '@domain/usecases/InserirSolicitacaoUseCase'
import { InserirTbdUseCase } from '@domain/usecases/InserirTbdUseCase'
import { InserirVisualizacaoFeedbackUseCase } from '@domain/usecases/InserirVisualizacaoFeedbackUseCase'
import { IntegracaoBancariaApi } from '@data/api/IntegracaoBancariaApi'
import { IntegracaoBancariaRepositoryImpl } from '@data/repositories/IntegracaoBancariaRepositoryImpl'
import { IntegracaoContabilApi } from '@data/api/IntegracaoContabilApi'
import { IntegracaoFolhaPontoApi } from '@data/api/IntegracaoFolhaPontoApi'
import { InteracoesApi } from '@data/api/InteracoesApi'
import { ListarAlocacoesColabETbdUseCase } from '@domain/usecases/ListarAlocacoesColabETbdUseCase'
import { ListarApontamentosPorVigenciaUseCase } from '@domain/usecases/ListarApontamentosPorVigenciaUseCase'
import { ListarAvaliacoesFeedback360UseCase } from '@domain/usecases/ListarAvaliacoesFeedback360UseCase'
import { ListarChatsUseCase } from '@domain/usecases/ListarChatsUseCase'
import { ListarClientesAlternativoMapaRelacionamentoUseCase } from '@domain/usecases/ListarClientesAlternativoMapaRelacionamentoUseCase'
import { ListarClientesMapaRelacionamentoUseCase } from '@domain/usecases/ListarClientesMapaRelacionamentoUseCase'
import { ListarClientesUseCase } from '@domain/usecases/ListarClientesUseCase'
import { ListarColaboradoresDisponiveisUseCase } from '@domain/usecases/ListarColaboradoresDisponiveisUseCase'
import { ListarColaboradoresEApontamentosPorGestorUseCase } from '@domain/usecases/ListarColaboradoresEApontamentosPorGestorUseCase'
import { ListarColaboradoresETbdsUseCase } from '@domain/usecases/ListarColaboradoresETbdsUseCase'
import { ListarColaboradoresExternosAlocadosUseCase } from '@domain/usecases/ListarColaboradoresExternosAlocadosUseCase'
import { ListarColaboradoresOrgUseCase } from '@domain/usecases/ListarColaboradoresOrgUseCase'
import { ListarColaboradoresVinculadosGerenteDeProjetoUseCase } from '@domain/usecases/ListarColaboradoresVinculadosGerenteDeProjetoUseCase'
import { ListarComunicacaoComunidadesUseCase } from '@domain/usecases/ListarComunicacaoComunidadesUseCase'
import { ListarComunicacaoFeedUseCase } from '@domain/usecases/ListarComunicacaoFeedUseCase'
import { ListarComunicacaoGruposUseCase } from '@domain/usecases/ListarComunicacaoGruposUseCase'
import { ListarComunicacaoProfissionaisUseCase } from '@domain/usecases/ListarComunicacaoProfissionaisUseCase'
import { ListarDepartamentosMapaRelacionamentoUseCase } from '@domain/usecases/ListarDepartamentosMapaRelacionamentoUseCase'
import { ListarDiretoriasDisponiveisUseCase } from '@domain/usecases/ListarDiretoriasDisponiveisUseCase'
import { ListarDiretoriosSistemasGruposUseCase } from '@domain/usecases/ListarDiretoriosSistemasGruposUseCase'
import { ListarDoresPorPosicaoIdUseCase } from '@domain/usecases/ListarDoresPorPosicaoIdUseCase'
import { ListarEnviadosFeedback360UseCase } from '@domain/usecases/ListarEnviadosFeedback360UseCase'
import { ListarEquipamentosPadroesAninhadosUseCase } from '@domain/usecases/ListarEquipamentosPadroesAninhadosUseCase'
import { ListarEscolaridadeColaboradorUseCase } from '@domain/usecases/ListarEscolaridadeColaboradorUseCase'
import { ListarFuncionalidadesSistemaUseCase } from '@domain/usecases/ListarFuncionalidadesSistemaUseCase'
import { ListarGerenteAdmDosColabsGPUseCase } from '@domain/usecases/ListarGerenteAdmDosColabsGPUseCase'
import { ListarGruposAcessoUseCase } from '@domain/usecases/ListarGruposAcessoUseCase'
import { ListarImpactosDoresUseCase } from '@domain/usecases/ListarImpactosDoresUseCase'
import { ListarNomesGestoresUseCase } from '@domain/usecases/ListarNomesGestoresUseCase'
import { ListarNotificacoesUseCase } from '@domain/usecases/ListarNotificacoesUseCase'
import { ListarPerfilAlocacaoUseCase } from '@domain/usecases/ListarPerfilAlocacaoUseCase'
import { ListarPosicoesUseCase } from '@domain/usecases/ListarPosicoesUseCase'
import { ListarProjetosColaboradorMapaUseCase } from '@domain/usecases/ListarProjetosColaboradorMapaUseCase'
import { ListarProjetosComAtividadesUseCase } from '@domain/usecases/ListarProjetosComAtividadesUseCase'
import { ListarProjetosVisaoGerenteDeProjetoUseCase } from '@domain/usecases/ListarProjetosVisaoGerenteDeProjetoUseCase'
import { ListarQuestoesUseCase } from '@domain/usecases/ListarQuestoesUseCase'
import { ListarRecebidosFeedback360UseCase } from '@domain/usecases/ListarRecebidosFeedback360UseCase'
import { ListarRelacionamentosFeedback360UseCase } from '@domain/usecases/ListarRelacionamentosFeedback360UseCase'
import { ListarSkillsSumarioUseCase } from '@domain/usecases/ListarSkillsSumarioUseCase'
import { ListarStatusProjetoUseCase } from '@domain/usecases/ListarStatusProjetoUseCase'
import { ListarTbdUseCase } from '@domain/usecases/ListarTbdUseCase'
import { ListarTemplateSemanaVigenciaUseCase } from '@domain/usecases/ListarTemplateSemanaVigenciaUseCase'
import { ListarTodasSkillsColaboradorUseCase } from '@domain/usecases/ListarTodasSkillsColaboradorUseCase'
import { ListarUrgenciasDoresUseCase } from '@domain/usecases/ListarUrgenciasDoresUseCase'
import { ListarVigenciasApontamentosGerenteDeProjetoUseCase } from '@domain/usecases/ListarVigenciasApontamentosGerenteDeProjetoUseCase'
import { ListarVigenciasColaboradorUseCase } from '@domain/usecases/ListarVigenciasColaboradorUseCase'
import { LogoutUseCase } from '@domain/usecases/LogoutUseCase'
import { MapaAlocacaoApi } from '@data/api/MapaAlocacaoApi'
import { MapaAlocacaoRepositoryImpl } from '@data/repositories/MapaAlocacaoRepositoryImpl'
import { MapaRelacionamentoApi } from '@data/api/MapaRelacionamentoApi'
import { MapaRelacionamentoRepositoryImpl } from '@data/repositories/MapaRelacionamentoRepositoryImpl'
import { MarcarNotificacoesComoLidasUseCase } from '@domain/usecases/MarcarNotificacoesComoLidasUseCase'
import { MenuApi } from '@data/api/MenuApi'
import { MenuRepositoryImpl } from '@data/repositories/MenuRepositoryImpl'
import { MicrosoftGraphApi } from '@data/api/MicrosoftGraphApi'
import { MinhaEquipeApi } from '@data/api/MinhaEquipeApi'
import { MinhaEquipeRepositoryImpl } from '@data/repositories/MinhaEquipeRepositoryImpl'
import { MinhaJornadaApi } from '@data/api/MinhaJornadaApi'
import { MinhaJornadaRepositoryImpl } from '@data/repositories/MinhaJornadaRepositoryImpl'
import { MinhaJornadaXanoApi } from '@data/api/MinhaJornadaXanoApi'
import { MinhaJornadaXanoPDIApi } from '@data/api/MinhaJornadaApi'
import { NotasFiscaisApi } from '@data/api/NotasFiscaisApi'
import { NotasFiscaisRepositoryImpl } from '@data/repositories/NotasFiscaisRepositoryImpl'
import { NotificacaoApi } from '@data/api/NotificacaoApi'
import { NotificacaoRepositoryImpl } from '@data/repositories/NotificacaoRepositoryImpl'
import { ObterAcoesEmAtrasoDetalheUseCase } from '@domain/usecases/ObterAcoesEmAtrasoDetalheUseCase'
import { ObterAgendaRealizadaDetalheUseCase } from '@domain/usecases/ObterAgendaRealizadaDetalheUseCase'
import { ObterAgendaSemInteracaoDetalheUseCase } from '@domain/usecases/ObterAgendaSemInteracaoDetalheUseCase'
import { ObterAnalyticsResumoUseCase } from '@domain/usecases/ObterAnalyticsResumoUseCase'
import { ObterBigNumbersCategoriaUseCase } from '@domain/usecases/ObterBigNumbersCategoriaUseCase'
import { ObterCategoriaComInteracaoDetalheUseCase } from '@domain/usecases/ObterCategoriaComInteracaoDetalheUseCase'
import { ObterClientesImpactadosDetalheUseCase } from '@domain/usecases/ObterClientesImpactadosDetalheUseCase'
import { ObterComunicacaoGrupoPorIdUseCase } from '@domain/usecases/ObterComunicacaoGrupoPorIdUseCase'
import { ObterConfirmacoesLeituraUseCase } from '@domain/usecases/ObterConfirmacoesLeituraUseCase'
import { ObterDadosColaboradorUseCase } from '@domain/usecases/ObterDadosColaboradorUseCase'
import { ObterDashboardColaboradorAvaliadoUseCase } from '@domain/usecases/ObterDashboardColaboradorAvaliadoUseCase'
import { ObterDashboardColaboradorUseCase } from '@domain/usecases/ObterDashboardColaboradorUseCase'
import { ObterDashboardGestorUseCase } from '@domain/usecases/ObterDashboardGestorUseCase'
import { ObterDashboardRHUseCase } from '@domain/usecases/ObterDashboardRHUseCase'
import { ObterEncontrosBigNumbersUseCase } from '@domain/usecases/ObterEncontrosBigNumbersUseCase'
import { ObterListaColaboradoresRHUseCase } from '@domain/usecases/ObterListaColaboradoresRHUseCase'
import { ObterMeusColaboradoresUseCase } from '@domain/usecases/ObterMeusColaboradoresUseCase'
import { ObterParametrizacaoUseCase } from '@domain/usecases/ObterParametrizacaoUseCase'
import { ObterPermissoesUsuarioLogadoUseCase } from '@domain/usecases/ObterPermissoesUsuarioLogadoUseCase'
import { ObterPublicacaoPorIdUseCase } from '@domain/usecases/ObterPublicacaoPorIdUseCase'
import { ObterUsuarioIdPorCpfUseCase } from '@domain/usecases/ObterUsuarioIdPorCpfUseCase'
import { OcultarNoFeedPublicacaoUseCase } from '@domain/usecases/OcultarNoFeedPublicacaoUseCase'
import { ProcessarConteudoIaComunicacaoUseCase } from '@domain/usecases/ProcessarConteudoIaComunicacaoUseCase'
import { OrganogramaApi } from '@data/api/OrganogramaApi'
import { OrganogramaRepositoryImpl } from '@data/repositories/OrganogramaRepositoryImpl'
import { ParametrosApi } from '@data/api/ParametrosApi'
import { ParametrosRepositoryImpl } from '@data/repositories/ParametrosRepositoryImpl'
import { ParceriaApi } from '@data/api/ParceriaApi'
import { ParceriaRepositoryImpl } from '@data/repositories/ParceriaRepositoryImpl'
import { PermissionamentoRepositoryImpl } from '@data/repositories/PermissionamentoRepositoryImpl'
import { ProfileApi } from '@data/api/ProfileApi'
import { ProjetosApi } from '@data/api/ProjetosApi'
import { ProjetosRepositoryImpl } from '@data/repositories/ProjetosRepositoryImpl'
import { PublicarAgoraPublicacaoUseCase } from '@domain/usecases/PublicarAgoraPublicacaoUseCase'
import { ReembolsoComponentesApi } from '@data/api/ReembolsoComponentesApi'
import { ReembolsoParametrosApi } from '@data/api/ReembolsoParametrosApi'
import { ReembolsoSolicitacaoApi } from '@data/api/ReembolsoSolicitacaoApi'
import { ReembolsoSolicitacaoRepositoryImpl } from '@data/repositories/ReembolsoSolicitacaoRepositoryImpl'
import { ReembolsosApi } from '@data/api/ReembolsosApi'
import { ReembolsosRepositoryImpl } from '@data/repositories/ReembolsosRepositoryImpl'
import { RejeitarPublicacaoUseCase } from '@domain/usecases/RejeitarPublicacaoUseCase'
import { RelatorioMinhaJornadaApi } from '@data/api/RelatorioMinhaJornadaApi'
import { RelatorioMinhaJornadaRepositoryImpl } from '@data/repositories/RelatorioMinhaJornadaRepositoryImpl'
import { RemoveSkillFromPerfil360UseCase } from '@domain/usecases/RemoveSkillFromPerfil360UseCase'
import { RemoverAlocacoesEmLoteUseCase } from '@domain/usecases/RemoverAlocacoesEmLoteUseCase'
import { RemoverChatUseCase } from '@domain/usecases/RemoverChatUseCase'
import { RemoverFuncionalidadeGrupoUseCase } from '@domain/usecases/RemoverFuncionalidadeGrupoUseCase'
import { RemoverUsuarioGrupoUseCase } from '@domain/usecases/RemoverUsuarioGrupoUseCase'
import { SalvarDadosVcx360UseCase } from '@domain/usecases/SalvarDadosVcx360UseCase'
import { SalvarNoMapaRelacionamentoUseCase } from '@domain/usecases/SalvarNoMapaRelacionamentoUseCase'
import { SearchSkillsUseCase } from '@domain/usecases/SearchSkillsUseCase'
import { SendTokenEmailUseCase } from '@domain/usecases/SendTokenEmailUseCase'
import { SkillsApi } from '@data/api/SkillsApi'
import { SkillsDashboardApi } from '@data/api/SkillsDashboardApi'
import { SkillsDashboardRepositoryImpl } from '@data/repositories/SkillsDashboardRepositoryImpl'
import { SkillsRepositoryImpl } from '@data/repositories/SkillsRepositoryImpl'
import { SubstituirDadosAlocacoesPorPeriodoUseCase } from '@domain/usecases/SubstituirDadosAlocacoesPorPeriodoUseCase'
import { SuggestNewSkillUseCase } from '@domain/usecases/SuggestNewSkillUseCase'
import { TbdApi } from '@data/api/TbdApi'
import { TbdRepositoryImpl } from '@data/repositories/TbdRepositoryImpl'
import { TimesheetComponentesApi } from '@data/api/TimesheetComponentesApi'
import { TimesheetRepositoryImpl } from '@data/repositories/TimesheetRepositoryImpl'
import { UpdateSkillInterestUseCase } from '@domain/usecases/UpdateSkillInterestUseCase'
import { UpdateSkillLevelUseCase } from '@domain/usecases/UpdateSkillLevelUseCase'
import { ValidateTokenEmailUseCase } from '@domain/usecases/ValidateTokenEmailUseCase'
import { Vcx360RepositoryImpl } from '@data/repositories/Vcx360RepositoryImpl'
import { VcxApi } from '@data/api/VcxApi'
import { VcxRepositoryImpl } from '@data/repositories/VcxRepositoryImpl'
import { ViaCepApi } from '@data/api/ViaCepApi'
import { ViaCepRepositoryImpl } from '@data/repositories/ViaCepRepositoryImpl'

import { registerRecrutamentoModule } from './modules/recrutamento/registerRecrutamento'
import { registerRecrutamentoUseCases } from './modules/recrutamento/registerRecrutamentoUseCases'
import { DiTokens } from './tokens'

let dependenciesRegistered = false

export const setupDependencyInjection = () => {
  if (dependenciesRegistered) {
    return
  }

  dependenciesRegistered = true

  // Referência explícita para satisfazer noUnusedLocals (registros em setup abaixo)
  void [
    AtualizarEncontroAiProximosPassosUseCase,
    AtualizarInteracaoCategoriaUseCase,
    BuscarEncontroAiPorEncontroIdUseCase,
    CarregarAgendaPorIdUseCase,
    CriarEncontroAiProximosPassosUseCase,
    CriarGestorExternoUseCase,
    EnviarEmailTemplateCandidatoUseCase,
    InserirComentarioCandidaturaUseCase,
    ListarDiretoriosSistemasGruposUseCase,
    ListarEquipamentosPadroesAninhadosUseCase,
    ObterAcoesEmAtrasoDetalheUseCase,
    ObterAgendaSemInteracaoDetalheUseCase,
    ObterCategoriaComInteracaoDetalheUseCase,
    ObterClientesImpactadosDetalheUseCase,
  ]

  // APIs
  if (!container.isRegistered(DiTokens.authApi)) {
    container.registerSingleton(DiTokens.authApi, AuthApi)
  }

  if (!container.isRegistered(DiTokens.menuApi)) {
    container.registerSingleton(DiTokens.menuApi, MenuApi)
  }

  if (!container.isRegistered(DiTokens.colaboradoresApi)) {
    container.registerSingleton(DiTokens.colaboradoresApi, ColaboradoresApi)
  }

  if (!container.isRegistered(DiTokens.projetosApi)) {
    container.registerSingleton(DiTokens.projetosApi, ProjetosApi)
  }

  if (!container.isRegistered(DiTokens.holeritesApi)) {
    container.registerSingleton(DiTokens.holeritesApi, HoleritesApi)
  }

  if (!container.isRegistered(DiTokens.reembolsosApi)) {
    container.registerSingleton(DiTokens.reembolsosApi, ReembolsosApi)
  }

  if (!container.isRegistered(DiTokens.notasFiscaisApi)) {
    container.registerSingleton(DiTokens.notasFiscaisApi, NotasFiscaisApi)
  }

  if (!container.isRegistered(DiTokens.integracaoFolhaPontoApi)) {
    container.registerSingleton(DiTokens.integracaoFolhaPontoApi, IntegracaoFolhaPontoApi)
  }

  if (!container.isRegistered(DiTokens.conciliacaoFolhaPagamentoApi)) {
    container.registerSingleton(DiTokens.conciliacaoFolhaPagamentoApi, ConciliacaoFolhaPagamentoApi)
  }

  if (!container.isRegistered(DiTokens.integracaoContabilApi)) {
    container.registerSingleton(DiTokens.integracaoContabilApi, IntegracaoContabilApi)
  }

  if (!container.isRegistered(DiTokens.competenciaRemessaApi)) {
    container.registerSingleton(DiTokens.competenciaRemessaApi, CompetenciaRemessaApi)
  }

  if (!container.isRegistered(DiTokens.reembolsoComponentesApi)) {
    container.registerSingleton(DiTokens.reembolsoComponentesApi, ReembolsoComponentesApi)
  }

  if (!container.isRegistered(DiTokens.timesheetComponentesApi)) {
    container.registerSingleton(DiTokens.timesheetComponentesApi, TimesheetComponentesApi)
  }

  if (!container.isRegistered(DiTokens.mapaAlocacaoApi)) {
    container.registerSingleton(DiTokens.mapaAlocacaoApi, MapaAlocacaoApi)
  }

  if (!container.isRegistered(DiTokens.profileApi)) {
    container.registerSingleton(DiTokens.profileApi, ProfileApi)
  }

  if (!container.isRegistered(DiTokens.reembolsoParametrosApi)) {
    container.registerSingleton(DiTokens.reembolsoParametrosApi, ReembolsoParametrosApi)
  }

  if (!container.isRegistered(DiTokens.parametrosApi)) {
    container.registerSingleton(DiTokens.parametrosApi, ParametrosApi)
  }

  if (!container.isRegistered(DiTokens.beneficioXanoApi)) {
    container.registerSingleton(DiTokens.beneficioXanoApi, BeneficioXanoApi)
  }

  if (!container.isRegistered(DiTokens.reembolsoSolicitacaoApi)) {
    container.registerSingleton(DiTokens.reembolsoSolicitacaoApi, ReembolsoSolicitacaoApi)
  }

  if (!container.isRegistered(DiTokens.dadosBancariosApi)) {
    container.registerSingleton(DiTokens.dadosBancariosApi, DadosBancariosApi)
  }

  registerRecrutamentoModule(container)

  if (!container.isRegistered(DiTokens.minhaJornadaApi)) {
    container.registerSingleton(DiTokens.minhaJornadaApi, MinhaJornadaApi)
  }

  if (!container.isRegistered(DiTokens.competenciasApi)) {
    container.registerSingleton(DiTokens.competenciasApi, CompetenciasApi)
  }

  if (!container.isRegistered(DiTokens.competenciaRepository)) {
    container.registerSingleton(DiTokens.competenciaRepository, CompetenciaRepositoryImpl)
  }

  if (!container.isRegistered(ListarSkillsSumarioUseCase)) {
    container.registerSingleton(ListarSkillsSumarioUseCase, ListarSkillsSumarioUseCase)
  }

  if (!container.isRegistered(ListarTodasSkillsColaboradorUseCase)) {
    container.registerSingleton(ListarTodasSkillsColaboradorUseCase, ListarTodasSkillsColaboradorUseCase)
  }

  if (!container.isRegistered(DiTokens.minhaJornadaXanoApi)) {
    container.registerSingleton(DiTokens.minhaJornadaXanoApi, MinhaJornadaXanoApi)
  }

  if (!container.isRegistered(DiTokens.minhaJornadaXanoPDIApi)) {
    container.registerSingleton(DiTokens.minhaJornadaXanoPDIApi, MinhaJornadaXanoPDIApi)
  }

  if (!container.isRegistered(DiTokens.felizometroApi)) {
    container.registerSingleton(DiTokens.felizometroApi, FelizometroApi)
  }

  if (!container.isRegistered(DiTokens.notificacaoApi)) {
    container.registerSingleton(DiTokens.notificacaoApi, NotificacaoApi)
  }

  if (!container.isRegistered(DiTokens.skillsDashboardApi)) {
    container.registerSingleton(DiTokens.skillsDashboardApi, SkillsDashboardApi)
  }

  if (!container.isRegistered(DiTokens.relatorioMinhaJornadaApi)) {
    container.registerSingleton(DiTokens.relatorioMinhaJornadaApi, RelatorioMinhaJornadaApi)
  }

  if (!container.isRegistered(DiTokens.skillsApi)) {
    container.registerSingleton(DiTokens.skillsApi, SkillsApi)
  }

  if (!container.isRegistered(DiTokens.campanhaApi)) {
    container.registerSingleton(DiTokens.campanhaApi, CampanhaApi)
  }

  if (!container.isRegistered(DiTokens.viaCepApi)) {
    container.registerSingleton(DiTokens.viaCepApi, ViaCepApi)
  }

  if (!container.isRegistered(DiTokens.fourmakersApi)) {
    container.registerSingleton(DiTokens.fourmakersApi, FourmakersApi)
  }

  if (!container.isRegistered(DiTokens.canalDenunciaApi)) {
    container.registerSingleton(DiTokens.canalDenunciaApi, CanalDenunciaApi)
  }

  if (!container.isRegistered(DiTokens.integracaoBancariaApi)) {
    container.registerSingleton(DiTokens.integracaoBancariaApi, IntegracaoBancariaApi)
  }

  if (!container.isRegistered(DiTokens.mapaRelacionamentoApi)) {
    container.registerSingleton(DiTokens.mapaRelacionamentoApi, MapaRelacionamentoApi)
  }
    if (!container.isRegistered(DiTokens.minhaEquipeApi)) {
    container.registerSingleton(DiTokens.minhaEquipeApi, MinhaEquipeApi)
  }

  if (!container.isRegistered(DiTokens.parceriaApi)) {
    container.registerSingleton(DiTokens.parceriaApi, ParceriaApi)
  }

  if (!container.isRegistered(DiTokens.organogramaApi)) {
    container.registerSingleton(DiTokens.organogramaApi, OrganogramaApi)
  }

  if (!container.isRegistered(DiTokens.feedback360Api)) {
    container.registerSingleton(DiTokens.feedback360Api, Feedback360Api)
  }

  if (!container.isRegistered(DiTokens.comunicacaoFeedApi)) {
    container.registerSingleton(DiTokens.comunicacaoFeedApi, ComunicacaoPublicacaoApi)
  }

  if (!container.isRegistered(DiTokens.comunicacaoGrupoApi)) {
    container.registerSingleton(DiTokens.comunicacaoGrupoApi, ComunicacaoGrupoApi)
  }

  if (!container.isRegistered(DiTokens.comunicacaoComunidadeApi)) {
    container.registerSingleton(DiTokens.comunicacaoComunidadeApi, ComunicacaoComunidadeApi)
  }

  if (!container.isRegistered(DiTokens.comunicacaoProfissionaisApi)) {
    container.registerSingleton(DiTokens.comunicacaoProfissionaisApi, ComunicacaoProfissionaisApi)
  }

  if (!container.isRegistered(DiTokens.comunicacaoLabelApi)) {
    container.registerSingleton(DiTokens.comunicacaoLabelApi, ComunicacaoLabelApi)
  }

  if (!container.isRegistered(DiTokens.comunicacaoIaApi)) {
    container.registerSingleton(DiTokens.comunicacaoIaApi, ComunicacaoIaApi)
  }

  if (!container.isRegistered(DiTokens.comunicacaoAnalyticsApi)) {
    container.registerSingleton(DiTokens.comunicacaoAnalyticsApi, ComunicacaoAnalyticsApi)
  }

  if (!container.isRegistered(DiTokens.vcxApi)) {
    container.registerSingleton(DiTokens.vcxApi, VcxApi)
  }

  // Repositories
  if (!container.isRegistered(DiTokens.authRepository)) {
    container.registerSingleton<AuthRepository>(
      DiTokens.authRepository,
      AuthRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.menuRepository)) {
    container.registerSingleton<MenuRepository>(
      DiTokens.menuRepository,
      MenuRepositoryImpl,
    )
  }

  // Sempre registrar novamente para garantir que métodos novos sejam incluídos
  // O TSyringe permite re-registrar, mas pode manter instância antiga em desenvolvimento
  // Em produção, o container será reinicializado e terá o método disponível
  container.registerSingleton<ColaboradoresRepository>(
    DiTokens.colaboradoresRepository,
    ColaboradoresRepositoryImpl,
  )

  if (!container.isRegistered(DiTokens.projetosRepository)) {
    container.registerSingleton<ProjetosRepository>(
      DiTokens.projetosRepository,
      ProjetosRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.holeritesRepository)) {
    container.registerSingleton<HoleritesRepository>(
      DiTokens.holeritesRepository,
      HoleritesRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.reembolsosRepository)) {
    container.registerSingleton<ReembolsosRepository>(
      DiTokens.reembolsosRepository,
      ReembolsosRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.notasFiscaisRepository)) {
    container.registerSingleton<NotasFiscaisRepository>(
      DiTokens.notasFiscaisRepository,
      NotasFiscaisRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.parametrosRepository)) {
    container.registerSingleton<ParametrosRepository>(
      DiTokens.parametrosRepository,
      ParametrosRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.reembolsoSolicitacaoRepository)) {
    container.registerSingleton<ReembolsoSolicitacaoRepository>(
      DiTokens.reembolsoSolicitacaoRepository,
      ReembolsoSolicitacaoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.dadosBancariosRepository)) {
    container.registerSingleton<DadosBancariosRepository>(
      DiTokens.dadosBancariosRepository,
      DadosBancariosRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.timesheetRepository)) {
    container.registerSingleton<TimesheetRepository>(
      DiTokens.timesheetRepository,
      TimesheetRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.minhaJornadaRepository)) {
    container.registerSingleton<MinhaJornadaRepository>(
      DiTokens.minhaJornadaRepository,
      MinhaJornadaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.felizometroRepository)) {
    container.registerSingleton<FelizometroRepository>(
      DiTokens.felizometroRepository,
      FelizometroRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.notificacaoRepository)) {
    container.registerSingleton<NotificacaoRepository>(
      DiTokens.notificacaoRepository,
      NotificacaoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.skillsDashboardRepository)) {
    container.registerSingleton<SkillsDashboardRepository>(
      DiTokens.skillsDashboardRepository,
      SkillsDashboardRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.skillsRepository)) {
    container.registerSingleton<SkillsRepository>(
      DiTokens.skillsRepository,
      SkillsRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.relatorioMinhaJornadaRepository)) {
    container.registerSingleton<RelatorioMinhaJornadaRepository>(
      DiTokens.relatorioMinhaJornadaRepository,
      RelatorioMinhaJornadaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.campanhaRepository)) {
    container.registerSingleton<CampanhaRepository>(
      DiTokens.campanhaRepository,
      CampanhaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.viaCepRepository)) {
    container.registerSingleton<ViaCepRepository>(
      DiTokens.viaCepRepository,
      ViaCepRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.aniversariantesRepository)) {
    container.registerSingleton<AniversariantesRepository>(
      DiTokens.aniversariantesRepository,
      AniversariantesRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.avaliacaoRepository)) {
    container.registerSingleton<AvaliacaoRepository>(
      DiTokens.avaliacaoRepository,
      AvaliacaoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.canalDenunciaRepository)) {
    container.registerSingleton<CanalDenunciaRepository>(
      DiTokens.canalDenunciaRepository,
      CanalDenunciaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.gestaoDesempenhoRepository)) {
    container.registerSingleton<GestaoDesempenhoRepository>(
      DiTokens.gestaoDesempenhoRepository,
      GestaoDesempenhoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.mapaRelacionamentoRepository)) {
    container.registerSingleton<MapaRelacionamentoRepository>(
      DiTokens.mapaRelacionamentoRepository,
      MapaRelacionamentoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.vcx360Repository)) {
    container.registerSingleton<Vcx360Repository>(
      DiTokens.vcx360Repository,
      Vcx360RepositoryImpl,
    )
  }

  // Agenda Gestor module (Aba Agenda VCX 360)
  if (!container.isRegistered(DiTokens.agendasApi)) {
    container.registerSingleton(DiTokens.agendasApi, AgendasApi)
  }

  if (!container.isRegistered(DiTokens.interacoesApi)) {
    container.registerSingleton(DiTokens.interacoesApi, InteracoesApi)
  }

  if (!container.isRegistered(DiTokens.acoesApi)) {
    container.registerSingleton(DiTokens.acoesApi, AcoesApi)
  }

  if (!container.isRegistered(DiTokens.agendaGestorRepository)) {
    container.registerSingleton<AgendaGestorRepository>(
      DiTokens.agendaGestorRepository,
      AgendaGestorRepositoryImpl,
    )
  }

  // Dashboard Comercial - Encontros Big Numbers
  if (!container.isRegistered(DiTokens.encontrosBigNumbersApi)) {
    container.registerSingleton(DiTokens.encontrosBigNumbersApi, EncontrosBigNumbersApi)
  }
  if (!container.isRegistered(DiTokens.encontrosBigNumbersRepository)) {
    container.registerSingleton<EncontrosBigNumbersRepository>(
      DiTokens.encontrosBigNumbersRepository,
      EncontrosBigNumbersRepositoryImpl,
    )
  }
  if (!container.isRegistered(ObterEncontrosBigNumbersUseCase)) {
    container.registerSingleton(ObterEncontrosBigNumbersUseCase, ObterEncontrosBigNumbersUseCase)
  }
  if (!container.isRegistered(ObterBigNumbersCategoriaUseCase)) {
    container.registerSingleton(ObterBigNumbersCategoriaUseCase, ObterBigNumbersCategoriaUseCase)
  }
  if (!container.isRegistered(ObterAgendaRealizadaDetalheUseCase)) {
    container.registerSingleton(ObterAgendaRealizadaDetalheUseCase, ObterAgendaRealizadaDetalheUseCase)
  }
  if (!container.isRegistered(ObterClientesImpactadosDetalheUseCase)) {
    container.registerSingleton(ObterClientesImpactadosDetalheUseCase, ObterClientesImpactadosDetalheUseCase)
  }
  if (!container.isRegistered(ObterAgendaSemInteracaoDetalheUseCase)) {
    container.registerSingleton(ObterAgendaSemInteracaoDetalheUseCase, ObterAgendaSemInteracaoDetalheUseCase)
  }
  if (!container.isRegistered(ObterAcoesEmAtrasoDetalheUseCase)) {
    container.registerSingleton(ObterAcoesEmAtrasoDetalheUseCase, ObterAcoesEmAtrasoDetalheUseCase)
  }
  if (!container.isRegistered(ObterCategoriaComInteracaoDetalheUseCase)) {
    container.registerSingleton(ObterCategoriaComInteracaoDetalheUseCase, ObterCategoriaComInteracaoDetalheUseCase)
  }
  // Microsoft Graph / Teams (Agendas Comerciais)
  if (!container.isRegistered(DiTokens.microsoftGraphApi)) {
    container.registerSingleton(DiTokens.microsoftGraphApi, MicrosoftGraphApi)
  }
  if (!container.isRegistered(DiTokens.graphApiRepository)) {
    container.registerSingleton<GraphApiRepository>(
      DiTokens.graphApiRepository,
      GraphApiRepositoryImpl,
    )
  }
  if (!container.isRegistered(DiTokens.criarReuniaoTeamsUseCase)) {
    container.registerSingleton(
      DiTokens.criarReuniaoTeamsUseCase,
      CriarReuniaoTeamsUseCase,
    )
  }

  if (!container.isRegistered(BuscarAgendaGestorUseCase)) {
    container.registerSingleton(BuscarAgendaGestorUseCase, BuscarAgendaGestorUseCase)
  }

  if (!container.isRegistered(CriarAgendaUseCase)) {
    container.registerSingleton(CriarAgendaUseCase, CriarAgendaUseCase)
  }

  if (!container.isRegistered(AtualizarAgendaUseCase)) {
    container.registerSingleton(AtualizarAgendaUseCase, AtualizarAgendaUseCase)
  }

  if (!container.isRegistered(DeletarAgendaUseCase)) {
    container.registerSingleton(DeletarAgendaUseCase, DeletarAgendaUseCase)
  }

  if (!container.isRegistered(InserirInteracaoIaUseCase)) {
    container.registerSingleton(InserirInteracaoIaUseCase, InserirInteracaoIaUseCase)
  }

  if (!container.isRegistered(AtualizarInteracaoIaUseCase)) {
    container.registerSingleton(AtualizarInteracaoIaUseCase, AtualizarInteracaoIaUseCase)
  }

  if (!container.isRegistered(DeletarInteracaoIaUseCase)) {
    container.registerSingleton(DeletarInteracaoIaUseCase, DeletarInteracaoIaUseCase)
  }

  if (!container.isRegistered(InserirArquivoEncontroUseCase)) {
    container.registerSingleton(InserirArquivoEncontroUseCase, InserirArquivoEncontroUseCase)
  }

  if (!container.isRegistered(DeletarArquivoEncontroUseCase)) {
    container.registerSingleton(DeletarArquivoEncontroUseCase, DeletarArquivoEncontroUseCase)
  }

  if (!container.isRegistered(AtualizarStatusAcoesUseCase)) {
    container.registerSingleton(AtualizarStatusAcoesUseCase, AtualizarStatusAcoesUseCase)
  }

  if (!container.isRegistered(InserirComentarioAcaoUseCase)) {
    container.registerSingleton(InserirComentarioAcaoUseCase, InserirComentarioAcaoUseCase)
  }

  if (!container.isRegistered(AceitarRecusarConviteAgendaUseCase)) {
    container.registerSingleton(AceitarRecusarConviteAgendaUseCase, AceitarRecusarConviteAgendaUseCase)
  }

    if (!container.isRegistered(CarregarAgendaDetalheUseCase)) {
    container.registerSingleton(CarregarAgendaDetalheUseCase, CarregarAgendaDetalheUseCase)
  }

  if (!container.isRegistered(BuscarAgendasFilhosUseCase)) {
    container.registerSingleton(BuscarAgendasFilhosUseCase, BuscarAgendasFilhosUseCase)
  }

  if (!container.isRegistered(DiTokens.mapaAlocacaoRepository)) {
    container.registerSingleton<MapaAlocacaoRepository>(
      DiTokens.mapaAlocacaoRepository,
      MapaAlocacaoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.integracaoBancariaRepository)) {
    container.registerSingleton<IntegracaoBancariaRepository>(
      DiTokens.integracaoBancariaRepository,
      IntegracaoBancariaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.minhaEquipeRepository)) {
    container.registerSingleton<MinhaEquipeRepository>(
      DiTokens.minhaEquipeRepository,
      MinhaEquipeRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.parceriaRepository)) {
    container.registerSingleton<ParceriaRepository>(
      DiTokens.parceriaRepository,
      ParceriaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.organogramaRepository)) {
    container.registerSingleton<OrganogramaRepository>(
      DiTokens.organogramaRepository,
      OrganogramaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.feedback360Repository)) {
    container.registerSingleton<Feedback360Repository>(
      DiTokens.feedback360Repository,
      Feedback360RepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.comunicacaoFeedRepository)) {
    container.registerSingleton<ComunicacaoFeedRepository>(
      DiTokens.comunicacaoFeedRepository,
      ComunicacaoFeedRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.comunicacaoGrupoRepository)) {
    container.registerSingleton<ComunicacaoGrupoRepository>(
      DiTokens.comunicacaoGrupoRepository,
      ComunicacaoGrupoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.comunicacaoAnalyticsRepository)) {
    container.registerSingleton<ComunicacaoAnalyticsRepository>(
      DiTokens.comunicacaoAnalyticsRepository,
      ComunicacaoAnalyticsRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.comunicacaoIaRepository)) {
    container.registerSingleton<ComunicacaoIaRepository>(
      DiTokens.comunicacaoIaRepository,
      ComunicacaoIaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.comunicacaoComunidadeRepository)) {
    container.registerSingleton<ComunicacaoComunidadeRepository>(
      DiTokens.comunicacaoComunidadeRepository,
      ComunicacaoComunidadeRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.comunicacaoProfissionaisRepository)) {
    container.registerSingleton<ComunicacaoProfissionaisRepository>(
      DiTokens.comunicacaoProfissionaisRepository,
      ComunicacaoProfissionaisRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.grupoAcessoRepository)) {
    container.registerSingleton<GrupoAcessoRepository>(
      DiTokens.grupoAcessoRepository,
      GrupoAcessoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.funcionalidadeSistemaRepository)) {
    container.registerSingleton<FuncionalidadeSistemaRepository>(
      DiTokens.funcionalidadeSistemaRepository,
      FuncionalidadeSistemaRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.clienteRepository)) {
    container.registerSingleton<ClienteRepository>(
      DiTokens.clienteRepository,
      ClienteRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.permissionamentoRepository)) {
    container.registerSingleton<PermissionamentoRepository>(
      DiTokens.permissionamentoRepository,
      PermissionamentoRepositoryImpl,
    )
  }

  if (!container.isRegistered(DiTokens.vcxRepository)) {
    container.registerSingleton<VcxRepository>(
      DiTokens.vcxRepository,
      VcxRepositoryImpl,
    )
  }

  // Use Cases
  if (!container.isRegistered(GetShowmeProfileUseCase)) {
    container.registerSingleton(GetShowmeProfileUseCase, GetShowmeProfileUseCase)
  }

  if (!container.isRegistered(GetMenuResourcesUseCase)) {
    container.registerSingleton(GetMenuResourcesUseCase, GetMenuResourcesUseCase)
  }

  if (!container.isRegistered(LogoutUseCase)) {
    container.registerSingleton(LogoutUseCase, LogoutUseCase)
  }

  if (!container.isRegistered(CriarFeedback360UseCase)) {
    container.registerSingleton(CriarFeedback360UseCase, CriarFeedback360UseCase)
  }
  if (!container.isRegistered(AtualizarFeedback360UseCase)) {
    container.registerSingleton(AtualizarFeedback360UseCase, AtualizarFeedback360UseCase)
  }
  if (!container.isRegistered(ListarAvaliacoesFeedback360UseCase)) {
    container.registerSingleton(ListarAvaliacoesFeedback360UseCase, ListarAvaliacoesFeedback360UseCase)
  }
  if (!container.isRegistered(ListarRelacionamentosFeedback360UseCase)) {
    container.registerSingleton(ListarRelacionamentosFeedback360UseCase, ListarRelacionamentosFeedback360UseCase)
  }
  if (!container.isRegistered(ListarEnviadosFeedback360UseCase)) {
    container.registerSingleton(ListarEnviadosFeedback360UseCase, ListarEnviadosFeedback360UseCase)
  }
  if (!container.isRegistered(ListarRecebidosFeedback360UseCase)) {
    container.registerSingleton(ListarRecebidosFeedback360UseCase, ListarRecebidosFeedback360UseCase)
  }

  if (!container.isRegistered(ListarComunicacaoFeedUseCase)) {
    container.registerSingleton(ListarComunicacaoFeedUseCase, ListarComunicacaoFeedUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoGruposUseCase)) {
    container.registerSingleton(ListarComunicacaoGruposUseCase, ListarComunicacaoGruposUseCase)
  }
  if (!container.isRegistered(ObterPermissoesUsuarioLogadoUseCase)) {
    container.registerSingleton(ObterPermissoesUsuarioLogadoUseCase, ObterPermissoesUsuarioLogadoUseCase)
  }
  if (!container.isRegistered(ObterAnalyticsResumoUseCase)) {
    container.registerSingleton(ObterAnalyticsResumoUseCase, ObterAnalyticsResumoUseCase)
  }
  if (!container.isRegistered(ProcessarConteudoIaComunicacaoUseCase)) {
    container.registerSingleton(
      ProcessarConteudoIaComunicacaoUseCase,
      ProcessarConteudoIaComunicacaoUseCase,
    )
  }
  if (!container.isRegistered(CriarComunicacaoGrupoUseCase)) {
    container.registerSingleton(CriarComunicacaoGrupoUseCase, CriarComunicacaoGrupoUseCase)
  }
  if (!container.isRegistered(ObterComunicacaoGrupoPorIdUseCase)) {
    container.registerSingleton(ObterComunicacaoGrupoPorIdUseCase, ObterComunicacaoGrupoPorIdUseCase)
  }
  if (!container.isRegistered(AtualizarComunicacaoGrupoUseCase)) {
    container.registerSingleton(AtualizarComunicacaoGrupoUseCase, AtualizarComunicacaoGrupoUseCase)
  }
  if (!container.isRegistered(DeletarComunicacaoGrupoUseCase)) {
    container.registerSingleton(DeletarComunicacaoGrupoUseCase, DeletarComunicacaoGrupoUseCase)
  }
  if (!container.isRegistered(ArquivarComunidadeUseCase)) {
    container.registerSingleton(ArquivarComunidadeUseCase, ArquivarComunidadeUseCase)
  }
  if (!container.isRegistered(AtualizarComunidadeUseCase)) {
    container.registerSingleton(AtualizarComunidadeUseCase, AtualizarComunidadeUseCase)
  }
  if (!container.isRegistered(CriarComunicacaoComunidadeUseCase)) {
    container.registerSingleton(CriarComunicacaoComunidadeUseCase, CriarComunicacaoComunidadeUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoComunidadesUseCase)) {
    container.registerSingleton(ListarComunicacaoComunidadesUseCase, ListarComunicacaoComunidadesUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoProfissionaisUseCase)) {
    container.registerSingleton(ListarComunicacaoProfissionaisUseCase, ListarComunicacaoProfissionaisUseCase)
  }
  if (!container.isRegistered(ListarColaboradoresDisponiveisUseCase)) {
    container.registerSingleton(ListarColaboradoresDisponiveisUseCase, ListarColaboradoresDisponiveisUseCase)
  }
  if (!container.isRegistered(ConfirmarLeituraObrigatoriaUseCase)) {
    container.registerSingleton(ConfirmarLeituraObrigatoriaUseCase, ConfirmarLeituraObrigatoriaUseCase)
  }
  if (!container.isRegistered(ArquivarPublicacaoUseCase)) {
    container.registerSingleton(ArquivarPublicacaoUseCase, ArquivarPublicacaoUseCase)
  }
  if (!container.isRegistered(OcultarNoFeedPublicacaoUseCase)) {
    container.registerSingleton(OcultarNoFeedPublicacaoUseCase, OcultarNoFeedPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirPublicacaoUseCase)) {
    container.registerSingleton(ExcluirPublicacaoUseCase, ExcluirPublicacaoUseCase)
  }
  if (!container.isRegistered(AprovarPublicacaoUseCase)) {
    container.registerSingleton(AprovarPublicacaoUseCase, AprovarPublicacaoUseCase)
  }
  if (!container.isRegistered(PublicarAgoraPublicacaoUseCase)) {
    container.registerSingleton(PublicarAgoraPublicacaoUseCase, PublicarAgoraPublicacaoUseCase)
  }
  if (!container.isRegistered(AdicionarAnexosPublicacaoUseCase)) {
    container.registerSingleton(AdicionarAnexosPublicacaoUseCase, AdicionarAnexosPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirAnexoPublicacaoUseCase)) {
    container.registerSingleton(ExcluirAnexoPublicacaoUseCase, ExcluirAnexoPublicacaoUseCase)
  }
  if (!container.isRegistered(RejeitarPublicacaoUseCase)) {
    container.registerSingleton(RejeitarPublicacaoUseCase, RejeitarPublicacaoUseCase)
  }
  if (!container.isRegistered(AtualizarPublicacaoUseCase)) {
    container.registerSingleton(AtualizarPublicacaoUseCase, AtualizarPublicacaoUseCase)
  }
  if (!container.isRegistered(CriarPublicacaoUseCase)) {
    container.registerSingleton(CriarPublicacaoUseCase, CriarPublicacaoUseCase)
  }
  if (!container.isRegistered(InserirComentarioPublicacaoUseCase)) {
    container.registerSingleton(InserirComentarioPublicacaoUseCase, InserirComentarioPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirInteracaoComentarioUseCase)) {
    container.registerSingleton(ExcluirInteracaoComentarioUseCase, ExcluirInteracaoComentarioUseCase)
  }
  if (!container.isRegistered(ExcluirInteracaoPublicacaoUseCase)) {
    container.registerSingleton(ExcluirInteracaoPublicacaoUseCase, ExcluirInteracaoPublicacaoUseCase)
  }
  if (!container.isRegistered(ObterConfirmacoesLeituraUseCase)) {
    container.registerSingleton(ObterConfirmacoesLeituraUseCase, ObterConfirmacoesLeituraUseCase)
  }
  if (!container.isRegistered(ObterPublicacaoPorIdUseCase)) {
    container.registerSingleton(ObterPublicacaoPorIdUseCase, ObterPublicacaoPorIdUseCase)
  }
  if (!container.isRegistered(InserirInteracaoComentarioUseCase)) {
    container.registerSingleton(InserirInteracaoComentarioUseCase, InserirInteracaoComentarioUseCase)
  }
  if (!container.isRegistered(InserirInteracaoPublicacaoUseCase)) {
    container.registerSingleton(InserirInteracaoPublicacaoUseCase, InserirInteracaoPublicacaoUseCase)
  }
  if (!container.isRegistered(AtualizarComentarioPublicacaoUseCase)) {
    container.registerSingleton(AtualizarComentarioPublicacaoUseCase, AtualizarComentarioPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirComentarioPublicacaoUseCase)) {
    container.registerSingleton(ExcluirComentarioPublicacaoUseCase, ExcluirComentarioPublicacaoUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoFeedUseCase)) {
    container.registerSingleton(ListarComunicacaoFeedUseCase, ListarComunicacaoFeedUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoGruposUseCase)) {
    container.registerSingleton(ListarComunicacaoGruposUseCase, ListarComunicacaoGruposUseCase)
  }
  if (!container.isRegistered(ObterPermissoesUsuarioLogadoUseCase)) {
    container.registerSingleton(ObterPermissoesUsuarioLogadoUseCase, ObterPermissoesUsuarioLogadoUseCase)
  }
  if (!container.isRegistered(ObterAnalyticsResumoUseCase)) {
    container.registerSingleton(ObterAnalyticsResumoUseCase, ObterAnalyticsResumoUseCase)
  }

  if (!container.isRegistered(ProcessarConteudoIaComunicacaoUseCase)) {
    container.registerSingleton(
      ProcessarConteudoIaComunicacaoUseCase,
      ProcessarConteudoIaComunicacaoUseCase,
    )
  }


  if (!container.isRegistered(CriarComunicacaoGrupoUseCase)) {
    container.registerSingleton(CriarComunicacaoGrupoUseCase, CriarComunicacaoGrupoUseCase)
  }
  if (!container.isRegistered(ObterComunicacaoGrupoPorIdUseCase)) {
    container.registerSingleton(ObterComunicacaoGrupoPorIdUseCase, ObterComunicacaoGrupoPorIdUseCase)
  }
  if (!container.isRegistered(AtualizarComunicacaoGrupoUseCase)) {
    container.registerSingleton(AtualizarComunicacaoGrupoUseCase, AtualizarComunicacaoGrupoUseCase)
  }
  if (!container.isRegistered(DeletarComunicacaoGrupoUseCase)) {
    container.registerSingleton(DeletarComunicacaoGrupoUseCase, DeletarComunicacaoGrupoUseCase)
  }

  if (!container.isRegistered(CriarComunicacaoComunidadeUseCase)) {
    container.registerSingleton(CriarComunicacaoComunidadeUseCase, CriarComunicacaoComunidadeUseCase)
  }

  if (!container.isRegistered(ArquivarComunidadeUseCase)) {
    container.registerSingleton(ArquivarComunidadeUseCase, ArquivarComunidadeUseCase)
  }
  if (!container.isRegistered(AtualizarComunidadeUseCase)) {
    container.registerSingleton(AtualizarComunidadeUseCase, AtualizarComunidadeUseCase)
  }
  if (!container.isRegistered(CriarComunicacaoComunidadeUseCase)) {
    container.registerSingleton(CriarComunicacaoComunidadeUseCase, CriarComunicacaoComunidadeUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoComunidadesUseCase)) {
    container.registerSingleton(ListarComunicacaoComunidadesUseCase, ListarComunicacaoComunidadesUseCase)
  }
  if (!container.isRegistered(ListarComunicacaoProfissionaisUseCase)) {
    container.registerSingleton(ListarComunicacaoProfissionaisUseCase, ListarComunicacaoProfissionaisUseCase)
  }

  if (!container.isRegistered(ListarColaboradoresDisponiveisUseCase)) {
    container.registerSingleton(ListarColaboradoresDisponiveisUseCase, ListarColaboradoresDisponiveisUseCase)
  }

  if (!container.isRegistered(ConfirmarLeituraObrigatoriaUseCase)) {
    container.registerSingleton(ConfirmarLeituraObrigatoriaUseCase, ConfirmarLeituraObrigatoriaUseCase)
  }
  if (!container.isRegistered(ArquivarPublicacaoUseCase)) {
    container.registerSingleton(ArquivarPublicacaoUseCase, ArquivarPublicacaoUseCase)
  }
  if (!container.isRegistered(OcultarNoFeedPublicacaoUseCase)) {
    container.registerSingleton(OcultarNoFeedPublicacaoUseCase, OcultarNoFeedPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirPublicacaoUseCase)) {
    container.registerSingleton(ExcluirPublicacaoUseCase, ExcluirPublicacaoUseCase)
  }
  if (!container.isRegistered(AprovarPublicacaoUseCase)) {
    container.registerSingleton(AprovarPublicacaoUseCase, AprovarPublicacaoUseCase)
  }
  if (!container.isRegistered(PublicarAgoraPublicacaoUseCase)) {
    container.registerSingleton(PublicarAgoraPublicacaoUseCase, PublicarAgoraPublicacaoUseCase)
  }
  if (!container.isRegistered(AdicionarAnexosPublicacaoUseCase)) {
    container.registerSingleton(AdicionarAnexosPublicacaoUseCase, AdicionarAnexosPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirAnexoPublicacaoUseCase)) {
    container.registerSingleton(ExcluirAnexoPublicacaoUseCase, ExcluirAnexoPublicacaoUseCase)
  }
  if (!container.isRegistered(RejeitarPublicacaoUseCase)) {
    container.registerSingleton(RejeitarPublicacaoUseCase, RejeitarPublicacaoUseCase)
  }
  if (!container.isRegistered(AtualizarPublicacaoUseCase)) {
    container.registerSingleton(AtualizarPublicacaoUseCase, AtualizarPublicacaoUseCase)
  }
  if (!container.isRegistered(CriarPublicacaoUseCase)) {
    container.registerSingleton(CriarPublicacaoUseCase, CriarPublicacaoUseCase)
  }
  if (!container.isRegistered(InserirComentarioPublicacaoUseCase)) {
    container.registerSingleton(InserirComentarioPublicacaoUseCase, InserirComentarioPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirInteracaoComentarioUseCase)) {
    container.registerSingleton(ExcluirInteracaoComentarioUseCase, ExcluirInteracaoComentarioUseCase)
  }
  if (!container.isRegistered(ExcluirInteracaoPublicacaoUseCase)) {
    container.registerSingleton(ExcluirInteracaoPublicacaoUseCase, ExcluirInteracaoPublicacaoUseCase)
  }
  if (!container.isRegistered(ObterConfirmacoesLeituraUseCase)) {
    container.registerSingleton(ObterConfirmacoesLeituraUseCase, ObterConfirmacoesLeituraUseCase)
  }
  if (!container.isRegistered(ObterPublicacaoPorIdUseCase)) {
    container.registerSingleton(ObterPublicacaoPorIdUseCase, ObterPublicacaoPorIdUseCase)
  }
  if (!container.isRegistered(InserirInteracaoComentarioUseCase)) {
    container.registerSingleton(InserirInteracaoComentarioUseCase, InserirInteracaoComentarioUseCase)
  }
  if (!container.isRegistered(InserirInteracaoPublicacaoUseCase)) {
    container.registerSingleton(InserirInteracaoPublicacaoUseCase, InserirInteracaoPublicacaoUseCase)
  }
  if (!container.isRegistered(AtualizarComentarioPublicacaoUseCase)) {
    container.registerSingleton(AtualizarComentarioPublicacaoUseCase, AtualizarComentarioPublicacaoUseCase)
  }
  if (!container.isRegistered(ExcluirComentarioPublicacaoUseCase)) {
    container.registerSingleton(ExcluirComentarioPublicacaoUseCase, ExcluirComentarioPublicacaoUseCase)
  }

  if (!container.isRegistered(ListarColaboradoresOrgUseCase)) {
    container.registerSingleton(ListarColaboradoresOrgUseCase, ListarColaboradoresOrgUseCase)
  }

  if (!container.isRegistered(ListarGruposAcessoUseCase)) {
    container.registerSingleton(ListarGruposAcessoUseCase, ListarGruposAcessoUseCase)
  }

  if (!container.isRegistered(CriarGrupoAcessoUseCase)) {
    container.registerSingleton(CriarGrupoAcessoUseCase, CriarGrupoAcessoUseCase)
  }

  if (!container.isRegistered(EditarGrupoAcessoUseCase)) {
    container.registerSingleton(EditarGrupoAcessoUseCase, EditarGrupoAcessoUseCase)
  }

  if (!container.isRegistered(ListarFuncionalidadesSistemaUseCase)) {
    container.registerSingleton(ListarFuncionalidadesSistemaUseCase, ListarFuncionalidadesSistemaUseCase)
  }

  if (!container.isRegistered(AtribuirFuncionalidadeGrupoUseCase)) {
    container.registerSingleton(AtribuirFuncionalidadeGrupoUseCase, AtribuirFuncionalidadeGrupoUseCase)
  }

  if (!container.isRegistered(RemoverFuncionalidadeGrupoUseCase)) {
    container.registerSingleton(RemoverFuncionalidadeGrupoUseCase, RemoverFuncionalidadeGrupoUseCase)
  }

  if (!container.isRegistered(AdicionarUsuarioGrupoUseCase)) {
    container.registerSingleton(AdicionarUsuarioGrupoUseCase, AdicionarUsuarioGrupoUseCase)
  }

  if (!container.isRegistered(RemoverUsuarioGrupoUseCase)) {
    container.registerSingleton(RemoverUsuarioGrupoUseCase, RemoverUsuarioGrupoUseCase)
  }

  if (!container.isRegistered(ObterUsuarioIdPorCpfUseCase)) {
    container.registerSingleton(ObterUsuarioIdPorCpfUseCase, ObterUsuarioIdPorCpfUseCase)
  }

  if (!container.isRegistered(SendTokenEmailUseCase)) {
    container.registerSingleton(SendTokenEmailUseCase, SendTokenEmailUseCase)
  }

  if (!container.isRegistered(ValidateTokenEmailUseCase)) {
    container.registerSingleton(ValidateTokenEmailUseCase, ValidateTokenEmailUseCase)
  }

  if (!container.isRegistered(CadastrarUsuarioUseCase)) {
    container.registerSingleton(CadastrarUsuarioUseCase, CadastrarUsuarioUseCase)
  }

  if (!container.isRegistered(GetAccessTokenUseCase)) {
    container.registerSingleton(GetAccessTokenUseCase, GetAccessTokenUseCase)
  }

  if (!container.isRegistered(GetColaboradoresUseCase)) {
    container.registerSingleton(GetColaboradoresUseCase, GetColaboradoresUseCase)
  }

  if (!container.isRegistered(InserirColaboradorUseCase)) {
    container.registerSingleton(InserirColaboradorUseCase, InserirColaboradorUseCase)
  }

  if (!container.isRegistered(EditarColaboradorUseCase)) {
    container.registerSingleton(EditarColaboradorUseCase, EditarColaboradorUseCase)
  }

  if (!container.isRegistered(BuscarDadosColaboradorUseCase)) {
    container.registerSingleton(BuscarDadosColaboradorUseCase, BuscarDadosColaboradorUseCase)
  }
  if (!container.isRegistered(ObterDadosColaboradorUseCase)) {
    container.registerSingleton(ObterDadosColaboradorUseCase, ObterDadosColaboradorUseCase)
  }
  if (!container.isRegistered(EditarDadosColaboradorUseCase)) {
    container.registerSingleton(EditarDadosColaboradorUseCase, EditarDadosColaboradorUseCase)
  }
  if (!container.isRegistered(BaixarCertificadoColaboradorUseCase)) {
    container.registerSingleton(BaixarCertificadoColaboradorUseCase, BaixarCertificadoColaboradorUseCase)
  }

  if (!container.isRegistered(ListarEscolaridadeColaboradorUseCase)) {
    container.registerSingleton(ListarEscolaridadeColaboradorUseCase, ListarEscolaridadeColaboradorUseCase)
  }
  if (!container.isRegistered(GerarRelatorioColaboradoresUseCase)) {
    container.registerSingleton(GerarRelatorioColaboradoresUseCase, GerarRelatorioColaboradoresUseCase)
  }

  if (!container.isRegistered(GetProjetosUseCase)) {
    container.registerSingleton(GetProjetosUseCase, GetProjetosUseCase)
  }

  if (!container.isRegistered(GetHoleritesUseCase)) {
    container.registerSingleton(GetHoleritesUseCase, GetHoleritesUseCase)
  }

  if (!container.isRegistered(GetReembolsosUseCase)) {
    container.registerSingleton(GetReembolsosUseCase, GetReembolsosUseCase)
  }

  if (!container.isRegistered(GetNotasFiscaisUseCase)) {
    container.registerSingleton(GetNotasFiscaisUseCase, GetNotasFiscaisUseCase)
  }

  if (!container.isRegistered(GetManagersUseCase)) {
    container.registerSingleton(GetManagersUseCase, GetManagersUseCase)
  }

  if (!container.isRegistered(GetUserProfileUseCase)) {
    container.registerSingleton(GetUserProfileUseCase, GetUserProfileUseCase)
  }

  if (!container.isRegistered(GetProjetosColaboradorUseCase)) {
    container.registerSingleton(GetProjetosColaboradorUseCase, GetProjetosColaboradorUseCase)
  }

  if (!container.isRegistered(GetVerbasUseCase)) {
    container.registerSingleton(GetVerbasUseCase, GetVerbasUseCase)
  }

  if (!container.isRegistered(AnalisarComprovantesUseCase)) {
    container.registerSingleton(AnalisarComprovantesUseCase, AnalisarComprovantesUseCase)
  }

  if (!container.isRegistered(InserirSolicitacaoUseCase)) {
    container.registerSingleton(InserirSolicitacaoUseCase, InserirSolicitacaoUseCase)
  }

  if (!container.isRegistered(GetDadosBancariosUseCase)) {
    container.registerSingleton(GetDadosBancariosUseCase, GetDadosBancariosUseCase)
  }

  if (!container.isRegistered(CriarDadosBancariosUseCase)) {
    container.registerSingleton(CriarDadosBancariosUseCase, CriarDadosBancariosUseCase)
  }

  if (!container.isRegistered(EditarDadosBancariosUseCase)) {
    container.registerSingleton(EditarDadosBancariosUseCase, EditarDadosBancariosUseCase)
  }

  if (!container.isRegistered(ListarDiretoriasDisponiveisUseCase)) {
    container.registerSingleton(ListarDiretoriasDisponiveisUseCase, ListarDiretoriasDisponiveisUseCase)
  }

  // Timesheet Use Cases
  if (!container.isRegistered(ListarVigenciasColaboradorUseCase)) {
    container.registerSingleton(ListarVigenciasColaboradorUseCase, ListarVigenciasColaboradorUseCase)
  }

  if (!container.isRegistered(ListarProjetosComAtividadesUseCase)) {
    container.registerSingleton(ListarProjetosComAtividadesUseCase, ListarProjetosComAtividadesUseCase)
  }

  if (!container.isRegistered(ListarTemplateSemanaVigenciaUseCase)) {
    container.registerSingleton(ListarTemplateSemanaVigenciaUseCase, ListarTemplateSemanaVigenciaUseCase)
  }

  if (!container.isRegistered(ListarApontamentosPorVigenciaUseCase)) {
    container.registerSingleton(ListarApontamentosPorVigenciaUseCase, ListarApontamentosPorVigenciaUseCase)
  }

  if (!container.isRegistered(ApontarHorasEmLoteUseCase)) {
    container.registerSingleton(ApontarHorasEmLoteUseCase, ApontarHorasEmLoteUseCase)
  }

  if (!container.isRegistered(EditarApontamentoUseCase)) {
    container.registerSingleton(EditarApontamentoUseCase, EditarApontamentoUseCase)
  }

  if (!container.isRegistered(DeletarApontamentoUseCase)) {
    container.registerSingleton(DeletarApontamentoUseCase, DeletarApontamentoUseCase)
  }

  if (!container.isRegistered(BuscarPeriodoFechadoUseCase)) {
    container.registerSingleton(BuscarPeriodoFechadoUseCase, BuscarPeriodoFechadoUseCase)
  }

  if (!container.isRegistered(ListarColaboradoresEApontamentosPorGestorUseCase)) {
    container.registerSingleton(ListarColaboradoresEApontamentosPorGestorUseCase, ListarColaboradoresEApontamentosPorGestorUseCase)
  }

  if (!container.isRegistered(EnviarEmailNotificacaoAprovadoresUseCase)) {
    container.registerSingleton(EnviarEmailNotificacaoAprovadoresUseCase, EnviarEmailNotificacaoAprovadoresUseCase)
  }

  if (!container.isRegistered(FecharAlterarPeriodoUseCase)) {
    container.registerSingleton(FecharAlterarPeriodoUseCase, FecharAlterarPeriodoUseCase)
  }

  if (!container.isRegistered(ListarVigenciasApontamentosGerenteDeProjetoUseCase)) {
    container.registerSingleton(ListarVigenciasApontamentosGerenteDeProjetoUseCase, ListarVigenciasApontamentosGerenteDeProjetoUseCase)
  }

  if (!container.isRegistered(ListarProjetosVisaoGerenteDeProjetoUseCase)) {
    container.registerSingleton(ListarProjetosVisaoGerenteDeProjetoUseCase, ListarProjetosVisaoGerenteDeProjetoUseCase)
  }

  if (!container.isRegistered(ListarGerenteAdmDosColabsGPUseCase)) {
    container.registerSingleton(ListarGerenteAdmDosColabsGPUseCase, ListarGerenteAdmDosColabsGPUseCase)
  }

  if (!container.isRegistered(ListarColaboradoresVinculadosGerenteDeProjetoUseCase)) {
    container.registerSingleton(ListarColaboradoresVinculadosGerenteDeProjetoUseCase, ListarColaboradoresVinculadosGerenteDeProjetoUseCase)
  }

  // Minha Jornada Use Cases
  if (!container.isRegistered(GetMinhaJornadaContextUseCase)) {
    container.registerSingleton(GetMinhaJornadaContextUseCase, GetMinhaJornadaContextUseCase)
  }

  if (!container.isRegistered(UpdateSkillInterestUseCase)) {
    container.registerSingleton(UpdateSkillInterestUseCase, UpdateSkillInterestUseCase)
  }

  if (!container.isRegistered(UpdateSkillLevelUseCase)) {
    container.registerSingleton(UpdateSkillLevelUseCase, UpdateSkillLevelUseCase)
  }

  if (!container.isRegistered(CreatePdiUseCase)) {
    container.registerSingleton(CreatePdiUseCase, CreatePdiUseCase)
  }

  if (!container.isRegistered(GetPDIsColaboradorUseCase)) {
    container.registerSingleton(GetPDIsColaboradorUseCase, GetPDIsColaboradorUseCase)
  }

  if (!container.isRegistered(EnviarSentimentoUseCase)) {
    container.registerSingleton(EnviarSentimentoUseCase, EnviarSentimentoUseCase)
  }

  if (!container.isRegistered(ContarNotificacoesNaoLidasUseCase)) {
    container.registerSingleton(ContarNotificacoesNaoLidasUseCase, ContarNotificacoesNaoLidasUseCase)
  }

  if (!container.isRegistered(ListarNotificacoesUseCase)) {
    container.registerSingleton(ListarNotificacoesUseCase, ListarNotificacoesUseCase)
  }

  if (!container.isRegistered(MarcarNotificacoesComoLidasUseCase)) {
    container.registerSingleton(MarcarNotificacoesComoLidasUseCase, MarcarNotificacoesComoLidasUseCase)
  }

  if (!container.isRegistered(ListarAlocacoesColabETbdUseCase)) {
    container.registerSingleton(ListarAlocacoesColabETbdUseCase, ListarAlocacoesColabETbdUseCase)
  }

  if (!container.isRegistered(ListarColaboradoresETbdsUseCase)) {
    container.registerSingleton(ListarColaboradoresETbdsUseCase, ListarColaboradoresETbdsUseCase)
  }

  if (!container.isRegistered(ListarProjetosColaboradorMapaUseCase)) {
    container.registerSingleton(ListarProjetosColaboradorMapaUseCase, ListarProjetosColaboradorMapaUseCase)
  }

  if (!container.isRegistered(SubstituirDadosAlocacoesPorPeriodoUseCase)) {
    container.registerSingleton(SubstituirDadosAlocacoesPorPeriodoUseCase, SubstituirDadosAlocacoesPorPeriodoUseCase)
  }

  if (!container.isRegistered(EditarAlocacaoUseCase)) {
    container.registerSingleton(EditarAlocacaoUseCase, EditarAlocacaoUseCase)
  }

  if (!container.isRegistered(RemoverAlocacoesEmLoteUseCase)) {
    container.registerSingleton(RemoverAlocacoesEmLoteUseCase, RemoverAlocacoesEmLoteUseCase)
  }

  if (!container.isRegistered(ExportarRelatorioAlocacoesUseCase)) {
    container.registerSingleton(ExportarRelatorioAlocacoesUseCase, ExportarRelatorioAlocacoesUseCase)
  }

  if (!container.isRegistered(ListarNomesGestoresUseCase)) {
    container.registerSingleton(ListarNomesGestoresUseCase, ListarNomesGestoresUseCase)
  }

  if (!container.isRegistered(ListarStatusProjetoUseCase)) {
    container.registerSingleton(ListarStatusProjetoUseCase, ListarStatusProjetoUseCase)
  }

  if (!container.isRegistered(CadastrarMapaAlocacaoUseCase)) {
    container.registerSingleton(CadastrarMapaAlocacaoUseCase, CadastrarMapaAlocacaoUseCase)
  }

  if (!container.isRegistered(ListarPerfilAlocacaoUseCase)) {
    container.registerSingleton(ListarPerfilAlocacaoUseCase, ListarPerfilAlocacaoUseCase)
  }

  if (!container.isRegistered(AlteraPerfilAlocacaoUseCase)) {
    container.registerSingleton(AlteraPerfilAlocacaoUseCase, AlteraPerfilAlocacaoUseCase)
  }

  if (!container.isRegistered(GetMapaAlocacaoResumoUseCase)) {
    container.registerSingleton(GetMapaAlocacaoResumoUseCase, GetMapaAlocacaoResumoUseCase)
  }

  if (!container.isRegistered(GetMapaAlocacaoRecursoUseCase)) {
    container.registerSingleton(GetMapaAlocacaoRecursoUseCase, GetMapaAlocacaoRecursoUseCase)
  }

  if (!container.isRegistered(ConsultaProjetoHorasUseCase)) {
    container.registerSingleton(ConsultaProjetoHorasUseCase, ConsultaProjetoHorasUseCase)
  }

  // Bot Fourmakers
  if (!container.isRegistered(DiTokens.botFourmakersApi)) {
    container.registerSingleton(DiTokens.botFourmakersApi, BotFourmakersApi)
  }

  if (!container.isRegistered(DiTokens.botFourmakersRepository)) {
    container.registerSingleton<BotFourmakersRepository>(
      DiTokens.botFourmakersRepository,
      BotFourmakersRepositoryImpl
    )
  }

  if (!container.isRegistered(InserirQuestaoUseCase)) {
    container.registerSingleton(InserirQuestaoUseCase, InserirQuestaoUseCase)
  }

  if (!container.isRegistered(ListarChatsUseCase)) {
    container.registerSingleton(ListarChatsUseCase, ListarChatsUseCase)
  }

  if (!container.isRegistered(ListarQuestoesUseCase)) {
    container.registerSingleton(ListarQuestoesUseCase, ListarQuestoesUseCase)
  }

  if (!container.isRegistered(RemoverChatUseCase)) {
    container.registerSingleton(RemoverChatUseCase, RemoverChatUseCase)
  }

  if (!container.isRegistered(InserirFeedbackBotUseCase)) {
    container.registerSingleton(InserirFeedbackBotUseCase, InserirFeedbackBotUseCase)
  }

  // TBD
  if (!container.isRegistered(DiTokens.tbdApi)) {
    container.registerSingleton(DiTokens.tbdApi, TbdApi)
  }

  if (!container.isRegistered(DiTokens.tbdRepository)) {
    container.registerSingleton<TbdRepository>(
      DiTokens.tbdRepository,
      TbdRepositoryImpl
    )
  }

  if (!container.isRegistered(ListarTbdUseCase)) {
    container.registerSingleton(ListarTbdUseCase, ListarTbdUseCase)
  }

  if (!container.isRegistered(InserirTbdUseCase)) {
    container.registerSingleton(InserirTbdUseCase, InserirTbdUseCase)
  }

  if (!container.isRegistered(AtualizarTbdUseCase)) {
    container.registerSingleton(AtualizarTbdUseCase, AtualizarTbdUseCase)
  }
  if (!container.isRegistered(AddSkillToPerfil360UseCase)) {
    container.registerSingleton(AddSkillToPerfil360UseCase, AddSkillToPerfil360UseCase)
  }
  if (!container.isRegistered(RemoveSkillFromPerfil360UseCase)) {
    container.registerSingleton(RemoveSkillFromPerfil360UseCase, RemoveSkillFromPerfil360UseCase)
  }

  if (!container.isRegistered(SuggestNewSkillUseCase)) {
    container.registerSingleton(SuggestNewSkillUseCase, SuggestNewSkillUseCase)
  }

  // Skills Dashboard Use Cases
  if (!container.isRegistered(GetSkillsDashboardDataUseCase)) {
    container.registerSingleton(GetSkillsDashboardDataUseCase, GetSkillsDashboardDataUseCase)
  }
  if (!container.isRegistered(GetSkillLogsPaginatedUseCase)) {
    container.registerSingleton(GetSkillLogsPaginatedUseCase, GetSkillLogsPaginatedUseCase)
  }

  if (!container.isRegistered(DownloadRelatorioLogDetalhadoUseCase)) {
    container.registerSingleton(DownloadRelatorioLogDetalhadoUseCase, DownloadRelatorioLogDetalhadoUseCase)
  }

  if (!container.isRegistered(ListarClientesMapaRelacionamentoUseCase)) {
    container.registerSingleton(ListarClientesMapaRelacionamentoUseCase, ListarClientesMapaRelacionamentoUseCase)
  }

  if (!container.isRegistered(ListarClientesAlternativoMapaRelacionamentoUseCase)) {
    container.registerSingleton(ListarClientesAlternativoMapaRelacionamentoUseCase, ListarClientesAlternativoMapaRelacionamentoUseCase)
  }

  if (!container.isRegistered(ListarClientesUseCase)) {
    container.registerSingleton(ListarClientesUseCase, ListarClientesUseCase)
  }

  if (!container.isRegistered(ListarDepartamentosMapaRelacionamentoUseCase)) {
    container.registerSingleton(ListarDepartamentosMapaRelacionamentoUseCase, ListarDepartamentosMapaRelacionamentoUseCase)
  }

  if (!container.isRegistered(BuscarDadosVcx360UseCase)) {
    container.registerSingleton(BuscarDadosVcx360UseCase, BuscarDadosVcx360UseCase)
  }

  if (!container.isRegistered(SalvarDadosVcx360UseCase)) {
    container.registerSingleton(SalvarDadosVcx360UseCase, SalvarDadosVcx360UseCase)
  }

  if (!container.isRegistered(SalvarNoMapaRelacionamentoUseCase)) {
    container.registerSingleton(SalvarNoMapaRelacionamentoUseCase, SalvarNoMapaRelacionamentoUseCase)
  }

  if (!container.isRegistered(SearchSkillsUseCase)) {
    container.registerSingleton(SearchSkillsUseCase, SearchSkillsUseCase)
  }

  if (!container.isRegistered(GetSkillLevelsUseCase)) {
    container.registerSingleton(GetSkillLevelsUseCase, GetSkillLevelsUseCase)
  }

  if (!container.isRegistered(GetSuggestionHistoryUseCase)) {
    container.registerSingleton(GetSuggestionHistoryUseCase, GetSuggestionHistoryUseCase)
  }

  if (!container.isRegistered(ColetaPerfilColaboradorCampanhaUseCase)) {
    container.registerSingleton(ColetaPerfilColaboradorCampanhaUseCase, ColetaPerfilColaboradorCampanhaUseCase)
  }

  if (!container.isRegistered(GetAddressByCepUseCase)) {
    container.registerSingleton(GetAddressByCepUseCase, GetAddressByCepUseCase)
  }

  // Aniversariantes Use Case
  if (!container.isRegistered(GetAniversariantesSemanaUseCase)) {
    container.registerSingleton(GetAniversariantesSemanaUseCase, GetAniversariantesSemanaUseCase)
  }

  // Avaliacao Use Case
  if (!container.isRegistered(InserirAvaliacaoSatisfacaoUseCase)) {
    container.registerSingleton(InserirAvaliacaoSatisfacaoUseCase, InserirAvaliacaoSatisfacaoUseCase)
  }

  if (!container.isRegistered(ObterDashboardGestorUseCase)) {
    container.registerSingleton(ObterDashboardGestorUseCase, ObterDashboardGestorUseCase)
  }

  if (!container.isRegistered(ObterMeusColaboradoresUseCase)) {
    container.registerSingleton(ObterMeusColaboradoresUseCase, ObterMeusColaboradoresUseCase)
  }

  if (!container.isRegistered(ObterDashboardColaboradorAvaliadoUseCase)) {
    container.registerSingleton(ObterDashboardColaboradorAvaliadoUseCase, ObterDashboardColaboradorAvaliadoUseCase)
  }

  if (!container.isRegistered(ObterDashboardColaboradorUseCase)) {
    container.registerSingleton(ObterDashboardColaboradorUseCase, ObterDashboardColaboradorUseCase)
  }

  if (!container.isRegistered(ObterDashboardRHUseCase)) {
    container.registerSingleton(ObterDashboardRHUseCase, ObterDashboardRHUseCase)
  }

  if (!container.isRegistered(ObterListaColaboradoresRHUseCase)) {
    container.registerSingleton(ObterListaColaboradoresRHUseCase, ObterListaColaboradoresRHUseCase)
  }

  if (!container.isRegistered(InserirPautaSugeridaUseCase)) {
    container.registerSingleton(InserirPautaSugeridaUseCase, InserirPautaSugeridaUseCase)
  }

  if (!container.isRegistered(InserirPautaSugeridaColaboradorUseCase)) {
    container.registerSingleton(InserirPautaSugeridaColaboradorUseCase, InserirPautaSugeridaColaboradorUseCase)
  }

  if (!container.isRegistered(InserirFeedbackGestaoDesempenhoUseCase)) {
    container.registerSingleton(InserirFeedbackGestaoDesempenhoUseCase, InserirFeedbackGestaoDesempenhoUseCase)
  }

  if (!container.isRegistered(InserirOneOnOneUseCase)) {
    container.registerSingleton(InserirOneOnOneUseCase, InserirOneOnOneUseCase)
  }

  if (!container.isRegistered(InserirParametrizacaoUseCase)) {
    container.registerSingleton(InserirParametrizacaoUseCase, InserirParametrizacaoUseCase)
  }

  if (!container.isRegistered(ObterParametrizacaoUseCase)) {
    container.registerSingleton(ObterParametrizacaoUseCase, ObterParametrizacaoUseCase)
  }

  if (!container.isRegistered(InserirVisualizacaoFeedbackUseCase)) {
    container.registerSingleton(InserirVisualizacaoFeedbackUseCase, InserirVisualizacaoFeedbackUseCase)
  }

  if (!container.isRegistered(EnviarDenunciaUseCase)) {
    container.registerSingleton(EnviarDenunciaUseCase, EnviarDenunciaUseCase)
  }

  // Minha Equipe Use Cases
  if (!container.isRegistered(GetMinhaEquipeContextUseCase)) {
    container.registerSingleton(GetMinhaEquipeContextUseCase, GetMinhaEquipeContextUseCase)
  }

  if (!container.isRegistered(GetColaboradorRadarUseCase)) {
    container.registerSingleton(GetColaboradorRadarUseCase, GetColaboradorRadarUseCase)
  }

  if (!container.isRegistered(AprovarSugestaoSkillUseCase)) {
    container.registerSingleton(AprovarSugestaoSkillUseCase, AprovarSugestaoSkillUseCase)
  }

  // Parceria Use Cases
  if (!container.isRegistered(BuscarTodosParceirosUseCase)) {
    container.registerSingleton(BuscarTodosParceirosUseCase, BuscarTodosParceirosUseCase)
  }

  if (!container.isRegistered(InserirParceiroUseCase)) {
    container.registerSingleton(InserirParceiroUseCase, InserirParceiroUseCase)
  }

  if (!container.isRegistered(AtualizarParceiroUseCase)) {
    container.registerSingleton(AtualizarParceiroUseCase, AtualizarParceiroUseCase)
  }

  if (!container.isRegistered(InserirContratoUseCase)) {
    container.registerSingleton(InserirContratoUseCase, InserirContratoUseCase)
  }

  if (!container.isRegistered(AtualizarContratoUseCase)) {
    container.registerSingleton(AtualizarContratoUseCase, AtualizarContratoUseCase)
  }

  if (!container.isRegistered(DeletarContratoUseCase)) {
    container.registerSingleton(DeletarContratoUseCase, DeletarContratoUseCase)
  }

  if (!container.isRegistered(InserirArquivoParceiroUseCase)) {
    container.registerSingleton(InserirArquivoParceiroUseCase, InserirArquivoParceiroUseCase)
  }

  if (!container.isRegistered(GerarRelatorioParceriaUseCase)) {
    container.registerSingleton(GerarRelatorioParceriaUseCase, GerarRelatorioParceriaUseCase)
  }

  // Organograma Use Cases
  if (!container.isRegistered(ListarPosicoesUseCase)) {
    container.registerSingleton(ListarPosicoesUseCase, ListarPosicoesUseCase)
  }

  if (!container.isRegistered(CriarDepartamentoUseCase)) {
    container.registerSingleton(CriarDepartamentoUseCase, CriarDepartamentoUseCase)
  }

  if (!container.isRegistered(AtualizarDepartamentoUseCase)) {
    container.registerSingleton(AtualizarDepartamentoUseCase, AtualizarDepartamentoUseCase)
  }

  if (!container.isRegistered(DeletarDepartamentoUseCase)) {
    container.registerSingleton(DeletarDepartamentoUseCase, DeletarDepartamentoUseCase)
  }

  if (!container.isRegistered(CriarPosicaoUseCase)) {
    container.registerSingleton(CriarPosicaoUseCase, CriarPosicaoUseCase)
  }

  if (!container.isRegistered(AtualizarPosicaoUseCase)) {
    container.registerSingleton(AtualizarPosicaoUseCase, AtualizarPosicaoUseCase)
  }

  if (!container.isRegistered(DeletarPosicaoUseCase)) {
    container.registerSingleton(DeletarPosicaoUseCase, DeletarPosicaoUseCase)
  }

  if (!container.isRegistered(CriarAlocacaoUseCase)) {
    container.registerSingleton(CriarAlocacaoUseCase, CriarAlocacaoUseCase)
  }

  if (!container.isRegistered(AtualizarAlocacaoUseCase)) {
    container.registerSingleton(AtualizarAlocacaoUseCase, AtualizarAlocacaoUseCase)
  }

  if (!container.isRegistered(DeletarAlocacaoUseCase)) {
    container.registerSingleton(DeletarAlocacaoUseCase, DeletarAlocacaoUseCase)
  }

  if (!container.isRegistered(ListarColaboradoresExternosAlocadosUseCase)) {
    container.registerSingleton(ListarColaboradoresExternosAlocadosUseCase, ListarColaboradoresExternosAlocadosUseCase)
  }

  // VCX Use Cases
  if (!container.isRegistered(ListarDoresPorPosicaoIdUseCase)) {
    container.registerSingleton(ListarDoresPorPosicaoIdUseCase, ListarDoresPorPosicaoIdUseCase)
  }

  if (!container.isRegistered(BuscarDorPorIdUseCase)) {
    container.registerSingleton(BuscarDorPorIdUseCase, BuscarDorPorIdUseCase)
  }

  if (!container.isRegistered(CriarDorUseCase)) {
    container.registerSingleton(CriarDorUseCase, CriarDorUseCase)
  }

  if (!container.isRegistered(AtualizarDorUseCase)) {
    container.registerSingleton(AtualizarDorUseCase, AtualizarDorUseCase)
  }

  if (!container.isRegistered(ExcluirDorUseCase)) {
    container.registerSingleton(ExcluirDorUseCase, ExcluirDorUseCase)
  }

  if (!container.isRegistered(ListarImpactosDoresUseCase)) {
    container.registerSingleton(ListarImpactosDoresUseCase, ListarImpactosDoresUseCase)
  }

  if (!container.isRegistered(ListarUrgenciasDoresUseCase)) {
    container.registerSingleton(ListarUrgenciasDoresUseCase, ListarUrgenciasDoresUseCase)
  }

  if (!container.isRegistered(BuscarHistoricoVcxUseCase)) {
    container.registerSingleton(BuscarHistoricoVcxUseCase, BuscarHistoricoVcxUseCase)
  }

  registerRecrutamentoUseCases(container)
}

export { container }
