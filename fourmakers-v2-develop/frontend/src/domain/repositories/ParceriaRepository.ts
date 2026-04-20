import type {
  BuscarParceirosParams,
  BuscarParceirosResponse,
  InserirParceiroPayload,
  AtualizarParceiroPayload,
  ParceiroResponse,
  DeletarParceiroParams,
  DeletarParceiroResponse,
} from '@domain/entities/Parceiro'
import type {
  InserirContratoPayload,
  AtualizarContratoPayload,
  DeletarContratoParams,
  ContratoResponse,
  DeletarContratoResponse,
} from '@domain/entities/Contrato'
import type {
  InserirArquivoParams,
  InserirArquivoResponse,
} from '@domain/entities/ArquivoParceiro'
import type { RelatorioParceriaResponse } from '@domain/entities/RelatorioParceria'
import type { ListarUnidadesResponse } from '@domain/entities/NotaFiscalGestao'

export interface ParceriaRepository {
  // Parceiros
  buscarTodosParceiros(
    token: string,
    params: BuscarParceirosParams
  ): Promise<BuscarParceirosResponse>

  inserirParceiro(
    token: string,
    payload: InserirParceiroPayload
  ): Promise<ParceiroResponse>

  atualizarParceiro(
    token: string,
    payload: AtualizarParceiroPayload
  ): Promise<ParceiroResponse>

  deletarParceiro(
    token: string,
    params: DeletarParceiroParams
  ): Promise<DeletarParceiroResponse>

  // Contratos
  inserirContrato(
    token: string,
    payload: InserirContratoPayload
  ): Promise<ContratoResponse>

  atualizarContrato(
    token: string,
    payload: AtualizarContratoPayload
  ): Promise<ContratoResponse>

  deletarContrato(
    token: string,
    params: DeletarContratoParams
  ): Promise<DeletarContratoResponse>

  // Arquivo
  inserirArquivo(
    token: string,
    params: InserirArquivoParams
  ): Promise<InserirArquivoResponse>

  // Relatório
  gerarRelatorioParceria(token: string): Promise<RelatorioParceriaResponse>

  // Unidades
  listarUnidades(token: string): Promise<ListarUnidadesResponse>
}
