import React, { useState, useEffect, useRef } from 'react';
import { ChevronDown, ChevronUp, HelpCircle, ChevronRight, Search, Loader2 } from '@/components/ui/system-icons';
import type { ForecastMonth, VacationModel } from '@domain/entities/SimulatorTypes';
import { trackEvent } from '@shared/utils/analytics';
import { Card, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Tooltip as UITooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';

interface CardProps {
  title: string;
  description?: string;
  children: React.ReactNode;
  defaultOpen?: boolean;
}

export const AccordionCard: React.FC<CardProps> = ({ title, description, children, defaultOpen = true }) => {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  const handleToggle = () => {
    const newState = !isOpen;
    setIsOpen(newState);
    trackEvent(newState ? 'expand_section' : 'collapse_section', {
        section: title,
        label: title
    });
  };

  return (
    <Card className="mb-4">
      <button 
        onClick={handleToggle}
        className="w-full px-6 py-4 flex items-center justify-between hover:bg-secondary transition-colors text-left rounded-lg"
      >
        <div className="flex flex-col gap-1 pr-4">
          <span className="text-lg font-semibold text-foreground leading-tight">{title}</span>
          {description ? (
            <span className="text-xs text-muted-foreground leading-snug">{description}</span>
          ) : null}
        </div>
        {isOpen ? <ChevronUp className="w-5 h-5 text-muted-foreground" /> : <ChevronDown className="w-5 h-5 text-muted-foreground" />}
      </button>
      
      {isOpen && (
        <CardContent className="px-6 pb-6 pt-4 border-t border-border">
          {children}
        </CardContent>
      )}
    </Card>
  );
};

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  prefix?: string;
  error?: boolean;
}

export const InputField: React.FC<InputProps> = ({ label, prefix, className = "", error, onBlur, ...props }) => {
  
  const handleBlur = (e: React.FocusEvent<HTMLInputElement>) => {
    trackEvent('edit_field', {
        section: 'Formulário',
        field: label,
        value_length: e.target.value.length
    });
    
    if (onBlur) onBlur(e);
  };

  return (
    <div className="flex flex-col gap-2 w-full">
      <Label className={cn("text-sm font-medium text-foreground", error && "text-destructive")}>{label}</Label>
      <div className={`relative flex items-center ${className}`}>
        {prefix && (
          <span className="absolute left-3 text-muted-foreground text-sm font-medium z-10">{prefix}</span>
        )}
        <Input 
          className={prefix ? 'pl-10' : ''}
          error={error}
          onBlur={handleBlur}
          {...props}
        />
      </div>
    </div>
  );
};

interface AutocompleteProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  onSelect: (item: any) => void;
  options: any[];
  displayKey: string;
  placeholder?: string;
  loading?: boolean;
  disableLocalFilter?: boolean;
}

const normalizeText = (text: string) => {
  return text
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLowerCase();
};

export const AutocompleteField: React.FC<AutocompleteProps> = ({
  label,
  value,
  onChange,
  onSelect,
  options,
  displayKey,
  placeholder,
  loading,
  disableLocalFilter
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [filteredOptions, setFilteredOptions] = useState<any[]>([]);
  const wrapperRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (disableLocalFilter) {
      setFilteredOptions(options);
    } else if (!value) {
      setFilteredOptions(options.slice(0, 50)); 
    } else {
      const normalizedInput = normalizeText(value);
      const filtered = options.filter(opt => {
        const itemText = String(opt[displayKey]);
        return normalizeText(itemText).includes(normalizedInput);
      });
      setFilteredOptions(filtered.slice(0, 50)); 
    }
  }, [value, options, displayKey, disableLocalFilter]);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [wrapperRef]);

  const handleSelect = (item: any) => {
    trackEvent('select_option', {
        section: 'Formulário',
        field: label,
        selected_item: item[displayKey]
    });
    onChange(item[displayKey]);
    onSelect(item);
    setIsOpen(false);
  };

  return (
    <div className="flex flex-col gap-2 w-full relative" ref={wrapperRef}>
      <Label className="text-sm font-medium text-foreground">{label}</Label>
      <div className="relative">
        <Input
          value={value}
          onChange={(e) => {
            onChange(e.target.value);
            setIsOpen(true);
          }}
          onFocus={() => {
            setIsOpen(true);
            trackEvent('focus_field', { section: 'Formulário', field: label });
          }}
          placeholder={placeholder}
          className="pr-10"
        />
        <div className="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
          {loading ? (
             <Loader2 className="w-4 h-4 text-muted-foreground animate-spin" />
          ) : (
             <Search className="w-4 h-4 text-muted-foreground" />
          )}
        </div>
      </div>

      {isOpen && (
        <div className="absolute top-full left-0 right-0 mt-1 bg-popover border border-border rounded-lg shadow-lg max-h-60 overflow-y-auto z-50">
          {filteredOptions.length > 0 ? (
            <ul className="py-1">
              {filteredOptions.map((opt, idx) => (
                <li 
                  key={idx}
                  onClick={() => handleSelect(opt)}
                  className="px-3 py-2 hover:bg-accent hover:text-accent-foreground cursor-pointer text-sm text-foreground transition-colors"
                >
                  {opt[displayKey]}
                </li>
              ))}
            </ul>
          ) : (
            <div className="px-3 py-2 text-sm text-muted-foreground italic">
              {loading ? 'Carregando...' : 'Nenhum resultado encontrado'}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

interface CurrencyInputProps {
  label: string;
  value: number;
  onChange: (value: number) => void;
  className?: string;
  containerClassName?: string;
  inputClassName?: string;
  placeholder?: string;
  disabled?: boolean;
  readOnly?: boolean;
  /** Quando true, campo read-only mantém aparência de habilitado (sem bg-muted). */
  readOnlyLookEnabled?: boolean;
  error?: boolean;
  onFocus?: () => void;
  onClick?: () => void;
}

export const CurrencyInput: React.FC<CurrencyInputProps> = ({
  label,
  value,
  onChange,
  className = '',
  containerClassName,
  inputClassName,
  placeholder,
  disabled,
  readOnly,
  readOnlyLookEnabled,
  error,
  onFocus,
  onClick,
}) => {
  const formattedValue = new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (readOnly) return;
    const inputValue = e.target.value;
    const digits = inputValue.replace(/\D/g, '');
    const realValue = parseInt(digits || '0', 10) / 100;
    onChange(realValue);
  };

  const handleBlur = () => {
    trackEvent('edit_field', {
      section: 'Formulário',
      field: label,
      value: value,
    });
  };

  const wrapperClasses = containerClassName || className;

  return (
    <div className={`flex flex-col gap-2 w-full ${wrapperClasses || ''}`}>
      <Label className={cn('text-sm font-medium text-foreground', error && 'text-destructive')}>{label}</Label>
      <Input
        type="text"
        inputMode="numeric"
        value={formattedValue}
        onChange={handleChange}
        onBlur={handleBlur}
        onFocus={onFocus}
        onClick={onClick}
        placeholder={placeholder}
        disabled={disabled}
        readOnly={readOnly}
        error={error}
        className={cn(
          (disabled || (readOnly && !readOnlyLookEnabled)) && 'bg-muted text-muted-foreground cursor-default',
          readOnlyLookEnabled && 'cursor-default',
          inputClassName || '',
        )}
      />
    </div>
  );
};

interface SelectProps {
  label: string;
  options: { label: string; value: string | number }[];
  value?: string | number;
  onChange?: (e: React.ChangeEvent<HTMLSelectElement>) => void;
  disabled?: boolean;
  /** Quando true, select mantém aparência habilitada mas ao abrir exibe callback (ex.: toast) e não altera valor. */
  readOnly?: boolean;
  onReadOnlyClick?: () => void;
}

export const SelectField: React.FC<SelectProps> = ({ label, options, value, onChange, disabled, readOnly, onReadOnlyClick }) => {
  const [open, setOpen] = useState(false);
  const isReadOnly = readOnly && Boolean(onReadOnlyClick);

  const handleOpenChange = (next: boolean) => {
    if (isReadOnly && next) {
      onReadOnlyClick?.();
      return;
    }
    setOpen(next);
  };

  const handleValueChange = (selectedValue: string) => {
    if (isReadOnly) return;
    trackEvent('select_option', {
        section: 'Formulário',
        field: label,
        selected_value: selectedValue
    });
    if (onChange) {
      const event = { target: { value: selectedValue } } as React.ChangeEvent<HTMLSelectElement>;
      onChange(event);
    }
  };

  return (
    <div className="flex flex-col gap-2 w-full">
      <Label className="text-sm font-medium text-foreground">{label}</Label>
      <Select
        value={String(value || '')}
        onValueChange={handleValueChange}
        disabled={disabled && !isReadOnly}
        open={isReadOnly ? open : undefined}
        onOpenChange={isReadOnly ? handleOpenChange : undefined}
      >
        <SelectTrigger className={cn(isReadOnly && 'cursor-default opacity-100')}>
          <SelectValue />
        </SelectTrigger>
        <SelectContent>
          {options.map((opt, i) => (
            <SelectItem key={i} value={String(opt.value)}>
              {opt.label}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
};

export const Tooltip: React.FC<{ text: string }> = ({ text }) => {
  return (
    <TooltipProvider>
      <UITooltip>
        <TooltipTrigger asChild>
          <HelpCircle className="w-4 h-4 text-muted-foreground cursor-help" />
        </TooltipTrigger>
        <TooltipContent className="max-w-xs">
          <p className="text-xs">{text}</p>
        </TooltipContent>
      </UITooltip>
    </TooltipProvider>
  );
};

interface ForecastCardItemProps {
    month: ForecastMonth;
    index: number;
    formatCurrency: (v: number) => string;
    vacationModel?: VacationModel;
    onModelChange?: (model: VacationModel) => void;
    forceExpanded?: boolean;
}

export const ForecastCardItem: React.FC<ForecastCardItemProps> = ({ 
    month, 
    index, 
    formatCurrency, 
    vacationModel, 
    onModelChange,
    forceExpanded
}) => {
  const [isExpanded, setIsExpanded] = useState(false);
  
  useEffect(() => {
    if (forceExpanded !== undefined) {
        setIsExpanded(forceExpanded);
    }
  }, [forceExpanded]);

  const toggleExpand = () => {
    const newState = !isExpanded;
    setIsExpanded(newState);
    trackEvent(newState ? 'expand_forecast_details' : 'collapse_forecast_details', {
        section: 'Previsão de Recebimentos',
        month_index: index + 1,
        month_label: month.month
    });
  };

  const earnings = month.breakdown.filter(i => i.type === 'earnings');
  const deductions = month.breakdown.filter(i => i.type === 'deduction');
  const benefits = month.breakdown.filter(i => i.type === 'benefit');
  const isVacationMonth = index === 11; 

  const handleModelChange = (newModel: VacationModel) => {
    if (onModelChange) {
        trackEvent('select_vacation_model', {
            section: 'Previsão de Recebimentos',
            model: newModel,
            month_index: index + 1
        });
        onModelChange(newModel);
    }
  };

  return (
    <Card className={`transition-all ${isVacationMonth ? 'bg-yellow-50 dark:bg-yellow-950 border-yellow-200 dark:border-yellow-800' : ''}`}>
      <CardContent className="p-4">
        <div className="flex flex-col sm:flex-row items-center justify-between">
          <div className="flex items-center gap-2 mb-2 sm:mb-0 w-full sm:w-auto">
            <Button 
              variant="ghost" 
              size="icon" 
              className="h-8 w-8 hover:bg-secondary"
              onClick={toggleExpand}
            >
              {isExpanded ? <ChevronDown className="w-5 h-5" /> : <ChevronRight className="w-5 h-5" />}
            </Button>
            
            <div className="bg-secondary p-2 rounded-lg shadow-sm text-center min-w-[3rem] border border-border">
              <span className="block text-xs text-muted-foreground font-bold uppercase">{index + 1}º</span>
              <span className="block text-sm font-bold text-foreground">Mês</span>
            </div>

            <div className="flex flex-col">
              <h4 className="font-bold text-foreground capitalize">{month.month}</h4>
              
              {isVacationMonth && onModelChange ? (
                  <div className="mt-1 flex items-center gap-2">
                      <Select 
                        value={vacationModel} 
                        onValueChange={(value) => handleModelChange(value as VacationModel)}
                      >
                        <SelectTrigger className="h-8 text-xs">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="30_dias">30 Dias de Férias</SelectItem>
                          <SelectItem value="20_dias_abono">20 Dias + Abono</SelectItem>
                          <SelectItem value="15_dias">15 Dias (Dividido)</SelectItem>
                        </SelectContent>
                      </Select>
                  </div>
              ) : (
                  <>
                      {month.note && (
                      <Badge 
                        variant={month.note.includes('Retorno') ? 'destructive' : 'outline'}
                        className="mt-1 w-max"
                      >
                        {month.note}
                      </Badge>
                      )}
                      {!month.note && (
                      <span className="text-xs text-muted-foreground">Salário (25) + Benefícios (01)</span>
                      )}
                  </>
              )}
            </div>
          </div>
          
          <div 
              className="text-right w-full sm:w-auto pl-12 sm:pl-0 cursor-pointer"
              onClick={toggleExpand}
          >
            <div className="text-xl font-bold text-foreground">{formatCurrency(month.total)}</div>
            <div className="text-xs text-muted-foreground">
              Liq: {formatCurrency(month.salaryPayment)} | Ben: {formatCurrency(month.benefitsPayment)}
            </div>
          </div>
        </div>

        {isExpanded && (
          <div className="mt-4 pl-0 sm:pl-16 border-t border-border pt-4">
            <div className={`grid grid-cols-1 ${deductions.length > 0 ? 'md:grid-cols-3' : 'md:grid-cols-2'} gap-6 text-sm`}>
               <div>
                  <h5 className="font-bold text-green-600 dark:text-green-400 mb-2 border-b border-border pb-1">Proventos (Líquido)</h5>
                  <ul className="space-y-1">
                      {earnings.map((item, i) => (
                          <li key={i} className="flex justify-between text-muted-foreground">
                              <span>{item.label}</span>
                              <span className="font-medium text-green-600 dark:text-green-400">{formatCurrency(item.value)}</span>
                          </li>
                      ))}
                      {earnings.length === 0 && <li className="text-muted-foreground italic">--</li>}
                  </ul>
               </div>
               
               {deductions.length > 0 && (
               <div>
                  <h5 className="font-bold text-destructive mb-2 border-b border-border pb-1">Descontos (Variáveis)</h5>
                  <ul className="space-y-1">
                      {deductions.map((item, i) => (
                          <li key={i} className="flex justify-between text-muted-foreground">
                              <span>{item.label}</span>
                              <span className="font-medium text-destructive">- {formatCurrency(item.value)}</span>
                          </li>
                      ))}
                  </ul>
               </div>
               )}

               <div>
                  <h5 className="font-bold text-blue-600 dark:text-blue-400 mb-2 border-b border-border pb-1">Benefícios</h5>
                  <ul className="space-y-1">
                      {benefits.map((item, i) => (
                          <li key={i} className="flex justify-between text-muted-foreground">
                              <span>{item.label}</span>
                              <span className="font-medium text-blue-600 dark:text-blue-400">{formatCurrency(item.value)}</span>
                          </li>
                      ))}
                  </ul>
               </div>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
