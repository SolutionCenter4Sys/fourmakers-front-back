import { useState, useMemo, useEffect, useCallback, useRef } from 'react'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Spinner } from '@/components/ui/spinner'
import { Separator } from '@/components/ui/separator'
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from '@/components/ui/accordion'
import {
  Calendar,
  User,
  MapPin,
  Video,
  Users,
  FileText,
  CheckSquare,
  MessageSquare,
  Building2,
  Clock,
  Mail,
  CheckCircle2,
  XCircle,
  AlertCircle,
  Sparkles,
  Timer,
  PlusCircle,
  Paperclip,
  Download,
  Trash2,
  Edit,
  RefreshCw,
  Tag,
} from 'lucide-react'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale'
import { calcularDuracaoAgenda, LABEL_DATA_NAO_INFORMADA } from '@shared/utils/agendaUtils'
import {
  parseDataAgendaParaExibicao,
  parseDataHoraAgendaParaExibicao,
  obterDiaCalendarioAgenda,
} from '@shared/utils/timezoneAgendaUtils'
import { getTipoInteracaoIcon } from '@shared/utils/agendaTipoInteracaoIcon'
import { normalizarParticipante } from '@shared/utils/agendaNormalizers'
import type { ItemAgendaGestor, TipoItemAgenda } from '@domain/entities/AgendaGestor'
import { container } from '@core/di/container'
import { BuscarAgendasFilhosUseCase } from '@domain/usecases/BuscarAgendasFilhosUseCase'
import { DeletarArquivoEncontroUseCase } from '@domain/usecases/DeletarArquivoEncontroUseCase'
import { ListarCategoriasSubPorInteracaoIdUseCase } from '@domain/usecases/ListarCategoriasSubPorInteracaoIdUseCase'
import { ListarCategoriasAssuntoComSubUseCase } from '@domain/usecases/ListarCategoriasAssuntoComSubUseCase'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { carregarAgendaDetalhe } from '@app/store/slices/agendasComerciaisSlice'
import { toast } from 'sonner'
import { podeCriarInteracao, podeEditarExcluirInteracao } from '@shared/utils/agendaPermissions'
import { processarUrlComToken, baixarArquivoPorUrl } from '@shared/utils/urlUtils'
import { InteracaoCard } from './InteracaoCard'
import { NovaAgendaModal } from './NovaAgendaModal'
import { CriarOuEditarProximosPassosModal } from './CriarOuEditarProximosPassosModal'
import { EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO } from './agendasComerciaisConstants'

/** Quando true, segmento "Agendas filhas" mostra "Em Breve" e botão desabilitado (sem buscar/listar). */
const AGENDAS_FILHAS_EM_BREVE = true

/** Converte encontro da agenda (item.encontros) para ItemAgendaGestor e exibe com InteracaoCard (Editar/Excluir).
 * Inclui dados da agenda pai (cliente, colaboradores, gestores, tipoInteracao, dataInicio, dataFim, localizacao)
 * para o modal de Próximos Passos montar o request Moxe completo.
 * data = data de agendamento da agenda (não data de criação do encontro). */
function encontroParaItemAgendaGestor(
  encontro: NonNullable<ItemAgendaGestor['encontros']>[number],
  agenda: ItemAgendaGestor
): ItemAgendaGestor {
  return {
    tipo: 'interacao',
    id: String(encontro.id),
    agendaId: agenda.agendaId,
    titulo: encontro.tituloInteracao ?? agenda.titulo,
    responsavel: encontro.nomeCompletoColaboradorCriador,
    data: agenda.data,
    dataRequisicao: encontro.dataRequisicao,
    status: 'Registrada',
    descricao: encontro.descricaoInteracao ?? agenda.descricao,
    resumoInteracao: encontro.resumoInteracao,
    encontroAi: encontro.encontroAi,
    interacaoCategorias: encontro.interacaoCategorias,
    arquivos: encontro.arquivos,
    tipoInteracao: agenda.tipoInteracao,
    dataInicio: agenda.dataInicio,
    dataFim: agenda.dataFim,
    localizacao: agenda.localizacao,
    linkReuniao: agenda.linkReuniao,
    cliente: agenda.cliente,
    colaboradores: agenda.colaboradores,
    gestoresExternos: agenda.gestoresExternos,
    participantes: agenda.participantes,
  }
}

/** Enriquece a interação com dados da agenda pai (cliente, colaboradores, gestores, tipo, horário, local) para o Moxe. */
function enriquecerInteracaoComAgendaPai(
  interacao: ItemAgendaGestor,
  agendaPai: ItemAgendaGestor,
): ItemAgendaGestor {
  return {
    ...interacao,
    titulo: interacao.titulo || agendaPai.titulo,
    tipoInteracao: interacao.tipoInteracao ?? agendaPai.tipoInteracao,
    dataInicio: interacao.dataInicio ?? agendaPai.dataInicio,
    dataFim: interacao.dataFim ?? agendaPai.dataFim,
    localizacao: interacao.localizacao ?? agendaPai.localizacao,
    linkReuniao: interacao.linkReuniao ?? agendaPai.linkReuniao,
    descricao: interacao.descricao ?? agendaPai.descricao,
    cliente: interacao.cliente ?? agendaPai.cliente,
    colaboradores: interacao.colaboradores ?? agendaPai.colaboradores,
    gestoresExternos: interacao.gestoresExternos ?? agendaPai.gestoresExternos,
    participantes: interacao.participantes ?? agendaPai.participantes,
  }
}

/** Item de participante para exibição (criador ou de participantesDetalhes) */
interface ParticipanteExibicao {
  nome: string
  email?: string
  tipo: string
  confirmado: boolean
  dataConfirmacao?: string
  /** True quando é o criador da agenda (sempre considerado confirmado; exibe ícone/label de Criador) */
  eCriador?: boolean
}

interface DetalhesAgendaModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  item: ItemAgendaGestor
  tipo: TipoItemAgenda
  onNovaInteracao?: (agendaId: number) => void
  /** Quando fornecido, abre o modal de nova interação no nível da página (evita dialog aninhado). */
  onAbrirNovaInteracao?: (agendaId: number) => void
  /** Exibir loading enquanto carrega detalhes da agenda (carregarAgenda). */
  loadingDetalhe?: boolean
  /** Chamado quando uma interação da agenda é editada ou deletada; use para recarregar detalhe e lista. */
  onInteracaoEditOrDelete?: () => void
  /** Chamado quando uma agenda filha é criada (Nova Agenda com Cliente); use para recarregar detalhe e lista. */
  onAgendaFilhaCriada?: () => void
  /** Abre modal de editar interação no nível da página. Pode receber onSuccessEdit (ex.: recarregar detalhe). */
  onAbrirEditarInteracao?: (interacao: ItemAgendaGestor, onSuccessEdit?: () => void) => void
  /** Lista de agendas; quando tipo==='interacao', usada para permitir editar/excluir e remover arquivo só se usuário for criador da interação ou da agenda. */
  agendasParaPermissao?: ItemAgendaGestor[]
  /** Recarrega os dados do item (ex.: após criar interação por outro meio). Exibe botão "Atualizar" no rodapé quando informado e tipo===agenda. */
  onRecarregarDetalhe?: () => void | Promise<void>
}

function getTipoIcon(tipo: TipoItemAgenda) {
  switch (tipo) {
    case 'agenda':
      return <Calendar className="h-5 w-5" />
    case 'interacao':
      return <MessageSquare className="h-5 w-5" />
    case 'acao':
      return <CheckSquare className="h-5 w-5" />
  }
}

export function DetalhesAgendaModal({
  open,
  onOpenChange,
  item,
  tipo,
  onNovaInteracao,
  onAbrirNovaInteracao,
  loadingDetalhe = false,
  onInteracaoEditOrDelete,
  onAgendaFilhaCriada,
  onAbrirEditarInteracao,
  agendasParaPermissao,
  onRecarregarDetalhe,
}: DetalhesAgendaModalProps) {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  /** Só criador da interação ou criador da agenda pode editar/excluir interação e remover arquivos. */
  const podeEditarExcluirInteracaoItem =
    tipo === 'interacao' && podeEditarExcluirInteracao(item, user, agendasParaPermissao)
  const [agendasFilhas, setAgendasFilhas] = useState<ItemAgendaGestor[]>([])
  const [loadingFilhas, setLoadingFilhas] = useState(false)
  const [erroFilhas, setErroFilhas] = useState<string | null>(null)
  const [novaAgendaModalOpen, setNovaAgendaModalOpen] = useState(false)
  const [refreshFilhasTrigger, setRefreshFilhasTrigger] = useState(0)
  /** Agenda pai carregada quando o item é interação vinda da timeline (sem cliente/colaboradores), para enriquecer o payload Moxe. */
  const [agendaPaiParaInteracao, setAgendaPaiParaInteracao] = useState<ItemAgendaGestor | null>(null)
  /** Evita disparar carregarAgendaDetalhe duas vezes (ex.: React Strict Mode) para o mesmo agendaId. */
  const ultimoAgendaIdPaiCarregadoRef = useRef<number | null>(null)
  /** Categorias da interação (carregadas sob demanda ao visualizar interação). */
  const [interacaoCategorias, setInteracaoCategorias] = useState<ItemAgendaGestor['interacaoCategorias']>([])
  const [loadingCategorias, setLoadingCategorias] = useState(false)
  /** Arquivos da interação ao visualizar (tipo==='interacao'); atualizado ao excluir. */
  const [arquivosExistentes, setArquivosExistentes] = useState<NonNullable<ItemAgendaGestor['arquivos']>>([])
  const [deletandoArquivoId, setDeletandoArquivoId] = useState<number | null>(null)
  const [baixandoArquivoId, setBaixandoArquivoId] = useState<number | null>(null)
  const [proximosPassosModalOpen, setProximosPassosModalOpen] = useState(false)

  const buscarFilhas = useCallback(async () => {
    if (!token || !item.agendaId || tipo !== 'agenda') return
    setLoadingFilhas(true)
    setErroFilhas(null)
    try {
      const useCase = container.resolve(BuscarAgendasFilhosUseCase)
      const lista = await useCase.execute(token, item.agendaId)
      setAgendasFilhas(lista)
    } catch (e) {
      setErroFilhas(e instanceof Error ? e.message : 'Erro ao carregar agendas filhas')
      setAgendasFilhas([])
    } finally {
      setLoadingFilhas(false)
    }
  }, [token, item.agendaId, tipo])

  useEffect(() => {
    if (!AGENDAS_FILHAS_EM_BREVE && open && tipo === 'agenda' && item.agendaId && token) {
      void buscarFilhas()
    } else if (!open) {
      setAgendasFilhas([])
      setErroFilhas(null)
    }
  }, [open, tipo, item.agendaId, token, refreshFilhasTrigger, buscarFilhas])

  useEffect(() => {
    if (open && tipo === 'interacao') {
      setArquivosExistentes(item.arquivos ?? [])
    }
  }, [open, tipo, item.arquivos])

  // Carregar categorias da interação sob demanda (só ao visualizar interação específica)
  useEffect(() => {
    if (!open || tipo !== 'interacao' || !item.id || !token) {
      setInteracaoCategorias([])
      return
    }
    const interacaoId = parseInt(item.id, 10)
    if (!Number.isFinite(interacaoId)) return
    setLoadingCategorias(true)
    const carregarCategorias = async () => {
      try {
        const [categoriasList, catalogoCategorias] = await Promise.all([
          container.resolve(ListarCategoriasSubPorInteracaoIdUseCase).execute(token, interacaoId),
          container.resolve(ListarCategoriasAssuntoComSubUseCase).execute(token),
        ])
        const categoriaDescPorId = new Map<number, string>()
        const subcategoriaDescPorId = new Map<number, string>()
        for (const cat of catalogoCategorias ?? []) {
          if (cat.id != null && cat.descricao != null) categoriaDescPorId.set(cat.id, cat.descricao)
          for (const sub of cat.subcategorias ?? []) {
            if (sub.id != null && sub.descricao != null) subcategoriaDescPorId.set(sub.id, sub.descricao)
          }
        }
        const enriched = (Array.isArray(categoriasList) ? categoriasList : []).map((c) => ({
          interacaoCategoriaId: c.interacaoCategoriaId,
          categoriaAssuntoId: c.categoriaAssuntoId,
          subCategoriaAssuntoId: c.subCategoriaAssuntoId,
          categoriaDescricao: c.categoriaAssuntoId != null ? categoriaDescPorId.get(c.categoriaAssuntoId) : undefined,
          subCategoriaDescricao:
            c.subCategoriaAssuntoId != null ? subcategoriaDescPorId.get(c.subCategoriaAssuntoId) : undefined,
        }))
        setInteracaoCategorias(enriched.length > 0 ? enriched : [])
      } catch {
        setInteracaoCategorias([])
      } finally {
        setLoadingCategorias(false)
      }
    }
    void carregarCategorias()
  }, [open, tipo, item.id, token])

  // Carregar agenda pai quando a interação vem da timeline (sem cliente/colaboradores) para enriquecer o request Moxe.
  // Ref evita chamada duplicada quando o efeito roda duas vezes (ex.: React Strict Mode).
  useEffect(() => {
    if (!open) {
      ultimoAgendaIdPaiCarregadoRef.current = null
      setAgendaPaiParaInteracao(null)
      return
    }
    if (tipo !== 'interacao' || !item.agendaId || !token) {
      setAgendaPaiParaInteracao(null)
      return
    }
    const precisaEnriquecer = !item.cliente && !item.colaboradores?.length
    if (!precisaEnriquecer) {
      setAgendaPaiParaInteracao(null)
      return
    }
      if (ultimoAgendaIdPaiCarregadoRef.current === item.agendaId) {
        return
      }
    ultimoAgendaIdPaiCarregadoRef.current = item.agendaId
    let cancelado = false
    dispatch(carregarAgendaDetalhe({ token, agendaId: item.agendaId }))
      .unwrap()
      .then((agenda) => {
        if (!cancelado) setAgendaPaiParaInteracao(agenda)
      })
      .catch(() => {
        if (!cancelado) setAgendaPaiParaInteracao(null)
      })
    return () => {
      cancelado = true
    }
  }, [open, tipo, item.agendaId, item.cliente, item.colaboradores, token, dispatch])

  const removerArquivoInteracao = useCallback(
    async (arquivoId: number) => {
      if (!token) return
      setDeletandoArquivoId(arquivoId)
      try {
        const useCase = container.resolve(DeletarArquivoEncontroUseCase)
        await useCase.execute(token, arquivoId)
        setArquivosExistentes((prev) => prev.filter((a) => a.id !== arquivoId))
        toast.success('Arquivo removido')
        onInteracaoEditOrDelete?.()
      } catch (err) {
        toast.error('Falha ao remover arquivo', {
          description: err instanceof Error ? err.message : undefined,
        })
      } finally {
        setDeletandoArquivoId(null)
      }
    },
    [token, onInteracaoEditOrDelete],
  )

  // Agenda: data exata = dataAgendada (dia) + dataInicio (hora). Outros: item.data apenas.
  const dataParsed =
    item.tipo === 'agenda' ? obterDiaCalendarioAgenda(item.data) : parseDataAgendaParaExibicao(item.data)
  const dataCompleta = dataParsed
    ? format(dataParsed, "EEEE, d 'de' MMMM 'de' yyyy", { locale: ptBR })
    : LABEL_DATA_NAO_INFORMADA

  const dataInicioParsed = item.dataInicio ? parseDataHoraAgendaParaExibicao(item.dataInicio) : null
  const dataFimParsed = item.dataFim ? parseDataHoraAgendaParaExibicao(item.dataFim) : null
  
  const horaInicio = dataInicioParsed
    ? format(dataInicioParsed, 'HH:mm', { locale: ptBR })
    : null
  const horaFim = dataFimParsed
    ? format(dataFimParsed, 'HH:mm', { locale: ptBR })
    : null
  
  // Calcular duração se tiver dataInicio e dataFim
  const duracao = calcularDuracaoAgenda(dataInicioParsed, dataFimParsed)

  // Lista de participantes para exibição: criador primeiro, depois participantesDetalhes (sem duplicar criador)
  const participantesParaExibicao = useMemo((): ParticipanteExibicao[] => {
    const lista: ParticipanteExibicao[] = []
    const detalhes = item.participantesDetalhes ?? []
    const codCriador = item.codColaboradorCriador

    if (codCriador && item.responsavel) {
      const emailCriador = item.colaboradores?.find(
        (c) => c.codInternoColaborador === codCriador
      )?.email
      lista.push({
        nome: item.responsavel,
        email: emailCriador,
        tipo: 'Criador',
        confirmado: true,
        eCriador: true,
      })
    }

    const restante = detalhes.filter(
      (p) => p.codigoColaborador !== codCriador
    )
    restante.forEach((p) => {
      const norm = normalizarParticipante(p, item.colaboradores, item.gestoresExternos)
      lista.push({
        nome: norm.nome,
        email: norm.email,
        tipo: norm.tipo ?? 'Participante',
        confirmado: p.confirmado,
        dataConfirmacao: p.dataConfirmacao,
      })
    })
    return lista
  }, [item.responsavel, item.codColaboradorCriador, item.participantesDetalhes, item.colaboradores, item.gestoresExternos])

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto overflow-x-hidden w-[calc(100vw-2rem)]">
        <DialogHeader className="min-w-0">
          <div className="flex items-center gap-3 min-w-0">
            <div className="h-10 w-10 shrink-0 rounded-lg bg-muted flex items-center justify-center">
              {tipo === 'agenda'
                ? getTipoInteracaoIcon(item.tipoInteracao, 'h-5 w-5')
                : getTipoIcon(tipo)}
            </div>
            <div className="flex-1 min-w-0">
              <DialogTitle className="text-xl break-words">{item.titulo}</DialogTitle>
              <DialogDescription className="mt-1">
                {tipo === 'agenda' && item.tipoInteracao && (
                  <Badge variant="secondary">
                    {item.tipoInteracao}
                  </Badge>
                )}
              </DialogDescription>
            </div>
          </div>
        </DialogHeader>

        {loadingDetalhe ? (
          <div className="flex items-center justify-center py-12">
            <Spinner className="text-primary" />
          </div>
        ) : (
        <>
        <div className="space-y-6 mt-4 min-w-0 overflow-hidden">
          {/* Categoria e Subcategoria no topo (somente ao ver interação) */}
          {tipo === 'interacao' && (
            loadingCategorias ? (
              <div className="flex items-center gap-2 text-sm text-muted-foreground pb-2 border-b border-border">
                <Spinner size={14} />
                <span>Carregando categorias...</span>
              </div>
            ) : interacaoCategorias && interacaoCategorias.length > 0 ? (
              <div className="flex flex-wrap items-center gap-2 text-sm text-muted-foreground pb-2 border-b border-border">
                <Tag className="h-4 w-4 shrink-0" />
                <span className="font-medium">Categoria e Subcategoria:</span>
                {interacaoCategorias.map((cat, idx) => (
                  <span key={idx}>
                    <span className="font-medium text-foreground">{cat.categoriaDescricao ?? 'Categoria'}</span>
                    {cat.subCategoriaDescricao != null && cat.subCategoriaDescricao !== '' && (
                      <span> → {cat.subCategoriaDescricao}</span>
                    )}
                    {idx < interacaoCategorias.length - 1 && <span className="text-muted-foreground/60"> • </span>}
                  </span>
                ))}
              </div>
            ) : null
          )}

          {/* Informações Principais */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 min-w-0">
            {tipo !== 'interacao' && (
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Calendar className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Data e Hora</span>
                </div>
                <div className="space-y-2 pl-6">
                  <div className="flex items-center gap-2">
                    <Calendar className="h-4 w-4 shrink-0 text-muted-foreground" />
                    <span className="text-sm font-medium">{dataCompleta}</span>
                  </div>
                  {horaInicio && horaFim && (
                    <div className="flex items-center gap-2 text-sm text-muted-foreground">
                      <Clock className="h-4 w-4 shrink-0" />
                      <span>{horaInicio} - {horaFim}</span>
                      {duracao && (
                        <>
                          <span className="text-muted-foreground/60">•</span>
                          <div className="flex items-center gap-1.5">
                            <Timer className="h-3.5 w-3.5 shrink-0" />
                            <span className="text-xs">{duracao} min</span>
                          </div>
                        </>
                      )}
                    </div>
                  )}
                </div>
              </div>
            )}

            <div className="space-y-1">
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <User className="h-4 w-4 shrink-0" />
                <span className="font-medium">Responsável</span>
              </div>
              <p className="text-sm font-medium pl-6 break-words">{item.responsavel}</p>
            </div>

            {item.cliente && (
              <div className="space-y-1">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Building2 className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Cliente</span>
                </div>
                <p className="text-sm font-medium pl-6 break-words">{item.cliente.nomeCliente}</p>
                {item.cliente.codigoCliente &&
                  String(item.cliente.codigoCliente).trim() !== '' &&
                  String(item.cliente.codigoCliente) !== '0' && (
                    <p className="text-xs text-muted-foreground pl-6">
                      Código: {item.cliente.codigoCliente}
                    </p>
                  )}
              </div>
            )}

            {item.localizacao && (
              <div className="space-y-1">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <MapPin className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Localização</span>
                </div>
                <p className="text-sm font-medium pl-6 break-words">{item.localizacao}</p>
              </div>
            )}

            {item.linkReuniao && (
              <div className="space-y-1 md:col-span-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Video className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Link da Reunião</span>
                </div>
                <a
                  href={item.linkReuniao}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-primary hover:underline text-sm font-medium pl-6 block break-all"
                >
                  {item.linkReuniao}
                </a>
              </div>
            )}

            {tipo !== 'interacao' && item.dataCriacao && (
              <div className="space-y-1">
                <span className="text-xs text-muted-foreground">Criado em:</span>
                <p className="text-xs font-medium">
                  {item.dataCriacao && parseDataHoraAgendaParaExibicao(item.dataCriacao)
                    ? format(parseDataHoraAgendaParaExibicao(item.dataCriacao)!, 'dd/MM/yyyy HH:mm', { locale: ptBR })
                    : '-'}
                </p>
              </div>
            )}

            {tipo !== 'interacao' && item.dataAtualizacao && (
              <div className="space-y-1">
                <span className="text-xs text-muted-foreground">Atualizado em:</span>
                <p className="text-xs font-medium">
                  {item.dataAtualizacao && parseDataHoraAgendaParaExibicao(item.dataAtualizacao)
                    ? format(parseDataHoraAgendaParaExibicao(item.dataAtualizacao)!, 'dd/MM/yyyy HH:mm', { locale: ptBR })
                    : '-'}
                </p>
              </div>
            )}
          </div>

          {/* Colaboradores */}
          {item.colaboradores && item.colaboradores.length > 0 && (
            <>
              <Separator />
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Users className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Colaboradores</span>
                </div>
                <div className="space-y-2">
                  {item.colaboradores.map((colab, idx) => (
                    <div key={idx} className="bg-muted/50 rounded-lg p-3">
                      <p className="text-sm font-medium">{colab.nome}</p>
                      {colab.email && (
                        <div className="flex items-center gap-1.5 mt-1 text-xs text-muted-foreground">
                          <Mail className="h-3 w-3" />
                          <span>{colab.email}</span>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            </>
          )}

          {/* Participantes (criador + lista da API, com status do convite) */}
          {participantesParaExibicao.length > 0 && (
            <>
              <Separator />
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <Users className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Participantes ({participantesParaExibicao.length})</span>
                </div>
                <div className="space-y-2">
                  {participantesParaExibicao.map((participante, idx) => (
                    <div key={idx} className="bg-muted/50 rounded-lg p-3 flex items-center justify-between">
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium truncate">{participante.nome}</p>
                        {participante.email && (
                          <div className="flex items-center gap-1.5 mt-1 text-xs text-muted-foreground">
                            <Mail className="h-3 w-3" />
                            <span className="truncate">{participante.email}</span>
                          </div>
                        )}
                        {participante.tipo && (
                          <p className="text-xs text-muted-foreground mt-1">
                            {participante.tipo}
                          </p>
                        )}
                        {participante.dataConfirmacao && (
                          <p className="text-xs text-muted-foreground mt-1">
                            Confirmado em: {parseDataHoraAgendaParaExibicao(participante.dataConfirmacao)
                              ? format(parseDataHoraAgendaParaExibicao(participante.dataConfirmacao)!, 'dd/MM/yyyy HH:mm', { locale: ptBR })
                              : '-'}
                          </p>
                        )}
                      </div>
                      <div className="shrink-0 ml-3">
                        {participante.eCriador ? (
                          <div className="flex items-center gap-1.5 text-success">
                            <CheckCircle2 className="h-5 w-5" />
                            <span className="text-xs font-medium">Criador</span>
                          </div>
                        ) : participante.confirmado ? (
                          <div className="flex items-center gap-1.5 text-success">
                            <CheckCircle2 className="h-5 w-5" />
                            <span className="text-xs font-medium">Confirmado</span>
                          </div>
                        ) : (
                          <div className="flex items-center gap-1.5 text-muted-foreground">
                            <XCircle className="h-5 w-5" />
                            <span className="text-xs">Pendente</span>
                          </div>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </>
          )}

          {/* Descrição */}
          {item.descricao && (
            <>
              <Separator />
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <FileText className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Descrição</span>
                </div>
                <div className="bg-muted/50 rounded-lg p-4">
                  <p className="text-sm whitespace-pre-wrap break-words">{item.descricao}</p>
                </div>
              </div>
            </>
          )}

          {/* Resumo da Interação (para interações) */}
          {tipo === 'interacao' && item.resumoInteracao && (
            <>
              <Separator />
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <MessageSquare className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Resumo da Interação</span>
                </div>
                <div className="bg-muted/50 rounded-lg p-4">
                  <p className="text-sm whitespace-pre-wrap">{item.resumoInteracao}</p>
                </div>
              </div>
            </>
          )}

          {/* Encontro AI (para interações) */}
          {tipo === 'interacao' && item.encontroAi && (
            <>
              <Separator />
              <div className="space-y-3">
                <div className="flex items-center gap-2 text-sm font-medium">
                  <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-accent to-primary flex items-center justify-center">
                    <Sparkles className="h-4 w-4 text-white" />
                  </div>
                  <span>Insights Gerados por IA</span>
                </div>
                
                <Accordion type="single" collapsible defaultValue="resumo" className="w-full min-w-0">
                  {item.encontroAi.resumo && (
                    <AccordionItem value="resumo">
                      <AccordionTrigger className="text-sm">
                        <div className="flex items-center gap-2">
                          <MessageSquare className="h-4 w-4" />
                          <span>Resumo da Interação</span>
                        </div>
                      </AccordionTrigger>
                      <AccordionContent>
                        <div className="bg-gradient-to-br from-accent/10 to-primary/10 dark:from-accent/20 dark:to-primary/20 rounded-lg p-4 border border-accent/20 dark:border-accent/40">
                          <p className="text-sm whitespace-pre-wrap leading-relaxed">{item.encontroAi.resumo}</p>
                        </div>
                      </AccordionContent>
                    </AccordionItem>
                  )}
                  
                  {item.encontroAi.passos && item.encontroAi.passos.length > 0 && (
                    <AccordionItem value="passos">
                      <AccordionTrigger className="text-sm">
                        <div className="flex items-center gap-2">
                          <CheckSquare className="h-4 w-4" />
                          <span>Próximos Passos Sugeridos ({item.encontroAi.passos.length})</span>
                        </div>
                      </AccordionTrigger>
                      <AccordionContent>
                        <div className="space-y-3">
                          {item.encontroAi.passos.map((passo, idx) => (
                            <div
                              key={idx}
                              className="bg-gradient-to-br from-info/5 to-info/10 dark:from-info/10 dark:to-info/20 rounded-lg p-4 border border-info/20 dark:border-info/80"
                            >
                              <div className="flex items-start gap-3">
                                <div className="flex-1 min-w-0">
                                  <p className="text-sm font-medium block">{passo.texto}</p>
                                  <div className="mt-2 flex flex-wrap items-center gap-x-3 gap-y-1 text-xs text-muted-foreground">
                                    <div className="flex items-center gap-1">
                                      <User className="h-3 w-3" />
                                      <span className="font-medium">{passo.nomeColaborador}</span>
                                    </div>
                                    {passo.dataLimite && (
                                      <div className="flex items-center gap-1">
                                        <Calendar className="h-3 w-3" />
                                        <span>
                                          Prazo: {parseDataAgendaParaExibicao(passo.dataLimite)
                                            ? format(parseDataAgendaParaExibicao(passo.dataLimite)!, 'dd/MM/yyyy', { locale: ptBR })
                                            : '-'}
                                        </span>
                                      </div>
                                    )}
                                  </div>
                                </div>
                              </div>
                            </div>
                          ))}
                        </div>
                      </AccordionContent>
                    </AccordionItem>
                  )}
                </Accordion>
              </div>
            </>
          )}

          {/* Arquivos anexados (ao visualizar interação/encontro) */}
          {tipo === 'interacao' && arquivosExistentes.length > 0 && (
            <>
              <Separator />
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm font-medium text-muted-foreground">
                  <Paperclip className="h-4 w-4 shrink-0" />
                  <span>Arquivos anexados</span>
                </div>
                <ul className="rounded-lg border border-border divide-y divide-border">
                  {arquivosExistentes.map((arquivo) => {
                    const link = arquivo.linkImagemInteracao ?? arquivo.linkAudioInteracao ?? null
                    const urlComToken = link ? processarUrlComToken(link, token) ?? link : null
                    return (
                      <li
                        key={arquivo.id}
                        className="flex items-center justify-between gap-3 p-3 text-sm"
                      >
                        <div className="min-w-0 flex-1 flex items-center gap-2">
                          {urlComToken ? (
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              className="text-primary hover:text-primary shrink-0"
                              disabled={baixandoArquivoId === arquivo.id}
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
                            <span className="text-muted-foreground truncate">Arquivo (requer login)</span>
                          ) : (
                            <span className="text-muted-foreground">Arquivo (id {arquivo.id})</span>
                          )}
                        </div>
                        {EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO && podeEditarExcluirInteracaoItem && (
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            className="shrink-0 text-destructive hover:text-destructive"
                            disabled={deletandoArquivoId === arquivo.id}
                            onClick={() => removerArquivoInteracao(arquivo.id)}
                            aria-label={`Remover arquivo ${arquivo.id}`}
                          >
                            {deletandoArquivoId === arquivo.id ? (
                              <Spinner size={14} className="text-destructive" />
                            ) : (
                              <>
                                <Trash2 className="h-4 w-4 mr-1" />
                                Remover
                              </>
                            )}
                          </Button>
                        )}
                      </li>
                    )
                  })}
                </ul>
              </div>
            </>
          )}

          {/* Informações de Ação (para ações) */}
          {tipo === 'acao' && (item.atrasado || item.vencendo) && (
            <>
              <Separator />
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <AlertCircle className="h-4 w-4 shrink-0" />
                  <span className="font-medium">Status da Ação</span>
                </div>
                <div className="flex gap-2">
                  {item.atrasado && (
                    <Badge variant="destructive">Atrasada</Badge>
                  )}
                  {item.vencendo && (
                    <Badge variant="secondary">Vencendo</Badge>
                  )}
                </div>
              </div>
            </>
          )}

          {/* Encontros/Interações da Agenda (com Editar/Excluir via InteracaoCard) */}
          {tipo === 'agenda' && item.encontros && item.encontros.length > 0 && (
            <>
              <Separator />
              <div className="space-y-3">
                <div className="flex items-center gap-2 text-sm font-medium">
                  <MessageSquare className="h-4 w-4 shrink-0" />
                  <span>Interações ({item.encontros.length})</span>
                </div>
                <div className="space-y-3">
                  {item.encontros.map((encontro) => (
                    <InteracaoCard
                      key={encontro.id}
                      interacao={encontroParaItemAgendaGestor(encontro, item)}
                      agendas={[item]}
                      onEditSuccess={onInteracaoEditOrDelete}
                      onDeleteSuccess={onInteracaoEditOrDelete}
                      onAbrirEditarInteracao={
                        onAbrirEditarInteracao
                          ? (interacao) => onAbrirEditarInteracao(interacao, onInteracaoEditOrDelete)
                          : undefined
                      }
                      compact
                    />
                  ))}
                </div>
              </div>
            </>
          )}

          {/* Agendas filhas (Nova Agenda com Cliente) */}
          {tipo === 'agenda' && item.agendaId && (
            <>
              <Separator />
              <div className="space-y-3">
                <div className="flex items-center justify-between gap-2">
                  <div className="flex items-center gap-2 text-sm font-medium">
                    <Calendar className="h-4 w-4 shrink-0" />
                    <span>Agendas filhas</span>
                  </div>
                  <Button
                    variant="outline"
                    size="sm"
                    disabled={AGENDAS_FILHAS_EM_BREVE}
                    onClick={() => !AGENDAS_FILHAS_EM_BREVE && setNovaAgendaModalOpen(true)}
                    aria-label={AGENDAS_FILHAS_EM_BREVE ? 'Agendas no Cliente: Em Breve' : 'Nova Agenda com Cliente'}
                  >
                    <PlusCircle className="h-4 w-4 mr-2" />
                    Nova Agenda com Cliente
                  </Button>
                </div>
                {AGENDAS_FILHAS_EM_BREVE ? (
                  <p className="text-sm text-muted-foreground">Em Breve</p>
                ) : loadingFilhas ? (
                  <div className="flex items-center justify-center py-6">
                    <Spinner className="text-primary" />
                  </div>
                ) : erroFilhas ? (
                  <p className="text-sm text-destructive">{erroFilhas}</p>
                ) : agendasFilhas.length === 0 ? (
                  <p className="text-sm text-muted-foreground">Nenhuma agenda filha vinculada.</p>
                ) : (
                  <ul className="space-y-2">
                    {agendasFilhas.map((filha) => (
                      <li
                        key={filha.id}
                        className="bg-muted/50 rounded-lg p-3 flex items-center justify-between gap-2"
                      >
                        <div className="min-w-0">
                          <p className="text-sm font-medium truncate">{filha.titulo}</p>
                          <p className="text-xs text-muted-foreground">
                            {filha.data && parseDataAgendaParaExibicao(filha.data)
                              ? format(parseDataAgendaParaExibicao(filha.data)!, 'dd/MM/yyyy', { locale: ptBR })
                              : '-'}
                            {filha.cliente?.nomeCliente && ` • ${filha.cliente.nomeCliente}`}
                          </p>
                        </div>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </>
          )}
        </div>

        <div className="flex justify-end gap-2 mt-6 flex-wrap">
          {!loadingDetalhe && tipo === 'interacao' && item.id && (
            <Button
              variant="outline"
              onClick={() => setProximosPassosModalOpen(true)}
            >
              <Sparkles className="h-4 w-4 mr-2" />
              {item.encontroAi ? 'Editar Próximos Passos' : 'Gerar Próximos Passos com IA'}
            </Button>
          )}
          {EDITAR_EXCLUIR_INTERACAO_E_ARQUIVOS_HABILITADO && !loadingDetalhe && tipo === 'interacao' && onAbrirEditarInteracao && (
            <Button
              variant="outline"
              onClick={() => {
                onAbrirEditarInteracao(item, onInteracaoEditOrDelete)
                onOpenChange(false)
              }}
            >
              <Edit className="h-4 w-4 mr-2" />
              Editar interação
            </Button>
          )}
          {!loadingDetalhe && tipo === 'agenda' && item.agendaId && podeCriarInteracao(item, user) && (
            <Button
              variant="primary"
              onClick={() => {
                if (onAbrirNovaInteracao) {
                  onAbrirNovaInteracao(item.agendaId!)
                } else {
                  onNovaInteracao?.(item.agendaId!)
                }
              }}
            >
              <MessageSquare className="h-4 w-4 mr-2" />
              Nova Interação
            </Button>
          )}
          {!loadingDetalhe && tipo === 'agenda' && onRecarregarDetalhe && (
            <Button
              variant="outline"
              onClick={async () => {
                await onRecarregarDetalhe()
              }}
            >
              <RefreshCw className="h-4 w-4 mr-2" />
              Atualizar
            </Button>
          )}
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Fechar
          </Button>
        </div>
        </>
        )}
      </DialogContent>

      {tipo === 'agenda' && (
        <NovaAgendaModal
          open={novaAgendaModalOpen}
          onOpenChange={setNovaAgendaModalOpen}
          agendaPai={item}
          onSuccess={() => {
            setRefreshFilhasTrigger((t) => t + 1)
            onAgendaFilhaCriada?.()
          }}
        />
      )}
      {tipo === 'interacao' && item.id && (
        <CriarOuEditarProximosPassosModal
          open={proximosPassosModalOpen}
          onOpenChange={setProximosPassosModalOpen}
          encontroId={parseInt(item.id, 10)}
          encontroAiExistente={item.encontroAi ?? undefined}
          onSuccess={onInteracaoEditOrDelete}
          ataInteracao={item.resumoInteracao ?? item.descricao ?? ''}
          transcricoesAudio={
            (arquivosExistentes ?? item.arquivos ?? [])
              .map((a) => a.transcricao ?? '')
              .filter(Boolean)
              .join('\n') ?? ''
          }
          itemInteracao={
            agendaPaiParaInteracao
              ? enriquecerInteracaoComAgendaPai(item, agendaPaiParaInteracao)
              : item
          }
        />
      )}
    </Dialog>
  )
}
