import { useState, useEffect, useMemo } from 'react'
import { container } from 'tsyringe'
import { useAppSelector } from '@app/store/hooks'
import type { HomeConfig } from '@shared/types/homeBuilder'
import { createDefaultConfig } from '@presentation/components/home-builder/defaultConfig'
import { HomeRenderer } from '@presentation/components/home-renderer/HomeRenderer'
import { ListarParametrosConfiguracaoUseCase } from '@domain/usecases/ListarParametrosConfiguracaoUseCase'
import { Skeleton } from '@/components/ui/skeleton'

const HOME_PARAMETRIZADA_CODE = 'HOME_PARAMETRIZADA'

function parseHomeConfigFromParam(valorParametro: string): HomeConfig | null {
  try {
    const parsed = JSON.parse(valorParametro) as unknown
    if (
      parsed &&
      typeof parsed === 'object' &&
      'version' in parsed &&
      Array.isArray((parsed as HomeConfig).sections)
    ) {
      return parsed as HomeConfig
    }
  } catch {
    // ignore
  }
  return null
}

export function HomeNew() {
  const token = useAppSelector((state) => state.auth.token)
  const [config, setConfig] = useState<HomeConfig>(createDefaultConfig)
  const [loading, setLoading] = useState(true)

  const listarUseCase = useMemo(() => container.resolve(ListarParametrosConfiguracaoUseCase), [])

  useEffect(() => {
    if (!token) {
      setLoading(false)
      return
    }
    let cancelled = false
    setLoading(true)
    listarUseCase
      .execute(token)
      .then((res) => {
        if (cancelled || !res?.retorno) return
        const param = res.retorno.find((p) => p.codigoParametro === HOME_PARAMETRIZADA_CODE)
        if (param) {
          const parsed = parseHomeConfigFromParam(param.valorParametro)
          if (parsed) {
            setConfig(parsed)
          }
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [token, listarUseCase])

  if (loading) {
    return (
      <div className="w-full min-w-0 p-4 sm:p-6">
        <div className="flex flex-col gap-4 sm:gap-6 min-w-0">
          {/* Banner skeleton */}
          <Skeleton className="h-40 w-full rounded-lgToken" />

          {/* Grid de colunas */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6 items-start min-w-0">
            <div className="flex flex-col gap-4 sm:gap-6 min-w-0">
              <Skeleton className="h-32 w-full rounded-lgToken" />
              <Skeleton className="h-48 w-full rounded-lgToken" />
              <Skeleton className="h-24 w-full rounded-lgToken" />
            </div>
            <div className="flex flex-col gap-4 sm:gap-6 min-w-0">
              <Skeleton className="h-64 w-full rounded-lgToken" />
              <Skeleton className="h-28 w-full rounded-lgToken" />
              <Skeleton className="h-36 w-full rounded-lgToken" />
            </div>
          </div>

          {/* Bloco full */}
          <Skeleton className="h-24 w-full rounded-lgToken" />
        </div>
      </div>
    )
  }

  return (
    <div className="w-full min-w-0 p-4 sm:p-6">
      <HomeRenderer config={config} />
    </div>
  )
}
