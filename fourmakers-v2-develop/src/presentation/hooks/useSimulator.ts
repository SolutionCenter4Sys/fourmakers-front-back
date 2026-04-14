import { useEffect, useMemo, useRef, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '@app/store/hooks';
import { trackEvent } from '@shared/utils/analytics';
import { logUserAction } from '@shared/utils/firebaseAnalytics';
import {
  AGE_RANGES,
  calculateAge,
  calculateReverseSalary,
  calculateSalary,
  formatCep,
  formatCurrency,
  getAgeRange,
  getHealthPlanPrice,
  HEALTH_PLANS,
  HOURS_PER_MONTH,
  MEAL_VOUCHER_BASE,
  MOBILITY_RATE_PER_KM,
  fetchCepData,
  formatCepForInput,
  isAbortError,
  isCepComplete,
  normalizeCep,
  type CepAutofillSnapshot,
} from '@shared/utils/calculations';

const CEP_LOOKUP_DEBOUNCE_MS = 1000;
import type { SimulationData, Manager } from '@domain/entities/SimulatorTypes';
import type { CandidaturaEditPayload } from '@domain/entities/CandidaturaDetails';
import type { RemuneracaoCalculationPayload } from '@domain/entities/RemuneracaoCalculation';
import { useModelosTrabalho } from '@/hooks/useModelosTrabalho';
import { toast } from 'sonner';
import { usePdfExport } from '@/hooks/usePdfExport';
import {
  fetchManagers,
  fetchUserProfile,
  fetchVagaDetails,
  fetchCandidateDetails,
  fetchCandidaturaDetails,
  saveCandidaturaData,
  fetchVagasList,
  fetchCandidatosList,
  calculateRemuneracaoTotal,
  resetForm,
  setApiToken,
  setSelectedCandidateId,
  setSelectedCandidaturaId,
  setSelectedVagaId,
  setSavingCandidaturaError,
  updateFormData,
} from '@app/store/slices/simulatorSlice';

export type ViewMode = 'form' | 'recruiter' | 'collaborator';

export const useSimulator = () => {
  const dispatch = useAppDispatch();
  const {
    formData,
    managers,
    loadingManagers,
    apiToken,
    vagaDetails,
    candidateDetails,
    loadingVagaDetails,
    loadingCandidateDetails,
    loadingCandidaturaDetails,
    calculationLoading,
    calculationResult,
    calculationValidations,
    selectedVagaId,
    selectedCandidateId,
    selectedCandidaturaId,
    candidaturaDetails,
    savingCandidaturaData,
    vagasList,
    candidatosList,
    loadingVagasList,
    loadingCandidatosList,
  } = useAppSelector((state) => state.simulator);
  const authToken = useAppSelector((state) => state.auth.token);
  const user = useAppSelector((state) => state.auth.user);
  const { generatePDF } = usePdfExport();
  const effectiveToken = authToken || apiToken;
  const { list: workModels, loading: loadingModelosTrabalho, refetch: refetchModelosTrabalho } = useModelosTrabalho(effectiveToken);

  const [showDetails, setShowDetails] = useState(false);
  const [showTokenModal, setShowTokenModal] = useState(false);
  const [tempToken, setTempToken] = useState('');
  const [cepLookupStatus, setCepLookupStatus] = useState<'idle' | 'loading' | 'success' | 'error'>('idle');
  const [cepFieldError, setCepFieldError] = useState<string | null>(null);
  const [showCompanyCost, setShowCompanyCost] = useState(false);
  const [viewMode, setViewMode] = useState<ViewMode>('recruiter');
  /** Incrementa quando o cálculo retorna sucesso; força re-render para habilitar Detalhes no mesmo clique. */
  const [calculationSuccessVersion, setCalculationSuccessVersion] = useState(0);
  const lastCepAutoFill = useRef<CepAutofillSnapshot | null>(null);
  const urlParamsApplied = useRef(false);
  /** Contexto (vaga/candidato/form) no momento do último "Gerar Cálculo" bem-sucedido. Detalhes só habilitado se igual ao contexto atual. */
  const lastValidCalculationContextRef = useRef<string | null>(null);
  const location = useLocation();

  useEffect(() => {
    trackEvent('load_application_state', { source: 'simulator_page' });
  }, []);

  // Garantir chamada ListarModelosTrabalho ao carregar o simulador com token (para exibir modelo da vaga e regra de frequência).
  useEffect(() => {
    if (!effectiveToken) return;
    refetchModelosTrabalho(true);
  }, [effectiveToken]); // eslint-disable-line react-hooks/exhaustive-deps -- refetch apenas quando token fica disponível

  useEffect(() => {
    if (workModels.length === 0 || !formData.transportModel || formData.modeloTrabalhoId) return;
    const toTransport = (desc: string): SimulationData['transportModel'] => {
      const n = desc.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
      if (n.includes('presencial') && !n.includes('hibrido')) return 'Presencial';
      if (n.includes('hibrido')) return 'Hibrido';
      return '100% Remoto';
    };
    const match = workModels.find((m) => toTransport(m.descricao) === formData.transportModel);
    if (match) dispatch(updateFormData({ modeloTrabalhoId: match.id }));
  }, [workModels, formData.transportModel, formData.modeloTrabalhoId, dispatch]);

  // Pré-seleção a partir da URL: idVaga (detalhes da vaga + inscritos) e/ou idCandidatura + codigoInternoColaborador (dados da candidatura)
  useEffect(() => {
    if (!effectiveToken || urlParamsApplied.current) return;
    const params = new URLSearchParams(location.search);
    const idVaga = params.get('idVaga')?.trim();
    const idCandidatura = params.get('idCandidatura')?.trim();
    const codigoInternoColaborador = params.get('codigoInternoColaborador')?.trim();
    const hasVagaParam = Boolean(idVaga);
    const hasCandidaturaParams = Boolean(idCandidatura && codigoInternoColaborador);
    if (!hasVagaParam && !hasCandidaturaParams) return;
    urlParamsApplied.current = true;

    // Definir candidato/candidatura antes do fetch da vaga para fetchVagaDetails.fulfilled não limpar a seleção
    if (hasCandidaturaParams) {
      dispatch(setSelectedCandidateId(codigoInternoColaborador!));
      dispatch(setSelectedCandidaturaId(idCandidatura!));
    } else if (hasVagaParam) {
      dispatch(setSelectedCandidateId(''));
      dispatch(setSelectedCandidaturaId(''));
    }

    if (hasVagaParam) {
      dispatch(setSelectedVagaId(idVaga!));
      dispatch(fetchVagaDetails({ token: effectiveToken, vagaId: idVaga! }))
        .unwrap()
        .catch((err: unknown) => {
          const msg = err instanceof Error ? err.message : null;
          toast.error(msg ?? 'Erro ao carregar vaga.');
        });
    }

    if (hasCandidaturaParams) {
      dispatch(
        fetchCandidaturaDetails({
          token: effectiveToken,
          candidateId: codigoInternoColaborador!,
          candidaturaId: idCandidatura!,
        }),
      )
        .unwrap()
        .catch((err: unknown) => {
          const msg = err instanceof Error ? err.message : null;
          toast.error(msg ?? 'Erro ao carregar candidatura.');
          if (!hasVagaParam) {
            urlParamsApplied.current = false;
            dispatch(setSelectedCandidateId(''));
            dispatch(setSelectedCandidaturaId(''));
          }
        });
    }
  }, [location.search, effectiveToken, dispatch]);

  useEffect(() => {
    if (authToken && authToken !== apiToken) {
      dispatch(setApiToken(authToken));
    }
  }, [authToken, apiToken, dispatch]);

  useEffect(() => {
    if (!authToken) {
      return;
    }
    dispatch(fetchUserProfile(authToken));
  }, [authToken, dispatch]);

  useEffect(() => {
    if (!effectiveToken) {
      return;
    }
    dispatch(fetchVagasList(effectiveToken));
  }, [effectiveToken, dispatch]);

  useEffect(() => {
    const price = getHealthPlanPrice(formData.healthPlanRole, formData.ageRange);
    if (price !== formData.healthPlanValue) {
      dispatch(updateFormData({ healthPlanValue: price }));
    }
  }, [formData.healthPlanRole, formData.ageRange, dispatch, formData.healthPlanValue]);

  useEffect(() => {
    if (formData.birthDate) {
      const age = calculateAge(formData.birthDate);
      const range = getAgeRange(age);
      if (formData.ageRange !== range) {
        dispatch(updateFormData({ ageRange: range }));
      }
    }
  }, [formData.birthDate, dispatch, formData.ageRange]);

  useEffect(() => {
    const timer = setTimeout(() => {
      if (formData.manager && apiToken) {
        dispatch(fetchManagers({ token: apiToken, search: formData.manager }));
      }
    }, 500);
    return () => clearTimeout(timer);
  }, [formData.manager, apiToken, dispatch]);

  useEffect(() => {
    if (!selectedVagaId || !effectiveToken) {
      return;
    }
    dispatch(fetchCandidatosList({ token: effectiveToken, vagaId: selectedVagaId }));
  }, [selectedVagaId, effectiveToken, dispatch]);

  useEffect(() => {
    const cleanZip = (zip: string) => zip.replace(/\D/g, '');
    const z1 = cleanZip(formData.zipCode);
    const z2 = cleanZip(formData.workZipCode);

    if (z1.length === 8 && z2.length === 8) {
      const sum = parseInt(z1.substring(0, 4)) + parseInt(z2.substring(0, 4));
      const mockKm = (sum % 40) + 10;

      if (formData.dailyKm !== mockKm) {
        dispatch(updateFormData({ dailyKm: mockKm }));
        trackEvent('calculate_mobility', { section: 'Mobilidade', value: mockKm });
      }
    }
  }, [formData.zipCode, formData.workZipCode, dispatch, formData.dailyKm]);

  const { proposedResults, desiredResults, desiredGross } = useMemo(() => {
    const calcFormData = viewMode !== 'collaborator' ? { ...formData, variableExpenses: [] } : formData;

    const proposed = calculateSalary(calcFormData);
    const totalVarExpenses =
      calcFormData.variableExpenses?.reduce((acc, curr) => acc + (curr.value || 0), 0) || 0;
    const planDependentsCount =
      calcFormData.wantsFoursysPlan && calcFormData.wantsFoursysPlanIncludeDependents
        ? Math.max(1, calcFormData.wantsFoursysPlanDependentsCount || 1)
        : 0;
    const planDependentsCost =
      (calcFormData.wantsFoursysPlan ? calcFormData.healthPlanValue || 0 : 0) * planDependentsCount;
    const targetNetBeforeVar =
      (calcFormData.desiredNetSalary || 0) + totalVarExpenses + planDependentsCost;
    const calculatedGross = calculateReverseSalary(targetNetBeforeVar, calcFormData.dependentsIRPF || 0);

    const desiredSimulation: SimulationData = {
      ...calcFormData,
      grossSalary: calculatedGross,
    };

    const desired = calculateSalary(desiredSimulation);

    return {
      proposedResults: proposed,
      desiredResults: desired,
      desiredGross: calculatedGross,
    };
  }, [formData, viewMode]);

  const planDependentsCount =
    formData.wantsFoursysPlan && formData.wantsFoursysPlanIncludeDependents
      ? Math.max(1, formData.wantsFoursysPlanDependentsCount || 1)
      : 0;
  const planDependentsUnit = formData.wantsFoursysPlan ? formData.healthPlanValue || 0 : 0;
  const planDependentsTotal = planDependentsCount > 0 ? planDependentsUnit * planDependentsCount : 0;
  const totalEducationCost =
    (formData.studiesCurrently ? formData.educationMonthlyCost : 0) +
    (formData.childrenStudyUpTo24 ? formData.childrenEducationMonthlyCost : 0);

  /** Contexto usado para habilitar o botão Detalhes: só após Gerar Cálculo e sem alteração de vaga/candidato/form. */
  const getCalculationContext = () =>
    JSON.stringify({
      vagaId: selectedVagaId ?? '',
      candidateId: selectedCandidateId ?? '',
      candidaturaId: selectedCandidaturaId ?? '',
      desiredNetSalary: formData.desiredNetSalary,
      dependentsIRPF: formData.dependentsIRPF,
      foodAllowance: formData.foodAllowance,
      desiredAjudaDeCusto: formData.desiredAjudaDeCusto,
      dailyKm: formData.dailyKm,
      mobilityCost: proposedResults.mobilityCost,
      totalEducationCost,
    });

  // Invalidar "Detalhes" só quando o contexto atual for diferente do que foi usado no último cálculo (evita zerar logo após Gerar Cálculo).
  useEffect(() => {
    const currentContext = getCalculationContext();
    if (
      lastValidCalculationContextRef.current !== null &&
      lastValidCalculationContextRef.current !== currentContext
    ) {
      lastValidCalculationContextRef.current = null;
    }
  }, [
    selectedVagaId,
    selectedCandidateId,
    selectedCandidaturaId,
    formData.desiredNetSalary,
    formData.dependentsIRPF,
    formData.foodAllowance,
    formData.desiredAjudaDeCusto,
    formData.dailyKm,
    proposedResults.mobilityCost,
    totalEducationCost,
  ]);

  const handleInputChange = (field: keyof SimulationData, value: SimulationData[keyof SimulationData]) => {
    if (field === 'zipCode') setCepFieldError(null);
    dispatch(updateFormData({ [field]: value } as Partial<SimulationData>));
  };

  const descricaoToTransportModel = (desc: string): SimulationData['transportModel'] => {
    const n = desc.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
    if (n.includes('presencial') && !n.includes('hibrido')) return 'Presencial';
    if (n.includes('hibrido')) return 'Hibrido';
    return '100% Remoto';
  };

  const handleModeloTrabalhoSelect = (modeloTrabalhoId: string) => {
    const opt = workModels.find((m) => m.id === modeloTrabalhoId);
    const transportModel = opt ? descricaoToTransportModel(opt.descricao) : formData.transportModel;
    dispatch(updateFormData({ modeloTrabalhoId: modeloTrabalhoId || undefined, transportModel }));
  };

  const updateForm = (payload: Partial<SimulationData>) => {
    dispatch(updateFormData(payload));
  };

  useEffect(() => {
    if ((formData.educationCost || 0) !== totalEducationCost) {
      dispatch(updateFormData({ educationCost: totalEducationCost }));
    }
  }, [dispatch, formData.educationCost, totalEducationCost]);

  const setChildrenCount = (count: number) => {
    const nextCount = Math.max(0, count);
    const nextBirthDates = [...(formData.childrenBirthDates || [])];
    if (nextCount > nextBirthDates.length) {
      nextBirthDates.push(...Array.from({ length: nextCount - nextBirthDates.length }, () => ''));
    } else if (nextCount < nextBirthDates.length) {
      nextBirthDates.splice(nextCount);
    }
    dispatch(updateFormData({ childrenCount: nextCount, childrenBirthDates: nextBirthDates }));
  };

  useEffect(() => {
    const cep = normalizeCep(formData.zipCode || '');
    if (cep.length !== 8) {
      setCepLookupStatus('idle');
      return;
    }

    const controller = new AbortController();
    const timer = setTimeout(async () => {
      try {
        setCepLookupStatus('loading');
        const cepData = await fetchCepData(cep, controller.signal);
        const formUpdate: Partial<SimulationData> = {
          address: (cepData.logradouro || '').trim(),
          city: (cepData.localidade || '').trim(),
          state: (cepData.uf || '').trim(),
          addressComplement: (cepData.complemento || '').trim(),
        };
        dispatch(updateFormData(formUpdate));
        lastCepAutoFill.current = {
          cep,
          address: formUpdate.address || '',
          city: formUpdate.city || '',
          state: formUpdate.state || '',
          addressComplement: formUpdate.addressComplement || '',
        };
        setCepLookupStatus('success');
      } catch (error) {
        if (isAbortError(error)) return;
        setCepLookupStatus('error');
      }
    }, CEP_LOOKUP_DEBOUNCE_MS);

    return () => {
      clearTimeout(timer);
      controller.abort();
    };
  }, [dispatch, formData.zipCode]);

  /** Custo Hora Vaga e Remuneração (168h) vêm somente da API da vaga (custoProfissional) e não são alterados pelo cálculo. */
  const jobHourlyCost =
    typeof vagaDetails?.custoProfissional === 'number' ? vagaDetails.custoProfissional : 0;

  const handleManagerSelect = (manager: Manager) => {
    trackEvent('select_manager', {
      section: 'Informações da Vaga',
      manager_id: manager.cd_Profissional,
    });
  };

  const updateVariableExpensesCount = (increment: boolean) => {
    const current = formData.variableExpenses || [];
    let newExpenses;
    if (increment) {
      newExpenses = [...current, { id: Date.now().toString(), description: '', value: 0 }];
    } else {
      if (current.length === 0) return;
      newExpenses = [...current];
      newExpenses.pop();
    }
    dispatch(updateFormData({ variableExpenses: newExpenses }));
  };

  const updateVariableExpenseItem = (index: number, field: 'description' | 'value', value: string | number) => {
    const newExpenses = [...formData.variableExpenses];
    newExpenses[index] = { ...newExpenses[index], [field]: value };
    dispatch(updateFormData({ variableExpenses: newExpenses }));
  };

  const handleClearForm = () => {
    dispatch(resetForm());
    trackEvent('click_button', { button: 'Limpar Formulário', section: 'Header' });
  };

  const handleExportPDF = async (type: 'proposed' | 'desired') => {
    logUserAction('Simulador', 'ExportarPDF', { type }, user);
    const pdfViewMode = viewMode === 'collaborator' ? 'collaborator' : 'recruiter';
    if (type === 'proposed') {
      await generatePDF(formData, proposedResults, 'proposed', pdfViewMode);
    } else {
      const desiredData = { ...formData, grossSalary: desiredGross };
      await generatePDF(desiredData, desiredResults, 'desired', pdfViewMode);
    }
  };

  const handleGenerateCalculation = () => {
    if (!effectiveToken) return;
    const pretensaoLiquida = formData.desiredNetSalary ?? 0;
    if (typeof pretensaoLiquida !== 'number' || pretensaoLiquida <= 0) {
      toast.error('Informe a Pretensão Líquida (Ref.) maior que zero para gerar o cálculo.');
      return;
    }
    logUserAction('Simulador', 'GerarCalculo', { vagaId: selectedVagaId, candidaturaId: selectedCandidaturaId }, user);
    const payload: RemuneracaoCalculationPayload = {
      idVaga: selectedVagaId || vagaDetails?.id || '',
      liquidoPretendido: formData.desiredNetSalary || 0,
      quantidadeDependentes: formData.dependentsIRPF || 0,
      alimentacao: formData.foodAllowance || 0,
      mobilidade: proposedResults.mobilityCost || 0,
      educacao: totalEducationCost || 0,
      ajudaDeCusto: formData.desiredAjudaDeCusto ?? 0,
      km: Number(formData.dailyKm) || 0,
    };
    dispatch(calculateRemuneracaoTotal({ token: effectiveToken, payload }))
      .unwrap()
      .then((result) => {
        lastValidCalculationContextRef.current = getCalculationContext();
        setCalculationSuccessVersion((v) => v + 1);
        const mensagem = result?.mensagem?.trim();
        if (mensagem) toast.success(mensagem);
      })
      .catch((err: unknown) => {
        const msg = typeof err === 'string' ? err : (err instanceof Error ? err.message : null);
        toast.error(msg ?? 'Erro ao gerar cálculo.');
      });
  };

  const hasTextValue = (value?: string | null): value is string =>
    typeof value === 'string' && value.trim().length > 0;

  const mergeStringValue = (
    formValue?: string,
    fallback?: string | null | undefined,
  ): string | null => {
    if (hasTextValue(formValue)) {
      return formValue.trim();
    }
    if (hasTextValue(fallback)) {
      return fallback.trim();
    }
    return null;
  };

  const mergeNumberValue = (
    formValue?: number,
    fallback?: number | null | undefined,
  ): number => {
    if (typeof formValue === 'number' && !Number.isNaN(formValue)) {
      return formValue;
    }
    if (typeof fallback === 'number' && !Number.isNaN(fallback)) {
      return fallback;
    }
    return 0;
  };

  const mergeBooleanValue = (
    formValue?: boolean,
    fallback?: boolean | null | undefined,
  ): boolean | null => {
    if (typeof formValue === 'boolean') {
      return formValue;
    }
    if (typeof fallback === 'boolean') {
      return fallback;
    }
    return null;
  };

  const parseNumberFromString = (value?: string | null | undefined): number | null => {
    if (typeof value !== 'string') {
      return null;
    }
    const cleaned = value.replace(/[^\d.-]/g, '');
    const parsed = Number(cleaned);
    return Number.isNaN(parsed) ? null : parsed;
  };

  const buildChildrenPayload = () => {
    const existingChildren = candidaturaDetails?.dadosDemograficos?.filhos ?? [];
    const formChildrenDates =
      (formData.childrenBirthDates || [])
        .map((value) => value?.trim())
        .filter(Boolean) as string[];
    const originalDates = existingChildren
      .map((child) => child.dataNascimento ?? '')
      .filter(Boolean);
    const sourceDates = formChildrenDates.length ? formChildrenDates : originalDates;
    return sourceDates.map((date, index) => ({
      id: existingChildren[index]?.id ?? null,
      dataNascimento: date,
    }));
  };

  const PREFERRED_ONSITE_DAYS_TO_QUANTIDADE: Record<string, number> = {
    '1 dia presencial': 1,
    '2 dias presenciais': 2,
    '3 dias presenciais': 3,
    '4 dias presenciais': 4,
  };

  const buildCandidaturaPayload = (): CandidaturaEditPayload | null => {
    if (!selectedCandidateId || !selectedCandidaturaId) {
      return null;
    }

    const original = candidaturaDetails;
    const demographics = original?.dadosDemograficos;
    const collaboratorOriginal = original?.colaborador;
    const childrenPayload = buildChildrenPayload();
    const mealVoucherValue = MEAL_VOUCHER_BASE + (formData.foodAllowance || 0);
    return {
      colaborador: {
        codigoInternoColaborador: selectedCandidateId,
        nomeCompleto: mergeStringValue(formData.candidateName, collaboratorOriginal?.nomeCompleto),
        dataNascimento: mergeStringValue(formData.birthDate, collaboratorOriginal?.dataNascimento),
        rg: mergeStringValue(formData.rg, collaboratorOriginal?.rg),
        documentoColaborador: mergeStringValue(
          formData.cpf,
          collaboratorOriginal?.documentoColaborador,
        ),
        contatoPrincipal: mergeStringValue(formData.phone, collaboratorOriginal?.contatoPrincipal),
        contatoPrincipalDdi: collaboratorOriginal?.contatoPrincipalDdi ?? null,
        contatoOutro: collaboratorOriginal?.contatoOutro ?? null,
        email: collaboratorOriginal?.email ?? null,
        emailAlternativo: mergeStringValue(formData.personalEmail, collaboratorOriginal?.emailAlternativo),
        matricula: collaboratorOriginal?.matricula ?? null,
        ativo: collaboratorOriginal?.ativo ?? true,
        candidato: collaboratorOriginal?.candidato ?? true,
        passaporte: collaboratorOriginal?.passaporte ?? null,
        estadoCivil: collaboratorOriginal?.estadoCivil ?? null,
        genero: collaboratorOriginal?.genero ?? null,
        etnia: collaboratorOriginal?.etnia ?? null,
        orientacaoSexual: collaboratorOriginal?.orientacaoSexual ?? null,
        escolaridade: collaboratorOriginal?.escolaridade ?? null,
        refugiado: collaboratorOriginal?.refugiado,
        nacionalidade: collaboratorOriginal?.nacionalidade ?? null,
        sobre: collaboratorOriginal?.sobre ?? null,
        urlLinkedin: collaboratorOriginal?.urlLinkedin ?? null,
        dataSyncLinkedin: collaboratorOriginal?.dataSyncLinkedin ?? null,
        visualizarBuscaAderencia: collaboratorOriginal?.visualizarBuscaAderencia,
        qualificado: collaboratorOriginal?.qualificado,
        colaboradorSaudeId: collaboratorOriginal?.colaboradorSaudeId ?? null,
        endereco: {
          cep: formData.zipCode.trim() && isCepComplete(formData.zipCode)
            ? normalizeCep(formData.zipCode)
            : mergeStringValue('', collaboratorOriginal?.endereco?.cep),
          endereco: mergeStringValue(formData.address, collaboratorOriginal?.endereco?.endereco),
          complemento: mergeStringValue(
            formData.addressComplement,
            collaboratorOriginal?.endereco?.complemento,
          ),
          numero:
            parseNumberFromString(formData.addressNumber) ??
            collaboratorOriginal?.endereco?.numero ??
            null,
          bairro: collaboratorOriginal?.endereco?.bairro ?? null,
          cidade: mergeStringValue(formData.city, collaboratorOriginal?.endereco?.cidade),
          estado: mergeStringValue(formData.state, collaboratorOriginal?.endereco?.estado),
          com_quem_mora: collaboratorOriginal?.endereco?.com_quem_mora ?? null,
          internacional_linha_um: collaboratorOriginal?.endereco?.internacional_linha_um ?? null,
          internacional_linha_dois:
            collaboratorOriginal?.endereco?.internacional_linha_dois ?? null,
          id: collaboratorOriginal?.endereco?.id ?? null,
        },
        saude: collaboratorOriginal?.saude ?? null,
      },
      dadosDemograficos: {
        quantidadePessoasResidencia: Math.max(
          1,
          mergeNumberValue(
            formData.residentsCount,
            demographics?.quantidadePessoasResidencia,
          ),
        ),
        dependentesIRPF: mergeNumberValue(formData.dependentsIRPF, demographics?.dependentesIRPF),
        possuiConjuge: mergeBooleanValue(formData.hasSpouse, demographics?.possuiConjuge),
        dataNascimentoConjuge: mergeStringValue(
          formData.spouseBirthDate,
          demographics?.dataNascimentoConjuge,
        ),
        possuiFilhos: mergeBooleanValue(formData.hasChildren, demographics?.possuiFilhos),
        filhos: childrenPayload,
        possuiSeguroSaude: mergeBooleanValue(
          formData.hasHealthInsurance,
          demographics?.possuiSeguroSaude,
        ),
        valorAtualSeguroSaude: mergeNumberValue(
          formData.healthInsuranceCurrentValue,
          demographics?.valorAtualSeguroSaude,
        ),
        operadoraSeguroSaude: mergeStringValue(
          formData.healthInsuranceProvider,
          demographics?.operadoraSeguroSaude,
        ),
        acomodacaoSeguroSaude: mergeStringValue(
          formData.healthInsuranceAccommodation,
          demographics?.acomodacaoSeguroSaude,
        ),
        seguroSaudePossuiCoparticipacao: mergeBooleanValue(
          formData.healthInsuranceHasCopay,
          demographics?.seguroSaudePossuiCoparticipacao,
        ),
        observacoesSeguroSaude: mergeStringValue(
          formData.healthInsuranceNotes,
          demographics?.observacoesSeguroSaude,
        ),
        possuiInteressePlanoFoursys: mergeBooleanValue(
          formData.wantsFoursysPlan,
          demographics?.possuiInteressePlanoFoursys,
        ),
        faixaEtaria: (formData.ageRange?.trim() || demographics?.faixaEtaria) ?? null,
        categoriaPlanoSaude: mergeStringValue(formData.healthPlanRole, demographics?.categoriaPlanoSaude ?? null),
        incluirDependentesPlanoFoursys: mergeBooleanValue(
          formData.wantsFoursysPlanIncludeDependents,
          demographics?.incluirDependentesPlanoFoursys,
        ),
        quantidadeDependentesPlanoFoursys: mergeNumberValue(
          formData.wantsFoursysPlanDependentsCount,
          demographics?.quantidadeDependentesPlanoFoursys,
        ),
        valorPlanoDependentes: mergeNumberValue(
          formData.healthPlanValue,
          demographics?.valorPlanoDependentes,
        ),
        valorCartaoRefeicao: mergeNumberValue(
          mealVoucherValue,
          demographics?.valorCartaoRefeicao,
        ),
        valorCartaoAlimentacao: mergeNumberValue(
          formData.foodAllowance,
          demographics?.valorCartaoAlimentacao,
        ),
        estudaAtualmente: mergeBooleanValue(
          formData.studiesCurrently,
          demographics?.estudaAtualmente,
        ),
        custoMensalEducacao: mergeNumberValue(
          formData.educationMonthlyCost,
          demographics?.custoMensalEducacao,
        ),
        filhosEstudamAte24Anos: mergeBooleanValue(
          formData.childrenStudyUpTo24,
          demographics?.filhosEstudamAte24Anos,
        ),
        custoMensalEducacaoFilhos: mergeNumberValue(
          formData.childrenEducationMonthlyCost,
          demographics?.custoMensalEducacaoFilhos,
        ),
        custoTotalEducacao: totalEducationCost ?? 0,
        modeloDeTrabalhoPretendido: mergeStringValue(
          formData.preferredWorkModel,
          demographics?.modeloDeTrabalhoPretendido,
        ),
        diasPresenciaisDesejados: mergeStringValue(
          formData.preferredOnsiteDays,
          demographics?.diasPresenciaisDesejados,
        ),
        pretencaoLiquidaRef: mergeNumberValue(
          formData.desiredNetSalary,
          demographics?.pretencaoLiquidaRef,
        ),
        distanciaIdaVolta: mergeNumberValue(formData.dailyKm, demographics?.distanciaIdaVolta),
        outrosCustos: (() => {
          const list = formData.variableExpenses ?? [];
          const originalIds = new Set(
            (candidaturaDetails?.dadosDemograficos?.outrosCustos ?? []).map((oc: { id?: string | null }) => oc.id?.trim()).filter(Boolean),
          );
          const validItems = list.filter((exp) => {
            const desc = (exp.description ?? '').trim();
            const valor = typeof exp.value === 'number' ? exp.value : 0;
            return desc !== '' && valor >= 0;
          });
          if (validItems.length === 0) return null;
          return validItems.map((exp) => {
            const isFromApi = exp.id != null && originalIds.has(String(exp.id));
            return {
              id: isFromApi ? exp.id : null,
              descricao: (exp.description ?? '').trim(),
              valor: typeof exp.value === 'number' ? exp.value : 0,
            };
          });
        })(),
      },
      dadosCandidatura: {
        idCandidatura: selectedCandidaturaId,
        modeloTrabalhoId: formData.modeloTrabalhoId ?? null,
        quantidadeDiasPresencial: (() => {
          if (formData.transportModel !== 'Hibrido') return null;
          const fromWorkOnsite =
            formData.workOnsiteDays && PREFERRED_ONSITE_DAYS_TO_QUANTIDADE[formData.workOnsiteDays] != null
              ? PREFERRED_ONSITE_DAYS_TO_QUANTIDADE[formData.workOnsiteDays]
              : null;
          const fromPreferred =
            formData.preferredOnsiteDays && PREFERRED_ONSITE_DAYS_TO_QUANTIDADE[formData.preferredOnsiteDays] != null
              ? PREFERRED_ONSITE_DAYS_TO_QUANTIDADE[formData.preferredOnsiteDays]
              : null;
          return fromWorkOnsite ?? fromPreferred ?? original?.dadosCandidatura?.quantidadeDiasPresencial ?? null;
        })(),
        pretencaoSalarial: mergeNumberValue(
          formData.desiredNetSalary,
          original?.dadosCandidatura?.pretencaoSalarial,
        ),
      },
    };
  };

  const validateCandidaturaPayload = (): string | null => {
    const residents = formData.residentsCount ?? 0;
    if (residents < 0) {
      return 'O número de pessoas com quem mora não pode ser negativo.';
    }
    if (formData.hasSpouse && !formData.spouseBirthDate?.trim()) {
      return 'Data de nascimento do cônjuge é obrigatória quando possui cônjuge.';
    }
    if (formData.hasChildren) {
      const hasValidChild = (formData.childrenBirthDates ?? []).some((d) => d?.trim());
      if (!hasValidChild) {
        return 'Deve informar pelo menos um filho com data de nascimento quando possui filhos.';
      }
    }
    if (formData.wantsFoursysPlanIncludeDependents && (formData.wantsFoursysPlanDependentsCount ?? 0) < 1) {
      return 'A quantidade de dependentes do plano Foursys deve ser no mínimo 1 quando incluir dependentes.';
    }
    const variableExpenses = formData.variableExpenses ?? [];
    for (let i = 0; i < variableExpenses.length; i++) {
      const exp = variableExpenses[i];
      const desc = (exp.description ?? '').trim();
      const valor = typeof exp.value === 'number' ? exp.value : NaN;
      if (desc === '' && valor > 0) {
        return 'Preencha a descrição do custo variável que possui valor informado.';
      }
      if (desc !== '' && (Number.isNaN(valor) || valor < 0)) {
        return `Preencha o valor do custo variável "${desc || 'item ' + (i + 1)}".`;
      }
    }
    return null;
  };

  const handleSaveCandidaturaData = () => {
    if (!effectiveToken) {
      toast.error('Token não disponível. Faça login ou informe o token da API.');
      return;
    }
    if (!selectedCandidateId || !selectedCandidaturaId) {
      toast.error('Selecione a vaga e o candidato/candidatura nos Identificadores para salvar.');
      return;
    }
    if (formData.zipCode.trim()) {
      if (!isCepComplete(formData.zipCode)) {
        setCepFieldError('CEP deve ter 8 dígitos no formato 00000-000.');
        toast.error('CEP do candidato está incompleto. Preencha no formato 00000-000 (ex.: 01310-000).');
        return;
      }
      setCepFieldError(null);
    }
    dispatch(setSavingCandidaturaError(null));
    const validationError = validateCandidaturaPayload();
    if (validationError) {
      toast.error(validationError);
      return;
    }
    const payload = buildCandidaturaPayload();
    if (!payload) {
      toast.error('Não foi possível montar os dados. Verifique se vaga e candidatura estão selecionados.');
      return;
    }
    dispatch(saveCandidaturaData({ token: effectiveToken, payload }))
      .unwrap()
      .then((result) => {
        logUserAction('Simulador', 'SalvarCandidatura', { vagaId: selectedVagaId, candidaturaId: selectedCandidaturaId }, user);
        const mensagem = result?.mensagem?.trim();
        if (mensagem) toast.success(mensagem);
      })
      .catch((err: unknown) => {
        const msg = typeof err === 'string' ? err : (err instanceof Error ? err.message : null);
        toast.error(msg ?? 'Erro ao salvar candidatura.');
      });
  };

  const handleLoadVagaDetails = (vagaId: string) => {
    const trimmed = vagaId.trim();
    if (!trimmed || !effectiveToken) return;
    dispatch(setSelectedVagaId(trimmed));
    dispatch(setSelectedCandidateId(''));
    dispatch(setSelectedCandidaturaId(''));
    dispatch(fetchVagaDetails({ token: effectiveToken, vagaId: trimmed }))
      .unwrap()
      .catch((err: unknown) => {
        const msg = err instanceof Error ? err.message : null;
        toast.error(msg ?? 'Erro ao carregar vaga.');
      });
  };

  const handleLoadCandidateDetails = (candidateId: string) => {
    const trimmed = candidateId.trim();
    if (!trimmed || !effectiveToken) return;
    dispatch(fetchCandidateDetails({ token: effectiveToken, candidateId: trimmed }))
      .unwrap()
      .catch((err: unknown) => {
        const msg = err instanceof Error ? err.message : null;
        toast.error(msg ?? 'Erro ao carregar candidato.');
      });
    dispatch(setSelectedCandidateId(trimmed));
  };

  const handleLoadCandidatura = (candidaturaId: string, candidateId?: string) => {
    const trimmedCandidatura = candidaturaId.trim();
    const candidate = (candidateId && candidateId.trim()) || selectedCandidateId;
    if (!trimmedCandidatura || !candidate || !effectiveToken) return;
    dispatch(setSelectedCandidaturaId(trimmedCandidatura));
    if (candidate) dispatch(setSelectedCandidateId(candidate));
    dispatch(
      fetchCandidaturaDetails({
        token: effectiveToken,
        candidateId: candidate,
        candidaturaId: trimmedCandidatura,
      }),
    )
      .unwrap()
      .catch((err: unknown) => {
        const msg = err instanceof Error ? err.message : null;
        toast.error(msg ?? 'Erro ao carregar candidatura.');
      });
  };

  const handleSelectVaga = (vagaId: string) => {
    logUserAction('Simulador', 'SelecionarVaga', { vagaId }, user);
    handleLoadVagaDetails(vagaId);
  };

  const handleSelectCandidato = (payload: { candidaturaId: string; candidateId: string }) => {
    const trimmedCandidato = payload.candidateId.trim();
    const trimmedCandidatura = payload.candidaturaId.trim();
    if (!trimmedCandidatura || !trimmedCandidato) return;
    logUserAction('Simulador', 'SelecionarCandidato', { candidaturaId: trimmedCandidatura, candidateId: trimmedCandidato }, user);
    handleLoadCandidatura(trimmedCandidatura, trimmedCandidato);
  };

  const handleSaveToken = () => {
    dispatch(setApiToken(tempToken));
    setShowTokenModal(false);
  };

  const isCompliant =
    calculationResult?.retorno?.emConformidadeComAPolitica ??
    (proposedResults.netSalary >= formData.desiredNetSalary);

  return {
    formData,
    managers,
    loadingManagers,
    apiToken,
    viewMode,
    setViewMode,
    handleInputChange,
    proposedResults,
    desiredResults,
    desiredGross,
    planDependentsCount,
    planDependentsUnit,
    planDependentsTotal,
    totalEducationCost,
    handleManagerSelect,
    updateVariableExpensesCount,
    updateVariableExpenseItem,
    handleClearForm,
    handleExportPDF,
    handleGenerateCalculation,
    handleSaveCandidaturaData,
    savingCandidaturaData,
    isCompliant,
    updateForm,
    showDetails,
    setShowDetails,
    showTokenModal,
    setShowTokenModal,
    tempToken,
    setTempToken,
    handleSaveToken,
    handleLoadVagaDetails,
    handleLoadCandidateDetails,
    handleLoadCandidatura,
    handleSelectVaga,
    handleSelectCandidato,
    vagasList,
    candidatosList,
    loadingVagasList,
    loadingCandidatosList,
    selectedVagaId,
    selectedCandidateId,
    selectedCandidaturaId,
    jobHourlyCost,
    loadingVagaDetails,
    loadingCandidateDetails,
    loadingCandidaturaDetails,
    calculationLoading,
    calculationResult,
    calculationValidations,
    vagaDetails,
    candidateDetails,
    candidaturaDetails,
    cepLookupStatus,
    cepFieldError,
    setChildrenCount,
    showCompanyCost,
    setShowCompanyCost,
    formatCurrency,
    formatCep,
    formatCepForInput,
    MOBILITY_RATE_PER_KM,
    HOURS_PER_MONTH,
    MEAL_VOUCHER_BASE,
    AGE_RANGES,
    HEALTH_PLANS,
    workModels,
    loadingModelosTrabalho,
    handleModeloTrabalhoSelect,
    /** Habilita o botão Detalhes apenas após "Gerar Cálculo" e sem alteração de vaga/candidato/campos de cálculo. */
    detailsEnabled:
      Boolean(calculationResult) &&
      lastValidCalculationContextRef.current !== null &&
      lastValidCalculationContextRef.current === getCalculationContext() &&
      calculationSuccessVersion >= 0,
  };
};
