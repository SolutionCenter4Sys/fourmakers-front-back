import type { ColaboradoresResponse, ColaboradoresParams } from '@domain/entities/Colaborador'

export interface OrigemColaboradorItem {
  id: string
  descricao: string
  ativo: boolean
}

export interface ListarOrigensColaboradorResponse {
  retorno?: OrigemColaboradorItem[]
  sucesso?: boolean
  mensagem?: string | null
  erros?: unknown
}
import type { ColaboradoresCchResponse, ListarColaboradoresOrgParams } from '@domain/entities/ColaboradorCch'
import type { DepartamentosResponse, ListarDepartamentosParams } from '@domain/entities/Departamento'
import type { DiretoriasResponse } from '@domain/entities/Diretoria'
import type { EmpresasRelacionadasResponse, ListarEmpresasRelacionadasParams } from '@domain/entities/EmpresaRelacionada'
import type { ModelosContratacaoResponse } from '@domain/entities/ModeloContratacao'
import type { CargosResponse } from '@domain/entities/Cargo'
import { httpClient } from './httpClient'
import type { AniversariantesSemanaResponse, AniversariantesSemanaParams } from '@domain/entities/Aniversariante'
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360'
import type { ListarEscolaridadeColaboradorResponse, ListarEscolaridadeColaboradorParams } from '@domain/entities/Escolaridade'
import type {
  ObterDadosColaboradorResponse,
  EditarDadosColaboradorPayload,
  EditarDadosColaboradorResponse,
} from '@domain/entities/ColaboradorDadosPessoais'
import type { AlterarDadosColaboradorParametrosPayload } from '@domain/entities/AlterarDadosColaboradorParametros'
import type { AlterarFormularioColaboradorPayload } from '@domain/repositories/ColaboradoresRepository'
import { API_BASE_URL } from '@shared/constants'
import { processarUrlComToken } from '@shared/utils/urlUtils'

export type { AlterarDadosColaboradorParametrosPayload }

export class ColaboradoresApi {
  async getColaboradores(token: string, params: ColaboradoresParams): Promise<ColaboradoresResponse> {
    const queryParams = new URLSearchParams({
      cursor: params.cursor.toString(),
      limite: params.limite.toString(),
      nomeOuEmail: params.nomeOuEmail,
      org: params.org.toString(),
    })

    if (params.codigoCliente != null && params.codigoCliente.trim() !== '') {
      queryParams.append('codigoCliente', params.codigoCliente.trim())
    }
    if (params.codExterno) {
      queryParams.append('codExterno', params.codExterno)
    }
    if (params.fourtalents !== undefined) {
      queryParams.append('fourtalents', params.fourtalents.toString())
    }

    return httpClient.get<ColaboradoresResponse>(
      `/api/Colaborador/ListaColaboradoresOrgId?${queryParams.toString()}`,
      { token }
    )
  }

  async listarColaboradoresOrg(token: string, params: ListarColaboradoresOrgParams): Promise<ColaboradoresCchResponse> {
    const queryParams = new URLSearchParams({
      busca: params.busca,
      cursor: params.cursor.toString(),
      limite: params.limite.toString(),
    })

    return httpClient.get<ColaboradoresCchResponse>(
      `/api/MapaDeAlocacao/ListarColaboradoresOrg?${queryParams.toString()}`,
      { token }
    )
  }

  async listarDepartamentos(token: string, params: ListarDepartamentosParams): Promise<DepartamentosResponse> {
    const queryParams = new URLSearchParams({
      codigoDiretoria: params.codigoDiretoria || '',
    })

    return httpClient.get<DepartamentosResponse>(
      `/api/Colaborador/Departamento/ListarDepartamentosDosColaboradores?${queryParams.toString()}`,
      { token }
    )
  }

  async listarDiretorias(token: string): Promise<DiretoriasResponse> {
    return httpClient.get<DiretoriasResponse>(
      '/api/Colaborador/Diretoria/ListarDiretoriaDosColaboradores',
      { token }
    )
  }

  async listarEmpresasRelacionadas(token: string, params: ListarEmpresasRelacionadasParams): Promise<EmpresasRelacionadasResponse> {
    const queryParams = new URLSearchParams({
      nomeEmpresa: params.nomeEmpresa,
      limite: params.limite.toString(),
      cursor: params.cursor.toString(),
    })

    return httpClient.get<EmpresasRelacionadasResponse>(
      `/api/Colaborador/Sugestao/ListarEmpresasRelacionadas?${queryParams.toString()}`,
      { token }
    )
  }

  async listarModelosContratacao(token: string): Promise<ModelosContratacaoResponse> {
    return httpClient.get<ModelosContratacaoResponse>(
      '/api/Colaborador/ListarModelosDeContratacaoPorOrg',
      { token }
    )
  }

  async listarCargos(token: string): Promise<CargosResponse> {
    return httpClient.get<CargosResponse>(
      '/api/Colaborador/ListarCargos',
      { token }
    )
  }

  async listarOrigensColaborador(token: string): Promise<ListarOrigensColaboradorResponse> {
    return httpClient.get<ListarOrigensColaboradorResponse>(
      '/api/Colaborador/ListarOrigensColaborador',
      { token }
    )
  }

  async inserirColaborador(token: string, payload: {
    codColaborador?: string
    nomeColaborador?: string
    dataAdmissao?: string
    documentoColaborador?: string
    email?: string
    codDiretoria?: string
    diretoria?: string
    codDepartamento?: string
    departamento?: string
    codGestor?: string
    contatoPrincipalDDI?: string
    contatoPrincipal?: string
    modeloContratacao?: string
    empresaRelacionada?: string
    modeloTrabalho?: string
    diasPorSemana?: number | null
    valorHora?: number
    custoHora?: number
    baseHoraMes?: number | null
    considerarBancoDeTalentos?: boolean
    cargo?: string
    codCargo?: string
  }): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    // Retornar a resposta mesmo em caso de erro HTTP para que o componente possa tratar
    // A API pode retornar sucesso: false com mensagem e erros mesmo com status HTTP de erro
    return httpClient.post<{ sucesso: boolean; mensagem?: string; erros?: string[] }>(
      '/api/Colaborador/InserirColaborador',
      payload,
      { token }
    )
  }

  async editarColaborador(token: string, payload: {
    codColaborador: string
    nomeColaborador?: string
    cpf?: string
    dataAdmissao?: string
    documentoColaborador?: string
    email?: string
    codDiretoria?: string
    diretoria?: string
    codDepartamento?: string
    departamento?: string
    codGestor?: string
    ativo?: boolean
    dataInativacao?: string
    contatoPrincipal?: string
    contatoPrincipalDDI?: string
    baseHoraMes?: number | null
    custoHora?: number
    valorHora?: number
    diasPorSemana?: number | null
    modeloTrabalho?: string
    modeloContratacao?: string
    empresaRelacionada?: string
    considerarBancoDeTalentos?: boolean
    cargo?: string
    codigoCargo?: string
  }): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }> {
    return httpClient.post<{ sucesso: boolean; mensagem?: string; erros?: string[] }>(
      '/api/Colaborador/EditarColaborador',
      payload,
      { token }
    )
  }

  async aniversariantesSemana(token: string, params?: AniversariantesSemanaParams): Promise<AniversariantesSemanaResponse> {
    const queryParams = new URLSearchParams()
    
    if (params?.codDiretoria) {
      queryParams.append('codDiretoria', params.codDiretoria)
    }

    const queryString = queryParams.toString() ? `?${queryParams.toString()}` : ''
    
    return httpClient.get<AniversariantesSemanaResponse>(
      `/api/Colaborador/AniversariantesSemana${queryString}`,
      { token }
    )
  }

  async buscarDadosColaborador(token: string, cpf: string): Promise<BuscarDadosColaboradorResponse> {
    return httpClient.get<BuscarDadosColaboradorResponse>(
      `/api/Colaborador/BuscarDadosColaborador?cpf=${cpf}`,
      { token }
    )
  }

  async listarEscolaridadeColaborador(token: string, params: ListarEscolaridadeColaboradorParams): Promise<ListarEscolaridadeColaboradorResponse> {
    const queryParams = new URLSearchParams({
      busca: params.busca,
      cursor: params.cursor.toString(),
      limite: params.limite.toString(),
    })

    return httpClient.get<ListarEscolaridadeColaboradorResponse>(
      `/api/EscolaridadeColaborador/ListarEscolaridadeColaborador?${queryParams.toString()}`,
      { token }
    )
  }

  async obterDadosColaborador(token: string, codigoInternoColaborador: string): Promise<ObterDadosColaboradorResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('codigoInternoColaborador', codigoInternoColaborador)
    queryParams.append('token', token)

    return httpClient.get<ObterDadosColaboradorResponse>(
      `/api/Colaborador/ObterDadosColaborador?${queryParams.toString()}`,
      { token },
    )
  }

  async editarDadosColaborador(
    token: string,
    payload: EditarDadosColaboradorPayload,
  ): Promise<EditarDadosColaboradorResponse> {
    return httpClient.post<EditarDadosColaboradorResponse>(
      '/api/Colaborador/EditarDadosColaborador',
      payload,
      { token },
    )
  }

  async alterarFormularioColaborador(
    token: string,
    payload: AlterarFormularioColaboradorPayload,
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return httpClient.post<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }>(
      '/api/Colaborador/AlterarFormularioColaborador',
      payload,
      { token },
    )
  }

  async alterarDadosColaboradorParametros(
    token: string,
    payload: AlterarDadosColaboradorParametrosPayload,
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }> {
    return httpClient.post<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }>(
      '/api/Colaborador/AlterarDadosColaboradorParametros',
      payload,
      { token },
    )
  }

  async gerarRelatorioColaboradores(
    token: string
  ): Promise<{ arrayBuffer: ArrayBuffer; contentType: string; fileName: string }> {
    const response = await httpClient.getBlob(
      `${API_BASE_URL}/api/Usuario/RelatorioColaboradores`,
      {
        token,
        headers: {
          Accept: '*/*',
          'Cache-Control': 'no-cache',
          Pragma: 'no-cache',
        },
      },
    )

    // Detectar Content-Type da resposta
    const contentType = response.headers.get('Content-Type') || 'application/octet-stream'
    
    // Obter nome do arquivo do header Content-Disposition
    const contentDisposition = response.headers.get('Content-Disposition') || ''
    let fileName = 'relatorio_colaboradores.xlsx' // Nome padrão
    
    if (contentDisposition) {
      // Tentar primeiro o formato filename*=UTF-8'' (RFC 5987) - mais confiável
      const filenameStarMatch = contentDisposition.match(/filename\*=UTF-8''([^;]+?)(?:;|$)/i)
      if (filenameStarMatch && filenameStarMatch[1]) {
        try {
          fileName = decodeURIComponent(filenameStarMatch[1].trim())
        } catch {
          // Se falhar o decode, usar o valor direto
          fileName = filenameStarMatch[1].trim()
        }
      } else {
        // Fallback para o formato filename= simples
        const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]+?)(?:;|$)/i)
        if (filenameMatch && filenameMatch[1]) {
          fileName = filenameMatch[1].replace(/['"]/g, '').trim()
        }
      }
    }

    // Retornar ArrayBuffer junto com metadados
    const arrayBuffer = await response.arrayBuffer()
    return {
      arrayBuffer,
      contentType,
      fileName,
    }
  }

  async baixarCertificadoColaborador(
    token: string,
    path: string,
  ): Promise<{ arrayBuffer: ArrayBuffer; contentType: string; fileName: string }> {
    const processedUrl = processarUrlComToken(path, token)

    if (!processedUrl) {
      throw new Error('Não foi possível processar a URL do certificado.')
    }

    const response = await httpClient.getBlob(processedUrl, {
      token,
      headers: {
        Accept: '*/*',
      },
    })

    const contentType = response.headers.get('Content-Type') || 'application/octet-stream'
    const contentDisposition = response.headers.get('Content-Disposition') || ''
    let fileName = path.split('/').pop() || 'certificado.pdf'

    if (contentDisposition) {
      const filenameStarMatch = contentDisposition.match(/filename\*=UTF-8''([^;]+?)(?:;|$)/i)
      if (filenameStarMatch && filenameStarMatch[1]) {
        try {
          fileName = decodeURIComponent(filenameStarMatch[1].trim())
        } catch {
          fileName = filenameStarMatch[1].trim()
        }
      } else {
        const filenameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]+?)(?:;|$)/i)
        if (filenameMatch && filenameMatch[1]) {
          fileName = filenameMatch[1].replace(/['"]/g, '').trim()
        }
      }
    }

    const arrayBuffer = await response.arrayBuffer()
    return {
      arrayBuffer,
      contentType,
      fileName,
    }
  }
}
