import { useEffect } from 'react'
import { useLocation } from 'react-router-dom'
import { useSelector } from 'react-redux'
import type { RootState } from '@app/store'
import { logPageView } from '@shared/utils/firebaseAnalytics'

/**
 * Mapeamento de rotas para títulos de página mais descritivos
 */
const PAGE_TITLES: Record<string, string> = {
  '/dashboard': 'Dashboard',
  '/homeDashboard': 'Dashboard',
  '/profile360': 'Perfil 360',
  '/personal-data': 'Dados Pessoais',
  '/_react/timesheet': 'Timesheet (React)',
  '/timesheet': 'Timesheet',
  '/colaboradores': 'Colaboradores',
  '/colaboradores/novo': 'Novo Colaborador',
  '/book-colaborador': 'Book de Colaborador',
  '/gestao-desempenho-gestor': 'Gestão de Desempenho - Gestor',
  '/gestao-desempenho-colaborador': 'Gestão de Desempenho - Colaborador',
  '/gestao-desempenho-rh': 'Gestão de Desempenho - RH',
  '/gestao-desempenho-rh/parametrizacao': 'Parametrização de Desempenho',
  '/recrutamento/parametrizacao': 'Parametrização de Recrutamento',
  '/recrutamento/dashboard': 'Dashboard de Recrutamento',
  '/recrutamento/candidaturasfmu': 'Candidaturas da FMU',
  '/recrutamento/historico-candidato': 'Histórico de movimentações da candidatura',
  '/reembolso': 'Reembolso',
  '/reembolso/aprovar': 'Aprovar Reembolso',
  '/reembolso/remessa-cnab': 'Remessa CNAB',
  '/inserir-reembolso': 'Inserir Reembolso',
  '/reembolso-parametros': 'Parâmetros de Reembolso',
  '/mapa-alocacao': 'Mapa de Alocação',
  '/mapa-alocacao/nova': 'Nova Alocação',
  '/projetos': 'Projetos',
  '/integracao-folha-ponto': 'Integração Folha de Ponto',
  '/integracao-contabil': 'Integração Contábil',
  '/conciliacao-folha-pagamento': 'Conciliação Folha de Pagamento',
  '/rubricas': 'Rubricas',
  '/meu-faturamento': 'Meu Faturamento',
  '/gestao-notas-fiscais': 'Gestão de Notas Fiscais',
  '/meu-holerite': 'Meu Holerite',
  '/documentacao': 'Documentação',
  '/recrutamento/perfil': 'Perfil de Atuação',
  '/recrutamento/candidatos': 'Candidatos - Gestão de Vagas',
  '/recrutamento/relatorios': 'Relatórios de Vagas',
  '/criarPerfilAtuacao': 'Perfil de Atuação',
  '/simulador': 'Simulador',
  '/pdijornada': 'Minha Jornada - PDI',
  '/atualizar-perfil': 'Atualizar Perfil',
  '/dashboard_jornadas': 'Dashboard de Skills',
  '/recrutamento': 'Recrutamento',
  '/gestao/parceria': 'Gestão de Parcerias',
  '/agendas-comerciais': 'Agendas Comerciais',
  '/dashboard-comercial': 'Dashboard Comercial',
  '/login': 'Login',
  '/login2': 'Login Numen',
  '/loginfmu': 'Login FMU',
  '/sso': 'Login SSO',
  '/public/vaga': 'Candidatura externa - Vaga',
}

/**
 * Obtém o título da página baseado no pathname
 * @param pathname Caminho da página
 * @returns Título da página
 */
const getPageTitle = (pathname: string): string => {
  // Verifica se é uma rota exata
  if (PAGE_TITLES[pathname]) {
    return PAGE_TITLES[pathname]
  }

  // Trata rotas com parâmetros dinâmicos
  if (pathname.startsWith('/colaboradores/editar/')) {
    return 'Editar Colaborador'
  }
  if (pathname.startsWith('/book-colaborador/')) {
    return 'Detalhes do Colaborador'
  }
  if (pathname.startsWith('/gestao-desempenho-gestor/')) {
    return 'Gestão de Desempenho - Detalhes do Colaborador'
  }
  if (pathname.startsWith('/gestao-desempenho-rh/colaborador/')) {
    return 'Gestão de Desempenho - RH - Perfil do Colaborador'
  }
  if (pathname.startsWith('/page/')) {
    // Para páginas FlutterFlow, extrai o nome da rota
    const pageName = pathname.replace('/page/', '').split('/')[0]
    return `FlutterFlow - ${pageName}`
  }
  if (pathname.startsWith('/login/')) {
    const orgSlug = pathname.replace('/login/', '')
    return `Login - ${orgSlug}`
  }
  if (pathname.startsWith('/recrutamento/template-contratacao')) {
    return 'Template de Contratação'
  }
  if (pathname.startsWith('/public/vaga')) {
    return 'Candidatura externa - Vaga'
  }

  // Fallback: capitaliza a última parte do caminho
  const parts = pathname.split('/').filter(Boolean)
  if (parts.length > 0) {
    const lastPart = parts[parts.length - 1]
    return lastPart.charAt(0).toUpperCase() + lastPart.slice(1).replace(/-/g, ' ')
  }

  return 'Página'
}

/**
 * Hook para rastrear visualizações de página no Firebase Analytics
 * Registra automaticamente um evento page_view sempre que a rota mudar
 */
export const usePageTracking = (): void => {
  const location = useLocation()
  const user = useSelector((state: RootState) => state.auth.user)

  useEffect(() => {
    // Registra a visualização da página
    const pathname = location.pathname
    const pageTitle = getPageTitle(pathname)
    
    logPageView(pathname, pageTitle, user)
  }, [location, user])
}
