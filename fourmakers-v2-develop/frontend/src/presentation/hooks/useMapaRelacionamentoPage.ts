import { useState, useEffect, useRef, useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { logUserAction } from '@shared/utils/firebaseAnalytics'
import type { NoMapaRelacionamento, ClienteMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import {
  listarDepartamentosMapaRelacionamento,
  listarClientesMapa,
  setClientes,
  setClienteSelecionado,
  setEstruturaMapa,
  setNoSelecionado,
  setMostrarApenasCLevels,
} from '@app/store/slices/mapaRelacionamentoSlice'
import { moverNo, gerarIdNo, removerNoMapa, buscarNoPorIdMapa, atualizarNoMapa } from '@domain/services/mapaRelacionamentoTreeService'
import { MOCK_CLIENTES_MAPA_RELACIONAMENTO, getMockTreeForClient } from '@shared/mocks/mapaRelacionamentoMock'
import { MAPA_RELACIONAMENTO_STORAGE_PREFIX } from '@shared/constants/mapaRelacionamentoConstants'
import { exportarMapaComoJSON } from '@shared/utils/mapaRelacionamentoExportUtils'
import type { DiagramaMapaRef } from '@presentation/components/mapa-relacionamento'
import { container } from '@core/di/container'
import { ListarPosicoesUseCase } from '@domain/usecases/ListarPosicoesUseCase'
import { AtualizarPosicaoUseCase } from '@domain/usecases/AtualizarPosicaoUseCase'
import { AtualizarAlocacaoUseCase } from '@domain/usecases/AtualizarAlocacaoUseCase'
import { SalvarNoMapaRelacionamentoUseCase } from '@domain/usecases/SalvarNoMapaRelacionamentoUseCase'
import { converterPosicoesParaArvore } from '@domain/services/organogramaTreeMapper'
import type { PosicaoPayload } from '@domain/entities/Organograma'
import { toast } from 'sonner'
import { logger } from '@shared/utils/logger'
import { isDevelopment } from '@shared/utils/envUtils'
import { ListarClientesAlternativoMapaRelacionamentoUseCase } from '@domain/usecases/ListarClientesAlternativoMapaRelacionamentoUseCase'
import type { PayloadComModalState, PayloadComIsGestor } from '@shared/types/mapaRelacionamentoTypes'

export function useMapaRelacionamentoPage() {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const {
    clientes,
    clienteSelecionado,
    estruturaMapa,
    noSelecionado,
    modoVisualizacao,
    mostrarApenasCLevels,
  } = useAppSelector((state) => state.mapaRelacionamento)

  const [noEmEdicao, setNoEmEdicao] = useState<NoMapaRelacionamento | null>(null)
  const [selectedClientName, setSelectedClientName] = useState<string>('')
  const diagramaRef = useRef<DiagramaMapaRef>(null)
  const [isLoadingEstrutura, setIsLoadingEstrutura] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [showErroVinculoDepartamento, setShowErroVinculoDepartamento] = useState(false)
  const [erroVinculoMensagem, setErroVinculoMensagem] = useState<string>('')
  /** Incrementado na atualização otimista para forçar remount do Tree e o card exibir dados novos */
  const [treeDataVersion, setTreeDataVersion] = useState(0)
  const clientesLoadingRef = useRef(false)
  /** Evita reexecutar carga inicial quando state.clientes fica vazio (ex.: busca no autocomplete retornou 0). */
  const cargaInicialClientesFeitaRef = useRef(false)

  // Load clients from backend (apenas uma vez por sessão da página)
  useEffect(() => {
    if (!token || !user?.colaboradorOrg?.orgId || cargaInicialClientesFeitaRef.current || clientesLoadingRef.current) return

    const orgId = user.colaboradorOrg.orgId
    clientesLoadingRef.current = true
    cargaInicialClientesFeitaRef.current = true
    dispatch(listarClientesMapa({ token, orgId, limite: 50, cursor: 0, nomeCliente: '' }))
      .unwrap()
      .then(() => {
        clientesLoadingRef.current = false
      })
      .catch(async (error) => {
        console.error('Erro ao carregar clientes (endpoint primário):', error)
        
        // Em desenvolvimento: tentar endpoint alternativo
        if (isDevelopment()) {
          logger.debug('🔧 [DEV] Tentando endpoint alternativo para listar clientes')
          try {
            const listarClientesAlternativoUseCase = container.resolve(ListarClientesAlternativoMapaRelacionamentoUseCase)
            const clientesMapeados = await listarClientesAlternativoUseCase.execute(token, 50000)
            
            logger.debug('📋 [DEV] Clientes mapeados:', clientesMapeados)
            
            if (clientesMapeados.length > 0) {
              dispatch(setClientes(clientesMapeados))
              toast.success(`${clientesMapeados.length} clientes carregados via endpoint alternativo`)
              logger.debug('✅ [DEV] Clientes carregados com sucesso via endpoint alternativo:', clientesMapeados.length)
            } else {
              throw new Error('Nenhum cliente retornado pelo endpoint alternativo')
            }
          } catch (errorAlternativo) {
            console.error('Erro ao carregar clientes (endpoint alternativo):', errorAlternativo)
            logger.error('❌ [DEV] Erro detalhado:', errorAlternativo)
            // Fallback final para mock
            dispatch(setClientes(MOCK_CLIENTES_MAPA_RELACIONAMENTO))
            toast.error('Erro ao carregar clientes. Usando dados locais.')
          }
        } else {
          // Em outros ambientes: usar mock como fallback
          dispatch(setClientes(MOCK_CLIENTES_MAPA_RELACIONAMENTO))
          toast.error('Erro ao carregar clientes. Usando dados locais.')
        }
        
        clientesLoadingRef.current = false
      })
  }, [token, user?.colaboradorOrg?.orgId, dispatch])

  // Carrega apenas departamentos ao selecionar cliente.
  // Gestores e perfis (GestaoDeAlocados) não são usados nesta página e geram erro; o modal usa Organograma (BuscarPerfisPorOrg, ListarColaboradoresExternos).
  useEffect(() => {
    if (!token || !clienteSelecionado) return
    const codigoCliente = clienteSelecionado.codigoCliente
    dispatch(listarDepartamentosMapaRelacionamento({ token, codigoCliente })).unwrap().catch((err) => {
      logger.error('Erro ao carregar departamentos:', err)
    })
  }, [clienteSelecionado, dispatch, token])

  const recarregarEstrutura = useCallback(async (resetarFiltro = false, substituirTelaPorLoading = true): Promise<NoMapaRelacionamento | null> => {
    if (!token || !clienteSelecionado || !user?.colaboradorOrg?.orgId) return null

    const deveMostrarLoading = substituirTelaPorLoading
    if (deveMostrarLoading) {
      setIsLoadingEstrutura(true)
    }
    try {
      const orgId = user.colaboradorOrg.orgId
      const listarPosicoesUseCase = container.resolve(ListarPosicoesUseCase)
      const response = await listarPosicoesUseCase.execute(token, clienteSelecionado.codigoCliente, orgId)
      const arvore = converterPosicoesParaArvore(response.posicoes)

      if (arvore) {
        dispatch(setEstruturaMapa(arvore))
      } else {
        dispatch(setEstruturaMapa(null))
        return null
      }

      if (resetarFiltro) {
        dispatch(setMostrarApenasCLevels(false))
      }
      return arvore
    } catch (error) {
      console.error('Erro ao carregar posições:', error)
      toast.error('Erro ao carregar estrutura do organograma')
      return null
    } finally {
      if (deveMostrarLoading) {
        setIsLoadingEstrutura(false)
      }
    }
  }, [token, clienteSelecionado, user, dispatch])

  // Load organograma structure when cliente is selected
  useEffect(() => {
    if (!token || !clienteSelecionado || !user?.colaboradorOrg?.orgId) return
    // Resetar filtro ao carregar estrutura de novo cliente (carregamento inicial: substituir tela por loading)
    recarregarEstrutura(true, true)
  }, [clienteSelecionado, token, user, recarregarEstrutura])

  const handleSelecionarCliente = useCallback(
    async (codigoCliente: string, clienteFromSelector?: ClienteMapaRelacionamento) => {
      // Usar cliente repassado pelo seletor (ex.: busca) ou buscar na lista global
      const cliente = clienteFromSelector ?? clientes?.find((c) => c.codigoCliente === codigoCliente)

      // Rastreamento Firebase Analytics
      if (user) {
        logUserAction(
          'MapaRelacionamento',
          'SelecionarCliente',
          {
            clienteId: codigoCliente,
            clienteNome: cliente?.nomeCliente || codigoCliente,
          },
          user,
        )
      }

      if (cliente) {
        dispatch(setClienteSelecionado(cliente))
        setSelectedClientName(cliente.nomeCliente)
      } else {
        // Entrada manual (código sem objeto correspondente)
        dispatch(
          setClienteSelecionado({
            id: codigoCliente,
            codigoCliente,
            nomeCliente: codigoCliente,
            qtdAlocados: 0,
            qtdGestoresSemPerfil: 0,
            qtdGestores: 0,
          }),
        )
        setSelectedClientName(codigoCliente)
      }

      // O recarregamento da estrutura será feito automaticamente pelo useEffect
      // que observa mudanças em clienteSelecionado
    },
    [clientes, dispatch, user, recarregarEstrutura],
  )

  const handleSelecionarNo = useCallback((no: NoMapaRelacionamento | null) => {
    dispatch(setNoSelecionado(no))
    
    // Rastreamento Firebase Analytics - AbrirPainelVcx360
    if (no && user) {
      logUserAction(
        'MapaRelacionamento',
        'AbrirPainelVcx360',
        {
          nodeId: no.id,
          employeeCode: no.employeeId || null,
          profileId: no.profileId || null,
          employeeId: no.employeeId || null,
          departmentId: no.departmentId || null,
        },
        user
      )
    }
  }, [dispatch, user])

  const handleEditarNo = useCallback((no: NoMapaRelacionamento, isCreatingNew: boolean = false) => {
    // Rastreamento Firebase Analytics
    if (user) {
      logUserAction(
        'MapaRelacionamento',
        isCreatingNew ? 'CriarNo' : 'EditarNo',
        {
          nodeId: no.id,
          profileId: no.profileId || null,
          employeeId: no.employeeId || null,
        },
        user
      )
    }
    
    // Adicionar flag indicando se é criação ou edição
    const noComFlag = { ...no, _isCreatingNew: isCreatingNew } as NoMapaRelacionamento & { _isCreatingNew?: boolean }
    setNoEmEdicao(noComFlag)
  }, [user])

  const handleAdicionarFilho = useCallback((pai: NoMapaRelacionamento) => {
    // Verificar se o pai tem posicaoId (deve ter sido salvo antes)
    if (!pai.posicaoId) {
      toast.error('Erro: A posição pai deve ser salva antes de adicionar um subordinado. Por favor, salve a posição pai primeiro.')
      return
    }
    
    // Rastreamento Firebase Analytics
    if (user) {
      logUserAction(
        'MapaRelacionamento',
        'AdicionarFilho',
        {
          paiId: pai.id,
          paiPosicaoId: pai.posicaoId,
        },
        user
      )
    }
    
    // Criar novo nó filho
    const novoNo: NoMapaRelacionamento = {
      id: gerarIdNo(),
      profileId: null,
      employeeId: 'vacant',
      profileName: '',
      employeeName: 'Vago',
      isCLevel: false,
      isExternal: false,
      children: [],
      // Manter referência ao pai para definir organogramaPosicaoIdSuperior
      _paiId: pai.id,
      _paiPosicaoId: pai.posicaoId, // Já verificamos que existe acima
    } as NoMapaRelacionamento & { _paiId?: string; _paiPosicaoId?: string; _isCreatingNew?: boolean }
    
    // Marcar como criação
    const noComFlag = { ...novoNo, _isCreatingNew: true } as NoMapaRelacionamento & { _isCreatingNew?: boolean; _paiId?: string; _paiPosicaoId?: string }
    logger.debug('👶 Criando novo nó filho com pai:', { paiId: pai.id, paiPosicaoId: pai.posicaoId })
    setNoEmEdicao(noComFlag)
  }, [user])

  const handleSalvarNo = useCallback(async (payload: NoMapaRelacionamento) => {
    if (!token || !user || !clienteSelecionado) {
      toast.error('Erro: usuário ou cliente não selecionado')
      return
    }

    // Extrair modalState do payload (adicionado pelo EditModalMapa)
    const payloadComModalState = payload as PayloadComModalState
    const modalState = payloadComModalState.modalState

    const orgId = user.colaboradorOrg?.orgId
    if (!orgId) {
      toast.error('Erro: orgId não encontrado no usuário')
      return
    }
    const codigoCliente = clienteSelecionado.codigoCliente
    const payloadComIsGestor = payload as PayloadComIsGestor
    const isGestor = payloadComIsGestor.isGestor

    setIsSaving(true)
    try {
      const salvarNoUseCase = container.resolve(SalvarNoMapaRelacionamentoUseCase)
      const resultado = await salvarNoUseCase.execute({
        token,
        payload,
        modalState,
        orgId,
        codigoCliente,
        estruturaMapa,
        isGestor,
      })
      const posicaoIdSalvo = resultado.posicaoId

      // Atualização otimista: atualizar o nó na árvore local para o card refletir imediatamente (apenas edição, não criação)
      if (estruturaMapa && (payload.id || payload.posicaoId)) {
        const noAtual = (payload.id ? buscarNoPorIdMapa(estruturaMapa, payload.id) : null) || (payload.posicaoId ? buscarNoPorIdMapa(estruturaMapa, payload.posicaoId) : null)
        if (noAtual) {
          const dadosAtualizados: Partial<NoMapaRelacionamento> = {
            profileId: payload.profileId !== undefined ? payload.profileId : noAtual.profileId,
            employeeId: payload.employeeId !== undefined ? payload.employeeId : noAtual.employeeId,
            profileName: payload.profileName !== undefined ? payload.profileName : noAtual.profileName,
            employeeName: payload.employeeName !== undefined ? payload.employeeName : noAtual.employeeName,
            employeeEmail: payload.employeeEmail !== undefined ? payload.employeeEmail : noAtual.employeeEmail,
            employeeAvatarUrl: payload.employeeAvatarUrl !== undefined ? payload.employeeAvatarUrl : noAtual.employeeAvatarUrl,
            departmentId: payload.departmentId !== undefined ? payload.departmentId : noAtual.departmentId,
            departmentName: payload.departmentName !== undefined ? payload.departmentName : noAtual.departmentName,
            departamentoBackendId: payload.departamentoBackendId !== undefined ? payload.departamentoBackendId : noAtual.departamentoBackendId,
            perfilCorporativoBackendId: payload.perfilCorporativoBackendId !== undefined ? payload.perfilCorporativoBackendId : noAtual.perfilCorporativoBackendId,
            isCLevel: payload.isCLevel !== undefined ? payload.isCLevel : noAtual.isCLevel,
            isExternal: payload.isExternal !== undefined ? payload.isExternal : noAtual.isExternal,
            wasConnected: payload.wasConnected !== undefined ? payload.wasConnected : noAtual.wasConnected,
            alocacaoId: payload.alocacaoId !== undefined ? payload.alocacaoId : noAtual.alocacaoId,
          }
          const novaEstrutura = atualizarNoMapa(estruturaMapa, noAtual.id, {
            ...dadosAtualizados,
            children: noAtual.children,
          })
          dispatch(setEstruturaMapa(novaEstrutura))
          setTreeDataVersion((v) => v + 1)
        }
      }

      // Recarregar lista de departamentos se necessário (quando criou novo departamento)
      if (modalState?.departamento?.foiCriado) {
        dispatch(listarDepartamentosMapaRelacionamento({ token, codigoCliente }))
      }

      // Recarregar estrutura do backend; depois mesclar o nó que acabamos de salvar (criado ou editado) para o card exibir os dados corretos
      const arvoreApi = await recarregarEstrutura(false, false)
      if (arvoreApi && posicaoIdSalvo) {
        const noNaArvoreApi = buscarNoPorIdMapa(arvoreApi, posicaoIdSalvo)
        if (noNaArvoreApi) {
          const dadosMesclados: Partial<NoMapaRelacionamento> = {
            profileId: payload.profileId !== undefined ? payload.profileId : noNaArvoreApi.profileId,
            employeeId: payload.employeeId !== undefined ? payload.employeeId : noNaArvoreApi.employeeId,
            profileName: payload.profileName !== undefined ? payload.profileName : noNaArvoreApi.profileName,
            employeeName: payload.employeeName !== undefined ? payload.employeeName : noNaArvoreApi.employeeName,
            employeeEmail: payload.employeeEmail !== undefined ? payload.employeeEmail : noNaArvoreApi.employeeEmail,
            employeeAvatarUrl: payload.employeeAvatarUrl !== undefined ? payload.employeeAvatarUrl : noNaArvoreApi.employeeAvatarUrl,
            departmentId: payload.departmentId !== undefined ? payload.departmentId : noNaArvoreApi.departmentId,
            departmentName: payload.departmentName !== undefined ? payload.departmentName : noNaArvoreApi.departmentName,
            departamentoBackendId: payload.departamentoBackendId !== undefined ? payload.departamentoBackendId : noNaArvoreApi.departamentoBackendId,
            perfilCorporativoBackendId: payload.perfilCorporativoBackendId !== undefined ? payload.perfilCorporativoBackendId : noNaArvoreApi.perfilCorporativoBackendId,
            isCLevel: payload.isCLevel !== undefined ? payload.isCLevel : noNaArvoreApi.isCLevel,
            isExternal: payload.isExternal !== undefined ? payload.isExternal : noNaArvoreApi.isExternal,
            wasConnected: payload.wasConnected !== undefined ? payload.wasConnected : noNaArvoreApi.wasConnected,
            alocacaoId: payload.alocacaoId !== undefined ? payload.alocacaoId : noNaArvoreApi.alocacaoId,
          }
          const arvoreComCardAtualizado = atualizarNoMapa(arvoreApi, noNaArvoreApi.id, {
            ...dadosMesclados,
            children: noNaArvoreApi.children,
          })
          dispatch(setEstruturaMapa(arvoreComCardAtualizado))
          setTreeDataVersion((v) => v + 1)
        }
      }

      setNoEmEdicao(null)
      toast.success('Alterações salvas com sucesso')
    } catch (error) {
      console.error('Erro ao salvar nó:', error)
      toast.error('Erro ao salvar alterações. Tente novamente.')
    } finally {
      setIsSaving(false)
    }
  }, [token, user, clienteSelecionado, estruturaMapa, dispatch, recarregarEstrutura])

  const handleMoverNo = useCallback(async (origem: NoMapaRelacionamento, destino: NoMapaRelacionamento | null) => {
    if (!estruturaMapa || !token || !user || !clienteSelecionado) {
      toast.error('Erro: dados insuficientes para mover nó')
      return
    }

    // Verificar se origem tem posicaoId (ID do backend)
    if (!origem.posicaoId) {
      toast.error('Erro: posição não possui ID do backend. Salve a posição antes de movê-la.')
      return
    }

    const novoPaiId = destino?.posicaoId || null

    setIsSaving(true)
    try {
      // Buscar dados atuais da posição para atualizar apenas o pai
      const orgId = user.colaboradorOrg?.orgId
      if (!orgId) {
        toast.error('Erro: orgId não encontrado')
        return
      }
      const listarPosicoesUseCase = container.resolve(ListarPosicoesUseCase)
      const response = await listarPosicoesUseCase.execute(token, clienteSelecionado.codigoCliente, orgId)
      const posicaoAtual = response.posicoes.find((p) => p.id === origem.posicaoId)

      if (!posicaoAtual) {
        toast.error('Erro: posição não encontrada no backend')
        return
      }

      // Atualizar posição com novo pai
      if (!orgId) {
        toast.error('Erro: orgId não encontrado no usuário')
        await recarregarEstrutura(false, false)
        return
      }
      const atualizarPosicaoUseCase = container.resolve(AtualizarPosicaoUseCase)
      const posicaoPayload: PosicaoPayload = {
        orgId: orgId,
        codigoCliente: clienteSelecionado.codigoCliente,
        organogramaDepartamentoId: posicaoAtual.departamentoId || null,
        perfilCorporativoId: posicaoAtual.perfilCorporativoId || null,
        organogramaPosicaoIdSuperior: novoPaiId,
        ativo: posicaoAtual.ativo,
        cLevel: posicaoAtual.cLevel,
        profissionalExterno: posicaoAtual.profissionalExterno,
      }

      await atualizarPosicaoUseCase.execute(token, {
        ...posicaoPayload,
        id: origem.posicaoId,
      })

      // Atualizar estado local imediatamente para feedback visual
      // Se destino for null, move para raiz (sem atualizar hierarquia local, só após reload)
      if (destino) {
        const novoMapa = moverNo(estruturaMapa, origem.id, destino.id)
        if (novoMapa) {
          dispatch(setEstruturaMapa(novoMapa))
        }
      }

      // Recarregar estrutura do backend para garantir sincronização (refresh: manter diagrama visível)
      await recarregarEstrutura(false, false)

      if (destino) {
        toast.success('Posição movida com sucesso')
      } else {
        toast.success('Posição movida para raiz com sucesso')
      }

      // Rastreamento Firebase Analytics
      if (user) {
        logUserAction(
          'MapaRelacionamento',
          'MoverNo',
          {
            origemId: origem.id,
            destinoId: destino?.id || 'ROOT',
            origemPosicaoId: origem.posicaoId,
            destinoPosicaoId: destino?.posicaoId || null,
          },
          user,
        )
      }
    } catch (error) {
      console.error('Erro ao mover nó:', error)
      toast.error('Erro ao mover posição. Tente novamente.')
      // Reverter mudança local em caso de erro
      await recarregarEstrutura(false, false)
    } finally {
      setIsSaving(false)
    }
  }, [estruturaMapa, token, user, clienteSelecionado, dispatch, recarregarEstrutura])

  const handleExportJSON = useCallback(() => {
    if (!estruturaMapa || !clienteSelecionado) return
    
    // Contar nós recursivamente
    const contarNos = (no: NoMapaRelacionamento): number => {
      return 1 + no.children.reduce((acc, child) => acc + contarNos(child), 0)
    }
    const nodeCount = contarNos(estruturaMapa)
    
    // Rastreamento Firebase Analytics
    if (user) {
      logUserAction(
        'MapaRelacionamento',
        'ExportarJSON',
        {
          clienteId: clienteSelecionado.codigoCliente,
          clienteNome: selectedClientName || clienteSelecionado.nomeCliente || 'MapaRelacionamento',
          nodeCount,
        },
        user
      )
    }
    
    exportarMapaComoJSON(
      estruturaMapa,
      selectedClientName || clienteSelecionado.nomeCliente || 'MapaRelacionamento',
      clienteSelecionado.codigoCliente
    )
  }, [estruturaMapa, clienteSelecionado, selectedClientName, user])

  const handleToggleCLevels = useCallback(() => {
    const novoEstado = !mostrarApenasCLevels
    
    // Rastreamento Firebase Analytics
    if (user) {
      logUserAction(
        'MapaRelacionamento',
        'ToggleCLevels',
        {
          mostrarApenasCLevels: novoEstado,
        },
        user
      )
    }
    
    dispatch(setMostrarApenasCLevels(novoEstado))
  }, [dispatch, mostrarApenasCLevels, user])

  const handleAdicionarPrimeiraPosicao = useCallback(() => {
    // Rastreamento Firebase Analytics
    if (user && clienteSelecionado) {
      logUserAction(
        'MapaRelacionamento',
        'AdicionarPrimeiraPosicao',
        {
          clienteId: clienteSelecionado.codigoCliente,
        },
        user
      )
    }
    
    const novoNo: NoMapaRelacionamento = {
      id: gerarIdNo(),
      profileId: null,
      employeeId: 'vacant',
      profileName: '',
      employeeName: 'Vago',
      isCLevel: false,
      isExternal: false,
      children: [],
    }
    // Marcar como criação
    const noComFlag = { ...novoNo, _isCreatingNew: true } as NoMapaRelacionamento & { _isCreatingNew?: boolean }
    setNoEmEdicao(noComFlag)
  }, [user, clienteSelecionado])

  const handleResetToMock = useCallback(() => {
    if (!clienteSelecionado) return
    const codigoCliente = clienteSelecionado.codigoCliente
    const chave = `${MAPA_RELACIONAMENTO_STORAGE_PREFIX}${codigoCliente}`
    localStorage.removeItem(chave)
    const mockTree = getMockTreeForClient(codigoCliente)
    dispatch(setEstruturaMapa(mockTree))
  }, [clienteSelecionado, dispatch])

  const isMockClient = useCallback((codigoCliente: string): boolean => {
    return MOCK_CLIENTES_MAPA_RELACIONAMENTO.some((c) => c.codigoCliente === codigoCliente)
  }, [])

  const handleCentralizarArvore = useCallback(() => {
    diagramaRef.current?.centralizarArvore()
  }, [])

  const handleDeletarNo = useCallback(async (no: NoMapaRelacionamento) => {
    if (!token || !user || !clienteSelecionado || !estruturaMapa) {
      toast.error('Erro: dados insuficientes para deletar nó')
      return
    }

    // Verificar se o nó tem posicaoId (ID do backend)
    if (!no.posicaoId) {
      // Se não tem posicaoId, é um nó local que ainda não foi salvo
      // Remover da estrutura local
      const novaEstrutura = removerNoMapa(estruturaMapa, no.id)
      if (novaEstrutura) {
        dispatch(setEstruturaMapa(novaEstrutura))
        toast.success('Posição removida com sucesso')
      } else {
        toast.error('Não é possível remover a posição raiz')
      }
      return
    }

    setIsSaving(true)
    try {
      const orgId = user.colaboradorOrg?.orgId

      if (!orgId) {
        toast.error('Erro: orgId não encontrado')
        return
      }

      // Rastreamento Firebase Analytics
      if (user) {
        logUserAction(
          'MapaRelacionamento',
          'DeletarNo',
          {
            nodeId: no.id,
            posicaoId: no.posicaoId,
            profileId: no.profileId || null,
            employeeId: no.employeeId || null,
            hasChildren: !!(no.children && no.children.length > 0),
          },
          user
        )
      }

      // Buscar dados atuais da posição
      const listarPosicoesUseCase = container.resolve(ListarPosicoesUseCase)
      const response = await listarPosicoesUseCase.execute(token, clienteSelecionado.codigoCliente, orgId)

      const posicaoAtual = response.posicoes.find((p) => p.id === no.posicaoId)

      if (!posicaoAtual) {
        logger.error('❌ [DELETE] Posição não encontrada no backend')
        toast.error('Posição não encontrada no backend')
        return
      }

      // Verificar se o nó tem filhos no backend (outras posições que apontam para esta como pai)
      const temFilhos = response.posicoes.some((p) => p.posicaoIdSuperior === no.posicaoId)

      // CASO 2.1: Quando não existe nó filho
      if (!temFilhos) {
        // Atualizar alocação com ativo=false (se existir alocação com colaborador válido)
        if (posicaoAtual.alocacoes.length > 0) {
          // Procurar alocação com colaborador válido (não vago)
          // Priorizar alocação ativa, senão pegar a mais recente
          const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'
          const alocacaoComColaborador = posicaoAtual.alocacoes
            .filter((a) => a.codigoInternoColaborador && a.codigoInternoColaborador !== EMPTY_GUID)
            .sort((a, b) => {
              // Priorizar ativa, depois por data mais recente
              if (a.ativo && !b.ativo) return -1
              if (!a.ativo && b.ativo) return 1
              return new Date(b.dataCriacao).getTime() - new Date(a.dataCriacao).getTime()
            })[0]

          if (alocacaoComColaborador) {
            const payload = {
              id: alocacaoComColaborador.id,
              orgId,
              codigoCliente: clienteSelecionado.codigoCliente,
              organogramaPosicaoId: no.posicaoId,
              codigoInternoColaborador: alocacaoComColaborador.codigoInternoColaborador,
              dataInicio: alocacaoComColaborador.dataInicio,
              dataFim: alocacaoComColaborador.dataFim,
              ativo: false,
            }

            const atualizarAlocacaoUseCase = container.resolve(AtualizarAlocacaoUseCase)
            await atualizarAlocacaoUseCase.execute(token, payload)
          }
        }

        // Atualizar posição com ativo=false
        const atualizarPosicaoUseCase = container.resolve(AtualizarPosicaoUseCase)
        const posicaoPayload: PosicaoPayload = {
          orgId,
          codigoCliente: clienteSelecionado.codigoCliente,
          organogramaDepartamentoId: posicaoAtual.departamentoId || null,
          perfilCorporativoId: posicaoAtual.perfilCorporativoId || null,
          organogramaPosicaoIdSuperior: posicaoAtual.posicaoIdSuperior,
          ativo: false, // Inativar posição
          cLevel: posicaoAtual.cLevel,
          profissionalExterno: posicaoAtual.profissionalExterno,
        }

        await atualizarPosicaoUseCase.execute(token, {
          ...posicaoPayload,
          id: no.posicaoId,
        })

        // Recarregar organograma (refresh: manter diagrama visível)
        await recarregarEstrutura(false, false)
        toast.success('Posição removida com sucesso')
      } 
      // CASO 2.2: Quando existe nó filho
      else {
        // Buscar pai no backend usando posicaoIdSuperior
        const paiPosicaoId = posicaoAtual.posicaoIdSuperior || null

        // Salvar referência dos filhos diretos do backend
        const filhosDirectos = response.posicoes.filter((p) => p.posicaoIdSuperior === no.posicaoId)

        if (isDevelopment()) {
          logger.debug('🔍 [DELETE] Caso 2.2 - Com filhos', { posicaoAtual, alocacoes: posicaoAtual.alocacoes })
        }

        // Atualizar alocação com ativo=false (se existir alocação com colaborador válido)
        if (posicaoAtual.alocacoes.length > 0) {
          // Procurar alocação com colaborador válido (não vago)
          // Priorizar alocação ativa, senão pegar a mais recente
          const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'
          const alocacaoComColaborador = posicaoAtual.alocacoes
            .filter((a) => a.codigoInternoColaborador && a.codigoInternoColaborador !== EMPTY_GUID)
            .sort((a, b) => {
              // Priorizar ativa, depois por data mais recente
              if (a.ativo && !b.ativo) return -1
              if (!a.ativo && b.ativo) return 1
              return new Date(b.dataCriacao).getTime() - new Date(a.dataCriacao).getTime()
            })[0]

          if (alocacaoComColaborador) {
            const payload = {
              id: alocacaoComColaborador.id,
              orgId,
              codigoCliente: clienteSelecionado.codigoCliente,
              organogramaPosicaoId: no.posicaoId,
              codigoInternoColaborador: alocacaoComColaborador.codigoInternoColaborador,
              dataInicio: alocacaoComColaborador.dataInicio,
              dataFim: alocacaoComColaborador.dataFim,
              ativo: false,
            }

            const atualizarAlocacaoUseCase = container.resolve(AtualizarAlocacaoUseCase)
            await atualizarAlocacaoUseCase.execute(token, payload)
          }
        }

        // Atualizar posição com ativo=false
        const atualizarPosicaoUseCase = container.resolve(AtualizarPosicaoUseCase)
        const posicaoPayload: PosicaoPayload = {
          orgId,
          codigoCliente: clienteSelecionado.codigoCliente,
          organogramaDepartamentoId: posicaoAtual.departamentoId || null,
          perfilCorporativoId: posicaoAtual.perfilCorporativoId || null,
          organogramaPosicaoIdSuperior: posicaoAtual.posicaoIdSuperior,
          ativo: false, // Inativar posição
          cLevel: posicaoAtual.cLevel,
          profissionalExterno: posicaoAtual.profissionalExterno,
        }

        await atualizarPosicaoUseCase.execute(token, {
          ...posicaoPayload,
          id: no.posicaoId,
        })

        // Atualizar posição dos filhos diretos com posição superior usando o pai do nó deletado (em paralelo)
        const promessasFilhos = filhosDirectos
          .filter((filho) => filho.id)
          .map((filho) => {
            const posicaoFilhoPayload: PosicaoPayload = {
              orgId,
              codigoCliente: clienteSelecionado.codigoCliente,
              organogramaDepartamentoId: filho.departamentoId || null,
              perfilCorporativoId: filho.perfilCorporativoId || null,
              organogramaPosicaoIdSuperior: paiPosicaoId,
              ativo: filho.ativo,
              cLevel: filho.cLevel,
              profissionalExterno: filho.profissionalExterno,
            }
            return atualizarPosicaoUseCase.execute(token, {
              ...posicaoFilhoPayload,
              id: filho.id,
            })
          })
        await Promise.all(promessasFilhos)
        
        // Recarregar organograma (refresh: manter diagrama visível)
        await recarregarEstrutura(false, false)
        toast.success('Posição removida e filhos realocados com sucesso')
      }
    } catch (error) {
      logger.error('❌ [DELETE] Erro ao deletar posição:', error)

      const errorMessage = error instanceof Error ? error.message : 'Erro ao deletar posição. Tente novamente.'
      logger.error('❌ [DELETE] Error message:', errorMessage)
      
      // Verificar se o erro é relacionado a vínculo com departamento
      const errorMessageLower = errorMessage.toLowerCase()
      const isErroVinculoDepartamento = 
        (errorMessageLower.includes('vínculo') || errorMessageLower.includes('vinculo')) && 
        (errorMessageLower.includes('departamento') || errorMessageLower.includes('departamento'))
      
      if (isErroVinculoDepartamento) {
        setErroVinculoMensagem(errorMessage)
        setShowErroVinculoDepartamento(true)
      } else {
        toast.error(errorMessage)
      }
      
      // Reverter mudança local em caso de erro
      await recarregarEstrutura(false, false)
    } finally {
      setIsSaving(false)
    }
  }, [token, user, clienteSelecionado, estruturaMapa, dispatch, recarregarEstrutura])

  return {
    // State
    token,
    clientes,
    clienteSelecionado,
    estruturaMapa,
    noSelecionado,
    noEmEdicao,
    selectedClientName,
    modoVisualizacao,
    mostrarApenasCLevels,
    diagramaRef,
    isLoadingEstrutura,
    isSaving,
    treeDataVersion,
    
    // Handlers
    handleSelecionarCliente,
    handleSelecionarNo,
    handleEditarNo,
    handleAdicionarFilho,
    handleSalvarNo,
    handleMoverNo,
    handleExportJSON,
    handleToggleCLevels,
    handleAdicionarPrimeiraPosicao,
    handleResetToMock,
    handleCentralizarArvore,
    handleDeletarNo,
    isMockClient,
    setNoEmEdicao,
    setSelectedClientName,
    recarregarEstrutura,
    showErroVinculoDepartamento,
    setShowErroVinculoDepartamento,
    erroVinculoMensagem,
  }
}
