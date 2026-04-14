export interface ColaboradorDadosPessoaisEndereco {
  id?: number | null;
  cep?: string | null;
  endereco?: string | null;
  complemento?: string | null;
  numero?: number | null;
  bairro?: string | null;
  cidade?: string | null;
  estado?: string | null;
  comQuemMora?: string | null;
  internacionalLinhaUm?: string | null;
  internacionalLinhaDois?: string | null;
}

export interface ColaboradorDadosPessoaisSaude {
  /** 0 = não PCD, 1 = PCD (derivado de enumPCD: 5 → 0, outro → 1). */
  pcd?: number | null;
  enumPCD?: number | null;
  grupoDeRiscoCovid?: number | null;
  condicaoDeSaudeRelevante?: string | null;
}

export interface ColaboradorDadosPessoais {
  codigoInternoColaborador: string;
  nomeCompleto?: string | null;
  dataNascimento?: string | null;
  rg?: string | null;
  matricula?: string | null;
  enderecoId?: number | null;
  ativo?: boolean | null;
  contatoPrincipalDdi?: string | null;
  contatoPrincipal?: string | null;
  contatoOutro?: string | null;
  imagemId?: number | null;
  candidato?: boolean | null;
  passaporte?: string | null;
  colaboradorSaudeId?: number | null;
  estadoCivil?: string | null;
  genero?: string | null;
  etnia?: string | null;
  orientacaoSexual?: string | null;
  escolaridade?: string | null;
  refugiado?: boolean | null;
  emailAlternativo?: string | null;
  email?: string | null;
  nacionalidade?: string | null;
  sobre?: string | null;
  documentoColaborador?: string | null;
  urlLinkedin?: string | null;
  dataSyncLinkedin?: string | null;
  visualizarBuscaAderencia?: boolean | null;
  qualificado?: boolean | null;
  endereco?: ColaboradorDadosPessoaisEndereco | null;
  saude?: ColaboradorDadosPessoaisSaude | null;
}

export interface ObterDadosColaboradorResponse {
  retorno?: ColaboradorDadosPessoais | null;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown;
}

export interface EditarDadosColaboradorPayload {
  codigoInternoColaborador: string;
  nomeCompleto: string;
  colaboradorSaudeId: number | string;
  nacionalidade: string;
  sobre: string;
  urlLinkedin: string;
  visualizarBuscaAderencia: boolean;
  qualificado: boolean;
  dataNascimento: string;
  documentoColaborador: string;
  rg: string;
  estadoCivil: string;
  escolaridade: string;
  etnia: string;
  genero: string;
  orientacaoSexual: string;
  refugiado: boolean;
  email: string;
  emailAlternativo: string;
  celular: string;
  passaporte: string;
  endereco: ColaboradorDadosPessoaisEndereco;
  matricula: string;
  ativo: boolean;
  contatoPrincipal: string;
  contatoPrincipalDdi: string;
  contatoOutro: string;
  candidato: boolean;
  saude: ColaboradorDadosPessoaisSaude;
}

export interface EditarDadosColaboradorResponse {
  retorno?: unknown;
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: string[] | null;
}

