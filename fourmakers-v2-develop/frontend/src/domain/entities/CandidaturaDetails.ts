export interface CandidaturaColaborador {
  codigoInternoColaborador: string;
  nomeCompleto?: string | null;
  dataNascimento?: string | null;
  rg?: string | null;
  documentoColaborador?: string | null;
  contatoPrincipal?: string | null;
  contatoOutro?: string | null;
  contatoPrincipalDdi?: string | null;
  email?: string | null;
  emailAlternativo?: string | null;
  matricula?: string | null;
  ativo?: boolean;
  candidato?: boolean;
  passaporte?: string | null;
  estadoCivil?: string | null;
  genero?: string | null;
  etnia?: string | null;
  orientacaoSexual?: string | null;
  escolaridade?: string | null;
  refugiado?: boolean;
  nacionalidade?: string | null;
  sobre?: string | null;
  urlLinkedin?: string | null;
  dataSyncLinkedin?: string | null;
  visualizarBuscaAderencia?: boolean;
  qualificado?: boolean;
  colaboradorSaudeId?: number | null;
  endereco?: {
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
    id?: number | null;
  } | null;
  saude?: {
    pcd?: number | null;
    grupoDeRiscoCovid?: number | null;
    condicaoDeSaudeRelevante?: string | null;
  } | null;
}

export interface CandidaturaDados {
  idCandidatura: string;
  modeloTrabalhoId?: string | null;
  quantidadeDiasPresencial?: number | null;
  pretencaoSalarial?: number | null;
  /** Pretensão de ajuda de custo; preenchido quando a API retornar. */
  ajudaDeCusto?: number | null;
  /** CEP do trabalho (Informações da Vaga); aplicado no campo CEP do Trabalho quando disponível. */
  cep?: string | null;
}

export interface CandidaturaFilho {
  id?: string | null;
  dataNascimento?: string | null;
}

export interface CandidaturaOutroCusto {
  id?: string | null;
  descricao: string;
  valor: number;
}

export interface CandidaturaDadosDemograficos {
  quantidadePessoasResidencia?: number | null;
  dependentesIRPF?: number | null;
  possuiConjuge?: boolean | null;
  dataNascimentoConjuge?: string | null;
  possuiFilhos?: boolean | null;
  filhos?: CandidaturaFilho[] | null;
  possuiSeguroSaude?: boolean | null;
  valorAtualSeguroSaude?: number | null;
  operadoraSeguroSaude?: string | null;
  acomodacaoSeguroSaude?: string | null;
  seguroSaudePossuiCoparticipacao?: boolean | null;
  observacoesSeguroSaude?: string | null;
  possuiInteressePlanoFoursys?: boolean | null;
  faixaEtaria?: string | null;
  categoriaPlanoSaude?: string | null;
  incluirDependentesPlanoFoursys?: boolean | null;
  quantidadeDependentesPlanoFoursys?: number | null;
  valorPlanoDependentes?: number | null;
  valorCartaoRefeicao?: number | null;
  valorCartaoAlimentacao?: number | null;
  estudaAtualmente?: boolean | null;
  custoMensalEducacao?: number | null;
  filhosEstudamAte24Anos?: boolean | null;
  custoMensalEducacaoFilhos?: number | null;
  custoTotalEducacao?: number | null;
  modeloDeTrabalhoPretendido?: string | null;
  diasPresenciaisDesejados?: string | null;
  pretencaoLiquidaRef?: number | null;
  distanciaIdaVolta?: number | null;
  outrosCustos?: CandidaturaOutroCusto[] | null;
}

export interface CandidaturaDetails {
  colaborador: CandidaturaColaborador;
  dadosCandidatura?: CandidaturaDados | null;
  dadosDemograficos?: CandidaturaDadosDemograficos | null;
}

export interface CandidaturaEditPayload {
  colaborador: CandidaturaColaborador;
  dadosDemograficos?: CandidaturaDadosDemograficos | null;
  dadosCandidatura?: CandidaturaDados | null;
}
