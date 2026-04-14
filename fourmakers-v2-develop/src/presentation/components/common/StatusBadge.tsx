import { Badge } from "@/components/ui/badge";

interface StatusBadgeProps {
  status: string;
  variant?: "reembolso" | "projeto" | "timesheet";
  /** Quando true, evita quebra de linha e trunca com reticências (textos longos em tabelas). */
  truncate?: boolean;
}

export const StatusBadge = ({ status, variant = "reembolso", truncate }: StatusBadgeProps) => {
  const getStatusColor = (status: string, variant: string) => {
    if (variant === "projeto") {
      switch (status) {
        case "Proposta":
          return "bg-blue-100 text-blue-700 hover:bg-blue-100 dark:bg-blue-900 dark:text-blue-300";
        case "Cancelado":
          return "bg-gray-100 text-gray-700 hover:bg-gray-100 dark:bg-gray-900 dark:text-gray-300";
        case "Desenvolvimento":
          return "bg-green-100 text-green-700 hover:bg-green-100 dark:bg-green-900 dark:text-green-300";
        case "Em processamento":
          return "bg-amber-100 text-amber-800 hover:bg-amber-100 dark:bg-amber-900/30 dark:text-amber-300 border border-amber-300/50";
        default:
          return "bg-gray-100 text-gray-700 hover:bg-gray-100 dark:bg-gray-900 dark:text-gray-300";
      }
    }

    if (variant === "timesheet") {
      switch (status) {
        case "Aprovado":
          return "bg-green-600 text-white hover:bg-green-600";
        case "Pendente":
        case "Aguardando Retorno":
          return "bg-blue-600 text-white hover:bg-blue-600";
        case "Rejeitado":
        case "Reprovado":
          return "bg-red-600 text-white hover:bg-red-600";
        default:
          return "bg-gray-100 text-gray-800 hover:bg-gray-100 dark:bg-gray-900 dark:text-gray-300";
      }
    }

    // Variant reembolso
    switch (status) {
      case "Aprovado":
        return "bg-green-100 text-green-800 hover:bg-green-100 dark:bg-green-900 dark:text-green-300";
      case "Aprovado Parcial":
        return "bg-yellow-100 text-yellow-800 hover:bg-yellow-100 dark:bg-yellow-900 dark:text-yellow-300";
      case "Pendente":
      case "Aguardando Retorno":
        return "bg-blue-100 text-blue-800 hover:bg-blue-100 dark:bg-blue-900 dark:text-blue-300";
      case "Rejeitado":
      case "Reprovado":
        return "bg-red-100 text-red-800 hover:bg-red-100 dark:bg-red-900 dark:text-red-300";
      case "Processada":
        return "bg-green-100 text-green-800 hover:bg-green-100 dark:bg-green-900 dark:text-green-300";
      case "Erro":
        return "bg-red-100 text-red-800 hover:bg-red-100 dark:bg-red-900 dark:text-red-300";
      case "Pago":
        return "bg-green-100 text-green-800 hover:bg-green-100 dark:bg-green-900 dark:text-green-300";
      default:
        return "bg-gray-100 text-gray-800 hover:bg-gray-100 dark:bg-gray-900 dark:text-gray-300";
    }
  };

  const isProcessing = status === "Em processamento";

  return (
    <Badge
      className={`relative overflow-hidden ${getStatusColor(status, variant)} ${isProcessing ? "py-1.5 pr-2" : ""} ${truncate ? "max-w-full min-w-0 inline-flex" : ""}`}
      title={truncate ? status : undefined}
    >
      <span className={`relative z-10 ${truncate ? "truncate block min-w-0" : ""}`}>{status}</span>
      {isProcessing && (
        <span
          className="absolute bottom-0 left-0 right-0 z-0 h-1.5 rounded-b-[inherit] bg-amber-200/30 dark:bg-amber-600/15 overflow-hidden"
          aria-hidden
        >
          <span
            className="absolute inset-y-0 left-0 h-full bg-amber-300/40 dark:bg-amber-500/25 rounded-r-full origin-left"
            style={{
              animation: "status-badge-fill 1.2s ease-in-out infinite",
            }}
          />
        </span>
      )}
    </Badge>
  );
};
