import { useState, useEffect, useMemo } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { InserirContratoUseCase } from '@domain/usecases/InserirContratoUseCase'
import { AtualizarContratoUseCase } from '@domain/usecases/AtualizarContratoUseCase'
import { InserirArquivoParceiroUseCase } from '@domain/usecases/InserirArquivoParceiroUseCase'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { formatarValorMonetario, desformatarValorMonetario, formatarDataParaYYYYMMDD, convertBackendDateToInputFormat } from '@shared/utils/formatUtils'
import type { Contrato, StatusContratoId } from '@domain/entities/Contrato'
import { statusValorParaId } from '@domain/entities/Contrato'
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
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Checkbox } from '@/components/ui/checkbox'
import { FileText, Plus, X, Download, Building2 } from '@/components/ui/system-icons'
import { FileUpload } from './FileUpload'

interface ContratoFormModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  contrato?: Contrato
  parceiroId: string
  parceiroNome?: string
  onSuccess?: () => void
}

const OPCOES_STATUS: { value: StatusContratoId; label: string }[] = [
  { value: 0, label: 'Andamento' },
  { value: 1, label: 'Arquivado' },
  { value: 2, label: 'Completo' },
]

const PLATAFORMAS_ASSINATURA = [
  'DocuSign',
  'Clicksign',
  'Adobe Sign',
  'SignNow',
  'HelloSign',
  'Outro',
]

export const ContratoFormModal = ({
  open,
  onOpenChange,
  contrato,
  parceiroId,
  parceiroNome,
  onSuccess,
}: ContratoFormModalProps) => {
  const { token, user } = useAppSelector((state) => state.auth)
  const [salvando, setSalvando] = useState(false)

  // Form state
  const [numeroContrato, setNumeroContrato] = useState('')
  const [dataInicio, setDataInicio] = useState('')
  const [dataFim, setDataFim] = useState('')
  const [valorContrato, setValorContrato] = useState('')
  const [status, setStatus] = useState<StatusContratoId>(0)
  const [plataformaAssinatura, setPlataformaAssinatura] = useState('')
  const [reajusteAnual, setReajusteAnual] = useState('')
  const [arquivo, setArquivo] = useState<File | null>(null)
  const [contratoAssinado, setContratoAssinado] = useState(false)
  const [renovado, setRenovado] = useState(false)
  const [clausulaPenalidades, setClausulaPenalidades] = useState('')
  const [numeroPaginas, setNumeroPaginas] = useState('')
  const [referenciaCotacao, setReferenciaCotacao] = useState('')
  const [emailsNotificacao, setEmailsNotificacao] = useState<string[]>([])
  const [novoEmail, setNovoEmail] = useState('')
  
  // Estados de erro para feedback visual
  const [erros, setErros] = useState<Record<string, string>>({})

  const inserirContratoUseCase = container.resolve(InserirContratoUseCase)
  const atualizarContratoUseCase = container.resolve(AtualizarContratoUseCase)
  const inserirArquivoUseCase = container.resolve(InserirArquivoParceiroUseCase)

  // Preencher formulário quando editar
  useEffect(() => {
    if (contrato && open) {
      setNumeroContrato(contrato.contrato)
      // Converter datas do backend para formato YYYY-MM-DD do input
      // Debug: log raw backend values and converted result to diagnose off-by-one/timezone issues
      try {
        // eslint-disable-next-line no-console
        console.debug('ContratoFormModal: raw dataInicio:', contrato.dataInicio, 'converted:', convertBackendDateToInputFormat(contrato.dataInicio))
        // eslint-disable-next-line no-console
        console.debug('ContratoFormModal: raw dataFim:', contrato.dataFim, 'converted:', convertBackendDateToInputFormat(contrato.dataFim))
      } catch (e) {
        // ignore logging errors
      }
      setDataInicio(convertBackendDateToInputFormat(contrato.dataInicio) || '')
      setDataFim(convertBackendDateToInputFormat(contrato.dataFim) || '')
      setValorContrato(contrato.valorContrato > 0 ? formatarValorMonetario((contrato.valorContrato * 100).toString()) : '')
      setStatus(
        statusValorParaId(contrato.status ?? 'Em andamento')
      )
      setPlataformaAssinatura(contrato.plataformaAssinatura)
      setReajusteAnual(typeof contrato.reajusteAnual === 'string' ? contrato.reajusteAnual : '')
      setContratoAssinado(contrato.contratoAssinado)
      setRenovado(contrato.renovado ?? false)
      setClausulaPenalidades(contrato.clausulaPenalidades)
      setNumeroPaginas(contrato.numeroPaginas.toString())
      setReferenciaCotacao(contrato.referenciaCotacao)
      setEmailsNotificacao(
        contrato.emailsNotificacao && contrato.emailsNotificacao.length > 0
          ? contrato.emailsNotificacao
          : []
      )
      setErros({}) // Limpar erros ao carregar contrato existente
    } else if (!contrato && open) {
      // Reset para novo contrato
      setNumeroContrato('')
      setDataInicio('')
      setDataFim('')
      setValorContrato('')
      setStatus(0)
      setPlataformaAssinatura('')
      setReajusteAnual('')
      setArquivo(null)
      setContratoAssinado(false)
      setRenovado(false)
      setClausulaPenalidades('')
      setNumeroPaginas('')
      setReferenciaCotacao('')
      setEmailsNotificacao([])
      setErros({}) // Limpar erros ao resetar formulário
    }
  }, [contrato, open])

  const existingFileUrl = useMemo(() => {
    try {
      if (!contrato) return undefined
      return contrato.arquivo?.urlDownload || contrato.urlAnexo || undefined
    } catch {
      return undefined
    }
  }, [contrato])

  const existingFileName = useMemo(() => {
    if (!existingFileUrl) return undefined
    return decodeURIComponent(existingFileUrl.split('/').pop() || '')
  }, [existingFileUrl])

  const adicionarEmail = () => {
    const emailTrimmed = novoEmail.trim()
    if (!emailTrimmed) {
      toast.error('Email não pode estar vazio')
      return
    }
    
    // Validar formato básico de email
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailTrimmed)) {
      toast.error('Email inválido')
      return
    }
    
    // Validar que o email é @foursys.com.br
    if (!emailTrimmed.endsWith('@foursys.com.br')) {
      toast.error('Apenas emails @foursys.com.br são permitidos para notificações')
      return
    }
    
    setEmailsNotificacao([...emailsNotificacao, emailTrimmed])
    setNovoEmail('')
  }


  const removerEmail = (index: number) => {
    setEmailsNotificacao(emailsNotificacao.filter((_, i) => i !== index))
  }

  const validarFormulario = (): boolean => {
    const novosErros: Record<string, string> = {}
    const errosLista: string[] = []

    if (!numeroContrato.trim()) {
      novosErros.numeroContrato = 'Nome do contrato é obrigatório'
      errosLista.push('Nome do contrato')
    }
    if (!dataInicio.trim()) {
      novosErros.dataInicio = 'Data de início é obrigatória'
      errosLista.push('Data de início')
    }
    if (!dataFim.trim()) {
      novosErros.dataFim = 'Data de fim é obrigatória'
      errosLista.push('Data de fim')
    }
    
    const dataInicioFormatada = formatarDataParaYYYYMMDD(dataInicio)
    const dataFimFormatada = formatarDataParaYYYYMMDD(dataFim)
    
    if (dataInicio.trim() && !dataInicioFormatada) {
      novosErros.dataInicio = 'Data de início com formato inválido'
      errosLista.push('Data de início (formato inválido)')
    }
    if (dataFim.trim() && !dataFimFormatada) {
      novosErros.dataFim = 'Data de fim com formato inválido'
      errosLista.push('Data de fim (formato inválido)')
    }
    
    if (dataInicioFormatada && dataFimFormatada && new Date(dataFim) < new Date(dataInicio)) {
      novosErros.dataFim = 'Data de fim deve ser maior ou igual à data de início'
      errosLista.push('Data de fim deve ser maior ou igual à data de início')
    }
    
    // Arquivo é obrigatório apenas quando criando ou se não houver arquivo existente no contrato
    const hasExistingFile = !!existingFileUrl
    if ((!contrato || !hasExistingFile) && !arquivo) {
      novosErros.arquivo = 'Arquivo PDF é obrigatório'
      errosLista.push('Arquivo PDF')
    }

    setErros(novosErros)

    if (errosLista.length > 0) {
      const mensagem = errosLista.length === 1
        ? `Campo obrigatório não preenchido: ${errosLista[0]}`
        : `Campos obrigatórios não preenchidos:\n\n${errosLista.map((e) => `• ${e}`).join('\n')}`
      
      toast.error('Validação de dados', {
        description: mensagem,
        duration: 6000,
      })
      return false
    }

    setErros({})
    return true
  }

  const handleSubmit = async () => {
    if (!token || !user) {
      toast.error('Token de autenticação não encontrado')
      return
    }

    if (!validarFormulario()) return

    try {
      setSalvando(true)

      // Filtrar emails válidos (opcional)
      const emailsValidos = emailsNotificacao.filter(
      (email) => email.trim().length > 0 && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())
    )

      // Converter valor monetário (opcional, pode ser 0)
      const valorNumerico = valorContrato.trim() 
        ? parseFloat(desformatarValorMonetario(valorContrato)) || 0
        : 0

      // reajusteAnual é sempre string
      // Garantir que datas estejam no formato YYYY-MM-DD
      // A validação já foi feita em validarFormulario(), mas verificamos novamente por segurança
      const dataInicioFormatada = formatarDataParaYYYYMMDD(dataInicio)
      const dataFimFormatada = formatarDataParaYYYYMMDD(dataFim)

      if (!dataInicio || !dataInicioFormatada) {
        toast.error('Validação de dados', {
          description: 'Data de início é obrigatória e deve estar em formato válido',
          duration: 5000,
        })
        setSalvando(false)
        return
      }
      if (!dataFim || !dataFimFormatada) {
        toast.error('Validação de dados', {
          description: 'Data de fim é obrigatória e deve estar em formato válido',
          duration: 5000,
        })
        setSalvando(false)
        return
      }

      const payload = {
        parceiroId,
        contrato: numeroContrato,
        dataInicio: dataInicioFormatada,
        dataFim: dataFimFormatada,
        valorContrato: valorNumerico,
        status,
        plataformaAssinatura,
        reajusteAnual: reajusteAnual.trim(),
        contratoAssinado,
        renovado,
        clausulaPenalidades,
        numeroPaginas: parseInt(numeroPaginas) || 0,
        referenciaCotacao,
        emailsNotificacao: emailsValidos,
        // Incluir codigoInternoColaborador se disponível (backend requires it)
        codigoInternoColaborador: (user as any).cpf || (contrato as any)?.codigoInternoColaborador || undefined,
      }

      let contratoId: string | undefined
      let urlAnexo: string | undefined

      if (contrato) {
        // Edição: Inserir arquivo primeiro, depois atualizar contrato com URL
        if (arquivo && token) {
          try {
            const arquivoResponse = await inserirArquivoUseCase.execute(token, {
              parceiroId,
              arquivoTipoId: 1, // 1=PDF
              arquivoOriginId: 1, // Origem padrão
              parceiroGestaoContratoId: contrato.id,
              file: arquivo,
            })
            
            if (arquivoResponse.dados?.urlDownload) {
              urlAnexo = arquivoResponse.dados.urlDownload
            }
          } catch (error) {
            console.error('Erro ao fazer upload do arquivo:', error)
            toast.error('Erro ao fazer upload do arquivo')
            setSalvando(false)
            return
          }
        }

        // Atualizar contrato com URL do arquivo
        await atualizarContratoUseCase.execute(token, {
          ...payload,
          id: contrato.id,
          urlAnexo: urlAnexo || contrato.arquivo?.urlDownload,
        })
        contratoId = contrato.id
        toast.success('Contrato atualizado com sucesso!')
        if (user) {
          logUserAction('GestaoParceria', 'EditarContrato', {
            contratoId: contrato.id,
          }, user)
        }
      } else {
        // Criação: Criar contrato primeiro, depois inserir arquivo e atualizar com URL
        const response = await inserirContratoUseCase.execute(token, payload)
        contratoId = response.dados?.id
        
        if (!contratoId) {
          toast.error('Erro ao criar contrato')
          setSalvando(false)
          return
        }

        // Inserir arquivo após criar contrato
        if (arquivo && token) {
          try {
            const arquivoResponse = await inserirArquivoUseCase.execute(token, {
              parceiroId,
              arquivoTipoId: 1, // 1=PDF
              arquivoOriginId: 1, // Origem padrão
              parceiroGestaoContratoId: contratoId,
              file: arquivo,
            })
            
            if (arquivoResponse.dados?.urlDownload) {
              urlAnexo = arquivoResponse.dados.urlDownload
              
              // Atualizar contrato com URL do arquivo
              await atualizarContratoUseCase.execute(token, {
                ...payload,
                id: contratoId,
                urlAnexo,
              })
            }
          } catch (error) {
            console.error('Erro ao fazer upload do arquivo:', error)
            toast.error('Erro ao fazer upload do arquivo')
            setSalvando(false)
            return
          }
        }
        
        toast.success('Contrato criado com sucesso!')
        if (user && response.dados) {
          logUserAction('GestaoParceria', 'CriarContrato', {
            contratoId: response.dados.id,
            parceiroId,
          }, user)
        }
      }

      onOpenChange(false)
      setArquivo(null)
      onSuccess?.()
    } catch (error) {
      console.error('Erro ao salvar contrato:', error)
      toast.error('Erro ao salvar contrato')
    } finally {
      setSalvando(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <div className="p-2 bg-primary/10 rounded-lg">
              <FileText className="h-6 w-6 text-primary" />
            </div>
            <div className="flex-1">
              <DialogTitle className="text-2xl font-bold">
                {contrato ? 'Editar Contrato' : 'Novo Contrato'}
              </DialogTitle>
              <DialogDescription className="mt-1">
                {contrato
                  ? 'Atualize as informações do contrato'
                  : 'Preencha os dados para cadastrar um novo contrato'}
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        <div className="space-y-4 py-4">
          {/* Indicador da Empresa */}
          {parceiroNome && (
            <div className="flex items-center gap-3 p-4 bg-muted/50 rounded-lg border border-border">
              <div className="p-2 bg-primary/10 rounded-lg">
                <Building2 className="h-5 w-5 text-primary" />
              </div>
              <div className="flex-1">
                <p className="text-sm text-muted-foreground">Empresa</p>
                <p className="text-base font-semibold text-foreground">{parceiroNome}</p>
              </div>
            </div>
          )}
          {/* Informações Básicas */}
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="numeroContrato">
                Nome do Contrato <span className="text-destructive">*</span>
              </Label>
              <Input
                id="numeroContrato"
                value={numeroContrato}
                onChange={(e) => {
                  setNumeroContrato(e.target.value)
                  if (erros.numeroContrato) {
                    setErros((prev) => {
                      const novos = { ...prev }
                      delete novos.numeroContrato
                      return novos
                    })
                  }
                }}
                placeholder="Nome do contrato"
                className={erros.numeroContrato ? 'border-destructive focus-visible:ring-destructive' : ''}
              />
              {erros.numeroContrato && (
                <p className="text-sm text-destructive">{erros.numeroContrato}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="status">Status</Label>
              <Select
                value={String(status)}
                onValueChange={(v) => setStatus(Number(v) as StatusContratoId)}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {OPCOES_STATUS.map((opcao) => (
                    <SelectItem key={opcao.value} value={String(opcao.value)}>
                      {opcao.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="dataInicio">
                Data Início <span className="text-destructive">*</span>
              </Label>
              <Input
                id="dataInicio"
                type="date"
                value={dataInicio}
                onChange={(e) => {
                  setDataInicio(e.target.value)
                  if (erros.dataInicio) {
                    setErros((prev) => {
                      const novos = { ...prev }
                      delete novos.dataInicio
                      return novos
                    })
                  }
                }}
                className={erros.dataInicio ? 'border-destructive focus-visible:ring-destructive' : ''}
              />
              {erros.dataInicio && (
                <p className="text-sm text-destructive">{erros.dataInicio}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="dataFim">
                Data Fim <span className="text-destructive">*</span>
              </Label>
              <Input
                id="dataFim"
                type="date"
                value={dataFim}
                onChange={(e) => {
                  setDataFim(e.target.value)
                  if (erros.dataFim) {
                    setErros((prev) => {
                      const novos = { ...prev }
                      delete novos.dataFim
                      return novos
                    })
                  }
                }}
                min={dataInicio}
                className={erros.dataFim ? 'border-destructive focus-visible:ring-destructive' : ''}
              />
              {erros.dataFim && (
                <p className="text-sm text-destructive">{erros.dataFim}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="valorContrato">
                Valor do Contrato
              </Label>
              <Input
                id="valorContrato"
                type="text"
                value={valorContrato}
                onChange={(e) => setValorContrato(formatarValorMonetario(e.target.value))}
                placeholder="R$ 0,00"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="plataformaAssinatura">Plataforma de Assinatura</Label>
              <Select
                value={plataformaAssinatura}
                onValueChange={setPlataformaAssinatura}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione a plataforma" />
                </SelectTrigger>
                <SelectContent>
                  {PLATAFORMAS_ASSINATURA.map((plataforma) => (
                    <SelectItem key={plataforma} value={plataforma}>
                      {plataforma}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="numeroPaginas">Número de Páginas</Label>
              <Input
                id="numeroPaginas"
                type="number"
                min="0"
                value={numeroPaginas}
                onChange={(e) => setNumeroPaginas(e.target.value)}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="referenciaCotacao">Referência de Cotação</Label>
              <Input
                id="referenciaCotacao"
                value={referenciaCotacao}
                onChange={(e) => setReferenciaCotacao(e.target.value)}
              />
            </div>
          </div>

          {/* Reajuste Anual */}
          <div className="space-y-2">
            <Label htmlFor="reajusteAnual">Reajuste Anual</Label>
            <Input
              id="reajusteAnual"
              value={reajusteAnual}
              onChange={(e) => setReajusteAnual(e.target.value)}
              placeholder="Ex: IPCA, IGPM, 5% ao ano"
            />
          </div>

          {/* Checkboxes */}
          <div className="flex flex-wrap gap-6">

            <div className="flex items-center space-x-2">
              <Checkbox
                id="contratoAssinado"
                checked={contratoAssinado}
                onCheckedChange={(checked) => setContratoAssinado(checked === true)}
              />
              <Label htmlFor="contratoAssinado" className="cursor-pointer">
                Contrato Assinado
              </Label>
            </div>

            <div className="flex items-center space-x-2">
              <Checkbox
                id="renovado"
                checked={renovado}
                onCheckedChange={(checked) => setRenovado(checked === true)}
              />
              <Label htmlFor="renovado" className="cursor-pointer">
                Renovado
              </Label>
            </div>
          </div>

          {/* Cláusula de Penalidades */}
          <div className="space-y-2">
            <Label htmlFor="clausulaPenalidades">Cláusula de Penalidades</Label>
            <Textarea
              id="clausulaPenalidades"
              value={clausulaPenalidades}
              onChange={(e) => setClausulaPenalidades(e.target.value)}
              placeholder="Descreva as cláusulas de penalidades..."
              rows={3}
            />
          </div>

          {/* Upload de Arquivo */}
          {/* Mostrar arquivo existente, se houver, e permitir download */}
          {existingFileUrl && !arquivo && (
            <div className="flex items-center gap-3 p-3 bg-muted rounded-md">
              <FileText className="h-5 w-5 text-muted-foreground" />
              <div className="flex-1">
                <p className="text-sm font-medium">{existingFileName || 'Arquivo anexado'}</p>
                <p className="text-xs text-muted-foreground">Arquivo já anexado</p>
              </div>
              <Button
                variant="outline"
                size="sm"
                onClick={() => window.open(existingFileUrl, '_blank')}
              >
                <Download className="h-4 w-4 mr-2" />
                Baixar
              </Button>
            </div>
          )}

          <div className="space-y-2">
            <FileUpload
              label="Arquivo do Contrato (PDF)"
              required={!existingFileUrl}
              fileType="pdf"
              accept=".pdf,application/pdf"
              maxSizeMB={20}
              value={arquivo}
              onChange={(file) => {
                if (file) {
                  if (file.type !== 'application/pdf') {
                    toast.error('Apenas arquivos PDF são permitidos')
                    return
                  }
                  if (file.size > 20 * 1024 * 1024) {
                    toast.error('Arquivo muito grande. Tamanho máximo: 20MB')
                    return
                  }
                }
                setArquivo(file)
                if (erros.arquivo) {
                  setErros((prev) => {
                    const novos = { ...prev }
                    delete novos.arquivo
                    return novos
                  })
                }
              }}
              disabled={salvando}
            />
            {erros.arquivo && (
              <p className="text-sm text-destructive">{erros.arquivo}</p>
            )}
          </div>

          {/* Emails de Notificação */}
          <div className="space-y-2">
            <Label>Emails de Notificação</Label>
            <div className="flex gap-2">
              <Input
                value={novoEmail}
                onChange={(e) => setNovoEmail(e.target.value)}
                onKeyPress={(e) => {
                  if (e.key === 'Enter') {
                    e.preventDefault()
                    adicionarEmail()
                  }
                }}
                placeholder="email@exemplo.com"
                type="email"
              />
              <Button type="button" variant="outline" onClick={adicionarEmail}>
                <Plus className="h-4 w-4" />
              </Button>
            </div>
            {emailsNotificacao.length > 0 && (
              <div className="flex flex-wrap gap-2 mt-2">
                {emailsNotificacao.map((email, index) => (
                  <div
                    key={index}
                    className="flex items-center gap-1 px-2 py-1 bg-muted rounded-md text-sm"
                  >
                    <span>{email}</span>
                    <button
                      type="button"
                      onClick={() => removerEmail(index)}
                      className="text-muted-foreground hover:text-foreground transition-colors"
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={salvando}>
            Cancelar
          </Button>
          <Button onClick={handleSubmit} disabled={salvando}>
            {salvando ? 'Salvando...' : 'Salvar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
