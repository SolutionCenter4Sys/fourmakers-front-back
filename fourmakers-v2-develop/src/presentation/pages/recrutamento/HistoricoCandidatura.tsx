import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { useLocation, useNavigate, useParams } from 'react-router-dom'
import { Card, CardContent } from '@/components/ui/card'
import { Spinner } from '@/components/ui/spinner'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from '@/components/ui/tooltip'
import { PageBreadcrumb } from '@presentation/components/common'
import { EditarDadosPessoaisModal } from '@presentation/components/gestao-vagas'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { ObterHistoricoCandidaturaUseCase } from '@domain/usecases/ObterHistoricoCandidaturaUseCase'
import type {
  HistoricoCandidaturaItem,
  HistoricoCandidaturaAlteracao,
  HistoricoCandidaturaComentario,
  HistoricoCandidaturaArquivo,
} from '@domain/entities/HistoricoCandidatura'
import {
  ArrowDown,
  ArrowLeft,
  ArrowUp,
  ArrowUpDown,
  ChevronDown,
  ChevronUp,
  Download,
  Edit,
  MessageCircle,
  Search,
  Star,
} from '@/components/ui/system-icons'
import { cn } from '@/lib/utils'
import { toast } from 'sonner'

const PREVIA_COMENTARIO_LENGTH = 40

/** Formata data ISO para dd/MM/yyyy */
function formatarData(iso: string | null | undefined): string {
  if (!iso) return '—'
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return iso
  return d.toLocaleDateString('pt-BR')
}

/** Formata data ISO para dd/MM/yyyy HH:mm */
function formatarDataHora(iso: string | null | undefined): string {
  if (!iso) return '—'
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return iso
  return d.toLocaleString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

/** Resumo para Mod. Trabalho e Pretensão: "100% Re... R$: 5000" */
function resumoModeloPretensao(modelo: string | null | undefined, pretencao: number | undefined): string {
  const mod = (modelo ?? '').trim() || '—'
  const valor = pretencao != null && pretencao > 0
    ? `R$: ${Number(pretencao).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
    : 'R$: 0,00'
  if (mod === '—') return valor
  const resumoMod = mod.length > 10 ? `${mod.slice(0, 10)}...` : mod
  return `${resumoMod} ${valor}`
}

/** Texto completo para tooltip */
function textoCompletoModeloPretensao(modelo: string | null | undefined, pretencao: number | undefined): string {
  const mod = (modelo ?? '').trim() || 'Não informado'
  const valor = pretencao != null && pretencao > 0
    ? `R$: ${Number(pretencao).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
    : 'R$: 0,00'
  return `${mod} - ${valor}`
}

/** Prévia de texto para comentário */
function previaTexto(texto: string | null | undefined): string {
  if (!texto || !String(texto).trim()) return 'Sem comentários.'
  const t = String(texto).trim()
  return t.length <= PREVIA_COMENTARIO_LENGTH ? t : `${t.slice(0, PREVIA_COMENTARIO_LENGTH)}...`
}

function CelulaComTooltip({ texto, children }: { texto: string; children: React.ReactNode }) {
  const precisaTooltip = texto.length > 30
  if (!precisaTooltip) return <>{children}</>
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <span className="cursor-default block truncate max-w-[200px]">{children}</span>
      </TooltipTrigger>
      <TooltipContent side="top" className="max-w-sm break-words">
        {texto}
      </TooltipContent>
    </Tooltip>
  )
}

function DownloadArquivo({ arquivo }: { arquivo: HistoricoCandidaturaArquivo }) {
  const url = arquivo.url ?? (arquivo as { urlDownload?: string }).urlDownload
  const nome = arquivo.nome ?? (arquivo as { nomeArquivo?: string }).nomeArquivo ?? 'Arquivo'
  if (!url) return <span className="text-muted-foreground text-xs">—</span>
  return (
    <Button
      type="button"
      variant="ghost"
      size="sm"
      className="h-8 w-8 p-0"
      onClick={() => window.open(url, '_blank')}
      aria-label={`Baixar ${nome}`}
    >
      <Download className="h-4 w-4" />
    </Button>
  )
}

export default function HistoricoCandidatura() {
  const { codigoInternoColaborador } = useParams<{ codigoInternoColaborador: string }>()
  const location = useLocation()
  const navigate = useNavigate()
  const { token } = useAppSelector((state) => state.auth)

  /** Estado vindo da navegação (ex.: da tela Gestão de Vagas > Candidatos). */
  const locationState = location.state as {
    nome?: string
    vagaId?: string
    vagaTitle?: string
    vagaRaw?: unknown
  } | null
  const nomePassadoNaNavegacao = locationState?.nome?.trim()
  const vagaIdRetorno = locationState?.vagaId
  const vagaTitleRetorno = locationState?.vagaTitle ?? 'Candidatos'

  const [lista, setLista] = useState<HistoricoCandidaturaItem[]>([])
  const [nomeCandidato, setNomeCandidato] = useState<string>('')
  const [carregando, setCarregando] = useState(true)
  const [busca, setBusca] = useState('')
  const [buscaAplicada, setBuscaAplicada] = useState('')
  const [expandidos, setExpandidos] = useState<Set<string>>(new Set())
  const [sortKey, setSortKey] = useState<
    'codVaga' | 'nomeVaga' | 'statusDaVaga' | 'nomeCliente' | 'nomeGestor' | 'modeloTrabalhoDescricao' | 'quantidadeDiasPresencial' | 'dataUltimaAlteracao'
  >('dataUltimaAlteracao')
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc')
  /** Por idCandidatura: Movimentações e Comentários começam fechados; clique na linha do cabeçalho alterna. */
  const [secoesAbertas, setSecoesAbertas] = useState<
    Record<string, { movimentacoes: boolean; comentarios: boolean }>
  >({})
  const [editarDadosPessoaisOpen, setEditarDadosPessoaisOpen] = useState(false)

  const toggleSecao = useCallback((idCandidatura: string, secao: 'movimentacoes' | 'comentarios') => {
    setSecoesAbertas((prev) => {
      const atual = prev[idCandidatura] ?? { movimentacoes: false, comentarios: false }
      return {
        ...prev,
        [idCandidatura]: {
          movimentacoes: secao === 'movimentacoes' ? !atual.movimentacoes : atual.movimentacoes,
          comentarios: secao === 'comentarios' ? !atual.comentarios : atual.comentarios,
        },
      }
    })
  }, [])

  const carregarHistorico = useCallback(
    async (filtro?: string) => {
      if (!token || !codigoInternoColaborador) {
        setCarregando(false)
        return
      }
      setCarregando(true)
      try {
        const useCase = container.resolve(ObterHistoricoCandidaturaUseCase)
        const res = await useCase.execute(token, codigoInternoColaborador, filtro?.trim() || undefined)
        const retorno = Array.isArray(res?.retorno) ? res.retorno : []
        setLista(retorno)
        const primeiro = retorno[0]
        setNomeCandidato(primeiro?.nomeCandidato ?? '')
      } catch (e) {
        toast.error('Não foi possível carregar o histórico do candidato.')
        setLista([])
      } finally {
        setCarregando(false)
      }
    },
    [token, codigoInternoColaborador],
  )

  useEffect(() => {
    carregarHistorico(buscaAplicada)
  }, [carregarHistorico, codigoInternoColaborador])

  const handleBuscar = () => {
    setBuscaAplicada(busca)
    carregarHistorico(busca)
  }

  const handleLimparFiltros = () => {
    setBusca('')
    setBuscaAplicada('')
    carregarHistorico()
  }

  const toggleExpandir = (idCandidatura: string) => {
    setExpandidos((prev) => {
      const next = new Set(prev)
      if (next.has(idCandidatura)) next.delete(idCandidatura)
      else next.add(idCandidatura)
      return next
    })
  }

  const listagemFiltrada = useMemo(() => {
    if (!buscaAplicada.trim()) return lista
    const b = buscaAplicada.toLowerCase().trim()
    return lista.filter(
      (item) =>
        String(item.codVaga).includes(b) ||
        (item.nomeGestor ?? '').toLowerCase().includes(b) ||
        (item.nomeVaga ?? '').toLowerCase().includes(b),
    )
  }, [lista, buscaAplicada])

  type SortKey = typeof sortKey
  const listagemOrdenada = useMemo(() => {
    const ordenadas = [...listagemFiltrada]
    const dir = sortDirection === 'asc' ? 1 : -1
    ordenadas.sort((a, b) => {
      let valueA: string | number | null | undefined
      let valueB: string | number | null | undefined
      switch (sortKey as SortKey) {
        case 'codVaga':
          valueA = a.codVaga
          valueB = b.codVaga
          return (((valueA ?? 0) as number) - ((valueB ?? 0) as number)) * dir
        case 'nomeVaga':
          valueA = (a.codVaga + ' ' + (a.nomeVaga ?? '')).toLowerCase()
          valueB = (b.codVaga + ' ' + (b.nomeVaga ?? '')).toLowerCase()
          return (valueA as string).localeCompare(valueB as string, 'pt-BR') * dir
        case 'statusDaVaga':
          valueA = (a.statusDaVaga ?? '').toLowerCase()
          valueB = (b.statusDaVaga ?? '').toLowerCase()
          return (valueA as string).localeCompare(valueB as string, 'pt-BR') * dir
        case 'nomeCliente':
          valueA = (a.nomeCliente ?? '').toLowerCase()
          valueB = (b.nomeCliente ?? '').toLowerCase()
          return (valueA as string).localeCompare(valueB as string, 'pt-BR') * dir
        case 'nomeGestor':
          valueA = (a.nomeGestor ?? '').toLowerCase()
          valueB = (b.nomeGestor ?? '').toLowerCase()
          return (valueA as string).localeCompare(valueB as string, 'pt-BR') * dir
        case 'modeloTrabalhoDescricao':
          valueA = (a.modeloTrabalhoDescricao ?? '').toLowerCase()
          valueB = (b.modeloTrabalhoDescricao ?? '').toLowerCase()
          return (valueA as string).localeCompare(valueB as string, 'pt-BR') * dir
        case 'quantidadeDiasPresencial':
          valueA = a.quantidadeDiasPresencial ?? -1
          valueB = b.quantidadeDiasPresencial ?? -1
          return ((valueA as number) - (valueB as number)) * dir
        case 'dataUltimaAlteracao':
          valueA = a.dataUltimaAlteracao ? new Date(a.dataUltimaAlteracao).getTime() : 0
          valueB = b.dataUltimaAlteracao ? new Date(b.dataUltimaAlteracao).getTime() : 0
          return ((valueA as number) - (valueB as number)) * dir
        default:
          return 0
      }
    })
    return ordenadas
  }, [listagemFiltrada, sortKey, sortDirection])

  const handleOrdenar = (key: SortKey) => {
    if (sortKey === key) {
      setSortDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'))
    } else {
      setSortKey(key)
      setSortDirection('desc')
    }
  }

  /** Ícone de ordenação: seta única na coluna ativa (direção atual), senão ArrowUpDown. */
  const IconeOrdenacao = ({ chave }: { chave: SortKey }) => {
    if (sortKey !== chave) {
      return <ArrowUpDown className="ml-2 h-4 w-4 shrink-0 text-muted-foreground" aria-hidden />
    }
    return sortDirection === 'desc' ? (
      <ArrowDown className="ml-2 h-4 w-4 shrink-0 text-primary" aria-hidden />
    ) : (
      <ArrowUp className="ml-2 h-4 w-4 shrink-0 text-primary" aria-hidden />
    )
  }

  if (!codigoInternoColaborador) {
    return (
      <div className="container mx-auto p-4 md:p-6">
        <p className="text-muted-foreground">Candidato não identificado.</p>
      </div>
    )
  }

  const breadcrumbItems = useMemo(() => {
    const base = [{ label: 'Recrutamento', href: '/recrutamento' }]
    if (vagaIdRetorno) {
      const candidatosHref = `/recrutamento/candidatos?vagaId=${encodeURIComponent(vagaIdRetorno)}&nomeVaga=${encodeURIComponent(vagaTitleRetorno)}`
      return [
        ...base,
        { label: `Candidatos - ${vagaTitleRetorno}`, href: candidatosHref },
        { label: 'Histórico de movimentações da candidatura' },
      ]
    }
    return [
      ...base,
      { label: 'Histórico de movimentações da candidatura' },
    ]
  }, [vagaIdRetorno, vagaTitleRetorno])

  return (
    <TooltipProvider>
      <div className="container mx-auto p-4 md:p-6 space-y-6">
        <PageBreadcrumb items={breadcrumbItems} />

        <div className="flex items-center gap-3">
          <Button
            variant="ghost"
            size="icon"
            aria-label="Voltar"
            onClick={() => navigate(-1)}
          >
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <div className="flex-1 flex items-center gap-2">
            <h1 className="page-title">
              Histórico de movimentações da candidatura
              {(nomePassadoNaNavegacao ?? nomeCandidato)?.trim() &&
              (nomePassadoNaNavegacao ?? nomeCandidato)?.trim().toLowerCase() !== 'string'
                ? ` - ${(nomePassadoNaNavegacao ?? nomeCandidato)?.trim()}`
                : ' .'}
            </h1>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  aria-label="Editar dados pessoais"
                  onClick={() => setEditarDadosPessoaisOpen(true)}
                >
                  <Edit className="h-4 w-4" />
                </Button>
              </TooltipTrigger>
              <TooltipContent>Editar dados pessoais</TooltipContent>
            </Tooltip>
          </div>
        </div>

        <Card>
          <CardContent className="p-6 space-y-4">
            <div>
              <h2 className="text-sm font-semibold text-foreground">Painel de Movimentações</h2>
              <p className="text-sm text-muted-foreground mt-0.5">
                Aqui estão todas as movimentações e feedbacks do candidato.
              </p>
            </div>
            <div className="flex flex-wrap items-end gap-2">
              <div className="flex-1 min-w-[200px] space-y-2">
                <Label htmlFor="busca-historico">Buscar Cód. Vaga, Nome Gestor ou Nome Vaga</Label>
                <Input
                  id="busca-historico"
                  placeholder="Buscar Cód. Vaga, Nome Gestor ou Nome Vaga"
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && busca.trim() && handleBuscar()}
                />
              </div>
              <Button type="button" onClick={handleBuscar} disabled={!busca.trim()}>
                <Search className="h-4 w-4 mr-2" />
                Buscar
              </Button>
              {buscaAplicada && (
                <button
                  type="button"
                  onClick={handleLimparFiltros}
                  className="text-sm text-muted-foreground hover:text-foreground underline cursor-pointer"
                >
                  Limpar Filtros
                </button>
              )}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-0 overflow-x-auto">
            {carregando ? (
              <div className="flex flex-col items-center justify-center gap-2 p-8 text-muted-foreground">
                <Spinner size={24} aria-hidden />
                <span>Carregando...</span>
              </div>
            ) : listagemFiltrada.length === 0 ? (
              <div className="p-8 text-center text-muted-foreground">Nenhum registro encontrado.</div>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-10" />
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('nomeVaga') }}
                      >
                        Cód. Nome Vaga
                        <IconeOrdenacao chave="nomeVaga" />
                      </Button>
                    </TableHead>
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('statusDaVaga') }}
                      >
                        Status Vaga
                        <IconeOrdenacao chave="statusDaVaga" />
                      </Button>
                    </TableHead>
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('nomeCliente') }}
                      >
                        Cliente
                        <IconeOrdenacao chave="nomeCliente" />
                      </Button>
                    </TableHead>
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('nomeGestor') }}
                      >
                        Gestor
                        <IconeOrdenacao chave="nomeGestor" />
                      </Button>
                    </TableHead>
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('modeloTrabalhoDescricao') }}
                      >
                        Mod. Trabalho e Pretensão
                        <IconeOrdenacao chave="modeloTrabalhoDescricao" />
                      </Button>
                    </TableHead>
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('quantidadeDiasPresencial') }}
                      >
                        Dias Presencial
                        <IconeOrdenacao chave="quantidadeDiasPresencial" />
                      </Button>
                    </TableHead>
                    <TableHead>
                      <Button
                        variant="ghost"
                        className="h-8 px-2 -ml-2 text-left font-medium text-muted-foreground hover:text-foreground"
                        onClick={(e) => { e.stopPropagation(); handleOrdenar('dataUltimaAlteracao') }}
                      >
                        Últimas alterações
                        <IconeOrdenacao chave="dataUltimaAlteracao" />
                      </Button>
                    </TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {listagemOrdenada.map((item) => {
                    const expandido = expandidos.has(item.idCandidatura)
                    const resumoMod = resumoModeloPretensao(item.modeloTrabalhoDescricao, item.pretencaoSalarial)
                    const textoCompletoMod = textoCompletoModeloPretensao(item.modeloTrabalhoDescricao, item.pretencaoSalarial)
                    const diasPresencial =
                      item.quantidadeDiasPresencial != null
                        ? String(item.quantidadeDiasPresencial)
                        : item.disponibilidadeEntrevistaDescricao ?? 'Não registrado.'
                    return (
                      <React.Fragment key={item.idCandidatura}>
                        <TableRow
                          key={item.idCandidatura}
                          className="cursor-pointer hover:bg-muted/50"
                          onClick={() => toggleExpandir(item.idCandidatura)}
                        >
                          <TableCell className="w-10">
                            {expandido ? (
                              <ChevronUp className="h-4 w-4" />
                            ) : (
                              <ChevronDown className="h-4 w-4" />
                            )}
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-2">
                              <Star
                                className={cn(
                                  'h-4 w-4 shrink-0',
                                  item.qualificado === true && 'text-success',
                                  item.qualificado === false && 'text-destructive',
                                  (item.qualificado === null || item.qualificado === undefined) && 'text-muted-foreground',
                                )}
                                aria-hidden
                              />
                              <span>{item.codVaga} {item.nomeVaga}</span>
                            </div>
                          </TableCell>
                          <TableCell>
                            <CelulaComTooltip texto={item.statusDaVaga ?? ''}>
                              {item.statusDaVaga ?? '—'}
                            </CelulaComTooltip>
                          </TableCell>
                          <TableCell>
                            <CelulaComTooltip texto={item.nomeCliente ?? ''}>
                              {item.nomeCliente ?? '—'}
                            </CelulaComTooltip>
                          </TableCell>
                          <TableCell>
                            <CelulaComTooltip texto={item.nomeGestor ?? ''}>
                              {item.nomeGestor ?? '—'}
                            </CelulaComTooltip>
                          </TableCell>
                          <TableCell>
                            <Tooltip>
                              <TooltipTrigger asChild>
                                <span className="cursor-default block truncate max-w-[180px]">
                                  {resumoMod}
                                </span>
                              </TooltipTrigger>
                              <TooltipContent side="top" className="max-w-sm">
                                {textoCompletoMod}
                              </TooltipContent>
                            </Tooltip>
                          </TableCell>
                          <TableCell>{diasPresencial}</TableCell>
                          <TableCell>
                            <span className="text-muted-foreground">
                              {item.descricaoUltimaAlteracao ?? '—'} em {formatarData(item.dataUltimaAlteracao)}
                            </span>
                          </TableCell>
                        </TableRow>
                        {expandido && (
                          <TableRow key={`${item.idCandidatura}-expand`}>
                            <TableCell colSpan={8} className="bg-muted/30 p-4">
                              <div className="space-y-6">
                                {/* Movimentações — fechado por padrão; clique na linha do cabeçalho abre/fecha */}
                                <div>
                                  <Table>
                                    <TableHeader>
                                      <TableRow
                                        className="cursor-pointer hover:bg-muted/50"
                                        onClick={() => toggleSecao(String(item.idCandidatura), 'movimentacoes')}
                                      >
                                        <TableHead className="w-10">
                                          {secoesAbertas[String(item.idCandidatura)]?.movimentacoes ? (
                                            <ChevronUp className="h-4 w-4" />
                                          ) : (
                                            <ChevronDown className="h-4 w-4" />
                                          )}
                                        </TableHead>
                                        <TableHead colSpan={6} className="text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                                          Movimentações
                                        </TableHead>
                                      </TableRow>
                                      {secoesAbertas[String(item.idCandidatura)]?.movimentacoes && (
                                        <TableRow>
                                          <TableHead className="w-10" />
                                          <TableHead>Status Movimentação</TableHead>
                                          <TableHead>Recrutador</TableHead>
                                          <TableHead>Data da Alteração</TableHead>
                                          <TableHead>Comentário na Movimentação</TableHead>
                                          <TableHead>Rec. Responsável</TableHead>
                                          <TableHead>Arquivos</TableHead>
                                        </TableRow>
                                      )}
                                    </TableHeader>
                                    {secoesAbertas[String(item.idCandidatura)]?.movimentacoes && (
                                      <TableBody>
                                        {[...(item.alteracoes ?? [])]
                                          .sort((a, b) => {
                                            const tA = a.dataAlteracao ? new Date(a.dataAlteracao).getTime() : 0
                                            const tB = b.dataAlteracao ? new Date(b.dataAlteracao).getTime() : 0
                                            return tB - tA
                                          })
                                          .map((alt: HistoricoCandidaturaAlteracao, idx: number) => (
                                          <TableRow key={`${item.idCandidatura}-alt-${idx}`}>
                                            <TableCell className="w-10" aria-hidden />
                                            <TableCell>{alt.statusDescricao ?? '—'}</TableCell>
                                            <TableCell>{alt.recrutador ?? '—'}</TableCell>
                                            <TableCell>{formatarDataHora(alt.dataAlteracao)}</TableCell>
                                            <TableCell>
                                              <Tooltip>
                                                <TooltipTrigger asChild>
                                                  <span className="cursor-default flex items-center gap-1 truncate max-w-[200px]">
                                                    <MessageCircle className="h-3 w-3 shrink-0 text-primary" />
                                                    {previaTexto(alt.comentario)}
                                                  </span>
                                                </TooltipTrigger>
                                                <TooltipContent side="top" className="max-w-sm break-words">
                                                  {alt.comentario?.trim() || 'Sem comentários.'}
                                                </TooltipContent>
                                              </Tooltip>
                                            </TableCell>
                                            <TableCell>{alt.recrutadorResponsavel?.trim() || 'Não Atribuído.'}</TableCell>
                                            <TableCell>
                                              {(alt.arquivos ?? []).length > 0 ? (
                                                (alt.arquivos ?? []).map((ar, i) => (
                                                  <DownloadArquivo key={i} arquivo={ar} />
                                                ))
                                              ) : (
                                                <span className="text-muted-foreground">—</span>
                                              )}
                                            </TableCell>
                                          </TableRow>
                                          ))}
                                      </TableBody>
                                    )}
                                  </Table>
                                </div>
                                {/* Comentários — fechado por padrão; clique na linha do cabeçalho abre/fecha */}
                                <div>
                                  <Table>
                                    <TableHeader>
                                      <TableRow
                                        className="cursor-pointer hover:bg-muted/50"
                                        onClick={() => toggleSecao(String(item.idCandidatura), 'comentarios')}
                                      >
                                        <TableHead className="w-10">
                                          {secoesAbertas[String(item.idCandidatura)]?.comentarios ? (
                                            <ChevronUp className="h-4 w-4" />
                                          ) : (
                                            <ChevronDown className="h-4 w-4" />
                                          )}
                                        </TableHead>
                                        <TableHead colSpan={3} className="text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                                          Comentários
                                        </TableHead>
                                      </TableRow>
                                      {secoesAbertas[String(item.idCandidatura)]?.comentarios && (
                                        <TableRow>
                                          <TableHead className="w-10" />
                                          <TableHead>Recrutador</TableHead>
                                          <TableHead>Data da Inclusão</TableHead>
                                          <TableHead>Comentário da Jornada</TableHead>
                                        </TableRow>
                                      )}
                                    </TableHeader>
                                    {secoesAbertas[String(item.idCandidatura)]?.comentarios && (
                                      <TableBody>
                                        {[...(item.comentarios ?? [])]
                                          .sort((a, b) => {
                                            const tA = a.dataCriacao ? new Date(a.dataCriacao).getTime() : 0
                                            const tB = b.dataCriacao ? new Date(b.dataCriacao).getTime() : 0
                                            return tB - tA
                                          })
                                          .map((com: HistoricoCandidaturaComentario) => (
                                          <TableRow key={com.id}>
                                            <TableCell className="w-10" aria-hidden />
                                            <TableCell>{com.codigoInternoColaboradorNome ?? '—'}</TableCell>
                                            <TableCell>{formatarDataHora(com.dataCriacao)}</TableCell>
                                            <TableCell>
                                              <Tooltip>
                                                <TooltipTrigger asChild>
                                                  <span className="cursor-default flex items-center gap-1 truncate max-w-[280px]">
                                                    <MessageCircle className="h-3 w-3 shrink-0 text-primary" />
                                                    {previaTexto(com.texto)}
                                                  </span>
                                                </TooltipTrigger>
                                                <TooltipContent side="top" className="max-w-sm break-words">
                                                  {com.texto?.trim() || 'Sem comentários.'}
                                                </TooltipContent>
                                              </Tooltip>
                                            </TableCell>
                                          </TableRow>
                                          ))}
                                      </TableBody>
                                    )}
                                  </Table>
                                </div>
                              </div>
                            </TableCell>
                          </TableRow>
                        )}
                      </React.Fragment>
                    )
                  })}
                </TableBody>
              </Table>
            )}
          </CardContent>
        </Card>
      </div>

      <EditarDadosPessoaisModal
        open={editarDadosPessoaisOpen}
        onOpenChange={setEditarDadosPessoaisOpen}
        token={token}
        codigoInternoColaborador={codigoInternoColaborador ?? null}
        nomeCandidato={(nomePassadoNaNavegacao ?? nomeCandidato)?.trim() || undefined}
        onSaved={() => carregarHistorico(buscaAplicada)}
      />
    </TooltipProvider>
  )
}
