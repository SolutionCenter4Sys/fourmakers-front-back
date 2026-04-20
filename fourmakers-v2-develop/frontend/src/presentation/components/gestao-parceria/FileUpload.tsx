import { useState, useRef, useCallback, useEffect } from 'react'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Upload, X, FileText } from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'

interface FileUploadProps {
  label?: string
  required?: boolean
  accept?: string
  maxSizeMB?: number
  fileType?: 'image' | 'pdf' | 'all'
  value: File | null
  onChange: (file: File | null) => void
  disabled?: boolean
  error?: string
}

export const FileUpload = ({
  label,
  required = false,
  accept,
  maxSizeMB = 10,
  fileType = 'all',
  value,
  onChange,
  disabled = false,
  error,
}: FileUploadProps) => {
  const [isDragging, setIsDragging] = useState(false)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const fileInputRef = useRef<HTMLInputElement>(null)

  // Limpar URL do objeto quando componente desmontar ou arquivo mudar
  useEffect(() => {
    if (value && value.type.startsWith('image/')) {
      const url = URL.createObjectURL(value)
      setPreviewUrl(url)
      return () => {
        URL.revokeObjectURL(url)
      }
    } else {
      setPreviewUrl(null)
    }
  }, [value])

  const validateFile = (file: File): { valid: boolean; error?: string } => {
    // Validar tipo
    if (fileType === 'image' && !file.type.startsWith('image/')) {
      return { valid: false, error: 'Apenas arquivos de imagem são permitidos' }
    }
    if (fileType === 'pdf' && file.type !== 'application/pdf') {
      return { valid: false, error: 'Apenas arquivos PDF são permitidos' }
    }

    // Validar tamanho
    if (file.size > maxSizeMB * 1024 * 1024) {
      return { valid: false, error: `Arquivo muito grande. Tamanho máximo: ${maxSizeMB}MB` }
    }

    return { valid: true }
  }

  const handleFileSelect = (file: File) => {
    const validation = validateFile(file)
    if (!validation.valid) {
      // A validação será tratada pelo componente pai via toast
      return false
    }
    onChange(file)
    return true
  }

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (file) {
      handleFileSelect(file)
    }
  }

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    if (!disabled) {
      setIsDragging(true)
    }
  }, [disabled])

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    setIsDragging(false)
  }, [])

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault()
    e.stopPropagation()
    setIsDragging(false)

    if (disabled) return

    const file = e.dataTransfer.files?.[0]
    if (file) {
      handleFileSelect(file)
    }
  }, [disabled])

  const handleClick = () => {
    if (!disabled) {
      fileInputRef.current?.click()
    }
  }

  const formatFileSize = (bytes: number): string => {
    if (bytes < 1024) return `${bytes} B`
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(2)} KB`
    return `${(bytes / 1024 / 1024).toFixed(2)} MB`
  }

  const getFileIcon = () => {
    if (fileType === 'image') {
      return (
        <svg
          className="h-12 w-12 text-muted-foreground"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
          />
        </svg>
      )
    }
    if (fileType === 'pdf') {
      return <FileText className="h-12 w-12 text-muted-foreground" />
    }
    return <Upload className="h-12 w-12 text-muted-foreground" />
  }

  const getAcceptString = () => {
    if (accept) return accept
    if (fileType === 'image') return 'image/*'
    if (fileType === 'pdf') return '.pdf,application/pdf'
    return undefined
  }

  return (
    <div className="space-y-2">
      {label && (
        <Label>
          {label} {required && <span className="text-destructive">*</span>}
        </Label>
      )}

      <input
        ref={fileInputRef}
        type="file"
        accept={getAcceptString()}
        onChange={handleInputChange}
        disabled={disabled}
        className="hidden"
      />

      {!value ? (
        <div
          onClick={handleClick}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          className={cn(
            'border-2 border-dashed rounded-lg p-8 text-center transition-all cursor-pointer',
            isDragging
              ? 'border-primary bg-primary/5'
              : 'border-muted-foreground/25 hover:border-primary/50 hover:bg-muted/50',
            disabled && 'opacity-50 cursor-not-allowed'
          )}
        >
          <div className="flex flex-col items-center gap-3">
            {getFileIcon()}
            <div>
              <p className="text-sm font-medium text-foreground mb-1">
                Clique para selecionar ou arraste o arquivo aqui
              </p>
              <p className="text-xs text-muted-foreground">
                {fileType === 'image' && 'Apenas imagens'}
                {fileType === 'pdf' && 'Apenas PDF'}
                {fileType === 'all' && 'Qualquer arquivo'}
                {` • Tamanho máximo: ${maxSizeMB}MB`}
              </p>
            </div>
          </div>
        </div>
      ) : (
        <div className="border-2 border-border rounded-lg p-4 bg-muted/30">
          <div className="flex items-center gap-4">
            {fileType === 'image' && previewUrl ? (
              <img
                src={previewUrl}
                alt="Preview"
                className="h-16 w-16 rounded-lg object-cover border border-border"
              />
            ) : (
              <div className="h-16 w-16 rounded-lg bg-muted flex items-center justify-center border border-border">
                {fileType === 'pdf' ? (
                  <FileText className="h-8 w-8 text-muted-foreground" />
                ) : (
                  <FileText className="h-8 w-8 text-muted-foreground" />
                )}
              </div>
            )}
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-foreground truncate">
                {value.name}
              </p>
              <p className="text-xs text-muted-foreground mt-1">
                {formatFileSize(value.size)}
              </p>
            </div>
            <Button
              type="button"
              variant="ghost"
              size="sm"
              onClick={() => onChange(null)}
              disabled={disabled}
              className="flex-shrink-0"
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
          {!disabled && (
            <div className="mt-3 pt-3 border-t border-border">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleClick}
                className="w-full"
              >
                <Upload className="h-4 w-4 mr-2" />
                Trocar arquivo
              </Button>
            </div>
          )}
        </div>
      )}

      {error && (
        <p className="text-sm text-destructive">{error}</p>
      )}
    </div>
  )
}
