import { useState } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { InserirArquivoParceiroUseCase } from '@domain/usecases/InserirArquivoParceiroUseCase'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { toast } from 'sonner'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Upload } from '@/components/ui/system-icons'
import { FileUpload } from './FileUpload'

interface UploadArquivoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  contratoId: string
  parceiroId: string
  onSuccess?: () => void
}

export const UploadArquivoModal = ({
  open,
  onOpenChange,
  contratoId,
  parceiroId,
  onSuccess,
}: UploadArquivoModalProps) => {
  const { token, user } = useAppSelector((state) => state.auth)
  const [arquivo, setArquivo] = useState<File | null>(null)
  const [uploading, setUploading] = useState(false)

  const inserirArquivoUseCase = container.resolve(InserirArquivoParceiroUseCase)

  const handleFileChange = (file: File | null) => {
    if (file) {
      // Validar tipo (apenas PDF)
      if (file.type !== 'application/pdf') {
        toast.error('Apenas arquivos PDF são permitidos')
        return
      }
      // Validar tamanho (20MB máximo)
      if (file.size > 20 * 1024 * 1024) {
        toast.error('Arquivo muito grande. Tamanho máximo: 20MB')
        return
      }
    }
    setArquivo(file)
  }

  const handleUpload = async () => {
    if (!arquivo || !token || !user) return

    try {
      setUploading(true)

      await inserirArquivoUseCase.execute(token, {
        parceiroId,
        arquivoTipoId: 1, // 1=PDF
        arquivoOriginId: 1, // Origem padrão
        parceiroGestaoContratoId: contratoId,
        file: arquivo,
      })

      toast.success('Arquivo enviado com sucesso!')
      
      if (user) {
        logUserAction('GestaoParceria', 'UploadArquivo', {
          contratoId,
          parceiroId,
        }, user)
      }

      // Limpar estado
      setArquivo(null)
      onOpenChange(false)
      onSuccess?.()
    } catch (error) {
      console.error('Erro ao fazer upload:', error)
      toast.error('Erro ao fazer upload do arquivo')
    } finally {
      setUploading(false)
    }
  }

  const handleClose = () => {
    if (!uploading) {
      setArquivo(null)
      onOpenChange(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-2 bg-primary/10 rounded-lg">
              <Upload className="h-6 w-6 text-primary" />
            </div>
            <div>
              <DialogTitle className="text-2xl font-bold">
                Upload de Arquivo
              </DialogTitle>
              <DialogDescription className="mt-1">
                Envie o arquivo PDF do contrato
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          <FileUpload
            label="Arquivo PDF do Contrato"
            required={true}
            fileType="pdf"
            accept=".pdf,application/pdf"
            maxSizeMB={20}
            value={arquivo}
            onChange={handleFileChange}
            disabled={uploading}
          />
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={handleClose}
            disabled={uploading}
          >
            Cancelar
          </Button>
          <Button
            onClick={handleUpload}
            disabled={!arquivo || uploading}
          >
            {uploading ? 'Enviando...' : 'Enviar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
