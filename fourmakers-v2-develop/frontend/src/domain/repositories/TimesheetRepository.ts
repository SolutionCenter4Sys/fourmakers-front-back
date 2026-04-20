import type {
  ListarVigenciasColaboradorResponse,
  ListarProjetosComAtividadesResponse,
  ListarTemplateSemanaVigenciaResponse,
  ListarApontamentosPorVigenciaResponse,
  PeriodoFechado,
  ListarGestoresApontamentoResponse,
  ProjetoGerenteDeProjeto,
  ListarGestoresProjetoResponse,
  StatusApontamento,
  ListarColaboradoresEApontamentosPorGestorResponse,
  ListarVigenciasApontamentosGerenteDeProjetoResponse,
} from '@data/api/TimesheetComponentesApi'

export interface PayloadApontarHorasEmLote {
  vigencia: {
    mes: number
    ano: number
  }
  apontamentos: Array<{
    data: string
    projetoId: string
    atividadeId: string
    horas: number
    resumoAtividades?: string
    feriado?: boolean
  }>
}

export interface PayloadEditarApontamento {
  apontamentoId: string
  horas: number
  resumoAtividades?: string
}

export interface TimesheetRepository {
  listarVigenciasColaborador(
    token: string,
    cpfColaborador: string
  ): Promise<ListarVigenciasColaboradorResponse>

  listarProjetosComAtividades(
    token: string,
    cpfColaborador?: string
  ): Promise<ListarProjetosComAtividadesResponse>

  listarTemplateSemanaVigencia(
    token: string,
    mes: number,
    ano: number
  ): Promise<ListarTemplateSemanaVigenciaResponse>

  listarApontamentosPorVigencia(
    token: string,
    mes: number,
    ano: number,
    cpfColaborador: string
  ): Promise<ListarApontamentosPorVigenciaResponse>

  apontarHorasEmLote(
    token: string,
    payload: PayloadApontarHorasEmLote
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }>

  editarApontamento(
    token: string,
    payload: PayloadEditarApontamento
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }>

  deletarApontamentoColaborador(
    token: string,
    apontamentoId: string,
    dataColetaDeDados: string
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }>

  buscarPeriodoFechado(
    token: string
  ): Promise<PeriodoFechado>

  listarGestoresApontamento(
    token: string
  ): Promise<ListarGestoresApontamentoResponse>

  listarProjetosGerenteDeProjeto(
    token: string
  ): Promise<ProjetoGerenteDeProjeto[]>

  listarGestoresProjeto(
    token: string,
    codigoDiretoria?: string,
    codigoDepartamento?: string,
    codigoGestorAdm?: string
  ): Promise<ListarGestoresProjetoResponse>

  listarStatus(
    token: string
  ): Promise<StatusApontamento[]>

  listarColaboradoresEApontamentosPorGestor(
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
    }
  ): Promise<ListarColaboradoresEApontamentosPorGestorResponse>

  enviaEmailNotificacaoAprovadoresStatusPendentes(
    token: string,
    params: {
      nomeColaborador?: string
      codigoGerente?: string
      codigoStatus?: string
      mesVigencia: number
      anoVigencia: number
      codProjeto?: string
      codColaboradorExternoAprovador?: string
    }
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }>

  fecharAlterarPeriodo(
    token: string,
    dataFim: string
  ): Promise<{ sucesso: boolean; mensagem: string; erros: string[] | null }>

  listarVigenciasApontamentosGerenteDeProjeto(
    token: string
  ): Promise<ListarVigenciasApontamentosGerenteDeProjetoResponse>

  listarProjetosVisaoGerenteDeProjeto(
    token: string,
    params: {
      codProjeto?: string
      mes: number
      ano: number
      cpfColaborador?: string
      codStatusGrupo?: string
      cpfGerenteAdm?: string
    }
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
    }
  }>

  listarGerenteAdmDosColabsGP(
    token: string,
    mes: number,
    ano: number
  ): Promise<{
    retorno: Array<{
      colaboradorCpf: string
      nomeColaborador: string
      orgId: number
    }>
  }>

  listarColaboradoresVinculadosGerenteDeProjeto(
    token: string,
    codProjeto: string,
    mes: number,
    ano: number
  ): Promise<Array<{
    colaboradorCpf: string
    nomeColaborador: string
    orgId: number
  }>>
}

