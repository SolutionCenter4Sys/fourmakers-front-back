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
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Download, Tag, X } from 'lucide-react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { inserirInteracaoIa, atualizarInteracaoIa } from '@app/store/slices/agendasComerciaisSlice'
import { toast } from 'sonner'
import { container } from '@core/di/container'
import { InserirArquivoEncontroUseCase } from '@domain/usecases/InserirArquivoEncontroUseCase'
import { DeletarArquivoEncontroUseCase } from '@domain/usecases/DeletarArquivoEncontroUseCase'
import type {
  ItemAgendaGestor,
  InserirInteracaoIaPayload,
  AtualizarInteracaoIaPayload,
  CategoriaAssuntoComSubModel,
} from '@domain/entities/AgendaGestor'
import { ListarCategoriasAssuntoComSubUseCase } from '@domain/usecases/ListarCategoriasAssuntoComSubUseCase'
import { ListarCategoriasSubPorInteracaoIdUseCase } from '@domain/usecases/ListarCategoriasSubPorInteracaoIdUseCase'
import { InserirInteracaoCategoriaUseCase } from '@domain/usecases/InserirInteracaoCategoriaUseCase'
import { AtualizarInteracaoCategoriaUseCase } from '@domain/usecases/AtualizarInteracaoCategoriaUseCase'
import { EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO } from './agendasComerciaisConstants'
import { processarUrlComToken, baixarArquivoPorUrl } from '@shared/utils/urlUtils'

/** Arquivo já anexado ao encontro (exibido ao editar interação). */
type ArquivoExistente = NonNullable<ItemAgendaGestor['arquivos']>[number]

/** Extensões permitidas para anexos (imagens, áudio, documentos). */
const EXTENSOES_PERMITIDAS = [
  '.png',
  '.jpg',
  '.jpeg',
  '.gif',
  '.webp',
  '.m4a',
  '.mp3',
  '.wav',
  '.pdf',
]
const TAMANHO_MAXIMO_MB = 20

interface ArquivoPendente {
  file: File
  nomeArquivo: string
  tipoArquivo: string
  transcricao?: string
}

/** Item de categoria/subcategoria selecionada para a interação. */
interface SelectedCategoriaItem {
  interacaoCategoriaId?: number
  categoriaAssuntoId: number
  subCategoriaAssuntoId?: number
}

interface NovaInteracaoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  interacaoParaEditar?: ItemAgendaGestor | null
  agendaId: number // ID da agenda relacionada (obrigatório)
  /** Nome da agenda para compor o nome padrão do arquivo no upload: [nomeAgenda]-[numeroInteracao]-timestamp.ext */
  nomeAgenda?: string
  onSuccess?: () => void
}

export function NovaInteracaoModal({
  open,
  onOpenChange,
  interacaoParaEditar,
  agendaId,
  onSuccess,
}: NovaInteracaoModalProps) {
  const dispatch = useAppDispatch()
  const { token } = useAppSelector((state) => state.auth)
  const isEditing = !!interacaoParaEditar

  // Estados do formulário
  const [tituloInteracao, setTituloInteracao] = useState('')
  const [descricaoInteracao, setDescricaoInteracao] = useState('')
  const [resumoInteracao, setResumoInteracao] = useState('')
  const [errors, setErrors] = useState<{ [campo: string]: string }>({})
  /** Loading local: mantém o modal aberto com overlay para o usuário não perder a referência da agenda. */
  const [isSubmitting, setIsSubmitting] = useState(false)
  /** Arquivos a serem enviados após criar/atualizar o encontro. */
  const [arquivosPendentes, setArquivosPendentes] = useState<ArquivoPendente[]>([])
  /** Arquivos já anexados ao encontro (ao abrir/editar interação); atualizado ao excluir. */
  const [arquivosExistentes, setArquivosExistentes] = useState<ArquivoExistente[]>([])
  const [deletandoArquivoId, setDeletandoArquivoId] = useState<number | null>(null)
  const [baixandoArquivoId, setBaixandoArquivoId] = useState<number | null>(null)
  const inputArquivoRef = useRef<HTMLInputElement>(null)

  // Categoria e Subcategoria (Jornada Comercial)
  const [categoriasCatalogo, setCategoriasCatalogo] = useState<CategoriaAssuntoComSubModel[]>([])
  const [selectedCategorias, setSelectedCategorias] = useState<SelectedCategoriaItem[]>([])
  const [categoriaSelecionadaId, setCategoriaSelecionadaId] = useState<string>('')
  const [subcategoriaSelecionadaId, setSubcategoriaSelecionadaId] = useState<string>('')
  const [loadingCategorias, setLoadingCategorias] = useState(false)

  const validarArquivo = useCallback((file: File): string | null => {
    const ext = '.' + (file.name.split('.').pop()?.toLowerCase() ?? '')
    if (!EXTENSOES_PERMITIDAS.includes(ext)) {
      return `Formato não permitido. Use: ${EXTENSOES_PERMITIDAS.join(', ')}`
    }
    const maxBytes = TAMANHO_MAXIMO_MB * 1024 * 1024
    if (file.size > maxBytes) {
      return `Arquivo muito grande. Máximo: ${TAMANHO_MAXIMO_MB} MB`
    }
    return null
  }, [])

  const adicionarArquivos = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const files = e.target.files
      if (!files?.length) return
      const novos: ArquivoPendente[] = []
      for (let i = 0; i < files.length; i++) {
        const file = files[i]
        const erro = validarArquivo(file)
        if (erro) {
          toast.error('Arquivo inválido', { description: erro })
          continue
        }
        const nome = file.name
        const ext = '.' + (file.name.split('.').pop()?.toLowerCase() ?? '')
        novos.push({ file, nomeArquivo: nome, tipoArquivo: ext })
      }
      if (novos.length) {
        setArquivosPendentes((prev) => [...prev, ...novos])
      }
      e.target.value = ''
    },
    [validarArquivo],
  )

  const removerArquivoPendente = useCallback((index: number) => {
    setArquivosPendentes((prev) => prev.filter((_, i) => i !== index))
  }, [])

  // Preencher formulário e arquivos existentes ao abrir (nova ou edição)
  useEffect(() => {
    if (interacaoParaEditar && open) {
      setTituloInteracao(interacaoParaEditar.titulo || '')
      setDescricaoInteracao(interacaoParaEditar.descricao || '')
      setResumoInteracao(interacaoParaEditar.resumoInteracao || '')
      setErrors({})
      setArquivosPendentes([])
      setArquivosExistentes(interacaoParaEditar.arquivos ?? [])
      // Limpa categorias para serem preenchidas pelo efeito que carrega por interacaoId
      setSelectedCategorias([])
    } else if (!interacaoParaEditar && open) {
      setTituloInteracao('')
      setDescricaoInteracao('')
      setResumoInteracao('')
      setErrors({})
      setArquivosPendentes([])
      setArquivosExistentes([])
      setSelectedCategorias([])
    }
  }, [interacaoParaEditar, open])

  // Carregar catálogo de categorias e categorias da interação (ao editar)
  useEffect(() => {
    if (!open || !token) return
    setLoadingCategorias(true)
    const listarCatalogo = async () => {
      try {
        const useCase = container.resolve(ListarCategoriasAssuntoComSubUseCase)
        const list = await useCase.execute(token)
        setCategoriasCatalogo(Array.isArray(list) ? list : [])
      } catch {
        setCategoriasCatalogo([])
      } finally {
        setLoadingCategorias(false)
      }
    }
    void listarCatalogo()
  }, [open, token])

  // Carregar categorias da interação ao abrir em modo edição
  useEffect(() => {
    if (!open || !token || !interacaoParaEditar?.id) return
    const interacaoId = parseInt(interacaoParaEditar.id, 10)
    if (!Number.isFinite(interacaoId)) return
    const useCase = container.resolve(ListarCategoriasSubPorInteracaoIdUseCase)
    useCase
      .execute(token, interacaoId)
      .then((list) => {
        setSelectedCategorias(
          (Array.isArray(list) ? list : []).map((c) => ({
            interacaoCategoriaId: c.interacaoCategoriaId,
            categoriaAssuntoId: c.categoriaAssuntoId ?? 0,
            subCategoriaAssuntoId: c.subCategoriaAssuntoId ?? undefined,
          })),
        )
      })
      .catch(() => setSelectedCategorias([]))
  }, [open, token, interacaoParaEditar?.id])

  const removerArquivoExistente = useCallback(
    async (arquivoId: number) => {
      if (!token) return
      setDeletandoArquivoId(arquivoId)
      try {
        const useCase = container.resolve(DeletarArquivoEncontroUseCase)
        await useCase.execute(token, arquivoId)
        setArquivosExistentes((prev) => prev.filter((a) => a.id !== arquivoId))
        toast.success('Arquivo removido')
      } catch (err) {
        toast.error('Falha ao remover arquivo', {
          description: err instanceof Error ? err.message : undefined,
        })
      } finally {
        setDeletandoArquivoId(null)
      }
    },
    [token],
  )

  const validarFormulario = (): boolean => {
    const newErrors: { [campo: string]: string } = {}
    const erros: string[] = []

    if (!agendaId) {
      erros.push('Agenda')
      newErrors.agendaId = 'Agenda é obrigatória para criar uma interação'
    }

    if (!tituloInteracao.trim()) {
      erros.push('Título da interação')
      newErrors.tituloInteracao = 'Título é obrigatório'
    }

    setErrors(newErrors)

    if (erros.length > 0) {
      const mensagem =
        erros.length === 1
          ? `Campo obrigatório não preenchido: ${erros[0]}`
          : `Campos obrigatórios não preenchidos:\n\n${erros.map((e) => `• ${e}`).join('\n')}`

      toast.error('Validação de dados', {
        description: mensagem,
        duration: 6000,
      })
      return false
    }

    return true
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!token) {
      toast.error('Erro', { description: 'Token de autenticação não encontrado' })
      return
    }

    if (!validarFormulario()) {
      return
    }

    setIsSubmitting(true)
    try {
      let encontroId: number

      if (isEditing && interacaoParaEditar?.id) {
        // Id da interação existente: usado para atualizar e para vincular arquivos (encontroId obrigatório no upload)
        const idInteracaoExistente = parseInt(interacaoParaEditar.id, 10)
        encontroId = Number.isFinite(idInteracaoExistente) ? idInteracaoExistente : 0

        const payload: AtualizarInteracaoIaPayload = {
          id: idInteracaoExistente,
          agendaId: (agendaId || interacaoParaEditar.agendaId) ?? 0,
          tituloInteracao: tituloInteracao.trim(),
          descricaoInteracao: descricaoInteracao.trim() || undefined,
          resumoInteracao: resumoInteracao.trim() || undefined,
        }
        await dispatch(atualizarInteracaoIa({ token, payload })).unwrap()
        toast.success('Interação atualizada', { description: 'A interação foi atualizada com sucesso' })
      } else {
        if (!agendaId) {
          toast.error('Erro', { description: 'Agenda é obrigatória para criar uma interação' })
          setIsSubmitting(false)
          return
        }
        const payload: InserirInteracaoIaPayload = {
          agendaId: agendaId,
          tituloInteracao: tituloInteracao.trim(),
          descricaoInteracao: descricaoInteracao.trim() || undefined,
          resumoInteracao: resumoInteracao.trim() || undefined,
        }
        const raw = await dispatch(inserirInteracaoIa({ token, payload })).unwrap() as
          | { retorno?: { id?: number }; Retorno?: { id?: number }; id?: number }
          | null
        // Resposta de CriarEncontro (envelope ou direta); id do encontro é obrigatório para InserirArquivo
        const inner = raw?.retorno ?? (raw as { Retorno?: { id?: number } } | null)?.Retorno ?? raw
        const idCriado = inner && typeof inner === 'object' && 'id' in inner ? (inner as { id?: number }).id : undefined
        encontroId = typeof idCriado === 'number' && Number.isFinite(idCriado) ? idCriado : 0
        toast.success('Interação criada', { description: 'A interação foi criada com sucesso' })
      }

      const idInteracaoValido = typeof encontroId === 'number' && Number.isFinite(encontroId) && encontroId > 0
      if (!idInteracaoValido) {
        toast.error('Erro ao obter ID da interação', {
          description: 'Não foi possível anexar arquivos. Tente editar a interação e anexar novamente.',
        })
      }

      // Upload de anexos após criar/atualizar encontro (exige id da interação = tb_interacoes.id)
      if (idInteracaoValido && arquivosPendentes.length > 0) {
        const useCase = container.resolve(InserirArquivoEncontroUseCase)
        let falhas = 0
        for (const item of arquivosPendentes) {
          try {
            await useCase.execute(token, {
              encontroId,
              file: item.file,
              nomeArquivo: item.nomeArquivo,
              tipoArquivo: item.tipoArquivo,
              transcricao: item.transcricao,
            })
          } catch (err) {
            falhas++
            toast.error('Falha ao anexar arquivo', {
              description: item.nomeArquivo + (err instanceof Error ? `: ${err.message}` : ''),
            })
          }
        }
        if (falhas === 0) {
          toast.success('Anexos enviados', { description: `${arquivosPendentes.length} arquivo(s) anexado(s).` })
        } else if (falhas < arquivosPendentes.length) {
          toast.warning('Alguns anexos não foram enviados', { description: `${falhas} de ${arquivosPendentes.length} falharam.` })
        }
      }

      // Sincronizar categorias/subcategorias (Jornada Comercial)
      if (idInteracaoValido && selectedCategorias.length > 0) {
        const inserirUseCase = container.resolve(InserirInteracaoCategoriaUseCase)
        const atualizarUseCase = container.resolve(AtualizarInteracaoCategoriaUseCase)
        for (const item of selectedCategorias) {
          try {
            if (item.interacaoCategoriaId != null) {
              await atualizarUseCase.execute(token, {
                interacaoCategoriaId: item.interacaoCategoriaId,
                categoriaAssuntoId: item.categoriaAssuntoId,
                subCategoriaAssuntoId: item.subCategoriaAssuntoId ?? null,
              })
            } else {
              await inserirUseCase.execute(token, {
                interacaoId: encontroId,
                categoriaAssuntoId: item.categoriaAssuntoId,
                subCategoriaAssuntoId: item.subCategoriaAssuntoId ?? null,
              })
            }
          } catch {
            // não bloqueia o fluxo
          }
        }
      }

      await onSuccess?.()
      onOpenChange(false)
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao salvar interação',
      })
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(open) => {
        if (!isSubmitting) onOpenChange(open)
      }}
    >
      <DialogContent
        className="max-w-2xl max-h-[90vh] overflow-y-auto"
      >
        {/* Overlay de loading: modal permanece aberto e o usuário mantém referência da agenda */}
        {isSubmitting && (
          <div
            className="absolute inset-0 z-10 flex items-center justify-center rounded-lg bg-background/80 backdrop-blur-[1px]"
            aria-busy="true"
            aria-live="polite"
          >
            <div className="flex flex-col items-center gap-3">
              <Spinner className="text-primary h-8 w-8" />
              <p className="text-sm font-medium text-muted-foreground">Salvando interação...</p>
            </div>
          </div>
        )}

        <DialogHeader>
          <DialogTitle>{isEditing ? 'Editar Interação' : 'Nova Interação'}</DialogTitle>
          <DialogDescription>
            {isEditing
              ? 'Atualize os dados da interação comercial.'
              : 'Preencha os campos para criar uma nova interação comercial.'}
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="tituloInteracao">
              Título da Interação <span className="text-destructive">*</span>
            </Label>
            <Input
              id="tituloInteracao"
              value={tituloInteracao}
              onChange={(e) => {
                setTituloInteracao(e.target.value)
                if (errors.tituloInteracao) {
                  const newErrors = { ...errors }
                  delete newErrors.tituloInteracao
                  setErrors(newErrors)
                }
              }}
              placeholder="Ex: Reunião de alinhamento com cliente"
              error={!!errors.tituloInteracao}
              disabled={isSubmitting}
            />
            {errors.tituloInteracao && <p className="text-sm text-destructive">{errors.tituloInteracao}</p>}
          </div>

          <div className="space-y-2">
            <Label htmlFor="descricaoInteracao">Descrição</Label>
            <Textarea
              id="descricaoInteracao"
              value={descricaoInteracao}
              onChange={(e) => setDescricaoInteracao(e.target.value)}
              placeholder="Descreva os detalhes da interação..."
              rows={4}
              disabled={isSubmitting}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="resumoInteracao">Resumo da Interação</Label>
            <Textarea
              id="resumoInteracao"
              value={resumoInteracao}
              onChange={(e) => setResumoInteracao(e.target.value)}
              placeholder="Resumo ou pontos principais da interação..."
              rows={3}
              disabled={isSubmitting}
            />
          </div>

          {/* Categoria e Subcategoria (Jornada Comercial) — opcionais */}
          <div className="space-y-2">
            <div className="flex items-center gap-2">
              <Tag className="h-4 w-4 shrink-0 text-muted-foreground" />
              <Label className="text-muted-foreground font-normal">Categoria e Subcategoria (opcional)</Label>
            </div>
            {loadingCategorias ? (
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Spinner size={14} />
                Carregando catálogo...
              </div>
            ) : (
              <>
                <div className="flex flex-wrap items-end gap-2">
                  <div className="min-w-[180px] space-y-1.5">
                    <Label className="text-xs text-muted-foreground">Categoria</Label>
                    <Select
                      value={categoriaSelecionadaId}
                      onValueChange={(v) => {
                        setCategoriaSelecionadaId(v)
                        setSubcategoriaSelecionadaId('')
                      }}
                      disabled={isSubmitting}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione a categoria" />
                      </SelectTrigger>
                      <SelectContent>
                        {categoriasCatalogo.map((c) => (
                          <SelectItem key={c.id ?? 0} value={String(c.id ?? '')}>
                            {c.descricao ?? ''}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="min-w-[180px] space-y-1.5">
                    <Label className="text-xs text-muted-foreground">Subcategoria</Label>
                    <Select
                      value={subcategoriaSelecionadaId}
                      onValueChange={setSubcategoriaSelecionadaId}
                      disabled={isSubmitting || !categoriaSelecionadaId}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Opcional" />
                      </SelectTrigger>
                      <SelectContent>
                        {categoriasCatalogo
                          .find((c) => String(c.id) === categoriaSelecionadaId)
                          ?.subcategorias?.map((s) => (
                            <SelectItem key={s.id ?? 0} value={String(s.id ?? '')}>
                              {s.descricao ?? ''}
                            </SelectItem>
                          )) ?? []}
                      </SelectContent>
                    </Select>
                  </div>
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    disabled={isSubmitting || !categoriaSelecionadaId}
                    onClick={() => {
                      const catId = parseInt(categoriaSelecionadaId, 10)
                      if (!Number.isFinite(catId)) return
                      setSelectedCategorias((prev) => [
                        ...prev,
                        {
                          categoriaAssuntoId: catId,
                          subCategoriaAssuntoId: subcategoriaSelecionadaId
                            ? parseInt(subcategoriaSelecionadaId, 10)
                            : undefined,
                        },
                      ])
                      setCategoriaSelecionadaId('')
                      setSubcategoriaSelecionadaId('')
                    }}
                  >
                    Adicionar
                  </Button>
                </div>
                {selectedCategorias.length > 0 && (
                  <ul className="mt-2 space-y-1.5 rounded-lg border border-border p-3">
                    {selectedCategorias.map((item, idx) => {
                      const cat = categoriasCatalogo.find((c) => c.id === item.categoriaAssuntoId)
                      const sub = cat?.subcategorias?.find((s) => s.id === item.subCategoriaAssuntoId)
                      const label = sub
                        ? `${cat?.descricao ?? ''} → ${sub.descricao ?? ''}`
                        : (cat?.descricao ?? 'Categoria')
                      return (
                        <li
                          key={`${item.categoriaAssuntoId}-${item.subCategoriaAssuntoId ?? 0}-${idx}`}
                          className="flex items-center justify-between gap-2 text-sm"
                        >
                          <span className="text-muted-foreground">{label}</span>
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            className="h-7 w-7 shrink-0 p-0 text-destructive hover:text-destructive"
                            disabled={isSubmitting}
                            onClick={() =>
                              setSelectedCategorias((prev) => prev.filter((_, i) => i !== idx))
                            }
                            aria-label="Remover categoria"
                          >
                            <X className="h-3.5 w-3.5" />
                          </Button>
                        </li>
                      )
                    })}
                  </ul>
                )}
              </>
            )}
          </div>

          {arquivosExistentes.length > 0 && (
            <div className="space-y-2">
              <Label>Arquivos anexados</Label>
              <ul className="rounded-lg border border-border p-3 space-y-2">
                {arquivosExistentes.map((arquivo) => {
                  const link = arquivo.linkImagemInteracao ?? arquivo.linkAudioInteracao ?? null
                  const urlComToken = link ? processarUrlComToken(link, token) ?? link : null
                  return (
                    <li
                      key={arquivo.id}
                      className="flex items-center justify-between gap-2 text-sm text-muted-foreground"
                    >
                      {urlComToken ? (
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="text-primary hover:text-primary shrink-0"
                          disabled={isSubmitting || baixandoArquivoId === arquivo.id}
                          onClick={async () => {
                            setBaixandoArquivoId(arquivo.id)
                            try {
                              await baixarArquivoPorUrl(urlComToken)
                              toast.success('Download iniciado')
                            } catch {
                              toast.error('Falha ao baixar o arquivo')
                            } finally {
                              setBaixandoArquivoId(null)
                            }
                          }}
                        >
                          {baixandoArquivoId === arquivo.id ? (
                            <Spinner size={14} className="text-primary" />
                          ) : (
                            <Download className="h-3.5 w-3.5 shrink-0" />
                          )}
                          <span className="ml-1.5">Baixar</span>
                        </Button>
                      ) : link ? (
                        <span className="truncate text-muted-foreground">Arquivo (requer login)</span>
                      ) : (
                        <span className="truncate">Arquivo (id {arquivo.id})</span>
                      )}
                      {EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO && (
                        <Button
                          type="button"
                          variant="ghost"
                          size="sm"
                          className="shrink-0 text-destructive hover:text-destructive"
                          disabled={isSubmitting || deletandoArquivoId === arquivo.id}
                          onClick={() => removerArquivoExistente(arquivo.id)}
                          aria-label={`Remover arquivo ${arquivo.id}`}
                        >
                          {deletandoArquivoId === arquivo.id ? (
                            <Spinner size={14} className="text-destructive" />
                          ) : (
                            'Remover'
                          )}
                        </Button>
                      )}
                    </li>
                  )
                })}
              </ul>
            </div>
          )}

          <div className="space-y-2">
            <Label>Anexos</Label>
            <input
              ref={inputArquivoRef}
              type="file"
              multiple
              accept={EXTENSOES_PERMITIDAS.join(',')}
              className="hidden"
              onChange={adicionarArquivos}
              aria-label="Selecionar arquivos para anexar"
            />
            <Button
              type="button"
              variant="outline"
              size="sm"
              disabled={isSubmitting}
              onClick={() => inputArquivoRef.current?.click()}
            >
              Adicionar arquivo
            </Button>
            {arquivosPendentes.length > 0 && (
              <ul className="mt-2 space-y-2 rounded-lg border border-border p-3">
                {arquivosPendentes.map((item, index) => (
                  <li
                    key={`${item.nomeArquivo}-${index}`}
                    className="flex items-center justify-between gap-2 text-sm text-muted-foreground"
                  >
                    <span className="truncate">{item.nomeArquivo}</span>
                    <Button
                      type="button"
                      variant="ghost"
                      size="sm"
                      className="shrink-0 text-destructive hover:text-destructive"
                      disabled={isSubmitting}
                      onClick={() => removerArquivoPendente(index)}
                      aria-label={`Remover ${item.nomeArquivo}`}
                    >
                      Remover
                    </Button>
                  </li>
                ))}
              </ul>
            )}
          </div>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => onOpenChange(false)} disabled={isSubmitting}>
              Cancelar
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? (
                <>
                  <Spinner size={16} className="mr-2 text-primary" />
                  Salvando...
                </>
              ) : isEditing ? (
                'Atualizar'
              ) : (
                'Criar'
              )}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
