import { useState, useEffect, useCallback, useRef, useMemo } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { GetMinhaEquipeContextUseCase } from '@domain/usecases/GetMinhaEquipeContextUseCase'
import { GetColaboradorRadarUseCase } from '@domain/usecases/GetColaboradorRadarUseCase'
import { AprovarSugestaoSkillUseCase } from '@domain/usecases/AprovarSugestaoSkillUseCase'
import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'
import type { MinhaEquipeSugestao } from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import { toast } from 'sonner'
import { isDevelopment } from '@shared/utils/envUtils'
import {
  mockOrquestracaoColaboradores,
  mockOrquestracaoKPIs,
  mockOrquestracaoSugestoes,
  mockOrquestracaoPDIMetas,
} from '@data/mocks/orquestracaoMock'

export const useMinhaEquipeViewModel = (isOrquestracao: boolean) => {
  const { token, user } = useAppSelector((state) => state.auth)

  // Use cases - ref para manter referência estável
  const useCasesRef = useRef<{
    getContextUseCase: GetMinhaEquipeContextUseCase
    getRadarUseCase: GetColaboradorRadarUseCase
    aprovarSugestaoUseCase: AprovarSugestaoSkillUseCase
  } | null>(null)

  if (!useCasesRef.current) {
    useCasesRef.current = {
      getContextUseCase: container.resolve(GetMinhaEquipeContextUseCase),
      getRadarUseCase: container.resolve(GetColaboradorRadarUseCase),
      aprovarSugestaoUseCase: container.resolve(AprovarSugestaoSkillUseCase),
    }
  }

  const { getContextUseCase, getRadarUseCase, aprovarSugestaoUseCase } =
    useCasesRef.current

  // Estado principal
  const [colaboradores, setColaboradores] = useState<MinhaEquipeColaborador[]>(
    []
  )
  const [kpis, setKpis] = useState<MinhaEquipeKPIs>({
    totColaboradores: 0,
    totPendentesSkills: 0,
    mediaMatch: 0,
  })
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  // Estado do modal Radar
  const [radarModalOpen, setRadarModalOpen] = useState(false)
  const [selectedColaborador, setSelectedColaborador] =
    useState<MinhaEquipeColaborador | null>(null)
  const [sugestoes, setSugestoes] = useState<MinhaEquipeSugestao[]>([])
  const [pdiMetas, setPdiMetas] = useState<MinhaEquipePDI[]>([])
  const [loadingRadar, setLoadingRadar] = useState(false)

  // Filtros
  const [searchQuery, setSearchQuery] = useState('')
  const [clienteFilter, setClienteFilter] = useState<string | null>(null)
  const [gestorFilter, setGestorFilter] = useState<string | null>(null)
  const [filtrosResetKey, setFiltrosResetKey] = useState(0) // Key para forçar re-render dos Selects

  // Ref para rastrear parâmetros anteriores e evitar chamadas duplicadas
  const lastLoadParamsRef = useRef<{
    token: string
    codGestorAdm: string
    orgId: number
    codGestorOper: string
    isOrquestracao: boolean
  } | null>(null)

  // Carregar contexto inicial
  const loadContext = useCallback(async () => {
    // Em desenvolvimento E orquestração, usar dados mock
    // Homolog e Production sempre usam dados reais
    
    if (isDevelopment() && isOrquestracao) {
      // Simular delay de carregamento
      setIsLoading(true)
      setError(null)
      
      setTimeout(() => {
        setColaboradores(mockOrquestracaoColaboradores)
        setKpis(mockOrquestracaoKPIs)
        setIsLoading(false)
      }, 500) // Simular delay de API
      
      return
    }

    const currentToken = token || localStorage.getItem('authToken')
    
    // Em desenvolvimento E orquestração, não bloquear acesso mesmo sem token
    if (!currentToken) {
      if (isDevelopment() && isOrquestracao) {
        // Em desenvolvimento e orquestração, permitir continuar sem token
        setIsLoading(false)
        return
      }
      setIsLoading(false)
      return
    }

    // Aguardar usuário estar disponível (vindo do Showme)
    if (!user) {
      // Em desenvolvimento E orquestração, não bloquear acesso
      if (isDevelopment() && isOrquestracao) {
        setIsLoading(false)
        return
      }
      // Se não há usuário, manter loading até que seja carregado
      // O useEffect nas páginas vai disparar o fetchShowmeProfile
      return
    }

    // Obter dados do usuário dentro do callback para garantir que estejam atualizados
    const orgId = user.orgId || user.colaboradorOrg?.orgId || 0
    const codGestorAdm = isOrquestracao ? '' : user.cpf || ''
    const codGestorOper = ''

    // Em desenvolvimento E orquestração, não bloquear por falta de orgId
    if (orgId === undefined || orgId === null || orgId === 0) {
      if (isDevelopment() && isOrquestracao) {
        setIsLoading(false)
        return
      }
      setIsLoading(false)
      setError('Organização não encontrada')
      return
    }

    // Se não é orquestração, garantir que codGestorAdm não esteja vazio
    if (!isOrquestracao && !codGestorAdm) {
      if (isDevelopment() && isOrquestracao) {
        setIsLoading(false)
        return
      }
      setIsLoading(false)
      setError('Código do gestor administrativo (CPF) não encontrado. Aguarde o carregamento do perfil.')
      return
    }

    // Verificar se os parâmetros mudaram para evitar chamadas duplicadas
    const currentParams = {
      token: currentToken,
      codGestorAdm,
      orgId,
      codGestorOper,
      isOrquestracao,
    }
    const lastParams = lastLoadParamsRef.current

    if (
      lastParams &&
      lastParams.token === currentParams.token &&
      lastParams.codGestorAdm === currentParams.codGestorAdm &&
      lastParams.orgId === currentParams.orgId &&
      lastParams.codGestorOper === currentParams.codGestorOper &&
      lastParams.isOrquestracao === currentParams.isOrquestracao
    ) {
      // Parâmetros não mudaram, não fazer nova chamada
      return
    }

    // Atualizar referência dos parâmetros
    lastLoadParamsRef.current = currentParams

    try {
      setIsLoading(true)
      setError(null)

      const result = await getContextUseCase.execute({
        token: currentToken,
        codGestorAdm,
        orgId,
        codGestorOper,
        isOrquestracao,
      })

      setColaboradores(result.colaboradores)
      setKpis(result.kpis)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      console.error('Erro ao carregar contexto:', err)
    } finally {
      setIsLoading(false)
    }
  }, [token, user, isOrquestracao])

  // Carregar no mount
  useEffect(() => {
    loadContext()
  }, [loadContext])

  // Abrir modal Radar
  const handleOpenRadar = useCallback(
    async (colaborador: MinhaEquipeColaborador) => {
      // Em desenvolvimento E orquestração, usar dados mock
      // Homolog e Production sempre usam dados reais
      
      if (isDevelopment() && isOrquestracao) {
        setSelectedColaborador(colaborador)
        setRadarModalOpen(true)
        setLoadingRadar(true)
        
        // Simular delay de carregamento
        setTimeout(() => {
          // Filtrar sugestões e PDI para o colaborador selecionado
          const sugestoesFiltradas = mockOrquestracaoSugestoes.filter(
            (s) => s.codigoInternoColaborador === colaborador.codigoInternoColaborador
          )
          setSugestoes(sugestoesFiltradas)
          setPdiMetas(mockOrquestracaoPDIMetas)
          setLoadingRadar(false)
        }, 500)
        
        return
      }

      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !user?.cpf) return

      setSelectedColaborador(colaborador)
      setRadarModalOpen(true)
      setLoadingRadar(true)

      try {
        const result = await getRadarUseCase.execute({
          token: currentToken,
          codInternoGestor: user.cpf,
          perfilId: colaborador.perfilId,
          codigoInternoColaborador: colaborador.codigoInternoColaborador,
          uuid_colab: colaborador.codigoInternoColaborador,
          uuid_manager: user.cpf,
        })

        setSugestoes(result.sugestoes)
        setPdiMetas(result.pdiMetas)
      } catch (err) {
        toast.error('Erro ao carregar dados do radar')
        console.error('Erro ao carregar radar:', err)
      } finally {
        setLoadingRadar(false)
      }
    },
    [token, user, getRadarUseCase, isOrquestracao]
  )

  // Função auxiliar para aprovar ou rejeitar sugestão
  const handleAprovarRejeitarSugestao = useCallback(
    async (sugestaoId: string, comentario: string, aprovado: boolean) => {
      // Em desenvolvimento E orquestração, simular aprovação/rejeição
      // Homolog e Production sempre usam API real
      
      if (isDevelopment() && isOrquestracao) {
        // Atualizar status da sugestão localmente
        setSugestoes((prev) =>
          prev.map((s) =>
            s.sugestaoId === sugestaoId
              ? { ...s, tbStatusSugestaoId: aprovado ? 1 : 3, observacao: comentario }
              : s
          )
        )
        
        toast.success(
          aprovado ? 'Sugestão aprovada com sucesso!' : 'Sugestão rejeitada',
        )
        return
      }

      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !selectedColaborador) return

      // Buscar sugestão para obter campos adicionais
      const sugestao = sugestoes.find((s) => s.sugestaoId === sugestaoId)
      if (!sugestao) {
        toast.error('Sugestão não encontrada')
        return
      }

      try {
        await aprovarSugestaoUseCase.execute(currentToken, {
          sugestaoId,
          codigoInternoColaborador: selectedColaborador.codigoInternoColaborador,
          aprovado,
          tbStatusSugestaoId: aprovado ? 1 : 3,
          perfil_Id: selectedColaborador.perfilId,
          observacao: comentario,
          skillId: sugestao.skillId,
          nivelId: sugestao.nivelId,
          itemPerfil: sugestao.perfilTipoId,
        })

        toast.success(
          aprovado ? 'Sugestão aprovada com sucesso!' : 'Sugestão rejeitada',
        )

        // Recarregar sugestões
        if (selectedColaborador) {
          await handleOpenRadar(selectedColaborador)
        }
      } catch (err) {
        toast.error(
          aprovado ? 'Erro ao aprovar sugestão' : 'Erro ao rejeitar sugestão',
        )
        console.error(
          aprovado ? 'Erro ao aprovar sugestão:' : 'Erro ao rejeitar sugestão:',
          err,
        )
      }
    },
    [token, selectedColaborador, sugestoes, aprovarSugestaoUseCase, handleOpenRadar, isOrquestracao]
  )

  // Aprovar sugestão
  const handleAprovarSugestao = useCallback(
    (sugestaoId: string, comentario: string) =>
      handleAprovarRejeitarSugestao(sugestaoId, comentario, true),
    [handleAprovarRejeitarSugestao]
  )

  // Rejeitar sugestão
  const handleRejeitarSugestao = useCallback(
    (sugestaoId: string, comentario: string) =>
      handleAprovarRejeitarSugestao(sugestaoId, comentario, false),
    [handleAprovarRejeitarSugestao]
  )

  // Limpar todos os filtros
  const handleLimparFiltros = useCallback(() => {
    setSearchQuery('')
    setClienteFilter(null)
    setGestorFilter(null)
    // Incrementar key para forçar re-render dos Selects
    setFiltrosResetKey((prev) => prev + 1)
    toast.info('Filtros limpos')
  }, [])

  // Filtrar colaboradores
  const colaboradoresFiltrados = useMemo(
    () =>
      colaboradores.filter((colab) => {
        const matchesSearch =
          searchQuery.length === 0 ||
          colab.nomeColaborador.toLowerCase().includes(searchQuery.toLowerCase())

        const matchesCliente =
          clienteFilter === null || colab.nomeCliente === clienteFilter

        const matchesGestorCliente =
          gestorFilter === null || colab.nomeGestorCliente === gestorFilter

        return matchesSearch && matchesCliente && matchesGestorCliente
      }),
    [colaboradores, searchQuery, clienteFilter, gestorFilter]
  )

  // Opções únicas de filtros
  const clientesUnicos = useMemo(
    () =>
      Array.from(
        new Set(colaboradores.map((c) => c.nomeCliente).filter(Boolean))
      ),
    [colaboradores]
  )

  const gestoresUnicos = useMemo(
    () =>
      Array.from(
        new Set(
          colaboradores
            .map((c) => c.nomeGestorCliente)
            .filter((nome) => !!nome && nome.length > 0)
        )
      ),
    [colaboradores]
  )

  return {
    // Dados
    colaboradores: colaboradoresFiltrados,
    kpis,

    // Filtros
    searchQuery,
    setSearchQuery,
    clienteFilter,
    setClienteFilter,
    gestorFilter,
    setGestorFilter,
    clientesUnicos,
    gestoresUnicos,
    filtrosResetKey,
    handleLimparFiltros,

    // Estados
    isLoading,
    error,

    // Modal Radar
    radarModalOpen,
    setRadarModalOpen,
    selectedColaborador,
    sugestoes,
    pdiMetas,
    loadingRadar,
    handleOpenRadar,
    handleAprovarSugestao,
    handleRejeitarSugestao,

    // Actions
    refreshContext: loadContext,
  }
}
