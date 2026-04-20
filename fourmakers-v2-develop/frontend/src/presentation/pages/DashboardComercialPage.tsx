import { useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  buscarEncontrosBigNumbers,
  buscarBigNumbersCategoria,
  selectBigNumbersCarregando,
  selectBigNumbersErro,
  selectFiltros,
} from '@app/store/slices/dashboardComercialSlice'
import { PageBreadcrumb } from '@presentation/components/common'
import {
  FiltrosComercial,
  BigNumbersComercial,
  GraficosRadarComercial,
} from '@presentation/components/dashboard-comercial'
import { Layers } from '@/components/ui/system-icons'
import { Spinner } from '@/components/ui/spinner'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'

export function DashboardComercialPage() {
  const dispatch = useAppDispatch()
  const token = useAppSelector((state) => state.auth.token)
  const filtros = useAppSelector(selectFiltros)
  const bigNumbersCarregando = useAppSelector(selectBigNumbersCarregando)
  const bigNumbersErro = useAppSelector(selectBigNumbersErro)

  useEffect(() => {
    if (token) {
      dispatch(buscarEncontrosBigNumbers({ token, filtros }))
      dispatch(buscarBigNumbersCategoria({ token, filtros }))
    }
  }, [token, dispatch, filtros])

  return (
    <div className="min-h-screen p-4 font-sans md:p-8">
      <div className="mx-auto max-w-7xl space-y-6">
        <PageBreadcrumb items={[{ label: 'Dashboard Comercial' }]} />

        <header className="mb-8 flex items-center gap-3">
          <div className="rounded-lg bg-primary p-2 shadow-md">
            <Layers className="text-primary-foreground" size={28} />
          </div>
          <div>
            <h1 className="text-3xl font-bold tracking-tight text-primary">
              Dashboard Comercial
            </h1>
            <p className="text-sm text-muted-foreground">
              Radar de relacionamento — KPIs de saúde, alcance e gráficos
            </p>
          </div>
        </header>

        {bigNumbersErro && (
          <Alert variant="destructive">
            <AlertTitle>Erro ao carregar indicadores</AlertTitle>
            <AlertDescription>{bigNumbersErro}</AlertDescription>
          </Alert>
        )}

        <FiltrosComercial />
        {bigNumbersCarregando ? (
          <div className="flex flex-col items-center justify-center gap-4 py-12">
            <Spinner size={32} className="text-primary" />
            <p className="text-sm text-muted-foreground">Carregando indicadores...</p>
          </div>
        ) : (
          <>
            <BigNumbersComercial />
            <GraficosRadarComercial />
          </>
        )}
      </div>
    </div>
  )
}
