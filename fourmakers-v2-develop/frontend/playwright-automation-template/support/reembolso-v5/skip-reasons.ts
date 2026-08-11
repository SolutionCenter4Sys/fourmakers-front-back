export type MassaCenarioV5 = {
  id: string
  tipo: string
  tela: string
  titulo: string
  preCondicoes?: Record<string, unknown>
  ui?: Record<string, unknown> | null
  expectativa?: Record<string, unknown> | null
}

export const CENARIOS_EXECUTAVEIS_V5 = new Set([
  'RD-01',
  'RD-03',
  'RD-08',
  'RD-10',
  'RD-11',
  'RD-14',
  'AP-08',
  'IR-11',
  'IR-14',
  'IR-15',
  'IR-18',
])

export function obterMotivoSkipV5(massa: MassaCenarioV5): string | null {
  if (CENARIOS_EXECUTAVEIS_V5.has(massa.id)) return null

  const perfil = massa.preCondicoes?.perfil as string | undefined
  if (perfil === 'gestor') return 'requer perfil gestor (usuário OTP padrão é colaborador)'
  if (perfil === 'aprovador') return 'requer perfil aprovador'
  if (perfil === 'admin') return 'requer perfil administrador de parâmetros'

  if (massa.tela.includes('Remessa CNAB')) {
    return 'requer perfil gestor, diretoria e remessa CNAB habilitada'
  }
  if (massa.tela.includes('Aprovar Reembolso')) {
    return 'requer perfil aprovador e pendências cadastradas'
  }
  if (massa.tela.startsWith('Reembolso Parâmetros')) {
    return 'requer acesso autenticado a /reembolso-parametros (perfil admin)'
  }
  if (massa.tela.includes('Gestão ADM')) {
    return 'requer perfil gestor na aba Gestão ADM'
  }
  if (massa.tela.includes('Aprovações')) {
    return 'requer perfil aprovador na aba Aprovações'
  }

  if (massa.id.startsWith('IR-')) {
    return 'requer verbas e projetos configurados para o usuário OTP'
  }

  if (['RD-02', 'RD-04', 'RD-05', 'RD-06', 'RD-07', 'RD-09', 'RD-12', 'RD-13', 'RD-15'].includes(massa.id)) {
    return 'requer solicitações cadastradas, perfil específico ou volume de dados no backend'
  }

  return 'depende de setup não disponível para o usuário OTP padrão da automação'
}
