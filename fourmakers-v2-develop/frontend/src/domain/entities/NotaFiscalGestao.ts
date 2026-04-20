/**
 * Entidades relacionadas à Gestão de Notas Fiscais
 */

/**
 * Status de Nota Fiscal retornado pela API
 */
export interface NotaFiscalStatus {
  id: number
  nome: string
  descricao?: string
}

/**
 * Resposta da API para listar status de notas fiscais
 */
export interface ListarNotaFiscalStatusResponse {
  sucesso: boolean
  mensagem?: string
  retorno?: NotaFiscalStatus[]
}

/**
 * Unidade retornada pela API
 */
export interface Unidade {
  id: string
  descricao: string
}

/**
 * Resposta da API para listar unidades
 */
export interface ListarUnidadesResponse {
  ListaUnidadesResult?: Unidade[]
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
  retorno?: Unidade[] // Fallback para outras estruturas
}

/**
 * Colaborador retornado pela API de Mapa de Alocação
 */
export interface ColaboradorMapaAlocacao {
  codProfissional: string
  nomeProfissional: string
  labelCodigoNome: string
  cpf: string
  ehTbd: boolean
}

/**
 * Resposta da API para listar colaboradores e TBDs
 */
export interface ListarColaboradoresETbdsResponse {
  sucesso: boolean
  mensagem?: string
  retorno?: {
    colaboradores?: ColaboradorMapaAlocacao[]
    tbds?: ColaboradorMapaAlocacao[]
  }
}

/**
 * Parâmetros para listar colaboradores e TBDs
 */
export interface ListarColaboradoresETbdsParams {
  codigoDiretoria?: number
  codigoGestor?: number
  filtroTipoProfissional?: number
  codigoDepartamento?: string
}

/**
 * Parâmetros para listar notas fiscais por vigência (visão gestor)
 */
export interface ListarNotasFiscaisPorVigenciaVisaoGestorParams {
  filtro?: string
  mes?: number
  ano?: number
  statusId?: number | string
  documentoColaborador?: string
  codDiretoria?: string
  cursor?: number
  limite?: number
}

/**
 * Rubrica de nota fiscal retornada pela API
 */
export interface RubricaNotaFiscal {
  rubricaDescricao?: string | null
  tipo?: string | null
  codigoRubrica?: string | null
  natureza?: string | null
  id?: string | null
  valor?: number | null
}

/**
 * Nota fiscal retornada pela API
 */
export interface NotaFiscalPorVigencia {
  id: string
  nomeColaborador?: string | null
  documentoColaborador?: string | null
  notaFiscalStatusDescricao?: string | null
  rubricas?: RubricaNotaFiscal[] | null
  sumarioValorTotalDeRubricas?: number | null
  vigenciaMes?: number | null
  vigenciaAno?: number | null
  numeroNf?: string | null
  dataEmissaoNotaFiscal?: string | null
  valor?: number | null
  valorAnalisado?: number | null
  urlNotaFiscalDownload?: string | null
  notaFiscalStatusId?: number | null
  motivoReprovacao?: string | null
}

/**
 * Resposta da API para listar notas fiscais por vigência (visão gestor)
 */
export interface ListarNotasFiscaisPorVigenciaVisaoGestorResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
  retorno?: NotaFiscalPorVigencia[]
  cursor?: number
  total?: number
}

/**
 * Parâmetros para aprovar notas fiscais
 */
export interface AprovarNotasFiscaisParams {
  ids: string[]
  motivoReprovacao?: string
}

/**
 * Resposta da API para aprovar notas fiscais
 */
export interface AprovarNotasFiscaisResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

/**
 * Parâmetros para reprovar notas fiscais (motivoReprovacao obrigatório)
 */
export interface ReprovarNotasFiscaisParams {
  ids: string[]
  motivoReprovacao: string
}

/**
 * Resposta da API para reprovar notas fiscais
 */
export interface ReprovarNotasFiscaisResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

/**
 * Parâmetros para liberar emissão de notas fiscais por vigência
 */
export interface LiberarEmissaoNotasFiscaisPorVigenciaParams {
  mes: number
  ano: number
  enviarEmail: boolean
  codigoDiretoria: string
}

/**
 * Resposta da API para liberar emissão de notas fiscais por vigência
 */
export interface LiberarEmissaoNotasFiscaisPorVigenciaResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

/**
 * Nota fiscal associada a uma rubrica
 */
export interface NotaFiscalRubrica {
  statusNotaFiscal: number
  numeroNotaFiscal?: string | null
  statusDescricao?: string | null
}

/**
 * Rubrica do colaborador para liberação de NF
 */
export interface RubricaColaborador {
  id: string
  descricao: string
  valor: number
  emUso: boolean
  natureza: string
  notasFiscais?: NotaFiscalRubrica[] | null
}

/**
 * Parâmetros para listar rubricas do colaborador para liberação de NF
 */
export interface ListarRubricasColaboradorParaLiberacaoDeNfParams {
  mes: number
  ano: number
}

/**
 * Resposta da API para listar rubricas do colaborador para liberação de NF
 */
export interface ListarRubricasColaboradorParaLiberacaoDeNfResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
  retorno?: RubricaColaborador[]
}

/**
 * Base64 objeto para upload de arquivo
 */
export interface Base64Objeto {
  base64: string
  tipo: number // 2 = PDF/Imagem
}

/**
 * Parâmetros para inserir nota fiscal
 */
export interface InserirNotaFiscalParams {
  base64Objeto: Base64Objeto
  numeroNf: string
  vigenciaMes: number
  vigenciaAno: number
  listaDeIdsRubricasLiberacao: string[]
}

/**
 * Resposta da API para inserir nota fiscal
 */
export interface InserirNotaFiscalResponse {
  sucesso: boolean
  mensagem?: string | null
  erros?: string[] | null
}

