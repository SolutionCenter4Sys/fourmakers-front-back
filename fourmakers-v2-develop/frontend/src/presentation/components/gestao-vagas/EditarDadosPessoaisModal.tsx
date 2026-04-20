import { useCallback, useEffect, useMemo, useState } from 'react'
import { container } from 'tsyringe'
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
import { Switch } from '@/components/ui/switch'
import { Spinner } from '@/components/ui/spinner'
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible'
import { Check, ChevronDown, Save } from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'
import { toast } from 'sonner'
import { useViaCep } from '@presentation/hooks/useViaCep'
import {
  normalizeBoolean,
  normalizeNullableString,
  toDateInputValue,
} from '@shared/utils/colaboradorFormNormalizers'
import type {
  ColaboradorDadosPessoais,
  EditarDadosColaboradorPayload,
} from '@domain/entities/ColaboradorDadosPessoais'
import { ObterDadosColaboradorUseCase } from '@domain/usecases/ObterDadosColaboradorUseCase'
import { EditarDadosColaboradorUseCase } from '@domain/usecases/EditarDadosColaboradorUseCase'

const EMPTY_SELECT = '__empty__'

const ESTADOS_CIVIS = [
  'Solteiro (a)',
  'Casado (a)',
  'Divorciado (a)',
  'Separado (a)',
  'União Estável',
  'Viúvo (a)',
] as const

const ETNIAS = ['Branca', 'Preta', 'Parda', 'Amarela', 'Indígena', 'Não informado'] as const
const ORIENTACOES = ['Heterossexual', 'Homossexual', 'Bissexual', 'Assexual', 'Não informado'] as const
const GENEROS = ['Homem cisgênero', 'Mulher cisgênero', 'Homem trans', 'Mulher trans', 'Não binário', 'Não informado'] as const
const ESCOLARIDADES = [
  'Ensino Fundamental',
  'Ensino Médio',
  'Ensino Técnico',
  'Graduação',
  'Pós-graduação',
  'MBA',
  'Mestrado',
  'Doutorado',
  'Não informado',
] as const
/** Índices da API EditarDadosColaborador para enumPCD. pcd (int 0/1) é derivado: enumPCD === 5 → 0, caso contrário → 1. */
const ENUM_PCD_OPTIONS: { value: number; label: string }[] = [
  { value: 5, label: 'Nenhuma' },
  { value: 1, label: 'Visual' },
  { value: 2, label: 'Física' },
  { value: 3, label: 'Intelectual' },
  { value: 4, label: 'TEA' },
  { value: 0, label: 'Auditiva' },
  { value: 6, label: 'Psicossocial/Mental' },
]

const UF_OPTIONS = ['AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA', 'MT', 'MS', 'MG', 'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN', 'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'] as const

type FormState = EditarDadosColaboradorPayload

interface EditarDadosPessoaisModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  token: string | null
  codigoInternoColaborador: string | null
  nomeCandidato?: string
  onSaved?: () => void
}

function initialFormState(codigoInternoColaborador: string): FormState {
  return {
    codigoInternoColaborador,
    nomeCompleto: '',
    colaboradorSaudeId: '',
    nacionalidade: '',
    sobre: '',
    urlLinkedin: '',
    visualizarBuscaAderencia: false,
    qualificado: false,
    dataNascimento: '',
    documentoColaborador: '',
    rg: '',
    estadoCivil: '',
    escolaridade: '',
    etnia: '',
    genero: '',
    orientacaoSexual: '',
    refugiado: false,
    email: '',
    emailAlternativo: '',
    celular: '',
    passaporte: '',
    endereco: {
      id: null,
      cep: '',
      endereco: '',
      complemento: '',
      numero: null,
      bairro: '',
      cidade: '',
      estado: '',
      comQuemMora: '',
    },
    matricula: '',
    ativo: false,
    contatoPrincipal: '',
    contatoPrincipalDdi: '',
    contatoOutro: '',
    candidato: false,
    saude: {
      pcd: null,
      enumPCD: 5,
      grupoDeRiscoCovid: 0,
      condicaoDeSaudeRelevante: '',
    },
  }
}

function mapDataToForm(data: ColaboradorDadosPessoais): FormState {
  const codigo = normalizeNullableString(data.codigoInternoColaborador)
  const numeroEndereco = data.endereco?.numero
  return {
    codigoInternoColaborador: codigo,
    nomeCompleto: normalizeNullableString(data.nomeCompleto),
    colaboradorSaudeId: data.colaboradorSaudeId ?? '',
    nacionalidade: normalizeNullableString(data.nacionalidade),
    sobre: normalizeNullableString(data.sobre),
    urlLinkedin: normalizeNullableString(data.urlLinkedin),
    visualizarBuscaAderencia: normalizeBoolean(data.visualizarBuscaAderencia),
    qualificado: normalizeBoolean(data.qualificado),
    dataNascimento: toDateInputValue(data.dataNascimento),
    documentoColaborador: formatCpfInput(normalizeNullableString(data.documentoColaborador)),
    rg: formatRgInput(normalizeNullableString(data.rg)),
    estadoCivil: normalizeNullableString(data.estadoCivil),
    escolaridade: normalizeNullableString(data.escolaridade),
    etnia: normalizeNullableString(data.etnia),
    genero: normalizeNullableString(data.genero),
    orientacaoSexual: normalizeNullableString(data.orientacaoSexual),
    refugiado: normalizeBoolean(data.refugiado),
    email: normalizeNullableString(data.email),
    emailAlternativo: normalizeNullableString(data.emailAlternativo),
    celular: formatPhoneInput(normalizeNullableString(data.contatoPrincipal)),
    passaporte: normalizeNullableString(data.passaporte),
    endereco: {
      id: data.endereco?.id ?? data.enderecoId ?? null,
      cep: formatCepInput(normalizeNullableString(data.endereco?.cep)),
      endereco: normalizeNullableString(data.endereco?.endereco),
      complemento: normalizeNullableString(data.endereco?.complemento),
      numero: typeof numeroEndereco === 'number' ? numeroEndereco : null,
      bairro: normalizeNullableString(data.endereco?.bairro),
      cidade: normalizeNullableString(data.endereco?.cidade),
      estado: normalizeNullableString(data.endereco?.estado),
      comQuemMora: normalizeNullableString(data.endereco?.comQuemMora),
    },
    matricula: normalizeNullableString(data.matricula),
    ativo: normalizeBoolean(data.ativo),
    contatoPrincipal: formatPhoneInput(normalizeNullableString(data.contatoPrincipal)),
    contatoPrincipalDdi: normalizeNullableString(data.contatoPrincipalDdi) || 'Não informado',
    contatoOutro: formatPhoneInput(normalizeNullableString(data.contatoOutro)),
    candidato: normalizeBoolean(data.candidato),
    saude: {
      pcd: null,
      enumPCD: data.saude?.enumPCD ?? 5,
      grupoDeRiscoCovid: data.saude?.grupoDeRiscoCovid ?? 0,
      condicaoDeSaudeRelevante: normalizeNullableString(data.saude?.condicaoDeSaudeRelevante),
    },
  }
}

function asSelectValue(value: string): string {
  return value || EMPTY_SELECT
}

function fromSelectValue(value: string): string {
  return value === EMPTY_SELECT ? '' : value
}

function onlyDigits(value: string): string {
  return value.replace(/\D/g, '')
}

function formatCpfInput(value: string): string {
  const digits = onlyDigits(value).slice(0, 11)
  if (digits.length <= 3) return digits
  if (digits.length <= 6) return `${digits.slice(0, 3)}.${digits.slice(3)}`
  if (digits.length <= 9) return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6)}`
  return `${digits.slice(0, 3)}.${digits.slice(3, 6)}.${digits.slice(6, 9)}-${digits.slice(9)}`
}

function formatRgInput(value: string): string {
  const digits = onlyDigits(value).slice(0, 9)
  if (digits.length <= 2) return digits
  if (digits.length <= 5) return `${digits.slice(0, 2)}.${digits.slice(2)}`
  if (digits.length <= 8) return `${digits.slice(0, 2)}.${digits.slice(2, 5)}.${digits.slice(5)}`
  return `${digits.slice(0, 2)}.${digits.slice(2, 5)}.${digits.slice(5, 8)}-${digits.slice(8)}`
}

function formatCepInput(value: string): string {
  const digits = onlyDigits(value).slice(0, 8)
  if (digits.length <= 5) return digits
  return `${digits.slice(0, 5)}-${digits.slice(5)}`
}

function formatPhoneInput(value: string): string {
  const digits = onlyDigits(value).slice(0, 11)
  if (digits.length <= 2) return digits.length ? `(${digits}` : ''
  if (digits.length <= 6) return `(${digits.slice(0, 2)}) ${digits.slice(2)}`
  if (digits.length <= 10) return `(${digits.slice(0, 2)}) ${digits.slice(2, 6)}-${digits.slice(6)}`
  return `(${digits.slice(0, 2)}) ${digits.slice(2, 7)}-${digits.slice(7)}`
}

export function EditarDadosPessoaisModal({
  open,
  onOpenChange,
  token,
  codigoInternoColaborador,
  nomeCandidato,
  onSaved,
}: EditarDadosPessoaisModalProps) {
  const obterDadosColaboradorUseCase = container.resolve(ObterDadosColaboradorUseCase)
  const editarDadosColaboradorUseCase = container.resolve(EditarDadosColaboradorUseCase)
  const { buscarCep } = useViaCep()

  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [perfilOpen, setPerfilOpen] = useState(true)
  const [saudeOpen, setSaudeOpen] = useState(true)
  const [enderecoOpen, setEnderecoOpen] = useState(true)
  const [contatosOpen, setContatosOpen] = useState(true)
  const [form, setForm] = useState<FormState | null>(null)

  const modalTitle = useMemo(
    () => `Editar dados pessoais${nomeCandidato ? ` - ${nomeCandidato}` : ''}`,
    [nomeCandidato],
  )

  const handleLoad = useCallback(async () => {
    if (!open || !token || !codigoInternoColaborador) return
    setLoading(true)
    try {
      const response = await obterDadosColaboradorUseCase.execute(token, codigoInternoColaborador)
      const dados = response?.retorno
      if (!dados?.codigoInternoColaborador) {
        throw new Error(response?.mensagem ?? 'Não foi possível obter dados do colaborador.')
      }
      setForm(mapDataToForm(dados))
    } catch (error) {
      console.error('Erro ao obter dados do colaborador', error)
      toast.error(error instanceof Error ? error.message : 'Erro ao carregar dados pessoais.')
      onOpenChange(false)
    } finally {
      setLoading(false)
    }
  }, [open, token, codigoInternoColaborador, obterDadosColaboradorUseCase, onOpenChange])

  useEffect(() => {
    if (!open || !codigoInternoColaborador) return
    setForm(initialFormState(codigoInternoColaborador))
    handleLoad()
  }, [open, codigoInternoColaborador, handleLoad])

  const updateField = <K extends keyof FormState>(key: K, value: FormState[K]) => {
    setForm((prev) => (prev ? { ...prev, [key]: value } : prev))
  }

  const updateEndereco = <K extends keyof NonNullable<FormState['endereco']>>(
    key: K,
    value: NonNullable<FormState['endereco']>[K],
  ) => {
    setForm((prev) =>
      prev
        ? {
            ...prev,
            endereco: {
              ...prev.endereco,
              [key]: value,
            },
          }
        : prev,
    )
  }

  const updateSaude = <K extends keyof NonNullable<FormState['saude']>>(
    key: K,
    value: NonNullable<FormState['saude']>[K],
  ) => {
    setForm((prev) =>
      prev
        ? {
            ...prev,
            saude: {
              ...prev.saude,
              [key]: value,
            },
          }
        : prev,
    )
  }

  const handleBuscarCep = useCallback(async () => {
    if (!form) return
    const cep = onlyDigits(form.endereco.cep ?? '')
    if (cep.length !== 8) return
    const enderecoCep = await buscarCep(cep)
    if (!enderecoCep) return
    setForm((prev) =>
      prev
        ? {
            ...prev,
            endereco: {
              ...prev.endereco,
              endereco: enderecoCep.endereco || prev.endereco.endereco,
              bairro: enderecoCep.bairro || prev.endereco.bairro,
              cidade: enderecoCep.cidade || prev.endereco.cidade,
              estado: enderecoCep.estado || prev.endereco.estado,
            },
          }
        : prev,
    )
  }, [buscarCep, form])

  const handleSave = async () => {
    if (!token || !form || !codigoInternoColaborador) return
    setSaving(true)
    try {
      const enumPCD = form.saude.enumPCD ?? 5
      const payload: EditarDadosColaboradorPayload = {
        ...form,
        codigoInternoColaborador,
        celular: onlyDigits(form.contatoPrincipal || form.celular),
        contatoPrincipal: onlyDigits(form.contatoPrincipal || form.celular),
        contatoOutro: onlyDigits(form.contatoOutro),
        contatoPrincipalDdi: form.contatoPrincipalDdi || 'Não informado',
        dataNascimento: form.dataNascimento || '0001-01-01',
        documentoColaborador: onlyDigits(form.documentoColaborador),
        rg: onlyDigits(form.rg),
        endereco: {
          ...form.endereco,
          cep: onlyDigits(form.endereco.cep || ''),
          numero:
            typeof form.endereco.numero === 'number'
              ? form.endereco.numero
              : Number(form.endereco.numero || 0),
        },
        saude: {
          ...form.saude,
          enumPCD,
          pcd: enumPCD === 5 ? 0 : 1,
        },
      }

      const response = await editarDadosColaboradorUseCase.execute(token, payload)
      if (response?.sucesso === false) {
        throw new Error(response?.mensagem ?? 'Não foi possível atualizar os dados pessoais.')
      }
      toast.success(response?.mensagem ?? 'Dados pessoais atualizados com sucesso!')
      onSaved?.()
      onOpenChange(false)
    } catch (error) {
      console.error('Erro ao salvar dados do colaborador', error)
      toast.error(error instanceof Error ? error.message : 'Erro ao salvar dados pessoais.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent
        className="max-w-[95vw] w-[1400px] max-h-[92vh] flex flex-col p-4 sm:p-6"
        aria-labelledby="editar-dados-pessoais-title"
        aria-describedby="editar-dados-pessoais-desc"
      >
        <DialogHeader>
          <DialogTitle id="editar-dados-pessoais-title">{modalTitle}</DialogTitle>
          <DialogDescription id="editar-dados-pessoais-desc">
            Atualize os dados pessoais, de saude, endereco e contato do candidato.
          </DialogDescription>
        </DialogHeader>

        {loading || !form ? (
          <div className="flex items-center justify-center gap-3 py-20 text-muted-foreground">
            <Spinner size={28} />
            <span>Carregando dados...</span>
          </div>
        ) : (
          <>
            <div className="flex-1 min-h-0 overflow-y-auto pr-1 space-y-4">
              <div className="rounded-lg border border-borderSoft bg-surfaceElevated">
                <Collapsible open={perfilOpen} onOpenChange={setPerfilOpen}>
                  <CollapsibleTrigger className="flex w-full items-center justify-between px-4 py-3 text-left">
                    <span className="font-semibold">Perfil Profissional</span>
                    <ChevronDown className={cn('h-4 w-4 transition-transform', perfilOpen && 'rotate-180')} />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <div className="border-t border-borderSoft px-4 py-4 space-y-4">
                      <div className="grid gap-4 md:grid-cols-4">
                        <div className="md:col-span-1">
                          <Label>Nome completo</Label>
                          <Input value={form.nomeCompleto} onChange={(e) => updateField('nomeCompleto', e.target.value)} />
                        </div>
                        <div>
                          <Label>RG</Label>
                          <Input
                            value={form.rg}
                            onChange={(e) => updateField('rg', formatRgInput(e.target.value))}
                            placeholder="12.345.678-9"
                          />
                        </div>
                        <div>
                          <Label>CPF</Label>
                          <Input
                            value={form.documentoColaborador}
                            onChange={(e) => updateField('documentoColaborador', formatCpfInput(e.target.value))}
                            placeholder="000.000.000-00"
                          />
                        </div>
                        <div>
                          <Label>Data de nascimento</Label>
                          <Input type="date" value={form.dataNascimento} onChange={(e) => updateField('dataNascimento', e.target.value)} />
                        </div>
                      </div>

                      <div className="grid gap-4 md:grid-cols-4">
                        <div>
                          <Label>Orientacao sexual</Label>
                          <Select
                            value={asSelectValue(form.orientacaoSexual)}
                            onValueChange={(v) => updateField('orientacaoSexual', fromSelectValue(v))}
                          >
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value={EMPTY_SELECT}>Selecione</SelectItem>
                              {ORIENTACOES.map((option) => <SelectItem key={option} value={option}>{option}</SelectItem>)}
                            </SelectContent>
                          </Select>
                        </div>
                        <div>
                          <Label>Etnia</Label>
                          <Select value={asSelectValue(form.etnia)} onValueChange={(v) => updateField('etnia', fromSelectValue(v))}>
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value={EMPTY_SELECT}>Selecione</SelectItem>
                              {ETNIAS.map((option) => <SelectItem key={option} value={option}>{option}</SelectItem>)}
                            </SelectContent>
                          </Select>
                        </div>
                        <div>
                          <Label>Escolaridade</Label>
                          <Select value={asSelectValue(form.escolaridade)} onValueChange={(v) => updateField('escolaridade', fromSelectValue(v))}>
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value={EMPTY_SELECT}>Selecione</SelectItem>
                              {ESCOLARIDADES.map((option) => <SelectItem key={option} value={option}>{option}</SelectItem>)}
                            </SelectContent>
                          </Select>
                        </div>
                        <div>
                          <Label>Estado civil</Label>
                          <Select value={asSelectValue(form.estadoCivil)} onValueChange={(v) => updateField('estadoCivil', fromSelectValue(v))}>
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value={EMPTY_SELECT}>Selecione</SelectItem>
                              {ESTADOS_CIVIS.map((option) => <SelectItem key={option} value={option}>{option}</SelectItem>)}
                            </SelectContent>
                          </Select>
                        </div>
                      </div>

                      <div className="grid gap-4 md:grid-cols-4">
                        <div>
                          <Label>Pessoa refugiada?</Label>
                          <Select
                            value={form.refugiado ? 'sim' : 'nao'}
                            onValueChange={(v) => updateField('refugiado', v === 'sim')}
                          >
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value="nao">Nao</SelectItem>
                              <SelectItem value="sim">Sim</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                        <div>
                          <Label>Genero</Label>
                          <Select value={asSelectValue(form.genero)} onValueChange={(v) => updateField('genero', fromSelectValue(v))}>
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value={EMPTY_SELECT}>Selecione</SelectItem>
                              {GENEROS.map((option) => <SelectItem key={option} value={option}>{option}</SelectItem>)}
                            </SelectContent>
                          </Select>
                        </div>
                        <div>
                          <Label>Nacionalidade</Label>
                          <Input value={form.nacionalidade} onChange={(e) => updateField('nacionalidade', e.target.value)} />
                        </div>
                        <div>
                          <Label>URL Linkedin</Label>
                          <Input value={form.urlLinkedin} onChange={(e) => updateField('urlLinkedin', e.target.value)} />
                        </div>
                      </div>

                      <div className="grid gap-4 md:grid-cols-4">
                        <div className="flex items-center justify-between rounded-lg border border-borderSoft px-3 py-2">
                          <Label>Candidato qualificado?</Label>
                          <Switch checked={form.qualificado} onCheckedChange={(v) => updateField('qualificado', Boolean(v))} />
                        </div>
                        <div className="flex items-center justify-between rounded-lg border border-borderSoft px-3 py-2">
                          <Label>Ativo?</Label>
                          <Switch checked={form.ativo} onCheckedChange={(v) => updateField('ativo', Boolean(v))} />
                        </div>
                        <div className="flex items-center justify-between rounded-lg border border-borderSoft px-3 py-2">
                          <Label>E candidato?</Label>
                          <Switch checked={form.candidato} onCheckedChange={(v) => updateField('candidato', Boolean(v))} />
                        </div>
                        <div className="flex items-center justify-between rounded-lg border border-borderSoft px-3 py-2">
                          <Label>Ver na busca aderencia?</Label>
                          <Switch checked={form.visualizarBuscaAderencia} onCheckedChange={(v) => updateField('visualizarBuscaAderencia', Boolean(v))} />
                        </div>
                      </div>

                      <div className="grid gap-4 md:grid-cols-4">
                        <div>
                          <Label>Matricula</Label>
                          <Input value={form.matricula} onChange={(e) => updateField('matricula', e.target.value)} />
                        </div>
                        <div>
                          <Label>Passaporte</Label>
                          <Input value={form.passaporte} onChange={(e) => updateField('passaporte', e.target.value)} />
                        </div>
                      </div>
                      <div>
                        <Label>Sobre</Label>
                        <Textarea
                          value={form.sobre}
                          onChange={(e) => updateField('sobre', e.target.value)}
                          maxLength={300}
                          rows={3}
                        />
                      </div>
                    </div>
                  </CollapsibleContent>
                </Collapsible>
              </div>

              <div className="rounded-lg border border-borderSoft bg-surfaceElevated">
                <Collapsible open={saudeOpen} onOpenChange={setSaudeOpen}>
                  <CollapsibleTrigger className="flex w-full items-center justify-between px-4 py-3 text-left">
                    <span className="font-semibold">Dados de Saude</span>
                    <ChevronDown className={cn('h-4 w-4 transition-transform', saudeOpen && 'rotate-180')} />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <div className="border-t border-borderSoft px-4 py-4 space-y-4">
                      <div className="grid gap-4 md:grid-cols-2">
                        <div>
                          <Label>Possui deficiência?</Label>
                          <Select
                            value={String(form.saude.enumPCD ?? 5)}
                            onValueChange={(v) => updateSaude('enumPCD', Number(v))}
                          >
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              {ENUM_PCD_OPTIONS.map((opt) => (
                                <SelectItem key={opt.value} value={String(opt.value)}>{opt.label}</SelectItem>
                              ))}
                            </SelectContent>
                          </Select>
                        </div>
                        <div>
                          <Label>Risco COVID?</Label>
                          <Select
                            value={String(form.saude.grupoDeRiscoCovid ?? 0)}
                            onValueChange={(v) => updateSaude('grupoDeRiscoCovid', Number(v))}
                          >
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value="0">Nao</SelectItem>
                              <SelectItem value="1">Sim</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                      </div>
                      <div>
                        <Label>Necessita de adaptacao?</Label>
                        <Input
                          value={form.saude.condicaoDeSaudeRelevante ?? ''}
                          onChange={(e) => updateSaude('condicaoDeSaudeRelevante', e.target.value)}
                        />
                      </div>
                    </div>
                  </CollapsibleContent>
                </Collapsible>
              </div>

              <div className="rounded-lg border border-borderSoft bg-surfaceElevated">
                <Collapsible open={enderecoOpen} onOpenChange={setEnderecoOpen}>
                  <CollapsibleTrigger className="flex w-full items-center justify-between px-4 py-3 text-left">
                    <span className="font-semibold">Endereco</span>
                    <ChevronDown className={cn('h-4 w-4 transition-transform', enderecoOpen && 'rotate-180')} />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <div className="border-t border-borderSoft px-4 py-4 space-y-4">
                      <div className="grid gap-4 md:grid-cols-4">
                        <div>
                          <Label>CEP</Label>
                          <Input
                            value={form.endereco.cep ?? ''}
                            onChange={(e) => updateEndereco('cep', formatCepInput(e.target.value))}
                            onBlur={handleBuscarCep}
                            placeholder="00000-000"
                          />
                        </div>
                        <div>
                          <Label>Numero</Label>
                          <Input
                            value={form.endereco.numero == null ? '' : String(form.endereco.numero)}
                            onChange={(e) => updateEndereco('numero', e.target.value ? Number(e.target.value) : null)}
                          />
                        </div>
                        <div className="md:col-span-2">
                          <Label>Rua</Label>
                          <Input value={form.endereco.endereco ?? ''} onChange={(e) => updateEndereco('endereco', e.target.value)} />
                        </div>
                      </div>
                      <div className="grid gap-4 md:grid-cols-4">
                        <div className="md:col-span-2">
                          <Label>Complemento</Label>
                          <Input value={form.endereco.complemento ?? ''} onChange={(e) => updateEndereco('complemento', e.target.value)} />
                        </div>
                        <div>
                          <Label>Cidade</Label>
                          <Input value={form.endereco.cidade ?? ''} onChange={(e) => updateEndereco('cidade', e.target.value)} />
                        </div>
                        <div>
                          <Label>Bairro</Label>
                          <Input value={form.endereco.bairro ?? ''} onChange={(e) => updateEndereco('bairro', e.target.value)} />
                        </div>
                      </div>
                      <div className="grid gap-4 md:grid-cols-3">
                        <div>
                          <Label>Estado</Label>
                          <Select value={asSelectValue(form.endereco.estado ?? '')} onValueChange={(v) => updateEndereco('estado', fromSelectValue(v))}>
                            <SelectTrigger><SelectValue placeholder="Selecione" /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value={EMPTY_SELECT}>Selecione</SelectItem>
                              {UF_OPTIONS.map((uf) => <SelectItem key={uf} value={uf}>{uf}</SelectItem>)}
                            </SelectContent>
                          </Select>
                        </div>
                        <div className="md:col-span-2">
                          <Label>Com quem mora?</Label>
                          <Input value={form.endereco.comQuemMora ?? ''} onChange={(e) => updateEndereco('comQuemMora', e.target.value)} />
                        </div>
                      </div>
                    </div>
                  </CollapsibleContent>
                </Collapsible>
              </div>

              <div className="rounded-lg border border-borderSoft bg-surfaceElevated">
                <Collapsible open={contatosOpen} onOpenChange={setContatosOpen}>
                  <CollapsibleTrigger className="flex w-full items-center justify-between px-4 py-3 text-left">
                    <span className="font-semibold">Contatos</span>
                    <ChevronDown className={cn('h-4 w-4 transition-transform', contatosOpen && 'rotate-180')} />
                  </CollapsibleTrigger>
                  <CollapsibleContent>
                    <div className="border-t border-borderSoft px-4 py-4 space-y-4">
                      <div className="grid gap-4 md:grid-cols-4">
                        <div>
                          <Label>Celular</Label>
                          <Input
                            value={form.contatoPrincipal}
                            onChange={(e) => updateField('contatoPrincipal', formatPhoneInput(e.target.value))}
                            placeholder="(11) 99999-9999"
                          />
                        </div>
                        <div>
                          <Label>Telefone</Label>
                          <Input
                            value={form.contatoOutro}
                            onChange={(e) => updateField('contatoOutro', formatPhoneInput(e.target.value))}
                            placeholder="(11) 99999-9999"
                          />
                        </div>
                        <div>
                          <Label>E-mail pessoal</Label>
                          <Input value={form.emailAlternativo} onChange={(e) => updateField('emailAlternativo', e.target.value)} />
                        </div>
                        <div>
                          <Label>E-mail corporativo</Label>
                          <Input value={form.email} onChange={(e) => updateField('email', e.target.value)} />
                        </div>
                      </div>
                    </div>
                  </CollapsibleContent>
                </Collapsible>
              </div>
            </div>

            <DialogFooter className="pt-4">
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)} disabled={saving}>
                Cancelar
              </Button>
              <Button type="button" onClick={handleSave} disabled={saving}>
                {saving ? <Spinner size={16} className="mr-2" /> : <Save className="h-4 w-4 mr-2" />}
                {saving ? 'Salvando...' : 'Atualizar dados'}
                {!saving && <Check className="h-4 w-4 ml-2" />}
              </Button>
            </DialogFooter>
          </>
        )}
      </DialogContent>
    </Dialog>
  )
}
