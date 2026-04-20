import { useState, useEffect, useCallback, useMemo, useRef } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { GetMinhaJornadaContextUseCase } from '@domain/usecases/GetMinhaJornadaContextUseCase'
import { UpdateSkillInterestUseCase } from '@domain/usecases/UpdateSkillInterestUseCase'
import { UpdateSkillLevelUseCase } from '@domain/usecases/UpdateSkillLevelUseCase'
import { CreatePdiUseCase } from '@domain/usecases/CreatePdiUseCase'
import { AddSkillToPerfil360UseCase } from '@domain/usecases/AddSkillToPerfil360UseCase'
import { SuggestNewSkillUseCase } from '@domain/usecases/SuggestNewSkillUseCase'
import { GetPDIsColaboradorUseCase } from '@domain/usecases/GetPDIsColaboradorUseCase'
import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { MinhaJornadaProfile } from '@domain/entities/MinhaJornadaProfile'
import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'
import type { MinhaJornadaInteresse } from '@domain/entities/MinhaJornadaInteresse'
import type { MinhaJornadaSugestao } from '@domain/entities/MinhaJornadaSugestao'
import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'
import type { MinhaJornadaRepository } from '@domain/repositories/MinhaJornadaRepository'
import { DiTokens } from '@core/di/tokens'
import { calcularGaps, getSeniorityTier } from '@shared/utils/minhaJornadaComparators'

export interface SkillGroup {
  type: MinhaJornadaSkillType
  skills: MinhaJornadaSkill[]
  metCount: number
  totalCount: number
}

export const useMinhaJornadaViewModel = () => {
  const { token, user } = useAppSelector((state) => state.auth)

  // Use cases - usando useRef para manter referência estável
  const useCasesRef = useRef<{
    getContextUseCase: GetMinhaJornadaContextUseCase
    updateInterestUseCase: UpdateSkillInterestUseCase
    updateLevelUseCase: UpdateSkillLevelUseCase
    createPdiUseCase: CreatePdiUseCase
    addSkillUseCase: AddSkillToPerfil360UseCase
    suggestSkillUseCase: SuggestNewSkillUseCase
    getPdisUseCase: GetPDIsColaboradorUseCase
  } | null>(null)

  if (!useCasesRef.current) {
    useCasesRef.current = {
      getContextUseCase: container.resolve(GetMinhaJornadaContextUseCase),
      updateInterestUseCase: container.resolve(UpdateSkillInterestUseCase),
      updateLevelUseCase: container.resolve(UpdateSkillLevelUseCase),
      createPdiUseCase: container.resolve(CreatePdiUseCase),
      addSkillUseCase: container.resolve(AddSkillToPerfil360UseCase),
      suggestSkillUseCase: container.resolve(SuggestNewSkillUseCase),
      getPdisUseCase: container.resolve(GetPDIsColaboradorUseCase),
    }
  }

  const { getContextUseCase, updateInterestUseCase, updateLevelUseCase, createPdiUseCase, addSkillUseCase, suggestSkillUseCase, getPdisUseCase } = useCasesRef.current

  // Estado principal
  const [perfil360, setPerfil360] = useState<MinhaJornadaSkill[]>([])
  const [perfilAtuacao, setPerfilAtuacao] = useState<MinhaJornadaProfile | null>(null)
  const [perfisDisponiveis, setPerfisDisponiveis] = useState<MinhaJornadaProfile[]>([])
  const [aderencia, setAderencia] = useState<MinhaJornadaAdherence | null>(null)
  const [gaps, setGaps] = useState<MinhaJornadaSkill[]>([])
  const [pdis, setPdis] = useState<Array<{
    id: number
    skills: Array<{
      id: number
      skill_name: string
      pdi_id: number
    }>
  }>>([])
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // Obter dados do usuário
  const codColaborador = user?.cpf || ''
  const orgId = user?.orgId || user?.colaboradorOrg?.orgId || 0

  // Ref para rastrear parâmetros anteriores e evitar chamadas duplicadas
  const lastLoadParamsRef = useRef<{
    token: string | null
    codColaborador: string
    orgId: number
  } | null>(null)

  // Carregar contexto inicial
  const loadContext = useCallback(async (isSilent = false) => {
    // Verificar se token está disponível (do state ou localStorage)
    const currentToken = token || localStorage.getItem('authToken')
    
    // orgId pode ser 0 (default), então validamos apenas null/undefined
    if (!currentToken || !codColaborador || orgId === undefined || orgId === null) {
      if (!isSilent) setIsLoading(false)
      return
    }

    // Verificar se os parâmetros mudaram para evitar chamadas duplicadas
    const currentParams = { token: currentToken, codColaborador, orgId }
    const lastParams = lastLoadParamsRef.current
    
    if (
      !isSilent &&
      lastParams &&
      lastParams.token === currentParams.token &&
      lastParams.codColaborador === currentParams.codColaborador &&
      lastParams.orgId === currentParams.orgId
    ) {
      // Parâmetros não mudaram, não fazer nova chamada
      return
    }

    // Atualizar referência dos parâmetros
    lastLoadParamsRef.current = currentParams

    try {
      if (!isSilent) setIsLoading(true)
      setError(null)

      const context = await getContextUseCase!.execute({
        codColaborador,
        orgId,
        token: currentToken,
      })

      setPerfil360(context.perfil360)
      setPerfisDisponiveis(context.perfisDisponiveis)
      setAderencia(context.aderencia)
      setGaps(context.gaps)

      // Preservar o perfil selecionado se ele ainda existir na nova lista
      setPerfilAtuacao((current) => {
        if (!current?.perfilId) return context.perfilAtuacao
        const stillExists = context.perfisDisponiveis.find(
          (p) => p.perfilId === current.perfilId,
        )
        return stillExists || context.perfilAtuacao
      })

      // Carregar PDIs do usuário
      try {
        // UUID do colaborador vem do cpf ou codigoColaboradorInterno
        const uuidColab = user?.cpf || user?.colaboradorOrg?.codColaborador || codColaborador
        if (uuidColab) {
          const result = await getPdisUseCase!.execute({
            token: currentToken,
            uuid_colab: uuidColab,
          })
          setPdis(result.pdis)
        }
      } catch (err) {
        // Não bloquear o carregamento se houver erro ao buscar PDIs
        console.error('Erro ao carregar PDIs:', err)
      }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        if (!isSilent) setIsLoading(false)
      }
  }, [token, codColaborador, orgId, user])

  useEffect(() => {
    // Só carregar contexto se houver token e dados do usuário
    const currentToken = token || localStorage.getItem('authToken')
    if (currentToken && user) {
      loadContext()
    }
  }, [loadContext, token, user])

  // Criar Set com nomes de skills presentes nos PDIs (normalizados)
  const skillNamesInPdis = useMemo(() => {
    const skillNameSet = new Set<string>()
    pdis.forEach((pdi) => {
      pdi.skills.forEach((skill) => {
        // Normalizar nome da skill para comparação
        const normalizedName = (skill.skill_name || '').trim().toLowerCase()
        if (normalizedName) {
          skillNameSet.add(normalizedName)
        }
      })
    })
    return skillNameSet
  }, [pdis])

  // Agrupar todas as skills do perfil por tipo (não apenas gaps)
  const skillGroups = useMemo<SkillGroup[]>(() => {
    const typeMap: Record<MinhaJornadaSkillType, MinhaJornadaSkill[]> = {
      hard: [],
      soft: [],
      methodology: [],
      domain: [],
      language: [],
    }

    // Função auxiliar para normalizar nome (mesma lógica do calcularGaps)
    const normalizeName = (name?: string) => (name || '').trim().toLowerCase()

    // Criar mapa de skills do perfil 360 por nome normalizado
    const perfil360ByName = new Map<string, MinhaJornadaSkill>()
    perfil360.forEach((skill) => {
      const key = normalizeName(skill.habilidade)
      if (key) {
        const existing = perfil360ByName.get(key)
        if (!existing) {
          perfil360ByName.set(key, skill)
        } else {
          // Manter a skill com maior tier
          const tierExisting = getSeniorityTier(existing.senioridade) ?? 0
          const tierNew = getSeniorityTier(skill.senioridade) ?? 0
          if (tierNew > tierExisting) {
            perfil360ByName.set(key, skill)
          }
        }
      }
    })

    // Processar todas as skills do perfil de atuação (não apenas gaps)
    if (perfilAtuacao) {
      perfilAtuacao.skills.forEach((skill) => {
        // Mapear perfilTipoId para tipo baseado na resposta da API
        // 1 = COMPETENCIA (hard), 3 = METODOLOGIA, 8 = SOFTSKILL, 9 = IDIOMA, 4 = DOMINIO
        let tipo: MinhaJornadaSkillType = 'hard'
        if (skill.perfilTipoId === 1) tipo = 'hard'
        else if (skill.perfilTipoId === 3) tipo = 'methodology'
        else if (skill.perfilTipoId === 4) tipo = 'domain'
        else if (skill.perfilTipoId === 8) tipo = 'soft'
        else if (skill.perfilTipoId === 9) tipo = 'language'

        // Buscar skill correspondente no perfil 360
        const key = normalizeName(skill.habilidade)
        const skillNo360 = key ? perfil360ByName.get(key) : null

        // Calcular tiers
        const tierExigida = getSeniorityTier(skill.senioridade)
        const tierAtual = skillNo360 ? getSeniorityTier(skillNo360.senioridade) : null

        // Verificar se a skill está presente em algum PDI (comparando por nome normalizado)
        const skillNameNormalized = normalizeName(skill.habilidade)
        const isInPdi = skillNameNormalized ? skillNamesInPdis.has(skillNameNormalized) : false

        // Criar skill com tiers calculados e informação de PDI
        const skillCompleta: MinhaJornadaSkill & { isInPdi?: boolean } = {
          ...skill,
          senioridadeTierExigida: tierExigida,
          senioridadeTierAtual: tierAtual,
          isInPdi,
        }

        typeMap[tipo].push(skillCompleta)
      })
    }

    // Calcular totais e metas atingidas por tipo
    const totalByType: Record<MinhaJornadaSkillType, number> = {
      hard: 0,
      soft: 0,
      methodology: 0,
      domain: 0,
      language: 0,
    }

    if (perfilAtuacao) {
      perfilAtuacao.skills.forEach((skill) => {
        let tipo: MinhaJornadaSkillType = 'hard'
        if (skill.perfilTipoId === 1) tipo = 'hard'
        else if (skill.perfilTipoId === 3) tipo = 'methodology'
        else if (skill.perfilTipoId === 4) tipo = 'domain'
        else if (skill.perfilTipoId === 8) tipo = 'soft'
        else if (skill.perfilTipoId === 9) tipo = 'language'
        totalByType[tipo]++
      })
    }

    return Object.entries(typeMap).map(([type, skills]) => {
      const tipo = type as MinhaJornadaSkillType
      const total = totalByType[tipo]
      // Contar skills completadas (tier atual >= tier exigida)
      const met = skills.filter(
        (s) =>
          s.senioridadeTierAtual !== null &&
          s.senioridadeTierAtual !== undefined &&
          s.senioridadeTierExigida !== null &&
          s.senioridadeTierExigida !== undefined &&
          s.senioridadeTierAtual >= s.senioridadeTierExigida
      ).length

      return {
        type: tipo,
        skills,
        metCount: met,
        totalCount: total,
      }
    })
  }, [perfilAtuacao, perfil360, skillNamesInPdis])

  // Handlers
  const handleMarkAlreadyHave = useCallback(
    async (skill: MinhaJornadaSkill, nivelId: number) => {
      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !user?.cpf) return

      try {
        setIsSaving(true)
        setError(null)

        // Determinar tipo da skill baseado na resposta da API
        // 1 = COMPETENCIA (hard), 3 = METODOLOGIA, 8 = SOFTSKILL, 9 = IDIOMA, 4 = DOMINIO
        let tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma' = 'hard'
        if (skill.perfilTipoId === 1) tipo = 'hard'
        else if (skill.perfilTipoId === 3) tipo = 'metodologia'
        else if (skill.perfilTipoId === 4) tipo = 'dominio'
        else if (skill.perfilTipoId === 8) tipo = 'soft'
        else if (skill.perfilTipoId === 9) tipo = 'idioma'

        // Verificar se skill já existe no perfil 360 (buscarSkillsColaborador)
        // Encontrar a skill correspondente no perfil360 pelo skillId
        const skillNo360 = perfil360.find((s) => s.skillId === skill.skillId)

        if (skillNo360) {
          // Skill existe no perfil 360 - usar AtualizarSkill
          // Para AtualizarSkill, o 'id' é o ID da competência vinculada ao colaborador
          // Usar skillId do registro no perfil360
          await updateLevelUseCase!.execute({
            id: skillNo360.skillId, // Usar skillId do registro no perfil360
            nivelId,
            cpf: user.cpf,
            tipo,
            gestorExternoPerfil: perfilAtuacao?.perfilId || '',
            minhaJornada: true,
            token: currentToken,
          })
        } else {
          // Skill não existe no perfil 360 - usar inserirSkill
          // Para AdicionarSkill, o 'id' é o skillId da skill a ser adicionada (do gap)
          await addSkillUseCase!.execute({
            tipo,
            items: [
              {
                id: skill.skillId, // skillId do gap (perfil de atuação)
                descricao: skill.habilidade,
                nivelId,
                cpf: user.cpf, // CPF do colaborador (showMe)
                gestorExternoPerfil: perfilAtuacao?.perfilId || '',
                minhaJornada: true,
              },
            ],
            token: currentToken,
          })
        }

        // Recarregar contexto para atualizar gaps
        // Resetar referência para forçar reload mesmo com mesmos parâmetros
        lastLoadParamsRef.current = null
        await loadContext(true)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao atualizar skill')
        throw err
      } finally {
        setIsSaving(false)
      }
    },
    [token, user, perfil360, loadContext, perfilAtuacao],
  )

  const handleMarkNoInterest = useCallback(
    async (skill: MinhaJornadaSkill) => {
      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !user?.cpf) return

      try {
        setIsSaving(true)
        setError(null)

        const interesse: MinhaJornadaInteresse = {
          cpf: user.cpf,
          skillId: skill.skillId,
          tipoId: skill.perfilTipoId,
          interesseAtivo: false,
          gestorExternoPerfil: perfilAtuacao?.perfilId || '',
          minhaJornada: true,
        }

        await updateInterestUseCase!.execute({
          interesses: [interesse],
          token: currentToken,
        })

        // Recarregar contexto
        // Resetar referência para forçar reload mesmo com mesmos parâmetros
        lastLoadParamsRef.current = null
        await loadContext(true)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao atualizar interesse')
        throw err
      } finally {
        setIsSaving(false)
      }
    },
    [token, user, loadContext, perfilAtuacao],
  )

  const handleRevertInterest = useCallback(
    async (skill: MinhaJornadaSkill) => {
      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !user?.cpf) return

      try {
        setIsSaving(true)
        setError(null)

        const interesse: MinhaJornadaInteresse = {
          cpf: user.cpf,
          skillId: skill.skillId,
          tipoId: skill.perfilTipoId,
          interesseAtivo: true,
          gestorExternoPerfil: perfilAtuacao?.perfilId || '',
          minhaJornada: true,
        }

        await updateInterestUseCase!.execute({
          interesses: [interesse],
          token: currentToken,
        })

        // Recarregar contexto
        // Resetar referência para forçar reload mesmo com mesmos parâmetros
        lastLoadParamsRef.current = null
        await loadContext(true)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao reverter interesse')
        throw err
      } finally {
        setIsSaving(false)
      }
    },
    [token, user, loadContext, perfilAtuacao],
  )

  const handleMarkWantToDevelop = useCallback(
    async (_skills: MinhaJornadaSkill[], payload: CriarMinhaJornadaPdiPayload) => {
      const currentToken = token || localStorage.getItem('authToken')
      console.debug('DEBUG handleMarkWantToDevelop called', { hasToken: !!currentToken, hasUser: !!user })
      if (!currentToken || !user) return

      try {
        setIsSaving(true)
        setError(null)

        await createPdiUseCase!.execute(payload)

        // Recarregar contexto
        // Resetar referência para forçar reload mesmo com mesmos parâmetros
        lastLoadParamsRef.current = null
        await loadContext(true)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao criar PDI')
        throw err
      } finally {
        setIsSaving(false)
      }
    },
    [token, user, loadContext, perfilAtuacao],
  )

  const handleSelectProfile = useCallback(
    async (perfilId: string) => {
      const selected = perfisDisponiveis.find((p) => p.perfilId === perfilId)
      if (!selected || !selected.perfilId) return

      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !codColaborador) return

      try {
        setIsSaving(true)
        setPerfilAtuacao(selected)

        // Verificar se há habilidades no perfil antes de calcular aderência
        const hasHabilidades = selected.skills && selected.skills.length > 0

        // Calcular gaps localmente apenas se houver habilidades
        const newGaps = hasHabilidades
          ? calcularGaps({
              requisitos: selected.skills,
              possuidas: perfil360,
            })
          : []
        setGaps(newGaps)

        // Buscar nova aderência apenas se houver habilidades
        // Caso contrário, o endpoint retornará erro
        if (hasHabilidades) {
          const repository = container.resolve<MinhaJornadaRepository>(DiTokens.minhaJornadaRepository)
          const novaAderencia = await repository.getAdherence(
            codColaborador,
            selected.perfilId,
            currentToken
          ).catch(() => null)

          if (novaAderencia) {
            setAderencia(novaAderencia)
          }
        } else {
          // Se não houver habilidades, definir aderência como null
          setAderencia(null)
        }
      } catch (err) {
        console.error('Erro ao trocar perfil:', err)
      } finally {
        setIsSaving(false)
      }
    },
    [perfisDisponiveis, codColaborador, token, perfil360]
  )

  const handleSuggestSkill = useCallback(
    async (sugestao: MinhaJornadaSugestao) => {
      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken) return

      try {
        setIsSaving(true)
        setError(null)

        // Garantir que a sugestão use os dados do perfil atualmente selecionado
        const payload: MinhaJornadaSugestao = {
          ...sugestao,
          perfilId: perfilAtuacao?.perfilId || sugestao.perfilId,
          codigoGestorAdm: perfilAtuacao?.codigoInternoGestorAdm || sugestao.codigoGestorAdm,
          codigoCliente: perfilAtuacao?.codigoCliente || sugestao.codigoCliente,
          gestorExternoPerfil: perfilAtuacao?.perfilId || sugestao.gestorExternoPerfil || '',
          minhaJornada: true,
        }

        await suggestSkillUseCase!.execute({
          sugestao: payload,
          token: currentToken,
        })

        // Recarregar contexto
        // Resetar referência para forçar reload mesmo com mesmos parâmetros
        lastLoadParamsRef.current = null
        await loadContext(true)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao sugerir skill')
        throw err
      } finally {
        setIsSaving(false)
      }
    },
    [token, loadContext, perfilAtuacao],
  )

  const handleGravarLogPdiScreenMovement = useCallback(
    async (skills: MinhaJornadaSkill[]) => {
      const currentToken = token || localStorage.getItem('authToken')
      if (!currentToken || !user?.cpf || !perfilAtuacao?.perfilId) return

      try {
        const repository = container.resolve<MinhaJornadaRepository>(
          DiTokens.minhaJornadaRepository,
        )

        // Disparar log de movimentação (assíncrono e não bloqueante)
        void repository.gravarLogPdiScreenMovement({
          token: currentToken,
          codigoInternoColaborador: user.cpf,
          gestorExternoPerfil: perfilAtuacao.perfilId,
          skills,
        })
      } catch (error) {
        console.error('Erro ao disparar log de PDI:', error)
      }
    },
    [token, user, perfilAtuacao],
  )

  // Verificar estados vazios para empty states
  // hasNoClientes: quando não há perfis disponíveis (lista de clientes vazia do endpoint)
  const hasNoClientes = perfisDisponiveis.length === 0
  
  // hasNoHabilidades: quando há perfil mas não há skills (lista de habilidades vazia do endpoint)
  const hasNoHabilidades = perfilAtuacao !== null && 
    (!perfilAtuacao.skills || perfilAtuacao.skills.length === 0)

  return {
    // Dados
    perfil360,
    perfilAtuacao,
    perfisDisponiveis,
    aderencia,
    gaps,
    skillGroups,

    // Estados
    isLoading,
    isSaving,
    error,
    hasNoClientes,
    hasNoHabilidades,

    // Handlers
    handleMarkAlreadyHave,
    handleMarkNoInterest,
    handleRevertInterest,
    handleMarkWantToDevelop,
    handleSuggestSkill,
    handleSelectProfile,
    handleGravarLogPdiScreenMovement,
    refreshContext: loadContext,
  }
}

