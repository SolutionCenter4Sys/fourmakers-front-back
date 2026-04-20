
import type { SimulationData, CalculationResult, ForecastMonth, BreakdownItem, VacationModel, HealthPlanRole, AgeRange } from '../../domain/entities/SimulatorTypes';

export const HOURS_PER_MONTH = 168;
export const MEAL_VOUCHER_BASE = 660;
export const MOBILITY_RATE_PER_KM = 1.5;
export const WORKING_DAYS_PER_MONTH = 22;
export const CEP_LOOKUP_DELAY_MS = 350;

const INSS_RANGES = [
  { limit: 1518.00, rate: 0.075 },
  { limit: 2793.88, rate: 0.09 },
  { limit: 4190.83, rate: 0.12 },
  { limit: 8157.41, rate: 0.14 },
];

const IRRF_RANGES = [
  { limit: 2259.20, rate: 0, deduction: 0 },
  { limit: 2826.65, rate: 0.075, deduction: 169.44 },
  { limit: 3751.05, rate: 0.15, deduction: 381.44 },
  { limit: 4664.68, rate: 0.225, deduction: 662.77 },
  { limit: Infinity, rate: 0.275, deduction: 896.00 },
];

const DEDUCTION_PER_DEPENDENT = 189.59;

export const AGE_RANGES: { label: string; value: AgeRange }[] = [
    { label: 'I (0 a 18 anos)', value: '0-18' },
    { label: 'II (19 a 23 anos)', value: '19-23' },
    { label: 'III (24 a 28 anos)', value: '24-28' },
    { label: 'IV (29 a 33 anos)', value: '29-33' },
    { label: 'V (34 a 38 anos)', value: '34-38' },
    { label: 'VI (39 a 43 anos)', value: '39-43' },
    { label: 'VII (44 a 48 anos)', value: '44-48' },
    { label: 'VIII (49 a 53 anos)', value: '49-53' },
    { label: 'IX (54 a 58 anos)', value: '54-58' },
    { label: 'X (59 anos ou mais)', value: '59+' },
];

export const HEALTH_PLANS = {
    general: { label: 'Profissionais Gerais (Direto Nacional Apto)', price: 520.28 },
    supervisor: { label: 'Supervisores/Coord. (Especial 100 R1)', price: 1156.10 },
    executive: { label: 'Executivos/Diretores (Executivo R1)', price: 2535.74 },
};

export const getHealthPlanPrice = (role: HealthPlanRole, _age: AgeRange): number => {
    return HEALTH_PLANS[role]?.price || 0;
};

export const calculateAge = (birthDate: string): number => {
    if (!birthDate) return 0;
    const [year, month, day] = birthDate.split('-').map(Number);
    if (!year || !month || !day) return 0;

    const today = new Date();
    let age = today.getFullYear() - year;
    const m = (today.getMonth() + 1) - month;
    
    if (m < 0 || (m === 0 && today.getDate() < day)) {
        age--;
    }
    return age;
};

export const getAgeRange = (age: number): AgeRange => {
    if (age <= 18) return '0-18';
    if (age <= 23) return '19-23';
    if (age <= 28) return '24-28';
    if (age <= 33) return '29-33';
    if (age <= 38) return '34-38';
    if (age <= 43) return '39-43';
    if (age <= 48) return '44-48';
    if (age <= 53) return '49-53';
    if (age <= 58) return '54-58';
    return '59+';
};

export const calculateTaxes = (gross: number, dependents: number) => {
  let inss = 0;
  let previousLimit = 0;
  for (const range of INSS_RANGES) {
    if (gross > previousLimit) {
      const taxableInRange = Math.min(gross, range.limit) - previousLimit;
      inss += taxableInRange * range.rate;
      previousLimit = range.limit;
    } else {
      break;
    }
  }

  const dependentsDeduction = dependents * DEDUCTION_PER_DEPENDENT;
  const irrfBase = gross - inss - dependentsDeduction;
  let irrf = 0;
  if (irrfBase > 0) {
    const range = IRRF_RANGES.find(r => irrfBase <= r.limit) || IRRF_RANGES[IRRF_RANGES.length - 1];
    irrf = (irrfBase * range.rate) - range.deduction;
  }
  if (irrf < 0) irrf = 0;

  return { inss, irrf, net: gross - inss - irrf };
};

export const calculateReverseSalary = (targetNetBeforeVar: number, dependents: number): number => {
    let low = targetNetBeforeVar;
    let high = Math.max(targetNetBeforeVar * 2, 1000000); 
    
    let safety = 0;
    while (calculateTaxes(high, dependents).net < targetNetBeforeVar && safety < 20) {
        high *= 2;
        safety++;
    }

    let iterations = 0;
    while (high - low > 0.01 && iterations < 100) {
        const mid = (low + high) / 2;
        const { net } = calculateTaxes(mid, dependents);
        if (net < targetNetBeforeVar) {
            low = mid;
        } else {
            high = mid;
        }
        iterations++;
    }
    return high;
};

const getMonthName = (date: Date) => {
  return new Intl.DateTimeFormat('pt-BR', { month: 'long', year: 'numeric' }).format(date);
};

const calculateVacationValues = (
    grossSalary: number, 
    dependents: number, 
    model: VacationModel
) => {
    let vacationGross = 0;
    let vacationBonus = 0;
    let abonoPecuniario = 0;
    let abonoBonus = 0;
    
    if (model === '30_dias') {
        vacationGross = grossSalary;
        vacationBonus = grossSalary / 3;
    } else if (model === '20_dias_abono') {
        vacationGross = (grossSalary / 30) * 20;
        vacationBonus = vacationGross / 3;
        abonoPecuniario = (grossSalary / 30) * 10;
        abonoBonus = abonoPecuniario / 3;
    } else if (model === '15_dias') {
        vacationGross = (grossSalary / 30) * 15;
        vacationBonus = vacationGross / 3;
    }

    const totalVacationTaxable = vacationGross + vacationBonus;
    const taxes = calculateTaxes(totalVacationTaxable, dependents);
    
    const totalNet = taxes.net + abonoPecuniario + abonoBonus;

    return {
        vacationGross,
        vacationBonus,
        abonoPecuniario,
        abonoBonus,
        inss: taxes.inss,
        irrf: taxes.irrf,
        totalNet
    };
};

export const calculateSalary = (data: SimulationData): CalculationResult => {
  const salary = data.grossSalary || 0;
  const { inss, irrf, net: netBeforeVar } = calculateTaxes(salary, data.dependentsIRPF || 0);

  const totalVariableExpenses = data.variableExpenses?.reduce((acc, curr) => acc + (curr.value || 0), 0) || 0;
  const annualVariableExpenses = totalVariableExpenses * 12;

  const foursysPlanCandidateValue = data.wantsFoursysPlan ? (data.healthPlanValue || 0) : 0;
  const foursysPlanDependentsCount = data.wantsFoursysPlan && data.wantsFoursysPlanIncludeDependents
    ? Math.max(1, data.wantsFoursysPlanDependentsCount || 1)
    : 0;
  const foursysPlanDependentsValue = foursysPlanDependentsCount > 0
    ? foursysPlanCandidateValue * foursysPlanDependentsCount
    : 0;

  const totalDeductions = inss + irrf + totalVariableExpenses + foursysPlanDependentsValue;
  const netSalary = netBeforeVar - totalVariableExpenses - foursysPlanDependentsValue;

  const fgts = salary * 0.08;
  const vacationProvision = salary / 12;
  const vacationBonusProvision = vacationProvision / 3;
  const thirteenthProvision = salary / 12;
  
  const mobilityCost = (data.dailyKm || 0) * MOBILITY_RATE_PER_KM * WORKING_DAYS_PER_MONTH;

  const mealVoucher = MEAL_VOUCHER_BASE;
  const foodAllowance = data.foodAllowance || 0;
  const benefitsCost = foursysPlanCandidateValue + mealVoucher + foodAllowance + (data.educationCost || 0) + mobilityCost;
  
  const employerINSS = salary * 0.20; 
  const systemS = salary * 0.058; 
  
  const companyCost = salary + fgts + vacationProvision + vacationBonusProvision + thirteenthProvision + benefitsCost + employerINSS + systemS;

  const annualTotal = (netSalary * 13.33) + (benefitsCost * 12);
  const monthlyAverage = annualTotal / 12;

  const forecast: ForecastMonth[] = [];
  if (data.startDate) {
    // Parse YYYY-MM-DD como partes locais para evitar que dia 01 vire mês anterior (UTC → fuso local)
    const parts = data.startDate.trim().split('-');
    const startYear = Number.isFinite(parseInt(parts[0], 10)) ? parseInt(parts[0], 10) : new Date().getFullYear();
    const startMonth1Based = Math.max(1, Math.min(12, parseInt(parts[1], 10) || 1));
    const startDay = Math.max(1, Math.min(31, parseInt(parts[2], 10) || 1));
    const startMonth0Based = startMonth1Based - 1;
    let currentMonth = new Date(startYear, startMonth0Based, 1);

    for (let i = 0; i < 13; i++) {
      let monthlyCashNet = 0;
      let monthlyVariable = totalVariableExpenses;
      let note = undefined;
      let breakdown: BreakdownItem[] = [];
      const monthlyPlanDependents = foursysPlanDependentsValue;

      const isFirstMonth = i === 0;
      const isVacationMonth = i === 11;
      const isReturnMonth = i === 12;

      if (isFirstMonth) {
         const daysInMonth = 30;
         const daysWorked = Math.max(0, daysInMonth - startDay + 1);
         
         if (daysWorked < 30) {
             const grossProRata = (salary / 30) * daysWorked;
             const taxesProRata = calculateTaxes(grossProRata, data.dependentsIRPF || 0);
             monthlyCashNet = taxesProRata.net - monthlyVariable - monthlyPlanDependents;
             note = `Proporcional (${daysWorked} dias)`;
             
             breakdown.push({ label: `Salário Líquido (${daysWorked} dias)`, value: taxesProRata.net, type: 'earnings' });
         } else {
             monthlyCashNet = netBeforeVar - monthlyVariable - monthlyPlanDependents;
             breakdown.push({ label: 'Salário Líquido', value: netBeforeVar, type: 'earnings' });
         }
      } 
      else if (isVacationMonth) {
          note = 'Mês de Férias';
          const daysWorked = Math.max(0, startDay - 1);
          let netSalaryProRata = 0;
          if (daysWorked > 0) {
             const grossProRata = (salary / 30) * daysWorked;
             const taxesProRata = calculateTaxes(grossProRata, data.dependentsIRPF || 0);
             netSalaryProRata = taxesProRata.net;
             breakdown.push({ label: `Salário Líquido (${daysWorked} dias)`, value: netSalaryProRata, type: 'earnings' });
          }

          const vacValues = calculateVacationValues(salary, data.dependentsIRPF || 0, data.vacationModel);
          breakdown.push({ label: 'Férias (Líquido + 1/3)', value: vacValues.totalNet, type: 'earnings' });

          monthlyCashNet = netSalaryProRata + vacValues.totalNet - monthlyVariable;
          monthlyCashNet -= monthlyPlanDependents;
      } 
      else if (isReturnMonth) {
          note = 'Mês de Retorno';
          let vacationDuration = 30;
          if (data.vacationModel === '20_dias_abono') vacationDuration = 20;
          if (data.vacationModel === '15_dias') vacationDuration = 15;

          const daysInM12 = 30;
          const vacationDaysInM12 = Math.min(vacationDuration, daysInM12 - startDay + 1);
          const vacationDaysInM13 = Math.max(0, vacationDuration - vacationDaysInM12);
          const daysWorkedM13 = 30 - vacationDaysInM13;

          let netSalaryProRata = 0;
          if (daysWorkedM13 > 0) {
              const grossReturn = (salary / 30) * daysWorkedM13;
              const taxesReturn = calculateTaxes(grossReturn, data.dependentsIRPF || 0);
              netSalaryProRata = taxesReturn.net;
              breakdown.push({ label: `Salário Líquido (${daysWorkedM13} dias)`, value: netSalaryProRata, type: 'earnings' });
              
              if (vacationDaysInM13 > 0) {
                  note = `Retorno (Férias até dia ${vacationDaysInM13})`;
              }
          } else {
              note = 'Férias (Mês Completo)';
          }
          monthlyCashNet = netSalaryProRata - monthlyVariable - monthlyPlanDependents;
      } 
      else {
          monthlyCashNet = netBeforeVar - monthlyVariable - monthlyPlanDependents;
          breakdown.push({ label: 'Salário Líquido', value: netBeforeVar, type: 'earnings' });
      }

      if (monthlyVariable > 0) {
          breakdown.push({ label: 'Gastos Variáveis', value: monthlyVariable, type: 'deduction' });
      }
      if (monthlyPlanDependents > 0) {
          breakdown.push({ label: 'Plano Saúde (Dependentes)', value: monthlyPlanDependents, type: 'deduction' });
      }
      
      breakdown.push(
          { label: 'Refeição', value: mealVoucher, type: 'benefit' },
          { label: 'Alimentação', value: foodAllowance, type: 'benefit' },
          { label: 'Saúde', value: foursysPlanCandidateValue, type: 'benefit' },
          { label: 'Mobilidade', value: mobilityCost, type: 'benefit' }
      );
      if(data.educationCost > 0) {
           breakdown.push({ label: 'Educação', value: data.educationCost, type: 'benefit' });
      }

      forecast.push({
        month: getMonthName(currentMonth),
        salaryPayment: monthlyCashNet,
        benefitsPayment: benefitsCost,
        variableExpensesPayment: monthlyVariable,
        total: monthlyCashNet + benefitsCost,
        note,
        breakdown
      });

      currentMonth.setMonth(currentMonth.getMonth() + 1);
    }
  }

  return {
    inss,
    inssRate: inss / salary,
    irrf,
    irrfRate: irrf / salary,
    totalDeductions,
    totalVariableExpenses,
    netSalary,
    companyCost,
    annualTotal,
    monthlyBenefits: benefitsCost,
    mobilityCost,
    annualVariableExpenses,
    monthlyAverage,
    forecast
  };
};

export const normalizeCep = (cep: string) => cep.replace(/\D/g, '').slice(0, 8);

/** CEP para exibição: sempre 8 dígitos no formato XXXXX-XXX (preenche com 0 à direita se faltar). */
export const formatCep = (value: string) => {
  const digits = normalizeCep(value);
  const padded = digits.padEnd(8, '0').slice(0, 8);
  if (padded.length <= 5) return padded;
  return `${padded.slice(0, 5)}-${padded.slice(5)}`;
};

/** CEP para uso no input: só dígitos (máx. 8) com hífen após o 5º; sem padding, para digitação e delete corretos. */
export const formatCepForInput = (value: string) => {
  const digits = normalizeCep(value);
  if (digits.length <= 5) return digits;
  return `${digits.slice(0, 5)}-${digits.slice(5)}`;
};

/** CEP para envio à API: apenas 8 dígitos, sem hífen. Não preenche com zeros (CEP incompleto não é válido). */
export const cepToApiFormat = (cep: string) => normalizeCep(cep).slice(0, 8);

/** Retorna true somente quando o CEP tem exatamente 8 dígitos (formato #####-###). */
export const isCepComplete = (cep: string) => normalizeCep(cep).length === 8;

export interface CepLookupData {
  logradouro?: string;
  localidade?: string;
  uf?: string;
  complemento?: string;
}

export interface CepAutofillSnapshot {
  cep: string;
  address: string;
  city: string;
  state: string;
  addressComplement: string;
}

export const fetchCepData = async (cep: string, signal?: AbortSignal): Promise<CepLookupData> => {
  const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`, { signal });
  if (!response.ok) throw new Error('cep_lookup_failed');

  const data: CepLookupData & { erro?: boolean } = await response.json();
  if (data.erro) throw new Error('cep_lookup_failed');

  return {
    logradouro: (data.logradouro || '').trim(),
    localidade: (data.localidade || '').trim(),
    uf: (data.uf || '').trim(),
    complemento: (data.complemento || '').trim(),
  };
};

export const resolveCepAutofill = ({
  formData,
  cep,
  data,
  lastFill,
}: {
  formData: SimulationData;
  cep: string;
  data: CepLookupData;
  lastFill: CepAutofillSnapshot | null;
}) => {
  const shouldOverwriteAddress = !formData.address || (lastFill?.cep === cep && formData.address === lastFill.address);
  const shouldOverwriteCity = !formData.city || (lastFill?.cep === cep && formData.city === lastFill.city);
  const shouldOverwriteState = !formData.state || (lastFill?.cep === cep && formData.state === lastFill.state);
  const shouldOverwriteComplement =
    !formData.addressComplement || (lastFill?.cep === cep && formData.addressComplement === lastFill.addressComplement);

  const nextAddress = data.logradouro || '';
  const nextCity = data.localidade || '';
  const nextState = data.uf || '';
  const nextComplement = data.complemento || '';

  const formUpdate: Partial<SimulationData> = {
    address: shouldOverwriteAddress ? nextAddress : formData.address,
    city: shouldOverwriteCity ? nextCity : formData.city,
    state: shouldOverwriteState ? nextState : formData.state,
    addressComplement: shouldOverwriteComplement ? nextComplement : formData.addressComplement,
  };

  const snapshot: CepAutofillSnapshot = {
    cep,
    address: formUpdate.address || '',
    city: formUpdate.city || '',
    state: formUpdate.state || '',
    addressComplement: formUpdate.addressComplement || '',
  };

  return { formUpdate, snapshot };
};

export const isAbortError = (error: unknown) => (error as { name?: string }).name === 'AbortError';

export const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value);
};

export const formatPercent = (value: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'percent',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value);
};
