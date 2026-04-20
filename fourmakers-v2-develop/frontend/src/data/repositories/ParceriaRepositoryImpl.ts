import { inject, injectable } from 'tsyringe'
import type { ParceriaRepository } from '@domain/repositories/ParceriaRepository'
import type {
  BuscarParceirosParams,
  BuscarParceirosResponse,
  InserirParceiroPayload,
  AtualizarParceiroPayload,
  ParceiroResponse,
  DeletarParceiroParams,
  DeletarParceiroResponse,
  Parceiro,
  ContatoParceiro,
} from '@domain/entities/Parceiro'
import type {
  InserirContratoPayload,
  AtualizarContratoPayload,
  DeletarContratoParams,
  ContratoResponse,
  DeletarContratoResponse,
  Contrato,
  StatusContrato,
} from '@domain/entities/Contrato'
import type {
  InserirArquivoParams,
  InserirArquivoResponse,
} from '@domain/entities/ArquivoParceiro'
import type { RelatorioParceriaResponse } from '@domain/entities/RelatorioParceria'
import type { ListarUnidadesResponse } from '@domain/entities/NotaFiscalGestao'
import { ParceriaApi } from '@data/api/ParceriaApi'
import { DiTokens } from '@core/di/tokens'
import { API_BASE_URL } from '@shared/constants'
import { formatarDataParaYYYYMMDD } from '@shared/utils/formatUtils'

// Tipos do Backend
interface BackendParceiro {
  id: string
  nomeParceiro: string
  descricaoCurta: string
  descricaoLonga: string
  tipoParceria: string
  avaliacao: number
  urlSite: string | null
  urlLinkedin: string | null
  urlLogo: string | null
  unidade: string | null
  parceirosCategoria: Array<{ categoria: string }>
  parceirosContato: Array<{
    nome: string
    email: string
    telefone: string
  }>
  parceirosGestaoContrato: BackendContrato[]
  dataCadastro?: string
  dataAtualizacao?: string | null
  nomeColaborador?: string | null
  orgId: number
}

interface BackendContrato {
  id: string
  parceiroId: string
  contrato: string | null
  inicioContrato: string | null
  fimContrato: string | null
  clausulaPenalidade: string | null
  numeroPagina: number | null
  valorContrato: number | null
  cotacaoRelacionada: string | null
  plataformaDigital: string | null
  reajusteAnual: string | null
  contratoAssinado: boolean | null
  status: string | null
  renovado: boolean | null
  urlAnexo: string | null
  emailsNotificacao: string[]
}

interface BackendResponse {
  retorno: BackendParceiro[]
  sucesso: boolean
  mensagem?: string
  erros?: string[] | null
}

// Função para converter data DD/MM/YYYY para ISO
const converterDataParaISO = (dataStr: string | null | undefined): string => {
  if (!dataStr || dataStr === '00/00/0000' || dataStr.trim() === '') {
    return ''
  }
  
  try {
    const partes = dataStr.split(' ')
    const dataParte = partes[0].trim()
    const [dia, mes, ano] = dataParte.split('/')
    
    if (dia && mes && ano && dia !== '00' && mes !== '00' && ano !== '0000') {
      // Formato: YYYY-MM-DD (sem timezone)
      const anoPadded = ano.padStart(4, '0')
      const mesPadded = mes.padStart(2, '0')
      const diaPadded = dia.padStart(2, '0')
      const isoString = `${anoPadded}-${mesPadded}-${diaPadded}`
      
      // Verificar se a data é válida
      const data = new Date(`${isoString}T00:00:00.000Z`)
      if (!isNaN(data.getTime())) {
        return isoString
      }
    }
  } catch (error) {
    console.warn('Erro ao converter data:', dataStr, error)
  }
  
  return ''
}

// Função para normalizar status. Retorna null quando a API não informa (não aciona badge "Vencido").
const normalizarStatus = (status: string | null | undefined): StatusContrato | null => {
  if (status == null || (typeof status === 'string' && status.trim() === '')) return null

  const statusLower = status.toLowerCase().trim()

  // Mapear variações de "Em andamento"
  if (
    statusLower.includes('andamento') ||
    statusLower.includes('andand') ||
    statusLower === 'em andamento' ||
    statusLower === 'andamento'
  ) {
    return 'Em andamento'
  }

  // Mapear variações de "Completo"
  if (statusLower.includes('completo') || statusLower === 'completo') {
    return 'Completo'
  }

  // Mapear variações de "Arquivado"
  if (statusLower.includes('arquivado') || statusLower === 'arquivado') {
    return 'Arquivado'
  }

  // Valor desconhecido: manter null para não exibir "Vencido" indevidamente
  return null
}

// Função para converter boolean ou string para boolean
const converterParaBoolean = (valor: boolean | string | null): boolean => {
  if (typeof valor === 'boolean') return valor
  if (typeof valor === 'string') {
    const lower = valor.toLowerCase()
    return lower === 'true' || lower === 'sim' || lower === 'yes'
  }
  return false
}

// Função para normalizar URL do logo
const normalizarLogoUrl = (urlLogo: string | null | undefined): string | null => {
  if (!urlLogo || urlLogo.trim() === '' || urlLogo === 'null' || urlLogo === 'undefined') {
    return null
  }
  
  // Se já é uma URL completa (http/https), retornar como está
  if (urlLogo.startsWith('http://') || urlLogo.startsWith('https://')) {
    return urlLogo
  }
  
  // Se começa com //, adicionar https:
  if (urlLogo.startsWith('//')) {
    return `https:${urlLogo}`
  }
  
  // Se é uma URL relativa, adicionar API_BASE_URL
  if (urlLogo.startsWith('/')) {
    return `${API_BASE_URL}${urlLogo}`
  }
  
  // Se não começa com /, adicionar / e API_BASE_URL
  return `${API_BASE_URL}/${urlLogo}`
}

// Função para mapear tipo de parceria do backend para tipo de empresa do frontend
const mapearTipoEmpresa = (tipoParceria: string): Parceiro['tipoEmpresa'] => {
  const tipoLower = tipoParceria.toLowerCase()
  
  if (tipoLower.includes('aliança') || tipoLower.includes('alianca')) {
    return 'Aliança'
  }
  if (tipoLower.includes('parceria')) {
    return 'Parceria'
  }
  if (tipoLower.includes('cliente')) {
    return 'Cliente'
  }
  if (tipoLower.includes('fornecedor') || tipoLower.includes('prestador')) {
    return 'Fornecedor'
  }
  if (tipoLower.includes('benefício') || tipoLower.includes('beneficio')) {
    return 'Benefício'
  }
  
  // Default
  return 'Fornecedor'
}

// Mapper: Backend Contrato -> Frontend Contrato
const mapearContrato = (backend: BackendContrato): Contrato => {
  // Só mapear contratos que têm pelo menos um campo preenchido
  const temDados = 
    backend.contrato || 
    backend.inicioContrato || 
    backend.fimContrato || 
    backend.status ||
    backend.valorContrato !== null
  
  // Status null/undefined/vazio: deixar undefined para que o contrato NUNCA receba indicador de vencido
  const statusBruto = backend.status
  const statusNormalizado: StatusContrato | null =
    statusBruto != null && String(statusBruto).trim() !== ''
      ? normalizarStatus(String(statusBruto))
      : null

  if (!temDados) {
    // Retornar um contrato mínimo se não houver dados (sem status para não exibir vencido)
    return {
      id: backend.id,
      parceiroId: backend.parceiroId,
      contrato: '',
      dataInicio: new Date().toISOString(),
      dataFim: new Date().toISOString(),
      clausulaPenalidades: '',
      numeroPaginas: 0,
      referenciaCotacao: '',
      valorContrato: 0,
      plataformaAssinatura: '',
      reajusteAnual: '',
      contratoAssinado: false,
      status: null,
      renovado: false,
      emailsNotificacao: backend.emailsNotificacao || [],
    }
  }

  return {
    id: backend.id,
    parceiroId: backend.parceiroId,
    contrato: backend.contrato || '',
    dataInicio: converterDataParaISO(backend.inicioContrato),
    dataFim: converterDataParaISO(backend.fimContrato),
    clausulaPenalidades: backend.clausulaPenalidade || '',
    numeroPaginas: backend.numeroPagina || 0,
    referenciaCotacao: backend.cotacaoRelacionada || '',
    valorContrato: backend.valorContrato || 0,
    plataformaAssinatura: backend.plataformaDigital || '',
    reajusteAnual: backend.reajusteAnual || '',
    contratoAssinado: converterParaBoolean(backend.contratoAssinado),
    status: statusNormalizado,
    renovado: converterParaBoolean(backend.renovado),
    emailsNotificacao: backend.emailsNotificacao || [],
    arquivo: backend.urlAnexo
      ? {
          id: backend.id,
          nomeArquivo: backend.urlAnexo.split('/').pop() || 'arquivo.pdf',
          urlDownload: backend.urlAnexo,
          tipo: 'application/pdf',
          tamanho: 0,
          dataUpload: new Date().toISOString(),
        }
      : undefined,
    urlAnexo: backend.urlAnexo || undefined,
  }
}

// Mapper: Backend Parceiro -> Frontend Parceiro
const mapearParceiro = (backend: BackendParceiro): Parceiro => {
  // Normalizar avaliação de 0-10 para 0-5 (dividir por 2)
  const avaliacaoNormalizada = Math.min(5, Math.max(0, backend.avaliacao / 2))
  
  return {
    id: backend.id,
    nome: backend.nomeParceiro,
    avaliacao: Math.round(avaliacaoNormalizada * 10) / 10, // Arredondar para 1 casa decimal
    logo: normalizarLogoUrl(backend.urlLogo),
    unidade: backend.unidade || '',
    tipoEmpresa: mapearTipoEmpresa(backend.tipoParceria),
    website: backend.urlSite || '',
    linkedIn: backend.urlLinkedin || '',
    descricaoLonga: backend.descricaoLonga || '',
    descricaoCurta: backend.descricaoCurta || '',
    tags: backend.parceirosCategoria?.map((cat) => cat.categoria) || [],
    contatos: backend.parceirosContato?.map(
      (contato): ContatoParceiro => ({
        nome: contato.nome,
        email: contato.email,
        telefone: contato.telefone,
      })
    ) || [],
    contratos: backend.parceirosGestaoContrato?.map(mapearContrato) || [],
    orgId: backend.orgId,
    dataCriacao: backend.dataCadastro,
    dataAtualizacao: backend.dataAtualizacao || undefined,
    nomeColaborador: backend.nomeColaborador || undefined,
  }
}

@injectable()
export class ParceriaRepositoryImpl implements ParceriaRepository {
  constructor(
    @inject(DiTokens.parceriaApi)
    private readonly api: ParceriaApi
  ) {}

  async buscarTodosParceiros(
    token: string,
    params: BuscarParceirosParams
  ): Promise<BuscarParceirosResponse> {
    const response = await this.api.buscarTodosParceiros(token, params)
    
    // Se a resposta já está no formato esperado (com "parceiros"), retornar diretamente
    if (response && typeof response === 'object' && 'parceiros' in response) {
      return response as BuscarParceirosResponse
    }
    
    // Se a resposta está no formato do backend (com "retorno"), mapear
    const backendResponse = response as unknown as BackendResponse
    if (backendResponse && backendResponse.retorno && Array.isArray(backendResponse.retorno)) {
      return {
        parceiros: backendResponse.retorno.map(mapearParceiro),
        proximoCursor: null,
        total: backendResponse.retorno.length,
      }
    }
    
    // Fallback: retornar array vazio
    console.warn('Formato de resposta inesperado do backend:', response)
    return {
      parceiros: [],
      proximoCursor: null,
      total: 0,
    }
  }

  async inserirParceiro(
    token: string,
    payload: InserirParceiroPayload
  ): Promise<ParceiroResponse> {
    // Mapear payload frontend -> backend esperado pelo endpoint
    const backendPayload: any = {
      orgId: payload.orgId,
      unidade: payload.unidade || '',
      nomeParceiro: payload.nome,
      tipoParceria: payload.tipoEmpresa,
      urlSite: payload.website || null,
      urlLinkedin: payload.linkedIn || null,
      descricaoLonga: payload.descricaoLonga || null,
      descricaoCurta: payload.descricaoCurta || null,
      // Backend aceita array de strings para categorias ao inserir
      parceirosCategoria: Array.isArray(payload.tags) ? payload.tags : [],
      // Mapear contatos para o formato do backend
      parceirosContato: Array.isArray(payload.contatos)
        ? payload.contatos.map((c) => ({
            nome: c.nome || '',
            telefone: c.telefone || '',
            email: c.email || '',
          }))
        : [],
      // Avaliação no backend é 0-10, frontend usa 0-5, então multiplicar por 2 ao enviar
      avaliacao: (payload.avaliacao || 0) * 2,
      // codigoInternoColaborador pode ser preenchido pelo frontend se disponível
      codigoInternoColaborador: (payload as any).codigoInternoColaborador || undefined,
    }

    const response = await this.api.inserirParceiro(token, backendPayload)
    // Normalizar resposta do backend: { retorno: { id: ... } } -> { dados: { id: ... } }
    if (response && typeof response === 'object' && 'retorno' in response) {
      const backendResponse = response as any
      return {
        sucesso: backendResponse.sucesso || false,
        mensagem: backendResponse.mensagem,
        dados: backendResponse.retorno ? { id: backendResponse.retorno.id, ...backendResponse.retorno } : undefined,
        erros: backendResponse.erros,
      } as any
    }
    return response
  }

  async atualizarParceiro(
    token: string,
    payload: AtualizarParceiroPayload
  ): Promise<ParceiroResponse> {
    const backendPayload: any = {
      id: payload.id,
      parceiroId: payload.id,
      orgId: payload.orgId,
      unidade: payload.unidade || '',
      nomeParceiro: payload.nome,
      tipoParceria: payload.tipoEmpresa,
      urlSite: payload.website || null,
      urlLinkedin: payload.linkedIn || null,
      descricaoLonga: payload.descricaoLonga || null,
      descricaoCurta: payload.descricaoCurta || null,
      parceirosCategoria: Array.isArray(payload.tags) ? payload.tags : [],
      parceirosContato: Array.isArray(payload.contatos)
        ? payload.contatos.map((c) => ({
            nome: c.nome || '',
            telefone: c.telefone || '',
            email: c.email || '',
          }))
        : [],
      // Avaliação no backend é 0-10, frontend usa 0-5, então multiplicar por 2 ao enviar
      avaliacao: (payload.avaliacao || 0) * 2,
      codigoInternoColaborador: (payload as any).codigoInternoColaborador || undefined,
      urlLogo: (payload as any).urlLogo || (payload as any).logo || undefined,
    }

    return this.api.atualizarParceiro(token, backendPayload)
  }

  async deletarParceiro(
    token: string,
    params: DeletarParceiroParams
  ): Promise<DeletarParceiroResponse> {
    return this.api.deletarParceiro(token, params)
  }

  async inserirContrato(
    token: string,
    payload: InserirContratoPayload
  ): Promise<ContratoResponse> {
    // Mapear payload do frontend para formato do backend
    // Backend espera: inicioContrato, fimContrato (formato YYYY-MM-DD como string)
    // Frontend envia: dataInicio, dataFim
    const { dataInicio, dataFim, codigoInternoColaborador, ...restPayload } = payload
    
    // Garantir que as datas estão no formato YYYY-MM-DD (string)
    // Mesmo que já estejam formatadas no modal, garantir aqui também
    const inicioContrato = formatarDataParaYYYYMMDD(dataInicio)
    const fimContrato = formatarDataParaYYYYMMDD(dataFim)
    
    const backendPayload = {
      ...restPayload,
      status: payload.status,
      inicioContrato,
      fimContrato,
      // Incluir código interno do colaborador se presente
      ...(codigoInternoColaborador ? { codigoInternoColaborador } : {}),
    } as any

    const response = await this.api.inserirContrato(token, backendPayload)
    // Normalizar formatos de resposta: backend pode retornar { retorno: {...}, sucesso: true }
    if (response && typeof response === 'object' && 'retorno' in response) {
      const backendResponse = response as any
      return {
        sucesso: backendResponse.sucesso || false,
        mensagem: backendResponse.mensagem,
        dados: backendResponse.retorno ? { id: backendResponse.retorno.id, ...backendResponse.retorno } : undefined,
        erros: backendResponse.erros,
      } as any
    }
    return response
  }

  async atualizarContrato(
    token: string,
    payload: AtualizarContratoPayload
  ): Promise<ContratoResponse> {
    // Mapear payload do frontend para formato do backend
    // Backend espera: inicioContrato, fimContrato (formato YYYY-MM-DD como string)
    // Frontend envia: dataInicio, dataFim
    const { dataInicio, dataFim, codigoInternoColaborador, ...restPayload } = payload
    
    // Garantir que as datas estão no formato YYYY-MM-DD (string)
    // Mesmo que já estejam formatadas no modal, garantir aqui também
    const inicioContrato = formatarDataParaYYYYMMDD(dataInicio)
    const fimContrato = formatarDataParaYYYYMMDD(dataFim)
    
    const backendPayload = {
      ...restPayload,
      status: payload.status,
      inicioContrato,
      fimContrato,
      ...(codigoInternoColaborador ? { codigoInternoColaborador } : {}),
    } as any

    const response = await this.api.atualizarContrato(token, backendPayload)
    if (response && typeof response === 'object' && 'retorno' in response) {
      const backendResponse = response as any
      return {
        sucesso: backendResponse.sucesso || false,
        mensagem: backendResponse.mensagem,
        dados: backendResponse.retorno ? { id: backendResponse.retorno.id, ...backendResponse.retorno } : undefined,
        erros: backendResponse.erros,
      } as any
    }
    return response
  }

  async deletarContrato(
    token: string,
    params: DeletarContratoParams
  ): Promise<DeletarContratoResponse> {
    return this.api.deletarContrato(token, params)
  }

  async inserirArquivo(
    token: string,
    params: InserirArquivoParams
  ): Promise<InserirArquivoResponse> {
    const response = await this.api.inserirArquivo(token, params)
    
    // Mapear resposta do backend para formato esperado pelo frontend
    // Backend retorna: { retorno: { link: "..." }, sucesso: true }
    // Frontend espera: { dados: { urlDownload: "..." }, sucesso: true }
    if (response && typeof response === 'object' && 'retorno' in response) {
      const backendResponse = response as any
      const retorno = backendResponse.retorno
      
      // Extrair nome do arquivo do link se não estiver disponível
      let nomeArquivo = retorno?.nomeArquivo || ''
      if (!nomeArquivo && retorno?.link) {
        const urlParts = retorno.link.split('/')
        nomeArquivo = urlParts[urlParts.length - 1] || params.file.name || 'arquivo.pdf'
      }
      
      return {
        sucesso: backendResponse.sucesso || false,
        mensagem: backendResponse.mensagem,
        dados: retorno
          ? {
              id: retorno.id || '',
              nomeArquivo,
              urlDownload: retorno.link || '',
            }
          : undefined,
        erros: backendResponse.erros,
      }
    }
    
    return response
  }

  async gerarRelatorioParceria(
    token: string
  ): Promise<RelatorioParceriaResponse> {
    return this.api.gerarRelatorioParceria(token)
  }

  async listarUnidades(
    token: string
  ): Promise<ListarUnidadesResponse> {
    const response = await this.api.listarUnidades(token)
    // Normalizar resposta do backend: { retorno: [{ id, descricao }] } -> { retorno: [...] }
    if (response && typeof response === 'object' && 'retorno' in response && Array.isArray(response.retorno)) {
      return {
        ...response,
        retorno: response.retorno.map((u: any) => ({
          id: u.id || '',
          descricao: u.descricao || u.id || '',
        })),
      }
    }
    return response
  }
}
