/**
 * Utilitários de validação para formulários de agenda comercial
 */

export interface ErrosValidacao {
  [campo: string]: string
}

export interface DadosFormularioAgenda {
  titulo: string
  tipoInteracao: number
  dataAgendada: Date | null
  horaInicio: string
  horaFim: string
  clienteSelecionado: { id: string; name: string } | null
  gestoresSelecionados: Array<{ codGestorExterno: string }>
  linkReuniao: string
}

/**
 * Retorna os campos obrigatórios baseado no tipo de interação
 * @param tipoInteracao Tipo de interação (1=Reunião, 2=Ligação, 3=Chat, 4=Email, 5=Presencial)
 * @returns Array com os nomes dos campos obrigatórios
 */
export function getCamposObrigatoriosPorTipoInteracao(tipoInteracao: number): string[] {
  // Reunião (1) e Presencial (5): início e fim são obrigatórios
  if (tipoInteracao === 1 || tipoInteracao === 5) {
    return ['dataInicio', 'dataFim']
  }
  return []
}

/**
 * Valida se a hora fim é posterior à hora início
 * @param horaInicio Hora de início no formato HH:mm
 * @param horaFim Hora de fim no formato HH:mm
 * @returns true se hora fim é posterior à hora início
 */
export function validarHoraFimPosterior(horaInicio: string, horaFim: string): boolean {
  if (!horaInicio || !horaFim) return true // Se não tiver ambas, não valida
  
  const [horaInicioH, horaInicioM] = horaInicio.split(':').map(Number)
  const [horaFimH, horaFimM] = horaFim.split(':').map(Number)
  const inicioMinutos = horaInicioH * 60 + horaInicioM
  const fimMinutos = horaFimH * 60 + horaFimM
  
  return fimMinutos > inicioMinutos
}

/**
 * Valida se uma URL é válida
 * @param url URL a ser validada
 * @returns true se a URL é válida
 */
export function validarURL(url: string): boolean {
  if (!url.trim()) return true // URL vazia é válida (opcional)
  
  try {
    new URL(url.trim())
    return true
  } catch {
    return false
  }
}

/**
 * Valida o formulário de agenda comercial
 * @param dados Dados do formulário a serem validados
 * @returns Objeto com erros encontrados (vazio se válido)
 */
export function validarFormularioAgenda(dados: DadosFormularioAgenda): ErrosValidacao {
  const erros: ErrosValidacao = {}

  // Título obrigatório
  if (!dados.titulo.trim()) {
    erros.titulo = 'Título é obrigatório'
  }

  // Tipo de interação obrigatório
  if (!dados.tipoInteracao || dados.tipoInteracao < 1) {
    erros.tipoInteracao = 'Tipo de interação é obrigatório'
  }

  // Data agendada obrigatória
  if (!dados.dataAgendada) {
    erros.dataAgendada = 'Data agendada é obrigatória'
  }

  // Validação condicional: início e fim obrigatórios para Reunião (1) e Presencial (5)
  const camposObrigatorios = getCamposObrigatoriosPorTipoInteracao(dados.tipoInteracao)
  if (camposObrigatorios.includes('dataInicio') && !dados.horaInicio) {
    erros.dataInicio = 'Hora de início é obrigatória para este tipo de interação'
  }
  if (camposObrigatorios.includes('dataFim') && !dados.horaFim) {
    erros.dataFim = 'Hora de fim é obrigatória para este tipo de interação'
  }

  // Validar que hora fim é posterior à hora início
  if (dados.horaInicio && dados.horaFim && dados.dataAgendada) {
    if (!validarHoraFimPosterior(dados.horaInicio, dados.horaFim)) {
      erros.dataFim = 'Hora de fim deve ser posterior à hora de início'
    }
  }

  // Cliente obrigatório
  if (!dados.clienteSelecionado) {
    erros.cliente = 'Cliente é obrigatório'
  }

  // Pelo menos um gestor externo obrigatório
  if (dados.gestoresSelecionados.length === 0) {
    erros.gestores = 'Selecione pelo menos um gestor externo'
  }

  // Validar URL do link se preenchido (apenas para Reunião)
  if (dados.tipoInteracao === 1 && dados.linkReuniao.trim()) {
    if (!validarURL(dados.linkReuniao)) {
      erros.linkReuniao = 'URL inválida'
    }
  }

  return erros
}

/**
 * Mapeia o tipo de interação de string para número
 * @param tipo Tipo de interação como string
 * @returns Número correspondente ao tipo (1=Reunião, 2=Ligação, 3=Chat, 4=Email, 5=Presencial)
 */
export function mapearTipoInteracao(tipo?: string): number {
  const map: Record<string, number> = {
    'Reunião': 1,
    'Ligação': 2,
    'Chat': 3,
    'Email': 4,
    'Presencial': 5,
  }
  return map[tipo || ''] || 1
}
