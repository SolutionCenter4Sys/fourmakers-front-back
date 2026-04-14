import { useEffect, useRef } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { selectFilteredData, fetchSkillLogsPaginated, setPage, setItemsPerPage } from '@app/store/slices/skillsDashboardSlice'
import { DataTable, type Column } from '@presentation/components/common'
import { TablePagination } from '@presentation/components/common/TablePagination'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { AlertCircle, RefreshCw, ChevronLeft } from 'lucide-react'
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert'
import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'
import { useDragScroll } from '@presentation/hooks/useDragScroll'
import { getEventLabel, getEventBadgeStyle } from '@shared/utils/skillsDashboardUtils'

export const SkillsDashboardDataTable = () => {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const data = useAppSelector((state) => selectFilteredData(state))
  const { pagination, status, error } = useAppSelector((state) => state.skillsDashboard)
  const orgId = user?.colaboradorOrg?.orgId
  const lastParamsRef = useRef<{ token: string; orgId: number; limit: number; cursor: number } | null>(null)
  const isInitialMountRef = useRef(true)
  const dragScrollRef = useDragScroll<HTMLDivElement>()

  const handleRetry = () => {
    if (token && orgId && orgId > 0) {
      // Resetar o ref para permitir nova tentativa
      lastParamsRef.current = null
      void dispatch(fetchSkillLogsPaginated({
        token,
        orgId,
        limit: pagination.itemsPerPage,
        cursor: pagination.cursor,
      }))
    }
  }

  useEffect(() => {
    if (!token || !orgId || orgId <= 0) {
      return
    }

    const currentParams = {
      token,
      orgId,
      limit: pagination.itemsPerPage,
      cursor: pagination.cursor,
    }

    // Verificar se os parâmetros mudaram para evitar chamadas duplicadas
    const lastParams = lastParamsRef.current
    const paramsChanged = !lastParams ||
      lastParams.token !== currentParams.token ||
      lastParams.orgId !== currentParams.orgId ||
      lastParams.limit !== currentParams.limit ||
      lastParams.cursor !== currentParams.cursor

    // Se os parâmetros não mudaram, não fazer nova chamada
    // Esta é a verificação PRINCIPAL que previne loops infinitos
    if (!paramsChanged) {
      return
    }

    // Não fazer chamada se já estiver carregando paginação (evita chamadas concorrentes)
    if (pagination.isLoading) {
      // Mas atualizar o ref para evitar que seja executado novamente quando loading terminar
      lastParamsRef.current = currentParams
      return
    }

    // Na primeira montagem (initial load), a página principal já carrega os dados
    // Não fazer chamada se for a primeira vez e cursor é 0
    if (isInitialMountRef.current) {
      if (pagination.cursor === 0) {
        // Primeira vez com cursor 0: página principal já carregou
        isInitialMountRef.current = false
        lastParamsRef.current = currentParams
        return
      }
      // Primeira vez mas cursor > 0: pode fazer chamada
      isInitialMountRef.current = false
    }

    // Atualizar referência dos parâmetros ANTES de fazer a chamada
    // Isso garante que mesmo se houver erro, não tentaremos novamente com os mesmos parâmetros
    lastParamsRef.current = currentParams
    void dispatch(fetchSkillLogsPaginated(currentParams))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dispatch, token, orgId, pagination.itemsPerPage, pagination.cursor, pagination.isLoading])

  const columns: Column[] = [
    { id: 'nome', label: 'Nome', sortable: true },
    { id: 'cliente', label: 'Cliente', sortable: true },
    { id: 'perfil', label: 'Perfil', sortable: true },
    { id: 'skill', label: 'Skill', sortable: true },
    { id: 'tipoSkill', label: 'Tipo', sortable: true },
    { id: 'senioridade', label: 'Senioridade', sortable: true },
    { id: 'evento', label: 'Evento', sortable: true },
  ]

  /**
   * Mapeia valores de tipoSkill do backend para textos legíveis
   */
  const getTipoSkillLabel = (tipoSkill: string): string => {
    if (!tipoSkill) return ''
    
    // Normalizar o tipoSkill removendo espaços e convertendo para uppercase para comparação
    const normalized = tipoSkill.trim().toUpperCase()
    
    const tipoSkillMap: Record<string, string> = {
      'COMPETENCIA': 'Hard Skill',
      'HARDSKILL': 'Hard Skill',
      'HARD_SKILL': 'Hard Skill',
      'SOFTSKILL': 'Soft Skill',
      'SOFT_SKILL': 'Soft Skill',
      'DOMINIONEGOCIO': 'Domínio de Negócio',
      'DOMINIO_NEGOCIO': 'Domínio de Negócio',
      'DOMINIO NEGOCIO': 'Domínio de Negócio',
      'METODOLOGIA': 'Metodologia',
      'IDIOMA': 'Idioma',
      'IDIOMAS': 'Idioma',
    }
    
    // Tentar primeiro com o valor original (case-sensitive), depois com o normalizado
    if (tipoSkillMap[tipoSkill]) {
      return tipoSkillMap[tipoSkill]
    }
    
    // Tentar com o valor normalizado (uppercase)
    if (tipoSkillMap[normalized]) {
      return tipoSkillMap[normalized]
    }
    
    // Se não encontrou, retornar o valor original
    return tipoSkill
  }


  const renderCell = (item: SkillLogEntry, columnId: string) => {
    switch (columnId) {
      case 'nome':
        return <span className="font-medium">{item.nome ?? '—'}</span>
      case 'cliente':
        return item.cliente ?? '—'
      case 'perfil':
        return item.perfil ?? '—'
      case 'skill':
        return <span className="font-semibold">{item.skill ?? '—'}</span>
      case 'tipoSkill':
        return <Badge variant="outline">{getTipoSkillLabel(item.tipoSkill ?? '')}</Badge>
      case 'senioridade':
        return item.senioridade ?? '—'
      case 'evento': {
        // Converter explicitamente para string para garantir que o mapeamento funcione
        const eventoStr = String(item.evento ?? '')
        return (
          <span
            className={`inline-flex items-center justify-center rounded-pillToken border px-2.5 py-1 text-xs font-semibold text-center whitespace-normal ${getEventBadgeStyle(eventoStr)}`}
          >
            {getEventLabel(eventoStr)}
          </span>
        )
      }
      default:
        return null
    }
  }

  return (
    <Card id="data-table" className="scroll-mt-6 overflow-hidden rounded-lgToken border-borderSoft shadow-softToken">
      <CardHeader className="border-b border-borderDefault bg-surfaceSubtle p-lg">
        <CardTitle className="text-lg font-bold text-primaryText">Logs de Ações</CardTitle>
      </CardHeader>
      <CardContent>
        {pagination.isLoading ? (
          <div className="min-h-[500px]">
            <div className="standard-table-wrapper">
              <div ref={dragScrollRef} className="relative w-full overflow-x-auto overflow-y-auto max-h-[600px]" style={{ minWidth: 0 }}>
                <div className="min-w-max">
                  <table className="w-full caption-bottom text-sm">
                  <thead className="sticky top-0 z-20 bg-surfaceElevated border-b border-borderDefault shadow-sm">
                    <tr>
                      {columns.map((column) => (
                        <th
                          key={column.id}
                          className="h-12 px-4 text-left align-middle font-medium text-muted-foreground"
                        >
                          <div className="h-4 w-24 bg-muted animate-pulse rounded" />
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {Array.from({ length: pagination.itemsPerPage }).map((_, rowIndex) => (
                      <tr key={rowIndex} className="border-b border-borderDefault/50">
                        {columns.map((column, colIndex) => (
                          <td key={column.id} className="h-14 px-4 align-middle">
                            <div
                              className={`h-4 bg-muted animate-pulse rounded ${
                                colIndex === 0
                                  ? 'w-32'
                                  : colIndex === 3
                                    ? 'w-40'
                                    : colIndex === 6
                                      ? 'w-28'
                                      : 'w-24'
                              }`}
                            />
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
                </div>
              </div>
            </div>
          </div>
        ) : status === 'failed' && error ? (
          <div className="space-y-4 py-8">
            <Alert variant="destructive">
              <AlertCircle className="h-4 w-4" />
              <AlertTitle>Erro ao carregar logs</AlertTitle>
              <AlertDescription>{error}</AlertDescription>
            </Alert>
            <div className="flex justify-center">
              <button
                onClick={handleRetry}
                className="flex items-center gap-2 rounded-pillToken bg-primary px-4 py-2 text-sm font-medium text-inverseText transition-colors hover:bg-primary/90"
              >
                <RefreshCw size={16} />
                Tentar novamente
              </button>
            </div>
          </div>
        ) : (
          <>
            {data.length === 0 && pagination.cursor > 0 ? (
              <div className="space-y-4 py-8">
                <div className="text-center">
                  <p className="text-muted-foreground mb-4">Nenhum dado encontrado nesta página.</p>
                  <button
                    onClick={() => dispatch(setPage(1))}
                    className="flex items-center gap-2 rounded-pillToken bg-primary px-4 py-2 text-sm font-medium text-inverseText transition-colors hover:bg-primary/90 mx-auto"
                  >
                    <ChevronLeft size={16} />
                    Voltar para primeira página
                  </button>
                </div>
              </div>
            ) : (
              <>
                <div 
                  ref={dragScrollRef}
                  className="relative w-full overflow-x-auto overflow-y-auto max-h-[600px]"
                  style={{ minWidth: 0 }}
                >
                  <div className="min-w-max">
                    <DataTable
                      columns={columns}
                      data={data}
                      keyExtractor={(item) => String(item.id ?? '')}
                      renderCell={renderCell}
                      emptyMessage="Nenhum dado encontrado. Tente ajustar os filtros acima para ver resultados."
                    />
                  </div>
                </div>
                {(data.length > 0 || pagination.hasMore) && (
                  <TablePagination
                    currentPage={pagination.currentPage}
                    totalItems={
                      data.length > 0
                        ? // Se tem dados, calcular total considerando hasMore
                          pagination.hasMore
                          ? // Se há mais dados, garantir que totalItems permita próxima página
                            Math.max(data.length + 1, (pagination.currentPage + 1) * pagination.itemsPerPage)
                          : // Se não há mais dados, usar apenas o que temos
                            data.length
                        : // Se não tem dados mas hasMore é true, permitir próxima página
                          pagination.hasMore
                          ? (pagination.currentPage + 1) * pagination.itemsPerPage
                          : 0
                    }
                    itemsPerPage={pagination.itemsPerPage}
                    hasMore={pagination.hasMore}
                    onPageChange={(page) => {
                      dispatch(setPage(page))
                    }}
                    onItemsPerPageChange={(items) => {
                      dispatch(setItemsPerPage(Number(items)))
                    }}
                  />
                )}
              </>
            )}
          </>
        )}
      </CardContent>
    </Card>
  )
}

