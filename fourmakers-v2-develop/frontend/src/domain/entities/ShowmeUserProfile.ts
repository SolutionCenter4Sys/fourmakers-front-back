export interface FuncionalidadeSistema {
  id: number
  descricao: string
  ativo: boolean
  dataCriacao: string
  dataAlteracao: string
}

export interface Cargo {
  cargo: string | null
  id: number
}

export interface Diretoria {
  id: string
  diretoria: string
  id_externo: string | null
}

export interface Status {
  id: number
  descricao: string | null
  sucesso: boolean
  mensagem: string | null
  erros: any | null
}

export interface Endereco {
  cep: string
  endereco: string
  complemento: string
  numero: number
  address_number: string | null
  bairro: string
  cidade: string
  estado: string
  com_quem_mora: string
  internacional_linha_um: string | null
  internacional_linha_dois: string | null
}

export interface Candidato {
  id: number
  dataEstagioProcesso: string
  descricaoEstagioProcesso: string | null
  pathCurriculo: string | null
  pretencaoSalarial: string | null
  cargoAtualUltimo: string | null
  salarioAtualUltimo: string | null
  tipoContratoAtualUltimo: string | null
  modalidadeAtualUltima: number
  aceitaSugestoes: boolean | null
}

export interface Saude {
  pcd: number
  enum_pcd: number
  grupoDeRiscoCovid: number
  condicaoDeSaudeRelevante: string
}

export interface Curriculo {
  pathCurriculo: string
  ativo: boolean
  colaboradorCPF: string
}

export interface EmpresaVinculada {
  nome_fantasia: string
  cnpj: string
  confirmado: boolean
  pendente: boolean
  data_convite: string
}

export interface ColaboradorDetails {
  cpf: string
  /** Código interno do colaborador (UUID), usado ex.: para comparar autor de publicação no feed. */
  codigoColaboradorInterno?: string
  nomeCompleto: string
  sobre: string
  vistos: any[]
  passaportes: any[]
  email: string
  contatoPrincipal: string
  contatoPrincipalDDI: string | null
  contatoOutros: string | null
  slack_id: string | null
  dataNascimento: string
  cargo: Cargo
  diretoria: Diretoria
  status: Status
  rg: string
  fcmToken: string | null
  matricula: string
  dataAdmissao: string
  modeloContratacao: string | null
  empresaRelacionada: string | null
  modeloTrabalho: string | null
  diasPorSemana: number | null
  valorHora: number | null
  custoHora: number | null
  baseHoraMes: number | null
  endereco: Endereco
  flagCandidato: boolean
  flagAtivo: boolean
  candidato: Candidato
  urlFoto: string
  urlFotoThumb: string
  urlFotoThumbMini: string
  urlFotoThumbVeryMini: string
  meSegue: boolean
  estouSeguindo: boolean
  acessoBackoffice: boolean
  acessoBuscaAvançada: boolean
  passaporte: string
  estadoCivil: string
  etnia: string
  genero: string
  orientacaoSexual: string
  escolaridade: string
  pessoa_refugiada: boolean
  email_alternativo: string
  saude: Saude
  nacionaliade: string | null
  curriculo: Curriculo
  empresas_vinculadas: EmpresaVinculada[]
  documentoColaborador: string
  urlLinkedin: string
  dataSincronizacaoLinkedin: string
  visualizarBuscaAderencia: boolean
  cidadanias: any | null
  quemCadastrou: any | null
  codigoModeloContratacao: any | null
}

export interface ColaboradorOrgDetails {
  cpf: string
  orgId: number
  codColaborador: string
  codCargo: string
  cargo: string
  codDiretoria: string
  diretoria: string
  codDepartamento: string
  departamento: string
  dataAdmissao: string
  modeloContratacao: string
  empresaRelacionada: string
  modeloTrabalho: string
  diasPorSemana: number | null
  valorHora: number
  custoHora: number
  baseHoraMes: number | null
  idioma: string
  ativo: boolean
  codigoModeloContratacao: string | null
}

export interface OrgHierarquiaDetails {
  codigoProfissionalSuperior: string
  nomeProfissionalSuperior: string
  codigoInternoProfissionalSuperior: string
  emailProfissionalSuperior: string
}

export interface ShowmeUserProfile {
  nomeColaborador: string
  contatoPrincipalDDI: string | null
  contatoPrincipal: string
  usuarioId: number
  cpf: string
  email: string
  colaborador: ColaboradorDetails
  colaboradorOrg: ColaboradorOrgDetails
  orgHierarquia: OrgHierarquiaDetails
  funcionalidadeSistema: FuncionalidadeSistema[]
  ultimaAlteracao: { origem: string; data: string }
  orgId: number
  souGestorDeAprovadores: boolean
  ocultaTimeSheet: boolean
  questionariosPreenchidos: string[]
  questionariosAtivos: string[]
}

