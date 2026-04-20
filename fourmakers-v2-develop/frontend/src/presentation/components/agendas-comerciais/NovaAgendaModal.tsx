import { Spinner } from '@/components/ui/spinner'
import { format } from 'date-fns'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { Button } from '@/components/ui/button'
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
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { cn } from '@/lib/utils'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { aceitarConviteAgenda, atualizarAgenda, criarAgenda } from '@app/store/slices/agendasComerciaisSlice'
import { fetchColaboradores } from '@app/store/slices/colaboradoresSlice'
import { setMicrosoftAccessToken } from '@app/store/slices/authSlice'
import { container } from '@core/di/container'
import { DiTokens } from '@core/di/tokens'
import type { AtualizarAgendaPayload, CriarAgendaPayload, ItemAgendaGestor } from '@domain/entities/AgendaGestor'
import type { Colaborador } from '@domain/entities/Colaborador'
import { CriarReuniaoTeamsUseCase } from '@domain/usecases/CriarReuniaoTeamsUseCase'
import { ListarClientesUseCase } from '@domain/usecases/ListarClientesUseCase'
import { InserirGestorExternoModal } from '@presentation/components/mapa-relacionamento/modais/InserirGestorExternoModal'
import { useGestoresExternos, type GestorExterno } from '@presentation/hooks/useGestoresExternos'
import { useMicrosoftAuth } from '@presentation/hooks/useMicrosoftAuth'
import { MSAL_NAO_CONFIGURADO_MENSAGEM } from '@core/auth/msalConfig'
import { getCodInternoColaborador } from '@shared/utils/agendaUtils'
import {
  formatarDataAgendadaParaPayload,
  formatarDataHoraAgendaParaPayload,
  parseDataAgendaParaExibicao,
  parseDataHoraAgendaParaExibicao,
} from '@shared/utils/timezoneAgendaUtils'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import { clearFieldError, clearFieldErrors } from '@shared/utils/formUtils'
import { mapearTipoInteracao, validarFormularioAgenda } from '@shared/utils/validacaoAgenda'
import { toast } from 'sonner'
import { CamposDataHora } from './CamposDataHora'
import { SelecaoCliente } from './SelecaoCliente'
import { SelecaoColaboradores, type ColaboradorSelectOption } from './SelecaoColaboradores'
import { SelecaoGestores } from './SelecaoGestores'

interface NovaAgendaModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  agendaParaEditar?: ItemAgendaGestor | null
  /** Quando definido, abre no modo "Nova Agenda com Cliente": cliente pré-preenchido e desabilitado, agendaPaiId no payload. */
  agendaPai?: ItemAgendaGestor | null
  onSuccess?: () => void
}

const TIPOS_INTERACAO = [
  { value: 1, label: 'Reunião' },
  { value: 2, label: 'Ligação' },
  { value: 3, label: 'Chat' },
  { value: 4, label: 'Email' },
  { value: 5, label: 'Presencial' },
]

// Status não é editável - preservado do agenda original (conforme Flutter)

// Helper para visibilidade condicional
const getCamposVisiveisPorTipoInteracao = (tipoInteracao: number) => {
  // Reunião (1) e Presencial (5): mostram início/fim e local
  if (tipoInteracao === 1 || tipoInteracao === 5) {
    return {
      showInicioFim: true,
      showLocal: true,
      showLink: false, // Link não é um campo separado, vai no campo "local" para Reunião
    }
  }
  // Ligação (2), Chat (3), Email (4): não mostram início/fim nem local
  return {
    showInicioFim: false,
    showLocal: false,
    showLink: false,
  }
}

export function NovaAgendaModal({ open, onOpenChange, agendaParaEditar, agendaPai, onSuccess }: NovaAgendaModalProps) {
  const dispatch = useAppDispatch()
  const { token, user, microsoftAccessToken } = useAppSelector((state) => state.auth)
  const { status: agendaStatus } = useAppSelector((state) => state.agendasComerciais)
  const { colaboradores, status: colaboradoresStatus } = useAppSelector((state) => state.colaboradores)
  const { isAuthenticated: isMicrosoftAuthenticated, signIn: signInMicrosoft } = useMicrosoftAuth()
  const isEditing = !!agendaParaEditar

  const [colaboradoresSelecionados, setColaboradoresSelecionados] = useState<ColaboradorSelectOption[]>([])
  const [colaboradorSearch, setColaboradorSearch] = useState('')
  const [debouncedColaboradorSearch, setDebouncedColaboradorSearch] = useState('')
  const [isColaboradorPopoverOpen, setIsColaboradorPopoverOpen] = useState(false)

  const [gestoresSelecionados, setGestoresSelecionados] = useState<GestorExterno[]>([])
  const [gestorSearch, setGestorSearch] = useState('')
  const [isGestorPopoverOpen, setIsGestorPopoverOpen] = useState(false)
  const [inserirGestorExternoModalOpen, setInserirGestorExternoModalOpen] = useState(false)
  const [gestoresRefreshTrigger, setGestoresRefreshTrigger] = useState(0)
  const gestoresJaMapeadosRef = useRef(false) // Flag para rastrear se gestores já foram mapeados

  const [clientes, setClientes] = useState<Array<{ id: string; name: string }>>([])
  const [clienteSearch, setClienteSearch] = useState('')
  const [debouncedClienteSearch, setDebouncedClienteSearch] = useState('')
  const [clienteSelecionado, setClienteSelecionado] = useState<{ id: string; name: string } | null>(null)
  const [isClientePopoverOpen, setIsClientePopoverOpen] = useState(false)
  const [clientsLoading, setClientsLoading] = useState(false)

  // Hook para buscar gestores externos quando cliente for selecionado
  const { gestores, loading: gestoresLoading } = useGestoresExternos({
    token,
    codigoCliente: clienteSelecionado?.id || null,
    busca: gestorSearch,
    enabled: !!clienteSelecionado,
    refreshTrigger: gestoresRefreshTrigger,
  })

  // Forçar refresh dos gestores quando um novo gestor for criado
  const handleGestorExternoCriado = useCallback(() => {
    // Incrementar trigger para forçar refresh no hook
    setGestoresRefreshTrigger((prev) => prev + 1)
    // Limpar busca para mostrar todos os gestores atualizados
    setGestorSearch('')
  }, [])

  // Estados do formulário (validação manual)
  const [titulo, setTitulo] = useState('')
  const [descricao, setDescricao] = useState('')
  const [tipoInteracao, setTipoInteracao] = useState<number>(1)
  const [statusOriginal, setStatusOriginal] = useState<string>('Pendente') // Preservar status original
  const [dataAgendada, setDataAgendada] = useState<Date | null>(null)
  const [horaInicio, setHoraInicio] = useState<string>('')
  const [horaFim, setHoraFim] = useState<string>('')
  const [localizacao, setLocalizacao] = useState('')
  const [linkReuniao, setLinkReuniao] = useState('')
  const [errors, setErrors] = useState<{ [campo: string]: string }>({})
  const [confirmacaoTeamsOpen, setConfirmacaoTeamsOpen] = useState(false)
  const [erroTeamsOpen, setErroTeamsOpen] = useState(false)
  const [erroTeamsMensagem, setErroTeamsMensagem] = useState('')
  const [loadingTeams, setLoadingTeams] = useState(false)

  const orgId = user?.colaboradorOrg?.orgId || 0

  // Campos visíveis baseado no tipo de interação
  const camposVisiveis = useMemo(
    () => getCamposVisiveisPorTipoInteracao(tipoInteracao),
    [tipoInteracao],
  )

  // Debounce do termo de busca de colaboradores
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedColaboradorSearch(colaboradorSearch)
    }, 300)
    return () => clearTimeout(timer)
  }, [colaboradorSearch])

  // Buscar colaboradores quando debounced search mudar ou modal abrir (listagem com fourtalents=true para agendas)
  useEffect(() => {
    if (open && token && orgId > 0) {
      dispatch(
        fetchColaboradores({
          token,
          orgId,
          cursor: 0,
          limite: 1000,
          nomeOuEmail: debouncedColaboradorSearch.trim(),
          fourtalents: true,
        }),
      )
    }
  }, [open, token, orgId, dispatch, debouncedColaboradorSearch])

  const colaboradoresIds = useMemo(
    () => colaboradores.map(c => c.cpf || c.codColaborador).filter((id): id is string => Boolean(id)),
    [colaboradores]
  )

  // Estabilizar array de gestores para evitar re-execuções desnecessárias do useEffect
  const gestoresIds = useMemo(
    () => gestores.map(g => g.codGestorExterno).filter((id): id is string => Boolean(id)),
    [gestores]
  )

  // Preencher colaboradores selecionados quando estiver editando e colaboradores forem carregados.
  // Garantir que o responsável pela criação (codColaboradorCriador) sempre apareça na lista.
  useEffect(() => {
    if (
      isEditing &&
      agendaParaEditar &&
      colaboradoresIds.length > 0 &&
      colaboradoresSelecionados.length === 0 &&
      colaboradoresStatus !== 'loading'
    ) {
      const codCriador = agendaParaEditar.codColaboradorCriador
      const nomeCriador = agendaParaEditar.responsavel || 'Responsável pela agenda'

      let opcoes: ColaboradorSelectOption[] = (agendaParaEditar.colaboradores ?? [])
        .map((colabAgenda) =>
          colaboradores.find(
            (c) =>
              c.cpf === colabAgenda.codInternoColaborador ||
              c.codColaborador === colabAgenda.codInternoColaborador,
          )
        )
        .filter((c): c is Colaborador => c !== undefined)
        .reduce<ColaboradorSelectOption[]>((acc, c) => {
          const id = c.cpf || c.codColaborador
          if (id && !acc.some((o) => o.cpf === id || o.codColaborador === id)) {
            acc.push({ cpf: c.cpf, codColaborador: c.codColaborador, nome: c.nome, email: c.email })
          }
          return acc
        }, [])

      const idCriadorPresente = codCriador && opcoes.some((o) => o.cpf === codCriador || o.codColaborador === codCriador)
      if (codCriador && !idCriadorPresente) {
        const criadorCompleto = colaboradores.find(
          (c) => c.cpf === codCriador || c.codColaborador === codCriador,
        )
        if (criadorCompleto) {
          opcoes = [{ cpf: criadorCompleto.cpf, codColaborador: criadorCompleto.codColaborador, nome: criadorCompleto.nome, email: criadorCompleto.email }, ...opcoes]
        } else {
          opcoes = [{ cpf: codCriador, codColaborador: codCriador, nome: nomeCriador, email: '' }, ...opcoes]
        }
      }

      if (opcoes.length > 0) {
        setColaboradoresSelecionados(opcoes)
      }
    }
  }, [
    isEditing,
    agendaParaEditar?.colaboradores,
    agendaParaEditar?.codColaboradorCriador,
    agendaParaEditar?.responsavel,
    colaboradoresIds.length,
    colaboradoresSelecionados.length,
    colaboradoresStatus,
    colaboradores,
  ])

  // A API já retorna os colaboradores filtrados, não é necessário filtrar localmente

  // Quantidade de participantes é calculada automaticamente baseada nos colaboradores selecionados

  const handleSelectColaborador = useCallback((colaborador: Colaborador) => {
    if (!colaborador) {
      return // Validação defensiva
    }
    const colaboradorId = colaborador.cpf || colaborador.codColaborador
    if (!colaboradorId) {
      return // Não adicionar colaborador sem identificador válido
    }
    setColaboradoresSelecionados((prev) => {
      const jaExiste = prev.find((c) =>
        (c.cpf && colaborador.cpf && c.cpf === colaborador.cpf) ||
        (c.codColaborador && colaborador.codColaborador && c.codColaborador === colaborador.codColaborador)
      )
      if (!jaExiste) {
        return [...prev, colaborador]
      }
      return prev
    })
    setColaboradorSearch('')
    // Não fechar o popover automaticamente para permitir seleção múltipla
  }, [])

  const handleRemoveColaborador = useCallback((idColaborador: string) => {
    if (!idColaborador) {
      return // Validação defensiva
    }
    setColaboradoresSelecionados((prev) =>
      prev.filter((c) => c.cpf !== idColaborador && c.codColaborador !== idColaborador)
    )
  }, [])

  // Handlers para gestores externos
  const handleSelectGestor = useCallback((gestor: GestorExterno) => {
    if (!gestor) {
      return // Validação defensiva
    }
    // Permitir gestores temporários (com tempId) ou gestores com codGestorExterno válido
    if (!gestor.codGestorExterno) {
      return // Validação defensiva - gestor deve ter um identificador
    }
    setGestoresSelecionados((prev) => {
      if (!prev.find((g) => g?.codGestorExterno && g.codGestorExterno === gestor.codGestorExterno)) {
        return [...prev, gestor]
      }
      return prev
    })
    setGestorSearch('')
  }, [])

  const handleRemoveGestor = useCallback((codGestorExterno: string) => {
    if (!codGestorExterno) {
      return // Validação defensiva
    }
    setGestoresSelecionados((prev) => {
      const filtered = prev.filter((g) => {
        // Comparar codGestorExterno, permitindo também gestores temporários
        return g?.codGestorExterno && g.codGestorExterno !== codGestorExterno
      })
      // Limpar erro de gestores quando remover (se ainda houver gestores selecionados)
      if (errors.gestores && filtered.length > 0) {
        setErrors((prevErrors) => clearFieldError(prevErrors, 'gestores'))
      }
      return filtered
    })
  }, [errors.gestores])

  // Debounce do termo de busca de clientes
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedClienteSearch(clienteSearch)
    }, 300)
    return () => clearTimeout(timer)
  }, [clienteSearch])

  // Buscar clientes quando o popover abrir ou termo de busca debounced mudar
  useEffect(() => {
    if (!token || !isClientePopoverOpen) {
      return
    }

    const loadClients = async () => {
      setClientsLoading(true)
      try {
        const useCase = container.resolve(ListarClientesUseCase)
        const data = await useCase.execute(token, debouncedClienteSearch.trim())
        // Atualizar sempre que o popover estiver aberto
        setClientes(data)
      } catch (error) {
        console.error('Erro ao buscar clientes:', error)
        setClientes([])
      } finally {
        setClientsLoading(false)
      }
    }

    void loadClients()
  }, [token, debouncedClienteSearch, isClientePopoverOpen])

  // Validar formulário usando função utilitária
  const validarFormulario = (): boolean => {
    const dados = {
      titulo,
      tipoInteracao,
      dataAgendada,
      horaInicio,
      horaFim,
      clienteSelecionado,
      gestoresSelecionados,
      linkReuniao,
    }

    const newErrors = validarFormularioAgenda(dados)
    setErrors(newErrors)

    if (Object.keys(newErrors).length > 0) {
      const erros = Object.keys(newErrors)
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

  // Preencher formulário sempre que o modal abrir para edição (open + agendaParaEditar).
  // Inclui id da agenda nas deps para garantir re-sincronização ao abrir para outra agenda.
  const agendaIdParaEditar = agendaParaEditar?.agendaId ?? (agendaParaEditar?.id ? String(agendaParaEditar.id) : null)
  useEffect(() => {
    if (agendaParaEditar && open) {
      setTitulo(agendaParaEditar.titulo || '')
      setDescricao(agendaParaEditar.descricao || '')
      setTipoInteracao(mapearTipoInteracao(agendaParaEditar.tipoInteracao))
      // Garantir que o status nunca seja string vazia
      const statusValido = agendaParaEditar.status?.trim() || 'Pendente'
      setStatusOriginal(statusValido)
      // Para Reunião, linkReuniao pode estar em localizacao. Para Presencial, localizacao normal
      if (agendaParaEditar.tipoInteracao === 'Reunião') {
        setLinkReuniao(agendaParaEditar.linkReuniao || agendaParaEditar.localizacao || '')
        setLocalizacao('')
      } else if (agendaParaEditar.tipoInteracao === 'Presencial') {
        setLocalizacao(agendaParaEditar.localizacao || '')
        setLinkReuniao('')
      } else {
        setLocalizacao('')
        setLinkReuniao('')
      }

      // Preencher cliente selecionado se existir
      // IMPORTANTE: Limpar colaboradores e gestores para forçar repopulação ao reabrir (o effect de preenchimento só roda quando length === 0)
      setColaboradoresSelecionados([])
      setGestoresSelecionados([])
      setGestorSearch('')
      setIsGestorPopoverOpen(false) // Fechar popover de gestores
      gestoresJaMapeadosRef.current = false
      if (agendaParaEditar.cliente?.codigoCliente) {
        setClienteSelecionado({
          id: agendaParaEditar.cliente.codigoCliente,
          name: agendaParaEditar.cliente.nomeCliente,
        })
      }

      // Preencher gestores selecionados se existirem (após gestores serem carregados)
      // Isso será feito em um useEffect separado quando gestores forem carregados

      if (agendaParaEditar.data) {
        const dataParsed = parseDataAgendaParaExibicao(agendaParaEditar.data)
        setDataAgendada(dataParsed)
      }
      if (agendaParaEditar.dataInicio) {
        const inicioDate = parseDataHoraAgendaParaExibicao(agendaParaEditar.dataInicio)
        setHoraInicio(inicioDate ? format(inicioDate, 'HH:mm') : '')
      } else {
        setHoraInicio('')
      }
      if (agendaParaEditar.dataFim) {
        const fimDate = parseDataHoraAgendaParaExibicao(agendaParaEditar.dataFim)
        setHoraFim(fimDate ? format(fimDate, 'HH:mm') : '')
      } else {
        setHoraFim('')
      }

      setErrors({})
    } else if (!agendaParaEditar && open) {
      // Resetar formulário (ou pré-preencher cliente quando for "Nova Agenda com Cliente")
      setTitulo('')
      setDescricao('')
      setTipoInteracao(1)
      setStatusOriginal('Pendente')
      setDataAgendada(null)
      setHoraInicio('')
      setHoraFim('')
      setLocalizacao('')
      setLinkReuniao('')
      setColaboradoresSelecionados(() => {
        if (!user?.colaborador) {
          return []
        }
        return [
          {
            cpf: user.colaborador.cpf,
            nome: user.colaborador.nomeCompleto,
            email: user.colaborador.email,
          },
        ]
      })
      setColaboradorSearch('')
      setGestoresSelecionados([])
      setGestorSearch('')
      if (agendaPai?.cliente?.codigoCliente) {
        setClienteSelecionado({
          id: agendaPai.cliente.codigoCliente,
          name: agendaPai.cliente.nomeCliente,
        })
      } else {
        setClienteSelecionado(null)
      }
      setClienteSearch('')
      setErrors({})
      gestoresJaMapeadosRef.current = false
    }
  }, [agendaParaEditar, open, agendaIdParaEditar, agendaPai?.cliente?.codigoCliente, agendaPai?.cliente?.nomeCliente])

  // Ao fechar o modal: resetar todo o estado do formulário para que na próxima abertura
  // os dados sejam sempre carregados a partir dos props (evita exibir dados vazios ou de outra agenda).
  useEffect(() => {
    if (!open) {
      setTitulo('')
      setDescricao('')
      setTipoInteracao(1)
      setStatusOriginal('Pendente')
      setDataAgendada(null)
      setHoraInicio('')
      setHoraFim('')
      setLocalizacao('')
      setLinkReuniao('')
      setErrors({})
      setColaboradoresSelecionados([])
      setColaboradorSearch('')
      setGestoresSelecionados([])
      setGestorSearch('')
      setClienteSelecionado(null)
      setClienteSearch('')
      setIsColaboradorPopoverOpen(false)
      setIsGestorPopoverOpen(false)
      setIsClientePopoverOpen(false)
      gestoresJaMapeadosRef.current = false
    }
  }, [open])

  // Resetar flag quando modal abrir/fechar ou agenda mudar
  useEffect(() => {
    if (!open || !isEditing) {
      gestoresJaMapeadosRef.current = false
    }
  }, [open, isEditing])

  // Preencher gestores selecionados quando estiver editando e gestores forem carregados
  // IMPORTANTE: Aguardar o carregamento da listagem de gestores antes de preencher
  // IMPORTANTE: Só preencher se o cliente selecionado for o mesmo da agenda (não preencher se cliente mudou)
  useEffect(() => {
    const clienteAgenda = agendaParaEditar?.cliente?.codigoCliente
    const clienteSelecionadoId = clienteSelecionado?.id

    if (
      isEditing &&
      agendaParaEditar?.gestoresExternos &&
      agendaParaEditar.gestoresExternos.length > 0 &&
      clienteSelecionado &&
      clienteAgenda === clienteSelecionadoId && // Só preencher se o cliente for o mesmo da agenda
      !gestoresLoading && // Aguardar carregamento terminar (pode ter gestores ou não)
      !gestoresJaMapeadosRef.current // Só mapear uma vez
    ) {
      const gestoresEncontrados: GestorExterno[] = []

      for (const gestorAgenda of agendaParaEditar.gestoresExternos) {
        if (!gestorAgenda) continue // Validação defensiva

        // Quando codigoInternoColaborador é nulo, priorizar match e exibição por codGestorExterno
        const codigoInternoNulo = !gestorAgenda.codigoInternoColaborador?.trim()

        // 1) Tentar encontrar por codGestorExterno (sempre, principalmente quando codigoInternoColaborador é nulo)
        if (gestorAgenda.codGestorExterno) {
          const porCodGestor = gestores.find(
            (g) => g?.codGestorExterno && g.codGestorExterno === gestorAgenda.codGestorExterno,
          )
          if (porCodGestor) {
            gestoresEncontrados.push(porCodGestor)
            continue
          }
        }

        // 2) Se codigoInternoColaborador existe, tentar match por ele
        if (!codigoInternoNulo) {
          const porCodInterno = gestores.find(
            (g) => g?.codigoInternoColaborador && g.codigoInternoColaborador === gestorAgenda.codigoInternoColaborador,
          )
          if (porCodInterno) {
            gestoresEncontrados.push(porCodInterno)
            continue
          }
        }

        // 3) Tentar match por nome e email
        if (gestorAgenda.nome) {
          const porNomeEmail = gestores.find(
            (g) =>
              g?.nome && g.nome === gestorAgenda.nome &&
              (gestorAgenda.email ? (g?.email && g.email === gestorAgenda.email) : true)
          )
          if (porNomeEmail) {
            gestoresEncontrados.push(porNomeEmail)
            continue
          }
        }

        // 4) Criar gestor temporário com dados da agenda
        // Quando codigoInternoColaborador é nulo, usar codGestorExterno como identificador principal
        const tempId = gestorAgenda.codGestorExterno || `temp-${gestorAgenda.nome || 'gestor'}-${gestorAgenda.email || 'sem-email'}`
        gestoresEncontrados.push({
          codGestorExterno: tempId,
          codigoInternoColaborador: gestorAgenda.codigoInternoColaborador?.trim() || '',
          nome: gestorAgenda.nome || 'Nome não informado',
          email: gestorAgenda.email || '',
        })
      }

      if (gestoresEncontrados.length > 0) {
        setGestoresSelecionados(gestoresEncontrados)
        gestoresJaMapeadosRef.current = true // Marcar como mapeado
      } else if (gestores.length === 0 && !gestoresLoading) {
        // Se não encontrou nenhum gestor e a lista está vazia (não carregando),
        // ainda assim marcar como mapeado para evitar tentativas infinitas
        gestoresJaMapeadosRef.current = true
      }
    }
  }, [
    isEditing,
    agendaParaEditar?.gestoresExternos,
    agendaParaEditar?.cliente?.codigoCliente,
    gestoresIds, // Usar IDs estabilizados ao invés do array completo
    clienteSelecionado?.id, // Usar apenas o ID para evitar re-execuções
    gestoresLoading,
  ])

  const executarEnvio = useCallback(
    async (teamsResult?: { id: string; joinUrl: string }) => {
      if (!token || !dataAgendada) return
      let codigosColaboradores = colaboradoresSelecionados
        .map((c) => c.cpf?.trim())
        .filter((cpf): cpf is string => Boolean(cpf))
      if (!isEditing && user?.cpf) {
        const cpfCriador = user.cpf.trim()
        if (!codigosColaboradores.includes(cpfCriador)) {
          codigosColaboradores = [cpfCriador, ...codigosColaboradores]
        }
      }
      const codigosGestoresExternos = gestoresSelecionados
        .map((g) => g.codGestorExterno?.trim())
        .filter((cod): cod is string => Boolean(cod))
      const dataInicioCompleta = horaInicio && dataAgendada
        ? new Date(`${format(dataAgendada, 'yyyy-MM-dd')}T${horaInicio}:00`)
        : undefined
      const dataFimCompleta = horaFim && dataAgendada
        ? new Date(`${format(dataAgendada, 'yyyy-MM-dd')}T${horaFim}:00`)
        : undefined
      const quantidadeParticipantes = codigosColaboradores.length + gestoresSelecionados.length
      const linkFinal = teamsResult?.joinUrl ?? (tipoInteracao === 1 ? linkReuniao.trim() || undefined : undefined)
      const payload: CriarAgendaPayload | AtualizarAgendaPayload = {
        ...(isEditing && agendaParaEditar?.agendaId ? { id: agendaParaEditar.agendaId } : {}),
        titulo: titulo.trim(),
        descricao: descricao.trim() || undefined,
        tipoInteracao,
        dataAgendada: formatarDataAgendadaParaPayload(dataAgendada),
        dataInicio: dataInicioCompleta ? formatarDataHoraAgendaParaPayload(dataInicioCompleta) : undefined,
        dataFim: dataFimCompleta ? formatarDataHoraAgendaParaPayload(dataFimCompleta) : undefined,
        status: statusOriginal.trim() || 'Pendente',
        localizacao: tipoInteracao === 1 ? (linkFinal ?? undefined) : (localizacao.trim() || undefined),
        linkReuniao: tipoInteracao === 1 ? (linkFinal ?? undefined) : undefined,
        ...(teamsResult ? { graphEventId: teamsResult.id } : {}),
        quantidadeParticipantes,
        codigoCliente: clienteSelecionado?.id,
        codigosColaboradores: codigosColaboradores.length > 0 ? codigosColaboradores : undefined,
        codigosGestoresExternos: codigosGestoresExternos.length > 0 ? codigosGestoresExternos : undefined,
        ...(agendaPai != null && typeof agendaPai.agendaId === 'number' && !isEditing
          ? { agendaPaiId: agendaPai.agendaId }
          : {}),
      }
      try {
      const codigoCriador = getCodInternoColaborador(user) || user?.cpf || user?.colaborador?.cpf
      
      if (isEditing && agendaParaEditar?.agendaId) {
        await dispatch(atualizarAgenda({ token, payload: payload as AtualizarAgendaPayload })).unwrap()
        toast.success('Agenda atualizada', { description: 'A agenda foi atualizada com sucesso' })
        
        // Após editar, confirmar presença do usuário na agenda
        if (codigoCriador) {
          try {
            await dispatch(
              aceitarConviteAgenda({
                token,
                agendaId: String(agendaParaEditar.agendaId),
                codigoColaborador: codigoCriador.trim(),
                decisaoStatus: 1,
              }),
            ).unwrap()
          } catch (error) {
            // Silencioso na edição - pode já estar aceito
            console.warn('Não foi possível confirmar presença na agenda editada:', error)
          }
        }
      } else {
        const result = await dispatch(criarAgenda({ token, payload: payload as CriarAgendaPayload })).unwrap()
        toast.success('Agenda criada', { description: 'A agenda foi criada com sucesso' })

        const agendaIdCriada = result.id
        if (agendaPai?.agendaId && agendaIdCriada && user) {
          logUserAction(
            'AgendasComerciais',
            'NovaAgendaComClienteCriada',
            { agendaPaiId: agendaPai.agendaId, novaAgendaId: agendaIdCriada, titulo: titulo.trim() },
            user
          )
        }

        // Após criar, chamar AceitarRecusarConvite para confirmar presença do criador na agenda
        if (agendaIdCriada && codigoCriador) {
          try {
            await dispatch(
              aceitarConviteAgenda({
                token,
                agendaId: String(agendaIdCriada),
                codigoColaborador: codigoCriador.trim(),
                decisaoStatus: 1,
              }),
            ).unwrap()
          } catch (error) {
            toast.error('Convite não confirmado', {
              description: error instanceof Error ? error.message : 'Não foi possível confirmar sua presença na agenda. Tente aceitar o convite manualmente.',
            })
          }
        }
      }

      onOpenChange(false)
      onSuccess?.()
    } catch (error) {
      toast.error('Erro', {
        description: error instanceof Error ? error.message : 'Erro ao salvar agenda',
      })
    }
  },
  [
    token,
    dataAgendada,
    colaboradoresSelecionados,
    gestoresSelecionados,
    horaInicio,
    horaFim,
    titulo,
    descricao,
    tipoInteracao,
    statusOriginal,
    linkReuniao,
    localizacao,
    clienteSelecionado?.id,
    agendaPai,
    agendaParaEditar?.agendaId,
    isEditing,
    user,
    dispatch,
    onOpenChange,
    onSuccess,
  ]
  )

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!token) {
      toast.error('Erro', { description: 'Token de autenticação não encontrado' })
      return
    }
    if (!validarFormulario()) return

    // Oferecer criar reunião no Teams quando: nova agenda (não edição) e tipo Reunião.
    // Se MSAL não estiver configurado e o usuário clicar em "Sim", o fluxo exibe mensagem de erro.
    if (!isEditing && tipoInteracao === 1) {
      setConfirmacaoTeamsOpen(true)
      return
    }
    await executarEnvio()
  }

  const handleConfirmacaoTeamsSim = useCallback(async () => {
    if (!dataAgendada) return
    setLoadingTeams(true)
    try {
      if (!isMicrosoftAuthenticated && !microsoftAccessToken) {
        try {
          const result = await signInMicrosoft()
          if (result?.accessToken) {
            dispatch(setMicrosoftAccessToken(result.accessToken))
          }
        } catch (loginError) {
          setLoadingTeams(false)
          setConfirmacaoTeamsOpen(false)
          const mensagem =
            loginError instanceof Error && loginError.message === MSAL_NAO_CONFIGURADO_MENSAGEM
              ? 'Integração com Microsoft Teams não está configurada para este ambiente. Entre em contato com o administrador ou verifique as variáveis de ambiente (VITE_MICROSOFT_CLIENT_ID, etc.) e reconstrua a aplicação.'
              : loginError instanceof Error
                ? `Erro ao autenticar no Microsoft: ${loginError.message}`
                : 'Erro ao autenticar no Microsoft. Faça login para criar reuniões no Teams.'
          setErroTeamsMensagem(mensagem)
          setErroTeamsOpen(true)
          return
        }
      }

      const participantes: Array<{ email: string; nome?: string }> = []
      colaboradoresSelecionados.forEach((c) => {
        const email = (c.email ?? '').trim()
        if (email && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
          participantes.push({ email, nome: c.nome })
        }
      })
      gestoresSelecionados.forEach((g) => {
        const email = (g.email ?? '').trim()
        if (email && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
          participantes.push({ email, nome: g.nome })
        }
      })
      const dataInicioCompleta = horaInicio && dataAgendada
        ? new Date(`${format(dataAgendada, 'yyyy-MM-dd')}T${horaInicio}:00`)
        : undefined
      const dataFimCompleta = horaFim && dataAgendada
        ? new Date(`${format(dataAgendada, 'yyyy-MM-dd')}T${horaFim}:00`)
        : undefined
      const dataInicioPadrao = dataAgendada
        ? new Date(`${format(dataAgendada, 'yyyy-MM-dd')}T09:00:00`)
        : null
      const dataFimPadrao = dataAgendada
        ? new Date(`${format(dataAgendada, 'yyyy-MM-dd')}T10:00:00`)
        : null
      const useCase = container.resolve(DiTokens.criarReuniaoTeamsUseCase) as CriarReuniaoTeamsUseCase
      const resultado = await useCase.execute(
        {
          titulo: titulo.trim() || 'Reunião',
          dataInicioIso: dataInicioCompleta
            ? formatarDataHoraAgendaParaPayload(dataInicioCompleta)
            : formatarDataHoraAgendaParaPayload(dataInicioPadrao),
          dataFimIso: dataFimCompleta
            ? formatarDataHoraAgendaParaPayload(dataFimCompleta)
            : formatarDataHoraAgendaParaPayload(dataFimPadrao),
          participantes,
        },
        microsoftAccessToken,
      )
      setConfirmacaoTeamsOpen(false)
      setLoadingTeams(false)
      await executarEnvio(resultado)
    } catch (error) {
      setLoadingTeams(false)
      setConfirmacaoTeamsOpen(false)
      setErroTeamsMensagem(error instanceof Error ? error.message : 'Erro ao criar reunião no Teams')
      setErroTeamsOpen(true)
    }
  }, [
    dataAgendada,
    colaboradoresSelecionados,
    gestoresSelecionados,
    horaInicio,
    horaFim,
    titulo,
    executarEnvio,
    isMicrosoftAuthenticated,
    signInMicrosoft,
    microsoftAccessToken,
    dispatch,
  ])

  const handleConfirmacaoTeamsNao = useCallback(() => {
    setConfirmacaoTeamsOpen(false)
    void executarEnvio()
  }, [executarEnvio])

  const handleErroTeamsContinuar = useCallback(() => {
    setErroTeamsOpen(false)
    void executarEnvio()
  }, [executarEnvio])

  const handleErroTeamsCancelar = useCallback(() => {
    setErroTeamsOpen(false)
  }, [])

  const isLoading = agendaStatus === 'loading'

  // Verificar se alguma listagem está carregando
  const isLoadingLists = colaboradoresStatus === 'loading' || gestoresLoading || clientsLoading

  // Desabilitar campos enquanto listagens estão carregando (especialmente ao editar)
  const isFormDisabled = isLoading || (isEditing && isLoadingLists)

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? 'Editar Agenda' : agendaPai ? 'Nova Agenda com Cliente' : 'Nova Agenda Comercial'}
          </DialogTitle>
          <DialogDescription>
            {isEditing
              ? 'Atualize os dados da agenda comercial.'
              : agendaPai
                ? 'Crie uma agenda vinculada ao mesmo cliente da agenda selecionada.'
                : 'Preencha os campos para criar uma nova agenda comercial.'}
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Tipo de Interação - PRIMEIRO */}
          <div className="space-y-2">
            <Label htmlFor="tipoInteracao">
              Tipo de Interação <span className="text-destructive">*</span>
            </Label>
            <Select
              value={tipoInteracao.toString()}
              onValueChange={(value) => {
                setTipoInteracao(parseInt(value))

                // Limpar campos quando tipo mudar
                const novosCamposVisiveis = getCamposVisiveisPorTipoInteracao(parseInt(value))
                const tipoAnterior = tipoInteracao

                if (!novosCamposVisiveis.showInicioFim) {
                  setHoraInicio('')
                  setHoraFim('')
                }

                // Limpar local e link quando tipo mudar
                if (!novosCamposVisiveis.showLocal) {
                  setLocalizacao('')
                  setLinkReuniao('')
                } else {
                  // Se mudou de Reunião para Presencial ou vice-versa, limpar campos
                  if ((tipoAnterior === 1 && parseInt(value) === 5) || (tipoAnterior === 5 && parseInt(value) === 1)) {
                    setLocalizacao('')
                    setLinkReuniao('')
                  }
                }

                // Limpar erros relacionados aos campos que não são mais visíveis
                const camposParaLimpar: string[] = ['tipoInteracao']
                if (!novosCamposVisiveis.showInicioFim) {
                  camposParaLimpar.push('dataInicio', 'dataFim')
                }
                if (!novosCamposVisiveis.showLocal) {
                  camposParaLimpar.push('linkReuniao')
                }
                setErrors(clearFieldErrors(errors, camposParaLimpar))
              }}
            >
              <SelectTrigger
                id="tipoInteracao"
                className={cn(errors.tipoInteracao && 'border-destructive')}
              >
                <SelectValue placeholder="Selecione o tipo" />
              </SelectTrigger>
              <SelectContent>
                {TIPOS_INTERACAO.map((tipo) => (
                  <SelectItem key={tipo.value} value={tipo.value.toString()}>
                    {tipo.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.tipoInteracao && <p className="text-sm text-destructive">{errors.tipoInteracao}</p>}
          </div>

          {/* Cliente - MOVIDO PARA O INÍCIO (desabilitado quando agenda filha = agendaPai definido) */}
          <SelecaoCliente
            clienteSelecionado={clienteSelecionado}
            clientes={clientes}
            clienteSearch={clienteSearch}
            clientsLoading={clientsLoading}
            isPopoverOpen={isClientePopoverOpen}
            error={errors.cliente}
            disabled={isFormDisabled || !!agendaPai}
            onClienteSelect={(cliente) => {
              // Limpar gestores e busca de gestores quando cliente muda
              setGestoresSelecionados([])
              setGestorSearch('')
              setIsGestorPopoverOpen(false) // Fechar popover de gestores
              gestoresJaMapeadosRef.current = false // Resetar flag quando cliente mudar
              // Atualizar cliente selecionado
              setClienteSelecionado(cliente)
              setIsClientePopoverOpen(false)
              setClienteSearch('')
              if (errors.cliente) {
                setErrors(clearFieldError(errors, 'cliente'))
              }
              // Limpar erro de gestores se houver
              if (errors.gestores) {
                setErrors(clearFieldError(errors, 'gestores'))
              }
            }}
            onSearchChange={setClienteSearch}
            onPopoverChange={setIsClientePopoverOpen}
          />

          {/* Gestores Externos - MOVIDO PARA O INÍCIO */}
          <SelecaoGestores
            gestoresSelecionados={gestoresSelecionados}
            gestores={gestores}
            gestorSearch={gestorSearch}
            gestoresLoading={gestoresLoading}
            isPopoverOpen={isGestorPopoverOpen}
            clienteSelecionado={clienteSelecionado}
            error={errors.gestores}
            disabled={isFormDisabled}
            onGestorSelect={handleSelectGestor}
            onGestorRemove={handleRemoveGestor}
            onSearchChange={setGestorSearch}
            onPopoverChange={setIsGestorPopoverOpen}
            onErrorClear={() => {
              if (errors.gestores) {
                setErrors(clearFieldError(errors, 'gestores'))
              }
            }}
            onAdicionarGestor={() => setInserirGestorExternoModalOpen(true)}
          />

          {/* Colaboradores - MOVIDO PARA O INÍCIO */}
          <SelecaoColaboradores
            colaboradoresSelecionados={colaboradoresSelecionados}
            colaboradores={colaboradores}
            colaboradorSearch={colaboradorSearch}
            colaboradoresStatus={colaboradoresStatus}
            isPopoverOpen={isColaboradorPopoverOpen}
            disabled={isFormDisabled}
            onColaboradorSelect={handleSelectColaborador}
            onColaboradorRemove={handleRemoveColaborador}
            onSearchChange={setColaboradorSearch}
            onPopoverChange={setIsColaboradorPopoverOpen}
            ownerColaboradorId={
              isEditing
                ? agendaParaEditar?.codColaboradorCriador || agendaParaEditar?.colaboradores?.[0]?.codInternoColaborador
                : user?.colaborador?.cpf
            }
          />

          {/* Título - DEPOIS DOS SELETORES */}
          <div className="space-y-2">
            <Label htmlFor="titulo">
              Título da Agenda <span className="text-destructive">*</span>
            </Label>
            <Input
              id="titulo"
              value={titulo}
              onChange={(e) => {
                setTitulo(e.target.value)
                if (errors.titulo) {
                  setErrors(clearFieldError(errors, 'titulo'))
                }
              }}
              error={!!errors.titulo}
              placeholder="Ex.: Daily / Kick-off"
              maxLength={80}
            />
            {errors.titulo && <p className="text-sm text-destructive">{errors.titulo}</p>}
          </div>

          {/* Data e Hora */}
          <CamposDataHora
            dataAgendada={dataAgendada}
            horaInicio={horaInicio}
            horaFim={horaFim}
            tipoInteracao={tipoInteracao}
            errors={errors}
            showInicioFim={camposVisiveis.showInicioFim}
            onDataChange={(date) => {
              setDataAgendada(date || null)
              if (errors.dataAgendada) {
                setErrors(clearFieldError(errors, 'dataAgendada'))
              }
            }}
            onHoraInicioChange={(hora) => {
              setHoraInicio(hora)
              if (errors.dataInicio) {
                setErrors(clearFieldError(errors, 'dataInicio'))
              }
            }}
            onHoraFimChange={(hora) => {
              setHoraFim(hora)
              if (errors.dataFim) {
                setErrors(clearFieldError(errors, 'dataFim'))
              }
            }}
            onErrorClear={(campo) => {
              setErrors(clearFieldError(errors, campo))
            }}
          />

          {/* Local - visível apenas para Reunião e Presencial */}
          {camposVisiveis.showLocal && (
            <div className="space-y-2">
              <Label htmlFor="localizacao">
                {tipoInteracao === 1 ? 'Link da Reunião (Teams)' : 'Localização'}
              </Label>
              <Input
                id="localizacao"
                value={tipoInteracao === 1 ? linkReuniao : localizacao}
                onChange={(e) => {
                  if (tipoInteracao === 1) {
                    setLinkReuniao(e.target.value)
                    if (errors.linkReuniao) {
                      setErrors(clearFieldError(errors, 'linkReuniao'))
                    }
                  } else {
                    setLocalizacao(e.target.value)
                  }
                }}
                type={tipoInteracao === 1 ? 'url' : 'text'}
                placeholder={
                  tipoInteracao === 1
                    ? 'https://teams.microsoft.com/l/meetup-join/...'
                    : 'Ex: Sala de reuniões, Endereço físico...'
                }
                error={tipoInteracao === 1 ? !!errors.linkReuniao : false}
              />
              {tipoInteracao === 1 && errors.linkReuniao && (
                <p className="text-sm text-destructive">{errors.linkReuniao}</p>
              )}
            </div>
          )}

          {/* Descrição - DEPOIS DO LOCAL/LINK */}
          <div className="space-y-2">
            <Label htmlFor="descricao">Descrição (opcional)</Label>
            <Textarea
              id="descricao"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              placeholder="Descreva os detalhes da agenda..."
              rows={3}
              maxLength={255}
            />
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => {
                onOpenChange(false)
              }}
              disabled={isLoading || loadingTeams}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={isLoading || loadingTeams}>
              {(isLoading || loadingTeams) && <Spinner size={16} className="mr-2 text-primary" />}
              {isEditing ? 'Atualizar' : 'Criar'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>

      {/* Modal para adicionar gestor externo - só aparece quando cliente está selecionado */}
      {clienteSelecionado && (
        <InserirGestorExternoModal
          open={inserirGestorExternoModalOpen}
          onOpenChange={setInserirGestorExternoModalOpen}
          codigoCliente={clienteSelecionado.id}
          onSuccess={handleGestorExternoCriado}
        />
      )}

      {/* Confirmação: criar reunião no Microsoft Teams (nova agenda, tipo Reunião) */}
      <AlertDialog open={confirmacaoTeamsOpen} onOpenChange={setConfirmacaoTeamsOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Criar reunião no Microsoft Teams?</AlertDialogTitle>
            <AlertDialogDescription>
              Será gerado um link de reunião que será associado à agenda. Os participantes serão convidados automaticamente.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={handleConfirmacaoTeamsNao} disabled={loadingTeams}>
              Não, continuar sem
            </AlertDialogCancel>
            <AlertDialogAction onClick={(e) => { e.preventDefault(); void handleConfirmacaoTeamsSim() }} disabled={loadingTeams}>
              {loadingTeams ? <Spinner size={16} className="mr-2 text-primary" /> : null}
              Sim, criar reunião
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Erro ao criar reunião Teams */}
      <AlertDialog open={erroTeamsOpen} onOpenChange={(open) => !open && setErroTeamsOpen(false)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Não foi possível criar reunião no Teams</AlertDialogTitle>
            <AlertDialogDescription>
              {erroTeamsMensagem}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={handleErroTeamsCancelar}>
              Cancelar
            </AlertDialogCancel>
            <AlertDialogAction onClick={handleErroTeamsContinuar}>
              Continuar sem Teams
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </Dialog>
  )
}
