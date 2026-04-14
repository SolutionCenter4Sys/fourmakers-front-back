// Interface para operações CRUD do Organograma

import type {
  DepartamentoPayload,
  DepartamentoResponse,
  PerfilCorporativoPayload,
  PerfilCorporativoResponse,
  PosicaoPayload,
  PosicaoResponse,
  AlocacaoPayload,
  AlocacaoResponse,
  PosicaoCompletaResponse,
} from '@domain/entities/Organograma'

export interface OrganogramaRepository {
  // Listar Organograma Completo
  listarOrganogramaCompleto(token: string, codigoCliente: string, orgId: number): Promise<PosicaoCompletaResponse>

  // Departamento
  criarDepartamento(token: string, payload: DepartamentoPayload): Promise<DepartamentoResponse>
  atualizarDepartamento(token: string, payload: DepartamentoPayload & { id: string }): Promise<DepartamentoResponse>
  deletarDepartamento(token: string, departamentoId: string): Promise<void>

  // Perfil Corporativo
  criarPerfilCorporativo(token: string, payload: PerfilCorporativoPayload): Promise<PerfilCorporativoResponse>
  atualizarPerfilCorporativo(
    token: string,
    payload: PerfilCorporativoPayload & { id: string },
  ): Promise<PerfilCorporativoResponse>
  deletarPerfilCorporativo(token: string, perfilCorpId: string): Promise<void>

  // Posição
  criarPosicao(token: string, payload: PosicaoPayload): Promise<PosicaoResponse>
  atualizarPosicao(token: string, payload: PosicaoPayload & { id: string }): Promise<PosicaoResponse>
  deletarPosicao(token: string, posicaoId: string): Promise<void>

  // Alocação
  criarAlocacao(token: string, payload: AlocacaoPayload): Promise<AlocacaoResponse>
  atualizarAlocacao(token: string, payload: AlocacaoPayload & { id: string }): Promise<AlocacaoResponse>
  deletarAlocacao(token: string, alocacaoId: string): Promise<void>

  // Buscar Perfis Corporativos por OrgId
  buscarPerfisPorOrg(
    token: string,
    orgId: number,
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[]; retorno: PerfilCorporativoResponse[] }>

  // Buscar Perfil Corporativo por ID
  // O endpoint retorna o perfil completo com todos os campos
  buscarPerfilCorporativoPorId(
    token: string,
    perfilCorpId: string,
  ): Promise<{
    sucesso: boolean
    mensagem?: string
    erros?: string[]
    retorno: PerfilCorporativoResponse & {
      codigoInternoColaboradorCriacao?: string
      codigoInternoColaboradorAlteracao?: string | null
      dataCriacao?: string
      dataAlteracao?: string
    }
  }>

  // Listar Colaboradores Externos por Cliente (paginação e filtro por nome)
  listarColaboradoresExternosPorCliente(
    token: string,
    codigoCliente: string,
    cursor: number,
    limit: number,
    nome?: string,
  ): Promise<Array<{ CodigoInternoColaborador: string; NomeColaborador: string }>>

  // Inserir Gestor Externo
  inserirGestorExterno(
    token: string,
    payload: {
      codGestorExterno?: string
      nome: string
      email: string
      telefone: string
      codigoCliente?: string
      perfilLinkedin?: string
      areasDeAtuacao?: Array<{
        areaDeAtuacao: { descricao: string }
        permanencia: { id: string }
      }>
      preferenciasPessoais?: string
    },
  ): Promise<{ sucesso: boolean; mensagem?: string; erros?: string[] }>
}
