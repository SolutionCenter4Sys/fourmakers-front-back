export interface CargoProfile360 {
  cargo: string | null
  id: number
}

export interface DiretoriaProfile360 {
  id: string
  diretoria: string
  idExterno: string | null
}

export interface StatusProfile360 {
  id: number
  descricao: string | null
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface EnderecoProfile360 {
  cep: string
  endereco: string
  complemento: string
  numero: number
  bairro: string
  cidade: string
  estado: string
  comQuemMora: string
  internacionalLinhaUm: string | null
  internacionalLinhaDois: string | null
  id: string | null
}

export interface CandidatoProfile360 {
  id: number
  dataEstagioProcesso: string
  descricaoEstagioProcesso: string | null
  pathCurriculo: string | null
  pretencaoSalarial: number | null
  cargoAtualUltimo: string | null
  salarioAtualUltimo: number | null
  tipoContratoAtualUltimo: string | null
  modalidadeAtualUltima: string
  aceitaSugestoes: boolean | null
}

export interface CurriculoProfile360 {
  pathCurriculo: string
  ativo: boolean
  colaboradorCPF: string
}

export interface EmpresaVinculadaProfile360 {
  nomeFantasia: string
  cnpj: string
  confirmado: boolean
  pendente: boolean
  dataConvite: string
}

export interface SaudeProfile360 {
  pcd: string
  enumPCD: number
  grupoDeRiscoCovid: number
  condicaoDeSaudeRelevante: string
}

export interface VistoProfile360 {
  id: number
  idPais: number
  validade: string
  descricaoPais: string
}

export interface PassaporteProfile360 {
  id: number
  idNacionalidade: number
  validade: string
  descricaoNacionalidade: string
}

export interface ColaboradorProfile360 {
  cpf: string
  nomeCompleto: string
  sobre: string
  vistos: VistoProfile360[]
  passaportes: PassaporteProfile360[]
  email: string
  contatoPrincipal: string
  contatoPrincipalDDI: string | null
  contatoOutros: string | null
  slack_id: string | null
  dataNascimento: string
  cargo: CargoProfile360
  diretoria: DiretoriaProfile360
  status: StatusProfile360
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
  endereco: EnderecoProfile360
  flagCandidato: boolean
  flagAtivo: boolean
  candidato: CandidatoProfile360
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
  pessoaRefugiada: boolean
  emailAlternativo: string
  saude: SaudeProfile360
  nacionalidade: string | null
  colaboradorCurriculo: CurriculoProfile360
  empresasVinculadas: EmpresaVinculadaProfile360[]
  documentoColaborador: string
  urlLinkedin: string
  dataSincronizacaoLinkedin: string
  visualizarBuscaAderencia: boolean
  cidadanias: any[]
  quemCadastrou: string | null
  codigoModeloContratacao: string | null
}

export interface CompetenciaProfile360 {
  id: number
  idCompetencia: number
  idNivel: number | null
  colaboradorCpf: string | null
  listaIdCertificado: number[] | null
  competencia: {
    id: number
    descricao: string
    usuarioCriacaoId: number | null
    pendente: boolean
    ativo: boolean
  }
  data: string
  certificados: any[]
  endosso: any | null
  nivel: {
    id: number
    descricao: string
    prioridadeUnificacao: number
    ordemExibicao: number | null
  }
  endossoConcedido: any[]
}

export interface DominioProfile360 {
  id: number
  idDominio: number
  idNivel: number | null
  colaboradorCpf: string | null
  dominio: {
    id: number
    nome: string | null
    descricao: string
    pendente: boolean
  }
  data: string
  endosso: {
    descricaoStatus: string | null
    endossado: boolean
    nivelEndosso: number
    quantidadeEndosso: number
    quantidadeSolicitacaoEndosso: number
  }
  nivel: {
    id: number
    descricao: string
    prioridadeUnificacao: number
    ordemExibicao: number | null
  }
  endossoConcedido: any[]
}

export interface MetodologiaProfile360 {
  metodologia: {
    id: number
    descricao: string
    gestorExternoPerfil: string | null
    usuarioCriacaoId: number
    pendente: boolean
    nivelId: number
    ativo: boolean
  }
  perfilMetodologia: any | null
  nivel: {
    id: number
    descricao: string
    prioridadeUnificacao: number
    ordemExibicao: number | null
  }
}

export interface SoftSkillProfile360 {
  id: number
  idSoftSkill: number
  idNivel: number | null
  colaboradorCpf: string | null
  softSkill: {
    id: number
    nome: string | null
    descricao: string
    pendente: boolean
  }
  data: string
  nivel: {
    id: number
    descricao: string
    prioridadeUnificacao: number
    ordemExibicao: number | null
  }
}

export interface IdiomaProfile360 {
  id: number
  idIdioma: number
  idNivel: number | null
  colaboradorCpf: string | null
  idioma: {
    id: number
    descricao: string
    cpfUsuarioCriacao: string | null
    pendente: boolean
  }
  data: string
  endosso: {
    descricaoStatus: string | null
    endossado: boolean
    nivelEndosso: number
    quantidadeEndosso: number
    quantidadeSolicitacaoEndosso: number
  }
  nivel: {
    id: number
    descricao: string
    prioridadeUnificacao: number
    ordemExibicao: number | null
  }
  endossoConcedido: any[]
}

export interface ExperienciaEmpresaProfile360 {
  id: number
  funcao: string
  projetos: string[]
  atividades: string
  dataInicio: string
  dataSaida: string | null
  atual: boolean
  colaboradorCpf: string
}

export interface RealizacaoProfile360 {
  cliente: string
  projeto: string
  horas: number
  dataInicio: string
  dataFim: string
  atual: boolean
  perfil: string
  skills: Array<{
    id: number
    descricao: string
    nivel: {
      id: number
      descricao: string
      prioridadeUnificacao: number
      ordemExibicao: number | null
    }
    tipoSkill: string
    relevante: boolean
    perfilId: number | null
  }>
}

export interface CertificadoProfile360 {
  path: string | null
  thumb: string | null
  idCertificado: number
  principal: boolean
  ativo: boolean
  conclusao: string
  descricao: string
  instituicao: string
  cargaHoraria: number
  codigoInternoColaborador: string | null
}

export interface CertificadoCompletoProfile360 {
  competencia: any | null
  certificado: CertificadoProfile360
}

export interface ExperienciaEmpresaCompletaProfile360 {
  empresa: string
  dataInicio: string
  dataSaida: string | null
  atual: boolean
  experiencias: ExperienciaEmpresaProfile360[]
  realizacoes: RealizacaoProfile360[]
}

export interface PerfilProfissionalProfile360 {
  competencias: CompetenciaProfile360[]
  formacoes: any | null
  dominios: DominioProfile360[]
  metodologias: MetodologiaProfile360[]
  modelos: any | null
  interesses: any | null
  hobbies: any | null
  softskills: SoftSkillProfile360[]
  certificados: CertificadoCompletoProfile360[]
  idiomas: IdiomaProfile360[]
  experienciaEmpresas: ExperienciaEmpresaCompletaProfile360[]
}

export interface DependenteColaboradorProfile360 {
  dependentes: any[]
  quantidadeDependentes: number
  quantidadeDependentesFilhos: number
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface BuscarDadosColaboradorResponse {
  colaborador: ColaboradorProfile360
  redeColaborador: any | null
  perfilProfissional: PerfilProfissionalProfile360
  atividadeColaborador: any | null
  dependenteColaborador: DependenteColaboradorProfile360
  srsCandidate: any | null
  usuario: any | null
  totalResultCount: number
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}
