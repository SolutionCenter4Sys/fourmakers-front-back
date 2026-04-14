import type { MinhaJornadaSkill } from './MinhaJornadaSkill'

export interface MinhaJornadaProfile {
  // Identificação do perfil/cliente/alocação
  perfilId?: string | null
  perfilNome: string
  codigoCliente: string
  nomeCliente: string
  idAlocacao: number
  codigoProjeto: string
  orgId: number

  // Colaborador e gestores
  codigoInternoColaborador: string
  nomeColaborador: string
  codigoInternoGestorAdm: string
  nomeGestorAdm: string
  codGestorCliente: string
  nomeGestorCliente: string

  // Lista flatten de habilidades do perfil
  skills: MinhaJornadaSkill[]
}


