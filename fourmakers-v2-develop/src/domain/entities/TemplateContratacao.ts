/** Valor default para "Grupo de E-mail - Contrato" ao criar template. String sem acentos para evitar erros. */
export const GRUPO_EMAIL_CONTRATO_DEFAULT = 'funcionarios@foursys.com.br';

/**
 * Detalhes da vaga/candidatura para exibição no cabeçalho do template de contratação.
 */
export interface CandidaturaDetalhesVaga {
  codigoVaga?: number | string | null;
  requisicao?: string | null;
  tipoContratacao?: string | null;
  tipoVaga?: string | null;
  tituloVaga?: string | null;
  modeloTrabalhoId?: string | null;
  nomeCliente?: string | null;
  requisitante?: string | null;
  dataAbertura?: string | null;
  descricaoBreveVaga?: string | null;
  status?: string | null;
  /** Gestor/responsável (API: colaboradorResponsavel). */
  colaboradorResponsavel?: string | null;
  /** Proposta / pretensão salarial (API: pretencaoSalarial). */
  pretencaoSalarial?: number | string | null;
  /** Frequência presencial em dias (API: quantidadeDiasPresencial). */
  quantidadeDiasPresencial?: number | null;
  /** Última alteração (API: ultimaAlteracao). */
  ultimaAlteracao?: string | null;
  /** Observações internas, se retornadas pela API. */
  observacoesInternas?: string | null;
  [key: string]: unknown;
}

/**
 * Endereço no template de contratação.
 */
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

/**
 * Saúde do candidato no template de contratação.
 */
export interface TemplateContratacaoSaude {
  pcd?: string | null;
  tipoPcd?: string | null;
  enumPCD?: number | null;
  grupoDeRiscoCovid?: number | null;
  condicaoDeSaudeRelevante?: string | null;
}

export interface TemplateContratacaoItemLiberado {
  id?: string;
  descricao?: string;
  /** Nome alternativo retornado por algumas APIs. */
  nome?: string;
  leitura?: boolean;
  escrita?: boolean;
  [key: string]: unknown;
}

/**
 * Template de contratação do candidato (entidade de domínio).
 */
export interface TemplateContratacaoData {
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
  /** DDD (2 dígitos) - campo separado conforme layout. */
  dddColaborador?: string | null;
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
  /** FourSys | Cliente */
  proprietarioMaquina?: string | null;
  observacoesAcessoUsuario?: string | null;
  grupoEmailContrato?: string | null;
  outrosGrupos?: (string | { emailGrupo?: string })[] | null;
  sistemasLiberados?: TemplateContratacaoItemLiberado[] | null;
  diretorios?: TemplateContratacaoItemLiberado[] | null;
  gruposEmails?: TemplateContratacaoItemLiberado[] | null;
  observacoesAprovadorAcessos?: string | null;
  /** Telefone adicional (DDD + número). */
  telefone?: string | null;
  /** Necessita de adaptação (Saúde do Candidato). */
  restricaoAdaptacao?: string | null;
  /** Observações do checklist de instalação/localização. */
  observacoesLocalizacao?: string | null;
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

/**
 * Resultado do caso de uso de obter template e detalhes da candidatura.
 */
export interface ObterTemplateContratacaoResult {
  template: TemplateContratacaoData | null;
  candidatura: CandidaturaDetalhesVaga | null;
}

// --- Tipos de listagem (equipamentos, diretórios, sistemas, grupos) ---
// Usados pelo repositório e pelo hook; evita dependência da apresentação em @data/api.

/** Opção de equipamento padrão (API ListarEquipamentosPadroesAninhados). */
export interface EquipamentoPadraoOpcao {
  tipoOpcao?: string | null;
  categoriaNome?: string | null;
  tipoEquipamento?: string | null;
  cpuGeracao?: string | null;
  memoriaRam?: string | null;
  armazenamentoDisco?: string | null;
  so?: string | null;
  gpu?: string | null;
}

/** Cargo/função com opções de equipamento (API ListarEquipamentosPadroesAninhados). */
export interface EquipamentoPadraoCargoFuncao {
  idCargoFuncao?: string | null;
  grupoArea?: string | null;
  nomeCargoFuncao?: string | null;
  opcoesEquipamento?: EquipamentoPadraoOpcao[] | null;
}

/** Grupo por área (ex.: Administrativo, Desenvolvedor) com cargos/funções. */
export interface EquipamentosPadroesAninhadosGrupo {
  grupoArea?: string | null;
  cargosFuncoes?: EquipamentoPadraoCargoFuncao[] | null;
}

/** Item de diretório (API ListarDiretorios – Acessos do Usuário). */
export interface DiretorioContratacaoItem {
  id?: string;
  descricao?: string;
  leitura?: boolean;
  escrita?: boolean;
}

/** Item de sistema liberado (API ListarSistemasLiberados – Acessos do Usuário). */
export interface SistemaLiberadoContratacaoItem {
  id?: string;
  descricao?: string;
}

/** Item de grupo de e-mail (API ListarGruposEmails – Acessos do Usuário). */
export interface GrupoEmailContratacaoItem {
  id?: string;
  descricao?: string;
}

/** Resultado agregado do use case de listar diretórios, sistemas liberados e grupos de e-mail. */
export interface ListagemAcessosUsuarioResult {
  diretorios: DiretorioContratacaoItem[];
  sistemasLiberados: SistemaLiberadoContratacaoItem[];
  gruposEmails: GrupoEmailContratacaoItem[];
}
