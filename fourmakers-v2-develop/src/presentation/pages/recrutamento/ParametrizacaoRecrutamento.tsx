import { useEffect, useState } from 'react'

import { Card, CardContent } from '@/components/ui/card'
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import { ChevronDown, ChevronUp, Mail, Plus, Settings, X } from '@/components/ui/system-icons'

import { PageBreadcrumb, PageHeader } from '@presentation/components/common'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { AtualizarParametrizacaoNotificacaoCandidatosUseCase } from '@domain/usecases/AtualizarParametrizacaoNotificacaoCandidatosUseCase'
import { ObterParametrizacaoNotificacaoCandidatosPorIdUseCase } from '@domain/usecases/ObterParametrizacaoNotificacaoCandidatosPorIdUseCase'
import { SalvarParametrizacaoNotificacaoCandidatosUseCase } from '@domain/usecases/SalvarParametrizacaoNotificacaoCandidatosUseCase'
import { toast } from 'sonner'

const normalizeEmail = (value: string): string => value.trim().toLowerCase()

const parseEmails = (value: string): string[] =>
  value
    .split(/[;,\s]+/)
    .map((email) => normalizeEmail(email))
    .filter(Boolean)

const isValidEmail = (value: string): boolean =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)

const normalizeEmailsFromResponse = (value: unknown): string[] => {
  if (Array.isArray(value)) {
    return value
      .flatMap((email) => parseEmails(String(email)))
      .filter(Boolean)
  }

  if (typeof value === 'string') {
    return parseEmails(value)
  }

  return []
}

type TemplateEmailItem = {
  id: string
  titulo: string
  subtitulo: string
  tituloEnvio: string
  descricao: string
  emailsCC: string[]
  emailInput: string
  isOpen: boolean
  isNew: boolean
  isSaving: boolean
  isDeleting: boolean
  savedTituloEnvio: string
  savedDescricao: string
  savedEmailsCC: string[]
}

const blocosSimulados = [
  {
    titulo: 'Emails de Reprovação de Candidato',
    subtitulo: 'Notificações por E-mail aos Candidatos',
  },
  {
    titulo: 'Emails de Aprovação de Candidato',
    subtitulo: 'Notificações por E-mail aos Candidatos',
  },
  {
    titulo: 'Emails de Convite de Candidato',
    subtitulo: 'Notificações por E-mail aos Candidatos',
  },
]

const criarItem = (
  index: number,
  data?: { titulo?: string; descricao?: string; emailsCC?: string[] },
  isNew: boolean = false,
): TemplateEmailItem => {
  const base = blocosSimulados[index]
  const titulo = base?.titulo ?? `Envio de Email ${index + 1}`
  const subtitulo = base?.subtitulo ?? 'Notificações por E-mail aos Candidatos'
  const tituloEnvio = data?.titulo ?? ''
  const descricao = data?.descricao ?? ''
  const emailsCC = data?.emailsCC ?? []

  return {
    id: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
    titulo,
    subtitulo,
    tituloEnvio,
    descricao,
    emailsCC,
    emailInput: '',
    isOpen: false,
    isNew,
    isSaving: false,
    isDeleting: false,
    savedTituloEnvio: tituloEnvio,
    savedDescricao: descricao,
    savedEmailsCC: emailsCC,
  }
}

export default function ParametrizacaoRecrutamento() {
  const { token, user } = useAppSelector((state) => state.auth)
  const orgId = user?.orgId || user?.colaboradorOrg?.orgId || 0

  const [carregando, setCarregando] = useState(true)
  const [itens, setItens] = useState<TemplateEmailItem[]>(() => [criarItem(0)])

  useEffect(() => {
    const carregarParametrizacao = async () => {
      if (!token || !orgId) {
        setCarregando(false)
        return
      }

      setCarregando(true)
      try {
        const useCase = container.resolve(ObterParametrizacaoNotificacaoCandidatosPorIdUseCase)
        const response = await useCase.execute(token, orgId)
        if (response?.sucesso && response?.retorno) {
          const r = response.retorno
          const emailsResponse = normalizeEmailsFromResponse(r.emailsCC)
          setItens((prev) => {
            const [primeiro, ...resto] = prev.length > 0 ? prev : [criarItem(0)]
            const atualizado = {
              ...primeiro,
              tituloEnvio: r.titulo ?? '',
              descricao: r.descricao ?? '',
              emailsCC: emailsResponse,
              emailInput: '',
              savedTituloEnvio: r.titulo ?? '',
              savedDescricao: r.descricao ?? '',
              savedEmailsCC: emailsResponse,
            }
            return [atualizado, ...resto]
          })
        }
      } catch (error) {
        console.error('Erro ao carregar parametrização:', error)
        toast.error('Não foi possível carregar a parametrização.')
      } finally {
        setCarregando(false)
      }
    }

    carregarParametrizacao()
  }, [token, orgId])

  const atualizarItem = (itemId: string, update: (item: TemplateEmailItem) => TemplateEmailItem) => {
    setItens((prev) => prev.map((item) => (item.id === itemId ? update(item) : item)))
  }

  const handleAddEmails = (itemId: string) => {
    const item = itens.find((current) => current.id === itemId)
    if (!item) {
      return
    }

    const candidatos = parseEmails(item.emailInput)
    if (candidatos.length === 0) {
      return
    }

    const invalidos = candidatos.filter((email) => !isValidEmail(email))
    if (invalidos.length > 0) {
      toast.error('Informe e-mails válidos para a cópia.')
      return
    }

    atualizarItem(itemId, (current) => ({
      ...current,
      emailsCC: Array.from(new Set([...current.emailsCC, ...candidatos])),
      emailInput: '',
    }))
  }

  const handleRemoveEmail = (itemId: string, email: string) => {
    atualizarItem(itemId, (current) => ({
      ...current,
      emailsCC: current.emailsCC.filter(
        (itemEmail) => normalizeEmail(itemEmail) !== normalizeEmail(email)
      ),
    }))
  }

  const handleCancelar = (itemId: string) => {
    const item = itens.find((current) => current.id === itemId)
    if (!item) {
      return
    }

    if (item.isNew) {
      setItens((prev) => prev.filter((current) => current.id !== itemId))
      return
    }

    atualizarItem(itemId, (current) => ({
      ...current,
      tituloEnvio: current.savedTituloEnvio,
      descricao: current.savedDescricao,
      emailsCC: current.savedEmailsCC,
      emailInput: '',
    }))
  }

  const handleSalvarItem = async (itemId: string) => {
    const item = itens.find((current) => current.id === itemId)
    if (!item) {
      return
    }

    if (!token || !orgId) {
      toast.error('Token de autenticação não disponível.')
      return
    }

    if (!item.tituloEnvio.trim()) {
      toast.error('Informe o título do envio de e-mail.')
      return
    }

    if (!item.descricao.trim()) {
      toast.error('Informe a descrição do envio de e-mail.')
      return
    }

    const emailsUnicos = Array.from(
      new Set(item.emailsCC.map((email) => normalizeEmail(email)))
    ).filter(Boolean)
    const emailsSeparados = emailsUnicos.join(';')

    if (emailsUnicos.length === 0) {
      toast.error('Informe pelo menos um e-mail de cópia.')
      return
    }

    const invalidos = emailsUnicos.filter((email) => !isValidEmail(email))
    if (invalidos.length > 0) {
      toast.error('Informe e-mails válidos para a cópia.')
      return
    }

    atualizarItem(itemId, (current) => ({ ...current, isSaving: true }))
    try {
      if (item.isNew) {
        const useCase = container.resolve(SalvarParametrizacaoNotificacaoCandidatosUseCase)
        const response = await useCase.execute(token, {
          orgId,
          titulo: item.tituloEnvio.trim(),
          descricao: item.descricao.trim(),
          emailsCC: emailsSeparados,
        })

        if (!response) {
          toast.error('Erro ao salvar parametrização.')
          return
        }
      } else {
        const useCase = container.resolve(AtualizarParametrizacaoNotificacaoCandidatosUseCase)
        const response = await useCase.execute(token, {
          orgId,
          titulo: item.tituloEnvio.trim(),
          descricao: item.descricao.trim(),
          emailsCC: emailsSeparados,
        })

        if (!response) {
          toast.error('Erro ao atualizar parametrização.')
          return
        }
      }

      atualizarItem(itemId, (current) => ({
        ...current,
        isNew: false,
        savedTituloEnvio: current.tituloEnvio.trim(),
        savedDescricao: current.descricao.trim(),
        savedEmailsCC: emailsUnicos,
        emailInput: '',
      }))
      toast.success('Envio de e-mail salvo com sucesso!')
    } catch (error: any) {
      console.error('Erro ao salvar parametrização:', error)
      toast.error('Erro ao salvar parametrização.')
    } finally {
      atualizarItem(itemId, (current) => ({ ...current, isSaving: false }))
    }
  }

  if (carregando) {
    return (
      <div className="container mx-auto p-4 md:p-6">
        <div className="text-center py-8 text-muted-foreground">Carregando...</div>
      </div>
    )
  }

  return (
    <div className="container mx-auto p-4 md:p-6 space-y-6">
      <PageBreadcrumb
        items={[
          { label: 'Recrutamento' },
          { label: 'Parametrização' },
        ]}
      />

      <PageHeader
        title="Parametrização"
        description="Configure os padrões de notificação inviadas por e-mail."
      />

      <Card>
        <CardContent className="p-6 space-y-6">
          <div className="flex items-center justify-between gap-4">
            <div>
              <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">
                Lista de envios de Email
              </h3>
              <p className="text-sm text-muted-foreground">
                Estrutura preparada para receber novos envios configurados.
              </p>
            </div>
          </div>

          <div className="space-y-6">
            {itens.map((item) => (
              <Collapsible
                key={item.id}
                open={item.isOpen}
                onOpenChange={(open) => {
                  atualizarItem(item.id, (current) => ({ ...current, isOpen: open }))
                }}
              >
                <Card className="border border-borderSoft">
                  <CardContent className="p-6 space-y-6">
                    <CollapsibleTrigger asChild>
                      <Button variant="ghost" className="w-full justify-between px-0">
                        <span className="flex items-center gap-2 text-left">
                          <Mail className="h-5 w-5 text-primary" />
                          <span>
                            <span className="block text-lg font-semibold">{item.titulo}</span>
                            <span className="block text-sm text-muted-foreground">{item.subtitulo}</span>
                          </span>
                        </span>
                        {item.isOpen ? (
                          <ChevronUp className="h-4 w-4" />
                        ) : (
                          <ChevronDown className="h-4 w-4" />
                        )}
                      </Button>
                    </CollapsibleTrigger>

                    <CollapsibleContent className="space-y-6">
                      <div className="space-y-2">
                        <Label htmlFor={`titulo-${item.id}`}>Título</Label>
                        <Input
                          id={`titulo-${item.id}`}
                          value={item.tituloEnvio}
                          onChange={(event) => {
                            const value = event.target.value
                            atualizarItem(item.id, (current) => ({ ...current, tituloEnvio: value }))
                          }}
                          placeholder="Informe o título do envio."
                          maxLength={200}
                        />
                        <p className="text-xs text-muted-foreground">
                          {item.tituloEnvio.length}/200 caracteres
                        </p>
                      </div>

                      <div className="space-y-2">
                        <Label htmlFor={`descricao-${item.id}`}>Descrição do envio</Label>
                        <Textarea
                          id={`descricao-${item.id}`}
                          value={item.descricao}
                          onChange={(event) => {
                            const value = event.target.value
                            atualizarItem(item.id, (current) => ({ ...current, descricao: value }))
                          }}
                          placeholder="Descreva o template de envio deste e-mail."
                          className="min-h-[120px]"
                          maxLength={1000}
                        />
                        <p className="text-xs text-muted-foreground">
                          {item.descricao.length}/1000 caracteres
                        </p>
                      </div>

                      <div className="space-y-3">
                        <Label htmlFor={`emails-copia-${item.id}`}>
                          E-mail padrão para receber cópia da notificação
                        </Label>
                        <div className="flex flex-col md:flex-row gap-3">
                          <Input
                            id={`emails-copia-${item.id}`}
                            value={item.emailInput}
                            onChange={(event) => {
                              const value = event.target.value
                              atualizarItem(item.id, (current) => ({ ...current, emailInput: value }))
                            }}
                            onKeyDown={(event) => {
                              if (event.key === 'Enter') {
                                event.preventDefault()
                              }
                            }}
                            placeholder="Digite um e-mail"
                            maxLength={100}
                          />
                          <Button
                            type="button"
                            variant="outline"
                            onClick={() => handleAddEmails(item.id)}
                            disabled={item.isSaving || item.isDeleting}
                          >
                            <Plus className="h-4 w-4 mr-2" />
                            Adicionar
                          </Button>
                        </div>
                        <p className="text-xs text-muted-foreground">
                          {item.emailInput.length}/100 caracteres
                        </p>


                                               {item.emailsCC.length > 0 ? (
                          <div className="flex flex-wrap gap-2">
                            {item.emailsCC.map((email) => (
                              <Badge key={email} variant="secondary" className="flex items-center gap-2">
                                {email}
                                <button
                                  type="button"
                                  className="text-muted-foreground hover:text-foreground"
                                  onClick={() => handleRemoveEmail(item.id, email)}
                                  aria-label={`Remover ${email}`}
                                >
                                  <X className="h-3 w-3" />
                                </button>
                              </Badge>
                            ))}
                          </div>
                        ) : (
                          <p className="text-xs text-muted-foreground">
                            Adicione um ou mais e-mails para receberem cópia das notificações.
                          </p>
                        )}



                        <div className="flex flex-wrap gap-2">
                          <Button
                            variant="outline"
                            onClick={() => handleCancelar(item.id)}
                            disabled={item.isSaving || item.isDeleting}
                          >
                            Cancelar
                          </Button>
                          <Button
                            onClick={() => handleSalvarItem(item.id)}
                            disabled={item.isSaving || item.isDeleting}
                          >
                            {item.isSaving ? 'Salvando...' : (
                              <>
                                <Settings className="h-4 w-4 mr-2" />
                                Salvar
                              </>
                            )}
                          </Button>
                 
                        </div>

 
                      </div>
                    </CollapsibleContent>
                  </CardContent>
                </Card>
              </Collapsible>
            ))}
          </div>
        </CardContent>
      </Card>

    </div>
  )
}
