/**
 * Filhos de <Route path="/recrutamento" element={<Outlet />}>.
 * Não usar <Routes> aninhado aqui — React Router 7 falha ao compor a árvore.
 */
import { Fragment } from 'react'
import { Navigate, Route } from 'react-router-dom'

import {
  CandidaturasFMU,
  CriarPerfilAtuacao,
  DashboardRecrutamento,
  EditarVaga,
  GerarMatchPromptPage,
  GestaoVagas,
  GestaoVagasAdmissao,
  GestaoVagasCandidatos,
  GestaoVagasRelatorios,
  HistoricoCandidatura,
  MinhasImportacoes,
  ParametrizacaoRecrutamento,
  TalentosInscritos,
  TemplateContratacaoCandidato,
  VagasDetalhePage,
} from '@presentation/pages/recrutamento'

export const RecrutamentoChildRoutes = (
  <Fragment>
    <Route index element={<GestaoVagas />} />
    <Route path="parametrizacao" element={<ParametrizacaoRecrutamento />} />
    <Route path="dashboard" element={<DashboardRecrutamento />} />
    <Route path="candidaturasfmu" element={<CandidaturasFMU />} />
    <Route
      path="historico-candidato/:codigoInternoColaborador"
      element={<HistoricoCandidatura />}
    />
    <Route path="perfil" element={<CriarPerfilAtuacao />} />
    <Route path="detalhe/:vagaCodigo" element={<VagasDetalhePage />} />
    <Route path="editar/:vagaId" element={<EditarVaga />} />
    <Route path="talentosInscritos" element={<TalentosInscritos />} />
    <Route path="uploads" element={<MinhasImportacoes />} />
    <Route path="candidatos" element={<GestaoVagasCandidatos />} />
    <Route path="gerar-match" element={<GerarMatchPromptPage />} />
    <Route path="template-contratacao/:idCandidatura" element={<TemplateContratacaoCandidato />} />
    <Route path="relatorios" element={<GestaoVagasRelatorios />} />
    <Route path="admissao" element={<GestaoVagasAdmissao />} />
    <Route path="*" element={<Navigate to="/recrutamento" replace />} />
  </Fragment>
)
