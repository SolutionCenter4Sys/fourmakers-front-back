import { useCallback, useEffect, useRef, useState } from 'react'
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
import { Spinner } from '@/components/ui/spinner'
import { Plus, Trash2, Sparkles } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  criarEncontroAiProximosPassos,
  atualizarEncontroAiProximosPassos,
  buscarEncontroAiPorEncontroId,
  buscarProximosPassosMoxe,
} from '@app/store/slices/agendasComerciaisSlice'
import { fetchColaboradores } from '@app/store/slices/colaboradoresSlice'
import { format } from 'date-fns'
import { toast } from 'sonner'
import type {
  ItemAgendaGestor,
  BuscarProximosPassosMoxeRequest,
  EncontroAiResponse,
} from '@domain/entities/AgendaGestor'
import type { Colaborador } from '@domain/entities/Colaborador'
import {
  parseDataAgendaParaExibicao,
  parseDataHoraAgendaParaExibicao,
} from '@shared/utils/timezoneAgendaUtils'
import { DropdownResponsavelProximosPassos, type ResponsavelValue } from './DropdownResponsavelProximosPassos'

const LIMITE_COLABORADORES_PAGINA = 20

interface PassoForm {
  id?: number
  texto: string
  nomeColaborador: string
  codigoColaborador?: string
  dataLimite: string
  statusAcoesId?: number
}

type EncontroAiExistente = NonNullable<ItemAgendaGestor['encontroAi']> & { id?: number }

const MAX_PASSOS = 10
const PASSOS_INICIAIS_IA = 3

/** Converte dataLimite da API (ISO, ex: 2026-03-10T00:00:00Z) para o formato do input type="date" (yyyy-MM-dd). */
function dataLimiteApiParaInput(iso?: string | null): string {
  if (!iso || typeof iso !== 'string') return ''
  const t = iso.trim()
  if (/^\d{4}-\d{2}-\d{2}/.test(t)) return t.slice(0, 10)
  try {
    const d = new Date(t)
    if (Number.isNaN(d.getTime())) return ''
    return format(d, 'yyyy-MM-dd')
  } catch {
    return ''
  }
}

/** Objeto bruto de passo vindo da API (pode ser PascalCase ou camelCase). */
type PassoApiBruto = Record<string, unknown>

/** Normaliza um passo da resposta da API para PassoForm (aceita PascalCase ou camelCase). */
function passoApiParaForm(p: PassoApiBruto): PassoForm {
  const id = (p.id ?? p.Id) as number | undefined
  const texto = ((p.texto ?? p.Texto) as string) ?? ''
  const nomeColaborador = ((p.nomeColaborador ?? p.NomeColaborador) as string) ?? ''
  const codigoColaborador = (p.codigoColaborador ?? p.CodigoColaborador) as string | undefined
  const dataLimite = dataLimiteApiParaInput((p.dataLimite ?? p.DataLimite) as string | undefined)
  const statusAcoesId = (p.statusAcoesId ?? p.StatusAcoesId) as number | undefined
  return {
    id: id != null && typeof id === 'number' ? id : undefined,
    texto,
    nomeColaborador,
    codigoColaborador: codigoColaborador && String(codigoColaborador).trim() ? String(codigoColaborador).trim() : undefined,
    dataLimite: dataLimite || '',
    statusAcoesId: statusAcoesId != null && typeof statusAcoesId === 'number' ? statusAcoesId : undefined,
  }
}

/** Monta o request Moxe (IntegracaoMoxeRequest) a partir do item da interação, ata e transcrições. */
function montarPayloadMoxe(
  item: ItemAgendaGestor,
  ata: string,
  principaisPontosAudio: string,
  encontroId: number,
): BuscarProximosPassosMoxeRequest {
  const dataDate = parseDataAgendaParaExibicao(item.data)
  const dataFormatada = dataDate ? format(dataDate, 'dd/MM/yyyy') : ''
  const inicioDate = parseDataHoraAgendaParaExibicao(item.dataInicio)
  const fimDate = parseDataHoraAgendaParaExibicao(item.dataFim)
  const horario =
    inicioDate && fimDate
      ? `${format(inicioDate, 'HH:mm')} - ${format(fimDate, 'HH:mm')}`
      : inicioDate
        ? format(inicioDate, 'HH:mm')
        : ''
  const local = (item.localizacao ?? item.linkReuniao ?? '').trim()

  const payload: BuscarProximosPassosMoxeRequest = {
    interacaoId: encontroId,
    interacao: {
      ata: ata.trim() || undefined,
      principaisPontosAudio: principaisPontosAudio.trim().slice(0, 1500) || undefined,
    },
    agenda: {
      titulo: (item.titulo ?? '').trim(),
      tipoInteracao: (item.tipoInteracao ?? '').trim(),
      data: dataFormatada,
      horario,
      local,
      descricao: (item.descricao ?? '').trim(),
      vagas: item.participantes ?? 0,
    },
  }

  if (item.cliente) {
    payload.cliente = {
      empresa: (item.cliente.nomeCliente ?? '').trim(),
      codigo: (item.cliente.codigoCliente ?? '').trim(),
      qtdAlocados: 0,
      gestores: (item.gestoresExternos ?? []).map((g) => ({
        nome: (g.nome ?? '').trim(),
        email: (g.email ?? '').trim(),
      })),
    }
  }

  if (item.colaboradores && item.colaboradores.length > 0) {
    payload.equipeFourtalentsParticipante = item.colaboradores
      .map((c) => (c.nome ?? '').trim())
      .filter(Boolean)
  }

  return payload
}

interface CriarOuEditarProximosPassosModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  encontroId: number
  encontroAiExistente?: EncontroAiExistente | null
  onSuccess?: () => void
  /** Ata/descrição da interação para enviar à IA (BuscarProximosPassosIntegracaoMoxe). Legado: step 4. */
  ataInteracao?: string
  /** Transcrições de áudio para a IA (máx. 1500 chars no legado). */
  transcricoesAudio?: string
  /** Item da interação/agenda para montar request completo (agenda, cliente, equipe) no Moxe. */
  itemInteracao?: ItemAgendaGestor | null
}

export function CriarOuEditarProximosPassosModal({
  open,
  onOpenChange,
  encontroId,
  encontroAiExistente,
  onSuccess,
  ataInteracao = '',
  transcricoesAudio = '',
  itemInteracao = null,
}: CriarOuEditarProximosPassosModalProps) {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const { colaboradores: colaboradoresPagina, status: colaboradoresStatus, hasMore: colaboradoresHasMoreRedux } = useAppSelector(
    (state) => state.colaboradores,
  )
  const orgId = user?.colaboradorOrg?.orgId ?? 0
  const isEdicao = !!encontroAiExistente

  const [resumo, setResumo] = useState('')
  const [passos, setPassos] = useState<PassoForm[]>([{ texto: '', nomeColaborador: '', dataLimite: '' }])
  const [erroResumo, setErroResumo] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [processandoIA, setProcessandoIA] = useState(false)
  const [encontroAiId, setEncontroAiId] = useState<number | null>(null)
  /** Dados completos do EncontroAi carregados para edição (encontroId, dataGerada e passos com id). */
  const [dadosEncontroAiCarregado, setDadosEncontroAiCarregado] = useState<EncontroAiResponse | null>(null)
  const [carregandoEncontroAi, setCarregandoEncontroAi] = useState(false)

  const [colaboradoresAccumulated, setColaboradoresAccumulated] = useState<Colaborador[]>([])
  const [colaboradoresSearch, setColaboradoresSearch] = useState('')
  const [debouncedColaboradoresSearch, setDebouncedColaboradoresSearch] = useState('')
  const lastFetchCursorRef = useRef<number>(-1)
  const colaboradoresLoading = colaboradoresStatus === 'loading'
  const colaboradoresHasMore = colaboradoresHasMoreRedux

  /** Em edição, sempre busca EncontroAi por id para ter passos com id e codigoColaborador (lista pode não trazer). */
  const carregarEncontroAiParaEdicao = useCallback(async () => {
    if (!token || !open || !encontroAiExistente || encontroAiId != null) return
    setCarregandoEncontroAi(true)
    try {
      const result = await dispatch(
        buscarEncontroAiPorEncontroId({ token, encontroId }),
      ).unwrap()
      if (result?.id) {
        setEncontroAiId(result.id)
        setDadosEncontroAiCarregado(result)
        if (result.passos?.length) {
          setPassos(
            result.passos.map((p) => passoApiParaForm((p ?? {}) as unknown as PassoApiBruto)),
          )
        }
      }
    } catch {
      toast.error('Não foi possível carregar os dados para edição')
    } finally {
      setCarregandoEncontroAi(false)
    }
  }, [token, open, encontroId, encontroAiExistente, encontroAiId, dispatch])

  const passoVazio = (): PassoForm => ({ texto: '', nomeColaborador: '', dataLimite: '' })
  const dadosParaPayloadAtualizacao = dadosEncontroAiCarregado ?? (encontroAiExistente as EncontroAiResponse | undefined)

  useEffect(() => {
    if (open && encontroAiExistente) {
      const resumoBruto = encontroAiExistente.resumo ?? (encontroAiExistente as PassoApiBruto).Resumo
      setResumo((resumoBruto as string) ?? '')
      // Não preencher passos aqui: carregarEncontroAiParaEdicao vai buscar dados completos (com id e codigoColaborador)
      void carregarEncontroAiParaEdicao()
    } else if (open && !encontroAiExistente) {
      setResumo('')
      setPassos([passoVazio()])
      setEncontroAiId(null)
      setDadosEncontroAiCarregado(null)
    }
  }, [open, encontroAiExistente, carregarEncontroAiParaEdicao])

  const adicionarPasso = useCallback(() => {
    setPassos((prev) => (prev.length >= MAX_PASSOS ? prev : [...prev, passoVazio()]))
  }, [])

  // Debounce da busca de colaboradores (igual ao modal de criar agendas)
  useEffect(() => {
    const timer = window.setTimeout(() => {
      setDebouncedColaboradoresSearch(colaboradoresSearch)
    }, 300)
    return () => clearTimeout(timer)
  }, [colaboradoresSearch])

  // Quando o termo de busca (debounced) mudar, recarregar primeira página de colaboradores
  useEffect(() => {
    if (!open || !token || orgId <= 0) return
    setColaboradoresAccumulated([])
    lastFetchCursorRef.current = 0
    dispatch(
      fetchColaboradores({
        token,
        orgId,
        cursor: 0,
        limite: LIMITE_COLABORADORES_PAGINA,
        nomeOuEmail: debouncedColaboradoresSearch.trim(),
        fourtalents: true,
      }),
    )
  }, [open, token, orgId, debouncedColaboradoresSearch, dispatch])

  // Acumular páginas de colaboradores quando o Redux retornar (scroll infinito)
  useEffect(() => {
    if (colaboradoresStatus !== 'succeeded' || lastFetchCursorRef.current < 0) return
    const cursor = lastFetchCursorRef.current
    if (cursor === 0) {
      setColaboradoresAccumulated(colaboradoresPagina)
    } else {
      setColaboradoresAccumulated((prev) => [...prev, ...colaboradoresPagina])
    }
    lastFetchCursorRef.current = -1
  }, [colaboradoresStatus, colaboradoresPagina])

  const onDropdownResponsavelOpen = useCallback(() => {
    if (colaboradoresAccumulated.length > 0 || colaboradoresLoading || !token || orgId <= 0) return
    lastFetchCursorRef.current = 0
    dispatch(
      fetchColaboradores({
        token,
        orgId,
        cursor: 0,
        limite: LIMITE_COLABORADORES_PAGINA,
        nomeOuEmail: debouncedColaboradoresSearch.trim(),
        fourtalents: true,
      }),
    )
  }, [colaboradoresAccumulated.length, colaboradoresLoading, token, orgId, debouncedColaboradoresSearch, dispatch])

  const onLoadMoreColaboradores = useCallback(() => {
    if (!colaboradoresHasMore || colaboradoresLoading || !token || orgId <= 0) return
    const nextCursor = colaboradoresAccumulated.length
    lastFetchCursorRef.current = nextCursor
    dispatch(
      fetchColaboradores({
        token,
        orgId,
        cursor: nextCursor,
        limite: LIMITE_COLABORADORES_PAGINA,
        nomeOuEmail: debouncedColaboradoresSearch.trim(),
        fourtalents: true,
      }),
    )
  }, [colaboradoresHasMore, colaboradoresLoading, token, orgId, colaboradoresAccumulated.length, debouncedColaboradoresSearch, dispatch])

  const setResponsavelPasso = useCallback((indice: number, value: ResponsavelValue | null) => {
    setPassos((prev) =>
      prev.map((p, i) =>
        i === indice
          ? { ...p, nomeColaborador: value?.nomeColaborador ?? '', codigoColaborador: value?.codigoColaborador }
          : p,
      ),
    )
  }, [])

  const processarComIA = useCallback(async () => {
    if (!token) return
    setProcessandoIA(true)
    setErroResumo('')
    try {
      const ata = (ataInteracao ?? '').trim() || resumo.trim()
      const principaisPontosAudio = (transcricoesAudio ?? '').trim().slice(0, 1500)
      const payload: BuscarProximosPassosMoxeRequest = itemInteracao
        ? montarPayloadMoxe(itemInteracao, ata || 'Informação não fornecida', principaisPontosAudio, encontroId)
        : {
            ata: ata || undefined,
            principaisPontosAudio: principaisPontosAudio || undefined,
            interacaoId: encontroId,
          }
      const result = await dispatch(
        buscarProximosPassosMoxe({
          token,
          payload,
        }),
      ).unwrap()
      if (result.resumo?.trim()) {
        if (isEdicao && resumo.trim()) {
          setResumo(`${resumo}\n\n— Reprocessado —\n${result.resumo.trim()}`)
        } else {
          setResumo(result.resumo.trim())
        }
      }
      const novosPassos = (result.passos ?? []).slice(0, PASSOS_INICIAIS_IA).map((texto) => ({
        texto: typeof texto === 'string' ? texto : '',
        nomeColaborador: '',
        dataLimite: '',
      }))
      if (novosPassos.length > 0) {
        setPassos((prev) => {
          const existentes = isEdicao ? prev.filter((p) => p.texto.trim()) : []
          const merged = [...existentes, ...novosPassos].slice(0, MAX_PASSOS)
          return merged.length ? merged : [{ texto: '', nomeColaborador: '', dataLimite: '' }]
        })
      }
      toast.success('Resumo e próximos passos gerados com IA.')
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Não foi possível gerar o resumo com IA.')
    } finally {
      setProcessandoIA(false)
    }
  }, [token, dispatch, ataInteracao, transcricoesAudio, encontroId, isEdicao, resumo, itemInteracao])

  const removerPasso = useCallback((indice: number) => {
    setPassos((prev) => (prev.length <= 1 ? prev : prev.filter((_, i) => i !== indice)))
  }, [])

  const atualizarPasso = useCallback((indice: number, campo: keyof PassoForm, valor: string) => {
    setPassos((prev) =>
      prev.map((p, i) => (i === indice ? { ...p, [campo]: valor } : p)),
    )
  }, [])

  const validar = useCallback((): boolean => {
    const r = resumo.trim()
    if (!r) {
      setErroResumo('Resumo é obrigatório.')
      return false
    }
    setErroResumo('')
    return true
  }, [resumo])

  const handleSalvar = useCallback(async () => {
    if (!token || !validar()) return
    setIsSubmitting(true)
    try {
      const passosFiltrados = passos
        .filter((p) => p.texto.trim() !== '')
        .map((p) => ({
          id: p.id,
          texto: p.texto.trim(),
          nomeColaborador: p.nomeColaborador.trim() || undefined,
          codigoColaborador: p.codigoColaborador?.trim() || undefined,
          dataLimite: p.dataLimite.trim() || undefined,
          statusAcoesId: 2,
        }))

      if (isEdicao && encontroAiId != null) {
        const encontroIdPayload = dadosParaPayloadAtualizacao?.encontroId ?? encontroId
        const dataGeradaPayload = dadosParaPayloadAtualizacao?.dataGerada ?? undefined
        await dispatch(
          atualizarEncontroAiProximosPassos({
            token,
            payload: {
              id: encontroAiId,
              encontroId: encontroIdPayload,
              dataGerada: dataGeradaPayload ?? undefined,
              resumo: resumo.trim(),
              passos: passosFiltrados.length
                ? passosFiltrados.map((p) => ({
                    id: p.id,
                    texto: p.texto,
                    nomeColaborador: p.nomeColaborador,
                    codigoColaborador: p.codigoColaborador,
                    dataLimite: p.dataLimite,
                    statusAcoesId: 2,
                  }))
                : undefined,
            },
          }),
        ).unwrap()
        toast.success('Próximos passos atualizados.')
      } else {
        await dispatch(
          criarEncontroAiProximosPassos({
            token,
            payload: {
              encontroId,
              resumo: resumo.trim(),
              passos: passosFiltrados.length ? passosFiltrados : undefined,
            },
          }),
        ).unwrap()
        toast.success('Próximos passos criados.')
      }
      onSuccess?.()
      onOpenChange(false)
    } catch (err) {
      toast.error(
        err instanceof Error ? err.message : 'Erro ao salvar próximos passos.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }, [
    token,
    validar,
    passos,
    resumo,
    isEdicao,
    encontroAiId,
    encontroId,
    dadosParaPayloadAtualizacao,
    dispatch,
    onSuccess,
    onOpenChange,
  ])

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {isEdicao ? 'Editar próximos passos' : 'Criar próximos passos'}
          </DialogTitle>
          <DialogDescription>
            {isEdicao
              ? 'Altere o resumo e a lista de próximos passos desta interação.'
              : 'Preencha o resumo e os próximos passos sugeridos para esta interação.'}
          </DialogDescription>
        </DialogHeader>

        {carregandoEncontroAi ? (
          <div className="flex justify-center py-8">
            <Spinner className="text-primary" />
          </div>
        ) : (
          <div className="space-y-4 py-2">
            <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={processarComIA}
                disabled={processandoIA}
                className="rounded-full border-primary/50 text-primary hover:bg-primary/10"
              >
                {processandoIA ? (
                  <Spinner className="h-4 w-4 mr-2" />
                ) : (
                  <Sparkles className="h-4 w-4 mr-2" />
                )}
                {processandoIA ? 'Processando…' : 'Processar com IA'}
              </Button>
              <p className="text-xs text-muted-foreground">
                Gera resumo e sugestões de próximos passos a partir da ata da interação.
              </p>
            </div>
            <div className="space-y-2">
              <Label htmlFor="resumo-proximos-passos">Resumo</Label>
              <Textarea
                id="resumo-proximos-passos"
                value={resumo}
                onChange={(e) => setResumo(e.target.value)}
                placeholder="Resumo da interação (gerado por IA ou preenchido manualmente)"
                className="min-h-[100px] rounded-lg"
                aria-invalid={!!erroResumo}
              />
              {erroResumo && (
                <p className="text-sm text-destructive" role="alert">
                  {erroResumo}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between">
                <Label>Próximos passos (máx. {MAX_PASSOS})</Label>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={adicionarPasso}
                  disabled={passos.length >= MAX_PASSOS}
                  className="rounded-full"
                >
                  <Plus className="h-4 w-4 mr-1" />
                  Adicionar
                </Button>
              </div>
              <div className="space-y-3">
                {passos.map((passo, indice) => (
                  <div
                    key={indice}
                    className="flex flex-col gap-2 rounded-lg border border-border p-3 bg-surfaceElevated"
                  >
                    <div className="flex justify-end">
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={() => removerPasso(indice)}
                        disabled={passos.length <= 1}
                        aria-label={`Remover passo ${indice + 1}`}
                        className="text-destructive hover:text-destructive"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                    <div className="grid grid-cols-1 gap-2 sm:grid-cols-3">
                      <div className="sm:col-span-2">
                        <Label htmlFor={`passo-texto-${indice}`}>Descrição</Label>
                        <Input
                          id={`passo-texto-${indice}`}
                          value={passo.texto}
                          onChange={(e) =>
                            atualizarPasso(indice, 'texto', e.target.value)
                          }
                          placeholder="Texto do próximo passo"
                          className="rounded-lg"
                        />
                      </div>
                      <div>
                        <Label htmlFor={`passo-responsavel-${indice}`}>
                          Responsável
                        </Label>
                        <DropdownResponsavelProximosPassos
                          id={`passo-responsavel-${indice}`}
                          value={
                            passo.nomeColaborador || passo.codigoColaborador
                              ? { nomeColaborador: passo.nomeColaborador, codigoColaborador: passo.codigoColaborador }
                              : null
                          }
                          onChange={(v) => setResponsavelPasso(indice, v)}
                          colaboradores={colaboradoresAccumulated}
                          loading={colaboradoresLoading}
                          hasMore={colaboradoresHasMore}
                          search={colaboradoresSearch}
                          onSearchChange={setColaboradoresSearch}
                          onOpenChange={(isOpen) => {
                            if (isOpen) onDropdownResponsavelOpen()
                          }}
                          onLoadMore={onLoadMoreColaboradores}
                          disabled={isSubmitting}
                          placeholder="Selecione o responsável"
                        />
                      </div>
                    </div>
                    <div className="w-full sm:w-48">
                      <Label htmlFor={`passo-prazo-${indice}`}>Data limite</Label>
                      <Input
                        id={`passo-prazo-${indice}`}
                        type="date"
                        value={passo.dataLimite}
                        onChange={(e) =>
                          atualizarPasso(indice, 'dataLimite', e.target.value)
                        }
                        className="rounded-lg"
                      />
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={isSubmitting}
          >
            Cancelar
          </Button>
          <Button
            type="button"
            onClick={handleSalvar}
            disabled={isSubmitting || carregandoEncontroAi}
            className="rounded-full"
          >
            {isSubmitting ? (
              <Spinner size={18} className="text-primary-foreground" />
            ) : (
              'Salvar'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
