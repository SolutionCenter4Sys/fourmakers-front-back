import type { ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import type { ShowmeUserProfile } from '@domain/entities/ShowmeUserProfile'
import { getCodInternoColaborador, getIdentificadoresColaborador } from './agendaUtils'

/** Status da solicitação de participação: null = sem solicitação, 0 = Pendente, 1 = Aceito, 2 = Recusado, 3 = Talvez */
export type StatusSolicitacaoParticipante = 0 | 1 | 2 | 3 | null

/**
 * Obtém o status da solicitação de participação (convite) para o usuário logado.
 * A confirmação de participação vem de participacaoUsuarioLogado (listagem e detalhe da agenda).
 * Prioridade: statusSolicitacaoParticipante (0/1/2/3); se ausente, usa confirmado (0=pendente, 1=confirmado, 2=recusado).
 *
 * @param participacao participacaoUsuarioLogado retornado pela API (agenda list + agenda detail)
 * @returns 0=Pendente, 1=Aceito, 2=Recusado, 3=Talvez, null=Sem solicitação
 */
export function obterStatusSolicitacaoParticipante(
  participacao: ItemAgendaGestor['participacaoUsuarioLogado'] | null | undefined
): StatusSolicitacaoParticipante {
  if (!participacao) {
    return null
  }

  const raw = participacao.statusSolicitacaoParticipante as number | string | null | undefined

  if (typeof raw === 'number') {
    if (raw === 0 || raw === 1 || raw === 2 || raw === 3) return raw
    return null
  }

  if (typeof raw === 'string') {
    const s = raw.toLowerCase()
    if (s === 'pendente') return 0
    if (s === 'aceito') return 1
    if (s === 'recusado') return 2
    if (s === 'talvez') return 3
  }

  // API pode enviar apenas confirmado: 0=pendente, 1=confirmado, 2=recusado (ex.: participacaoUsuarioLogado.confirmado)
  const confirmado = (participacao as { confirmado?: number | boolean }).confirmado
  if (typeof confirmado === 'number') {
    if (confirmado === 0 || confirmado === 1 || confirmado === 2) return confirmado
  }
  if (typeof confirmado === 'boolean') {
    return confirmado ? 1 : 0
  }

  return null
}

/**
 * Obtém o status do convite com fallback: primeiro usa participacaoUsuarioLogado;
 * se não houver status, busca o usuário logado em participantesDetalhes (campo participantes da API)
 * e deriva o status: confirmado true = 1 (Aceito), false = 0 (Pendente).
 *
 * @param agenda Agenda com participacaoUsuarioLogado e/ou participantesDetalhes
 * @param user Usuário logado
 * @returns 0=Pendente, 1=Aceito, 2=Recusado, 3=Talvez, null=Sem solicitação
 */
export function obterStatusSolicitacaoParticipanteComFallback(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): StatusSolicitacaoParticipante {
  const statusPrincipal = obterStatusSolicitacaoParticipante(agenda.participacaoUsuarioLogado)
  if (statusPrincipal !== null) {
    return statusPrincipal
  }

  if (!user || !agenda.participantesDetalhes?.length) {
    return null
  }

  const meusIds = getIdentificadoresColaborador(user)
  if (meusIds.length === 0) {
    const cod = getCodInternoColaborador(user)
    if (cod?.trim()) meusIds.push(cod.trim().toLowerCase())
  }

  const participante = agenda.participantesDetalhes.find((p) => {
    const codColab = (p as { codigoColaborador?: string }).codigoColaborador?.trim().toLowerCase()
    const codGestor = (p as { codigoGestorExterno?: string }).codigoGestorExterno?.trim().toLowerCase()
    return (
      (codColab && meusIds.some((id) => id === codColab)) ||
      (codGestor && meusIds.some((id) => id === codGestor))
    )
  })

  if (!participante) {
    return null
  }

  const confirmado = (participante as { confirmado?: boolean }).confirmado
  return typeof confirmado === 'boolean' ? (confirmado ? 1 : 0) : null
}

/** Retorno do badge de convite para UI (variant do Badge + className opcional para cores do DS) */
export interface BadgeConvite {
  label: string
  variant: 'default' | 'secondary' | 'destructive' | 'outline'
  className?: string
}

/**
 * Retorna o badge de convite conforme statusSolicitacaoParticipante.
 * null = sem badge; 0 = Convite Pendente; 1 = Convite Aceito; 2 = Convite Recusado; 3 = Talvez.
 */
export function obterBadgeConvite(
  participacao: ItemAgendaGestor['participacaoUsuarioLogado'] | null | undefined
): BadgeConvite | null {
  const status = obterStatusSolicitacaoParticipante(participacao)
  return statusParaBadgeConvite(status)
}

/**
 * Retorna o badge de convite com fallback: usa participacaoUsuarioLogado e, se não houver status,
 * deriva de participantesDetalhes (participantes).
 */
export function obterBadgeConviteDoUsuario(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): BadgeConvite | null {
  const status = obterStatusSolicitacaoParticipanteComFallback(agenda, user)
  return statusParaBadgeConvite(status)
}

function statusParaBadgeConvite(status: StatusSolicitacaoParticipante): BadgeConvite | null {
  if (status === null) return null
  if (status === 0) return { label: 'Convite Pendente', variant: 'default', className: 'bg-warning text-warning-foreground border-transparent' }
  if (status === 1) return { label: 'Convite Aceito', variant: 'default', className: 'bg-success/10 text-success border-transparent' }
  if (status === 2) return { label: 'Convite Recusado', variant: 'destructive' }
  if (status === 3) return { label: 'Talvez', variant: 'secondary' }
  return null
}

function normalizarNome(n: string | null | undefined): string {
  if (n == null || typeof n !== 'string') return ''
  return n.trim().toLowerCase().replace(/\s+/g, ' ')
}

/**
 * Verifica se o usuário logado é o criador do item (agenda ou interação/encontro).
 * Compara por: (1) codColaboradorCriador vs CPF/codColaborador do user; (2) fallback por nome (responsavel vs nomeColaborador).
 *
 * @param item Item (agenda ou interação) a ser verificado
 * @param user Usuário logado
 * @returns true se o usuário é o criador
 */
export function verificarSeUsuarioECriador(
  item: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  if (!user) return false

  // 1) Comparação por código (API pode devolver CPF ou codColaborador)
  if (item.codColaboradorCriador) {
    const meusIds = getIdentificadoresColaborador(user)
    if (meusIds.length === 0) {
      const fallback = getCodInternoColaborador(user)
      if (fallback) meusIds.push(fallback.trim().toLowerCase())
    }
    const codCriador = String(item.codColaboradorCriador).trim().toLowerCase()
    if (meusIds.some((id) => id === codCriador)) return true
  }

  // 2) Fallback por nome (quando a API não envia codigoInternoColaborador)
  const nomeCriador = item.responsavel
  const meuNome = user.nomeColaborador || user.colaborador?.nomeCompleto
  if (nomeCriador && meuNome && normalizarNome(nomeCriador) === normalizarNome(meuNome)) {
    return true
  }

  return false
}

/**
 * Indica se o usuário pode editar ou excluir uma interação/encontro.
 * Conforme legado: (1) criador da interação pode; (2) criador da agenda pode editar qualquer interação da agenda.
 *
 * @param interacao Interação a ser verificada
 * @param user Usuário logado
 * @param agendas Lista de agendas (opcional); se informada, criador da agenda também pode editar/excluir
 */
export function podeEditarExcluirInteracao(
  interacao: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined,
  agendas: ItemAgendaGestor[] | null | undefined
): boolean {
  if (!user) return false
  if (verificarSeUsuarioECriador(interacao, user)) return true
  if (interacao.agendaId && Array.isArray(agendas) && agendas.length > 0) {
    const agenda = agendas.find((a) => a.agendaId === interacao.agendaId)
    if (agenda && verificarSeUsuarioECriador(agenda, user)) return true
  }
  return false
}

/**
 * Indica se o usuário logado pode criar interações nesta agenda.
 * Conforme legado: souParticipante — participação formal OU criador OU em colaboradores/gestores/participantes.
 * Usado para exibir/ocultar o botão "Nova Interação".
 *
 * @param agenda Agenda a ser verificada
 * @param user Usuário logado
 * @returns true se o usuário pode criar interações na agenda
 */
export function podeCriarInteracao(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  if (!user) return false

  // Participação formal, criador ou envolvido nas listas
  return (
    agenda.participacaoUsuarioLogado != null ||
    verificarSeUsuarioECriador(agenda, user) ||
    verificarSeUsuarioEstaEnvolvido(agenda, user)
  )
}

/**
 * Indica se o usuário pode criar ou editar próximos passos (EncontroAi) na interação.
 * Usa a mesma regra da criação de interações: quem pode criar interação na agenda pode gerenciar próximos passos.
 *
 * @param agendaPai Agenda à qual a interação pertence (deve estar carregada com participacaoUsuarioLogado, colaboradores, etc.)
 * @param user Usuário logado
 * @returns true se o usuário pode criar/editar próximos passos
 */
export function podeCriarOuEditarProximosPassos(
  agendaPai: ItemAgendaGestor | null | undefined,
  user: ShowmeUserProfile | null | undefined
): boolean {
  if (agendaPai == null || !user) return false
  return podeCriarInteracao(agendaPai, user)
}

/**
 * Helper genérico: verifica se o usuário está em uma lista cujo item tem um código comparável ao código do colaborador.
 */
function verificarSeUsuarioEstaNaListaPorCodigo<T>(
  user: ShowmeUserProfile | null | undefined,
  lista: T[] | null | undefined,
  obterCodigoDoItem: (item: T) => string | null | undefined
): boolean {
  if (!user || !lista || lista.length === 0) return false
  const meuCodigo = getCodInternoColaborador(user)
  if (!meuCodigo) return false
  const meuCodigoNorm = meuCodigo.trim().toLowerCase()
  return lista.some((item) => {
    if (!item) return false
    const cod = obterCodigoDoItem(item)?.trim().toLowerCase() ?? ''
    return cod === meuCodigoNorm
  })
}

/**
 * Verifica se o usuário está na lista de colaboradores da agenda.
 */
export function verificarSeUsuarioEstaEmColaboradores(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  return verificarSeUsuarioEstaNaListaPorCodigo(
    user,
    agenda.colaboradores,
    (colab) => colab.codInternoColaborador
  )
}

/**
 * Verifica se o usuário está na lista de gestores externos da agenda.
 */
export function verificarSeUsuarioEstaEmGestoresExternos(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  return verificarSeUsuarioEstaNaListaPorCodigo(
    user,
    agenda.gestoresExternos,
    (gestor) => gestor.codGestorExterno
  )
}

/**
 * Verifica se o usuário está na lista de participantes da agenda (participantesDetalhes).
 */
export function verificarSeUsuarioEstaEmParticipantes(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  return verificarSeUsuarioEstaNaListaPorCodigo(
    user,
    agenda.participantesDetalhes,
    (p) => (p as { codigoColaborador?: string }).codigoColaborador
  )
}

/**
 * Verifica se o usuário está em participantesDetalhes (campo participantes da API),
 * por codigoColaborador ou codigoGestorExterno.
 */
function verificarSeUsuarioEstaEmParticipantesDetalhes(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  if (!user || !agenda.participantesDetalhes?.length) return false
  const meusIds = getIdentificadoresColaborador(user)
  if (meusIds.length === 0) {
    const cod = getCodInternoColaborador(user)
    if (cod?.trim()) meusIds.push(cod.trim().toLowerCase())
  }
  return agenda.participantesDetalhes.some((p) => {
    const codColab = (p as { codigoColaborador?: string }).codigoColaborador?.trim().toLowerCase()
    const codGestor = (p as { codigoGestorExterno?: string }).codigoGestorExterno?.trim().toLowerCase()
    return (
      (codColab && meusIds.some((id) => id === codColab)) ||
      (codGestor && meusIds.some((id) => id === codGestor))
    )
  })
}

/**
 * Verifica se o usuário está envolvido na agenda (colaborador, gestor externo, participante ou tem participação).
 * 
 * @param agenda Agenda a ser verificada
 * @param user Usuário logado
 * @returns true se o usuário está envolvido na agenda
 */
export function verificarSeUsuarioEstaEnvolvido(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  return (
    verificarSeUsuarioEstaEmColaboradores(agenda, user) ||
    verificarSeUsuarioEstaEmGestoresExternos(agenda, user) ||
    verificarSeUsuarioEstaEmParticipantes(agenda, user) ||
    agenda.participacaoUsuarioLogado != null
  )
}

/**
 * Verifica se deve mostrar os botões de convite (aceitar/recusar).
 * Status com fallback: participacaoUsuarioLogado e, se ausente, participantesDetalhes (participantes).
 * Mostra quando: status é Pendente (0) e usuário está envolvido.
 *
 * @param agenda Agenda a ser verificada
 * @param user Usuário logado
 * @returns true se deve mostrar os botões de convite
 */
export function verificarSeDeveMostrarBotoesConvite(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  if (!user) {
    return false
  }

  const status = obterStatusSolicitacaoParticipanteComFallback(agenda, user)
  const isPendente = status === 0

  if (!isPendente) {
    return false
  }

  const isCriador = verificarSeUsuarioECriador(agenda, user)
  const estaEnvolvido = verificarSeUsuarioEstaEnvolvido(agenda, user)
  const estaEmParticipantesDetalhes = verificarSeUsuarioEstaEmParticipantesDetalhes(agenda, user)

  // Se for criador, só mostra botões se também estiver nas listas de participação
  if (isCriador) {
    return (
      verificarSeUsuarioEstaEmColaboradores(agenda, user) ||
      verificarSeUsuarioEstaEmGestoresExternos(agenda, user) ||
      verificarSeUsuarioEstaEmParticipantes(agenda, user)
    )
  }

  return estaEnvolvido || estaEmParticipantesDetalhes
}

/**
 * Deve mostrar botão de participação (Cancelar Confirmação ou Reconsiderar) quando o status da solicitação
 * é o informado e o usuário está envolvido na agenda. Usa fallback em participantes (participantesDetalhes).
 * @param statusDesejado 1 = Cancelar Confirmação, 2 = Reconsiderar
 */
export function deveMostrarBotaoPorStatusParticipacao(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined,
  statusDesejado: 1 | 2
): boolean {
  if (!user) return false
  const status = obterStatusSolicitacaoParticipanteComFallback(agenda, user)
  const estaEnvolvido = verificarSeUsuarioEstaEnvolvido(agenda, user) || verificarSeUsuarioEstaEmParticipantesDetalhes(agenda, user)
  return status === statusDesejado && estaEnvolvido
}

/** Deve mostrar o botão "Cancelar Confirmação" (status confirmado = 1). */
export function deveMostrarCancelarConfirmacao(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  return deveMostrarBotaoPorStatusParticipacao(agenda, user, 1)
}

/** Deve mostrar o botão "Reconsiderar" (status recusado = 2). */
export function deveMostrarReconsiderar(
  agenda: ItemAgendaGestor,
  user: ShowmeUserProfile | null | undefined
): boolean {
  return deveMostrarBotaoPorStatusParticipacao(agenda, user, 2)
}
