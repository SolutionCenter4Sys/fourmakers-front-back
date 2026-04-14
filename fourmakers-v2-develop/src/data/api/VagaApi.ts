import { injectable } from 'tsyringe';
import type { RelatorioVagaResult } from '@domain/repositories/VagaRepository';
import { httpClient } from './httpClient';

export interface VagaApiResponse {
  retorno: {
    id: string;
    codigo?: number;
    titulo?: string;
    descricao?: string | null;
    cargo?: string | null;
    custoProfissional?: number | null;
    rateCard?: number | null;
    modeloTrabalhoId?: string | null;
    modeloTrabalhoDescricao?: string | null;
    nomeGestor?: string | null;
    nomeCliente?: string | null;
    codigoCliente?: string | null;
    localizacao?: string | null;
    cidade?: string | null;
    estado?: string | null;
    pais?: string | null;
    dataCriacao?: string | null;
    dataUltimaAlteracao?: string | null;
    statusVagaCod?: string | null;
    frequencia?: string | number | null;
    skills?: Array<{
      id: string;
      skillId: number;
      skillDescription: string;
      skillNivelDescription: string;
      tipoSkillId: number;
      relevante: boolean;
    }>;
  };
  sucesso?: boolean;
  mensagem?: string;
}

export interface ListarCandidatosInscritosParams {
  vagaId: string;
  busca?: string;
  cursor?: number;
  limite?: number;
  dataInicio?: string;
  dataFim?: string;
  qualificados?: boolean;
  diasUltimaAlteracao?: number;
  localizacaoCidade?: string;
  localizacaoEstado?: string;
}

/** Payload da API ListarCandidatosAderentes (POST). */
export interface ListarCandidatosAderentesParams {
  vagaId: string;
  cursor?: number;
  limite?: number;
  busca?: string;
  dataInicio?: string;
  dataFim?: string;
  origens?: string[];
  pesoHardSkills?: number;
  pesoSoftSkills?: number;
  pesoMetodologias?: number;
  pesoDominiosNegocio?: number;
  pesoIdiomas?: number;
  pesoDisponibilidades?: number;
  qualificados?: boolean;
  diasUltimaAlteracao?: number;
  localizacaoCidade?: string;
  localizacaoEstado?: string;
}

/** Item bruto da API ListarCandidatosAderentes (retorno[].retornoMatch usa RetornoMatchRaw). */
export interface CandidatoAderenteRaw {
  nome?: string;
  codigo?: string;
  percentualAderencia?: number;
  email?: string;
  ehCandidato?: boolean;
  retornoMatch?: RetornoMatchRaw | null;
  qualificado?: boolean | null;
  orgId?: number;
  orgDescricao?: string;
  ativoNaOrg?: boolean;
  origem?: string;
  organizacoes?: Array<{ orgId?: number; orgDescricao?: string; ativoNaOrg?: boolean }>;
  [key: string]: unknown;
}

export interface ListarCandidatosAderentesResponse {
  retorno?: CandidatoAderenteRaw[];
  sucesso?: boolean;
  mensagem?: string;
}

/** Candidato inscrito (modelo normalizado para UI). */
export interface CandidatoInscrito {
  id?: string;
  idCandidatura?: string;
  codigo?: string;
  nome?: string;
  nomeColaborador?: string;
  descricaoStatus?: string;
  statusCandidaturaId?: string;
  dataCandidatura?: string;
  criadoPor?: string;
  modificadoEm?: string;
  tempoDecorridoHoras?: number;
  tempoDecorridoTexto?: string;
  percentualMatch?: number;
  origem?: string;
  totalInscritoOutrasVagas?: number;
  recrutadorResponsavel?: string | null;
  qualificado?: boolean | null;
  dataQualificacao?: string | null;
  nomeDeQuemQualificou?: string | null;
  retornoMatch?: RetornoMatchRaw | null;
  organizacoes?: Array<{ orgId?: number; orgDescricao?: string; ativoNaOrg?: boolean }>;
  slaDecorridoDaEtapaAtual?: string | null;
  [key: string]: unknown;
}

/** Objeto retornoMatch da API (camelCase + snake_case). Usado no modal de aderência. */
export interface RetornoMatchRaw {
  codigoInternoColaborador?: string;
  nome?: string;
  orgs?: number[];
  match?: number;
  score_candidato?: number;
  score_vaga?: number;
  detalhamento_calculo?: Record<string, { score_bruto_categoria?: number; score_bruto_obrigatorio?: number; score_bruto_desejavel?: number }>;
  comparativo_por_skill?: Record<string, Array<{
    skill_requisitada?: string;
    nivel_requerido?: string;
    obrigatoriedade?: string;
    skill_do_candidato?: string;
    nivel_do_candidato?: string;
    pontuacao_da_skill?: number;
  }>>;
  origem?: string | null;
  [key: string]: unknown;
}

/** Item bruto da API ListarCandidatosInscritos */
export interface CandidatoInscritoRaw {
  idCandidatura?: string;
  nome?: string;
  codigo?: string;
  candidatura?: string;
  ultimaAlteracao?: string;
  idStatusCandidatura?: string;
  descricaoStatusCandidatura?: string;
  nomeCompletoDeQuemCadastrou?: string | null;
  match?: number;
  slaDecorridoTotal?: string | null;
  totalInscritoOutrasVagas?: number;
  recrutadorResponsavel?: string | null;
  codRecrutadorResponsavel?: string | null;
  qualificado?: boolean | null;
  dataQualificacao?: string | null;
  nomeDeQuemQualificou?: string | null;
  retornoMatch?: RetornoMatchRaw | null;
  organizacoes?: Array<{ orgId?: number; orgDescricao?: string; ativoNaOrg?: boolean }>;
  slaDecorridoDaEtapaAtual?: string | null;
  [key: string]: unknown;
}

export interface ListarCandidatosInscritosResponse {
  retorno?: CandidatoInscritoRaw[];
  sucesso?: boolean;
  mensagem?: string;
}

export interface StatusCandidatura {
  id: string;
  nome: string;
  descricao?: string;
  ordem?: number;
  ativo?: boolean;
  [key: string]: unknown;
}

/** Item bruto da API ListarStatusCandidaturaRecrutamento: { id: number, descricao: string } */
export interface StatusCandidaturaRaw {
  id: number;
  descricao: string;
  [key: string]: unknown;
}

export interface ListarStatusCandidaturaRecrutamentoResponse {
  retorno?: StatusCandidaturaRaw[];
  sucesso?: boolean;
  mensagem?: string;
}

/** Item da API ListarStatusVagaRecrutamento: { codigo: string, descricao: string } */
export interface StatusVagaRecrutamentoItem {
  codigo: string;
  descricao: string;
}

export interface ListarStatusVagaRecrutamentoResponse {
  retorno?: StatusVagaRecrutamentoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Item da API ListarUnidades: { id: string, descricao: string } */
export interface UnidadeItem {
  id: string;
  descricao: string;
}

export interface ListarUnidadesResponse {
  retorno?: UnidadeItem[];
  sucesso?: boolean;
  mensagem?: string;
}

/** Item da API ListarTiposVaga: { id: string, descricao: string } */
export interface TipoVagaItem {
  id: string;
  descricao: string;
}

export interface ListarTiposVagaResponse {
  retorno?: TipoVagaItem[];
  sucesso?: boolean;
  mensagem?: string;
}

/** Item da API ListarTiposContratacao: { id: number, descricao: string } */
export interface TipoContratacaoItem {
  id: number;
  descricao: string;
}

export interface ListarTiposContratacaoResponse {
  retorno?: TipoContratacaoItem[];
  sucesso?: boolean;
  mensagem?: string;
}

/** Payload para InserirInformacoesComplementaresVagaRecrutamento (PUT). */
export interface InserirInformacoesComplementaresPayload {
  colaboradorCodigoInternoColaboradorGestorOrgLogada?: string | null;
  propostaCrm?: string | null;
  idVaga: string;
  tipoVagaId?: string | null;
  tipoContratacaoId?: number | null;
  unidadeId?: string | null;
  codColaboradoresEntrevistadores?: string[] | null;
  numeroDeVagas?: number;
  maquinaColaborador?: string | null;
  recrutadorVaga?: string | null;
  emailsAnaliseGestor?: string[] | null;
  observacoesInternas?: string | null;
}

/** Payload para MudarStatusVaga (POST). */
export interface MudarStatusVagaPayload {
  codigoVaga: string;
  codigoStatus: number;
  comentarioVaga?: string;
}

/** Item da API ListarMotivosPerdaVaga. */
export interface MotivoPerdaVagaItem {
  id: string;
  descricao: string;
  explicacao?: string;
  ordem?: number;
}

/** Payload para GravarPerdaVaga (POST). */
export interface GravarPerdaVagaPayload {
  codigoVaga: number;
  idMotivoPerda: string;
  comentario: string;
}

/** Payload para MudarStatusCandidatura (POST). */
export interface MudarStatusCandidaturaPayload {
  idCandidatura: string;
  codigoStatus: number;
  comentario: string;
}

/** Retorno da API MudarStatusCandidatura. */
export interface MudarStatusCandidaturaRetorno {
  candidaturaId?: string;
  comentarioId?: string;
}

/** Item da API ListarOpcoesContato: { id: string, descricao: string } */
export interface OpcaoContatoItem {
  id: string;
  descricao: string;
}

export interface ListarOpcoesContatoResponse {
  retorno?: OpcaoContatoItem[];
  sucesso?: boolean;
  mensagem?: string;
}

export interface TotaisInscritos {
  totalInscritos?: number;
  totalAprovados?: number;
  totalReprovados?: number;
  totalDeclinados?: number;
  [key: string]: number | undefined;
}

/** Resposta da API ObterTotaisInscritos: retorno.totalCandidatosInscritos = { "1": 23, "2": 1, ... } (idStatus -> quantidade) */
export interface ObterTotaisInscritosResponse {
  retorno?: {
    totalCandidatosInscritos?: Record<string, number>;
  };
  sucesso?: boolean;
  mensagem?: string;
}

/** Normaliza um candidato bruto da API para o modelo de UI */
export function normalizeCandidatoInscrito(raw: CandidatoInscritoRaw): CandidatoInscrito {
  return {
    id: raw.idCandidatura,
    idCandidatura: raw.idCandidatura,
    codigo: raw.codigo,
    nome: raw.nome,
    descricaoStatus: raw.descricaoStatusCandidatura,
    statusCandidaturaId: raw.idStatusCandidatura ? String(raw.idStatusCandidatura) : undefined,
    dataCandidatura: raw.candidatura,
    modificadoEm: raw.ultimaAlteracao,
    criadoPor: raw.nomeCompletoDeQuemCadastrou ?? undefined,
    percentualMatch: raw.match,
    tempoDecorridoTexto: raw.slaDecorridoTotal ?? undefined,
    origem: (raw as Record<string, unknown>).origem as string | undefined,
    totalInscritoOutrasVagas: raw.totalInscritoOutrasVagas ?? 0,
    recrutadorResponsavel: raw.recrutadorResponsavel ?? null,
    qualificado: raw.qualificado ?? null,
    dataQualificacao: raw.dataQualificacao ?? null,
    nomeDeQuemQualificou: raw.nomeDeQuemQualificou ?? null,
    retornoMatch: raw.retornoMatch ?? null,
    organizacoes: raw.organizacoes ?? [],
    slaDecorridoDaEtapaAtual: raw.slaDecorridoDaEtapaAtual ?? null,
  };
}

/** Normaliza totais da API (totalCandidatosInscritos por status) para TotaisInscritos. Status 9=Aprovado, 10=Reprovado, 12=Declinado. */
export function normalizeTotaisInscritos(retorno: ObterTotaisInscritosResponse['retorno']): TotaisInscritos {
  const obj = retorno?.totalCandidatosInscritos ?? {};
  const values = Object.values(obj);
  const totalInscritos = values.reduce((acc, n) => acc + (typeof n === 'number' ? n : 0), 0);
  return {
    totalInscritos,
    totalAprovados: typeof obj['9'] === 'number' ? obj['9'] : 0,
    totalReprovados: typeof obj['10'] === 'number' ? obj['10'] : 0,
    totalDeclinados: typeof obj['12'] === 'number' ? obj['12'] : 0,
  };
}

/** Endereço no template de contratação. */
export interface TemplateContratacaoEndereco {
  cep?: string | null;
  endereco?: string | null;
  complemento?: string | null;
  numero?: number | null;
  bairro?: string | null;
  cidade?: string | null;
  estado?: string | null;
  com_quem_mora?: string | null;
  internacional_linha_um?: string | null;
  internacional_linha_dois?: string | null;
  id?: string | null;
}

/** Saúde do candidato no template de contratação. */
export interface TemplateContratacaoSaude {
  pcd?: string | null;
  tipoPcd?: string | null;
  enumPCD?: number | null;
  grupoDeRiscoCovid?: number | null;
  condicaoDeSaudeRelevante?: string | null;
}

/** Item de sistema liberado / diretório / grupo no template. */
export interface TemplateContratacaoItemLiberado {
  id?: string;
  descricao?: string;
  [key: string]: unknown;
}

/** Retorno da API ObterTemplatePorCandidatura (template de contratação do candidato). */
export interface TemplateContratacaoRetorno {
  id: string;
  colaboradorCodigoInternoColaboradorAnalista?: string | null;
  nomeColaboradorAnalista?: string | null;
  colaboradorCodigoInternoColaborador?: string | null;
  candidatoVagaId?: string | null;
  cargo?: string | null;
  equipamentoPadraoCargoFuncaoId?: string | null;
  grupoAreaEquipamentoPadraoCargoFuncao?: string | null;
  dataInicio?: string | null;
  horarioJornada?: string | null;
  tipoHorarioJornada?: string | null;
  documentoColaborador?: string | null;
  rgColaborador?: string | null;
  dataNascimento?: string | null;
  contatoPrincipal?: string | null;
  nomeCompleto?: string | null;
  tamanhoCamiseta?: string | null;
  descricaoMaquina?: string | null;
  hardware?: string | null;
  softwaresNecessarios?: string | null;
  softwaresEc?: string | null;
  colaboradorCodigoInternoColaboradorSuperiorImediato?: string | null;
  nomeColaboradorSuperiorImediato?: string | null;
  emailPessoal?: string | null;
  emailCorporativo?: string | null;
  loginRede?: string | null;
  tipoLoginRede?: string | null;
  tipoMaquina?: string | null;
  observacoesAcessoUsuario?: string | null;
  grupoEmailContrato?: string | null;
  outrosGrupos?: string[] | null;
  sistemasLiberados?: TemplateContratacaoItemLiberado[] | null;
  diretorios?: TemplateContratacaoItemLiberado[] | null;
  gruposEmails?: TemplateContratacaoItemLiberado[] | null;
  observacoesAprovadorAcessos?: string | null;
  endereco?: TemplateContratacaoEndereco | null;
  salario?: string | number | null;
  custoHora?: string | number | null;
  vr?: string | number | null;
  va?: string | number | null;
  assistenciaMedica?: string | number | null;
  ajudaDeCusto?: string | number | null;
  mobilidade?: string | number | null;
  educacao?: string | number | null;
  remuneracaoTotal?: string | number | null;
  celular?: boolean;
  planoDados?: boolean;
  quantidadeMinutosPlanoDados?: number;
  cartaoVisitas?: boolean;
  quantidadeCartaoVisitas?: number;
  outrosEquipamentos?: string | null;
  codigoVaga?: number;
  tituloVaga?: string | null;
  nomeClienteVaga?: string | null;
  descricaoTipoVaga?: string | null;
  primeiraOpcaoEquipamentoPadraoCargoFuncao?: string | null;
  segundaOpcaoEquipamentoPadraoCargoFuncao?: string | null;
  modeloTrabalhoId?: string | null;
  modeloTrabalhoDescricao?: string | null;
  quantidadeDiasPresencial?: number | null;
  cargoConfianca?: boolean;
  valorAdicionalCargoConfianca?: string | number | null;
  exColaborador?: boolean;
  saude?: TemplateContratacaoSaude | null;
  [key: string]: unknown;
}

export interface ObterTemplatePorCandidaturaResponse {
  retorno?: TemplateContratacaoRetorno;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Opção de equipamento dentro de um cargo/função (API ListarEquipamentosPadroesAninhados). */
export interface EquipamentoPadraoOpcao {
  tipoOpcao?: string | null;
  categoriaNome?: string | null;
  tipoEquipamento?: string | null;
  cpuGeracao?: string | null;
  memoriaRam?: string | null;
  armazenamentoDisco?: string | null;
  so?: string | null;
  gpu?: string | null;
  modelosPossiveis?: string[] | null;
  descricaoUpgrade?: string | null;
}

/** Cargo/função com opções de equipamento (API ListarEquipamentosPadroesAninhados). */
export interface EquipamentoPadraoCargoFuncao {
  idCargoFuncao?: string | null;
  grupoArea?: string | null;
  nomeCargoFuncao?: string | null;
  opcoesEquipamento?: EquipamentoPadraoOpcao[] | null;
}

/** Grupo por área (Administrativo, Desenvolvedor) com cargos (API ListarEquipamentosPadroesAninhados). */
export interface EquipamentosPadroesAninhadosGrupo {
  grupoArea?: string | null;
  cargosFuncoes?: EquipamentoPadraoCargoFuncao[] | null;
}

export interface ListarEquipamentosPadroesAninhadosResponse {
  retorno?: EquipamentosPadroesAninhadosGrupo[] | null;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Item retornado por ListarDiretorios (api/Vaga/Contratacao/ListarDiretorios). */
export interface DiretorioContratacaoItem {
  id?: string;
  descricao?: string;
  leitura?: boolean;
  escrita?: boolean;
}

/** Item retornado por ListarSistemasLiberados (api/Vaga/Contratacao/ListarSistemasLiberados). */
export interface SistemaLiberadoContratacaoItem {
  id?: string;
  descricao?: string;
}

/** Item retornado por ListarGruposEmails (api/Vaga/Contratacao/ListarGruposEmails). */
export interface GrupoEmailContratacaoItem {
  id?: string;
  descricao?: string;
}

export interface ListarDiretoriosResponse {
  retorno?: DiretorioContratacaoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface ListarSistemasLiberadosResponse {
  retorno?: SistemaLiberadoContratacaoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface ListarGruposEmailsResponse {
  retorno?: GrupoEmailContratacaoItem[];
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

export interface SalvarTemplateResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

/** Objeto enviado no campo "param" da API EnviarEmailTemplateCandidato (multipart: param + File opcional). */
export interface EnviarEmailTemplateCandidatoParam {
  idCandidatura: string;
  emailsAdicionais: string[];
  ocultarValores: boolean;
  anexo: boolean;
}

@injectable()
export class VagaApi {
  async getVagaDetalhes(token: string, vagaId: string): Promise<VagaApiResponse> {
    return httpClient.get<VagaApiResponse>(`/api/Vaga/ObterVagaRecrutamentoPorId/${vagaId}`, { token });
  }

  /** Detalhes da vaga para portal público (por código numérico). Endpoint público, sem token. */
  async getVagaDetalhesPublico(codigoVaga: number): Promise<VagaApiResponse | null> {
    try {
      const params = new URLSearchParams({
        id_vaga: '0',
        cod_vaga_recrutamento: String(codigoVaga),
      });
      const res = await httpClient.get<VagaApiResponse>(
        `/api/Srs/DetalharVaga?${params.toString()}`
      );
      return res ?? null;
    } catch {
      return null;
    }
  }

  /** Inscrição do usuário logado na vaga (portal público). Requer pretencaoSalarial, modeloTrabalhoId e opcoesContatoIds (lista de IDs string). */
  async candidatarSe(
    token: string,
    payload: {
      codigoVaga: number;
      opcoesContatoIds: string[];
      pretencaoSalarial: string;
      modeloTrabalhoId: string;
    }
  ): Promise<{ retorno?: boolean; sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return httpClient.post<{ retorno?: boolean; sucesso?: boolean; mensagem?: string; erros?: string[] | null }>(
      '/api/Vaga/CandidatarSe',
      payload,
      { token }
    );
  }

  /**
   * Obtém template de contratação por id da candidatura.
   * GET /api/Vaga/Contratacao/ObterTemplatePorCandidatura/{idCandidatura}
   * Modo criação quando retorno.id === '00000000-0000-0000-0000-000000000000'; caso contrário, modo edição.
   */
  async obterTemplatePorCandidatura(
    token: string,
    idCandidatura: string
  ): Promise<ObterTemplatePorCandidaturaResponse> {
    return httpClient.get<ObterTemplatePorCandidaturaResponse>(
      `/api/Vaga/Contratacao/ObterTemplatePorCandidatura/${encodeURIComponent(idCandidatura)}`,
      { token }
    );
  }

  /** Cria template de contratação. */
  async criarTemplate(
    token: string,
    payload: Record<string, unknown>
  ): Promise<SalvarTemplateResponse> {
    return httpClient.post<SalvarTemplateResponse>('/api/Vaga/Contratacao/CriarTemplate', payload, { token });
  }

  /** Atualiza template de contratação existente. */
  async atualizarTemplate(
    token: string,
    idTemplate: string,
    payload: Record<string, unknown>
  ): Promise<SalvarTemplateResponse> {
    return httpClient.put<SalvarTemplateResponse>(
      `/api/Vaga/Contratacao/AtualizarTemplate/${encodeURIComponent(idTemplate)}`,
      payload,
      { token }
    );
  }

  /**
   * Envia e-mail do template de contratação.
   * Form Data: campo "param" com JSON { idCandidatura, emailsAdicionais, ocultarValores, anexo }; "File" opcional quando houver PDF.
   */
  async enviarEmailTemplateCandidato(
    token: string,
    param: EnviarEmailTemplateCandidatoParam,
    file?: File
  ): Promise<{ sucesso?: boolean; mensagem?: string | null }> {
    const anexo = Boolean(file);
    const paramPayload = {
      idCandidatura: param.idCandidatura,
      emailsAdicionais: param.emailsAdicionais,
      ocultarValores: param.ocultarValores,
      anexo,
    };
    const formData = new FormData();
    formData.append('param', JSON.stringify(paramPayload));
    if (file) {
      formData.append('File', file, file.name);
    }
    return httpClient.post<{ sucesso?: boolean; mensagem?: string | null }>(
      '/api/Vaga/Contratacao/EnviarEmailTemplateCandidato',
      formData,
      { token }
    );
  }

  /** Lista equipamentos padrão aninhados (grupo área → cargos/funções) para Tipo de equipamento + Cargo x Máquina. */
  async listarEquipamentosPadroesAninhados(
    token: string
  ): Promise<ListarEquipamentosPadroesAninhadosResponse> {
    return httpClient.get<ListarEquipamentosPadroesAninhadosResponse>(
      '/api/Vaga/Contratacao/ListarEquipamentosPadroesAninhados',
      { token }
    );
  }

  /** Lista diretórios de rede para template de contratação (Acessos do Usuário). */
  async listarDiretorios(token: string): Promise<ListarDiretoriosResponse> {
    return httpClient.get<ListarDiretoriosResponse>('/api/Vaga/Contratacao/ListarDiretorios', {
      token,
    });
  }

  /** Lista sistemas liberados para template de contratação (Acessos do Usuário). */
  async listarSistemasLiberados(token: string): Promise<ListarSistemasLiberadosResponse> {
    return httpClient.get<ListarSistemasLiberadosResponse>(
      '/api/Vaga/Contratacao/ListarSistemasLiberados',
      { token }
    );
  }

  /** Lista grupos de e-mail para template de contratação (Acessos do Usuário). */
  async listarGruposEmails(token: string): Promise<ListarGruposEmailsResponse> {
    return httpClient.get<ListarGruposEmailsResponse>('/api/Vaga/Contratacao/ListarGruposEmails', {
      token,
    });
  }

  /** Lista candidatos inscritos na vaga (para Kanban por status). */
  async listarCandidatosInscritos(
    token: string,
    params: ListarCandidatosInscritosParams
  ): Promise<ListarCandidatosInscritosResponse> {
    const search = new URLSearchParams();
    search.set('vagaId', params.vagaId);
    if (params.busca != null) search.set('busca', params.busca);
    if (params.cursor != null) search.set('cursor', String(params.cursor));
    if (params.limite != null) search.set('limite', String(params.limite));
    if (params.dataInicio) search.set('dataInicio', params.dataInicio);
    if (params.dataFim) search.set('dataFim', params.dataFim);
    if (params.qualificados != null) search.set('qualificados', String(params.qualificados));
    if (params.diasUltimaAlteracao != null) search.set('diasUltimaAlteracao', String(params.diasUltimaAlteracao));
    if (params.localizacaoCidade) search.set('localizacaoCidade', params.localizacaoCidade);
    if (params.localizacaoEstado) search.set('localizacaoEstado', params.localizacaoEstado);
    return httpClient.get<ListarCandidatosInscritosResponse>(
      `/api/Vaga/ListarCandidatosInscritos?${search.toString()}`,
      { token }
    );
  }

  /** Lista candidatos aderentes e qualificados à vaga (POST com filtros). */
  async listarCandidatosAderentes(
    token: string,
    params: ListarCandidatosAderentesParams
  ): Promise<ListarCandidatosAderentesResponse> {
    const body = {
      vagaId: params.vagaId,
      cursor: params.cursor ?? 0,
      limite: params.limite ?? 20,
      busca: params.busca ?? '',
      dataInicio: params.dataInicio ?? '2000-01-01',
      dataFim: params.dataFim ?? '2100-01-01',
      origens: params.origens ?? [],
      pesoHardSkills: params.pesoHardSkills ?? 1,
      pesoSoftSkills: params.pesoSoftSkills ?? 1,
      pesoMetodologias: params.pesoMetodologias ?? 1,
      pesoDominiosNegocio: params.pesoDominiosNegocio ?? 1,
      pesoIdiomas: params.pesoIdiomas ?? 1,
      pesoDisponibilidades: params.pesoDisponibilidades ?? 1,
      qualificados: params.qualificados ?? false,
      diasUltimaAlteracao: params.diasUltimaAlteracao ?? 0,
      localizacaoCidade: params.localizacaoCidade ?? '',
      localizacaoEstado: params.localizacaoEstado ?? '',
    };
    return httpClient.post<ListarCandidatosAderentesResponse>(
      '/api/Vaga/ListarCandidatosAderentes',
      body,
      { token }
    );
  }

  /** Lista status de candidatura (colunas do Kanban). */
  async listarStatusCandidaturaRecrutamento(
    token: string
  ): Promise<ListarStatusCandidaturaRecrutamentoResponse> {
    return httpClient.get<ListarStatusCandidaturaRecrutamentoResponse>(
      '/api/Vaga/ListarStatusCandidaturaRecrutamento',
      { token }
    );
  }

  /** Lista status de vaga para recrutamento (combo filtros). */
  async listarStatusVagaRecrutamento(
    token: string
  ): Promise<ListarStatusVagaRecrutamentoResponse> {
    return httpClient.get<ListarStatusVagaRecrutamentoResponse>(
      '/api/Vaga/ListarStatusVagaRecrutamento',
      { token }
    );
  }

  /** Lista opções de contato para inscrição (preferência de contato). */
  async listarOpcoesContato(token: string): Promise<ListarOpcoesContatoResponse> {
    return httpClient.get<ListarOpcoesContatoResponse>('/api/Vaga/ListarOpcoesContato', { token });
  }

  /** Inscreve outra pessoa na vaga (colaborador → vaga). */
  async candidatarOutraPessoa(
    token: string,
    payload: { codigoVaga: string | number; codigoColaborador: string; opcoesContatoIds: string[] }
  ): Promise<{ sucesso?: boolean; mensagem?: string }> {
    return httpClient.post<{ sucesso?: boolean; mensagem?: string }>(
      '/api/Vaga/CandidatarOutraPessoa',
      payload,
      { token }
    );
  }

  /** Totais de inscritos por tipo (Total, Aprovados, Reprovados, Declinados). */
  async obterTotaisInscritos(
    token: string,
    vagaId: string
  ): Promise<ObterTotaisInscritosResponse> {
    const search = new URLSearchParams({ vagaId });
    return httpClient.get<ObterTotaisInscritosResponse>(
      `/api/Vaga/ObterTotaisInscritos?${search.toString()}`,
      { token }
    );
  }

  /** Lista unidades da organização (para lookup unidadeId → descricao). */
  async listarUnidades(token: string): Promise<ListarUnidadesResponse> {
    return httpClient.get<ListarUnidadesResponse>('/api/Vaga/ListarUnidades', { token });
  }

  /** Lista tipos de vaga (para lookup tipoVagaId → descricao). */
  async listarTiposVaga(token: string): Promise<ListarTiposVagaResponse> {
    return httpClient.get<ListarTiposVagaResponse>('/api/Vaga/ListarTiposVaga', { token });
  }

  /** Lista tipos de contratação (para lookup tipoContratacaoId → descricao). */
  async listarTiposContratacao(token: string): Promise<ListarTiposContratacaoResponse> {
    return httpClient.get<ListarTiposContratacaoResponse>('/api/Vaga/ListarTiposContratacao', { token });
  }

  /** Resposta da API AdicionarRecrutadorVaga. */
  async adicionarRecrutadorVaga(
    token: string,
    vagaId: string,
    codInternoColaboradorRecrutador: string
  ): Promise<{ retorno?: string; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    const params = new URLSearchParams({
      vagaId,
      codInternoColaboradorRecrutador,
    });
    return httpClient.post<{ retorno?: string; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }>(
      `/api/Vaga/AdicionarRecrutadorVaga?${params.toString()}`,
      {},
      { token }
    );
  }

  /** Item da API ListarVagasRecrutamentoPorParentEmAndamento (vagas filhas em andamento). */
  async listarVagasRecrutamentoPorParentEmAndamento(
    token: string,
    vagaIdParent: string
  ): Promise<{
    retorno?: Array<{
      id: string;
      codigo?: number;
      titulo?: string;
      statusVagaCod?: string;
      idPerfilGerador?: string;
      codigoCliente?: string;
      nomeCliente?: string;
      codigoGestor?: string;
      nomeGestor?: string;
      [key: string]: unknown;
    }>;
    sucesso?: boolean;
    mensagem?: string | null;
    erros?: string[] | null;
  }> {
    const params = new URLSearchParams({ vagaIdParent });
    return httpClient.get(
      `/api/Vaga/ListarVagasRecrutamentoPorParentEmAndamento?${params.toString()}`,
      { token }
    );
  }

  async inserirInformacoesComplementaresVagaRecrutamento(
    token: string,
    payload: InserirInformacoesComplementaresPayload
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    return httpClient.put(
      '/api/Vaga/InserirInformacoesComplementaresVagaRecrutamento',
      payload,
      { token }
    );
  }

  async mudarStatusVaga(
    token: string,
    payload: MudarStatusVagaPayload
  ): Promise<{ retorno?: unknown; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    return httpClient.post<{
      retorno?: unknown;
      sucesso?: boolean;
      mensagem?: string | null;
      erros?: string[] | null;
    }>('/api/Vaga/MudarStatusVaga', payload, { token });
  }

  async listarMotivosPerdaVaga(
    token: string
  ): Promise<{ retorno?: MotivoPerdaVagaItem[]; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    return httpClient.get<{
      retorno?: MotivoPerdaVagaItem[];
      sucesso?: boolean;
      mensagem?: string | null;
      erros?: string[] | null;
    }>('/api/Vaga/ListarMotivosPerdaVaga', { token });
  }

  async gravarPerdaVaga(
    token: string,
    payload: GravarPerdaVagaPayload
  ): Promise<{ retorno?: boolean; sucesso?: boolean; mensagem?: string | null; erros?: string[] | null }> {
    return httpClient.post<{
      retorno?: boolean;
      sucesso?: boolean;
      mensagem?: string | null;
      erros?: string[] | null;
    }>('/api/Vaga/GravarPerdaVaga', payload, { token });
  }

  /**
   * Altera o status da candidatura (com comentário obrigatório).
   * POST /api/Vaga/MudarStatusCandidatura
   */
  async mudarStatusCandidatura(
    token: string,
    payload: MudarStatusCandidaturaPayload
  ): Promise<{
    retorno?: MudarStatusCandidaturaRetorno;
    sucesso?: boolean;
    mensagem?: string | null;
    erros?: string[] | null;
  }> {
    return httpClient.post<{
      retorno?: MudarStatusCandidaturaRetorno;
      sucesso?: boolean;
      mensagem?: string | null;
      erros?: string[] | null;
    }>('/api/Vaga/MudarStatusCandidatura', payload, { token });
  }

  /** GET /api/Vaga/RelatorioProdutividade?dataInicio=&dataFim= — retorno binário xlsx (200) ou 204 sem dados */
  async getRelatorioProdutividade(
    token: string,
    dataInicio: string,
    dataFim: string
  ): Promise<RelatorioVagaResult> {
    const params = new URLSearchParams({ dataInicio, dataFim });
    const response = await httpClient.getBlob(
      `/api/Vaga/RelatorioProdutividade?${params}`,
      { token }
    );
    if (response.status === 204) return { status: 204 };
    const blob = await response.blob();
    return { status: 200, blob };
  }

  /** GET /api/Vaga/RelatorioVagas?dataInicio=&dataFim= — retorno binário xlsx (200) ou 204 sem dados */
  async getRelatorioVagas(
    token: string,
    dataInicio: string,
    dataFim: string
  ): Promise<RelatorioVagaResult> {
    const params = new URLSearchParams({ dataInicio, dataFim });
    const response = await httpClient.getBlob(
      `/api/Vaga/RelatorioVagas?${params}`,
      { token }
    );
    if (response.status === 204) return { status: 204 };
    const blob = await response.blob();
    return { status: 200, blob };
  }

  /** GET /api/Vaga/RelatorioVagasCandidaturas?dataInicio=&dataFim= — retorno binário xlsx (200) ou 204 sem dados */
  async getRelatorioVagasCandidaturas(
    token: string,
    dataInicio: string,
    dataFim: string
  ): Promise<RelatorioVagaResult> {
    const params = new URLSearchParams({ dataInicio, dataFim });
    const response = await httpClient.getBlob(
      `/api/Vaga/RelatorioVagasCandidaturas?${params}`,
      { token }
    );
    if (response.status === 204) return { status: 204 };
    const blob = await response.blob();
    return { status: 200, blob };
  }
}
