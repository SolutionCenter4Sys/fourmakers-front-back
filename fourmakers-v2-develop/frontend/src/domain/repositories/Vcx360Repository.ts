import type { DadosVcx360 } from '@domain/entities/Vcx360'

export interface Vcx360Repository {
  buscarDadosVcx(codigoCliente: string, departamentoId: string): Promise<DadosVcx360 | null>
  salvarDadosVcx(codigoCliente: string, dados: DadosVcx360): Promise<void>
  removerDadosVcx(codigoCliente: string, departamentoId: string): Promise<void>
}
