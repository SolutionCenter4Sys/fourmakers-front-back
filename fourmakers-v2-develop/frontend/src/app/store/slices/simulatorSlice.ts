import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import { container } from '@core/di/container';
import { GetManagersUseCase } from '@domain/usecases/GetManagersUseCase';
import { GetUserProfileUseCase } from '@domain/usecases/GetUserProfileUseCase';
import { GetCandidateDetailsUseCase } from '@domain/usecases/GetCandidateDetailsUseCase';
import { GetVagaDetalhesUseCase } from '@domain/usecases/GetVagaDetalhesUseCase';
import { GetCandidaturaDetailsUseCase } from '@domain/usecases/GetCandidaturaDetailsUseCase';
import { SimulateRemuneracaoTotalUseCase } from '@domain/usecases/SimulateRemuneracaoTotalUseCase';
import { ListVagasUseCase } from '@domain/usecases/ListVagasUseCase';
import { ListCandidatosUseCase } from '@domain/usecases/ListCandidatosUseCase';
import { SaveCandidaturaDetailsUseCase } from '@domain/usecases/SaveCandidaturaDetailsUseCase';
import type { SimulationData, Manager, UserProfile } from '@domain/entities/SimulatorTypes';
import type { CandidateDetails } from '@domain/entities/CandidateDetails';
import type { VagaDetails } from '@domain/entities/VagaDetails';
import type { VagaListItem } from '@domain/entities/VagaListItem';
import type { CandidatoListItem } from '@domain/entities/CandidatoListItem';
import { EMPTY_FORM_STATE } from '@domain/entities/SimulatorTypes';
import type {
  RemuneracaoCalculationPayload,
  RemuneracaoCalculationResponse,
  RemuneracaoValidation,
  ValidationFieldName,
} from '@domain/entities/RemuneracaoCalculation';
import type { CandidaturaDetails, CandidaturaEditPayload } from '@domain/entities/CandidaturaDetails';
import {
  formatCep,
  HOURS_PER_MONTH,
  MOBILITY_RATE_PER_KM,
  WORKING_DAYS_PER_MONTH,
} from '@shared/utils/calculations';

// Async Thunks
export const fetchManagers = createAsyncThunk(
  'simulator/fetchManagers',
  async (params: { token: string; search: string }) => {
    const useCase = container.resolve(GetManagersUseCase);
    return await useCase.execute(params.token, params.search);
  }
);

export const fetchUserProfile = createAsyncThunk(
  'simulator/fetchUserProfile',
  async (token: string) => {
    const useCase = container.resolve(GetUserProfileUseCase);
    return await useCase.execute(token);
  }
);

export const fetchVagaDetails = createAsyncThunk(
  'simulator/fetchVagaDetails',
  async (params: { token: string; vagaId: string }) => {
    const useCase = container.resolve(GetVagaDetalhesUseCase);
    return await useCase.execute(params.token, params.vagaId);
  }
);

export const fetchCandidateDetails = createAsyncThunk(
  'simulator/fetchCandidateDetails',
  async (params: { token: string; candidateId: string }) => {
    const useCase = container.resolve(GetCandidateDetailsUseCase);
    return await useCase.execute(params.token, params.candidateId);
  }
);

export const fetchVagasList = createAsyncThunk(
  'simulator/fetchVagasList',
  async (token: string) => {
    const useCase = container.resolve(ListVagasUseCase);
    return await useCase.execute(token);
  },
);

export const fetchCandidatosList = createAsyncThunk(
  'simulator/fetchCandidatosList',
  async (params: { token: string; vagaId: string }) => {
    const useCase = container.resolve(ListCandidatosUseCase);
    return await useCase.execute(params.token, params.vagaId);
  },
);

export const calculateRemuneracaoTotal = createAsyncThunk(
  'simulator/calculateRemuneracaoTotal',
  async (
    params: { token: string; payload: RemuneracaoCalculationPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(SimulateRemuneracaoTotalUseCase);
      const result = await useCase.execute(params.token, params.payload);
      if (result && result.sucesso === false) {
        return rejectWithValue(result.mensagem ?? 'Erro ao gerar cálculo.');
      }
      return result;
    } catch (err: unknown) {
      const message =
        err instanceof Error ? err.message : 'Erro ao gerar cálculo.';
      return rejectWithValue(message);
    }
  },
);

export const fetchCandidaturaDetails = createAsyncThunk(
  'simulator/fetchCandidaturaDetails',
  async (params: { token: string; candidateId: string; candidaturaId?: string }) => {
    const useCase = container.resolve(GetCandidaturaDetailsUseCase);
    return await useCase.execute(params.token, params.candidateId, params.candidaturaId);
  },
);

export const saveCandidaturaData = createAsyncThunk(
  'simulator/saveCandidaturaData',
  async (
    params: { token: string; payload: CandidaturaEditPayload },
    { rejectWithValue },
  ) => {
    try {
      const useCase = container.resolve(SaveCandidaturaDetailsUseCase);
      const result = await useCase.execute(params.token, params.payload);
      return result ?? { sucesso: true };
    } catch (err: unknown) {
      const message =
        err && typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: string }).message)
          : 'Erro ao salvar candidatura.';
      return rejectWithValue(message);
    }
  },
);

interface SimulatorState {
  formData: SimulationData;
  managers: Manager[];
  loadingManagers: boolean;
  userProfile: UserProfile | null;
  apiToken: string;
  vagaDetails: VagaDetails | null;
  candidateDetails: CandidateDetails | null;
  loadingVagaDetails: boolean;
  loadingCandidateDetails: boolean;
  calculationLoading: boolean;
  calculationResult: RemuneracaoCalculationResponse | null;
  calculationValidations: CalculationValidationMap;
  selectedVagaId: string;
  selectedCandidateId: string;
  selectedCandidaturaId: string;
  candidaturaDetails: CandidaturaDetails | null;
  loadingCandidaturaDetails: boolean;
  vagasList: VagaListItem[];
  candidatosList: CandidatoListItem[];
  loadingVagasList: boolean;
  loadingCandidatosList: boolean;
  savingCandidaturaData: boolean;
  savingCandidaturaError: string | null;
}

const DEFAULT_API_TOKEN = import.meta.env.VITE_SIMULATOR_API_TOKEN || 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJFbWFpbCI6ImRvdWdsYXMuZ29tZXNAZm91cnN5cy5jb20uYnIiLCJDcGYiOiJmZTFjMDA3Mi1jZGY1LTRhYWQtOGIzZi1kYzIyZjlmMTcyODkiLCJDb2RDb2xhYm9yYWRvciI6IiIsIk9yZ0lkIjoiMiIsIkxvZ2luVHlwZSI6IjAiLCJuYmYiOjE3NjUyMDY0MDUsImV4cCI6MTc2NTYzODQwNSwiaWF0IjoxNzY1MjA2NDA1fQ.RuQnqJDHy5LUq3Wh4j1p_mE_UIExk1rKIHptiaQhNIw';

const normalizeTransportModel = (value?: string | null): SimulationData['transportModel'] => {
  if (!value) {
    return '100% Remoto';
  }

  const normalized = value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase();

  if (normalized.includes('presencial') && !normalized.includes('hibrido')) {
    return 'Presencial';
  }

  if (normalized.includes('hibrido')) {
    return 'Hibrido';
  }

  return normalized.includes('remoto') ? '100% Remoto' : '100% Remoto';
};

const FREQUENCIA_TO_WORK_ONSITE_DAYS: Record<string, SimulationData['workOnsiteDays']> = {
  '1': '1 dia presencial',
  '2': '2 dias presenciais',
  '3': '3 dias presenciais',
  '4': '4 dias presenciais',
};

const mapFrequenciaToWorkOnsiteDays = (frequencia?: string | number | null): SimulationData['workOnsiteDays'] =>
  (frequencia != null && FREQUENCIA_TO_WORK_ONSITE_DAYS[String(frequencia)]) || '';

const QUANTIDADE_DIAS_TO_ONSITE_DAYS: Record<number, SimulationData['workOnsiteDays']> = {
  1: '1 dia presencial',
  2: '2 dias presenciais',
  3: '3 dias presenciais',
  4: '4 dias presenciais',
};

const mapQuantidadeDiasToOnsiteDays = (qty?: number | null): SimulationData['workOnsiteDays'] =>
  (qty != null && QUANTIDADE_DIAS_TO_ONSITE_DAYS[qty]) || '';

const VALID_AGE_RANGES: SimulationData['ageRange'][] = [
  '0-18', '19-23', '24-28', '29-33', '34-38', '39-43', '44-48', '49-53', '54-58', '59+',
];

export type CalculationValidationLevel = 'success' | 'error';

export interface CalculationValidationInfo {
  level: CalculationValidationLevel;
  message: string;
}

export type CalculationValidationMap = Record<ValidationFieldName, CalculationValidationInfo | null>;

const INITIAL_VALIDATIONS: CalculationValidationMap = {
  clt: null,
  valeRefeicao: null,
  valeAlimentacao: null,
  auxilioEducacao: null,
  mobilidade: null,
  ajudaDeCusto: null,
};

const mapValidation = (validation?: RemuneracaoValidation | null): CalculationValidationInfo | null => {
  if (!validation?.mensagem) {
    return null;
  }
  return {
    level: validation.dentroDaPolitica ? 'success' : 'error',
    message: validation.mensagem,
  };
};

const initialState: SimulatorState = {
  formData: { ...EMPTY_FORM_STATE },
  managers: [],
  loadingManagers: false,
  userProfile: null,
  apiToken: localStorage.getItem('fourmakers_api_token') || DEFAULT_API_TOKEN,
  vagaDetails: null,
  candidateDetails: null,
  loadingVagaDetails: false,
  loadingCandidateDetails: false,
  calculationLoading: false,
  calculationResult: null,
  calculationValidations: INITIAL_VALIDATIONS,
  selectedVagaId: '',
  selectedCandidateId: '',
  selectedCandidaturaId: '',
  candidaturaDetails: null,
  loadingCandidaturaDetails: false,
  vagasList: [],
  candidatosList: [],
  loadingVagasList: false,
  loadingCandidatosList: false,
  savingCandidaturaData: false,
  savingCandidaturaError: null,
};

const simulatorSlice = createSlice({
  name: 'simulator',
  initialState,
  reducers: {
    updateFormData: (state, action: PayloadAction<Partial<SimulationData>>) => {
      state.formData = { ...state.formData, ...action.payload };
    },
    setApiToken: (state, action: PayloadAction<string>) => {
      state.apiToken = action.payload;
      localStorage.setItem('fourmakers_api_token', action.payload);
    },
    resetForm: (state) => {
      state.formData = { ...EMPTY_FORM_STATE };
      state.selectedVagaId = '';
      state.selectedCandidateId = '';
      state.selectedCandidaturaId = '';
      state.vagaDetails = null;
      state.candidateDetails = null;
      state.candidaturaDetails = null;
    },
    setSelectedVagaId: (state, action: PayloadAction<string>) => {
      state.selectedVagaId = action.payload;
      state.calculationResult = null;
      state.calculationValidations = INITIAL_VALIDATIONS;
    },
    setSelectedCandidateId: (state, action: PayloadAction<string>) => {
      state.selectedCandidateId = action.payload;
      state.calculationResult = null;
      state.calculationValidations = INITIAL_VALIDATIONS;
    },
    setSelectedCandidaturaId: (state, action: PayloadAction<string>) => {
      state.selectedCandidaturaId = action.payload;
      state.calculationResult = null;
      state.calculationValidations = INITIAL_VALIDATIONS;
    },
    setSavingCandidaturaError: (state, action: PayloadAction<string | null>) => {
      state.savingCandidaturaError = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchManagers.pending, (state) => {
        state.loadingManagers = true;
      })
      .addCase(fetchManagers.fulfilled, (state, action) => {
        state.loadingManagers = false;
        state.managers = action.payload;
      })
      .addCase(fetchManagers.rejected, (state) => {
        state.loadingManagers = false;
      })
      .addCase(fetchUserProfile.fulfilled, (state, action) => {
        state.userProfile = action.payload;
      })
      .addCase(fetchUserProfile.rejected, (state) => {
        state.userProfile = null;
      })
      .addCase(fetchVagaDetails.pending, (state) => {
        state.loadingVagaDetails = true;
      })
      .addCase(fetchVagaDetails.fulfilled, (state, action) => {
        state.loadingVagaDetails = false;
        state.vagaDetails = action.payload;
        state.selectedVagaId = action.payload.id || '';
        // Só limpa candidato/candidatura/form quando não há seleção (usuário trocou de vaga no dropdown).
        // Quando veio da URL com os 3 params, selectedCandidateId/selectedCandidaturaId já estão setados e não devem ser apagados.
        if (!state.selectedCandidateId && !state.selectedCandidaturaId) {
          state.candidateDetails = null;
          state.candidaturaDetails = null;
          state.formData = { ...EMPTY_FORM_STATE };
        }
        const hourlyCost =
          typeof action.payload.custoProfissional === 'number' ? action.payload.custoProfissional : undefined;
        const transportModel = normalizeTransportModel(action.payload.modeloTrabalhoDescricao);
        const workOnsiteDays = mapFrequenciaToWorkOnsiteDays(action.payload.frequencia);
        state.formData = {
          ...state.formData,
          role: action.payload.titulo ?? '',
          manager: action.payload.nomeGestor ?? '',
          client: action.payload.nomeCliente ?? action.payload.codigoCliente ?? '',
          transportModel,
          preferredWorkModel: transportModel,
          workOnsiteDays,
          preferredOnsiteDays: workOnsiteDays || state.formData.preferredOnsiteDays || '',
          modeloTrabalhoId: action.payload.modeloTrabalhoId ?? state.formData.modeloTrabalhoId ?? undefined,
          grossSalary: hourlyCost !== undefined ? hourlyCost * HOURS_PER_MONTH : 0,
        };
      })
      .addCase(fetchVagaDetails.rejected, (state) => {
        state.loadingVagaDetails = false;
      })
      .addCase(fetchCandidateDetails.pending, (state) => {
        state.loadingCandidateDetails = true;
      })
      .addCase(fetchCandidateDetails.fulfilled, (state, action) => {
        state.loadingCandidateDetails = false;
        state.candidateDetails = action.payload;
        const birthDate =
          action.payload.dataNascimento?.split('T')[0] ?? state.formData.birthDate;
        state.formData = {
          ...state.formData,
          candidateName: action.payload.nomeCompleto || state.formData.candidateName,
          personalEmail: action.payload.email ?? state.formData.personalEmail,
          cpf: action.payload.cpf || state.formData.cpf,
          rg: action.payload.rg ?? state.formData.rg,
          phone: action.payload.contatoPrincipal ?? state.formData.phone,
          birthDate,
          zipCode:
            (action.payload.endereco?.cep && formatCep(action.payload.endereco.cep)) ||
            state.formData.zipCode,
          address: action.payload.endereco?.endereco ?? state.formData.address,
          addressNumber:
            action.payload.endereco?.numero != null
              ? String(action.payload.endereco.numero)
              : state.formData.addressNumber,
          addressComplement:
            action.payload.endereco?.complemento ?? state.formData.addressComplement,
          city: action.payload.endereco?.cidade ?? state.formData.city,
          state: action.payload.endereco?.estado ?? state.formData.state,
        };
      })
      .addCase(fetchCandidateDetails.rejected, (state) => {
        state.loadingCandidateDetails = false;
      })
      .addCase(fetchVagasList.pending, (state) => {
        state.loadingVagasList = true;
      })
      .addCase(fetchVagasList.fulfilled, (state, action) => {
        state.loadingVagasList = false;
        state.vagasList = action.payload;
      })
      .addCase(fetchVagasList.rejected, (state) => {
        state.loadingVagasList = false;
      })
      .addCase(fetchCandidatosList.pending, (state) => {
        state.loadingCandidatosList = true;
        state.candidatosList = [];
      })
      .addCase(fetchCandidatosList.fulfilled, (state, action) => {
        state.loadingCandidatosList = false;
        state.candidatosList = action.payload;
      })
      .addCase(fetchCandidatosList.rejected, (state) => {
        state.loadingCandidatosList = false;
      })
      .addCase(fetchCandidaturaDetails.pending, (state) => {
        state.loadingCandidaturaDetails = true;
        state.formData = {
          ...state.formData,
          candidateName: EMPTY_FORM_STATE.candidateName,
          personalEmail: EMPTY_FORM_STATE.personalEmail,
          cpf: EMPTY_FORM_STATE.cpf,
          rg: EMPTY_FORM_STATE.rg,
          phone: EMPTY_FORM_STATE.phone,
          birthDate: EMPTY_FORM_STATE.birthDate,
          zipCode: EMPTY_FORM_STATE.zipCode,
          address: EMPTY_FORM_STATE.address,
          addressNumber: EMPTY_FORM_STATE.addressNumber,
          addressComplement: EMPTY_FORM_STATE.addressComplement,
          city: EMPTY_FORM_STATE.city,
          state: EMPTY_FORM_STATE.state,
          residentsCount: EMPTY_FORM_STATE.residentsCount,
          dependentsIRPF: EMPTY_FORM_STATE.dependentsIRPF,
          hasSpouse: EMPTY_FORM_STATE.hasSpouse,
          spouseBirthDate: EMPTY_FORM_STATE.spouseBirthDate,
          hasChildren: EMPTY_FORM_STATE.hasChildren,
          childrenCount: EMPTY_FORM_STATE.childrenCount,
          childrenBirthDates: EMPTY_FORM_STATE.childrenBirthDates,
          hasHealthInsurance: EMPTY_FORM_STATE.hasHealthInsurance,
          healthInsuranceCurrentValue: EMPTY_FORM_STATE.healthInsuranceCurrentValue,
          healthInsuranceProvider: EMPTY_FORM_STATE.healthInsuranceProvider,
          healthInsuranceAccommodation: EMPTY_FORM_STATE.healthInsuranceAccommodation,
          healthInsuranceHasCopay: EMPTY_FORM_STATE.healthInsuranceHasCopay,
          healthInsuranceNotes: EMPTY_FORM_STATE.healthInsuranceNotes,
          wantsFoursysPlan: EMPTY_FORM_STATE.wantsFoursysPlan,
          wantsFoursysPlanIncludeDependents: EMPTY_FORM_STATE.wantsFoursysPlanIncludeDependents,
          wantsFoursysPlanDependentsCount: EMPTY_FORM_STATE.wantsFoursysPlanDependentsCount,
          studiesCurrently: EMPTY_FORM_STATE.studiesCurrently,
          educationMonthlyCost: EMPTY_FORM_STATE.educationMonthlyCost,
          childrenStudyUpTo24: EMPTY_FORM_STATE.childrenStudyUpTo24,
          childrenEducationMonthlyCost: EMPTY_FORM_STATE.childrenEducationMonthlyCost,
          educationCost: EMPTY_FORM_STATE.educationCost,
          foodAllowance: EMPTY_FORM_STATE.foodAllowance,
          preferredOnsiteDays: EMPTY_FORM_STATE.preferredOnsiteDays,
          desiredNetSalary: EMPTY_FORM_STATE.desiredNetSalary,
          desiredAjudaDeCusto: EMPTY_FORM_STATE.desiredAjudaDeCusto,
          dailyKm: EMPTY_FORM_STATE.dailyKm,
        };
      })
      .addCase(fetchCandidaturaDetails.fulfilled, (state, action) => {
        state.loadingCandidaturaDetails = false;
        state.candidaturaDetails = action.payload;
        const colaborador = action.payload.colaborador;
        const demographics = action.payload.dadosDemograficos;
        const responseChildrenDates =
          (demographics?.filhos ?? [])
            .map((filho) => filho.dataNascimento?.trim())
            .filter(Boolean) as string[];
        const accommodationValue = demographics?.acomodacaoSeguroSaude;
        const normalizedAccommodation =
          accommodationValue === 'Apartamento' || accommodationValue === 'Enfermaria'
            ? accommodationValue
            : EMPTY_FORM_STATE.healthInsuranceAccommodation;
        const onsiteDaysValue = demographics?.diasPresenciaisDesejados;
        const allowedOnsiteDays: Array<'1 dia presencial' | '2 dias presenciais' | '3 dias presenciais' | '4 dias presenciais' | ''> = [
          '1 dia presencial',
          '2 dias presenciais',
          '3 dias presenciais',
          '4 dias presenciais',
          '',
        ];
        const preferredFromDemographics =
          onsiteDaysValue && allowedOnsiteDays.includes(onsiteDaysValue as never)
            ? (onsiteDaysValue as '1 dia presencial' | '2 dias presenciais' | '3 dias presenciais' | '4 dias presenciais' | '')
            : '';
        const preferredOnsiteDays =
          preferredFromDemographics ||
          mapQuantidadeDiasToOnsiteDays(action.payload.dadosCandidatura?.quantidadeDiasPresencial) ||
          EMPTY_FORM_STATE.preferredOnsiteDays;
        const variableExpensesFromApi = (demographics?.outrosCustos ?? []).map((item) => ({
          id: item.id?.trim() || `oc-${Date.now()}-${Math.random().toString(36).slice(2)}`,
          description: item.descricao?.trim() ?? '',
          value: typeof item.valor === 'number' ? item.valor : 0,
        }));
        const variableExpenses =
          variableExpensesFromApi.length > 0
            ? variableExpensesFromApi
            : EMPTY_FORM_STATE.variableExpenses;
        const faixaEtariaValue = demographics?.faixaEtaria?.trim();
        const ageRangeFromApi =
          faixaEtariaValue && (VALID_AGE_RANGES as string[]).includes(faixaEtariaValue)
            ? (faixaEtariaValue as SimulationData['ageRange'])
            : EMPTY_FORM_STATE.ageRange;
        state.formData = {
          ...state.formData,
          candidateName: colaborador.nomeCompleto ?? EMPTY_FORM_STATE.candidateName,
          personalEmail:
            colaborador.emailAlternativo?.trim() ||
            EMPTY_FORM_STATE.personalEmail,
          cpf: colaborador.documentoColaborador?.trim() ?? '',
          rg: colaborador.rg ?? EMPTY_FORM_STATE.rg,
          phone: colaborador.contatoPrincipal ?? EMPTY_FORM_STATE.phone,
          birthDate: colaborador.dataNascimento?.split('T')[0] ?? EMPTY_FORM_STATE.birthDate,
          zipCode:
            (colaborador.endereco?.cep && formatCep(colaborador.endereco.cep)) ||
            EMPTY_FORM_STATE.zipCode,
          address: colaborador.endereco?.endereco ?? EMPTY_FORM_STATE.address,
          addressNumber:
            colaborador.endereco?.numero != null
              ? String(colaborador.endereco.numero)
              : EMPTY_FORM_STATE.addressNumber,
          addressComplement:
            colaborador.endereco?.complemento ?? EMPTY_FORM_STATE.addressComplement,
          city: colaborador.endereco?.cidade ?? EMPTY_FORM_STATE.city,
          state: colaborador.endereco?.estado ?? EMPTY_FORM_STATE.state,
          residentsCount: demographics?.quantidadePessoasResidencia ?? EMPTY_FORM_STATE.residentsCount,
          dependentsIRPF: demographics?.dependentesIRPF ?? EMPTY_FORM_STATE.dependentsIRPF,
          hasSpouse: demographics?.possuiConjuge ?? EMPTY_FORM_STATE.hasSpouse,
          spouseBirthDate: demographics?.dataNascimentoConjuge ?? EMPTY_FORM_STATE.spouseBirthDate,
          hasChildren: demographics?.possuiFilhos ?? EMPTY_FORM_STATE.hasChildren,
          childrenCount: responseChildrenDates.length,
          childrenBirthDates: responseChildrenDates,
          hasHealthInsurance: demographics?.possuiSeguroSaude ?? EMPTY_FORM_STATE.hasHealthInsurance,
          healthInsuranceCurrentValue:
            demographics?.valorAtualSeguroSaude ?? EMPTY_FORM_STATE.healthInsuranceCurrentValue,
          healthInsuranceProvider:
            demographics?.operadoraSeguroSaude ?? EMPTY_FORM_STATE.healthInsuranceProvider,
          healthInsuranceAccommodation: normalizedAccommodation,
          healthInsuranceHasCopay:
            demographics?.seguroSaudePossuiCoparticipacao ?? EMPTY_FORM_STATE.healthInsuranceHasCopay,
          healthInsuranceNotes:
            demographics?.observacoesSeguroSaude ?? EMPTY_FORM_STATE.healthInsuranceNotes,
          wantsFoursysPlan:
            demographics?.possuiInteressePlanoFoursys ?? EMPTY_FORM_STATE.wantsFoursysPlan,
          wantsFoursysPlanIncludeDependents:
            demographics?.incluirDependentesPlanoFoursys ??
            EMPTY_FORM_STATE.wantsFoursysPlanIncludeDependents,
          wantsFoursysPlanDependentsCount:
            demographics?.quantidadeDependentesPlanoFoursys ??
            EMPTY_FORM_STATE.wantsFoursysPlanDependentsCount,
          studiesCurrently: (() => {
            const estuda = demographics?.estudaAtualmente;
            if (estuda != null) return estuda;
            const total = demographics?.custoTotalEducacao;
            const custo = demographics?.custoMensalEducacao;
            if (total != null && typeof total === 'number' && total > 0 && (custo == null || custo === 0))
              return true;
            return EMPTY_FORM_STATE.studiesCurrently;
          })(),
          educationMonthlyCost: (() => {
            const custo = demographics?.custoMensalEducacao;
            const total = demographics?.custoTotalEducacao;
            if (custo != null && typeof custo === 'number') return custo;
            if (total != null && typeof total === 'number' && total > 0) return total;
            return EMPTY_FORM_STATE.educationMonthlyCost;
          })(),
          childrenStudyUpTo24:
            demographics?.filhosEstudamAte24Anos ?? EMPTY_FORM_STATE.childrenStudyUpTo24,
          childrenEducationMonthlyCost:
            demographics?.custoMensalEducacaoFilhos ?? EMPTY_FORM_STATE.childrenEducationMonthlyCost,
          educationCost: demographics?.custoTotalEducacao ?? EMPTY_FORM_STATE.educationCost,
          foodAllowance:
            demographics?.valorCartaoAlimentacao ?? EMPTY_FORM_STATE.foodAllowance,
          preferredWorkModel:
            demographics?.modeloDeTrabalhoPretendido
              ? normalizeTransportModel(demographics.modeloDeTrabalhoPretendido)
              : state.formData.preferredWorkModel,
          preferredOnsiteDays,
          modeloTrabalhoId:
            action.payload.dadosCandidatura?.modeloTrabalhoId ??
            state.formData.modeloTrabalhoId ??
            undefined,
          ageRange: ageRangeFromApi,
          desiredNetSalary:
            action.payload.dadosCandidatura?.pretencaoSalarial ??
            demographics?.pretencaoLiquidaRef ??
            EMPTY_FORM_STATE.desiredNetSalary,
          desiredAjudaDeCusto:
            action.payload.dadosCandidatura?.ajudaDeCusto ??
            EMPTY_FORM_STATE.desiredAjudaDeCusto,
          dailyKm: demographics?.distanciaIdaVolta ?? EMPTY_FORM_STATE.dailyKm,
          variableExpenses,
          workZipCode:
            (() => {
              const raw = action.payload.dadosCandidatura?.cep;
              if (raw == null || raw === '' || String(raw).toLowerCase() === 'null') return EMPTY_FORM_STATE.workZipCode;
              return formatCep(String(raw).trim());
            })(),
        };
      })
      .addCase(fetchCandidaturaDetails.rejected, (state) => {
        state.loadingCandidaturaDetails = false;
      })
      .addCase(saveCandidaturaData.pending, (state) => {
        state.savingCandidaturaData = true;
        state.savingCandidaturaError = null;
      })
      .addCase(saveCandidaturaData.fulfilled, (state) => {
        state.savingCandidaturaData = false;
        state.savingCandidaturaError = null;
      })
      .addCase(saveCandidaturaData.rejected, (state) => {
        state.savingCandidaturaData = false;
        state.savingCandidaturaError = null;
      })
      .addCase(calculateRemuneracaoTotal.pending, (state) => {
        state.calculationLoading = true;
        state.calculationValidations = INITIAL_VALIDATIONS;
      })
      .addCase(calculateRemuneracaoTotal.fulfilled, (state, action) => {
        state.calculationLoading = false;
        state.calculationResult = action.payload;
        const desired = action.payload?.retorno?.remuneracaoPretendida;

        if (desired?.clt?.salarioBruto != null) {
          state.formData.grossSalary = desired.clt.salarioBruto;
        }

        if (typeof desired?.valeAlimentacao === 'number') {
          state.formData.foodAllowance = desired.valeAlimentacao;
        }

        if (typeof desired?.auxilioEducacao === 'number') {
          state.formData.educationCost = desired.auxilioEducacao;
        }

        if (typeof desired?.mobilidade === 'number') {
          const divisor = MOBILITY_RATE_PER_KM * WORKING_DAYS_PER_MONTH;
          if (divisor > 0) {
            const derivedKm = desired.mobilidade / divisor;
            if (!Number.isNaN(derivedKm)) {
              state.formData.dailyKm = Number(derivedKm.toFixed(2));
            }
          }
        }

        if (typeof desired?.ajudaDeCusto === 'number') {
          state.formData.desiredAjudaDeCusto = desired.ajudaDeCusto;
        }

        state.calculationValidations = {
          clt: mapValidation(desired?.validacaoCLT),
          valeRefeicao: mapValidation(desired?.validacaoValeRefeicao),
          valeAlimentacao: mapValidation(desired?.validacaoValeAlimentacao),
          auxilioEducacao: mapValidation(desired?.validacaoAuxilioEducacao),
          mobilidade: mapValidation(desired?.validacaoMobilidade),
          ajudaDeCusto: mapValidation(desired?.validacaoAjudaDeCusto),
        };
      })
      .addCase(calculateRemuneracaoTotal.rejected, (state) => {
        state.calculationLoading = false;
        state.calculationValidations = INITIAL_VALIDATIONS;
      });
  },
});

export const {
  updateFormData,
  setApiToken,
  resetForm,
  setSelectedVagaId,
  setSelectedCandidateId,
  setSelectedCandidaturaId,
  setSavingCandidaturaError,
} = simulatorSlice.actions;
export default simulatorSlice.reducer;
