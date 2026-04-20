import type { MinhaEquipeHabilidade } from '@domain/entities/MinhaEquipeHabilidade'

export interface MinhaEquipeColaborador {
  id: string
  nomeColaborador: string
  nomeCliente: string
  nomeGestorCliente: string
  perfil: string
  perfilId: string
  nomeGestorOperacional: string
  nomeGestorAdm: string
  codigoInternoColaborador: string
  codigoGestorAdm: string
  idAlocacao: number
  match: number // Aderência 0-100
  retornoMatch: {
    hardSkills: number
    softSkills: number
    methodologies: number
    domains: number
    languages: number
  }
  resultadoHabilidades: MinhaEquipeHabilidade[] | null
  gestoresOperacionais?: string[]
}
