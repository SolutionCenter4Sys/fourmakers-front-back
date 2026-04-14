import { injectable } from 'tsyringe'

import type { DadosVcx360 } from '@domain/entities/Vcx360'
import type { Vcx360Repository } from '@domain/repositories/Vcx360Repository'

@injectable()
export class Vcx360RepositoryImpl implements Vcx360Repository {
  private readonly PREFIX = 'vcx360_data'

  private gerarChave(codigoCliente: string, departamentoId: string): string {
    return `${this.PREFIX}_${codigoCliente}_${departamentoId}`
  }

  async buscarDadosVcx(codigoCliente: string, departamentoId: string): Promise<DadosVcx360 | null> {
    try {
      const chave = this.gerarChave(codigoCliente, departamentoId)
      const dados = localStorage.getItem(chave)

      if (!dados) {
        return null
      }

      return JSON.parse(dados) as DadosVcx360
    } catch (error) {
      console.error('Erro ao buscar dados VCX 360:', error)
      return null
    }
  }

  async salvarDadosVcx(codigoCliente: string, dados: DadosVcx360): Promise<void> {
    try {
      const chave = this.gerarChave(codigoCliente, dados.departamentoId)
      const dadosComTimestamp = {
        ...dados,
        dataUltimaAtualizacao: new Date().toISOString(),
      }
      localStorage.setItem(chave, JSON.stringify(dadosComTimestamp))
    } catch (error) {
      console.error('Erro ao salvar dados VCX 360:', error)
      throw error
    }
  }

  async removerDadosVcx(codigoCliente: string, departamentoId: string): Promise<void> {
    try {
      const chave = this.gerarChave(codigoCliente, departamentoId)
      localStorage.removeItem(chave)
    } catch (error) {
      console.error('Erro ao remover dados VCX 360:', error)
      throw error
    }
  }
}
