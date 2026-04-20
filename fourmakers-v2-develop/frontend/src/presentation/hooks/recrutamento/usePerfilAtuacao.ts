import { useCallback, useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { useAppSelector } from '@app/store/hooks';
import { useToast } from '@/hooks/use-toast';
import type { PerfilAtuacaoFormData, PerfilSkill, PerfilSkillType, PerfilAtuacaoOption } from '@domain/entities/PerfilAtuacao';
import {
  defaultSkillOptions,
  initialPerfilAtuacaoState,
  mapSkillsToPayload,
  validatePerfilForm,
  validatePerfilFormNovaVaga,
  formatCep,
  normalizeCep,
} from '@shared/utils/perfilAtuacao';
import { formatarValorMonetario, desformatarValorMonetario } from '@shared/utils/formatUtils';
import {
  fetchLocalidades,
  fetchModelosTrabalho,
  fetchNiveisExperiencia,
  fetchPermanencias,
  fetchTiposEmprego,
  fetchClientes,
  fetchGestores,
  fetchCompetencias,
  fetchSoftskills,
  fetchMetodologias,
  fetchDominios,
  fetchIdiomas,
  fetchNiveisCompetencia,
  fetchNiveisSoftskill,
  fetchNiveisMetodologia,
  fetchNiveisDominio,
  fetchNiveisIdioma,
  extrairPerfilPorIA,
  inserirGestorExternoPerfil,
  obterGestorExternoPerfilPorId,
  atualizarGestorExternoPerfil,
} from '@data/api/PerfilAtuacaoApi';
import { fetchCep } from '@data/api/ViaCepApi';
import { useDebounced } from '@shared/hooks/useDebounced';

// TODO(ARCH): Débito técnico — migrar chamadas para UseCases; presentation não deve importar de @data/api (ARCHITECTURE.md).

const mergeSkills = (form: PerfilAtuacaoFormData) => [
  ...form.hardSkills,
  ...form.softSkills,
  ...form.methodologies,
  ...form.businessDomains,
  ...form.languages,
];

export const usePerfilAtuacao = () => {
  const { token } = useAppSelector(state => state.auth);
  const { toast } = useToast();
  const [searchParams] = useSearchParams();
  const idPerfilFromUrl = searchParams.get('idPerfil') ?? undefined;
  const novaVagaFromUrl = searchParams.get('novaVaga') === 'true';
  const codigoClienteUrl = searchParams.get('codigoCliente') ?? undefined;
  const nomeClienteUrl = searchParams.get('nomeCliente') ? decodeURIComponent(searchParams.get('nomeCliente')!) : undefined;
  const codigoGestorUrl = searchParams.get('codigoGestor') ?? undefined;
  const nomeGestorUrl = searchParams.get('nomeGestor') ? decodeURIComponent(searchParams.get('nomeGestor')!) : undefined;

  const [formData, setFormData] = useState<PerfilAtuacaoFormData>(initialPerfilAtuacaoState);
  const [optionsLoading, setOptionsLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [clients, setClients] = useState<PerfilAtuacaoOption[]>([]);
  const [managers, setManagers] = useState<{ id: string; name: string; internalCode: string }[]>([]);
  const [clientSearch, setClientSearch] = useState('');
  const [managerSearch, setManagerSearch] = useState('');
  const [clientsLoading, setClientsLoading] = useState(false);
  const [managersLoading, setManagersLoading] = useState(false);
  const [iaToken, setIaToken] = useState('');
  const [showTokenModal, setShowTokenModal] = useState(false);
  const [skillModalOpen, setSkillModalOpen] = useState(false);
  const [skillModalType, setSkillModalType] = useState<PerfilSkillType>('COMPETENCIA');
  const [skillSearch, setSkillSearch] = useState('');
  const [skillResults, setSkillResults] = useState<PerfilSkill[]>([]);
  const [skillLoading, setSkillLoading] = useState(false);
  const [skillHasSearched, setSkillHasSearched] = useState(false);
  const [selectedSkill, setSelectedSkill] = useState<PerfilSkill | null>(null);
  const [skillLevels, setSkillLevels] = useState<{ id: number; descricao: string }[]>([]);
  const [skillLevelsLoading, setSkillLevelsLoading] = useState(false);
  const [selectedLevelId, setSelectedLevelId] = useState<string>('');
  const [pendingSkills, setPendingSkills] = useState<PerfilSkill[]>([]);
  const [editingSkillForLevel, setEditingSkillForLevel] = useState<{
    skill: PerfilSkill;
    context: 'modal' | 'page';
    type: PerfilSkillType;
  } | null>(null);
  const [editLevelSelectedId, setEditLevelSelectedId] = useState<string>('');
  const [aiLoading, setAiLoading] = useState(false);
  const [aiError, setAiError] = useState<string | null>(null);
  const [aiSummary, setAiSummary] = useState<{
    resumo?: Record<string, string | null>;
    completude?: number;
  } | null>(null);
  const [permanenceOptions, setPermanenceOptions] = useState<PerfilAtuacaoOption[]>([]);
  const [workModels, setWorkModels] = useState<PerfilAtuacaoOption[]>([]);
  const [locationOptions, setLocationOptions] = useState<PerfilAtuacaoOption[]>([]);
  const [employmentTypes, setEmploymentTypes] = useState<PerfilAtuacaoOption[]>([]);
  const [experienceLevels, setExperienceLevels] = useState<PerfilAtuacaoOption[]>([]);
  const [fieldErrors, setFieldErrors] = useState<Record<string, boolean>>({});
  const [skillIdsWithLevelError, setSkillIdsWithLevelError] = useState<string[]>([]);
  const [profileLoading, setProfileLoading] = useState(false);
  const fieldLabelMap: Record<string, string> = {
    name: 'Nome do Perfil',
    clientCode: 'Cliente',
    clientName: 'Cliente',
    managerCode: 'Gestor',
    managerName: 'Gestor',
    cost: 'Custo',
    ratecard: 'Ratecard',
    workModelId: 'Modelo de Trabalho',
    permanenceId: 'Permanência',
    locationId: 'Localidade',
    employmentTypeId: 'Tipo de Emprego',
    experienceLevelId: 'Nível de Experiência',
    hybridDays: 'Dias presenciais',
    cep: 'CEP',
    uf: 'UF',
    city: 'Cidade',
    responsibilities: 'Atribuições',
    hardSkills: 'Hard Skills (mín. 2)',
    softSkills: 'Soft Skills (mín. 1)',
  };

  const skillOptions = useMemo(() => defaultSkillOptions, []);
  const debouncedClientSearch = useDebounced(clientSearch, 300);
  const debouncedManagerSearch = useDebounced(managerSearch, 300);
  const debouncedSkillSearch = useDebounced(skillSearch, 300);

  useEffect(() => {
    const loadOptions = async () => {
      if (!token) return;
      setOptionsLoading(true);
      try {
        const [permanencias, modelos, localidades, tiposEmprego, niveis] = await Promise.all([
          fetchPermanencias(token),
          fetchModelosTrabalho(token),
          fetchLocalidades(token),
          fetchTiposEmprego(token),
          fetchNiveisExperiencia(token),
        ]);

        setPermanenceOptions(permanencias.map(p => ({ id: p.id, name: p.descricao })));
        setWorkModels(
          modelos.map(m => ({
            id: m.id,
            name: m.descricao,
            code: m.codigo,
            type: m.codigo === 2 ? 'hibrido' : m.codigo === 3 ? 'remoto' : 'presencial',
          })),
        );
        setLocationOptions(localidades.map(l => ({ id: l.id, name: l.descricao })));
        setEmploymentTypes(tiposEmprego.map(t => ({ id: t.id, name: t.descricao })));
        setExperienceLevels(niveis.map(n => ({ id: n.id, name: n.descricao })));
      } catch (error) {
        console.error('Erro ao carregar opções do perfil de atuação', error);
        toast({
          title: 'Não foi possível carregar opções',
          description: 'Verifique o token ou tente novamente em instantes.',
          variant: 'destructive',
        });
      } finally {
        setOptionsLoading(false);
      }
    };

    void loadOptions();
  }, [token, toast]);

  useEffect(() => {
    if (!codigoClienteUrl && !nomeClienteUrl && !codigoGestorUrl && !nomeGestorUrl) return;
    setFormData(prev => ({
      ...prev,
      ...(codigoClienteUrl && { clientCode: codigoClienteUrl }),
      ...(nomeClienteUrl && { clientName: nomeClienteUrl }),
      ...(codigoGestorUrl && { managerCode: codigoGestorUrl }),
      ...(nomeGestorUrl && { managerName: nomeGestorUrl }),
    }));
    if (codigoClienteUrl && nomeClienteUrl) {
      setClients(prev => {
        const has = prev.some(c => c.id === codigoClienteUrl);
        if (!has) return [...prev, { id: codigoClienteUrl, name: nomeClienteUrl }];
        return prev;
      });
    }
    if (codigoGestorUrl && nomeGestorUrl) {
      setManagers(prev => {
        const has = prev.some(m => m.id === codigoGestorUrl);
        if (!has) return [...prev, { id: codigoGestorUrl, name: nomeGestorUrl, internalCode: '' }];
        return prev;
      });
    }
  }, [codigoClienteUrl, nomeClienteUrl, codigoGestorUrl, nomeGestorUrl]);

  useEffect(() => {
    const loadProfileById = async () => {
      if (!idPerfilFromUrl?.trim() || !token) return;
      setProfileLoading(true);
      try {
        const perfil = await obterGestorExternoPerfilPorId(token, idPerfilFromUrl.trim());
        if (!perfil) {
          toast({
            title: 'Perfil não encontrado',
            description: 'Não foi possível carregar os dados do perfil.',
            variant: 'destructive',
          });
          return;
        }
        const getOptionId = (value: string | null | undefined, options: PerfilAtuacaoOption[]) =>
          value && options.some(opt => opt.id === value) ? value : (value || '');

        const mapSkillsByItemId = (itemId: number, type: PerfilSkillType) =>
          (perfil.gestorExternoPerfilSkills || [])
            .filter(s => s.itemPerfil.id === itemId)
            .map(s => ({
              id: String(s.skill.id),
              name: s.skill.descricao,
              type,
              levelId: s.nivel.id,
              levelName: s.nivel.descricao,
              relevante: s.relevante ?? true,
            }));

        const hardSkills = mapSkillsByItemId(1, 'COMPETENCIA');
        const softSkills = mapSkillsByItemId(8, 'SOFTSKILL');
        const methodologies = mapSkillsByItemId(3, 'METODOLOGIA');
        const businessDomains = mapSkillsByItemId(4, 'DOMINIONEGOCIO');
        const languages = mapSkillsByItemId(9, 'IDIOMA');

        const selectedWorkModel = workModels.find(w => w.id === (perfil.modeloTrabalhoId || ''));
        const workModelCode = selectedWorkModel?.code ?? 0;

        const costFormatted =
          perfil.custoPerfil != null ? formatarValorMonetario(String(Math.round(perfil.custoPerfil * 100))) : undefined;
        const ratecardFormatted =
          perfil.ratecardPerfil != null ? formatarValorMonetario(String(Math.round(perfil.ratecardPerfil * 100))) : undefined;
        setFormData(prev => ({
          ...prev,
          name: perfil.nomePerfil || prev.name,
          clientCode: codigoClienteUrl || prev.clientCode,
          clientName: nomeClienteUrl || prev.clientName,
          managerCode: codigoGestorUrl || perfil.codGestorExterno || prev.managerCode,
          managerName: nomeGestorUrl || prev.managerName,
          cost: costFormatted ?? prev.cost,
          ratecard: ratecardFormatted ?? prev.ratecard,
          permanenceId: getOptionId(perfil.permanenciaId, permanenceOptions) || perfil.permanenciaId || prev.permanenceId,
          workModelId: getOptionId(perfil.modeloTrabalhoId, workModels) || perfil.modeloTrabalhoId || prev.workModelId,
          workModelCode,
          locationId: getOptionId(perfil.profissionalLocalidadeId, locationOptions) || perfil.profissionalLocalidadeId || prev.locationId,
          city: perfil.cidade || prev.city,
          uf: perfil.estado || prev.uf,
          cep: perfil.cep || prev.cep,
          hybridDays: (() => {
            const v = perfil.hibridoDias ?? prev.hybridDays;
            return v == null ? prev.hybridDays : Math.min(4, Math.max(1, v));
          })(),
          employmentTypeId: perfil.tipoEmpregoLinkedin || prev.employmentTypeId,
          experienceLevelId: perfil.nivelExperienciaLinkedin || prev.experienceLevelId,
          responsibilities: perfil.atribuicoes || prev.responsibilities,
          notes: perfil.informacoesRelevantes ?? prev.notes ?? '',
          linkedinInfo: perfil.informacoesRelevantes ?? prev.linkedinInfo ?? '',
          hardSkills,
          softSkills,
          methodologies,
          businessDomains,
          languages,
        }));
        toast({ title: 'Perfil carregado', description: 'Dados do perfil preenchidos para edição.', variant: 'success' });
      } catch (error) {
        console.error('Erro ao carregar perfil', error);
        toast({
          title: 'Erro ao carregar perfil',
          description: 'Não foi possível carregar os dados. Tente novamente.',
          variant: 'destructive',
        });
      } finally {
        setProfileLoading(false);
      }
    };
    void loadProfileById();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [idPerfilFromUrl, token]);

  useEffect(() => {
    const loadClients = async () => {
      if (!token) return;
      setClientsLoading(true);
      try {
        const data = await fetchClientes(token, debouncedClientSearch.trim());
        setClients(
          data.map(c => ({
            id: c.codigoCliente,
            name: c.nomeCliente,
          })),
        );
      } catch (error) {
        console.error('Erro ao buscar clientes', error);
        setClients([]);
      } finally {
        setClientsLoading(false);
      }
    };
    void loadClients();
  }, [token, debouncedClientSearch]);

  useEffect(() => {
    const loadManagers = async () => {
      if (!token || !formData.clientCode) {
        setManagers([]);
        return;
      }
      setManagersLoading(true);
      try {
        const data = await fetchGestores(token, formData.clientCode, debouncedManagerSearch.trim());
        setManagers(
          data.map(m => ({
            id: m.codGestorExterno,
            name: m.nome,
            internalCode: m.codigoInternoColaborador,
          })),
        );
      } catch (error) {
        console.error('Erro ao buscar gestores', error);
        setManagers([]);
      } finally {
        setManagersLoading(false);
      }
    };
    void loadManagers();
  }, [token, formData.clientCode, debouncedManagerSearch]);

  // Buscar skills por tipo
  useEffect(() => {
    const searchSkills = async () => {
      if (!token || !skillModalOpen) return;
      const query = debouncedSkillSearch.trim();
      if (!query || query.length < 2) {
        setSkillResults([]);
        setSkillHasSearched(false);
        return;
      }
      setSkillLoading(true);
      setSkillHasSearched(true);
      try {
        let data: PerfilSkill[] = [];
        switch (skillModalType) {
          case 'SOFTSKILL': {
            const softs = await fetchSoftskills(token, query);
            data = softs.map(s => ({ id: String(s.id), name: s.descricao, type: 'SOFTSKILL' }));
            break;
          }
          case 'METODOLOGIA': {
            const metodologias = await fetchMetodologias(token, query);
            data = metodologias.map(m => ({ id: String(m.id), name: m.descricao, type: 'METODOLOGIA' }));
            break;
          }
          case 'DOMINIONEGOCIO': {
            const dominios = await fetchDominios(token, query);
            data = dominios.map(d => ({ id: String(d.id), name: d.descricao || d.nome || '', type: 'DOMINIONEGOCIO' }));
            break;
          }
          case 'IDIOMA': {
            const idiomas = await fetchIdiomas(token, query);
            data = idiomas.map(i => ({ id: String(i.id), name: i.descricao || i.nome || '', type: 'IDIOMA' }));
            break;
          }
          default: {
            const competencias = await fetchCompetencias(token, query);
            data = competencias.map(c => ({ id: String(c.id), name: c.descricao || c.nome || '', type: 'COMPETENCIA' }));
          }
        }
        const existingIds = new Set([
          ...mergeSkills(formData).map(s => s.id),
          ...pendingSkills.map(s => s.id),
        ]);
        setSkillResults(data.filter(skill => !existingIds.has(skill.id)));
      } catch (error) {
        console.error('Erro ao buscar skills', error);
        setSkillResults([]);
      } finally {
        setSkillLoading(false);
      }
    };
    void searchSkills();
  }, [debouncedSkillSearch, skillModalType, token, skillModalOpen, formData, pendingSkills]);

  const handleInputChange = useCallback(<K extends keyof PerfilAtuacaoFormData>(field: K, value: PerfilAtuacaoFormData[K]) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  }, []);

  const handleWorkModelChange = useCallback(
    (workModelId: string) => {
      const selected = workModels.find(w => w.id === workModelId);
      const isHybrid = selected?.code === 2;
      setFormData(prev => ({
        ...prev,
        workModelId,
        workModelCode: selected?.code || 0,
        ...(isHybrid
          ? { hybridDays: Math.min(4, Math.max(1, prev.hybridDays ?? 1)) }
          : {}),
      }));
    },
    [workModels],
  );

  const handleAddSkill = useCallback((type: PerfilSkillType, skillId: string) => {
    const available = skillOptions[type].find(s => s.id === skillId);
    if (!available) return;
    const fieldMap: Record<PerfilSkillType, keyof PerfilAtuacaoFormData> = {
      COMPETENCIA: 'hardSkills',
      SOFTSKILL: 'softSkills',
      METODOLOGIA: 'methodologies',
      DOMINIONEGOCIO: 'businessDomains',
      IDIOMA: 'languages',
    };
    const field = fieldMap[type];
    setFormData(prev => {
      const current = prev[field] as PerfilSkill[];
      if (current.some(skill => skill.id === available.id)) return prev;
      return { ...prev, [field]: [...current, available] };
    });
  }, [skillOptions]);

  const handleAddCustomSkill = useCallback((type: PerfilSkillType, name: string) => {
    if (!name.trim()) return;
    const fieldMap: Record<PerfilSkillType, keyof PerfilAtuacaoFormData> = {
      COMPETENCIA: 'hardSkills',
      SOFTSKILL: 'softSkills',
      METODOLOGIA: 'methodologies',
      DOMINIONEGOCIO: 'businessDomains',
      IDIOMA: 'languages',
    };
    const field = fieldMap[type];
    const newSkill: PerfilSkill = {
      id: `${Date.now()}`,
      name: name.trim(),
      type,
    };
    setFormData(prev => ({ ...prev, [field]: [...(prev[field] as PerfilSkill[]), newSkill] }));
  }, []);

  const handleRemoveSkill = useCallback((type: PerfilSkillType, id: string) => {
    const fieldMap: Record<PerfilSkillType, keyof PerfilAtuacaoFormData> = {
      COMPETENCIA: 'hardSkills',
      SOFTSKILL: 'softSkills',
      METODOLOGIA: 'methodologies',
      DOMINIONEGOCIO: 'businessDomains',
      IDIOMA: 'languages',
    };
    const field = fieldMap[type];
    setFormData(prev => ({
      ...prev,
      [field]: (prev[field] as PerfilSkill[]).filter(skill => skill.id !== id),
    }));
  }, []);

  const handleCepLookup = useCallback(async (value: string) => {
    const cep = normalizeCep(value);
    if (cep.length !== 8) return;
    try {
      const data = await fetchCep(cep);
      if (data.erro) throw new Error('CEP não encontrado');
      setFormData(prev => ({
        ...prev,
        city: data.localidade ?? prev.city,
        uf: data.uf ?? prev.uf,
      }));
      toast({ title: 'CEP encontrado', description: 'Endereço preenchido automaticamente.', variant: 'info' });
    } catch (error) {
      console.error('Erro ao buscar CEP', error);
      toast({
        title: 'Não foi possível buscar o CEP',
        description: 'Verifique o código informado.',
        variant: 'destructive',
      });
      setFormData(prev => ({ ...prev, city: '', uf: '' }));
    }
  }, [toast]);

  const openSkillModal = useCallback((type: PerfilSkillType) => {
    setSkillModalType(type);
    setPendingSkills(({
      COMPETENCIA: formData.hardSkills,
      SOFTSKILL: formData.softSkills,
      METODOLOGIA: formData.methodologies,
      DOMINIONEGOCIO: formData.businessDomains,
      IDIOMA: formData.languages,
    } as Record<PerfilSkillType, PerfilSkill[]>)[type]);
    setSkillSearch('');
    setSkillResults([]);
    setSelectedSkill(null);
    setSelectedLevelId('');
    setSkillModalOpen(true);
  }, [formData]);

  const loadSkillLevels = useCallback(async (type: PerfilSkillType) => {
    if (!token) return;
    setSkillLevelsLoading(true);
    try {
      let niveis: { id: number; descricao: string }[] = [];
      switch (type) {
        case 'SOFTSKILL':
          niveis = await fetchNiveisSoftskill(token);
          break;
        case 'METODOLOGIA':
          niveis = await fetchNiveisMetodologia(token);
          break;
        case 'DOMINIONEGOCIO':
          niveis = await fetchNiveisDominio(token);
          break;
        case 'IDIOMA':
          niveis = await fetchNiveisIdioma(token);
          break;
        default:
          niveis = await fetchNiveisCompetencia(token);
      }
      setSkillLevels(niveis);
    } catch (error) {
      console.error('Erro ao carregar níveis de skill', error);
      setSkillLevels([]);
    } finally {
      setSkillLevelsLoading(false);
    }
  }, [token]);

  const handleSelectSkill = useCallback((skill: PerfilSkill) => {
    setSelectedSkill(skill);
    setSelectedLevelId('');
    void loadSkillLevels(skill.type);
  }, [loadSkillLevels]);

  const handleSelectLevel = useCallback((levelId: string) => {
    setSelectedLevelId(levelId);
    if (!selectedSkill) return;
    const nivel = skillLevels.find(n => n.id.toString() === levelId);
    const skillWithLevel: PerfilSkill = {
      ...selectedSkill,
      levelId: Number(levelId),
      levelName: nivel?.descricao || '',
      relevante: true,
    };
    setPendingSkills(prev => {
      const existing = prev.findIndex(
        s => s.id === skillWithLevel.id && (s.id !== '0' || s.name === skillWithLevel.name)
      );
      if (existing >= 0) {
        const clone = [...prev];
        clone[existing] = { ...skillWithLevel, relevante: prev[existing].relevante ?? true };
        return clone;
      }
      return [...prev, skillWithLevel];
    });
    setSelectedSkill(null);
    setSelectedLevelId('');
    setSkillSearch('');
    setSkillResults([]);
    setSkillHasSearched(false);
    setSkillLevels([]);
  }, [selectedSkill, skillLevels]);

  const handleRemovePendingSkill = useCallback((id: string, name?: string) => {
    setPendingSkills(prev =>
      prev.filter(s => (id !== '0' ? s.id !== id : !(s.id === id && s.name === name)))
    );
  }, []);

  const handleTogglePendingSkillRelevante = useCallback((skillId: string) => {
    setPendingSkills(prev =>
      prev.map(s => (s.id === skillId ? { ...s, relevante: !(s.relevante ?? true) } : s))
    );
  }, []);

  const handleToggleSkillRelevante = useCallback((type: PerfilSkillType, skillId: string) => {
    const fieldMap: Record<PerfilSkillType, keyof PerfilAtuacaoFormData> = {
      COMPETENCIA: 'hardSkills',
      SOFTSKILL: 'softSkills',
      METODOLOGIA: 'methodologies',
      DOMINIONEGOCIO: 'businessDomains',
      IDIOMA: 'languages',
    };
    const field = fieldMap[type];
    setFormData(prev => ({
      ...prev,
      [field]: (prev[field] as PerfilSkill[]).map(s =>
        s.id === skillId ? { ...s, relevante: !(s.relevante ?? true) } : s
      ),
    }));
  }, []);

  const openEditLevelModal = useCallback(
    (skill: PerfilSkill, context: 'modal' | 'page', type?: PerfilSkillType) => {
      const skillType = type ?? skill.type;
      setEditingSkillForLevel({ skill, context, type: skillType });
      setEditLevelSelectedId(String(skill.levelId ?? ''));
      void loadSkillLevels(skillType);
    },
    [loadSkillLevels]
  );

  const closeEditLevelModal = useCallback(() => {
    setEditingSkillForLevel(null);
    setEditLevelSelectedId('');
  }, []);

  const handleSelectLevelForEdit = useCallback((levelId: string) => {
    setEditLevelSelectedId(levelId);
  }, []);

  const handleUpdateSkillLevel = useCallback(() => {
    if (!editingSkillForLevel) return;
    const nivel = skillLevels.find(n => n.id.toString() === editLevelSelectedId);
    if (!nivel) return;
    const { skill, context, type } = editingSkillForLevel;
    const updated: PerfilSkill = {
      ...skill,
      levelId: nivel.id,
      levelName: nivel.descricao,
    };
    if (context === 'modal') {
      setPendingSkills(prev =>
        prev.map(s =>
          s.id === skill.id && (s.id !== '0' || s.name === skill.name) ? { ...updated, relevante: s.relevante } : s
        )
      );
    } else {
      const fieldMap: Record<PerfilSkillType, keyof PerfilAtuacaoFormData> = {
        COMPETENCIA: 'hardSkills',
        SOFTSKILL: 'softSkills',
        METODOLOGIA: 'methodologies',
        DOMINIONEGOCIO: 'businessDomains',
        IDIOMA: 'languages',
      };
      const field = fieldMap[type];
      setFormData(prev => ({
        ...prev,
        [field]: (prev[field] as PerfilSkill[]).map(s => (s.id === skill.id ? updated : s)),
      }));
    }
    setEditingSkillForLevel(null);
    setEditLevelSelectedId('');
  }, [editingSkillForLevel, skillLevels, editLevelSelectedId]);

  const handleCreateSkillFromSearch = useCallback(() => {
    const name = skillSearch.trim();
    if (name.length < 2) return;
    const newSkill: PerfilSkill = { id: '0', name, type: skillModalType };
    setSelectedSkill(newSkill);
    setSelectedLevelId('');
    void loadSkillLevels(skillModalType);
  }, [skillSearch, skillModalType, loadSkillLevels]);

  const handleSavePendingSkills = useCallback(() => {
    const fieldMap: Record<PerfilSkillType, keyof PerfilAtuacaoFormData> = {
      COMPETENCIA: 'hardSkills',
      SOFTSKILL: 'softSkills',
      METODOLOGIA: 'methodologies',
      DOMINIONEGOCIO: 'businessDomains',
      IDIOMA: 'languages',
    };
    const field = fieldMap[skillModalType];
    setFormData(prev => ({ ...prev, [field]: pendingSkills }));
    setSkillModalOpen(false);
    setSkillResults([]);
    setSelectedSkill(null);
    setSelectedLevelId('');
  }, [pendingSkills, skillModalType]);

  const handleGenerateWithAI = useCallback(async () => {
    if (!formData.aiPrompt.trim()) {
      toast({
        title: 'Informe um prompt',
        description: 'Descreva o perfil para gerar com IA.',
        variant: 'destructive',
      });
      return;
    }
    const tokenToUse = iaToken?.trim() || token;
    if (!tokenToUse) {
      toast({
        title: 'Token ausente',
        description: 'Faça login para gerar com IA.',
        variant: 'destructive',
      });
      return;
    }
    setAiLoading(true);
    setAiError(null);
    try {
      const response = await extrairPerfilPorIA(tokenToUse, formData.aiPrompt);
      const perfil = response.perfil_extraido;
      if (!perfil) throw new Error('Resposta inválida da IA');

      const resumoInfo = response.validacao_informacoes?.resumo_informacoes as Record<string, string | null> | undefined;
      const completude = response.validacao_informacoes?.completude_percentual as number | undefined;
      const getOptionId = (value: string | null | undefined, options: PerfilAtuacaoOption[]) =>
        value && options.some(opt => opt.id === value) ? value : '';

      const normalizeCepValue = (value?: string | null) => {
        if (!value) return '';
        const digits = value.replace(/\D/g, '').slice(0, 8);
        if (digits.length !== 8 || /^0+$/.test(digits)) return '';
        return `${digits.slice(0, 5)}-${digits.slice(5)}`;
      };

      const cepFromPerfil = normalizeCepValue(perfil.cep);
      const cepFromResumo = normalizeCepValue(resumoInfo?.cep || resumoInfo?.cep);
      const finalCep = cepFromPerfil || cepFromResumo || '';

      const mapSkillsByItemId = (itemId: number, type: PerfilSkillType) =>
        (perfil.gestorExternoPerfilSkills || [])
          .filter(s => s.itemPerfil.id === itemId)
          .map(s => ({
            id: String(s.skill.id),
            name: s.skill.descricao,
            type,
            levelId: s.nivel.id,
            levelName: s.nivel.descricao,
          }));

      const hardSkills = mapSkillsByItemId(1, 'COMPETENCIA');
      const softSkills = mapSkillsByItemId(8, 'SOFTSKILL');
      const methodologies = mapSkillsByItemId(3, 'METODOLOGIA');
      const businessDomains = mapSkillsByItemId(4, 'DOMINIONEGOCIO');
      const languages = mapSkillsByItemId(9, 'IDIOMA');

      const rawModelId = perfil.modeloTrabalhoId != null ? String(perfil.modeloTrabalhoId) : '';
      const workModelIdResolved = getOptionId(perfil.modeloTrabalhoId, workModels) || rawModelId;
      const selectedWorkModelFromAi = workModels.find(w => w.id === workModelIdResolved);
      const workModelCodeFromAi = selectedWorkModelFromAi?.code ?? 0;

      setFormData(prev => ({
        ...prev,
        name: perfil.nomePerfil || prev.name,
        cost: perfil.custoPerfil ? String(perfil.custoPerfil) : prev.cost,
        ratecard: perfil.ratecardPerfil ? String(perfil.ratecardPerfil) : prev.ratecard,
        permanenceId: getOptionId(perfil.permanenciaId, permanenceOptions) || prev.permanenceId,
        workModelId: workModelIdResolved || prev.workModelId,
        workModelCode: workModelCodeFromAi,
        locationId: getOptionId(perfil.profissionalLocalidadeId, locationOptions) || prev.locationId,
        city: perfil.cidade || prev.city,
        uf: perfil.estado || prev.uf,
        cep: finalCep || prev.cep,
        hybridDays: (() => {
          const v = perfil.hibridoDias ?? prev.hybridDays;
          return v == null ? prev.hybridDays : Math.min(4, Math.max(1, v));
        })(),
        employmentTypeId: perfil.tipoEmpregoLinkedin || prev.employmentTypeId,
        experienceLevelId: perfil.nivelExperienciaLinkedin || prev.experienceLevelId,
        responsibilities: perfil.atribuicoes || prev.responsibilities,
        linkedinInfo: perfil.informacoesLinkedin || prev.linkedinInfo,
        hardSkills,
        softSkills,
        methodologies,
        businessDomains,
        languages,
      }));
      setAiSummary({ resumo: resumoInfo, completude });
      toast({ title: 'Perfil gerado pela IA', description: 'Campos preenchidos automaticamente.', variant: 'success' });
    } catch (error) {
      console.error('Erro ao gerar perfil com IA', error);
      setAiError('Não foi possível gerar com IA');
      toast({
        title: 'Erro ao gerar com IA',
        description: 'Tente novamente mais tarde.',
        variant: 'destructive',
      });
    } finally {
      setAiLoading(false);
    }
  }, [formData.aiPrompt, token, toast, workModels, permanenceOptions, locationOptions]);

  const handleSubmit = useCallback(async () => {
    setFieldErrors({});
    const selectedWorkModel = workModels.find(w => w.id === formData.workModelId);
    const isRemote = selectedWorkModel?.type === 'remoto';
    const isHybrid = selectedWorkModel?.type === 'hibrido';

    if (novaVagaFromUrl) {
      const result = validatePerfilFormNovaVaga(formData, !!isRemote, !!isHybrid);
      if (!result.isValid) {
        setFieldErrors(result.errors);
        const missingLabels = result.missing.map(f => fieldLabelMap[f] || f);
        toast({
          title: 'Perfil incompleto para criar vaga',
          description: `Para criar uma vaga é necessário preencher todos os campos do perfil e ter no mínimo 2 Hard Skills e 1 Soft Skill. Faltando: ${missingLabels.join(', ')}`,
          variant: 'destructive',
        });
        return { success: false, missing: result.missing };
      }
    } else {
      const { missing } = validatePerfilForm(formData);
      const missingFields = [
        ...missing,
        ...(formData.hardSkills.length === 0 ? ['hardSkills'] : []),
        ...(formData.softSkills.length === 0 ? ['softSkills'] : []),
        ...(formData.employmentTypeId ? [] : ['employmentTypeId']),
        ...(formData.experienceLevelId ? [] : ['experienceLevelId']),
        ...(isHybrid && !formData.hybridDays ? ['hybridDays'] : []),
      ];
      if (missingFields.length > 0) {
        const errors: Record<string, boolean> = {};
        missingFields.forEach(f => {
          errors[f] = true;
        });
        setFieldErrors(errors);
        const missingLabels = missingFields.map(f => fieldLabelMap[f] || f);
        toast({
          title: 'Preencha os campos obrigatórios',
          description: `Campos faltantes: ${missingLabels.join(', ')}`,
          variant: 'destructive',
        });
        return { success: false, missing: missingFields };
      }
      if (!isRemote && !formData.cep.replace(/\D/g, '')) {
        setFieldErrors(prev => ({ ...prev, cep: true }));
        toast({
          title: 'CEP obrigatório',
          description: 'Informe o CEP para modelos presencial ou híbrido.',
          variant: 'destructive',
        });
        return { success: false, missing: ['cep'] };
      }
    }
    if (!token) {
      toast({
        title: 'Token ausente',
        description: 'Faça login para registrar o perfil.',
        variant: 'destructive',
      });
      return { success: false, missing: ['token'] };
    }

    const allSkills = mergeSkills(formData);
    const skillsWithInvalidLevel = allSkills.filter(
      s => !s.levelName?.trim() || s.levelName.trim().toLowerCase() === 'a definir'
    );
    if (skillsWithInvalidLevel.length > 0) {
      const ids = skillsWithInvalidLevel.map(s => s.id);
      setSkillIdsWithLevelError(ids);
      const names = skillsWithInvalidLevel.map(s => s.name).join(', ');
      toast({
        title: 'Nível da habilidade obrigatório',
        description: `É necessário definir o nível da habilidade (não pode ser "A definir"). Habilidades: ${names}`,
        variant: 'destructive',
      });
      return { success: false, missing: ['skillsLevel'] };
    }
    setSkillIdsWithLevelError([]);

    setSubmitting(true);
    try {
      const skillsPayload = mapSkillsToPayload(mergeSkills(formData));
      const dataCriacaoSkill = new Date().toISOString().slice(0, 19);
      const skillsWithDataCriacao = skillsPayload.map(s => ({ ...s, dataCriacao: dataCriacaoSkill }));
      const costNum = formData.cost ? Number(desformatarValorMonetario(formData.cost)) : null;
      const ratecardNum = formData.ratecard ? Number(desformatarValorMonetario(formData.ratecard)) : null;
      const payload = {
        gestorExternoPerfilSkills: idPerfilFromUrl ? skillsWithDataCriacao : skillsPayload,
        codGestorExterno: formData.managerCode,
        nomePerfil: formData.name,
        custoPerfil: costNum,
        ratecardPerfil: ratecardNum,
        informacoesRelevantes: formData.linkedinInfo ?? '',
        permanenciaId: formData.permanenceId,
        modeloTrabalhoId: formData.workModelId,
        profissionalLocalidadeId: formData.locationId,
        Cidade: formData.city || null,
        Estado: formData.uf || null,
        Cep: formData.cep || null,
        hibridoDias: formData.workModelCode === 2 ? formData.hybridDays : null,
        nivelExperienciaLinkedin: formData.experienceLevelId,
        tipoEmpregoLinkedin: formData.employmentTypeId,
        atribuicoes: formData.responsibilities || null,
      };

      if (idPerfilFromUrl) {
        await atualizarGestorExternoPerfil(token, idPerfilFromUrl, payload);
        setFieldErrors({});
        setSkillIdsWithLevelError([]);
        toast({ title: 'Perfil atualizado', description: 'Perfil de atuação atualizado com sucesso.', variant: 'success' });
        return { success: true };
      }
      await inserirGestorExternoPerfil(token, payload);
      setFieldErrors({});
      setSkillIdsWithLevelError([]);
      toast({ title: 'Perfil criado', description: 'Perfil de atuação registrado com sucesso.', variant: 'success' });
      setFormData(initialPerfilAtuacaoState);
      return { success: true };
    } catch (error) {
      console.error('Erro ao salvar perfil', error);
      toast({
        title: 'Erro ao salvar',
        description: idPerfilFromUrl ? 'Não foi possível atualizar o perfil. Tente novamente.' : 'Não foi possível criar o perfil. Tente novamente.',
        variant: 'destructive',
      });
      return { success: false, missing: [] };
    } finally {
      setSubmitting(false);
    }
  }, [formData, toast, token, idPerfilFromUrl, novaVagaFromUrl, workModels]);

  const buildLinkedinTemplate = useCallback(() => {
    const line = '\n';
    const formatList = (title: string, items: PerfilSkill[]) =>
      items.length ? `${title}${line}${items.map(skill => `• ${skill.name}${skill.levelName ? ` (${skill.levelName})` : ''}`).join(line)}` : '';

    const selectedWorkModel = workModels.find(w => w.id === formData.workModelId);
    const code = selectedWorkModel?.code ?? formData.workModelCode;
    const modelo =
      code === 2 ? 'Híbrido' : code === 3 ? 'Remoto' : formData.workModelId && selectedWorkModel ? selectedWorkModel.name : 'Definir modelo';
    const diasLine = code === 2 ? `📅 Dias presenciais: ${formData.hybridDays ?? 1}` : '';
    const desafios = formData.responsibilities?.trim() || 'Desafios principais do perfil...';

    const blocks = [
      `🚀 Oportunidade: ${formData.name || 'Perfil Fourmakers'}`,
      `📍 Modelo: ${modelo}`,
      diasLine,
      `📋 Desafios:\n${desafios}`,
      formatList('💡 Hard Skills:', formData.hardSkills),
      formatList('🤝 Soft Skills:', formData.softSkills),
      formatList('🧭 Metodologias:', formData.methodologies),
      formatList('🏢 Domínios de Negócio:', formData.businessDomains),
      formatList('🌐 Idiomas:', formData.languages),
      '🎯 Benefícios:\n• Plano de saúde\n• Vale refeição/alimentação\n• Ambiente de trabalho colaborativo\n• Oportunidades de crescimento',
      '#vaga #oportunidade #tech #usabilidade',
    ]
      .filter(Boolean)
      .join(`${line}${line}`);

    return blocks.trim();
  }, [formData, workModels]);

  const handleFillLinkedinTemplate = useCallback(() => {
    setFormData(prev => ({ ...prev, linkedinInfo: buildLinkedinTemplate() }));
    toast({
      title: 'Template aplicado',
      description: 'Texto atualizado com o template padrão Foursys.',
      variant: 'info',
    });
  }, [buildLinkedinTemplate, toast]);

  return {
    idPerfilFromUrl,
    novaVagaMode: novaVagaFromUrl,
    profileLoading,
    formData,
    optionsLoading,
    submitting,
    aiLoading,
    aiError,
    iaToken,
    setIaToken,
    showTokenModal,
    setShowTokenModal,
    aiSummary,
    fieldErrors,
    clients,
    managers,
    clientsLoading,
    managersLoading,
    setClientSearch,
    setManagerSearch,
    permanenceOptions,
    workModels,
    locationOptions,
    employmentTypes,
    experienceLevels,
    skillOptions,
    handleInputChange,
    handleWorkModelChange,
    handleAddSkill,
    handleAddCustomSkill,
    handleRemoveSkill,
    openSkillModal,
    skillModalOpen,
    setSkillModalOpen,
    skillModalType,
    skillSearch,
    setSkillSearch,
    skillResults,
    skillLoading,
    skillHasSearched,
    selectedSkill,
    handleSelectSkill,
    skillLevels,
    skillLevelsLoading,
    selectedLevelId,
    handleSelectLevel,
    handleRemovePendingSkill,
    handleTogglePendingSkillRelevante,
    handleToggleSkillRelevante,
    editingSkillForLevel,
    openEditLevelModal,
    closeEditLevelModal,
    editLevelSelectedId,
    handleSelectLevelForEdit,
    handleUpdateSkillLevel,
    handleCreateSkillFromSearch,
    pendingSkills,
    handleSavePendingSkills,
    handleCepLookup,
    handleSubmit,
    handleGenerateWithAI,
    handleFillLinkedinTemplate,
    formatCep,
    formatarValorMonetario,
    skillIdsWithLevelError,
  };
};
