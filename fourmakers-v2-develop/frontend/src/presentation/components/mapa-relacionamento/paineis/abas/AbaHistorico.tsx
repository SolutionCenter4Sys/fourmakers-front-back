import { useEffect, useRef } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { buscarHistoricoVcx } from '@app/store/slices/vcx360Slice'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Spinner } from '@/components/ui/spinner'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { AlertCircle } from 'lucide-react'
import { getImpactBadgeColor, getUrgencyBadgeColor, getAcaoBadgeColor, getStatusColor, getImpactIcon, getUrgencyIcon, getStatusIcon, getAcaoIcon, validarStatusIniciativa } from '@shared/utils/vcxHelpers'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import type { AcaoLog } from '@domain/entities/VcxHistorico'

interface AbaHistoricoProps {
  no: NoMapaRelacionamento
}

export function AbaHistorico({ no }: AbaHistoricoProps) {
  const dispatch = useAppDispatch()
  const { historico, historicoStatus, historicoError } = useAppSelector(
    (state) => state.vcx360,
  )
  const loadedPosicaoRef = useRef<string | null>(null)

  useEffect(() => {
    if (!no.posicaoId || loadedPosicaoRef.current === no.posicaoId) {
      return
    }

    loadedPosicaoRef.current = no.posicaoId
    dispatch(buscarHistoricoVcx({ posicaoId: no.posicaoId }))
  }, [dispatch, no.posicaoId])

  if (!no.posicaoId) {
    return (
      <p className="text-sm text-muted-foreground text-center py-8">
        Posição não possui ID do backend. Não é possível buscar histórico.
      </p>
    )
  }

  if (historicoStatus === 'loading') {
    return (
      <div className="flex items-center justify-center gap-2 py-8">
        <Spinner size={16} className="text-primary" />
        <span className="text-sm text-muted-foreground">
          Carregando histórico...
        </span>
      </div>
    )
  }

  if (historicoError) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertDescription>{historicoError}</AlertDescription>
      </Alert>
    )
  }

  if (!historico || (!historico.doresLog.length && !historico.iniciativasLog.length)) {
    return (
      <p className="text-sm text-muted-foreground text-center py-8">
        Nenhum histórico disponível
      </p>
    )
  }

  // Combinar e ordenar logs por data (mais recente primeiro)
  const todosLogs = [
    ...historico.doresLog.map((log) => ({ ...log, tipo: 'dor' as const })),
    ...historico.iniciativasLog.map((log) => ({ ...log, tipo: 'iniciativa' as const })),
  ].sort(
    (a, b) =>
      new Date(b.dataAlteracao).getTime() - new Date(a.dataAlteracao).getTime(),
  )

  const getAcaoLabel = (acao: AcaoLog): string => {
    switch (acao) {
      case 'INSERT':
        return 'Criado'
      case 'UPDATE':
        return 'Atualizado'
      case 'DELETE':
        return 'Excluído'
      default:
        return acao
    }
  }

  // Função auxiliar para verificar se um valor é válido (não vazio)
  const temValor = (valor: string | null | undefined): boolean => {
    return valor !== null && valor !== undefined && typeof valor === 'string' && valor.trim() !== ''
  }

  return (
    <div className="space-y-4">
      {todosLogs.map((log) => (
        <Card key={log.id} className="border-borderSoft">
          <CardHeader className="pb-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                {(() => {
                  const AcaoIcon = getAcaoIcon(log.acao)
                  return AcaoIcon ? <AcaoIcon className="h-4 w-4" /> : null
                })()}
                <CardTitle className="text-sm font-semibold">
                  {log.tipo === 'dor' ? 'Dor' : 'Iniciativa'}:{' '}
                  {log.alteracao?.titulo || log.objeto?.titulo || 'Sem título'}
                </CardTitle>
              </div>
              {(() => {
                const AcaoIcon = getAcaoIcon(log.acao)
                const badgeClasses = getAcaoBadgeColor(log.acao)
                return (
                  <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                    {AcaoIcon && <AcaoIcon className="h-3 w-3" />}
                    {getAcaoLabel(log.acao)}
                  </Badge>
                )
              })()}
            </div>
            <p className="text-xs text-muted-foreground mt-1">
              {format(new Date(log.dataAlteracao), "dd 'de' MMMM 'de' yyyy 'às' HH:mm", {
                locale: ptBR,
              })}
            </p>
          </CardHeader>
          <CardContent className="pt-0">
            {/* Exibir detalhes baseado no tipo e ação */}
            {log.tipo === 'dor' && (
              <div className="space-y-2">
                {log.acao === 'UPDATE' && log.objeto && log.alteracao && (
                  <div className="space-y-2 text-sm">
                    <div>
                      <p className="font-medium text-muted-foreground mb-1">
                        Antes:
                      </p>
                      <p className="text-foreground">{log.objeto.titulo}</p>
                      {temValor(log.objeto.descricao) && (
                        <p className="text-xs text-muted-foreground">
                          {log.objeto.descricao}
                        </p>
                      )}
                      {(temValor(log.objeto.vcxImpactosDescricao) || temValor(log.objeto.vcxUrgenciasDescricao)) && (
                        <div className="flex gap-2 mt-1 flex-wrap">
                          {temValor(log.objeto.vcxImpactosDescricao) && (() => {
                            const ImpactIcon = getImpactIcon(log.objeto.vcxImpactosDescricao)
                            const badgeClasses = getImpactBadgeColor(log.objeto.vcxImpactosDescricao)
                            return (
                              <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                                {ImpactIcon && <ImpactIcon className="h-3 w-3" />}
                                {log.objeto.vcxImpactosDescricao}
                              </Badge>
                            )
                          })()}
                          {temValor(log.objeto.vcxUrgenciasDescricao) && (() => {
                            const UrgencyIcon = getUrgencyIcon(log.objeto.vcxUrgenciasDescricao)
                            const badgeClasses = getUrgencyBadgeColor(log.objeto.vcxUrgenciasDescricao)
                            return (
                              <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                                {UrgencyIcon && <UrgencyIcon className="h-3 w-3" />}
                                {log.objeto.vcxUrgenciasDescricao}
                              </Badge>
                            )
                          })()}
                        </div>
                      )}
                    </div>
                    <div>
                      <p className="font-medium text-muted-foreground mb-1">
                        Depois:
                      </p>
                      <p className="text-foreground">{log.alteracao.titulo}</p>
                      {temValor(log.alteracao.descricao) && (
                        <p className="text-xs text-muted-foreground">
                          {log.alteracao.descricao}
                        </p>
                      )}
                      {(temValor(log.alteracao.vcxImpactosDescricao) || temValor(log.alteracao.vcxUrgenciasDescricao)) && (
                        <div className="flex gap-2 mt-1 flex-wrap">
                          {temValor(log.alteracao.vcxImpactosDescricao) && (() => {
                            const ImpactIcon = getImpactIcon(log.alteracao.vcxImpactosDescricao)
                            const badgeClasses = getImpactBadgeColor(log.alteracao.vcxImpactosDescricao)
                            return (
                              <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                                {ImpactIcon && <ImpactIcon className="h-3 w-3" />}
                                {log.alteracao.vcxImpactosDescricao}
                              </Badge>
                            )
                          })()}
                          {temValor(log.alteracao.vcxUrgenciasDescricao) && (() => {
                            const UrgencyIcon = getUrgencyIcon(log.alteracao.vcxUrgenciasDescricao)
                            const badgeClasses = getUrgencyBadgeColor(log.alteracao.vcxUrgenciasDescricao)
                            return (
                              <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                                {UrgencyIcon && <UrgencyIcon className="h-3 w-3" />}
                                {log.alteracao.vcxUrgenciasDescricao}
                              </Badge>
                            )
                          })()}
                        </div>
                      )}
                    </div>
                  </div>
                )}
                {(log.acao === 'INSERT' || log.acao === 'DELETE') && (
                  <div className="space-y-2 text-sm">
                    <p className="font-medium">
                      {log.alteracao?.titulo || log.objeto?.titulo}
                    </p>
                    {(temValor(log.alteracao?.descricao) || temValor(log.objeto?.descricao)) && (
                      <p className="text-xs text-muted-foreground">
                        {log.alteracao?.descricao || log.objeto?.descricao}
                      </p>
                    )}
                    {(temValor((log.alteracao || log.objeto)?.vcxImpactosDescricao) || temValor((log.alteracao || log.objeto)?.vcxUrgenciasDescricao)) && (
                      <div className="flex gap-2 flex-wrap">
                        {temValor((log.alteracao || log.objeto)?.vcxImpactosDescricao) && (() => {
                          const ImpactIcon = getImpactIcon((log.alteracao || log.objeto)?.vcxImpactosDescricao ?? null)
                          const badgeClasses = getImpactBadgeColor((log.alteracao || log.objeto)?.vcxImpactosDescricao ?? null)
                          return (
                            <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken border`}>
                              {ImpactIcon && <ImpactIcon className="h-3 w-3" />}
                              {(log.alteracao || log.objeto)?.vcxImpactosDescricao}
                            </Badge>
                          )
                        })()}
                        {temValor((log.alteracao || log.objeto)?.vcxUrgenciasDescricao) && (() => {
                          const UrgencyIcon = getUrgencyIcon((log.alteracao || log.objeto)?.vcxUrgenciasDescricao ?? null)
                          const badgeClasses = getUrgencyBadgeColor((log.alteracao || log.objeto)?.vcxUrgenciasDescricao ?? null)
                          return (
                            <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken border`}>
                              {UrgencyIcon && <UrgencyIcon className="h-3 w-3" />}
                              {(log.alteracao || log.objeto)?.vcxUrgenciasDescricao}
                            </Badge>
                          )
                        })()}
                      </div>
                    )}
                  </div>
                )}
              </div>
            )}

            {log.tipo === 'iniciativa' && (
              <div className="space-y-2">
                {log.acao === 'UPDATE' && log.objeto && log.alteracao && (
                  <div className="space-y-2 text-sm">
                    <div>
                      <p className="font-medium text-muted-foreground mb-1">
                        Antes:
                      </p>
                      <p className="text-foreground">{log.objeto.titulo}</p>
                      {temValor(log.objeto.descricao) && (
                        <p className="text-xs text-muted-foreground">
                          {log.objeto.descricao}
                        </p>
                      )}
                      {(temValor(log.objeto.vcxStatusDescricao) || temValor(log.objeto.vcxTemasDescricao)) && (
                        <div className="flex gap-2 mt-1 flex-wrap">
                          {temValor(log.objeto.vcxStatusDescricao) && (() => {
                            const statusValidado = validarStatusIniciativa(log.objeto.vcxStatusDescricao)
                            const StatusIcon = statusValidado ? getStatusIcon(statusValidado) : null
                            const badgeClasses = statusValidado 
                              ? getStatusColor(statusValidado) 
                              : 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
                            return (
                              <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                                {StatusIcon && <StatusIcon className="h-3 w-3" />}
                                {log.objeto.vcxStatusDescricao}
                              </Badge>
                            )
                          })()}
                          {temValor(log.objeto.vcxTemasDescricao) && (
                            <Badge variant="outline" className="!px-3 !py-1 whitespace-nowrap">
                              {log.objeto.vcxTemasDescricao}
                            </Badge>
                          )}
                        </div>
                      )}
                    </div>
                    <div>
                      <p className="font-medium text-muted-foreground mb-1">
                        Depois:
                      </p>
                      <p className="text-foreground">{log.alteracao.titulo}</p>
                      {temValor(log.alteracao.descricao) && (
                        <p className="text-xs text-muted-foreground">
                          {log.alteracao.descricao}
                        </p>
                      )}
                      {(temValor(log.alteracao.vcxStatusDescricao) || temValor(log.alteracao.vcxTemasDescricao)) && (
                        <div className="flex gap-2 mt-1 flex-wrap">
                          {temValor(log.alteracao.vcxStatusDescricao) && (() => {
                            const statusValidado = validarStatusIniciativa(log.alteracao.vcxStatusDescricao)
                            const StatusIcon = statusValidado ? getStatusIcon(statusValidado) : null
                            const badgeClasses = statusValidado 
                              ? getStatusColor(statusValidado) 
                              : 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
                            return (
                              <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                                {StatusIcon && <StatusIcon className="h-3 w-3" />}
                                {log.alteracao.vcxStatusDescricao}
                              </Badge>
                            )
                          })()}
                          {temValor(log.alteracao.vcxTemasDescricao) && (
                            <Badge variant="outline" className="!px-3 !py-1 whitespace-nowrap">
                              {log.alteracao.vcxTemasDescricao}
                            </Badge>
                          )}
                        </div>
                      )}
                    </div>
                  </div>
                )}
                {(log.acao === 'INSERT' || log.acao === 'DELETE') && (
                  <div className="space-y-2 text-sm">
                    <p className="font-medium">
                      {log.alteracao?.titulo || log.objeto?.titulo}
                    </p>
                    {(temValor(log.alteracao?.descricao) || temValor(log.objeto?.descricao)) && (
                      <p className="text-xs text-muted-foreground">
                        {log.alteracao?.descricao || log.objeto?.descricao}
                      </p>
                    )}
                    {(temValor((log.alteracao || log.objeto)?.vcxStatusDescricao) || temValor((log.alteracao || log.objeto)?.vcxTemasDescricao)) && (
                      <div className="flex gap-2 flex-wrap">
                        {temValor((log.alteracao || log.objeto)?.vcxStatusDescricao) && (() => {
                          const statusValidado = validarStatusIniciativa((log.alteracao || log.objeto)?.vcxStatusDescricao)
                          const StatusIcon = statusValidado ? getStatusIcon(statusValidado) : null
                          const badgeClasses = statusValidado 
                            ? getStatusColor(statusValidado) 
                            : 'bg-muted/10 text-muted-foreground border border-muted !border-muted dark:bg-muted/20 dark:text-muted-foreground dark:border-muted dark:!border-muted'
                          return (
                            <Badge className={`${badgeClasses} !px-3 !py-1 whitespace-nowrap flex items-center gap-1.5 rounded-pillToken`}>
                              {StatusIcon && <StatusIcon className="h-3 w-3" />}
                              {(log.alteracao || log.objeto)?.vcxStatusDescricao}
                            </Badge>
                          )
                        })()}
                        {temValor((log.alteracao || log.objeto)?.vcxTemasDescricao) && (
                          <Badge variant="outline" className="!px-3 !py-1 whitespace-nowrap">
                            {(log.alteracao || log.objeto)?.vcxTemasDescricao}
                          </Badge>
                        )}
                      </div>
                    )}
                  </div>
                )}
              </div>
            )}
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
