import { httpClient } from './httpClient'
import type { ListarDiretoriasDisponiveisResponse } from '@domain/entities/Diretoria'

// ============================================
// TIPOS - ESTRUTURAS COMPLETAS
// ============================================

export type StatusRemessa = 'GERANDO ARQUIVO' | 'ARQUIVO GERADO' | 'FINALIZADO' | 'CANCELADO'
export type TipoRemessa = string // Tipo dinâmico baseado nos módulos retornados pela API
export type FormaPagamento = 'PIX' | 'TED' | 'DOC'
export type StatusCnab = 'AGUARDANDO' | 'PAGO' | 'ERRO'

export interface SolicitacaoReembolsoCNAB {
  id: string
  tipoSolicitacao: TipoRemessa
  valorPagamento: number
  descricao: string
  dataSolicitacao: string
}

export interface LancamentoRemessa {
  id: string
  codigoInternoColaborador: string
  nomeColaborador: string
  cpfColaborador: string
  valorPagamentoTotal: number
  formaPagamento: FormaPagamento
  statusCnab: StatusCnab
  descricaoErro: string | null
  solicitacoes: SolicitacaoReembolsoCNAB[]
}

export interface RemessaCnab {
  id: string
  hashRemessa: string
  tipo: TipoRemessa
  competencia: string
  mesAnoProcessamento?: string
  status: StatusRemessa
  nomeArquivoRemessa: string | null
  nomeArquivoRetorno: string | null
  valorTotal: number
  dataCriacao: string
  lancamentos: LancamentoRemessa[]
}

// Interface original usada em outros contextos
export interface SolicitacaoPagamento {
  id: number
  codigoColaborador: string
  nome: string
  cliente: string
  projeto: string
  dataDaDespesa: string
  dataDoPedido: string
  dataDaAprovacao: string
  nomeDoAprovador: string
  valorSolicitado: number
  valorAprovado: number
  valorAbatidoDoSaldo: number
  valorParaPagamento: number
  custoCliente: boolean
  operacao: string
  solicitacaoReembolsoId: number
  formaPagamento: FormaPagamento | null
}

// Interface específica para retorno do CNAB
export interface SolicitacaoRemessaCNAB {
  id: string
  codigoColaborador: string
  nome: string
  cliente: string
  projeto: string
  valorParaPagamento: number
  dataSolicitacao: string
}

export interface ColaboradorSolicitacaoCNAB {
  codigoColaborador: string
  nomeColaborador: string
  quantidadeSolicitacoes: number
  valorTotal: number
  formaPagamento: FormaPagamento
  solicitacoes: SolicitacaoRemessaCNAB[]
}

// ============================================
// RESPONSES
// ============================================

export interface ListarRemessasCnabResponse {
  retorno: {
    remessas: RemessaCnab[]
    totalRemessas: number
  }
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ListarSolicitacoesPagamentoCNABResponse {
  retorno: {
    colaboradores: ColaboradorSolicitacaoCNAB[]
    totalSolicitacoes: number
    totalColaboradores: number
    valorTotal: number
  }
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ProcessarRemessaBancariaCNABResponse {
  retorno: {
    remessas: RemessaCnab[]
    totalRemessas: number
  }
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface ProcessarRetornoCnabResponse {
  retorno: {
    retornoId: string
    totalLinhasProcessadas: number
    pagamentosAtualizados: number
    mensagem: string
  }
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface BuscarReembolsosPagosViaCNABResponse {
  retorno?: any[] // Estrutura não fornecida ainda
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

export interface BuscarModulosRemessaResponse {
  retorno: string[]
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
}

// ============================================
// PARÂMETROS
// ============================================

export interface ListarRemessasCnabParams {
  tipoRemessa: TipoRemessa // Obrigatório: Ex: "REEMBOLSO"
  competencia?: string // Opcional: Formato: "MM/YYYY" (ex: "12/2025")
  status?: StatusRemessa // Opcional: 'GERANDO ARQUIVO' | 'ARQUIVO GERADO' | 'FINALIZADO' | 'CANCELADO'
}

export interface ListarSolicitacoesPagamentoCNABParams {
  tipoRemessa: TipoRemessa // Ex: "REEMBOLSO"
  codDiretoria?: string // Código da diretoria (opcional, vazio se não fornecido ou se for TODOS)
}

export interface ProcessarRemessaBancariaCNABParams {
  tipoRemessa: TipoRemessa // Ex: "REEMBOLSO"
  codDiretoria: string | null // Código da diretoria ou null para "TODOS"
  solicitacoesPagamentoIds: string[] // IDs das solicitações selecionadas (como string)
}

export interface ProcessarRetornoCnabParams {
  file: File
  hashRemessa: string // Obrigatório: hash da remessa
  tipoRemessa: TipoRemessa // Obrigatório: tipo da remessa (ex: "REEMBOLSO")
}

export interface BuscarReembolsosPagosViaCNABParams {
  competencia?: string
  dataInicio?: string
  dataFim?: string
}

// ============================================
// API CLASS
// ============================================

export class IntegracaoBancariaApi {
  /**
   * 0. Listar Remessas CNAB
   * GET /api/Financeiro/IntegracaoBancaria/RemessaBancaria/ListarRemessasCnab
   */
  async listarRemessasCnab(
    token: string,
    params: ListarRemessasCnabParams
  ): Promise<ListarRemessasCnabResponse> {
    // Construir query string manualmente para manter a barra na competência sem codificação
    const queryParts: string[] = [
      `tipoRemessa=${encodeURIComponent(params.tipoRemessa)}`
    ]
    
    if (params.competencia) {
      queryParts.push(`mesAnoProcessamento=${params.competencia}`)
    }
    if (params.status) {
      queryParts.push(`status=${encodeURIComponent(params.status)}`)
    }

    const queryString = `?${queryParts.join('&')}`
    
    console.log('[IntegracaoBancariaApi] Listando remessas com params:', params)
    console.log('[IntegracaoBancariaApi] Token presente:', !!token)
    
    const data = await httpClient.get<ListarRemessasCnabResponse>(
      `/api/Financeiro/IntegracaoBancaria/RemessaBancaria/ListarRemessasCnab${queryString}`,
      { token }
    )
    
    console.log('[IntegracaoBancariaApi] Resposta completa remessas:', JSON.stringify(data, null, 2))
    console.log('[IntegracaoBancariaApi] Total de remessas:', data.retorno?.remessas?.length || 0)
    return data
  }

  /**
   * 1. Listar Solicitações de Pagamento CNAB
   * GET /api/Financeiro/IntegracaoBancaria/RemessaBancaria/ListarSolicitacoesPagamentoCNAB
   */
  async listarSolicitacoesPagamentoCNAB(
    token: string,
    params: ListarSolicitacoesPagamentoCNABParams
  ): Promise<ListarSolicitacoesPagamentoCNABResponse> {
    // Construir query string
    const queryParts = [
      `tipoRemessa=${encodeURIComponent(params.tipoRemessa)}`
    ]
    
    // Adicionar codDiretoria apenas se fornecido e não vazio
    if (params.codDiretoria) {
      queryParts.push(`codDiretoria=${encodeURIComponent(params.codDiretoria)}`)
    }
    
    const queryString = queryParts.join('&')

    console.log('[IntegracaoBancariaApi] TipoRemessa enviado:', params.tipoRemessa)
    console.log('[IntegracaoBancariaApi] CodDiretoria enviado:', params.codDiretoria || 'vazio')

    const data = await httpClient.get<ListarSolicitacoesPagamentoCNABResponse>(
      `/api/Financeiro/IntegracaoBancaria/RemessaBancaria/ListarSolicitacoesPagamentoCNAB?${queryString}`,
      { token }
    )
    
    console.log('[IntegracaoBancariaApi] Resposta completa:', data)
    return data
  }

  /**
   * 2. Processar Remessa Bancária CNAB
   * POST /api/Financeiro/IntegracaoBancaria/RemessaBancaria/ProcessarRemessaBancariaCNAB
   * 
   * Processa todas as solicitações aprovadas para os filtros informados
   */
  async processarRemessaBancariaCNAB(
    token: string,
    params: ProcessarRemessaBancariaCNABParams
  ): Promise<ProcessarRemessaBancariaCNABResponse> {
    const body = {
      TipoRemessa: params.tipoRemessa,
      CodDiretoria: params.codDiretoria,
      SolicitacoesPagamentoIds: params.solicitacoesPagamentoIds,
    }

    console.log('[IntegracaoBancariaApi] Processando remessa com body:', body)

    return httpClient.post<ProcessarRemessaBancariaCNABResponse>(
      '/api/Financeiro/IntegracaoBancaria/RemessaBancaria/ProcessarRemessaBancariaCNAB',
      body,
      { token }
    )
  }

  /**
   * 3. Processar Retorno CNAB
   * POST /api/Financeiro/IntegracaoBancaria/RetornoBancaria/ProcessarRetornoCnab
   * Form-data: file / hashRemessa
   * 
   * Processa o arquivo de retorno do banco e atualiza os status dos pagamentos
   */
  async processarRetornoCnab(
    token: string,
    params: ProcessarRetornoCnabParams
  ): Promise<ProcessarRetornoCnabResponse> {
    const formData = new FormData()
    formData.append('file', params.file)
    formData.append('hashRemessa', params.hashRemessa)
    formData.append('tipoRemessa', params.tipoRemessa)

    console.log('[IntegracaoBancariaApi] Processando retorno CNAB - tipoRemessa:', params.tipoRemessa, 'hashRemessa:', params.hashRemessa)

    // Para FormData, o httpClient.request agora detecta automaticamente e não adiciona Content-Type
    return httpClient.request<ProcessarRetornoCnabResponse>({
      url: '/api/Financeiro/IntegracaoBancaria/RetornoBancaria/ProcessarRetornoCnab',
      method: 'POST',
      token,
      body: formData,
    })
  }

  /**
   * 4. Buscar Reembolsos Pagos via CNAB
   * GET /api/Financeiro/IntegracaoBancaria/Externo/BuscarReembolsosPagosViaCNAB
   * 
   * Busca reembolsos que foram pagos através de remessas CNAB
   */
  async buscarReembolsosPagosViaCNAB(
    token: string,
    params?: BuscarReembolsosPagosViaCNABParams
  ): Promise<BuscarReembolsosPagosViaCNABResponse> {
    // Construir query string manualmente para manter a barra na competência sem codificação
    const queryParts: string[] = []
    
    if (params?.competencia) {
      queryParts.push(`competencia=${params.competencia}`)
    }
    if (params?.dataInicio) {
      queryParts.push(`dataInicio=${encodeURIComponent(params.dataInicio)}`)
    }
    if (params?.dataFim) {
      queryParts.push(`dataFim=${encodeURIComponent(params.dataFim)}`)
    }

    const queryString = queryParts.length > 0 ? `?${queryParts.join('&')}` : ''

    return httpClient.get<BuscarReembolsosPagosViaCNABResponse>(
      `/api/Financeiro/IntegracaoBancaria/Externo/BuscarReembolsosPagosViaCNAB${queryString}`,
      { token }
    )
  }

  /**
   * 5. Listar Diretorias Disponíveis
   * GET /api/Financeiro/IntegracaoBancaria/CnabOrg/ListarDiretoriasDisponiveis
   * 
   * Lista todas as diretorias disponíveis para configuração CNAB
   */
  async listarDiretoriasDisponiveis(
    token: string
  ): Promise<ListarDiretoriasDisponiveisResponse> {
    return httpClient.get<ListarDiretoriasDisponiveisResponse>(
      '/api/Financeiro/IntegracaoBancaria/CnabOrg/ListarDiretoriasDisponiveis',
      { token }
    )
  }

  /**
   * 6. Buscar Módulos de Remessa
   * GET /api/Financeiro/IntegracaoBancaria/RemessaBancaria/BuscarModulosRemessa
   * 
   * Retorna a lista de módulos/tipos de remessa disponíveis para a organização
   */
  async buscarModulosRemessa(
    token: string
  ): Promise<BuscarModulosRemessaResponse> {
    console.log('[IntegracaoBancariaApi] Buscando módulos de remessa')
    
    const data = await httpClient.get<BuscarModulosRemessaResponse>(
      '/api/Financeiro/IntegracaoBancaria/RemessaBancaria/BuscarModulosRemessa',
      { token }
    )
    
    console.log('[IntegracaoBancariaApi] Módulos retornados:', data.retorno)
    return data
  }
}
