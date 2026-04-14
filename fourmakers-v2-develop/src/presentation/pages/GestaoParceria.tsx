import React, { useState, useEffect, useMemo, useRef } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { BuscarTodosParceirosUseCase } from '@domain/usecases/BuscarTodosParceirosUseCase'
import { GerarRelatorioParceriaUseCase } from '@domain/usecases/GerarRelatorioParceriaUseCase'
import { ListarUnidadesParceriaUseCase } from '@domain/usecases/ListarUnidadesParceriaUseCase'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import {
  calcularDiasParaVencimento,
  podeExibirIndicadorVencido,
} from '@shared/utils/contratoHelpers'
import { processarUrlComToken } from '@shared/utils/urlUtils'
import type { BuscarParceirosResponse, Parceiro, TipoEmpresa } from '@domain/entities/Parceiro'
import type { Contrato, StatusContrato } from '@domain/entities/Contrato'
import type { Unidade } from '@domain/entities/NotaFiscalGestao'
import { toast } from 'sonner'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Checkbox } from '@/components/ui/checkbox'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'
import {
  FileText,
  Download,
  Plus,
  Search,
  SlidersHorizontal,
  Building2,
  AlertTriangle,
  AlertCircle,
  X,
} from '@/components/ui/system-icons'

// Componente de estrela simples
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
import {
  ParceiroFormModal,
  ContratoFormModal,
  ContratoDetailsModal,
  DetalhesContratoModal,
} from '@presentation/components/gestao-parceria'

type FiltroVencimento = '' | 'vencidos' | 'proximos' | 'indeterminado'
type BucketVencimento = '' | '1-7' | '8-30' | '31-60' | '60'

const TIPOS_EMPRESA: TipoEmpresa[] = ['Benefício', 'Parceria', 'Aliança', 'Cliente', 'Fornecedor']

// Função helper para determinar status do contrato do parceiro
const determinarStatusContrato = (parceiro: Parceiro): StatusContrato | null => {
  if (!parceiro.contratos || parceiro.contratos.length === 0) {
    return null
  }

  // Prioridade: Em andamento > Completo > Arquivado. Null do backend é exibido como "Em andamento".
  const temEmAndamento = parceiro.contratos.some(
    (c) => c.status === 'Em andamento' || c.status == null
  )
  if (temEmAndamento) return 'Em andamento'

  const temCompleto = parceiro.contratos.some(
    (c) => c.status != null && c.status === 'Completo'
  )
  if (temCompleto) return 'Completo'

  const temArquivado = parceiro.contratos.some(
    (c) => c.status != null && c.status === 'Arquivado'
  )
  if (temArquivado) return 'Arquivado'

  return null
}

// Função helper para obter cor do badge de status
const obterCorStatusContrato = (status: StatusContrato): string => {
  switch (status) {
    case 'Em andamento':
      return 'bg-yellow-100 text-yellow-800 border-yellow-300'
    case 'Completo':
      return 'bg-green-100 text-green-800 border-green-300'
    case 'Arquivado':
      return 'bg-gray-100 text-gray-800 border-gray-300'
    default:
      return 'bg-muted text-muted-foreground'
  }
}

// Função helper para calcular indicador de vencimento (precisa estar antes do componente)
const calcularIndicadorVencimento = (parceiro: Parceiro) => {
  if (!parceiro.contratos || parceiro.contratos.length === 0) return null

  // Regra: quando status for null (API não informou), NÃO acionar nenhuma flag de vencido ou próximo a vencimento.
  // Apenas contratos com status explícito "Em andamento" e não renovados entram no cálculo.
  const contratosEmAndamento = parceiro.contratos.filter(
    podeExibirIndicadorVencido
  )

  if (contratosEmAndamento.length === 0) return null

  // Encontrar contrato com vencimento mais próximo ou mais crítico
  let contratoMaisCritico = null
  let menorDias = Infinity

  for (const contrato of contratosEmAndamento) {
    if (!contrato.dataFim) continue
    
    const dias = calcularDiasParaVencimento(contrato.dataFim)
    
    // Considerar apenas contratos com alerta (≤ 150 dias) ou vencidos (dias < 0)
    if (dias <= 150) {
      if (Math.abs(dias) < Math.abs(menorDias) || dias < 0) {
        menorDias = dias
        contratoMaisCritico = contrato
      }
    }
  }

  if (!contratoMaisCritico) return null

  const dias = menorDias

  // Determinar cor e ícone baseado na urgência
  if (dias < 0) {
    // Vencido
    return {
      tipo: 'vencido' as const,
      dias: Math.abs(dias),
      texto: `Vencido há ${Math.abs(dias)} dias`,
      cor: 'bg-red-500 text-white',
      icone: AlertCircle,
    }
  } else if (dias === 0) {
    // Vence Hoje
    return {
      tipo: 'venceHoje' as const,
      dias: 0,
      texto: 'Vence Hoje',
      cor: 'bg-red-500 text-white',
      icone: AlertCircle,
    }
  } else if (dias <= 30) {
    // Crítico (1-30 dias)
    return {
      tipo: 'critico' as const,
      dias,
      texto: `${dias} dias para vencer`,
      cor: 'bg-red-100 text-red-800',
      icone: AlertTriangle,
    }
  } else if (dias <= 90) {
    // Atenção (31-90 dias)
    return {
      tipo: 'atencao' as const,
      dias,
      texto: `${dias} dias para vencer`,
      cor: 'bg-yellow-100 text-yellow-800',
      icone: AlertTriangle,
    }
  } else {
    // Informativo (91-150 dias)
    return {
      tipo: 'informativo' as const,
      dias,
      texto: `${dias} dias para vencer`,
      cor: 'bg-blue-100 text-blue-800',
      icone: AlertTriangle,
    }
  }
}

// Componente separado para o Card do Parceiro (para gerenciar estado do logo)
interface ParceiroCardProps {
  parceiro: Parceiro
  indicadorVencimento: ReturnType<typeof calcularIndicadorVencimento>
  Icone: React.ComponentType<{ className?: string }> | undefined
  statusContrato: StatusContrato | null
  onVerContratos: (parceiro: Parceiro) => void
  renderAvaliacaoEstrelas: (avaliacao: number) => React.ReactElement
}

const ParceiroCard = ({
  parceiro,
  indicadorVencimento,
  Icone,
  statusContrato,
  onVerContratos,
  renderAvaliacaoEstrelas,
}: ParceiroCardProps) => {
  const { token } = useAppSelector((state) => state.auth)
  const [logoError, setLogoError] = useState(false)
  const [logoLoading, setLogoLoading] = useState(true)

  // Processar URL do logo com token (substituir $1 pelo token codificado)
  const urlLogoProcessada = processarUrlComToken(parceiro.logo, token)

  // Resetar estado do logo quando o parceiro mudar
  useEffect(() => {
    setLogoError(false)
    setLogoLoading(!!urlLogoProcessada)
  }, [urlLogoProcessada, parceiro.id])

  const mostrarLogo = urlLogoProcessada && !logoError

  return (
    <Card
      key={parceiro.id}
      className="flex flex-col h-[340px]"
    >
      {/* Tipo Parceiro e Status do Contrato - No topo do card */}
      <div className="px-6 pt-6 pb-0 flex items-center justify-between">
        {/* Tipo Parceiro - Esquerda */}
        <Badge variant="secondary" className="w-fit">
          {parceiro.tipoEmpresa}
        </Badge>
        
        {/* Status do Contrato - Direita */}
        {statusContrato && (
          <Badge 
            variant="outline" 
            className={`${obterCorStatusContrato(statusContrato)} border`}
          >
            {statusContrato}
          </Badge>
        )}
      </div>
      
      <CardHeader className="space-y-3 flex-shrink-0">
        {/* Logo e Avaliação */}
        <div className="flex items-start justify-between">
          <div className="flex flex-col gap-2">
            {/* Logo */}
            <div className="flex items-center gap-3 relative">
              {mostrarLogo && (
                <img
                  src={urlLogoProcessada!}
                  alt={parceiro.nome}
                  className="h-12 w-12 rounded object-cover"
                  onLoad={() => setLogoLoading(false)}
                  onError={() => {
                    setLogoError(true)
                    setLogoLoading(false)
                  }}
                  style={{ display: logoLoading ? 'none' : 'block' }}
                />
              )}
              {(logoLoading && mostrarLogo) && (
                <div className="h-12 w-12 rounded bg-muted animate-pulse" />
              )}
              {(!mostrarLogo || logoError) && (
                <div className="h-12 w-12 rounded overflow-hidden">
                  <img
                    src="/placeholder.svg"
                    alt="Logo não disponível"
                    className="h-full w-full object-cover scale-[6]"
                  />
                </div>
              )}
            </div>
          </div>
          {renderAvaliacaoEstrelas(parceiro.avaliacao)}
        </div>

        {/* Nome */}
        <div>
          <CardTitle className="text-lg line-clamp-1">
            {parceiro.nome}
          </CardTitle>
        </div>
      </CardHeader>

      <CardContent className="flex-1 flex flex-col p-lg pt-0 overflow-hidden">
        {/* Conteúdo flexível que pode crescer */}
        <div className="flex-1 flex flex-col min-h-0 overflow-hidden">
          {/* Descrição Curta */}
          <p className="text-sm text-muted-foreground line-clamp-2">
            {parceiro.descricaoCurta}
          </p>
        </div>

        {/* Container fixo no fundo: Badge de Vencimento (se existir) e Botão */}
        <div className="flex-shrink-0 pt-2 space-y-2">
          {/* Indicador de Vencimento - Sempre acima do botão */}
          {indicadorVencimento && Icone && (
            <Badge className={`${indicadorVencimento.cor} flex items-center gap-1 w-full justify-center`}>
              <Icone className="h-3 w-3" />
              {indicadorVencimento.texto}
            </Badge>
          )}
          
          {/* Botão de Ação - Sempre no fundo */}
          <Button
            variant="outline"
            className="w-full"
            onClick={() => onVerContratos(parceiro)}
          >
            Ver Empresa
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}

export default function GestaoParceria() {
  const { token, user } = useAppSelector((state) => state.auth)
  
  // Estado dos dados
  const [parceirosBrutos, setParceirosBrutos] = useState<Parceiro[]>([])
  const [loading, setLoading] = useState(true)
  const carregandoRef = useRef(false)

  // Filtros Frontend (processados localmente)
  const [buscarNome, setBuscarNome] = useState('')
  const [statusFiltro, setStatusFiltro] = useState<StatusContrato | ''>('')
  const [tiposEmpresaSelecionados, setTiposEmpresaSelecionados] = useState<TipoEmpresa[]>([])
  const [unidadeFiltro, setUnidadeFiltro] = useState<string>('')
  const [unidades, setUnidades] = useState<Unidade[]>([])
  const [carregandoUnidades, setCarregandoUnidades] = useState(false)
  const [tiposEmpresaPopoverOpen, setTiposEmpresaPopoverOpen] = useState(false)
  const [filtrosResetKey, setFiltrosResetKey] = useState(0) // Key para forçar re-render dos Selects
  
  // Filtros Backend (enviados na requisição)
  const [filtroVencimento, setFiltroVencimento] = useState<FiltroVencimento>('')
  const [bucketVencimento, setBucketVencimento] = useState<BucketVencimento>('')

  // Estados dos Modais
  const [parceiroModalOpen, setParceiroModalOpen] = useState(false)
  const [parceiroSelecionado, setParceiroSelecionado] = useState<Parceiro | undefined>(undefined)
  const [contratoDetailsModalOpen, setContratoDetailsModalOpen] = useState(false)
  const [contratoFormModalOpen, setContratoFormModalOpen] = useState(false)
  const [contratoParaEditar, setContratoParaEditar] = useState<{ id: string; parceiroId: string } | undefined>(undefined)
  const [detalhesContratoModalOpen, setDetalhesContratoModalOpen] = useState(false)
  const [contratoSelecionado, setContratoSelecionado] = useState<Contrato | null>(null)

  // Use Cases
  const buscarParceirosUseCase = container.resolve(BuscarTodosParceirosUseCase)
  const gerarRelatorioUseCase = container.resolve(GerarRelatorioParceriaUseCase)
  const listarUnidadesUseCase = container.resolve(ListarUnidadesParceriaUseCase)

  // Carregar parceiros do backend
  const carregarParceiros = async (
    filtroOverride?: FiltroVencimento,
    bucketOverride?: BucketVencimento
  ): Promise<BuscarParceirosResponse | null> => {
    if (!token || !user) return null
    if (carregandoRef.current) return null // Evitar chamadas duplicadas

    try {
      carregandoRef.current = true
      setLoading(true)
      const response = await buscarParceirosUseCase.execute(token, {
        orgId: user.orgId || user.colaboradorOrg?.orgId || 0,
        filtro: filtroOverride !== undefined ? filtroOverride : filtroVencimento,
        bucket: bucketOverride !== undefined ? bucketOverride : bucketVencimento,
        cursor: null,
        limite: null,
      })

      setParceirosBrutos(response.parceiros || [])
      
      // Firebase Analytics
      if (user) {
        const filtroUsado = filtroOverride !== undefined ? filtroOverride : filtroVencimento
        const bucketUsado = bucketOverride !== undefined ? bucketOverride : bucketVencimento
        logUserAction('GestaoParceria', 'ListarParceiros', {
          filtroVencimento: filtroUsado,
          bucketVencimento: bucketUsado,
        }, user)
      }

      return response
    } catch (error) {
      console.error('Erro ao carregar parceiros:', error)
      toast.error('Erro ao carregar parceiros')
      return null
    } finally {
      setLoading(false)
      carregandoRef.current = false
    }
  }

  // Aplicar filtros frontend
  const parceirosFiltrados = useMemo(() => {
    let resultado = [...parceirosBrutos]

    // Filtro por nome
    if (buscarNome.trim()) {
      const nomeLower = buscarNome.toLowerCase()
      resultado = resultado.filter((p) =>
        p.nome.toLowerCase().includes(nomeLower)
      )
    }

    // Filtro por status de contrato (null do backend conta como "Em andamento" no filtro)
    if (statusFiltro) {
      resultado = resultado.filter((p) =>
        p.contratos?.some(
          (c) =>
            c.status === statusFiltro ||
            (statusFiltro === 'Em andamento' && c.status == null)
        )
      )
    }

    // Filtro por tipos de empresa (multiselect)
    if (tiposEmpresaSelecionados.length > 0) {
      resultado = resultado.filter((p) =>
        tiposEmpresaSelecionados.includes(p.tipoEmpresa)
      )
    }

    // Filtro por unidade
    if (unidadeFiltro) {
      resultado = resultado.filter((p) =>
        p.unidade === unidadeFiltro
      )
    }

    return resultado
  }, [parceirosBrutos, buscarNome, statusFiltro, tiposEmpresaSelecionados, unidadeFiltro])

  // Gerar relatório
  const handleGerarRelatorio = async () => {
    if (!token || !user) return

    try {
      toast.info('Gerando relatório...')
      
      const { arrayBuffer, fileName } = await gerarRelatorioUseCase.execute(token)

      // Download
      const blob = new Blob([arrayBuffer])
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = fileName
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)

      toast.success('Relatório gerado com sucesso!')
      
      // Firebase Analytics
      if (user) {
        logUserAction('GestaoParceria', 'GerarRelatorio', {}, user)
      }
    } catch (error) {
      console.error('Erro ao gerar relatório:', error)
      toast.error('Erro ao gerar relatório')
    }
  }

  // Limpar todos os filtros e resetar componentes para valores padrão
  const handleLimparFiltros = () => {
    // Limpar filtros frontend (resetar para valores padrão)
    setBuscarNome('')
    setStatusFiltro('') // Resetar para string vazia (Select mostrará placeholder)
    setTiposEmpresaSelecionados([]) // Resetar array vazio
    setUnidadeFiltro('') // Resetar para string vazia (Select mostrará placeholder)
    
    // Fechar Popover de tipos de empresa se estiver aberto
    setTiposEmpresaPopoverOpen(false)
    
    // Limpar filtros backend (resetar para valores padrão)
    setFiltroVencimento('') // Resetar para string vazia (Select mostrará placeholder)
    setBucketVencimento('') // Resetar para string vazia (Select mostrará placeholder)
    
    // Incrementar key para forçar re-render dos Selects
    setFiltrosResetKey((prev) => prev + 1)
    
    // Recarregar dados após limpar filtros (passar valores vazios diretamente)
    // Isso garante que os dados sejam recarregados imediatamente, sem esperar o useEffect
    if (token && user) {
      carregarParceiros('', '')
    }
    
    toast.info('Filtros limpos')
  }

  // Toggle tipo de empresa no multiselect
  const toggleTipoEmpresa = (tipo: TipoEmpresa) => {
    setTiposEmpresaSelecionados((prev) =>
      prev.includes(tipo)
        ? prev.filter((t) => t !== tipo)
        : [...prev, tipo]
    )
  }


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
          ({avaliacao}/5)
        </span>
      </div>
    )
  }

  // Handlers dos Modais
  const handleNovoParceiro = () => {
    setParceiroSelecionado(undefined)
    setParceiroModalOpen(true)
  }

  const handleEditarParceiro = (parceiro: Parceiro) => {
    setParceiroSelecionado(parceiro)
    setParceiroModalOpen(true)
  }

  const handleVerContratos = (parceiro: Parceiro) => {
    setParceiroSelecionado(parceiro)
    setContratoDetailsModalOpen(true)
  }

  const handleVerDetalhesContrato = (contratoId: string) => {
    if (parceiroSelecionado?.contratos) {
      const contrato = parceiroSelecionado.contratos.find((c) => c.id === contratoId)
      if (contrato) {
        setContratoSelecionado(contrato)
        setDetalhesContratoModalOpen(true)
      }
    }
  }

  const handleNovoContrato = () => {
    if (parceiroSelecionado) {
      setContratoParaEditar(undefined)
      setContratoFormModalOpen(true)
    }
  }

  const handleEditarContrato = (contratoId: string) => {
    if (parceiroSelecionado) {
      setContratoParaEditar({
        id: contratoId,
        parceiroId: parceiroSelecionado.id,
      })
      setContratoFormModalOpen(true)
    }
  }

  const handleDownloadArquivo = (url: string, nome: string) => {
    const normalizedUrl = processarUrlComToken(url, token) || url
    const a = document.createElement('a')
    a.href = normalizedUrl
    a.download = nome
    document.body.appendChild(a)
    a.click()
    document.body.removeChild(a)
  }

  const handleParceiroSalvo = async () => {
    const response = await carregarParceiros()
    // Only update parceiroSelecionado if it already exists (for edit case)
    // For new parceiros, onCreateContrato will handle setting it
    if (parceiroSelecionado && response?.parceiros) {
      const parceiroAtualizado = response.parceiros.find((p) => p.id === parceiroSelecionado.id)
      if (parceiroAtualizado) {
        setParceiroSelecionado(parceiroAtualizado)
      }
    }
    // If parceiroSelecionado is not set, don't clear it - onCreateContrato will set it
  }

  const handleContratoSalvo = async (): Promise<Parceiro | null> => {
    const response = await carregarParceiros()
    // Recarregar parceiro selecionado para atualizar lista de contratos
    if (parceiroSelecionado && response?.parceiros) {
      const parceiroAtualizado = response.parceiros.find((p) => p.id === parceiroSelecionado.id)
      if (parceiroAtualizado) {
        setParceiroSelecionado(parceiroAtualizado)
        return parceiroAtualizado
      }
    }
    return null
  }

  const handleContratoDeletado = async () => {
    const response = await carregarParceiros()
    // Recarregar parceiro selecionado
    if (parceiroSelecionado && response?.parceiros) {
      const parceiroAtualizado = response.parceiros.find((p) => p.id === parceiroSelecionado.id)
      if (parceiroAtualizado) {
        setParceiroSelecionado(parceiroAtualizado)
      }
    }
  }

  // Carregar unidades
  useEffect(() => {
    const carregarUnidades = async () => {
      if (!token) return

      try {
        setCarregandoUnidades(true)
        const response = await listarUnidadesUseCase.execute(token)
        
        // Processar resposta da API
        let unidadesData: Unidade[] = []
        
        if (response && typeof response === 'object') {
          if (Array.isArray(response.retorno)) {
            unidadesData = response.retorno
              .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
              .map((u: any) => ({
                id: String(u.id || u.Id || ''),
                descricao: u.descricao || u.Descricao || '',
              }))
          } else if (response.ListaUnidadesResult && Array.isArray(response.ListaUnidadesResult)) {
            unidadesData = response.ListaUnidadesResult
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
  }, [token])

  // Carregar ao montar e quando filtros backend mudarem
  useEffect(() => {
    if (token && user) {
      carregarParceiros()
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtroVencimento, bucketVencimento, token, user])

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row items-start md:items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <Building2 className="h-8 w-8 text-primary" />
          <div>
            <h1 className="text-3xl font-bold">Gestão de Contratos</h1>
            <p className="text-muted-foreground">
              Gerencie contratos e empresas parceiras
            </p>
          </div>
        </div>

        <div className="flex gap-2">
          <Button onClick={handleGerarRelatorio} variant="outline">
            <Download className="h-4 w-4 mr-2" />
            Baixar Relatório
          </Button>
          <Button onClick={handleNovoParceiro}>
            <Plus className="h-4 w-4 mr-2" />
            Nova Empresa
          </Button>
        </div>
      </div>

      {/* Card de Filtros */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Filtros</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {/* Linha 1: Busca por nome */}
          <div className="flex gap-2">
            <div className="flex-1">
              <Input
                placeholder="Buscar por nome da empresa..."
                value={buscarNome}
                onChange={(e) => setBuscarNome(e.target.value)}
              />
            </div>
            <Button onClick={() => carregarParceiros()}>
              <Search className="h-4 w-4 mr-2" />
              Buscar
            </Button>
          </div>

          {/* Linha 2: Filtros */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            {/* Status */}
            <div className="space-y-2">
              <Label>Status do Contrato</Label>
              <Select
                key={`status-${filtrosResetKey}`}
                value={statusFiltro || undefined}
                onValueChange={(value) => setStatusFiltro(value === 'todos' ? '' : (value as StatusContrato | ''))}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Todos os Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todos os Status</SelectItem>
                  <SelectItem value="Em andamento">Em Andamento</SelectItem>
                  <SelectItem value="Completo">Completo</SelectItem>
                  <SelectItem value="Arquivado">Arquivado</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Vencimento (Backend) */}
            <div className="space-y-2">
              <Label>Vencimento</Label>
              <Select
                key={`vencimento-${filtrosResetKey}`}
                value={filtroVencimento || undefined}
                onValueChange={(value) => {
                  if (value === 'todos') {
                    setFiltroVencimento('')
                    setBucketVencimento('')
                  } else {
                    setFiltroVencimento(value as FiltroVencimento)
                    // Limpar bucket quando mudar de "vencidos" para outro
                    if (value !== 'vencidos') {
                      setBucketVencimento('')
                    }
                  }
                }}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Todos os Contratos" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todos os Contratos</SelectItem>
                  <SelectItem value="vencidos">Vencidos</SelectItem>
                  <SelectItem value="proximos">Próximos ao Vencimento</SelectItem>
                  <SelectItem value="indeterminado">Sem Vigência</SelectItem>
                </SelectContent>
              </Select>
            </div>

            {/* Unidade */}
            <div className="space-y-2">
              <Label>Unidade</Label>
              <Select
                key={`unidade-${filtrosResetKey}`}
                value={unidadeFiltro || undefined}
                onValueChange={(value) => setUnidadeFiltro(value === 'todos' ? '' : value)}
                disabled={carregandoUnidades}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder={carregandoUnidades ? "Carregando..." : "Todas as Unidades"} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todas as Unidades</SelectItem>
                  {unidades.length > 0 ? (
                    unidades.map((u) => (
                      <SelectItem key={u.id} value={u.descricao}>
                        {u.descricao}
                      </SelectItem>
                    ))
                  ) : (
                    !carregandoUnidades && (
                      <div className="px-2 py-1.5 text-sm text-muted-foreground">
                        Nenhuma unidade disponível
                      </div>
                    )
                  )}
                </SelectContent>
              </Select>
            </div>

            {/* Tipo de Empresa (Multiselect) */}
            <div className="space-y-2">
              <Label>Tipo de Empresa</Label>
              <Popover open={tiposEmpresaPopoverOpen} onOpenChange={setTiposEmpresaPopoverOpen}>
                <PopoverTrigger asChild>
                  <button
                    type="button"
                    className="flex h-10 w-full items-center justify-between rounded-lg border border-gray-300 bg-input px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 dark:border-input"
                  >
                    <span className="truncate flex-1 text-left">
                      {tiposEmpresaSelecionados.length > 0
                        ? `${tiposEmpresaSelecionados.length} selecionado(s)`
                        : 'Selecionar tipos'}
                    </span>
                    <SlidersHorizontal className="h-4 w-4 ml-2 shrink-0 opacity-50" />
                  </button>
                </PopoverTrigger>
                <PopoverContent className="w-64" align="start">
                  <div className="space-y-2">
                    {TIPOS_EMPRESA.map((tipo) => (
                      <div key={tipo} className="flex items-center space-x-2">
                        <Checkbox
                          id={`tipo-${tipo}`}
                          checked={tiposEmpresaSelecionados.includes(tipo)}
                          onCheckedChange={() => toggleTipoEmpresa(tipo)}
                        />
                        <label
                          htmlFor={`tipo-${tipo}`}
                          className="text-sm cursor-pointer flex-1 leading-none"
                        >
                          {tipo}
                        </label>
                      </div>
                    ))}
                  </div>
                </PopoverContent>
              </Popover>
            </div>
          </div>

          {/* Linha 3: Tempo de Vencimento (Condicional - só aparece se "Vencidos" selecionado) */}
          {filtroVencimento === 'vencidos' && (
            <div className="space-y-2">
              <Label>Tempo de Vencimento</Label>
              <Select
                key={`bucket-${filtrosResetKey}`}
                value={bucketVencimento || undefined}
                onValueChange={(value) => setBucketVencimento(value === 'todos' ? '' : (value as BucketVencimento))}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Todos os períodos" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="todos">Todos os períodos</SelectItem>
                  <SelectItem value="1-7">Vencidos em até 7 dias</SelectItem>
                  <SelectItem value="8-30">Vencidos de 8 a 30 dias</SelectItem>
                  <SelectItem value="31-60">Vencidos de 31 a 60 dias</SelectItem>
                  <SelectItem value="60">Vencidos há mais de 60 dias</SelectItem>
                </SelectContent>
              </Select>
            </div>
          )}

          {/* Linha 4: Limpar Filtros */}
          <div className="flex justify-end pt-2">
            <Button
              variant="outline"
              onClick={handleLimparFiltros}
              className="gap-2"
            >
              <X className="h-4 w-4" />
              Limpar Filtros
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Lista de Parceiros */}
      {loading ? (
        <div className="text-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4" />
          <p className="text-muted-foreground">Carregando parceiros...</p>
        </div>
      ) : parceirosFiltrados.length === 0 ? (
        <Card>
          <CardContent className="text-center py-12">
            <FileText className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
            <p className="text-lg font-medium mb-2">
              Nenhum parceiro encontrado
            </p>
            <p className="text-sm text-muted-foreground mb-4">
              Tente ajustar os filtros ou adicione uma nova empresa
            </p>
            <Button onClick={handleNovoParceiro}>
              <Plus className="h-4 w-4 mr-2" />
              Nova Empresa
            </Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
          {parceirosFiltrados.map((parceiro) => {
            const indicadorVencimento = calcularIndicadorVencimento(parceiro)
            const Icone = indicadorVencimento?.icone
            const statusContrato = determinarStatusContrato(parceiro)

            return (
              <ParceiroCard
                key={parceiro.id}
                parceiro={parceiro}
                indicadorVencimento={indicadorVencimento}
                Icone={Icone}
                statusContrato={statusContrato}
                onVerContratos={handleVerContratos}
                renderAvaliacaoEstrelas={renderAvaliacaoEstrelas}
              />
            )
          })}
        </div>
      )}

      {/* Modais */}
      <ParceiroFormModal
        open={parceiroModalOpen}
        onOpenChange={setParceiroModalOpen}
        parceiro={parceiroSelecionado}
        onSuccess={handleParceiroSalvo}
        onCreateContrato={async (parceiroId: string) => {
          // Recarregar lista e abrir fluxo de criação de contrato para o parceiro recém-criado
          const response = await carregarParceiros()
          const parceiroAtualizado = response?.parceiros?.find((p) => p.id === parceiroId)
          if (parceiroAtualizado) {
            // Set parceiro selecionado first to ensure ContratoFormModal can render
            setParceiroSelecionado(parceiroAtualizado)
            setContratoParaEditar(undefined)
            // Use requestAnimationFrame to ensure state is updated and component is rendered before opening modal
            requestAnimationFrame(() => {
              requestAnimationFrame(() => {
                setContratoFormModalOpen(true)
              })
            })
            return
          }

          // Tentar buscar novamente após pequeno delay caso backend demore a propagar
          setTimeout(async () => {
            const fallback = await carregarParceiros()
            const p = fallback?.parceiros?.find((pp) => pp.id === parceiroId)
            if (p) {
              setParceiroSelecionado(p)
              setContratoParaEditar(undefined)
              // Use requestAnimationFrame to ensure state is updated and component is rendered before opening modal
              requestAnimationFrame(() => {
                requestAnimationFrame(() => {
                  setContratoFormModalOpen(true)
                })
              })
            }
          }, 500)
        }}
      />

      {parceiroSelecionado && (
        <>
          <ContratoDetailsModal
            open={contratoDetailsModalOpen}
            onOpenChange={(open) => {
              setContratoDetailsModalOpen(open)
              if (!open && !contratoFormModalOpen) {
                // Limpar parceiro selecionado apenas se nenhum outro modal estiver aberto
                setParceiroSelecionado(undefined)
              }
            }}
            parceiro={parceiroSelecionado}
            onEditarParceiro={() => {
              setContratoDetailsModalOpen(false)
              handleEditarParceiro(parceiroSelecionado)
            }}
            onNovoContrato={() => {
              handleNovoContrato()
            }}
            onEditarContrato={(contratoId) => {
              handleEditarContrato(contratoId)
            }}
            onDownloadArquivo={handleDownloadArquivo}
            onContratoDeletado={handleContratoDeletado}
            onVerDetalhesContrato={handleVerDetalhesContrato}
          />

          <ContratoFormModal
            open={contratoFormModalOpen}
            onOpenChange={(open) => {
              setContratoFormModalOpen(open)
              if (!open) {
                setContratoParaEditar(undefined)
              }
            }}
            contrato={
              contratoParaEditar && parceiroSelecionado.contratos
                ? parceiroSelecionado.contratos.find((c) => c.id === contratoParaEditar.id)
                : undefined
            }
            parceiroId={parceiroSelecionado.id}
            parceiroNome={parceiroSelecionado.nome}
            onSuccess={async () => {
              setContratoFormModalOpen(false)
              setContratoParaEditar(undefined)
              const parceiroAtualizado = await handleContratoSalvo()
              // Reabrir modal de detalhes com dados atualizados
              if (parceiroAtualizado) {
                setParceiroSelecionado(parceiroAtualizado)
                setContratoDetailsModalOpen(true)
              }
            }}
          />
        </>
      )}

      <DetalhesContratoModal
        open={detalhesContratoModalOpen}
        onOpenChange={(open) => {
          setDetalhesContratoModalOpen(open)
          if (!open) {
            setContratoSelecionado(null)
          }
        }}
        contrato={contratoSelecionado}
        onDownloadArquivo={handleDownloadArquivo}
      />
    </div>
  )
}
