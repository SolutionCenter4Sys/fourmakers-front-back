import { useState, useEffect } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { InserirParceiroUseCase } from '@domain/usecases/InserirParceiroUseCase'
import { AtualizarParceiroUseCase } from '@domain/usecases/AtualizarParceiroUseCase'
import { ListarUnidadesParceriaUseCase } from '@domain/usecases/ListarUnidadesParceriaUseCase'
import { InserirArquivoParceiroUseCase } from '@domain/usecases/InserirArquivoParceiroUseCase'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import type { Parceiro, TipoEmpresa, ContatoParceiro } from '@domain/entities/Parceiro'
import type { Unidade } from '@domain/entities/NotaFiscalGestao'
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
import { Building2, Plus, X } from '@/components/ui/system-icons'
import { Slider } from '@/components/ui/slider'
import { FileUpload } from './FileUpload'

interface ParceiroFormModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  parceiro?: Parceiro
  onSuccess?: () => void
  onCreateContrato?: (parceiroId: string) => void
}

const TIPOS_EMPRESA: TipoEmpresa[] = ['Benefício', 'Parceria', 'Aliança', 'Cliente', 'Fornecedor']

// Componente de estrela para rating
const StarIcon = ({ filled, className }: { filled: boolean; className?: string }) => (
  <svg
    className={className}
    fill={filled ? 'currentColor' : 'none'}
    stroke="currentColor"
    viewBox="0 0 24 24"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      strokeLinecap="round"
      strokeLinejoin="round"
      strokeWidth={2}
      d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z"
    />
  </svg>
)

// Componente de Rating com estrelas e slider
interface StarRatingProps {
  value: number
  onChange: (value: number) => void
  label?: string
  required?: boolean
}

const StarRating = ({ value, onChange, label, required = false }: StarRatingProps) => {
  const handleStarClick = (starValue: number) => {
    // Se clicar na mesma estrela que está preenchida, alternar entre cheia e meia
    if (starValue === Math.ceil(value)) {
      if (value === starValue) {
        onChange(starValue - 0.5)
      } else if (value === starValue - 0.5) {
        onChange(starValue - 1)
      } else {
        onChange(starValue)
      }
    } else {
      onChange(starValue)
    }
  }

  const renderStar = (starNumber: number) => {
    const fullValue = starNumber
    const halfValue = starNumber - 0.5
    
    if (value >= fullValue) {
      // Estrela completamente preenchida
      return (
        <button
          key={starNumber}
          type="button"
          onClick={() => handleStarClick(starNumber)}
          className="focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 rounded transition-colors p-1"
          aria-label={`Avaliar ${starNumber} estrelas`}
        >
          <StarIcon
            filled={true}
            className="h-6 w-6 text-yellow-400 transition-colors"
          />
        </button>
      )
    } else if (value >= halfValue) {
      // Meia estrela - usar overlay com overflow hidden
      return (
        <div key={starNumber} className="relative inline-block">
          <button
            type="button"
            onClick={() => handleStarClick(starNumber)}
            className="focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 rounded transition-colors p-1 relative"
            aria-label={`Avaliar ${starNumber} estrelas`}
          >
            {/* Estrela vazia (fundo) */}
            <StarIcon
              filled={false}
              className="h-6 w-6 text-gray-300 transition-colors"
            />
            {/* Meia estrela preenchida (overlay) */}
            <div 
              className="absolute left-1 top-1 overflow-hidden" 
              style={{ 
                width: '12px', 
                height: '24px'
              }}
            >
              <StarIcon
                filled={true}
                className="h-6 w-6 text-yellow-400 transition-colors"
              />
            </div>
          </button>
        </div>
      )
    } else {
      // Estrela vazia
      return (
        <button
          key={starNumber}
          type="button"
          onClick={() => handleStarClick(starNumber)}
          className="focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 rounded transition-colors p-1"
          aria-label={`Avaliar ${starNumber} estrelas`}
        >
          <StarIcon
            filled={false}
            className="h-6 w-6 text-gray-300 transition-colors hover:text-yellow-200"
          />
        </button>
      )
    }
  }

  return (
    <div className="space-y-3">
      {label && (
        <Label>
          {label} {required && <span className="text-destructive">*</span>}
        </Label>
      )}
      <div className="flex flex-col gap-3">
        <div className="flex items-center gap-2">
          {[1, 2, 3, 4, 5].map((star) => renderStar(star))}
          <span className="text-sm text-muted-foreground ml-2">
            {value.toFixed(1)}/5.0
          </span>
        </div>
        <div className="space-y-2">
          <Slider
            value={[value]}
            onValueChange={(vals) => onChange(vals[0])}
            min={0}
            max={5}
            step={0.5}
            className="w-full"
          />
          <div className="flex justify-between text-xs text-muted-foreground">
            <span>0</span>
            <span>2.5</span>
            <span>5</span>
          </div>
        </div>
      </div>
    </div>
  )
}

export const ParceiroFormModal = ({
  open,
  onOpenChange,
  parceiro,
  onSuccess,
  onCreateContrato,
}: ParceiroFormModalProps) => {
  const { token, user } = useAppSelector((state) => state.auth)
  const [salvando, setSalvando] = useState(false)
  const [unidades, setUnidades] = useState<Unidade[]>([])
  const [carregandoUnidades, setCarregandoUnidades] = useState(false)
  const [resultadoOpen, setResultadoOpen] = useState(false)
  const [resultadoTitle, setResultadoTitle] = useState('')
  const [resultadoMessage, setResultadoMessage] = useState('')
  const [resultadoIsError, setResultadoIsError] = useState(false)
  const [ultimoParceiroId, setUltimoParceiroId] = useState<string | undefined>(undefined)

  // Form state
  const [nome, setNome] = useState('')
  const [avaliacao, setAvaliacao] = useState(0)
  const [logoArquivo, setLogoArquivo] = useState<File | null>(null)
  const [unidade, setUnidade] = useState('')
  const [tipoEmpresa, setTipoEmpresa] = useState<TipoEmpresa>('Parceria')
  const [website, setWebsite] = useState('')
  const [linkedIn, setLinkedIn] = useState('')
  const [descricaoLonga, setDescricaoLonga] = useState('')
  const [descricaoCurta, setDescricaoCurta] = useState('')
  const [tags, setTags] = useState<string[]>([])
  const [novaTag, setNovaTag] = useState('')
  const [contatos, setContatos] = useState<ContatoParceiro[]>([])

  const inserirParceiroUseCase = container.resolve(InserirParceiroUseCase)
  const atualizarParceiroUseCase = container.resolve(AtualizarParceiroUseCase)
  const listarUnidadesUseCase = container.resolve(ListarUnidadesParceriaUseCase)
  const inserirArquivoUseCase = container.resolve(InserirArquivoParceiroUseCase)

  // Carregar unidades quando modal abrir
  useEffect(() => {
    const carregarUnidades = async () => {
      if (!open || !token) return

      try {
        setCarregandoUnidades(true)
        const response = await listarUnidadesUseCase.execute(token)
        
        // Processar resposta da API (pode ter diferentes estruturas)
        let unidadesData: Unidade[] = []
        
        if (Array.isArray(response)) {
          unidadesData = response
            .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
            .map((u: any) => ({
              id: String(u.id || u.Id || ''),
              descricao: u.descricao || u.Descricao || '',
            }))
        } else if (response && typeof response === 'object') {
          // Verificar se tem ListaUnidadesResult
          if ('ListaUnidadesResult' in response && Array.isArray(response.ListaUnidadesResult)) {
            unidadesData = response.ListaUnidadesResult
              .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
              .map((u: any) => ({
                id: String(u.id || u.Id || ''),
                descricao: u.descricao || u.Descricao || '',
              }))
          }
          // Verificar se tem retorno direto
          else if ('retorno' in response && Array.isArray(response.retorno)) {
            unidadesData = response.retorno
              .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
              .map((u: any) => ({
                id: String(u.id || u.Id || ''),
                descricao: u.descricao || u.Descricao || '',
              }))
          }
        }
        
        setUnidades(unidadesData)
      } catch (error) {
        console.error('Erro ao carregar unidades:', error)
        toast.error('Erro ao carregar unidades')
      } finally {
        setCarregandoUnidades(false)
      }
    }

    carregarUnidades()
  }, [open, token])

  // Preencher formulário quando editar
  useEffect(() => {
    if (parceiro && open) {
      setNome(parceiro.nome)
      setAvaliacao(parceiro.avaliacao)
      setLogoArquivo(null) // Logo será carregado do backend, não precisamos do arquivo
      setUnidade(parceiro.unidade)
      setTipoEmpresa(parceiro.tipoEmpresa)
      setWebsite(parceiro.website)
      setLinkedIn(parceiro.linkedIn)
      setDescricaoLonga(parceiro.descricaoLonga)
      setDescricaoCurta(parceiro.descricaoCurta)
      setTags(parceiro.tags || [])
      setContatos(parceiro.contatos || [])
    } else if (!parceiro && open) {
      // Reset para novo parceiro
      setNome('')
      setAvaliacao(0)
      setLogoArquivo(null)
      setUnidade('')
      setTipoEmpresa('Parceria')
      setWebsite('')
      setLinkedIn('')
      setDescricaoLonga('')
      setDescricaoCurta('')
      setTags([])
      setContatos([])
    }
  }, [parceiro, open])

  const adicionarTag = () => {
    if (novaTag.trim() && !tags.includes(novaTag.trim())) {
      setTags([...tags, novaTag.trim()])
      setNovaTag('')
    }
  }

  const removerTag = (tag: string) => {
    setTags(tags.filter((t) => t !== tag))
  }

  const adicionarContato = () => {
    setContatos([
      ...contatos,
      { nome: '', email: '', telefone: '' },
    ])
  }

  const formatarTelefoneBR = (value: string): string => {
    const cleaned = value.replace(/\D/g, "")
    
    // Celular: (XX) 9XXXX-XXXX (11 dígitos)
    if (cleaned.length === 11) {
      return cleaned.replace(/(\d{2})(\d{5})(\d{4})/, "($1) $2-$3")
    }
    
    // Telefone fixo: (XX) XXXX-XXXX (10 dígitos)
    if (cleaned.length === 10) {
      return cleaned.replace(/(\d{2})(\d{4})(\d{4})/, "($1) $2-$3")
    }
    
    // Formatação parcial enquanto digita
    if (cleaned.length > 10) {
      return cleaned.replace(/(\d{2})(\d{5})(\d{0,4}).*/, "($1) $2-$3")
    } else if (cleaned.length > 6) {
      return cleaned.replace(/(\d{2})(\d{4})(\d{0,4}).*/, "($1) $2-$3")
    } else if (cleaned.length > 2) {
      return cleaned.replace(/(\d{2})(\d{0,5})/, "($1) $2")
    } else if (cleaned.length > 0) {
      return `(${cleaned}`
    }
    
    return value
  }

  const atualizarContato = (index: number, campo: keyof ContatoParceiro, valor: string) => {
    const novosContatos = [...contatos]
    let valorFormatado = valor
    
    // Aplicar máscara de telefone se o campo for telefone
    if (campo === 'telefone') {
      valorFormatado = formatarTelefoneBR(valor)
    }
    
    novosContatos[index] = { ...novosContatos[index], [campo]: valorFormatado }
    setContatos(novosContatos)
  }

  const removerContato = (index: number) => {
    setContatos(contatos.filter((_, i) => i !== index))
  }

  const validarEFormatarLinkedIn = (url: string): string | null => {
    if (!url || !url.trim()) {
      return null // LinkedIn é opcional
    }
    
    let urlFormatada = url.trim()
    
    // Encontrar a posição de "linkedin.com/"
    const linkedinPattern = /linkedin\.com\//i
    const match = urlFormatada.match(linkedinPattern)
    
    if (!match) {
      toast.error('URL do LinkedIn inválida. Deve conter "linkedin.com/"')
      return null
    }
    
    // Verificar se há algo após "linkedin.com/"
    const indexAfterPattern = match.index! + match[0].length
    const afterPattern = urlFormatada.substring(indexAfterPattern).trim()
    
    if (!afterPattern || afterPattern.length === 0) {
      toast.error('URL do LinkedIn incompleta. Informe o perfil após "linkedin.com/"')
      return null
    }
    
    // Remover parâmetros de query e fragmentos se houver
    const cleanAfterPattern = afterPattern.split('?')[0].split('#')[0].trim()
    if (!cleanAfterPattern) {
      toast.error('URL do LinkedIn incompleta. Informe o perfil após "linkedin.com/"')
      return null
    }
    
    // Reconstruir URL com a parte limpa
    const baseUrl = urlFormatada.substring(0, indexAfterPattern)
    urlFormatada = baseUrl + cleanAfterPattern
    
    // Adicionar https:// se não existir
    if (!urlFormatada.toLowerCase().startsWith('http://') && !urlFormatada.toLowerCase().startsWith('https://')) {
      urlFormatada = 'https://' + urlFormatada
    }
    
    // Adicionar www. se não existir (após o protocolo)
    if (urlFormatada.toLowerCase().startsWith('https://')) {
      const afterProtocol = urlFormatada.substring(8) // Remove "https://"
      if (!afterProtocol.toLowerCase().startsWith('www.')) {
        urlFormatada = 'https://www.' + afterProtocol
      }
    } else if (urlFormatada.toLowerCase().startsWith('http://')) {
      const afterProtocol = urlFormatada.substring(7) // Remove "http://"
      if (!afterProtocol.toLowerCase().startsWith('www.')) {
        urlFormatada = 'https://www.' + afterProtocol // Sempre usar https
      } else {
        urlFormatada = 'https://' + afterProtocol // Converter http para https
      }
    }
    
    return urlFormatada
  }

  const validarFormulario = (): boolean => {
    const erros: string[] = []

    if (!nome.trim() || nome.length < 2) {
      erros.push('Nome (deve ter pelo menos 2 caracteres)')
    }
    if (avaliacao < 0 || avaliacao > 5) {
      erros.push('Avaliação (deve ser entre 0 e 5)')
    }
    
    // Validar emails dos contatos (qualquer domínio é permitido)
    const emailsInvalidos: string[] = []
    for (const contato of contatos) {
      if (contato.email && contato.email.trim()) {
        const emailTrimmed = contato.email.trim()
        // Validar apenas formato básico de email
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailTrimmed)) {
          emailsInvalidos.push(emailTrimmed)
        }
      }
    }
    
    if (emailsInvalidos.length > 0) {
      erros.push(`Email(s) inválido(s): ${emailsInvalidos.join(', ')}`)
    }

    if (erros.length > 0) {
      const mensagem = erros.length === 1
        ? `Campo obrigatório inválido: ${erros[0]}`
        : `Campos obrigatórios inválidos:\n\n${erros.map((e) => `• ${e}`).join('\n')}`
      
      toast.error('Validação de dados', {
        description: mensagem,
        duration: 6000,
      })
      return false
    }

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

      // Garantir que LinkedIn está formatado corretamente antes de enviar
      let linkedInFormatado = linkedIn
      if (linkedIn.trim()) {
        const validado = validarEFormatarLinkedIn(linkedIn)
        if (validado === null) {
          setSalvando(false)
          return
        }
        linkedInFormatado = validado
      }

      const payload = {
        nome,
        avaliacao,
        unidade: unidade || '',
        tipoEmpresa,
        website,
        linkedIn: linkedInFormatado,
        descricaoLonga,
        descricaoCurta,
        tags,
        contatos: contatos.filter((c) => c.nome || c.email || c.telefone),
        orgId: user.orgId || user.colaboradorOrg?.orgId || 0,
        // Incluir codigoInternoColaborador obrigatório pelo backend
        codigoInternoColaborador: (user as any).cpf || (user as any).codigoInternoColaborador || '',
      }

      let parceiroId: string | undefined
      let logoUrl: string | undefined

      if (parceiro) {
        // Edição: Atualizar parceiro primeiro
        await atualizarParceiroUseCase.execute(token, {
          ...payload,
          id: parceiro.id,
        })
        parceiroId = parceiro.id
        setUltimoParceiroId(parceiroId)
        setResultadoTitle('Empresa atualizada')
        setResultadoMessage('Empresa atualizada com sucesso! Deseja adicionar um contrato agora?')
        setResultadoIsError(false)
        setResultadoOpen(true)
        if (user) {
          logUserAction('GestaoParceria', 'EditarParceiro', {
            parceiroId: parceiro.id,
          }, user)
        }
      } else {
        // Criação: Criar parceiro primeiro
        const response = await inserirParceiroUseCase.execute(token, payload)
        parceiroId = response.dados?.id
        
        if (!parceiroId) {
          console.error('Erro: parceiroId não retornado na resposta:', response)
          toast.error('Erro ao criar empresa: ID não retornado')
          setSalvando(false)
          return
        }
        
        setUltimoParceiroId(parceiroId)
        setResultadoTitle('Empresa criada')
        setResultadoMessage('Empresa criada com sucesso! Deseja adicionar um contrato agora?')
        setResultadoIsError(false)
        setResultadoOpen(true)
        if (user && response.dados) {
          logUserAction('GestaoParceria', 'CriarParceiro', {
            parceiroId: response.dados.id,
          }, user)
        }
      }

      // Upload de logo se arquivo foi selecionado
      if (logoArquivo && parceiroId && token) {
        try {
          // Validar tipo de arquivo (apenas imagens)
          if (!logoArquivo.type.startsWith('image/')) {
            toast.error('Apenas arquivos de imagem são permitidos para o logo')
            setSalvando(false)
            return
          }
          // Validar tamanho (4MB máximo)
          if (logoArquivo.size > 4 * 1024 * 1024) {
            toast.error('Arquivo muito grande. Tamanho máximo: 4MB')
            setSalvando(false)
            return
          }

          console.log('Fazendo upload do logo para parceiroId:', parceiroId)
          const arquivoResponse = await inserirArquivoUseCase.execute(token, {
            parceiroId,
            arquivoTipoId: 3, // 3=Imagem
            arquivoOriginId: 1, // Origem padrão
            parceiroGestaoContratoId: parceiroId, // Para logo, usar o próprio parceiroId
            file: logoArquivo,
          })

          console.log('Resposta do upload:', arquivoResponse)

          if (arquivoResponse.dados?.urlDownload) {
            logoUrl = arquivoResponse.dados.urlDownload
            console.log('URL do logo obtida:', logoUrl)

            // Atualizar parceiro com URL do logo
            await atualizarParceiroUseCase.execute(token, {
              ...payload,
              id: parceiroId,
              urlLogo: logoUrl,
            } as any)
            console.log('Empresa atualizada com logo')
            toast.success('Logo atualizado com sucesso!')
          } else {
            console.error('URL do logo não retornada na resposta:', arquivoResponse)
            toast.error('Erro: URL do logo não retornada')
          }
        } catch (error) {
          console.error('Erro ao fazer upload do logo:', error)
          toast.error('Erro ao fazer upload do logo')
          // Não retornar aqui - permitir que o fluxo continue mesmo se o logo falhar
        }
      }

      onOpenChange(false)
      setLogoArquivo(null)
      onSuccess?.()
    } catch (error) {
      console.error('Erro ao salvar empresa:', error)
      setResultadoTitle('Erro')
      setResultadoMessage('Erro ao salvar empresa. Verifique e tente novamente.')
      setResultadoIsError(true)
      setResultadoOpen(true)
    } finally {
      setSalvando(false)
    }
  }

  return (
    <>
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-primary/10 rounded-lg">
                <Building2 className="h-6 w-6 text-primary" />
              </div>
              <div>
                <DialogTitle className="text-2xl font-bold">
                  {parceiro ? 'Editar Empresa' : 'Nova Empresa'}
                </DialogTitle>
                <DialogDescription className="mt-1">
                  {parceiro
                    ? 'Atualize as informações da empresa'
                    : 'Preencha os dados para cadastrar uma nova empresa'}
                </DialogDescription>
              </div>
            </div>
          </DialogHeader>

          <div className="space-y-4 py-4">
            {/* Informações Básicas */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="nome">
                  Nome <span className="text-destructive">*</span>
                </Label>
                <Input
                  id="nome"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                  placeholder="Nome da empresa"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="unidade">Unidade</Label>
                <Select
                  value={unidade}
                  onValueChange={setUnidade}
                  disabled={carregandoUnidades}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={carregandoUnidades ? "Carregando..." : "Selecione a unidade"} />
                  </SelectTrigger>
                  <SelectContent>
                    {unidades.length > 0 ? (
                      unidades.map((u) => (
                        <SelectItem key={u.id} value={u.descricao}>
                          {u.descricao}
                        </SelectItem>
                      ))
                    ) : (
                      !carregandoUnidades && (
                        <SelectItem value="" disabled>
                          Nenhuma unidade disponível
                        </SelectItem>
                      )
                    )}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="tipoEmpresa">Tipo de Empresa</Label>
                <Select value={tipoEmpresa} onValueChange={(v) => setTipoEmpresa(v as TipoEmpresa)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {TIPOS_EMPRESA.map((tipo) => (
                      <SelectItem key={tipo} value={tipo}>
                        {tipo}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="website">Website</Label>
                <Input
                  id="website"
                  value={website}
                  onChange={(e) => setWebsite(e.target.value)}
                  placeholder="https://..."
                />
              </div>

              <div className="md:col-span-2">
                <div className="space-y-2">
                  <FileUpload
                    label="Logo (Imagem)"
                    fileType="image"
                    accept="image/*"
                    maxSizeMB={4}
                    value={logoArquivo}
                    onChange={(file) => {
                      if (file) {
                        if (!file.type.startsWith('image/')) {
                          toast.error('Apenas arquivos de imagem são permitidos')
                          return
                        }
                        if (file.size > 4 * 1024 * 1024) {
                          toast.error('Arquivo muito grande. Tamanho máximo: 4MB')
                          return
                        }
                      }
                      setLogoArquivo(file)
                    }}
                    disabled={salvando}
                  />
                </div>
              </div>
            </div>

            {/* Avaliação com estrelas e slider */}
            <div className="space-y-3">
              <StarRating
                value={avaliacao}
                onChange={setAvaliacao}
                label="Avaliação"
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2 col-span-2">
                <Label htmlFor="linkedIn">LinkedIn</Label>
                <Input
                  id="linkedIn"
                  value={linkedIn}
                  onChange={(e) => setLinkedIn(e.target.value)}
                  placeholder="https://linkedin.com/..."
                />
              </div>
            </div>

            {/* Descrições */}
            <div className="space-y-2">
              <Label htmlFor="descricaoCurta">Descrição Curta</Label>
              <Textarea
                id="descricaoCurta"
                value={descricaoCurta}
                onChange={(e) => setDescricaoCurta(e.target.value)}
                placeholder="Descrição breve..."
                rows={2}
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="descricaoLonga">Descrição Longa</Label>
              <Textarea
                id="descricaoLonga"
                value={descricaoLonga}
                onChange={(e) => setDescricaoLonga(e.target.value)}
                placeholder="Descrição completa..."
                rows={4}
              />
            </div>

            {/* Categorias */}
            <div className="space-y-2">
              <Label>Categorias</Label>
              <div className="flex gap-2">
                <Input
                  value={novaTag}
                  onChange={(e) => setNovaTag(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && adicionarTag()}
                  placeholder="Adicionar categoria"
                />
                <Button type="button" variant="outline" onClick={adicionarTag}>
                  <Plus className="h-4 w-4" />
                </Button>
              </div>
              {tags.length > 0 && (
                <div className="flex flex-wrap gap-2 mt-2">
                  {tags.map((tag) => (
                    <div
                      key={tag}
                      className="flex items-center gap-1 px-2 py-1 bg-muted rounded-md text-sm"
                    >
                      <span>{tag}</span>
                      <button
                        type="button"
                        onClick={() => removerTag(tag)}
                        className="text-muted-foreground hover:text-foreground"
                      >
                        <X className="h-3 w-3" />
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Contatos */}
            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <Label>Contatos</Label>
                <Button type="button" variant="outline" size="sm" onClick={adicionarContato}>
                  <Plus className="h-4 w-4 mr-2" />
                  Adicionar Contato
                </Button>
              </div>
              {contatos.map((contato, index) => (
                <div key={index} className="grid grid-cols-3 gap-2 p-3 border rounded-lg">
                  <Input
                    placeholder="Nome"
                    value={contato.nome}
                    onChange={(e) => atualizarContato(index, 'nome', e.target.value)}
                  />
                  <Input
                    placeholder="Email"
                    type="email"
                    value={contato.email}
                    onChange={(e) => atualizarContato(index, 'email', e.target.value)}
                  />
                  <div className="flex gap-2">
                    <Input
                      placeholder="(00) 00000-0000"
                      value={contato.telefone}
                      onChange={(e) => atualizarContato(index, 'telefone', e.target.value)}
                      maxLength={15}
                    />
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      onClick={() => removerContato(index)}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <DialogFooter>
            <div className="flex gap-2 ml-auto">
              <Button variant="outline" onClick={() => onOpenChange(false)} disabled={salvando}>
                Cancelar
              </Button>
              <Button onClick={handleSubmit} disabled={salvando}>
                {salvando ? 'Salvando...' : 'Salvar'}
              </Button>
            </div>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Resultado modal */}
      <Dialog open={resultadoOpen} onOpenChange={(o) => setResultadoOpen(o)}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>{resultadoTitle}</DialogTitle>
            <DialogDescription>{resultadoMessage}</DialogDescription>
          </DialogHeader>
          <DialogFooter>
            {!resultadoIsError && ultimoParceiroId && (
              <>
                <Button
                  variant="outline"
                  onClick={() => {
                    setResultadoOpen(false)
                    onOpenChange(false)
                    onSuccess?.()
                  }}
                >
                  Fechar
                </Button>
                <Button
                  onClick={async () => {
                    setResultadoOpen(false)
                    onOpenChange(false)
                    if (onCreateContrato && ultimoParceiroId) {
                      // Call onCreateContrato first to set parceiroSelecionado before onSuccess
                      await onCreateContrato(ultimoParceiroId)
                    }
                    // Call onSuccess after to refresh the list
                    onSuccess?.()
                  }}
                >
                  Adicionar contrato
                </Button>
              </>
            )}
            {resultadoIsError && (
              <Button
                onClick={() => {
                  setResultadoOpen(false)
                }}
              >
                Fechar
              </Button>
            )}
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}
