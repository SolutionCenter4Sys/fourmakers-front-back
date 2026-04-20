/**
 * Objeto mínimo para decidir se pode exibir indicador de vencido (badge/linha vermelha).
 * Contratos com status null/undefined ou renovado === true NUNCA devem exibir vencido.
 */
export interface ContratoParaVencimento {
  status?: string | null
  renovado?: boolean | null
}

/**
 * Fonte única de verdade: apenas contratos com status explícito "Em andamento" e não renovados
 * podem receber indicador de vencimento (badge "Vencido", linha vermelha, etc.).
 * Status null/undefined ou renovado === true → nunca exibir vencido.
 */
export const podeExibirIndicadorVencido = (
  c: ContratoParaVencimento | null | undefined
): boolean =>
  c != null &&
  c.status != null &&
  c.status === 'Em andamento' &&
  c.renovado !== true

/**
 * Calcula o número de dias até o vencimento de um contrato
 * @param dataFim - Data de fim do contrato em formato ISO (YYYY-MM-DD)
 * @returns Número de dias até o vencimento. Negativo se já vencido, 0 se vence hoje, positivo se ainda não vencido
 */
export const calcularDiasParaVencimento = (dataFim: string): number => {
  if (!dataFim || dataFim.trim() === '') {
    return Infinity // Data inválida
  }

  // Obter data de hoje no timezone local, zerando horas/minutos/segundos/milissegundos
  const hoje = new Date()
  const hojeLocal = new Date(hoje.getFullYear(), hoje.getMonth(), hoje.getDate(), 0, 0, 0, 0)

  // Parsear data de vencimento de forma segura, evitando problemas de timezone
  let vencimento: Date
  
  // Se a string está no formato YYYY-MM-DD (sem hora), parsear manualmente
  if (/^\d{4}-\d{2}-\d{2}$/.test(dataFim.trim())) {
    const [ano, mes, dia] = dataFim.trim().split('-').map(Number)
    
    // Validar se os valores são válidos
    if (isNaN(ano) || isNaN(mes) || isNaN(dia) || mes < 1 || mes > 12 || dia < 1 || dia > 31) {
      return Infinity // Data inválida
    }
    
    // Criar data no timezone local com horas zeradas
    vencimento = new Date(ano, mes - 1, dia, 0, 0, 0, 0)
    
    // Verificar se a data é válida (ex: 31/02 não é válido)
    if (
      vencimento.getFullYear() !== ano ||
      vencimento.getMonth() !== mes - 1 ||
      vencimento.getDate() !== dia
    ) {
      return Infinity // Data inválida
    }
  } else {
    // Se não estiver no formato esperado, tentar parsear normalmente
    vencimento = new Date(dataFim)
    
    // Verificar se a data é válida
    if (isNaN(vencimento.getTime())) {
      return Infinity // Data inválida
    }
    
    // Zerar horas/minutos/segundos/milissegundos no timezone local
    vencimento = new Date(
      vencimento.getFullYear(),
      vencimento.getMonth(),
      vencimento.getDate(),
      0, 0, 0, 0
    )
  }

  // Calcular diferença em milissegundos
  const diffTime = vencimento.getTime() - hojeLocal.getTime()
  
  // Converter para dias (divisão exata, sem arredondamento)
  // Como ambas as datas têm horas zeradas, a divisão sempre será um número inteiro
  const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24))
  
  return diffDays
}

/**
 * Verifica se um contrato está na janela de alerta de vencimento (≤ 150 dias)
 * @param dataFim - Data de fim do contrato em formato ISO (YYYY-MM-DD)
 * @returns true se o contrato está na janela de alerta (0 a 150 dias), false caso contrário
 */
export const verificarAlertaVencimento = (dataFim: string): boolean => {
  const dias = calcularDiasParaVencimento(dataFim)
  return dias <= 150 && dias >= 0
}

/**
 * Retorna um indicador de vencimento para exibição em badge.
 * @param dataFim Data de fim do contrato em formato ISO (YYYY-MM-DD)
 * @returns Objeto com tipo, dias, texto e classes de cor para badge
 */
export const obterIndicadorVencimento = (dataFim: string) => {
  const dias = calcularDiasParaVencimento(dataFim)

  if (dias < 0) {
    return {
      tipo: 'vencido' as const,
      dias: Math.abs(dias),
      texto: `Vencido há ${Math.abs(dias)} dias`,
      cor: 'bg-red-500 text-white',
    }
  }

  if (dias === 0) {
    return {
      tipo: 'venceHoje' as const,
      dias: 0,
      texto: 'Vence Hoje',
      cor: 'bg-red-500 text-white',
    }
  }

  if (dias <= 30) {
    return {
      tipo: 'critico' as const,
      dias,
      texto: `${dias} dias para vencer`,
      cor: 'bg-red-100 text-red-800',
    }
  }

  if (dias <= 90) {
    return {
      tipo: 'atencao' as const,
      dias,
      texto: `${dias} dias para vencer`,
      cor: 'bg-yellow-100 text-yellow-800',
    }
  }

  return {
    tipo: 'informativo' as const,
    dias,
    texto: `${dias} dias para vencer`,
    cor: 'bg-blue-100 text-blue-800',
  }
}
