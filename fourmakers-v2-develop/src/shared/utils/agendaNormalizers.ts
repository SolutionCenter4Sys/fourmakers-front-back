import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'

/**
 * Normaliza um código para comparação (trim + lowercase).
 * 
 * @param codigo Código a ser normalizado
 * @returns Código normalizado ou string vazia
 */
function normalizarCodigo(codigo: string | null | undefined): string {
  if (!codigo) {
    return ''
  }
  return codigo.trim().toLowerCase()
}

/**
 * Resultado da normalização de um participante.
 */
export interface ParticipanteNormalizado {
  nome: string
  email?: string
  tipo: string
}

/**
 * Normaliza um participante da agenda, buscando nome e email nos colaboradores ou gestores externos.
 * 
 * @param participante Participante a ser normalizado
 * @param colaboradores Lista de colaboradores da agenda
 * @param gestoresExternos Lista de gestores externos da agenda
 * @returns Participante normalizado com nome, email e tipo
 */
export function normalizarParticipante(
  participante: NonNullable<ItemAgendaGestor['participantesDetalhes']>[number],
  colaboradores?: ItemAgendaGestor['colaboradores'],
  gestoresExternos?: ItemAgendaGestor['gestoresExternos']
): ParticipanteNormalizado {
  let nomeParticipante: string | null = null
  let tipoParticipante = ''
  let emailParticipante: string | undefined

  if (participante.codigoColaborador) {
    const codigoParticipanteNormalizado = normalizarCodigo(participante.codigoColaborador)

    // Buscar nos colaboradores
    const colaborador = colaboradores?.find((c) => {
      const codigoColabNormalizado = normalizarCodigo(c.codInternoColaborador)
      return codigoColabNormalizado === codigoParticipanteNormalizado
    })

    if (colaborador && colaborador.nome && colaborador.nome.trim() !== '') {
      nomeParticipante = colaborador.nome
      emailParticipante = colaborador.email
      tipoParticipante = 'Colaborador'
    }
  } else if (participante.codigoGestorExterno) {
    const codigoGestor = participante.codigoGestorExterno
    const codigoGestorNormalizado = normalizarCodigo(codigoGestor)
    tipoParticipante = 'Gestor Externo'

    // Primeiro tentar usar o Nome da API se disponível
    if (participante.Nome && participante.Nome.trim() !== '') {
      nomeParticipante = participante.Nome

      // Mesmo com Nome, tentar buscar email nos gestoresExternos
      // Tentar match pelo codGestorExterno
      let gestor = gestoresExternos?.find((g) => {
        const codigoGestorExternoNormalizado = normalizarCodigo(g.codGestorExterno)
        return (
          codigoGestorExternoNormalizado === codigoGestorNormalizado &&
          codigoGestorExternoNormalizado !== ''
        )
      })

      // Se não encontrou pelo codGestorExterno, tentar match por codigoInternoColaborador
      if (!gestor) {
        gestor = gestoresExternos?.find((g) => {
          if (g.codigoInternoColaborador) {
            const codigoInternoNormalizado = normalizarCodigo(g.codigoInternoColaborador)
            return codigoInternoNormalizado === codigoGestorNormalizado
          }
          return false
        })
      }

      if (gestor && gestor.email) {
        emailParticipante = gestor.email
      }
    } else {
      // Tentar encontrar gestor pelo código para pegar nome e email
      // Primeiro tentar match pelo codGestorExterno
      let gestor = gestoresExternos?.find((g) => {
        const codigoGestorExternoNormalizado = normalizarCodigo(g.codGestorExterno)
        return (
          codigoGestorExternoNormalizado === codigoGestorNormalizado &&
          codigoGestorExternoNormalizado !== ''
        )
      })

      // Se não encontrou pelo codGestorExterno, tentar match por codigoInternoColaborador
      if (!gestor) {
        gestor = gestoresExternos?.find((g) => {
          if (g.codigoInternoColaborador) {
            const codigoInternoNormalizado = normalizarCodigo(g.codigoInternoColaborador)
            return codigoInternoNormalizado === codigoGestorNormalizado
          }
          return false
        })
      }

      // Se ainda não encontrou e há gestores externos no array,
      // pode ser que o codGestorExterno seja null no array mas o participante tenha o código
      // Nesse caso, se houver apenas um gestor externo, podemos assumir que é ele
      if (!gestor && gestoresExternos && gestoresExternos.length > 0) {
        // Se há apenas um gestor externo e o codGestorExterno dele é null/vazio,
        // podemos assumir que é esse gestor (caso comum quando o backend não preenche o código)
        if (gestoresExternos.length === 1) {
          const unicoGestor = gestoresExternos[0]
          if (
            (!unicoGestor.codGestorExterno || unicoGestor.codGestorExterno.trim() === '') &&
            unicoGestor.nome &&
            unicoGestor.nome.trim() !== ''
          ) {
            gestor = unicoGestor
          }
        }
      }

      if (gestor && gestor.nome && gestor.nome.trim() !== '') {
        nomeParticipante = gestor.nome
        emailParticipante = gestor.email
      }
    }
  }

  // Se não encontrou nome, usar fallback genérico (nunca usar código como nome)
  if (!nomeParticipante) {
    nomeParticipante = tipoParticipante || 'Participante'
  }

  return {
    nome: nomeParticipante,
    email: emailParticipante,
    tipo: tipoParticipante,
  }
}
