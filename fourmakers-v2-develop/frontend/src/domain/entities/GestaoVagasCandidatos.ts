/**
 * DTOs e tipos de domínio usados pela feature Gestão de Vagas / Candidatos.
 * A camada de apresentação importa apenas de @domain; a camada data mapeia das APIs para estes tipos.
 */

/** Item de motivo de reprovação (status 10). */
export interface MotivoReprovacao {
  id: string;
  descricao: string;
  ativo: boolean;
}

/** Item de motivo de declínio (status 12). */
export interface MotivoDeclinio {
  id: string;
  descricao: string;
  ativo: boolean;
}

/** Modelo de trabalho (ex.: presencial, híbrido). */
export interface ModeloTrabalho {
  id: string;
  descricao: string;
  codigo: number;
}

/** Retorno ao mudar status da candidatura. */
export interface MudarStatusCandidaturaRetorno {
  candidaturaId?: string;
  comentarioId?: string;
}

/** Item de candidatura em listagens (ex.: banco de talentos). */
export interface CandidaturaItemGestao {
  idCandidatura: string;
  tituloVaga: string;
  nomeCliente: string | null;
  codigoCliente: string | null;
  codigoGestor: string | null;
  nomeGestor: string | null;
}

/** Item do banco de talentos (BuscarBancoTalentos). */
export interface BancoTalentosItem {
  nome: string;
  codigoInternoColaborador: string;
  nomeCadastrante: string | null;
  dataDoCadastro: string;
  possuiCandidatura: boolean;
  candidaturas: CandidaturaItemGestao[] | null;
  match: number;
}

/** Pessoa cadastrada por colaborador (BuscarPessoasCadastradasPorColaborador). */
export interface PessoaCadastradaPorColaborador {
  nome: string;
  codigoInternoColaborador: string;
  nomeCadastrante: string;
  dataDoCadastro: string;
  possuiCandidatura: boolean;
  candidaturas: CandidaturaItemGestao[];
  match: number;
}

/** Pessoa cadastrada por organização (BuscarPessoasCadastradasPorOrg – Candidaturas FMU). */
export interface PessoaCadastradaPorOrg {
  nome: string;
  codigoInternoColaborador: string | null;
  nomeCadastrante: string | null;
  dataDoCadastro: string;
  emailUsuario: string;
}

/** Item de candidatura retornado por BuscarCandidaturasColaborador (detalhe na expansão da linha). */
export interface CandidaturaColaboradorItem {
  codigo: number;
  titulo: string;
  dataCriacao: string | null;
  dataUltimaAlteracao: string | null;
  statusCandidaturaId: string;
  statusCandidaturaDescricao: string;
  candidaturaId: string;
  pretensaoSalarial: string | null;
  modeloTrabalhoId: string | null;
  disponibilidadeEntrevistaId: string | null;
  quantidadeDiasPresencial: number | null;
  nomeCliente: string;
  codigoCliente: string;
  codigoGestor: string;
  nomeGestor: string;
  idVaga: string;
  statusVaga: string;
  dataCandidatura: string | null;
  codigoInternoColaborador: string | null;
}

/** Payload para inserir comentário em candidatura (movimentação de status). */
export interface InserirComentarioCandidaturaPayload {
  comentario: string;
  candidaturaId: string;
}

/** Candidatura no formato snake_case (prompt match). */
export interface CandidaturaPromptMatchItem {
  id_candidatura?: string;
  titulo_vaga?: string;
  codigo_cliente?: string | null;
  nome_cliente?: string | null;
  codigo_gestor?: string | null;
  nome_gestor?: string | null;
}

/** Retorno match (prompt) – detalhamento. */
export interface RetornoMatchPromptRaw {
  codigo_interno_colaborador?: string;
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
  [key: string]: unknown;
}

/** Última experiência do candidato (ex.: exibição na lista de match). */
export interface UltimaExperienciaItem {
  titulo?: string;
  descricao?: string;
}

/** Colaborador retornado por BuscarBancoTalentosComPromptMatch. */
export interface ColaboradorMatchPromptItem {
  origem?: string;
  nome?: string;
  codigo_interno_colaborador?: string;
  possui_candidatura?: boolean;
  candidaturas?: CandidaturaPromptMatchItem[] | null;
  match?: number;
  retornoMatch?: RetornoMatchPromptRaw | null;
  organizacoes?: Array<{ orgId?: number; orgDescricao?: string; ativoNaOrg?: boolean }>;
  /** Última experiência (simulação ou retorno futuro da API). */
  ultimaExperiencia?: UltimaExperienciaItem | null;
}

/** Item retornado por ListarMeusTalentos (candidaturas sob minha responsabilidade). */
export interface MeuTalentoItem {
  nomeColaborador: string;
  codigoVaga: string;
  nomeVaga: string;
  nomeCliente: string;
  nomeGestor: string;
  statusVaga: string;
  statusMovimentacao: string;
  ultimaAlteracao: string;
  codigoColaboradorUltimaMovimentacao: string | null;
  recrutadorUltimaMovimentacao: string;
  codColaborador: string;
  criadoPor: string;
}

/** Item de lote (minhas importações). */
export interface LoteItemGestao {
  id: string;
  dataCriacao?: string;
  dataInicioProcessamento?: string;
  processado?: boolean;
  totalItens?: number;
  quantidadeProcessada?: number;
  quantidadeAProcessar?: number;
  identificadorfila?: string;
}

/** Erro de item do lote. */
export interface ErroLoteItem {
  mensagemErro?: string;
  nomeArquivoCv?: string;
  codigoInternoColaborador?: string | null;
  idLote?: string;
  [key: string]: unknown;
}

/** Pessoa cadastrada no lote. */
export interface PessoaCadastradaNoLote {
  nome?: string;
  codigoInternoColaborador?: string;
  nomeCadastrante?: string | null;
  dataDoCadastro?: string;
  possuiCandidatura?: boolean;
  candidaturas?: unknown[] | null;
  match?: number;
}

/** Detalhes do lote (detalhes de upload). */
export interface DetalhesLote {
  lote?: LoteItemGestao & Record<string, unknown>;
  pessoasCadastradasNesteLote?: (PessoaCadastradaNoLote | null)[];
  erros?: (string | ErroLoteItem)[];
  pdfsAProcessar?: (string | { nomeArquivo?: string; nomeArquivoCv?: string; [key: string]: unknown })[];
}

/** Payload para atualizar candidatura (status 3 – modelo, pretensão, dias). */
export interface AtualizarCandidaturaPayload {
  idCandidatura: string;
  pretencaoSalarial: string;
  modeloTrabalhoId: string;
  quantidadeDiasPresencial: number;
}

/** Payload para reprovar candidatura (status 10). */
export interface ReprovarCandidaturaPayload {
  idCandidatura: string;
  comentario: string;
  idMotivoReprovacao: string;
}

/** Payload para declinar candidato (status 12). */
export interface DeclinarCandidaturaPayload {
  idCandidatura: string;
  idMotivoDeclinio: string;
  comentario: string;
}

/** Payload para mudar status da candidatura. */
export interface MudarStatusCandidaturaPayload {
  idCandidatura: string;
  codigoStatus: number;
  comentario: string;
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

/** Candidato inscrito (modelo normalizado para UI – Kanban). */
export interface CandidatoInscrito {
  id?: string;
  idCandidatura?: string;
  codigo?: string;
  nome?: string;
  nomeColaborador?: string;
  descricaoStatus?: string;
  statusCandidaturaId?: string;
  statusId?: string;
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
  /** Exibir bloco "Benefícios e Lançamento Complementar" no template de contratação (API ListarCandidatosInscritos). */
  exibirRemuneracao?: boolean;
  [key: string]: unknown;
}

/** Item bruto da API ListarCandidatosInscritos (entrada do normalizer). */
export interface CandidatoInscritoRaw {
  idCandidatura?: string;
  nome?: string;
  codigo?: string;
  candidatura?: string;
  ultimaAlteracao?: string;
  idStatusCandidatura?: string | number;
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
  origem?: string;
  exibirRemuneracao?: boolean;
  [key: string]: unknown;
}

/** Status de candidatura (coluna do Kanban). */
export interface StatusCandidatura {
  id: string;
  nome: string;
  descricao?: string;
  ordem?: number;
  ativo?: boolean;
  [key: string]: unknown;
}

/** Totais de inscritos por tipo (Total, Aprovados, Reprovados, Declinados). */
export interface TotaisInscritos {
  totalInscritos?: number;
  totalAprovados?: number;
  totalReprovados?: number;
  totalDeclinados?: number;
  [key: string]: number | undefined;
}

/** Retorno da API ObterTotaisInscritos (entrada do normalizer). */
export interface ObterTotaisInscritosRetorno {
  totalCandidatosInscritos?: Record<string, number>;
}

/** Item da API ListarUnidades. */
export interface UnidadeItem {
  id: string;
  descricao: string;
}

/** Item da API ListarTiposVaga. */
export interface TipoVagaItem {
  id: string;
  descricao: string;
}

/** Item da API ListarTiposContratacao. */
export interface TipoContratacaoItem {
  id: number;
  descricao: string;
}

/** Item bruto da API ListarCandidatosAderentes (aderentes à vaga). */
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

/** Item da API ListarOrigensColaborador (origem do colaborador). */
export interface OrigemColaboradorItem {
  id: string;
  descricao: string;
  ativo: boolean;
}

/** Payload para CandidatarOutraPessoa (inscrever colaborador na vaga). */
export interface CandidatarOutraPessoaPayload {
  codigoVaga: string | number;
  codigoColaborador: string;
  opcoesContatoIds: string[];
}

/** Payload para CandidatarSe (inscrição do usuário logado na vaga - portal público). */
export interface CandidatarSePayload {
  codigoVaga: number;
  opcoesContatoIds: number[];
}

/** Parâmetros para inserir arquivo em candidatura. */
export interface InserirArquivoCandidaturaParams {
  idCandidatura: string;
  idComentario: string;
  file: File;
  nomeArquivo?: string;
  tipoArquivo?: string;
}

/** Payload para inserir comentário da jornada da candidatura. */
export interface InserirComentarioCandidaturaPayload {
  comentario: string;
  candidaturaId: string;
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

/** Payload para MudarStatusVaga (POST). */
export interface MudarStatusVagaPayload {
  codigoVaga: string;
  codigoStatus: number;
  comentarioVaga?: string;
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

/** Item da API ListarOpcoesContato. */
export interface OpcaoContatoItem {
  id: string;
  descricao: string;
}

/** Item da API ListarVagasRecrutamentoPorParentEmAndamento (vaga filha). */
export interface VagaFilhaItem {
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
}

/** Retorno de ObterVagaRecrutamentoPorId e body de AtualizarVagaRecrutamentoPorId (edição de vaga). */
export interface VagaRecrutamentoCompleto {
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
  codigoGestor?: string | null;
  statusVagaCod?: string | null;
  idPerfilGerador?: string | null;
  modeloTrabalhoDescricao?: string | null;
  modeloTrabalhoId?: string | null;
  nomeGestor?: string | null;
  nomeCliente?: string | null;
  codigoCliente?: string | null;
  skills?: Array<{
    id: string;
    skillId: number;
    skillDescription: string;
    skillNivelDescription: string;
    tipoSkillId: number;
    typeSkillsDescription?: string;
    relevante?: boolean;
  }>;
  propostaCrm?: string | null;
  tipoVagaId?: string | null;
  tipoContratacaoId?: number | null;
  unidadeId?: string | null;
  recrutadorVaga?: string | null;
  nomeRecrutadorVaga?: string | null;
  maquinaColaborador?: string | null;
  emailsAnaliseGestor?: string[] | null;
  observacoesInternas?: string | null;
  [key: string]: unknown;
}
