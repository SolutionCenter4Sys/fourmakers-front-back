import { useEffect, useState } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { fetchSkillsDashboardData } from '@app/store/slices/skillsDashboardSlice'
import { PageBreadcrumb } from '@presentation/components/common'
import { Filters, BigNumbers, ChartCarousel, SkillsDashboardDataTable } from '@presentation/components/skills-dashboard'
import { Loader2, AlertCircle, LayoutDashboard, Download } from 'lucide-react'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import { container } from '@core/di/container'
import { DownloadRelatorioLogDetalhadoUseCase } from '@domain/usecases/DownloadRelatorioLogDetalhadoUseCase'

export const SkillsDashboardPage = () => {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const { status, error } = useAppSelector((state) => state.skillsDashboard)
  const [isDownloading, setIsDownloading] = useState(false)
  const [downloadError, setDownloadError] = useState<string | null>(null)

  // Extrair orgId com fallback para 0 para garantir tipo number
  const orgId = user?.colaboradorOrg?.orgId ?? 0

  useEffect(() => {
    // Só carregar dados iniciais quando status for 'idle' (primeira vez)
    // Não tentar novamente se houver erro, para evitar conflitos com fetchSkillLogsPaginated
    if (token && orgId > 0 && status === 'idle') {
      void dispatch(fetchSkillsDashboardData({ token, orgId }))
    }
  }, [dispatch, token, orgId, status])

  const handleDownloadRelatorio = async () => {
    if (!token) {
      setDownloadError('Token de autenticação não disponível')
      return
    }

    setIsDownloading(true)
    setDownloadError(null)

    try {
      const useCase = container.resolve(DownloadRelatorioLogDetalhadoUseCase)
      await useCase.execute(token)
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao baixar relatório'
      setDownloadError(errorMessage)
      console.error('[SkillsDashboardPage] Error downloading report:', err)
    } finally {
      setIsDownloading(false)
    }
  }

  return (
    <div className="min-h-screen bg-secondaryBackground p-4 md:p-8 font-sans">
      <div className="mx-auto max-w-7xl space-y-6">
        <PageBreadcrumb items={[{ label: 'Skills Dashboard' }]} />

        <header className="mb-8 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="rounded-lgToken bg-primary p-2 shadow-softToken">
              <LayoutDashboard className="text-inverseText" size={28} />
            </div>
            <div>
              <h1 className="text-3xl font-bold tracking-tight text-primaryText">Skill Dashboard</h1>
              <p className="text-sm text-secondaryText">Visão geral da evolução de skills dos colaboradores</p>
            </div>
          </div>
          <button
            onClick={handleDownloadRelatorio}
            disabled={isDownloading || !token}
            className="flex items-center gap-2 rounded-pillToken bg-primary px-4 py-2 text-sm font-medium text-inverseText transition-colors hover:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isDownloading ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin" />
                Baixando...
              </>
            ) : (
              <>
                <Download size={16} />
                Relatório
              </>
            )}
          </button>
        </header>

      {error && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Erro</AlertTitle>
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {downloadError && (
        <Alert variant="destructive">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Erro ao baixar relatório</AlertTitle>
          <AlertDescription>{downloadError}</AlertDescription>
        </Alert>
      )}

        {status === 'loading' ? (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        ) : (
          <>
            <Filters />
            <BigNumbers />
            <ChartCarousel />
            <SkillsDashboardDataTable />
          </>
        )}
      </div>
    </div>
  )
}

