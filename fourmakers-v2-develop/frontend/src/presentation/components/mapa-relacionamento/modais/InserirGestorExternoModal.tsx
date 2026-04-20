import { useState, useEffect } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { ListarPermanenciasUseCase } from '@domain/usecases/ListarPermanenciasUseCase'
import { CriarGestorExternoUseCase } from '@domain/usecases/CriarGestorExternoUseCase'
import { toast } from 'sonner'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { AutocompleteSimplesMapa } from '@presentation/components/mapa-relacionamento/seletores/AutocompleteSimplesMapa'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Label } from '@/components/ui/label'
import { X, Plus, User, Mail, Phone, Linkedin, Briefcase } from 'lucide-react'

interface InserirGestorExternoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  codigoCliente?: string
  onSuccess?: () => void
}

interface PermanenciaOption {
  id: string
  descricao: string
}

interface AreaDeAtuacao {
  areaDeAtuacao: { descricao: string }
  permanencia: { id: string }
}

export function InserirGestorExternoModal({
  open,
  onOpenChange,
  codigoCliente,
  onSuccess,
}: InserirGestorExternoModalProps) {
  const { token } = useAppSelector((state) => state.auth)
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [telefone, setTelefone] = useState('')
  const [perfilLinkedin, setPerfilLinkedin] = useState('')
  const [preferenciasPessoais, setPreferenciasPessoais] = useState('')
  const [areasDeAtuacao, setAreasDeAtuacao] = useState<AreaDeAtuacao[]>([])
  const [permanencias, setPermanencias] = useState<PermanenciaOption[]>([])
  const [loadingPermanencias, setLoadingPermanencias] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [errors, setErrors] = useState<{
    nome?: string
    email?: string
    telefone?: string
  }>({})

  // Load permanencias when modal opens
  useEffect(() => {
    if (!open || !token) return

    const loadPermanencias = async () => {
      setLoadingPermanencias(true)
      try {
        const listarPermanenciasUseCase = container.resolve(ListarPermanenciasUseCase)
        const permanenciasData = await listarPermanenciasUseCase.execute(token)
        setPermanencias(permanenciasData)
      } catch (error) {
        console.error('Erro ao carregar permanências:', error)
        toast.error('Erro ao carregar permanências')
      } finally {
        setLoadingPermanencias(false)
      }
    }

    void loadPermanencias()
  }, [open, token])

  // Reset form when modal closes
  useEffect(() => {
    if (!open) {
      setNome('')
      setEmail('')
      setTelefone('')
      setPerfilLinkedin('')
      setPreferenciasPessoais('')
      setAreasDeAtuacao([])
      setErrors({})
    }
  }, [open])

  const handleAddAreaDeAtuacao = () => {
    setAreasDeAtuacao([
      ...areasDeAtuacao,
      {
        areaDeAtuacao: { descricao: '' },
        permanencia: { id: '' },
      },
    ])
  }

  const handleRemoveAreaDeAtuacao = (index: number) => {
    setAreasDeAtuacao(areasDeAtuacao.filter((_, i) => i !== index))
  }

  const handleUpdateAreaDeAtuacao = (
    index: number,
    field: 'descricao' | 'permanenciaId',
    value: string,
  ) => {
    const updated = [...areasDeAtuacao]
    if (field === 'descricao') {
      updated[index] = {
        ...updated[index],
        areaDeAtuacao: { descricao: value },
      }
    } else {
      updated[index] = {
        ...updated[index],
        permanencia: { id: value },
      }
    }
    setAreasDeAtuacao(updated)
  }

  const validateForm = (): boolean => {
    const newErrors: { nome?: string; email?: string; telefone?: string } = {}

    if (!nome.trim()) {
      newErrors.nome = 'Nome é obrigatório'
    }

    if (!email.trim()) {
      newErrors.email = 'Email é obrigatório'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = 'Email inválido'
    }

    if (!telefone.trim()) {
      newErrors.telefone = 'Telefone é obrigatório'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async () => {
    if (!token) {
      toast.error('Erro: token não disponível')
      return
    }

    if (!validateForm()) {
      return
    }

    setIsSaving(true)
    try {
      const criarGestorExternoUseCase = container.resolve(CriarGestorExternoUseCase)
      const payload = {
        codGestorExterno: '',
        nome: nome.trim(),
        email: email.trim(),
        telefone: telefone.trim(),
        codigoCliente: codigoCliente?.trim() || undefined,
        perfilLinkedin: perfilLinkedin.trim() || undefined,
        areasDeAtuacao: areasDeAtuacao.filter(
          (area) => area.areaDeAtuacao.descricao.trim() && area.permanencia.id,
        ),
        preferenciasPessoais: preferenciasPessoais.trim() || undefined,
      }

      const response = await criarGestorExternoUseCase.execute(token, payload)

      if (response.sucesso) {
        toast.success('Gestor externo inserido com sucesso')
        onSuccess?.()
        onOpenChange(false)
      } else {
        const errorMessage = response.mensagem || 'Erro ao inserir gestor externo'
        const errorsList = response.erros?.length
          ? `\n${response.erros.join('\n')}`
          : ''
        toast.error(`${errorMessage}${errorsList}`)
      }
    } catch (error) {
      console.error('Erro ao inserir gestor externo:', error)
      toast.error('Erro ao inserir gestor externo. Tente novamente.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-visible">
        <DialogHeader>
          <DialogTitle>Inserir Gestor Externo</DialogTitle>
          <DialogDescription>
            Preencha os dados do gestor externo. Campos marcados com * são obrigatórios.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4 max-h-[calc(90vh-180px)] overflow-y-auto">
          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground after:content-['*'] after:ml-0.5 after:text-destructive">
              Nome
            </Label>
            <div className="relative">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                <User className="w-4 h-4" />
              </div>
              <Input
                className={`h-10 pl-12 rounded-lg ${errors.nome ? 'border-destructive' : ''}`}
                value={nome}
                onChange={(e) => {
                  setNome(e.target.value)
                  if (errors.nome) {
                    setErrors({ ...errors, nome: undefined })
                  }
                }}
                placeholder="Digite o nome do gestor externo"
              />
            </div>
            {errors.nome && <p className="text-sm text-destructive mt-1">{errors.nome}</p>}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground after:content-['*'] after:ml-0.5 after:text-destructive">
              Email
            </Label>
            <div className="relative">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                <Mail className="w-4 h-4" />
              </div>
              <Input
                type="email"
                className={`h-10 pl-12 rounded-lg ${errors.email ? 'border-destructive' : ''}`}
                value={email}
                onChange={(e) => {
                  setEmail(e.target.value)
                  if (errors.email) {
                    setErrors({ ...errors, email: undefined })
                  }
                }}
                placeholder="Digite o email do gestor externo"
              />
            </div>
            {errors.email && <p className="text-sm text-destructive mt-1">{errors.email}</p>}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground after:content-['*'] after:ml-0.5 after:text-destructive">
              Telefone
            </Label>
            <div className="relative">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                <Phone className="w-4 h-4" />
              </div>
              <Input
                className={`h-10 pl-12 rounded-lg ${errors.telefone ? 'border-destructive' : ''}`}
                value={telefone}
                onChange={(e) => {
                  setTelefone(e.target.value)
                  if (errors.telefone) {
                    setErrors({ ...errors, telefone: undefined })
                  }
                }}
                placeholder="Digite o telefone do gestor externo"
              />
            </div>
            {errors.telefone && <p className="text-sm text-destructive mt-1">{errors.telefone}</p>}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">Perfil LinkedIn</Label>
            <div className="relative">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                <Linkedin className="w-4 h-4" />
              </div>
              <Input
                className="h-10 pl-12 rounded-lg"
                value={perfilLinkedin}
                onChange={(e) => setPerfilLinkedin(e.target.value)}
                placeholder="Digite o perfil do LinkedIn"
              />
            </div>
          </div>

          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <Label className="text-sm font-bold text-foreground">Áreas de Atuação</Label>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleAddAreaDeAtuacao}
                className="h-8"
              >
                <Plus className="w-4 h-4 mr-1" />
                Adicionar
              </Button>
            </div>
            {areasDeAtuacao.length === 0 ? (
              <p className="text-sm text-muted-foreground italic">
                Nenhuma área de atuação adicionada
              </p>
            ) : (
              <div className="space-y-3">
                {areasDeAtuacao.map((area, index) => (
                  <div
                    key={index}
                    className="flex items-start gap-2 p-3 border border-border rounded-lg bg-muted/50"
                  >
                    <div className="flex-1 space-y-2">
                      <div className="relative">
                        <div className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                          <Briefcase className="w-4 h-4" />
                        </div>
                        <Input
                          className="h-9 pl-10 text-sm"
                          value={area.areaDeAtuacao.descricao}
                          onChange={(e) =>
                            handleUpdateAreaDeAtuacao(index, 'descricao', e.target.value)
                          }
                          placeholder="Descrição da área de atuação"
                        />
                      </div>
                      <AutocompleteSimplesMapa
                        value={area.permanencia.id}
                        onValueChange={(value) =>
                          handleUpdateAreaDeAtuacao(index, 'permanenciaId', value)
                        }
                        options={permanencias}
                        disabled={loadingPermanencias}
                        placeholder="Selecione a permanência"
                      />
                    </div>
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      onClick={() => handleRemoveAreaDeAtuacao(index)}
                      className="h-9 w-9 shrink-0"
                    >
                      <X className="w-4 h-4" />
                    </Button>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">Preferências Pessoais</Label>
            <Textarea
              className="min-h-[100px]"
              value={preferenciasPessoais}
              onChange={(e) => setPreferenciasPessoais(e.target.value)}
              placeholder="Digite as preferências pessoais do gestor externo"
            />
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={isSaving}>
            Cancelar
          </Button>
          <Button onClick={handleSubmit} disabled={isSaving}>
            {isSaving ? 'Salvando...' : 'Salvar'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
