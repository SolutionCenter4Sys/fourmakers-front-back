import { API_BASE_URL } from '@shared/constants'
import type {
  TimesheetStatMock,
  ProjetoApprovalMock,
  ColaboradorManagementMock,
  WeekDataMock,
  DayDetailMock,
} from '../mocks/timesheetComponentesMock'
import {
  timesheetStatsMock,
  approvalsStatsMock,
  projetosApprovalMock,
  managementStatsMock,
  colaboradoresManagementMock,
  weekDataMock,
  dayDetailsMock,
} from '../mocks/timesheetComponentesMock'
import { httpClient } from './httpClient'

export interface MesVigencia {
  mes: number
  ano: number
  label: string
}

export interface ListarVigenciasColaboradorResponse {
  mesVigente: MesVigencia
  meses: MesVigencia[]
}

export interface Atividade {
  id: string
  descricao: string
}

export interface ProjetoComAtividades {
  id: string
  nome: string
  codigoCliente: string
  nomeCliente: string
  atividades: Atividade[]
}

export interface ListarProjetosComAtividadesResponse {
  projetos_atividades: ProjetoComAtividades[]
}

export interface DiaTemplate {
  data: string
  numeroSemanaDia: number
  label: string
  numeroSemana: number
  mesAtivo: boolean
  feriado: boolean
}

export interface SemanaTemplate {
  numeroSemana: number
  primeiroDiaSemana: number
  ultimoDiaSemana: number
  dias: DiaTemplate[]
}

export interface ListarTemplateSemanaVigenciaResponse {
  semanas: SemanaTemplate[]
}

export interface Aprovador {
  cpf: string
  nome: string
}

export interface ProjetoApontamento {
  id: string
  nomeProjeto: string
  gerente: string
  codCliente: string
  nomeCliente: string
  permiteApontamentoSemAlocacao?: boolean
  permiteApontamentoSemAlocacaoParaOutroColaborador?: boolean
  dataFim?: string | null
}

export interface AtividadeApontamento {
  id: string
  descricao: string
}

export interface ColaboradorApontamento {
  id: string
  horas: number
  justificativa: string | null
  nomeUsuarioJustificativa: string | null
  dataUsuarioJustificativa: string | null
  data_registro: string
  codStatusApontamentoGrupo: string
  numeroSemana: number
  numeroSemanaDia: number
  projeto: ProjetoApontamento
  atividade: AtividadeApontamento
  isWeekend: boolean
  observacao: string
}

export interface ApontamentoMensal {
  projeto: ProjetoApontamento
  horas: number
  apontamentoReprovado: boolean
  codStatusGrupoMensal: number
  prioridadeStatusApontamentoGrupo: number
  aprovadores: Aprovador[]
  observacao: string | null
  atividade: string
}

export interface VigenciaApontamento {
  id: string | null
  mes: number
  ano: number
  horas_trabalhadas: number
  label: string
}

export interface TotalizadorApontamentosMensais {
  quantidadeTotalColaboradores: number
  quantidadeTotalProjetos: number
  somaHoras: number
}

export interface TotalizadorApontamentosBigNumbers {
  quantidadeTotalColaboradores: number
  somaHorasLancadas: number
  quantidadeTotalNaoApontado: string
  somaHorasAprovadas: number
  somaHorasPendentes: number
  somaHorasReprovdas: number
}

export interface ApontamentosPorVigencia {
  vigencia: VigenciaApontamento
  apontamentosMensais: ApontamentoMensal[]
  totalizadorApontamentosMensais: TotalizadorApontamentosMensais
  colaboradorApontamentos: ColaboradorApontamento[]
  totalizadorApontamentosBigNumbers: TotalizadorApontamentosBigNumbers
  pdfFolhaPontoUrl: string | null
  pdfHoleriteUrl: string | null
  dataColetaDeDados: string
  ocultaTimeSheet: boolean
}

export interface ListarApontamentosPorVigenciaResponse {
  apontamentos_por_vigencia: ApontamentosPorVigencia
  sucesso: boolean
  mensagem: string
  erros: string[] | null
}

export interface PeriodoFechado {
  id: number
  dataFim: string
}

export interface GestorApontamento {
  codigoGerente: string
  nomeCompleto: string
  codigoDiretoria: string
  diretoria: string
}

export interface ListarGestoresApontamentoResponse {
  retorno: GestorApontamento[]
}

export interface ProjetoGerenteDeProjeto {
  codProjeto: string
  nomeProjeto: string
}

export interface GestorProjeto {
  codGerente: string
  nomeGerente: string
  labelGerenteProjeto: string
}

export interface ListarGestoresProjetoResponse {
  retorno: GestorProjeto[]
}

export interface StatusApontamento {
  id: string
  codStatusGrupo: number
  descricao: string
}

/** Item de mensagem retornado pela API ao aprovar projetos em lote */
export interface AprovarProjetoVigenciaMensagem {
  codProjeto: string
  cpf: string
  projetoAprovado: boolean
  mensagem: string
}

export interface AprovarProjetoVigenciaEmLoteResponse {
  sucesso: boolean
  mensagem: string | null
  erros: string[] | null
  retorno?: {
    quantidadeProjetosAprovados: number
    quantidadeProjetosPulados: number
    mensagens: AprovarProjetoVigenciaMensagem[]
  }
}

export interface AprovadorColaborador {
  nomeAprovador: string
  codigoInternoAprovador: string
}

export interface ColaboradorEApontamento {
  colaboradorNome: string
  colaboradorCPF: string
  ativo: boolean
  dataAdmissao: string
  dataInativacao: string | null
  nomeCompletoGerente: string
  codigoGerente: string
  codigoStatusApontamentoPeriodo: number
  descricaoStatusApontamento: string
  somaHoras: number
  observacao: string | null
  projeto: string
  aprovadores: AprovadorColaborador[]
}

export interface TotalizadorBigNumbers {
  quantidadeTotalColaboradores: number
  somaHorasLancadas: number
  quantidadeTotalNaoApontado: string
  somaHorasAprovadas: number
  somaHorasPendentes: number
  somaHorasReprovdas: number
}

export interface TotalizadorProjetosVisaoGerente {
  quantidadeTotalColaboradores: number
  quantidadeTotalProjetos: number
  somaHoras: number
}

export interface ListarVigenciasApontamentosGerenteDeProjetoResponse {
  mesVigente: {
    mes: number
    ano: number
    label: string
  }
  meses: Array<{
    mes: number
    ano: number
    label: string
  }>
}

export interface ListarColaboradoresEApontamentosPorGestorResponse {
  retorno: {
    lista: ColaboradorEApontamento[]
    total?: number // Campo opcional caso a API retorne o total
    totalizadorBigNumbers?: TotalizadorBigNumbers
  }
}

export class TimesheetComponentesApi {
  async getTimesheetStats(): Promise<TimesheetStatMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...timesheetStatsMock]
  }

  async getApprovalsStats(): Promise<TimesheetStatMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...approvalsStatsMock]
  }

  async getProjetosApproval(): Promise<ProjetoApprovalMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...projetosApprovalMock]
  }

  async getManagementStats(): Promise<TimesheetStatMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...managementStatsMock]
  }

  async getColaboradoresManagement(): Promise<ColaboradorManagementMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 300))
    return [...colaboradoresManagementMock]
  }

  async getWeekData(): Promise<WeekDataMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...weekDataMock]
  }

  async getDayDetails(): Promise<DayDetailMock[]> {
    await new Promise((resolve) => setTimeout(resolve, 200))
    return [...dayDetailsMock]
  }

  async listarVigenciasColaborador(
    token: string,
    cpfColaborador: string = ''
  ): Promise<ListarVigenciasColaboradorResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('cpfColaborador', cpfColaborador)

    return httpClient.get<ListarVigenciasColaboradorResponse>(
      `/api/Apontamento/ListarVigenciasColaborador?${queryParams.toString()}`,
      { token }
    )
  }

  async listarProjetosComAtividades(
    token: string,
    cpfColaborador: string = ''
  ): Promise<ListarProjetosComAtividadesResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('cpfColaborador', cpfColaborador)

    return httpClient.get<ListarProjetosComAtividadesResponse>(
      `/api/Apontamento/ListarProjetosComAtividades?${queryParams.toString()}`,
      { token }
    )
  }

  async listarTemplateSemanaVigencia(
    token: string,
    mes: number,
    ano: number
  ): Promise<ListarTemplateSemanaVigenciaResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('mes', mes.toString())
    queryParams.append('ano', ano.toString())

    return httpClient.get<ListarTemplateSemanaVigenciaResponse>(
      `/api/Apontamento/ListarTemplateSemanaVigencia?${queryParams.toString()}`,
      { token }
    )
  }

  async listarApontamentosPorVigencia(
    token: string,
    mes: number,
    ano: number,
    cpfColaborador: string = ''
  ): Promise<ListarApontamentosPorVigenciaResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('cpfColaborador', cpfColaborador)

    return httpClient.get<ListarApontamentosPorVigenciaResponse>(
      `/api/Apontamento/ListarApontamentosPorVigencia/${mes}/${ano}?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': 'pt-BR',
        },
      }
    )
  }

  async apontarHorasEmLote(
    token: string,
    payload: {
      projetoId: string
      atividadeId: string
      cpfColaborador: string
      horas: number
      diaQuebraSemana: number
      deveSomarApontamentoDia: boolean
      dataInicio: string
      dataFim: string
      incluirSabado: boolean
      incluirDomingo: boolean
      incluirFeriado: boolean
      observacao: string
    },
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    return httpClient.post<{ sucesso: boolean; mensagem: string; erros: string[] | null }>(
      '/api/Apontamento/ApontarHorasEmLote',
      payload,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async editarApontamento(
    token: string,
    payload: {
      projetoId: string
      atividadeId: string
      horas: number
      dataRegistro: string
      apontamentoId: string
      cpfColaborador: string
      observacao: string
      dataColetaDeDados: string
    },
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    return httpClient.post<{ sucesso: boolean; mensagem: string; erros: string[] | null }>(
      '/api/Apontamento/EditarApontamento',
      payload,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async deletarApontamentoColaborador(
    token: string,
    colaboradorApontamentoId: string,
    dataColetaDeDados: string,
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    const queryParams = new URLSearchParams()
    queryParams.append('colaboradorApontamentoId', colaboradorApontamentoId)
    queryParams.append('dataColetaDeDados', dataColetaDeDados)

    return httpClient.delete<{ sucesso: boolean; mensagem: string; erros: string[] | null }>(
      `/api/Apontamento/DeletarApontamentoColaborador?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async buscarPeriodoFechado(
    token: string,
    language: string = 'pt-BR'
  ): Promise<PeriodoFechado | null> {
    return httpClient.get<PeriodoFechado | null>(
      '/api/Apontamento/PeriodoFechado/BuscaPeriodoFechado',
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarGestoresApontamento(
    token: string,
    language: string = 'pt-BR'
  ): Promise<ListarGestoresApontamentoResponse> {
    return httpClient.get<ListarGestoresApontamentoResponse>(
      '/api/Apontamento/ListarGestoresApontamento',
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarProjetosGerenteDeProjeto(
    token: string,
    language: string = 'pt-BR'
  ): Promise<ProjetoGerenteDeProjeto[]> {
    return httpClient.get<ProjetoGerenteDeProjeto[]>(
      '/api/Apontamento/ListarProjetosGerenteDeProjeto',
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarGestoresProjeto(
    token: string,
    codigoDiretoria: string = '',
    codigoDepartamento: string = '',
    codigoGestorAdm: string = '',
    language: string = 'pt-BR'
  ): Promise<ListarGestoresProjetoResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('codigoDiretoria', codigoDiretoria)
    queryParams.append('codigoDepartamento', codigoDepartamento)
    queryParams.append('codigoGestorAdm', codigoGestorAdm)

    return httpClient.get<ListarGestoresProjetoResponse>(
      `/api/Projeto/ProjetoOrg/ListarGestoresProjeto?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarStatus(
    token: string,
    language: string = 'pt-BR'
  ): Promise<StatusApontamento[]> {
    return httpClient.get<StatusApontamento[]>(
      '/api/Apontamento/ListarStatus',
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarColaboradoresEApontamentosPorGestor(
    token: string,
    params: {
      nomeColaborador?: string
      codigoGerente?: string
      codigoStatus?: string
      mesVigencia: number
      anoVigencia: number
      cursor?: number
      limite?: number
      codProjeto?: string
      codColaboradorExternoAprovador?: string
    },
    language: string = 'pt-BR'
  ): Promise<ListarColaboradoresEApontamentosPorGestorResponse> {
    const queryParams = new URLSearchParams()
    queryParams.append('nomeColaborador', params.nomeColaborador || '')
    queryParams.append('codigoGerente', params.codigoGerente || '0')
    queryParams.append('codigoStatus', params.codigoStatus || '0')
    queryParams.append('mesVigencia', params.mesVigencia.toString())
    queryParams.append('anoVigencia', params.anoVigencia.toString())
    queryParams.append('cursor', (params.cursor || 0).toString())
    queryParams.append('limite', (params.limite || 20).toString())
    queryParams.append('codProjeto', params.codProjeto || '')
    queryParams.append('codColaboradorExternoAprovador', params.codColaboradorExternoAprovador || '')

    return httpClient.get<ListarColaboradoresEApontamentosPorGestorResponse>(
      `/api/Apontamento/ListarColaboradoresEApontamentosPorGestor?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async enviarEmailNotificacaoAprovadoresStatusPendentes(
    token: string,
    params: {
      nomeColaborador?: string
      codigoGerente?: string
      codigoStatus?: string
      mesVigencia: number
      anoVigencia: number
      codProjeto?: string
      codColaboradorExternoAprovador?: string
    },
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    const queryParams = new URLSearchParams()
    queryParams.append('nomeColaborador', params.nomeColaborador || '')
    queryParams.append('codigoGerente', params.codigoGerente || '0')
    queryParams.append('codigoStatus', params.codigoStatus || '0')
    queryParams.append('mesVigencia', params.mesVigencia.toString())
    queryParams.append('anoVigencia', params.anoVigencia.toString())
    queryParams.append('codProjeto', params.codProjeto || '')
    queryParams.append('codColaboradorExternoAprovador', params.codColaboradorExternoAprovador || '')

    return httpClient.post<{ sucesso: boolean; mensagem: string; erros: string[] | null }>(
      `/api/Apontamento/EnviaEmailNotificacaoAprovadoresStatusPendentes?${queryParams.toString()}`,
      undefined,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async fecharAlterarPeriodo(
    token: string,
    dataFim: string,
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    return httpClient.post<{ sucesso: boolean; mensagem: string; erros: string[] | null }>(
      `/api/Apontamento/PeriodoFechado/FecharAlterarPeriodo?dataFim=${encodeURIComponent(dataFim)}`,
      undefined,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async aprovarProjetoVigenciaEmLote(
    token: string,
    payload: {
      mes: number
      ano: number
      justificativa: string
      projetos: Array<{ cpf: string; codProjeto: string }>
      dataColetaDeDados: string
    },
    language: string = 'pt-BR'
  ): Promise<AprovarProjetoVigenciaEmLoteResponse> {
    const url = `${API_BASE_URL}/api/Apontamento/AprovarProjetoVigenciaEmLote`

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
      body: JSON.stringify(payload),
    })

    if (!response.ok) {
      throw new Error('Erro ao aprovar projetos em lote')
    }

    const data = await response.json()
    return data
  }

  async aprovarApontamentoEmLoteGerenteDeProjeto(
    token: string,
    payload: {
      ids: string[]
      justificativa: string
      dataColetaDeDados: string
    },
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    const url = `${API_BASE_URL}/api/Apontamento/AprovarApontamentoEmLoteGerenteDeProjeto`

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
      body: JSON.stringify(payload),
    })

    const data = await response.json()

    // Se a resposta não for ok mas contém dados estruturados (sucesso, mensagem), retornar os dados
    // Caso contrário, lançar erro
    if (!response.ok) {
      // Se a resposta contém estrutura esperada (sucesso, mensagem), retornar normalmente
      if (data.sucesso !== undefined || data.mensagem) {
        return data
      }
      // Caso contrário, lançar erro
      const error = new Error(data.mensagem || 'Erro ao aprovar apontamentos em lote') as any
      error.response = { status: response.status, data }
      error.data = data
      throw error
    }

    return data
  }

  async reprovarApontamentosEmLoteGerenteDeProjeto(
    token: string,
    payload: {
      ids: string[]
      justificativa: string
    },
    language: string = 'pt-BR'
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }> {
    const url = `${API_BASE_URL}/api/Apontamento/ReprovarApontamentosEmLoteGerenteDeProjeto`

    const response = await fetch(url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
      body: JSON.stringify(payload),
    })

    const data = await response.json()

    // Se a resposta não for ok mas contém dados estruturados (sucesso, mensagem), retornar os dados
    // Caso contrário, lançar erro
    if (!response.ok) {
      // Se a resposta contém estrutura esperada (sucesso, mensagem), retornar normalmente
      if (data.sucesso !== undefined || data.mensagem) {
        return data
      }
      // Caso contrário, lançar erro
      const error = new Error(data.mensagem || 'Erro ao reprovar apontamentos em lote') as any
      error.response = { status: response.status, data }
      error.data = data
      throw error
    }

    return data
  }

  async listarApontamentosPorVigenciaRelacionadosAoGerente(
    token: string,
    cpfColaborador: string,
    mes: number,
    ano: number,
    language: string = 'pt-BR'
  ): Promise<ListarApontamentosPorVigenciaResponse> {
    const url = `${API_BASE_URL}/api/Apontamento/ListarApontamentosPorVigenciaRelacionadosAoGerente/${cpfColaborador}/${mes}/${ano}`

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
    })

    if (!response.ok) {
      throw new Error('Erro ao listar apontamentos por vigência relacionados ao gerente')
    }

    const data = await response.json()
    return data
  }

  async listarVigenciasApontamentosGerenteDeProjeto(
    token: string,
    language: string = 'pt-BR'
  ): Promise<ListarVigenciasApontamentosGerenteDeProjetoResponse> {
    return httpClient.get<ListarVigenciasApontamentosGerenteDeProjetoResponse>(
      '/api/Apontamento/ListarVigenciasApontamentosGerenteDeProjeto',
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarColaboradoresVinculadosGerenteDeProjeto(
    token: string,
    codProjeto: string,
    mes: number,
    ano: number,
    language: string = 'pt-BR'
  ): Promise<Array<{
    colaboradorCpf: string
    nomeColaborador: string
    orgId: number
  }>> {
    const queryParams = new URLSearchParams()
    queryParams.append('codProjeto', codProjeto)
    queryParams.append('mes', mes.toString())
    queryParams.append('ano', ano.toString())

    const data = await httpClient.get<Array<{
      colaboradorCpf: string
      nomeColaborador: string
      orgId: number
    }>>(
      `/api/Apontamento/ListarColaboradoresVinculadosGerenteDeProjeto?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
    return Array.isArray(data) ? data : []
  }

  async listarProjetosVisaoGerenteDeProjeto(
    token: string,
    params: {
      codProjeto?: string
      mes: number
      ano: number
      cpfColaborador?: string
      codStatusGrupo?: string
      cpfGerenteAdm?: string
    },
    language: string = 'pt-BR'
  ): Promise<{
    retorno: {
      lista: Array<{
        mes: number
        ano: number
        codProjeto: string
        nomeProjeto: string
        dataFimProjeto: string | null
        cpfColaborador: string
        nomeColaborador: string
        ativo: boolean
        dataInativacao: string | null
        dataAdmissao: string
        nomeGestorAdm: string
        orgId: number
        codStatusMensal: number
        descricaoStatusMensal: string
        totalHoras: number
        codCliente: string
        nomeCliente: string
      }>
      dataColetaDeDados?: string
      totalizador?: TotalizadorProjetosVisaoGerente
      totalizadorBigNumbers?: TotalizadorBigNumbers
    }
  }> {
    const queryParams = new URLSearchParams()
    queryParams.append('codProjeto', params.codProjeto || '')
    queryParams.append('mes', params.mes.toString())
    queryParams.append('ano', params.ano.toString())
    queryParams.append('cpfColaborador', params.cpfColaborador || '')
    queryParams.append('codStatusGrupo', params.codStatusGrupo || '')
    queryParams.append('cpfGerenteAdm', params.cpfGerenteAdm || '')

    return httpClient.get<{
      retorno: {
        lista: Array<{
          mes: number
          ano: number
          codProjeto: string
          nomeProjeto: string
          dataFimProjeto: string | null
          cpfColaborador: string
          nomeColaborador: string
          ativo: boolean
          dataInativacao: string | null
          dataAdmissao: string
          nomeGestorAdm: string
          orgId: number
          codStatusMensal: number
          descricaoStatusMensal: string
          totalHoras: number
          codCliente: string
          nomeCliente: string
        }>
        dataColetaDeDados?: string
        totalizador?: TotalizadorProjetosVisaoGerente
        totalizadorBigNumbers?: TotalizadorBigNumbers
      }
    }>(
      `/api/Apontamento/ListarProjetosVisaoGerenteDeProjeto?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async listarGerenteAdmDosColabsGP(
    token: string,
    mes: number,
    ano: number,
    language: string = 'pt-BR'
  ): Promise<{
    retorno: Array<{
      colaboradorCpf: string
      nomeColaborador: string
      orgId: number
    }>
  }> {
    const queryParams = new URLSearchParams()
    queryParams.append('mes', mes.toString())
    queryParams.append('ano', ano.toString())

    return httpClient.get<{
      retorno: Array<{
        colaboradorCpf: string
        nomeColaborador: string
        orgId: number
      }>
    }>(
      `/api/Apontamento/ListarGerenteAdmDosColabsGP?${queryParams.toString()}`,
      {
        token,
        headers: {
          'Accept-Language': language,
        },
      }
    )
  }

  async relatorioApontamentos(
    token: string,
    mes: number,
    ano: number,
    language: string = 'pt-BR'
  ): Promise<{
    retorno: {
      fileContents: string
      contentType: string
      fileDownloadName: string
      lastModified: string | null
      entityTag: string | null
      enableRangeProcessing: boolean
    }
    sucesso: boolean
    mensagem: string | null
    erros: string[] | null
  }> {
    const url = `${API_BASE_URL}/api/Apontamento/RelatorioApontamentos/${mes}/${ano}`

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
    })

    if (!response.ok) {
      throw new Error('Erro ao gerar relatório de apontamentos')
    }

    const data = await response.json()
    return data
  }

  async relatorioApontamentosSimplificado(
    token: string,
    mes: number,
    ano: number,
    language: string = 'pt-BR'
  ): Promise<{
    retorno: {
      fileContents: string
      contentType: string
      fileDownloadName: string
      lastModified: string | null
      entityTag: string | null
      enableRangeProcessing: boolean
    }
    sucesso: boolean
    mensagem: string | null
    erros: string[] | null
  }> {
    const url = `${API_BASE_URL}/api/Apontamento/RelatorioApontamentosSimplificado/${mes}/${ano}`

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
    })

    if (!response.ok) {
      throw new Error('Erro ao gerar relatório de apontamentos simplificado')
    }

    const data = await response.json()
    return data
  }

  async relatorioColaboradoresQueNaoApontaram(
    token: string,
    mes: number,
    ano: number,
    language: string = 'pt-BR'
  ): Promise<{
    retorno: {
      fileContents: string
      contentType: string
      fileDownloadName: string
      lastModified: string | null
      entityTag: string | null
      enableRangeProcessing: boolean
    }
    sucesso: boolean
    mensagem: string | null
    erros: string[] | null
  }> {
    const url = `${API_BASE_URL}/api/Apontamento/RelatorioColaboradoresQueNaoApontaram/${mes}/${ano}`

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        'Accept-Language': language,
      },
    })

    if (!response.ok) {
      throw new Error('Erro ao gerar relatório de colaboradores que não apontaram')
    }

    const data = await response.json()
    return data
  }
}
