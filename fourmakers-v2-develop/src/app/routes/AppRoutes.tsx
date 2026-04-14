import { lazy, Suspense } from "react";
import { Navigate, Outlet, Route, Routes } from "react-router-dom";

import { Spinner } from "@/components/ui/spinner";
import { MainLayout } from "@presentation/layouts/MainLayout";
import Profile360 from "@presentation/pages/Profile360";
import PersonalData from "@presentation/pages/PersonalData";
import Timesheet from "@presentation/pages/Timesheet";
import AprovarTimesheetColaborador from "@presentation/pages/AprovarTimesheetColaborador";
import MapaAlocacao from "@presentation/pages/MapaAlocacao";
import NovaAlocacao from "@presentation/pages/NovaAlocacao";
import CadastrarTbd from "@presentation/pages/CadastrarTbd";
import Colaboradores from "@presentation/pages/Colaboradores";
import NovoColaborador from "@presentation/pages/NovoColaborador";
import BookColaborador from "@presentation/pages/BookColaborador";
import BookColaboradorDetalhes from "@presentation/pages/BookColaboradorDetalhes";
import GestaoDesempenhoGestor from "@presentation/pages/GestaoDesempenhoGestor";
import GestaoDesempenhoColaboradorDetalhes from "@presentation/pages/GestaoDesempenhoColaboradorDetalhes";
import PdiWorkflowPage from "@presentation/pages/PdiWorkflowPage";
import CriacaoPdiPage from "@presentation/pages/CriacaoPdiPage";
import PdiPage from "@presentation/pages/PdiPage";
import PdisEquipePage from "@presentation/pages/PdisEquipePage";
import PdisMetricasPage from "@presentation/pages/PdisMetricasPage";
import GestaoDesempenhoColaborador from "@presentation/pages/GestaoDesempenhoColaborador";
import GestaoDesempenhoRH from "@presentation/pages/GestaoDesempenhoRH";
import ParametrizacaoDesempenho from "@presentation/pages/ParametrizacaoDesempenho";
import Projetos from "@presentation/pages/Projetos";
import InserirReembolso from "@presentation/pages/InserirReembolso";
import Reembolso from "@presentation/pages/Reembolso";
import AprovarReembolso from "@presentation/pages/AprovarReembolso";
import RemessaCNAB from "@presentation/pages/RemessaCNAB";
import ReembolsoParametros from "@presentation/pages/ReembolsoParametros";
import IntegracaoFolhaPonto from "@presentation/pages/IntegracaoFolhaPonto";
import IntegracaoContabil from "@presentation/pages/IntegracaoContabil";
import ConciliacaoFolhaPagamento from "@presentation/pages/ConciliacaoFolhaPagamento";
import Rubricas from "@presentation/pages/Rubricas";
import MeuFaturamento from "@presentation/pages/MeuFaturamento";
import GestaoNotasFiscais from "@presentation/pages/GestaoNotasFiscais";
import MeuHolerite from "@presentation/pages/MeuHolerite";
import Documentacao from "@presentation/pages/Documentacao";
import Login from "@presentation/pages/Login";
import NotFound from "@presentation/pages/NotFound";
import { Dashboard } from "@presentation/pages/Dashboard";
import { FlutterFlowContainer } from "@presentation/pages/FlutterFlowContainer";
import { SSO } from "@presentation/pages/SSO";
import { LoginNumen } from "@presentation/pages/LoginNumen";
import { LoginFMU } from "@presentation/pages/LoginFMU";
import Simulator from "@presentation/pages/Simulator";
import MinhaJornadaPage from "@presentation/pages/MinhaJornadaPage";
import MinhaEquipePage from "@presentation/pages/MinhaEquipePage";
import OrquestracaoPage from "@presentation/pages/OrquestracaoPage";
import { SkillsDashboardPage } from "@presentation/pages/SkillsDashboardPage";
import AtualizarPerfil from "@presentation/pages/AtualizarPerfil";
import SentimentoHoje from "@presentation/pages/SentimentoHoje";
const MapaRelacionamentoPage = lazy(() =>
  import("@presentation/pages/MapaRelacionamentoPage").then((m) => ({
    default: m.MapaRelacionamentoPage,
  }))
);
import Permissionamento from "@presentation/pages/Permissionamento";
import GestaoParceria from "@presentation/pages/GestaoParceria";
import Comunicacao from "@presentation/pages/Comunicacao";
import ComunicacaoGroupDetail from "@presentation/pages/ComunicacaoGroupDetail";
import ComunicacaoProfessionalDetail from "@presentation/pages/ComunicacaoProfessionalDetail";
import BeneficioDetail from "@presentation/pages/BeneficioDetail";
import Feedback360 from "@presentation/pages/Feedback360";
import AgendasComerciaisPage from "@presentation/pages/AgendasComerciaisPage";
import { MatchTalentosPage } from "@presentation/pages/MatchTalentosPage";
import { DashboardComercialPage } from "@presentation/pages/DashboardComercialPage";
import { HomeBuilder } from "@presentation/pages/HomeBuilder";
import { HomeNew } from "@presentation/pages/HomeNew";
import { PublicPage } from "@presentation/pages/PublicPage";
import { PublicVagaDetalhePage } from "@presentation/pages/recrutamento";
import {
  RecrutamentoChildRoutes,
  RedirectGestaodevagasToRecrutamento,
} from "./modules/recrutamento";
import { PrivateRoute } from "@presentation/components/common/PrivateRoute";
import { PublicRoute } from "@presentation/components/common/PublicRoute";
import { RootRedirect } from "./RootRedirect";
import { OldRouteRedirect } from "./OldRouteRedirect";
import { orgLoginConfigs } from "@shared/constants/orgConfig";

export const AppRoutes = () => {
  console.log("[AppRoutes] Renderizando rotas...");

  // Filtra apenas as configurações que têm rotaAntiga definida
  const configsComRotaAntiga = orgLoginConfigs.filter(
    (config) => config.rotaAntiga
  );

  return (
    <Routes>
      {/* Rotas Públicas (só acessíveis se não estiver logado) */}
      <Route element={<PublicRoute />}>
        <Route path="/login/*" element={<Login />} />
        {/* Rota específica da Numen - SSO customizado */}
        <Route path="/login2" element={<LoginNumen />} />
        {/* Rota específica da FMU (org 7) - SSO customizado */}
        <Route path="/loginfmu" element={<LoginFMU />} />
        {/* Rota SSO padrão */}
        <Route path="/sso" element={<SSO />} />
      </Route>

      <Route path="/public/vaga" element={<PublicVagaDetalhePage />} />
      {/* Rota pública agnóstica: /public/[qualquer] → iframe FlutterFlow (sem auth/postMessage) */}
      <Route path="/public/*" element={<PublicPage />} />

      {/* Rotas Privadas (só acessíveis se estiver logado) */}
      <Route element={<PrivateRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/homeDashboard" element={<Navigate to="/dashboard" replace />} />
          <Route path="/profile360" element={<Profile360 />} />
          <Route path="/curriculoProfissional" element={<Profile360 />} />
          <Route path="/personal-data" element={<PersonalData />} />
          {/* Rotas React (acesso para QA/testes) */}
          <Route path="/timesheet" element={<Timesheet />} />
          <Route path="/timesheet/aprovacao" element={<AprovarTimesheetColaborador />} />
          <Route path="/colaboradores" element={<Colaboradores />} />
          <Route
            path="/colaboradores/novo"
            element={<NovoColaborador />}
          />
          <Route
            path="/colaboradores/editar/:codColaborador"
            element={<NovoColaborador />}
          />
          {/* Rotas React para Book Colaborador */}
          <Route path="/book-colaborador" element={<BookColaborador />} />
          <Route
            path="/book-colaborador/:codColaborador"
            element={<BookColaboradorDetalhes />}
          />
          {/* Rotas React para Gestão de Desempenho */}
          <Route path="/pdi" element={<PdiPage />} />
          <Route path="/pdi/novo" element={<CriacaoPdiPage />} />
          <Route path="/pdi/:pdiId" element={<PdiWorkflowPage />} />
          <Route path="/pdis-equipe" element={<PdisEquipePage />} />
          <Route path="/pdis-metricas" element={<PdisMetricasPage />} />
          <Route path="/gestao-desempenho-gestor" element={<GestaoDesempenhoGestor />} />
          <Route
            path="/gestao-desempenho-gestor/:codColaborador"
            element={<GestaoDesempenhoColaboradorDetalhes />}
          />
          <Route
            path="/gestao-desempenho-gestor/:codColaborador/novo-pdi"
            element={<CriacaoPdiPage />}
          />
          <Route
            path="/gestao-desempenho-gestor/:codColaborador/pdi/:pdiId"
            element={<PdiWorkflowPage />}
          />
          <Route path="/gestao-desempenho-colaborador" element={<GestaoDesempenhoColaborador />} />
          <Route path="/gestao-desempenho-rh" element={<GestaoDesempenhoRH />} />
          <Route path="/gestao-desempenho-rh/parametrizacao" element={<ParametrizacaoDesempenho />} />
          <Route
            path="/gestao-desempenho-rh/colaborador/:codColaborador"
            element={<GestaoDesempenhoColaboradorDetalhes />}
          />
          <Route
            path="/gestao-desempenho-rh/colaborador/:codColaborador/pdi/:pdiId"
            element={<PdiWorkflowPage />}
          />
          <Route path="/recrutamento" element={<Outlet />}>
            {RecrutamentoChildRoutes}
          </Route>
          <Route
            path="/candidaturas"
            element={<Navigate to="/recrutamento/candidaturasfmu" replace />}
          />
          <Route
            path="/criarPerfilAtuacao"
            element={<Navigate to="/recrutamento/perfil" replace />}
          />
          <Route path="/vagas/gestao" element={<Navigate to="/recrutamento" replace />} />
          <Route
            path="/gestaodevagas/*"
            element={<RedirectGestaodevagasToRecrutamento />}
          />
          {/* Rotas alternativas para testes */}
          <Route
            path="/_react/book-colaborador"
            element={<BookColaborador />}
          />
          <Route
            path="/_react/book-colaborador/:codColaborador"
            element={<BookColaboradorDetalhes />}
          />
          <Route path="/reembolso" element={<Reembolso />} />

          <Route path="/mapa-alocacao" element={<MapaAlocacao />} />
          <Route path="/mapa-alocacao/nova" element={<NovaAlocacao />} />
          <Route path="/mapa-alocacao/tbd/:id" element={<CadastrarTbd />} />
          <Route path="/projetos" element={<Projetos />} />
          <Route path="/reembolso/aprovar" element={<AprovarReembolso />} />
          <Route path="/reembolso/remessa-cnab" element={<RemessaCNAB />} />
          <Route path="/inserir-reembolso" element={<InserirReembolso />} />
          <Route
            path="/reembolso-parametros"
            element={<ReembolsoParametros />}
          />
          <Route
            path="/integracao-folha-ponto"
            element={<IntegracaoFolhaPonto />}
          />
          <Route path="/integracao-contabil" element={<IntegracaoContabil />} />
          <Route
            path="/conciliacao-folha-pagamento"
            element={<ConciliacaoFolhaPagamento />}
          />
          <Route path="/rubricas" element={<Rubricas />} />
          <Route path="/meu-faturamento" element={<MeuFaturamento />} />
          <Route
            path="/gestao-notas-fiscais"
            element={<GestaoNotasFiscais />}
          />
          <Route path="/meu-holerite" element={<MeuHolerite />} />
          <Route path="/documentacao" element={<Documentacao />} />
          <Route path="/simulador" element={<Simulator />} />
          <Route path="/pdijornada" element={<MinhaJornadaPage />} />
          <Route path="/pdiequipe" element={<MinhaEquipePage />} />
          <Route path="/pdiorquestracao" element={<OrquestracaoPage />} />
          <Route path="/atualizar-perfil" element={<AtualizarPerfil />} />
          <Route path="/dashboard_jornadas" element={<SkillsDashboardPage />} />
          <Route path="/sentimentohoje" element={<SentimentoHoje />} />
          <Route
            path="/mapa-relacionamento"
            element={
              <Suspense fallback={<Spinner className="m-auto" />}>
                <MapaRelacionamentoPage />
              </Suspense>
            }
          />
          {/* Redirect old organograma route to new name */}
          <Route path="/organograma" element={<Navigate to="/mapa-relacionamento" replace />} />
          <Route path="/permissionamento" element={<Permissionamento />} />
          <Route path="/prototipo/matchTalentos" element={<MatchTalentosPage />} />
          <Route path="/gestao/parceria" element={<GestaoParceria />} />
          <Route path="/comunicacao" element={<Comunicacao />} />
          <Route path="/comunicacao/grupo/:groupId" element={<ComunicacaoGroupDetail />} />
          <Route path="/comunicacao/profissional/:id" element={<ComunicacaoProfessionalDetail />} />
          <Route path="/beneficio/:id" element={<BeneficioDetail />} />
          <Route path="/feedback360" element={<Feedback360 />} />
          <Route path="/agendas-comerciais" element={<AgendasComerciaisPage />} />
          <Route path="/prototipo/matchTalentos" element={<MatchTalentosPage />} />
          <Route path="/dashboard-comercial" element={<DashboardComercialPage />} />
          <Route path="/home-builder" element={<HomeBuilder />} />
          <Route path="/homeNew" element={<HomeNew />} />
          {/* Redirecionamento de /page/book-colaborador para página React */}
          <Route
            path="/page/book-colaborador"
            element={<Navigate to="/book-colaborador" replace />}
          />
          <Route
            path="/page/book/colaborador"
            element={<Navigate to="/book-colaborador" replace />}
          />
          <Route path="/page/*" element={<FlutterFlowContainer />} />
        </Route>
      </Route>

      {/* Rotas antigas: redirecionam para /login/slugRota */}
      {configsComRotaAntiga.map((config) => (
        <Route
          key={config.rotaAntiga}
          path={`/${config.rotaAntiga}`}
          element={<OldRouteRedirect />}
        />
      ))}

      {/* Rota padrão: redireciona baseado no estado de autenticação */}
      <Route path="/" element={<RootRedirect />} />

      {/* Rota 404 - DEVE SER A ÚLTIMA */}
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};
