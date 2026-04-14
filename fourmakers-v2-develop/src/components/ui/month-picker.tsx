import * as React from "react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

interface MonthPickerProps {
  selectedMonth?: number; // 1-12
  selectedYear?: number;
  onMonthChange: (month: number, year: number) => void;
  onCancel?: () => void;
}

const meses = [
  { value: 1, label: "Jan", fullLabel: "Janeiro" },
  { value: 2, label: "Fev", fullLabel: "Fevereiro" },
  { value: 3, label: "Mar", fullLabel: "Março" },
  { value: 4, label: "Abr", fullLabel: "Abril" },
  { value: 5, label: "Mai", fullLabel: "Maio" },
  { value: 6, label: "Jun", fullLabel: "Junho" },
  { value: 7, label: "Jul", fullLabel: "Julho" },
  { value: 8, label: "Ago", fullLabel: "Agosto" },
  { value: 9, label: "Set", fullLabel: "Setembro" },
  { value: 10, label: "Out", fullLabel: "Outubro" },
  { value: 11, label: "Nov", fullLabel: "Novembro" },
  { value: 12, label: "Dez", fullLabel: "Dezembro" },
];

const gerarAnos = (): number[] => {
  const anos: number[] = [];
  const anoAtual = new Date().getFullYear();
  // Últimos 5 anos até 5 anos no futuro
  for (let i = anoAtual - 5; i <= anoAtual + 5; i++) {
    anos.push(i);
  }
  return anos;
};

export function MonthPicker({
  selectedMonth,
  selectedYear,
  onMonthChange,
  onCancel,
}: MonthPickerProps) {
  const [view, setView] = React.useState<"months" | "years">("months");
  const anos = React.useMemo(() => gerarAnos(), []);
  
  const currentMonth = selectedMonth || new Date().getMonth() + 1;
  const currentYear = selectedYear || new Date().getFullYear();
  const currentMonthLabel = meses.find(m => m.value === currentMonth)?.fullLabel || "";

  const handleMonthClick = (month: number) => {
    onMonthChange(month, currentYear);
  };

  const handleYearClick = (year: number) => {
    onMonthChange(currentMonth, year);
    setView("months");
  };

  const handleHeaderClick = () => {
    setView(view === "months" ? "years" : "months");
  };

  return (
    <div className="p-4 w-auto">
      {/* Header */}
      <div
        className="flex items-center justify-between mb-4 cursor-pointer"
        onClick={handleHeaderClick}
      >
        <div className="flex items-center gap-2">
          {view === "months" ? (
            <>
              <span className="text-sm font-medium">{currentMonthLabel}</span>
              <span className="text-sm font-medium">{currentYear}</span>
            </>
          ) : (
            <>
              <span className="text-sm font-medium">{currentMonthLabel}</span>
              <span className="text-sm font-medium">{currentYear}</span>
            </>
          )}
        </div>
      </div>

      {/* Grid de Meses */}
      {view === "months" && (
        <div className="grid grid-cols-3 gap-2 mb-4">
          {meses.map((mes) => {
            const isSelected = mes.value === currentMonth;
            return (
              <button
                key={mes.value}
                type="button"
                onClick={() => handleMonthClick(mes.value)}
                className={cn(
                  "h-9 px-3 text-sm font-normal rounded-md transition-colors",
                  "hover:bg-accent hover:text-accent-foreground",
                  isSelected
                    ? "bg-black text-white hover:bg-black hover:text-white dark:bg-white dark:text-black"
                    : "bg-transparent text-foreground"
                )}
              >
                {mes.label}
              </button>
            );
          })}
        </div>
      )}

      {/* Grid de Anos */}
      {view === "years" && (
        <div className="grid grid-cols-3 gap-2 mb-4">
          {anos.map((ano) => {
            const isSelected = ano === currentYear;
            return (
              <button
                key={ano}
                type="button"
                onClick={() => handleYearClick(ano)}
                className={cn(
                  "h-9 px-3 text-sm font-normal rounded-md transition-colors",
                  "hover:bg-accent hover:text-accent-foreground",
                  isSelected
                    ? "bg-black text-white hover:bg-black hover:text-white dark:bg-white dark:text-black"
                    : "bg-transparent text-foreground"
                )}
              >
                {ano}
              </button>
            );
          })}
        </div>
      )}

      {/* Botão Cancelar */}
      {onCancel && (
        <div className="flex justify-end mt-4 pt-4 border-t">
          <Button
            type="button"
            variant="ghost"
            onClick={onCancel}
            className="text-sm uppercase"
          >
            Cancelar
          </Button>
        </div>
      )}
    </div>
  );
}

