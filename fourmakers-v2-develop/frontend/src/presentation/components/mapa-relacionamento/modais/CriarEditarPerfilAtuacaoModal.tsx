import { useState, useEffect, useRef } from 'react'
import { useAppSelector } from '@app/store/hooks'
import type { PerfilCorporativoPayload, PerfilCorporativoResponse } from '@domain/entities/Organograma'
import type { PerfilExterno } from '@domain/entities/MapaRelacionamento'
import { toast } from 'sonner'
import { container } from '@core/di/container'
import { BuscarPerfilCorporativoPorIdUseCase } from '@domain/usecases/BuscarPerfilCorporativoPorIdUseCase'
import { ListarPermanenciasUseCase } from '@domain/usecases/ListarPermanenciasUseCase'
import { ListarModelosTrabalhoUseCase } from '@domain/usecases/ListarModelosTrabalhoUseCase'
import { ListarLocalidadesUseCase } from '@domain/usecases/ListarLocalidadesUseCase'
import { ListarTiposEmpregoUseCase } from '@domain/usecases/ListarTiposEmpregoUseCase'
import { ListarNiveisExperienciaUseCase } from '@domain/usecases/ListarNiveisExperienciaUseCase'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { AutocompleteSimplesMapa } from '@presentation/components/mapa-relacionamento/seletores/AutocompleteSimplesMapa'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Label } from '@/components/ui/label'
import { Spinner } from '@/components/ui/spinner'
import { Briefcase, Clock, MapPin } from 'lucide-react'
import { logError } from '@shared/utils/firebaseCrashlytics'

interface CriarEditarPerfilAtuacaoModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  perfil?: PerfilCorporativoResponse | null
  // Opcional: lista de perfis externos para seleção (modo de edição do mapa de relacionamento)
  perfisExternos?: PerfilExterno[]
  perfilExternoSelecionado?: PerfilExterno | null
  // Callback alternativo para quando usar perfis externos
  onSaveWithPerfilId?: (payload: {
    perfilId: string
    descricao: string
    atribuicoes: string
    permanenciaId: string | null
    modeloTrabalhoId: string | null
    profissionalLocalidadeId: string | null
    experienciaLinkedinId: string | null
    empregoLinkdinId: string | null
  }) => Promise<void>
  onSave?: (payload: PerfilCorporativoPayload & { id?: string }) => void
}

interface PermanenciaOption {
  id: string
  descricao: string
}

interface ModeloTrabalhoOption {
  id: string
  descricao: string
  codigo: number
}

interface LocalidadeOption {
  id: string
  descricao: string
}

interface TipoEmpregoOption {
  id: string
  descricao: string
}

interface NivelExperienciaOption {
  id: string
  descricao: string
}

export function CriarEditarPerfilAtuacaoModal({
  open,
  onOpenChange,
  perfil,
  perfisExternos,
  perfilExternoSelecionado,
  onSaveWithPerfilId,
  onSave,
}: CriarEditarPerfilAtuacaoModalProps) {
  const { token, user } = useAppSelector((state) => state.auth)
  const [perfilId, setPerfilId] = useState<string>('')
  const [descricao, setDescricao] = useState('')
  const [atribuicoes, setAtribuicoes] = useState('')
  const [permanenciaId, setPermanenciaId] = useState<string>('')
  const [permanencias, setPermanencias] = useState<PermanenciaOption[]>([])
  const [loadingPermanencias, setLoadingPermanencias] = useState(false)
  const [modeloTrabalhoId, setModeloTrabalhoId] = useState<string>('')
  const [modelosTrabalho, setModelosTrabalho] = useState<ModeloTrabalhoOption[]>([])
  const [loadingModelosTrabalho, setLoadingModelosTrabalho] = useState(false)
  const [profissionalLocalidadeId, setProfissionalLocalidadeId] = useState<string>('')
  const [localidades, setLocalidades] = useState<LocalidadeOption[]>([])
  const [loadingLocalidades, setLoadingLocalidades] = useState(false)
  const [experienciaLinkedinId, setExperienciaLinkedinId] = useState<string>('')
  const [tiposEmprego, setTiposEmprego] = useState<TipoEmpregoOption[]>([])
  const [loadingTiposEmprego, setLoadingTiposEmprego] = useState(false)
  const [empregoLinkdinId, setEmpregoLinkdinId] = useState<string>('')
  const [niveisExperiencia, setNiveisExperiencia] = useState<NivelExperienciaOption[]>([])
  const [loadingNiveisExperiencia, setLoadingNiveisExperiencia] = useState(false)
  const [errors, setErrors] = useState<{ perfilId?: string; descricao?: string; permanenciaId?: string; modeloTrabalhoId?: string; profissionalLocalidadeId?: string; experienciaLinkedinId?: string; empregoLinkdinId?: string }>({})
  
  // Determinar se está no modo de seleção de perfis externos
  const isModoSelecaoPerfis = !!perfisExternos && !!onSaveWithPerfilId
  const [isSaving, setIsSaving] = useState(false)
  const [isLoadingPerfilData, setIsLoadingPerfilData] = useState(false)
  
  // Estado para armazenar os dados do perfil carregado (para garantir que sejam aplicados após opções carregarem)
  const [perfilDataLoaded, setPerfilDataLoaded] = useState<{
    id: string
    descricao: string
    atribuicoes: string
    permanenciaId: string
    modeloTrabalhoId: string
    profissionalLocalidadeId: string
    experienciaLinkedinId: string
    empregoLinkdinId: string
  } | null>(null)
  const perfilDataTimeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null)

  // Load permanencias, modelos trabalho, localidades, tipos emprego and niveis experiencia when modal opens
  useEffect(() => {
    if (!open || !token) return

    const loadOptions = async () => {
      setLoadingPermanencias(true)
      setLoadingModelosTrabalho(true)
      setLoadingLocalidades(true)
      setLoadingTiposEmprego(true)
      setLoadingNiveisExperiencia(true)
      try {
        const listarPermanenciasUseCase = container.resolve(ListarPermanenciasUseCase)
        const listarModelosTrabalhoUseCase = container.resolve(ListarModelosTrabalhoUseCase)
        const listarLocalidadesUseCase = container.resolve(ListarLocalidadesUseCase)
        const listarTiposEmpregoUseCase = container.resolve(ListarTiposEmpregoUseCase)
        const listarNiveisExperienciaUseCase = container.resolve(ListarNiveisExperienciaUseCase)

        const [permanenciasData, modelosTrabalhoData, localidadesData, tiposEmpregoData, niveisExperienciaData] = await Promise.all([
          listarPermanenciasUseCase.execute(token),
          listarModelosTrabalhoUseCase.execute(token),
          listarLocalidadesUseCase.execute(token),
          listarTiposEmpregoUseCase.execute(token),
          listarNiveisExperienciaUseCase.execute(token),
        ])
        setPermanencias(permanenciasData)
        setModelosTrabalho(modelosTrabalhoData)
        setLocalidades(localidadesData)
        setTiposEmprego(tiposEmpregoData)
        setNiveisExperiencia(niveisExperienciaData)
      } catch (error) {
        logError(error, { component: 'CriarEditarPerfilAtuacaoModal', action: 'loadOptions' }, user ?? undefined)
        toast.error('Erro ao carregar opções')
      } finally {
        setLoadingPermanencias(false)
        setLoadingModelosTrabalho(false)
        setLoadingLocalidades(false)
        setLoadingTiposEmprego(false)
        setLoadingNiveisExperiencia(false)
      }
    }

    loadOptions()
  }, [open, token])

  // Aplicar dados do perfil quando as opções carregarem; fallback com timeout de 2s se não carregarem
  useEffect(() => {
    const allOptionsFinishedLoading = !loadingPermanencias && !loadingModelosTrabalho && !loadingLocalidades && !loadingTiposEmprego && !loadingNiveisExperiencia
    const allOptionsHaveData = permanencias.length > 0 && modelosTrabalho.length > 0 && localidades.length > 0 && tiposEmprego.length > 0 && niveisExperiencia.length > 0

    const applyPerfilDropdowns = (data: NonNullable<typeof perfilDataLoaded>) => {
      setPermanenciaId(data.permanenciaId ? String(data.permanenciaId) : '')
      setModeloTrabalhoId(data.modeloTrabalhoId ? String(data.modeloTrabalhoId) : '')
      setProfissionalLocalidadeId(data.profissionalLocalidadeId ? String(data.profissionalLocalidadeId) : '')
      setExperienciaLinkedinId(data.experienciaLinkedinId ? String(data.experienciaLinkedinId) : '')
      setEmpregoLinkdinId(data.empregoLinkdinId ? String(data.empregoLinkdinId) : '')
      setPerfilDataLoaded(null)
      setIsLoadingPerfilData(false)
    }

    if (!perfilDataLoaded) {
      if (perfilDataTimeoutRef.current) {
        clearTimeout(perfilDataTimeoutRef.current)
        perfilDataTimeoutRef.current = null
      }
      return
    }

    if (allOptionsFinishedLoading && allOptionsHaveData) {
      if (perfilDataTimeoutRef.current) {
        clearTimeout(perfilDataTimeoutRef.current)
        perfilDataTimeoutRef.current = null
      }
      applyPerfilDropdowns(perfilDataLoaded)
      return
    }

    if (perfilDataTimeoutRef.current) {
      clearTimeout(perfilDataTimeoutRef.current)
    }
    const dataToApply = perfilDataLoaded
    perfilDataTimeoutRef.current = setTimeout(() => {
      applyPerfilDropdowns(dataToApply)
      perfilDataTimeoutRef.current = null
    }, 2000)

    return () => {
      if (perfilDataTimeoutRef.current) {
        clearTimeout(perfilDataTimeoutRef.current)
        perfilDataTimeoutRef.current = null
      }
    }
  }, [perfilDataLoaded, loadingPermanencias, loadingModelosTrabalho, loadingLocalidades, loadingTiposEmprego, loadingNiveisExperiencia, permanencias.length, modelosTrabalho.length, localidades.length, tiposEmprego.length, niveisExperiencia.length])

  useEffect(() => {
    if (open) {
      // Reset perfilDataLoaded when modal opens
      setPerfilDataLoaded(null)
      
      if (isModoSelecaoPerfis) {
        // Modo de seleção de perfis externos - buscar dados completos do perfil
        if (perfilExternoSelecionado && token && user?.colaboradorOrg?.orgId) {
          setIsLoadingPerfilData(true)
          const loadPerfilCompleto = async () => {
            try {
              // Buscar dados completos do perfil usando o endpoint
              // O endpoint retorna o perfil completo com todos os campos
              const buscarPerfilUseCase = container.resolve(BuscarPerfilCorporativoPorIdUseCase);
              const response = await buscarPerfilUseCase.execute(token, perfilExternoSelecionado.gestorExternoPerfilId);

              // O use case retorna { sucesso, retorno } onde retorno é o perfil completo
              // Mas verificar se a resposta tem a estrutura esperada
              let perfilCompleto = null;
              
              if (response.sucesso && response.retorno) {
                perfilCompleto = response.retorno;
              } else if (response && typeof response === 'object' && 'id' in response && 'descricao' in response) {
                perfilCompleto = response as unknown as PerfilCorporativoResponse;
              } else {
                logError(new Error('Estrutura de resposta inesperada'), { component: 'CriarEditarPerfilAtuacaoModal', action: 'loadPerfilCompleto', extra: { response } }, user ?? undefined)
              }

              if (perfilCompleto) {
                // Pre-popular campos de texto imediatamente (não dependem de opções)
                setPerfilId(perfilCompleto.id || perfilExternoSelecionado.gestorExternoPerfilId)
                setDescricao(perfilCompleto.descricao ?? '')
                setAtribuicoes(perfilCompleto.atribuicoes ?? '')
                
                // Armazenar os dados para aplicar após as opções carregarem
                // Isso garante que os Selects reconheçam os valores corretamente
                const perfilDataToStore = {
                  id: perfilCompleto.id || perfilExternoSelecionado.gestorExternoPerfilId,
                  descricao: perfilCompleto.descricao ?? '',
                  atribuicoes: perfilCompleto.atribuicoes ?? '',
                  permanenciaId: perfilCompleto.permanenciaId ?? '',
                  modeloTrabalhoId: perfilCompleto.modeloTrabalhoId ?? '',
                  profissionalLocalidadeId: perfilCompleto.profissionalLocalidadeId ?? '',
                  experienciaLinkedinId: perfilCompleto.experienciaLinkedinId ?? '',
                  empregoLinkdinId: perfilCompleto.empregoLinkdinId ?? '',
                };
                setPerfilDataLoaded(perfilDataToStore);
              } else {
                // Fallback: usar dados básicos
                setIsLoadingPerfilData(false)
                setPerfilId(perfilExternoSelecionado.gestorExternoPerfilId)
                setDescricao(perfilExternoSelecionado.gestorExternoPerfilNome)
                setAtribuicoes('')
                setPermanenciaId('')
                setModeloTrabalhoId('')
                setProfissionalLocalidadeId('')
                setExperienciaLinkedinId('')
                setEmpregoLinkdinId('')
              }
            } catch (error) {
              logError(error, { component: 'CriarEditarPerfilAtuacaoModal', action: 'loadPerfilCompleto' }, user ?? undefined)
              setIsLoadingPerfilData(false)
              // Fallback: usar dados básicos
              setPerfilId(perfilExternoSelecionado.gestorExternoPerfilId)
              setDescricao(perfilExternoSelecionado.gestorExternoPerfilNome)
              setAtribuicoes('')
              setPermanenciaId('')
              setModeloTrabalhoId('')
              setProfissionalLocalidadeId('')
              setExperienciaLinkedinId('')
              setEmpregoLinkdinId('')
            }
          };
          
          loadPerfilCompleto();
        } else if (perfilExternoSelecionado) {
          // Fallback: usar dados básicos se não tiver token ou orgId
          setPerfilId(perfilExternoSelecionado.gestorExternoPerfilId)
          setDescricao(perfilExternoSelecionado.gestorExternoPerfilNome)
          setAtribuicoes('')
          // Não limpar dropdowns aqui - podem ter valores válidos
        } else {
          // Só limpar tudo se não houver perfil selecionado
          setPerfilId('')
          setDescricao('')
          setAtribuicoes('')
          setPermanenciaId('')
          setModeloTrabalhoId('')
          setProfissionalLocalidadeId('')
          setExperienciaLinkedinId('')
          setEmpregoLinkdinId('')
        }
      } else {
        // Modo normal (criar/editar com PerfilCorporativoResponse)
        if (perfil) {
          setPerfilId(perfil.id)
          setDescricao(perfil.descricao ?? '')
          setAtribuicoes(perfil.atribuicoes ?? '')
          setPermanenciaId(perfil.permanenciaId ?? '')
          setModeloTrabalhoId(perfil.modeloTrabalhoId ?? '')
          setProfissionalLocalidadeId(perfil.profissionalLocalidadeId ?? '')
          setExperienciaLinkedinId(perfil.experienciaLinkedinId ?? '')
          setEmpregoLinkdinId(perfil.empregoLinkdinId ?? '')
        } else {
          setPerfilId('')
          setDescricao('')
          setAtribuicoes('')
          setPermanenciaId('')
          setModeloTrabalhoId('')
          setProfissionalLocalidadeId('')
          setExperienciaLinkedinId('')
          setEmpregoLinkdinId('')
        }
      }
      setErrors({})
      setIsSaving(false)
    } else {
      // Limpar dados quando modal fecha
      setPerfilDataLoaded(null)
    }
  }, [open, perfil, perfisExternos, perfilExternoSelecionado, isModoSelecaoPerfis, token, user])

  // Removido: useEffect que atualizava descrição quando perfil externo era selecionado
  // Agora no modo de edição, o perfil já vem selecionado e os dados são carregados diretamente no useEffect anterior

  const validate = (): boolean => {
    const newErrors: { perfilId?: string; descricao?: string; permanenciaId?: string; modeloTrabalhoId?: string; profissionalLocalidadeId?: string; experienciaLinkedinId?: string; empregoLinkdinId?: string } = {}
    
    // Removido: validação de perfilId no modo de seleção, pois não há mais dropdown
    // O perfil já vem selecionado quando está editando
    
    if (!descricao.trim()) {
      newErrors.descricao = 'O título do perfil é obrigatório.'
    } else if (descricao.trim().length < 2) {
      newErrors.descricao = 'O título do perfil deve ter pelo menos 2 caracteres.'
    } else if (descricao.trim().length > 200) {
      newErrors.descricao = 'O título do perfil não pode exceder 200 caracteres.'
    }

    if (!permanenciaId || !permanenciaId.trim()) {
      newErrors.permanenciaId = 'O período mínimo de experiência é obrigatório.'
    }

    if (!modeloTrabalhoId || !modeloTrabalhoId.trim()) {
      newErrors.modeloTrabalhoId = 'O modelo de trabalho é obrigatório.'
    }

    if (!profissionalLocalidadeId || !profissionalLocalidadeId.trim()) {
      newErrors.profissionalLocalidadeId = 'A seleção de profissionais de outros estados é obrigatória.'
    }

    if (!experienciaLinkedinId || !experienciaLinkedinId.trim()) {
      newErrors.experienciaLinkedinId = 'O nível de experiência é obrigatório.'
    }

    if (!empregoLinkdinId || !empregoLinkdinId.trim()) {
      newErrors.empregoLinkdinId = 'O tipo de emprego é obrigatório.'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSave = async () => {
    if (!validate()) {
      return
    }

    setIsSaving(true)

    try {
      if (isModoSelecaoPerfis && onSaveWithPerfilId) {
        // Modo de seleção de perfis externos
        await onSaveWithPerfilId({
          perfilId,
          descricao: descricao.trim(),
          atribuicoes: atribuicoes.trim(),
          permanenciaId: permanenciaId || null,
          modeloTrabalhoId: modeloTrabalhoId || null,
          profissionalLocalidadeId: profissionalLocalidadeId || null,
          experienciaLinkedinId: experienciaLinkedinId || null,
          empregoLinkdinId: empregoLinkdinId || null,
        })
      } else if (onSave) {
        // Modo normal (criar/editar com PerfilCorporativoResponse)
        const orgId = user?.colaboradorOrg?.orgId
        if (!orgId) {
          toast.error('Erro: orgId não encontrado no usuário')
          return
        }

        // Construir payload apenas com campos necessários
        const payload: PerfilCorporativoPayload = {
          orgId: orgId,
          descricao: descricao.trim(),
          ativo: perfil?.ativo ?? true,
        }
        
        // Adicionar permanenciaId (obrigatório)
        payload.permanenciaId = permanenciaId.trim()
        
        // Adicionar modeloTrabalhoId (obrigatório)
        payload.modeloTrabalhoId = modeloTrabalhoId.trim()
        
        // Adicionar profissionalLocalidadeId (obrigatório)
        payload.profissionalLocalidadeId = profissionalLocalidadeId.trim()
        
        // Adicionar experienciaLinkedinId (obrigatório)
        payload.experienciaLinkedinId = experienciaLinkedinId.trim()
        
        // Adicionar empregoLinkdinId (obrigatório)
        payload.empregoLinkdinId = empregoLinkdinId.trim()
        
        // Adicionar atribuicoes se preenchido
        if (atribuicoes.trim()) {
          payload.atribuicoes = atribuicoes.trim()
        }

        // Se for edição, incluir ID
        if (perfil?.id || perfilId) {
          (payload as PerfilCorporativoPayload & { id: string }).id = perfil?.id || perfilId
        }

        await onSave(payload as PerfilCorporativoPayload & { id?: string })
      } else {
        toast.error('Erro: Nenhum callback de salvamento fornecido')
        return
      }

      setPerfilId('')
      setDescricao('')
      setAtribuicoes('')
      setPermanenciaId('')
      setModeloTrabalhoId('')
      setProfissionalLocalidadeId('')
      setExperienciaLinkedinId('')
      setEmpregoLinkdinId('')
      setErrors({})
      onOpenChange(false)
    } catch (error) {
      logError(error, { component: 'CriarEditarPerfilAtuacaoModal', action: 'handleSave' }, user ?? undefined)
      toast.error('Erro ao salvar perfil de atuação. Tente novamente.')
    } finally {
      setIsSaving(false)
    }
  }

  const handleClose = (open: boolean) => {
    if (!open) {
      setPerfilId('')
      setDescricao('')
      setAtribuicoes('')
      setPermanenciaId('')
      setModeloTrabalhoId('')
      setProfissionalLocalidadeId('')
      setExperienciaLinkedinId('')
      setEmpregoLinkdinId('')
    }
    onOpenChange(open)
  }

  // Verificar se está carregando (no modo de seleção, esperar perfil e opções)
  const isLoading = isModoSelecaoPerfis && (isLoadingPerfilData || loadingPermanencias || loadingModelosTrabalho || loadingLocalidades || loadingTiposEmprego || loadingNiveisExperiencia || !!perfilDataLoaded)

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-visible">
        <DialogHeader>
          <DialogTitle>
            {isModoSelecaoPerfis 
              ? 'Editar Perfil Corporativo' 
              : perfil 
                ? 'Editar Perfil de Atuação' 
                : 'Criar Perfil de Atuação'}
          </DialogTitle>
          <DialogDescription>
            {isModoSelecaoPerfis
              ? 'Selecione um perfil e edite seu título e atribuições.'
              : perfil
                ? 'Edite as informações do perfil de atuação.'
                : 'Preencha os dados para criar um novo perfil de atuação.'}
          </DialogDescription>
        </DialogHeader>
        {isLoading ? (
          <div className="flex items-center justify-center py-12">
            <div className="flex flex-col items-center gap-4">
              <Spinner size={32} className="text-primary" />
              <p className="text-sm text-muted-foreground">Carregando dados do perfil...</p>
            </div>
          </div>
        ) : (
          <>
            <div className="space-y-6 py-4 max-h-[calc(90vh-180px)] overflow-y-auto">
              {/* Removido dropdown de seleção de perfil no modo de edição */}
              {/* Quando está editando, apenas mostra os dados do perfil selecionado */}

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Título <span className="text-destructive">*</span>
            </Label>
            <div className="relative">
              <div className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10 pointer-events-none">
                <Briefcase className="w-4 h-4" />
              </div>
              <Input
                className={`h-10 pl-12 rounded-lg transition-colors ${errors.descricao ? 'border-destructive border-2 focus-visible:border-destructive focus-visible:ring-destructive' : ''}`}
                placeholder="Digite o título do perfil..."
              value={descricao}
              onChange={(e) => {
                setDescricao(e.target.value)
                if (errors.descricao) {
                  setErrors({ ...errors, descricao: undefined })
                }
              }}
              disabled={false}
              autoFocus={!isModoSelecaoPerfis}
              />
            </div>
            {errors.descricao && (
              <p className="text-sm text-destructive mt-1 flex items-center gap-1">
                <span className="text-destructive">•</span>
                {errors.descricao}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Período mínimo de experiencia em cargos anteriores <span className="text-destructive">*</span>
            </Label>
            <AutocompleteSimplesMapa
              value={permanenciaId || ''}
              onValueChange={(value) => {
                setPermanenciaId(value)
                if (errors.permanenciaId) {
                  setErrors({ ...errors, permanenciaId: undefined })
                }
              }}
              options={permanencias}
              disabled={loadingPermanencias || (isModoSelecaoPerfis && !perfilId)}
              placeholder={loadingPermanencias ? 'Carregando...' : 'Selecione o período mínimo de experiência'}
              icon={<Clock className="w-4 h-4" />}
              error={!!errors.permanenciaId}
            />
            {errors.permanenciaId && (
              <p className="text-sm text-destructive mt-1 flex items-center gap-1">
                <span className="text-destructive">•</span>
                {errors.permanenciaId}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Modelo de Trabalho <span className="text-destructive">*</span>
            </Label>
            <AutocompleteSimplesMapa
              value={modeloTrabalhoId || ''}
              onValueChange={(value) => {
                setModeloTrabalhoId(value)
                if (errors.modeloTrabalhoId) {
                  setErrors({ ...errors, modeloTrabalhoId: undefined })
                }
              }}
              options={modelosTrabalho}
              disabled={loadingModelosTrabalho || (isModoSelecaoPerfis && !perfilId)}
              placeholder={loadingModelosTrabalho ? 'Carregando...' : 'Selecione um modelo de trabalho'}
              icon={<Briefcase className="w-4 h-4" />}
              error={!!errors.modeloTrabalhoId}
            />
            {errors.modeloTrabalhoId && (
              <p className="text-sm text-destructive mt-1 flex items-center gap-1">
                <span className="text-destructive">•</span>
                {errors.modeloTrabalhoId}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Profissionais de outros estados? <span className="text-destructive">*</span>
            </Label>
            <AutocompleteSimplesMapa
              value={profissionalLocalidadeId || ''}
              onValueChange={(value) => {
                setProfissionalLocalidadeId(value)
                if (errors.profissionalLocalidadeId) {
                  setErrors({ ...errors, profissionalLocalidadeId: undefined })
                }
              }}
              options={localidades}
              disabled={loadingLocalidades || (isModoSelecaoPerfis && !perfilId)}
              placeholder={loadingLocalidades ? 'Carregando...' : 'Selecione se aceita profissionais de outros estados'}
              icon={<MapPin className="w-4 h-4" />}
              error={!!errors.profissionalLocalidadeId}
            />
            {errors.profissionalLocalidadeId && (
              <p className="text-sm text-destructive mt-1 flex items-center gap-1">
                <span className="text-destructive">•</span>
                {errors.profissionalLocalidadeId}
              </p>
            )}
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-2">
              <Label className="text-sm font-bold text-foreground">
                Tipo de Emprego <span className="text-destructive">*</span>
              </Label>
              <AutocompleteSimplesMapa
                value={empregoLinkdinId || ''}
                onValueChange={(value) => {
                  setEmpregoLinkdinId(value)
                  if (errors.empregoLinkdinId) {
                    setErrors({ ...errors, empregoLinkdinId: undefined })
                  }
                }}
                options={tiposEmprego}
                disabled={loadingTiposEmprego || (isModoSelecaoPerfis && !perfilId)}
                placeholder={loadingTiposEmprego ? 'Carregando...' : 'Selecione o tipo de emprego'}
                error={!!errors.empregoLinkdinId}
              />
              {errors.empregoLinkdinId && (
                <p className="text-sm text-destructive mt-1 flex items-center gap-1">
                  <span className="text-destructive">•</span>
                  {errors.empregoLinkdinId}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <Label className="text-sm font-bold text-foreground">
                Nível de Experiência <span className="text-destructive">*</span>
              </Label>
              <AutocompleteSimplesMapa
                value={experienciaLinkedinId || ''}
                onValueChange={(value) => {
                  setExperienciaLinkedinId(value)
                  if (errors.experienciaLinkedinId) {
                    setErrors({ ...errors, experienciaLinkedinId: undefined })
                  }
                }}
                options={niveisExperiencia}
                disabled={loadingNiveisExperiencia || (isModoSelecaoPerfis && !perfilId)}
                placeholder={loadingNiveisExperiencia ? 'Carregando...' : 'Selecione o nível de experiência'}
                error={!!errors.experienciaLinkedinId}
              />
              {errors.experienciaLinkedinId && (
                <p className="text-sm text-destructive mt-1 flex items-center gap-1">
                  <span className="text-destructive">•</span>
                  {errors.experienciaLinkedinId}
                </p>
              )}
            </div>
          </div>

          <div className="space-y-2">
            <Label className="text-sm font-bold text-foreground">
              Atribuições
            </Label>
            <Textarea
              className="min-h-[120px] resize-none focus-visible:ring-0 focus-visible:ring-offset-0 focus-visible:border-primary"
              placeholder="Digite as atribuições do perfil..."
              value={atribuicoes}
              onChange={(e) => setAtribuicoes(e.target.value)}
              disabled={false}
            />
            <p className="text-xs text-muted-foreground">
              Descreva as responsabilidades e atribuições deste perfil
            </p>
          </div>
          </div>
            <DialogFooter>
              <Button variant="outline" onClick={() => handleClose(false)} disabled={isSaving || isLoading}>
                Cancelar
              </Button>
              <Button 
                onClick={handleSave} 
                disabled={
                  isLoading ||
                  isSaving || 
                  !descricao.trim() || 
                  !permanenciaId || 
                  !modeloTrabalhoId || 
                  !profissionalLocalidadeId ||
                  !experienciaLinkedinId ||
                  !empregoLinkdinId
                }
              >
                {isSaving ? 'Salvando...' : (isModoSelecaoPerfis || perfil) ? 'Salvar Alterações' : 'Criar Perfil'}
              </Button>
            </DialogFooter>
          </>
        )}
      </DialogContent>
    </Dialog>
  )
}
