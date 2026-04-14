import { useState } from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { FileText, Download, Loader2, ArrowRight } from 'lucide-react'
import { useAppSelector } from '@app/store/hooks'
import { useToast } from '@/hooks/use-toast'
import { container } from '@core/di/container'
import { GerarRelatorioColaboradoresUseCase } from '@domain/usecases/GerarRelatorioColaboradoresUseCase'

interface ExportarRelatorioModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
}

export const ExportarRelatorioModal = ({
  open,
  onOpenChange,
}: ExportarRelatorioModalProps) => {
  const { token } = useAppSelector((state) => state.auth)
  const { toast } = useToast()
  const [isExporting, setIsExporting] = useState(false)

  const handleExportarRelatorio = async () => {
    if (!token) {
      toast({
        title: 'Erro',
        description: 'Token de autenticação não encontrado',
        variant: 'destructive',
      })
      return
    }

    setIsExporting(true)

    try {
      const useCase = container.resolve(GerarRelatorioColaboradoresUseCase)
      const result = await useCase.execute(token)

      // Criar blob e fazer download
      const blob = new Blob([result.arrayBuffer], { type: result.contentType })
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.setAttribute('download', result.fileName)
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)

      toast({
        title: 'Sucesso',
        description: 'Relatório exportado com sucesso!',
      })

      onOpenChange(false)
    } catch (error) {
      console.error('Erro ao exportar relatório:', error)
      toast({
        title: 'Erro',
        description: error instanceof Error ? error.message : 'Falha ao exportar relatório',
        variant: 'destructive',
      })
    } finally {
      setIsExporting(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-3 bg-primary/10 rounded-lg">
              <FileText className="h-8 w-8 text-primary" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold">
                Exportar Relatório
              </DialogTitle>
              <DialogDescription className="mt-1">
                Tem certeza que deseja exportar?
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <Card className="border-primary/20 bg-primary/5">
            <CardContent className="pt-6">
              <div className="space-y-4">
                <div className="flex items-start gap-3">
                  <div className="p-2 bg-primary/10 rounded-lg mt-1">
                    <Download className="h-5 w-5 text-primary" />
                  </div>
                  <div className="flex-1 space-y-1">
                    <p className="text-sm text-muted-foreground">
                      O relatório será gerado e baixado automaticamente em seu dispositivo.
                    </p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        <DialogFooter>
          <Button 
            variant="outline" 
            onClick={() => onOpenChange(false)}
            disabled={isExporting}
          >
            Fechar
          </Button>
          <Button 
            onClick={handleExportarRelatorio} 
            className="gap-2"
            disabled={isExporting}
          >
            {isExporting ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin" />
                Exportando...
              </>
            ) : (
              <>
                Exportar
                <ArrowRight className="h-4 w-4" />
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}

