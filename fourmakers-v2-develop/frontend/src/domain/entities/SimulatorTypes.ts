
export interface VariableExpense {
  id: string;
  description: string;
  value: number;
}

export type VacationModel = '30_dias' | '20_dias_abono' | '15_dias';

export interface Manager {
  cd_Profissional: string;
  nm_Profissional: string;
  codigoColaboradorInterno: string;
}

export interface UserProfile {
  colaborador: {
    nomeCompleto: string;
    urlFoto: string;
    cargo?: {
        cargo: string | null;
    }
  }
}

export type HealthPlanRole = 'general' | 'supervisor' | 'executive';
export type AgeRange = '0-18' | '19-23' | '24-28' | '29-33' | '34-38' | '39-43' | '44-48' | '49-53' | '54-58' | '59+';

export interface SimulationData {
  role: string;
  isTrustPosition: boolean;
  manager: string;
  client: string;
  costCenter: string;
  grossSalary: number;
  startDate: string;
  vacationModel: VacationModel;
  candidateName: string;
  personalEmail: string;
  cpf: string;
  rg: string;
  phone: string;
  birthDate: string;
  desiredNetSalary: number;
  desiredAjudaDeCusto: number;
  preferredWorkModel: 'Presencial' | 'Hibrido' | '100% Remoto';
  preferredOnsiteDays?: '1 dia presencial' | '2 dias presenciais' | '3 dias presenciais' | '4 dias presenciais' | '';
  dependentsIRPF: number;
  pcdAnswer: 'yes' | 'no' | 'prefer_not';
  pcdNotes: string;
  residentsCount: number;
  hasSpouse: boolean;
  spouseBirthDate: string;
  hasChildren: boolean;
  childrenCount: number;
  childrenBirthDates: string[];
  hasHealthInsurance: boolean;
  healthInsuranceCurrentValue: number;
  healthInsuranceProvider: string;
  healthInsuranceAccommodation: 'Apartamento' | 'Enfermaria' | '';
  healthInsuranceHasCopay: boolean;
  healthInsuranceNotes: string;
  wantsFoursysPlan: boolean;
  wantsFoursysPlanIncludeDependents: boolean;
  wantsFoursysPlanDependentsCount: number;
  studiesCurrently: boolean;
  educationMonthlyCost: number;
  childrenStudyUpTo24: boolean;
  childrenEducationMonthlyCost: number;
  zipCode: string;
  address: string;
  addressNumber: string;
  addressComplement: string;
  city: string;
  state: string;
  workZipCode: string;
  dailyKm: number;
  transportModel: 'Presencial' | 'Hibrido' | '100% Remoto';
  workOnsiteDays?: '1 dia presencial' | '2 dias presenciais' | '3 dias presenciais' | '4 dias presenciais' | '';
  modeloTrabalhoId?: string | null;
  healthPlanRole: HealthPlanRole;
  ageRange: AgeRange;
  healthPlanValue: number;
  healthPlanCopay: boolean;
  foodAllowance: number;
  educationCost: number;
  variableExpenses: VariableExpense[];
}

export interface BreakdownItem {
  label: string;
  value: number;
  type: 'earnings' | 'deduction' | 'benefit' | 'info';
}

export interface ForecastMonth {
  month: string;
  salaryPayment: number;
  benefitsPayment: number;
  variableExpensesPayment: number;
  total: number;
  note?: string;
  breakdown: BreakdownItem[];
}

export interface CalculationResult {
  inss: number;
  inssRate: number;
  irrf: number;
  irrfRate: number;
  totalDeductions: number;
  netSalary: number;
  companyCost: number;
  annualTotal: number;
  monthlyBenefits: number;
  mobilityCost: number;
  totalVariableExpenses: number;
  annualVariableExpenses: number;
  monthlyAverage: number;
  forecast: ForecastMonth[];
}

/** Estado inicial com valores padrão (ex.: ao carregar a página). */
export const INITIAL_STATE: SimulationData = {
  role: 'Desenvolvedor JAVA Pleno',
  isTrustPosition: false,
  manager: '',
  client: 'Foursys',
  costCenter: '',
  grossSalary: 11000,
  startDate: new Date().toISOString().split('T')[0],
  vacationModel: '30_dias',
  candidateName: '',
  personalEmail: '',
  cpf: '',
  rg: '',
  phone: '',
  birthDate: '',
  desiredNetSalary: 11000,
  desiredAjudaDeCusto: 0,
  preferredWorkModel: '100% Remoto',
  preferredOnsiteDays: '',
  dependentsIRPF: 1,
  pcdAnswer: 'prefer_not',
  pcdNotes: '',
  residentsCount: 1,
  hasSpouse: false,
  spouseBirthDate: '',
  hasChildren: false,
  childrenCount: 0,
  childrenBirthDates: [],
  hasHealthInsurance: false,
  healthInsuranceCurrentValue: 0,
  healthInsuranceProvider: '',
  healthInsuranceAccommodation: '',
  healthInsuranceHasCopay: false,
  healthInsuranceNotes: '',
  wantsFoursysPlan: false,
  wantsFoursysPlanIncludeDependents: false,
  wantsFoursysPlanDependentsCount: 0,
  studiesCurrently: false,
  educationMonthlyCost: 0,
  childrenStudyUpTo24: false,
  childrenEducationMonthlyCost: 0,
  zipCode: '',
  address: '',
  addressNumber: '',
  addressComplement: '',
  city: '',
  state: '',
  workZipCode: '',
  dailyKm: 0,
  transportModel: '100% Remoto',
  workOnsiteDays: '',
  modeloTrabalhoId: undefined,
  healthPlanRole: 'general',
  ageRange: '29-33',
  healthPlanValue: 520.28,
  healthPlanCopay: false,
  foodAllowance: 0,
  educationCost: 0,
  variableExpenses: [
    { id: '1', description: '', value: 0 }
  ]
};

/** Estado vazio para o botão Limpar: todos os campos zerados/não selecionados. */
export const EMPTY_FORM_STATE: SimulationData = {
  role: '',
  isTrustPosition: false,
  manager: '',
  client: '',
  costCenter: '',
  grossSalary: 0,
  startDate: '',
  vacationModel: '30_dias',
  candidateName: '',
  personalEmail: '',
  cpf: '',
  rg: '',
  phone: '',
  birthDate: '',
  desiredNetSalary: 0,
  desiredAjudaDeCusto: 0,
  preferredWorkModel: '100% Remoto',
  preferredOnsiteDays: '',
  dependentsIRPF: 0,
  pcdAnswer: 'prefer_not',
  pcdNotes: '',
  residentsCount: 0,
  hasSpouse: false,
  spouseBirthDate: '',
  hasChildren: false,
  childrenCount: 0,
  childrenBirthDates: [],
  hasHealthInsurance: false,
  healthInsuranceCurrentValue: 0,
  healthInsuranceProvider: '',
  healthInsuranceAccommodation: '',
  healthInsuranceHasCopay: false,
  healthInsuranceNotes: '',
  wantsFoursysPlan: false,
  wantsFoursysPlanIncludeDependents: false,
  wantsFoursysPlanDependentsCount: 0,
  studiesCurrently: false,
  educationMonthlyCost: 0,
  childrenStudyUpTo24: false,
  childrenEducationMonthlyCost: 0,
  zipCode: '',
  address: '',
  addressNumber: '',
  addressComplement: '',
  city: '',
  state: '',
  workZipCode: '',
  dailyKm: 0,
  transportModel: '100% Remoto',
  workOnsiteDays: '',
  modeloTrabalhoId: undefined,
  healthPlanRole: 'general',
  ageRange: '29-33',
  healthPlanValue: 0,
  healthPlanCopay: false,
  foodAllowance: 0,
  educationCost: 0,
  variableExpenses: [
    { id: '1', description: '', value: 0 }
  ]
};
