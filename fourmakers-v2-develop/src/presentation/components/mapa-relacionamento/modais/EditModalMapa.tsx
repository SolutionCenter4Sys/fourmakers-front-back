import { useState, useEffect, useCallback, useRef, useMemo, memo } from 'react';
import { useLocation } from 'react-router-dom';
import { useAppSelector, useAppDispatch } from '@app/store/hooks';
import { container } from '@core/di/container';
import { CriarPerfilCorporativoUseCase } from '@domain/usecases/CriarPerfilCorporativoUseCase';
import { AtualizarPerfilCorporativoUseCase } from '@domain/usecases/AtualizarPerfilCorporativoUseCase';
import { AtualizarDepartamentoUseCase } from '@domain/usecases/AtualizarDepartamentoUseCase';
import { BuscarPerfisPorOrgUseCase } from '@domain/usecases/BuscarPerfisPorOrgUseCase';
import { BuscarPerfilCorporativoPorIdUseCase } from '@domain/usecases/BuscarPerfilCorporativoPorIdUseCase';
import type { DepartamentoPayload } from '@domain/entities/Organograma';
import type { 
  NoMapaRelacionamento, 
  PerfilExterno, 
  DepartamentoMapaRelacionamento 
} from '@domain/entities/MapaRelacionamento';
import type { PayloadCompleto } from '@shared/types/mapaRelacionamentoTypes';
import type { PerfilCorporativoPayload, PerfilCorporativoResponse } from '@domain/entities/Organograma';
import { Briefcase, Building, Plus, Info, Crown } from 'lucide-react';
import { toast } from 'sonner';
import { logError } from '@shared/utils/firebaseCrashlytics';
import { CriarEditarPerfilAtuacaoModal } from './CriarEditarPerfilAtuacaoModal';
import { EditarDepartamentoModal } from './EditarDepartamentoModal';
import { InserirGestorExternoModal } from './InserirGestorExternoModal';
import { SelecionarTipoPerfilModal, type TipoPerfil } from './SelecionarTipoPerfilModal';
import { AutocompleteProfissionalMapa } from '@presentation/components/mapa-relacionamento/seletores/AutocompleteProfissionalMapa';
import { AutocompleteGenericoMapa } from '@presentation/components/mapa-relacionamento/seletores/AutocompleteGenericoMapa';
import { listarDepartamentosMapaRelacionamento } from '@app/store/slices/mapaRelacionamentoSlice';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Switch } from '@/components/ui/switch';
import { Spinner } from '@/components/ui/spinner';

// Constante para o valor default de departamento quando não há departamento definido
const SEM_DEPARTAMENTO = 'Sem Departamento';


/** Dados do colaborador selecionado para preencher o payload (nome/email no card) */
export interface ColaboradorSelecionadoDisplay {
  nome: string;
  email: string;
}

// Estado de rastreamento de mudanças no modal (departamento, perfil, colaborador, etc.)
interface ModalState {
  departamento: {
    originalId?: string;
    novoNome?: string;
    foiCriado: boolean;
    foiEditado: boolean;
  };
  perfilAtuacao: {
    originalId?: string;
    novaDescricao?: string;
    foiCriado: boolean;
    foiEditado: boolean;
  };
  colaborador: {
    originalId?: string;
    novoId?: string;
    foiAlterado: boolean;
  };
  posicao: {
    originalId?: string;
    foiCriado: boolean;
    foiEditado: boolean;
  };
  alocacao: {
    originalId?: string;
    originalColaboradorId?: string;
    foiCriada: boolean;
    foiEditada: boolean;
  };
}

interface EditModalMapaProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  node: NoMapaRelacionamento;
  clientCode: string;
  perfis: PerfilExterno[];
  departamentos: DepartamentoMapaRelacionamento[];
  onSave: (payload: NoMapaRelacionamento) => void | Promise<void>;
}

// Tipo adaptado para o SearchableSelect usar PerfilCorporativoResponse
interface PerfilCorporativoAdaptado extends Record<string, unknown> {
  id: string;
  descricao: string;
  gestorExternoPerfilId: string;
  gestorExternoPerfilNome: string;
}

function EditModalMapaInner({ 
  open,
  onOpenChange,
  node, 
  clientCode, 
  perfis, 
  departamentos, 
  onSave 
}: EditModalMapaProps) {
  const { token, user } = useAppSelector((state) => state.auth);
  const dispatch = useAppDispatch();
  const location = useLocation();
  
  // Estados para perfis corporativos - DECLARAR ANTES de usar nas funções
  const [perfisCorporativos, setPerfisCorporativos] = useState<PerfilCorporativoAdaptado[]>([]);
  const [loadingPerfis, setLoadingPerfis] = useState(false);

  // Maps para lookups O(1) em vez de .find() O(n)
  const departamentosMap = useMemo(() => {
    const map = new Map<string, DepartamentoMapaRelacionamento>();
    departamentos.forEach(d => map.set(d.cod, d));
    return map;
  }, [departamentos]);

  const perfisCorporativosMap = useMemo(() => {
    const map = new Map<string, PerfilCorporativoAdaptado>();
    perfisCorporativos.forEach(p => map.set(p.gestorExternoPerfilId, p));
    return map;
  }, [perfisCorporativos]);

  // Valores iniciais só quando node/departamentos mudam (evita .find() em toda lista a cada render)
  const initialDepartmentId = useMemo(() => {
    if (node.departamentoBackendId && departamentosMap.size > 0) {
      const dept = departamentosMap.get(node.departamentoBackendId);
      if (dept) return dept.cod;
    }
    return node.departmentId || '';
  }, [node.departamentoBackendId, node.departmentId, departamentosMap]);

  const initialProfileId = useMemo(() => {
    return node.perfilCorporativoBackendId || node.profileId || '';
  }, [node.perfilCorporativoBackendId, node.profileId]);

  const initialIsManualProfile = !initialProfileId && !!node.profileName;
  const initialIsManualDept = !initialDepartmentId && !!node.departmentName && node.departmentName !== SEM_DEPARTAMENTO;

  const [profileId, setProfileId] = useState(initialProfileId);
  const [manualProfileName, setManualProfileName] = useState(node.profileName || '');
  const [employeeId, setEmployeeId] = useState(node.employeeId || '');
  const [departmentId, setDepartmentId] = useState(initialDepartmentId);
  const [manualDepartmentName, setManualDepartmentName] = useState(
    node.departmentName && node.departmentName !== SEM_DEPARTAMENTO ? node.departmentName : ''
  );
  const [isCLevel, setIsCLevel] = useState(node.isCLevel || false);
  const [avatarUrl, setAvatarUrl] = useState(node.employeeAvatarUrl || '');
  /** Nome/email do colaborador selecionado no dropdown (para o payload refletir no card ao salvar) */
  const [selectedColaboradorDisplay, setSelectedColaboradorDisplay] = useState<ColaboradorSelecionadoDisplay | null>(null);
  
  const [isManualProfile, setIsManualProfile] = useState(initialIsManualProfile);
  const [isManualDept, setIsManualDept] = useState(initialIsManualDept);
  
  // Estado para rastrear último departamento editado (evita sobrescrever valor editado)
  const [ultimoDepartamentoEditado, setUltimoDepartamentoEditado] = useState<string | null>(null);

  // Função para carregar perfis corporativos (reutilizável)
  const loadPerfisCorporativos = useCallback(async (perfilIdParaManter?: string) => {
    if (!token || !user?.colaboradorOrg?.orgId) return

    try {
      const buscarPerfisUseCase = container.resolve(BuscarPerfisPorOrgUseCase);
      const response = await buscarPerfisUseCase.execute(token, user.colaboradorOrg.orgId);
      
      // Verificar se a resposta tem o formato esperado
      let perfisData: PerfilCorporativoResponse[] = [];
      
      if (response && typeof response === 'object') {
        // Se tem campo retorno, usar ele
        if ('retorno' in response && Array.isArray(response.retorno)) {
          perfisData = response.retorno;
        } 
        // Se a resposta é diretamente um array
        else if (Array.isArray(response)) {
          perfisData = response;
        }
        // Se tem sucesso e retorno
        else if (response.sucesso && 'retorno' in response && Array.isArray(response.retorno)) {
          perfisData = response.retorno;
        }
      }
      
      if (perfisData.length > 0) {
        // Mapear PerfilCorporativoResponse[] para PerfilCorporativoAdaptado[]
        const perfisAdaptados: PerfilCorporativoAdaptado[] = perfisData.map((perfil) => ({
          id: perfil.id,
          descricao: perfil.descricao,
          gestorExternoPerfilId: perfil.id, // Usar o ID do perfil corporativo
          gestorExternoPerfilNome: perfil.descricao, // Usar a descrição como nome
        }));
        
        setPerfisCorporativos(perfisAdaptados);
        
        if (perfilIdParaManter) {
          const perfilEncontrado = perfisAdaptados.find(p => p.gestorExternoPerfilId === perfilIdParaManter);
          if (perfilEncontrado) {
            setProfileId(perfilEncontrado.gestorExternoPerfilId);
            setIsManualProfile(false);
            setManualProfileName('');
            setLoadingPerfis(false);
            return;
          }
        }
        
        if (!perfilIdParaManter && node.perfilCorporativoBackendId) {
          const perfilEncontrado = perfisAdaptados.find(p => p.gestorExternoPerfilId === node.perfilCorporativoBackendId);
          if (perfilEncontrado) {
            setProfileId(perfilEncontrado.gestorExternoPerfilId);
          }
        }
      } else {
        setPerfisCorporativos([]);
      }
    } catch (error) {
      logError(error, { component: 'EditModalMapa', action: 'loadPerfisCorporativos' }, user ?? undefined);
      toast.error('Erro ao carregar perfis corporativos');
      setPerfisCorporativos([]);
    } finally {
      setLoadingPerfis(false);
    }
  }, [token, user?.colaboradorOrg?.orgId, node.perfilCorporativoBackendId]);

  // Ref para rastrear se estamos no meio de uma criação de perfil
  const isCreatingProfileRef = useRef(false);

  // Carregar perfis corporativos quando o modal abrir
  useEffect(() => {
    if (open && !isCreatingProfileRef.current) {
      loadPerfisCorporativos();
    }
    // Resetar o ref quando o modal fechar
    if (!open) {
      isCreatingProfileRef.current = false;
    }
  }, [open, loadPerfisCorporativos]);

  // Estados para modais de perfil
  const [perfilModalOpen, setPerfilModalOpen] = useState(false);
  const [perfilParaEditar, setPerfilParaEditar] = useState<PerfilCorporativoResponse | null>(null);
  
  // Estado para modal de seleção de tipo de perfil
  const [selecionarTipoPerfilModalOpen, setSelecionarTipoPerfilModalOpen] = useState(false);
  const [modoPerfil, setModoPerfil] = useState<'criar' | 'editar'>('criar');

  // Estados para modal de edição de departamento
  const [editarDepartamentoModalOpen, setEditarDepartamentoModalOpen] = useState(false);
  const [departamentoParaEditar, setDepartamentoParaEditar] = useState<DepartamentoMapaRelacionamento | null>(null);

  // Estados para modal de editar perfil corporativo (agora usando CriarEditarPerfilAtuacaoModal unificado)
  const [editarPerfilCorporativoModalOpen, setEditarPerfilCorporativoModalOpen] = useState(false);
  const [perfilCorporativoParaEditar, setPerfilCorporativoParaEditar] = useState<PerfilExterno | null>(null);

  // Estados para modal de inserir gestor externo
  const [inserirGestorExternoModalOpen, setInserirGestorExternoModalOpen] = useState(false);
  const [colaboradoresRefreshTrigger, setColaboradoresRefreshTrigger] = useState(0);
  const [isSaving, setIsSaving] = useState(false);

  const [modalState, setModalState] = useState<ModalState>(() => ({
    departamento: {
      originalId: node.departamentoBackendId,
      novoNome: node.departmentName,
      foiCriado: false,
      foiEditado: false,
    },
    perfilAtuacao: {
      originalId: node.perfilCorporativoBackendId,
      novaDescricao: node.profileName,
      foiCriado: false,
      foiEditado: false,
    },
    colaborador: {
      originalId: node.employeeId || undefined,
      novoId: node.employeeId || undefined,
      foiAlterado: false,
    },
    posicao: {
      originalId: node.posicaoId,
      foiCriado: !node.posicaoId || !!(node as NoMapaRelacionamento & { _isCreatingNew?: boolean })._isCreatingNew,
      foiEditado: !!node.posicaoId && !(node as NoMapaRelacionamento & { _isCreatingNew?: boolean })._isCreatingNew,
    },
    alocacao: {
      originalId: node.alocacaoId,
      originalColaboradorId: node.employeeId && node.employeeId !== 'vacant' ? node.employeeId : undefined,
      foiCriada: !node.alocacaoId && !!node.employeeId && node.employeeId !== 'vacant',
      foiEditada: !!node.alocacaoId,
    },
  }));

  // Verificar se retornou da criação de colaborador
  useEffect(() => {
    if (open && location.state?.returnTo === '/mapa-relacionamento' && location.state?.novoColaboradorId) {
      const novoColaboradorId = location.state.novoColaboradorId;
      setEmployeeId(novoColaboradorId);
      // Limpar state para evitar re-trigger
      window.history.replaceState({}, document.title);
    }
  }, [open, location.state]);

  // Effect A: Sincronização principal a partir do node (sem depender de perfisCorporativos)
  useEffect(() => {
    if (isCreatingProfileRef.current) {
      return;
    }

    const initialProfileId = (node.perfilCorporativoBackendId || node.profileId) ?? '';

    if (ultimoDepartamentoEditado && ultimoDepartamentoEditado === departmentId) {
      setProfileId(initialProfileId);
      setManualProfileName(node.profileName ?? '');
      setEmployeeId(node.employeeId ?? '');
      setSelectedColaboradorDisplay(null);
      setIsCLevel(node.isCLevel ?? false);
      setAvatarUrl(node.employeeAvatarUrl ?? '');
      setIsManualProfile(!initialProfileId && !!node.profileName);
      return;
    }

    let mappedDepartmentId = node.departmentId ?? '';
    if (node.departamentoBackendId && departamentosMap.size > 0) {
      const dept = departamentosMap.get(node.departamentoBackendId);
      if (dept) {
        mappedDepartmentId = dept.cod;
      }
    }

    const newIsManualProfile = !initialProfileId && !!node.profileName;
    const newIsManualDept = !mappedDepartmentId && !!node.departmentName && node.departmentName !== SEM_DEPARTAMENTO;

    setProfileId(initialProfileId);
    setManualProfileName(node.profileName ?? '');
    setEmployeeId(node.employeeId ?? '');
    setSelectedColaboradorDisplay(null);
    setDepartmentId(mappedDepartmentId);
    setManualDepartmentName(
      node.departmentName && node.departmentName !== SEM_DEPARTAMENTO ? node.departmentName : ''
    );
    setIsCLevel(node.isCLevel ?? false);
    setAvatarUrl(node.employeeAvatarUrl ?? '');
    setIsManualProfile(newIsManualProfile);
    setIsManualDept(newIsManualDept);

    setModalState({
      departamento: {
        originalId: node.departamentoBackendId,
        novoNome: node.departmentName,
        foiCriado: false,
        foiEditado: false,
      },
      perfilAtuacao: {
        originalId: node.perfilCorporativoBackendId,
        novaDescricao: node.profileName,
        foiCriado: false,
        foiEditado: false,
      },
      colaborador: {
        originalId: node.employeeId || undefined,
        novoId: node.employeeId || undefined,
        foiAlterado: false,
      },
      posicao: {
        originalId: node.posicaoId,
        foiCriado: !node.posicaoId,
        foiEditado: !!node.posicaoId,
      },
      alocacao: {
        originalId: node.alocacaoId,
        originalColaboradorId: node.employeeId && node.employeeId !== 'vacant' ? node.employeeId : undefined,
        foiCriada: !node.alocacaoId && !!node.employeeId && node.employeeId !== 'vacant',
        foiEditada: !!node.alocacaoId,
      },
    });
  // Não incluir departmentId nas deps: quando o usuário altera o departamento no Select, o effect não deve rodar e sobrescrever.
  }, [node.id, node.profileId, node.profileName, node.employeeId, node.departmentId, node.departmentName, node.isCLevel, node.isExternal, node.employeeAvatarUrl, node.departamentoBackendId, node.perfilCorporativoBackendId, node.posicaoId, node.alocacaoId, open, departamentos, departamentosMap, ultimoDepartamentoEditado]);

  // Effect B: Ajustar profileId quando a lista de perfis corporativos carregar (não sobrescrever após criar perfil)
  useEffect(() => {
    if (isCreatingProfileRef.current || !open || !node.perfilCorporativoBackendId || perfisCorporativosMap.size === 0) {
      return;
    }
    const perfil = perfisCorporativosMap.get(node.perfilCorporativoBackendId);
    if (perfil) {
      setProfileId(perfil.gestorExternoPerfilId);
      setIsManualProfile(false);
    }
  }, [open, node.perfilCorporativoBackendId, perfisCorporativosMap]);

  // Removido: Buscar dados do colaborador não é necessário
  // Usamos os dados que já temos no node (employeeAvatarUrl, employeeName, employeeEmail)

  const handleToggleManualPerfil = useCallback(() => setIsManualProfile((prev) => !prev), []);
  const handleToggleManualDept = useCallback(() => setIsManualDept((prev) => !prev), []);
  const handlePerfilSelect = useCallback((newValue: string) => {
    if (isManualProfile) setManualProfileName(newValue);
    else setProfileId(newValue);
  }, [isManualProfile]);
  const handleDeptSelect = useCallback((newValue: string) => {
    if (isManualDept) setManualDepartmentName(newValue);
    else setDepartmentId(newValue);
  }, [isManualDept]);

  const handleCriarDepartamento = useCallback(() => {
    setIsManualDept(true);
    setManualDepartmentName('');
    setDepartmentId('');
    // Informar que esta cadeira será definida como líder do departamento
    toast.info('Esta cadeira será definida como líder do departamento e pode ser alterada posteriormente', {
      duration: 5000,
    });
  }, []);

  const handleEditarDepartamento = useCallback(() => {
    // Se tem departamento selecionado, usar ele
    if (departmentId && !isManualDept) {
      const departamento = departamentosMap.get(departmentId);
      if (departamento) {
        setDepartamentoParaEditar(departamento);
        setEditarDepartamentoModalOpen(true);
        return;
      }
    }
    
    // Se não tem departamento selecionado mas tem no node
    if (node.departamentoBackendId) {
      const departamento = departamentosMap.get(node.departamentoBackendId);
      if (departamento) {
        setDepartamentoParaEditar(departamento);
        setEditarDepartamentoModalOpen(true);
        return;
      }
    }
    
    toast.error('Selecione um departamento para editar');
  }, [departmentId, isManualDept, node.departamentoBackendId, departamentosMap]);

  const handleCriarPerfil = useCallback(() => {
    setModoPerfil('criar');
    setSelecionarTipoPerfilModalOpen(true);
  }, []);

  const handleEditarPerfilDireto = useCallback(async () => {
    // Verificar se há perfil selecionado antes de prosseguir
    if (!profileId || profileId.trim() === '') {
      // Não mostrar erro, apenas retornar silenciosamente
      return;
    }

    if (!token) {
      toast.error('Erro: token não disponível');
      return;
    }

    try {
      // Buscar dados completos do perfil corporativo usando o endpoint
      const buscarPerfilUseCase = container.resolve(BuscarPerfilCorporativoPorIdUseCase);
      const response = await buscarPerfilUseCase.execute(token, profileId);

      if (response.sucesso && response.retorno) {
        // O endpoint retorna o perfil completo diretamente
        const perfilCompleto = response.retorno;
        
        // Converter para PerfilExterno (compatibilidade com o modal)
        const perfilExterno: PerfilExterno = {
          nomeGestorExterno: '',
          codGestorExterno: '',
          gestorExternoPerfilId: perfilCompleto.id,
          gestorExternoPerfilNome: perfilCompleto.descricao,
          codigoInternoColaborador: perfilCompleto.codigoInternoColaboradorCriacao || '',
        };
        setPerfilCorporativoParaEditar(perfilExterno);
        setEditarPerfilCorporativoModalOpen(true);
      } else {
        // Se não encontrar pelo endpoint, usar a lista de perfis
        const currentPerfil = perfisCorporativosMap.get(profileId);
        if (currentPerfil) {
          const perfilExterno: PerfilExterno = {
            nomeGestorExterno: '',
            codGestorExterno: '',
            gestorExternoPerfilId: currentPerfil.gestorExternoPerfilId,
            gestorExternoPerfilNome: currentPerfil.gestorExternoPerfilNome,
            codigoInternoColaborador: '',
          };
          setPerfilCorporativoParaEditar(perfilExterno);
          setEditarPerfilCorporativoModalOpen(true);
        } else {
          toast.error('Perfil não encontrado');
        }
      }
    } catch (error) {
      logError(error, { component: 'EditModalMapa', action: 'handleEditarPerfilDireto' }, user ?? undefined);
      // Fallback: usar a lista de perfis corporativos
      const currentPerfil = perfisCorporativosMap.get(profileId);
      if (currentPerfil) {
        const perfilExterno: PerfilExterno = {
          nomeGestorExterno: '',
          codGestorExterno: '',
          gestorExternoPerfilId: currentPerfil.gestorExternoPerfilId,
          gestorExternoPerfilNome: currentPerfil.gestorExternoPerfilNome,
          codigoInternoColaborador: '',
        };
        setPerfilCorporativoParaEditar(perfilExterno);
        setEditarPerfilCorporativoModalOpen(true);
      } else {
        toast.error('Erro ao buscar perfil corporativo');
      }
    }
  }, [profileId, token, perfisCorporativos]);

  const handleSelecionarTipoPerfil = useCallback((tipo: TipoPerfil) => {
    if (tipo === 'completo') {
      if (modoPerfil === 'criar') {
        setPerfilParaEditar(null);
        setPerfilModalOpen(true);
      } else {
        handleEditarPerfilDireto();
      }
    } else if (tipo === 'reduzido') {
      // Por enquanto, não fazer nada (opção desabilitada)
    }
  }, [modoPerfil, handleEditarPerfilDireto]);

  const handleEditarPerfil = useCallback(() => {
    if (!profileId || profileId.trim() === '') {
      return;
    }
    // Abrir diretamente o modal de perfil completo com formulário preenchido (sem modal de seleção de tipo)
    handleEditarPerfilDireto();
  }, [profileId, handleEditarPerfilDireto]);

  const handleSalvarPerfilCorporativo = useCallback(async (payload: {
    perfilId: string
    descricao: string
    atribuicoes: string
    permanenciaId: string | null
    modeloTrabalhoId: string | null
    profissionalLocalidadeId: string | null
    experienciaLinkedinId: string | null
    empregoLinkdinId: string | null
  }) => {
    if (!token || !user?.colaboradorOrg?.orgId) {
      toast.error('Erro: token ou orgId não disponível');
      return;
    }

    try {
      // Validar que os campos obrigatórios estão preenchidos
      if (!payload.permanenciaId || !payload.modeloTrabalhoId || !payload.profissionalLocalidadeId) {
        toast.error('Erro: Todos os campos obrigatórios devem ser preenchidos');
        return;
      }

      // Garantir que os valores não sejam strings vazias
      const permanenciaId = typeof payload.permanenciaId === 'string' ? payload.permanenciaId.trim() : payload.permanenciaId;
      const modeloTrabalhoId = typeof payload.modeloTrabalhoId === 'string' ? payload.modeloTrabalhoId.trim() : payload.modeloTrabalhoId;
      const profissionalLocalidadeId = typeof payload.profissionalLocalidadeId === 'string' ? payload.profissionalLocalidadeId.trim() : payload.profissionalLocalidadeId;

      if (!permanenciaId || !modeloTrabalhoId || !profissionalLocalidadeId) {
        toast.error('Erro: Todos os campos obrigatórios devem ser preenchidos');
        return;
      }

      const atualizarPerfilUseCase = container.resolve(AtualizarPerfilCorporativoUseCase);
      const perfilPayload: PerfilCorporativoPayload & { id: string } = {
        id: payload.perfilId,
        orgId: user.colaboradorOrg.orgId,
        descricao: payload.descricao.trim(),
        atribuicoes: payload.atribuicoes.trim() || undefined,
        permanenciaId: permanenciaId,
        modeloTrabalhoId: modeloTrabalhoId,
        profissionalLocalidadeId: profissionalLocalidadeId,
        experienciaLinkedinId: payload.experienciaLinkedinId || undefined,
        empregoLinkdinId: payload.empregoLinkdinId || undefined,
        ativo: true,
      };


      await atualizarPerfilUseCase.execute(token, perfilPayload);
      
      toast.success('Perfil corporativo atualizado com sucesso');
      
      // Recarregar lista de perfis corporativos
      await loadPerfisCorporativos();
      
      // Atualizar o perfil selecionado no modal principal
      setProfileId(payload.perfilId);
      setEditarPerfilCorporativoModalOpen(false);
    } catch (error) {
      logError(error, { component: 'EditModalMapa', action: 'handleSalvarPerfilCorporativo' }, user ?? undefined);
      toast.error('Erro ao salvar perfil corporativo. Tente novamente.');
      throw error;
    }
  }, [token, user, loadPerfisCorporativos]);

  const handleSalvarPerfil = useCallback(async (payload: PerfilCorporativoPayload & { id?: string }) => {
    if (!token) {
      toast.error('Erro: token não disponível');
      return;
    }

    const isEdit = !!perfilParaEditar && !!payload.id;

    try {
      let perfilId: string;
      let perfilResponse: PerfilCorporativoResponse;

      if (isEdit && payload.id) {
        const atualizarPerfilUseCase = container.resolve(AtualizarPerfilCorporativoUseCase);
        perfilResponse = await atualizarPerfilUseCase.execute(token, payload as PerfilCorporativoPayload & { id: string });
        perfilId = perfilResponse.id;
        toast.success('Perfil de atuação atualizado com sucesso');
      } else {
        const criarPerfilUseCase = container.resolve(CriarPerfilCorporativoUseCase);
        perfilResponse = await criarPerfilUseCase.execute(token, payload);
        perfilId = perfilResponse.id;
        toast.success('Perfil de atuação criado com sucesso');
      }


      // Atualizar estado de rastreamento
      setModalState((prev) => ({
        ...prev,
        perfilAtuacao: {
          ...prev.perfilAtuacao,
          originalId: perfilId,
          novaDescricao: payload.descricao,
          foiCriado: !isEdit,
          foiEditado: isEdit,
        },
      }));

      // Marcar que estamos criando um perfil para evitar que o useEffect interfira
      isCreatingProfileRef.current = true;

      // Adicionar o perfil à lista imediatamente ANTES de recarregar
      const perfilAdaptado: PerfilCorporativoAdaptado = {
        id: perfilResponse.id,
        descricao: perfilResponse.descricao,
        gestorExternoPerfilId: perfilResponse.id,
        gestorExternoPerfilNome: perfilResponse.descricao,
      };


      // Usar um batch update para garantir que tudo seja atualizado junto
      setPerfisCorporativos((prev) => {
        // Remover perfil existente se houver (para evitar duplicatas)
        const filtered = prev.filter(p => p.gestorExternoPerfilId !== perfilResponse.id);
        const newList = [...filtered, perfilAdaptado];
        return newList;
      });

      // Setar o profileId imediatamente APÓS adicionar à lista
      // Usar setTimeout para garantir que o estado da lista foi atualizado primeiro
      setTimeout(() => {
        setProfileId(perfilId);
        setIsManualProfile(false);
        setManualProfileName('');
      }, 0);

      // Recarregar lista de perfis corporativos em background (não bloqueia)
      loadPerfisCorporativos(perfilId).then(() => {
        // Garantir que o profileId ainda está setado após recarregar
        setProfileId(perfilId);
        setIsManualProfile(false);
      });

      // Resetar o ref após delay para que Effect B não sobrescreva o perfil recém-selecionado
      setTimeout(() => {
        isCreatingProfileRef.current = false;
      }, 1500);

      setPerfilModalOpen(false);
    } catch (error) {
      logError(error, { component: 'EditModalMapa', action: 'handleSalvarPerfil' }, user ?? undefined);
      toast.error('Erro ao salvar perfil de atuação. Tente novamente.');
      isCreatingProfileRef.current = false;
    }
  }, [token, user, perfisCorporativos, perfilParaEditar, loadPerfisCorporativos]);

  const handleSalvarDepartamento = useCallback(async (payload: {
    departamentoId: string
    nome: string
    organogramaPosicaoIdLider: string | null
  }) => {
    if (!token || !user?.colaboradorOrg?.orgId) {
      toast.error('Erro: token ou orgId não disponível');
      return;
    }

    try {
      const departamentoId = payload.departamentoId;
      const organogramaPosicaoIdLider = payload.organogramaPosicaoIdLider;

      const atualizarDeptUseCase = container.resolve(AtualizarDepartamentoUseCase);
      const departamentoPayload: DepartamentoPayload & { id: string } = {
        id: departamentoId,
        orgId: user.colaboradorOrg.orgId,
        nome: payload.nome.trim(),
        codigoCliente: clientCode,
        organogramaPosicaoIdLider,
        ativo: true,
      };

      await atualizarDeptUseCase.execute(token, departamentoPayload);
      
      toast.success('Departamento atualizado com sucesso');
      
      // Recarregar lista de departamentos ANTES de atualizar o campo
      await dispatch(listarDepartamentosMapaRelacionamento({ token, codigoCliente: clientCode }));
      
      // Aguardar um pouco para garantir que a lista foi atualizada
      await new Promise(resolve => setTimeout(resolve, 100));
      
      // Atualizar o departamento selecionado no modal principal
      // IMPORTANTE: Usar o payload.departamentoId (ID do departamento editado)
      setDepartmentId(payload.departamentoId);
      setIsManualDept(false); // Garantir que não está em modo manual
      
      // Atualizar o nome manual com o novo nome do payload
      setManualDepartmentName(payload.nome.trim());
      
      // Atualizar modalState para refletir a edição
      setModalState(prev => ({
        ...prev,
        departamento: {
          originalId: payload.departamentoId, // Atualizar para o ID editado
          novoNome: payload.nome.trim(),
          foiCriado: false,
          foiEditado: true, // Marcar como editado
        },
      }));
      
      // Marcar como último departamento editado para evitar sobrescrever
      setUltimoDepartamentoEditado(payload.departamentoId);
      
      setEditarDepartamentoModalOpen(false);
    } catch (error) {
      logError(error, { component: 'EditModalMapa', action: 'handleSalvarDepartamento' }, user ?? undefined);
      toast.error('Erro ao salvar departamento. Tente novamente.');
      throw error;
    }
  }, [token, user?.colaboradorOrg?.orgId, clientCode, dispatch]);

  // Atualizar estado quando campos mudam
  useEffect(() => {
    const employeeChanged = employeeId !== modalState.colaborador.originalId;
    setModalState((prev) => ({
      ...prev,
      colaborador: {
        ...prev.colaborador,
        novoId: employeeId || undefined,
        foiAlterado: employeeChanged,
      },
      alocacao: {
        ...prev.alocacao,
        foiCriada: !prev.alocacao.originalId && !!employeeId && employeeId !== 'vacant',
        foiEditada: !!prev.alocacao.originalId && employeeChanged,
      },
    }));
  }, [employeeId, modalState.colaborador.originalId, modalState.alocacao.originalId]);

  // Removido: Não marcar como foiCriado automaticamente quando alterna para modo manual
  // Só marcar como foiCriado quando realmente criado no modal separado (CriarEditarDepartamentoModal/CriarEditarPerfilAtuacaoModal)

  const handleSave = async () => {
    if (isSaving) return;
    
    setIsSaving(true);
    try {
      // Criar estado atualizado para usar no payload
      const updatedModalState: ModalState = { ...modalState };
    
    // Se está em modo manual (criando novo departamento), marcar como novo departamento
    if (isManualDept && manualDepartmentName?.trim()) {
      // Se está digitando um novo nome de departamento, verificar se é realmente novo ou edição
      // Se não tinha departamentoBackendId antes OU o nome digitado é diferente do nome atual, é um novo departamento
      const originalDeptName = node.departmentName && node.departmentName !== SEM_DEPARTAMENTO ? node.departmentName : '';
      const isNewDepartamento = !node.departamentoBackendId || 
        (manualDepartmentName.trim() !== originalDeptName);
      updatedModalState.departamento = {
        ...modalState.departamento,
        novoNome: manualDepartmentName.trim(),
        originalId: isNewDepartamento ? undefined : node.departamentoBackendId,
        foiCriado: isNewDepartamento, // Novo se não tinha departamento antes OU nome é diferente
        foiEditado: !isNewDepartamento && !!node.departamentoBackendId, // Editado apenas se tinha departamento e nome é o mesmo
      };
    } else if (!isManualDept && departmentId) {
      // Se selecionou um departamento existente, marcar como editado se era diferente
      updatedModalState.departamento = {
        ...modalState.departamento,
        originalId: departmentId,
        foiCriado: false,
        foiEditado: departmentId !== node.departamentoBackendId,
      };
    }
    if (isManualProfile && manualProfileName?.trim()) {
      updatedModalState.perfilAtuacao = {
        ...modalState.perfilAtuacao,
        novaDescricao: manualProfileName.trim(),
        // Não marcar como foiCriado - só será true se criado no modal separado
        foiCriado: false,
        foiEditado: false,
      };
    } else if (!isManualProfile && profileId) {
      // Se selecionou um perfil no dropdown, atualizar o estado
      updatedModalState.perfilAtuacao = {
        ...modalState.perfilAtuacao,
        originalId: profileId, // Usar o ID do perfil selecionado no dropdown
        foiCriado: false,
        foiEditado: profileId !== node.perfilCorporativoBackendId, // Editado se mudou o perfil
      };
    }

    const profile = perfisCorporativosMap.get(profileId);
    const department = departamentosMap.get(departmentId);

    // Usar nome/email do colaborador selecionado no dropdown; senão do node (para o card atualizar ao salvar)
    const employeeName = selectedColaboradorDisplay?.nome ?? node.employeeName ?? 'Vago';
    const employeeEmail = selectedColaboradorDisplay?.email ?? node.employeeEmail;

    const isNowConnected = employeeId !== 'vacant' && employeeId !== null;
    const finalWasConnected = node.wasConnected || isNowConnected;

    // Obter IDs atualizados do modalState
    // IMPORTANTE: Só criar/atualizar Departamento/Perfil se realmente foram criados/editados no modal
    // Se apenas selecionados, usar os IDs que vêm diretamente do dropdown (departmentId, profileId)
    const departamentoBackendIdFinal = updatedModalState.departamento.originalId || node.departamentoBackendId || (isManualDept ? undefined : departmentId);
    // Priorizar: 1) ID do perfil criado/editado no modal separado, 2) profileId selecionado no dropdown, 3) valor antigo do node
    const perfilCorporativoBackendIdFinal = updatedModalState.perfilAtuacao.originalId || (isManualProfile ? undefined : profileId) || node.perfilCorporativoBackendId;

    const payloadBase: NoMapaRelacionamento = {
        id: node.id,
        profileId: isManualProfile ? null : (profileId || null),
        employeeId: employeeId || null,
        profileName: isManualProfile ? manualProfileName : (profile?.gestorExternoPerfilNome || manualProfileName),
        employeeName,
        employeeEmail,
        employeeAvatarUrl: avatarUrl,
        departmentId: isManualDept ? undefined : (departmentId || undefined),
        departmentName: isManualDept ? manualDepartmentName : (department?.departamento || manualDepartmentName),
        isCLevel,
        isExternal: true, // Sempre true
        wasConnected: finalWasConnected,
        children: node.children || [],
        // Usar IDs atualizados do modalState ou estado local
        posicaoId: node.posicaoId,
        alocacaoId: node.alocacaoId,
        departamentoBackendId: departamentoBackendIdFinal,
        perfilCorporativoBackendId: perfilCorporativoBackendIdFinal,
    };

    // Preservar informações do pai se existirem (para criação de filho)
    const nodeWithPai = node as NoMapaRelacionamento & { _paiPosicaoId?: string; _paiId?: string; _isCreatingNew?: boolean }
    
    // Criar payload completo com todas as propriedades dinâmicas usando a interface adequada
    const payload: PayloadCompleto = {
      ...payloadBase,
      modalState: updatedModalState,
      ...(nodeWithPai._paiPosicaoId && { _paiPosicaoId: nodeWithPai._paiPosicaoId }),
      ...(nodeWithPai._paiId && { _paiId: nodeWithPai._paiId }),
      ...(nodeWithPai._isCreatingNew && { _isCreatingNew: nodeWithPai._isCreatingNew }),
    }

      // PayloadCompleto estende NoMapaRelacionamento, então é compatível com onSave
      await onSave(payload as NoMapaRelacionamento);
      
      // Recarregar lista de colaboradores para refletir mudanças
      setColaboradoresRefreshTrigger(prev => prev + 1);
    } catch (error) {
      logError(error, { component: 'EditModalMapa', action: 'handleSave' });
      toast.error('Erro ao salvar alterações');
    } finally {
      setIsSaving(false);
    }
  };

  const handleInteractOutside = useCallback((e: Event) => {
    const target = e.target as HTMLElement;
    // Permitir cliques no dropdown de profissionais sem fechar o modal
    if (target?.closest?.('[role="listbox"]')) {
      e.preventDefault();
    }
  }, []);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-visible z-[110]" onInteractOutside={handleInteractOutside}>
        <DialogHeader>
          <DialogTitle>Configurações do Perfil de atuação</DialogTitle>
          <DialogDescription>
            Configure o perfil de atuação, departamento e profissional para esta posição na hierarquia.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-4 max-h-[calc(90vh-180px)] overflow-y-auto">
            <AutocompleteGenericoMapa<PerfilCorporativoAdaptado>
                label="Perfil Corporativo" icon={<Briefcase className="w-5 h-5" />}
                value={isManualProfile ? manualProfileName : (profileId || '')}
                options={loadingPerfis ? [] : perfisCorporativos} 
                onSelect={handlePerfilSelect}
                onToggleManual={handleToggleManualPerfil}
                isManual={isManualProfile} 
                displayKey="gestorExternoPerfilNome" 
                idKey="gestorExternoPerfilId" 
                required
                onCreateNew={handleCriarPerfil}
                onEdit={handleEditarPerfil}
                placeholder={loadingPerfis ? 'Carregando...' : 'Selecione um perfil corporativo...'}
            />


            <div className="flex items-center justify-between p-4 bg-muted border border-border rounded-lg">
                <div className="flex items-center gap-2">
                    <Crown className="w-4 h-4 text-amber-500 shrink-0" />
                    <span className="text-sm font-bold text-foreground">Nível C-Level</span>
                    <div className="group relative flex items-center">
                        <Info className="w-4 h-4 text-muted-foreground cursor-help shrink-0" />
                        <div className="absolute bottom-full left-0 mb-2 w-48 p-2 bg-popover text-popover-foreground text-xs rounded shadow-lg opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none z-[120] text-left border border-border">
                            C-Level refere-se aos Perfis de atuação executivos de mais alto nível (CEO, CFO, CTO, etc.).
                        </div>
                    </div>
                </div>
                <Switch checked={isCLevel} onCheckedChange={setIsCLevel} />
            </div>

            <div className="flex items-center justify-between p-4 bg-muted border border-border rounded-lg overflow-visible opacity-60">
                <div className="flex items-center gap-2 relative">
                    <span className="text-sm font-bold text-foreground">Profissional Externo</span>
                    <div className="group relative">
                        <Info className="w-4 h-4 text-muted-foreground cursor-help shrink-0" />
                        <div className="absolute bottom-full left-0 mb-2 w-48 p-2 bg-popover text-popover-foreground text-xs rounded shadow-lg opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none z-[120] text-left border border-border">
                            Indica se o profissional é externo à organização.
                        </div>
                    </div>
                </div>
                <Switch checked={true} disabled />
            </div>

            <div className="space-y-2">
                <AutocompleteGenericoMapa<DepartamentoMapaRelacionamento>
                    label="Departamento" icon={<Building className="w-5 h-5" />}
                    value={isManualDept ? manualDepartmentName : departmentId}
                    options={departamentos}
                    onSelect={handleDeptSelect}
                    onToggleManual={handleToggleManualDept}
                    isManual={isManualDept}
                    displayKey="departamento"
                    idKey="cod"
                    onCreateNew={handleCriarDepartamento}
                    onEdit={handleEditarDepartamento}
                />
                {isManualDept && (
                    <p className="text-xs text-muted-foreground italic flex items-center gap-1.5">
                        <Info className="w-3.5 h-3.5 shrink-0" />
                        Esta cadeira será definida como líder do departamento e pode ser alterada posteriormente.
                    </p>
                )}
            </div>

            <div className="space-y-2">
                <div className="flex items-end gap-2">
                    <div className="flex-1">
                        <AutocompleteProfissionalMapa
                            label="Profissional"
                            value={employeeId}
                            onSelect={setEmployeeId}
                            onColaboradorSelect={setSelectedColaboradorDisplay}
                            token={token}
                            user={user}
                            codigoCliente={clientCode}
                            open={open}
                            employeeName={node.employeeName}
                            refreshTrigger={colaboradoresRefreshTrigger}
                        />
                    </div>
                    <Button
                        type="button"
                        variant="outline"
                        size="icon"
                        onClick={() => setInserirGestorExternoModalOpen(true)}
                        className="shrink-0 rounded-lg h-10 w-10"
                        title="Adicionar gestor externo"
                    >
                        <Plus className="w-5 h-5" />
                    </Button>
                </div>
            </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)} disabled={isSaving}>
            Cancelar
          </Button>
          <Button onClick={handleSave} disabled={isSaving}>
            {isSaving ? (
              <>
                <Spinner size={16} className="text-current" />
                <span>Salvando...</span>
              </>
            ) : (
              'Salvar Alterações'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>

      {/* Montar modais aninhados só quando abertos para reduzir custo de render do modal principal */}
      {perfilModalOpen && (
        <CriarEditarPerfilAtuacaoModal
          open={perfilModalOpen}
          onOpenChange={setPerfilModalOpen}
          perfil={perfilParaEditar}
          onSave={handleSalvarPerfil}
        />
      )}

      {editarDepartamentoModalOpen && (
        <EditarDepartamentoModal
          open={editarDepartamentoModalOpen}
          onOpenChange={setEditarDepartamentoModalOpen}
          departamentos={departamentos}
          departamentoSelecionado={departamentoParaEditar}
          onSave={handleSalvarDepartamento}
        />
      )}

      {editarPerfilCorporativoModalOpen && (
        <CriarEditarPerfilAtuacaoModal
          open={editarPerfilCorporativoModalOpen}
          onOpenChange={setEditarPerfilCorporativoModalOpen}
          perfisExternos={perfis}
          perfilExternoSelecionado={perfilCorporativoParaEditar}
          onSaveWithPerfilId={handleSalvarPerfilCorporativo}
        />
      )}

      {inserirGestorExternoModalOpen && (
        <InserirGestorExternoModal
          open={inserirGestorExternoModalOpen}
          onOpenChange={setInserirGestorExternoModalOpen}
          codigoCliente={clientCode}
          onSuccess={() => {
            setColaboradoresRefreshTrigger((prev) => prev + 1);
          }}
        />
      )}

      {selecionarTipoPerfilModalOpen && (
        <SelecionarTipoPerfilModal
          open={selecionarTipoPerfilModalOpen}
          onOpenChange={setSelecionarTipoPerfilModalOpen}
          onSelect={handleSelecionarTipoPerfil}
          modo={modoPerfil}
        />
      )}
    </Dialog>
  );
}

export const EditModalMapa = memo(EditModalMapaInner)
EditModalMapa.displayName = 'EditModalMapa'
