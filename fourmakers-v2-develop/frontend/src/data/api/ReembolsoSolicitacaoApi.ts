import { httpClient } from './httpClient'

// Interfaces para os tipos de dados
export interface ProjetoColaborador {
  codigoProjeto: string
  codigoCliente: string
  projetos: string
  cliente: string
}

export interface Verba {
  verbaId?: number
  id?: number
  categoria: string
  unidade: string | null
  valor: number
  tipoCodigo: number
  exigirComprovante: boolean
}

export interface ParametroReembolso {
  validadeComprovanteDias: number
}

export interface VerbaEParametros {
  verbas: Verba[]
  parametroReembolso: ParametroReembolso
}

export interface ArquivoAnalise {
  base64: string
  tipo: number
}

export interface AnaliseComprovante {
  data: string
  empresa: string
  endereco: string
  itens: unknown[]
}

export interface ResultadoAnaliseComprovante {
  analises: AnaliseComprovante[]
  data: string | null
  valor: number | null
  quantidade: number | null
}

export interface ArquivoSolicitacao {
  base64: string
  tipo: number
  height?: number | null
  width?: number | null
  blurHash?: string | null
}

export interface SolicitacaoReembolso {
  projetoId: string
  clienteId: string
  verbaId: number
  descricao: string
  dataDespesa: string
  valor: number
  valorUnidade?: number
  quantidade?: number
  unidade?: string
  uuid: string
  tipoCusto: number
  categoria: string
  arquivos: ArquivoSolicitacao[]
}

export interface PayloadInserirSolicitacao {
  objetivo: string
  destino: string
  dataInicio: string
  dataFim: string
  solicitacoes: SolicitacaoReembolso[]
}

export class ReembolsoSolicitacaoApi {
  /**
   * Lista projetos do colaborador para combo cliente/projeto
   */
  async listarProjetosColaborador(
    token: string,
    codigoProfissional: string
  ): Promise<{ sucesso: boolean; retorno?: ProjetoColaborador[]; mensagem?: string }> {
    try {
      const data = await httpClient.post<{ sucesso?: boolean; Projetos?: ProjetoColaborador[] }>(
        '/api/MapaDeAlocacao/ListarProjetosColaborador',
        {
          codigoProfissional,
          ehTbd: false,
          codigoGerenteProjeto: '',
          listaCodigoCliente: [],
          status: '',
          prioritarioFiltro: 0,
        },
        { token }
      )
      return { sucesso: data?.sucesso !== false, retorno: data?.Projetos || [] }
    } catch {
      return { sucesso: false, mensagem: 'Erro ao carregar projetos' }
    }
  }

  /**
   * Lista verbas com exceção para combo categoria
   */
  async listarVerbasComExcecao(
    token: string,
    codigoProjeto: string,
    codigoCliente: string
  ): Promise<{ sucesso: boolean; retorno?: Verba[]; mensagem?: string }> {
    const queryParams = new URLSearchParams({
      codigoProjeto,
      codigoCliente,
    })

    try {
      const data = await httpClient.get<{ retorno?: Verba[] }>(
        `/api/Financeiro/Reembolso/Verba/ListarVerbasComExecao?${queryParams.toString()}`,
        { token }
      )
      return { sucesso: true, retorno: data?.retorno || [] }
    } catch {
      return { sucesso: false, mensagem: 'Erro ao carregar categorias' }
    }
  }

  /**
   * Obtém verbas e parâmetros de validação (validadeComprovanteDias)
   */
  async obterVerbasEParametroValidacao(
    token: string
  ): Promise<{ sucesso: boolean; retorno?: VerbaEParametros; mensagem?: string }> {
    try {
      const data = await httpClient.get<{ retorno?: VerbaEParametros }>(
        '/api/Financeiro/Reembolso/Verba/ObterVerbasEParametroValidacao',
        { token }
      )
      return { sucesso: true, retorno: data?.retorno }
    } catch {
      return { sucesso: false, mensagem: 'Erro ao carregar parâmetros' }
    }
  }

  /**
   * Analisa comprovantes fiscais (OCR) para extrair data e valor
   */
  async analisarComprovantesFiscais(
    token: string,
    arquivos: ArquivoAnalise[]
  ): Promise<{ sucesso: boolean; retorno?: ResultadoAnaliseComprovante; mensagem?: string }> {
    try {
      const data = await httpClient.post<{ retorno?: ResultadoAnaliseComprovante }>(
        '/api/Financeiro/Reembolso/Solicitacao/AnalisarComprovantesFiscais',
        arquivos,
        { token }
      )
      return { sucesso: true, retorno: data?.retorno }
    } catch {
      return { sucesso: false, mensagem: 'Erro ao analisar comprovantes' }
    }
  }

  /**
   * Envia solicitações de reembolso (JSON com base64)
   */
  async inserirSolicitacoes(
    token: string,
    payload: PayloadInserirSolicitacao
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return httpClient.post<{ sucesso: boolean; mensagem?: string; erros?: string[] }>(
      '/api/Financeiro/Reembolso/Solicitacao/Inserir',
      payload,
      { token }
    )
  }

  /**
   * Envia solicitações de reembolso via ZIP (multipart: dados.json + archive_N no ZIP).
   */
  async inserirSolicitacoesZip(
    token: string,
    arquivoZip: File
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    const formData = new FormData()
    formData.append('arquivoZip', arquivoZip)
    return httpClient.post<{ sucesso: boolean; mensagem?: string; erros?: string[] }>(
      '/api/Financeiro/Reembolso/Solicitacao/Inserir',
      formData,
      { token }
    )
  }
}

