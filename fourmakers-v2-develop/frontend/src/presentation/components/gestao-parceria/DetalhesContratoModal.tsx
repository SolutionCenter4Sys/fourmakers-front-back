import type { Contrato } from '@domain/entities/Contrato'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { FileText, Download } from '@/components/ui/system-icons'
import { AlertCircle, AlertTriangle } from '@/components/ui/system-icons'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale/pt-BR'
import { obterIndicadorVencimento } from '@shared/utils/contratoHelpers'

interface DetalhesContratoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  contrato: Contrato | null
  onDownloadArquivo?: (url: string, nome: string) => void
}

export const DetalhesContratoModal = ({
  open,
  onOpenChange,
  contrato,
  onDownloadArquivo,
}: DetalhesContratoModalProps) => {
  const formatarData = (data: string) => {
    try {
      if (!data) return '-'
      // Extrair apenas a parte da data (sem hora)
      const dataPart = data.includes('T') ? data.split('T')[0] : data.includes(' ') ? data.split(' ')[0] : data

      // YYYY-MM-DD -> criar Date local evitando parsing UTC
      const isoMatch = dataPart.match(/^(\d{4})-(\d{2})-(\d{2})$/)
      if (isoMatch) {
        const y = parseInt(isoMatch[1], 10)
        const m = parseInt(isoMatch[2], 10)
        const d = parseInt(isoMatch[3], 10)
        return format(new Date(y, m - 1, d), 'dd/MM/yyyy', { locale: ptBR })
      }

      // DD/MM/YYYY
      const brMatch = dataPart.match(/^(\d{2})\/(\d{2})\/(\d{4})$/)
      if (brMatch) {
        const d = parseInt(brMatch[1], 10)
        const m = parseInt(brMatch[2], 10)
        const y = parseInt(brMatch[3], 10)
        return format(new Date(y, m - 1, d), 'dd/MM/yyyy', { locale: ptBR })
      }

      // Fallback
      return format(new Date(data), 'dd/MM/yyyy', { locale: ptBR })
    } catch {
      return '-'
    }
  }

  const formatarValor = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor)
  }

  if (!contrato) return null

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-2 bg-primary/10 rounded-lg">
              <FileText className="h-6 w-6 text-primary" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold">
                Detalhes do Contrato
              </DialogTitle>
              <DialogDescription className="mt-1">
                {contrato.contrato || 'Contrato sem nome'}
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <Card>
            <CardHeader className="flex items-center justify-between">
              <CardTitle>Informações Básicas</CardTitle>
              {/* Status null: não acionar flag vencido/próximo a vencimento. Apenas "Em andamento" explícito. */}
              {contrato.dataFim && contrato.status === 'Em andamento' && !contrato.renovado && (() => {
                try {
                  const indicador = obterIndicadorVencimento(contrato.dataFim)
                  const Icon = indicador.tipo === 'vencido' || indicador.tipo === 'venceHoje' ? AlertCircle : AlertTriangle
                  return (
                    <div className="flex items-center gap-2">
                      <span className={`px-3 py-1 rounded-full text-sm font-medium ${indicador.cor} flex items-center gap-2`}>
                        <Icon className="h-4 w-4" />
                        <span>{indicador.texto}</span>
                      </span>
                    </div>
                  )
                } catch {
                  return null
                }
              })()}
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-muted-foreground">Nome do Contrato</p>
                  <p className="text-sm font-medium">{contrato.contrato || '-'}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Status</p>
                  <Badge variant="secondary">{contrato.status ?? 'Em andamento'}</Badge>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Data de Início</p>
                  <p className="text-sm font-medium">{formatarData(contrato.dataInicio)}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Data de Fim</p>
                  <p className="text-sm font-medium">{formatarData(contrato.dataFim)}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Valor do Contrato</p>
                  <p className="text-sm font-medium">{formatarValor(contrato.valorContrato)}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Plataforma de Assinatura</p>
                  <p className="text-sm font-medium">{contrato.plataformaAssinatura || '-'}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Reajuste Anual</p>
                  <p className="text-sm font-medium">
                    {typeof contrato.reajusteAnual === 'string' 
                      ? contrato.reajusteAnual || '-'
                      : '-'}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Número de Páginas</p>
                  <p className="text-sm font-medium">{contrato.numeroPaginas || '-'}</p>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Referência de Cotação</p>
                  <p className="text-sm font-medium">{contrato.referenciaCotacao || '-'}</p>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Status e Configurações</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-3 gap-4">
                <div>
                  <p className="text-sm text-muted-foreground">Contrato Assinado</p>
                  <Badge variant={contrato.contratoAssinado ? 'default' : 'secondary'}>
                    {contrato.contratoAssinado ? 'Sim' : 'Não'}
                  </Badge>
                </div>
                <div>
                  <p className="text-sm text-muted-foreground">Renovado</p>
                  <Badge variant={contrato.renovado ? 'default' : 'secondary'}>
                    {contrato.renovado ? 'Sim' : 'Não'}
                  </Badge>
                </div>
              </div>
            </CardContent>
          </Card>

          {contrato.clausulaPenalidades && (
            <Card>
              <CardHeader>
                <CardTitle>Cláusula de Penalidades</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-sm whitespace-pre-wrap">{contrato.clausulaPenalidades}</p>
              </CardContent>
            </Card>
          )}

          {contrato.emailsNotificacao && contrato.emailsNotificacao.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Emails de Notificação</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-1">
                  {contrato.emailsNotificacao.map((email, index) => (
                    <p key={index} className="text-sm">{email}</p>
                  ))}
                </div>
              </CardContent>
            </Card>
          )}

          {contrato.arquivo && (
            <Card>
              <CardHeader>
                <CardTitle>Arquivo Anexado</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center gap-3 p-3 bg-muted rounded-lg">
                  <FileText className="h-5 w-5 text-muted-foreground" />
                  <div className="flex-1">
                    <p className="text-sm font-medium">{contrato.arquivo.nomeArquivo}</p>
                    <p className="text-xs text-muted-foreground">
                      {formatarData(contrato.arquivo.dataUpload)}
                    </p>
                  </div>
                  {onDownloadArquivo && (
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() =>
                        onDownloadArquivo(contrato.arquivo!.urlDownload, contrato.arquivo!.nomeArquivo)
                      }
                    >
                      <Download className="h-4 w-4 mr-2" />
                      Baixar
                    </Button>
                  )}
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Fechar
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
