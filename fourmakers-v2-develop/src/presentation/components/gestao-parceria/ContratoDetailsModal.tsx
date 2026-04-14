import { useState, useEffect, useMemo } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { DeletarContratoUseCase } from '@domain/usecases/DeletarContratoUseCase'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { processarUrlComToken } from '@shared/utils/urlUtils'
import type { Parceiro } from '@domain/entities/Parceiro'
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
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'
import {
  Building2,
  Edit,
  Trash2,
  Download,
  FileText,
  Plus,
  ArrowUpDown,
  ArrowUp,
  ArrowDown,
} from '@/components/ui/system-icons'
import { AlertCircle, AlertTriangle } from '@/components/ui/system-icons'
import { format } from 'date-fns'
import { ptBR } from 'date-fns/locale/pt-BR'
import {
  calcularDiasParaVencimento,
  obterIndicadorVencimento,
  podeExibirIndicadorVencido,
} from '@shared/utils/contratoHelpers'

// Componente de estrela simples para avaliação
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

interface ContratoDetailsModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  parceiro: Parceiro
  onEditarParceiro?: () => void
  onNovoContrato?: () => void
  onEditarContrato?: (contratoId: string) => void
  onDownloadArquivo?: (url: string, nome: string) => void
  onContratoDeletado?: () => void
  onVerDetalhesContrato?: (contratoId: string) => void
}

export const ContratoDetailsModal = ({
  open,
  onOpenChange,
  parceiro,
  onEditarParceiro,
  onNovoContrato,
  onEditarContrato,
  onDownloadArquivo,
  onContratoDeletado,
  onVerDetalhesContrato,
}: ContratoDetailsModalProps) => {
  const { token, user } = useAppSelector((state) => state.auth)
  const [contratoParaDeletar, setContratoParaDeletar] = useState<string | null>(null)
  const [deletando, setDeletando] = useState(false)
  const [logoError, setLogoError] = useState(false)
  const [logoLoading, setLogoLoading] = useState(true)
  const [sortColumn, setSortColumn] = useState<'dataInicio' | 'dataFim' | null>(null)
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc')

  const deletarContratoUseCase = container.resolve(DeletarContratoUseCase)

  // Processar URL do logo com token (substituir $1 pelo token codificado)
  const urlLogoProcessada = processarUrlComToken(parceiro.logo, token)

  // Resetar estado do logo quando o parceiro mudar
  useEffect(() => {
    setLogoError(false)
    setLogoLoading(!!urlLogoProcessada)
  }, [urlLogoProcessada, parceiro.id])

  // Resetar ordenação quando o parceiro mudar
  useEffect(() => {
    setSortColumn(null)
    setSortDirection('asc')
  }, [parceiro.id])

  const mostrarLogo = urlLogoProcessada && !logoError

  const exibirNomeColaborador =
    parceiro.nomeColaborador != null &&
    typeof parceiro.nomeColaborador === 'string' &&
    parceiro.nomeColaborador.trim() !== ''

  // Renderizar estrelas de avaliação
  const renderAvaliacaoEstrelas = (avaliacao: number) => {
    return (
      <div className="flex items-center gap-1">
        {[1, 2, 3, 4, 5].map((estrela) => (
          <StarIcon
            key={estrela}
            filled={estrela <= avaliacao}
            className={`h-4 w-4 ${
              estrela <= avaliacao
                ? 'text-yellow-400'
                : 'text-gray-300'
            }`}
          />
        ))}
        <span className="text-sm text-muted-foreground ml-1">
          ({avaliacao.toFixed(1)}/5.0)
        </span>
      </div>
    )
  }

  const handleDeletarContrato = async () => {
    if (!contratoParaDeletar || !token || !user) return

    try {
      setDeletando(true)

      await deletarContratoUseCase.execute(token, {
        id: contratoParaDeletar,
      })

      toast.success('Contrato deletado com sucesso!')
      
      if (user) {
        logUserAction('GestaoParceria', 'DeletarContrato', {
          contratoId: contratoParaDeletar,
        }, user)
      }

      setContratoParaDeletar(null)
      onContratoDeletado?.()
    } catch (error) {
      console.error('Erro ao deletar contrato:', error)
      toast.error('Erro ao deletar contrato')
    } finally {
      setDeletando(false)
    }
  }

  const formatarData = (data: string) => {
    try {
      if (!data) return '-'
      // Extrair apenas a parte da data (sem hora)
      const dataPart = data.includes('T') ? data.split('T')[0] : data.includes(' ') ? data.split(' ')[0] : data

      // YYYY-MM-DD -> criar Date local evitando parsing UTC
      const isoMatch = dataPart.match(/^(\d{4})-(\d{2})-(\d{2})$/)
      if (isoMatch) {
        const y = parseInt(isoMatch[1], 10)
        const m = parseInt(isoMatch[2], 10)
        const d = parseInt(isoMatch[3], 10)
        return format(new Date(y, m - 1, d), 'dd/MM/yyyy', { locale: ptBR })
      }

      // DD/MM/YYYY
      const brMatch = dataPart.match(/^(\d{2})\/(\d{2})\/(\d{4})$/)
      if (brMatch) {
        const d = parseInt(brMatch[1], 10)
        const m = parseInt(brMatch[2], 10)
        const y = parseInt(brMatch[3], 10)
        return format(new Date(y, m - 1, d), 'dd/MM/yyyy', { locale: ptBR })
      }

      // Fallback: tentar parsear com Date (menos preferível)
      return format(new Date(data), 'dd/MM/yyyy', { locale: ptBR })
    } catch {
      return '-'
    }
  }

  // Função para converter data string para Date para comparação
  const parsearDataParaComparacao = (data: string | null | undefined): Date | null => {
    if (!data) return null
    try {
      const dataPart = data.includes('T') ? data.split('T')[0] : data.includes(' ') ? data.split(' ')[0] : data

      // YYYY-MM-DD
      const isoMatch = dataPart.match(/^(\d{4})-(\d{2})-(\d{2})$/)
      if (isoMatch) {
        const y = parseInt(isoMatch[1], 10)
        const m = parseInt(isoMatch[2], 10)
        const d = parseInt(isoMatch[3], 10)
        return new Date(y, m - 1, d)
      }

      // DD/MM/YYYY
      const brMatch = dataPart.match(/^(\d{2})\/(\d{2})\/(\d{4})$/)
      if (brMatch) {
        const d = parseInt(brMatch[1], 10)
        const m = parseInt(brMatch[2], 10)
        const y = parseInt(brMatch[3], 10)
        return new Date(y, m - 1, d)
      }

      return new Date(data)
    } catch {
      return null
    }
  }

  // Contratos ordenados
  const contratosOrdenados = useMemo(() => {
    if (!parceiro.contratos || parceiro.contratos.length === 0) return []
    
    const contratos = [...parceiro.contratos]
    
    if (!sortColumn) return contratos

    return contratos.sort((a, b) => {
      const dataA = parsearDataParaComparacao(sortColumn === 'dataInicio' ? a.dataInicio : a.dataFim)
      const dataB = parsearDataParaComparacao(sortColumn === 'dataInicio' ? b.dataInicio : b.dataFim)

      // Tratar nulls: nulls vão para o final
      if (!dataA && !dataB) return 0
      if (!dataA) return 1
      if (!dataB) return -1

      const comparacao = dataA.getTime() - dataB.getTime()
      return sortDirection === 'asc' ? comparacao : -comparacao
    })
  }, [parceiro.contratos, sortColumn, sortDirection])

  // Handler para ordenação
  const handleSort = (column: 'dataInicio' | 'dataFim') => {
    if (sortColumn === column) {
      // Se já está ordenando por esta coluna, alternar direção
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc')
    } else {
      // Se é uma nova coluna, começar com ascendente
      setSortColumn(column)
      setSortDirection('asc')
    }
  }

  const formatarValor = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor)
  }

  // Retorna classes CSS para destacar a linha do contrato conforme indicador de vencimento
  const getRowClassesForContrato = (c: any) => {
    try {
      const base = onVerDetalhesContrato ? 'cursor-pointer hover:bg-muted/50' : ''
      if (!c || !c.dataFim) return base

      // Status null: não acionar flag de vencido/próximo a vencimento. Apenas "Em andamento" explícito.
      if (c.status !== 'Em andamento' || c.renovado) return base

      const indicador = obterIndicadorVencimento(c.dataFim)
      switch (indicador.tipo) {
        case 'vencido':
          return `${base} bg-red-50 border-l-4 border-red-500`
        case 'venceHoje':
          return `${base} bg-red-100 border-l-4 border-red-500`
        case 'critico':
          return `${base} bg-red-50 border-l-4 border-red-400`
        case 'atencao':
          return `${base} bg-yellow-50 border-l-4 border-yellow-400`
        case 'informativo':
          return `${base} bg-blue-50 border-l-4 border-blue-400`
        default:
          return base
      }
    } catch {
      return onVerDetalhesContrato ? 'cursor-pointer hover:bg-muted/50' : ''
    }
  }

  return (
    <>
      <Dialog open={open} onOpenChange={onOpenChange}>
        <DialogContent className="max-w-6xl max-h-[90vh] overflow-hidden flex flex-col p-0 gap-0">
          {/* Header */}
          <div className="p-6 border-b bg-muted/30">
            <DialogHeader>
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                  <Building2 className="h-5 w-5 text-primary" />
                </div>
                <div className="flex-1">
                  <DialogTitle className="text-2xl font-bold">
                    {parceiro.nome}
                  </DialogTitle>
                  <DialogDescription className="mt-1">
                    Detalhes da empresa e contratos vinculados
                  </DialogDescription>
                </div>
              </div>
            </DialogHeader>
          </div>

          {/* Content - Scrollable */}
          <div className="flex-1 overflow-y-auto p-6 space-y-6">
            {/* Informações da Empresa */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle>Informações da Empresa</CardTitle>
                  <div className="flex items-center gap-3">
                    {/* Status null: não exibir badge vencido/próximo a vencimento. Apenas "Em andamento" explícito. */}
                    {parceiro.contratos && parceiro.contratos.length > 0 && (() => {
                      try {
                        const contratosEmAndamento = parceiro.contratos.filter(
                          podeExibirIndicadorVencido
                        )
                        
                        if (contratosEmAndamento.length === 0) return null
                        
                        let contratoMaisCritico = null as any
                        let menorDias = Infinity
                        for (const c of contratosEmAndamento) {
                          if (!c.dataFim) continue
                          const dias = calcularDiasParaVencimento(c.dataFim)
                          if (dias <= 150) {
                            if (Math.abs(dias) < Math.abs(menorDias) || dias < 0) {
                              menorDias = dias
                              contratoMaisCritico = c
                            }
                          }
                        }
                        if (contratoMaisCritico) {
                          const indicador = obterIndicadorVencimento(contratoMaisCritico.dataFim)
                          const Icon = indicador.tipo === 'vencido' || indicador.tipo === 'venceHoje' ? AlertCircle : AlertTriangle
                          return (
                            <span className={`px-3 py-1 rounded-full text-sm font-medium ${indicador.cor} flex items-center gap-2`}>
                              <Icon className="h-4 w-4" />
                              <span>{indicador.texto}</span>
                            </span>
                          )
                        }
                      } catch {
                        return null
                      }
                      return null
                    })()}

                    <div className="flex items-center gap-2">
                      {onEditarParceiro && (
                        <Button variant="outline" size="sm" onClick={onEditarParceiro}>
                          <Edit className="h-4 w-4 mr-2" />
                          Editar Empresa
                        </Button>
                      )}
                    </div>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Logo */}
                <div className="space-y-2">
                  <p className="text-sm text-muted-foreground">Logo</p>
                  <div className="flex items-center gap-3 relative">
                    {mostrarLogo && (
                      <img
                        src={urlLogoProcessada!}
                        alt={parceiro.nome}
                        className="h-16 w-16 rounded object-cover"
                        onLoad={() => setLogoLoading(false)}
                        onError={() => {
                          setLogoError(true)
                          setLogoLoading(false)
                        }}
                        style={{ display: logoLoading ? 'none' : 'block' }}
                      />
                    )}
                    {(logoLoading && mostrarLogo) && (
                      <div className="h-16 w-16 rounded bg-muted animate-pulse" />
                    )}
                    {(!mostrarLogo || logoError) && (
                      <div className="h-16 w-16 rounded overflow-hidden">
                        <img
                          src="/placeholder.svg"
                          alt="Logo não disponível"
                          className="h-full w-full object-cover scale-[6]"
                        />
                      </div>
                    )}
                  </div>
                </div>

                {/* Data de criação, edição e último editor */}
                {(parceiro.dataCriacao || parceiro.dataAtualizacao || exibirNomeColaborador) && (
                  <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 p-3 rounded-lg bg-muted/40 border border-borderSoft">
                    {parceiro.dataCriacao && (
                      <div>
                        <p className="text-sm text-muted-foreground">Data de criação</p>
                        <p className="text-sm font-medium">{formatarData(parceiro.dataCriacao)}</p>
                      </div>
                    )}
                    {parceiro.dataAtualizacao && (
                      <div>
                        <p className="text-sm text-muted-foreground">Data de edição</p>
                        <p className="text-sm font-medium">{formatarData(parceiro.dataAtualizacao)}</p>
                      </div>
                    )}
                    {exibirNomeColaborador && (
                      <div>
                        <p className="text-sm text-muted-foreground">Último editor</p>
                        <p className="text-sm font-medium">{parceiro.nomeColaborador!.trim()}</p>
                      </div>
                    )}
                  </div>
                )}

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-muted-foreground">Tipo</p>
                    <Badge variant="secondary">{parceiro.tipoEmpresa}</Badge>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Unidade</p>
                    <p className="text-sm font-medium">{parceiro.unidade || '-'}</p>
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Avaliação</p>
                    {renderAvaliacaoEstrelas(parceiro.avaliacao)}
                  </div>
                  {parceiro.website && (
                    <div>
                      <p className="text-sm text-muted-foreground">Website</p>
                      <a
                        href={parceiro.website}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-primary hover:underline"
                      >
                        {parceiro.website}
                      </a>
                    </div>
                  )}
                  {parceiro.linkedIn && (
                    <div>
                      <p className="text-sm text-muted-foreground">LinkedIn</p>
                      <a
                        href={parceiro.linkedIn}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-primary hover:underline"
                      >
                        {parceiro.linkedIn}
                      </a>
                    </div>
                  )}
                </div>
                {parceiro.descricaoCurta && (
                  <div>
                    <p className="text-sm text-muted-foreground mb-1">Descrição Curta</p>
                    <p className="text-sm">{parceiro.descricaoCurta}</p>
                  </div>
                )}
                {parceiro.descricaoLonga && (
                  <div>
                    <p className="text-sm text-muted-foreground mb-1">Descrição Longa</p>
                    <p className="text-sm whitespace-pre-wrap">{parceiro.descricaoLonga}</p>
                  </div>
                )}
                {parceiro.tags && parceiro.tags.length > 0 && (
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">Categorias</p>
                    <div className="flex flex-wrap gap-2">
                      {parceiro.tags.map((tag, index) => (
                        <Badge key={index} variant="outline">
                          {tag}
                        </Badge>
                      ))}
                    </div>
                  </div>
                )}
                {parceiro.contatos && parceiro.contatos.length > 0 && (
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">Contatos</p>
                    <div className="space-y-2">
                      {parceiro.contatos.map((contato, index) => (
                        <div key={index} className="p-3 border rounded-lg space-y-1">
                          {contato.nome && (
                            <p className="text-sm font-medium">{contato.nome}</p>
                          )}
                          {contato.email && (
                            <p className="text-sm text-muted-foreground">
                              <span className="font-medium">Email:</span> {contato.email}
                            </p>
                          )}
                          {contato.telefone && (
                            <p className="text-sm text-muted-foreground">
                              <span className="font-medium">Telefone:</span> {contato.telefone}
                            </p>
                          )}
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Contratos */}
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle>Contratos</CardTitle>
                  {onNovoContrato && (
                    <Button size="sm" onClick={onNovoContrato}>
                      <Plus className="h-4 w-4 mr-2" />
                      Novo Contrato
                    </Button>
                  )}
                </div>
              </CardHeader>
              <CardContent>
                {!parceiro.contratos || parceiro.contratos.length === 0 ? (
                  <div className="text-center py-8">
                    <FileText className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
                    <p className="text-sm text-muted-foreground">
                      Nenhum contrato cadastrado
                    </p>
                  </div>
                ) : (
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Nome</TableHead>
                        <TableHead>
                          <button
                            className="flex items-center gap-2 hover:opacity-80 transition-opacity"
                            onClick={() => handleSort('dataInicio')}
                          >
                            Data Início
                            {sortColumn === 'dataInicio' ? (
                              sortDirection === 'asc' ? (
                                <ArrowUp className="h-4 w-4" />
                              ) : (
                                <ArrowDown className="h-4 w-4" />
                              )
                            ) : (
                              <ArrowUpDown className="h-4 w-4 opacity-50" />
                            )}
                          </button>
                        </TableHead>
                        <TableHead>
                          <button
                            className="flex items-center gap-2 hover:opacity-80 transition-opacity"
                            onClick={() => handleSort('dataFim')}
                          >
                            Data Fim
                            {sortColumn === 'dataFim' ? (
                              sortDirection === 'asc' ? (
                                <ArrowUp className="h-4 w-4" />
                              ) : (
                                <ArrowDown className="h-4 w-4" />
                              )
                            ) : (
                              <ArrowUpDown className="h-4 w-4 opacity-50" />
                            )}
                          </button>
                        </TableHead>
                        <TableHead>Valor</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead className="text-right">Ações</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {contratosOrdenados.map((contrato) => (
                      <TableRow
                          key={contrato.id}
                          className={getRowClassesForContrato(contrato)}
                          onClick={() => onVerDetalhesContrato?.(contrato.id)}
                        >
                          <TableCell className="font-medium">
                            {contrato.contrato}
                          </TableCell>
                          <TableCell>{formatarData(contrato.dataInicio)}</TableCell>
                          <TableCell>{formatarData(contrato.dataFim)}</TableCell>
                          <TableCell>{formatarValor(contrato.valorContrato)}</TableCell>
                          <TableCell>
                            <Badge variant="secondary">{contrato.status ?? 'Em andamento'}</Badge>
                          </TableCell>
                          <TableCell className="text-right">
                            <div className="flex items-center justify-end gap-2">
                              {onEditarContrato && (
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={(event) => {
                                    event.stopPropagation()
                                    onEditarContrato(contrato.id)
                                  }}
                                >
                                  <Edit className="h-4 w-4" />
                                </Button>
                              )}
                              {contrato.arquivo && onDownloadArquivo && (
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={(event) => {
                                    event.stopPropagation()
                                    onDownloadArquivo(
                                      contrato.arquivo!.urlDownload,
                                      contrato.arquivo!.nomeArquivo
                                    )
                                  }}
                                >
                                  <Download className="h-4 w-4" />
                                </Button>
                              )}
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={(event) => {
                                  event.stopPropagation()
                                  setContratoParaDeletar(contrato.id)
                                }}
                              >
                                <Trash2 className="h-4 w-4 text-destructive" />
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Footer */}
          <div className="p-6 border-t">
            <DialogFooter>
              <Button variant="outline" onClick={() => onOpenChange(false)}>
                Fechar
              </Button>
            </DialogFooter>
          </div>
        </DialogContent>
      </Dialog>

      {/* Alert Dialog para Confirmar Exclusão de Contrato */}
      <AlertDialog
        open={!!contratoParaDeletar}
        onOpenChange={(open) => !open && setContratoParaDeletar(null)}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirmar Exclusão</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja excluir este contrato? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deletando}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeletarContrato}
              disabled={deletando}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deletando ? 'Excluindo...' : 'Excluir'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}
