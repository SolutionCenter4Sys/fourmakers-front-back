import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMinhaEquipeViewModel } from '@presentation/hooks/useMinhaEquipeViewModel'
import { isDevelopment } from '@shared/utils/envUtils'
import { PageHeader, PageBreadcrumb } from '@presentation/components/common'
import { PDITabs } from '@presentation/components/pdi-tabs'
import {
  EquipeKPICards,
  EquipeFiltros,
  EquipeTable,
  RadarModal,
} from '@presentation/components/minha-equipe'
import { Loader2, AlertCircle } from '@/components/ui/system-icons'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'

const OrquestracaoPage = () => {
  const navigate = useNavigate()

  // MainLayout já carrega o perfil do usuário, não precisamos fazer aqui

  const {
    colaboradores,
    kpis,
    isLoading,
    error,
    searchQuery,
    setSearchQuery,
    clienteFilter,
    setClienteFilter,
    gestorFilter,
    setGestorFilter,
    clientesUnicos,
    gestoresUnicos,
    filtrosResetKey,
    handleLimparFiltros,
    radarModalOpen,
    setRadarModalOpen,
    selectedColaborador,
    sugestoes,
    pdiMetas,
    loadingRadar,
    handleOpenRadar,
    handleAprovarSugestao,
    handleRejeitarSugestao,
  } = useMinhaEquipeViewModel(true) // true = orquestração

  // Redirecionar para dashboard em caso de erro (apenas em produção e homolog)
  useEffect(() => {
    if (error && !isLoading && !isDevelopment()) {
      navigate('/dashboard')
    }
  }, [error, isLoading, navigate])

  return (
    <div className="container mx-auto p-4 space-y-6 pb-40">
      <PageBreadcrumb items={[{ label: 'Orquestração' }]} />

      <PDITabs currentTab="orquestracao" />

      <PageHeader
        title="PDI - Orquestração"
        description="Visão estratégica do desenvolvimento organizacional"
      />

      {error && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {isLoading ? (
        <div className="flex items-center justify-center py-20">
          <Loader2 className="w-8 h-8 animate-spin text-primary" />
        </div>
      ) : (
        <>
          {/* KPI Cards */}
          <EquipeKPICards kpis={kpis} isLoading={isLoading} />

          {/* Filtros */}
          <EquipeFiltros
            searchQuery={searchQuery}
            onSearchChange={setSearchQuery}
            clienteFilter={clienteFilter}
            onClienteChange={setClienteFilter}
            gestorFilter={gestorFilter}
            onGestorChange={setGestorFilter}
            clientesUnicos={clientesUnicos}
            gestoresUnicos={gestoresUnicos}
            filtrosResetKey={filtrosResetKey}
            onLimparFiltros={handleLimparFiltros}
          />

          {/* Tabela */}
          <EquipeTable
            colaboradores={colaboradores}
            onOpenRadar={handleOpenRadar}
            isLoading={isLoading}
          />
        </>
      )}

      <RadarModal
        open={radarModalOpen}
        onOpenChange={setRadarModalOpen}
        colaborador={selectedColaborador}
        sugestoes={sugestoes}
        pdiMetas={pdiMetas}
        loadingRadar={loadingRadar}
        onAprovarSugestao={handleAprovarSugestao}
        onRejeitarSugestao={handleRejeitarSugestao}
      />
    </div>
  )
}

export default OrquestracaoPage
