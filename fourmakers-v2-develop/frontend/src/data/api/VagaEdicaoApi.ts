import { injectable } from 'tsyringe';
import { httpClient } from './httpClient';

/**
 * API exclusiva para edição de vaga recrutamento.
 * Em arquivo separado para não modificar VagaApi.ts e evitar conflito ao mergear com develop.
 */

/** Retorno de ObterVagaRecrutamentoPorId e body de AtualizarVagaRecrutamentoPorId (PUT). */
export interface VagaRecrutamentoRetorno {
  id: string;
  codigo?: number;
  titulo?: string;
  numeroDeVagas?: number;
  custoProfissional?: number | null;
  rateCard?: number | null;
  descricao?: string | null;
  cargo?: string | null;
  dataCriacao?: string | null;
  dataUltimaAlteracao?: string | null;
  localizacao?: string | null;
  estado?: string | null;
  cidade?: string | null;
  cep?: string | null;
  pais?: string | null;
  cpfUsuarioCriador?: string | null;
  nomeUsuarioCriador?: string | null;
  cpfUsuarioAprovador?: string | null;
  codigoGestor?: string | null;
  statusVagaCod?: string | null;
  origemVagaCod?: string | null;
  orgId?: string | null;
  idPerfilGerador?: string | null;
  frequencia?: string | number | null;
  modeloTrabalhoCod?: number | null;
  modeloTrabalhoDescricao?: string | null;
  modeloTrabalhoId?: string | null;
  nomeGestor?: string | null;
  nomeCliente?: string | null;
  codigoCliente?: string | null;
  nomeUsuarioAlterador?: string | null;
  slaContando?: boolean;
  slaDecorridoTotal?: string | null;
  slaDecorridoDaEtapaAtual?: string | null;
  skills?: Array<{
    id: string;
    skillId: number;
    skillDescription: string;
    skillNivelId?: number;
    skillNivelDescription: string;
    tipoSkillId: number;
    typeSkillsDescription?: string;
    vagaId?: string;
    ativo?: boolean;
    dataCriacao?: string;
    dataAlteracao?: string;
    relevante: boolean;
  }>;
  quantidadeCandidatosPorEstagio?: unknown;
  colaboradorCodigoInternoColaboradorGestorOrgLogada?: string | null;
  propostaCrm?: string | null;
  tipoVagaId?: string | null;
  tipoContratacaoId?: number | null;
  unidadeId?: string | null;
  codColaboradoresEntrevistadores?: string[] | null;
  tracking?: string | null;
  numeroVagaCliente?: string | null;
  codigoClienteFourmakers?: string | null;
  tipoEmpregoLinkedin?: string | null;
  nivelExperienciaLinkedin?: string | null;
  permanenciaId?: string | null;
  recrutadorVaga?: string | null;
  nomeRecrutadorVaga?: string | null;
  idVagaParent?: string | null;
  maquinaColaborador?: string | null;
  candidatosContratados?: number;
  posicoesRestantes?: number;
  emailsAnaliseGestor?: string[] | null;
  observacoesInternas?: string | null;
  idVaga?: string;
  [key: string]: unknown;
}

export interface ObterVagaRecrutamentoPorIdResponse {
  retorno: VagaRecrutamentoRetorno;
  sucesso?: boolean;
  mensagem?: string;
}

@injectable()
export class VagaEdicaoApi {
  /** GET /api/Vaga/ObterVagaRecrutamentoPorId/{vagaId} — retorno completo para tela de edição. */
  async getVagaRecrutamentoPorId(
    token: string,
    vagaId: string
  ): Promise<ObterVagaRecrutamentoPorIdResponse> {
    return httpClient.get<ObterVagaRecrutamentoPorIdResponse>(
      `/api/Vaga/ObterVagaRecrutamentoPorId/${vagaId}`,
      { token }
    );
  }

  /** PUT /api/Vaga/AtualizarVagaRecrutamentoPorId — atualiza vaga com payload completo (incl. dados adicionais). */
  async atualizarVagaRecrutamentoPorId(
    token: string,
    body: VagaRecrutamentoRetorno
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    const payload = { ...body, idVaga: body.id };
    return httpClient.put<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }>(
      '/api/Vaga/AtualizarVagaRecrutamentoPorId',
      payload,
      { token }
    );
  }
}
