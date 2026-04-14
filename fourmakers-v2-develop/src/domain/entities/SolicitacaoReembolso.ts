export interface SolicitacaoDocumento {
  id: number
  tipo: string
  url: string
}

export interface SolicitacaoObjeto {
  id: number
  clienteId: string
  clienteDescricao: string
  projetoId: string
  projetoDescricao: string
  categoria: string
  valorSolicitado: number
  valorAprovado: number
  dataAprovacao: string | null
  data: string
  status: string
  statusId: number
  observacao: string | null
  objetivo: string
  destino: string | null
  dataInicio: string | null
  dataFim: string | null
  solicitacaoDocumentos: SolicitacaoDocumento[]
}

export interface SolicitacaoReembolso {
  nomeColaborador: string
  objetivo: string
  destino: string | null
  periodo: string
  cliente: string
  projeto: string
  dataSolicitacao: string
  somaValores: number
  objeto: SolicitacaoObjeto[]
}

export interface ListarSolicitacoesResponse {
  solicitacoes: SolicitacaoReembolso[]
  totalSolicitado: number
  totalAprovado: number
  souAprovador: boolean
  souGestor: boolean
  saldo: number
}

export interface ListarSolicitacoesApiResponse {
  retorno: ListarSolicitacoesResponse
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface SolicitacaoObjetoVisaoAdm {
  id: number
  codigoColaborador: string
  nomeColaborador: string
  valor: number // Campo retornado pela API
  valorSolicitado?: number // Campo alternativo (se existir)
  valorAprovado: number
  dataSolicitacao: string
  status: string
  statusId: number
  projeto: string
  cliente: string
  observacao: string | null
  descricao: string
  totalDespesas: number
  nomeAprovador: string | null
  valorExcecao: number | null
  custoClienteExcecao: number | null
  objetivo: string
  destino: string | null
  dataInicio: string | null
  dataFim: string | null
  categoria?: string // Campo opcional (pode não existir no retorno)
  solicitacaoDocumentos?: SolicitacaoDocumento[]
}

export interface SolicitacaoVisaoAdm {
  nomeColaborador: string
  saldoAdiantamentos?: number
  objetivo: string
  destino: string | null
  periodo: string
  cliente: string
  projeto: string
  dataSolicitacao: string
  somaValores: number
  objeto: SolicitacaoObjetoVisaoAdm[]
}

export interface ListarSolicitacoesVisaoAdmResponse {
  solicitacoes: SolicitacaoVisaoAdm[]
  colaboradores: number
  valorSolicitado: number
  valorAprovado: number
  valorReprovado: number
  valorPendente: number
  valorPago: number
}

export interface ListarSolicitacoesVisaoAdmApiResponse {
  retorno: ListarSolicitacoesVisaoAdmResponse
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface SolicitacaoObjetoGerenteProjeto {
  id: number
  codigoInternoColaborador: string
  colaborador: string
  clienteDescricao: string
  projetoDescricao: string
  observacao: string | null
  valor: string
  status: string
  objetivo: string
  destino: string | null
  dataInicio: string | null
  dataFim: string | null
  dataSolicitacao: string
  solicitacaoDocumentos?: SolicitacaoDocumento[]
}

export interface SolicitacaoGerenteProjeto {
  nomeColaborador: string
  objetivo: string
  destino: string | null
  periodo: string
  cliente: string
  projeto: string
  dataSolicitacao: string
  somaValores: number
  objeto: SolicitacaoObjetoGerenteProjeto[]
}

export interface ListarSolicitacoesGerenteProjetoApiResponse {
  retorno: SolicitacaoGerenteProjeto[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface BuscarSolicitacaoBigNumbersResponse {
  qtdColaborador: number
  valoresLancados: number
  valoresAprovados: number
  valoresReprovados: number
  valoresPendentes: number
}

export interface BuscarSolicitacaoBigNumbersApiResponse {
  retorno: BuscarSolicitacaoBigNumbersResponse
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface SolicitacaoDocumentoAprovacao {
  id: number
  tipo: string
  url: string
}

export interface SolicitacaoAprovacaoPorColaborador {
  id: number
  clienteProjeto?: string // Campo opcional (formato antigo)
  clienteDescricao?: string // Campo novo
  projetoDescricao?: string // Campo novo
  categoriaId: number
  categoriaDescricao: string
  dataDespesa: string
  valor: number
  descricao: string
  statusId: number
  statusDescricao: string
  solicitacaoDocumentos: SolicitacaoDocumentoAprovacao[]
  // Campos adicionais que podem vir na resposta
  dataInicio?: string | null
  dataFim?: string | null
  dataSolicitacao?: string
  destino?: string | null
  colaborador?: string
  objetivo?: string
}

export interface SolicitacaoAprovacaoAgrupada {
  nomeColaborador: string
  objetivo: string
  destino: string | null
  periodo: string
  cliente: string
  projeto: string
  dataSolicitacao: string
  somaValores: number
  objeto: SolicitacaoAprovacaoPorColaborador[]
}

export interface ListarSolicitacoesAprovacaoPorColaboradorApiResponse {
  retorno: SolicitacaoAprovacaoAgrupada[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ListarProjetosColaboradorParams {
  codigoProfissional?: string
  ehTbd?: boolean | null
  codigoGerenteProjeto?: string
  listaCodigoCliente?: string[]
  status?: string
  prioritarioFiltro?: number
}

