import type { ColaboradoresResponse, ColaboradoresParams } from '@domain/entities/Colaborador'
import type { ColaboradoresCchResponse, ListarColaboradoresOrgParams } from '@domain/entities/ColaboradorCch'
import type { DepartamentosResponse, ListarDepartamentosParams } from '@domain/entities/Departamento'
import type { DiretoriasResponse } from '@domain/entities/Diretoria'
import type { EmpresasRelacionadasResponse, ListarEmpresasRelacionadasParams } from '@domain/entities/EmpresaRelacionada'
import type { ModelosContratacaoResponse } from '@domain/entities/ModeloContratacao'
import type { CargosResponse } from '@domain/entities/Cargo'
import type { BuscarDadosColaboradorResponse } from '@domain/entities/Profile360'
import type { ListarEscolaridadeColaboradorResponse, ListarEscolaridadeColaboradorParams } from '@domain/entities/Escolaridade'
import type { OrigemColaboradorItem } from '@domain/entities/GestaoVagasCandidatos'
import type {
  ObterDadosColaboradorResponse,
  EditarDadosColaboradorPayload,
  EditarDadosColaboradorResponse,
} from '@domain/entities/ColaboradorDadosPessoais'
import type { AlterarDadosColaboradorParametrosPayload } from '@domain/entities/AlterarDadosColaboradorParametros'

export interface InserirColaboradorPayload {
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
}

export interface EditarColaboradorPayload {
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
}

export interface ColaboradorResponse {
  sucesso: boolean
  mensagem?: string
  erros?: string[]
}

export interface RelatorioColaboradoresResult {
  arrayBuffer: ArrayBuffer
  contentType: string
  fileName: string
}

export interface CertificadoColaboradorResult {
  arrayBuffer: ArrayBuffer
  contentType: string
  fileName: string
}

/** Payload para AlterarFormularioColaborador. Pode ser apenas email/celular (inscrição) ou o corpo completo do colaborador (vaga pública) para não apagar dados no backend. */
export interface AlterarFormularioColaboradorPayload {
  emailAlternativo?: string
  celular?: string
  documentoColaborador?: string
  endereco?: {
    cep?: string
    endereco?: string
    complemento?: string
    numero?: number
    bairro?: string
    cidade?: string
    estado?: string
    comQuemMora?: string
  }
  [key: string]: unknown
}

export interface ColaboradoresRepository {
  getColaboradores(token: string, params: ColaboradoresParams): Promise<ColaboradoresResponse>
  listarColaboradoresOrg(token: string, params: ListarColaboradoresOrgParams): Promise<ColaboradoresCchResponse>
  listarDepartamentos(token: string, params: ListarDepartamentosParams): Promise<DepartamentosResponse>
  listarDiretorias(token: string): Promise<DiretoriasResponse>
  listarEmpresasRelacionadas(token: string, params: ListarEmpresasRelacionadasParams): Promise<EmpresasRelacionadasResponse>
  listarModelosContratacao(token: string): Promise<ModelosContratacaoResponse>
  listarCargos(token: string): Promise<CargosResponse>
  inserirColaborador(token: string, payload: InserirColaboradorPayload): Promise<ColaboradorResponse>
  editarColaborador(token: string, payload: EditarColaboradorPayload): Promise<ColaboradorResponse>
  buscarDadosColaborador(token: string, cpf: string): Promise<BuscarDadosColaboradorResponse>
  listarEscolaridadeColaborador(token: string, params: ListarEscolaridadeColaboradorParams): Promise<ListarEscolaridadeColaboradorResponse>
  gerarRelatorioColaboradores(token: string): Promise<RelatorioColaboradoresResult>
  baixarCertificadoColaborador(token: string, path: string): Promise<CertificadoColaboradorResult>
  listarOrigensColaborador(token: string): Promise<OrigemColaboradorItem[]>
  obterDadosColaborador(token: string, codigoInternoColaborador: string): Promise<ObterDadosColaboradorResponse>
  editarDadosColaborador(token: string, payload: EditarDadosColaboradorPayload): Promise<EditarDadosColaboradorResponse>
  alterarFormularioColaborador(
    token: string,
    payload: AlterarFormularioColaboradorPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }>
  alterarDadosColaboradorParametros(
    token: string,
    payload: AlterarDadosColaboradorParametrosPayload
  ): Promise<{ sucesso?: boolean; mensagem?: string; erros?: string[] | null }>
}
